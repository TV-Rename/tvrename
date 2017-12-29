using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class DownloadIdentifiersController
    {
        private readonly List<DownloadIdentifier> _identifiers;
        
        public DownloadIdentifiersController() {
            _identifiers = new List<DownloadIdentifier>
            {
                new DownloadFolderJPG(),
                new DownloadEpisodeJPG(),
                new DownloadFanartJPG(),
                new DownloadMede8ErMetaData(),
                new DownloadpyTivoMetaData(),
                new DownloadSeriesJPG(),
                new DownloadKodiMetaData(),
                new DownloadKodiImages()
            };
        }

        public void NotifyComplete(FileInfo file)
        {
            foreach (DownloadIdentifier di in _identifiers)
            {
                di.NotifyComplete(file);
            }
        }

        public ItemList ProcessShow(ShowItem si)
        {
            ItemList theActionList = new ItemList(); 
            foreach (DownloadIdentifier di in _identifiers)
            {
                theActionList.Add(di.ProcessShow(si));
            }
            return theActionList;
        }

        public ItemList ProcessSeason(ShowItem si, string folder, int snum)
        {
            ItemList theActionList = new ItemList(); 
            foreach (DownloadIdentifier di in _identifiers)
            {
                theActionList.Add(di.ProcessSeason (si,folder,snum));
            }
            return theActionList;
        }

        public ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo)
        {
            ItemList theActionList = new ItemList();
            foreach (DownloadIdentifier di in _identifiers)
            {
                theActionList.Add(di.ProcessEpisode(dbep,filo));
            }
            return theActionList;
        }

        public  void Reset() {
            foreach (DownloadIdentifier di in _identifiers)
            {
                di.Reset();
            }
        }

        public ItemList ForceUpdateShow(DownloadIdentifier.DownloadType dt, ShowItem si)
        {
            ItemList theActionList = new ItemList();
            foreach (DownloadIdentifier di in _identifiers)
            {
                if (dt == di.GetDownloadType())
                    theActionList.Add(di.ProcessShow(si,true));
            }
            return theActionList;
        }

        public ItemList ForceUpdateSeason(DownloadIdentifier.DownloadType dt, ShowItem si, string folder, int snum)
        {
            ItemList theActionList = new ItemList();
            foreach (DownloadIdentifier di in _identifiers)
            {
                if (dt == di.GetDownloadType())
                    theActionList.Add(di.ProcessSeason(si, folder,snum, true));
            }
            return theActionList;
        }

        public ItemList ForceUpdateEpisode(DownloadIdentifier.DownloadType dt, ProcessedEpisode dbep, FileInfo filo)
        {
            ItemList theActionList = new ItemList();
            foreach (DownloadIdentifier di in _identifiers)
            {
                if (dt == di.GetDownloadType())
                    theActionList.Add(di.ProcessEpisode(dbep,filo, true));
            }
            return theActionList;
        }

    }
}
