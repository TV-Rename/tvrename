using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadEpisodeJpg : DownloadIdentifier
    {
        private const string DEFAULT_EXTENSION = ".jpg";

        public override DownloadType GetDownloadType() => DownloadType.downloadImage;

        public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
        {
            if (!TVSettings.Instance.EpJPGs)
            {
                return null;
            }

            ItemList theActionList = new();

            string ban = episode.Filename;
            if (string.IsNullOrEmpty(ban))
            {
                return null;
            }

            string basefn = file.RemoveExtension();

            FileInfo imgjpg = FileHelper.FileInFolder(file.Directory, basefn + DEFAULT_EXTENSION);

            if (forceRefresh || !imgjpg.Exists)
            {
                theActionList.Add(new ActionDownloadImage(episode.Show, episode, imgjpg, ban, TVSettings.Instance.ShrinkLargeMede8erImages));
            }

            return theActionList;
        }
    }
}
