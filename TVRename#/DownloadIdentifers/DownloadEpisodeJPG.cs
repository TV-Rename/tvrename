using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


namespace TVRename
{
    class DownloadEpisodeJPG : DownloadIdentifier
    {
        private List<string> doneJPG;
        private const string defaultExtension = ".jpg";

        public DownloadEpisodeJPG() 
        {
            reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.downloadImage;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.EpJPGs)
            {
                ItemList TheActionList = new ItemList(); 
                string ban = dbep.GetFilename();
                if (!string.IsNullOrEmpty(ban))
                {
                    string basefn = filo.RemoveExtension();

                    FileInfo imgjpg = FileHelper.FileInFolder(filo.Directory, basefn + ".jpg");

                    if (forceRefresh || !imgjpg.Exists)
                        TheActionList.Add(new ActionDownloadImage(dbep.SI, dbep, imgjpg, ban, TVSettings.Instance.ShrinkLargeMede8erImages));
                }

                return TheActionList;

            }

            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        public override void reset()
        {
            doneJPG = new List<string>();
            base.reset();
        }
    }

}
