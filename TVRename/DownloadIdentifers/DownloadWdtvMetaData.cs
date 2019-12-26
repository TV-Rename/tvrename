// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadWdtvMetaData : DownloadIdentifier
    {
        private List<string> doneFiles;
        public DownloadWdtvMetaData()
        {
            Reset();
        }

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (!TVSettings.Instance.wdLiveTvMeta)
            {
                return null;
            }

            ItemList theActionList = new ItemList();
            string fn = filo.RemoveExtension() + ".xml";
            FileInfo nfo = FileHelper.FileInFolder(filo.Directory, fn);

            if (forceRefresh || !nfo.Exists || dbep.SrvLastUpdated > TimeZoneHelper.Epoch(nfo.LastWriteTime))
            {
                theActionList.Add(new ActionWdtvMeta(nfo, dbep));
            }

            return theActionList;
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            if (TVSettings.Instance.wdLiveTvMeta)
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
                    theActionList.Add(new ActionWdtvMeta(tvshowxml, si));
                }
                return theActionList;
            }
            return base.ProcessShow(si, forceRefresh);
        }

        public sealed override void Reset()
        {
            doneFiles = new List<string>();
        }
    }
}
