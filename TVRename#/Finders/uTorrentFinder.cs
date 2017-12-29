using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class UTorrentFinder:Finder
    {
        public UTorrentFinder(TVDoc i) : base(i) { }

        public override bool Active()
        {
            return TVSettings.Instance.CheckuTorrent;
        }

        public override FinderDisplayType DisplayType()
        {
            return FinderDisplayType.Downloading;
        }

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            // get list of files being downloaded by uTorrent
            string resDatFile = TVSettings.Instance.ResumeDatPath;
            if (string.IsNullOrEmpty(resDatFile) || !File.Exists(resDatFile))
                return;

            BtResume btr = new BtResume(prog, resDatFile);
            if (!btr.LoadResumeDat())
                return;

            List<TorrentEntry> downloading = btr.AllFilesBeingDownloaded();

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = TheActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct + totPct * n / c);
            foreach (Item action1 in TheActionList)
            {
                if (ActionCancel)
                    return;

                n++;
                prog.Invoke(100 * n / c);

                if (!(action1 is ItemMissing))
                    continue;

                ItemMissing action = (ItemMissing)(action1);


                foreach (TorrentEntry te in downloading)
                {
                    FileInfo file = new FileInfo(te.DownloadingTo);
                    if (!TVSettings.Instance.UsefulExtension(file.Extension, false)) // not a usefile file extension
                        continue;

                    //do any of the possible names for the series match the filename?
                    Boolean matched = (action.Episode.Si.GetSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(file.FullName, name)));

                    if (matched) 
                    {
                        int seasF;
                        int epF;
                        if (TVDoc.FindSeasEp(file, out seasF, out epF, action.Episode.Si) && (seasF == action.Episode.SeasonNumber) && (epF == action.Episode.EpNum))
                        {
                            toRemove.Add(action1);
                            newList.Add(new ItemuTorrenting(te, action.Episode, action.TheFileNoExt));
                            break;
                        }
                    }
                    
                }
            }

            foreach (Item i in toRemove)
                TheActionList.Remove(i);

            foreach (Item action in newList)
                TheActionList.Add(action);

            prog.Invoke(startpct + totPct);

        }
    }
}
