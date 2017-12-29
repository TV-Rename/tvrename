using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class DownloadFolderJPG : DownloadIdentifier
    {
        private List<string> _doneFolderJPG;
        private const string DefaultFileName = "folder.jpg";

        public DownloadFolderJPG() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.DownloadImage;
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            

            if (TVSettings.Instance.FolderJpg)
            {
                ItemList theActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(si.AutoAddFolderBase, DefaultFileName);
                bool fileDoesntExist = !_doneFolderJPG.Contains(fi.FullName) && !fi.Exists;

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
                        theActionList.Add(new ActionDownload(si, null, fi, downloadPath, false));
                    _doneFolderJPG.Add(fi.FullName);
                }
                return theActionList;

            }
            return null;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.FolderJpg)
            {
                // season folders JPGs

                ItemList theActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(folder, DefaultFileName);
                if (!_doneFolderJPG.Contains(fi.FullName) && (!fi.Exists|| forceRefresh))
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
                        theActionList.Add(new ActionDownload(si, null, fi, bannerPath,
                                                                  TVSettings.Instance.ShrinkLargeMede8ErImages));
                    _doneFolderJPG.Add(fi.FullName);
                }
                return theActionList;
            }

            
            return base.ProcessSeason(si,folder,snum,forceRefresh);
        }
        public override void Reset()
        {
            _doneFolderJPG  = new List<string>();
            base.Reset();
        }
    }

}
