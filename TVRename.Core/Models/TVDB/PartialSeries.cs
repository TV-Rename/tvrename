using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class PartialSeries
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public List<string> Aliases { get; set; } = new List<string>();

        public string Banner { get; set; }

        public string FirstAired { get; set; }

        public int Id { get; set; }

        public string Network { get; set; }

        public string Overview { get; set; }

        public string SeriesName { get; set; }

        public Status Status { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="PartialSeries"/> to <see cref="SearchResult"/>.
        /// </summary>
        /// <param name="partialSeries">The partial series.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SearchResult(PartialSeries partialSeries)
        {
            DateTime? firstAired = null;

            try
            {
                if (!string.IsNullOrEmpty(partialSeries.FirstAired)) firstAired = DateTime.Parse(partialSeries.FirstAired).Date;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"[#{partialSeries.Id}] Failed to convert \"{partialSeries.FirstAired}\" to type DateTime");
            }

            return new SearchResult
            {
                Aliases = partialSeries.Aliases,
                Banner = partialSeries.Banner,
                FirstAired = firstAired,
                Id = partialSeries.Id,
                Name = partialSeries.SeriesName,
                Network = partialSeries.Network,
                Overview = partialSeries.Overview,
                Status = partialSeries.Status
            };
        }
    }
}
