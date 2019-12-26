using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class DownloadMede8erMetaData : DownloadIdentifier
    {
        private List<string> doneFiles;
        public DownloadMede8erMetaData() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            if (TVSettings.Instance.Mede8erXML)
            {
                ItemList theActionList = new ItemList();

                FileInfo tvshowxml = FileHelper.FileInFolder(si.AutoAddFolderBase, "series.xml");

                SeriesInfo seriesInfo = si.TheSeries();
                bool needUpdate = !tvshowxml.Exists ||
                                  seriesInfo is null ||
                                  seriesInfo.SrvLastUpdated > TimeZoneHelper.Epoch(tvshowxml.LastWriteTime);

                if ((forceRefresh || needUpdate) && !doneFiles.Contains(tvshowxml.FullName))
                {
                    doneFiles.Add(tvshowxml.FullName);
                    theActionList.Add(new ActionMede8erXML(tvshowxml, si));
                }

                //Updates requested by zakwaan@gmail.com on 18/4/2013
                FileInfo viewxml = FileHelper.FileInFolder(si.AutoAddFolderBase, "View.xml");
                if (!viewxml.Exists && !doneFiles.Contains(viewxml.FullName))
                {
                    doneFiles.Add(viewxml.FullName);
                    theActionList.Add(new ActionMede8erViewXML(viewxml, si));
                }

                return theActionList;
            }

            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (!TVSettings.Instance.Mede8erXML)
            {
                return null;
            }

            ItemList theActionList = new ItemList();

            //Updates requested by zakwaan@gmail.com on 18/4/2013
            FileInfo viewxml = FileHelper.FileInFolder(folder, "View.xml");
            if (!viewxml.Exists)
            {
                theActionList.Add(new ActionMede8erViewXML(viewxml, si, snum));
            }

            return theActionList;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (!TVSettings.Instance.Mede8erXML)
            {
                return null;
            }

            ItemList theActionList = new ItemList();
            string fn = filo.RemoveExtension() + ".xml";
            FileInfo nfo = FileHelper.FileInFolder(filo.Directory, fn);

            if (forceRefresh || !nfo.Exists || dbep.SrvLastUpdated > TimeZoneHelper.Epoch(nfo.LastWriteTime))
            {
                theActionList.Add(new ActionMede8erXML(nfo, dbep));
            }

            return theActionList;
        }

        public sealed override void Reset()
        {
            doneFiles = new List<string>();
        }
    }
}
