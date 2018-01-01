using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Identifies missing "series.xml" and "view.xml" show, "view.xml" season and "[episode].xml" episode metadata files and queues generation.
    /// </summary>
    /// <seealso cref="DownloadIdentifier" />
    // ReSharper disable once InconsistentNaming
    internal class DownloadMede8erMetadata : DownloadIdentifier
    {
        public override string Extension => "xml";

        public override DownloadType Type => DownloadType.Metadata;

        public override IEnumerable<Item> ProcessShow(ShowItem show, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.Mede8erXML) return Enumerable.Empty<Item>();

            FileInfo series = FileHelper.FileInFolder(show.AutoAdd_FolderBase, $"series.{this.Extension}");
            FileInfo view = FileHelper.FileInFolder(show.AutoAdd_FolderBase, $"view.{this.Extension}");

            List<Item> actions = new List<Item>();

            if (forceRefresh || !this.Processed.Contains(series.FullName) || !series.Exists || show.TheSeries().Srv_LastUpdated > TimeZone.Epoch(series.LastWriteTime)) actions.Add(new ActionMede8erXML(series, show));
            if (!this.Processed.Contains(view.FullName) && !view.Exists) actions.Add(new ActionMede8erViewXML(view, show));

            this.Processed.Add(series.FullName);

            return actions;
        }

        public override IEnumerable<Item> ProcessSeason(ShowItem show, string folder, int season, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.Mede8erXML) return Enumerable.Empty<Item>();

            FileInfo view = FileHelper.FileInFolder(folder, $"view.{this.Extension}");

            // Never need to regenerate the file
            if (this.Processed.Contains(view.FullName) || view.Exists) return Enumerable.Empty<Item>();

            this.Processed.Add(view.FullName);

            return new List<Item> { new ActionMede8erViewXML(view, show, season) };
        }

        public override IEnumerable<Item> ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh = false)
        {
            if (!TVSettings.Instance.Mede8erXML) return Enumerable.Empty<Item>();

            FileInfo meta = FileHelper.FileInFolder(file.Directory, $"{Path.GetFileNameWithoutExtension(file.Name)}.{this.Extension}");

            // TODO: Check if updated?
            if (this.Processed.Contains(meta.FullName) || meta.Exists && !forceRefresh) return Enumerable.Empty<Item>();

            this.Processed.Add(meta.FullName);

            return new List<Item> { new ActionMede8erXML(meta, episode) };
        }
    }
}
