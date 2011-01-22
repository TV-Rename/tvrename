// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.IO;
using System.Windows.Forms;

// Opens, understands, manipulates, and writes out BEncoded .torrent files, and uTorrent's resume.dat

namespace TVRename
{
    using System.Text.RegularExpressions;

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

    public class TorrentEntry // represents a torrent downloading in uTorrent
    {
        public string DownloadingTo;
        public int PercentDone;
        public string TorrentFile;

        public TorrentEntry(string torrentfile, string to, int percent)
        {
            this.TorrentFile = torrentfile;
            this.DownloadingTo = to;
            this.PercentDone = percent;
        }
    }

    public abstract class BTItem
    {
        public BTChunk Type; // from enum

        protected BTItem(BTChunk type)
        {
            this.Type = type;
        }

        public virtual string AsText()
        {
            return string.Concat("Type =", this.Type.ToString());
        }

        public virtual void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTItem:" + this.Type);
            tn.Add(n);
        }

        public abstract void Write(Stream sw);
    }

    public class BTString : BTItem
    {
        public byte[] Data;

        public BTString(string s)
            : base(BTChunk.kString)
        {
            this.SetString(s);
        }

        public BTString()
            : base(BTChunk.kString)
        {
            this.Data = new byte[0];
        }

        public void SetString(string s)
        {
            this.Data = System.Text.Encoding.UTF8.GetBytes(s);
        }

        public override string AsText()
        {
            return "String=" + this.AsString();
        }

        public string AsString()
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            return enc.GetString(this.Data);
        }

        public byte[] StringTwentyBytePiece(int pieceNum)
        {
            byte[] res = new byte[20];
            if (((pieceNum * 20) + 20) > this.Data.Length)
                return null;
            Array.Copy(this.Data, pieceNum * 20, res, 0, 20);
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
            return CharsToHex(this.Data, pieceNum * 20, 20);
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode(string.Concat("String:", this.AsString()));
            tn.Add(n);
        }

        public override void Write(Stream sw)
        {
            // Byte strings are encoded as follows: <string length encoded in base ten ASCII>:<string data>

            Byte[] len = System.Text.Encoding.ASCII.GetBytes(this.Data.Length.ToString());
            sw.Write(len, 0, len.Length);
            sw.WriteByte((byte) ':');
            sw.Write(this.Data, 0, this.Data.Length);
        }
    }

    public class BTEOF : BTItem
    {
        public BTEOF()
            : base(BTChunk.kBTEOF)
        {
        }

        public override void Write(Stream sw)
        {
        }
    }

    public class BTError : BTItem
    {
        public string Message;

        public BTError()
            : base(BTChunk.kError)
        {
            this.Message = "";
        }

        public override string AsText()
        {
            return string.Concat("Error:", this.Message);
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTError:" + this.Message);
            tn.Add(n);
        }

        public override void Write(Stream sw)
        {
        }
    }

    public class BTListOrDictionaryEnd : BTItem
    {
        public BTListOrDictionaryEnd()
            : base(BTChunk.kListOrDictionaryEnd)
        {
        }

        public override void Write(Stream sw)
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
            this.Key = k;
            this.Data = d;
        }

        public override string AsText()
        {
            if ((this.Key == "pieces") && (this.Data.Type == BTChunk.kString))
                return "<File hash data>";
            return string.Concat(this.Key, "=>", this.Data.AsText());
        }

        public override void Tree(TreeNodeCollection tn)
        {
            if ((this.Key == "pieces") && (this.Data.Type == BTChunk.kString))
            {
                // 20 byte chunks of SHA1 hash values
                TreeNode n = new TreeNode("Key=" + this.Key);
                tn.Add(n);
                n.Nodes.Add(new TreeNode("<File hash data>" + ((BTString) this.Data).PieceAsNiceString(0)));
            }
            else
            {
                TreeNode n = new TreeNode("Key=" + this.Key);
                tn.Add(n);
                this.Data.Tree(n.Nodes);
            }
        }

        public override void Write(Stream sw)
        {
            new BTString(this.Key).Write(sw);
            this.Data.Write(sw);
        }
    }

    public class BTDictionary : BTItem
    {
        public System.Collections.Generic.List<BTDictionaryItem> Items;

        public BTDictionary()
            : base(BTChunk.kDictionary)
        {
            this.Items = new System.Collections.Generic.List<BTDictionaryItem>();
        }

        public override string AsText()
        {
            string r = "Dictionary=[";
            for (int i = 0; i < this.Items.Count; i++)
            {
                r += this.Items[i].AsText();
                if (i != (this.Items.Count - 1))
                    r += ",";
            }
            r += "]";
            return r;
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("Dictionary");
            tn.Add(n);
            for (int i = 0; i < this.Items.Count; i++)
                this.Items[i].Tree(n.Nodes);
        }

        public bool RemoveItem(string key)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Key == key)
                {
                    this.Items.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public BTItem GetItem(string key)
        {
            return this.GetItem(key, false);
        }

        public BTItem GetItem(string key, bool ignoreCase)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if ((this.Items[i].Key == key) || (ignoreCase && ((this.Items[i].Key.ToLower() == key.ToLower()))))
                    return this.Items[i].Data;
            }
            return null;
        }

        public override void Write(Stream sw)
        {
            sw.WriteByte((byte) 'd');
            foreach (BTItem i in this.Items)
                i.Write(sw);
            sw.WriteByte((byte) 'e');
        }
    }

    public class BTList : BTItem
    {
        public System.Collections.Generic.List<BTItem> Items;

        public BTList()
            : base(BTChunk.kList)
        {
            this.Items = new System.Collections.Generic.List<BTItem>();
        }

        public override string AsText()
        {
            string r = "List={";
            for (int i = 0; i < this.Items.Count; i++)
            {
                r += this.Items[i].AsText();
                if (i != (this.Items.Count - 1))
                    r += ",";
            }
            r += "}";
            return r;
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("List");
            tn.Add(n);
            for (int i = 0; i < this.Items.Count; i++)
                this.Items[i].Tree(n.Nodes);
        }

        public override void Write(Stream sw)
        {
            sw.WriteByte((byte) 'l');
            foreach (BTItem i in this.Items)
                i.Write(sw);
            sw.WriteByte((byte) 'e');
        }
    }

    public class BTInteger : BTItem
    {
        public Int64 Value;

        public BTInteger()
            : base(BTChunk.kInteger)
        {
            this.Value = 0;
        }

        public BTInteger(Int64 n)
            : base(BTChunk.kInteger)
        {
            this.Value = n;
        }

        public override string AsText()
        {
            return "Integer=" + this.Value;
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("Integer:" + this.Value);
            tn.Add(n);
        }

        public override void Write(Stream sw)
        {
            sw.WriteByte((byte) 'i');
            byte[] b = System.Text.Encoding.ASCII.GetBytes(this.Value.ToString());
            sw.Write(b, 0, b.Length);
            sw.WriteByte((byte) 'e');
        }
    }

    public class BTFile
    {
        public System.Collections.Generic.List<BTItem> Items;

        public BTFile()
        {
            this.Items = new System.Collections.Generic.List<BTItem>();
        }

        public StringList AllFilesInTorrent()
        {
            StringList r = new StringList();

            BTItem bti = this.GetItem("info");
            if ((bti == null) || (bti.Type != BTChunk.kDictionary))
                return null;

            BTDictionary infoDict = (BTDictionary) (bti);

            bti = infoDict.GetItem("files");

            if (bti == null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti == null) || (bti.Type != BTChunk.kString))
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

        public string AsText()
        {
            string res = "File= ";
            for (int i = 0; i < this.Items.Count; i++)
                res += this.Items[i].AsText() + " ";
            return res;
        }

        public void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BT File");
            tn.Add(n);
            for (int i = 0; i < this.Items.Count; i++)
                this.Items[i].Tree(n.Nodes);
        }

        public BTItem GetItem(string key)
        {
            return this.GetItem(key, false);
        }

        public BTDictionary GetDict()
        {
            System.Diagnostics.Debug.Assert(this.Items.Count == 1);
            System.Diagnostics.Debug.Assert(this.Items[0].Type == BTChunk.kDictionary);

            // our first (and only) Item will be a dictionary of stuff
            return (BTDictionary) (this.Items[0]);
        }

        public BTItem GetItem(string key, bool ignoreCase)
        {
            if (this.Items.Count == 0)
                return null;
            BTDictionary btd = this.GetDict();
            return btd.GetItem(key, ignoreCase);
        }

        public void Write(Stream sw)
        {
            foreach (BTItem i in this.Items)
                i.Write(sw);
        }
    }

    public class HashCacheItem
    {
        public Int64 fileSize;
        public Int64 pieceSize;
        public byte[] theHash;
        public Int64 whereInFile;

        public HashCacheItem(Int64 wif, Int64 ps, Int64 fs, byte[] h)
        {
            this.whereInFile = wif;
            this.pieceSize = ps;
            this.fileSize = fs;
            this.theHash = h;
        }
    }

    public class BEncodeLoader
    {
        private CommandLineArgs Args;
        
        public BEncodeLoader(CommandLineArgs args)
        {
            Args = args;
        }

        public BTItem ReadString(Stream sr, Int64 length)
        {
            BinaryReader br = new BinaryReader(sr);

            byte[] c = br.ReadBytes((int) length);

            BTString bts = new BTString();
            bts.Data = c;
            return bts;
        }

        public BTItem ReadInt(FileStream sr)
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

            BTInteger bti = new BTInteger();
            bti.Value = r;
            return bti;
        }

        public BTItem ReadDictionary(FileStream sr)
        {
            BTDictionary d = new BTDictionary();
            for (;;)
            {
                BTItem next = this.ReadNext(sr);
                if ((next.Type == BTChunk.kListOrDictionaryEnd) || (next.Type == BTChunk.kBTEOF))
                    return d;

                if (next.Type != BTChunk.kString)
                {
                    BTError e = new BTError();
                    e.Message = "Didn't get string as first of pair in dictionary";
                    return e;
                }

                BTDictionaryItem di = new BTDictionaryItem();
                di.Key = ((BTString) next).AsString();
                di.Data = this.ReadNext(sr);

                d.Items.Add(di);
            }
        }

        public BTItem ReadList(FileStream sr)
        {
            BTList ll = new BTList();
            for (;;)
            {
                BTItem next = this.ReadNext(sr);
                if (next.Type == BTChunk.kListOrDictionaryEnd)
                    return ll;

                ll.Items.Add(next);
            }
        }

        public BTItem ReadNext(FileStream sr)
        {
            if (sr.Length == sr.Position)
                return new BTEOF();

            // Read the next character from the stream to see what is next

            int c = sr.ReadByte();
            if (c == 'd')
                return this.ReadDictionary(sr); // dictionary
            if (c == 'l')
                return this.ReadList(sr); // list
            if (c == 'i')
                return this.ReadInt(sr); // integer
            if (c == 'e')
                return new BTListOrDictionaryEnd(); // end of list/dictionary/etc.
            if ((c >= '0') && (c <= '9')) // digits mean it is a string of the specified length
            {
                string r = Convert.ToString(c - '0');
                while ((c = sr.ReadByte()) != ':')
                    r += Convert.ToString(c - '0');
                return this.ReadString(sr, Convert.ToInt32(r));
            }
                
            BTError e = new BTError {
                                        Message = String.Concat("Error: unknown BEncode item type: ", c)
                                    };
            return e;
        }

        public BTFile Load(string filename)
        {
            BTFile f = new BTFile();

            FileStream sr = null;
            try
            {
                sr = new FileStream(filename, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "TVRename Torrent Reader", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            while (sr.Position < sr.Length)
                f.Items.Add(this.ReadNext(sr));

            sr.Close();

            return f;
        }
    }

    public abstract class BTCore
    {
        protected int CacheChecks;
        protected int CacheHits;
        protected int CacheItems;
        protected bool DoHashChecking;
        protected DirCache FileCache;
        protected string FileCacheIsFor;
        protected bool FileCacheWithSubFolders;
        protected System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<HashCacheItem>> HashCache;
        protected SetProgressDelegate SetProg;

        protected BTCore(SetProgressDelegate setprog)
        {
            this.SetProg = setprog;

            this.HashCache = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<HashCacheItem>>();
            this.CacheChecks = this.CacheItems = this.CacheHits = 0;
            this.FileCache = null;
            this.FileCacheIsFor = null;
            this.FileCacheWithSubFolders = false;
        }

        protected void Prog(int percent)
        {
            if (this.SetProg != null)
                this.SetProg.Invoke(percent);
        }

        public abstract bool NewTorrentEntry(string torrentFile, int numberInTorrent);

        public abstract bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent, string nameInTorrent);

        public abstract bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent, string nameInTorrent);

        public abstract bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename);

        public FileInfo FindLocalFileWithHashAt(byte[] findMe, Int64 whereInFile, Int64 pieceSize, Int64 fileSize)
        {
            if (whereInFile < 0)
                return null;

            foreach (DirCacheEntry dc in this.FileCache)
                //for (int i = 0; i < FileCache.Cache.Count; i++)
            {
                FileInfo fiTemp = dc.TheFile;
                Int64 flen = dc.Length;

                if ((flen != fileSize) || (flen < (whereInFile + pieceSize))) // this file is wrong size || too small
                    continue;

                byte[] theHash = this.CheckCache(fiTemp.FullName, whereInFile, pieceSize, fileSize);
                if (theHash == null)
                {
                    // not cached, figure it out ourselves
                    FileStream sr = null;
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
                    this.CacheThis(fiTemp.FullName, whereInFile, pieceSize, fileSize, theHash);
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
            this.CacheItems++;
            if (!this.HashCache.ContainsKey(filename))
                this.HashCache[filename] = new System.Collections.Generic.List<HashCacheItem>();
            this.HashCache[filename].Add(new HashCacheItem(whereInFile, piecesize, fileSize, hash));
        }

        protected byte[] CheckCache(string filename, Int64 whereInFile, Int64 piecesize, Int64 fileSize)
        {
            this.CacheChecks++;
            if (this.HashCache.ContainsKey(filename))
            {
                foreach (HashCacheItem h in this.HashCache[filename])
                {
                    if ((h.whereInFile == whereInFile) && (h.pieceSize == piecesize) && (h.fileSize == fileSize))
                    {
                        this.CacheHits++;
                        return h.theHash;
                    }
                }
            }
            return null;
        }

        protected void BuildFileCache(string folder, bool subFolders)
        {
            if ((this.FileCache == null) || (this.FileCacheIsFor == null) || (this.FileCacheIsFor != folder) || (this.FileCacheWithSubFolders != subFolders))
            {
                this.FileCache = new DirCache(null, folder, subFolders, null);
                this.FileCacheIsFor = folder;
                this.FileCacheWithSubFolders = subFolders;
            }
        }

        public bool ProcessTorrentFile(string torrentFile, TreeView tvTree, CommandLineArgs args)
        {
            // ----------------------------------------
            // read in torrent file

            if (tvTree != null)
                tvTree.Nodes.Clear();

            BEncodeLoader bel = new BEncodeLoader(args);
            BTFile btFile = bel.Load(torrentFile);

            if (btFile == null)
                return false;

            BTItem bti = btFile.GetItem("info");
            if ((bti == null) || (bti.Type != BTChunk.kDictionary))
                return false;

            BTDictionary infoDict = (BTDictionary) (bti);

            bti = infoDict.GetItem("piece length");
            if ((bti == null) || (bti.Type != BTChunk.kInteger))
                return false;

            Int64 pieceSize = ((BTInteger) bti).Value;

            bti = infoDict.GetItem("pieces");
            if ((bti == null) || (bti.Type != BTChunk.kString))
                return false;

            BTString torrentPieces = (BTString) (bti);

            bti = infoDict.GetItem("files");

            if (bti == null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti == null) || (bti.Type != BTChunk.kString))
                    return false;

                BTString di = (BTString) (bti);
                string nameInTorrent = di.AsString();

                BTItem fileSizeI = infoDict.GetItem("length");
                Int64 fileSize = ((BTInteger) fileSizeI).Value;

                this.NewTorrentEntry(torrentFile, -1);
                if (this.DoHashChecking)
                {
                    byte[] torrentPieceHash = torrentPieces.StringTwentyBytePiece(0);

                    FileInfo fi = this.FindLocalFileWithHashAt(torrentPieceHash, 0, pieceSize, fileSize);
                    if (fi != null)
                        this.FoundFileOnDiskForFileInTorrent(torrentFile, fi, -1, nameInTorrent);
                    else
                        this.DidNotFindFileOnDiskForFileInTorrent(torrentFile, -1, nameInTorrent);
                }
                this.FinishedTorrentEntry(torrentFile, -1, nameInTorrent);

                // don't worry about updating overallPosition as this is the only file in the torrent
            }
            else
            {
                Int64 overallPosition = 0;
                Int64 lastPieceLeftover = 0;

                if (bti.Type != BTChunk.kList)
                    return false;

                BTList fileList = (BTList) (bti);

                // list of dictionaries
                for (int i = 0; i < fileList.Items.Count; i++)
                {
                    this.Prog(100 * i / fileList.Items.Count);
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
                    Int64 fileSize = ((BTInteger) fileSizeI).Value;

                    int pieceNum = (int) (overallPosition / pieceSize);
                    if (overallPosition % pieceSize != 0)
                        pieceNum++;

                    this.NewTorrentEntry(torrentFile, i);

                    if (this.DoHashChecking)
                    {
                        byte[] torrentPieceHash = torrentPieces.StringTwentyBytePiece(pieceNum);

                        FileInfo fi = this.FindLocalFileWithHashAt(torrentPieceHash, lastPieceLeftover, pieceSize, fileSize);
                        if (fi != null)
                            this.FoundFileOnDiskForFileInTorrent(torrentFile, fi, i, fileName.AsString());
                        else
                            this.DidNotFindFileOnDiskForFileInTorrent(torrentFile, i, fileName.AsString());
                    }

                    this.FinishedTorrentEntry(torrentFile, i, fileName.AsString());

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

            this.Prog(0);

            return true;
        }
    }

    // btcore

    public class BTFileRenamer : BTCore
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
        
        public BTFileRenamer(SetProgressDelegate setprog)
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
            this.RenameListOut.Add(new ActionCopyMoveRename(this.CopyNotMove ? ActionCopyMoveRename.Op.Copy : ActionCopyMoveRename.Op.Rename, onDisk, Helpers.FileInFolder(this.CopyNotMove ? this.CopyToFolder : onDisk.Directory.Name, nameInTorrent), null));

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
            string r = "Hash Cache: " + this.CacheItems + " items for " + this.HashCache.Count + " files.  " + this.CacheHits + " hits from " + this.CacheChecks + " lookups";
            if (this.CacheChecks != 0)
                r += " (" + (100 * this.CacheHits / this.CacheChecks) + "%)";
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

            this.CopyNotMove = copyNotMove;
            this.CopyToFolder = copyDest;
            this.DoHashChecking = true;
            this.RenameListOut = renameListOut;

            this.Prog(0);

            this.BuildFileCache(folder, false); // don't do subfolders

            this.RenameListOut.Clear();

            bool r = this.ProcessTorrentFile(torrentFile, tvTree, args);

            return r;
        }
    }

    // BTProcessor

    public class BTResume : BTCore
    {
        private static class BTPrio { public const int Normal = 0x08, Skip = 0x80; }

        public bool Altered;
        public bool DoMatchMissing;
        public bool HashSearch;
        public ItemList MissingList;

        public string NewLocation;
        public bool PrioWasSet;
        public ListView Results;

        public BTFile ResumeDat; // resume file, if we're using it
        public string ResumeDatPath;

        public FNPRegexList Rexps; // used by MatchMissing
        public bool SearchSubFolders;
        public bool SetPrios;
        public bool TestMode;
        public string Type;

        public BTResume(SetProgressDelegate setprog, string resumeDatFile)
            : base(setprog)
        {
            this.ResumeDatPath = resumeDatFile;
        }

        public BTDictionary GetTorrentDict(string torrentFile)
        {
            // find dictionary for the specified torrent file

            BTItem it = this.ResumeDat.GetDict().GetItem(torrentFile, true);
            if ((it == null) || (it.Type != BTChunk.kDictionary))
                return null;
            BTDictionary dict = (BTDictionary) (it);
            return dict;
        }

        public static int PercentBitsOn(BTString s)
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

        public System.Collections.Generic.List<TorrentEntry> AllFilesBeingDownloaded(TVSettings settings, CommandLineArgs args)
        {
            System.Collections.Generic.List<TorrentEntry> r = new System.Collections.Generic.List<TorrentEntry>();

            BEncodeLoader bel = new BEncodeLoader(args);
            foreach (BTItem it in this.ResumeDat.GetDict().Items)
            {
                if ((it.Type != BTChunk.kDictionaryItem))
                    continue;

                BTDictionaryItem dictitem = (BTDictionaryItem) (it);

                if ((dictitem.Key == ".fileguard") || (dictitem.Data.Type != BTChunk.kDictionary))
                    continue;

                string torrentFile = dictitem.Key;
                BTDictionary d2 = (BTDictionary) (dictitem.Data);

                BTItem p = d2.GetItem("prio");
                if ((p == null) || (p.Type != BTChunk.kString))
                    continue;

                BTString prioString = (BTString) (p);
                string directoryName = Path.GetDirectoryName(this.ResumeDatPath) + System.IO.Path.DirectorySeparatorChar;

                if (!File.Exists(torrentFile)) // if the torrent file doesn't exist
                    torrentFile = directoryName + torrentFile; // ..try prepending the resume.dat folder's path to it.

                if (!File.Exists(torrentFile))
                    continue; // can't find it.  give up!
                
                BTFile tor = bel.Load(torrentFile);
                if (tor == null)
                    continue;

                StringList a = tor.AllFilesInTorrent();
                if (a != null)
                {
                    int c = 0;

                    p = d2.GetItem("path");
                    if ((p == null) || (p.Type != BTChunk.kString))
                        continue;
                    string defaultFolder = ((BTString) p).AsString();

                    BTItem targets = d2.GetItem("targets");
                    bool hasTargets = ((targets != null) && (targets.Type == BTChunk.kList));
                    BTList targetList = (BTList) (targets);

                    foreach (string s in a)
                    {
                        if ((c < prioString.Data.Length) && (prioString.Data[c] != BTPrio.Skip))
                        {
                            string saveTo = Helpers.FileInFolder(defaultFolder, settings.FilenameFriendly(s)).Name;
                            if (hasTargets)
                            {
                                // see if there is a target for this (the c'th) file
                                for (int i = 0; i < targetList.Items.Count; i++)
                                {
                                    BTList l = (BTList) (targetList.Items[i]);
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
                            TorrentEntry te = new TorrentEntry(torrentFile, saveTo, percent);
                            r.Add(te);
                        }
                        c++;
                    }
                }
            }

            return r;
        }

        public string GetResumePrio(string torrentFile, int fileNum)
        {
            BTDictionary dict = this.GetTorrentDict(torrentFile);
            if (dict == null)
                return "";
            BTItem p = dict.GetItem("prio");
            if ((p == null) || (p.Type != BTChunk.kString))
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

        public void SetResumePrio(string torrentFile, int fileNum, byte newPrio)
        {
            if (!this.SetPrios)
                return;

            if (fileNum == -1)
                fileNum = 0;
            BTDictionary dict = this.GetTorrentDict(torrentFile);
            if (dict == null)
                return;
            BTItem p = dict.GetItem("prio");
            if ((p == null) || (p.Type != BTChunk.kString))
                return;
            BTString prioString = (BTString) (p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
                return;

            this.Altered = true;
            this.PrioWasSet = true;

            prioString.Data[fileNum] = newPrio;

            string ps;
            if (newPrio == BTPrio.Skip)
                ps = "Skip";
            else if (newPrio == BTPrio.Normal)
                ps = "Normal";
            else
                ps = newPrio.ToString();
        }

        public void AlterResume(string torrentFile, int fileNum, string toHere)
        {
            toHere = RemoveUT(toHere);

            BTDictionary dict = this.GetTorrentDict(torrentFile);
            if (dict == null)
                return;

            this.Altered = true;

            if (fileNum == -1) // single file torrent
            {
                BTItem p = dict.GetItem("path");
                if (p == null)
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
                if (p == null)
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
                if (theList == null)
                    return;

                // the list contains two element lists, of integer/string which are filenumber/path

                BTList thisFileList = null;
                // see if this file is already in the list
                for (int i = 0; i < theList.Items.Count; i++)
                {
                    if (theList.Items[i].Type != BTChunk.kList)
                        return;

                    BTList l2 = (BTList) (theList.Items[i]);
                    if ((l2.Items.Count != 2) || (l2.Items[0].Type != BTChunk.kInteger) || (l2.Items[1].Type != BTChunk.kString))
                        return;
                    int n = (int) ((BTInteger) (l2.Items[0])).Value;
                    if (n == fileNum)
                    {
                        thisFileList = l2;
                        break;
                    }
                }
                if (thisFileList == null) // didn't find it
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
            this.ResumeDat.GetDict().RemoveItem(".fileguard");
            MemoryStream ms = new MemoryStream();
            this.ResumeDat.Write(ms);
            System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();
            byte[] theHash = sha1.ComputeHash(ms.GetBuffer(), 0, (int) ms.Length);
            ms.Close();
            string newfg = BTString.CharsToHex(theHash, 0, 20);
            this.ResumeDat.GetDict().Items.Add(new BTDictionaryItem(".fileguard", new BTString(newfg)));
        }

        public FileInfo MatchMissing(string torrentFile, int torrentFileNum, string nameInTorrent)
        {
            // returns true if we found a match (if actSetPrio is on, true also means we have set a priority for this file)
            string simplifiedfname = Helpers.SimplifyName(nameInTorrent);

            foreach (Item Action1 in this.MissingList)
            {
                if ((!(Action1 is ItemMissing)) && (!(Action1 is ItemuTorrenting)))
                    continue;

                ProcessedEpisode m = null;
                string name = null;

                if (Action1 is ItemMissing)
                {
                    ItemMissing Action = (ItemMissing) (Action1);
                    m = Action.Episode;
                    name = Action.TheFileNoExt;
                }
                else if (Action1 is ItemuTorrenting)
                {
                    ItemuTorrenting Action = (ItemuTorrenting) (Action1);
                    m = Action.Episode;
                    name = Action.DesiredLocationNoExt;
                }

                if ((m == null) || string.IsNullOrEmpty(name))
                    continue;

                // see if the show name matches...
                if (Regex.Match(simplifiedfname, "\\b" + m.TheSeries.Name + "\\b", RegexOptions.IgnoreCase).Success)
                {
                    // see if season and episode match
                    int seasF;
                    int epF;
                    if (TVDoc.FindSeasEp("", simplifiedfname, out seasF, out epF, m.SI, this.Rexps) && (seasF == m.SeasonNumber) && (epF == m.EpNum))
                    {
                        // match!
                        // get extension from nameInTorrent
                        int p = nameInTorrent.LastIndexOf(".");
                        string ext = (p == -1) ? "" : nameInTorrent.Substring(p);
                        this.AlterResume(torrentFile, torrentFileNum, name + ext);
                        if (this.SetPrios)
                            this.SetResumePrio(torrentFile, torrentFileNum, BTPrio.Normal);
                        return new FileInfo(name + ext);
                    }
                }
            }
            return null;
        }

        public void WriteResumeDat()
        {
            this.FixFileguard();
            // write out new resume.dat file
            string to = this.ResumeDatPath + ".before_tvrename";
            if (File.Exists(to))
                File.Delete(to);
            File.Move(this.ResumeDatPath, to);
            Stream s = File.Create(this.ResumeDatPath);
            this.ResumeDat.Write(s);
            s.Close();
        }

        public override bool NewTorrentEntry(string torrentFile, int numberInTorrent)
        {
            this.NewLocation = "";
            this.PrioWasSet = false;
            this.Type = "?";
            return true;
        }

        public override bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent, string nameInTorrent)
        {
            this.NewLocation = onDisk.FullName;
            this.Type = "Hash";

            this.AlterResume(torrentFile, numberInTorrent, onDisk.FullName); // make resume.dat point to the file we found

            if (this.SetPrios)
                this.SetResumePrio(torrentFile, numberInTorrent, BTPrio.Normal);

            return true;
        }

        public override bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent, string nameInTorrent)
        {
            this.Type = "Not Found";

            if (this.SetPrios)
                this.SetResumePrio(torrentFile, numberInTorrent, BTPrio.Skip);
            return true;
        }

        public override bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename)
        {
            if (this.DoMatchMissing)
            {
                FileInfo s = this.MatchMissing(torrentFile, numberInTorrent, filename);
                if (s != null)
                {
                    this.PrioWasSet = true;
                    this.NewLocation = s.FullName;
                    this.Type = "Missing";
                }
            }

            if (this.SetPrios && !this.PrioWasSet)
            {
                this.SetResumePrio(torrentFile, numberInTorrent, BTPrio.Skip);
                this.Type = "Not Missing";
            }

            bool prioChanged = this.SetPrios && this.PrioWasSet;
            if (prioChanged || (!string.IsNullOrEmpty(this.NewLocation)))
                this.AddResult(this.Type, torrentFile, (numberInTorrent + 1).ToString(), prioChanged ? this.GetResumePrio(torrentFile, numberInTorrent) : "", this.NewLocation);
            return true;
        }

        public bool LoadResumeDat(CommandLineArgs args)
        {
            BEncodeLoader bel = new BEncodeLoader(args);
            this.ResumeDat = bel.Load(this.ResumeDatPath);
            return (this.ResumeDat != null);
        }

        public bool DoWork(StringList Torrents, string searchFolder, ListView results, bool hashSearch, bool matchMissing, bool setPrios, bool testMode, 
                           bool searchSubFolders, ItemList missingList, FNPRegexList rexps, CommandLineArgs args)
        {
            this.Rexps = rexps;

            if (!matchMissing && !hashSearch)
                return true; // nothing to do

            if (hashSearch && string.IsNullOrEmpty(searchFolder))
                return false;

            if (matchMissing && ((missingList == null) || (rexps == null)))
                return false;

            this.MissingList = missingList;
            this.DoMatchMissing = matchMissing;
            this.DoHashChecking = hashSearch;
            this.SetPrios = setPrios;
            this.Results = results;

            this.Prog(0);

            if (!this.LoadResumeDat(args))
                return false;

            bool r = true;

            this.Prog(0);

            if (hashSearch)
                this.BuildFileCache(searchFolder, searchSubFolders);

            foreach (string tf in Torrents)
            {
                r = this.ProcessTorrentFile(tf, null, args);
                if (!r) // stop on the first failure
                    break;
            }

            if (this.Altered && !testMode)
                this.WriteResumeDat();

            this.Prog(0);

            return r;
        }

        public static string RemoveUT(string s)
        {
            // if it is a .!ut file, we can remove the extension
            if (s.EndsWith(".!ut"))
                return s.Remove(s.Length - 4);
            else
                return s;
        }

        public void AddResult(string type, string torrent, string num, string prio, string location)
        {
            if (this.Results == null)
                return;

            int p = torrent.LastIndexOf(System.IO.Path.DirectorySeparatorChar.ToString());
            if (p != -1)
                torrent = torrent.Substring(p + 1);
            ListViewItem lvi = new ListViewItem(type);
            lvi.SubItems.Add(torrent);
            lvi.SubItems.Add(num);
            lvi.SubItems.Add(prio);
            lvi.SubItems.Add(RemoveUT(location));

            this.Results.Items.Add(lvi);
            lvi.EnsureVisible();
            this.Results.Update();
        }
    }
}
