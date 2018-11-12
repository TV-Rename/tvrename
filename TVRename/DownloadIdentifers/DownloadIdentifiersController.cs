// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadIdentifiersController
    {
        private readonly List<DownloadIdentifier> identifiers;
        
        public DownloadIdentifiersController() {
            identifiers = new List<DownloadIdentifier>
            {
                new DownloadFolderJpg(),
                new DownloadEpisodeJpg(),
                new DownloadFanartJpg(),
                new DownloadMede8erMetaData(),
                new DownloadpyTivoMetaData(),
                new DownloadWdtvMetaData(),
                new DownloadSeriesJpg(),
                new DownloadKodiMetaData(),
                new DownloadKodiImages(),
                new IncorrectFileDates(),
            };
        }

        public void NotifyComplete(FileInfo file)
        {
            foreach (DownloadIdentifier di in identifiers)
            {
                di.NotifyComplete(file);
            }
        }

        public ItemList ProcessShow(ShowItem si)
        {
            ItemList theActionList = new ItemList(); 
            foreach (DownloadIdentifier di in identifiers)
            {
                theActionList.Add(di.ProcessShow(si));
            }
            return theActionList;
        }

        public ItemList ProcessSeason(ShowItem si, string folder, int snum)
        {
            ItemList theActionList = new ItemList(); 
            foreach (DownloadIdentifier di in identifiers)
            {
                theActionList.Add(di.ProcessSeason (si,folder,snum));
            }
            return theActionList;
        }

        public ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo)
        {
            ItemList theActionList = new ItemList();
            foreach (DownloadIdentifier di in identifiers)
            {
                theActionList.Add(di.ProcessEpisode(dbep,filo));
            }
            return theActionList;
        }

        public  void Reset() {
            foreach (DownloadIdentifier di in identifiers)
            {
                di.Reset();
            }
        }

        public ItemList ForceUpdateShow(DownloadIdentifier.DownloadType dt, ShowItem si)
        {
            ItemList theActionList = new ItemList();
            foreach (DownloadIdentifier di in identifiers)
            {
                if (dt == di.GetDownloadType())
                    theActionList.Add(di.ProcessShow(si,true));
            }
            return theActionList;
        }

        public ItemList ForceUpdateSeason(DownloadIdentifier.DownloadType dt, ShowItem si, string folder, int snum)
        {
            ItemList theActionList = new ItemList();
            foreach (DownloadIdentifier di in identifiers)
            {
                if (dt == di.GetDownloadType())
                    theActionList.Add(di.ProcessSeason(si, folder,snum, true));
            }
            return theActionList;
        }

        public ItemList ForceUpdateEpisode(DownloadIdentifier.DownloadType dt, ProcessedEpisode dbep, FileInfo filo)
        {
            ItemList theActionList = new ItemList();
            foreach (DownloadIdentifier di in identifiers)
            {
                if (dt == di.GetDownloadType())
                    theActionList.Add(di.ProcessEpisode(dbep,filo, true));
            }
            return theActionList;
        }
    }
}
