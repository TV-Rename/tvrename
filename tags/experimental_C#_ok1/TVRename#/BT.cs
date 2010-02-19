//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;


namespace TVRename
{

    public enum BTChunk : int
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

    public abstract class BTItem
    {
        public BTChunk Type; // from enum

        protected BTItem(BTChunk type)
        {
            Type = type;
        }

        public virtual string AsText()
        {
            return string.Concat("Type =", Type.ToString());
        }

        public virtual void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTItem:" + Type.ToString());
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
            string r = "";
            for (int i = 0; i < n; i++)
                r += (data[start + i] < 16 ? "0" : "") + data[start + i].ToString("x").ToUpper();
            return r;
        }


        public string PieceAsNiceString(int pieceNum)
        {
            return CharsToHex(Data, pieceNum * 20, 20);
            //                String ^r = gc"";
            //				for (int i=0;i<20;i++,p++)
            //				r += (Data[p]<16?"0":"") + Data[p].ToString("x")->ToUpper();
            //
            //				return r;
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
            sw.WriteByte((byte)':');
            sw.Write(Data, 0, Data.Length);
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

    public class BTListOrDictionaryEnd : BTItem
    {
        public BTListOrDictionaryEnd()
            : base(BTChunk.kListOrDictionaryEnd)
        {
        }
        public override void Write(Stream sw)
        {
            sw.WriteByte((byte)'e');
        }

    }

    public class BTDictionaryItem : BTItem
    {
        public string Key;
        public BTItem Data;

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
            {
                return "<File hash data>";
            }
            else
                return string.Concat(Key, "=>", Data.AsText());
        }

        public override void Tree(TreeNodeCollection tn)
        {
            if ((Key == "pieces") && (Data.Type == BTChunk.kString))
            {
                // 20 byte chunks of SHA1 hash values
                TreeNode n = new TreeNode("Key=" + Key);
                tn.Add(n);
                n.Nodes.Add(new TreeNode("<File hash data>" + ((BTString)Data).PieceAsNiceString(0)));
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
            new BTString(Key).Write(sw);
            Data.Write(sw);
        }
    }

    public class BTDictionary : BTItem
    {
        public System.Collections.Generic.List<BTDictionaryItem> Items;

        public BTDictionary()
            : base(BTChunk.kDictionary)
        {
            Items = new System.Collections.Generic.List<BTDictionaryItem>();
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
        public BTItem GetItem(string key)
        {
            return GetItem(key, false);
        }
        public BTItem GetItem(string key, bool ignoreCase)
        {
            for (int i = 0; i < Items.Count; i++)
                if ((Items[i].Key == key) || (ignoreCase && ((Items[i].Key.ToLower() == key.ToLower()))))
                    return Items[i].Data;
            return null;
        }
        public override void Write(Stream sw)
        {
            sw.WriteByte((byte)'d');
            foreach (BTItem i in Items)
                i.Write(sw);
            sw.WriteByte((byte)'e');
        }

    }

    public class BTList : BTItem
    {
        public System.Collections.Generic.List<BTItem> Items;

        public BTList()
            : base(BTChunk.kList)
        {
            Items = new System.Collections.Generic.List<BTItem>();
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
            sw.WriteByte((byte)'l');
            foreach (BTItem i in Items)
                i.Write(sw);
            sw.WriteByte((byte)'e');
        }
    }

    public class BTInteger : BTItem
    {
        public Int64 Value;

        public BTInteger()
            : base(BTChunk.kInteger)
        {
            Value = 0;
        }

        public BTInteger(Int64 n)
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
            TreeNode n = new TreeNode("Integer:" + Value.ToString());
            tn.Add(n);
        }
        public override void Write(Stream sw)
        {
            sw.WriteByte((byte)'i');
            byte[] b = System.Text.Encoding.ASCII.GetBytes(Value.ToString());
            sw.Write(b, 0, b.Length);
            sw.WriteByte((byte)'e');
        }
    }

    public class BTFile
    {
        public System.Collections.Generic.List<BTItem> Items;

        public BTFile()
        {
            Items = new System.Collections.Generic.List<BTItem>();
        }

        public StringList AllFilesInTorrent()
        {
            StringList r = new StringList();

            BTItem bti = GetItem("info");
            if ((bti == null) || (bti.Type != BTChunk.kDictionary))
                return null;

            BTDictionary infoDict = (BTDictionary)(bti);

            bti = infoDict.GetItem("files");

            if (bti == null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti == null) || (bti.Type != BTChunk.kString))
                    return null;
                r.Add(((BTString)bti).AsString());
            }
            else
            { // multiple file torrent
                BTList fileList = (BTList)(bti);

                foreach (BTItem it in fileList.Items)
                {
                    BTDictionary file = (BTDictionary)(it);
                    BTItem thePath = file.GetItem("path");
                    if (thePath.Type != BTChunk.kList)
                        return null;
                    BTList pathList = (BTList)(thePath);
                    // want the last of the items in the list, which is the filename itself
                    int n = pathList.Items.Count - 1;
                    if (n < 0)
                        return null;
                    BTString fileName = (BTString)(pathList.Items[n]);
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

        public BTItem GetItem(string key)
        {
            return GetItem(key, false);
        }

        public BTDictionary GetDict()
        {
            System.Diagnostics.Debug.Assert(Items.Count == 1);
            System.Diagnostics.Debug.Assert(Items[0].Type == BTChunk.kDictionary);

            // our first (and only) Item will be a dictionary of stuff
            return (BTDictionary)(Items[0]);
        }

        public BTItem GetItem(string key, bool ignoreCase)
        {
            if (Items.Count == 0)
                return null;
            BTDictionary btd = GetDict();
            return btd.GetItem(key, ignoreCase);
        }

        public void Write(Stream sw)
        {
            foreach (BTItem i in Items)
                i.Write(sw);
        }

    }

    public class HashCacheItem
    {
        public Int64 whereInFile;
        public Int64 pieceSize;
        public Int64 fileSize;
        public byte[] theHash;

        public HashCacheItem(Int64 wif, Int64 ps, Int64 fs, byte[] h)
        {
            whereInFile = wif;
            pieceSize = ps;
            fileSize = fs;
            theHash = h;
        }

    }


    public class BEncodeLoader
    {
        public BEncodeLoader()
        {
        }
        public BTItem ReadString(Stream sr, Int64 length)
        {
            BinaryReader br = new BinaryReader(sr);

            byte[] c = br.ReadBytes((int)length);

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
                else
                    if ((c >= '0') && (c <= '9'))
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
            for (; ; )
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

                BTDictionaryItem di = new BTDictionaryItem();
                di.Key = ((BTString)next).AsString();
                di.Data = ReadNext(sr);

                d.Items.Add(di);
            }
        }
        public BTItem ReadList(FileStream sr)
        {
            BTList ll = new BTList();
            for (; ; )
            {
                BTItem next = ReadNext(sr);
                if (next.Type == BTChunk.kListOrDictionaryEnd)
                    return ll;

                ll.Items.Add(next);
            }
        }
        public BTItem ReadNext(FileStream sr)
        {
            if (sr.Length == sr.Position)
                return new BTEOF();

            string t = NextThing(sr);

            if (t == "d")
            {
                return ReadDictionary(sr);
            }
            else if (t == "l")
            {
                return ReadList(sr);
            }
            else if (t == "e")
            {
                return new BTListOrDictionaryEnd();
            }
            else if (t[0] == 's')
            {
                return ReadString(sr, Convert.ToInt32(t.Substring(1)));
            }
            else if (t[0] == 'i')
            {
                return ReadInt(sr);
            }
            else
            {
                BTError e = new BTError();
                e.Message = string.Concat("Error: unknown thing: ", t);
                return e;
            }
        }
        public string NextThing(FileStream sr)
        {
            int c = sr.ReadByte();
            if (c == 'd')
                return "d"; // dictionary
            if (c == 'l')
                return "l"; // list
            if (c == 'i')
                return "i"; // integer
            if (c == 'e')
                return "e"; // end of list/dictionary/etc.
            if ((c >= '0') && (c <= '9'))
            {
                string r = Convert.ToString(c - '0');
                while ((c = sr.ReadByte()) != ':')
                    r += Convert.ToString(c - '0');
                return "s" + r;
            }
            return "?";
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
                f.Items.Add(ReadNext(sr));

            sr.Close();

            return f;
        }

    }

    public abstract class BTCore
    {
        protected SetProgressDelegate SetProg;
        protected System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<HashCacheItem>> HashCache;
        protected int CacheChecks;
        protected int CacheItems;
        protected int CacheHits;
        protected string FileCacheIsFor;
        protected bool FileCacheWithSubFolders;
        protected DirCacheList FileCache;
        protected bool DoHashChecking;


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

            for (int i = 0; i < FileCache.Count; i++)
            {
                FileInfo fiTemp = FileCache[i].TheFile;
                Int64 flen = FileCache[i].Length;

                if ((flen != fileSize) || (flen < (whereInFile + pieceSize))) // this file is wrong size || too small
                    continue;

                byte[] theHash = CheckCache(fiTemp.FullName, whereInFile, pieceSize, fileSize);
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
                    int n = sr.Read(thePiece, 0, (int)pieceSize);
                    sr.Close();

                    System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();

                    theHash = sha1.ComputeHash(thePiece, 0, n);
                    CacheThis(fiTemp.FullName, whereInFile, pieceSize, fileSize, theHash);
                }

                bool allGood = true;
                for (int j = 0; j < 20; j++)
                    if (theHash[j] != findMe[j])
                    {
                        allGood = false;
                        break;
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
                HashCache[filename] = new System.Collections.Generic.List<HashCacheItem>();
            HashCache[filename].Add(new HashCacheItem(whereInFile, piecesize, fileSize, hash));
        }

        protected byte[] CheckCache(string filename, Int64 whereInFile, Int64 piecesize, Int64 fileSize)
        {
            CacheChecks++;
            if (HashCache.ContainsKey(filename))
            {
                foreach (HashCacheItem h in HashCache[filename])
                    if ((h.whereInFile == whereInFile) && (h.pieceSize == piecesize) && (h.fileSize == fileSize))
                    {
                        CacheHits++;
                        return h.theHash;
                    }
            }
            return null;
        }

        protected void BuildFileCache(string folder, bool subFolders)
        {
            if ((FileCache == null) || (FileCacheIsFor == null) || (FileCacheIsFor != folder) || (FileCacheWithSubFolders != subFolders))
            {
                FileCache = new DirCacheList();
                DirCache.BuildDirCache(null, 0, 0, FileCache, folder, subFolders, null);
                FileCacheIsFor = folder;
                FileCacheWithSubFolders = subFolders;

            }
        }

        public bool ProcessTorrentFile(string torrentFile, TreeView tvTree)
        {
            // ----------------------------------------
            // read in torrent file

            if (tvTree != null)
                tvTree.Nodes.Clear();

            BEncodeLoader bel = new BEncodeLoader();
            BTFile btFile = bel.Load(torrentFile);

            if (btFile == null)
                return false;

            BTItem bti = btFile.GetItem("info");
            if ((bti == null) || (bti.Type != BTChunk.kDictionary))
                return false;

            BTDictionary infoDict = (BTDictionary)(bti);

            bti = infoDict.GetItem("piece length");
            if ((bti == null) || (bti.Type != BTChunk.kInteger))
                return false;

            Int64 pieceSize = ((BTInteger)bti).Value;

            bti = infoDict.GetItem("pieces");
            if ((bti == null) || (bti.Type != BTChunk.kString))
                return false;

            BTString torrentPieces = (BTString)(bti);

            bti = infoDict.GetItem("files");

            if (bti == null) // single file torrent
            {
                bti = infoDict.GetItem("name");
                if ((bti == null) || (bti.Type != BTChunk.kString))
                    return false;

                BTString di = (BTString)(bti);
                string nameInTorrent = di.AsString();

                BTItem fileSizeI = infoDict.GetItem("length");
                Int64 fileSize = ((BTInteger)fileSizeI).Value;

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

                if (bti.Type != BTChunk.kList)
                    return false;

                BTList fileList = (BTList)(bti);

                // list of dictionaries
                for (int i = 0; i < fileList.Items.Count; i++)
                {
                    Prog(100 * i / fileList.Items.Count);
                    if (fileList.Items[i].Type != BTChunk.kDictionary)
                        return false;

                    BTDictionary file = (BTDictionary)(fileList.Items[i]);
                    BTItem thePath = file.GetItem("path");
                    if (thePath.Type != BTChunk.kList)
                        return false;
                    BTList pathList = (BTList)(thePath);
                    // want the last of the items in the list, which is the filename itself
                    int n = pathList.Items.Count - 1;
                    if (n < 0)
                        return false;
                    BTString fileName = (BTString)(pathList.Items[n]);

                    BTItem fileSizeI = file.GetItem("length");
                    Int64 fileSize = ((BTInteger)fileSizeI).Value;

                    int pieceNum = (int)(overallPosition / pieceSize);
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

                    int sizeInPieces = (int)(fileSize / pieceSize);
                    if (fileSize % pieceSize != 0)
                        sizeInPieces++; // another partial piece

                    lastPieceLeftover = (lastPieceLeftover + (Int32)((sizeInPieces * pieceSize) - fileSize)) % pieceSize;
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

        protected BTCore(SetProgressDelegate setprog)
        {
            SetProg = setprog;

            HashCache = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<HashCacheItem>>();
            CacheChecks = CacheItems = CacheHits = 0;
            FileCache = null;
            FileCacheIsFor = null;
            FileCacheWithSubFolders = false;
        }
    } // btcore

    public class BTFileRenamer : BTCore
    {
        // Hash and file caches

        // settings for processing the torrent (and optionally resume) files
        //    int Action;
        //    String ^torrentFile;
        //    String ^folder;
        public TreeView tvTree;
        //TextBox ^status;

        public bool CopyNotMove;
        public string CopyToFolder;

        public System.Collections.Generic.List<AIOItem> RenameListOut;

        //    String ^secondFolder; // resume.dat location, or where to copy/move to

        override public bool NewTorrentEntry(string torrentFile, int numberInTorrent)
        {
            return true;
        }
        override public bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent, string nameInTorrent)
        {
            RenameListOut.Add(new AIOCopyMoveRename(CopyNotMove ? AIOCopyMoveRename.Op.Copy : AIOCopyMoveRename.Op.Rename, onDisk, Helpers.FileInFolder(CopyNotMove ? CopyToFolder : onDisk.Directory.Name, nameInTorrent), null));

            return true;
        }
        override public bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent, string nameInTorrent)
        {
            return true;
        }
        override public bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename)
        {
            return true;
        }

        public BTFileRenamer(SetProgressDelegate setprog)
            : base(setprog)
        {
        }
        public string CacheStats()
        {
            string r = "Hash Cache: " + CacheItems + " items for " + HashCache.Count + " files.  " + CacheHits + " hits from " + CacheChecks + " lookups";
            if (CacheChecks != 0)
                r += " (" + (100 * CacheHits / CacheChecks) + "%)";
            return r;
        }

        public bool RenameFilesOnDiskToMatchTorrent(string torrentFile, string folder, TreeView tvTree, System.Collections.Generic.List<AIOItem> renameListOut, SetProgressDelegate _prog, bool copyNotMove, string copyDest)
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

            bool r = ProcessTorrentFile(torrentFile, tvTree);

            return r;
        }
        // bool Go();
    } // BTProcessor


    public class BTResume : BTCore
    {
        class BTPrio
        {
            public const int Normal = 8;
            public const int Skip = 0x80;
        }

        public bool HashSearch;
        public bool TestMode;
        public bool DoMatchMissing;
        public bool SetPrios;
        public bool SearchSubFolders;
        public ListView Results;

        public string NewLocation;
        public string Type;
        public bool PrioWasSet;

        public BTFile ResumeDat; // resume file, if we're using it
        public string ResumeDatPath;

        public bool Altered;
        public FNPRegexList Rexps; // used by MatchMissing

        public System.Collections.Generic.List<AIOItem> MissingList;

        public BTDictionary GetTorrentDict(string torrentFile)
        {
            // find dictionary for the specified torrent file

            BTItem it = ResumeDat.GetDict().GetItem(torrentFile, true);
            if ((it == null) || (it.Type != BTChunk.kDictionary))
                return null;
            BTDictionary dict = (BTDictionary)(it);
            return dict;
        }
        public int PercentBitsOn(BTString s)
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
        public System.Collections.Generic.List<TorrentEntry> AllFilesBeingDownloaded()
        {
            System.Collections.Generic.List<TorrentEntry> r = new System.Collections.Generic.List<TorrentEntry>();

            BEncodeLoader bel = new BEncodeLoader();
            foreach (BTItem it in ResumeDat.GetDict().Items)
            {
                if ((it.Type != BTChunk.kDictionaryItem))
                    continue;

                BTDictionaryItem dictitem = (BTDictionaryItem)(it);

                if ((dictitem.Key == ".fileguard") || (dictitem.Data.Type != BTChunk.kDictionary))
                    continue;

                string torrentFile = dictitem.Key;
                BTDictionary d2 = (BTDictionary)(dictitem.Data);

                BTItem p = d2.GetItem("prio");
                if ((p == null) || (p.Type != BTChunk.kString))
                    continue;

                BTString prioString = (BTString)(p);

                BTFile tor = bel.Load(torrentFile);
                if (tor == null)
                    continue;

                System.Collections.Generic.List<string> a = tor.AllFilesInTorrent();
                if (a != null)
                {
                    int c = 0;

                    p = d2.GetItem("path");
                    if ((p == null) || (p.Type != BTChunk.kString))
                        continue;
                    string defaultFolder = ((BTString)p).AsString();

                    BTItem targets = d2.GetItem("targets");
                    bool hasTargets = ((targets != null) && (targets.Type == BTChunk.kList));
                    BTList targetList = (BTList)(targets);

                    foreach (string s in a)
                    {
                        if ((c < prioString.Data.Length) && ((int)prioString.Data[c] != BTPrio.Skip)) // TODO: will break on conversion of character to integer?
                        {
                            string saveTo = Helpers.FileInFolder(defaultFolder, s).Name;
                            if (hasTargets)
                            {
                                // see if there is a target for this (the c'th) file
                                for (int i = 0; i < targetList.Items.Count; i++)
                                {
                                    BTList l = (BTList)(targetList.Items[i]);
                                    BTInteger n = (BTInteger)(l.Items[0]);
                                    BTString dest = (BTString)(l.Items[1]);
                                    if (n.Value == c)
                                    {
                                        saveTo = dest.AsString();
                                        break;
                                    }
                                }
                            }
                            int percent = (a.Count == 1) ? PercentBitsOn((BTString)(d2.GetItem("have"))) : -1;
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
            BTDictionary dict = GetTorrentDict(torrentFile);
            if (dict == null)
                return "";
            BTItem p = dict.GetItem("prio");
            if ((p == null) || (p.Type != BTChunk.kString))
                return "";
            BTString prioString = (BTString)(p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
                return "";

            int pr = prioString.Data[fileNum];
            if (pr == BTPrio.Normal)
                return "Normal";
            else if (pr == BTPrio.Skip)
                return "Skip";
            else
                return pr.ToString();
        }
        public void SetResumePrio(string torrentFile, int fileNum, byte newPrio)
        {
            if (!SetPrios)
                return;

            if (fileNum == -1)
                fileNum = 0;
            BTDictionary dict = GetTorrentDict(torrentFile);
            if (dict == null)
                return;
            BTItem p = dict.GetItem("prio");
            if ((p == null) || (p.Type != BTChunk.kString))
                return;
            BTString prioString = (BTString)(p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
                return;

            Altered = true;
            PrioWasSet = true;

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

            BTDictionary dict = GetTorrentDict(torrentFile);
            if (dict == null)
                return;

            Altered = true;

            if (fileNum == -1) // single file torrent
            {
                BTItem p = dict.GetItem("path");
                if (p == null)
                {
                    dict.Items.Add(new BTDictionaryItem("path", new BTString(toHere)));
                }
                else
                {
                    if (p.Type != BTChunk.kString)
                        return;
                    ((BTString)p).SetString(toHere);
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
                    theList = (BTList)(p);
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

                    BTList l2 = (BTList)(theList.Items[i]);
                    if ((l2.Items.Count != 2) || (l2.Items[0].Type != BTChunk.kInteger) || (l2.Items[1].Type != BTChunk.kString))
                        return;
                    int n = (int)((BTInteger)(l2.Items[0])).Value;
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
                {
                    thisFileList.Items[1] = new BTString(toHere);
                }
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
            byte[] theHash = sha1.ComputeHash(ms.GetBuffer(), 0, (int)ms.Length);
            ms.Close();
            string newfg = BTString.CharsToHex(theHash, 0, 20);
            ResumeDat.GetDict().Items.Add(new BTDictionaryItem(".fileguard", new BTString(newfg)));
        }
        public FileInfo MatchMissing(string torrentFile, int torrentFileNum, string nameInTorrent)
        {
            // returns true if we found a match (if actSetPrio is on, true also means we have set a priority for this file)
            string simplifiedfname = Helpers.SimplifyName(nameInTorrent);

            foreach (AIOItem aio1 in MissingList)
            {
                if ((aio1.Type != AIOType.kMissing) && (aio1.Type != AIOType.kuTorrenting))
                    continue;

                ProcessedEpisode m = null;
                string name = null;

                if (aio1.Type == AIOType.kMissing)
                {
                    AIOMissing aio = (AIOMissing)(aio1);
                    m = aio.PE;
                    name = aio.TheFileNoExt;
                }
                else if (aio1.Type == AIOType.kuTorrenting)
                {
                    AIOuTorrenting aio = (AIOuTorrenting)(aio1);
                    m = aio.PE;
                    name = aio.DesiredLocationNoExt;
                }

                if ((m == null) || string.IsNullOrEmpty(name))
                    continue;

                // see if the show name matches...
                if (Regex.Match(simplifiedfname, "\\b" + m.TheSeries.Name + "\\b", RegexOptions.IgnoreCase).Success)
                {
                    // see if season and episode match
                    int seasF;
                    int epF;
                    if (TVDoc.FindSeasEp("", simplifiedfname, out seasF, out epF, m.TheSeries.Name, Rexps) && (seasF == m.SeasonNumber) && (epF == m.EpNum))
                    {
                        // match!
                        // get extension from nameInTorrent
                        int p = nameInTorrent.LastIndexOf(".");
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
            Stream s = File.Create(ResumeDatPath);
            ResumeDat.Write(s);
            s.Close();
        }
        override public bool NewTorrentEntry(string torrentFile, int numberInTorrent)
        {
            NewLocation = "";
            PrioWasSet = false;
            Type = "?";
            return true;
        }
        override public bool FoundFileOnDiskForFileInTorrent(string torrentFile, FileInfo onDisk, int numberInTorrent, string nameInTorrent)
        {
            NewLocation = onDisk.FullName;
            Type = "Hash";

            AlterResume(torrentFile, numberInTorrent, onDisk.FullName); // make resume.dat point to the file we found

            if (SetPrios)
            {
                SetResumePrio(torrentFile, numberInTorrent, BTPrio.Normal);
            }

            return true;
        }
        override public bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent, string nameInTorrent)
        {
            Type = "Not Found";

            if (SetPrios)
            {
                SetResumePrio(torrentFile, numberInTorrent, BTPrio.Skip);
            }
            return true;
        }
        override public bool FinishedTorrentEntry(string torrentFile, int numberInTorrent, string filename)
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
                AddResult(Type, torrentFile, (numberInTorrent + 1).ToString(), prioChanged ? GetResumePrio(torrentFile, numberInTorrent) : "", NewLocation);
            return true;
        }
        public bool LoadResumeDat()
        {
            BEncodeLoader bel = new BEncodeLoader();
            ResumeDat = bel.Load(ResumeDatPath);
            return (ResumeDat != null);
        }
        public bool DoWork(StringList Torrents, string searchFolder, ListView results, bool hashSearch, bool matchMissing, bool setPrios, bool testMode, bool searchSubFolders, System.Collections.Generic.List<AIOItem> missingList, FNPRegexList rexps)
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


            foreach (string tf in Torrents)
            {
                r = r && ProcessTorrentFile(tf, null);
                if (!r)
                    break;
            }

            if (Altered && !testMode)
                WriteResumeDat();

            Prog(0);

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
            if (Results == null)
                return;

            int p = torrent.LastIndexOf("\\");
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

        public BTResume(SetProgressDelegate setprog, string resumeDatFile)
            : base(setprog)
        {
            ResumeDatPath = resumeDatFile;
        }

    } // btresume


} // namespace
