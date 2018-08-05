using System;
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

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            // for each tv show, optionally write a tvshow.nfo file
            if (TVSettings.Instance.NFOShows)
            {
                ItemList theActionList = new ItemList();
                FileInfo tvshownfo = FileHelper.FileInFolder(si.AutoAddFolderBase, "tvshow.nfo");

                bool needUpdate = !tvshownfo.Exists ||
                                  (si.TheSeries().SrvLastUpdated > TimeZone.Epoch(tvshownfo.LastWriteTime)) ||
                    // was it written before we fixed the bug in <episodeguideurl> ?
                                  (tvshownfo.LastWriteTime.ToUniversalTime().CompareTo(new DateTime(2009, 9, 13, 7, 30, 0, 0, DateTimeKind.Utc)) < 0);

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
            if (!TVSettings.Instance.NFOEpisodes) return null;

            ItemList theActionList = new ItemList();

            string fn = filo.RemoveExtension() + ".nfo";
            FileInfo nfo = FileHelper.FileInFolder(filo.Directory, fn);

            if (!nfo.Exists || (dbep.SrvLastUpdated > TimeZone.Epoch(nfo.LastWriteTime)) || forceRefresh)
            {
                //If we do not already have plans to put the file into place
                if (!(DoneNfo.Contains(nfo.FullName)))
                {
                    theActionList.Add(new ActionNfo(nfo, dbep));
                    DoneNfo.Add(nfo.FullName);
                }
            }
            return theActionList;
        }

        public sealed override void Reset() => DoneNfo = new List<string>();
    }
}
