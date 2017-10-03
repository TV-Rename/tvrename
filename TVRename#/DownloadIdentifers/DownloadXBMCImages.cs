using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadXBMCImages : DownloadIdentifier
    {
        private List<string> donePosterJPG;
        private List<string> doneBannerJPG;
        private List<string> doneFanartJPG;
        private List<string> doneTBN;

        public DownloadXBMCImages() 
        {
            reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.downloadImage;
        }

        public override void notifyComplete(FileInfo file)
        {
            if (file.Name.EndsWith(".tbn", true, new CultureInfo("en")))
            {
                this.doneTBN.Add(file.FullName);
            } 
            base.notifyComplete(file);
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            //If we have XBMC New style images being downloaded then we want to check that 3 files exist
            //for the series:
            //http://wiki.xbmc.org/index.php?title=XBMC_v12_(Frodo)_FAQ#Local_images
            //poster
            //banner
            //fanart

            if (TVSettings.Instance.XBMCImages)
            {
                ItemList TheActionList = new ItemList();
                // base folder:
                if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (si.AllFolderLocations(false).Count > 0))
                {
                    FileInfo posterJPG = FileHelper.FileInFolder(si.AutoAdd_FolderBase, "poster.jpg");
                    FileInfo bannerJPG = FileHelper.FileInFolder(si.AutoAdd_FolderBase, "banner.jpg");
                    FileInfo fanartJPG = FileHelper.FileInFolder(si.AutoAdd_FolderBase, "fanart.jpg");

                    if ((forceRefresh || (!posterJPG.Exists)) && (!donePosterJPG.Contains(si.AutoAdd_FolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesPosterPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            TheActionList.Add(new ActionDownload(si, null, posterJPG, path, false));
                            donePosterJPG.Add(si.AutoAdd_FolderBase);
                        }
                    }

                    if ((forceRefresh || (!bannerJPG.Exists)) && (!doneBannerJPG.Contains(si.AutoAdd_FolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesWideBannerPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            TheActionList.Add(new ActionDownload(si, null, bannerJPG, path, false));
                            doneBannerJPG.Add(si.AutoAdd_FolderBase);
                        }
                    }

                    if ((forceRefresh || (!fanartJPG.Exists)) && (!doneFanartJPG.Contains(si.AutoAdd_FolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesFanartPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            TheActionList.Add(new ActionDownload(si, null, fanartJPG, path));
                            doneFanartJPG.Add(si.AutoAdd_FolderBase);
                        }
                    }
                }
                return TheActionList;
            }

            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.XBMCImages)
            {
                ItemList TheActionList = new ItemList();
                if (TVSettings.Instance.DownloadFrodoImages())
                {
                    //If we have XBMC New style images being downloaded then we want to check that 3 files exist
                    //for the series:
                    //http://wiki.xbmc.org/index.php?title=XBMC_v12_(Frodo)_FAQ#Local_images
                    //poster
                    //banner
                    //fanart - we do not have the option in TVDB to get season specific fanart, so we'll leave that

                    string filenamePrefix = "";

                    if (!si.AutoAdd_FolderPerSeason)
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
                            TheActionList.Add(new ActionDownload(si, null, posterJPG, path));
                    }

                    FileInfo bannerJPG = FileHelper.FileInFolder(folder, filenamePrefix + "banner.jpg");
                    if (forceRefresh || !bannerJPG.Exists)
                    {
                        string path = si.TheSeries().GetSeasonWideBannerPath(snum);
                        if (!string.IsNullOrEmpty(path))
                            TheActionList.Add(new ActionDownload(si, null, bannerJPG, path));
                    }
                }
                if (TVSettings.Instance.DownloadEdenImages())
                {
                    string filenamePrefix = "season";

                    if (snum == 0) filenamePrefix += "-specials";
                    else if (snum < 10) filenamePrefix += "0" + snum;
                    else filenamePrefix += snum;

                    FileInfo posterTBN = FileHelper.FileInFolder(si.AutoAdd_FolderBase, filenamePrefix + ".tbn");
                    if (forceRefresh || !posterTBN.Exists)
                    {
                        string path = si.TheSeries().GetSeasonBannerPath(snum);
                        if (!string.IsNullOrEmpty(path))
                            TheActionList.Add(new ActionDownload(si, null, posterTBN, path));
                    }
                }
                return TheActionList;
            }

            return base.ProcessSeason(si, folder, snum, forceRefresh);
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.EpTBNs || TVSettings.Instance.XBMCImages)
            {
                ItemList TheActionList = new ItemList();
                string ban = dbep.GetFilename();
                if (!string.IsNullOrEmpty(ban))
                {
                    string basefn = filo.Name;
                    basefn = basefn.Substring(0, basefn.Length - filo.Extension.Length); //remove extension
                    FileInfo imgtbn = FileHelper.FileInFolder(filo.Directory, basefn + ".tbn");
                    if (!imgtbn.Exists ||forceRefresh)
                        if (!(this.doneTBN.Contains(imgtbn.FullName))){
                            TheActionList.Add(new ActionDownload(dbep.SI, dbep, imgtbn, ban));
                            doneTBN.Add(filo.FullName);
                        }
                }
                return TheActionList;
            }
            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        public override void reset()
        {
            doneBannerJPG = new List<String>();
            donePosterJPG = new List<String>();
            doneFanartJPG = new List<String>();
            doneTBN = new List<String>();
            base.reset();
        }

    }
}
