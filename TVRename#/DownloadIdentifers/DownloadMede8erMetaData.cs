using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class DownloadMede8ErMetaData : DownloadIdentifier
    {

        public DownloadMede8ErMetaData() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.DownloadMetaData;
        }

        public override ItemList ProcessShow(ShowItem si,bool forceRefresh)
        {
            if (TVSettings.Instance.Mede8ErXML)
            {
                ItemList theActionList = new ItemList();

                FileInfo tvshowxml = FileHelper.FileInFolder(si.AutoAddFolderBase, "series.xml");

                bool needUpdate = !tvshowxml.Exists ||
                                  (si.TheSeries().SrvLastUpdated > TimeZone.Epoch(tvshowxml.LastWriteTime));

                if (forceRefresh || needUpdate)
                    theActionList.Add(new ActionMede8ErXML(tvshowxml, si));

                //Updates requested by zakwaan@gmail.com on 18/4/2013
                FileInfo viewxml = FileHelper.FileInFolder(si.AutoAddFolderBase, "view.xml");
                if (!viewxml.Exists) theActionList.Add(new ActionMede8ErViewXML(viewxml,si));


                return theActionList;
            }

            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.Mede8ErXML)
            {
                ItemList theActionList = new ItemList();

                //Updates requested by zakwaan@gmail.com on 18/4/2013
                FileInfo viewxml = FileHelper.FileInFolder(folder, "view.xml");
                if (!viewxml.Exists) theActionList.Add(new ActionMede8ErViewXML(viewxml,si,snum));


                return theActionList;
            }

            return base.ProcessSeason(si, folder, snum, forceRefresh);
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.Mede8ErXML)
            {
                ItemList theActionList = new ItemList();
                string fn = filo.Name;
                fn = fn.Substring(0, fn.Length - filo.Extension.Length);
                fn += ".xml";
                FileInfo nfo = FileHelper.FileInFolder(filo.Directory, fn);

                if (forceRefresh || !nfo.Exists || (dbep.SrvLastUpdated > TimeZone.Epoch(nfo.LastWriteTime)))
                    theActionList.Add(new ActionMede8ErXML(nfo, dbep));

                return theActionList;

            }
            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        public sealed override void Reset()
        {
           base.Reset();
        }

    }
}
