using System.Collections.Generic;
using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadFolderJpg : DownloadIdentifier
    {
        private List<string> doneFolderJpg = new();
        private const string DEFAULT_FILE_NAME = "folder.jpg";

        public DownloadFolderJpg() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadImage;

        [NotNull]
        public override ItemList ProcessMovie(MovieConfiguration mc, FileInfo file, bool forceRefresh)
        {
            if (!TVSettings.Instance.FolderJpg)
            {
                return new ItemList();
            }

            FileInfo fi = mc.IsDvdBluRay()
                ? FileHelper.FileInFolder(file.Directory.Parent, DEFAULT_FILE_NAME)
                : FileHelper.FileInFolder(file.Directory, DEFAULT_FILE_NAME);

            bool fileDoesntExist = !doneFolderJpg.Contains(fi.FullName) && !fi.Exists;

            if (!forceRefresh && !fileDoesntExist)
            {
                return new ItemList();
            }

            CachedMovieInfo? cachedMovie = mc.CachedMovie;

            if (cachedMovie is null)
            {
                return new ItemList();
            }

            ItemList theActionList = new();

            //default to poster
            string downloadPath = cachedMovie.PosterUrl;

            if (!string.IsNullOrEmpty(downloadPath))
            {
                theActionList.Add(new ActionDownloadImage(mc, null, fi, downloadPath));
            }

            doneFolderJpg.Add(fi.FullName);
            return theActionList;
        }

        [NotNull]
        public override ItemList ProcessShow(ShowConfiguration si, bool forceRefresh)
        {
            ItemList theActionList = new();

            if (!TVSettings.Instance.FolderJpg)
            {
                return theActionList;
            }

            FileInfo fi = FileHelper.FileInFolder(si.AutoAddFolderBase, DEFAULT_FILE_NAME);
            bool fileDoesntExist = !doneFolderJpg.Contains(fi.FullName) && !fi.Exists;

            if (forceRefresh || fileDoesntExist)
            {
                CachedSeriesInfo cachedSeries = si.CachedShow;

                if (cachedSeries is null)
                {
                    return theActionList;
                }

                //default to poster when we want season posters for the season specific folders
                string downloadPath = TVSettings.Instance.SeasonSpecificFolderJPG()
                    ? cachedSeries.GetSeriesPosterPath()
                    : cachedSeries.GetImage(TVSettings.Instance.ItemForFolderJpg());

                if (!string.IsNullOrEmpty(downloadPath))
                {
                    theActionList.Add(new ActionDownloadImage(si, null, fi, downloadPath, false));
                }

                doneFolderJpg.Add(fi.FullName);
            }
            return theActionList;
        }

        public override ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum, bool forceRefresh)
        {
            if (!TVSettings.Instance.FolderJpg)
            {
                return null;
            }

            // season folders JPGs
            ItemList theActionList = new();
            FileInfo fi = FileHelper.FileInFolder(folder, DEFAULT_FILE_NAME);

            if (!doneFolderJpg.Contains(fi.FullName) && (!fi.Exists || forceRefresh))
            // some folders may come up multiple times
            {
                string bannerPath = TVSettings.Instance.SeasonSpecificFolderJPG()
                    ? si.CachedShow?.GetSeasonBannerPath(snum)
                    : si.CachedShow?.GetImage(TVSettings.Instance.ItemForFolderJpg());

                if (!string.IsNullOrEmpty(bannerPath))
                {
                    theActionList.Add(new ActionDownloadImage(si, null, fi, bannerPath,
                        TVSettings.Instance.ShrinkLargeMede8erImages));
                }

                doneFolderJpg.Add(fi.FullName);
            }
            return theActionList;
        }

        public sealed override void Reset()
        {
            doneFolderJpg = new List<string>();
        }
    }
}
