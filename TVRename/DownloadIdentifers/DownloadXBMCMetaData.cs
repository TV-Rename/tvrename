// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadKodiMetaData : DownloadIdentifier
    {
        private static List<string> DoneNfo;

        public DownloadKodiMetaData() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override void NotifyComplete([NotNull] FileInfo file)
        {
            if (file.FullName.EndsWith(".nfo", true, new CultureInfo("en")))
            {
                DoneNfo.Add(file.FullName);
            }
            base.NotifyComplete(file);
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            // for each tv show, optionally write a tvshow.nfo file
            if (TVSettings.Instance.NFOShows)
            {
                ItemList theActionList = new ItemList();
                FileInfo tvshownfo = FileHelper.FileInFolder(si.AutoAddFolderBase, "tvshow.nfo");

                SeriesInfo seriesInfo = si.TheSeries();
                bool needUpdate = !tvshownfo.Exists ||
                                  seriesInfo is null ||
                                  seriesInfo.SrvLastUpdated > TimeZoneHelper.Epoch(tvshownfo.LastWriteTime);

                bool alreadyOnTheList = DoneNfo.Contains(tvshownfo.FullName);

                if ((forceRefresh || needUpdate) && !alreadyOnTheList)
                {
                    theActionList.Add(new ActionNfo(tvshownfo, si));
                    DoneNfo.Add(tvshownfo.FullName);
                }
                return theActionList;
            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo,bool forceRefresh)
        {
            if (!TVSettings.Instance.NFOEpisodes)
            {
                return null;
            }

            string fn = filo.RemoveExtension() + ".nfo";
            FileInfo nfo = FileHelper.FileInFolder(filo.Directory, fn);

            if (nfo.Exists && dbep.SrvLastUpdated <= TimeZoneHelper.Epoch(nfo.LastWriteTime) && !forceRefresh)
            {
                return new ItemList();
            }

            //If we do not already have plans to put the file into place
            if (DoneNfo.Contains(nfo.FullName))
            {
                return new ItemList();
            }

            DoneNfo.Add(nfo.FullName);
            return new ItemList { new ActionNfo(nfo, dbep) };
        }

        public sealed override void Reset() => DoneNfo = new List<string>();
    }
}
