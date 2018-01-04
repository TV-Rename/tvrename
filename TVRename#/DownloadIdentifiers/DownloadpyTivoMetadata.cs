using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Identifies missing "[episode].[ext].txt" episode metadata files and queues generation.
    /// </summary>
    /// <seealso cref="DownloadIdentifier" />
    internal class DownloadpyTivoMetadata : DownloadIdentifier
    {
        public override string Extension => "txt";

        public override DownloadType Type => DownloadType.Metadata;

        public override IEnumerable<Item> ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.pyTivoMeta) return Enumerable.Empty<Item>();

            string folder = file.DirectoryName;
            if (TVSettings.Instance.pyTivoMetaSubFolder) folder += "\\.meta"; // TODO: Check path

            FileInfo meta = FileHelper.FileInFolder(folder, $"{file.Name}.{this.Extension}");

            if (this.Processed.Contains(meta.FullName) || meta.Exists || episode.Srv_LastUpdated < TimeZone.Epoch(meta.LastWriteTime) && !forceRefresh) return Enumerable.Empty<Item>();

            this.Processed.Add(meta.FullName);

            return new List<Item> { new ActionPyTivoMeta(meta, episode) };
        }
    }
}
