using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename
{
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

        public BTFile? ResumeDat; // resume file, if we're using it
        public string ResumeDatPath;

        public List<TVSettings.FilenameProcessorRE> Rexps; // used by MatchMissing
        public bool SetPrios;
        public string Type;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public BTResume(SetProgressDelegate setprog, string resumeDatFile)
            : base(setprog)
        {
            ResumeDatPath = resumeDatFile;
        }

        public BTDictionary? GetTorrentDict(string torrentFile)
        {
            // find dictionary for the specified torrent file

            BTItem? it = ResumeDat?.GetDict().GetItem(torrentFile, true);
            if ((it is null) || (it.Type != BTChunk.kDictionary))
            {
                return null;
            }

            BTDictionary dict = (BTDictionary)(it);
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
                    {
                        bitsOn++;
                    }

                    c >>= 1;
                }
            }

            return (100 * bitsOn + totalBits / 2) / totalBits;
        }

        public List<TorrentEntry> AllFilesBeingDownloaded()
        {
            List<TorrentEntry> r = new List<TorrentEntry>();

            if (ResumeDat is null)
            {
                return r;
            }

            BEncodeLoader bel = new BEncodeLoader();
            foreach (BTDictionaryItem dictitem in ResumeDat.GetDict().Items)
            {
                if ((dictitem.Type != BTChunk.kDictionaryItem))
                {
                    continue;
                }

                if ((dictitem.Key == ".fileguard") || (dictitem.Data.Type != BTChunk.kDictionary))
                {
                    continue;
                }

                if (dictitem.Data is BTError err)
                {
                    Logger.Error($"Error finding BT items: {err.Message}");
                    return r;
                }

                BTDictionary d2 = (BTDictionary)(dictitem.Data);

                BTItem p = d2.GetItem("prio");
                if ((p is null) || (p.Type != BTChunk.kString))
                {
                    continue;
                }

                BTString prioString = (BTString)(p);
                string directoryName = Path.GetDirectoryName(ResumeDatPath) + System.IO.Path.DirectorySeparatorChar;

                string torrentFile = dictitem.Key;
                if (!File.Exists(torrentFile)) // if the torrent file doesn't exist
                {
                    torrentFile = directoryName + torrentFile; // ..try prepending the resume.dat folder's path to it.
                }

                if (!File.Exists(torrentFile))
                {
                    continue; // can't find it.  give up!
                }

                BTFile tor = bel.Load(torrentFile);
                if (tor is null)
                {
                    continue;
                }

                List<string> a = tor.AllFilesInTorrent();
                if (a is null)
                {
                    continue;
                }

                int c = 0;

                p = d2.GetItem("path");
                if ((p is null) || (p.Type != BTChunk.kString))
                {
                    continue;
                }

                string defaultFolder = ((BTString)p).AsString();

                BTItem targets = d2.GetItem("targets");
                bool hasTargets = ((targets != null) && (targets.Type == BTChunk.kList));
                BTList targetList = (BTList)(targets);

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
                                    BTList l = (BTList)(t);
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
                            bool completed = ((BTInteger)d2.GetItem("order")).Value == -1;
                            TorrentEntry te = new TorrentEntry(torrentFile, saveTo, percent, completed, torrentFile);
                            r.Add(te);
                        }
                        catch (System.IO.PathTooLongException ptle)
                        {
                            //this is not the file we are looking for
                            Logger.Debug(ptle);
                        }
                    }

                    c++;
                }
            }

            return r;
        }

        private string GetResumePrio(string torrentFile, int fileNum)
        {
            BTDictionary dict = GetTorrentDict(torrentFile);

            BTItem p = dict?.GetItem("prio");
            if (p is null || (p.Type != BTChunk.kString))
            {
                return "";
            }

            BTString prioString = (BTString)(p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
            {
                return "";
            }

            int pr = prioString.Data[fileNum];
            if (pr == BTPrio.Normal)
            {
                return "Normal";
            }

            if (pr == BTPrio.Skip)
            {
                return "Skip";
            }

            return pr.ToString();
        }

        private void SetResumePrio(string torrentFile, int fileNum, byte newPrio)
        {
            if (!SetPrios)
            {
                return;
            }

            if (fileNum == -1)
            {
                fileNum = 0;
            }

            BTDictionary dict = GetTorrentDict(torrentFile);

            BTItem p = dict?.GetItem("prio");
            if (p is null || (p.Type != BTChunk.kString))
            {
                return;
            }

            BTString prioString = (BTString)(p);
            if ((fileNum < 0) || (fileNum > prioString.Data.Length))
            {
                return;
            }

            Altered = true;
            PrioWasSet = true;

            prioString.Data[fileNum] = newPrio;
        }

        public void AlterResume(string torrentFile, int fileNum, string toHere)
        {
            toHere = RemoveUT(toHere);

            BTDictionary dict = GetTorrentDict(torrentFile);
            if (dict is null)
            {
                return;
            }

            Altered = true;

            if (fileNum == -1) // single file torrent
            {
                BTItem p = dict.GetItem("path");
                if (p is null)
                {
                    dict.Items.Add(new BTDictionaryItem("path", new BTString(toHere)));
                }
                else
                {
                    if (p.Type != BTChunk.kString)
                    {
                        return;
                    }

                    ((BTString)p).SetString(toHere);
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
                    {
                        return;
                    }

                    theList = (BTList)(p);
                }

                // the list contains two element lists, of integer/string which are filenumber/path

                BTList thisFileList = null;
                // see if this file is already in the list
                foreach (BTItem t in theList.Items)
                {
                    if (t.Type != BTChunk.kList)
                    {
                        return;
                    }

                    BTList l2 = (BTList)(t);
                    if ((l2.Items.Count != 2) || (l2.Items[0].Type != BTChunk.kInteger) ||
                        (l2.Items[1].Type != BTChunk.kString))
                    {
                        return;
                    }

                    int n = (int)((BTInteger)(l2.Items[0])).Value;
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
                {
                    thisFileList.Items[1] = new BTString(toHere);
                }
            }
        }

        private void FixFileguard()
        {
            if (ResumeDat == null)
            {
                return;
            }
            // finally, fix up ".fileguard"
            // this is the SHA1 of the entire file, without the .fileguard
            ResumeDat.GetDict().RemoveItem(".fileguard");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ResumeDat.Write(ms);
            System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();
            byte[] theHash = sha1.ComputeHash(ms.GetBuffer(), 0, (int)ms.Length);
            ms.Close();
            string newfg = BTString.CharsToHex(theHash, 0, 20);
            ResumeDat.GetDict().Items.Add(new BTDictionaryItem(".fileguard", new BTString(newfg)));
        }

        public FileInfo? MatchMissing(string torrentFile, int torrentFileNum, string nameInTorrent)
        {
            // returns true if we found a match (if actSetPrio is on, true also means we have set a priority for this file)
            string simplifiedfname = nameInTorrent.CompareName();

            foreach (Item action1 in MissingList)
            {
                if ((!(action1 is ItemMissing)) && (!(action1 is ItemDownloading)))
                {
                    continue;
                }

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
                {
                    continue;
                }

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
                        {
                            SetResumePrio(torrentFile, torrentFileNum, BTPrio.Normal);
                        }

                        return new FileInfo(name + ext);
                    }
                }
            }

            return null;
        }

        private void WriteResumeDat()
        {
            if (ResumeDat == null)
            {
                return;
            }
            FixFileguard();
            // write out new resume.dat file
            string to = ResumeDatPath + ".before_tvrename";
            if (File.Exists(to))
            {
                File.Delete(to);
            }

            File.Move(ResumeDatPath, to);
            System.IO.Stream s = File.Create(ResumeDatPath);
            ResumeDat?.Write(s);
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
            {
                SetResumePrio(torrentFile, numberInTorrent, BTPrio.Normal);
            }

            return true;
        }

        protected override bool DidNotFindFileOnDiskForFileInTorrent(string torrentFile, int numberInTorrent,
            string nameInTorrent)
        {
            Type = "Not Found";

            if (SetPrios)
            {
                SetResumePrio(torrentFile, numberInTorrent, BTPrio.Skip);
            }

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
            {
                AddResult(Type, torrentFile, (numberInTorrent + 1).ToString(),
                    prioChanged ? GetResumePrio(torrentFile, numberInTorrent) : string.Empty, NewLocation);
            }

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
            {
                return true; // nothing to do
            }

            if (hashSearch && string.IsNullOrEmpty(searchFolder))
            {
                return false;
            }

            if (matchMissing && ((missingList is null) || (rexps is null)))
            {
                return false;
            }

            MissingList = missingList;
            DoMatchMissing = matchMissing;
            DoHashChecking = hashSearch;
            SetPrios = setPrios;
            Results = results;

            Prog(0, string.Empty);

            if (!LoadResumeDat())
            {
                return false;
            }

            bool r = true;

            Prog(0, string.Empty);

            if (hashSearch)
            {
                BuildFileCache(searchFolder, searchSubFolders);
            }

            foreach (string tf in Torrents)
            {
                r = ProcessTorrentFile(tf, null, args);
                if (!r) // stop on the first failure
                {
                    break;
                }
            }

            if (Altered && !testMode)
            {
                WriteResumeDat();
            }

            Prog(0, string.Empty);

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
            {
                return;
            }

            int p = torrent.LastIndexOf(System.IO.Path.DirectorySeparatorChar.ToString());
            if (p != -1)
            {
                torrent = torrent.Substring(p + 1);
            }

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
