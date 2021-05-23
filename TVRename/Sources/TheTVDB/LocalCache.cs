//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Humanizer;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TVRename.Forms.Utilities;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

// Talk to the TheTVDB web API, and get tv cachedSeries info

// Hierarchy is:
//   TheTVDB -> Series (class CachedSeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

namespace TVRename.TheTVDB
{
    // ReSharper disable once InconsistentNaming
    public class LocalCache : MediaCache, iTVSource, iMovieSource
    {
#nullable enable

        private ConcurrentDictionary<int, ExtraEp>
            extraEpisodes; // IDs of extra episodes to grab and merge in on next update

        private ConcurrentDictionary<int, ExtraEp> removeEpisodeIds; // IDs of episodes that should be removed

        private ConcurrentDictionary<int, int> forceReloadOn;
        private UpdateTimeTracker LatestUpdateTime;

        private CommandLineArgs args;

        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile LocalCache? InternalInstance;
        private static readonly object SyncRoot = new object();

        [NotNull]
        public static LocalCache Instance
        {
            get
            {
                if (InternalInstance is null)
                {
                    lock (SyncRoot)
                    {
                        // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                        if (InternalInstance is null)
                        {
                            InternalInstance = new LocalCache();
                        }
                    }
                }

                return InternalInstance;
            }
        }

        public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TheTVDB;

        public void Setup(FileInfo? loadFrom, FileInfo cache, CommandLineArgs cla)
        {
            args = cla;

            System.Diagnostics.Debug.Assert(cache != null);
            CacheFile = cache;

            LastErrorMessage = string.Empty;
            IsConnected = false;
            extraEpisodes = new ConcurrentDictionary<int, ExtraEp>();
            removeEpisodeIds = new ConcurrentDictionary<int, ExtraEp>();

            LanguageList = Languages.Instance;
            RegionList = Regions.Instance;

            //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
            //If we have no prior install then the app has no shows and is by definition up-to-date
            LatestUpdateTime = new UpdateTimeTracker();

            LOGGER.Info($"Assumed we have updates until {LatestUpdateTime}");

            LoadOk = loadFrom is null || CachePersistor.LoadTvCache(loadFrom, this);

            forceReloadOn = new ConcurrentDictionary<int, int>();
        }

        public byte[]? GetTvdbDownload(string url)
        {
            try
            {
                return API.GetTvdbDownload(url, false);
            }
            catch (WebException e)
            {
                LOGGER.Warn(CurrentDLTask + " : " + e.LoggableDetails() + " : " + url);
                LastErrorMessage = CurrentDLTask + " : " + e.LoggableDetails();
                return null;
            }
        }

        public CachedSeriesInfo? GetSeriesAndDownload(int id, bool showErrorMsgBox) => HasSeries(id)
            ? Series[id]
            : DownloadSeriesNow(id, false, false, new Locale(TVSettings.Instance.PreferredTVDBLanguage),
                showErrorMsgBox);

        public ConcurrentDictionary<int, CachedSeriesInfo> GetSeriesDict() => Series;

        [NotNull]
        private Dictionary<int, CachedSeriesInfo> GetSeriesDictMatching(string testShowName)
        {
            Dictionary<int, CachedSeriesInfo> matchingSeries = new Dictionary<int, CachedSeriesInfo>();

            testShowName = testShowName.CompareName();

            if (string.IsNullOrEmpty(testShowName))
            {
                return matchingSeries;
            }

            foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series)
            {
                string show = kvp.Value.Name.CompareName();

                if (show.Contains(testShowName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //We have a match
                    matchingSeries.Add(kvp.Key, kvp.Value);
                }
            }

            return matchingSeries;
        }

        public void UpdatesDoneOk()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            LatestUpdateTime.RecordSuccessfulUpdate();
        }

        public CachedMovieInfo GetMovie(PossibleNewMovie show, Locale locale, bool showErrorMsgBox) =>
            throw new NotImplementedException();

        public CachedMovieInfo GetMovie(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            lock (MOVIE_LOCK)
            {
                return HasMovie(id.Value) ? Movies[id.Value] : null;
            }
        }

        public CachedSeriesInfo GetSeries(string showName, bool showErrorMsgBox, Locale preferredLocale)
        {
            Search(showName, showErrorMsgBox, MediaConfiguration.MediaType.tv, preferredLocale);

            if (string.IsNullOrEmpty(showName))
            {
                return null;
            }

            showName = showName.ToLower();

            List<CachedSeriesInfo> matchingShows = GetSeriesDictMatching(showName).Values.ToList();

            return matchingShows.Count switch
            {
                0 => null,
                1 => matchingShows.First(),
                _ => null
            };
        }

        [NotNull]
        internal IEnumerable<CachedSeriesInfo> ServerAccuracyCheck()
        {
            Say("TVDB Accuracy Check running");
            TvdbAccuracyCheck check = new TvdbAccuracyCheck(this);
            lock (SERIES_LOCK)
            {
                foreach (CachedSeriesInfo si in Series.Values.Where(info => !info.IsSearchResultOnly)
                    .OrderBy(s => s.Name).ToList())
                {
                    check.ServerAccuracyCheck(si);
                }
            }

            foreach (string issue in check.Issues)
            {
                LOGGER.Warn(issue);
            }

            SayNothing();
            return check.ShowsToUpdate;
        }

        private Episode? FindEpisodeById(int id)
        {
            lock (SERIES_LOCK)
            {
                foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series.ToList())
                {
                    foreach (Episode e in kvp.Value.Episodes.Where(e => e.EpisodeId == id))
                    {
                        return e;
                    }
                }
            }

            return null;
        }

        public bool Connect(bool showErrorMsgBox) => TVDBLogin(showErrorMsgBox);

        public void Tidy(IEnumerable<MovieConfiguration> libraryValues)
        {
            // remove any shows from cache that aren't in My Movies
            List<int> removeList = new List<int>();

            lock (MOVIE_LOCK)
            {
                foreach (KeyValuePair<int, CachedMovieInfo> kvp in Movies)
                {
                    bool found = libraryValues.Any(si => si.TvdbCode == kvp.Key);
                    if (!found)
                    {
                        removeList.Add(kvp.Key);
                    }
                }

                foreach (int i in removeList)
                {
                    ForgetMovie(i);
                }
            }
        }

        public void ForgetEverything()
        {
            lock (SERIES_LOCK)
            {
                Series.Clear();
            }

            IsConnected = false;
            SaveCache();

            //All cachedSeries will be forgotten and will be fully refreshed, so we'll only need updates after this point
            LatestUpdateTime.Reset();
            LOGGER.Info($"Forget everything, so we assume we have updates until {LatestUpdateTime}");
        }

        public void ForgetMovie(int id)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id))
                {
                    Movies.TryRemove(id, out _);
                }
            }
        }

        public void ForgetMovie(ISeriesSpecifier s)
        {
            ForgetMovie(s.TvdbId);
            lock (MOVIE_LOCK)
            {
                if (s.TvdbId > 0)
                {
                    AddPlaceholderSeries(s);
                }
            }
        }

        public void Update(CachedMovieInfo si)
        {
            throw new NotImplementedException();
        }

        public void AddPoster(int seriesId, IEnumerable<Banner> @select)
        {
            throw new NotImplementedException();
        }

        public void ForgetShow(ISeriesSpecifier ss)
        {
            ForgetShow(ss.TvdbId);
            lock (SERIES_LOCK)
            {
                if (ss.TvdbId > 0)
                {
                    AddPlaceholderSeries(ss);

                    forceReloadOn.TryAdd(ss.TvdbId, ss.TvdbId);
                }
            }
        }

        public void ForgetShow(int id)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(id))
                {
                    Series.TryRemove(id, out _);
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        private bool TVDBLogin(bool showErrorMsgBox)
        {
            Say("TheTVDB Login");
            try
            {
                API.Login(false);
                IsConnected = true;
                return true;
            }
            catch (WebException ex)
            {
                HandleConnectionProblem(showErrorMsgBox, ex);
                return false;
            }
            finally
            {
                SayNothing();
            }
        }

        private void HandleConnectionProblem(bool showErrorMsgBox, WebException ex)
        {
            Say("Could not connect to TVDB");

            LOGGER.LogWebException("Error obtaining Languages from TVDB", ex);
            LastErrorMessage = ex.LoggableDetails();

            if (showErrorMsgBox)
            {
                CannotConnectForm ccform =
                    new CannotConnectForm("Error while downloading languages from TVDB", ex.LoggableDetails());

                DialogResult ccresult = ccform.ShowDialog();
                if (ccresult == DialogResult.Abort)
                {
                    TVSettings.Instance.OfflineMode = true;
                    LastErrorMessage = string.Empty;
                }
            }
        }

        private void AddPlaceholderSeries([NotNull] ISeriesSpecifier ss)
            => AddPlaceholderSeries(ss.TvdbId, ss.TvMazeId, ss.TmdbId, ss.TargetLocale);

        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, IEnumerable<ISeriesSpecifier> ss)
        {
            Say("Validating TheTVDB cache");
            foreach (ISeriesSpecifier downloadShow in ss.Where(downloadShow => !HasSeries(downloadShow.TvdbId)))
            {
                AddPlaceholderSeries(downloadShow);
            }

            Say("Updates list from TVDB");

            if (!IsConnected && !Connect(showErrorMsgBox))
            {
                SayNothing();
                return false;
            }

            long updateFromEpochTime = LatestUpdateTime.LastSuccessfulServerUpdateTimecode();

            if (updateFromEpochTime == 0)
            {
                updateFromEpochTime = GetUpdateTimeFromShows();
            }

            MarkPlaceholdersDirty();

            if (updateFromEpochTime == 0 && Series.Values.Any(info => !info.IsSearchResultOnly))
            {
                SayNothing();
                LOGGER.Error(
                    $"Not updating as update time is 0. Need to do a Full Refresh on {Series.Values.Count(info => !info.IsSearchResultOnly)} shows. {LatestUpdateTime}");

                ForgetEverything();
                return true; // that's it for now
            }

            if (updateFromEpochTime == 0)
            {
                SayNothing();
                LOGGER.Info("We have no shows yet to get TVDB updates for. Not getting latest updates.");
                return true; // that's it for now
            }

            //We need to ask for updates in blocks of 7 days
            //We'll keep asking until we get to a date within 7 days of today
            //(up to a maximum of 52 - if you are this far behind then you may need multiple refreshes)

            List<JObject> updatesResponses = new List<JObject>();

            bool moreUpdates = true;
            int numberofCallsMade = 0;

            while (moreUpdates)
            {
                if (cts.IsCancellationRequested)
                {
                    SayNothing();
                    return true;
                }

                //If this date is in the last week then this needs to be the last call to the update
                DateTime requestedTime = GetRequestedTime(updateFromEpochTime, numberofCallsMade);

                if ((DateTime.UtcNow - requestedTime).TotalDays < 7)
                {
                    moreUpdates = false;
                }

                JObject jsonUpdateResponse = GetUpdatesJson(updateFromEpochTime, requestedTime);
                if (jsonUpdateResponse is null)
                {
                    return false;
                }

                int? numberOfResponses = GetNumResponses(jsonUpdateResponse, requestedTime);
                if (numberOfResponses is null)
                {
                    return false;
                }

                long maxUpdateTime;

                if (numberOfResponses == 0 &&
                    updateFromEpochTime + 7.Days().TotalSeconds < DateTime.UtcNow.ToUnixTime())
                {
                    maxUpdateTime = updateFromEpochTime + (int)7.Days().TotalSeconds;
                }
                else
                {
                    updatesResponses.Add(jsonUpdateResponse);
                    numberofCallsMade++;
                    maxUpdateTime = GetUpdateTime(jsonUpdateResponse);
                }

                if (maxUpdateTime > 0)
                {
                    LatestUpdateTime.RegisterServerUpdate(maxUpdateTime);

                    LOGGER.Info(
                        $"Obtained {numberOfResponses} responses from lastupdated query #{numberofCallsMade} - since (local) {requestedTime.ToLocalTime()} - to (local) {LatestUpdateTime}");

                    if (updateFromEpochTime == maxUpdateTime)
                    {
                        updateFromEpochTime++;
                    }

                    if (updateFromEpochTime < maxUpdateTime)
                    {
                        updateFromEpochTime = maxUpdateTime;
                    }
                }

                //As a safety measure we check that no more than 52 calls are made
                const int MAX_NUMBER_OF_CALLS = 52;
                if (numberofCallsMade > MAX_NUMBER_OF_CALLS)
                {
                    if (cts.IsCancellationRequested)
                    {
                        SayNothing();
                        return false;
                    }

                    moreUpdates = false;
                    string errorMessage =
                        $"We have run {MAX_NUMBER_OF_CALLS} weeks of updates and we are not up to date.  The system will need to check again once this set of updates have been processed.{Environment.NewLine}Last Updated time was {LatestUpdateTime.LastSuccessfulServerUpdateDateTime()}{Environment.NewLine}New Last Updated time is {LatestUpdateTime.ProposedServerUpdateDateTime()}{Environment.NewLine}{Environment.NewLine}If the dates keep getting more recent then let the system keep getting {MAX_NUMBER_OF_CALLS} week blocks of updates, otherwise consider a 'Force Refresh All'";

                    LOGGER.Error(errorMessage);
                    if (!args.Unattended && !args.Hide && Environment.UserInteractive)
                    {
                        MessageBox.Show(errorMessage, "Long Running Update", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    if (!LatestUpdateTime.HasIncreased)
                    {
                        //Probably some issue has occurred with TVRename, so we need to restart the cache
                        LOGGER.Error("Update times did not increase - need to refresh all cachedSeries");
                        ForgetEverything();
                    }
                }
            }

            Say("Processing Updates from TVDB");

            Parallel.ForEach(updatesResponses, o => ProcessUpdate(o, cts));

            Say("Upgrading dirty locks");

            UpgradeDirtyLocks();

            SayNothing();

            return true;
        }

        private int? GetNumResponses(JObject jsonUpdateResponse, DateTime requestedTime)
        {
            try
            {
                JToken dataToken = jsonUpdateResponse["data"];
                if (dataToken is null)
                {
                    return 0;
                }

                return !dataToken.HasValues ? 0 : ((JArray)dataToken).Count;
            }
            catch (InvalidCastException ex)
            {
                SayNothing();
                LastErrorMessage = ex.Message;

                string msg = "Unable to get latest updates from TVDB " + Environment.NewLine +
                             "Trying to get updates since " + requestedTime.ToLocalTime() +
                             Environment.NewLine + Environment.NewLine +
                             "If the date is very old, please consider a full refresh";

                LOGGER.Warn($"Error obtaining lastupdated query -since(local) {requestedTime.ToLocalTime()}");

                LOGGER.Warn(ex, msg);

                if (!args.Unattended && !args.Hide && Environment.UserInteractive)
                {
                    MessageBox.Show(msg, "Error obtaining updates from TVDB", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                return null;
            }
        }

        private JObject? GetUpdatesJson(long updateFromEpochTime, DateTime requestedTime)
        {
            try
            {
                return API.GetShowUpdatesSince(updateFromEpochTime,
                    TVSettings.Instance.PreferredTVDBLanguage.Abbreviation);
            }
            catch (IOException iex)
            {
                LOGGER.LogIoException(
                    $"Error obtaining lastupdated query since (local) {requestedTime.ToLocalTime()}: Message is", iex);

                SayNothing();
                LastErrorMessage = iex.LoggableDetails();
                return null;
            }
            catch (WebException ex)
            {
                LOGGER.LogWebException(
                    $"Error obtaining lastupdated query since (local) {requestedTime.ToLocalTime()}: Message is", ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                return null;
            }
            catch (AggregateException aex) when (aex.InnerException is WebException ex)
            {
                LOGGER.LogWebException(
                    $"Error obtaining lastupdated query since (local) {requestedTime.ToLocalTime()}: Message is", ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                return null;
            }
            catch (AggregateException aex) when (aex.InnerException is System.Net.Http.HttpRequestException ex)
            {
                LOGGER.LogHttpRequestException(
                    $"Error obtaining lastupdated query since (local) {requestedTime.ToLocalTime()}: Message is", ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                return null;
            }
        }

        private DateTime GetRequestedTime(long updateFromEpochTime, int numberofCallsMade)
        {
            try
            {
                return Helpers.FromUnixTime(updateFromEpochTime).ToUniversalTime();
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex,
                    $"Could not get updates({numberofCallsMade}): LastSuccessFullServer {LatestUpdateTime.LastSuccessfulServerUpdateTimecode()}: Series Time: {GetUpdateTimeFromShows()} {LatestUpdateTime}, Tried to parse {updateFromEpochTime}");
            }

            //Have to do something!!
            return Helpers.FromUnixTime(0).ToUniversalTime();
        }

        private void UpgradeDirtyLocks()
        {
            // if more than x% of a show's episodes are marked as dirty, just download the entire show again
            foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series)
            {
                int totaleps = kvp.Value.Episodes.Count;
                int totaldirty = kvp.Value.Episodes.Count(episode => episode.Dirty);

                float percentDirty = 100;
                if (totaldirty > 0 || totaleps > 0)
                {
                    percentDirty = 100 * totaldirty / (float)totaleps;
                }

                if (totaleps > 0 && percentDirty >= TVSettings.Instance.PercentDirtyUpgrade()) // 10%
                {
                    kvp.Value.Dirty = true;
                    kvp.Value.ClearEpisodes();
                    LOGGER.Info(
                        $"Planning to download all of {kvp.Value.Name} as {percentDirty}% of the episodes need to be updated");
                }
                else
                {
                    LOGGER.Trace(
                        $"Not planning to download all of {kvp.Value.Name} as {percentDirty}% of the episodes need to be updated and that's less than the 10% limit to upgrade.");
                }
            }
        }

        private void ProcessUpdate([NotNull] JObject jsonResponse, CancellationToken cts)
        {
            // if updatetime > localtime for item, then remove it, so it will be downloaded later
            JToken? jToken = jsonResponse["data"];

            if (jToken is null)
            {
                throw new SourceConsistencyException($"Could not get data element from {jsonResponse}",
                    TVDoc.ProviderType.TheTVDB);
            }

            try
            {
                foreach (JObject seriesResponse in jToken.Cast<JObject>())
                {
                    if (!cts.IsCancellationRequested)
                    {
                        if (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
                        {
                            ProcessSeriesUpdateV4(seriesResponse);
                        }
                        else
                        {
                            ProcessSeriesUpdate(seriesResponse);
                        }
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                LOGGER.Error("Did not receive the expected format of json from lastupdated query.");
                LOGGER.Error(ex);
                LOGGER.Error(jToken.ToString());
            }
            catch (OverflowException ex)
            {
                LOGGER.Error("Could not parse the json from lastupdated query.");
                LOGGER.Error(ex);
                LOGGER.Error(jToken.ToString());
            }
        }

        private void ProcessSeriesUpdate([NotNull] JObject seriesResponse)
        {
            int id = (int)seriesResponse["id"];
            long time = (long)seriesResponse["lastUpdated"];

            if (!Series.ContainsKey(id))
            {
                return;
            }

            CachedSeriesInfo selectedCachedSeriesInfo = Series[id];

            if (time > selectedCachedSeriesInfo.SrvLastUpdated) // newer version on the server
            {
                selectedCachedSeriesInfo.Dirty = true; // mark as dirty, so it'll be fetched again later
            }
            else
            {
                LOGGER.Info(selectedCachedSeriesInfo.Name + " has a lastupdated of  " +
                            Helpers.FromUnixTime(selectedCachedSeriesInfo.SrvLastUpdated) + " server says " +
                            Helpers.FromUnixTime(time));
            }

            //now we wish to see if any episodes from the cachedSeries have been updated. If so then mark them as dirty too
            List<JObject> episodeDefaultLangResponses = null;
            try
            {
                List<JObject> episodeResponses = GetEpisodes(id, selectedCachedSeriesInfo.ActualLocale);
                if (episodeResponses is null)
                {
                    //we got nothing good back from TVDB
                    LOGGER.Warn(
                        $"Aborting updates for {selectedCachedSeriesInfo.Name} ({id}) as there was an issue obtaining episodes");

                    return;
                }

                if (selectedCachedSeriesInfo.ActualLocale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
                {
                    episodeDefaultLangResponses =
                        GetEpisodes(id, new Locale(TVSettings.Instance.PreferredTVDBLanguage));

                    if (episodeDefaultLangResponses is null)
                    {
                        //we got nothing good back from TVDB
                        LOGGER.Warn(
                            $"Aborting updates for {selectedCachedSeriesInfo.Name} ({id}) as there was an issue obtaining episodes in {TVSettings.Instance.PreferredTVDBLanguage.EnglishName}");

                        return;
                    }
                }

                Dictionary<int, Tuple<JToken, JToken>> episodesResponses =
                    MergeEpisodeResponses(episodeResponses, episodeDefaultLangResponses);

                ProcessEpisodes(selectedCachedSeriesInfo, episodesResponses);
            }
            catch (MediaNotFoundException ex)
            {
                LOGGER.Warn(
                    $"Episodes were not found for {ex.ShowId}:{selectedCachedSeriesInfo.Name} in languange {selectedCachedSeriesInfo.ActualLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName} or {TVSettings.Instance.PreferredTVDBLanguage.EnglishName}");
            }
            catch (KeyNotFoundException kex)
            {
                //We assue this is due to the update being for a recently removed show.
                LOGGER.Error(kex);
            }
        }

        private void ProcessSeriesUpdateV4([NotNull] JObject seriesResponse)
        {
            int id = (int)seriesResponse["recordId"];
            long time = (long)seriesResponse["timeStamp"];
            string entityType = (string)seriesResponse["entityType"];
            string method = (string)seriesResponse["method"];

            if (entityType == "series" || entityType == "translatedseries")
            {
                if (Series.ContainsKey(id))
                {
                    CachedSeriesInfo selectedCachedSeriesInfo = Series[id];

                    if (time > selectedCachedSeriesInfo.SrvLastUpdated) // newer version on the server
                    {
                        selectedCachedSeriesInfo.Dirty = true; // mark as dirty, so it'll be fetched again later
                    }
                    else
                    {
                        LOGGER.Info(selectedCachedSeriesInfo.Name + " has a lastupdated of  " +
                                    Helpers.FromUnixTime(selectedCachedSeriesInfo.SrvLastUpdated) + " server says " +
                                    Helpers.FromUnixTime(time));
                    }
                }

                return;
            }

            if (entityType == "movies" || entityType == "translatedmovies" || entityType == "movie-genres")
            {
                if (Movies.ContainsKey(id))
                {
                    CachedMovieInfo selectedMovieCachedData = Movies[id];
                    if (time > selectedMovieCachedData.SrvLastUpdated) // newer version on the server
                    {
                        selectedMovieCachedData.Dirty = true; // mark as dirty, so it'll be fetched again later
                    }
                    else
                    {
                        LOGGER.Info(selectedMovieCachedData.Name + " has a lastupdated of  " +
                                    Helpers.FromUnixTime(selectedMovieCachedData.SrvLastUpdated) + " server says " +
                                    Helpers.FromUnixTime(time));
                    }
                }

                return;
            }

            if (entityType == "artwork" || entityType == "people" || entityType == "characters" || entityType == "award-nominees")
            {
                return;
            }
            if (entityType == "episodes" || entityType == "translatedepisodes")
            {
                var x = Series.Values.Where(y => y.Episodes.Any(e => e.EpisodeId == id));
                if (!x.Any())
                {
                    return;
                }
                LOGGER.Info("");
                return;
            }
            if (entityType == "seasons")
            {
                var x = Series.Values.Where(y => y.Seasons.Any(e => e.SeasonId == id));
                if (!x.Any())
                {
                    return;
                }
                LOGGER.Info("");
                return;
            }
            if (entityType == "")
            {
                return;
            }
        }

        private void ProcessEpisodes([NotNull] CachedSeriesInfo si,
            [NotNull] Dictionary<int, Tuple<JToken, JToken>> episodesResponses)
        {
            int numberOfNewEpisodes = 0;
            int numberOfUpdatedEpisodes = 0;

            ICollection<int> oldEpisodeIds = GetOldEpisodeIds(si.TvdbCode);

            foreach (KeyValuePair<int, Tuple<JToken, JToken>> episodeData in episodesResponses)
            {
                try
                {
                    JToken episodeToUse = episodeData.Value.Item1 ?? episodeData.Value.Item2;
                    long serverUpdateTime = (long)episodeToUse["lastUpdated"];
                    (int newEps, int updatedEps) = ProcessEpisode(serverUpdateTime, episodeData, si, oldEpisodeIds);
                    numberOfNewEpisodes += newEps;
                    numberOfUpdatedEpisodes += updatedEps;
                }
                catch (InvalidCastException ex)
                {
                    LOGGER.Error(ex, "Did not recieve the expected format of episode json:");
                    LOGGER.Error(episodeData.Value.Item1?.ToString());
                    LOGGER.Error(episodeData.Value.Item2.ToString());
                }
                catch (OverflowException ex)
                {
                    LOGGER.Error(ex, "Did not recieve the expected format of episode json:");
                    LOGGER.Error(episodeData.Value.Item1?.ToString());
                    LOGGER.Error(episodeData.Value.Item2.ToString());
                }
            }

            LOGGER.Info(si.Name + " had " + numberOfUpdatedEpisodes +
                        " episodes updated and " + numberOfNewEpisodes + " new episodes ");

            if (oldEpisodeIds.Count > 0)
            {
                LOGGER.Warn(
                    $"{si.Name} had {oldEpisodeIds.Count} episodes deleted: {string.Join(",", oldEpisodeIds)}");
            }

            foreach (int episodeId in oldEpisodeIds)
            {
                removeEpisodeIds.TryAdd(episodeId, new ExtraEp(si.TvdbCode, episodeId));
            }
        }

        private (int newEps, int updatedEps) ProcessEpisode(long serverUpdateTime,
            KeyValuePair<int, Tuple<JToken, JToken>> episodeData, [NotNull] CachedSeriesInfo si,
            ICollection<int> oldEpisodeIds)
        {
            int newEpisodeCount = 0;
            int updatedEpisodeCount = 0;
            int serverEpisodeId = episodeData.Key;

            bool found = false;
            foreach (Episode ep in si.Episodes.Where(ep => ep.EpisodeId == serverEpisodeId))
            {
                oldEpisodeIds.Remove(serverEpisodeId);

                if (ep.SrvLastUpdated < serverUpdateTime)
                {
                    ep.Dirty = true; // mark episode as dirty.
                    updatedEpisodeCount++;
                }

                found = true;
                break;
            }

            if (!found)
            {
                // must be a new episode
                extraEpisodes.TryAdd(serverEpisodeId, new ExtraEp(si.TvdbCode, serverEpisodeId));
                newEpisodeCount++;
            }

            return (newEpisodeCount, updatedEpisodeCount);
        }

        [NotNull]
        private ICollection<int> GetOldEpisodeIds(int seriesId)
        {
            ICollection<int> oldEpisodeIds = new List<int>();
            foreach (Episode ep in GetSeries(seriesId)?.Episodes ?? new List<Episode>())
            {
                oldEpisodeIds.Add(ep.EpisodeId);
            }

            return oldEpisodeIds;
        }

        private static long GetUpdateTime([NotNull] JObject jsonUpdateResponse)
        {
            try
            {
                string keyName = TVSettings.Instance.TvdbVersion == ApiVersion.v4 ? "timeStamp" : "lastUpdated";
                IEnumerable<long>? updateTimes = jsonUpdateResponse["data"]?.Select(a => (long)a[keyName]);
                long maxUpdateTime = updateTimes?.DefaultIfEmpty(0)?.Max() ?? 0;

                //Add a day to take into account any timezone issues
                long nowTime = DateTime.UtcNow.ToUnixTime() + (int)1.Days().TotalSeconds;
                if (maxUpdateTime > nowTime)
                {
                    LOGGER.Error(
                        $"Assuming up to date: Update time from TVDB API is greater than current time for {maxUpdateTime} > {nowTime} ({Helpers.FromUnixTime(maxUpdateTime)} > {Helpers.FromUnixTime(nowTime)}) from: {jsonUpdateResponse}");

                    return DateTime.UtcNow.ToUnixTime();
                }

                return maxUpdateTime;
            }
            catch (Exception e)
            {
                LOGGER.Error(e, jsonUpdateResponse.ToString());
                return 0;
            }
        }

        private long GetUpdateTimeFromShows()
        {
            // we can use the oldest thing we have locally.  It isn't safe to use the newest thing.
            // This will only happen the first time we do an update, so a false _all update isn't too bad.
            return Series.Values.Where(info => !info.IsSearchResultOnly).Select(info => info.SrvLastUpdated)
                .Where(i => i > 0)
                .DefaultIfEmpty(0).Min();
        }

        private void MarkPlaceholdersDirty()
        {
            // anything with a srv_lastupdated of 0 should be marked as dirty
            // typically, this'll be placeholder cachedSeries
            foreach (CachedSeriesInfo ser in Series.Select(kvp => kvp.Value))
            {
                if (ser.SrvLastUpdated == 0 || ser.Episodes.Count == 0)
                {
                    ser.Dirty = true;
                }

                foreach (Episode ep in ser.Episodes.Where(ep => ep.SrvLastUpdated == 0))
                {
                    ep.Dirty = true;
                }
            }
        }

        public enum PagingMethod
        {
            proper, // uses the links/next method
            brute //keeps asking until we get a 0 length response
        }

        internal List<JObject>? GetEpisodes(int id, Locale locale)
        {
            //Now deal with obtaining any episodes for the cachedSeries
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /cachedSeries/{id}/episodes/summary to get the total
            List<JObject> episodeResponses = new List<JObject>();

            int pageNumber = 1;
            bool morePages = true;
            Language languageToUse = locale.LanguageToUse(TVDoc.ProviderType.TheTVDB);

            while (morePages)
            {
                try
                {
                    JObject jsonEpisodeResponse = API.GetSeriesEpisodes(id, languageToUse.Abbreviation, pageNumber);

                    episodeResponses.Add(jsonEpisodeResponse);
                    try
                    {
                        JToken? jToken = jsonEpisodeResponse["data"];

                        if (jToken is null)
                        {
                            throw new SourceConsistencyException($"Data element not found in {jsonEpisodeResponse}",
                                TVDoc.ProviderType.TheTVDB);
                        }

                        int numberOfResponses = ((JArray)jToken).Count;
                        bool moreResponses;

                        if (TVSettings.TVDBPagingMethod == PagingMethod.proper)
                        {
                            JToken x = jsonEpisodeResponse["links"]?["next"];

                            if (x is null)
                            {
                                throw new SourceConsistencyException(
                                    $"links/next element not found in {jsonEpisodeResponse}",
                                    TVDoc.ProviderType.TheTVDB);
                            }

                            moreResponses = !string.IsNullOrWhiteSpace(x.ToString());
                            LOGGER.Info(
                                $"Page {pageNumber} of {GetSeries(id)?.Name} had {numberOfResponses} episodes listed in {languageToUse.EnglishName} with {(moreResponses ? "" : "no ")}more to come");
                        }
                        else
                        {
                            moreResponses = numberOfResponses > 0;
                            LOGGER.Info(
                                $"Page {pageNumber} of {GetSeries(id)?.Name} had {numberOfResponses} episodes listed in {languageToUse.EnglishName} with {(moreResponses ? "maybe " : "no ")}more to come");
                        }

                        if (numberOfResponses < 100 || !moreResponses)
                        {
                            morePages = false;
                        }
                        else
                        {
                            pageNumber++;
                        }
                    }
                    catch (NullReferenceException nre)
                    {
                        LOGGER.Error(nre,
                            $"Error obtaining page {pageNumber} of episodes for {id} in lang {languageToUse.EnglishName}: Response was {jsonEpisodeResponse}");

                        morePages = false;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse
                        { StatusCode: HttpStatusCode.NotFound })
                    {
                        if (pageNumber > 1 && TVSettings.TVDBPagingMethod == PagingMethod.brute)
                        {
                            LOGGER.Info(
                                $"Have got to the end of episodes for this show: Episodes were not found for {id} from TVDB (got a 404). Error obtaining page {pageNumber} in lang {languageToUse.EnglishName} using url {ex.Response.ResponseUri.AbsoluteUri}");

                            morePages = false;
                        }
                        else
                        {
                            LOGGER.Warn(
                                $"Episodes were not found for {id} from TVDB (got a 404). Error obtaining page {pageNumber} in lang {languageToUse.EnglishName} using url {ex.Response.ResponseUri.AbsoluteUri}");

                            return null;
                        }
                    }
                    else
                    {
                        LOGGER.LogWebException($"Error obtaining episode {id} in {languageToUse.EnglishName}:", ex);
                        return null;
                    }
                }
                catch (IOException ex)
                {
                    LOGGER.Warn(ex, $"Connection to TVDB Failed whilst loading episode with Id {id}.");
                    return null;
                }
            }

            return episodeResponses;
        }

        public void AddOrUpdateEpisode(Episode e)
        {
            lock (SERIES_LOCK)
            {
                if (!Series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the cachedSeries to add the episode to (TheTVDB). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}",
                        TVDoc.ProviderType.TheTVDB);
                }

                CachedSeriesInfo ser = Series[e.SeriesId];

                ser.AddEpisode(e);
            }
        }

        private bool DoWeForceReloadFor(int code)
        {
            return forceReloadOn.ContainsKey(code) || !Series.ContainsKey(code);
        }

        private CachedSeriesInfo? DownloadSeriesNow([NotNull] ISeriesSpecifier deets, bool episodesToo, bool bannersToo,
            bool showErrorMsgBox) =>
            DownloadSeriesNow(deets.TvdbId, episodesToo, bannersToo, deets.TargetLocale, showErrorMsgBox);

        private CachedSeriesInfo? DownloadSeriesNow(int code, bool episodesToo, bool bannersToo, Locale locale,
            bool showErrorMsgBox)
        {
            if (code == 0)
            {
                SayNothing();
                return null;
            }

            bool forceReload = DoWeForceReloadFor(code);

            Say(GenerateMessage(code, episodesToo, bannersToo));

            CachedSeriesInfo? si;
            try
            {
                si = DownloadSeriesInfo(code, locale, showErrorMsgBox);
            }
            catch (SourceConnectivityException)
            {
                SayNothing();
                return null;
            }

            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(si.TvdbCode))
                {
                    Series[si.TvdbCode].Merge(si);
                }
                else
                {
                    Series[si.TvdbCode] = si;
                }

                si = GetSeries(code);
            }

            //Now deal with obtaining any episodes for the cachedSeries (we then group them into seasons)
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /series/{id}/episodes/summary to get the total

            if (episodesToo || forceReload)
            {
                if (si != null)
                {
                    ReloadEpisodes(code, locale, si);
                }
            }

            if (bannersToo || forceReload)
            {
                if (si != null)
                {
                    if (TVSettings.Instance.TvdbVersion != ApiVersion.v4)
                    {
                        DownloadSeriesBanners(code, si, locale);
                    }
                    else
                    {
                        si.BannersLoaded = true; //todo - not really true, but will do for now
                    }
                }
            }

            //V4 has actors in the main request
            if (TVSettings.Instance.TvdbVersion != ApiVersion.v4)
            {
                DownloadSeriesActors(code);
            }

            HaveReloaded(code);

            Series.TryGetValue(code, out CachedSeriesInfo returnValue);
            SayNothing();
            return returnValue;
        }

        private void HaveReloaded(int code)
        {
            forceReloadOn.TryRemove(code, out _);
        }

        [NotNull]
        internal CachedSeriesInfo DownloadSeriesInfo(int code, [NotNull] Locale locale, bool showErrorMsgBox)
        {
            if (!IsConnected && !Connect(showErrorMsgBox))
            {
                Say("Failed to Connect to TVDB");
                SayNothing();
                throw new SourceConnectivityException();
            }

            CachedSeriesInfo si;
            if (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
            {
                Language languagCodeToUse;
                ProcessedSeason.SeasonType st = ProcessedSeason.SeasonType.aired; //todo get this from the show
                (si, languagCodeToUse) = GenerateSeriesInfoV4(DownloadSeriesJson(code, locale), locale, st);

                AddTranslations(si, DownloadSeriesTranslationsJsonV4(code, new Locale(languagCodeToUse)));
            }
            else
            {
                (JObject jsonResponse, JObject jsonDefaultLangResponse) =
                    DownloadSeriesJsonWithTranslations(code, locale);

                si = GenerateSeriesInfo(jsonResponse, jsonDefaultLangResponse, locale);
            }

            if (si is null)
            {
                LOGGER.Error(
                    $"Error obtaining cachedSeries {code} - no cound not generate a cachedSeries from the responses");

                SayNothing();
                throw new SourceConnectivityException();
            }

            return si;
        }

        private void AddTranslations(CachedSeriesInfo si, JObject downloadSeriesTranslationsJsonV4)
        {
            si.Name = downloadSeriesTranslationsJsonV4["data"]["name"].ToString();
            si.Overview = downloadSeriesTranslationsJsonV4["data"]["overview"].ToString();
            //Set a language code on the SI?? si.lan ==downloadSeriesTranslationsJsonV4["data"]["language"].ToString();
            IEnumerable<string> aliases = downloadSeriesTranslationsJsonV4["data"]["aliases"]?.OfType<string>();
            if (aliases == null)
            {
                return;
            }

            foreach (string alias in aliases)
            {
                si.AddAlias(alias);
            }
        }

        private void AddTranslations(CachedMovieInfo si, JObject downloadSeriesTranslationsJsonV4)
        {
            si.Name = downloadSeriesTranslationsJsonV4["data"]["name"].ToString();
            si.Overview = downloadSeriesTranslationsJsonV4["data"]["overview"].ToString();

            //TODO /si.TagLine = downloadSeriesTranslationsJsonV4["data"]["TagLine"].ToString();

            IEnumerable<string> aliases = downloadSeriesTranslationsJsonV4["data"]["aliases"]?.OfType<string>();
            if (aliases == null)
            {
                return;
            }

            foreach (string alias in aliases)
            {
                si.AddAlias(alias);
            }
        }

        internal CachedMovieInfo DownloadMovieInfo(int code, Locale locale, bool showErrorMsgBox)
        {
            if (!IsConnected && !Connect(showErrorMsgBox))
            {
                Say("Failed to Connect to TVDB");
                SayNothing();
                throw new SourceConnectivityException();
            }

            CachedMovieInfo si;
            if (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
            {
                Language languageCode;
                (si, languageCode) = GenerateMovieInfoV4(DownloadMovieJson(code, locale), locale);
                AddTranslations(si, DownloadMovieTranslationsJsonV4(code, new Locale(languageCode)));
            }
            else
            {
                (JObject jsonResponse, JObject jsonDefaultLangResponse) =
                    DownloadMovieWithTranslationsJson(code, locale);

                si = GenerateMovieInfo(jsonResponse, jsonDefaultLangResponse, locale);
            }

            if (si is null)
            {
                LOGGER.Error($"Error obtaining movie {code} - cound not generate a cachedMovie from the responses");
                SayNothing();
                throw new SourceConnectivityException();
            }

            return si;
        }

        private (JObject, JObject) DownloadMovieWithTranslationsJson(int code, Locale locale)
        {
            JObject jsonDefaultLangResponse = new JObject();

            JObject jsonResponse = DownloadMovieJson(code, locale);

            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                jsonDefaultLangResponse = DownloadMovieJson(code, new Locale(this.PreferredLanguage()));
            }

            if (jsonResponse is null)
            {
                LOGGER.Error($"Error obtaining movie information - no response available {code}");
                SayNothing();
                throw new SourceConnectivityException();
            }

            return (jsonResponse, jsonDefaultLangResponse);
        }

        private JObject DownloadMovieJson(int code, Locale locale)
        {
            JObject jsonResponse;
            try
            {
                jsonResponse = TVSettings.Instance.TvdbVersion == ApiVersion.v4
                    ? API.GetMovieV4(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
                    : API.GetMovie(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse
                    { StatusCode: HttpStatusCode.NotFound })
                {
                    LOGGER.Warn($"Movie with Id {code} is no longer available from TVDB (got a 404).");
                    SayNothing();

                    if (API.TvdbIsUp() && !CanFindEpisodesFor(code, locale)
                    ) //todo - CHeck whether this is right? willbe no episodes for a movie
                    {
                        LastErrorMessage = ex.LoggableDetails();
                        string msg = $"Movie with TVDB Id {code} is no longer found on TVDB. Please Update";
                        throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                            TVDoc.ProviderType.TheTVDB);
                    }
                }

                LOGGER.LogWebException(
                    $"Error obtaining movie {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                throw new SourceConnectivityException();
            }

            return jsonResponse;
        }

        private CachedMovieInfo GenerateMovieInfo(JObject jsonResponse, JObject jsonDefaultLangResponse, Locale locale)
        {
            if (jsonResponse is null)
            {
                throw new ArgumentNullException(nameof(jsonResponse));
            }

            JObject seriesData = (JObject)jsonResponse["data"];
            if (seriesData is null)
            {
                throw new SourceConsistencyException($"Data element not found in {jsonResponse}",
                    TVDoc.ProviderType.TheTVDB);
            }

            CachedMovieInfo si;
            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                JObject seriesDataDefaultLang =
                    (JObject)jsonDefaultLangResponse["data"] ?? throw new InvalidOperationException();

                si = GenerateCachedMovieInfo(seriesData, seriesDataDefaultLang,
                    new Locale(TVSettings.Instance.PreferredTVDBLanguage));
            }
            else
            {
                si = GenerateCachedMovieInfo(seriesData, locale);
            }

            return si;
        }

        private CachedMovieInfo GenerateCachedMovieInfo(JObject r, Locale locale)
        {
            CachedMovieInfo si = new CachedMovieInfo(locale)
            {
                //BannerString = GetBannerV4(r),
                FirstAired = GetReleaseDate(r, locale.RegionToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation) ??
                             GetReleaseDate(r, "global"),
                TvdbCode = (int)r["id"],
                Imdb = GetExternalId(r, "IMDB"),
                Runtime = ((string)r["runtime"])?.Trim(),
                Name = GetTranslation(locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), "name", r),
                TagLine = GetTranslation(locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), "tagline", r),
                Overview = GetTranslation(locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), "overview", r),
                TrailerUrl = r["trailers"]?.FirstOrDefault()?["url"]?.ToString(),
                Genres = r["genres"]?.Select(x => x["name"].ToString()).ToList(),
                IsSearchResultOnly = false,
                Dirty = false,
                PosterUrl = "https://artworks.thetvdb.com" + GetArtwork(r, "Poster"),
                FanartUrl = "https://artworks.thetvdb.com" + GetArtwork(r, "Background"),
                OfficialUrl = GetExternalId(r, "Official Website"),
                FacebookId = GetExternalId(r, "Facebook"),
                InstagramId = GetExternalId(r, "Instagram"),
                TwitterId = GetExternalId(r, "Twitter"),
                //Icon = "https://artworks.thetvdb.com" + GetArtwork(r, "Icon"), TODO - Other Image Downloads
            };

            if (!(r["people"]?["actors"] is null))
            {
                foreach (var actorJson in r["people"]["actors"])
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["name"]?.ToString();
                    string image = "https://artworks.thetvdb.com" + actorJson["people_image"];
                    string role = actorJson["role"]?.ToString();
                    si.AddActor(new Actor(id, image, name, role, 0, id));
                }
            }

            if (!(r["people"]?["directors"] is null))
            {
                foreach (var actorJson in r["people"]["directors"])
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["name"]?.ToString();
                    string image = "https://artworks.thetvdb.com" + actorJson["people_image"];
                    string role = actorJson["role"]?.ToString();
                    si.AddCrew(
                        new Crew(id, image, name, role.HasValue() ? role : "Director", "Directing", string.Empty));
                }
            }

            if (!(r["people"]?["producers"] is null))
            {
                foreach (var actorJson in r["people"]["producers"])
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["name"]?.ToString();
                    string image = "https://artworks.thetvdb.com" + actorJson["people_image"];
                    string role = actorJson["role"]?.ToString();
                    si.AddCrew(new Crew(id, image, name, role.HasValue() ? role : "Producer", "Production",
                        string.Empty));
                }
            }

            if (!(r["people"]?["writers"] is null))
            {
                foreach (var actorJson in r["people"]["writers"])
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["name"]?.ToString();
                    string image = "https://artworks.thetvdb.com" + actorJson["people_image"];
                    string role = actorJson["role"]?.ToString();
                    si.AddCrew(new Crew(id, image, name, role.HasValue() ? role : "Writer", "Writing", string.Empty));
                }
            }

            return si;
        }

        private string? GetArtwork(JObject json, string type)
        {
            return json["artworks"]?.FirstOrDefault(x => x["artwork_type"].ToString() == type)?["url"]?.ToString();
        }

        private string? GetArtworkV4(JObject json, int type)
        {
            return json["data"]["artworks"]?.FirstOrDefault(x => ((int)x["type"]) == type)?["image"]
                ?.ToString(); //TODO use max score to get preferred
        }

        private string? GetExternalId(JObject json, string source)
        {
            return json["remoteids"]?.FirstOrDefault(x => x["source_name"].ToString() == source)?["id"]?.ToString();
        }

        private string? GetExternalIdV4(JObject json, string source)
        {
            return json["data"]["remoteIds"]?.FirstOrDefault(x => x["sourceName"].ToString() == source)?["id"]
                ?.ToString();
        }

        private string? GetContentRatingV4(JObject json, string country)
        {
            return json["data"]["contentRatings"]?.FirstOrDefault(x => x["country"].ToString() == country)?["name"]
                ?.ToString();
        }

        private DateTime? GetReleaseDate(JObject json, string region)
        {
            string date =
                json["release_dates"]?.FirstOrDefault(x =>
                    x["country"].ToString() == region && x["type"].ToString() == "release_date")?["date"]?.ToString();

            try
            {
                if (!date.HasValue())
                {
                    return null;
                }

                return DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private DateTime? GetReleaseDateV4(JObject json, string region)
        {
            string date = region is null
                ? json["data"]["releases"]?.FirstOrDefault()?["date"]?.ToString()
                : json["data"]["releases"]?.FirstOrDefault(x => x["country"].ToString() == region)?["date"]?.ToString();

            try
            {
                if (!date.HasValue())
                {
                    return null;
                }

                return DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private string? GetTranslation(Language preferredLanguage, string field, JObject json)
        {
            return json["translations"]
                ?.FirstOrDefault(x => x["language_code"].ToString() == preferredLanguage.ThreeAbbreviation)?[field]
                ?.ToString();
        }

        private CachedMovieInfo GenerateCachedMovieInfo(JObject r, JObject seriesDataDefaultLang, Locale locale)
        {
            CachedMovieInfo si = GenerateCachedMovieInfo(r, locale);
            //todo see if there is anything useful in the other json
            return si;
        }

        private (CachedMovieInfo, Language) GenerateMovieInfoV4(JObject r, Locale locale)
        {
            Language lang = locale.LanguageToUse(TVDoc.ProviderType.TheTVDB);
            CachedMovieInfo si = new CachedMovieInfo(locale)
            {
                FirstAired = GetReleaseDateV4(r, locale.RegionToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation) ??
                             GetReleaseDateV4(r, "global") ?? GetReleaseDateV4(r, null),
                TvdbCode = (int)r["data"]["id"],
                Slug = ((string)r["data"]["slug"])?.Trim(),
                Imdb = GetExternalIdV4(r, "IMDB"),
                Runtime = ((string)r["data"]["runtime"])?.Trim(),
                Name = r["data"]["name"]?.ToString(),
                TrailerUrl = r["data"]["trailers"]?.FirstOrDefault(x =>
                        x["language"]?.ToString() == locale.RegionToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)?[
                        "url"]
                    ?.ToString(),
                IsSearchResultOnly = false,
                Dirty = false,
                PosterUrl = "https://artworks.thetvdb.com" + GetArtworkV4(r, 14),
                FanartUrl = "https://artworks.thetvdb.com" + GetArtworkV4(r, 15),
                //TODO multiple posters and artwork
                //todo BannerUrl = "https://artworks.thetvdb.com" + GetArtworkV4(r, 16),
                OfficialUrl = GetExternalIdV4(r, "Official Website"),
                FacebookId = GetExternalIdV4(r, "Facebook"),
                InstagramId = GetExternalIdV4(r, "Instagram"),
                TwitterId = GetExternalIdV4(r, "Twitter"),
                //Icon = "https://artworks.thetvdb.com" + GetArtwork(r, "Icon"), TODO - Other Image Downloads
                Network = r["data"]["studios"]?.FirstOrDefault()?["name"].ToString(),
                ShowLanguage = r["data"]["audioLanguages"]?.ToString(),
                ContentRating = GetContentRatingV4(r, "aus") ?? GetContentRatingV4(r, "usa"),
                Status = r["data"]["status"]["name"]?.ToString(),
                SrvLastUpdated = ((DateTime)r["data"]["lastUpdated"]).ToUnixTime(),
                Genres = r["data"]["genres"]?.Select(x => x["name"].ToString()).ToList(),
                CollectionId = r["data"]["lists"]?.FirstOrDefault(x =>
                        x["isOfficial"].ToString() == "true" &&
                        ((JArray)x["nameTranslations"]).ContainsTyped(lang.ThreeAbbreviation))?["id"]
                    .ToString().ToInt(),

                //todo - get collection name translations
                CollectionName = r["data"]["lists"]?.FirstOrDefault(x =>
                        x["isOfficial"].ToString() == "true" &&
                        ((JArray)x["nameTranslations"]).ContainsTyped(lang.ThreeAbbreviation))?[
                        "name"]
                    .ToString(),

                //todo load multiple companies r.data.companies
                //todo load country?  "originalCountry": "usa",
            };

            if (!(r["data"]?["aliases"] is null))
            {
                foreach (var x in r["data"]["aliases"]?.Where(x => x["language"]?.ToString() == lang.ThreeAbbreviation))
                {
                    si.AddAlias(x["name"]?.ToString());
                }
            }

            if (!(r["data"]?["characters"] is null))
            {
                foreach (var actorJson in r["data"]["characters"]?.Where(x => x["peopleType"]?.ToString() == "Actor"))
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["personName"]?.ToString();
                    string image = "https://artworks.thetvdb.com" + actorJson["image"];
                    string role = actorJson["name"]?.ToString();
                    int? sort = actorJson["sort"]?.ToString().ToInt();
                    si.AddActor(new Actor(id, image, name, role, 0, sort));
                }

                foreach (var actorJson in r["data"]["characters"]?.Where(x => x["peopleType"]?.ToString() != "Actor"))
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["personName"]?.ToString();
                    string role = actorJson["peopleType"]?.ToString();
                    string sort = actorJson["sort"]?.ToString();
                    si.AddCrew(new Crew(id, string.Empty, name, role, string.Empty, sort));
                }
            }

            return (si, GetAppropriateLanguage(r["data"]["nameTranslations"], locale));
        }

        private (CachedSeriesInfo, Language) GenerateSeriesInfoV4(JObject r, Locale locale,
            ProcessedSeason.SeasonType seasonType)
        {
            CachedSeriesInfo si = new CachedSeriesInfo(locale)
            {
                AirsTime = GetAirsTimeV4(r),
                TvdbCode = (int)r["data"]["id"],
                Imdb = GetExternalIdV4(r, "IMDB"),
                OfficialUrl = GetExternalIdV4(r, "Official Website"),
                FacebookId = GetExternalIdV4(r, "Facebook"),
                InstagramId = GetExternalIdV4(r, "Instagram"),
                TwitterId = GetExternalIdV4(r, "Twitter"),
                TmdbCode = GetExternalIdV4(r, "TheMovieDB.com")?.ToInt() ?? -1,
                SeriesId = GetExternalIdV4(r, "TV.com"),
                PosterUrl = GetArtworkV4(r, 2),
                IsSearchResultOnly = false,
                Dirty = false,
                Slug = ((string)r["data"]["slug"])?.Trim(),
                Genres = r["data"]["genres"]?.Select(x => x["name"].ToString()).ToList(),
                ShowLanguage = r["data"]["originalLanguage"]?.ToString(),
                TrailerUrl = r["data"]["trailers"]?.FirstOrDefault(x =>
                        x["language"]?.ToString() ==
                        locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)?["url"]
                    ?.ToString(), //todo use lang code and backup to first in any language
                SrvLastUpdated = ((DateTime)r["data"]["lastUpdated"]).ToUnixTime(),
                Status = (string)r["data"]["status"]["name"],
                FirstAired = JsonHelper.ParseFirstAired((string)r["data"]["firstAired"]),
                AirsDay = GetAirsDayV4(r),
                Network = r["data"]["companies"]
                    ?.FirstOrDefault(x => x["companyType"]["companyTypeName"].ToString() == "Network")?["name"]
                    ?.ToString(),

                //todo load country?  "originalCountry": "usa",
                //todo load multiple companies r.data.companies
            };

            if (!(r["data"]?["aliases"] is null))
            {
                foreach (var x in r["data"]["aliases"]?.Where(x =>
                    x["language"]?.ToString() == locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
                ) //todo use lang code and backup to first in any language
                {
                    si.AddAlias(x["name"]?.ToString());
                }
            }

            string s = (string)r["data"]["name"];
            if (s != null)
            {
                si.Name = System.Web.HttpUtility.HtmlDecode(s).Trim();
            }

            if (!(r["data"]?["characters"] is null))
            {
                foreach (var actorJson in r["data"]["characters"]?.Where(x => x["peopleType"]?.ToString() == "Actor"))
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["personName"]?.ToString();
                    string image = "https://artworks.thetvdb.com" + actorJson["image"];
                    string role = actorJson["name"]?.ToString();
                    int? sort = actorJson["sort"]?.ToString().ToInt();
                    si.AddActor(new Actor(id, image, name, role, 0, sort));
                }

                foreach (var actorJson in r["data"]["characters"]?.Where(x => x["peopleType"]?.ToString() != "Actor"))
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["personName"]?.ToString();
                    string role = actorJson["peopleType"]?.ToString();
                    string sort = actorJson["sort"]?.ToString();
                    si.AddCrew(new Crew(id, string.Empty, name, role, string.Empty, sort));
                }
            }

            if (!(r["data"]?["seasons"] is null))
            {
                foreach (var seasonJson in r["data"]["seasons"])
                {
                    if (seasonType == getSeasonType(seasonJson))
                    {
                        int SeasonId = (int)seasonJson["id"];
                        string SeasonName = (string)seasonJson["name"] + " " + seasonJson["number"];
                        int SeasonSeriesId = (int)seasonJson["seriesId"];
                        int SeasonNumber = (int)seasonJson["number"];
                        string SeasonDescription = string.Empty;
                        string ImageUrl = (string)seasonJson["image"];
                        string url = string.Empty;

                        si.AddSeason(new Season(SeasonId, SeasonNumber
                            , SeasonName, SeasonDescription, url, ImageUrl, SeasonSeriesId));
                    }
                }
            }

            return (si, GetAppropriateLanguage(r["data"]["nameTranslations"], locale));
        }

        private ProcessedSeason.SeasonType getSeasonType(JToken seasonJson)
        {
            if (seasonJson["type"]["type"].ToString() == "official") return ProcessedSeason.SeasonType.aired;
            if (seasonJson["type"]["type"].ToString() == "dvd") return ProcessedSeason.SeasonType.dvd;
            if (seasonJson["type"]["type"].ToString() == "absolute") return ProcessedSeason.SeasonType.absolute;
            return ProcessedSeason.SeasonType.absolute;
        }

        private Language GetAppropriateLanguage(JToken languageOptions, Locale preferredLocale)
        {
            if (((JArray)languageOptions).ContainsTyped(preferredLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB)
                .ThreeAbbreviation))
            {
                return preferredLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB);
            }

            if ((((JArray)languageOptions).Count == 1))
            {
                return Languages.Instance.GetLanguageFromThreeCode(((JArray)languageOptions).Single().ToString());
            }

            if (((JArray)languageOptions).ContainsTyped(TVSettings.Instance.PreferredTVDBLanguage.ThreeAbbreviation))
            {
                return TVSettings.Instance.PreferredTVDBLanguage;
            }

            if ((((JArray)languageOptions).Count == 0))
            {
                throw new SourceConsistencyException($"Element exists with no language", TVDoc.ProviderType.TheTVDB);
            }

            if (((JArray)languageOptions).ContainsTyped(Languages.Instance.FallbackLanguage.ThreeAbbreviation))
            {
                return Languages.Instance.FallbackLanguage;
            }

            return
                Languages.Instance.GetLanguageFromThreeCode(((JArray)languageOptions).First
                    .ToString()); //todo make  use of the json to find the most appropriate language code
        }

        private DateTime? GetAirsTimeV4(JObject r)
        {
            string airsTimeString = (string)r["data"]["airsTime"];
            return JsonHelper.ParseAirTime(airsTimeString);
        }

        private string GetAirsDayV4(JObject r)
        {
            JToken jTokens = r["data"]["airsDays"];
            IEnumerable<JToken> days = jTokens.Children().Where(token => (bool)token.Values().First());
            return days.Select(ConvertDayName).ToCsv();
        }

        private string ConvertDayName(JToken t)
        {
            JProperty p = (JProperty)t;
            return p.Name.UppercaseFirst();
        }

        [NotNull]
        private CachedSeriesInfo GenerateSeriesInfo([NotNull] JObject jsonResponse, JObject jsonDefaultLangResponse,
            Locale locale)
        {
            if (jsonResponse is null)
            {
                throw new ArgumentNullException(nameof(jsonResponse));
            }

            if (locale is null)
            {
                throw new ArgumentNullException(nameof(locale));
            }

            JObject seriesData = (JObject)jsonResponse["data"];
            if (seriesData is null)
            {
                throw new SourceConsistencyException($"Data element not found in {jsonResponse}",
                    TVDoc.ProviderType.TheTVDB);
            }

            CachedSeriesInfo si;
            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                JObject seriesDataDefaultLang =
                    (JObject)jsonDefaultLangResponse["data"] ?? throw new InvalidOperationException();

                si = new CachedSeriesInfo(seriesData, seriesDataDefaultLang, locale);
            }
            else
            {
                si = new CachedSeriesInfo(seriesData, locale, false);
            }

            return si;
        }

        private (JObject jsonResponse, JObject jsonDefaultLangResponse) DownloadSeriesJsonWithTranslations(int code,
            Locale locale)
        {
            JObject jsonDefaultLangResponse = new JObject();

            JObject jsonResponse = DownloadSeriesJson(code, locale);

            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                jsonDefaultLangResponse = DownloadSeriesJson(code, new Locale(PreferredLanguage()));
            }

            if (jsonResponse is null)
            {
                LOGGER.Error($"Error obtaining cachedSeries information - no response available {code}");
                SayNothing();
                throw new SourceConnectivityException();
            }

            return (jsonResponse, jsonDefaultLangResponse);
        }

        [NotNull]
        private JObject DownloadSeriesJson(int code, Locale locale)
        {
            JObject jsonResponse;
            try
            {
                jsonResponse = TVSettings.Instance.TvdbVersion == ApiVersion.v4
                    ? API.GetSeriesV4(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
                    : API.GetSeries(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse
                    { StatusCode: HttpStatusCode.NotFound })
                {
                    LOGGER.Warn($"Show with Id {code} is no longer available from TVDB (got a 404).");
                    SayNothing();

                    if (API.TvdbIsUp() && !CanFindEpisodesFor(code, locale))
                    {
                        LastErrorMessage = ex.LoggableDetails();
                        string msg = $"Show with TVDB Id {code} is no longer found on TVDB. Please Update";
                        throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                            TVDoc.ProviderType.TheTVDB);
                    }
                }

                LOGGER.LogWebException(
                    $"Error obtaining cachedSeries {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                throw new SourceConnectivityException();
            }

            return jsonResponse;
        }

        [NotNull]
        private JObject DownloadMovieTranslationsJsonV4(int code, Locale locale)
        {
            try
            {
                return API.GetMovieTranslationsV4(code,
                    locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation);
            }
            catch (WebException ex)
            {
                LOGGER.LogWebException(
                    $"Error obtaining translations for {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                throw new SourceConnectivityException();
            }
        }

        [NotNull]
        private JObject DownloadSeriesTranslationsJsonV4(int code, Locale locale)
        {
            try
            {
                return API.GetSeriesTranslationsV4(code,
                    locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation);
            }
            catch (WebException ex)
            {
                LOGGER.LogWebException(
                    $"Error obtaining translations for {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                throw new SourceConnectivityException();
            }
        }

        private static bool CanFindEpisodesFor(int code, Locale locale)
        {
            try
            {
                API.GetSeriesEpisodes(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse
                    { StatusCode: HttpStatusCode.NotFound })
                {
                    return false;
                }
            }

            return true;
        }

        private void DownloadSeriesActors(int code)
        {
            //Get the actors too then we'll need another call for that
            try
            {
                CachedSeriesInfo si = GetSeries(code);

                if (si is null)
                {
                    LOGGER.Warn($"Asked to get actors for cachedSeries with id:{code}, but it can't be found");
                    return;
                }

                JObject jsonActorsResponse = API.GetSeriesActors(code);

                si.ClearActors();
                JToken? jsonActors = jsonActorsResponse["data"];
                if (jsonActors is null)
                {
                    throw new SourceConsistencyException($"Data element not found in {jsonActorsResponse}",
                        TVDoc.ProviderType.TheTVDB);
                }

                foreach (JToken jsonActor in jsonActors)
                {
                    int actorId = (int)jsonActor["id"];
                    string actorImage = (string)jsonActor["image"];
                    string actorName = (string)jsonActor["name"] ??
                                       throw new SourceConsistencyException("No Actor", TVDoc.ProviderType.TheTVDB);

                    string actorRole = (string)jsonActor["role"];
                    int actorSeriesId = (int)jsonActor["seriesId"];
                    int actorSortOrder = (int)jsonActor["sortOrder"];

                    si.AddActor(new Actor(actorId, actorImage, actorName, actorRole, actorSeriesId,
                        actorSortOrder));
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is null) //probably a timeout
                {
                    LOGGER.LogWebException($"Unble to obtain actors for {Series[code].Name}", ex);
                }
                else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    LOGGER.Info(
                        $"No actors found for {Series[code].Name} using url {ex.Response.ResponseUri.AbsoluteUri}");
                }
                else
                {
                    LOGGER.LogWebException($"Unble to obtain actors for {Series[code].Name}", ex);
                }

                LastErrorMessage = ex.LoggableDetails();
            }
        }

        private void DownloadSeriesBanners(int code, [NotNull] CachedSeriesInfo si, Locale locale)
        {
            (List<JObject> bannerDefaultLangResponses, List<JObject> bannerResponses) = DownloadBanners(code, locale);

            List<int> latestBannerIds = new List<int>();

            ProcessBannerResponses(code, si, locale, bannerResponses, latestBannerIds);
            ProcessBannerResponses(code, si, new Locale(TVSettings.Instance.PreferredTVDBLanguage),
                bannerDefaultLangResponses, latestBannerIds);

            si.UpdateBanners(latestBannerIds);

            si.BannersLoaded = true;
        }

        private void ProcessBannerResponses(int code, CachedSeriesInfo si, Locale locale,
            [NotNull] IEnumerable<JObject> bannerResponses,
            ICollection<int> latestBannerIds)
        {
            foreach (JObject response in bannerResponses)
            {
                JToken? jToken = response["data"];
                if (jToken is null)
                {
                    throw new SourceConsistencyException($"Data element not found in {response}",
                        TVDoc.ProviderType.TheTVDB);
                }

                try
                {
                    foreach (Banner b in jToken
                        .Cast<JObject>()
                        .Select(bannerData => new Banner(si.TvdbCode, bannerData,
                            locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBId)))
                    {
                        lock (SERIES_LOCK)
                        {
                            si.AddOrUpdateBanner(b);
                            latestBannerIds.Add(b.BannerId);
                        }
                    }
                }
                catch (InvalidCastException ex)
                {
                    LOGGER.Error(ex,
                        $"Did not receive the expected format of json from when downloading banners for cachedSeries {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");

                    LOGGER.Error(jToken.ToString());
                }
            }
        }

        private static (List<JObject> bannerDefaultLangResponses, List<JObject> bannerResponses) DownloadBanners(
            int code,
            Locale locale)
        {
            // get /series/id/images if the bannersToo is set - may need to make multiple calls to for each image type

            IEnumerable<string> imageTypes =
                API.GetImageTypes(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation);

            List<JObject> bannerResponses = API.GetImages(code,
                locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation, imageTypes);

            if (!locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                return (new List<JObject>(), bannerResponses);
            }

            IEnumerable<string> imageDefaultLangTypes =
                API.GetImageTypes(code, TVSettings.Instance.PreferredTVDBLanguage.Abbreviation);

            List<JObject> bannerDefaultLangResponses =
                API.GetImages(code, TVSettings.Instance.PreferredTVDBLanguage.Abbreviation, imageDefaultLangTypes);

            return (bannerDefaultLangResponses, bannerResponses);
        }

        [NotNull]
        private string GenerateMessage(int code, bool episodesToo, bool bannersToo)
        {
            string txt;
            if (Series.ContainsKey(code))
            {
                txt = Series[code].Name.HasValue() ? Series[code].Name : "Code " + code;
            }
            else
            {
                txt = "Code " + code;
            }

            if (episodesToo)
            {
                txt += " (Everything)";
            }
            else
            {
                txt += " Overview";
            }

            if (bannersToo)
            {
                txt += " plus banners";
            }

            return txt;
        }

        private void ReloadEpisodesV4(int code, Locale locale, CachedSeriesInfo si)
        {
            Parallel.ForEach(si.Seasons, s =>
            {
                try
                {
                    JObject seasonInfo = API.GetSeasonEpisoedesV4(code, s.SeasonId,
                        locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation);

                    JToken episodeData = seasonInfo["data"]["episodes"];
                    if (episodeData != null)
                    {
                        Parallel.ForEach(episodeData, x =>
                        {
                            try
                            {
                                (Episode newEp, Language bestLanguage) = GenerateEpisodeV4(x, code, si, locale);
                                AddTranslations(newEp,
                                    API.GetEpisodeTranslationsV4(newEp.EpisodeId, bestLanguage.ThreeAbbreviation));

                                si.AddEpisode(newEp);
                            }
                            catch (SourceConnectivityException sce1)
                            {
                                LOGGER.Error(sce1);
                            }
                            catch (SourceConsistencyException sce1)
                            {
                                LOGGER.Error(sce1);
                            }
                        });
                    }
                }
                catch (SourceConnectivityException sce)
                {
                    LOGGER.Error(sce);
                }
                catch (SourceConsistencyException sce)
                {
                    LOGGER.Error(sce);
                }
            });
        }

        private void AddTranslations(Episode newEp, JObject downloadSeriesTranslationsJsonV4)
        {
            newEp.Name = downloadSeriesTranslationsJsonV4["data"]["name"].ToString();
            newEp.Overview = downloadSeriesTranslationsJsonV4["data"]["overview"].ToString();
            //Set a language code on the SI?? si.lan ==downloadSeriesTranslationsJsonV4["data"]["language"].ToString();
        }

        private (Episode, Language) GenerateEpisodeV4(JToken episodeJson, int code, CachedSeriesInfo si, Locale locale)
        {
            Episode x = new Episode(code, si)
            {
                EpisodeId = episodeJson["id"].ToObject<int>(),
                SeriesId = episodeJson["seriesId"].ToObject<int>(),
                Name = episodeJson["name"].ToObject<string>(),
                FirstAired = GetEpisodeAiredDate(episodeJson),
                Runtime = episodeJson["runtime"].ToObject<string>(),
                AiredSeasonNumber = episodeJson["seasonNumber"].ToObject<int>(),
                AiredEpNum = episodeJson["number"].ToObject<int>(),
                Filename = episodeJson["image"].ToObject<string>(),
            };

            return (x, GetAppropriateLanguage(episodeJson["nameTranslations"], locale));
        }

        private DateTime? GetEpisodeAiredDate(JToken episodeJson)
        {
            string date = episodeJson["aired"]?.ToString();
            try
            {
                if (!date.HasValue())
                {
                    return null;
                }

                return DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private void ReloadEpisodes(int code, Locale locale, CachedSeriesInfo si)
        {
            if (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
            {
                ReloadEpisodesV4(code, locale, si);
                return;
            }

            List<JObject> episodePrefLangResponses = GetEpisodes(code, locale);
            List<JObject> episodeDefaultLangResponses = null;
            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                episodeDefaultLangResponses = GetEpisodes(code, new Locale(TVSettings.Instance.PreferredTVDBLanguage));
            }

            Dictionary<int, Tuple<JToken, JToken>> episodeResponses =
                MergeEpisodeResponses(episodePrefLangResponses, episodeDefaultLangResponses);

            Parallel.ForEach(episodeResponses, episodeData =>
            {
                int episodeId = episodeData.Key;
                JToken prefLangEpisode = episodeData.Value.Item1;
                JToken defltLangEpisode = episodeData.Value.Item2;
                try
                {
                    //TODO - Establish whether this has any value?
                    UpdateEpisodeNow(code, prefLangEpisode, defltLangEpisode, si);
                }
                catch (InvalidCastException ex)
                {
                    LOGGER.Error(ex,
                        $"<TVDB ISSUE?>: Did not recieve the expected format of json for {episodeId}. {prefLangEpisode} ::: {defltLangEpisode}");
                }
                catch (OverflowException ex)
                {
                    LOGGER.Error(ex,
                        $"<TVDB ISSUE?>: Could not parse the episode json from {episodeId}. {prefLangEpisode} ::: {defltLangEpisode}");
                }
            });
        }

        [NotNull]
        private static Dictionary<int, Tuple<JToken, JToken>> MergeEpisodeResponses(
            List<JObject>? episodeResponses, List<JObject>? episodeDefaultLangResponses)
        {
            Dictionary<int, Tuple<JToken, JToken>> episodeIds = new Dictionary<int, Tuple<JToken, JToken>>();

            if (episodeResponses != null)
            {
                foreach (JObject epResponse in episodeResponses)
                {
                    JToken? episodeDatas = epResponse["data"];
                    if (episodeDatas is null)
                    {
                        throw new SourceConsistencyException($"Could not get data element from {epResponse}",
                            TVDoc.ProviderType.TheTVDB);
                    }

                    foreach (JToken episodeData in episodeDatas)
                    {
                        int x = (int)episodeData["id"];
                        if (x > 0)
                        {
                            if (episodeIds.ContainsKey(x))
                            {
                                LOGGER.Warn($"Duplicate episode {x} contained in episode data call");
                            }
                            else
                            {
                                episodeIds.Add(x, new Tuple<JToken, JToken>(episodeData, null));
                            }
                        }
                    }
                }
            }

            if (episodeDefaultLangResponses != null)
            {
                foreach (JObject epResponse in episodeDefaultLangResponses)
                {
                    JToken? episodeDatas = epResponse["data"];
                    if (episodeDatas is null)
                    {
                        throw new SourceConsistencyException($"Could not get data element from {epResponse}",
                            TVDoc.ProviderType.TheTVDB);
                    }

                    foreach (JToken episodeData in episodeDatas)
                    {
                        int x = (int)episodeData["id"];
                        if (x > 0)
                        {
                            if (episodeIds.ContainsKey(x))
                            {
                                JToken old = episodeIds[x].Item1;
                                episodeIds[x] = new Tuple<JToken, JToken>(old, episodeData);
                            }
                            else
                            {
                                episodeIds.Add(x, new Tuple<JToken, JToken>(null, episodeData));
                            }
                        }
                    }
                }
            }

            return episodeIds;
        }

        private bool DownloadEpisodeNow(int seriesId, int episodeId, Locale locale, bool dvdOrder = false)
        {
            if (episodeId == 0)
            {
                LOGGER.Warn($"Asked to download episodeId = 0 for cachedSeries {seriesId}");
                SayNothing();
                return true;
            }

            if (!Series.ContainsKey(seriesId))
            {
                return false; // shouldn't happen
            }

            Episode ep = FindEpisodeById(episodeId);
            string eptxt = EpisodeDescription(dvdOrder, episodeId, ep);

            CachedSeriesInfo cachedSeriesInfo = Series[seriesId];
            Say($"{cachedSeriesInfo.Name} ({eptxt}) in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");

            JObject jsonEpisodeResponse;
            JObject jsonEpisodeDefaultLangResponse = new JObject();

            try
            {
                jsonEpisodeResponse =
                    API.GetEpisode(episodeId, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation);

                if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
                {
                    jsonEpisodeDefaultLangResponse = API.GetEpisode(episodeId,
                        (TVSettings.Instance.PreferredTVDBLanguage.Abbreviation));
                }
            }
            catch (WebException ex)
            {
                LOGGER.LogWebException($"Error obtaining episode[{episodeId}]:", ex);

                LastErrorMessage = ex.LoggableDetails();
                return false;
            }
            finally
            {
                SayNothing();
            }

            JObject jsonResponseData = (JObject)jsonEpisodeResponse["data"] ??
                                       throw new SourceConsistencyException("No Data in Ep Response",
                                           TVDoc.ProviderType.TheTVDB);

            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                JObject seriesDataDefaultLang = (JObject)jsonEpisodeDefaultLangResponse["data"];
                return UpdateEpisodeNow(seriesId, jsonResponseData, seriesDataDefaultLang, cachedSeriesInfo);
            }
            else
            {
                return UpdateEpisodeNow(seriesId, jsonResponseData, null, cachedSeriesInfo);
            }
        }

        private bool UpdateEpisodeNow(int seriesId, JToken jsonResponseData, JToken? seriesDataDefaultLang,
            CachedSeriesInfo si)
        {
            if (!Series.ContainsKey(seriesId))
            {
                return false; // shouldn't happen
            }

            try
            {
                Episode e = seriesDataDefaultLang != null
                    ? new Episode(seriesId, (JObject)jsonResponseData, (JObject)seriesDataDefaultLang, si)
                    : new Episode(seriesId, (JObject)jsonResponseData, si);

                if (e.Ok())
                {
                    AddOrUpdateEpisode(e);
                }
                else
                {
                    LOGGER.Error(
                        $"<TVDB ISSUE?>: problem with JSON received for episode {jsonResponseData} - {seriesDataDefaultLang}");
                }
            }
            catch (SourceConsistencyException e)
            {
                LOGGER.Error("<TVDB ISSUE?>: Could not parse TVDB Response " + e.Message);
                LastErrorMessage = e.Message;
                SayNothing();
                return false;
            }

            return true;
        }

        [NotNull]
        private static string EpisodeDescription(bool dvdOrder, int episodeId, Episode? ep)
        {
            if (ep == null)
            {
                return "New Episode Id = " + episodeId;
            }

            if (dvdOrder)
            {
                return $"S{ep.DvdSeasonNumber:00}E{ep.DvdEpNum:00}";
            }

            return $"S{ep.AiredSeasonNumber:00}E{ep.AiredEpNum:00}";
        }

        private void AddPlaceholderSeries(int tvdb, int tvmaze, int tmdb, Locale locale)
        {
            Series[tvdb] = new CachedSeriesInfo(tvdb, tvmaze, tmdb, locale) { Dirty = true };
        }

        public override Language? PreferredLanguage() => TVSettings.Instance.PreferredTVDBLanguage;

        public override bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
        {
            if (s.Provider != TVDoc.ProviderType.TheTVDB)
            {
                throw new SourceConsistencyException(
                    $"Asked to update {s.Name} from The TVDB, but the Id is not for The TVDB.",
                    TVDoc.ProviderType.TheTVDB);
            }

            if (s.Type == MediaConfiguration.MediaType.movie)
            {
                return EnsureMovieUpdated(s.TvdbId, s.TargetLocale, s.Name, showErrorMsgBox);
            }

            return EnsureSeriesUpdated(s, bannersToo, showErrorMsgBox);
        }

        private bool EnsureMovieUpdated(int id, Locale locale, string name, bool showErrorMsgBox)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id) && !Movies[id].Dirty)
                {
                    return true;
                }
            }

            Say($"Movie: {name} from The TVDB");
            try
            {
                CachedMovieInfo downloadedSi = DownloadMovieNow(id, locale, showErrorMsgBox);

                if (downloadedSi.TvdbCode != id && id == -1)
                {
                    lock (MOVIE_LOCK)
                    {
                        Movies.TryRemove(-1, out _);
                    }
                }

                lock (MOVIE_LOCK)
                {
                    AddMovieToCache(downloadedSi);
                }
            }
            catch (SourceConnectivityException conex)
            {
                LastErrorMessage = conex.Message;
                return true;
            }
            catch (SourceConsistencyException sce)
            {
                LOGGER.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return true;
            }
            finally
            {
                SayNothing();
            }

            return true;
        }

        private void AddMovieToCache([NotNull] CachedMovieInfo si)
        {
            int id = si.TvdbCode;
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id))
                {
                    Movies[id].Merge(si);
                }
                else
                {
                    Movies[id] = si;
                }
            }
        }

        private bool EnsureSeriesUpdated(ISeriesSpecifier seriesd, bool bannersToo, bool showErrorMsgBox)
        {
            int code = seriesd.TvdbId;

            if (DoWeForceReloadFor(code) || Series[code].Episodes.Count == 0)
            {
                return DownloadSeriesNow(seriesd, true, bannersToo, showErrorMsgBox) != null; // the whole lot!
            }

            bool ok = true;

            bool seriesNeedsUpdating = Series[code].Dirty;
            bool bannersNeedUpdating = bannersToo && !Series[code].BannersLoaded;
            if (seriesNeedsUpdating || bannersNeedUpdating)
            {
                ok = DownloadSeriesNow(seriesd, false, bannersToo, showErrorMsgBox) != null;
            }

            ICollection<Episode> collection = Series[code]?.Episodes;
            if (collection != null)
            {
                foreach (Episode e in collection.Where(e => e.Dirty && e.EpisodeId > 0))
                {
                    extraEpisodes.TryAdd(e.EpisodeId, new ExtraEp(e.SeriesId, e.EpisodeId));
                }
            }

            Parallel.ForEach(extraEpisodes, ee =>
            {
                if (ee.Value.SeriesId != code || ee.Value.Done)
                {
                    return;
                }

                ok = DownloadEpisodeNow(ee.Value.SeriesId, ee.Key, seriesd.TargetLocale) && ok;
                ee.Value.Done = true;
            });

            foreach (ExtraEp episodetoRemove in removeEpisodeIds.Values)
            {
                Series[episodetoRemove.SeriesId].RemoveEpisode(episodetoRemove.EpisodeId);
            }

            removeEpisodeIds.Clear();

            HaveReloaded(code);

            return ok;
        }

        public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type, Locale locale)
        {
            if (!IsConnected && !Connect(showErrorMsgBox))
            {
                Say("Failed to Connect to TVDB");
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                Say("Please Search for a Show Name");
                return;
            }

            text = text.RemoveDiacritics(); // API doesn't like accented characters

            bool isNumber = Regex.Match(text, "^[0-9]+$").Success;

            if (isNumber)
            {
                if (int.TryParse(text, out int textAsInt))
                {
                    try
                    {
                        switch (type)
                        {
                            case MediaConfiguration.MediaType.tv:
                                DownloadSeriesNow(textAsInt, false, false, locale, showErrorMsgBox);
                                break;

                            case MediaConfiguration.MediaType.movie:
                                DownloadMovieNow(textAsInt, locale, showErrorMsgBox);
                                break;
                        }
                    }
                    catch (MediaNotFoundException)
                    {
                        //not really an issue so we can continue
                    }
                }
            }

            // but, the number could also be a name, so continue searching as usual
            //text = text.Replace(".", " ");

            JObject jsonSearchResponse = null;
            JObject jsonSearchDefaultLangResponse = null;
            try
            {
                jsonSearchResponse = TVSettings.Instance.TvdbVersion == ApiVersion.v4
                    ? API.SearchV4(text, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation, type)
                    : type == MediaConfiguration.MediaType.tv
                        ? API.SearchTvShow(text, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation)
                        : null; //Can't search for Movies in the v3 API
            }
            catch (WebException ex)
            {
                if (ex.Response is null) //probably a timeout
                {
                    LOGGER.LogWebException($"Error obtaining results for search term '{text}':", ex);
                    LastErrorMessage = ex.LoggableDetails();
                    SayNothing();
                }
                else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    LOGGER.Info(
                        $"Could not find any search results for {text} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");
                }
                else
                {
                    LOGGER.LogWebException($"Error obtaining results for search term '{text}':", ex);
                    LastErrorMessage = ex.LoggableDetails();
                    SayNothing();
                }
            }

            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB) &&
                TVSettings.Instance.TvdbVersion != ApiVersion.v4 && type == MediaConfiguration.MediaType.tv)
            {
                try
                {
                    jsonSearchDefaultLangResponse =
                        API.SearchTvShow(text, TVSettings.Instance.PreferredTVDBLanguage.Abbreviation);
                }
                catch (WebException ex)
                {
                    if (ex.Response is null) //probably a timeout
                    {
                        LOGGER.LogWebException(
                            $"Error obtaining results for search term '{text}' in {TVSettings.Instance.PreferredTVDBLanguage.EnglishName}:",
                            ex);

                        LastErrorMessage = ex.LoggableDetails();
                        SayNothing();
                    }
                    else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        LOGGER.Info(
                            $"Could not find any search results for {text} in {TVSettings.Instance.PreferredTVDBLanguage.EnglishName}");
                    }
                    else
                    {
                        LOGGER.Error(
                            $"Error obtaining {ex.Response.ResponseUri} for search term '{text}' in {TVSettings.Instance.PreferredTVDBLanguage.EnglishName}: {ex.LoggableDetails()}");

                        LastErrorMessage = ex.LoggableDetails();
                        SayNothing();
                    }
                }
            }

            if (jsonSearchResponse != null)
            {
                ProcessSearchResult(jsonSearchResponse, locale);
            }

            if (jsonSearchDefaultLangResponse != null)
            //we also want to search for search terms that match in default language
            {
                ProcessSearchResult(jsonSearchDefaultLangResponse,
                    new Locale(TVSettings.Instance.PreferredTVDBLanguage));
            }
        }

        private void ProcessSearchResult([NotNull] JObject jsonResponse, Locale locale)
        {
            JToken? jToken = jsonResponse["data"];
            if (jToken is null)
            {
                throw new SourceConsistencyException($"Could not get data element from {jsonResponse}",
                    TVDoc.ProviderType.TheTVDB);
            }

            try
            {
                IEnumerable<CachedSeriesInfo> cachedSeriesInfos = (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
                    ? GetEnumSeriesV4(jToken, locale, true)
                    : jToken.Cast<JObject>()
                        .Select(seriesResponse => new CachedSeriesInfo(seriesResponse, locale, true));

                foreach (CachedSeriesInfo si in cachedSeriesInfos)
                {
                    lock (SERIES_LOCK)
                    {
                        if (Series.ContainsKey(si.TvdbCode))
                        {
                            Series[si.TvdbCode].Merge(si);
                        }
                        else
                        {
                            Series[si.TvdbCode] = si;
                        }
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                LOGGER.Error("<TVDB ISSUE?>: Did not receive the expected format of json from search results.");
                LOGGER.Error(ex);
                LOGGER.Error(jToken.ToString());
            }
        }

        private IEnumerable<CachedSeriesInfo> GetEnumSeriesV4(JToken jToken, Locale locale, bool b)
        {
            JArray ja = (JArray)jToken;
            List<CachedSeriesInfo> ses = new List<CachedSeriesInfo>();

            foreach (JToken jt in ja.Children())
            {
                JObject showJson = (JObject)jt;
                ses.Add(GenerateSeriesV4(showJson, locale, b));
            }

            return ses;
        }

        private CachedSeriesInfo GenerateSeriesV4(JObject r, Locale locale, bool searchResult)
        {
            CachedSeriesInfo si = new CachedSeriesInfo(locale)
            {
                TvdbCode = (int)r["tvdb_id"],
                Slug = ((string)r["id"])?.Trim(),
                PosterUrl = (string)r["image_url"],
                Overview = (string)r["overview"],
                Network = (string)r["network"],
                Status = (string)r["status"],
                IsSearchResultOnly = searchResult,
                ShowLanguage = (string)r["primary_language"],
                FirstAired = new DateTime((int)r["year"], 1, 1),
            };

            string s = (string)r["name"];
            if (s != null)
            {
                si.Name = System.Web.HttpUtility.HtmlDecode(s).Trim();
            }

            if (string.IsNullOrEmpty(si.Name))
            {
                LOGGER.Warn("Issue with cachedSeries " + si);
                LOGGER.Warn(r.ToString());
            }

            if (si.SrvLastUpdated == 0 && !searchResult)
            {
                LOGGER.Warn("Issue with cachedSeries (update time is 0) " + si);
                LOGGER.Warn(r.ToString());
                si.SrvLastUpdated = 100;
            }

            return si;
        }

        // Next episode to air of a given show
        /*
                public Episode? NextAiring(int code)
                {
                    if (!cachedSeries.ContainsKey(code) || cachedSeries[code].AiredSeasons.Count == 0)
                    {
                        return null; // DownloadSeries(code, true);
                    }

                    Episode next = null;
                    DateTime today = DateTime.Now;
                    DateTime mostSoonAfterToday = new DateTime(0);

                    CachedSeriesInfo ser = cachedSeries[code];
                    foreach (KeyValuePair<int, Season> kvp2 in ser.AiredSeasons)
                    {
                        Season s = kvp2.Value;

                        foreach (Episode e in s.Episodes.Values)
                        {
                            LocalDateTime? adt = e.GetAirDateDt();
                            if (adt is null)
                            {
                                continue;
                            }

                            LocalDateTime dt = (LocalDateTime) adt;
                            if (dt.CompareTo(today) > 0 && (mostSoonAfterToday.CompareTo(new DateTime(0)) == 0 ||
                                                              dt.CompareTo(mostSoonAfterToday) < 0))
                            {
                                mostSoonAfterToday = dt;
                                next = e;
                            }
                        }
                    }

                    return next;
                }
        */

        public void Tidy(IEnumerable<ShowConfiguration> libraryValues)
        {
            // remove any shows from thetvdb that aren't in My Shows
            List<int> removeList = new List<int>();
            List<ShowConfiguration> showConfigurations = libraryValues.ToList();

            lock (SERIES_LOCK)
            {
                foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series)
                {
                    bool found = showConfigurations.Any(si => si.TvdbCode == kvp.Key);
                    if (!found)
                    {
                        removeList.Add(kvp.Key);
                    }
                }

                foreach (int i in removeList)
                {
                    ForgetShow(i);
                }
            }
        }

        public void SaveCache()
        {
            lock (SERIES_LOCK)
            {
                CachePersistor.SaveCache(Series, Movies, CacheFile,
                    LatestUpdateTime.LastSuccessfulServerUpdateTimecode());
            }
        }

        public void UpdateSeries(CachedSeriesInfo si)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(si.TvdbCode))
                {
                    Series[si.TvdbCode].Merge(si);
                }
                else
                {
                    Series[si.TvdbCode] = si;
                }
            }
        }

        public void AddBanners(int seriesId, IEnumerable<Banner> seriesBanners)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(seriesId))
                {
                    foreach (Banner b in seriesBanners)
                    {
                        if (!Series.ContainsKey(b.SeriesId))
                        {
                            throw new SourceConsistencyException(
                                $"Can't find the cachedSeries to add the banner {b.BannerId} to (TheTVDB). {seriesId},{b.SeriesId}",
                                TVDoc.ProviderType.TheTVDB);
                        }

                        CachedSeriesInfo ser = Series[b.SeriesId];

                        ser.AddOrUpdateBanner(b);
                    }

                    Series[seriesId].BannersLoaded = true;
                }
                else
                {
                    LOGGER.Warn($"Banners were found for cachedSeries {seriesId} - Ignoring them.");
                }
            }
        }

        public void LatestUpdateTimeIs(string time)
        {
            LatestUpdateTime.Load(time);
            LOGGER.Info($"Loaded file with updates until {LatestUpdateTime.LastSuccessfulServerUpdateDateTime()}");
        }

        public CachedMovieInfo? GetMovieAndDownload(int id, Locale locale, bool showErrorMsgBox) => HasMovie(id)
            ? CachedMovieData[id]
            : DownloadMovieNow(id, locale, showErrorMsgBox);

        private CachedMovieInfo? DownloadMovieNow(int tvdbId, Locale locale, bool showErrorMsgBox)
        {
            if (tvdbId == 0)
            {
                SayNothing();
                return null;
            }

            bool forceReload = DoWeForceReloadFor(tvdbId);

            Say($"Movie with id {tvdbId} from TheTVDB");

            CachedMovieInfo? si;
            try
            {
                si = DownloadMovieInfo(tvdbId, locale, showErrorMsgBox);
            }
            catch (SourceConnectivityException)
            {
                SayNothing();
                return null;
            }

            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(si.TvdbCode))
                {
                    Movies[si.TvdbCode].Merge(si);
                }
                else
                {
                    Movies[si.TvdbCode] = si;
                }

                si = GetMovie(tvdbId);
            }

            //TODO Reinstate
            //DownloadMovieActors(tvdbId);

            HaveReloaded(tvdbId);

            lock (MOVIE_LOCK)
            {
                Movies.TryGetValue(tvdbId, out CachedMovieInfo returnValue);
                SayNothing();
                return returnValue;
            }
        }

        public void ReConnect(bool showErrorMsgBox)
        {
            Say("TheTVDB Login");
            try
            {
                API.Login(true);
                IsConnected = true;
            }
            catch (WebException ex)
            {
                HandleConnectionProblem(showErrorMsgBox, ex);
            }
            finally
            {
                SayNothing();
            }
        }
    }
}
