using System;
using System.Collections.Generic;
using System.Globalization;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadKODIMetaData : DownloadIdentifier
    {
        private static List<string> doneNFO;

        public DownloadKODIMetaData() 
        {
            reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.downloadMetaData;
        }

        public override void notifyComplete(FileInfo file)
        {
            if (file.FullName.EndsWith(".nfo", true, new CultureInfo("en")))
            {
                doneNFO.Add(file.FullName);
            }
            base.notifyComplete(file);
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            // for each tv show, optionally write a tvshow.nfo file
            if (TVSettings.Instance.NFOShows)
            {
                ItemList TheActionList = new ItemList();
                FileInfo tvshownfo = FileHelper.FileInFolder(si.AutoAdd_FolderBase, "tvshow.nfo");

                bool needUpdate = !tvshownfo.Exists ||
                                  (si.TheSeries().Srv_LastUpdated > TimeZone.Epoch(tvshownfo.LastWriteTime)) ||
                    // was it written before we fixed the bug in <episodeguideurl> ?
                                  (tvshownfo.LastWriteTime.ToUniversalTime().CompareTo(new DateTime(2009, 9, 13, 7, 30, 0, 0, DateTimeKind.Utc)) < 0);

                bool alreadyOnTheList = doneNFO.Contains(tvshownfo.FullName);

                if ((forceRefresh || needUpdate) && !alreadyOnTheList)
                {
                    TheActionList.Add(new ActionNFO(tvshownfo, si));
                    doneNFO.Add(tvshownfo.FullName);
                }
                return TheActionList;

            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo,bool forceRefresh)
        {
            if (TVSettings.Instance.NFOEpisodes)
            {
                ItemList TheActionList = new ItemList();

                string fn = filo.Name;
                fn = fn.Substring(0, fn.Length - filo.Extension.Length);
                fn += ".nfo";
                FileInfo nfo = FileHelper.FileInFolder(filo.Directory, fn);

                if (!nfo.Exists || (dbep.Srv_LastUpdated > TimeZone.Epoch(nfo.LastWriteTime)) || forceRefresh)
                {
                    //If we do not already have plans to put the file into place
                    if (!(doneNFO.Contains(nfo.FullName)))
                    {
                        TheActionList.Add(new ActionNFO(nfo, dbep));
                        doneNFO.Add(nfo.FullName);
                    }
                }
                return TheActionList;
            }
            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        public override void reset()
        {
            doneNFO = new List<String>();
            base.reset();
        }

    }
}
