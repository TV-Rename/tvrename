// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Collections.Generic;
using System.Windows.Forms;

// Opens, understands, manipulates, and writes out BEncoded .torrent files, and uTorrent's resume.dat

namespace TVRename
{
    using System.IO;
    using Directory = Alphaleonis.Win32.Filesystem.Directory;
    using File = Alphaleonis.Win32.Filesystem.File;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
    using Path = Alphaleonis.Win32.Filesystem.Path;

    public enum BtChunk
    {
        KError,
        KDictionary,
        KDictionaryItem,
        KList,
        KListOrDictionaryEnd,
        KInteger,
        KString,
        KBteof
    }

    public class TorrentEntry // represents a torrent downloading in uTorrent
    {
        public string DownloadingTo;
        public int PercentDone;
        public string TorrentFile;

        public TorrentEntry(string torrentfile, string to, int percent)
        {
            TorrentFile = torrentfile;
            DownloadingTo = to;
            PercentDone = percent;
        }
    }

    public abstract class BtItem
    {
        public BtChunk Type; // from enum

        protected BtItem(BtChunk type)
        {
            Type = type;
        }

        public virtual string AsText()
        {
            return string.Concat("Type =", Type.ToString());
        }

        public virtual void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTItem:" + Type);
            tn.Add(n);
        }

        public abstract void Write(Stream sw);
    }

    public class BtString : BtItem
    {
        public byte[] Data;

        public BtString(string s)
            : base(BtChunk.KString)
        {
            SetString(s);
        }

        public BtString()
            : base(BtChunk.KString)
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
            string r = "";
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
            TreeNode n = new TreeNode(string.Concat("String:", AsString()));
            tn.Add(n);
        }

        public override void Write(Stream sw)
        {
            // Byte strings are encoded as follows: <string length encoded in base ten ASCII>:<string data>

            Byte[] len = System.Text.Encoding.ASCII.GetBytes(Data.Length.ToString());
            sw.Write(len, 0, len.Length);
            sw.WriteByte((byte) ':');
            sw.Write(Data, 0, Data.Length);
        }
    }

    public class Bteof : BtItem
    {
        public Bteof()
            : base(BtChunk.KBteof)
        {
        }

        public override void Write(Stream sw)
        {
        }
    }

    public class BtError : BtItem
    {
        public string Message;

        public BtError()
            : base(BtChunk.KError)
        {
            Message = "";
        }

        public override string AsText()
        {
            return string.Concat("Error:", Message);
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTError:" + Message);
            tn.Add(n);
        }

        public override void Write(Stream sw)
        {
        }
    }

    public class BtListOrDictionaryEnd : BtItem
    {
        public BtListOrDictionaryEnd()
            : base(BtChunk.KListOrDictionaryEnd)
        {
        }

        public override void Write(Stream sw)
        {
            sw.WriteByte((byte) 'e');
        }
    }

    public class BtDictionaryItem : BtItem
    {
        public BtItem Data;
        public string Key;

        public BtDictionaryItem()
            : base(BtChunk.KDictionaryItem)
        {
        }

        public BtDictionaryItem(string k, BtItem d)
            : base(BtChunk.KDictionaryItem)
        {
            Key = k;
            Data = d;
        }

        public override string AsText()
        {
            if ((Key == "pieces") && (Data.Type == BtChunk.KString))
                return "<File hash data>";
            return string.Concat(Key, "=>", Data.AsText());
        }

        public override void Tree(TreeNodeCollection tn)
        {
            if ((Key == "pieces") && (Data.Type == BtChunk.KString))
            {
                // 20 byte chunks of SHA1 hash values
                TreeNode n = new TreeNode("Key=" + Key);
                tn.Add(n);
                n.Nodes.Add(new TreeNode("<File hash data>" + ((BtString) Data).PieceAsNiceString(0)));
            }
            else
            {
                TreeNode n = new TreeNode("Key=" + Key);
                tn.Add(n);
                Data.Tree(n.Nodes);
            }
        }

        public override void Write(Stream sw)
        {
            new BtString(Key).Write(sw);
            Data.Write(sw);
        }
    }

    public class BtDictionary : BtItem
    {
        public List<BtDictionaryItem> Items;

        public BtDictionary()
            : base(BtChunk.KDictionary)
        {
            Items = new List<BtDictionaryItem>();
        }

        public override string AsText()
        {
            string r = "Dictionary=[";
            for (int i = 0; i < Items.Count; i++)
            {
                r += Items[i].AsText();
                if (i != (Items.Count - 1))
                    r += ",";
            }
            r += "]";
            return r;
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("Dictionary");
            tn.Add(n);
            for (int i = 0; i < Items.Count; i++)
                Items[i].Tree(n.Nodes);
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

        public BtItem GetItem(string key)
        {
            return GetItem(key, false);
        }

        public BtItem GetItem(string key, bool ignoreCase)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if ((Items[i].Key == key) || (ignoreCase && ((Items[i].Key.ToLower() == key.ToLower()))))
                    return Items[i].Data;
            }
            return null;
        }

        public override void Write(Stream sw)
        {
            sw.WriteByte((byte) 'd');
            foreach (BtDictionaryItem i in Items)
                i.Write(sw);
            sw.WriteByte((byte) 'e');
        }
    }

    public class BtList : BtItem
    {
        public List<BtItem> Items;

        public BtList()
            : base(BtChunk.KList)
        {
            Items = new List<BtItem>();
        }

        public override string AsText()
        {
            string r = "List={";
            for (int i = 0; i < Items.Count; i++)
            {
                r += Items[i].AsText();
                if (i != (Items.Count - 1))
                    r += ",";
            }
            r += "}";
            return r;
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("List");
            tn.Add(n);
            for (int i = 0; i < Items.Count; i++)
                Items[i].Tree(n.Nodes);
        }

        public override void Write(Stream sw)
        {
            sw.WriteByte((byte) 'l');
            foreach (BtItem i in Items)
                i.Write(sw);
            sw.WriteByte((byte) 'e');
        }
    }

    public class BtInteger : BtItem
    {
        public Int64 Value;

        public BtInteger()
            : base(BtChunk.KInteger)
        {
            Value = 0;
        }

        public BtInteger(Int64 n)
            : base(BtChunk.KInteger)
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

        public override void Write(Stream sw)
        {
            sw.WriteByte((byte) 'i');
            byte[] b = System.Text.Encoding.ASCII.GetBytes(Value.ToString());
            sw.Write(b, 0, b.Length);
            sw.WriteByte((byte) 'e');
        }
    }

    public class BtFile
    {
        public List<BtItem> Items;

        public BtFile()
        {
            Items = new List<BtItem>();
        }

        public List<string> AllFilesInTorrent()
        {
            List<string> r = new List<String>();

            BtItem bti = GetItem("info");
            if ((bti == null) || (bti.Type != BtChunk.KDictionary))
                return null;

            BtDictionary infoDict = (BtDictionary) (bti);

            bti = infoDict.GetItem("files");

            if (bti == null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti == null) || (bti.Type != BtChunk.KString))
                    return null;
                r.Add(((BtString) bti).AsString());
            }
            else
            {
                // multiple file torrent
                BtList fileList = (BtList) (bti);

                foreach (BtItem it in fileList.Items)
                {
                    BtDictionary file = (BtDictionary) (it);
                    BtItem thePath = file.GetItem("path");
                    if (thePath.Type != BtChunk.KList)
                        return null;
                    BtList pathList = (BtList) (thePath);
                    // want the last of the items in the list, which is the filename itself
                    int n = pathList.Items.Count - 1;
                    if (n < 0)
                        return null;
                    BtString fileName = (BtString) (pathList.Items[n]);
                    r.Add(fileName.AsString());
                }
            }

            return r;
        }

        public string AsText()
        {
            string res = "File= ";
            for (int i = 0; i < Items.Count; i++)
                res += Items[i].AsText() + " ";
            return res;
        }

        public void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BT File");
            tn.Add(n);
            for (int i = 0; i < Items.Count; i++)
                Items[i].Tree(n.Nodes);
        }

        public BtItem GetItem(string key)
        {
            return GetItem(key, false);
        }

        public BtDictionary GetDict()
        {
            System.Diagnostics.Debug.Assert(Items.Count == 1);
            System.Diagnostics.Debug.Assert(Items[0].Type == BtChunk.KDictionary);

            // our first (and only) Item will be a dictionary of stuff
            return (BtDictionary) (Items[0]);
        }

        public BtItem GetItem(string key, bool ignoreCase)
        {
            if (Items.Count == 0)
                return null;
            BtDictionary btd = GetDict();
            return btd.GetItem(key, ignoreCase);
        }

        public void Write(Stream sw)
        {
            foreach (BtItem i in Items)
                i.Write(sw);
        }
    }

    public class HashCacheItem
    {
        public Int64 FileSize;
        public Int64 PieceSize;
        public byte[] TheHash;
        public Int64 WhereInFile;

        public HashCacheItem(Int64 wif, Int64 ps, Int64 fs, byte[] h)
        {
            WhereInFile = wif;
            PieceSize = ps;
            FileSize = fs;
            TheHash = h;
        }
    }

    public class BEncodeLoader
    {
        public BtItem ReadString(Stream sr, Int64 length)
        {
            BinaryReader br = new BinaryReader(sr);

            byte[] c = br.ReadBytes((int) length);

            BtString bts = new BtString();
            bts.Data = c;
            return bts;
        }

        public BtItem ReadInt(FileStream sr)
        {
            Int64 r = 0;
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

            BtInteger bti = new BtInteger();
            bti.Value = r;
            return bti;
        }

        public BtItem ReadDictionary(FileStream sr)
        {
            BtDictionary d = new BtDictionary();
            for (;;)
            {
                BtItem next = ReadNext(sr);
                if ((next.Type == BtChunk.KListOrDictionaryEnd) || (next.Type == BtChunk.KBteof))
                    return d;

                if (next.Type != BtChunk.KString)
                {
                    BtError e = new BtError();
                    e.Message = "Didn't get string as first of pair in dictionary";
                    return e;
                }

                BtDictionaryItem di = new BtDictionaryItem
                                          {
                                              Key = ((BtString)next).AsString(),
                                              Data = ReadNext(sr)
                                          };

                d.Items.Add(di);
            }
        }

        public BtItem ReadList(FileStream sr)
        {
            BtList ll = new BtList();
            for (;;)
            {
                BtItem next = ReadNext(sr);
                if (next.Type == BtChunk.KListOrDictionaryEnd)
                    return ll;

                ll.Items.Add(next);
            }
        }

        public BtItem ReadNext(FileStream sr)
        {
            if (sr.Length == sr.Position)
                return new Bteof();

            // Read the next character from the stream to see what is next

            int c = sr.ReadByte();
            if (c == 'd')
                return ReadDictionary(sr); // dictionary
            if (c == 'l')
                return ReadList(sr); // list
            if (c == 'i')
                return ReadInt(sr); // integer
            if (c == 'e')
                return new BtListOrDictionaryEnd(); // end of list/dictionary/etc.
            if ((c >= '0') && (c <= '9')) // digits mean it is a string of the specified length
            {
                string r = Convert.ToString(c - '0');
                while ((c = sr.ReadByte()) != ':')
                    r += Convert.ToString(c - '0');
                return ReadString(sr, Convert.ToInt32(r));
            }
                
            BtError e = new BtError {
                                        Message = String.Concat("Error: unknown BEncode item type: ", c)
                                    };
            return e;
        }

        public BtFile Load(string filename)
        {
            BtFile f = new BtFile();

            FileStream sr;
            try
            {
                sr = new FileStream(filename, FileMode.Open, FileAccess.Read);
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
        protected static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    }

    public abstract class BtCore
    {
        protected int CacheChecks;
        protected int CacheHits;
        protected int CacheItems;
        protected bool DoHashChecking;
        protected DirCache FileCache;
        protected string FileCacheIsFor;
        protected bool FileCacheWithSubFolders;
        protected Dictionary<string, List<HashCacheItem>> HashCache;
        protected SetProgressDelegate SetProg;

        protected BtCore(SetProgressDelegate setprog)
        {
            SetProg = setprog;

            HashCache = new Dictionary<string, List<HashCacheItem>>();
            CacheChecks = CacheItems = CacheHits = 0;
            FileCache = null;
            FileCacheIsFor = null;
            FileCacheWithSubFolders = false;
        }

        protected void Prog(int percent)
        {
            if (SetProg != null)
                SetProg.Invoke(percent);
        }

        public abstract bool NewTorrentEntry(string torrentFile, int numberInTorrent);

        public abstract bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent, string nameInTorrent);

        public abstract bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent, string nameInTorrent);

        public abstract bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename);

        public FileInfo FindLocalFileWithHashAt(byte[] findMe, Int64 whereInFile, Int64 pieceSize, Int64 fileSize)
        {
            if (whereInFile < 0)
                return null;

            foreach (DirCacheEntry dc in FileCache)
                //for (int i = 0; i < FileCache.Cache.Count; i++)
            {
                FileInfo fiTemp = dc.TheFile;
                Int64 flen = dc.Length;

                if ((flen != fileSize) || (flen < (whereInFile + pieceSize))) // this file is wrong size || too small
                    continue;

                byte[] theHash = CheckCache(fiTemp.FullName, whereInFile, pieceSize, fileSize);
                if (theHash == null)
                {
                    // not cached, figure it out ourselves
                    FileStream sr;
                    try
                    {
                        sr = new FileStream(fiTemp.FullName, FileMode.Open);
                    }
                    catch
                    {
                        return null;
                    }

                    byte[] thePiece = new byte[pieceSize];
                    sr.Seek(whereInFile, SeekOrigin.Begin);
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

        protected void CacheThis(string filename, Int64 whereInFile, Int64 piecesize, Int64 fileSize, byte[] hash)
        {
            CacheItems++;
            if (!HashCache.ContainsKey(filename))
                HashCache[filename] = new List<HashCacheItem>();
            HashCache[filename].Add(new HashCacheItem(whereInFile, piecesize, fileSize, hash));
        }

        protected byte[] CheckCache(string filename, Int64 whereInFile, Int64 piecesize, Int64 fileSize)
        {
            CacheChecks++;
            if (HashCache.ContainsKey(filename))
            {
                foreach (HashCacheItem h in HashCache[filename])
                {
                    if ((h.WhereInFile == whereInFile) && (h.PieceSize == piecesize) && (h.FileSize == fileSize))
                    {
                        CacheHits++;
                        return h.TheHash;
                    }
                }
            }
            return null;
        }

        protected void BuildFileCache(string folder, bool subFolders)
        {
            if ((FileCache == null) || (FileCacheIsFor == null) || (FileCacheIsFor != folder) || (FileCacheWithSubFolders != subFolders))
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
            BtFile btFile = bel.Load(torrentFile);

            if (btFile == null)
                return false;

            BtItem bti = btFile.GetItem("info");
            if ((bti == null) || (bti.Type != BtChunk.KDictionary))
                return false;

            BtDictionary infoDict = (BtDictionary) (bti);

            bti = infoDict.GetItem("piece length");
            if ((bti == null) || (bti.Type != BtChunk.KInteger))
                return false;

            Int64 pieceSize = ((BtInteger) bti).Value;

            bti = infoDict.GetItem("pieces");
            if ((bti == null) || (bti.Type != BtChunk.KString))
                return false;

            BtString torrentPieces = (BtString) (bti);

            bti = infoDict.GetItem("files");

            if (bti == null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti == null) || (bti.Type != BtChunk.KString))
                    return false;

                BtString di = (BtString) (bti);
                string nameInTorrent = di.AsString();

                BtItem fileSizeI = infoDict.GetItem("length");
                Int64 fileSize = ((BtInteger) fileSizeI).Value;

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
                Int64 overallPosition = 0;
                Int64 lastPieceLeftover = 0;

                if (bti.Type != BtChunk.KList)
                    return false;

                BtList fileList = (BtList) (bti);

                // list of dictionaries
                for (int i = 0; i < fileList.Items.Count; i++)
                {
                    Prog(100 * i / fileList.Items.Count);
                    if (fileList.Items[i].Type != BtChunk.KDictionary)
                        return false;

                    BtDictionary file = (BtDictionary) (fileList.Items[i]);
                    BtItem thePath = file.GetItem("path");
                    if (thePath.Type != BtChunk.KList)
                        return false;
                    BtList pathList = (BtList) (thePath);
                    // want the last of the items in the list, which is the filename itself
                    int n = pathList.Items.Count - 1;
                    if (n < 0)
                        return false;
                    BtString fileName = (BtString) (pathList.Items[n]);

                    BtItem fileSizeI = file.GetItem("length");
                    Int64 fileSize = ((BtInteger) fileSizeI).Value;

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

                    lastPieceLeftover = (lastPieceLeftover + (Int32) ((sizeInPieces * pieceSize) - fileSize)) % pieceSize;
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

            Prog(0);

            return true;
        }
    }

    // btcore

    public class BtFileRenamer : BtCore
    {
        // Hash and file caches

        // settings for processing the torrent (and optionally resume) files
        //    int Action;
        //    String ^torrentFile;
        //    String ^folder;
        //TextBox ^status;

        public bool CopyNotMove;
        public string CopyToFolder;

        public ItemList RenameListOut;
        
        public BtFileRenamer(SetProgressDelegate setprog)
            : base(setprog)
        {
        }

        //    String ^secondFolder; // resume.dat location, or where to copy/move to

        public override bool NewTorrentEntry(string torrentFile, int numberInTorrent)
        {
            return true;
        }

        public override bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent, string nameInTorrent)
        {
            RenameListOut.Add(new ActionCopyMoveRename(CopyNotMove ? ActionCopyMoveRename.Op.Copy : ActionCopyMoveRename.Op.Rename, onDisk, FileHelper.FileInFolder(CopyNotMove ? CopyToFolder : onDisk.Directory.Name, nameInTorrent), null,null));

            return true;
        }

        public override bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent, string nameInTorrent)
        {
            return true;
        }

        public override bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename)
        {
            return true;
        }

        public string CacheStats()
        {
            string r = "Hash Cache: " + CacheItems + " items for " + HashCache.Count + " files.  " + CacheHits + " hits from " + CacheChecks + " lookups";
            if (CacheChecks != 0)
                r += " (" + (100 * CacheHits / CacheChecks) + "%)";
            return r;
        }

        public bool RenameFilesOnDiskToMatchTorrent(string torrentFile, string folder, TreeView tvTree,
            ItemList renameListOut,
            bool copyNotMove, string copyDest, CommandLineArgs args)
        {
            if ((string.IsNullOrEmpty(folder) || !Directory.Exists(folder)))
                return false;

            if (string.IsNullOrEmpty(torrentFile))
                return false;

            if (renameListOut == null)
                return false;

            if (copyNotMove && (string.IsNullOrEmpty(copyDest) || !Directory.Exists(copyDest)))
                return false;

            CopyNotMove = copyNotMove;
            CopyToFolder = copyDest;
            DoHashChecking = true;
            RenameListOut = renameListOut;

            Prog(0);

            BuildFileCache(folder, false); // don't do subfolders

            RenameListOut.Clear();

            bool r = ProcessTorrentFile(torrentFile, tvTree, args);

            return r;
        }
    }

    // BTProcessor

    public class BtResume : BtCore
    {
        private static class BtPrio { public const int Normal = 0x08, Skip = 0x80; }

        public bool Altered;
        public bool DoMatchMissing;
        public bool HashSearch;
        public ItemList MissingList;

        public string NewLocation;
        public bool PrioWasSet;
        public ListView Results;

        public BtFile ResumeDat; // resume file, if we're using it
        public string ResumeDatPath;

        public List<FilenameProcessorRe> Rexps; // used by MatchMissing
        public bool SearchSubFolders;
        public bool SetPrios;
        public bool TestMode;
        public string Type;

        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public BtResume(SetProgressDelegate setprog, string resumeDatFile)
            : base(setprog)
        {
            ResumeDatPath = resumeDatFile;
        }

        public BtDictionary GetTorrentDict(string torrentFile)
        {
            // find dictionary for the specified torrent file

            BtItem it = ResumeDat.GetDict().GetItem(torrentFile, true);
            if ((it == null) || (it.Type != BtChunk.KDictionary))
                return null;
            BtDictionary dict = (BtDictionary) (it);
            return dict;
        }

        public static int PercentBitsOn(BtString s)
        {
            int totalBits = 0;
            int bitsOn = 0;

            for (int i = 0; i < s.Data.Length; i++)
            {
                totalBits += 8;
                byte c = s.Data[i];
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
            foreach (BtDictionaryItem it in ResumeDat.GetDict().Items)
            {
                if ((it.Type != BtChunk.KDictionaryItem))
                    continue;

                BtDictionaryItem dictitem = (BtDictionaryItem) (it);

                if ((dictitem.Key == ".fileguard") || (dictitem.Data.Type != BtChunk.KDictionary))
                    continue;

                string torrentFile = dictitem.Key;
                BtDictionary d2 = (BtDictionary) (dictitem.Data);

                BtItem p = d2.GetItem("prio");
                if ((p == null) || (p.Type != BtChunk.KString))
                    continue;

                BtString prioString = (BtString) (p);
                string directoryName = Path.GetDirectoryName(ResumeDatPath) + System.IO.Path.DirectorySeparatorChar;

                if (!File.Exists(torrentFile)) // if the torrent file doesn't exist
                    torrentFile = directoryName + torrentFile; // ..try prepending the resume.dat folder's path to it.

                if (!File.Exists(torrentFile))
                    continue; // can't find it.  give up!
                
                BtFile tor = bel.Load(torrentFile);
                if (tor == null)
                    continue;

                List<string> a = tor.AllFilesInTorrent();
                if (a != null)
                {
                    int c = 0;

                    p = d2.GetItem("path");
                    if ((p == null) || (p.Type != BtChunk.KString))
                        continue;
                    string defaultFolder = ((BtString) p).AsString();

                    BtItem targets = d2.GetItem("targets");
                    bool hasTargets = ((targets != null) && (targets.Type == BtChunk.KList));
                    BtList targetList = (BtList) (targets);

                    foreach (string s in a)
                    {
                        if ((c < prioString.Data.Length) && (prioString.Data[c] != BtPrio.Skip))
                        {
                            try
                            {
                                string saveTo = FileHelper.FileInFolder(defaultFolder, TVSettings.Instance.FilenameFriendly(s)).Name;
                                if (hasTargets)
                                {
                                    // see if there is a target for this (the c'th) file
                                    for (int i = 0; i < targetList.Items.Count; i++)
                                    {
                                        BtList l = (BtList)(targetList.Items[i]);
                                        BtInteger n = (BtInteger)(l.Items[0]);
                                        BtString dest = (BtString)(l.Items[1]);
                                        if (n.Value == c)
                                        {
                                            saveTo = dest.AsString();
                                            break;
                                        }
                                    }
                                }
                                int percent = (a.Count == 1) ? PercentBitsOn((BtString)(d2.GetItem("have"))) : -1;
                                TorrentEntry te = new TorrentEntry(torrentFile, saveTo, percent);
                                r.Add(te);

                            }
                            catch (PathTooLongException ptle)
                            {
                                //this is not the file we are looking for
                                _logger.Debug(ptle);
                            }
                        }
                        c++;
                    }
                }
            }

            return r;
        }

        public string GetResumePrio(string torrentFile, int fileNum)
        {
            BtDictionary dict = GetTorrentDict(torrentFile);
            if (dict == null)
                return "";
            BtItem p = dict.GetItem("prio");
            if ((p == null) || (p.Type != BtChunk.KString))
                return "";
            BtString prioString = (BtString) (p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
                return "";

            int pr = prioString.Data[fileNum];
            if (pr == BtPrio.Normal)
                return "Normal";
            if (pr == BtPrio.Skip)
                return "Skip";
            return pr.ToString();
        }

        public void SetResumePrio(string torrentFile, int fileNum, byte newPrio)
        {
            if (!SetPrios)
                return;

            if (fileNum == -1)
                fileNum = 0;
            BtDictionary dict = GetTorrentDict(torrentFile);
            if (dict == null)
                return;
            BtItem p = dict.GetItem("prio");
            if ((p == null) || (p.Type != BtChunk.KString))
                return;
            BtString prioString = (BtString) (p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
                return;

            Altered = true;
            PrioWasSet = true;

            prioString.Data[fileNum] = newPrio;

            string ps;
            if (newPrio == BtPrio.Skip)
                ps = "Skip";
            else if (newPrio == BtPrio.Normal)
                ps = "Normal";
            else
                ps = newPrio.ToString();
        }

        public void AlterResume(string torrentFile, int fileNum, string toHere)
        {
            toHere = RemoveUt(toHere);

            BtDictionary dict = GetTorrentDict(torrentFile);
            if (dict == null)
                return;

            Altered = true;

            if (fileNum == -1) // single file torrent
            {
                BtItem p = dict.GetItem("path");
                if (p == null)
                    dict.Items.Add(new BtDictionaryItem("path", new BtString(toHere)));
                else
                {
                    if (p.Type != BtChunk.KString)
                        return;
                    ((BtString) p).SetString(toHere);
                }
            }
            else
            {
                // multiple file torrent, uses a list called "targets"
                BtItem p = dict.GetItem("targets");
                BtList theList = null;
                if (p == null)
                {
                    theList = new BtList();
                    dict.Items.Add(new BtDictionaryItem("targets", theList));
                }
                else
                {
                    if (p.Type != BtChunk.KList)
                        return;
                    theList = (BtList) (p);
                }
                if (theList == null)
                    return;

                // the list contains two element lists, of integer/string which are filenumber/path

                BtList thisFileList = null;
                // see if this file is already in the list
                for (int i = 0; i < theList.Items.Count; i++)
                {
                    if (theList.Items[i].Type != BtChunk.KList)
                        return;

                    BtList l2 = (BtList) (theList.Items[i]);
                    if ((l2.Items.Count != 2) || (l2.Items[0].Type != BtChunk.KInteger) || (l2.Items[1].Type != BtChunk.KString))
                        return;
                    int n = (int) ((BtInteger) (l2.Items[0])).Value;
                    if (n == fileNum)
                    {
                        thisFileList = l2;
                        break;
                    }
                }
                if (thisFileList == null) // didn't find it
                {
                    thisFileList = new BtList();
                    thisFileList.Items.Add(new BtInteger(fileNum));
                    thisFileList.Items.Add(new BtString(toHere));
                    theList.Items.Add(thisFileList);
                }
                else
                    thisFileList.Items[1] = new BtString(toHere);
            }
        }

        public void FixFileguard()
        {
            // finally, fix up ".fileguard"
            // this is the SHA1 of the entire file, without the .fileguard
            ResumeDat.GetDict().RemoveItem(".fileguard");
            MemoryStream ms = new MemoryStream();
            ResumeDat.Write(ms);
            System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();
            byte[] theHash = sha1.ComputeHash(ms.GetBuffer(), 0, (int) ms.Length);
            ms.Close();
            string newfg = BtString.CharsToHex(theHash, 0, 20);
            ResumeDat.GetDict().Items.Add(new BtDictionaryItem(".fileguard", new BtString(newfg)));
        }

        public FileInfo MatchMissing(string torrentFile, int torrentFileNum, string nameInTorrent)
        {
            // returns true if we found a match (if actSetPrio is on, true also means we have set a priority for this file)
            string simplifiedfname = Helpers.SimplifyName(nameInTorrent);

            foreach (ITem action1 in MissingList)
            {
                if ((!(action1 is ItemMissing)) && (!(action1 is ItemuTorrenting)) && (!(action1 is ItemSaBnzbd)))
                    continue;

                ProcessedEpisode m = null;
                string name = null;

                if (action1 is ItemMissing)
                {
                    ItemMissing action = (ItemMissing) (action1);
                    m = action.Episode;
                    name = action.TheFileNoExt;
                }
                else if (action1 is ItemuTorrenting)
                {
                    ItemuTorrenting action = (ItemuTorrenting) (action1);
                    m = action.Episode;
                    name = action.DesiredLocationNoExt;
                }
                else if (action1 is ItemSaBnzbd)
                {
                    ItemSaBnzbd action = (ItemSaBnzbd)(action1);
                    m = action.Episode;
                    name = action.DesiredLocationNoExt;
                }

                if ((m == null) || string.IsNullOrEmpty(name))
                    continue;

                // see if the show name matches...
                if ( FileHelper.SimplifyAndCheckFilename(simplifiedfname, m.TheSeries.Name,false,true))
                {
                    // see if season and episode match
                    int seasF;
                    int epF;
                    if (TVDoc.FindSeasEp("", simplifiedfname, out seasF, out epF, m.Si, Rexps) && (seasF == m.SeasonNumber) && (epF == m.EpNum))
                    {
                        // match!
                        // get extension from nameInTorrent
                        int p = nameInTorrent.LastIndexOf(".");
                        string ext = (p == -1) ? "" : nameInTorrent.Substring(p);
                        AlterResume(torrentFile, torrentFileNum, name + ext);
                        if (SetPrios)
                            SetResumePrio(torrentFile, torrentFileNum, BtPrio.Normal);
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
            Stream s = File.Create(ResumeDatPath);
            ResumeDat.Write(s);
            s.Close();
        }

        public override bool NewTorrentEntry(string torrentFile, int numberInTorrent)
        {
            NewLocation = "";
            PrioWasSet = false;
            Type = "?";
            return true;
        }

        public override bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent, string nameInTorrent)
        {
            NewLocation = onDisk.FullName;
            Type = "Hash";

            AlterResume(torrentFile, numberInTorrent, onDisk.FullName); // make resume.dat point to the file we found

            if (SetPrios)
                SetResumePrio(torrentFile, numberInTorrent, BtPrio.Normal);

            return true;
        }

        public override bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent, string nameInTorrent)
        {
            Type = "Not Found";

            if (SetPrios)
                SetResumePrio(torrentFile, numberInTorrent, BtPrio.Skip);
            return true;
        }

        public override bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename)
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
                SetResumePrio(torrentFile, numberInTorrent, BtPrio.Skip);
                Type = "Not Missing";
            }

            bool prioChanged = SetPrios && PrioWasSet;
            if (prioChanged || (!string.IsNullOrEmpty(NewLocation)))
                AddResult(Type, torrentFile, (numberInTorrent + 1).ToString(), prioChanged ? GetResumePrio(torrentFile, numberInTorrent) : "", NewLocation);
            return true;
        }

        public bool LoadResumeDat()
        {
            BEncodeLoader bel = new BEncodeLoader();
            ResumeDat = bel.Load(ResumeDatPath);
            return (ResumeDat != null);
        }

        public bool DoWork(List<string> torrents, string searchFolder, ListView results, bool hashSearch, bool matchMissing, bool setPrios, bool testMode, 
                           bool searchSubFolders, ItemList missingList, List<FilenameProcessorRe> rexps, CommandLineArgs args)
        {
            Rexps = rexps;

            if (!matchMissing && !hashSearch)
                return true; // nothing to do

            if (hashSearch && string.IsNullOrEmpty(searchFolder))
                return false;

            if (matchMissing && ((missingList == null) || (rexps == null)))
                return false;

            MissingList = missingList;
            DoMatchMissing = matchMissing;
            DoHashChecking = hashSearch;
            SetPrios = setPrios;
            Results = results;

            Prog(0);

            if (!LoadResumeDat())
                return false;

            bool r = true;

            Prog(0);

            if (hashSearch)
                BuildFileCache(searchFolder, searchSubFolders);

            foreach (string tf in torrents)
            {
                r = ProcessTorrentFile(tf, null, args);
                if (!r) // stop on the first failure
                    break;
            }

            if (Altered && !testMode)
                WriteResumeDat();

            Prog(0);

            return r;
        }

        public static string RemoveUt(string s)
        {
            // if it is a .!ut file, we can remove the extension
            if (s.EndsWith(".!ut"))
                return s.Remove(s.Length - 4);
            else
                return s;
        }

        public void AddResult(string type, string torrent, string num, string prio, string location)
        {
            if (Results == null)
                return;

            int p = torrent.LastIndexOf(System.IO.Path.DirectorySeparatorChar.ToString());
            if (p != -1)
                torrent = torrent.Substring(p + 1);
            ListViewItem lvi = new ListViewItem(type);
            lvi.SubItems.Add(torrent);
            lvi.SubItems.Add(num);
            lvi.SubItems.Add(prio);
            lvi.SubItems.Add(RemoveUt(location));

            Results.Items.Add(lvi);
            lvi.EnsureVisible();
            Results.Update();
        }
    }
}
