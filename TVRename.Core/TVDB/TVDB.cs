using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TVRename.Core.Extensions;
using TVRename.Core.Models;
using TVRename.Core.Models.Cache;
using TVRename.Core.Models.TVDB;
using Episode = TVRename.Core.Models.TVDB.Episode;
using File = Alphaleonis.Win32.Filesystem.File;
using Language = TVRename.Core.Models.Language;
using Show = TVRename.Core.Models.Cache.Show;

namespace TVRename.Core.TVDB
{
    /// <summary>
    /// Stores and retrieves a local cache of TheTVDB results.
    /// </summary>
    public class TVDB
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static TVDB instance;

        private readonly Client client = new Client();

        private readonly ConcurrentDictionary<int, Show> shows = new ConcurrentDictionary<int, Show>();

        public static TVDB Instance => instance ?? (instance = new TVDB());

        /// <summary>
        /// Gets or sets the file encoding used when serislizing to file.
        /// </summary>
        /// <value>
        /// The file encoding.
        /// </value>
        [JsonIgnore]
        public static Encoding FileEncoding { get; set; } = new UTF8Encoding();

        /// <summary>
        /// Gets or sets the time the cache was last updated with TheTVDB updates.
        /// </summary>
        /// <value>
        /// The time the cache was last updated with TheTVDB updates.
        /// </value>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the list of cached shows.
        /// </summary>
        /// <value>
        /// The list of cached shows.
        /// </value>
        public IReadOnlyDictionary<int, Show> Shows => this.shows;

        /// <summary>
        /// Gets or sets the default TheTVDB lookup language.
        /// </summary>
        /// <value>
        /// The default TheTVDB lookup language.
        /// </value>
        [JsonIgnore]
        public Language Language { get; set; } = new Language
        {
            Id = 7,
            Abbreviation = "en"
        };

        /// <summary>
        /// Gets or sets the number of threads to use when downloading from TheTVDB.
        /// </summary>
        /// <value>
        /// The number of threads to use when downloading from TheTVDB.
        /// </value>
        [JsonIgnore]
        public int Threads { get; set; } = 5;

        /// <summary>
        /// Deserilizes and loads the specified file.
        /// </summary>
        /// <param name="file">The file to load.</param>
        public static async Task Load(string file)
        {
            try
            {
                Logger.Info($"Loading TVDB cache from {file}");

                using (StreamReader reader = File.OpenText(file)) // TODO: Explicit encoding?
                {
                    instance = JsonConvert.DeserializeObject<TVDB>(await reader.ReadToEndAsync());
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Warn($"TVDB cache file {file} not found, using empty cache");

                instance = new TVDB();

                await Save(file);
            }

            instance.client.Language = instance.Language.Abbreviation;
        }

        /// <summary>
        /// Saves the current cache to the specified file.
        /// </summary>
        /// <param name="file">The file to save to.</param>
        public static async Task Save(string file)
        {
            Logger.Info($"Saving TVDB cache to {file}");

            using (StreamWriter writer = new StreamWriter(file, false, FileEncoding))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(instance, Formatting.Indented));
            }
        }

        /// <summary>
        /// Gets a list of supported languages from TheTVDB.
        /// </summary>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns>List of supported TheTVDB languages.</returns>
        public async Task<IEnumerable<Models.TVDB.Language>> Languages(CancellationToken ct)
        {
            return await this.client.GetLanguages(ct);
        }

        /// <summary>
        /// Adds and downloads the specified show to the cache.
        /// </summary>
        /// <param name="id">TheTVDB show identifier.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        public async Task Add(int id, CancellationToken ct)
        {
            if (this.shows.ContainsKey(id)) return;

            Logger.Info($"Adding TVDB series {id}");

            Series show = await this.client.GetSeries(id, ct, true); // Get full show
            show.LanguageId = this.Language.Id;

            this.shows[id] = show;
        }

        /// <summary>
        /// Removes the specified show from the cache.
        /// </summary>
        /// <param name="id">TheTVDB show identifier.</param>
        public void Remove(int id)
        {
            if (!this.shows.ContainsKey(id)) return;

            Logger.Info($"Removing TVDB series {id}");

            this.shows.TryRemove(id, out Show _);
        }

        /// <summary>
        /// Refreshes all shows in the cache.
        /// </summary>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <param name="progress">The callback for reporting progress.</param>
        public async Task Refresh(CancellationToken ct, IProgress<int> progress = null)
        {
            Logger.Info("Refreshing all shows");

            await this.shows.ForEachAsync(async s =>
            {
                progress?.Report(s.Key);

                await Refresh(s.Key, ct);
            }, this.Threads);
        }

        /// <summary>
        /// Refreshes the specified show in the cache.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        public async Task Refresh(int id, CancellationToken ct)
        {
            if (!this.shows.ContainsKey(id)) return;

            Logger.Info($"Refreshing TVDB series {id}");

            Series show = await this.client.GetSeries(id, ct, true); // Get full show
            show.LanguageId = this.Language.Id;

            this.shows[id] = show;
        }

        /// <summary>
        /// Searches TheTVDB for the specified show.
        /// </summary>
        /// <param name="search">The show search text.</param>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <returns>List of TheTVDB search results.</returns>
        public async Task<List<SearchResult>> Search(string search, CancellationToken ct)
        {
            Logger.Info($"Searching for show {search}");

            List<PartialSeries> results = await this.client.Search(search, ct);
            return results.Select(r => (SearchResult)r).ToList();
        }

        /// <summary>
        /// Updates the cache from TheTVDB update feed, downloading fresh metadata as needed.
        /// </summary>
        /// <param name="ct">The cancellation token to cancel operation.</param>
        /// <param name="progress">The callback for reporting progress.</param>
        public async Task Update(CancellationToken ct, IProgress<string> progress = null)
        {
            Logger.Info($"Updating shows, last updated {this.LastUpdated}");

            // This is probably more costly than just getting all the updates
            //// Check the cache is recent
            //if (this.LastUpdated < DateTime.Now.AddYears(-1))
            //{
            //    Logger.Info("TVDB cache is over a year old, rebuilding");

            //    // Set to now
            //    this.LastUpdated = DateTime.UtcNow;

            //    progress?.Report("Refreshing all shows");

            //    // Download all metadata for all shows and episodes
            //    await Refresh(ct);
            //}

            bool moreUpdates = true;
            DateTime updateStart = this.LastUpdated;
            DateTime updateEnd = this.LastUpdated;
            List<Update> updates = new List<Update>();

            while (moreUpdates)
            {
                // TheTVDB API only returns a max of seven days of results
                updateEnd = updateStart.AddDays(7);

                // Get all updates till now
                if (updateEnd > DateTime.UtcNow)
                {
                    updateEnd = DateTime.UtcNow;
                    moreUpdates = false;
                }

                progress?.Report($"Downloading updates for the week of {updateStart}");

                Logger.Info($"Getting TVDB updates from {updateStart} to {updateEnd}");

                updates.AddRange(await this.client.GetUpdates(updateStart, ct));

                updateStart = updateEnd;
            }

            // Drop updates for shows we don't have
            updates = updates.AsParallel().Where(u => this.shows.ContainsKey(u.Id)).ToList();

            Logger.Info($"Processing {updates.Count} updates");

            List<Tuple<int, PartialEpisode>> newEpisodes = new List<Tuple<int, PartialEpisode>>();

            // Process updates and download in parallel
            await updates.ForEachAsync(async update =>
            {
                if (!this.shows.TryGetValue(update.Id, out Show show)) return;

                if (update.LastUpdated > show.LastUpdated) show.Dirty = true; // Show has been updated

                Logger.Info($"Getting episode summery for {show.Name}");

                // Get basic episodes and check if the cached episode is out of date
                List<PartialEpisode> partialEpisodes = await this.client.GetPartialEpisodes(show.Id, ct);

                partialEpisodes.AsParallel().ForAll(partialEpisode =>
                {
                    // Look for the cached episode
                    Models.Cache.Episode episode = show.Seasons.SelectMany(s => s.Value.Episodes).FirstOrDefault(e => e.Key == partialEpisode.Id).Value;

                    if (episode != null)
                    {
                        if (episode.LastUpdated >= partialEpisode.LastUpdated) return;

                        // Mark as dirty
                        episode.Dirty = true;

                        Logger.Info($"{show.Name} episode {episode.Name} is updated");
                    }
                    else
                    {
                        Logger.Info($"New episode for {show.Name}");

                        // Add to new episode list
                        newEpisodes.Add(new Tuple<int, PartialEpisode>(show.Id, partialEpisode));
                    }

                });

                // Remove extra cached episodes
                foreach (KeyValuePair<int, Season> season in this.shows[show.Id].Seasons)
                {
                    foreach (KeyValuePair<int, Models.Cache.Episode> episode in season.Value.Episodes.Where(e => partialEpisodes.All(pe => pe.Id != e.Key)))
                    {
                        this.shows[show.Id].Seasons[season.Key].Episodes.TryRemove(episode.Key, out Models.Cache.Episode _);
                    }
                }
            }, this.Threads);

            // Download new episodes in parallel
            await newEpisodes.ForEachAsync(async n =>
            {
                Episode episode = await this.client.GetEpisode(n.Item2.Id, ct);

                Season season = this.shows[n.Item1].Seasons.GetOrAdd(episode.AiredSeason, new Season(episode.AiredSeason));

                this.shows[n.Item1].Seasons[season.Number].Episodes[episode.Id] = episode;
            }, this.Threads);

            // Download dirty or empty shows in parallel
            await this.shows.Where(s => s.Value.Seasons.Count == 0 || s.Value.Dirty).ForEachAsync(async s =>
            {
                Show show = await this.client.GetSeries(s.Key, ct, s.Value.Seasons.Count == 0);
                show.LanguageId = this.Language.Id;

                if (show.Seasons.Count == 0 && s.Value.Seasons.Count > 0)
                {
                    show.Seasons = s.Value.Seasons;
                }

                this.shows[s.Key] = show;
            }, this.Threads);

            // Download dirty episodes in parallel
            foreach (KeyValuePair<int, Show> show in this.shows)
            {
                foreach (KeyValuePair<int, Season> season in show.Value.Seasons)
                {
                    await season.Value.Episodes.Where(e => e.Value.Dirty).ForEachAsync(async e =>
                    {
                        Episode episode = await this.client.GetEpisode(e.Key, ct);

                        this.shows[show.Key].Seasons.GetOrAdd(episode.AiredSeason, new Season(episode.AiredSeason));

                        this.shows[show.Key].Seasons[season.Key].Episodes[e.Key] = episode;
                    }, this.Threads);
                }
            }

            // TODO: Remove empty seasons

            this.LastUpdated = updateEnd;
        }
    }
}
