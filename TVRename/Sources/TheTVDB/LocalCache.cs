//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
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

        private UpdateTimeTracker LatestUpdateTime;

        private bool showConnectionIssues;

        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile LocalCache? InternalInstance;
        private static readonly object SyncRoot = new();

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

        public TVDoc.ProviderType SourceProvider() => TVDoc.ProviderType.TheTVDB;

        public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TheTVDB;

        public void Setup(FileInfo? loadFrom, FileInfo cache, bool showIssues)
        {
            showConnectionIssues = showIssues;

            System.Diagnostics.Debug.Assert(cache != null);
            CacheFile = cache;

            LastErrorMessage = string.Empty;
            IsConnected = false;
            extraEpisodes = new ConcurrentDictionary<int, ExtraEp>();
            removeEpisodeIds = new ConcurrentDictionary<int, ExtraEp>();

            //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
            //If we have no prior install then the app has no shows and is by definition up-to-date
            LatestUpdateTime = new UpdateTimeTracker();

            LOGGER.Info($"Assumed we have updates until {LatestUpdateTime}");

            if (loadFrom is null)
            {
                LoadOk = true;
                return;
            }

            bool mOk = CachePersistor.LoadMovieCache(loadFrom, this);
            bool tvOk = CachePersistor.LoadTvCache(loadFrom, this);
            LoadOk = mOk && tvOk;
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
            catch (IOException e)
            {
                LOGGER.Warn(CurrentDLTask + " : " + e.LoggableDetails() + " : " + url);
                LastErrorMessage = CurrentDLTask + " : " + e.LoggableDetails();
                return null;
            }
        }

        public CachedSeriesInfo? GetSeriesAndDownload(ISeriesSpecifier id, bool showErrorMsgBox) => HasSeries(id.TvdbId)
            ? Series[id.TvdbId]
            : DownloadSeriesNow(id, false, false, new Locale(TVSettings.Instance.PreferredTVDBLanguage),
                showErrorMsgBox);

        public void UpdatesDoneOk()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            LatestUpdateTime.RecordSuccessfulUpdate();
        }

        public CachedMovieInfo? GetMovie(PossibleNewMovie show, Locale preferredLocale, bool showErrorMsgBox) => this.GetMovie(show.RefinedHint, show.PossibleYear, preferredLocale, showErrorMsgBox, false);

        internal IEnumerable<CachedSeriesInfo> ServerTvAccuracyCheck()
        {
            TvdbAccuracyCheck check = new(this);

            Say($"TVDB Accuracy Check (TV) running for {FullShows().Count} shows.");

            Parallel.ForEach(FullShows(), si => {
                Thread.CurrentThread.Name ??= $"TVDB Consistency Check: {si.Name}"; // Can only set it once
                check.ServerAccuracyCheck(si);
            });

            foreach (string issue in check.Issues)
            {
                LOGGER.Warn(issue);
            }

            SayNothing();
            return check.ShowsToUpdate;
        }
        internal IEnumerable<CachedMovieInfo> ServerMovieAccuracyCheck()
        {
            TvdbAccuracyCheck check = new(this);

            Say($"TVDB Accuracy Check (Movies) running {FullMovies().Count} shows.");

            Parallel.ForEach(FullMovies(), si => {
                Thread.CurrentThread.Name ??= $"TVDB Consistency Check: {si.Name}"; // Can only set it once
                check.ServerAccuracyCheck(si);
            });

            foreach (string issue in check.Issues)
            {
                LOGGER.Warn(issue);
            }

            SayNothing();
            return check.MoviesToUpdate;
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

        public void ForgetEverything()
        {
            lock (MOVIE_LOCK)
            {
                Movies.Clear();
            }
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
            catch (IOException ex)
            {
                HandleConnectionProblem(showErrorMsgBox, ex);
                return false;
            }
            finally
            {
                SayNothing();
            }
        }

        private void HandleConnectionProblem(bool showErrorMsgBox, Exception ex)
        {
            Say("Could not connect to TVDB");

            if (ex is WebException wex)
            {
                LOGGER.LogWebException("Error obtaining token from TVDB", wex);
                LastErrorMessage = wex.LoggableDetails();
            }
            else if (ex is IOException iex)
            {
                LOGGER.LogIoException("Error obtaining token from TVDB", iex);
                LastErrorMessage = iex.LoggableDetails();
            }
            else
            {
                LOGGER.Error(ex, "Error obtaining token from TVDB");
                LastErrorMessage = ex.Message;
            }

            if (showErrorMsgBox)
            {
                CannotConnectForm ccform = ex is WebException wexy
                    ? new("Error while obtaining token from TVDB", wexy.LoggableDetails(), TVDoc.ProviderType.TheTVDB)
                    : new("Error while obtaining token from TVDB", ex.Message, TVDoc.ProviderType.TheTVDB)
                    ;

                DialogResult ccresult = ccform.ShowDialog();
                if (ccresult == DialogResult.Abort)
                {
                    TVSettings.Instance.OfflineMode = true;
                    LastErrorMessage = string.Empty;
                }
            }
        }

        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, IEnumerable<ISeriesSpecifier> ss)
        {
            Say("Validating TheTVDB cache");
            IEnumerable<ISeriesSpecifier> seriesSpecifiers = ss.ToList();
            foreach (ISeriesSpecifier downloadShow in seriesSpecifiers.Where(downloadShow => downloadShow.Media==MediaConfiguration.MediaType.tv && !HasSeries(downloadShow.TvdbId)))
            {
                this.AddPlaceholderSeries(downloadShow);
            }
            foreach (ISeriesSpecifier downloadShow in seriesSpecifiers.Where(downloadShow => downloadShow.Media == MediaConfiguration.MediaType.movie && !HasMovie(downloadShow.TvdbId)))
            {
                this.AddPlaceholderMovie(downloadShow);
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

            this.MarkPlaceholdersDirty();

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

            List<JObject> updatesResponses = new();

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
                const int OFFSET =0;

                DateTime requestedTime = GetRequestedTime(updateFromEpochTime - OFFSET, numberofCallsMade);

                if (ApiVersion.v4 != TVSettings.Instance.TvdbVersion && (DateTime.UtcNow - requestedTime).TotalDays < 7)
                {
                    moreUpdates = false;
                }

                JObject jsonUpdateResponse = GetUpdatesJson(updateFromEpochTime - OFFSET, requestedTime, numberofCallsMade);
                if (jsonUpdateResponse is null)
                {
                    return false;
                }

                if (ApiVersion.v4 == TVSettings.Instance.TvdbVersion && !MoreFrom(jsonUpdateResponse))
                {
                    moreUpdates = false;
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
                if ( numberofCallsMade > MAX_NUMBER_OF_CALLS && TVSettings.Instance.TvdbVersion!=ApiVersion.v4 ||(TVSettings.Instance.TvdbVersion == ApiVersion.v4 && numberofCallsMade > 1000))
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
                    if (showConnectionIssues && Environment.UserInteractive)
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

            Parallel.ForEach(updatesResponses, o =>
            {
                Thread.CurrentThread.Name ??= "Recent Updates"; // Can only set it once
                ProcessUpdate(o, cts);
            });

            Say("Upgrading dirty locks");

            UpgradeDirtyLocks();

            SayNothing();

            return true;
        }

        private static bool MoreFrom(JObject jsonUpdateResponse)
        {
            JToken? x = jsonUpdateResponse["links"]["next"];
            return x is { Type: JTokenType.String };
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

                if (showConnectionIssues && Environment.UserInteractive)
                {
                    MessageBox.Show(msg, "Error obtaining updates from TVDB", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                return null;
            }
        }

        private JObject? GetUpdatesJson(long updateFromEpochTime, DateTime requestedTime, int page)
        {
            try
            {
                return API.GetShowUpdatesSince(updateFromEpochTime, TVSettings.Instance.PreferredTVDBLanguage.Abbreviation,page);
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

        private void ProcessUpdate(JObject jsonResponse, CancellationToken cts)
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

        private void ProcessSeriesUpdate(JObject seriesResponse)
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
                List<JObject> episodeResponses = GetEpisodes(id, selectedCachedSeriesInfo.ActualLocale ?? new Locale());
                if (episodeResponses is null)
                {
                    //we got nothing good back from TVDB
                    LOGGER.Warn(
                        $"Aborting updates for {selectedCachedSeriesInfo.Name} ({id}) as there was an issue obtaining episodes");

                    return;
                }

                if (selectedCachedSeriesInfo.ActualLocale?.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB) ?? true)
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
                    $"Episodes were not found for {ex.Media.TvdbId}:{selectedCachedSeriesInfo.Name} in languange {selectedCachedSeriesInfo.ActualLocale?.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName} or {TVSettings.Instance.PreferredTVDBLanguage.EnglishName}");
            }
            catch (KeyNotFoundException kex)
            {
                //We assume this is due to the update being for a recently removed show.
                LOGGER.Error(kex);
            }
        }

        private void ProcessSeriesUpdateV4(JObject seriesResponse)
        {
            int id = (int)seriesResponse["recordId"];
            long time = (long)seriesResponse["timeStamp"];
            string entityType = (string)seriesResponse["entityType"];
            //string method = (string)seriesResponse["method"];

            switch (entityType)
            {
                case "series":
                case "translatedseries":
                case "seriespeople":
                    {
                    CachedSeriesInfo? selectedCachedSeriesInfo = GetSeries(id);
                    if (selectedCachedSeriesInfo!=null)
                    {
                        ProcessUpdate(selectedCachedSeriesInfo, time, $"as it({id}) has been updated");
                    }
                    return;
                    }
                case "movies":
                case "translatedmovies":
                case "movie-genres":
                {
                    CachedMovieInfo? selectedMovieCachedData = GetMovie(id);
                    if (selectedMovieCachedData!=null)
                    {
                        ProcessUpdate(selectedMovieCachedData, time, $"as it({id}) has been updated");
                    }

                    return;
                    }
                case "episodes":
                case "translatedepisodes":
                    {
                        List<CachedSeriesInfo> matchingShows = Series.Values.Where(y => y.Episodes.Any(e => e.EpisodeId == id)).ToList();
                        if (!matchingShows.Any())
                        {
                            return;
                        }

                        foreach (CachedSeriesInfo? selectedCachedSeriesInfo in matchingShows)
                        {
                            ProcessUpdate(selectedCachedSeriesInfo, time, $"as episodes({id}) have been updated");
                        }
                        return;
                    }
                case "seasons":
                case "translatedseasons":
                    {
                        List<CachedSeriesInfo> matchingShows = Series.Values.Where(y => y.Seasons.Any(e => e.SeasonId == id)).ToList();
                        if (!matchingShows.Any())
                        {
                            return;
                        }
                        foreach (CachedSeriesInfo? selectedCachedSeriesInfo in matchingShows)
                        {
                            ProcessUpdate(selectedCachedSeriesInfo, time, $"as seasons({id}) have been updated");
                        }
                        return;
                    }
                case "artwork":
                case "artworktypes":
                case "people":
                case "characters":
                case "award-nominees":
                case "award_categories":
                case "companies":
                case "awards":
                case "company_types":
                case "movie_status":
                case "content_ratings":
                case "countries":
                case "entity_types":
                case "genres":
                case "languages":
                case "peopletypes":
                case "seasontypes":
                case "sourcetypes":
                case "translatedpeople":
                case "translatedcharacters":
                case "lists":
                case "translatedlists":
                case "translatedcompanies":
                case "tags":
                case "tag-options":
                case "award-categories":

                    return;

                default:
                    LOGGER.Error($"Found update record for '{entityType}' = {id}");
                    return;
            }
        }

        private static void ProcessUpdate(CachedMediaInfo selectedCachedSeriesInfo, long time, string message)
        {
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

            LOGGER.Info($"Updating {selectedCachedSeriesInfo.Name} {message}");
        }

        private void ProcessEpisodes(CachedSeriesInfo si, Dictionary<int, Tuple<JToken, JToken>> episodesResponses)
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
                removeEpisodeIds.TryAdd(episodeId, new ExtraEp(si.TvdbCode, episodeId, si.SeasonOrder));
            }
        }

        private (int newEps, int updatedEps) ProcessEpisode(long serverUpdateTime,
            KeyValuePair<int, Tuple<JToken, JToken>> episodeData, CachedSeriesInfo si,
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
                extraEpisodes.TryAdd(serverEpisodeId, new ExtraEp(si.TvdbCode, serverEpisodeId,si.SeasonOrder));
                newEpisodeCount++;
            }

            return (newEpisodeCount, updatedEpisodeCount);
        }

        private ICollection<int> GetOldEpisodeIds(int seriesId)
        {
            ICollection<int> oldEpisodeIds = new List<int>();
            foreach (Episode ep in GetSeries(seriesId)?.Episodes ?? new List<Episode>())
            {
                oldEpisodeIds.Add(ep.EpisodeId);
            }

            return oldEpisodeIds;
        }

        private static long GetUpdateTime(JObject jsonUpdateResponse)
        {
            try
            {
                string keyName = TVSettings.Instance.TvdbVersion == ApiVersion.v4 ? "timeStamp" : "lastUpdated";
                IEnumerable<long>? updateTimes = jsonUpdateResponse["data"]?.Select(a => (long)a[keyName]);
                long maxUpdateTime = updateTimes?.DefaultIfEmpty(0).Max() ?? 0;

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
            List<JObject> episodeResponses = new();

            int pageNumber = 1;
            bool morePages = true;
            Language languageToUse = locale.LanguageToUse(TVDoc.ProviderType.TheTVDB);

            while (morePages)
            {
                try
                {
                    JObject? jsonEpisodeResponse = API.GetSeriesEpisodes(id, languageToUse.Abbreviation, pageNumber);

                    try
                    {
                        JToken? jToken = jsonEpisodeResponse?["data"];

                        if (jToken is null)
                        {
                            throw new SourceConsistencyException($"Data element not found in {jsonEpisodeResponse}",
                                TVDoc.ProviderType.TheTVDB);
                        }

                        episodeResponses.Add(jsonEpisodeResponse);

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

        private CachedSeriesInfo? DownloadSeriesNow(ISeriesSpecifier deets, bool episodesToo, bool bannersToo,
            bool showErrorMsgBox) =>
            DownloadSeriesNow(deets, episodesToo, bannersToo, deets.TargetLocale, showErrorMsgBox);

        private CachedSeriesInfo? DownloadSeriesNow(ISeriesSpecifier code, bool episodesToo, bool bannersToo, Locale locale,
            bool showErrorMsgBox)
        {
            if (code.TvdbId == 0)
            {
                SayNothing();
                return null;
            }

            bool forceReload = DoWeForceReloadFor(code.TvdbId);

            Say(GenerateMessage(code.TvdbId, episodesToo, bannersToo));

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
            this.AddSeriesToCache(si);
            lock (SERIES_LOCK)
            {
                si = GetSeries(code.TvdbId);
            }

            //Now deal with obtaining any episodes for the cachedSeries (we then group them into seasons)
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /series/{id}/episodes/summary to get the total

            if (episodesToo || forceReload)
            {
                if (si != null)
                {
                    ProcessedSeason.SeasonType st = code is ShowConfiguration sc
                        ? sc.Order
                        : ProcessedSeason.SeasonType.aired;
                    ReloadEpisodes(code, locale, si,st);
                }
            }

            if (bannersToo && forceReload)
            {
                if (si != null)
                {
                    if (TVSettings.Instance.TvdbVersion != ApiVersion.v4)
                    {
                        DownloadSeriesBanners(code.TvdbId, si, locale);
                    }
                    else
                    {
                        //No need to do anything, V4 images are in the main http request
                    }
                }
            }

            //V4 has actors in the main request
            if (TVSettings.Instance.TvdbVersion != ApiVersion.v4)
            {
                DownloadSeriesActors(code.TvdbId);
            }

            HaveReloaded(code.TvdbId);

            Series.TryGetValue(code.TvdbId, out CachedSeriesInfo returnValue);
            SayNothing();
            return returnValue;
        }

        internal CachedSeriesInfo DownloadSeriesInfo(ISeriesSpecifier code, Locale locale, bool showErrorMsgBox)
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
                Language? languageCodeToUse;
                ProcessedSeason.SeasonType st = code is ShowConfiguration showConfig
                    ? showConfig.Order
                    : ProcessedSeason.SeasonType.aired;

                (si, languageCodeToUse) = GenerateSeriesInfoV4(DownloadSeriesJson(code, locale), locale, st);
                if (languageCodeToUse!=null)
                {
                    AddTranslations(si, DownloadSeriesTranslationsJsonV4(code, new Locale(languageCodeToUse)));
                }
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
            si.Name = downloadSeriesTranslationsJsonV4["data"]?["name"]?.ToString() ?? si.Name;
            si.Overview = downloadSeriesTranslationsJsonV4["data"]?["overview"]?.ToString() ?? si.Overview;
            //Set a language code on the SI?? si.lan ==downloadSeriesTranslationsJsonV4["data"]["language"].ToString();
            IEnumerable<string>? aliases = downloadSeriesTranslationsJsonV4["data"]["aliases"]?.Select(x => x.ToString());
            if (aliases == null)
            {
                return;
            }

            foreach (string alias in aliases)
            {
                si.AddAlias(alias);
            }
        }

        private static void AddTranslations(CachedMovieInfo si, JObject downloadSeriesTranslationsJsonV4)
        {
            si.Name = downloadSeriesTranslationsJsonV4["data"]?["name"]?.ToString() ?? si.Name;
            si.Overview = downloadSeriesTranslationsJsonV4["data"]?["overview"]?.ToString() ?? si.Overview;

            IEnumerable<string> aliases = downloadSeriesTranslationsJsonV4["data"]["aliases"]?.Select(x => x.ToString());
            if (aliases == null)
            {
                return;
            }

            foreach (string alias in aliases)
            {
                si.AddAlias(alias);
            }
        }

        private CachedMovieInfo DownloadMovieInfo(ISeriesSpecifier code, Locale locale, bool showErrorMsgBox)
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
                Language? languageCode;
                (si, languageCode) = GenerateMovieInfoV4(DownloadMovieJson(code, locale), locale);
                if (languageCode!=null)
                {
                    AddTranslations(si, DownloadMovieTranslationsJsonV4(code, new Locale(languageCode)));
                }
            }
            else
            {
                JObject jsonResponse = DownloadMovieJson(code, locale);

                if (jsonResponse is null)
                {
                    LOGGER.Error($"Error obtaining movie information - no response available {code}");
                    SayNothing();
                    throw new SourceConnectivityException();
                }

                JObject seriesData = (JObject)jsonResponse["data"];
                if (seriesData is null)
                {
                    throw new SourceConsistencyException($"Data element not found in {jsonResponse}", TVDoc.ProviderType.TheTVDB);
                }

                si =  GenerateCachedMovieInfo(seriesData, locale);
            }

            if (si is null)
            {
                LOGGER.Error($"Error obtaining movie {code} - cound not generate a cachedMovie from the responses");
                SayNothing();
                throw new SourceConnectivityException();
            }

            return si;
        }

        private JObject DownloadMovieJson(ISeriesSpecifier code, Locale locale)
        {
            try
            {
                return TVSettings.Instance.TvdbVersion == ApiVersion.v4
                    ? API.GetMovieV4(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
                    : API.GetMovie(code.TvdbId, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation) ?? throw new SourceConnectivityException();
            }
            catch (IOException ioex)
            {
                LOGGER.LogIoException(
                    $"Error obtaining movie {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ioex);

                SayNothing();
                LastErrorMessage = ioex.LoggableDetails();
                throw new SourceConnectivityException();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse
                    { StatusCode: HttpStatusCode.NotFound })
                {
                    LOGGER.Warn($"Movie with Id {code} is no longer available from TVDB (got a 404).");
                    SayNothing();

                    if (API.TvdbIsUp() && !CanFindEpisodesFor(code, locale)
                    ) //todo - Check whether this is right? will be no episodes for a movie
                    {
                        LastErrorMessage = ex.LoggableDetails();
                        string msg = $"Movie with TVDB Id {code} is no longer found on TVDB. Please Update";
                        throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                            TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie);
                    }
                }

                LOGGER.LogWebException(
                    $"Error obtaining movie {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                throw new SourceConnectivityException();
            }
        }

        private CachedMovieInfo GenerateCachedMovieInfo(JObject r, Locale locale)
        {
            CachedMovieInfo si = new(locale, TVDoc.ProviderType.TheTVDB)
            {
                FirstAired = GetReleaseDate(r, locale.RegionToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation) ??
                             GetReleaseDate(r, "global"),
                TvdbCode = (int)r["id"],
                Imdb = GetExternalId(r, "IMDB"),
                Runtime = ((string)r["runtime"])?.Trim(),
                Name = GetTranslation(locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), "name", r) ?? string.Empty,
                TagLine = GetTranslation(locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), "tagline", r),
                Overview = GetTranslation(locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), "overview", r),
                TrailerUrl = r["trailers"]?.FirstOrDefault()?["url"]?.ToString(),
                Genres = r["genres"]?.Select(x => x["name"].ToString()).ToSafeList() ?? new SafeList<string>(),
                IsSearchResultOnly = false,
                Dirty = false,
                PosterUrl = API.GetImageURL(GetArtwork(r, "Poster")),
                FanartUrl = API.GetImageURL(GetArtwork(r, "Background")),
                OfficialUrl = GetExternalId(r, "Official Website"),
                FacebookId = GetExternalId(r, "Facebook"),
                InstagramId = GetExternalId(r, "Instagram"),
                TwitterId = GetExternalId(r, "Twitter"),
            };
            AddCastCrew(r, si);
            AddMovieImagesV4(r, si);

            return si;
        }

        private void AddMovieImagesV4(JObject r, CachedMovieInfo si)
        {
            if (r["data"]?["artworks"] is not null)
            {
                foreach (JToken? imageJson in r["data"]["artworks"])
                {
                    int imageCodeType = (int) imageJson["type"];
                    if (imageCodeType == 13) //Person Snaphot
                    {
                        continue;
                    }
                    MovieImage mi = new()
                    {
                        Id = (int)imageJson["id"],
                        ImageUrl = API.GetImageURL((string)imageJson["image"]),
                        ThumbnailUrl = API.GetImageURL((string)imageJson["thumbnail"]),
                        LanguageCode = (string)imageJson["language"],
                        Rating = (int)imageJson["score"],
                        MovieId = si.TvdbCode,
                        ImageStyle = MapBannerTVDBV4APICode(imageCodeType),
                        MovieSource = TVDoc.ProviderType.TheTVDB,
                        RatingCount = 1
                    };

                    si.AddOrUpdateImage(mi);
                }
            }
        }

        private void AddShowImagesV4(JObject r, CachedSeriesInfo si)
        {
            //JObject x = API.ImageTypesV4();
            if (r["data"]?["artworks"] is not null)
            {
                foreach (JToken? imageJson in r["data"]["artworks"])
                {
                    ShowImage mi = ConvertJsonToImage(imageJson, si);

                    si.AddOrUpdateImage(mi);
                }
            }
        }

        private MediaImage.ImageType MapBannerTVDBV4APICode(int v)
        {
            // from call to API.ImageTypesV4()
            return v switch
            {
                14 => MediaImage.ImageType.poster,
                2 => MediaImage.ImageType.poster,
                7 => MediaImage.ImageType.poster,

                1 => MediaImage.ImageType.wideBanner,
                6 => MediaImage.ImageType.wideBanner,
                16 => MediaImage.ImageType.wideBanner,

                15 => MediaImage.ImageType.background,
                3 => MediaImage.ImageType.background,
                8 => MediaImage.ImageType.background,

                5 => MediaImage.ImageType.icon,
                10 => MediaImage.ImageType.icon,
                18 => MediaImage.ImageType.icon,
                19 => MediaImage.ImageType.icon,

                11 => MediaImage.ImageType.thumbs,
                12 => MediaImage.ImageType.thumbs,

                25=>  MediaImage.ImageType.clearLogo,
                24 => MediaImage.ImageType.clearArt,

                _ => MediaImage.ImageType.poster
            };
        }

        private MediaImage.ImageSubject MapSubjectTVDBV4APICode(int v)
        {
            // from call to API.ImageTypesV4()
            return v switch
            {
                1 => MediaImage.ImageSubject.show,
                2 => MediaImage.ImageSubject.show,
                3 => MediaImage.ImageSubject.show,
                5 => MediaImage.ImageSubject.show,
                20 => MediaImage.ImageSubject.show,

                6 => MediaImage.ImageSubject.season,
                7 => MediaImage.ImageSubject.season,
                8 => MediaImage.ImageSubject.season,
                10 => MediaImage.ImageSubject.season,

                11 => MediaImage.ImageSubject.episode,
                12 => MediaImage.ImageSubject.episode,

                14 => MediaImage.ImageSubject.movie,
                15 => MediaImage.ImageSubject.movie,
                16 => MediaImage.ImageSubject.movie,
                18 => MediaImage.ImageSubject.movie,
                21 => MediaImage.ImageSubject.movie,
                24 => MediaImage.ImageSubject.movie,
                25 => MediaImage.ImageSubject.movie,

                _ => MediaImage.ImageSubject.show
            };
        }

        private static void AddCastCrew(JObject r, CachedMovieInfo si)
        {
            if (r["people"]?["actors"] is not null)
            {
                foreach (JToken? actorJson in r["people"]["actors"])
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["name"]?.ToString() ?? string.Empty;
                    string image = API.GetImageURL(actorJson["people_image"]?.ToObject<string?>());
                    string role = actorJson["role"]?.ToString();
                    si.AddActor(new Actor(id, image, name, role, 0, id));
                }
            }

            si.ClearCrew();
            if (r["people"]?["directors"] is not null)
            {
                foreach (JToken? actorJson in r["people"]["directors"])
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["name"]?.ToString() ?? string.Empty;
                    string image = API.GetImageURL(actorJson["people_image"]?.ToObject<string?>());
                    string role = actorJson["role"]?.ToString();
                    si.AddCrew(
                        new Crew(id, image, name, role.HasValue() ? role : "Director", "Directing", string.Empty));
                }
            }

            if (r["people"]?["producers"] is not null)
            {
                foreach (JToken? actorJson in r["people"]["producers"])
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["name"]?.ToString() ?? string.Empty;
                    string image = API.GetImageURL(actorJson["people_image"]?.ToObject<string?>());
                    string role = actorJson["role"]?.ToString();
                    si.AddCrew(new Crew(id, image, name, role.HasValue() ? role : "Producer", "Production",
                        string.Empty));
                }
            }

            if (r["people"]?["writers"] is not null)
            {
                foreach (JToken? actorJson in r["people"]["writers"])
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["name"]?.ToString() ?? string.Empty;
                    string image = API.GetImageURL(actorJson["people_image"]?.ToObject<string?>());
                    string role = actorJson["role"]?.ToString();
                    si.AddCrew(new Crew(id, image, name, role.HasValue() ? role : "Writer", "Writing", string.Empty));
                }
            }
        }

        private string? GetArtwork(JObject json, string type)
        {
            return json["artworks"]?.FirstOrDefault(x => x["artwork_type"].ToString() == type)?["url"]?.ToString();
        }

        private string? GetArtworkV4(JObject json, int type)
        {
            return json["data"]["artworks"]
                ?.OrderByDescending(x=>x["score"].ToObject<int>())
                .FirstOrDefault(x => (int)x["type"] == type)
                ?["image"]
                ?.ToString();
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

        private string? GetExternalIdSearchResultV4(JObject json, string source)
        {
            return json["remote_ids"]?.FirstOrDefault(x => x["sourceName"].ToString() == source)?["id"]
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
            return ParseDate(date);
        }

        private DateTime? ParseDate(string? date)
        {
            try
            {
                if (!date.HasValue())
                {
                    return null;
                }

                return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private DateTime? GetReleaseDateV4(JObject json, string? region)
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

                return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
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

        private (CachedMovieInfo, Language?) GenerateMovieInfoV4(JObject r, Locale locale)
        {
            CachedMovieInfo si = GenerateCoreMovieInfoV4(r, locale);
            AddAliasesV4(r, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), si);
            AddCastAndCrew(r, si);
            AddMovieImagesV4(r, si);

            return (si, GetAppropriateLanguage(r["data"]["nameTranslations"], locale));
        }

        private CachedMovieInfo GenerateCoreMovieInfoV4(JObject r, Locale locale)
        {
            JToken dataNode = r["data"];
            JToken? collectionNode = GetCollectionNodeV4(dataNode);

            return new CachedMovieInfo(locale, TVDoc.ProviderType.TheTVDB)
            {
                FirstAired = GetReleaseDateV4(r, locale),
                TvdbCode = (int) dataNode["id"],
                Slug = ((string) dataNode["slug"])?.Trim(),
                Imdb = GetExternalIdV4(r, "IMDB"),
                Runtime = ((string) dataNode["runtime"])?.Trim(),
                Name = dataNode["name"]?.ToString() ?? string.Empty,
                TrailerUrl = GetTrailerUrl(r, locale),
                IsSearchResultOnly = false,
                PosterUrl = API.GetImageURL(GetArtworkV4(r, 14)),
                FanartUrl = API.GetImageURL(GetArtworkV4(r, 15)),
                OfficialUrl = GetExternalIdV4(r, "Official Website"),
                FacebookId = GetExternalIdV4(r, "Facebook"),
                InstagramId = GetExternalIdV4(r, "Instagram"),
                TwitterId = GetExternalIdV4(r, "Twitter"),
                Dirty = false,
                Network = dataNode["studios"]?.Select(x=>x["name"].ToString()).ToPsv(),
                ShowLanguage = dataNode["audioLanguages"]?.ToString(),
                Country = dataNode["originalCountry"]?.ToString(),
                ContentRating = GetContentRatingV4(r, locale),
                Status = dataNode["status"]?["name"]?.ToString(),
                SrvLastUpdated = ((DateTime) dataNode["lastUpdated"]).ToUnixTime(),
                Genres = GetGenresV4(r),

                CollectionId = collectionNode?["id"]?.ToString().ToInt(),
                CollectionName = collectionNode?["name"]?.ToString(),
            };
        }

        private static JToken? GetCollectionNodeV4(JToken r)
        {
            return r["lists"]?.FirstOrDefault(x => x["isOfficial"].ToObject<bool>());
        }

        private DateTime? GetReleaseDateV4(JObject r, Locale locale)
        {
            return GetReleaseDateV4(r, locale.RegionToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
                    ?? GetReleaseDateV4(r, "global")
                    ?? GetReleaseDateV4(r, (string?)null);
        }

        private string? GetContentRatingV4(JObject r, Locale locale)
        {
            return GetContentRatingV4(r, locale.RegionToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
                    ?? GetContentRatingV4(r, Regions.Instance.FallbackRegion.ThreeAbbreviation)
                    ?? GetContentRatingV4(r, "usa");
        }

        private static string? GetTrailerUrl(JObject r, Locale locale)
        {
            JToken trailersNode = r["data"]["trailers"];
            return
                TrailerUrl(trailersNode, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB)) ??
                TrailerUrl(trailersNode, TVSettings.Instance.PreferredTVDBLanguage)??
                TrailerUrl(trailersNode, Languages.Instance.FallbackLanguage) ??
                trailersNode?.FirstOrDefault()?["url"]?.ToString();
        }

        private static string? TrailerUrl(JToken? trailersNode, Language language)
        {
            return trailersNode
                ?.FirstOrDefault(x =>
                    x["language"]?.ToString() == language.ThreeAbbreviation)
                ?["url"]
                ?.ToString();
        }

        private static void AddAliasesV4(JObject r, Language lang, CachedMediaInfo si)
        {
            JToken aliasNode = r["data"]?["aliases"];
            if (aliasNode is null || !aliasNode.HasValues)
            {
                return;
            }

            List<JToken> languageNodes = aliasNode.Where(x => x["language"]?.ToString() == lang.ThreeAbbreviation).ToList();
            if (languageNodes.Any())
            {
                foreach (JToken? x in languageNodes)
                {
                    si.AddAlias(x["name"]?.ToString());
                }
                return;
            }

            languageNodes = aliasNode.Where(x => x["language"]?.ToString() == TVSettings.Instance.PreferredTVDBLanguage.ThreeAbbreviation).ToList();
            if (languageNodes.Any())
            {
                foreach (JToken? x in languageNodes)
                {
                    si.AddAlias(x["name"]?.ToString());
                }
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private static void AddCastAndCrew(JObject r, CachedMovieInfo si)
        {
            if (r["data"]?["characters"] is not null)
            {
                foreach (JToken? actorJson in r["data"]["characters"]?.Where(x => x["peopleType"]?.ToString() == "Actor"))
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["personName"]?.ToString() ?? string.Empty;
                    string image = API.GetImageURL(actorJson["image"]?.ToObject<string?>());
                    string role = actorJson["name"]?.ToString();
                    int? sort = actorJson["sort"]?.ToString().ToInt();
                    si.AddActor(new Actor(id, image, name, role, 0, sort));
                }

                foreach (JToken? actorJson in r["data"]["characters"]?.Where(x => x["peopleType"]?.ToString() != "Actor"))
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["personName"]?.ToString() ?? string.Empty;
                    string role = actorJson["peopleType"]?.ToString();
                    string sort = actorJson["sort"]?.ToString();
                    si.AddCrew(new Crew(id, string.Empty, name, role, string.Empty, sort));
                }
            }
        }

        private (CachedSeriesInfo, Language?) GenerateSeriesInfoV4(JObject r, Locale locale,
            ProcessedSeason.SeasonType seasonType)
        {
            CachedSeriesInfo si = GenerateCoreSeriesInfoV4(r, locale,seasonType);

            AddAliasesV4(r, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), si);
            AddCastAndCrewv4(r, si);
            AddShowImagesV4(r, si);
            AddSeasonsV4(r, seasonType, si);

            return (si, GetAppropriateLanguage(r["data"]["nameTranslations"], locale));
        }

        private CachedSeriesInfo GenerateCoreSeriesInfoV4(JObject r, Locale locale, ProcessedSeason.SeasonType st)
        {
            return new(locale, TVDoc.ProviderType.TheTVDB)
            {
                Name = GetName(r),
                AirsTime = GetAirsTimeV4(r),
                TvdbCode = (int) r["data"]["id"],
                IsSearchResultOnly = false,
                Dirty = false,
                Slug = ((string) r["data"]["slug"])?.Trim(),
                Genres = GetGenresV4(r),
                ShowLanguage = r["data"]["originalLanguage"]?.ToString(),
                Country = r["data"]["originalCountry"]?.ToString(),
                TrailerUrl = GetTrailerUrl(r, locale),
                SrvLastUpdated = ((DateTime) r["data"]["lastUpdated"]).ToUnixTime(),
                Status = (string) r["data"]["status"]?["name"],
                FirstAired = JsonHelper.ParseFirstAired((string) r["data"]["firstAired"]),
                AirsDay = GetAirsDayV4(r),
                Network = GetNetworks(r),
                Imdb = GetExternalIdV4(r, "IMDB"),
                OfficialUrl = GetExternalIdV4(r, "Official Website"),
                FacebookId = GetExternalIdV4(r, "Facebook"),
                InstagramId = GetExternalIdV4(r, "Instagram"),
                TwitterId = GetExternalIdV4(r, "Twitter"),
                TmdbCode = GetExternalIdV4(r, "TheMovieDB.com")?.ToInt() ?? -1,
                SeriesId = GetExternalIdV4(r, "TV.com"),
                PosterUrl = GetArtworkV4(r, 2),
                FanartUrl = GetArtworkV4(r, 3),
                SeasonOrderType = st,
            };
        }

        private static string GetName(JObject r) => System.Web.HttpUtility.HtmlDecode((string)r["data"]["name"]??string.Empty).Trim();

        // ReSharper disable once UnusedMember.Local
        private static string? GetNetwork(JObject r)
        {
            return r["data"]?["originalNetwork"]?["name"]?.ToString();
        }
        private static string? GetNetworks(JObject r)
        {
            JToken? jToken = r["data"]["companies"];
            return jToken
                ?.Where(x => x["companyType"]?["companyTypeName"]?.ToString().Equals("Network", StringComparison.OrdinalIgnoreCase) ?? false)
                .Select(x=>x["name"]?.ToString())
                .ToPsv();
        }
        private static SafeList<string> GetGenresV4(JObject r)
        {
            return r["data"]["genres"]?.Select(x => x["name"]?.ToString()).ToSafeList() ?? new SafeList<string>();
        }

        private static void AddCastAndCrewv4(JObject r, CachedSeriesInfo si)
        {
            if (r["data"]?["characters"] is not null)
            {
                foreach (JToken? actorJson in r["data"]["characters"]?.Where(x => x["peopleType"]?.ToString() == "Actor"))
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["personName"]?.ToString() ?? string.Empty;
                    string image = API.GetImageURL(actorJson["image"]?.ToObject<string?>());
                    string role = actorJson["name"]?.ToString();
                    int? sort = actorJson["sort"]?.ToString().ToInt();
                    si.AddActor(new Actor(id, image, name, role, 0, sort));
                }

                foreach (JToken? actorJson in r["data"]["characters"]?.Where(x => x["peopleType"]?.ToString() != "Actor"))
                {
                    int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                    string name = actorJson["personName"]?.ToString() ?? string.Empty;
                    string role = actorJson["peopleType"]?.ToString();
                    string sort = actorJson["sort"]?.ToString();
                    si.AddCrew(new Crew(id, string.Empty, name, role, string.Empty, sort));
                }
            }
        }

        private void AddSeasonsV4(JObject r, ProcessedSeason.SeasonType seasonType, CachedSeriesInfo si)
        {
            if (r["data"]?["seasons"] is not null)
            {
                foreach (JToken? seasonJson in r["data"]["seasons"])
                {
                    if (seasonType == getSeasonType(seasonJson))
                    {
                        int seasonId = (int)seasonJson["id"];
                        string seasonName = (string)seasonJson["name"] + " " + seasonJson["number"];
                        int seasonSeriesId = (int)seasonJson["seriesId"];
                        int seasonNumber = (int)seasonJson["number"];
                        string seasonDescription = string.Empty;
                        string imageUrl = (string)seasonJson["image"];
                        string url = string.Empty;

                        si.AddSeason(new Season(seasonId, seasonNumber
                            , seasonName, seasonDescription, url, imageUrl, seasonSeriesId));
                    }
                }
            }
        }

        private ProcessedSeason.SeasonType getSeasonType(JToken seasonJson)
        {
            return seasonJson["type"]["type"]?.ToString() switch
            {
                "official" => ProcessedSeason.SeasonType.aired,
                "dvd" => ProcessedSeason.SeasonType.dvd,
                "absolute" => ProcessedSeason.SeasonType.absolute,
                "alternate" => ProcessedSeason.SeasonType.alternate,
                _ => ProcessedSeason.SeasonType.absolute
            };
        }

        private Language? GetAppropriateLanguage(JToken? languageOptions, Locale preferredLocale)
        {
            if (languageOptions == null)
            {
                return Languages.Instance.FallbackLanguage;
            }

            if (((JArray)languageOptions).ContainsTyped(preferredLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB)
                .ThreeAbbreviation))
            {
                return preferredLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB);
            }

            if (((JArray)languageOptions).Count == 1)
            {
                return Languages.Instance.GetLanguageFromThreeCode(((JArray)languageOptions).Single().ToString()) ?? Languages.Instance.FallbackLanguage;
            }

            if (((JArray)languageOptions).ContainsTyped(TVSettings.Instance.PreferredTVDBLanguage.ThreeAbbreviation))
            {
                return TVSettings.Instance.PreferredTVDBLanguage;
            }

            if (((JArray)languageOptions).Count == 0)
            {
                return null;
            }

            if (((JArray)languageOptions).ContainsTyped(Languages.Instance.FallbackLanguage.ThreeAbbreviation))
            {
                return Languages.Instance.FallbackLanguage;
            }

            string? code = ((JArray)languageOptions).First?.ToString();

            if (code.HasValue())
            {
                return Languages.Instance.GetLanguageFromThreeCode(code!) ?? Languages.Instance.FallbackLanguage;
            }

            return Languages.Instance.FallbackLanguage;
        }

        private DateTime? GetAirsTimeV4(JObject r)
        {
            string airsTimeString = (string)r["data"]["airsTime"];
            return JsonHelper.ParseAirTime(airsTimeString);
        }

        private string? GetAirsDayV4(JObject r)
        {
            JToken? jTokens = r["data"]["airsDays"];
            IEnumerable<JToken> days = jTokens?.Children().Where(token => (bool)token.Values().First());
            return days?.Select(ConvertDayName).ToCsv();
        }

        private string ConvertDayName(JToken t)
        {
            JProperty p = (JProperty)t;
            return p.Name.UppercaseFirst();
        }

        private CachedSeriesInfo GenerateSeriesInfo(JObject jsonResponse, JObject jsonDefaultLangResponse,
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

                si = new CachedSeriesInfo(seriesData, seriesDataDefaultLang, locale, TVDoc.ProviderType.TheTVDB);
            }
            else
            {
                si = new CachedSeriesInfo(seriesData, locale, false, TVDoc.ProviderType.TheTVDB);
            }

            return si;
        }

        private (JObject jsonResponse, JObject jsonDefaultLangResponse) DownloadSeriesJsonWithTranslations(ISeriesSpecifier code,
            Locale locale)
        {
            JObject jsonDefaultLangResponse = new();

            JObject jsonResponse = DownloadSeriesJson(code, locale);

            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                jsonDefaultLangResponse = DownloadSeriesJson(code, new Locale(TVSettings.Instance.PreferredTVDBLanguage));
            }

            if (jsonResponse is null)
            {
                LOGGER.Error($"Error obtaining cachedSeries information - no response available {code}");
                SayNothing();
                throw new SourceConnectivityException();
            }

            return (jsonResponse, jsonDefaultLangResponse);
        }

        private JObject DownloadSeriesJson(ISeriesSpecifier code, Locale locale)
        {
            JObject jsonResponse;
            try
            {
                jsonResponse = TVSettings.Instance.TvdbVersion == ApiVersion.v4
                    ? API.GetSeriesV4(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
                    : API.GetSeries(code.TvdbId, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation);
            }
            catch (IOException ioex)
            {
                LOGGER.LogIoException(
                    $"Error obtaining cachedSeries {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ioex);

                SayNothing();
                LastErrorMessage = ioex.LoggableDetails();
                throw new SourceConnectivityException();
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
                            TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv);
                    }
                }

                LOGGER.LogWebException(
                    $"Error obtaining cachedSeries {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                throw new SourceConnectivityException();
            }

            return jsonResponse ?? throw new SourceConnectivityException();
        }

        private JObject DownloadMovieTranslationsJsonV4(ISeriesSpecifier code, Locale locale)
        {
            try
            {
                return API.GetMovieTranslationsV4(code,
                    locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation);
            }
            catch (IOException ioex)
            {
                LOGGER.LogIoException(
                    $"Error obtaining translations for {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ioex);

                SayNothing();
                LastErrorMessage = ioex.LoggableDetails();
                throw new SourceConnectivityException();
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

        private JObject DownloadSeriesTranslationsJsonV4(ISeriesSpecifier code, Locale locale)
        {
            try
            {
                return API.GetSeriesTranslationsV4(code,
                    locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation);
            }
            catch (IOException ioex)
            {
                LOGGER.LogIoException(
                    $"Error obtaining translations for {code.TvdbId} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ioex);

                SayNothing();
                LastErrorMessage = ioex.LoggableDetails();
                throw new SourceConnectivityException();
            }
            catch (WebException ex)
            {
                LOGGER.LogWebException(
                    $"Error obtaining translations for {code.TvdbId} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                    ex);

                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                throw new SourceConnectivityException();
            }
        }

        private static bool CanFindEpisodesFor(ISeriesSpecifier code, Locale locale)
        {
            try
            {
                API.GetSeriesEpisodes(code.TvdbId, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation);
            }
            catch (IOException)
            {
                return true;
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

                JObject jsonActorsResponse = API.GetSeriesActors(code) ?? throw new SourceConnectivityException();

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
                    string actorImage = API.GetImageURL((string)jsonActor["image"]);
                    string actorName = (string)jsonActor["name"] ??
                                       throw new SourceConsistencyException("No Actor", TVDoc.ProviderType.TheTVDB);

                    string actorRole = (string)jsonActor["role"];
                    int actorSeriesId = (int)jsonActor["seriesId"];
                    int actorSortOrder = (int)jsonActor["sortOrder"];

                    si.AddActor(new Actor(actorId, actorImage, actorName, actorRole, actorSeriesId,
                        actorSortOrder));
                }
            }
            catch (IOException ex)
            {
                LOGGER.LogIoException($"Unble to obtain actors for {Series[code].Name}", ex);
                LastErrorMessage = ex.LoggableDetails();
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

        private void DownloadSeriesBanners(int code, CachedSeriesInfo si, Locale locale)
        {
            (List<JObject> bannerDefaultLangResponses, List<JObject> bannerResponses) = DownloadBanners(code, locale);

            List<int> latestBannerIds = new();

            ProcessBannerResponses(code, si, locale, bannerResponses, latestBannerIds);
            ProcessBannerResponses(code, si, new Locale(TVSettings.Instance.PreferredTVDBLanguage),
                bannerDefaultLangResponses, latestBannerIds);

            si.UpdateBanners(latestBannerIds);
        }

        private void ProcessBannerResponses(int code, CachedSeriesInfo si, Locale locale,
            IEnumerable<JObject> bannerResponses,
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
                    foreach (JObject bannerData in jToken.Cast<JObject>())
                    {
                        ShowImage s = CreateShowImage(si.TvdbCode, bannerData);

                        lock (SERIES_LOCK)
                        {
                            si.AddOrUpdateImage(s);
                            latestBannerIds.Add(s.Id);
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

        private ShowImage CreateShowImage(int tvdbId, JObject bannerData)
        {
            double.TryParse((string)bannerData["ratingsInfo"]?["average"], NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out double rating);
            int.TryParse((string)bannerData["subKey"], out int seasonId);
            // {
            //  "fileName": "string",
            //  "id": 0,
            //  "keyType": "string",
            //  "languageId": 0,
            //  "ratingsInfo": {
            //      "average": 0,
            //      "count": 0
            //      },
            //  "resolution": "string",
            //  "subKey": "string",         //May Contain Season Number
            //  "thumbnail": "string"
            //  }
            return new ShowImage
            {
                SeriesSource = TVDoc.ProviderType.TheTVDB,
                SeriesId = tvdbId,
                ImageUrl = API.GetImageURL((string)bannerData["fileName"]),
                Id = (int)bannerData["id"],
                ImageStyle = MapBannerType((string)bannerData["keyType"]),
                Subject = MapTypeToSubject((string)bannerData["keyType"]),
                LanguageCode = Languages.Instance.GetLanguageFromCode((string)bannerData["language"])?.Abbreviation,
                SeasonNumber = seasonId,
                RatingCount = (int)bannerData["ratingsInfo"]?["count"],
                Rating = rating,
                Resolution = (string)bannerData["resolution"],
                ThumbnailUrl = API.GetImageURL((string)bannerData["thumbnail"])
            };
        }

        private MediaImage.ImageType MapBannerType(string? s)
        {
            return s switch
            {
                "fanart" => MediaImage.ImageType.background,
                "season" => MediaImage.ImageType.poster,
                "series" => MediaImage.ImageType.wideBanner,
                "seasonwide" => MediaImage.ImageType.wideBanner,
                "poster" => MediaImage.ImageType.poster,
                _ => MediaImage.ImageType.poster
            };
        }
        private MediaImage.ImageSubject MapTypeToSubject(string? s)
        {
            return s switch
            {
                "fanart" => MediaImage.ImageSubject.show,
                "season" => MediaImage.ImageSubject.season,
                "series" => MediaImage.ImageSubject.show,
                "seasonwide" => MediaImage.ImageSubject.season,
                "poster" => MediaImage.ImageSubject.show,
                _ => MediaImage.ImageSubject.show
            };
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

        public void ReloadEpisodesV4(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, ProcessedSeason.SeasonType order)
        {
            Parallel.ForEach(si.Seasons, s =>
            {
                Thread.CurrentThread.Name ??= $"Download Season {s.SeasonNumber} for {si.Name}"; // Can only set it once
                try
                {
                    JObject seasonInfo = API.GetSeasonEpisodesV4(si, s.SeasonId,
                        locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation);

                    JToken episodeData = seasonInfo["data"]?["episodes"];

                    if (episodeData != null)
                    {
                        Parallel.ForEach(episodeData, x =>
                        {
                            int? epNumber = x["number"]?.ToObject<int>();
                            Thread.CurrentThread.Name ??=
                                $"Creating S{s.SeasonNumber}E{epNumber} Episode for {si.Name}"; // Can only set it once

                            GenerateAddEpisodeV4(code, locale, si, x, order);
                        });
                    }

                    JToken imageData = seasonInfo["data"]?["artwork"];
                    if (imageData != null)
                    {
                        foreach (ShowImage newImage in imageData.Select(im => ConvertJsonToImage(im, si)))
                        {
                            newImage.SeasonId = s.SeasonId;
                            newImage.SeasonNumber = s.SeasonNumber;
                            si.AddOrUpdateImage(newImage);
                        }
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
                catch (MediaNotFoundException mnfe)
                {
                    LOGGER.Error($"Season Issue: {mnfe.Message}");
                }
            });
        }

        private ShowImage ConvertJsonToImage(JToken imageJson,CachedSeriesInfo si)
        {
            int imageCodeType = (int)imageJson["type"];

            return new ShowImage
            {
                Id = (int)imageJson["id"],
                ImageUrl = API.GetImageURL((string)imageJson["image"]),
                ThumbnailUrl = API.GetImageURL((string)imageJson["thumbnail"]),
                LanguageCode = (string)imageJson["language"],
                Rating = (int)imageJson["score"],
                SeriesId = si.TvdbCode,
                ImageStyle = MapBannerTVDBV4APICode(imageCodeType),
                Subject = MapSubjectTVDBV4APICode(imageCodeType),
                SeriesSource = TVDoc.ProviderType.TheTVDB,
                RatingCount = 1
            };
        }

        private void GenerateAddEpisodeV4(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, JToken x, ProcessedSeason.SeasonType order)
        {
            try
            {
                (Episode newEp, Language? bestLanguage) = GenerateCoreEpisodeV4(x, code.TvdbId, si, locale,order);
                if (bestLanguage !=null)
                {
                    AddTranslations(newEp,
                        API.GetEpisodeTranslationsV4(code  ,newEp.EpisodeId, bestLanguage.ThreeAbbreviation));
                }

                si.AddEpisode(newEp);
            }
            catch (MediaNotFoundException mnfe)
            {
                LOGGER.Error(mnfe, "Episode (+ Translations) claimed to exist, but got a 404 when searching for them. Ignoring Episode, but might be worth a full refresh of the show and contacting TVDB if it does not get resolved");
            }
            catch (SourceConnectivityException sce1)
            {
                LOGGER.Error(sce1);
            }
            catch (SourceConsistencyException sce1)
            {
                LOGGER.Error(sce1);
            }
        }

        private void AddTranslations(Episode newEp, JObject downloadSeriesTranslationsJsonV4)
        {
            string? transName = downloadSeriesTranslationsJsonV4["data"]["name"]?.ToString();
            newEp.Name = Translate(newEp.Name,transName);
            string? transOverview = downloadSeriesTranslationsJsonV4["data"]["overview"]?.ToString();
            newEp.Overview = Translate(newEp.Overview,transOverview);
            //Set a language code on the SI?? si.lan ==downloadSeriesTranslationsJsonV4["data"]["language"].ToString();
        }

        private static string Translate(string? originalName, string? transName)
        {
            //https://github.com/thetvdb/v4-api/issues/30

            if (transName.HasValue() && transName != "TBA")
            {
                return transName!;
            }

            if (transName.HasValue() && transName == "TBA" && originalName.HasValue() && originalName != "TBA")
            {
                ///issue
            }

            if (originalName.HasValue() && originalName != "TBA")
            {
                return originalName!;
            }

            return transName ?? originalName ?? string.Empty;
        }

        private (Episode, Language?) GenerateCoreEpisodeV4(JToken episodeJson, int code, CachedSeriesInfo si, Locale locale, ProcessedSeason.SeasonType order)
        {
            Episode x = new(code, si)
            {
                EpisodeId = episodeJson["id"].ToObject<int>(),
                SeriesId = episodeJson["seriesId"].ToObject<int?>() ?? -1,
                Name = episodeJson["name"].ToObject<string>() ?? string.Empty,
                FirstAired = GetEpisodeAiredDate(episodeJson),
                Runtime = episodeJson["runtime"].ToObject<string>(),
                Filename = episodeJson["image"].ToObject<string>(),
                SrvLastUpdated = GetUpdateTicks(episodeJson["lastUpdated"].ToObject<string>())
            };

            if (order == ProcessedSeason.SeasonType.dvd)
            {
                x.ReadDvdSeasonNum = episodeJson["seasonNumber"].ToObject<int?>()??0;
                x.DvdEpNum = episodeJson["number"].ToObject<int?>()??0;
            }
            else
            {
                x.AiredSeasonNumber = episodeJson["seasonNumber"].ToObject<int?>()??0;
                x.AiredEpNum = episodeJson["number"].ToObject<int?>()??0;
            }

            return (x, GetAppropriateLanguage(episodeJson["nameTranslations"], locale));
        }

        private long GetUpdateTicks(string? lastUpdateString)
        {
            const long DEFLT = 0; //equates to  1970/1/1

            try
            {
                if (!lastUpdateString.HasValue())
                {
                    return DEFLT;
                }
                //"lastUpdated": "2021-06-11 12:42:28",
                return DateTime.ParseExact(lastUpdateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToUnixTime();
            }
            catch
            {
                LOGGER.Error($"Failed to parse Epsiode update date {lastUpdateString}");
                return DEFLT;
            }
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

                return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private void ReloadEpisodes(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, ProcessedSeason.SeasonType order)
        {
            if (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
            {
                ReloadEpisodesV4(code, locale, si,order);
                return;
            }

            List<JObject> episodePrefLangResponses = GetEpisodes(code.TvdbId, locale);
            List<JObject> episodeDefaultLangResponses = null;
            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                episodeDefaultLangResponses = GetEpisodes(code.TvdbId, new Locale(TVSettings.Instance.PreferredTVDBLanguage));
            }

            Dictionary<int, Tuple<JToken, JToken>> episodeResponses =
                MergeEpisodeResponses(episodePrefLangResponses, episodeDefaultLangResponses);

            Parallel.ForEach(episodeResponses, episodeData =>
            {
                int episodeId = episodeData.Key;
                Thread.CurrentThread.Name ??= $"Episode {episodeId} for {si.Name}"; // Can only set it once
                JToken prefLangEpisode = episodeData.Value.Item1;
                JToken defltLangEpisode = episodeData.Value.Item2;
                try
                {
                    UpdateEpisodeNowv3(code.TvdbId, prefLangEpisode, defltLangEpisode, si);
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

        private static Dictionary<int, Tuple<JToken, JToken>> MergeEpisodeResponses(
            List<JObject>? episodeResponses, List<JObject>? episodeDefaultLangResponses)
        {
            Dictionary<int, Tuple<JToken, JToken>> episodeIds = new();

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

        private bool DownloadEpisodeNow(int seriesId, int episodeId, Locale locale, ProcessedSeason.SeasonType order)
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
            string eptxt = EpisodeDescription(order, episodeId, ep);

            CachedSeriesInfo cachedSeriesInfo = Series[seriesId];
            Say($"{cachedSeriesInfo.Name} ({eptxt}) in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");

            JObject jsonEpisodeResponse;
            JObject jsonEpisodeDefaultLangResponse = new();

            try
            {
                jsonEpisodeResponse =
                    API.GetEpisode(episodeId, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation) ?? throw new SourceConnectivityException();

                if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
                {
                    jsonEpisodeDefaultLangResponse = API.GetEpisode(episodeId,
                        TVSettings.Instance.PreferredTVDBLanguage.Abbreviation) ?? throw new SourceConnectivityException();
                }
            }
            catch (IOException ex)
            {
                LOGGER.LogIoException($"Error obtaining episode[{episodeId}]:", ex);

                LastErrorMessage = ex.LoggableDetails();
                return false;
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
            if (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
            {
                GenerateAddEpisodeV4(cachedSeriesInfo, locale,cachedSeriesInfo,jsonResponseData,order);
                return true;
            }
            if (locale.IsDifferentLanguageToDefaultFor(TVDoc.ProviderType.TheTVDB))
            {
                JObject seriesDataDefaultLang = (JObject)jsonEpisodeDefaultLangResponse["data"];
                return UpdateEpisodeNowv3(seriesId, jsonResponseData, seriesDataDefaultLang, cachedSeriesInfo);
            }
            else
            {
                return UpdateEpisodeNowv3(seriesId, jsonResponseData, null, cachedSeriesInfo);
            }
        }

        private bool UpdateEpisodeNowv3(int seriesId, JToken jsonResponseData, JToken? seriesDataDefaultLang,
            CachedSeriesInfo si)
        {
            if (!Series.ContainsKey(seriesId))
            {
                return false; // shouldn't happen
            }

            try
            {
                Episode e =
                    seriesDataDefaultLang != null
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

        private static string EpisodeDescription(ProcessedSeason.SeasonType order, int episodeId, Episode? ep)
        {
            if (ep == null)
            {
                return "New Episode Id = " + episodeId;
            }

            return order ==ProcessedSeason.SeasonType.dvd
                ? $"S{ep.DvdSeasonNumber:00}E{ep.DvdEpNum:00}"
                : $"S{ep.AiredSeasonNumber:00}E{ep.AiredEpNum:00}";
        }

        public override bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
        {
            if (s.Provider != TVDoc.ProviderType.TheTVDB)
            {
                throw new SourceConsistencyException(
                    $"Asked to update {s.Name} from The TVDB, but the Id is not for The TVDB.",
                    TVDoc.ProviderType.TheTVDB);
            }

            if (s.Media == MediaConfiguration.MediaType.movie)
            {
                return EnsureMovieUpdated(s, showErrorMsgBox);
            }

            return EnsureSeriesUpdated(s, bannersToo, showErrorMsgBox);
        }

        private bool EnsureMovieUpdated(ISeriesSpecifier id, bool showErrorMsgBox)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id.TvdbId) && !Movies[id.TvdbId].Dirty)
                {
                    return true;
                }
            }

            if (id.TvdbId == -1)
            {
                //We don't have access to search by other Ids
                throw new MediaNotFoundException(id, $"Please add movie {id.Name} to TVDB, or use another source for that show.", TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie);
            }

            Say($"Movie: {id.Name} from The TVDB");
            try
            {
                CachedMovieInfo downloadedSi = DownloadMovieNow(id, id.TargetLocale, showErrorMsgBox);

                if (downloadedSi is null)
                {
                    return false;
                }

                if (downloadedSi.TvdbCode != id.TvdbId && id.TvdbId == -1)
                {
                    lock (MOVIE_LOCK)
                    {
                        Movies.TryRemove(-1, out _);
                    }
                }

                this.AddMovieToCache(downloadedSi);

                return true;
            }
            catch (SourceConnectivityException conex)
            {
                LastErrorMessage = conex.Message;
                return false;
            }
            catch (SourceConsistencyException sce)
            {
                LOGGER.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return false;
            }
            finally
            {
                SayNothing();
            }
        }

        private bool EnsureSeriesUpdated(ISeriesSpecifier seriesd, bool bannersToo, bool showErrorMsgBox)
        {
            int code = seriesd.TvdbId;

            if (seriesd.TvdbId == -1)
            {
                //We don't have access to search by other Ids
                throw new MediaNotFoundException(seriesd, $"Please add movie {seriesd.Name} to TVDB, or use another source for that show.", TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv);
            }

            if (DoWeForceReloadFor(code) || Series[code].Episodes.Count == 0)
            {
                return DownloadSeriesNow(seriesd, true, bannersToo, showErrorMsgBox) != null; // the whole lot!
            }

            bool ok = true;

            bool seriesNeedsUpdating = Series[code].Dirty;

            if (seriesNeedsUpdating)
            {
                ok = DownloadSeriesNow(seriesd, false, bannersToo, showErrorMsgBox) != null;
            }

            ICollection<Episode> collection = Series[code]?.Episodes;
            if (collection != null)
            {
                foreach (Episode e in collection.Where(e => e.Dirty && e.EpisodeId > 0))
                {
                    extraEpisodes.TryAdd(e.EpisodeId, new ExtraEp(e.SeriesId, e.EpisodeId,seriesd.SeasonOrder));
                    extraEpisodes[e.EpisodeId].Done = false;
                }
            }

            Parallel.ForEach(extraEpisodes, ee =>
            {
                if (ee.Value.SeriesId != code || ee.Value.Done)
                {
                    return;
                }
                Thread.CurrentThread.Name ??= $"Download Episode {ee.Value.EpisodeId}"; // Can only set it once

                ok = DownloadEpisodeNow(ee.Value.SeriesId, ee.Key, seriesd.TargetLocale, ee.Value.Order) && ok;
                ee.Value.Done = true;
            });

            foreach (ExtraEp episodetoRemove in removeEpisodeIds.Values)
            {
                Series[episodetoRemove.SeriesId]?.RemoveEpisode(episodetoRemove.EpisodeId);
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
                    SearchSpecifier ss = new(textAsInt, locale, TVDoc.ProviderType.TheTVDB, type);
                    try
                    {
                        switch (type)
                        {
                            case MediaConfiguration.MediaType.tv:
                                DownloadSeriesNow(ss, false, false, locale, showErrorMsgBox);
                                break;

                            case MediaConfiguration.MediaType.movie:
                                DownloadMovieNow(ss, locale, showErrorMsgBox);
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
                catch (IOException ex)
                {
                    LOGGER.Error(ex,
                        $"Error obntaining search term '{text}' in {TVSettings.Instance.PreferredTVDBLanguage.EnglishName}: {ex.LoggableDetails()}");

                    LastErrorMessage = ex.LoggableDetails();
                    SayNothing();
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

        public override int PrimaryKey(ISeriesSpecifier ss) => ss.TvdbId;

        public override string CacheSourceName() => "TVDB";

        private void ProcessSearchResult(JObject jsonResponse, Locale locale)
        {
            JToken? jToken = jsonResponse["data"];
            if (jToken is null)
            {
                throw new SourceConsistencyException($"Could not get data element from {jsonResponse}",
                    TVDoc.ProviderType.TheTVDB);
            }

            try
            {
                IEnumerable<CachedSeriesInfo> cachedSeriesInfos = TVSettings.Instance.TvdbVersion == ApiVersion.v4
                    ? GetEnumSeriesV4(jToken, locale, true)
                    : jToken.Cast<JObject>()
                        .Select(seriesResponse => new CachedSeriesInfo(seriesResponse, locale, true, TVDoc.ProviderType.TheTVDB));

                foreach (CachedSeriesInfo si in cachedSeriesInfos)
                {
                    this.AddSeriesToCache(si);
                }

                IEnumerable<CachedMovieInfo> cachedMovieInfos = TVSettings.Instance.TvdbVersion == ApiVersion.v4
                    ? GetEnumMoviesV4(jToken, locale, true)
                    : new List<CachedMovieInfo>();

                foreach (CachedMovieInfo si in cachedMovieInfos)
                {
                    this.AddMovieToCache(si);
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
            List<CachedSeriesInfo> ses = new();

            foreach (JToken jt in ja.Children())
            {
                JObject showJson = (JObject)jt;
                if (jt["type"]?.ToString() == "movie")
                {
                    continue;
                }
                ses.Add(GenerateSeriesV4(showJson, locale, b,ProcessedSeason.SeasonType.aired)); //Assume Aired for Search Results
            }

            return ses;
        }

        private IEnumerable<CachedMovieInfo> GetEnumMoviesV4(JToken jToken, Locale locale, bool b)
        {
            JArray ja = (JArray)jToken;
            List<CachedMovieInfo> ses = new();

            foreach (JToken jt in ja.Children())
            {
                JObject showJson = (JObject)jt;
                if (jt["type"]?.ToString() == "movie")
                {
                    ses.Add(GenerateMovieV4(showJson, locale, b));
                }
            }

            return ses;
        }

        private CachedMovieInfo GenerateMovieV4(JObject r, Locale locale, bool searchResult)
        {
            CachedMovieInfo si = new(locale, TVDoc.ProviderType.TheTVDB)
            {
                TvdbCode = ParseIdFromObjectID(r["objectID"]),
                Slug = ((string)r["slug"])?.Trim(),
                PosterUrl = (string)r["image_url"],
                Status = (string)r["status"],
                IsSearchResultOnly = searchResult,
                ShowLanguage = (string)r["primary_language"],
                FirstAired = ParseDate((string)r["first_air_time"]) ?? GenerateFirstAiredDate(r),
                Name = FindTranslation(r, locale, "translations") ?? Decode(r, "name") ?? Decode(r, "extended_title") ?? string.Empty,
                Overview = FindTranslation(r, locale, "overviews") ?? Decode(r, "overview") ?? string.Empty,
                Country = (string)r["country"],
                Imdb = GetExternalIdSearchResultV4(r, "IMDB"),
                OfficialUrl = GetExternalIdSearchResultV4(r, "Official Website"),
                FacebookId = GetExternalIdSearchResultV4(r, "Facebook"),
                InstagramId = GetExternalIdSearchResultV4(r, "Instagram"),
                TwitterId = GetExternalIdSearchResultV4(r, "Twitter"),
                TmdbCode = GetExternalIdSearchResultV4(r, "TheMovieDB.com")?.ToInt() ?? -1,
                Genres = r["genres"]?.ToObject<string[]>()?.ToSafeList() ?? new(),
                Network = r["studios"]?.ToObject<string[]>()?.ToPsv() ?? string.Empty,
            };

            AddDirector(r, si);
            AddAliases(r, si);

            ValidateNewMedia(r, searchResult, si,"Movie");

            return si;
        }

        private static void ValidateNewMedia(JObject r, bool searchResult, CachedMediaInfo si, string type)
        {
            if (string.IsNullOrEmpty(si.Name))
            {
                LOGGER.Warn($"Issue with {type} {si}");
                LOGGER.Warn(r.ToString());
            }

            if (si.TvdbCode == 0)
            {
                LOGGER.Error($"Issue with {type} (No Id) {si}");
                LOGGER.Error(r.ToString());
            }

            if (si.SrvLastUpdated == 0 && !searchResult)
            {
                LOGGER.Warn($"Issue with {type} (update time is 0) {si}");
                LOGGER.Warn(r.ToString());
                si.SrvLastUpdated = 100;
            }
        }

        private static void AddAliases(JObject r, CachedMediaInfo si)
        {
            JToken? al = r["aliases"];
            if (al != null)
            {
                foreach (JValue a in ((JArray)al).Cast<JValue>())
                {
                    si.AddAlias(a.ToObject<string>());
                }
            }
        }

        private static void AddDirector(JObject r, CachedMovieInfo si)
        {
            string? directorName = (string)r["director"];
            if (directorName.HasValue())
            {
                si.AddCrew(new Crew(1, null, directorName!, "Director", "Directing", null));
            }
        }

        private int ParseIdFromObjectID(JToken jToken)
        {
            string baseValue = jToken.ToString();
            string[] splitString = baseValue.Split('-');
            if (splitString.Length == 2)
            {
                int? i = splitString[1].ToInt();
                if (i.HasValue)
                {
                    return i.Value;
                }
            }

            return 0;
        }

        private CachedSeriesInfo GenerateSeriesV4(JObject r, Locale locale, bool searchResult, ProcessedSeason.SeasonType st)
        {
            CachedSeriesInfo si = new(locale, TVDoc.ProviderType.TheTVDB)
            {
                TvdbCode = ParseIdFromObjectID(r["objectID"]),
                Slug = ((string)r["slug"])?.Trim(),
                PosterUrl = (string)r["image_url"],
                Network = (string)r["network"],
                Status = (string)r["status"],
                IsSearchResultOnly = searchResult,
                SeasonOrderType = st,
                ShowLanguage = (string)r["primary_language"],
                Country = (string)r["country"],
                FirstAired = ParseDate((string)r["first_air_time"]) ?? GenerateFirstAiredDate(r),
                Name = FindTranslation(r, locale, "translations") ?? Decode(r, "name") ?? Decode(r, "extended_title") ?? string.Empty,
                Overview = FindTranslation(r, locale, "overviews") ?? Decode(r, "overview") ?? string.Empty,

                Imdb = GetExternalIdSearchResultV4(r, "IMDB"),
                OfficialUrl = GetExternalIdSearchResultV4(r, "Official Website"),
                FacebookId = GetExternalIdSearchResultV4(r, "Facebook"),
                InstagramId = GetExternalIdSearchResultV4(r, "Instagram"),
                TwitterId = GetExternalIdSearchResultV4(r, "Twitter"),
                TmdbCode = GetExternalIdSearchResultV4(r, "TheMovieDB.com")?.ToInt() ?? -1,
                SeriesId = GetExternalIdSearchResultV4(r, "TV.com"),
            };
            AddAliases(r,si); //todo check whether this needs to be V4 version?

            ValidateNewMedia(r,searchResult,si,"Series");

            return si;
        }

        private string? FindTranslation(JObject r, Locale locale, string tag)
        {
            string langaugeCode = locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation;
            JToken languagesArray = r[tag];
            if (languagesArray == null ||!languagesArray.HasValues || languagesArray is not JObject)
            {
                return null;
            }

            JToken? languageValue = languagesArray[langaugeCode];
            if (languageValue == null || languageValue.Type != JTokenType.String)
            {
                return null;
            }

            return (string)languageValue;
        }

        private static string? Decode(JObject r, string tag)
        {
            string s = (string)r[tag];
            return s.HasValue() ? System.Web.HttpUtility.HtmlDecode(s)?.Trim() : null;
        }

        private DateTime? GenerateFirstAiredDate(JObject r)
        {
            int? year = r.Value<int?>("year");
            if (year>0)
            {
                try
                {
                    return new DateTime(year.Value, 1, 1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    LOGGER.Error($"Could not parse TVDB Series year from {r}");
                }
            }

            return null;
        }

        public void SaveCache()
        {
            lock (MOVIE_LOCK)
            {
                lock (SERIES_LOCK)
                {
                    CachePersistor.SaveCache(Series, Movies, CacheFile,
                        LatestUpdateTime.LastSuccessfulServerUpdateTimecode());
                }
            }
        }

        public void LatestUpdateTimeIs(string time)
        {
            LatestUpdateTime.Load(time);
            LOGGER.Info($"Loaded file with updates until {LatestUpdateTime.LastSuccessfulServerUpdateDateTime()}");
        }

        public CachedMovieInfo? GetMovieAndDownload(ISeriesSpecifier id, Locale locale, bool showErrorMsgBox) => HasMovie(id.TvdbId)
            ? CachedMovieData[id.TvdbId]
            : DownloadMovieNow(id, locale, showErrorMsgBox);

        internal CachedMovieInfo? DownloadMovieNow(ISeriesSpecifier tvdbId, Locale locale, bool showErrorMsgBox)
        {
            if (tvdbId.TvdbId == 0)
            {
                SayNothing();
                return null;
            }

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
            this.AddMovieToCache(si);

            lock (MOVIE_LOCK)
            {
                Movies.TryGetValue(tvdbId.TvdbId, out CachedMovieInfo returnValue);
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
            catch (IOException ex)
            {
                HandleConnectionProblem(showErrorMsgBox, ex);
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

        TVDoc.ProviderType iMovieSource.SourceProvider()
        {
            return Provider();
        }
    }
}
