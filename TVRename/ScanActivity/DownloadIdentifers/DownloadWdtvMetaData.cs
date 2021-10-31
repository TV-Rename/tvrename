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
        private List<string> doneFiles = new();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
        {
            if (!TVSettings.Instance.wdLiveTvMeta)
            {
                return null;
            }

            ItemList theActionList = new();
            string fn = file.RemoveExtension() + ".xml";
            FileInfo nfo = FileHelper.FileInFolder(file.Directory, fn);

            if (forceRefresh || !nfo.Exists || episode.SrvLastUpdated > TimeZoneHelper.Epoch(nfo.LastWriteTime))
            {
                theActionList.Add(new ActionWdtvMeta(nfo, episode));
            }

            return theActionList;
        }

        public override ItemList? ProcessShow(ShowConfiguration si, bool forceRefresh)
        {
            if (TVSettings.Instance.wdLiveTvMeta)
            {
                ItemList theActionList = new();
                FileInfo tvShowXml = FileHelper.FileInFolder(si.AutoAddFolderBase, "series.xml");

                CachedSeriesInfo cachedSeriesInfo = si.CachedShow;
                bool needUpdate = !tvShowXml.Exists ||
                                  cachedSeriesInfo is null ||
                                  cachedSeriesInfo.SrvLastUpdated > TimeZoneHelper.Epoch(tvShowXml.LastWriteTime);

                if ((forceRefresh || needUpdate) && !doneFiles.Contains(tvShowXml.FullName))
                {
                    doneFiles.Add(tvShowXml.FullName);
                    theActionList.Add(new ActionWdtvMeta(tvShowXml, si));
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
