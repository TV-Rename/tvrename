//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class DownloadKodiImages : DownloadIdentifier
    {
        private List<string> donePosterJpg = new List<string>();
        private List<string> doneBannerJpg = new List<string>();
        private List<string> doneFanartJpg = new List<string>();
        private List<string> doneTbn = new List<string>();

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

        public override ItemList? ProcessMovie(MovieConfiguration movie, FileInfo file, bool forceRefresh)
        {
            //If we have KODI New style images being downloaded then we want to check that 3 files exist
            //for the cachedSeries:
            //http://wiki.xbmc.org/index.php?title=XBMC_v12_(Frodo)_FAQ#Local_images
            //poster
            //banner
            //fanart

            if (TVSettings.Instance.KODIImages)
            {
                ItemList theActionList = new ItemList();
                string baseFileName = file.MovieFileNameBase();

                FileInfo posterJpg = FileHelper.FileInFolder(file.Directory, baseFileName + "-poster.jpg");
                FileInfo bannerJpg = FileHelper.FileInFolder(file.Directory, baseFileName + "-banner.jpg");
                FileInfo fanartJpg = FileHelper.FileInFolder(file.Directory, baseFileName + "-fanart.jpg");

                if ((forceRefresh || !posterJpg.Exists) && !donePosterJpg.Contains(file.Directory.FullName))
                {
                    string path = movie.CachedMovie?.PosterUrl;
                    if (!string.IsNullOrEmpty(path))
                    {
                        theActionList.Add(new ActionDownloadImage(movie, null, posterJpg, path, false));
                        donePosterJpg.Add(file.Directory.FullName);
                    }
                }

                if ((forceRefresh || !bannerJpg.Exists) && !doneBannerJpg.Contains(file.Directory.FullName))
                {
                    string path = movie.CachedMovie?.Images(MediaImage.ImageType.wideBanner).FirstOrDefault()?.ImageUrl ?? string.Empty; //todo link up movir banner url movie.CachedMovie?.BannerUrl;
                    if (!string.IsNullOrEmpty(path))
                    {
                        theActionList.Add(new ActionDownloadImage(movie, null, bannerJpg, path, false));
                        doneBannerJpg.Add(file.Directory.FullName);
                    }
                }

                if ((forceRefresh || !fanartJpg.Exists) && !doneFanartJpg.Contains(file.Directory.FullName))
                {
                    string path = movie.CachedMovie?.FanartUrl;
                    if (!string.IsNullOrEmpty(path))
                    {
                        theActionList.Add(new ActionDownloadImage(movie, null, fanartJpg, path));
                        doneFanartJpg.Add(file.Directory.FullName);
                    }
                }
                return theActionList;
            }

            return base.ProcessMovie(movie, file, forceRefresh);
        }

        public override ItemList? ProcessShow(ShowConfiguration si, bool forceRefresh)
        {
            //If we have KODI New style images being downloaded then we want to check that 3 files exist
            //for the cachedSeries:
            //http://wiki.xbmc.org/index.php?title=XBMC_v12_(Frodo)_FAQ#Local_images
            //poster
            //banner
            //fanart

            if (TVSettings.Instance.KODIImages)
            {
                ItemList theActionList = new ItemList();
                // base folder:
                if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && si.AllFolderLocations(false).Count > 0)
                {
                    FileInfo posterJpg = FileHelper.FileInFolder(si.AutoAddFolderBase, "poster.jpg");
                    FileInfo bannerJpg = FileHelper.FileInFolder(si.AutoAddFolderBase, "banner.jpg");
                    FileInfo fanartJpg = FileHelper.FileInFolder(si.AutoAddFolderBase, "fanart.jpg");

                    if ((forceRefresh || !posterJpg.Exists) && !donePosterJpg.Contains(si.AutoAddFolderBase))
                    {
                        string path = si.CachedShow?.GetSeriesPosterPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownloadImage(si, null, posterJpg, path, false));
                            donePosterJpg.Add(si.AutoAddFolderBase);
                        }
                    }

                    if ((forceRefresh || !bannerJpg.Exists) && !doneBannerJpg.Contains(si.AutoAddFolderBase))
                    {
                        string path = si.CachedShow?.GetSeriesWideBannerPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownloadImage(si, null, bannerJpg, path, false));
                            doneBannerJpg.Add(si.AutoAddFolderBase);
                        }
                    }

                    if ((forceRefresh || !fanartJpg.Exists) && !doneFanartJpg.Contains(si.AutoAddFolderBase))
                    {
                        string path = si.CachedShow?.GetSeriesFanartPath();
                        if (!string.IsNullOrEmpty(path))
                        {
                            theActionList.Add(new ActionDownloadImage(si, null, fanartJpg, path));
                            doneFanartJpg.Add(si.AutoAddFolderBase);
                        }
                    }
                }
                return theActionList;
            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.KODIImages)
            {
                ItemList theActionList = new ItemList();
                //If we have KODI New style images being downloaded then we want to check that 3 files exist
                //for the cachedSeries:
                //http://wiki.xbmc.org/index.php?title=XBMC_v12_(Frodo)_FAQ#Local_images
                //poster
                //banner
                //fanart - we do not have the option in TVDB to get season specific fanart, so we'll leave that

                string filenamePrefix = string.Empty;

                if (si.InOneFolder())
                {   // We have multiple seasons in the same folder
                    // We need to do slightly more work to come up with the filenamePrefix

                    filenamePrefix = "season";

                    if (snum == 0)
                    {
                        filenamePrefix += "-specials";
                    }
                    else if (snum < 10)
                    {
                        filenamePrefix += "0" + snum;
                    }
                    else
                    {
                        filenamePrefix += snum;
                    }

                    filenamePrefix += "-";
                }
                FileInfo posterJpg = FileHelper.FileInFolder(folder, filenamePrefix + "poster.jpg");
                if (forceRefresh || !posterJpg.Exists)
                {
                    string path = si.CachedShow?.GetSeasonBannerPath(snum);
                    if (!string.IsNullOrEmpty(path))
                    {
                        theActionList.Add(new ActionDownloadImage(si, null, posterJpg, path));
                    }
                }

                FileInfo bannerJpg = FileHelper.FileInFolder(folder, filenamePrefix + "banner.jpg");
                if (forceRefresh || !bannerJpg.Exists)
                {
                    string path = si.CachedShow?.GetSeasonWideBannerPath(snum);
                    if (!string.IsNullOrEmpty(path))
                    {
                        theActionList.Add(new ActionDownloadImage(si, null, bannerJpg, path));
                    }
                }
                return theActionList;
            }
            return base.ProcessSeason(si, folder, snum, forceRefresh);
        }

        public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
        {
            if (TVSettings.Instance.EpTBNs)
            {
                ItemList theActionList = new ItemList();
                if (episode.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
                {
                    //We have a merged episode, so we'll also download the images for the episodes had they been separate.
                    foreach (Episode sourceEp in episode.SourceEpisodes)
                    {
                        string foldername = file.DirectoryName;
                        string filename = TVSettings.Instance.FilenameFriendly(episode.Show, sourceEp);
                        ActionDownloadImage b = DoEpisode(episode.Show, sourceEp, new FileInfo(foldername + "/" + filename), ".jpg", forceRefresh);
                        if (b != null)
                        {
                            theActionList.Add(b);
                        }
                    }
                }
                else
                {
                    ActionDownloadImage a = DoEpisode(episode.Show, episode, file, ".tbn", forceRefresh);
                    if (a != null)
                    {
                        theActionList.Add(a);
                    }
                }
                return theActionList;
            }
            return base.ProcessEpisode(episode, file, forceRefresh);
        }

        private ActionDownloadImage? DoEpisode(ShowConfiguration si, [NotNull] Episode ep, FileInfo filo, string extension, bool forceRefresh)
        {
            string ban = ep.Filename;
            if (string.IsNullOrEmpty(ban))
            {
                return null;
            }

            string basefn = filo.RemoveExtension();
            FileInfo imgtbn = FileHelper.FileInFolder(filo.Directory, basefn + extension);
            if (imgtbn.Exists && !forceRefresh)
            {
                return null;
            }

            if (doneTbn.Contains(imgtbn.FullName))
            {
                return null;
            }

            doneTbn.Add(imgtbn.FullName);
            return new ActionDownloadImage(si, ep is ProcessedEpisode episode ? episode : new ProcessedEpisode(ep, si), imgtbn, ban);
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
