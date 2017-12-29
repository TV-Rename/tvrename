using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class DownloadEpisodeJPG : DownloadIdentifier
    {
        private const string DefaultExtension = ".jpg";

        public DownloadEpisodeJPG() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.DownloadImage;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.EpJpGs)
            {
                ItemList theActionList = new ItemList(); 
                string ban = dbep.GetFilename();
                if (!string.IsNullOrEmpty(ban))
                {
                    string basefn = filo.Name;
                    basefn = basefn.Substring(0, basefn.Length - filo.Extension.Length); //remove extension

                    FileInfo imgjpg = FileHelper.FileInFolder(filo.Directory, basefn + DefaultExtension);

                    if (forceRefresh || !imgjpg.Exists)
                        theActionList.Add(new ActionDownload(dbep.Si, dbep, imgjpg, ban, TVSettings.Instance.ShrinkLargeMede8ErImages));
                }

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
