using System;
using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Actions;
using TVRename.Core.Models;
using TVRename.Core.TVDB;

namespace TVRename.Core.Metadata.Identifiers
{
    public class TVDBImageIdentifier : ImageIdentifier
    {
        public override Target Target
        {
            get
            {
                switch (this.ImageType)
                {
                    case ImageType.ShowPoster:
                    case ImageType.ShowBanner:
                    case ImageType.ShowFanart:
                        return Target.Show;
                    case ImageType.SeasonPoster:
                    case ImageType.SeasonBanner:
                        return Target.Season;
                    case ImageType.EpisodeThumbnail:
                        return Target.Episode;
                    default:
                        return Target.Show;
                }
            }
            set { }
        }
        
        protected override IAction ProcessShow(ProcessedShow show, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            string url;

            switch (this.ImageType)
            {
                case ImageType.ShowPoster:
                    url = show.Poster;
                    break;

                case ImageType.ShowBanner:
                    url = show.Banner;
                    break;

                case ImageType.ShowFanart:
                    url = show.Fanart;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DownloadAction(file, Client.ImageUrl + url);
        }

        protected override IAction ProcessSeason(ProcessedShow show, ProcessedSeason season, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            string url;

            switch (this.ImageType)
            {
                case ImageType.SeasonPoster:
                    url = show.Poster; // TODO: Season
                    break;

                case ImageType.SeasonBanner:
                    url = show.Banner; // TODO: Season
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DownloadAction(file, Client.ImageUrl + url);
        }

        protected override IAction ProcessEpisode(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file, bool force = false)
        {
            if (!force && file.Exists && show.LastUpdated <= file.LastWriteTime) return null;

            string url;

            switch (this.ImageType)
            {
                case ImageType.EpisodeThumbnail:
                    url = episode.Thumbnail;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DownloadAction(file, Client.ImageUrl + url);
        }
    }
}
