//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Trending;
using TMDbLib.Objects.TvShows;
using TVRename.Forms;
using Cast = TMDbLib.Objects.Movies.Cast;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.TMDB
{
    // ReSharper disable once InconsistentNaming
    public class LocalCache : MediaCache, iMovieSource, iTVSource
    {
        private static readonly TMDbClient Client = new TMDbClient("2dcfd2d08f80439d7ef5210f217b80b4");

        private UpdateTimeTracker latestMovieUpdateTime;
        private UpdateTimeTracker latestTvUpdateTime;

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

        public void Setup(FileInfo? loadFrom, FileInfo cache, bool showIssues)
        {
            System.Diagnostics.Debug.Assert(cache != null);
            CacheFile = cache;

            //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
            //If we have no prior install then the app has no shows and is by definition up-to-date
            latestMovieUpdateTime = new UpdateTimeTracker();
            latestTvUpdateTime = new UpdateTimeTracker();

            LastErrorMessage = string.Empty;

            LoadOk = loadFrom is null || (CachePersistor.LoadMovieCache(loadFrom, this) && CachePersistor.LoadTvCache(loadFrom, this));
        }

        public bool Connect(bool showErrorMsgBox) => true;

        public void SaveCache()
        {
            lock (MOVIE_LOCK)
            {
                lock (SERIES_LOCK)
                {
                    CachePersistor.SaveCache(Series, Movies, CacheFile, latestMovieUpdateTime.LastSuccessfulServerUpdateTimecode());
                }
            }
        }

        public override Language PreferredLanguage() => TVSettings.Instance.TMDBLanguage;

        public override bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
        {
            if (s.Provider != TVDoc.ProviderType.TMDB)
            {
                throw new SourceConsistencyException($"Asked to update {s.Name} from TMDB, but the Id is not for TMDB.", TVDoc.ProviderType.TMDB);
            }

            if (s.Type == MediaConfiguration.MediaType.movie)
            {
                return EnsureMovieUpdated(s.TmdbId, s.TargetLocale, s.Name, showErrorMsgBox);
            }

            return EnsureSeriesUpdated(s, showErrorMsgBox);
        }

        private bool EnsureSeriesUpdated(ISeriesSpecifier s, bool showErrorMsgBox)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(s.TmdbId) && !Series[s.TmdbId].Dirty)
                {
                    return true;
                }
            }

            Say($"Series {s.Name} from TMDB");
            try
            {
                CachedSeriesInfo downloadedSi = DownloadSeriesNow(s, showErrorMsgBox);

                if (downloadedSi.TmdbCode != s.TmdbId && s.TmdbId == -1)
                {
                    lock (SERIES_LOCK)
                    {
                        Series.TryRemove(-1, out _);
                    }
                }

                if (downloadedSi.TmdbCode != s.TmdbId && s.TmdbId == 0)
                {
                    lock (SERIES_LOCK)
                    {
                        Series.TryRemove(0, out _);
                    }
                }

                lock (SERIES_LOCK)
                {
                    AddSeriesToCache(downloadedSi);
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

        private bool EnsureMovieUpdated(int id, Locale locale, string name, bool showErrorMsgBox)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id) && !Movies[id].Dirty)
                {
                    return true;
                }
            }

            Say($"{name} from TMDB");
            try
            {
                CachedMovieInfo downloadedSi = DownloadMovieNow(id, locale, showErrorMsgBox);

                if (downloadedSi.TmdbCode != id && id == -1)
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
            int id = si.TmdbCode;
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

        private void AddSeriesToCache([NotNull] CachedSeriesInfo si)
        {
            int id = si.TmdbCode;
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(id))
                {
                    Series[id].Merge(si);
                }
                else
                {
                    Series[id] = si;
                }
            }
        }

        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, IEnumerable<ISeriesSpecifier> ss)
        {
            Say("Validating TMDB cache");
            MarkPlaceHoldersDirty(ss);

            try
            {
                Say($"Updates list from TMDB since {latestMovieUpdateTime.LastSuccessfulServerUpdateDateTime()}");

                long updateFromEpochTime = latestMovieUpdateTime.LastSuccessfulServerUpdateTimecode();
                if (updateFromEpochTime == 0)
                {
                    MarkAllDirty();
                    latestMovieUpdateTime.RegisterServerUpdate(DateTime.Now.ToUnixTime());
                    return true;
                }

                List<int> updates = Client.GetChangesMovies(cts, latestMovieUpdateTime).Select(item => item.Id).Distinct().ToList();

                Say($"Processing {updates.Count} updates from TMDB. From between {latestMovieUpdateTime.LastSuccessfulServerUpdateDateTime()} and {latestMovieUpdateTime.ProposedServerUpdateDateTime()}");
                foreach (int id in updates)
                {
                    if (!cts.IsCancellationRequested)
                    {
                        if (HasMovie(id))
                        {
                            CachedMovieInfo? x = GetMovie(id);
                            if (!(x is null))
                            {
                                LOGGER.Info(
                                    $"Identified that Movie with TMDB Id {id} {x.Name} should be updated.");

                                x.Dirty = true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                lock (MOVIE_LOCK)
                {
                    Say($"Identified {Movies.Values.Count(info => info.Dirty && !info.IsSearchResultOnly)} TMDB Movies need updating");
                    LOGGER.Info(Movies.Values.Where(info => info.Dirty && !info.IsSearchResultOnly).Select(info => info.Name).ToCsv);
                }
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

        private void MarkPlaceHoldersDirty(IEnumerable<ISeriesSpecifier> ss)
        {
            foreach (ISeriesSpecifier downloadShow in ss)
            {
                if (downloadShow.Type == MediaConfiguration.MediaType.tv)
                {
                    if (!HasSeries(downloadShow.TmdbId))
                    {
                        AddPlaceholderSeries(downloadShow);
                    }
                    else
                    {
                        CachedSeriesInfo? Show = GetSeries(downloadShow.TmdbId);
                        Show?.UpgradeSearchResultToDirty();
                    }
                }
                else
                {
                    if (!HasMovie(downloadShow.TmdbId))
                    {
                        AddPlaceholderMovie(downloadShow);
                    }
                    else
                    {
                        CachedMovieInfo? movie = GetMovie(downloadShow.TmdbId);
                        movie?.UpgradeSearchResultToDirty();
                    }
                }
            }
        }

        private void MarkAllDirty()
        {
            lock (MOVIE_LOCK)
            {
                foreach (CachedMovieInfo m in Movies.Values)
                {
                    m.Dirty = true;
                }
            }
        }

        private void MarkPlaceholdersDirty()
        {
            lock (MOVIE_LOCK)
            {
                // anything with a srv_lastupdated of 0 should be marked as dirty
                // typically, this'll be placeholder cachedSeries
                foreach (CachedMovieInfo ser in Movies.Values.Where(ser => ser.SrvLastUpdated == 0))
                {
                    ser.Dirty = true;
                }
            }
        }

        private void AddPlaceholderSeries([NotNull] ISeriesSpecifier ss)
            => AddPlaceholderSeries(ss.TvdbId, ss.TvMazeId, ss.TmdbId, ss.TargetLocale);

        private void AddPlaceholderMovie([NotNull] ISeriesSpecifier ss)
            => AddPlaceholderMovie(ss.TvdbId, ss.TvMazeId, ss.TmdbId, ss.TargetLocale);

        public void UpdatesDoneOk()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            latestMovieUpdateTime.RecordSuccessfulUpdate();
        }

        public CachedSeriesInfo? GetSeries(string showName, bool showErrorMsgBox, Locale preferredLocale)
        {
            throw new NotImplementedException(); //todo - (BulkAdd Manager needs to work for new providers)
        }

        public CachedMovieInfo? GetMovie(PossibleNewMovie show, Locale preferredLocale, bool showErrorMsgBox) => GetMovie(show.RefinedHint, show.PossibleYear, preferredLocale, showErrorMsgBox, false);

        public CachedMovieInfo? GetMovie(string hint, int? possibleYear, Locale preferredLocale, bool showErrorMsgBox, bool useMostPopularMatch)
        {
            Search(hint, showErrorMsgBox, MediaConfiguration.MediaType.movie, preferredLocale);

            string showName = hint;

            if (string.IsNullOrEmpty(showName))
            {
                return null;
            }

            showName = showName.ToLower();

            List<CachedMovieInfo> matchingShows = GetSeriesDictMatching(showName).Values.ToList();

            if (matchingShows.Count == 0)
            {
                return null;
            }

            if (matchingShows.Count == 1)
            {
                return matchingShows.First();
            }

            List<CachedMovieInfo> exactMatchingShows = matchingShows
                .Where(info => info.Name.CompareName().Equals(showName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (exactMatchingShows.Count == 0 && !useMostPopularMatch)
            {
                return null;
            }

            if (exactMatchingShows.Count == 1)
            {
                return exactMatchingShows.First();
            }

            if (possibleYear is null && !useMostPopularMatch)
            {
                return null;
            }

            if (possibleYear != null)
            {
                List<CachedMovieInfo> exactMatchingShowsWithYear = exactMatchingShows
                    .Where(info => info.Year == possibleYear).ToList();

                if (exactMatchingShowsWithYear.Count == 0 && !useMostPopularMatch)
                {
                    return null;
                }

                if (exactMatchingShowsWithYear.Count == 1)
                {
                    return exactMatchingShowsWithYear.First();
                }
            }
            if (!useMostPopularMatch)
            {
                return null;
            }

            if (matchingShows.All(s => s.Popularity.HasValue))
            {
                return matchingShows.OrderByDescending(s => s.Popularity).First();
            }
            return null;
        }

        [NotNull]
        private Dictionary<int, CachedMovieInfo> GetSeriesDictMatching(string testShowName)
        {
            Dictionary<int, CachedMovieInfo> matchingSeries = new Dictionary<int, CachedMovieInfo>();

            testShowName = testShowName.CompareName();

            if (string.IsNullOrEmpty(testShowName))
            {
                return matchingSeries;
            }

            lock (MOVIE_LOCK)
            {
                foreach (KeyValuePair<int, CachedMovieInfo> kvp in Movies)
                {
                    string show = kvp.Value.Name.CompareName();

                    if (show.Contains(testShowName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //We have a match
                        matchingSeries.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            return matchingSeries;
        }

        public void Tidy(IEnumerable<MovieConfiguration> libraryValues)
        {
            // remove any shows from cache that aren't in My Movies
            List<MovieConfiguration> movieConfigurations = libraryValues.ToList();

            lock (MOVIE_LOCK)
            {
                List<int> removeList = Movies.Keys.Where(id => movieConfigurations.All(si => si.TmdbCode != id)).ToList();

                foreach (int i in removeList)
                {
                    ForgetMovie(i);
                }
            }
        }

        public void Tidy(IEnumerable<ShowConfiguration> libraryValues)
        {
            // remove any shows from TMDB that aren't in My Shows
            List<ShowConfiguration> showConfigurations = libraryValues.ToList();

            lock (SERIES_LOCK)
            {
                List<int> removeList = Series.Keys.Where(id => showConfigurations.All(si => si.TmdbCode != id)).ToList();

                foreach (int i in removeList)
                {
                    ForgetShow(i);
                }
            }
        }

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

            SaveCache();
            //All cachedSeries will be forgotten and will be fully refreshed, so we'll only need updates after this point
            latestMovieUpdateTime.Reset();
            LOGGER.Info($"Forget everything, so we assume we have TMDB updates until {latestMovieUpdateTime}");
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

        public void ForgetShow(ISeriesSpecifier ss)
        {
            ForgetShow(ss.TmdbId);
            lock (SERIES_LOCK)
            {
                if (ss.TmdbId > 0)
                {
                    AddPlaceholderSeries(ss);
                }
            }
        }

        public void UpdateSeries(CachedSeriesInfo si)
        {
            lock (SERIES_LOCK)
            {
                Series[si.TmdbCode] = si;
            }
        }

        public void AddOrUpdateEpisode(Episode e)
        {
            lock (SERIES_LOCK)
            {
                if (!Series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the cachedSeries to add the episode to (TVMaze). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}", TVDoc.ProviderType.TMDB);
                }

                CachedSeriesInfo ser = Series[e.SeriesId];

                ser.AddEpisode(e);
            }
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
            ForgetMovie(s.TmdbId);
            lock (MOVIE_LOCK)
            {
                if (s.TmdbId > 0)
                {
                    AddPlaceholderSeries(s);
                }
            }
        }

        public void AddPoster(int seriesId, IEnumerable<MovieImage> select)
        {
            throw new NotImplementedException(); //TODO
        }

        private void AddPlaceholderSeries(int tvdb, int tvmaze, int tmdb, Locale locale)
        {
            lock (SERIES_LOCK)
            {
                Series[tmdb] = new CachedSeriesInfo(tvdb, tvmaze, tmdb, locale, TVDoc.ProviderType.TMDB) { Dirty = true };
            }
        }

        private void AddPlaceholderMovie(int tvdb, int tvmaze, int tmdb, Locale locale)
        {
            lock (MOVIE_LOCK)
            {
                Movies[tmdb] = new CachedMovieInfo(tvdb, tvmaze, tmdb, locale, TVDoc.ProviderType.TMDB) { Dirty = true };
            }
        }

        public void Update(CachedMovieInfo si)
        {
            lock (MOVIE_LOCK)
            {
                Movies[si.TmdbCode] = si;
            }
        }

        public void LatestUpdateTimeIs(string time)
        {
            latestMovieUpdateTime.Load(time);
            LOGGER.Info($"Loaded file with updates until {latestMovieUpdateTime.LastSuccessfulServerUpdateDateTime()}");
        }

        public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TMDB;

        public CachedMovieInfo GetMovieAndDownload(int id, Locale locale, bool showErrorMsgBox) => HasMovie(id)
            ? CachedMovieData[id]
            : DownloadMovieNow(id, locale, showErrorMsgBox);

        internal CachedMovieInfo DownloadMovieNow(int id, Locale locale, bool showErrorMsgBox)
        {
            Movie downloadedMovie = Client.GetMovieAsync(id, locale.LanguageToUse(TVDoc.ProviderType.TMDB).Abbreviation, locale.LanguageToUse(TVDoc.ProviderType.TMDB).Abbreviation, MovieMethods.ExternalIds | MovieMethods.Images | MovieMethods.AlternativeTitles | MovieMethods.ReleaseDates | MovieMethods.Changes | MovieMethods.Videos | MovieMethods.Credits).Result;
            if (downloadedMovie is null)
            {
                throw new MediaNotFoundException(id, "TMDB no longer has this movie", TVDoc.ProviderType.TMDB, TVDoc.ProviderType.TMDB);
            }
            CachedMovieInfo m = new CachedMovieInfo(locale, TVDoc.ProviderType.TMDB)
            {
                Imdb = downloadedMovie.ExternalIds.ImdbId,
                TmdbCode = downloadedMovie.Id,
                Name = downloadedMovie.Title,
                Runtime = downloadedMovie.Runtime.ToString(),
                FirstAired = GetReleaseDateDetail(downloadedMovie, locale.RegionToUse(TVDoc.ProviderType.TMDB).Abbreviation) ?? GetReleaseDateDetail(downloadedMovie, TVSettings.Instance.TMDBRegion.Abbreviation) ?? downloadedMovie.ReleaseDate,
                Genres = downloadedMovie.Genres.Select(genre => genre.Name).ToList(),
                Overview = downloadedMovie.Overview,
                Network = downloadedMovie.ProductionCompanies.FirstOrDefault()?.Name, //TODO UPdate Movie to include multiple production companies
                Status = downloadedMovie.Status,
                ShowLanguage = downloadedMovie.OriginalLanguage,
                SiteRating = (float)downloadedMovie.VoteAverage,
                SiteRatingVotes = downloadedMovie.VoteCount,
                PosterUrl = PosterImageUrl(downloadedMovie.PosterPath),
                SrvLastUpdated = DateTime.UtcNow.Date.ToUnixTime(),
                CollectionName = downloadedMovie.BelongsToCollection?.Name,
                CollectionId = downloadedMovie.BelongsToCollection?.Id,
                TagLine = downloadedMovie.Tagline,
                Popularity = downloadedMovie.Popularity,
                TwitterId = downloadedMovie.ExternalIds.TwitterId,
                InstagramId = downloadedMovie.ExternalIds.InstagramId,
                FacebookId = downloadedMovie.ExternalIds.InstagramId,
                FanartUrl = OriginalImageUrl(downloadedMovie.BackdropPath),
                ContentRating = GetCertification(downloadedMovie, locale.RegionToUse(TVDoc.ProviderType.TMDB).Abbreviation) ?? GetCertification(downloadedMovie, TVSettings.Instance.TMDBRegion.Abbreviation) ?? GetCertification(downloadedMovie, Regions.Instance.FallbackRegion.Abbreviation) ?? string.Empty,
                OfficialUrl = downloadedMovie.Homepage,
                TrailerUrl = GetYouTubeUrl(downloadedMovie),
                Dirty = false,
            };

            foreach (string? s in downloadedMovie.AlternativeTitles.Titles.Select(title => title.Title))
            {
                m.AddAlias(s);
            }
            foreach (Cast? s in downloadedMovie.Credits.Cast)
            {
                m.AddActor(new Actor(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Character, s.CastId, s.Order));
            }
            foreach (TMDbLib.Objects.General.Crew? s in downloadedMovie.Credits.Crew)
            {
                m.AddCrew(new Crew(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Job, s.Department, s.CreditId));
            }

            File(m);

            return m;
        }

        private DateTime? GetReleaseDateDetail(Movie downloadedMovie, string? country)
        {
            List<DateTime> dates = downloadedMovie.ReleaseDates?.Results
                .Where(rel => rel.Iso_3166_1 == country)
                .SelectMany(rel => rel.ReleaseDates)
                .Select(d => d.ReleaseDate)
                .OrderBy(time => time).ToList();

            if (dates?.Any() ?? false)
            {
                return dates.First();
            }

            return null;
        }

        internal CachedSeriesInfo DownloadSeriesNow(ISeriesSpecifier ss, bool showErrorMsgBox)
        {
            int id = ss.TmdbId > 0 ? ss.TmdbId : GetSeriesIdFromOtherCodes(ss) ?? 0;

            TvShow? downloadedSeries = Client.GetTvShowAsync(id, TvShowMethods.ExternalIds | TvShowMethods.Images | TvShowMethods.AlternativeTitles | TvShowMethods.ContentRatings | TvShowMethods.Changes | TvShowMethods.Videos | TvShowMethods.Credits, ss.LanguageToUse().ISODialectAbbreviation).Result;
            if (downloadedSeries is null)
            {
                throw new MediaNotFoundException(id, "TMDB no longer has this show", TVDoc.ProviderType.TMDB, TVDoc.ProviderType.TMDB);
            }
            CachedSeriesInfo m = new CachedSeriesInfo(ss.TargetLocale, TVDoc.ProviderType.TMDB)
            {
                Imdb = downloadedSeries.ExternalIds.ImdbId,
                TmdbCode = downloadedSeries.Id,
                TvdbCode = downloadedSeries.ExternalIds.TvdbId.ToInt(ss.TvdbId),
                TvMazeCode = -1,
                Name = downloadedSeries.Name,
                Runtime = downloadedSeries.EpisodeRunTime.FirstOrDefault().ToString(System.Globalization.CultureInfo.CurrentCulture), //todo  use average?
                FirstAired = downloadedSeries.FirstAirDate,
                Genres = downloadedSeries.Genres.Select(genre => genre.Name).ToList(),
                Overview = downloadedSeries.Overview,
                Network = downloadedSeries.Networks.FirstOrDefault()?.Name, //TODO Utilise multiple networks
                Status = MapStatus(downloadedSeries.Status),
                ShowLanguage = downloadedSeries.OriginalLanguage,
                SiteRating = (float)downloadedSeries.VoteAverage,
                SiteRatingVotes = downloadedSeries.VoteCount,
                PosterUrl = PosterImageUrl(downloadedSeries.PosterPath),
                SrvLastUpdated = DateTime.UtcNow.Date.ToUnixTime(),
                TagLine = downloadedSeries.Tagline,
                TwitterId = downloadedSeries.ExternalIds.TwitterId,
                InstagramId = downloadedSeries.ExternalIds.InstagramId,
                FacebookId = downloadedSeries.ExternalIds.InstagramId,
                //FanartUrl = OriginalImageUrl(downloadedSeries.BackdropPath),  //TODO **** on Website
                ContentRating = GetCertification(downloadedSeries, ss.TargetLocale.RegionToUse(TVDoc.ProviderType.TMDB).Abbreviation) ?? GetCertification(downloadedSeries, TVSettings.Instance.TMDBRegion.Abbreviation) ?? GetCertification(downloadedSeries, Regions.Instance.FallbackRegion.Abbreviation) ?? string.Empty,
                OfficialUrl = downloadedSeries.Homepage,
                Type = downloadedSeries.Type,
                TrailerUrl = GetYouTubeUrl(downloadedSeries),
                Popularity = downloadedSeries.Popularity,
                Dirty = false,
            };

            foreach (string? s in downloadedSeries.AlternativeTitles.Results.Select(title => title.Title))
            {
                m.AddAlias(s);
            }
            foreach (TMDbLib.Objects.TvShows.Cast? s in downloadedSeries.Credits.Cast)
            {
                m.AddActor(new Actor(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Character, 0, s.Order));
            }
            foreach (TMDbLib.Objects.General.Crew? s in downloadedSeries.Credits.Crew)
            {
                m.AddCrew(new Crew(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Job, s.Department, s.CreditId));
            }
            if (downloadedSeries.Images.Backdrops.Any())
            {
                double bestBackdropRating = downloadedSeries.Images.Backdrops.Select(x => x.VoteAverage).Max();
                foreach (ImageData? image in downloadedSeries.Images.Backdrops.Where(x =>
                    Math.Abs(x.VoteAverage - bestBackdropRating) < .01))
                {
                    ShowImage newBanner = new ShowImage()
                    {
                        SeriesId = downloadedSeries.Id,
                        SeriesSource = TVDoc.ProviderType.TheTVDB,
                        Id = 1,
                        ImageUrl = OriginalImageUrl(image.FilePath),
                        ImageStyle = MediaImage.ImageType.Background,
                        Rating = image.VoteAverage,
                        RatingCount = image.VoteCount
                    };

                    m.AddOrUpdateImage(newBanner);
                }
            }
            if (downloadedSeries.Images.Posters.Any())
            {
                double bestPosterRating = downloadedSeries.Images.Posters.Select(x => x.VoteAverage).Max();
                foreach (ImageData? image in downloadedSeries.Images.Posters.Where(x =>
                    Math.Abs(x.VoteAverage - bestPosterRating) < .01))
                {
                    ShowImage newBanner = new ShowImage()
                    {
                        SeriesId = downloadedSeries.Id,
                        SeriesSource = TVDoc.ProviderType.TMDB,
                        Id = 2,
                        ImageUrl = PosterImageUrl(image.FilePath),
                        ImageStyle =MediaImage.ImageType.Poster,
                        Rating = image.VoteAverage,
                        RatingCount = image.VoteCount
                    };

                    m.AddOrUpdateImage(newBanner);
                }
            }

            foreach (var searchSeason in downloadedSeries.Seasons)
            {
                int snum = searchSeason.SeasonNumber;
                TvSeason? downloadedSeason = Client.GetTvSeasonAsync(downloadedSeries.Id, snum, TvSeasonMethods.Images, ss.LanguageToUse().ISODialectAbbreviation).Result;

                Season newSeason = new Season(downloadedSeason.Id ?? 0, snum, downloadedSeason.Name, downloadedSeason.Overview, string.Empty, downloadedSeason.PosterPath, downloadedSeries.Id);
                m.AddSeason(newSeason);

                foreach (TvSeasonEpisode? downloadedEpisode in downloadedSeason.Episodes)
                {
                    Episode newEpisode = new Episode(downloadedSeries.Id, m)
                    {
                        Name = downloadedEpisode.Name,
                        Overview = downloadedEpisode.Overview,
                        AirTime = downloadedEpisode.AirDate,
                        AirStamp = downloadedEpisode.AirDate,
                        FirstAired = downloadedEpisode.AirDate,
                        AiredEpNum = downloadedEpisode.EpisodeNumber,
                        AiredSeasonNumber = downloadedEpisode.SeasonNumber,
                        ProductionCode = downloadedEpisode.ProductionCode,
                        EpisodeId = downloadedEpisode.Id,
                        SiteRatingCount = downloadedEpisode.VoteCount,
                        EpisodeRating =
                            downloadedEpisode.VoteAverage.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        SeasonId = newSeason.SeasonId,
                        Filename = OriginalImageUrl(downloadedEpisode.StillPath),
                        EpisodeDirector = downloadedEpisode.Crew
                            .Where(x => x.Department == "Directing" && x.Job == "Director").Select(crew => crew.Name)
                            .ToPsv(),
                        EpisodeGuestStars = downloadedEpisode.GuestStars.Select(c => c.Name).ToPsv(),
                        Writer = downloadedEpisode.Crew
                            .Where(x => x.Department == "Writing").Select(crew => crew.Name)
                            .ToPsv()
                    };

                    m.AddEpisode(newEpisode);
                }

                if (downloadedSeason.Images != null && downloadedSeason.Images.Posters.Count > 0)
                {
                    double bestRating = downloadedSeason.Images.Posters.Select(x => x.VoteAverage).Max();
                    foreach (ImageData? image in downloadedSeason.Images.Posters.Where(x => Math.Abs(x.VoteAverage - bestRating) < .01))
                    {
                        ShowImage newBanner = new ShowImage()
                        {
                            Id = 10 + snum,
                            ImageUrl = OriginalImageUrl(image.FilePath),
                            ImageStyle = MediaImage.ImageType.Poster,
                            Rating = image.VoteAverage,
                            RatingCount = image.VoteCount,
                            SeasonId = downloadedSeason.Id ?? 0
                        };

                        m.AddOrUpdateImage(newBanner);
                    }
                }
            }

            File(m);

            return m;
        }

        private static string MapStatus(string s)
        {
            if (s == "Returning Series")
            {
                return "Continuing";
            }

            return s;
        }

        private int? GetSeriesIdFromOtherCodes(ISeriesSpecifier ss)
        {
            if (ss.ImdbCode.HasValue())
            {
                FindContainer? x = Client.FindAsync(FindExternalSource.Imdb, ss.ImdbCode).Result;

                if (ss.Type == MediaConfiguration.MediaType.tv)
                {
                    foreach (SearchTv? show in x.TvResults)
                    {
                        return show.Id;
                    }
                }
                else if (ss.Type == MediaConfiguration.MediaType.movie)
                {
                    foreach (SearchMovie? show in x.MovieResults)
                    {
                        return show.Id;
                    }
                }
            }

            if (ss.TvdbId > 0)
            {
                FindContainer? x = Client.FindAsync(FindExternalSource.TvDb, ss.TvdbId.ToString()).Result;

                if (ss.Type == MediaConfiguration.MediaType.tv)
                {
                    foreach (SearchTv? show in x.TvResults)
                    {
                        return show.Id;
                    }
                }
                else if (ss.Type == MediaConfiguration.MediaType.movie)
                {
                    foreach (SearchMovie? show in x.MovieResults)
                    {
                        return show.Id;
                    }
                }
            }

            return null;
        }

        private string GetYouTubeUrl(Movie downloadedMovie)
        {
            string yid = downloadedMovie.Videos.Results.Where(video => video.Type == "Trailer" && video.Site == "YouTube").OrderByDescending(v => v.Size).Select(video => video.Key).FirstOrDefault() ?? string.Empty;
            return yid.HasValue() ? $"https://www.youtube.com/watch?v={yid}" : string.Empty;
        }

        private string GetYouTubeUrl(TvShow downloadedMovie)
        {
            string yid = downloadedMovie.Videos.Results.Where(video => video.Type == "Trailer" && video.Site == "YouTube").OrderByDescending(v => v.Size).Select(video => video.Key).FirstOrDefault() ?? string.Empty;
            return yid.HasValue() ? $"https://www.youtube.com/watch?v={yid}" : string.Empty;
        }

        private string? GetCertification(Movie downloadedMovie, string country)
        {
            return downloadedMovie.ReleaseDates?.Results
                .Where(rel => rel.Iso_3166_1 == country)
                .Select(rel => rel.ReleaseDates.First().Certification)
                .FirstOrDefault();
        }

        private string? GetCertification(TvShow downloadedShow, string country)
        {
            return downloadedShow.ContentRatings?.Results
                .Where(rel => rel.Iso_3166_1 == country)
                .Select(rel => rel.Rating)
                .FirstOrDefault();
        }

        public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type,
            Locale locale)
        {
            if (type == MediaConfiguration.MediaType.movie)
            {
                SearchContainer<SearchMovie> results = Client.SearchMovieAsync(text, locale.LanguageToUse(TVDoc.ProviderType.TMDB).ISODialectAbbreviation).Result;
                LOGGER.Info(
                    $"Got {results.Results.Count:N0} of {results.TotalResults:N0} results searching for {text}");

                foreach (SearchMovie result in results.Results)
                {
                    LOGGER.Info($"   Movie: {result.Title}:{result.Id}   {result.Popularity}");
                    File(result);
                    try
                    {
                        DownloadMovieNow(result.Id, locale, showErrorMsgBox);
                    }
                    catch (MediaNotFoundException sex)
                    {
                        LOGGER.Warn($"Could not get full details of {result.Id} while searching for '{text}'");
                    }
                }
            }
            else
            {
                SearchContainer<SearchTv>? results = Client.SearchTvShowAsync(text).Result;
                LOGGER.Info(
                    $"Got {results.Results.Count:N0} of {results.TotalResults:N0} results searching for {text}");

                foreach (SearchTv result in results.Results)
                {
                    LOGGER.Info($"   TV Show: {result.Name}:{result.Id}   {result.Popularity}");
                    File(result);
                    try
                    {
                        ISeriesSpecifier ss = new SearchSeriesSpecifier(-1, -1, result.Id, locale, result.Name,
                            TVDoc.ProviderType.TMDB, null, MediaConfiguration.MediaType.tv);
                        DownloadSeriesNow(ss, showErrorMsgBox);
                    }
                    catch (MediaNotFoundException sex)
                    {
                        LOGGER.Warn($"Could not get full details of {result.Id} while searching for '{text}'");
                    }
                }
            }
        }

        private CachedSeriesInfo File(SearchTv result)
        {
            CachedSeriesInfo m = new CachedSeriesInfo(new Locale(), TVDoc.ProviderType.TMDB)
            {
                TmdbCode = result.Id,
                Name = result.Name,
                FirstAired = result.FirstAirDate,
                Overview = result.Overview,
                //Status = result.Status,
                ShowLanguage = result.OriginalLanguage,
                SiteRating = (float)result.VoteAverage,
                SiteRatingVotes = result.VoteCount,
                PosterUrl = PosterImageUrl(result.PosterPath),
                Popularity = result.Popularity,
                IsSearchResultOnly = true,
                Dirty = false,
                SrvLastUpdated = DateTime.UtcNow.Date.ToUnixTime(),
                //FanartUrl = OriginalImageUrl(result.BackdropPath),  //TODO **** on Website
            };

            File(m);
            return m;
        }

        private CachedMovieInfo File(SearchMovie result)
        {
            CachedMovieInfo m = new CachedMovieInfo(new Locale(), TVDoc.ProviderType.TMDB)
            {
                TmdbCode = result.Id,
                Name = result.Title,
                FirstAired = result.ReleaseDate,
                Overview = result.Overview,
                ShowLanguage = result.OriginalLanguage,
                SiteRating = (float)result.VoteAverage,
                SiteRatingVotes = result.VoteCount,
                PosterUrl = PosterImageUrl(result.PosterPath),
                Popularity = result.Popularity,
                FanartUrl = OriginalImageUrl(result.BackdropPath),
                IsSearchResultOnly = true,
                Dirty = false,
            };

            File(m);
            return m;
        }

        private static string? ImageUrl(string source) => ImageUrl(source, "w600_and_h900_bestv2");

        private static string? PosterImageUrl(string source) => ImageUrl(source, "w600_and_h900_bestv2");

        private static string? OriginalImageUrl(string source) => ImageUrl(source, "original");

        private static string? ImageUrl(string source, string type)
        {
            if (source.HasValue())
            {
                return "https://image.tmdb.org/t/p/" + type + source;
            }

            return null;
        }

        public CachedMovieInfo? LookupMovieByImdb(string imdbToTest, Locale locale, bool showErrorMsgBox)
        {
            FindContainer? results = Client.FindAsync(FindExternalSource.Imdb, imdbToTest).Result;
            LOGGER.Info($"Got {results.MovieResults.Count:N0} results searching for {imdbToTest}");
            foreach (SearchMovie result in results.MovieResults)
            {
                DownloadMovieNow(result.Id, locale, showErrorMsgBox);
            }

            if (results.MovieResults.Count == 0)
            {
                return null;
            }

            if (results.MovieResults.Count == 1)
            {
                lock (MOVIE_LOCK)
                {
                    return Movies[results.MovieResults.First().Id];
                }
            }

            return null;
        }

        public int? LookupTvbdIdByImdb(string imdbToTest, bool showErrorMsgBox)
        {
            FindContainer? results = Client.FindAsync(FindExternalSource.Imdb, imdbToTest).Result;
            LOGGER.Info($"Got {results.TvResults.Count:N0} results searching for {imdbToTest}");

            if (results.TvResults.Count == 0)
            {
                return null;
            }

            if (results.TvResults.Count == 1)
            {
                return results.TvResults.First().Id;
            }

            return null;
        }

        private void File(CachedMovieInfo cachedMovie)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(cachedMovie.TmdbCode))
                {
                    Movies[cachedMovie.TmdbCode].Merge(cachedMovie);
                }
                else
                {
                    Movies[cachedMovie.TmdbCode] = cachedMovie;
                }
            }
        }

        private void File(CachedSeriesInfo s)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(s.TmdbCode))
                {
                    Series[s.TmdbCode].Merge(s);
                }
                else
                {
                    Series[s.TmdbCode] = s;
                }
            }
        }

        public Dictionary<int, CachedMovieInfo> GetMovieIdsFromCollection(int collectionId, string languageCode)
        {
            Dictionary<int, CachedMovieInfo> returnValue = new Dictionary<int, CachedMovieInfo>();
            TMDbLib.Objects.Collections.Collection collection = Client.GetCollectionAsync(collectionId, languageCode, languageCode).Result;
            if (collection == null)
            {
                return returnValue;
            }

            foreach (SearchMovie? m in collection.Parts)
            {
                int id = m.Id;
                CachedMovieInfo info = File(m);
                returnValue.Add(id, info);
            }

            return returnValue;
        }

        public CachedMovieInfo? LookupMovieByTvdb(int tvdbId, bool showErrorMsgBox)
        {
            throw new NotImplementedException(); //TODO
        }

        public IEnumerable<CachedMovieInfo> ServerAccuracyCheck()
        {
            Say("TMDB Accuracy Check (Movies) running");
            TmdbAccuracyCheck check = new TmdbAccuracyCheck(this);
            lock (MOVIE_LOCK)
            {
                foreach (CachedMovieInfo si in Movies.Values.Where(info => !info.IsSearchResultOnly).OrderBy(s => s.Name).ToList())
                {
                    check.ServerAccuracyCheck(si);
                }
            }

            foreach (string issue in check.Issues)
            {
                LOGGER.Warn(issue);
            }

            SayNothing();
            return check.MoviesToUpdate;
        }

        public IEnumerable<CachedSeriesInfo> ServerShowsAccuracyCheck()
        {
            Say("TMDB Accuracy Check (TV) running");
            TmdbAccuracyCheck check = new TmdbAccuracyCheck(this);
            lock (SERIES_LOCK)
            {
                foreach (CachedSeriesInfo si in Series.Values.Where(info => !info.IsSearchResultOnly).OrderBy(s => s.Name).ToList())
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

        public async Task<Recomendations> GetRecommendations(TVDoc mDoc, BackgroundWorker sender, List<ShowConfiguration> shows, string languageCode)
        {
            int total = shows.Count;
            int current = 0;
            Task<SearchContainer<SearchTv>> topRated = Client.GetTvShowTopRatedAsync(language: languageCode);
            Task<SearchContainer<SearchTv>> trending = Client.GetTrendingTvAsync(TimeWindow.Week);
            await topRated;
            await trending;

            Recomendations returnValue = new Recomendations();

            foreach (SearchTv? top in topRated.Result.Results)
            {
                File(top);
                returnValue.AddTopRated(top.Id);
            }
            foreach (SearchTv? top in trending.Result.Results)
            {
                File(top);
                returnValue.AddTrending(top.Id);
            }

            foreach (ShowConfiguration? arg in shows)
            {
                try
                {
                    AddRecommendationsFrom(arg, returnValue, languageCode);

                    sender.ReportProgress(100 * current++ / total, arg.CachedShow?.Name);
                }
                catch
                {
                    //todo record and resolve /retry errors
                }
            }

            return returnValue;
        }

        private void AddRecommendationsFrom(ShowConfiguration arg, Recomendations returnValue, string languageCode)
        {
            if (arg.TmdbCode == 0)
            {
                string? imdb = arg.CachedShow?.Imdb;
                if (!imdb.HasValue())
                {
                    return;
                }

                int? tmdbcode = LookupTvbdIdByImdb(imdb!, false);
                if (!tmdbcode.HasValue)
                {
                    return;
                }

                arg.TmdbCode = tmdbcode.Value;
            }

            Task<SearchContainer<SearchTv>>? related = Client.GetTvShowRecommendationsAsync(arg.TmdbCode, languageCode);
            Task<SearchContainer<SearchTv>>? similar = Client.GetTvShowSimilarAsync(arg.TmdbCode, languageCode);

            Task.WaitAll(related, similar);
            if (related.Result != null)
            {
                foreach (SearchTv? s in related.Result.Results)
                {
                    File(s);
                    returnValue.AddRelated(s.Id, arg);
                }
            }

            if (similar.Result != null)
            {
                foreach (SearchTv? s in similar.Result.Results)
                {
                    File(s);
                    returnValue.AddSimilar(s.Id, arg);
                }
            }
        }

        public async Task<Recomendations> GetRecommendations(TVDoc mDoc, BackgroundWorker sender, List<MovieConfiguration> movies, string languageCode)
        {
            int total = movies.Count;
            int current = 0;
            Task<SearchContainer<SearchMovie>> topRated = Client.GetMovieTopRatedListAsync(languageCode);
            Task<SearchContainer<SearchMovie>> trending = Client.GetTrendingMoviesAsync(TimeWindow.Week);
            await topRated;
            await trending;

            Recomendations returnValue = new Recomendations();

            foreach (SearchMovie? top in topRated.Result.Results)
            {
                File(top);
                returnValue.AddTopRated(top.Id);
            }
            foreach (SearchMovie? top in trending.Result.Results)
            {
                File(top);
                returnValue.AddTrending(top.Id);
            }

            foreach (MovieConfiguration? arg in movies)
            {
                try
                {
                    Task<SearchContainer<SearchMovie>>? related = Client.GetMovieRecommendationsAsync(arg.TmdbCode, languageCode);
                    Task<SearchContainer<SearchMovie>>? similar = Client.GetMovieSimilarAsync(arg.TmdbCode, languageCode);

                    Task.WaitAll(related, similar);
                    foreach (SearchMovie? movie in related.Result.Results)
                    {
                        File(movie);
                        returnValue.AddRelated(movie.Id, arg);
                    }

                    foreach (SearchMovie? movie in similar.Result.Results)
                    {
                        File(movie);
                        returnValue.AddSimilar(movie.Id, arg);
                    }

                    sender.ReportProgress(100 * current++ / total, arg.CachedMovie?.Name);
                }
                catch
                {
                    //todo - record error, retry etc
                }
            }

            //var related = movies.Select(arg => (arg.TmdbCode,Client.GetMovieRecommendationsAsync(arg.TmdbCode))).ToList();
            //var similar = movies.Select(arg => (arg.TmdbCode,Client.GetMovieSimilarAsync(arg.TmdbCode))).ToList();

            //Task.WaitAll(related.Select(tuple => tuple.Item2).ToArray());
            //Task.WaitAll(similar.Select(tuple => tuple.Item2).ToArray());
            return returnValue;
        }

        public void ReConnect(bool b)
        {
            //nothing to be done here
        }

        public TVDoc.ProviderType SourceProvider() => TVDoc.ProviderType.TMDB;
    }
}
