//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Generic;
using System.Globalization;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadKodiMetaData : DownloadIdentifier
    {
        private static List<string> DoneNfo;

        public DownloadKodiMetaData() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override void NotifyComplete(FileInfo file)
        {
            if (file.FullName.EndsWith(".nfo", true, new CultureInfo("en")))
            {
                DoneNfo.Add(file.FullName);
            }
            base.NotifyComplete(file);
        }

        public override ItemList? ProcessShow(ShowConfiguration si, bool forceRefresh)
        {
            // for each tv show, optionally write a tvshow.nfo file
            if (TVSettings.Instance.NFOShows)
            {
                ItemList theActionList = new ItemList();
                FileInfo tvshownfo = FileHelper.FileInFolder(si.AutoAddFolderBase, "tvshow.nfo");

                CachedSeriesInfo cachedSeriesInfo = si.CachedShow;
                bool needUpdate = !tvshownfo.Exists ||
                                  cachedSeriesInfo is null ||
                                  System.Math.Abs(cachedSeriesInfo.SrvLastUpdated - TimeZoneHelper.Epoch(tvshownfo.LastWriteTime)) > 1;

                bool alreadyOnTheList = DoneNfo.Contains(tvshownfo.FullName);

                if ((forceRefresh || needUpdate) && !alreadyOnTheList)
                {
                    theActionList.Add(new ActionNfoShow(tvshownfo, si));
                    DoneNfo.Add(tvshownfo.FullName);
                }
                return theActionList;
            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
        {
            if (!TVSettings.Instance.NFOEpisodes)
            {
                return null;
            }

            FileInfo nfo = FileHelper.FileInFolder(file.Directory, file.RemoveExtension() + ".nfo");

            if (nfo.Exists && System.Math.Abs(episode.SrvLastUpdated - TimeZoneHelper.Epoch(nfo.LastWriteTime)) < 1 && !forceRefresh)
            {
                return null;
            }

            //If we do not already have plans to put the file into place
            if (DoneNfo.Contains(nfo.FullName))
            {
                return null;
            }

            DoneNfo.Add(nfo.FullName);
            return new ItemList { new ActionNfoEpisode(nfo, episode) };
        }

        public override ItemList? ProcessMovie(MovieConfiguration mc, FileInfo file, bool forceRefresh)
        {
            if (!TVSettings.Instance.NFOMovies || mc.CachedMovie is null)
            {
                return null;
            }

            FileInfo nfo = FileHelper.FileInFolder(file.Directory, file.MovieFileNameBase() + ".nfo");

            if (nfo.Exists && System.Math.Abs(mc.CachedMovie.SrvLastUpdated - TimeZoneHelper.Epoch(nfo.LastWriteTime)) < 1 && !forceRefresh)
            {
                return null;
            }

            //If we do not already have plans to put the file into place
            if (DoneNfo.Contains(nfo.FullName))
            {
                return null;
            }

            DoneNfo.Add(nfo.FullName);
            return new ItemList { new ActionNfoMovie(nfo, mc) };
        }

        public sealed override void Reset() => DoneNfo = new List<string>();
    }
}
