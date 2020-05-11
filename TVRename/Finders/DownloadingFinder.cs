// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
            int c = ActionList.Missing.Count() + 2;
            int n = 1;
            UpdateStatus(n, c, "Searching torrent queue...");
            foreach (ItemMissing action in ActionList.Missing.ToList())
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(n++, c, action.Filename);

                foreach (TorrentEntry te in downloading)
                {
                    FileInfo file = new FileInfo(te.DownloadingTo);
                    if (!file.IsMovieFile() && file.Extension != ".rar") // not a usefile file extension
                    {
                        continue;
                    }

                    //do any of the possible names for the series match the filename?
                    bool matched = action.Episode.Show.NameMatch(file,true);

                    if (!matched)
                    {
                        continue;
                    }

                    if (FinderHelper.FindSeasEp(file, out int seasF, out int epF, out int _, action.Episode.Show) && seasF == action.Episode.AppropriateSeasonNumber && epF == action.Episode.AppropriateEpNum)
                    {
                        toRemove.Add(action);
                        newList.Add(new ItemDownloading(te, action.Episode, action.TheFileNoExt, tApp));
                        break;
                    }
                }
            }
            ActionList.Replace(toRemove,newList);
        }

        protected DownloadingFinder(TVDoc doc) : base(doc)
        {
        }
    }
}
