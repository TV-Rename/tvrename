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
    public enum ApiVersion
    {
        v2,
        v3,
        v4
    }

    // ReSharper disable once InconsistentNaming
    public class LocalCache :  MediaCache, iTVSource, iMovieSource
    {
#nullable enable
        public static readonly ApiVersion VERS = ApiVersion.v3; //TODO - Revert and make user selectable

        private ConcurrentDictionary<int, ExtraEp>
            extraEpisodes; // IDs of extra episodes to grab and merge in on next update

        private ConcurrentDictionary<int, ExtraEp> removeEpisodeIds; // IDs of episodes that should be removed

        private ConcurrentDictionary<int, int> forceReloadOn;
        private UpdateTimeTracker LatestUpdateTime;


        // ReSharper disable once ConvertToConstant.Local
        private static readonly string DefaultLanguageCode = "en"; //Default backup language

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

        public string LastErrorMessage { get; set; }

        public void Setup(FileInfo? loadFrom, FileInfo cache, CommandLineArgs cla)
        {
            args = cla;

            System.Diagnostics.Debug.Assert(cache != null);
            CacheFile = cache;

            LastErrorMessage = string.Empty;
            IsConnected = false;
            extraEpisodes = new ConcurrentDictionary<int, ExtraEp>();
            removeEpisodeIds = new ConcurrentDictionary<int, ExtraEp>();

            LanguageList = new Languages { new Language(7, "en", "English", "English") };

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
            : DownloadSeriesNow(id, false, false, false, TVSettings.Instance.PreferredLanguageCode,showErrorMsgBox);

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

        public CachedMovieInfo GetMovie(PossibleNewMovie show, bool showErrorMsgBox) => throw new NotImplementedException();

        public CachedMovieInfo GetMovie(int? id) => throw new NotImplementedException();

        public CachedSeriesInfo? GetSeries(string showName, bool showErrorMsgBox)
        {
            Search(showName, showErrorMsgBox,MediaConfiguration.MediaType.tv);

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
                foreach (CachedSeriesInfo si in Series.Values.Where(info => !info.IsSearchResultOnly).OrderBy(s=>s.Name).ToList())
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

        public bool Connect(bool showErrorMsgBox)
        {
            switch (VERS)
            {
                case ApiVersion.v2:
                case ApiVersion.v3:
                    IsConnected = UpdateLanguages(showErrorMsgBox);
                    break;
                case ApiVersion.v4:
                    IsConnected = TVDBLogin(showErrorMsgBox);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return IsConnected;
        }

        public void Tidy(IEnumerable<MovieConfiguration> libraryValues)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void ForgetMovie(int tvdb, int tvmaze, int tmdb, bool makePlaceholder, bool useCustomLanguage, string langCode)
        {
            throw new NotImplementedException();
        }

        public void Update(CachedMovieInfo si)
        {
            throw new NotImplementedException();
        }

        public void AddPoster(int seriesId, IEnumerable<Banner> @select)
        {
            throw new NotImplementedException();
        }

        public void ForgetShow(int tvdb, int tvmaze, int tmdb, bool makePlaceholder, bool useCustomLanguage, string? customLanguageCode)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(tvdb))
                {
                    Series.TryRemove(tvdb, out CachedSeriesInfo _);

                    if (makePlaceholder)
                    {
                        if (useCustomLanguage && customLanguageCode.HasValue())
                        {
                            AddPlaceholderSeries(tvdb, tvmaze,tmdb, customLanguageCode!);
                        }
                        else
                        {
                            AddPlaceholderSeries(tvdb, tvmaze,tmdb);
                        }

                        forceReloadOn.TryAdd(tvdb, tvdb);
                    }
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
                API.Login();
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



        private bool UpdateLanguages(bool showErrorMsgBox)
        {
            Say("TheTVDB Languages");
            try
            {
                JObject jsonLanguagesResponse = API.GetLanguages();

                lock (LANGUAGE_LOCK)
                {
                    LanguageList ??= new Languages();

                    JToken? jTokens = jsonLanguagesResponse["data"];
                    if (jTokens is null)
                    {
                        throw new SourceConsistencyException($"Data element not found in {jsonLanguagesResponse}",
                            TVDoc.ProviderType.TheTVDB);
                    }

                    LanguageList.LoadLanguages(jTokens.Select(GenerateLanguage).Where(language => language != null));
                }

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

        private static Language? GenerateLanguage(JToken jToken)
        {
            JObject languageJson = (JObject)jToken;
            int? id = (int?)languageJson["id"];
            if (!id.HasValue)
            {
                return null;
            }

            string name = (string)languageJson["name"];
            string englishName = (string)languageJson["englishName"];
            string abbrev = (string)languageJson["abbreviation"];

            if (id != -1 && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(abbrev))
            {
                return new Language(id.Value, abbrev, name, englishName);
            }

            return null;
        }

        private void HandleConnectionProblem(bool showErrorMsgBox, WebException ex)
        {
            Say("Could not connect to TVDB");

            LOGGER.LogWebException("Error obtaining Languages from TVDB",ex);
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

        private void AddPlaceholderSeries([NotNull] SeriesSpecifier ss)
            => AddPlaceholderSeries(ss.TvdbSeriesId, ss.TvMazeSeriesId, ss.TmdbId,ss.CustomLanguageCode);

        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, IEnumerable<SeriesSpecifier> ss)
        {
            Say("Validating TheTVDB cache");
            foreach (SeriesSpecifier downloadShow in ss.Where(downloadShow => !HasSeries(downloadShow.TvdbSeriesId)))
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
                return API.GetShowUpdatesSince(updateFromEpochTime, TVSettings.Instance.PreferredLanguageCode);
            }
            catch (IOException iex)
            {
                LOGGER.LogIoException($"Error obtaining lastupdated query since (local) {requestedTime.ToLocalTime()}: Message is", iex);

                SayNothing();
                LastErrorMessage = iex.LoggableDetails();
                return null;
            }
            catch (WebException ex)
            {
                LOGGER.LogWebException($"Error obtaining lastupdated query since (local) {requestedTime.ToLocalTime()}: Message is", ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                return null;
            }
            catch (AggregateException aex) when (aex.InnerException is WebException ex)
            {
                LOGGER.LogWebException($"Error obtaining lastupdated query since (local) {requestedTime.ToLocalTime()}: Message is", ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                return null;
            }
            catch (AggregateException aex) when (aex.InnerException is System.Net.Http.HttpRequestException ex)
            {
                LOGGER.LogHttpRequestException($"Error obtaining lastupdated query since (local) {requestedTime.ToLocalTime()}: Message is", ex);

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
                    LOGGER.Info($"Planning to download all of {kvp.Value.Name} as {percentDirty}% of the episodes need to be updated");
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
                throw new SourceConsistencyException($"Could not get data element from {jsonResponse}", TVDoc.ProviderType.TheTVDB);
            }

            try
            {
                foreach (JObject seriesResponse in jToken.Cast<JObject>())
                {
                    if (!cts.IsCancellationRequested)
                    {
                        ProcessSeriesUpdate(seriesResponse);
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
            string requestedLanguageCode = selectedCachedSeriesInfo.UseCustomLanguage
                ? selectedCachedSeriesInfo.TargetLanguageCode ?? TVSettings.Instance.PreferredLanguageCode
                : TVSettings.Instance.PreferredLanguageCode;

            try
            {
                List<JObject> episodeResponses = GetEpisodes(id, requestedLanguageCode);
                if (episodeResponses is null)
                {
                    //we got nothing good back from TVDB
                    LOGGER.Warn($"Aborting updates for {selectedCachedSeriesInfo.Name} ({id}) as there was an issue obtaining episodes");
                    return;
                }
                if (IsNotDefaultLanguage(requestedLanguageCode))
                {
                    episodeDefaultLangResponses = GetEpisodes(id, DefaultLanguageCode);
                    if (episodeDefaultLangResponses is null)
                    {
                        //we got nothing good back from TVDB
                        LOGGER.Warn($"Aborting updates for {selectedCachedSeriesInfo.Name} ({id}) as there was an issue obtaining episodes in {DefaultLanguageCode}");
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
                    $"Episodes were not found for {ex.ShowId}:{selectedCachedSeriesInfo.Name} in languange {requestedLanguageCode} or {DefaultLanguageCode}");
            }
            catch (KeyNotFoundException kex)
            {
                //We assue this is due to the update being for a recently removed show.
                LOGGER.Error(kex);
            }
        }

        private void ProcessEpisodes([NotNull] CachedSeriesInfo si, [NotNull] Dictionary<int, Tuple<JToken, JToken>> episodesResponses)
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
            KeyValuePair<int, Tuple<JToken, JToken>> episodeData, [NotNull] CachedSeriesInfo si, ICollection<int> oldEpisodeIds)
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
                IEnumerable<long> updateTimes = from a in jsonUpdateResponse["data"] select (long)a["lastUpdated"];
                long maxUpdateTime = updateTimes.DefaultIfEmpty(0).Max();

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
            return Series.Values.Where(info => !info.IsSearchResultOnly).Select(info => info.SrvLastUpdated).Where(i => i > 0)
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

        internal List<JObject>? GetEpisodes(int id, string lang)
        {
            //Now deal with obtaining any episodes for the cachedSeries
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /cachedSeries/{id}/episodes/summary to get the total
            List<JObject> episodeResponses = new List<JObject>();

            int pageNumber = 1;
            bool morePages = true;

            while (morePages)
            {
                try
                {
                    JObject jsonEpisodeResponse = API.GetSeriesEpisodes(id, lang, pageNumber);

                    episodeResponses.Add(jsonEpisodeResponse);
                    try
                    {
                        JToken? jToken = jsonEpisodeResponse["data"];

                        if (jToken is null)
                        {
                            throw new SourceConsistencyException($"Data element not found in {jsonEpisodeResponse}", TVDoc.ProviderType.TheTVDB);
                        }
                        int numberOfResponses = ((JArray)jToken).Count;
                        bool moreResponses;

                        if (TVSettings.TVDBPagingMethod == PagingMethod.proper)
                        {
                            JToken x = jsonEpisodeResponse["links"]?["next"];

                            if (x is null)
                            {
                                throw new SourceConsistencyException($"links/next element not found in {jsonEpisodeResponse}", TVDoc.ProviderType.TheTVDB);
                            }
                            moreResponses = !string.IsNullOrWhiteSpace(x.ToString());
                            LOGGER.Info(
                                $"Page {pageNumber} of {GetSeries(id)?.Name} had {numberOfResponses} episodes listed in {lang} with {(moreResponses ? "" : "no ")}more to come");
                        }
                        else
                        {
                            moreResponses = numberOfResponses > 0;
                            LOGGER.Info(
                                $"Page {pageNumber} of {GetSeries(id)?.Name} had {numberOfResponses} episodes listed in {lang} with {(moreResponses ? "maybe " : "no ")}more to come");
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
                            $"Error obtaining page {pageNumber} of episodes for {id} in lang {lang}: Response was {jsonEpisodeResponse}");

                        morePages = false;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse {StatusCode: HttpStatusCode.NotFound})
                    {
                        if (pageNumber > 1 && TVSettings.TVDBPagingMethod == PagingMethod.brute)
                        {
                            LOGGER.Info(
                                $"Have got to the end of episodes for this show: Episodes were not found for {id} from TVDB (got a 404). Error obtaining page {pageNumber} in lang {lang} using url {ex.Response.ResponseUri.AbsoluteUri}");

                            morePages = false;
                        }
                        else
                        {
                            LOGGER.Warn(
                                $"Episodes were not found for {id} from TVDB (got a 404). Error obtaining page {pageNumber} in lang {lang} using url {ex.Response.ResponseUri.AbsoluteUri}");

                            return null;
                        }
                    }
                    else
                    {
                        LOGGER.LogWebException($"Error obtaining episode {id} in {lang}:",ex);
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

        private int GetLanguageId() =>
            LanguageList?.GetLanguageFromCode(TVSettings.Instance.PreferredLanguageCode)?.Id ?? 7;

        private int GetDefaultLanguageId() => LanguageList?.GetLanguageFromCode(DefaultLanguageCode)?.Id ?? 7;

        public void AddOrUpdateEpisode(Episode e)
        {
            lock (SERIES_LOCK)
            {
                if (!Series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the cachedSeries to add the episode to (TheTVDB). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}", TVDoc.ProviderType.TheTVDB);
                }

                CachedSeriesInfo ser = Series[e.SeriesId];

                ser.AddEpisode(e);
            }
        }

        private bool DoWeForceReloadFor(int code)
        {
            return forceReloadOn.ContainsKey(code) || !Series.ContainsKey(code);
        }

        private CachedSeriesInfo? DownloadSeriesNow([NotNull] SeriesSpecifier deets, bool episodesToo, bool bannersToo, bool showErrorMsgBox) =>
            DownloadSeriesNow(deets.TvdbSeriesId, episodesToo, bannersToo, deets.UseCustomLanguage,
                deets.CustomLanguageCode,showErrorMsgBox);

        private CachedSeriesInfo? DownloadSeriesNow(int code, bool episodesToo, bool bannersToo, bool useCustomLangCode,
            string langCode, bool showErrorMsgBox)
        {
            if (code == 0)
            {
                SayNothing();
                return null;
            }

            bool forceReload = DoWeForceReloadFor(code);

            Say(GenerateMessage(code, episodesToo, bannersToo));

            string requestedLanguageCode = useCustomLangCode ? langCode : TVSettings.Instance.PreferredLanguageCode;
            if (string.IsNullOrWhiteSpace(requestedLanguageCode))
            {
                LOGGER.Error(
                    $"An error has occurred and identified in DownloadSeriesNow and cachedSeries {code} has a blank language code. Using the default instead for now: {TVSettings.Instance.PreferredLanguageCode}");

                requestedLanguageCode = TVSettings.Instance.PreferredLanguageCode;
                if (string.IsNullOrWhiteSpace(requestedLanguageCode))
                {
                    requestedLanguageCode = "en";
                }
            }

            CachedSeriesInfo? si;
            try
            {
                si = DownloadSeriesInfo(code, requestedLanguageCode,showErrorMsgBox);
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
                    ReloadEpisodes(code, useCustomLangCode, langCode, si);
                }
            }

            if (bannersToo || forceReload)
            {
                if (si != null)
                {
                    DownloadSeriesBanners(code, si, requestedLanguageCode);
                }
            }

            DownloadSeriesActors(code);

            forceReloadOn.TryRemove(code, out _);

            Series.TryGetValue(code, out CachedSeriesInfo returnValue);
            SayNothing();
            return returnValue;
        }

        [NotNull]
        internal CachedSeriesInfo DownloadSeriesInfo(int code, [NotNull] string requestedLanguageCode, bool showErrorMsgBox)
        {
            if (!IsConnected && !Connect(showErrorMsgBox))
            {
                Say("Failed to Connect to TVDB");
                SayNothing();
                throw new SourceConnectivityException();
            }

            CachedSeriesInfo si;
            if (VERS == ApiVersion.v4)
            {
                si = GenerateSeriesInfoV4(DownloadSeriesJson(code, requestedLanguageCode));
            }
            else
            {
                bool isNotDefaultLanguage = IsNotDefaultLanguage(requestedLanguageCode);

                (JObject jsonResponse, JObject jsonDefaultLangResponse) =
                    DownloadSeriesJson(code, requestedLanguageCode, isNotDefaultLanguage);

                si = GenerateSeriesInfo(jsonResponse, jsonDefaultLangResponse, isNotDefaultLanguage,
                    requestedLanguageCode);
            }

            if (si is null)
            {
                LOGGER.Error($"Error obtaining cachedSeries {code} - no cound not generate a cachedSeries from the responses");
                SayNothing();
                throw new SourceConnectivityException();
            }

            return si;
        }

        private CachedSeriesInfo GenerateSeriesInfoV4(JObject r)
        {
            CachedSeriesInfo si = new CachedSeriesInfo
            {
                AirsDay = GetAirsDayV4(r),
                AirsTime = GetAirsTimeV4(r),
                //BannerString = GetBannerV4(r),
                FirstAired = JsonHelper.ParseFirstAired((string) r["data"]["firstAired"]),
                TvdbCode = (int) r["data"]["id"],
                //Imdb = ((string) r["imdbId"])?.Trim(),
                Network = ((string) r["data"]["originalNetwork"]["name"])?.Trim(),
                Slug = ((string) r["data"]["slug"])?.Trim(),
                //Overview = System.Web.HttpUtility.HtmlDecode((string) r["overview"])?.Trim(),
                //ContentRating = ((string) r["rating"])?.Trim(),
                //Runtime = ((string) r["runtime"])?.Trim(),
                //SeriesId = (string) r["seriesId"],
                Status = (string) r["data"]["status"]["name"]
            };
            //si.Aliases = (r["aliases"] ?? throw new SourceConsistencyException($"Can't find aliases in Series JSON: {r}", TVDoc.ProviderType.TheTVDB)).Select(x => x.Value<string>()).ToList();

            string s = (string)r["data"]["name"];
            if (s != null)
            {
                si.Name = System.Web.HttpUtility.HtmlDecode(s).Trim();
            }

            //si.SrvLastUpdated = long.TryParse((string)r["lastUpdated"], out long updateTime) ? updateTime : 0;

            if (r.ContainsKey("genre"))
            {
                si.Genres = r["data"]["genre"]?.Select(x => x.Value<string>().Trim()).Distinct().ToList() ?? new List<string>();
            }


            //string siteRatingString = (string)r["siteRating"];
            //float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRating);

            //string siteRatingVotesString = (string)r["siteRatingCount"];
            //int.TryParse(siteRatingVotesString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRatingVotes);
            return si;
        }

        private string? GetBannerV4(JObject r)
        {
            return (string) r["banner"];
        }

        private DateTime? GetAirsTimeV4(JObject r)
        {
            string airsTimeString = (string)r["data"]["airsTime"];
            return JsonHelper.ParseAirTime(airsTimeString);
        }

        private string GetAirsDayV4(JObject r)
        {
            JToken jTokens = r["data"]["airsDays"];
            IEnumerable<JToken> days = jTokens.Children().Where(token => (bool )token.Values().First());
            return days.Select(ConvertDayName).ToCsv();
        }

        private string ConvertDayName(JToken t)
        {
            JProperty p = (JProperty) t;
            return p.Name.UppercaseFirst();
        }

        [NotNull]
        private CachedSeriesInfo GenerateSeriesInfo([NotNull] JObject jsonResponse, JObject jsonDefaultLangResponse,
            bool isNotDefaultLanguage,
            [NotNull] string requestedLanguageCode)
        {
            if (jsonResponse is null)
            {
                throw new ArgumentNullException(nameof(jsonResponse));
            }

            if (requestedLanguageCode is null)
            {
                throw new ArgumentNullException(nameof(requestedLanguageCode));
            }

            if (LanguageList is null)
            {
                throw new ArgumentException("LanguageList not Setup", nameof(LanguageList));
            }

            Language languageFromCode = LanguageList.GetLanguageFromCode(requestedLanguageCode);
            if (languageFromCode is null)
            {
                throw new ArgumentException(
                    $"Requested language ({requestedLanguageCode}) not found in Language Cache, cache has ({LanguageList.Select(language => language.Abbreviation).ToCsv()})",
                    requestedLanguageCode);
            }
            int requestedLangId = languageFromCode.Id;

            JObject seriesData = (JObject)jsonResponse["data"];
            if (seriesData is null)
            {
                throw new SourceConsistencyException($"Data element not found in {jsonResponse}", TVDoc.ProviderType.TheTVDB);
            }
            CachedSeriesInfo si;
            if (isNotDefaultLanguage)
            {
                JObject seriesDataDefaultLang = (JObject)jsonDefaultLangResponse["data"] ?? throw new InvalidOperationException();

                si = new CachedSeriesInfo(seriesData, seriesDataDefaultLang, requestedLangId);
            }
            else
            {
                si = new CachedSeriesInfo(seriesData, requestedLangId, false);
            }

            return si;
        }

        private (JObject jsonResponse, JObject jsonDefaultLangResponse) DownloadSeriesJson(int code,
            string requestedLanguageCode, bool isNotDefaultLanguage)
        {
            JObject jsonDefaultLangResponse = new JObject();

            JObject jsonResponse = DownloadSeriesJson(code, requestedLanguageCode);

            if (isNotDefaultLanguage)
            {
                jsonDefaultLangResponse = DownloadSeriesJson(code, DefaultLanguageCode);
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
        private JObject DownloadSeriesJson(int code, string requestedLanguageCode)
        {
            JObject jsonResponse;
            try
            {
                jsonResponse = VERS==ApiVersion.v4
                    ?API.GetSeriesV4(code, requestedLanguageCode)
                    :API.GetSeries(code, requestedLanguageCode);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse {StatusCode: HttpStatusCode.NotFound})
                {
                    LOGGER.Warn($"Show with Id {code} is no longer available from TVDB (got a 404).");
                    SayNothing();

                    if (API.TvdbIsUp() && !CanFindEpisodesFor(code, requestedLanguageCode))
                    {
                        LastErrorMessage = ex.LoggableDetails();
                        string msg = $"Show with TVDB Id {code} is no longer found on TVDB. Please Update";
                        throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TheTVDB);
                    }
                }
                LOGGER.LogWebException($"Error obtaining cachedSeries {code} in {requestedLanguageCode}:",ex);
                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                throw new SourceConnectivityException();
            }

            return jsonResponse;
        }

        private static bool CanFindEpisodesFor(int code, string requestedLanguageCode)
        {
            try
            {
                API.GetSeriesEpisodes(code, requestedLanguageCode);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse {StatusCode: HttpStatusCode.NotFound})
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
                    throw new SourceConsistencyException($"Data element not found in {jsonActorsResponse}", TVDoc.ProviderType.TheTVDB);
                }
                foreach (JToken jsonActor in jsonActors)
                {
                    int actorId = (int)jsonActor["id"];
                    string actorImage = (string)jsonActor["image"];
                    string actorName = (string)jsonActor["name"] ?? throw new SourceConsistencyException("No Actor", TVDoc.ProviderType.TheTVDB);
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

        private void DownloadSeriesBanners(int code, [NotNull] CachedSeriesInfo si, string requestedLanguageCode)
        {
            (List<JObject> bannerDefaultLangResponses, List<JObject> bannerResponses) =
                DownloadBanners(code, requestedLanguageCode);

            List<int> latestBannerIds = new List<int>();

            ProcessBannerResponses(code, si, GetLanguageId(), requestedLanguageCode, bannerResponses, latestBannerIds);
            ProcessBannerResponses(code, si, GetDefaultLanguageId(), DefaultLanguageCode, bannerDefaultLangResponses, latestBannerIds);

            si.UpdateBanners(latestBannerIds);

            si.BannersLoaded = true;
        }

        private void ProcessBannerResponses(int code, CachedSeriesInfo si, int languageId, string languageCode, [NotNull] IEnumerable<JObject> bannerResponses,
            ICollection<int> latestBannerIds)
        {
            foreach (JObject response in bannerResponses)
            {
                JToken? jToken = response["data"];
                if (jToken is null)
                {
                    throw new SourceConsistencyException($"Data element not found in {response}", TVDoc.ProviderType.TheTVDB);
                }
                try
                {
                    foreach (Banner b in jToken
                        .Cast<JObject>()
                        .Select(bannerData => new Banner(si.TvdbCode, bannerData, languageId)))
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
                        $"Did not receive the expected format of json from when downloading banners for cachedSeries {code} in {languageCode}");

                    LOGGER.Error(jToken.ToString());
                }
            }
        }

        private static (List<JObject> bannerDefaultLangResponses, List<JObject> bannerResponses) DownloadBanners(int code,
            string requestedLanguageCode)
        {
            // get /series/id/images if the bannersToo is set - may need to make multiple calls to for each image type

            IEnumerable<string> imageTypes = API.GetImageTypes(code, requestedLanguageCode);

            List<JObject> bannerResponses = API.GetImages(code, requestedLanguageCode, imageTypes);

            if (!IsNotDefaultLanguage(requestedLanguageCode))
            {
                return (new List<JObject>(), bannerResponses);
            }

            IEnumerable<string> imageDefaultLangTypes = API.GetImageTypes(code, DefaultLanguageCode);

            List<JObject> bannerDefaultLangResponses =
                API.GetImages(code, DefaultLanguageCode, imageDefaultLangTypes);

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

        private void ReloadEpisodes(int code, bool useCustomLangCode, string langCode, CachedSeriesInfo si)
        {
            string requestLangCode = useCustomLangCode ? langCode : TVSettings.Instance.PreferredLanguageCode;
            List<JObject> episodePrefLangResponses = GetEpisodes(code, requestLangCode);
            List<JObject> episodeDefaultLangResponses = null;
            if (IsNotDefaultLanguage(requestLangCode))
            {
                episodeDefaultLangResponses = GetEpisodes(code, DefaultLanguageCode);
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
                        throw new SourceConsistencyException($"Could not get data element from {epResponse}", TVDoc.ProviderType.TheTVDB);
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
                        throw new SourceConsistencyException($"Could not get data element from {epResponse}", TVDoc.ProviderType.TheTVDB);
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

        private static bool InForeignLanguage() => DefaultLanguageCode != TVSettings.Instance.PreferredLanguageCode;

        private static bool IsNotDefaultLanguage(string languageCode) => DefaultLanguageCode != languageCode;

        private bool DownloadEpisodeNow(int seriesId, int episodeId, string requestLangCode, bool dvdOrder = false)
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
            Say($"{cachedSeriesInfo.Name} ({eptxt}) in {requestLangCode}");

            JObject jsonEpisodeResponse;
            JObject jsonEpisodeDefaultLangResponse = new JObject();

            try
            {
                jsonEpisodeResponse = API.GetEpisode(episodeId, requestLangCode);

                if (IsNotDefaultLanguage(requestLangCode))
                {
                    jsonEpisodeDefaultLangResponse = API.GetEpisode(episodeId, DefaultLanguageCode);
                }
            }
            catch (WebException ex)
            {
                LOGGER.LogWebException($"Error obtaining episode[{ episodeId}]:",ex);

                LastErrorMessage = ex.LoggableDetails();
                return false;
            }
            finally
            {
                SayNothing();
            }
            
            JObject jsonResponseData = (JObject)jsonEpisodeResponse["data"] ?? throw new SourceConsistencyException("No Data in Ep Response", TVDoc.ProviderType.TheTVDB);
            if (IsNotDefaultLanguage(requestLangCode))
            {
                JObject seriesDataDefaultLang = (JObject)jsonEpisodeDefaultLangResponse["data"];
                return UpdateEpisodeNow(seriesId, jsonResponseData, seriesDataDefaultLang, cachedSeriesInfo);
            }
            else
            {
                return UpdateEpisodeNow(seriesId, jsonResponseData, null, cachedSeriesInfo);
            }
        }

        private bool UpdateEpisodeNow(int seriesId, JToken jsonResponseData, JToken? seriesDataDefaultLang, CachedSeriesInfo si)
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
                    LOGGER.Error($"<TVDB ISSUE?>: problem with JSON received for episode {jsonResponseData} - {seriesDataDefaultLang}");
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

        private void AddPlaceholderSeries(int tvdb, int tvmaze,int tmdb)
        {
            Series[tvdb] = new CachedSeriesInfo(tvdb, tvmaze,tmdb) { Dirty = true };
        }

        private void AddPlaceholderSeries(int tvdb, int tvmaze, int tmdb, string customLanguageCode)
        {
            Series[tvdb] = new CachedSeriesInfo(tvdb, tvmaze,tmdb, customLanguageCode) { Dirty = true };
        }

        public override bool EnsureUpdated(SeriesSpecifier seriesd, bool bannersToo, bool showErrorMsgBox)
        {
            if (seriesd.Provider == TVDoc.ProviderType.TVmaze)
            {
                throw new SourceConsistencyException($"Asked to update {seriesd.Name} from TV Maze, but the Id is not for TV maze.", TVDoc.ProviderType.TVmaze);
            }

            int code = seriesd.TvdbSeriesId;

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

                ok = DownloadEpisodeNow(ee.Value.SeriesId, ee.Key, seriesd.CustomLanguageCode) && ok;
                ee.Value.Done = true;
            });

            foreach (ExtraEp episodetoRemove in removeEpisodeIds.Values)
            {
                Series[episodetoRemove.SeriesId].RemoveEpisode(episodetoRemove.EpisodeId);
            }

            removeEpisodeIds.Clear();

            forceReloadOn.TryRemove(code, out _);

            return ok;
        }

        public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type)
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
                                DownloadSeriesNow(textAsInt, false, false, false, TVSettings.Instance.PreferredLanguageCode, showErrorMsgBox);
                                break;
                            case MediaConfiguration.MediaType.movie:
                                DownloadMovieNow(textAsInt,  false, false, TVSettings.Instance.PreferredLanguageCode, showErrorMsgBox);
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
                jsonSearchResponse = VERS== ApiVersion.v4
                    ? API.SearchV4(text, TVSettings.Instance.PreferredLanguageCode,type) 
                    : type==MediaConfiguration.MediaType.tv
                        ? API.SearchTvShow(text, TVSettings.Instance.PreferredLanguageCode)
                        : null; //Can't search for Movies in the v3 API
            }
            catch (WebException ex)
            {
                if (ex.Response is null) //probably a timeout
                {
                    LOGGER.LogWebException($"Error obtaining results for search term '{text}':",ex);
                    LastErrorMessage = ex.LoggableDetails();
                    SayNothing();
                }
                else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    LOGGER.Info(
                        $"Could not find any search results for {text} in {TVSettings.Instance.PreferredLanguageCode}");
                }
                else
                {
                    LOGGER.LogWebException($"Error obtaining results for search term '{text}':",ex);
                    LastErrorMessage = ex.LoggableDetails();
                    SayNothing();
                }
            }

            if (InForeignLanguage() && VERS != ApiVersion.v4 && type== MediaConfiguration.MediaType.tv)
            {
                try
                {
                    jsonSearchDefaultLangResponse = API.SearchTvShow(text, DefaultLanguageCode);
                }
                catch (WebException ex)
                {
                    if (ex.Response is null) //probably a timeout
                    {
                        LOGGER.LogWebException($"Error obtaining results for search term '{text}' in {DefaultLanguageCode}:",ex);
                        LastErrorMessage = ex.LoggableDetails();
                        SayNothing();
                    }
                    else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        LOGGER.Info(
                            $"Could not find any search results for {text} in {DefaultLanguageCode}");
                    }
                    else
                    {
                        LOGGER.Error(
                            $"Error obtaining {ex.Response.ResponseUri} for search term '{text}' in {DefaultLanguageCode}: {ex.LoggableDetails()}");

                        LastErrorMessage = ex.LoggableDetails();
                        SayNothing();
                    }
                }
            }

            if (jsonSearchResponse != null)
            {
                ProcessSearchResult(jsonSearchResponse, GetLanguageId());
            }

            if (jsonSearchDefaultLangResponse != null)
            //we also want to search for search terms that match in default language
            {
                ProcessSearchResult(jsonSearchDefaultLangResponse, GetDefaultLanguageId());
            }
        }

        private void ProcessSearchResult([NotNull] JObject jsonResponse, int languageId)
        {
            JToken? jToken = jsonResponse["data"];
            if (jToken is null)
            {
                throw new SourceConsistencyException($"Could not get data element from {jsonResponse}", TVDoc.ProviderType.TheTVDB);
            }
            try
            {
                IEnumerable<CachedSeriesInfo> cachedSeriesInfos = (VERS == ApiVersion.v4)
                    ? GetEnumSeriesV4(jToken,languageId, true)
                    : jToken.Cast<JObject>().Select(seriesResponse => new CachedSeriesInfo(seriesResponse, languageId, true));

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

        private IEnumerable<CachedSeriesInfo> GetEnumSeriesV4(JToken jToken, int languageId, bool b)
        {
            JArray ja = (JArray) jToken;
            List<CachedSeriesInfo> ses = new List<CachedSeriesInfo>();

            foreach (JToken jt  in ja.Children())
            {
                JObject showJson = (JObject)jt;
                ses.Add(GenerateSeriesV4(showJson, languageId, b));
            }

            return ses;
        }

        private CachedSeriesInfo GenerateSeriesV4(JObject r, int langId, bool searchResult)
        {

            CachedSeriesInfo si = new CachedSeriesInfo
            {

                TvdbCode = (int)r["tvdb_id"],

                Slug = ((string)r["id"])?.Trim(),
                PosterUrl = (string)r["image_url"],
                Overview = (string)r["overview"],
                Network = (string)r["network"],
                LanguageId = langId,
                Status = (string)r["status"],
                IsSearchResultOnly = searchResult,
        };
            //si.Aliases = (r["aliases"] ?? throw new SourceConsistencyException($"Can't find aliases in Series JSON: {r}", TVDoc.ProviderType.TheTVDB)).Select(x => x.Value<string>()).ToList();

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

            lock (SERIES_LOCK)
            {
                foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series)
                {
                    bool found = libraryValues.Any(si => si.TvdbCode == kvp.Key);
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
                CachePersistor.SaveCache(Series,Movies, CacheFile, LatestUpdateTime.LastSuccessfulServerUpdateTimecode());
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
                                $"Can't find the cachedSeries to add the banner {b.BannerId} to (TheTVDB). {seriesId},{b.SeriesId}", TVDoc.ProviderType.TheTVDB);
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

        public CachedMovieInfo? GetMovieOrDownload(int tvdbId, bool showErrorMsgBox)
        {
            throw new NotImplementedException(); //TODO
        }
        private void DownloadMovieNow(int tvdbId, bool bannersToo, bool useCustomLangCode, string langCode, bool showErrorMsgBox)
        {
            throw new NotImplementedException(); //TODO
        }
    }
}
