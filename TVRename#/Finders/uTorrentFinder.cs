using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using System.Linq;

namespace TVRename
{
    class uTorrentFinder:Finder
    {
        public uTorrentFinder(TVDoc i) : base(i) { }

        public override bool Active()
        {
            return TVSettings.Instance.CheckuTorrent;
        }

        public override Finder.FinderDisplayType DisplayType()
        {
            return FinderDisplayType.Downloading;
        }

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            // get list of files being downloaded by uTorrent
            string resDatFile = TVSettings.Instance.ResumeDatPath;
            if (string.IsNullOrEmpty(resDatFile) || !File.Exists(resDatFile))
                return;

            BTResume btr = new BTResume(prog, resDatFile);
            if (!btr.LoadResumeDat())
                return;

            List<TorrentEntry> downloading = btr.AllFilesBeingDownloaded();

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = this.TheActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct + totPct * n / c);
            foreach (Item Action1 in this.TheActionList)
            {
                if (this.ActionCancel)
                    return;

                n++;
                prog.Invoke(100 * n / c);

                if (!(Action1 is ItemMissing))
                    continue;

                ItemMissing Action = (ItemMissing)(Action1);


                foreach (TorrentEntry te in downloading)
                {
                    FileInfo file = new FileInfo(te.DownloadingTo);
                    if (!TVSettings.Instance.UsefulExtension(file.Extension, false)) // not a usefile file extension
                        continue;

                    //do any of the possible names for the series match the filename?
                    Boolean matched = (Action.Episode.SI.getSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(file.FullName, name)));

                    if (!matched) continue;

                    if (TVDoc.FindSeasEp(file, out int seasF, out int epF, Action.Episode.SI) && (seasF == Action.Episode.SeasonNumber) && (epF == Action.Episode.EpNum))
                    {
                        toRemove.Add(Action1);
                        newList.Add(new ItemuTorrenting(te, Action.Episode, Action.TheFileNoExt));
                        break;
                    }

                }
            }

            foreach (Item i in toRemove)
                this.TheActionList.Remove(i);

            foreach (Item Action in newList)
                this.TheActionList.Add(Action);

            prog.Invoke(startpct + totPct);

        }
    }
}
