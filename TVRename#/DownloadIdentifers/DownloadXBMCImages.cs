using System;
using System.Collections.Generic;
using System.Globalization;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class DownloadKodiImages : DownloadIdentifier
    {
        private List<string> _donePosterJPG;
        private List<string> _doneBannerJPG;
        private List<string> _doneFanartJPG;
        private List<string> _doneTbn;

        public DownloadKodiImages() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.DownloadImage;
        }

        public override void NotifyComplete(FileInfo file)
        {
            if (file.Name.EndsWith(".tbn", true, new CultureInfo("en")))
            {
                _doneTbn.Add(file.FullName);
            } 
            base.NotifyComplete(file);
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            //If we have KODI New style images being downloaded then we want to check that 3 files exist
            //for the series:
            //http://wiki.xbmc.org/index.php?title=XBMC_v12_(Frodo)_FAQ#Local_images
            //poster
            //banner
            //fanart

            if (TVSettings.Instance.KodiImages)
            {
                ItemList theActionList = new ItemList();
                // base folder:
                if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AllFolderLocations(false).Count > 0))
                {
                    FileInfo posterJPG = FileHelper.FileInFolder(si.AutoAddFolderBase, "poster.jpg");
                    FileInfo bannerJPG = FileHelper.FileInFolder(si.AutoAddFolderBase, "banner.jpg");
                    FileInfo fanartJPG = FileHelper.FileInFolder(si.AutoAddFolderBase, "fanart.jpg");

                    if ((forceRefresh || (!posterJPG.Exists)) && (!_donePosterJPG.Contains(si.AutoAddFolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesPosterPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownload(si, null, posterJPG, path, false));
                            _donePosterJPG.Add(si.AutoAddFolderBase);
                        }
                    }

                    if ((forceRefresh || (!bannerJPG.Exists)) && (!_doneBannerJPG.Contains(si.AutoAddFolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesWideBannerPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownload(si, null, bannerJPG, path, false));
                            _doneBannerJPG.Add(si.AutoAddFolderBase);
                        }
                    }

                    if ((forceRefresh || (!fanartJPG.Exists)) && (!_doneFanartJPG.Contains(si.AutoAddFolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesFanartPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownload(si, null, fanartJPG, path));
                            _doneFanartJPG.Add(si.AutoAddFolderBase);
                        }
                    }
                }
                return theActionList;
            }

            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.KodiImages)
            {
                ItemList theActionList = new ItemList();
                if (TVSettings.Instance.DownloadFrodoImages())
                {
                    //If we have KODI New style images being downloaded then we want to check that 3 files exist
                    //for the series:
                    //http://wiki.xbmc.org/index.php?title=XBMC_v12_(Frodo)_FAQ#Local_images
                    //poster
                    //banner
                    //fanart - we do not have the option in TVDB to get season specific fanart, so we'll leave that

                    string filenamePrefix = "";

                    if (!si.AutoAddFolderPerSeason)
                    {   // We have multiple seasons in the same folder
                        // We need to do slightly more work to come up with the filenamePrefix

                        filenamePrefix = "season";

                        if (snum == 0) filenamePrefix += "-specials";
                        else if (snum < 10) filenamePrefix += "0" + snum;
                        else filenamePrefix += snum;

                        filenamePrefix += "-";

                    }

                    FileInfo posterJPG = FileHelper.FileInFolder(folder, filenamePrefix + "poster.jpg");
                    if (forceRefresh || !posterJPG.Exists)
                    {
                        string path = si.TheSeries().GetSeasonBannerPath(snum);
                        if (!string.IsNullOrEmpty(path))
                            theActionList.Add(new ActionDownload(si, null, posterJPG, path));
                    }

                    FileInfo bannerJPG = FileHelper.FileInFolder(folder, filenamePrefix + "banner.jpg");
                    if (forceRefresh || !bannerJPG.Exists)
                    {
                        string path = si.TheSeries().GetSeasonWideBannerPath(snum);
                        if (!string.IsNullOrEmpty(path))
                            theActionList.Add(new ActionDownload(si, null, bannerJPG, path));
                    }
                }
                if (TVSettings.Instance.DownloadEdenImages())
                {
                    string filenamePrefix = "season";

                    if (snum == 0) filenamePrefix += "-specials";
                    else if (snum < 10) filenamePrefix += "0" + snum;
                    else filenamePrefix += snum;

                    FileInfo posterTbn = FileHelper.FileInFolder(si.AutoAddFolderBase, filenamePrefix + ".tbn");
                    if (forceRefresh || !posterTbn.Exists)
                    {
                        string path = si.TheSeries().GetSeasonBannerPath(snum);
                        if (!string.IsNullOrEmpty(path))
                            theActionList.Add(new ActionDownload(si, null, posterTbn, path));
                    }
                }
                return theActionList;
            }

            return base.ProcessSeason(si, folder, snum, forceRefresh);
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.EpTbNs || TVSettings.Instance.KodiImages)
            {
                ItemList theActionList = new ItemList();
                string ban = dbep.GetFilename();
                if (!string.IsNullOrEmpty(ban))
                {
                    string basefn = filo.Name;
                    basefn = basefn.Substring(0, basefn.Length - filo.Extension.Length); //remove extension
                    FileInfo imgtbn = FileHelper.FileInFolder(filo.Directory, basefn + ".tbn");
                    if (!imgtbn.Exists ||forceRefresh)
                        if (!(_doneTbn.Contains(imgtbn.FullName))){
                            theActionList.Add(new ActionDownload(dbep.Si, dbep, imgtbn, ban));
                            _doneTbn.Add(filo.FullName);
                        }
                }
                return theActionList;
            }
            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        public override void Reset()
        {
            _doneBannerJPG = new List<String>();
            _donePosterJPG = new List<String>();
            _doneFanartJPG = new List<String>();
            _doneTbn = new List<String>();
            base.Reset();
        }

    }
}
