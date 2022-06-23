using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename;

internal sealed class DownloadSeriesJpg : DownloadIdentifier
{
    private List<string> doneJpg = new();
    private const string DEFAULT_FILE_NAME = "series.jpg";

    public override DownloadType GetDownloadType() => DownloadType.downloadImage;

    public override ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum, bool forceRefresh)
    {
        if (!TVSettings.Instance.SeriesJpg)
        {
            return null;
        }

        ItemList theActionList = new();
        FileInfo fi = FileHelper.FileInFolder(folder, DEFAULT_FILE_NAME);
        bool fileWorthDownloading = !doneJpg.Contains(fi.FullName) && !fi.Exists;
        if (forceRefresh || fileWorthDownloading)
        {
            string bannerPath = si.CachedShow?.GetSeasonBannerPath(snum);
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
