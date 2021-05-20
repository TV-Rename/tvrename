using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadFanartJpg : DownloadIdentifier
    {
        private static List<string> DoneFanartJpg;
        private const string DEFAULT_FILE_NAME = "fanart.jpg";

        public DownloadFanartJpg() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadImage;

        public override ItemList? ProcessShow(ShowConfiguration si, bool forceRefresh)
        {
            //We only want to do something if the fanart option is enabled. If the KODI option is enabled then let it do the work.
            if (TVSettings.Instance.FanArtJpg && !TVSettings.Instance.KODIImages)
            {
                ItemList theActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(si.AutoAddFolderBase, DEFAULT_FILE_NAME);

                bool doesntExist = !fi.Exists;
                if ((forceRefresh || doesntExist) && !DoneFanartJpg.Contains(fi.FullName))
                {
                    string bannerPath = si.CachedShow?.GetSeriesFanartPath();

                    if (!string.IsNullOrEmpty(bannerPath))
                    {
                        theActionList.Add(new ActionDownloadImage(si, null, fi, bannerPath, false));
                    }

                    DoneFanartJpg.Add(fi.FullName);
                }
                return theActionList;
            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList? ProcessMovie(MovieConfiguration si, FileInfo filo, bool forceRefresh)
        {
            //We only want to do something if the fanart option is enabled. If the KODI option is enabled then let it do the work.
            if (TVSettings.Instance.FanArtJpg && !TVSettings.Instance.KODIImages)
            {
                ItemList theActionList = new ItemList();
                foreach (string location in si.Locations)
                {
                    FileInfo fi = FileHelper.FileInFolder(location, DEFAULT_FILE_NAME);

                    bool doesntExist = !fi.Exists;
                    if ((forceRefresh || doesntExist) && !DoneFanartJpg.Contains(fi.FullName))
                    {
                        string bannerPath = si.CachedMovie?.PosterUrl;

                        if (!string.IsNullOrEmpty(bannerPath))
                        {
                            theActionList.Add(new ActionDownloadImage(si, null, fi, bannerPath, false));
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
            DoneFanartJpg = new List<string>();
        }
    }
}