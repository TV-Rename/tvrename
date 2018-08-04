using System.Collections.Generic;
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

        public override DownloadType GetDownloadType()
        {
            return DownloadType.downloadImage;
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            if (!TVSettings.Instance.FolderJpg) return null;

            ItemList theActionList = new ItemList();
            FileInfo fi = FileHelper.FileInFolder(si.AutoAddFolderBase, DEFAULT_FILE_NAME);
            bool fileDoesntExist = !doneFolderJpg.Contains(fi.FullName) && !fi.Exists;

            if (forceRefresh || fileDoesntExist)
            {
                string downloadPath;

                if (TVSettings.Instance.SeasonSpecificFolderJPG())
                {
                    //default to poster when we want season posters for the season specific folders;
                    downloadPath = si.TheSeries().GetSeriesPosterPath();
                }
                else
                {
                    downloadPath = si.TheSeries().GetImage(TVSettings.Instance.ItemForFolderJpg());
                }

                if (!string.IsNullOrEmpty(downloadPath))
                    theActionList.Add(new ActionDownloadImage(si, null, fi, downloadPath, false));
                doneFolderJpg.Add(fi.FullName);
            }
            return theActionList;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (!TVSettings.Instance.FolderJpg) return null;

            // season folders JPGs
            ItemList theActionList = new ItemList();
            FileInfo fi = FileHelper.FileInFolder(folder, DEFAULT_FILE_NAME);

            if (!doneFolderJpg.Contains(fi.FullName) && (!fi.Exists|| forceRefresh))
                // some folders may come up multiple times
            {
                string bannerPath;

                if (TVSettings.Instance.SeasonSpecificFolderJPG())
                {
                    //We are getting a Series Level image
                    bannerPath = si.TheSeries().GetSeasonBannerPath(snum);
                }
                else
                {
                    //We are getting a Show Level image
                    bannerPath = si.TheSeries().GetImage(TVSettings.Instance.ItemForFolderJpg());
                }
                if (!string.IsNullOrEmpty(bannerPath))
                    theActionList.Add(new ActionDownloadImage(si, null, fi, bannerPath,
                        TVSettings.Instance.ShrinkLargeMede8erImages));
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
