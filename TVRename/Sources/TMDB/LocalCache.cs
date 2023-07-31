//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using TMDbLib.Client;
using TMDbLib.Objects.Exceptions;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Trending;
using TMDbLib.Objects.TvShows;
using TVRename.Forms;
using static TVRename.TMDB.API;
using Cast = TMDbLib.Objects.Movies.Cast;

namespace TVRename.TMDB;

// ReSharper disable once InconsistentNaming
public class LocalCache : MediaCache, iMovieSource, iTVSource
{
    private const string KEY = "2dcfd2d08f80439d7ef5210f217b80b4";
    private static readonly TMDbClient Client = new(KEY);
    private UpdateTimeTracker latestUpdateTime = new();

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
        latestUpdateTime.Reset();
        LOGGER.Info($"Forget everything, so we assume we have TMDB updates until {latestUpdateTime}");
    }

    public override int PrimaryKey(ISeriesSpecifier ss) => ss.TmdbId;

    public override string CacheSourceName() => "TMDB";

    public void Setup(FileInfo? loadFrom, FileInfo cache, bool showIssues)
    {
        System.Diagnostics.Debug.Assert(cache != null);
        CacheFile = cache;

        //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
        //If we have no prior install then the app has no shows and is by definition up-to-date
        latestUpdateTime = new UpdateTimeTracker();

        LastErrorMessage = string.Empty;

        if (loadFrom is null)
        {
            LoadOk = true;
            return;
        }
        bool mOK = CachePersistor.LoadMovieCache(loadFrom, this);
        bool tvOk = CachePersistor.LoadTvCache(loadFrom, this);
        LoadOk = mOK && tvOk;
    }

    public bool Connect(bool showErrorMsgBox) => true;

    public void SaveCache()
    {
        lock (MOVIE_LOCK)
        {
            lock (SERIES_LOCK)
            {
                CachePersistor.SaveCache(Series, Movies, CacheFile!, latestUpdateTime.LastSuccessfulServerUpdateTimecode());
            }
        }
    }

    /// <exception cref="SourceConsistencyException">Condition.</exception>
    /// <exception cref="MediaNotFoundException">Condition.</exception>
    public override bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
    {
        if (s.Provider != TVDoc.ProviderType.TMDB)
        {
            throw new SourceConsistencyException($"Asked to update {s.Name} from TMDB, but the Id is not for TMDB.", TVDoc.ProviderType.TMDB);
        }
        if (s.TmdbId is -1 or 0)
        {
            throw new MediaNotFoundException(s, $"Please edit {s.Name} to ensure it has a TMDB Id, or use another source for that show.", TVDoc.ProviderType.TMDB, TVDoc.ProviderType.TMDB, s.Media);
        }

        return s.Media == MediaConfiguration.MediaType.movie
            ? EnsureMovieUpdated(s)
            : EnsureSeriesUpdated(s);
    }

    private bool EnsureSeriesUpdated(ISeriesSpecifier s)
    {
        lock (SERIES_LOCK)
        {
            if (Series.TryGetValue(s.TmdbId,out CachedSeriesInfo? si) && !si.Dirty)
            {
                return true;
            }
        }

        Say($"Series {s.Name} from TMDB");
        try
        {
            CachedSeriesInfo downloadedSi = DownloadSeriesNow(s);

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

            this.AddSeriesToCache(downloadedSi);
        }
        catch (SourceConnectivityException ex)
        {
            LOGGER.Warn(ex.ErrorText());
            LastErrorMessage = ex.Message;
            return false;
        }
        catch (SourceConsistencyException sce)
        {
            LOGGER.Error(sce.ErrorText());
            LastErrorMessage = sce.Message;
            return false;
        }
        catch (TaskCanceledException tce)
        {
            LOGGER.Warn($"Timeout obtaining {s.Name} from TMDB");
            LastErrorMessage = tce.Message;
            return false;
        }
        finally
        {
            SayNothing();
        }

        return true;
    }

    private bool EnsureMovieUpdated(ISeriesSpecifier id)
    {
        lock (MOVIE_LOCK)
        {
            if (Movies.TryGetValue(id.TmdbId, out CachedMovieInfo? movie) && !movie.Dirty)
            {
                return true;
            }
        }

        Say($"Movie: {id.Name} from TMDB");
        try
        {
            CachedMovieInfo downloadedSi = DownloadMovieNow(id);

            if (downloadedSi.TmdbCode != id.TmdbId && id.TmdbId == -1)
            {
                lock (MOVIE_LOCK)
                {
                    Movies.TryRemove(-1, out _);
                }
            }

            this.AddMovieToCache(downloadedSi);
            return true;
        }
        catch (SourceConnectivityException ex)
        {
            LOGGER.Warn(ex.ErrorText());
            LastErrorMessage = ex.Message;
            return false;
        }
        catch (SourceConsistencyException sce)
        {
            LOGGER.Error(sce.ErrorText());
            LastErrorMessage = sce.Message;
            return false;
        }
        finally
        {
            SayNothing();
        }
    }

    public override bool GetUpdates(List<ISeriesSpecifier> ss, bool showErrorMsgBox, CancellationToken cts)
    {
        Say("Validating TMDB cache");
        this.MarkPlaceHoldersDirty(ss);

        try
        {
            Say($"Updates list from TMDB since {latestUpdateTime.LastSuccessfulServerUpdateDateTime()}");

            long updateFromEpochTime = latestUpdateTime.LastSuccessfulServerUpdateTimecode();
            if (updateFromEpochTime == 0)
            {
                this.MarkAllDirty();
                latestUpdateTime.RegisterServerUpdate(TimeHelpers.UnixUtcNow());
                return true;
            }

            latestUpdateTime.RegisterServerUpdate(TimeHelpers.UnixUtcNow());

            List<int> movieUpdates = Client.GetChangesMovies(latestUpdateTime, cts).Select(item => item.Id)
                .Distinct().ToList();

            Say(
                $"Processing {movieUpdates.Count} movie updates from TMDB. From between {latestUpdateTime.LastSuccessfulServerUpdateDateTime()} and {latestUpdateTime.ProposedServerUpdateDateTime()}");

            lock (MOVIE_LOCK)
            {
                foreach (int id in movieUpdates)
                {
                    if (!cts.IsCancellationRequested)
                    {
                        if (HasMovie(id))
                        {
                            CachedMovieInfo? x = GetMovie(id);
                            if (x is not null)
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

                Say(
                    $"Identified {Movies.Values.Count(info => info.Dirty && !info.IsSearchResultOnly)} TMDB Movies need updating");
            }

            List<int> showUpdates = Client.GetChangesShows(latestUpdateTime, cts).Select(item => item.Id).Distinct()
                .ToList();

            Say(
                $"Processing {showUpdates.Count} show updates from TMDB. From between {latestUpdateTime.LastSuccessfulServerUpdateDateTime()} and {latestUpdateTime.ProposedServerUpdateDateTime()}");

            lock (SERIES_LOCK)
            {
                foreach (int id in showUpdates)
                {
                    if (!cts.IsCancellationRequested)
                    {
                        if (HasSeries(id))
                        {
                            CachedSeriesInfo? x = GetSeries(id);
                            if (x is not null)
                            {
                                LOGGER.Info(
                                    $"Identified that Show with TMDB Id {id} {x.Name} should be updated.");

                                x.Dirty = true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                Say(
                    $"Identified {Series.Values.Count(info => info.Dirty && !info.IsSearchResultOnly)} TMDB Shows need updating");
            }

            return true;
        }
        catch (GeneralHttpException ex)
        {
            LOGGER.Warn(ex.ErrorText());
            LastErrorMessage = ex.Message;
            return false;
        }
        catch (SourceConnectivityException ex)
        {
            LOGGER.Warn(ex.ErrorText());
            LastErrorMessage = ex.Message;
            return false;
        }
        catch (SourceConsistencyException sce)
        {
            LOGGER.Error(sce.ErrorText());
            LastErrorMessage = sce.Message;
            return false;
        }
        catch (TaskCanceledException)
        {
            LastErrorMessage = "Request to get updates Cancelled";
            LOGGER.Warn(LastErrorMessage);
            return false;
        }
        catch (TooManyCallsException)
        {
            LastErrorMessage = "Too Many Calls Made - cancelled";
            LOGGER.Warn(LastErrorMessage);
            return false;
        }
        finally
        {
            SayNothing();
        }
    }

    public void UpdatesDoneOk()
    {
        // call when all downloading and updating is done.  updates local Srv_Time with the tentative
        // new_srv_time value.
        latestUpdateTime.RecordSuccessfulUpdate();
    }

    public CachedMovieInfo? GetMovie(PossibleNewMovie show, Locale preferredLocale, bool showErrorMsgBox) => this.GetMovie(show.RefinedHint, show.PossibleYear, preferredLocale, showErrorMsgBox, false);

    /// <exception cref="SourceConsistencyException">Condition.</exception>
    public void AddOrUpdateEpisode(Episode e)
    {
        lock (SERIES_LOCK)
        {
            if (!Series.TryGetValue(e.SeriesId, out CachedSeriesInfo? ser))
            {
                throw new SourceConsistencyException(
                    $"Can't find the cachedSeries to add the episode to (TVMaze). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}", TVDoc.ProviderType.TMDB);
            }

            ser.AddEpisode(e);
        }
    }

    public void LatestUpdateTimeIs(string time)
    {
        latestUpdateTime.Load(time);
        LOGGER.Info($"Loaded file with updates until {latestUpdateTime.LastSuccessfulServerUpdateDateTime()}");
    }

    public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TMDB;

    /// <exception cref="MediaNotFoundException">Condition.</exception>
    /// <exception cref="SourceConnectivityException">Condition.</exception>
    public CachedMovieInfo GetMovieAndDownload(ISeriesSpecifier id) => HasMovie(id.TmdbId)
        ? CachedMovieData[id.TmdbId]
        : DownloadMovieNow(id);

    /// <exception cref="MediaNotFoundException">Condition.</exception>
    /// <exception cref="SourceConnectivityException">Condition.</exception>
    internal CachedMovieInfo DownloadMovieNow(ISeriesSpecifier id, bool saveToCache = true)
    {
        string errorMessage = $"Error obtaining TMDB Movie for {id} in {id.TargetLocale.LanguageToUse(TVDoc.ProviderType.TMDB).EnglishName}:";

        return HandleWebErrorsFor(() => DownloadMovieNowInternal(id, saveToCache), errorMessage);
    }

    /// <exception cref="MediaNotFoundException">Condition.</exception>
    private CachedMovieInfo DownloadMovieNowInternal(ISeriesSpecifier seriesSpecifier, bool b)
    {
        string imageLanguage = $"{seriesSpecifier.LanguageToUse().Abbreviation},null";
        Movie downloadedMovie = Client.GetMovieAsync(seriesSpecifier.TmdbId, seriesSpecifier.LanguageToUse().Abbreviation,
                imageLanguage,
                MovieMethods.ExternalIds | MovieMethods.Images | MovieMethods.AlternativeTitles |
                MovieMethods.ReleaseDates | MovieMethods.Changes | MovieMethods.Videos | MovieMethods.Credits)
            .Result ?? throw new MediaNotFoundException(seriesSpecifier, "TMDB no longer has this movie",
            TVDoc.ProviderType.TMDB, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie);

        CachedMovieInfo m = GenerateCachedMovieInfo(seriesSpecifier, downloadedMovie);

        if (b)
        {
            this.AddMovieToCache(m);
        }

        return m;
    }

    private static CachedMovieInfo GenerateCachedMovieInfo(ISeriesSpecifier id, Movie downloadedMovie)
    {
        DateTime? downloadedMovieReleaseDate = TVSettings.Instance.UseGlobalReleaseDate
            ? GetEarliestReleaseDateDetail(downloadedMovie)
              ?? downloadedMovie.ReleaseDate
            : GetReleaseDateDetail(downloadedMovie, id.RegionToUse().Abbreviation)
              ?? GetReleaseDateDetail(downloadedMovie, TVSettings.Instance.TMDBRegion.Abbreviation)
              ?? downloadedMovie.ReleaseDate;

        CachedMovieInfo m = new(id.TargetLocale, TVDoc.ProviderType.TMDB)
        {
            Imdb = downloadedMovie.ExternalIds?.ImdbId,
            TmdbCode = downloadedMovie.Id,
            Name = downloadedMovie.Title,
            Runtime = downloadedMovie.Runtime?.ToString(),
            FirstAired = downloadedMovieReleaseDate,
            Genres = downloadedMovie.Genres?.Select(genre => genre.Name).ToSafeList()??new SafeList<string>(),
            Overview = downloadedMovie.Overview,
            Network = downloadedMovie.ProductionCompanies?.Select(y => y.Name).ToPsv(),
            Status = downloadedMovie.Status,
            ShowLanguage = downloadedMovie.OriginalLanguage,
            SiteRating = (float)downloadedMovie.VoteAverage,
            SiteRatingVotes = downloadedMovie.VoteCount,
            PosterUrl = PosterImageUrl(downloadedMovie.PosterPath),
            SrvLastUpdated = TimeHelpers.UtcNow().Date.ToUnixTime(),
            CollectionName = downloadedMovie.BelongsToCollection?.Name,
            CollectionId = downloadedMovie.BelongsToCollection?.Id,
            TagLine = downloadedMovie.Tagline,
            Popularity = downloadedMovie.Popularity,
            TwitterId = downloadedMovie.ExternalIds?.TwitterId,
            InstagramId = downloadedMovie.ExternalIds?.InstagramId,
            FacebookId = downloadedMovie.ExternalIds?.InstagramId,
            FanartUrl = OriginalImageUrl(downloadedMovie.BackdropPath),
            ContentRating = GetCertification(downloadedMovie, id.RegionToUse().Abbreviation) ??
                            GetCertification(downloadedMovie, TVSettings.Instance.TMDBRegion.Abbreviation) ??
                            GetCertification(downloadedMovie, Regions.Instance.FallbackRegion.Abbreviation) ?? string.Empty,
            OfficialUrl = downloadedMovie.Homepage,
            TrailerUrl = GetYouTubeUrl(downloadedMovie),
            WebUrl = $"https://www.themoviedb.org/movie/{downloadedMovie.Id}",
            Dirty = false,
            Country = downloadedMovie.ProductionCountries.FirstOrDefault()?.Name,
        };

        if (downloadedMovie.AlternativeTitles !=null)
        {
            foreach (string? s in downloadedMovie.AlternativeTitles.Titles
                         .Where(t => t.Iso_3166_1 == id.RegionToUse().Abbreviation).Select(title => title.Title))
            {
                m.AddAlias(s);
            }
        }

        if (downloadedMovie.Credits != null)
        {
            foreach (Cast? s in downloadedMovie.Credits.Cast)
            {
                if (s is not null)
                {
                    m.AddActor(new Actor(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Character,
                        s.Order));
                }
            }
            foreach (TMDbLib.Objects.General.Crew? s in downloadedMovie.Credits.Crew)
            {
                if (s is not null)
                {
                    m.AddCrew(new Crew(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Job, s.Department, s.CreditId));
                }
            }
        }

        AddMovieImages(downloadedMovie, m);
        return m;
    }

    private static void AddMovieImages(Movie downloadedMovie, CachedMovieInfo m)
    {
        int imageId = 1; //TODO See https://www.themoviedb.org/talk/60ba61a4cb9f4b006f30f82b for  why we need this
        if (downloadedMovie.Images?.Backdrops != null && downloadedMovie.Images.Backdrops.Any())
        {
            foreach (ImageData? image in downloadedMovie.Images.Backdrops)
            {
                if (image is null)
                {
                    continue;
                }
                MovieImage newBanner = new()
                {
                    MovieId = downloadedMovie.Id,
                    MovieSource = TVDoc.ProviderType.TMDB,
                    Id = imageId++,
                    ImageUrl = OriginalImageUrl(image.FilePath),
                    ImageStyle = MediaImage.ImageType.background,
                    Rating = image.VoteAverage,
                    RatingCount = image.VoteCount,
                    LanguageCode = image.Iso_639_1
                };

                m.AddOrUpdateImage(newBanner);
            }
        }

        if (downloadedMovie.Images?.Posters != null && downloadedMovie.Images.Posters.Any())
        {
            foreach (ImageData? image in downloadedMovie.Images.Posters)
            {
                if (image is null)
                {
                    continue;
                }
                MovieImage newBanner = new()
                {
                    MovieId = downloadedMovie.Id,
                    MovieSource = TVDoc.ProviderType.TMDB,
                    Id = imageId++,
                    ImageUrl = PosterImageUrl(image.FilePath),
                    ImageStyle = MediaImage.ImageType.poster,
                    Rating = image.VoteAverage,
                    RatingCount = image.VoteCount,
                    LanguageCode = image.Iso_639_1
                };

                m.AddOrUpdateImage(newBanner);
            }
        }
    }

    private static DateTime? GetReleaseDateDetail(Movie downloadedMovie, string? country)
    {
        return downloadedMovie.ReleaseDates?.Results
            .Where(rel => rel.Iso_3166_1.Equals(country, StringComparison.OrdinalIgnoreCase))
            .SelectMany(rel => rel.ReleaseDates)
            .MinOrNull(d => d.ReleaseDate);
    }

    private static DateTime? GetEarliestReleaseDateDetail(Movie downloadedMovie)
    {
        return downloadedMovie.ReleaseDates?.Results
            .SelectMany(rel => rel.ReleaseDates)
            .MinOrNull(d => d.ReleaseDate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="webCall"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    /// <exception cref="SourceConnectivityException"></exception>
    private T HandleWebErrorsFor<T>(Func<T> webCall, string errorMessage)
    {
        try
        {
            return webCall.WithRetry(3,10.Seconds(),ex=>ex is RequestLimitExceededException ,errorMessage);
        }
        catch (System.IO.IOException ioex)
        {
            LOGGER.LogIoException($"Error {errorMessage}:", ioex);

            SayNothing();
            LastErrorMessage = ioex.LoggableDetails();
            throw new SourceConnectivityException(errorMessage, ioex);
        }
        catch (WebException ex)
        {
            LOGGER.LogWebException($"Error {errorMessage}:", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException(errorMessage, ex);
        }
        catch (GeneralHttpException ex)
        {
            LOGGER.Error($"Error {errorMessage}:", ex);
            SayNothing();
            LastErrorMessage = ex.Message + ex.InnerException?.Message;
            throw new SourceConnectivityException(errorMessage, ex);
        }
        catch (AggregateException ex) when (ex.InnerException is WebException wex)
        {
            LOGGER.LogWebException($"Error {errorMessage}:", wex);

            SayNothing();
            LastErrorMessage = wex.LoggableDetails();
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage, wex);
        }
        catch (HttpRequestException ex)
        {
            LOGGER.LogHttpRequestException($"Error {errorMessage}:", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException(errorMessage, ex);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            LOGGER.LogHttpRequestException($"Error {errorMessage}:", wex);

            SayNothing();
            LastErrorMessage = wex.LoggableDetails();
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage, wex);
        }
        catch (TaskCanceledException ex)
        {
            LOGGER.LogTaskCanceledException($"Error {errorMessage}:", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException(errorMessage, ex);
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException wex)
        {
            LOGGER.LogTaskCanceledException($"Error {errorMessage}:", wex);

            SayNothing();
            LastErrorMessage = wex.LoggableDetails();
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage, wex);
        }
    }

    /// <exception cref="MediaNotFoundException">Condition.</exception>
    /// <exception cref="SourceConnectivityException">Condition.</exception>
    internal CachedSeriesInfo DownloadSeriesNow(ISeriesSpecifier ss, bool saveToCache = true)
    {
        string errorMessage = $"Error obtaining TMDB Show for {ss} in {ss.TargetLocale.LanguageToUse(TVDoc.ProviderType.TMDB).EnglishName}:";
        return HandleWebErrorsFor(() => DownloadSeriesNowInternal(ss, saveToCache), errorMessage);
    }

    /// <exception cref="MediaNotFoundException">Condition.</exception>
    private CachedSeriesInfo DownloadSeriesNowInternal(ISeriesSpecifier seriesSpecifier, bool save)
    {
        int id = seriesSpecifier.TmdbId > 0 ? seriesSpecifier.TmdbId : GetSeriesIdFromOtherCodes(seriesSpecifier) ?? 0;
        string imageLanguage = $"{seriesSpecifier.LanguageToUse().Abbreviation},null";

        TvShow downloadedSeries = Client.GetTvShowAsync(id,
                                          TvShowMethods.ExternalIds | TvShowMethods.Images |
                                          TvShowMethods.AlternativeTitles |
                                          TvShowMethods.ContentRatings | TvShowMethods.Changes | TvShowMethods.Videos |
                                          TvShowMethods.Credits, seriesSpecifier.LanguageToUse().Abbreviation,
                                          imageLanguage)
                                      .Result
                                  ?? throw new MediaNotFoundException(seriesSpecifier, "TMDB no longer has this tv show",
                                      TVDoc.ProviderType.TMDB, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.tv);

        CachedSeriesInfo m = GenerateTvShow(seriesSpecifier, downloadedSeries);

        if (save)
        {
            this.AddSeriesToCache(m);
        }

        return m;
    }

    private static CachedSeriesInfo GenerateTvShow(ISeriesSpecifier ss, TvShow downloadedSeries)
    {
        CachedSeriesInfo m = new(ss.TargetLocale, TVDoc.ProviderType.TMDB)
        {
            Imdb = downloadedSeries.ExternalIds?.ImdbId,
            TmdbCode = downloadedSeries.Id,
            TvdbCode = downloadedSeries.ExternalIds?.TvdbId.ToInt(ss.TvdbId) ?? -1,
            TvMazeCode = -1,
            Name = downloadedSeries.Name,
            Runtime = DecodeAverage(downloadedSeries.EpisodeRunTime),
            FirstAired = downloadedSeries.FirstAirDate,
            Genres = downloadedSeries.Genres.Select(genre => genre.Name).ToSafeList(),
            Overview = downloadedSeries.Overview,
            Network = downloadedSeries.Networks.Select(n => n.Name).ToPsv(),
            Status = MapStatus(downloadedSeries.Status),
            ShowLanguage = downloadedSeries.OriginalLanguage,
            SiteRating = (float)downloadedSeries.VoteAverage,
            SiteRatingVotes = downloadedSeries.VoteCount,
            PosterUrl = PosterImageUrl(downloadedSeries.PosterPath),
            SrvLastUpdated = TimeHelpers.UtcNow().Date.ToUnixTime(),
            TagLine = downloadedSeries.Tagline,
            TwitterId = downloadedSeries.ExternalIds?.TwitterId,
            InstagramId = downloadedSeries.ExternalIds?.InstagramId,
            FacebookId = downloadedSeries.ExternalIds?.InstagramId,
            FanartUrl = OriginalImageUrl(downloadedSeries.BackdropPath),
            ContentRating =
                GetCertification(downloadedSeries, ss.TargetLocale.RegionToUse(TVDoc.ProviderType.TMDB).Abbreviation) ??
                GetCertification(downloadedSeries, TVSettings.Instance.TMDBRegion.Abbreviation) ??
                GetCertification(downloadedSeries, Regions.Instance.FallbackRegion.Abbreviation) ?? string.Empty,
            OfficialUrl = downloadedSeries.Homepage,
            SeriesType = downloadedSeries.Type,
            SeasonOrderType = ss.SeasonOrder,
            TrailerUrl = GetYouTubeUrl(downloadedSeries),
            WebUrl = $"https://www.themoviedb.org/tv/{downloadedSeries.Id}",
            Popularity = downloadedSeries.Popularity,
            Country = downloadedSeries.OriginCountry.FirstOrDefault(),
            Dirty = false,
        };

        foreach (string? s in downloadedSeries.AlternativeTitles.Results.Select(title => title.Title))
        {
            m.AddAlias(s);
        }

        foreach (TMDbLib.Objects.TvShows.Cast? s in downloadedSeries.Credits.Cast)
        {
            m.AddActor(new Actor(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Character, s.Order));
        }

        foreach (TMDbLib.Objects.General.Crew? s in downloadedSeries.Credits.Crew)
        {
            m.AddCrew(new Crew(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Job, s.Department, s.CreditId));
        }

        AddShowImages(downloadedSeries, m);
        AddSeasons(ss, downloadedSeries, m);
        return m;
    }

    private static string? DecodeAverage(IReadOnlyCollection<int> times) =>
        times.Any()
            ? times.Average().ToString("F0", System.Globalization.CultureInfo.CurrentCulture)
            : null;

    /// <exception cref="GeneralHttpException">Condition.</exception>
    private static void AddSeasons(ISeriesSpecifier ss, TvShow downloadedSeries, CachedSeriesInfo m)
    {
        foreach (SearchTvSeason searchSeason in downloadedSeries.Seasons)
        {
            int snum = searchSeason.SeasonNumber;
            TvSeason? downloadedSeason = Client.GetTvSeasonAsync(downloadedSeries.Id, snum, TvSeasonMethods.Images,
                ss.LanguageToUse().Abbreviation).Result;

            string seasonUrl = m.WebUrl.HasValue()
                ? m.WebUrl + $"/season/{snum}"
                : string.Empty;

            Season newSeason = new(downloadedSeason.Id ?? 0, snum, downloadedSeason.Name, downloadedSeason.Overview,
                seasonUrl, downloadedSeason.PosterPath, downloadedSeries.Id);

            m.AddSeason(newSeason);

            foreach (TvSeasonEpisode? downloadedEpisode in downloadedSeason.Episodes)
            {
                Episode newEpisode = new(downloadedSeries.Id, m)
                {
                    Name = downloadedEpisode.Name,
                    Overview = downloadedEpisode.Overview,
                    FirstAired = downloadedEpisode.AirDate,
                    AiredEpNum = downloadedEpisode.EpisodeNumber,
                    AiredSeasonNumber = downloadedEpisode.SeasonNumber,
                    ProductionCode = downloadedEpisode.ProductionCode,
                    EpisodeId = downloadedEpisode.Id,
                    LinkUrl = $"https://www.themoviedb.org/tv/{downloadedSeries.Id}/season/{searchSeason.SeasonNumber}/episode/{downloadedEpisode.EpisodeNumber}",
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
                        .ToPsv(),
                    SrvLastUpdated = TimeHelpers.UtcNow().Date.ToUnixTime()
                };

                m.AddEpisode(newEpisode);
            }

            if (downloadedSeason.Images != null && downloadedSeason.Images.Posters.Any())
            {
                int imageId = snum * 1000;
                foreach (ImageData? image in downloadedSeason.Images.Posters)
                {
                    ShowImage newBanner = new()
                    {
                        Id = imageId++,
                        ImageUrl = OriginalImageUrl(image.FilePath),
                        ImageStyle = MediaImage.ImageType.poster,
                        Subject = MediaImage.ImageSubject.season,
                        SeasonNumber = snum,
                        Rating = image.VoteAverage,
                        RatingCount = image.VoteCount,
                        SeasonId = downloadedSeason.Id ?? 0,
                        LanguageCode = image.Iso_639_1,
                        SeriesSource = TVDoc.ProviderType.TMDB,
                    };

                    m.AddOrUpdateImage(newBanner);
                }
            }
        }
    }

    private static void AddShowImages(TvShow downloadedSeries, CachedSeriesInfo m)
    {
        int imageId = 1; //TODO See https://www.themoviedb.org/talk/60ba61a4cb9f4b006f30f82b for  why we need this
        if (downloadedSeries.Images.Backdrops.Any())
        {
            foreach (ImageData? image in downloadedSeries.Images.Backdrops)
            {
                ShowImage newBanner = new()
                {
                    SeriesSource = TVDoc.ProviderType.TMDB,
                    Id = imageId++,
                    ImageUrl = OriginalImageUrl(image.FilePath),
                    ImageStyle = MediaImage.ImageType.background,
                    Subject = MediaImage.ImageSubject.show,
                    Rating = image.VoteAverage,
                    RatingCount = image.VoteCount,
                    LanguageCode = image.Iso_639_1
                };

                m.AddOrUpdateImage(newBanner);
            }
        }

        if (downloadedSeries.Images.Posters.Any())
        {
            foreach (ImageData? image in downloadedSeries.Images.Posters)
            {
                ShowImage newBanner = new()
                {
                    SeriesSource = TVDoc.ProviderType.TMDB,
                    Id = imageId++,
                    ImageUrl = PosterImageUrl(image.FilePath),
                    ImageStyle = MediaImage.ImageType.poster,
                    Subject = MediaImage.ImageSubject.show,
                    Rating = image.VoteAverage,
                    RatingCount = image.VoteCount,
                    LanguageCode = image.Iso_639_1
                };

                m.AddOrUpdateImage(newBanner);
            }
        }
    }

    private static string MapStatus(string s)
    {
        if (s == "Returning Series")
        {
            return "Continuing";
        }

        return s;
    }

    /// <exception cref="GeneralHttpException">Condition.</exception>
    private static int? GetSeriesIdFromOtherCodes(ISeriesSpecifier ss)
    {
        if (ss.ImdbCode.HasValue())
        {
            FindContainer? x = Client.FindAsync(FindExternalSource.Imdb, ss.ImdbCode).Result;

            if (ss.Media == MediaConfiguration.MediaType.tv)
            {
                foreach (SearchTv? show in x.TvResults)
                {
                    return show.Id;
                }
            }
            else if (ss.Media == MediaConfiguration.MediaType.movie)
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

            if (ss.Media == MediaConfiguration.MediaType.tv)
            {
                foreach (SearchTv? show in x.TvResults)
                {
                    return show.Id;
                }
            }
            else if (ss.Media == MediaConfiguration.MediaType.movie)
            {
                foreach (SearchMovie? show in x.MovieResults)
                {
                    return show.Id;
                }
            }
        }

        return null;
    }

    private static string GetYouTubeUrl(Movie downloadedMovie)
        => GetYouTubeUrl(downloadedMovie.Videos);

    private static string GetYouTubeUrl(TvShow downloadedShow)
        => GetYouTubeUrl(downloadedShow.Videos);

    private static string GetYouTubeUrl(ResultContainer<Video>? downloadedVideos)
    {
        string? yid = downloadedVideos?.Results.Where(video => video.Type == "Trailer" && video.Site == "YouTube")
            .OrderByDescending(v => v.Size).Select(video => video.Key).FirstOrDefault();

        return yid.HasValue() ? $"https://www.youtube.com/watch?v={yid}" : string.Empty;
    }

    private static string? GetCertification(Movie downloadedMovie, string country)
    {
        return downloadedMovie.ReleaseDates?.Results
            .Where(rel => rel.Iso_3166_1 == country)
            .Select(rel => rel.ReleaseDates.First().Certification)
            .FirstOrDefault();
    }

    private static string? GetCertification(TvShow downloadedShow, string country)
    {
        return downloadedShow.ContentRatings?.Results
            .Where(rel => rel.Iso_3166_1 == country)
            .Select(rel => rel.Rating)
            .FirstOrDefault();
    }

    /// <exception cref="SourceConnectivityException">Condition.</exception>
    /// <exception cref="GeneralHttpException">Condition.</exception>
    public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type,
        Locale locale)
    {
        if (text.IsNumeric())
        {
            if (int.TryParse(text, out int textAsInt))
            {
                SearchSpecifier ss = new(-1, -1, textAsInt, locale, text,
                    TVDoc.ProviderType.TMDB, null, type);
                try
                {
                    switch (type)
                    {
                        case MediaConfiguration.MediaType.tv:
                            DownloadSeriesNow(ss);
                            break;

                        case MediaConfiguration.MediaType.movie:
                            DownloadMovieNow(ss);
                            break;
                    }
                }
                catch (MediaNotFoundException)
                {
                    //not really an issue so we can continue
                }
            }
        }

        HandleWebErrorsFor(() => SearchInternal(text, type, locale), "Error searching on TMDB:");
    }

    /// <exception cref="GeneralHttpException">Condition.</exception>
    /// <exception cref="AggregateException"></exception>
    private bool SearchInternal(string s, MediaConfiguration.MediaType mediaType, Locale locale1)
    {
        if (mediaType == MediaConfiguration.MediaType.movie)
        {
            SearchContainer<SearchMovie> results = Client
                .SearchMovieAsync(s, locale1.LanguageToUse(TVDoc.ProviderType.TMDB).Abbreviation).Result;

            LOGGER.Info(
                $"Got {results.Results.Count:N0} of {results.TotalResults:N0} results searching for {s}");

            foreach (SearchMovie result in results.Results)
            {
                CachedMovieInfo filedResult = File(result);
                LOGGER.Info($"   Movie: {filedResult.Name}:{filedResult.Id()}   {filedResult.Popularity}");
            }
        }
        else
        {
            SearchContainer<SearchTv>? results = Client.SearchTvShowAsync(s).Result;
            LOGGER.Info(
                $"Got {results.Results.Count:N0} of {results.TotalResults:N0} results searching for {s}");

            foreach (SearchTv result in results.Results)
            {
                CachedSeriesInfo filedResult = File(result);
                LOGGER.Info($"   TV Show: {filedResult.Name}:{filedResult.Id()}   {filedResult.Popularity}");
            }
        }

        return true;
    }

    private CachedSeriesInfo File(SearchTv result)
    {
        CachedSeriesInfo m = new(new Locale(), TVDoc.ProviderType.TMDB)
        {
            TmdbCode = result.Id,
            Name = result.Name,
            FirstAired = result.FirstAirDate,
            Overview = result.Overview,
            ShowLanguage = result.OriginalLanguage,
            SiteRating = (float)result.VoteAverage,
            SiteRatingVotes = result.VoteCount,
            PosterUrl = PosterImageUrl(result.PosterPath),
            SeasonOrderType = ProcessedSeason.SeasonType.aired,
            Popularity = result.Popularity,
            IsSearchResultOnly = true,
            Dirty = false,
            SrvLastUpdated = TimeHelpers.UtcNow().Date.ToUnixTime(),
            FanartUrl = OriginalImageUrl(result.BackdropPath),
            Country = result.OriginCountry.FirstOrDefault(),
        };

        this.AddSeriesToCache(m);
        return m;
    }

    private CachedMovieInfo File(SearchMovie result)
    {
        CachedMovieInfo m = new(new Locale(), TVDoc.ProviderType.TMDB)
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
        this.AddMovieToCache(m);
        return m;
    }

    // ReSharper disable once StringLiteralTypo
    private static string? PosterImageUrl(string? source) => ImageUrl(source, "w600_and_h900_bestv2");

    private static string? OriginalImageUrl(string? source) => ImageUrl(source, "original");

    private static string? ImageUrl(string? source, string type)
    {
        if (source.HasValue())
        {
            return "https://image.tmdb.org/t/p/" + type + source;
        }

        return null;
    }

    /// <exception cref="GeneralHttpException">Condition.</exception>
    /// <exception cref="SourceConnectivityException">Condition.</exception>
    /// <exception cref="AggregateException"></exception>
    /// <exception cref="MediaNotFoundException">Condition.</exception>
    public CachedMovieInfo? LookupMovieByImdb(string imdbToTest, Locale locale)
    {
        FindContainer? results = Client.FindAsync(FindExternalSource.Imdb, imdbToTest).Result;
        LOGGER.Info($"Got {results.MovieResults.Count:N0} results searching for {imdbToTest}");
        foreach (SearchMovie result in results.MovieResults)
        {
            SearchSpecifier ss = new(result.Id, locale, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie);
            DownloadMovieNow(ss);
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

    private static int? LookupTvdbIdByImdb(string imdbToTest)
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

    public Dictionary<int, CachedMovieInfo> GetMovieIdsFromCollection(int collectionId, string languageCode)
    {
        Dictionary<int, CachedMovieInfo> returnValue = new();
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

    /// <exception cref="SourceConnectivityException">Condition.</exception>
    /// <exception cref="GeneralHttpException">Condition.</exception>
    public CachedMovieInfo? LookupMovieByTvdb(int tvdbId, Locale locale)
    {
        try
        {
            FindContainer? results = Client.FindAsync(FindExternalSource.TvDb, tvdbId.ToString()).Result;
            LOGGER.Info($"Got {results.MovieResults.Count:N0} results searching for {tvdbId}");
            foreach (SearchMovie result in results.MovieResults)
            {
                SearchSpecifier ss = new(result.Id, locale, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie);
                try
                {
                    DownloadMovieNow(ss);
                }
                catch (MediaNotFoundException ex)
                {
                    // Don't worry as others can be loaded
                    LOGGER.Error(ex);
                }
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
        }
        catch (AggregateException ex)
        {
            LOGGER.Error(ex);
            return null;
        }

        return null;
    }

    internal IEnumerable<CachedSeriesInfo> ServerTvAccuracyCheck()
    {
        TmdbAccuracyCheck check = new(this);

        Say($"TMDB Accuracy Check (TV) running for {FullShows().Count} shows.");

        try
        {
            Parallel.ForEach(FullShows(), new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, si =>
            {
                Thread.CurrentThread.Name ??= $"TMDB Consistency Check: {si.Name}"; // Can only set it once
                check.ServerAccuracyCheck(si);
            });
        }
        catch (OperationCanceledException ex)
        {
            LOGGER.Error(ex);
        }
        catch (AggregateException ex)
        {
            LOGGER.Error(ex);
        }

        foreach (string issue in check.Issues)
        {
            LOGGER.Warn(issue);
        }

        SayNothing();
        return check.ShowsToUpdate;
    }
    internal IEnumerable<CachedMovieInfo> ServerMovieAccuracyCheck()
    {
        TmdbAccuracyCheck check = new(this);

        Say($"TmDB Accuracy Check (Movies) running {FullMovies().Count} shows.");

        try
        {
            Parallel.ForEach(FullMovies(), new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, si =>
            {
                Thread.CurrentThread.Name ??= $"TMDB Consistency Check: {si.Name}"; // Can only set it once
                check.ServerAccuracyCheck(si);
            });
        }
        catch (OperationCanceledException ex)
        {
            LOGGER.Error(ex);
        }
        catch (AggregateException ex)
        {
            LOGGER.Error(ex);
        }

        foreach (string issue in check.Issues)
        {
            LOGGER.Warn(issue);
        }

        SayNothing();
        return check.MoviesToUpdate;
    }

    /// <exception cref="SourceConnectivityException">Condition.</exception>
    /// <exception cref="GeneralHttpException">Condition.</exception>
    public async Task<Recomendations> GetRecommendationsAsync(BackgroundWorker sender, List<ShowConfiguration> shows, string languageCode)
    {
        int total = shows.Count;
        int current = 0;
        Recomendations returnValue = await GetTrendingAsync(languageCode).ConfigureAwait(false);

        foreach (ShowConfiguration? arg in shows)
        {
            string errorMessage = $"Error obtaining TMDB Recommendations for {arg.Name}:";
            try
            {
                AddRecommendationsFrom(arg, returnValue, languageCode);

                sender.ReportProgress(100 * current++ / total, arg.CachedShow?.Name);
            }
            catch (AggregateException aex) when (aex.InnerException is HttpRequestException ex)
            {
                LOGGER.LogHttpRequestException(errorMessage, ex);
                SayNothing();
                LastErrorMessage = ex.LoggableDetails();
                // ReSharper disable once ThrowFromCatchWithNoInnerException
                throw new SourceConnectivityException(errorMessage, ex);
            }
            catch (Exception e)
            {
                LOGGER.Error(e, errorMessage);
                SayNothing();
                LastErrorMessage = e.Message;
                throw new SourceConnectivityException(errorMessage, e);
            }
        }

        return returnValue;
    }

    /// <exception cref="GeneralHttpException">Condition.</exception>
    private async Task<Recomendations> GetTrendingAsync(string languageCode)
    {
        Task<SearchContainer<SearchTv>> topRated = Client.GetTvShowTopRatedAsync(language: languageCode);
        Task<SearchContainer<SearchTv>> trending = Client.GetTrendingTvAsync(TimeWindow.Week);
        Recomendations returnValue = new();
        
        await topRated.ConfigureAwait(false);
        foreach (SearchTv? top in topRated.Result.Results)
        {
            File(top);
            returnValue.AddTopRated(top.Id);
        }

        await trending.ConfigureAwait(false);
        foreach (SearchTv? top in trending.Result.Results)
        {
            File(top);
            returnValue.AddTrending(top.Id);
        }

        return returnValue;
    }

    /// <exception cref="GeneralHttpException">Condition.</exception>
    private void AddRecommendationsFrom(ShowConfiguration arg, Recomendations returnValue, string languageCode)
    {
        if (arg.TmdbCode == 0)
        {
            string? imdb = arg.CachedShow?.Imdb;
            if (!imdb.HasValue())
            {
                return;
            }

            int? tmdbCode = LookupTvdbIdByImdb(imdb);
            if (!tmdbCode.HasValue)
            {
                return;
            }

            arg.TmdbCode = tmdbCode.Value;
        }

        Task<SearchContainer<SearchTv>?> related = Client.GetTvShowRecommendationsAsync(arg.TmdbCode, languageCode);
        Task<SearchContainer<SearchTv>?> similar = Client.GetTvShowSimilarAsync(arg.TmdbCode, languageCode);

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

    /// <exception cref="SourceConnectivityException">Condition.</exception>
    /// <exception cref="GeneralHttpException">Condition.</exception>
    public async Task<Recomendations> GetRecommendationsAsync(BackgroundWorker sender, List<MovieConfiguration> movies, string languageCode)
    {
        int total = movies.Count;
        int current = 0;
        Recomendations returnValue = new();

        Task<SearchContainer<SearchMovie>> topRated = Client.GetMovieTopRatedListAsync(languageCode);
        Task<SearchContainer<SearchMovie>> trending = Client.GetTrendingMoviesAsync(TimeWindow.Week);
        await topRated.ConfigureAwait(false);
        await trending.ConfigureAwait(false);

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

        foreach (MovieConfiguration? movie in movies)
        {
            string errorMessage = $"Error obtaining TMDB Recommendations for {movie.Name}";

            HandleWebErrorsFor(() => GetMovieRecommendations(languageCode, movie, returnValue), errorMessage);
            
            sender.ReportProgress(100 * current++ / total, movie.CachedMovie?.Name);
        }
        return returnValue;
    }

    /// <exception cref="GeneralHttpException">Condition.</exception>
    private bool GetMovieRecommendations(string languageCode, MovieConfiguration movie, Recomendations returnValue)
    {
        Task<SearchContainer<SearchMovie>?> related = Client.GetMovieRecommendationsAsync(movie.TmdbCode, languageCode);
        Task<SearchContainer<SearchMovie>?> similar = Client.GetMovieSimilarAsync(movie.TmdbCode, languageCode);

        Task.WaitAll(related, similar);
        if (related.Result != null)
        {
            foreach (SearchMovie? relatedMovie in related.Result.Results)
            {
                File(relatedMovie);
                returnValue.AddRelated(relatedMovie.Id, movie);
            }
        }

        if (similar.Result != null)
        {
            foreach (SearchMovie? similarMovie in similar.Result.Results)
            {
                File(similarMovie);
                returnValue.AddSimilar(similarMovie.Id, movie);
            }
        }

        return true;
    }

    public override void ReConnect(bool b)
    {
        //nothing to be done here
    }

    public TVDoc.ProviderType SourceProvider() => TVDoc.ProviderType.TMDB;
}
