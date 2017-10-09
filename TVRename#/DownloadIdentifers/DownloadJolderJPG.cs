using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace TVRename
{
    class DownloadFolderJPG : DownloadIdentifier
    {
        private List<string> doneFolderJPG;
        private const string defaultFileName = "folder.jpg";

        public DownloadFolderJPG() 
        {
            reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.downloadImage;
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            

            if (TVSettings.Instance.FolderJpg)
            {
                ItemList TheActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(si.AutoAdd_FolderBase, defaultFileName);
                bool fileDoesntExist = !doneFolderJPG.Contains(fi.FullName) && !fi.Exists;

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
                        TheActionList.Add(new ActionDownload(si, null, fi, downloadPath, false));
                    doneFolderJPG.Add(fi.FullName);
                }
                return TheActionList;

            }
            return null;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.FolderJpg)
            {
                // season folders JPGs

                ItemList TheActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(folder, defaultFileName);
                if (!doneFolderJPG.Contains(fi.FullName) && (!fi.Exists|| forceRefresh))
                // some folders may come up multiple times
                {

                    string bannerPath = "";

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
                        TheActionList.Add(new ActionDownload(si, null, fi, bannerPath,
                                                                  TVSettings.Instance.ShrinkLargeMede8erImages));
                    doneFolderJPG.Add(fi.FullName);
                }
                return TheActionList;
            }

            
            return base.ProcessSeason(si,folder,snum,forceRefresh);
        }
        public override void reset()
        {
            doneFolderJPG  = new List<string>();
            base.reset();
        }
    }

}
