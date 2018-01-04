using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Identifies missing "[episode].jpg" show files and queues downloads.
    /// </summary>
    /// <seealso cref="DownloadIdentifier" />
    internal sealed class DownloadEpisodeImage : DownloadIdentifier
    {
        public override string Extension => "jpg";

        public override DownloadType Type => DownloadType.Image;

        public override IEnumerable<Item> ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.EpJPGs) return Enumerable.Empty<Item>();

            FileInfo image = FileHelper.FileInFolder(file.Directory, $"{Path.GetFileNameWithoutExtension(file.Name)}.{this.Extension}");

            if (!forceRefresh && (this.Processed.Contains(image.FullName) || image.Exists)) return Enumerable.Empty<Item>();

            this.Processed.Add(image.FullName);

            List<Item> actions = new List<Item>();

            string url = episode.GetFilename();
            if (!string.IsNullOrEmpty(url)) actions.Add(new ActionDownload(episode.SI, episode, image, url, TVSettings.Instance.ShrinkLargeMede8erImages));

            return actions;
        }
    }
}
