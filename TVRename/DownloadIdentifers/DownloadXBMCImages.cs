using System.Collections.Generic;
using System.Globalization;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


namespace TVRename
{
    class DownloadKodiImages : DownloadIdentifier
    {
        private List<string> donePosterJpg;
        private List<string> doneBannerJpg;
        private List<string> doneFanartJpg;
        private List<string> doneTbn;

        public DownloadKodiImages() => Reset();
        
        public override DownloadType GetDownloadType() => DownloadType.downloadImage;
        
        public override void NotifyComplete(FileInfo file)
        {
            if (file.Name.EndsWith(".tbn", true, new CultureInfo("en")))
            {
                doneTbn.Add(file.FullName);
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

            if (TVSettings.Instance.KODIImages)
            {
                ItemList theActionList = new ItemList();
                // base folder:
                if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (si.AllFolderLocations(false).Count > 0))
                {
                    FileInfo posterJpg = FileHelper.FileInFolder(si.AutoAdd_FolderBase, "poster.jpg");
                    FileInfo bannerJpg = FileHelper.FileInFolder(si.AutoAdd_FolderBase, "banner.jpg");
                    FileInfo fanartJpg = FileHelper.FileInFolder(si.AutoAdd_FolderBase, "fanart.jpg");

                    if ((forceRefresh || (!posterJpg.Exists)) && (!donePosterJpg.Contains(si.AutoAdd_FolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesPosterPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownloadImage(si, null, posterJpg, path, false));
                            donePosterJpg.Add(si.AutoAdd_FolderBase);
                        }
                    }

                    if ((forceRefresh || (!bannerJpg.Exists)) && (!doneBannerJpg.Contains(si.AutoAdd_FolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesWideBannerPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownloadImage(si, null, bannerJpg, path, false));
                            doneBannerJpg.Add(si.AutoAdd_FolderBase);
                        }
                    }

                    if ((forceRefresh || (!fanartJpg.Exists)) && (!doneFanartJpg.Contains(si.AutoAdd_FolderBase)))
                    {
                        string path = si.TheSeries().GetSeriesFanartPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownloadImage(si, null, fanartJpg, path));
                            doneFanartJpg.Add(si.AutoAdd_FolderBase);
                        }
                    }
                }
                return theActionList;
            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.KODIImages)
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

                    if (!si.AutoAdd_FolderPerSeason)
                    {   // We have multiple seasons in the same folder
                        // We need to do slightly more work to come up with the filenamePrefix

                        filenamePrefix = "season";

                        if (snum == 0) filenamePrefix += "-specials";
                        else if (snum < 10) filenamePrefix += "0" + snum;
                        else filenamePrefix += snum;

                        filenamePrefix += "-";
                    }
                    FileInfo posterJpg = FileHelper.FileInFolder(folder, filenamePrefix + "poster.jpg");
                    if (forceRefresh || !posterJpg.Exists)
                    {
                        string path = si.TheSeries().GetSeasonBannerPath(snum);
                        if (!string.IsNullOrEmpty(path))
                            theActionList.Add(new ActionDownloadImage(si, null, posterJpg, path));
                    }

                    FileInfo bannerJpg = FileHelper.FileInFolder(folder, filenamePrefix + "banner.jpg");
                    if (forceRefresh || !bannerJpg.Exists)
                    {
                        string path = si.TheSeries().GetSeasonWideBannerPath(snum);
                        if (!string.IsNullOrEmpty(path))
                            theActionList.Add(new ActionDownloadImage(si, null, bannerJpg, path));
                    }
                }
                if (TVSettings.Instance.DownloadEdenImages())
                {
                    string filenamePrefix = "season";

                    if (snum == 0) filenamePrefix += "-specials";
                    else if (snum < 10) filenamePrefix += "0" + snum;
                    else filenamePrefix += snum;

                    FileInfo posterTbn = FileHelper.FileInFolder(si.AutoAdd_FolderBase, filenamePrefix + ".tbn");
                    if (forceRefresh || !posterTbn.Exists)
                    {
                        string path = si.TheSeries().GetSeasonBannerPath(snum);
                        if (!string.IsNullOrEmpty(path))
                            theActionList.Add(new ActionDownloadImage(si, null, posterTbn, path));
                    }
                }
                return theActionList;
            }
            return base.ProcessSeason(si, folder, snum, forceRefresh);
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.EpTBNs || TVSettings.Instance.KODIImages)
            {
                ItemList theActionList = new ItemList();
                if (dbep.type == ProcessedEpisode.ProcessedEpisodeType.merged)
                {
                    //We have a merged episode, so we'll also download the images for the episodes had they been separate.
                    foreach (Episode sourceEp in dbep.sourceEpisodes)
                    {
                        string foldername = filo.DirectoryName;
                        string filename =
                            TVSettings.Instance.FilenameFriendly(
                                TVSettings.Instance.NamingStyle.GetTargetEpisodeName(sourceEp,dbep.SI.ShowName, dbep.SI.GetTimeZone(), dbep.SI.DVDOrder));
                        ActionDownloadImage b = DoEpisode(dbep.SI,sourceEp,new FileInfo(foldername+"/"+filename), ".jpg", forceRefresh);
                        if (b != null) theActionList.Add(b);
                    }
                }
                else
                {
                    ActionDownloadImage a = DoEpisode(dbep.SI, dbep, filo, ".tbn", forceRefresh);
                    if (a != null) theActionList.Add(a);
                }
                return theActionList;
            }
            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        private ActionDownloadImage DoEpisode(ShowItem si, Episode ep, FileInfo filo,string extension, bool forceRefresh)
        {
            string ban = ep.Filename;
            if (!string.IsNullOrEmpty(ban))
            {
                string basefn = filo.RemoveExtension();
                FileInfo imgtbn = FileHelper.FileInFolder(filo.Directory, basefn + extension);
                if (!imgtbn.Exists || forceRefresh)
                    if (!(doneTbn.Contains(imgtbn.FullName)))
                    {
                        doneTbn.Add(imgtbn.FullName);

                        return new ActionDownloadImage(si, (ep is ProcessedEpisode) ? (ProcessedEpisode)ep  : new ProcessedEpisode(ep,si ), imgtbn, ban);
                    }
            }
            return null;
        }

        public sealed override void Reset()
        {
            doneBannerJpg = new List<string>();
            donePosterJpg = new List<string>();
            doneFanartJpg = new List<string>();
            doneTbn = new List<string>();
        }
    }
}
