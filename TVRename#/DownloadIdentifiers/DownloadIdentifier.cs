using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.DownloadIdentifiers
{
    /// <summary>
    /// Abstract base actions to identify and queue missing downloadable files.
    /// </summary>
    public abstract class DownloadIdentifier
    {
        /// <summary>
        /// Files which have already been processed or queued.
        /// </summary>
        protected readonly List<string> Processed = new List<string>();

        /// <summary>
        /// Gets the target file extension.
        /// </summary>
        /// <value>
        /// The file extension without a dot.
        /// </value>
        public abstract string Extension { get; }

        /// <summary>
        /// Gets the download file type.
        /// </summary>
        /// <value>
        /// The download file type.
        /// </value>
        public abstract DownloadType Type { get; }

        /// <summary>
        /// Add a file to the <see cref="Processed"/> list.
        /// </summary>
        /// <param name="file">The file to mark as processed.</param>
        public virtual void MarkProcessed(FileInfo file) { } // TODO: Improve name

        /// <summary>
        /// Processes a show.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <param name="forceRefresh">If set to <c>true</c> a refresh is forced.</param>
        /// <returns></returns>
        public virtual IEnumerable<Item> ProcessShow(ShowItem show, bool forceRefresh = false) => Enumerable.Empty<Item>();

        /// <summary>
        /// Processes a season.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <param name="folder">The show folder.</param>
        /// <param name="season">The season.</param>
        /// <param name="forceRefresh">If set to <c>true</c> a refresh is forced.</param>
        /// <returns></returns>
        public virtual IEnumerable<Item> ProcessSeason(ShowItem show, string folder, int season, bool forceRefresh = false) => Enumerable.Empty<Item>();

        /// <summary>
        /// Processes an episode.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="file">The episode file.</param>
        /// <param name="forceRefresh">If set to <c>true</c> a refresh is forced.</param>
        /// <returns></returns>
        public virtual IEnumerable<Item> ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh = false) => Enumerable.Empty<Item>();

        /// <summary>
        /// Resets this <see cref="Processed"/> list.
        /// </summary>
        public virtual void Reset()
        {
            this.Processed.Clear();
        }
    }
}
