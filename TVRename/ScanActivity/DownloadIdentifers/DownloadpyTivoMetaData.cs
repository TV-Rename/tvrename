using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadpyTivoMetaData : DownloadIdentifier
    {
        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
        {
            if (!TVSettings.Instance.pyTivoMeta)
            {
                return null;
            }

            ItemList theActionList = new ItemList();
            string fn = file.Name + ".txt";

            string folder = file.DirectoryName;
            if (TVSettings.Instance.pyTivoMetaSubFolder)
            {
                folder += "\\.meta";
            }

            FileInfo meta = FileHelper.FileInFolder(folder, fn);

            if (!meta.Exists || episode.SrvLastUpdated > TimeZoneHelper.Epoch(meta.LastWriteTime))
            {
                theActionList.Add(new ActionPyTivoMeta(meta, episode));
            }

            return theActionList;
        }
    }
}