using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadIdentifiersController
    {
        private List<DownloadIdentifier> _identifiers;
        
        public DownloadIdentifiersController() {
            _identifiers = new List<DownloadIdentifier>();
            _identifiers.Add(new DownloadFolderJPG());
            _identifiers.Add(new DownloadEpisodeJPG());
            _identifiers.Add(new DownloadFanartJPG());
            _identifiers.Add(new DownloadMede8ErMetaData());
            _identifiers.Add(new DownloadpyTivoMetaData());
            _identifiers.Add(new DownloadSeriesJPG());
            _identifiers.Add(new DownloadKodiMetaData());
            _identifiers.Add(new DownloadKodiImages());
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
