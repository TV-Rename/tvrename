using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;

namespace TVRename;

internal class DownloadFanartJpg : DownloadIdentifier
{
    private static List<string> DoneFanartJpg = null!;
    private const string DEFAULT_FILE_NAME = "fanart.jpg";

    public DownloadFanartJpg() => Reset();

    public override DownloadType GetDownloadType() => DownloadType.downloadImage;

    public override ItemList? ProcessShow(ShowConfiguration si, bool forceRefresh)
    {
        //We only want to do something if the fanart option is enabled. If the KODI option is enabled then let it do the work.
        if (TVSettings.Instance.FanArtJpg && !TVSettings.Instance.KODIImages)
        {
            ItemList theActionList = [];
            FileInfo fi = FileHelper.FileInFolder(si.AutoAddFolderBase, DEFAULT_FILE_NAME);

            bool doesntExist = !fi.Exists;
            if ((forceRefresh || doesntExist) && !DoneFanartJpg.Contains(fi.FullName))
            {
                string? bannerPath = si.CachedShow?.GetSeriesFanartPath();

                if (!string.IsNullOrEmpty(bannerPath))
                {
                    theActionList.Add(new ActionDownloadTvShowImage(si, fi, bannerPath));
                }

                DoneFanartJpg.Add(fi.FullName);
            }
            return theActionList;
        }
        return base.ProcessShow(si, forceRefresh);
    }

    public override ItemList? ProcessMovie(MovieConfiguration si, FileInfo filo, bool forceRefresh)
    {
        //We only want to do something if the fanart option is enabled.
        if (TVSettings.Instance.FanArtJpg)
        {
            ItemList theActionList = [];
            foreach (string location in si.Locations)
            {
                FileInfo fi = FileHelper.FileInFolder(location, DEFAULT_FILE_NAME);

                bool doesntExist = !fi.Exists;
                if ((forceRefresh || doesntExist) && !DoneFanartJpg.Contains(fi.FullName))
                {
                    string? bannerPath = si.CachedMovie?.FanartUrl;

                    if (!string.IsNullOrEmpty(bannerPath))
                    {
                        theActionList.Add(new ActionDownloadMovieImage(si, fi, bannerPath));
                    }

                    DoneFanartJpg.Add(fi.FullName);
                }
            }

            return theActionList;
        }

        return base.ProcessMovie(si, filo, forceRefresh);
    }

    public sealed override void Reset()
    {
        DoneFanartJpg = [];
    }
}
