using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TVRename.Exporter
{
    /// <summary>
    /// Base exporter for saving upcoming episode data.
    /// </summary>
    /// <seealso cref="Exporter" />
    /// <inheritdoc />
    internal abstract class UpcomingExporter : Exporter
    {
        protected readonly TVDoc Doc;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpcomingExporter"/> class.
        /// </summary>
        /// <param name="doc">The settings document.</param>
        protected UpcomingExporter(TVDoc doc)
        {
            this.Doc = doc;
        }

        /// <summary>
        /// Runs the exporter, saving a text list of upcoming episodes.
        /// </summary>
        public void Run()
        {
            if (!this.Active) return;

            using (StreamWriter file = new StreamWriter(this.Location)) file.Write(Produce());

            Logger.Info($"Exported upcoming to: {this.Location}");
        }

        /// <summary>
        /// Generates and saves a list of upcoming episodes to the specified stream.
        /// </summary>
        /// <param name="stream">The file stream to write to.</param>
        /// <param name="episodes">The list episodes.</param>
        /// <returns><c>true</c> is successful.</returns>
        protected abstract bool Generate(Stream stream, IEnumerable<ProcessedEpisode> episodes);

        /// <summary>
        /// Finds upcoming episodes and produces the the list.
        /// </summary>
        /// <returns>List of upcoming episodes.</returns>
        private string Produce()
        {
            try
            {
                // dirty try/catch to "fix" the problem that a share can disappear during a sleep/resume, and
                // when windows restarts, the share isn't "back" before this timer times out and fires
                // windows explorer tends to lose explorer windows on shares when slept/resumed, too, so its not
                // just me :P

                // TODO: Is this still needed?

                List<ProcessedEpisode> episodes = this.Doc.NextNShows(TVSettings.Instance.ExportRSSMaxShows, TVSettings.Instance.ExportRSSDaysPast, TVSettings.Instance.ExportRSSMaxDays);

                if (episodes != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        if (Generate(stream, episodes)) return Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                // TODO: Throw?
            }

            return string.Empty;
        }
    }
}
