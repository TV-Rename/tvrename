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
            int c = ActionList.Missing.Count + 2;
            int n = 1;
            UpdateStatus(n, c, "Searching torrent queue...");
            foreach (ItemMissing? action in ActionList.Missing.ToList())
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

                    if (action is ShowItemMissing showMissingAction)
                    {
                        //do any of the possible names for the cachedSeries match the filename?
                        ProcessedEpisode episode = showMissingAction.MissingEpisode;
                        bool matched = episode.Show.NameMatch(file, true);

                        if (!matched)
                        {
                            continue;
                        }

                        if (FinderHelper.FindSeasEp(file, out int seasF, out int epF, out int _,
                                episode.Show) && seasF == episode.AppropriateSeasonNumber &&
                            epF == episode.AppropriateEpNum)
                        {
                            toRemove.Add(action);
                            newList.Add(new ItemDownloading(te, episode, action.TheFileNoExt, tApp,action));
                            break;
                        }
                    }

                    if (action is MovieItemMissing movieMissingAction)
                    {
                        //do any of the possible names for the cachedSeries match the filename?
                        MovieConfiguration movie = movieMissingAction.MovieConfig;
                        bool matched = movie.NameMatch(file, true);

                        if (!matched)
                        {
                            continue;
                        }


                        toRemove.Add(action);
                        newList.Add(new ItemDownloading(te, movie, action.TheFileNoExt, tApp, action));
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
