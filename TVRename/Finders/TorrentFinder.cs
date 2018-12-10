// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;

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

        protected void SearchForAppropriateDownloads(List<TorrentEntry> downloading, DownloadApp tApp,TVDoc.ScanSettings settings)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = ActionList.MissingItems().Count() + 2;
            int n = 1;
            UpdateStatus(n, c, "Searhcing torrent queue...");
            foreach (ItemMissing action in ActionList.MissingItems())
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                UpdateStatus(n++, c, action.Filename);

                foreach (TorrentEntry te in downloading)
                {
                    FileInfo file = new FileInfo(te.DownloadingTo);
                    if (!file.IsMovieFile()) // not a usefile file extension
                        continue;

                    //do any of the possible names for the series match the filename?
                    bool matched = action.Episode.Show.NameMatch(file);

                    if (!matched) continue;

                    if (FinderHelper.FindSeasEp(file, out int seasF, out int epF, out int _, action.Episode.Show) && (seasF == action.Episode.AppropriateSeasonNumber) && (epF == action.Episode.AppropriateEpNum))
                    {
                        toRemove.Add(action);
                        newList.Add(new ItemDownloading(te, action.Episode, action.TheFileNoExt, tApp));
                        break;
                    }
                }
            }

            foreach (Item i in toRemove)
                ActionList.Remove(i);

            foreach (Item action in newList)
                ActionList.Add(action);
        }

        protected DownloadingFinder(TVDoc doc) : base(doc)
        {
        }
    }
}
