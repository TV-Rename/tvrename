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
        private const string KEY = "2dcfd2d08f80439d7ef5210f217b80b4";
        private static readonly TMDbClient Client = new TMDbClient(KEY);
        public static string EpisodeGuideUrl(ShowConfiguration selectedShow)
        {
            return $"http://api.themoviedb.org/3/tv/{selectedShow.TmdbId}?api_key={KEY}&language={selectedShow.LanguageToUse().Abbreviation}";
        }

        private UpdateTimeTracker latestUpdateTime;

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

            LoadOk = loadFrom is null || CachePersistor.LoadMovieCache(loadFrom, this) && CachePersistor.LoadTvCache(loadFrom, this);
        }

        public bool Connect(bool showErrorMsgBox) => true;

        public void SaveCache()
        {
            lock (MOVIE_LOCK)
            {
                lock (SERIES_LOCK)
                {
                    CachePersistor.SaveCache(Series, Movies, CacheFile, latestUpdateTime.LastSuccessfulServerUpdateTimecode());
                }
            }
        }

        public override bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
        {
            if (s.Provider != TVDoc.ProviderType.TMDB)
            {
                throw new SourceConsistencyException($"Asked to update {s.Name} from TMDB, but the Id is not for TMDB.", TVDoc.ProviderType.TMDB);
            }

            if (s.Media == MediaConfiguration.MediaType.movie)
            {
                return EnsureMovieUpdated(s, showErrorMsgBox);
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

                this.AddSeriesToCache(downloadedSi);
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

        private bool EnsureMovieUpdated(ISeriesSpecifier id, bool showErrorMsgBox)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id.TmdbId) && !Movies[id.TmdbId].Dirty)
                {
                    return true;
                }
            }

            Say($"{id.Name} from TMDB");
            try
            {
                CachedMovieInfo downloadedSi = DownloadMovieNow(id, showErrorMsgBox);

                if (downloadedSi.TmdbCode != id.TmdbId && id.TmdbId == -1)
                {
                    lock (MOVIE_LOCK)
                    {
                        Movies.TryRemove(-1, out _);
                    }
                }

                this.AddMovieToCache(downloadedSi);
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

        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, IEnumerable<ISeriesSpecifier> ss)
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
                    latestUpdateTime.RegisterServerUpdate(DateTime.Now.ToUnixTime());
                    return true;
                }

                latestUpdateTime.RegisterServerUpdate(DateTime.Now.ToUnixTime());

                List<int> movieUpdates = Client.GetChangesMovies(cts, latestUpdateTime).Select(item => item.Id).Distinct().ToList();
                Say($"Processing {movieUpdates.Count} movie updates from TMDB. From between {latestUpdateTime.LastSuccessfulServerUpdateDateTime()} and {latestUpdateTime.ProposedServerUpdateDateTime()}");
                lock (MOVIE_LOCK)
                {
                    foreach (int id in movieUpdates)
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
                    Say($"Identified {Movies.Values.Count(info => info.Dirty && !info.IsSearchResultOnly)} TMDB Movies need updating");
                }

                List<int> showUpdates = Client.GetChangesShows(cts, latestUpdateTime).Select(item => item.Id).Distinct().ToList();
                Say($"Processing {showUpdates.Count} show updates from TMDB. From between {latestUpdateTime.LastSuccessfulServerUpdateDateTime()} and {latestUpdateTime.ProposedServerUpdateDateTime()}");
                lock (SERIES_LOCK)
                {
                    foreach (int id in showUpdates)
                    {
                        if (!cts.IsCancellationRequested)
                        {
                            if (HasSeries(id))
                            {
                                CachedSeriesInfo? x = GetSeries(id);
                                if (!(x is null))
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
                    Say($"Identified {Series.Values.Count(info => info.Dirty && !info.IsSearchResultOnly)} TMDB Shows need updating");
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

        public void UpdatesDoneOk()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            latestUpdateTime.RecordSuccessfulUpdate();
        }

        public CachedMovieInfo? GetMovie(PossibleNewMovie show, Locale preferredLocale, bool showErrorMsgBox) => this.GetMovie(show.RefinedHint, show.PossibleYear, preferredLocale, showErrorMsgBox, false);

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

        public void LatestUpdateTimeIs(string time)
        {
            latestUpdateTime.Load(time);
            LOGGER.Info($"Loaded file with updates until {latestUpdateTime.LastSuccessfulServerUpdateDateTime()}");
        }

        public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TMDB;

        public CachedMovieInfo GetMovieAndDownload(ISeriesSpecifier id, bool showErrorMsgBox) => HasMovie(id.TmdbId)
            ? CachedMovieData[id.TmdbId]
            : DownloadMovieNow(id, showErrorMsgBox);

        internal CachedMovieInfo DownloadMovieNow(ISeriesSpecifier id, bool showErrorMsgBox,bool saveToCache = true)
        {
            string imageLanguage = $"{id.LanguageToUse().Abbreviation},null";
            Movie downloadedMovie = Client.GetMovieAsync(id.TmdbId, id.LanguageToUse().Abbreviation, imageLanguage, MovieMethods.ExternalIds | MovieMethods.Images | MovieMethods.AlternativeTitles | MovieMethods.ReleaseDates | MovieMethods.Changes | MovieMethods.Videos | MovieMethods.Credits).Result;
            if (downloadedMovie is null)
            {
                throw new MediaNotFoundException(id, "TMDB no longer has this movie", TVDoc.ProviderType.TMDB, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie);
            }
            CachedMovieInfo m = new CachedMovieInfo(id.TargetLocale, TVDoc.ProviderType.TMDB)
            {
                Imdb = downloadedMovie.ExternalIds.ImdbId,
                TmdbCode = downloadedMovie.Id,
                Name = downloadedMovie.Title,
                Runtime = downloadedMovie.Runtime.ToString(),
                FirstAired = GetReleaseDateDetail(downloadedMovie, id.RegionToUse().Abbreviation) ?? GetReleaseDateDetail(downloadedMovie, TVSettings.Instance.TMDBRegion.Abbreviation) ?? downloadedMovie.ReleaseDate,
                Genres = downloadedMovie.Genres.Select(genre => genre.Name).ToList(),
                Overview = downloadedMovie.Overview,
                Network = downloadedMovie.ProductionCompanies.Select(y=>y.Name).ToPsv(),
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
                ContentRating = GetCertification(downloadedMovie, id.RegionToUse().Abbreviation) ?? GetCertification(downloadedMovie, TVSettings.Instance.TMDBRegion.Abbreviation) ?? GetCertification(downloadedMovie, Regions.Instance.FallbackRegion.Abbreviation) ?? string.Empty,
                OfficialUrl = downloadedMovie.Homepage,
                TrailerUrl = GetYouTubeUrl(downloadedMovie),
                Dirty = false,
                Country = downloadedMovie.ProductionCountries.FirstOrDefault()?.Name,
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
            AddMovieImages(downloadedMovie,m);

            if (saveToCache)
            {
                this.AddMovieToCache(m);
            }

            return m;
        }

        private void AddMovieImages(Movie downloadedMovie, CachedMovieInfo m)
        {
            int imageId = 1; //TODO See https://www.themoviedb.org/talk/60ba61a4cb9f4b006f30f82b for  why we need this
            if (downloadedMovie.Images.Backdrops.Any())
            {
                foreach (ImageData? image in downloadedMovie.Images.Backdrops)
                {
                    MovieImage newBanner = new MovieImage 
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

            if (downloadedMovie.Images.Posters.Any())
            {
                foreach (ImageData? image in downloadedMovie.Images.Posters)
                {
                    MovieImage newBanner = new MovieImage
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

        private DateTime? GetReleaseDateDetail(Movie downloadedMovie, string? country)
        {
            List<DateTime> dates = downloadedMovie.ReleaseDates?.Results
                .Where(rel => rel.Iso_3166_1.Equals(country,StringComparison.OrdinalIgnoreCase))
                .SelectMany(rel => rel.ReleaseDates)
                .Select(d => d.ReleaseDate)
                .OrderBy(time => time).ToList();

            if (dates?.Any() ?? false)
            {
                return dates.First();
            }

            return null;
        }

        internal CachedSeriesInfo DownloadSeriesNow(ISeriesSpecifier ss, bool showErrorMsgBox, bool saveToCache = true)
        {
            int id = ss.TmdbId > 0 ? ss.TmdbId : GetSeriesIdFromOtherCodes(ss) ?? 0;

            string imageLanguage = $"{ss.LanguageToUse().Abbreviation},null";
            TvShow? downloadedSeries = Client.GetTvShowAsync(id, TvShowMethods.ExternalIds | TvShowMethods.Images | TvShowMethods.AlternativeTitles | TvShowMethods.ContentRatings | TvShowMethods.Changes | TvShowMethods.Videos | TvShowMethods.Credits, ss.LanguageToUse().Abbreviation,imageLanguage).Result;
            if (downloadedSeries is null)
            {
                throw new MediaNotFoundException(ss, "TMDB no longer has this show", TVDoc.ProviderType.TMDB, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.tv);
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
                Network = downloadedSeries.Networks.Select(n=>n.Name).ToPsv(),
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
                FanartUrl = OriginalImageUrl(downloadedSeries.BackdropPath),
                ContentRating = GetCertification(downloadedSeries, ss.TargetLocale.RegionToUse(TVDoc.ProviderType.TMDB).Abbreviation) ?? GetCertification(downloadedSeries, TVSettings.Instance.TMDBRegion.Abbreviation) ?? GetCertification(downloadedSeries, Regions.Instance.FallbackRegion.Abbreviation) ?? string.Empty,
                OfficialUrl = downloadedSeries.Homepage,
                Type = downloadedSeries.Type,
                TrailerUrl = GetYouTubeUrl(downloadedSeries),
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
                m.AddActor(new Actor(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Character, 0, s.Order));
            }
            foreach (TMDbLib.Objects.General.Crew? s in downloadedSeries.Credits.Crew)
            {
                m.AddCrew(new Crew(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Job, s.Department, s.CreditId));
            }
            AddShowImages(downloadedSeries, m);
            AddSeasons(ss, downloadedSeries, m);

            if (saveToCache)
            {
                this.AddSeriesToCache(m);
            }

            return m;
        }

        private static void AddSeasons(ISeriesSpecifier ss, TvShow downloadedSeries, CachedSeriesInfo m)
        {
            foreach (var searchSeason in downloadedSeries.Seasons)
            {
                int snum = searchSeason.SeasonNumber;
                TvSeason? downloadedSeason = Client.GetTvSeasonAsync(downloadedSeries.Id, snum, TvSeasonMethods.Images,
                    ss.LanguageToUse().Abbreviation).Result;

                Season newSeason = new Season(downloadedSeason.Id ?? 0, snum, downloadedSeason.Name, downloadedSeason.Overview,
                    string.Empty, downloadedSeason.PosterPath, downloadedSeries.Id);

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
                            .ToPsv()
                    };

                    m.AddEpisode(newEpisode);
                }

                if (downloadedSeason.Images != null && downloadedSeason.Images.Posters.Count > 0)
                {
                    int ImageId = snum * 1000;
                    foreach (ImageData? image in downloadedSeason.Images.Posters)
                    {
                        ShowImage newBanner = new ShowImage
                        {
                            Id = ImageId++,
                            ImageUrl = OriginalImageUrl(image.FilePath),
                            ImageStyle = MediaImage.ImageType.poster,
                            Subject =  MediaImage.ImageSubject.season,
                            SeasonNumber = snum,
                            Rating = image.VoteAverage,
                            RatingCount = image.VoteCount,
                            SeasonId = downloadedSeason.Id ?? 0,
                            LanguageCode = image.Iso_639_1,
                            SeriesId = downloadedSeries.Id,
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
                    ShowImage newBanner = new ShowImage
                    {
                        SeriesId = downloadedSeries.Id,
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
                    ShowImage newBanner = new ShowImage
                    {
                        SeriesId = downloadedSeries.Id,
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

        private int? GetSeriesIdFromOtherCodes(ISeriesSpecifier ss)
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
            bool isNumber = System.Text.RegularExpressions.Regex.Match(text, "^[0-9]+$").Success;

            if (isNumber)
            {
                if (int.TryParse(text, out int textAsInt))
                {
                    SearchSpecifier ss = new SearchSpecifier(-1, -1, textAsInt, locale, text,
                        TVDoc.ProviderType.TMDB, null, type);
                    try
                    {
                        switch (type)
                        {
                            case MediaConfiguration.MediaType.tv:
                                DownloadSeriesNow(ss, showErrorMsgBox);
                                break;

                            case MediaConfiguration.MediaType.movie:
                                DownloadMovieNow(ss, showErrorMsgBox);
                                break;
                        }
                    }
                    catch (MediaNotFoundException)
                    {
                        //not really an issue so we can continue
                    }
                }
            }

            if (type == MediaConfiguration.MediaType.movie)
            {
                SearchContainer<SearchMovie> results = Client.SearchMovieAsync(text, locale.LanguageToUse(TVDoc.ProviderType.TMDB).Abbreviation).Result;
                LOGGER.Info(
                    $"Got {results.Results.Count:N0} of {results.TotalResults:N0} results searching for {text}");

                foreach (SearchMovie result in results.Results)
                {
                    //TODO - Reconside this as it's really slow.
                    LOGGER.Info($"   Movie: {result.Title}:{result.Id}   {result.Popularity}");
                    File(result);
                    try
                    {
                        ISeriesSpecifier ss = new SearchSpecifier(-1, -1, result.Id, locale, result.Title,
                            TVDoc.ProviderType.TMDB, null, MediaConfiguration.MediaType.movie);
                        DownloadMovieNow(ss, showErrorMsgBox);
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
                        ISeriesSpecifier ss = new SearchSpecifier(-1, -1, result.Id, locale, result.Name,
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

        // ReSharper disable once UnusedMethodReturnValue.Local
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
                FanartUrl = OriginalImageUrl(result.BackdropPath),
            };

            this.AddSeriesToCache(m);
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

            this.AddMovieToCache(m);
            return m;
        }

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
                SearchSpecifier ss = new SearchSpecifier(result.Id, locale, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie);
                DownloadMovieNow(ss, showErrorMsgBox);
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

        public CachedMovieInfo? LookupMovieByTvdb(int tvdbId, bool showErrorMsgBox,Locale locale)
        {
            FindContainer? results = Client.FindAsync(FindExternalSource.TvDb, tvdbId.ToString()).Result;
            LOGGER.Info($"Got {results.MovieResults.Count:N0} results searching for {tvdbId}");
            foreach (SearchMovie result in results.MovieResults)
            {
                SearchSpecifier ss = new SearchSpecifier(result.Id, locale, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie);
                DownloadMovieNow(ss, showErrorMsgBox);
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

        internal IEnumerable<CachedSeriesInfo> ServerTvAccuracyCheck()
        {
            TmdbAccuracyCheck check = new TmdbAccuracyCheck(this);

            Say($"TMDB Accuracy Check (TV) running for {FullShows().Count} shows.");

            Parallel.ForEach(FullShows(), si => {
                Thread.CurrentThread.Name ??= $"TMDB Consistency Check: {si.Name}"; // Can only set it once
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
            TmdbAccuracyCheck check = new TmdbAccuracyCheck(this);

            Say($"TmDB Accuracy Check (Movies) running {FullMovies().Count} shows.");

            Parallel.ForEach(FullMovies(), si => {
                Thread.CurrentThread.Name ??= $"TMDB Consistency Check: {si.Name}"; // Can only set it once
                check.ServerAccuracyCheck(si);
            });

            foreach (string issue in check.Issues)
            {
                LOGGER.Warn(issue);
            }

            SayNothing();
            return check.MoviesToUpdate;
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
                    if (related.Result != null)
                    {
                        foreach (SearchMovie? movie in related.Result.Results)
                        {
                            File(movie);
                            returnValue.AddRelated(movie.Id, arg);
                        }
                    }

                    if (similar.Result != null)
                    {
                        foreach (SearchMovie? movie in similar.Result.Results)
                        {
                            File(movie);
                            returnValue.AddSimilar(movie.Id, arg);
                        }
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
