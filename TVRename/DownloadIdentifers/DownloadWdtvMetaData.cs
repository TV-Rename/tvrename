// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadWdtvMetaData : DownloadIdentifier
    {
        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (!TVSettings.Instance.wdLiveTvMeta) return null;

            ItemList theActionList = new ItemList();
            string fn = filo.RemoveExtension() + ".xml";
            FileInfo nfo = FileHelper.FileInFolder(filo.Directory, fn);

            if (forceRefresh || !nfo.Exists || (dbep.SrvLastUpdated > TimeZone.Epoch(nfo.LastWriteTime)))
                theActionList.Add(new ActionWdtvMeta(nfo, dbep));

            return theActionList;
        }
    }
}
