using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal abstract class TorrentFinder : Finder
    {
        public override FinderDisplayType DisplayType() => FinderDisplayType.downloading;

        protected void SearchForAppropriateDownloads(SetProgressDelegate prog, int startpct, int totPct, List<TorrentEntry> downloading, DownloadApp tApp)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = ActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct);
            foreach (Item action1 in ActionList)
            {
                if (ActionCancel)
                    return;

                prog.Invoke(startpct + ((totPct - startpct) * (++n) / (c)));

                if (!(action1 is ItemMissing action))
                    continue;

                foreach (TorrentEntry te in downloading)
                {
                    FileInfo file = new FileInfo(te.DownloadingTo);
                    if (!TVSettings.Instance.UsefulExtension(file.Extension, false)) // not a usefile file extension
                        continue;

                    //do any of the possible names for the series match the filename?
                    bool matched = (action.Episode.Show.GetSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(file.FullName, name)));

                    if (!matched) continue;

                    if (TVDoc.FindSeasEp(file, out int seasF, out int epF, out int _, action.Episode.Show) && (seasF == action.Episode.AppropriateSeasonNumber) && (epF == action.Episode.AppropriateEpNum))
                    {
                        toRemove.Add(action1);
                        newList.Add(new ItemDownloading(te, action.Episode, action.TheFileNoExt, tApp));
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

        protected TorrentFinder(TVDoc doc) : base(doc)
        {
        }
    }
}
