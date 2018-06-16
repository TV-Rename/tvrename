using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadpyTivoMetaData : DownloadIdentifier
    {
        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (!TVSettings.Instance.pyTivoMeta) return null;

            ItemList theActionList = new ItemList(); 
            string fn = filo.Name + ".txt";

            string folder = filo.DirectoryName;
            if (TVSettings.Instance.pyTivoMetaSubFolder)
                folder += "\\.meta";
            FileInfo meta = FileHelper.FileInFolder(folder, fn);

            if (!meta.Exists || (dbep.SrvLastUpdated > TimeZone.Epoch(meta.LastWriteTime)))
                theActionList.Add(new ActionPyTivoMeta(meta, dbep));

            return theActionList;
        }
    }
}
