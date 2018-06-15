using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


namespace TVRename
{
    internal class DownloadEpisodeJpg : DownloadIdentifier
    {
        private const string DEFAULT_EXTENSION = ".jpg";

        public override DownloadType GetDownloadType() => DownloadType.downloadImage;

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (!TVSettings.Instance.EpJPGs) return null;

            ItemList theActionList = new ItemList();

            string ban = dbep.GetFilename();
            if (string.IsNullOrEmpty(ban)) return null;

            string basefn = filo.RemoveExtension();

            FileInfo imgjpg = FileHelper.FileInFolder(filo.Directory, basefn + DEFAULT_EXTENSION);

            if (forceRefresh || !imgjpg.Exists)
                theActionList.Add(new ActionDownloadImage(dbep.SI, dbep, imgjpg, ban, TVSettings.Instance.ShrinkLargeMede8erImages));

            return theActionList;
        }
    }
}
