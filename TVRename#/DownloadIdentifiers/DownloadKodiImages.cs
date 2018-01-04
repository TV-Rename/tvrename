using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Identifies missing "poster.jpg", "banner.jpg", "fanart.jpg" show, "poster.jpg", "banner.jpg" season and "[episode].tbn" episode files and queues downloads.
    /// See http://kodi.wiki/view/XBMC_v12_(Frodo)_FAQ#Local_images
    /// </summary>
    /// <seealso cref="DownloadIdentifier" />
    internal class DownloadKodiImages : DownloadIdentifier
    {
        public override string Extension => "jpg";

        public override DownloadType Type => DownloadType.Image;

        public override void MarkProcessed(FileInfo file)
        {
            if (file.Name.EndsWith(".tbn", true, CultureInfo.InvariantCulture)) this.Processed.Add(file.FullName);
        }

        public override IEnumerable<Item> ProcessShow(ShowItem show, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.KODIImages) return Enumerable.Empty<Item>();

            if (string.IsNullOrEmpty(show.AutoAdd_FolderBase) || show.AllFolderLocations(false).Count <= 0) return Enumerable.Empty<Item>(); // TODO: Needed?

            FileInfo poster = FileHelper.FileInFolder(show.AutoAdd_FolderBase, $"poster.{this.Extension}");
            FileInfo banner = FileHelper.FileInFolder(show.AutoAdd_FolderBase, $"banner.{this.Extension}");
            FileInfo fanart = FileHelper.FileInFolder(show.AutoAdd_FolderBase, $"fanart.{this.Extension}");

            List<Item> actions = new List<Item>();

            if ((forceRefresh || !poster.Exists) && !this.Processed.Contains(poster.FullName))
            {
                this.Processed.Add(poster.FullName);

                string path = show.TheSeries().GetSeriesPosterPath();

                if (!string.IsNullOrEmpty(path)) actions.Add(new ActionDownload(show, null, poster, path, false));
            }

            if ((forceRefresh || !banner.Exists) && !this.Processed.Contains(banner.FullName))
            {
                this.Processed.Add(banner.FullName);

                string path = show.TheSeries().GetSeriesWideBannerPath();

                if (!string.IsNullOrEmpty(path)) actions.Add(new ActionDownload(show, null, banner, path, false));
            }

            if ((forceRefresh || !fanart.Exists) && !this.Processed.Contains(fanart.FullName))
            {
                this.Processed.Add(fanart.FullName);

                string path = show.TheSeries().GetSeriesFanartPath();

                if (!string.IsNullOrEmpty(path)) actions.Add(new ActionDownload(show, null, fanart, path));
            }

            return actions;
        }

        public override IEnumerable<Item> ProcessSeason(ShowItem show, string folder, int season, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.KODIImages) return Enumerable.Empty<Item>();

            string prefix = string.Empty;

            if (!show.AutoAdd_FolderPerSeason)
            {
                // Multiple seasons in the same folder

                prefix = "season";

                if (season == 0) prefix += "-specials";
                else if (season < 10) prefix += "0" + season;
                else prefix += season;

                prefix += "-";
            }

            FileInfo poster = FileHelper.FileInFolder(folder, prefix + $"poster.{this.Extension}");
            FileInfo banner = FileHelper.FileInFolder(folder, prefix + $"banner.{this.Extension}");
            // fanart - TheTVDB has no way to get season specific fanart

            List<Item> actions = new List<Item>();

            if (forceRefresh || !poster.Exists)
            {
                this.Processed.Add(poster.FullName);

                string path = show.TheSeries().GetSeasonBannerPath(season);

                if (!string.IsNullOrEmpty(path)) actions.Add(new ActionDownload(show, null, poster, path));
            }

            if (forceRefresh || !banner.Exists)
            {
                this.Processed.Add(banner.FullName);

                string path = show.TheSeries().GetSeasonWideBannerPath(season);

                if (!string.IsNullOrEmpty(path)) actions.Add(new ActionDownload(show, null, banner, path));
            }

            return actions;
        }

        public override IEnumerable<Item> ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.EpTBNs && !TVSettings.Instance.KODIImages) return Enumerable.Empty<Item>();

            FileInfo meta = FileHelper.FileInFolder(file.Directory, $"{Path.GetFileNameWithoutExtension(file.Name)}.tbn");

            if (!forceRefresh && meta.Exists || this.Processed.Contains(meta.FullName)) return Enumerable.Empty<Item>();

            this.Processed.Add(file.FullName);

            string url = episode.GetFilename();

            if (string.IsNullOrEmpty(url)) return Enumerable.Empty<Item>();

            return new List<Item> { new ActionDownload(episode.SI, episode, meta, url) };
        }
    }
}
