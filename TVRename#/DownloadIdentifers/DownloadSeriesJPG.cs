using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadSeriesJPG : DownloadIdentifier
    {
        private List<string> _doneJPG;
        private const string DefaultFileName = "series.jpg";

        public DownloadSeriesJPG() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.DownloadImage;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.SeriesJpg)
            {
                ItemList theActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(folder, DefaultFileName);
                if (forceRefresh ||(!_doneJPG.Contains(fi.FullName) && !fi.Exists))
                {
                    string bannerPath = si.TheSeries().GetSeasonBannerPath(snum);
                    if (!string.IsNullOrEmpty(bannerPath))
                        theActionList.Add(new ActionDownload(si, null, fi, bannerPath, TVSettings.Instance.ShrinkLargeMede8ErImages));
                    _doneJPG.Add(fi.FullName);
                }
                return theActionList;
            }


            return base.ProcessSeason(si, folder, snum, forceRefresh);
        }

        public override void Reset()
        {
            _doneJPG  = new List<string>();
            base.Reset();
        }
    }

}
