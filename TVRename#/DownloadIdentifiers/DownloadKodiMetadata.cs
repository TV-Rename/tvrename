using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Identifies missing "tvshow.nfo" show and "[episode].nfo" episode files and queues downloads.
    /// </summary>
    /// <seealso cref="DownloadIdentifier" />
    internal class DownloadKodiMetadata : DownloadIdentifier
    {
        public override string Extension => "nfo";

        public override DownloadType Type => DownloadType.Metadata;

        public override void MarkProcessed(FileInfo file)
        {
            if (file.FullName.EndsWith($".{this.Extension}", true, CultureInfo.InvariantCulture)) this.Processed.Add(file.FullName);
        }

        public override IEnumerable<Item> ProcessShow(ShowItem show, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.NFOShows) return Enumerable.Empty<Item>();

            FileInfo file = FileHelper.FileInFolder(show.AutoAdd_FolderBase, $"tvshow.{this.Extension}");
            
            if (!forceRefresh && file.Exists && show.TheSeries().Srv_LastUpdated <= TimeZone.Epoch(file.LastWriteTime) || this.Processed.Contains(file.FullName)) return Enumerable.Empty<Item>();

            this.Processed.Add(file.FullName);

            return new List<Item> { new ActionNFO(file, show) };
        }

        public override IEnumerable<Item> ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.NFOEpisodes) return Enumerable.Empty<Item>();

            FileInfo meta = FileHelper.FileInFolder(file.Directory, $"{Path.GetFileNameWithoutExtension(file.Name)}.{this.Extension}");

            if (!forceRefresh && meta.Exists && episode.Srv_LastUpdated <= TimeZone.Epoch(meta.LastWriteTime) || this.Processed.Contains(meta.FullName)) return Enumerable.Empty<Item>();

            this.Processed.Add(meta.FullName);

            return new List<Item> { new ActionNFO(meta, episode) };
        }
    }
}
