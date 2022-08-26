//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename;

internal class DownloadKodiImages : DownloadIdentifier
{
    private List<string> donePosterJpg = new();
    private List<string> doneBannerJpg = new();
    private List<string> doneFanartJpg = new();
    private List<string> doneThumbnails = new();

    public DownloadKodiImages() => Reset();

    public override DownloadType GetDownloadType() => DownloadType.downloadImage;

    public override void NotifyComplete(FileInfo file)
    {
        doneThumbnails.Add(file.FullName);
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
            ItemList theActionList = new();
            string baseFileName = file.MovieFileNameBase();

            FileInfo posterJpg = movie.IsDvdBluRay()
                ? FileHelper.FileInFolder(file.Directory.Parent, "poster.jpg")
                : FileHelper.FileInFolder(file.Directory, baseFileName + "-poster.jpg");
            FileInfo bannerJpg = movie.IsDvdBluRay()
                ? FileHelper.FileInFolder(file.Directory.Parent, "banner.jpg")
                : FileHelper.FileInFolder(file.Directory, baseFileName + "-banner.jpg");
            FileInfo fanartJpg = movie.IsDvdBluRay()
                ? FileHelper.FileInFolder(file.Directory.Parent, "fanart.jpg")
                : FileHelper.FileInFolder(file.Directory, baseFileName + "-fanart.jpg");

            if ((forceRefresh || !posterJpg.Exists) && !donePosterJpg.Contains(file.Directory.FullName))
            {
                string? path = movie.CachedMovie?.PosterUrl;
                if (!string.IsNullOrEmpty(path))
                {
                    theActionList.Add(new ActionDownloadMovieImage(movie, posterJpg, path, false));
                    donePosterJpg.Add(file.Directory.FullName);
                }
            }

            if ((forceRefresh || !bannerJpg.Exists) && !doneBannerJpg.Contains(file.Directory.FullName))
            {
                string? path = movie.CachedMovie?.Images(MediaImage.ImageType.wideBanner).FirstOrDefault()?.ImageUrl;
                if (!string.IsNullOrEmpty(path))
                {
                    theActionList.Add(new ActionDownloadMovieImage(movie, bannerJpg, path, false));
                    doneBannerJpg.Add(file.Directory.FullName);
                }
            }

            if ((forceRefresh || !fanartJpg.Exists) && !doneFanartJpg.Contains(file.Directory.FullName))
            {
                string? path = movie.CachedMovie?.FanartUrl;
                if (!string.IsNullOrEmpty(path))
                {
                    theActionList.Add(new ActionDownloadMovieImage(movie, fanartJpg, path));
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
            ItemList theActionList = new();
            // base folder:
            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && si.AllFolderLocations(false).Any())
            {
                FileInfo posterJpg = FileHelper.FileInFolder(si.AutoAddFolderBase, "poster.jpg");
                FileInfo bannerJpg = FileHelper.FileInFolder(si.AutoAddFolderBase, "banner.jpg");
                FileInfo fanartJpg = FileHelper.FileInFolder(si.AutoAddFolderBase, "fanart.jpg");

                if ((forceRefresh || !posterJpg.Exists) && !donePosterJpg.Contains(si.AutoAddFolderBase))
                {
                    string? path = si.CachedShow?.GetSeriesPosterPath();
                    if (!string.IsNullOrEmpty(path))
                    {
                        theActionList.Add(new ActionDownloadTvShowImage(si,  posterJpg, path, false));
                        donePosterJpg.Add(si.AutoAddFolderBase);
                    }
                }

                if ((forceRefresh || !bannerJpg.Exists) && !doneBannerJpg.Contains(si.AutoAddFolderBase))
                {
                    string? path = si.CachedShow?.GetSeriesWideBannerPath();
                    if (!string.IsNullOrEmpty(path))
                    {
                        theActionList.Add(new ActionDownloadTvShowImage(si, bannerJpg, path, false));
                        doneBannerJpg.Add(si.AutoAddFolderBase);
                    }
                }

                if ((forceRefresh || !fanartJpg.Exists) && !doneFanartJpg.Contains(si.AutoAddFolderBase))
                {
                    string? path = si.CachedShow?.GetSeriesFanartPath();
                    if (!string.IsNullOrEmpty(path))
                    {
                        theActionList.Add(new ActionDownloadTvShowImage(si, fanartJpg, path));
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
            ItemList theActionList = new();
            //If we have KODI New style images being downloaded then we want to check that 3 files exist
            //for the cachedSeries:
            //http://wiki.xbmc.org/index.php?title=XBMC_v12_(Frodo)_FAQ#Local_images
            //poster
            //banner
            //fanart - we do not have the option in TVDB to get season specific fanart, so we'll leave that
            //UPDATE - see https://kodi.wiki/view/Artwork/Season

            string filenamePrefix = $"season{SeasonLabel(snum)}-";
            string folderToUse = si.InOneFolder() ? folder : GetParentFolder(folder);

            FileInfo posterJpg = FileHelper.FileInFolder(folderToUse, filenamePrefix + "poster.jpg");
            if (forceRefresh || !posterJpg.Exists)
            {
                string? path = si.CachedShow?.GetSeasonBannerPath(snum);
                if (!string.IsNullOrEmpty(path))
                {
                    theActionList.Add(new ActionDownloadSeasonImage(si, snum, posterJpg, path));
                }
            }

            FileInfo bannerJpg = FileHelper.FileInFolder(folderToUse, filenamePrefix + "banner.jpg");
            if (forceRefresh || !bannerJpg.Exists)
            {
                string? path = si.CachedShow?.GetSeasonWideBannerPath(snum);
                if (!string.IsNullOrEmpty(path))
                {
                    theActionList.Add(new ActionDownloadSeasonImage(si, snum, bannerJpg, path));
                }
            }
            return theActionList;
        }
        return base.ProcessSeason(si, folder, snum, forceRefresh);
    }

    private static string GetParentFolder(string folder)
    {
        DirectoryInfo child = new(folder);
        return child.Parent.FullName;
    }

    private static string SeasonLabel(int snum)
    {
        return snum switch
        {
            0 => "-specials",
            < 10 => "0" + snum,
            _ => snum.ToString()
        };
    }

    public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
    {
        if (TVSettings.Instance.EpThumbnails)
        {
            ItemList theActionList = new();
            if (episode.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
            {
                //We have a merged episode, so we'll also download the images for the episodes had they been separate.
                foreach (Episode sourceEp in episode.SourceEpisodes)
                {
                    string foldername = file.DirectoryName;
                    string filename = TVSettings.Instance.FilenameFriendly(episode.Show, sourceEp);
                    ActionDownloadImage? downloadImage = DoEpisode(episode.Show, sourceEp, new FileInfo(foldername + "/" + filename + ".jpg"), "-thumb.jpg", forceRefresh);
                    if (downloadImage is not null)
                    {
                        theActionList.Add(downloadImage);
                    }
                }
            }

            ActionDownloadImage? actionDownloadImage = DoEpisode(episode.Show, episode, file, "-thumb.jpg", forceRefresh);
            if (actionDownloadImage is not null)
            {
                theActionList.Add(actionDownloadImage);
            }

            return theActionList;
        }
        return base.ProcessEpisode(episode, file, forceRefresh);
    }

    private ActionDownloadImage? DoEpisode(ShowConfiguration si, Episode ep, FileInfo filo, string extension, bool forceRefresh)
    {
        string? ban = ep.Filename;
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

        if (doneThumbnails.Contains(imgtbn.FullName))
        {
            return null;
        }

        doneThumbnails.Add(imgtbn.FullName);
        return new ActionDownloadImage(si, ep as ProcessedEpisode ?? new ProcessedEpisode(ep, si), imgtbn, ban);
    }

    public sealed override void Reset()
    {
        doneBannerJpg = new List<string>();
        donePosterJpg = new List<string>();
        doneFanartJpg = new List<string>();
        doneThumbnails = new List<string>();
    }
}
