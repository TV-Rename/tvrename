using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Identifies missing "fanart.jpg" show files and queues downloads.
    /// </summary>
    /// <seealso cref="DownloadIdentifier" />
    internal sealed class DownloadFanartImage : DownloadIdentifier
    {
        public override string Extension => "jpg";

        public override DownloadType Type => DownloadType.Image;

        public override IEnumerable<Item> ProcessShow(ShowItem show, bool forceRefresh = false)
        {
            // Only process if the fanart option is enabled. If the KODI option is enabled then let it do the work.
            if (!TVSettings.Instance.FanArtJpg || TVSettings.Instance.KODIImages) return Enumerable.Empty<Item>();

            FileInfo file = FileHelper.FileInFolder(show.AutoAdd_FolderBase, $"fanart.{this.Extension}");

            if (!forceRefresh && (this.Processed.Contains(file.FullName) || file.Exists)) return Enumerable.Empty<Item>();

            this.Processed.Add(file.FullName);

            List<Item> actions = new List<Item>();

            string url = show.TheSeries().GetSeriesFanartPath();
            if (!string.IsNullOrEmpty(url)) actions.Add(new ActionDownload(show, null, file, url, false));

            return actions;
        }
    }
}
