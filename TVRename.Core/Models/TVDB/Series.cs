using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TVRename.Core.Models.Cache;
using TVRename.Core.Models.Converters;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class Series : PartialSeries
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [JsonIgnore]
        public List<Actor> Actors { get; set; } = new List<Actor>();

        public string Added { get; set; }

        public string AddedBy { get; set; }

        public string AirsDayOfWeek { get; set; }

        public string AirsTime { get; set; }

        [JsonIgnore]
        public List<Episode> Episodes { get; set; } = new List<Episode>();

        public List<string> Genre { get; set; } = new List<string>();

        [JsonIgnore]
        public List<Image> Images { get; set; } = new List<Image>();

        public int LanguageId { get; set; }

        public string ImdbId { get; set; }

        [JsonConverter(typeof(EpochConverter))]
        public DateTime LastUpdated { get; set; }

        public string NetworkId { get; set; }

        public ContentRating Rating { get; set; }

        public string Runtime { get; set; }

        public string SeriesId { get; set; }

        public decimal SiteRating { get; set; }

        public int SiteRatingCount { get; set; }

        // ReSharper disable once InconsistentNaming
        public string Zap2itId { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Series"/> to <see cref="Show"/>.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Show(Series series)
        {
            Dictionary<int, Season> seasons = new Dictionary<int, Season>();

            foreach (Episode episode in series.Episodes) // TODO: Parallel?
            {
                if (seasons.All(s => s.Key != episode.AiredSeason))
                {
                    seasons[episode.AiredSeason] = new Season(episode.AiredSeason);
                }

                seasons[episode.AiredSeason].Episodes[episode.Id] = episode;
            }

            AirDay airDay = AirDay.Unknown;
            DateTime? firstAired = null;
            TimeSpan? airTime = null;

            try
            {
                if (Enum.IsDefined(typeof(AirDay), series.AirsDayOfWeek))
                {
                    airDay = (AirDay)Enum.Parse(typeof(AirDay), series.AirsDayOfWeek, true);
                }
                else if (!string.IsNullOrWhiteSpace(series.AirsDayOfWeek))
                {
                    throw new Exception("Not a valid AirDay");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"[#{series.Id}] Failed to convert \"{series.AirsDayOfWeek}\" to type AirDay");
            }

            try
            {
                if (!string.IsNullOrEmpty(series.FirstAired)) firstAired = DateTime.Parse(series.FirstAired).Date;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"[#{series.Id}] Failed to convert \"{series.FirstAired}\" to type DateTime");
            }

            try
            {
                if (!string.IsNullOrEmpty(series.AirsTime)) airTime = DateTime.Parse(series.AirsTime).TimeOfDay;
            }
            catch (Exception)
            {
                // Try some common TVDB formats:
                // 9.15 pm
                // 9.15pm
                // 21.15
                foreach (string format in new[] { "hh.mm tt", "hh.mmtt", "HH.mm" })
                {
                    try
                    {
                        airTime = DateTime.ParseExact(series.AirsTime, format, CultureInfo.InvariantCulture).TimeOfDay;

                        break;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (!airTime.HasValue)
                {
                    Logger.Error($"[#{series.Id}] Failed to convert \"{series.AirsTime}\" to type TimeSpan");
                }
            }

            return new Show
            {
                Actors = series.Actors.Select(a => a.Name.Trim()).ToList(),
                AirDay = airDay,
                AirTime = airTime,
                Aliases = series.Aliases,
                Banner = series.Images.FirstOrDefault(i => i.KeyType == "series")?.FileName, // TODO: Pick best images
                ContentRating = series.Rating,
                Fanart = series.Images.FirstOrDefault(i => i.KeyType == "fanart")?.FileName,
                FirstAired = firstAired,
                Genres = series.Genre,
                Id = series.Id,
                ImdbId = series.ImdbId,
                LanguageId = series.LanguageId,
                LastUpdated = series.LastUpdated,
                Name = series.SeriesName,
                Network = series.Network,
                Overview = series.Overview,
                Poster = series.Images.FirstOrDefault(i => i.KeyType == "poster")?.FileName,
                Rating = new Rating
                {
                    Score = series.SiteRating,
                    Votes = series.SiteRatingCount
                },
                Runtime = series.Runtime,
                Seasons = new ConcurrentDictionary<int, Season>(seasons),
                Status = series.Status
            };
        }
    }
}
