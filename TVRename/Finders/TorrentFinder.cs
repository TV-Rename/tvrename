// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public abstract class DownloadingFinder : Finder
    {
        public enum DownloadApp
        {
            // ReSharper disable once InconsistentNaming
            SABnzbd,
            uTorrent,
            qBitTorrent
        }

        public override FinderDisplayType DisplayType() => FinderDisplayType.downloading;

        protected void SearchForAppropriateDownloads(SetProgressDelegate prog, int startpct, int totPct, List<TorrentEntry> downloading, DownloadApp tApp)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = ActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct);
            foreach (ItemMissing action in ActionList.MissingItems())
            {
                if (ActionCancel)
                    return;

                prog.Invoke(startpct + ((totPct - startpct) * (++n) / (c)));

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
<<<<<<< HEAD
                        toRemove.Add(action1);
                        newList.Add(new ItemTorrentDownload(te, action.Episode, action.TheFileNoExt, tApp));
=======
                        toRemove.Add(action);
                        newList.Add(new ItemDownloading(te, action.Episode, action.TheFileNoExt, tApp));
>>>>>>> e1d0edf1b9da866771e48974e22cb04bcceb43b0
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

        protected DownloadingFinder(TVDoc doc) : base(doc)
        {
        }
    }
}
