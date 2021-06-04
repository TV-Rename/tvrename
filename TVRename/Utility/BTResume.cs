using System.Collections.Generic;
using System.IO;
using NLog;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class BTResume : BTCore
    {
        // ReSharper disable once InconsistentNaming
        private static class BTPrio
        {
            public const int SKIP = 0x80;
        }

        private BTFile? resumeDat; // resume file, if we're using it
        private readonly string resumeDatPath;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BTResume(string resumeDatFile)
        {
            resumeDatPath = resumeDatFile;
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

            if (resumeDat is null)
            {
                return r;
            }

            BEncodeLoader bel = new BEncodeLoader();
            foreach (BTDictionaryItem dictitem in resumeDat.GetDict().Items)
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
                string directoryName = Path.GetDirectoryName(resumeDatPath) + System.IO.Path.DirectorySeparatorChar;

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

                List<string> a = tor?.AllFilesInTorrent();
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
                    if ((c < prioString.Data.Length) && (prioString.Data[c] != BTPrio.SKIP))
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
                        catch (PathTooLongException ptle)
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

        public bool LoadResumeDat()
        {
            BEncodeLoader bel = new BEncodeLoader();
            resumeDat = bel.Load(resumeDatPath);
            return (resumeDat != null);
        }
    }
}
