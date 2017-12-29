using System;
using System.Collections.Generic;
using System.Globalization;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class DownloadKodiMetaData : DownloadIdentifier
    {
        private static List<string> _doneNfo;

        public DownloadKodiMetaData() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.DownloadMetaData;
        }

        public override void NotifyComplete(FileInfo file)
        {
            if (file.FullName.EndsWith(".nfo", true, new CultureInfo("en")))
            {
                _doneNfo.Add(file.FullName);
            }
            base.NotifyComplete(file);
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            // for each tv show, optionally write a tvshow.nfo file
            if (TVSettings.Instance.NfoShows)
            {
                ItemList theActionList = new ItemList();
                FileInfo tvshownfo = FileHelper.FileInFolder(si.AutoAddFolderBase, "tvshow.nfo");

                bool needUpdate = !tvshownfo.Exists ||
                                  (si.TheSeries().SrvLastUpdated > TimeZone.Epoch(tvshownfo.LastWriteTime)) ||
                    // was it written before we fixed the bug in <episodeguideurl> ?
                                  (tvshownfo.LastWriteTime.ToUniversalTime().CompareTo(new DateTime(2009, 9, 13, 7, 30, 0, 0, DateTimeKind.Utc)) < 0);

                bool alreadyOnTheList = _doneNfo.Contains(tvshownfo.FullName);

                if ((forceRefresh || needUpdate) && !alreadyOnTheList)
                {
                    theActionList.Add(new ActionNfo(tvshownfo, si));
                    _doneNfo.Add(tvshownfo.FullName);
                }
                return theActionList;

            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo,bool forceRefresh)
        {
            if (TVSettings.Instance.NfoEpisodes)
            {
                ItemList theActionList = new ItemList();

                string fn = filo.Name;
                fn = fn.Substring(0, fn.Length - filo.Extension.Length);
                fn += ".nfo";
                FileInfo nfo = FileHelper.FileInFolder(filo.Directory, fn);

                if (!nfo.Exists || (dbep.SrvLastUpdated > TimeZone.Epoch(nfo.LastWriteTime)) || forceRefresh)
                {
                    //If we do not already have plans to put the file into place
                    if (!(_doneNfo.Contains(nfo.FullName)))
                    {
                        theActionList.Add(new ActionNfo(nfo, dbep));
                        _doneNfo.Add(nfo.FullName);
                    }
                }
                return theActionList;
            }
            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        public sealed override void Reset()
        {
            _doneNfo = new List<String>();
            base.Reset();
        }

    }
}
