using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TVRename.Exporter
{
    /// <summary>
    /// Exports a list of shows to a text file.
    /// </summary>
    /// <seealso cref="Exporter" />
    /// <inheritdoc />
    internal class ShowsTxt : Exporter
    {
        /// <inheritdoc />
        public override bool Active => TVSettings.Instance.ExportShowsTXT;

        /// <inheritdoc />
        protected override string Location => TVSettings.Instance.ExportShowsTXTTo;

        /// <summary>
        /// Runs the exporter, saving a text list of shows.
        /// </summary>
        /// <param name="shows">The list of shows.</param>
        public void Run(IEnumerable<ShowItem> shows)
        {
            if (!TVSettings.Instance.ExportShowsTXT) return;

            try
            {
                using (StreamWriter file = new StreamWriter(this.Location, false, Encoding.UTF8))
                {
                    foreach (ShowItem show in shows) file.WriteLine(show.ShowName);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                // TODO: Throw?
            }
        }
    }
}
