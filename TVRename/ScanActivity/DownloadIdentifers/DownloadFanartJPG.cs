
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TVRename;

internal class DownloadFanartJpg : DownloadIdentifier
{
    private static List<string> DoneFanartJpg = null!;
    private const string DEFAULT_FILE_NAME = "fanart.jpg";

    public DownloadFanartJpg() => Reset();

    public override DownloadType GetDownloadType() => DownloadType.downloadImage;

    public override async Task<ItemList?> ProcessShowAsync(ShowConfiguration si, bool forceRefresh)
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
        return await base.ProcessShowAsync(si, forceRefresh);
    }

    public override async Task<ItemList?> ProcessMovieAsync(MovieConfiguration si, FileInfo filo, bool forceRefresh)
    {
        //We only want to do something if the fanart option is enabled.
        if (TVSettings.Instance.FanArtJpg)
        {
            ItemList theActionList = [];
            foreach (string location in await si.LocationsAsync())
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

        return await base.ProcessMovieAsync(si, filo, forceRefresh);
    }

    public sealed override void Reset()
    {
        DoneFanartJpg = [];
    }
}
