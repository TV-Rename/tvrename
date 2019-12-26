using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal sealed class DownloadSeriesJpg : DownloadIdentifier
    {
        private List<string> doneJpg;
        private const string DEFAULT_FILE_NAME = "series.jpg";

        public DownloadSeriesJpg() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadImage;

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (!TVSettings.Instance.SeriesJpg)
            {
                return null;
            }

            ItemList theActionList = new ItemList();
            FileInfo fi = FileHelper.FileInFolder(folder, DEFAULT_FILE_NAME);
            if (forceRefresh ||!doneJpg.Contains(fi.FullName) && !fi.Exists)
            {
                string bannerPath = si.TheSeries()?.GetSeasonBannerPath(snum);
                if (!string.IsNullOrEmpty(bannerPath))
                {
                    theActionList.Add(new ActionDownloadImage(si, null, fi, bannerPath, TVSettings.Instance.ShrinkLargeMede8erImages));
                }

                doneJpg.Add(fi.FullName);
            }
            return theActionList;
        }

        public override void Reset() => doneJpg = new List<string>();
    }
}
