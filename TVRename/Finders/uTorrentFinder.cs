using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using System.Linq;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class uTorrentFinder:Finder
    {
        public uTorrentFinder(TVDoc i) : base(i) { }
        public override bool Active() => TVSettings.Instance.CheckuTorrent;
        public override FinderDisplayType DisplayType() =>FinderDisplayType.Downloading;
        
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
            int c = ActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct);            
            foreach (Item action1 in ActionList)
            {
                if (ActionCancel)
                    return;

                prog.Invoke(startpct + (totPct - startpct) * (++n) / (c));


                if (!(action1 is ItemMissing action))
                    continue;

                foreach (TorrentEntry te in downloading)
                {
                    FileInfo file = new FileInfo(te.DownloadingTo);
                    if (!TVSettings.Instance.UsefulExtension(file.Extension, false)) // not a usefile file extension
                        continue;

                    //do any of the possible names for the series match the filename?
                    bool matched = (action.Episode.SI.getSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(file.FullName, name)));

                    if (!matched) continue;

                    if (TVDoc.FindSeasEp(file, out int seasF, out int epF, out int _, action.Episode.SI) && (seasF == action.Episode.AppropriateSeasonNumber) && (epF == action.Episode.AppropriateEpNum))
                    {
                        toRemove.Add(action1);
                        newList.Add(new ItemuTorrenting(te, action.Episode, action.TheFileNoExt));
                        break;
                    }

                }
            }

            foreach (Item i in toRemove)
                ActionList.Remove(i);

            foreach (Item action in newList)
                ActionList.Add(action);

            prog.Invoke(totPct);

        }
    }
}
