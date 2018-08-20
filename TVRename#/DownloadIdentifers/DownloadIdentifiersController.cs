using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadIdentifiersController
    {
        private List<DownloadIdentifier> Identifiers;
        
        public DownloadIdentifiersController() {
            Identifiers = new List<DownloadIdentifier>();
            Identifiers.Add(new DownloadFolderJPG());
            Identifiers.Add(new DownloadEpisodeJPG());
            Identifiers.Add(new DownloadFanartJPG());
            Identifiers.Add(new DownloadMede8erMetaData());
            Identifiers.Add(new DownloadpyTivoMetaData());
            Identifiers.Add(new DownloadSeriesJPG());
            Identifiers.Add(new DownloadKodiMetaData());
            Identifiers.Add(new DownloadKODIImages());
            Identifiers.Add(new IncorrectFileDates());
        }

        public void notifyComplete(FileInfo file)
        {
            foreach (DownloadIdentifier di in Identifiers)
            {
                di.notifyComplete(file);
            }
        }

        public ItemList ProcessShow(ShowItem si)
        {
            ItemList TheActionList = new ItemList(); 
            foreach (DownloadIdentifier di in Identifiers)
            {
                TheActionList.Add(di.ProcessShow(si));
            }
            return TheActionList;
        }

        public ItemList ProcessSeason(ShowItem si, string folder, int snum)
        {
            ItemList TheActionList = new ItemList(); 
            foreach (DownloadIdentifier di in Identifiers)
            {
                TheActionList.Add(di.ProcessSeason (si,folder,snum));
            }
            return TheActionList;
        }

        public ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo)
        {
            ItemList TheActionList = new ItemList();
            foreach (DownloadIdentifier di in Identifiers)
            {
                TheActionList.Add(di.ProcessEpisode(dbep,filo));
            }
            return TheActionList;
        }

        public  void reset() {
            foreach (DownloadIdentifier di in Identifiers)
            {
                di.reset();
            }
        }

        public ItemList ForceUpdateShow(DownloadIdentifier.DownloadType dt, ShowItem si)
        {
            ItemList TheActionList = new ItemList();
            foreach (DownloadIdentifier di in Identifiers)
            {
                if (dt == di.GetDownloadType())
                    TheActionList.Add(di.ProcessShow(si,true));
            }
            return TheActionList;
        }

        public ItemList ForceUpdateSeason(DownloadIdentifier.DownloadType dt, ShowItem si, string folder, int snum)
        {
            ItemList TheActionList = new ItemList();
            foreach (DownloadIdentifier di in Identifiers)
            {
                if (dt == di.GetDownloadType())
                    TheActionList.Add(di.ProcessSeason(si, folder,snum, true));
            }
            return TheActionList;
        }

        public ItemList ForceUpdateEpisode(DownloadIdentifier.DownloadType dt, ProcessedEpisode dbep, FileInfo filo)
        {
            ItemList TheActionList = new ItemList();
            foreach (DownloadIdentifier di in Identifiers)
            {
                if (dt == di.GetDownloadType())
                    TheActionList.Add(di.ProcessEpisode(dbep,filo, true));
            }
            return TheActionList;
        }

    }
}
