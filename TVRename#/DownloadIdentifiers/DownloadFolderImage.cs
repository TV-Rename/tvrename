using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Identifies missing "folder.jpg" show and season files and queues downloads.
    /// </summary>
    /// <seealso cref="DownloadIdentifier" />
    internal class DownloadFolderImage : DownloadIdentifier
    {
        public override string Extension => "jpg";

        public override DownloadType Type => DownloadType.Image;

        public override IEnumerable<Item> ProcessShow(ShowItem show, bool forceRefresh = false)
        {
            return Process(show, show.AutoAdd_FolderBase, show.TheSeries().GetSeriesPosterPath(), forceRefresh);
        }

        public override IEnumerable<Item> ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh = false)
        {
            return Process(si, folder, si.TheSeries().GetSeasonBannerPath(snum), forceRefresh);
        }

        private IEnumerable<Item> Process(ShowItem show, string folder, string seasonUrl, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.FolderJpg) return Enumerable.Empty<Item>();

            FileInfo file = FileHelper.FileInFolder(folder, $"folder.{this.Extension}");

            if (this.Processed.Contains(file.FullName) || file.Exists && !forceRefresh) return Enumerable.Empty<Item>();

            string url;

            if (TVSettings.Instance.SeasonSpecificFolderJPG())
            {
                // Use season image
                url = seasonUrl;
            }
            else
            {
                // Use the show image
                url = show.TheSeries().GetImage(TVSettings.Instance.ItemForFolderJpg());
            }

            List<Item> actions = new List<Item>();

            if (!string.IsNullOrEmpty(url)) actions.Add(new ActionDownload(show, null, file, url, TVSettings.Instance.ShrinkLargeMede8erImages));

            this.Processed.Add(file.FullName);

            return actions;
        }
    }
}
