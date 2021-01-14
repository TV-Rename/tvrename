// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

// ReSharper disable All

// Opens, understands, manipulates, and writes out BEncoded .torrent files, and uTorrent's resume.dat

namespace TVRename
{
    using Alphaleonis.Win32.Filesystem;
    
    public enum BTChunk
    {
        kError,
        kDictionary,
        kDictionaryItem,
        kList,
        kListOrDictionaryEnd,
        kInteger,
        kString,
        kBTEOF
    }

    public class FutureTorrentEntry : TorrentEntry
    {
        public FutureTorrentEntry([NotNull] string torrentfile, [NotNull] string to) : base(torrentfile, to, 0, false, string.Empty)
        {
        }
    }

    public class TorrentEntry: IDownloadInformation // represents a torrent downloading in a doewloader(Torrent)
    {
        public readonly string DownloadingTo;
        public readonly int PercentDone;
        public readonly string TorrentFile;
        public readonly bool Finished;
        public readonly string key;

        public TorrentEntry(string torrentfile, string to, int percent, bool finished, string key)
        {
            TorrentFile = torrentfile;
            DownloadingTo = to;
            PercentDone = percent;
            Finished = finished;
            this.key = key;
        }

        string IDownloadInformation.FileIdentifier => TorrentFile;

        string IDownloadInformation.Destination => DownloadingTo;

        string IDownloadInformation.RemainingText  
        {
            get
            {
                int p = PercentDone;
                return p == -1 ? "" : PercentDone + "% Complete";
            }
        }
    }

    public abstract class BTItem
    {
        public readonly BTChunk Type; // from enum

        protected BTItem(BTChunk type)
        {
            Type = type;
        }

        public virtual string AsText()
        {
            return $"Type ={Type}";
        }

        public virtual void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTItem:" + Type);
            tn.Add(n);
        }

        public abstract void Write(System.IO.Stream sw);
    }

    public class BTString : BTItem
    {
        public byte[] Data;

        public BTString(string s)
            : base(BTChunk.kString)
        {
            SetString(s);
        }

        public BTString()
            : base(BTChunk.kString)
        {
            Data = new byte[0];
        }

        public void SetString(string s)
        {
            Data = System.Text.Encoding.UTF8.GetBytes(s);
        }

        public override string AsText()
        {
            return "String=" + AsString();
        }

        public string AsString()
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            return enc.GetString(Data);
        }

        public byte[] StringTwentyBytePiece(int pieceNum)
        {
            byte[] res = new byte[20];
            if (((pieceNum * 20) + 20) > Data.Length)
                return null;

            Array.Copy(Data, pieceNum * 20, res, 0, 20);
            return res;
        }

        public static string CharsToHex(byte[] data, int start, int n)
        {
            string r = string.Empty;
            for (int i = 0; i < n; i++)
                r += (data[start + i] < 16 ? "0" : "") + data[start + i].ToString("x").ToUpper();

            return r;
        }

        public string PieceAsNiceString(int pieceNum)
        {
            return CharsToHex(Data, pieceNum * 20, 20);
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode($"String:{AsString()}");
            tn.Add(n);
        }

        public override void Write(System.IO.Stream sw)
        {
            // Byte strings are encoded as follows: <string length encoded in base ten ASCII>:<string data>

            byte[] len = System.Text.Encoding.ASCII.GetBytes(Data.Length.ToString());
            sw.Write(len, 0, len.Length);
            sw.WriteByte((byte) ':');
            sw.Write(Data, 0, Data.Length);
        }
    }

    public class BTEOF : BTItem
    {
        public BTEOF()
            : base(BTChunk.kBTEOF)
        {
        }

        public override void Write(System.IO.Stream sw)
        {
        }
    }

    public class BTError : BTItem
    {
        public string Message;

        public BTError()
            : base(BTChunk.kError)
        {
            Message = string.Empty;
        }

        public override string AsText()
        {
            return $"Error:{Message}";
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTError:" + Message);
            tn.Add(n);
        }

        public override void Write(System.IO.Stream sw)
        {
        }
    }

    public class BTListOrDictionaryEnd : BTItem
    {
        public BTListOrDictionaryEnd()
            : base(BTChunk.kListOrDictionaryEnd)
        {
        }

        public override void Write(System.IO.Stream sw)
        {
            sw.WriteByte((byte) 'e');
        }
    }

    public class BTDictionaryItem : BTItem
    {
        public BTItem Data;
        public string Key;

        public BTDictionaryItem()
            : base(BTChunk.kDictionaryItem)
        {
        }

        public BTDictionaryItem(string k, BTItem d)
            : base(BTChunk.kDictionaryItem)
        {
            Key = k;
            Data = d;
        }

        public override string AsText()
        {
            if ((Key == "pieces") && (Data.Type == BTChunk.kString))
                return "<File hash data>";

            return $"{Key}=>{Data.AsText()}";
        }

        public override void Tree(TreeNodeCollection tn)
        {
            if ((Key == "pieces") && (Data.Type == BTChunk.kString))
            {
                // 20 byte chunks of SHA1 hash values
                TreeNode n = new TreeNode("Key=" + Key);
                tn.Add(n);
                n.Nodes.Add(new TreeNode("<File hash data>" + ((BTString) Data).PieceAsNiceString(0)));
            }
            else
            {
                TreeNode n = new TreeNode("Key=" + Key);
                tn.Add(n);
                Data.Tree(n.Nodes);
            }
        }

        public override void Write(System.IO.Stream sw)
        {
            new BTString(Key).Write(sw);
            Data.Write(sw);
        }
    }

    public class BTDictionary : BTItem
    {
        public readonly List<BTDictionaryItem> Items;

        public BTDictionary()
            : base(BTChunk.kDictionary)
        {
            Items = new List<BTDictionaryItem>();
        }

        public override string AsText()
        {
            return "Dictionary=[" + Items.Select(x => x.AsText()).ToCsv() + "]";
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("Dictionary");
            tn.Add(n);
            foreach (BTDictionaryItem t in Items)
                t.Tree(n.Nodes);
        }

        public bool RemoveItem(string key)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Key == key)
                {
                    Items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public BTItem GetItem(string key) => GetItem(key, false);

        public BTItem GetItem(string key, bool ignoreCase)
        {
            foreach (BTDictionaryItem t in Items)
            {
                if ((t.Key == key) || (ignoreCase && ((t.Key.ToLower() == key.ToLower()))))
                    return t.Data;
            }

            return null;
        }

        public override void Write(System.IO.Stream sw)
        {
            sw.WriteByte((byte) 'd');
            foreach (BTDictionaryItem i in Items)
                i.Write(sw);

            sw.WriteByte((byte) 'e');
        }
    }

    public class BTList : BTItem
    {
        public readonly List<BTItem> Items;

        public BTList()
            : base(BTChunk.kList)
        {
            Items = new List<BTItem>();
        }

        public override string AsText()
        {
            return "List={" + Items.Select(x => x.AsText()).ToCsv() + "}";
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("List");
            tn.Add(n);
            foreach (BTItem t in Items)
                t.Tree(n.Nodes);
        }

        public override void Write(System.IO.Stream sw)
        {
            sw.WriteByte((byte) 'l');
            foreach (BTItem i in Items)
                i.Write(sw);

            sw.WriteByte((byte) 'e');
        }
    }

    public class BTInteger : BTItem
    {
        public long Value;

        public BTInteger()
            : base(BTChunk.kInteger)
        {
            Value = 0;
        }

        public BTInteger(long n)
            : base(BTChunk.kInteger)
        {
            Value = n;
        }

        public override string AsText()
        {
            return "Integer=" + Value;
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("Integer:" + Value);
            tn.Add(n);
        }

        public override void Write(System.IO.Stream sw)
        {
            sw.WriteByte((byte) 'i');
            byte[] b = System.Text.Encoding.ASCII.GetBytes(Value.ToString());
            sw.Write(b, 0, b.Length);
            sw.WriteByte((byte) 'e');
        }
    }

    public class BTFile
    {
        public List<BTItem> Items;

        public BTFile()
        {
            Items = new List<BTItem>();
        }

        public List<string> AllFilesInTorrent()
        {
            List<string> r = new List<string>();

            BTItem bti = GetItem("info");
            if ((bti is null) || (bti.Type != BTChunk.kDictionary))
                return null;

            BTDictionary infoDict = (BTDictionary) (bti);

            bti = infoDict.GetItem("files");

            if (bti is null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti is null) || (bti.Type != BTChunk.kString))
                    return null;

                r.Add(((BTString) bti).AsString());
            }
            else
            {
                // multiple file torrent
                BTList fileList = (BTList) (bti);

                foreach (BTItem it in fileList.Items)
                {
                    BTDictionary file = (BTDictionary) (it);
                    BTItem thePath = file.GetItem("path");
                    if (thePath.Type != BTChunk.kList)
                        return null;

                    BTList pathList = (BTList) (thePath);
                    // want the last of the items in the list, which is the filename itself
                    int n = pathList.Items.Count - 1;
                    if (n < 0)
                        return null;

                    BTString fileName = (BTString) (pathList.Items[n]);
                    r.Add(fileName.AsString());
                }
            }

            return r;
        }

        public void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BT File");
            tn.Add(n);
            foreach (BTItem t in Items)
                t.Tree(n.Nodes);
        }

        public BTItem GetItem(string key)
        {
            return GetItem(key, false);
        }

        public BTDictionary GetDict()
        {
            System.Diagnostics.Debug.Assert(Items.Count == 1);
            System.Diagnostics.Debug.Assert(Items[0].Type == BTChunk.kDictionary);

            // our first (and only) Item will be a dictionary of stuff
            return (BTDictionary) (Items[0]);
        }

        public BTItem GetItem(string key, bool ignoreCase)
        {
            if (Items.Count == 0)
                return null;

            BTDictionary btd = GetDict();
            return btd.GetItem(key, ignoreCase);
        }

        public void Write(System.IO.Stream sw)
        {
            foreach (BTItem i in Items)
                i.Write(sw);
        }
    }

    public class HashCacheItem
    {
        public long fileSize;
        public long pieceSize;
        public byte[] theHash;
        public long whereInFile;

        public HashCacheItem(long wif, long ps, long fs, byte[] h)
        {
            whereInFile = wif;
            pieceSize = ps;
            fileSize = fs;
            theHash = h;
        }
    }

    public class BEncodeLoader
    {
        public BTItem ReadString(System.IO.Stream sr, long length)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(sr);

            byte[] c = br.ReadBytes((int) length);

            BTString bts = new BTString();
            bts.Data = c;
            return bts;
        }

        public static BTItem ReadInt(System.IO.FileStream sr)
        {
            long r = 0;
            int c;
            bool neg = false;
            while ((c = sr.ReadByte()) != 'e')
            {
                if (c == '-')
                    neg = true;
                else if ((c >= '0') && (c <= '9'))
                    r = (r * 10) + c - '0';
            }

            if (neg)
                r = -r;

            BTInteger bti = new BTInteger {Value = r};
            return bti;
        }

        public BTItem ReadDictionary(System.IO.FileStream sr)
        {
            BTDictionary d = new BTDictionary();
            for (;;)
            {
                BTItem next = ReadNext(sr);
                if ((next.Type == BTChunk.kListOrDictionaryEnd) || (next.Type == BTChunk.kBTEOF))
                    return d;

                if (next.Type != BTChunk.kString)
                {
                    BTError e = new BTError();
                    e.Message = "Didn't get string as first of pair in dictionary";
                    return e;
                }

                BTDictionaryItem di = new BTDictionaryItem
                {
                    Key = ((BTString) next).AsString(),
                    Data = ReadNext(sr)
                };

                d.Items.Add(di);
            }
        }

        public BTItem ReadList(System.IO.FileStream sr)
        {
            BTList ll = new BTList();
            for (;;)
            {
                BTItem next = ReadNext(sr);
                if (next.Type == BTChunk.kListOrDictionaryEnd)
                    return ll;

                ll.Items.Add(next);
            }
        }

        public BTItem ReadNext(System.IO.FileStream sr)
        {
            if (sr.Length == sr.Position)
                return new BTEOF();

            // Read the next character from the stream to see what is next

            int c = sr.ReadByte();
            if (c == 'd')
                return ReadDictionary(sr); // dictionary

            if (c == 'l')
                return ReadList(sr); // list

            if (c == 'i')
                return ReadInt(sr); // integer

            if (c == 'e')
                return new BTListOrDictionaryEnd(); // end of list/dictionary/etc.

            if ((c >= '0') && (c <= '9')) // digits mean it is a string of the specified length
            {
                string r = Convert.ToString(c - '0');
                while ((c = sr.ReadByte()) != ':')
                    r += Convert.ToString(c - '0');

                return ReadString(sr, Convert.ToInt32(r));
            }

            BTError e = new BTError
            {
                Message = $"Error: unknown BEncode item type: {c}"
            };

            return e;
        }

        public BTFile Load(string filename)
        {
            BTFile f = new BTFile();

            System.IO.FileStream sr;
            try
            {
                sr = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }

            while (sr.Position < sr.Length)
                f.Items.Add(ReadNext(sr));

            sr.Close();

            return f;
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    }

    public abstract class BTCore
    {
        protected bool DoHashChecking;
        protected DirCache FileCache;
        protected string FileCacheIsFor;
        protected bool FileCacheWithSubFolders;
        protected Dictionary<string, List<HashCacheItem>> HashCache;
        protected SetProgressDelegate SetProg;

        protected BTCore(SetProgressDelegate setprog)
        {
            SetProg = setprog;

            HashCache = new Dictionary<string, List<HashCacheItem>>();
            FileCache = null;
            FileCacheIsFor = null;
            FileCacheWithSubFolders = false;
        }

        protected void Prog(int percent,string message) => SetProg?.Invoke(percent,message);

        protected abstract bool NewTorrentEntry(string torrentFile, int numberInTorrent);

        protected abstract bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent,
            string nameInTorrent);

        protected abstract bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent,
            string nameInTorrent);

        protected abstract bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename);

        private FileInfo FindLocalFileWithHashAt(byte[] findMe, long whereInFile, long pieceSize, long fileSize)
        {
            if (whereInFile < 0)
                return null;

            foreach (DirCacheEntry dc in FileCache)
            {
                FileInfo fiTemp = dc.TheFile;
                long flen = dc.Length;

                if ((flen != fileSize) || (flen < (whereInFile + pieceSize))) // this file is wrong size || too small
                    continue;

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
                    int n = sr.Read(thePiece, 0, (int) pieceSize);
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
                    return fiTemp;
            } // while enum

            return null;
        }

        protected void CacheThis(string filename, long whereInFile, long piecesize, long fileSize, byte[] hash)
        {
            if (!HashCache.ContainsKey(filename))
                HashCache[filename] = new List<HashCacheItem>();

            HashCache[filename].Add(new HashCacheItem(whereInFile, piecesize, fileSize, hash));
        }

        protected byte[] CheckCache(string filename, long whereInFile, long piecesize, long fileSize)
        {
            if (HashCache.ContainsKey(filename))
            {
                foreach (HashCacheItem h in HashCache[filename])
                {
                    if ((h.whereInFile == whereInFile) && (h.pieceSize == piecesize) && (h.fileSize == fileSize))
                    {
                        return h.theHash;
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

        public bool ProcessTorrentFile(string torrentFile, TreeView tvTree, CommandLineArgs args)
        {
            // ----------------------------------------
            // read in torrent file

            if (tvTree != null)
                tvTree.Nodes.Clear();

            BEncodeLoader bel = new BEncodeLoader();
            BTFile btFile = bel.Load(torrentFile);

            if (btFile is null)
                return false;

            BTItem bti = btFile.GetItem("info");
            if ((bti is null) || (bti.Type != BTChunk.kDictionary))
                return false;

            BTDictionary infoDict = (BTDictionary) (bti);

            bti = infoDict.GetItem("piece length");
            if ((bti is null) || (bti.Type != BTChunk.kInteger))
                return false;

            long pieceSize = ((BTInteger) bti).Value;

            bti = infoDict.GetItem("pieces");
            if ((bti is null) || (bti.Type != BTChunk.kString))
                return false;

            BTString torrentPieces = (BTString) (bti);

            bti = infoDict.GetItem("files");

            if (bti is null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti is null) || (bti.Type != BTChunk.kString))
                    return false;

                BTString di = (BTString) (bti);
                string nameInTorrent = di.AsString();

                BTItem fileSizeI = infoDict.GetItem("length");
                long fileSize = ((BTInteger) fileSizeI).Value;

                NewTorrentEntry(torrentFile, -1);
                if (DoHashChecking)
                {
                    byte[] torrentPieceHash = torrentPieces.StringTwentyBytePiece(0);

                    FileInfo fi = FindLocalFileWithHashAt(torrentPieceHash, 0, pieceSize, fileSize);
                    if (fi != null)
                        FoundFileOnDiskForFileInTorrent(torrentFile, fi, -1, nameInTorrent);
                    else
                        DidNotFindFileOnDiskForFileInTorrent(torrentFile, -1, nameInTorrent);
                }

                FinishedTorrentEntry(torrentFile, -1, nameInTorrent);

                // don't worry about updating overallPosition as this is the only file in the torrent
            }
            else
            {
                long overallPosition = 0;
                long lastPieceLeftover = 0;

                if (bti.Type != BTChunk.kList)
                    return false;

                BTList fileList = (BTList) (bti);

                // list of dictionaries
                for (int i = 0; i < fileList.Items.Count; i++)
                {
                    Prog(100 * i / fileList.Items.Count,i.ToString());
                    if (fileList.Items[i].Type != BTChunk.kDictionary)
                        return false;

                    BTDictionary file = (BTDictionary) (fileList.Items[i]);
                    BTItem thePath = file.GetItem("path");
                    if (thePath.Type != BTChunk.kList)
                        return false;

                    BTList pathList = (BTList) (thePath);
                    // want the last of the items in the list, which is the filename itself
                    int n = pathList.Items.Count - 1;
                    if (n < 0)
                        return false;

                    BTString fileName = (BTString) (pathList.Items[n]);

                    BTItem fileSizeI = file.GetItem("length");
                    long fileSize = ((BTInteger) fileSizeI).Value;

                    int pieceNum = (int) (overallPosition / pieceSize);
                    if (overallPosition % pieceSize != 0)
                        pieceNum++;

                    NewTorrentEntry(torrentFile, i);

                    if (DoHashChecking)
                    {
                        byte[] torrentPieceHash = torrentPieces.StringTwentyBytePiece(pieceNum);

                        FileInfo fi = FindLocalFileWithHashAt(torrentPieceHash, lastPieceLeftover, pieceSize, fileSize);
                        if (fi != null)
                            FoundFileOnDiskForFileInTorrent(torrentFile, fi, i, fileName.AsString());
                        else
                            DidNotFindFileOnDiskForFileInTorrent(torrentFile, i, fileName.AsString());
                    }

                    FinishedTorrentEntry(torrentFile, i, fileName.AsString());

                    int sizeInPieces = (int) (fileSize / pieceSize);
                    if (fileSize % pieceSize != 0)
                        sizeInPieces++; // another partial piece

                    lastPieceLeftover = (lastPieceLeftover + (int) ((sizeInPieces * pieceSize) - fileSize)) % pieceSize;
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

            Prog(0,string.Empty);

            return true;
        }
    }

    // btcore

    // BTProcessor

    public class BTResume : BTCore
    {
        private static class BTPrio
        {
            public const int Normal = 0x08, Skip = 0x80;
        }

        public bool Altered;
        public bool DoMatchMissing;
        public ItemList MissingList;

        public string NewLocation;
        public bool PrioWasSet;
        public ListView Results;

        public BTFile ResumeDat; // resume file, if we're using it
        public string ResumeDatPath;

        public List<TVSettings.FilenameProcessorRE> Rexps; // used by MatchMissing
        public bool SetPrios;
        public string Type;

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public BTResume(SetProgressDelegate setprog, string resumeDatFile)
            : base(setprog)
        {
            ResumeDatPath = resumeDatFile;
        }

        public BTDictionary GetTorrentDict(string torrentFile)
        {
            // find dictionary for the specified torrent file

            BTItem it = ResumeDat.GetDict().GetItem(torrentFile, true);
            if ((it is null) || (it.Type != BTChunk.kDictionary))
                return null;

            BTDictionary dict = (BTDictionary) (it);
            return dict;
        }

        private static int PercentBitsOn(BTString s)
        {
            int totalBits = 0;
            int bitsOn = 0;

            foreach (byte t in s.Data)
            {
                totalBits += 8;
                byte c = t;
                for (int j = 0; j < 8; j++)
                {
                    if ((c & 0x01) != 0)
                        bitsOn++;

                    c >>= 1;
                }
            }

            return (100 * bitsOn + totalBits / 2) / totalBits;
        }

        public List<TorrentEntry> AllFilesBeingDownloaded()
        {
            List<TorrentEntry> r = new List<TorrentEntry>();

            BEncodeLoader bel = new BEncodeLoader();
            foreach (BTDictionaryItem dictitem in ResumeDat.GetDict().Items)
            {
                if ((dictitem.Type != BTChunk.kDictionaryItem))
                    continue;

                if ((dictitem.Key == ".fileguard") || (dictitem.Data.Type != BTChunk.kDictionary))
                    continue;

                if  (dictitem.Data is BTError err)
                {
                    logger.Error($"Error finding BT items: {err.Message}");
                    return r;
                }

                BTDictionary d2 = (BTDictionary) (dictitem.Data);

                BTItem p = d2.GetItem("prio");
                if ((p is null) || (p.Type != BTChunk.kString))
                    continue;

                BTString prioString = (BTString) (p);
                string directoryName = Path.GetDirectoryName(ResumeDatPath) + System.IO.Path.DirectorySeparatorChar;

                string torrentFile = dictitem.Key;
                if (!File.Exists(torrentFile)) // if the torrent file doesn't exist
                    torrentFile = directoryName + torrentFile; // ..try prepending the resume.dat folder's path to it.

                if (!File.Exists(torrentFile))
                    continue; // can't find it.  give up!

                BTFile tor = bel.Load(torrentFile);
                if (tor is null)
                    continue;

                List<string> a = tor.AllFilesInTorrent();
                if (a is null) continue;

                int c = 0;

                p = d2.GetItem("path");
                if ((p is null) || (p.Type != BTChunk.kString))
                    continue;

                string defaultFolder = ((BTString) p).AsString();

                BTItem targets = d2.GetItem("targets");
                bool hasTargets = ((targets != null) && (targets.Type == BTChunk.kList));
                BTList targetList = (BTList) (targets);

                //foreach (var i in d2.Items)
                //{
                //logger.Info($"   {i.Key}  {i.Data.AsText()}");   
                //}

                foreach (string s in a)
                {
                    if ((c < prioString.Data.Length) && (prioString.Data[c] != BTPrio.Skip))
                    {
                        try
                        {
                            string saveTo = FileHelper
                                .FileInFolder(defaultFolder, TVSettings.Instance.FilenameFriendly(s)).Name;

                            if (hasTargets)
                            {
                                // see if there is a target for this (the c'th) file
                                foreach (BTItem t in targetList.Items)
                                {
                                    BTList l = (BTList) (t);
                                    BTInteger n = (BTInteger) (l.Items[0]);
                                    BTString dest = (BTString) (l.Items[1]);
                                    if (n.Value == c)
                                    {
                                        saveTo = dest.AsString();
                                        break;
                                    }
                                }
                            }

                            int percent = (a.Count == 1) ? PercentBitsOn((BTString) (d2.GetItem("have"))) : -1;
                            bool completed = ((BTInteger) d2.GetItem("order")).Value == -1;
                            TorrentEntry te = new TorrentEntry(torrentFile, saveTo, percent, completed, torrentFile); 
                            r.Add(te);
                        }
                        catch (System.IO.PathTooLongException ptle)
                        {
                            //this is not the file we are looking for
                            logger.Debug(ptle);
                        }
                    }

                    c++;
                }
            }

            return r;
        }

        public string GetResumePrio(string torrentFile, int fileNum)
        {
            BTDictionary dict = GetTorrentDict(torrentFile);
            if (dict is null)
                return "";

            BTItem p = dict.GetItem("prio");
            if ((p is null) || (p.Type != BTChunk.kString))
                return "";

            BTString prioString = (BTString) (p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
                return "";

            int pr = prioString.Data[fileNum];
            if (pr == BTPrio.Normal)
                return "Normal";

            if (pr == BTPrio.Skip)
                return "Skip";

            return pr.ToString();
        }

        private void SetResumePrio(string torrentFile, int fileNum, byte newPrio)
        {
            if (!SetPrios)
                return;

            if (fileNum == -1)
                fileNum = 0;

            BTDictionary dict = GetTorrentDict(torrentFile);

            BTItem p = dict?.GetItem("prio");
            if (p is null || (p.Type != BTChunk.kString))
                return;

            BTString prioString = (BTString) (p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
                return;

            Altered = true;
            PrioWasSet = true;

            prioString.Data[fileNum] = newPrio;
        }

        public void AlterResume(string torrentFile, int fileNum, string toHere)
        {
            toHere = RemoveUT(toHere);

            BTDictionary dict = GetTorrentDict(torrentFile);
            if (dict is null)
                return;

            Altered = true;

            if (fileNum == -1) // single file torrent
            {
                BTItem p = dict.GetItem("path");
                if (p is null)
                    dict.Items.Add(new BTDictionaryItem("path", new BTString(toHere)));
                else
                {
                    if (p.Type != BTChunk.kString)
                        return;

                    ((BTString) p).SetString(toHere);
                }
            }
            else
            {
                // multiple file torrent, uses a list called "targets"
                BTItem p = dict.GetItem("targets");
                BTList theList = null;
                if (p is null)
                {
                    theList = new BTList();
                    dict.Items.Add(new BTDictionaryItem("targets", theList));
                }
                else
                {
                    if (p.Type != BTChunk.kList)
                        return;

                    theList = (BTList) (p);
                }

                // the list contains two element lists, of integer/string which are filenumber/path

                BTList thisFileList = null;
                // see if this file is already in the list
                foreach (BTItem t in theList.Items)
                {
                    if (t.Type != BTChunk.kList)
                        return;

                    BTList l2 = (BTList) (t);
                    if ((l2.Items.Count != 2) || (l2.Items[0].Type != BTChunk.kInteger) ||
                        (l2.Items[1].Type != BTChunk.kString))
                        return;

                    int n = (int) ((BTInteger) (l2.Items[0])).Value;
                    if (n == fileNum)
                    {
                        thisFileList = l2;
                        break;
                    }
                }

                if (thisFileList is null) // didn't find it
                {
                    thisFileList = new BTList();
                    thisFileList.Items.Add(new BTInteger(fileNum));
                    thisFileList.Items.Add(new BTString(toHere));
                    theList.Items.Add(thisFileList);
                }
                else
                    thisFileList.Items[1] = new BTString(toHere);
            }
        }

        public void FixFileguard()
        {
            // finally, fix up ".fileguard"
            // this is the SHA1 of the entire file, without the .fileguard
            ResumeDat.GetDict().RemoveItem(".fileguard");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ResumeDat.Write(ms);
            System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();
            byte[] theHash = sha1.ComputeHash(ms.GetBuffer(), 0, (int) ms.Length);
            ms.Close();
            string newfg = BTString.CharsToHex(theHash, 0, 20);
            ResumeDat.GetDict().Items.Add(new BTDictionaryItem(".fileguard", new BTString(newfg)));
        }

        public FileInfo MatchMissing(string torrentFile, int torrentFileNum, string nameInTorrent)
        {
            // returns true if we found a match (if actSetPrio is on, true also means we have set a priority for this file)
            string simplifiedfname = nameInTorrent.CompareName();

            foreach (Item action1 in MissingList)
            {
                if ((!(action1 is ItemMissing)) && (!(action1 is ItemDownloading)))
                    continue;

                ProcessedEpisode m = action1.Episode;
                string name = null;

                switch (action1)
                {
                    case ItemMissing action:
                        name = action.TheFileNoExt;
                        break;
                    case ItemDownloading actionIp:
                        name = actionIp.DesiredLocationNoExt;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if ((m is null) || string.IsNullOrEmpty(name))
                    continue;

                // see if the show name matches...
                if (FileHelper.SimplifyAndCheckFilename(simplifiedfname, m.TheCachedSeries.Name, false, true))
                {
                    // see if season and episode match
                    bool findFile = FinderHelper.FindSeasEp("", simplifiedfname, out int seasF, out int epF, out int maxEp,
                        m.Show, Rexps,
                        out TVSettings.FilenameProcessorRE rex);

                    if (findFile && (seasF == m.AppropriateSeasonNumber) && (epF == m.AppropriateEpNum))
                    {
                        // match!
                        // get extension from nameInTorrent
                        int p = nameInTorrent.LastIndexOf(".", StringComparison.Ordinal);
                        string ext = (p == -1) ? "" : nameInTorrent.Substring(p);
                        AlterResume(torrentFile, torrentFileNum, name + ext);
                        if (SetPrios)
                            SetResumePrio(torrentFile, torrentFileNum, BTPrio.Normal);

                        return new FileInfo(name + ext);
                    }
                }
            }

            return null;
        }

        public void WriteResumeDat()
        {
            FixFileguard();
            // write out new resume.dat file
            string to = ResumeDatPath + ".before_tvrename";
            if (File.Exists(to))
                File.Delete(to);

            File.Move(ResumeDatPath, to);
            System.IO.Stream s = File.Create(ResumeDatPath);
            ResumeDat.Write(s);
            s.Close();
        }

        protected override bool NewTorrentEntry(string torrentFile, int numberInTorrent)
        {
            NewLocation = string.Empty;
            PrioWasSet = false;
            Type = "?";
            return true;
        }

        protected override bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent,
            string nameInTorrent)
        {
            NewLocation = onDisk.FullName;
            Type = "Hash";

            AlterResume(torrentFile, numberInTorrent, onDisk.FullName); // make resume.dat point to the file we found

            if (SetPrios)
                SetResumePrio(torrentFile, numberInTorrent, BTPrio.Normal);

            return true;
        }

        protected override bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent,
            string nameInTorrent)
        {
            Type = "Not Found";

            if (SetPrios)
                SetResumePrio(torrentFile, numberInTorrent, BTPrio.Skip);

            return true;
        }

        protected override bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename)
        {
            if (DoMatchMissing)
            {
                FileInfo s = MatchMissing(torrentFile, numberInTorrent, filename);
                if (s != null)
                {
                    PrioWasSet = true;
                    NewLocation = s.FullName;
                    Type = "Missing";
                }
            }

            if (SetPrios && !PrioWasSet)
            {
                SetResumePrio(torrentFile, numberInTorrent, BTPrio.Skip);
                Type = "Not Missing";
            }

            bool prioChanged = SetPrios && PrioWasSet;
            if (prioChanged || (!string.IsNullOrEmpty(NewLocation)))
                AddResult(Type, torrentFile, (numberInTorrent + 1).ToString(),
                    prioChanged ? GetResumePrio(torrentFile, numberInTorrent) : string.Empty, NewLocation);

            return true;
        }

        public bool LoadResumeDat()
        {
            BEncodeLoader bel = new BEncodeLoader();
            ResumeDat = bel.Load(ResumeDatPath);
            return (ResumeDat != null);
        }

        public bool DoWork(List<string> Torrents, string searchFolder, ListView results, bool hashSearch,
            bool matchMissing, bool setPrios, bool testMode,
            bool searchSubFolders, ItemList missingList, List<TVSettings.FilenameProcessorRE> rexps, CommandLineArgs args)
        {
            Rexps = rexps;

            if (!matchMissing && !hashSearch)
                return true; // nothing to do

            if (hashSearch && string.IsNullOrEmpty(searchFolder))
                return false;

            if (matchMissing && ((missingList is null) || (rexps is null)))
                return false;

            MissingList = missingList;
            DoMatchMissing = matchMissing;
            DoHashChecking = hashSearch;
            SetPrios = setPrios;
            Results = results;

            Prog(0,string.Empty);

            if (!LoadResumeDat())
                return false;

            bool r = true;

            Prog(0, string.Empty);

            if (hashSearch)
                BuildFileCache(searchFolder, searchSubFolders);

            foreach (string tf in Torrents)
            {
                r = ProcessTorrentFile(tf, null, args);
                if (!r) // stop on the first failure
                    break;
            }

            if (Altered && !testMode)
                WriteResumeDat();

            Prog(0,string.Empty);

            return r;
        }

        private static string RemoveUT(string s)
        {
            // if it is a .!ut file, we can remove the extension
            return s.EndsWith(".!ut") ? s.Remove(s.Length - 4) : s;
        }

        private void AddResult(string type, string torrent, string num, string prio, string location)
        {
            if (Results is null)
                return;

            int p = torrent.LastIndexOf(System.IO.Path.DirectorySeparatorChar.ToString());
            if (p != -1)
                torrent = torrent.Substring(p + 1);

            ListViewItem lvi = new ListViewItem(type);
            lvi.SubItems.Add(torrent);
            lvi.SubItems.Add(num);
            lvi.SubItems.Add(prio);
            lvi.SubItems.Add(RemoveUT(location));

            Results.Items.Add(lvi);
            lvi.EnsureVisible();
            Results.Update();
        }
    }
}
