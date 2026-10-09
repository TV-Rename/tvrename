//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//




namespace TVRename;

internal sealed class DownloadSeriesJpg : DownloadIdentifier
{
    private List<string> doneJpg = [];
    private const string DEFAULT_FILE_NAME = "series.jpg";

    public override DownloadType GetDownloadType() => DownloadType.downloadImage;

    public override ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum, bool forceRefresh)
    {
        if (!TVSettings.Instance.SeriesJpg)
        {
            return null;
        }

        ItemList theActionList = [];
        FileInfo fi = FileHelper.FileInFolder(folder, DEFAULT_FILE_NAME);
        bool fileWorthDownloading = !doneJpg.Contains(fi.FullName) && !fi.Exists;
        if (forceRefresh || fileWorthDownloading)
        {
            string? bannerPath = si.CachedShow?.GetSeasonBannerPath(snum);
            if (!string.IsNullOrEmpty(bannerPath))
            {
                theActionList.Add(new ActionDownloadImage(si, null, fi, bannerPath, TVSettings.Instance.ShrinkLargeMede8erImages));
            }

            doneJpg.Add(fi.FullName);
        }
        return theActionList;
    }

    public override void Reset() => doneJpg = [];
}
