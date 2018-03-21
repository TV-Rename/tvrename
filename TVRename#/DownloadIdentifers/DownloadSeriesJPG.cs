using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadSeriesJPG : DownloadIdentifier
    {
        private List<string> doneJPG;
        private const string defaultFileName = "series.jpg";

        public DownloadSeriesJPG() 
        {
            reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.downloadImage;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.SeriesJpg)
            {
                ItemList TheActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(folder, defaultFileName);
                if (forceRefresh ||(!doneJPG.Contains(fi.FullName) && !fi.Exists))
                {
                    string bannerPath = si.TheSeries().GetSeasonBannerPath(snum);
                    if (!string.IsNullOrEmpty(bannerPath))
                        TheActionList.Add(new ActionDownloadImage(si, null, fi, bannerPath, TVSettings.Instance.ShrinkLargeMede8erImages));
                    doneJPG.Add(fi.FullName);
                }
                return TheActionList;
            }


            return base.ProcessSeason(si, folder, snum, forceRefresh);
        }

        public override void reset()
        {
            doneJPG  = new List<string>();
            base.reset();
        }
    }

}
