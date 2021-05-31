using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public abstract class BTCore
    {
        protected bool DoHashChecking;
        private DirCache? FileCache;
        private string? FileCacheIsFor;
        private bool FileCacheWithSubFolders;
        private readonly Dictionary<string, List<HashCacheItem>> HashCache;
        private readonly SetProgressDelegate SetProg;

        protected BTCore(SetProgressDelegate setprog)
        {
            SetProg = setprog;

            HashCache = new Dictionary<string, List<HashCacheItem>>();
            FileCache = null;
            FileCacheIsFor = null;
            FileCacheWithSubFolders = false;
        }

        protected void Prog(int percent, string message) => SetProg(percent, message);

        protected abstract bool NewTorrentEntry(string torrentFile, int numberInTorrent);

        protected abstract bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent,
            string nameInTorrent);

        protected abstract bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent,
            string nameInTorrent);

        protected abstract bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename);

        private FileInfo? FindLocalFileWithHashAt(byte[] findMe, long whereInFile, long pieceSize, long fileSize)
        {
            if (whereInFile < 0 || FileCache is null)
            {
                return null;
            }

            foreach (DirCacheEntry dc in FileCache)
            {
                FileInfo fiTemp = dc.TheFile;
                long flen = dc.Length;

                if ((flen != fileSize) || (flen < (whereInFile + pieceSize))) // this file is wrong size || too small
                {
                    continue;
                }

                byte[] theHash = CheckCache(fiTemp.FullName, whereInFile, pieceSize, fileSize);
                if (theHash is null)
                {
                    // not cached, figure it out ourselves
                    System.IO.FileStream sr;
                    try
                    {
                        sr = new System.IO.FileStream(fiTemp.FullName, System.IO.FileMode.Open);
                    }
                    catch
                    {
                        return null;
                    }

                    byte[] thePiece = new byte[pieceSize];
                    sr.Seek(whereInFile, System.IO.SeekOrigin.Begin);
                    int n = sr.Read(thePiece, 0, (int)pieceSize);
                    sr.Close();

                    System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();

                    theHash = sha1.ComputeHash(thePiece, 0, n);
                    CacheThis(fiTemp.FullName, whereInFile, pieceSize, fileSize, theHash);
                }

                bool allGood = true;
                for (int j = 0; j < 20; j++)
                {
                    if (theHash[j] != findMe[j])
                    {
                        allGood = false;
                        break;
                    }
                }

                if (allGood)
                {
                    return fiTemp;
                }
            } // while enum

            return null;
        }

        private void CacheThis(string filename, long whereInFile, long piecesize, long fileSize, byte[] hash)
        {
            if (!HashCache.ContainsKey(filename))
            {
                HashCache[filename] = new List<HashCacheItem>();
            }

            HashCache[filename].Add(new HashCacheItem(whereInFile, piecesize, fileSize, hash));
        }

        private byte[]? CheckCache(string filename, long whereInFile, long piecesize, long fileSize)
        {
            if (HashCache.ContainsKey(filename))
            {
                foreach (HashCacheItem h in HashCache[filename])
                {
                    if ((h.WhereInFile == whereInFile) && (h.PieceSize == piecesize) && (h.FileSize == fileSize))
                    {
                        return h.TheHash;
                    }
                }
            }

            return null;
        }

        protected void BuildFileCache(string folder, bool subFolders)
        {
            if ((FileCache is null) || (FileCacheIsFor is null) || (FileCacheIsFor != folder) ||
                (FileCacheWithSubFolders != subFolders))
            {
                FileCache = new DirCache(null, folder, subFolders);
                FileCacheIsFor = folder;
                FileCacheWithSubFolders = subFolders;
            }
        }

        protected bool ProcessTorrentFile(string torrentFile, TreeView? tvTree, CommandLineArgs args)
        {
            // ----------------------------------------
            // read in torrent file

            tvTree?.Nodes.Clear();

            BEncodeLoader bel = new BEncodeLoader();
            BTFile? btFile = bel.Load(torrentFile);

            BTItem? bti = btFile?.GetItem("info");
            if (bti is null || (bti.Type != BTChunk.kDictionary))
            {
                return false;
            }

            BTDictionary infoDict = (BTDictionary)(bti);

            bti = infoDict.GetItem("piece length");
            if ((bti is null) || (bti.Type != BTChunk.kInteger))
            {
                return false;
            }

            long pieceSize = ((BTInteger)bti).Value;

            bti = infoDict.GetItem("pieces");
            if ((bti is null) || (bti.Type != BTChunk.kString))
            {
                return false;
            }

            BTString torrentPieces = (BTString)(bti);

            bti = infoDict.GetItem("files");

            if (bti is null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti is null) || (bti.Type != BTChunk.kString))
                {
                    return false;
                }

                BTString di = (BTString)(bti);
                string nameInTorrent = di.AsString();

                BTItem? fileSizeI = infoDict.GetItem("length");
                if (!(fileSizeI is BTInteger btInteger))
                {
                    return false;
                }
                long fileSize = btInteger.Value;

                NewTorrentEntry(torrentFile, -1);
                if (DoHashChecking)
                {
                    byte[] torrentPieceHash = torrentPieces.StringTwentyBytePiece(0);
                    if (torrentPieceHash is null)
                    {
                        return false;
                    }
                    FileInfo fi = FindLocalFileWithHashAt(torrentPieceHash, 0, pieceSize, fileSize);
                    if (fi != null)
                    {
                        FoundFileOnDiskForFileInTorrent(torrentFile, fi, -1, nameInTorrent);
                    }
                    else
                    {
                        DidNotFindFileOnDiskForFileInTorrent(torrentFile, -1, nameInTorrent);
                    }
                }

                FinishedTorrentEntry(torrentFile, -1, nameInTorrent);

                // don't worry about updating overallPosition as this is the only file in the torrent
            }
            else
            {
                long overallPosition = 0;
                long lastPieceLeftover = 0;

                if (bti.Type != BTChunk.kList)
                {
                    return false;
                }

                BTList fileList = (BTList)(bti);

                // list of dictionaries
                for (int i = 0; i < fileList.Items.Count; i++)
                {
                    Prog(100 * i / fileList.Items.Count, i.ToString());
                    if (fileList.Items[i].Type != BTChunk.kDictionary)
                    {
                        return false;
                    }

                    BTDictionary file = (BTDictionary)(fileList.Items[i]);
                    BTItem thePath = file.GetItem("path");
                    if (thePath is null || thePath.Type != BTChunk.kList)
                    {
                        return false;
                    }

                    BTList pathList = (BTList)(thePath);
                    // want the last of the items in the list, which is the filename itself
                    int n = pathList.Items.Count - 1;
                    if (n < 0)
                    {
                        return false;
                    }

                    BTString fileName = (BTString)(pathList.Items[n]);

                    BTItem? fileSizeI = file.GetItem("length");
                    if (!(fileSizeI is BTInteger btInteger))
                    {
                        return false;
                    }
                    long fileSize = btInteger.Value;

                    int pieceNum = (int)(overallPosition / pieceSize);
                    if (overallPosition % pieceSize != 0)
                    {
                        pieceNum++;
                    }

                    NewTorrentEntry(torrentFile, i);

                    if (DoHashChecking)
                    {
                        byte[]? torrentPieceHash = torrentPieces.StringTwentyBytePiece(pieceNum);
                        if (torrentPieceHash is null)
                        {
                            return false;
                        }
                        FileInfo fi = FindLocalFileWithHashAt(torrentPieceHash, lastPieceLeftover, pieceSize, fileSize);
                        if (fi != null)
                        {
                            FoundFileOnDiskForFileInTorrent(torrentFile, fi, i, fileName.AsString());
                        }
                        else
                        {
                            DidNotFindFileOnDiskForFileInTorrent(torrentFile, i, fileName.AsString());
                        }
                    }

                    FinishedTorrentEntry(torrentFile, i, fileName.AsString());

                    int sizeInPieces = (int)(fileSize / pieceSize);
                    if (fileSize % pieceSize != 0)
                    {
                        sizeInPieces++; // another partial piece
                    }

                    lastPieceLeftover = (lastPieceLeftover + (int)((sizeInPieces * pieceSize) - fileSize)) % pieceSize;
                    overallPosition += fileSize;
                } // for each file in the torrent
            }

            if (tvTree != null)
            {
                tvTree.BeginUpdate();
                btFile.Tree(tvTree.Nodes);
                tvTree.ExpandAll();
                tvTree.EndUpdate();
                tvTree.Update();
            }

            Prog(0, string.Empty);

            return true;
        }
    }
}
