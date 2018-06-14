using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadIdentifiersController
    {
        private readonly List<DownloadIdentifier> Identifiers;
        
        public DownloadIdentifiersController() {
            this.Identifiers = new List<DownloadIdentifier>
            {
                new DownloadFolderJPG(),
                new DownloadEpisodeJPG(),
                new DownloadFanartJPG(),
                new DownloadMede8erMetaData(),
                new DownloadpyTivoMetaData(),
                new DownloadSeriesJPG(),
                new DownloadKodiMetaData(),
                new DownloadKODIImages(),
                new IncorrectFileDates(),
                };

        }

        public void notifyComplete(FileInfo file)
        {
            foreach (DownloadIdentifier di in this.Identifiers)
            {
                di.notifyComplete(file);
            }
        }

        public ItemList ProcessShow(ShowItem si)
        {
            ItemList TheActionList = new ItemList(); 
            foreach (DownloadIdentifier di in this.Identifiers)
            {
                TheActionList.Add(di.ProcessShow(si));
            }
            return TheActionList;
        }

        public ItemList ProcessSeason(ShowItem si, string folder, int snum)
        {
            ItemList TheActionList = new ItemList(); 
            foreach (DownloadIdentifier di in this.Identifiers)
            {
                TheActionList.Add(di.ProcessSeason (si,folder,snum));
            }
            return TheActionList;
        }

        public ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo)
        {
            ItemList TheActionList = new ItemList();
            foreach (DownloadIdentifier di in this.Identifiers)
            {
                TheActionList.Add(di.ProcessEpisode(dbep,filo));
            }
            return TheActionList;
        }

        public  void reset() {
            foreach (DownloadIdentifier di in this.Identifiers)
            {
                di.reset();
            }
        }

        public ItemList ForceUpdateShow(DownloadIdentifier.DownloadType dt, ShowItem si)
        {
            ItemList TheActionList = new ItemList();
            foreach (DownloadIdentifier di in this.Identifiers)
            {
                if (dt == di.GetDownloadType())
                    TheActionList.Add(di.ProcessShow(si,true));
            }
            return TheActionList;
        }

        public ItemList ForceUpdateSeason(DownloadIdentifier.DownloadType dt, ShowItem si, string folder, int snum)
        {
            ItemList TheActionList = new ItemList();
            foreach (DownloadIdentifier di in this.Identifiers)
            {
                if (dt == di.GetDownloadType())
                    TheActionList.Add(di.ProcessSeason(si, folder,snum, true));
            }
            return TheActionList;
        }

        public ItemList ForceUpdateEpisode(DownloadIdentifier.DownloadType dt, ProcessedEpisode dbep, FileInfo filo)
        {
            ItemList TheActionList = new ItemList();
            foreach (DownloadIdentifier di in this.Identifiers)
            {
                if (dt == di.GetDownloadType())
                    TheActionList.Add(di.ProcessEpisode(dbep,filo, true));
            }
            return TheActionList;
        }

    }
}
