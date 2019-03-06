using System.Collections.Generic;
using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadFolderJpg : DownloadIdentifier
    {
        private List<string> doneFolderJpg;
        private const string DEFAULT_FILE_NAME = "folder.jpg";

        public DownloadFolderJpg() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType() => DownloadType.downloadImage;

        [NotNull]
        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            ItemList theActionList = new ItemList();

            if (!TVSettings.Instance.FolderJpg)
            {
                return theActionList;
            }

            if (si is null)
            {
                return theActionList;
            }

            FileInfo fi = FileHelper.FileInFolder(si.AutoAddFolderBase, DEFAULT_FILE_NAME);
            bool fileDoesntExist = !doneFolderJpg.Contains(fi.FullName) && !fi.Exists;

            if (forceRefresh || fileDoesntExist)
            {
                SeriesInfo series = si.TheSeries();

                if (series == null)
                {
                    return theActionList;
                }

                //default to poster when we want season posters for the season specific folders
                string downloadPath = TVSettings.Instance.SeasonSpecificFolderJPG()
                    ? series.GetSeriesPosterPath()
                    : series.GetImage(TVSettings.Instance.ItemForFolderJpg());

                if (!string.IsNullOrEmpty(downloadPath))
                {
                    theActionList.Add(new ActionDownloadImage(si, null, fi, downloadPath, false));
                }

                doneFolderJpg.Add(fi.FullName);
            }
            return theActionList;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (!TVSettings.Instance.FolderJpg)
            {
                return null;
            }

            // season folders JPGs
            ItemList theActionList = new ItemList();
            FileInfo fi = FileHelper.FileInFolder(folder, DEFAULT_FILE_NAME);

            if (!doneFolderJpg.Contains(fi.FullName) && (!fi.Exists|| forceRefresh))
                // some folders may come up multiple times
            {
                string bannerPath = TVSettings.Instance.SeasonSpecificFolderJPG()
                    ? si.TheSeries()?.GetSeasonBannerPath(snum)
                    : si.TheSeries()?.GetImage(TVSettings.Instance.ItemForFolderJpg());

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
            doneFolderJpg  = new List<string>();
        }
    }
}
