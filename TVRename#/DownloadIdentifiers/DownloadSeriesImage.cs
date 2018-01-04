using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Identifies missing "series.jpg" season files and queues downloads.
    /// </summary>
    /// <seealso cref="DownloadIdentifier" />
    internal class DownloadSeriesImage : DownloadIdentifier
    {
        public override string Extension => "jpg";

        public override DownloadType Type => DownloadType.Image;

        public override IEnumerable<Item> ProcessSeason(ShowItem show, string folder, int season, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.SeriesJpg) return Enumerable.Empty<Item>();

            FileInfo file = FileHelper.FileInFolder(folder, $"series.{this.Extension}");

            if (!forceRefresh && (this.Processed.Contains(file.FullName) || file.Exists)) return Enumerable.Empty<Item>();

            this.Processed.Add(file.FullName);

            List<Item> actions = new List<Item>();

            string url = show.TheSeries().GetSeasonBannerPath(season);
            if (!string.IsNullOrEmpty(url)) actions.Add(new ActionDownload(show, null, file, url, TVSettings.Instance.ShrinkLargeMede8erImages));

            return actions;
        }
    }
}
