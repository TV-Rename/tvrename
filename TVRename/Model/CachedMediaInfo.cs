using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public abstract class CachedMediaInfo : ISeriesSpecifier
    {
        public string Name;
        public string? Overview;
        public string? Runtime;
        public string? ContentRating;
        public float SiteRating;
        public int SiteRatingVotes;
        public string? Imdb;
        public int TvdbCode;
        public int TvMazeCode;
        public int TvRageCode;
        public int TmdbCode;
        public string? WebUrl;
        public string? OfficialUrl;
        public string? TrailerUrl;
        public string? ShowLanguage;
        public string? PosterUrl;
        public string? TwitterId;
        public string? InstagramId;
        public string? FacebookId;
        public string? TagLine;
        public string? Country;
        public string? SeriesId;
        public string? Slug;
        public double? Popularity;
        public DateTime? FirstAired;
        public Locale? ActualLocale; //The actual language obtained
        protected string? BannerString;
        public string? Network;
        public string? FanartUrl;

        public IEnumerable<string> Networks => Network!.FromPsv();

        public string? Status { get; set; }
        public bool IsSearchResultOnly; // set to true if local info is known to be just certain fields found from search results. Do not need to be saved

        public SafeList<Actor> Actors;
        public SafeList<Crew> Crew;
        public SafeList<string> Genres;
        protected SafeList<string> Aliases;

        public bool Dirty; // set to true if local info is known to be older than whats on the server
        public long SrvLastUpdated;

        private protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
        protected internal readonly TVDoc.ProviderType Source;

        protected CachedMediaInfo(Locale locale, TVDoc.ProviderType source) : this(source)
        {
            ActualLocale = locale;
        }

        protected CachedMediaInfo(int tvdb, int tvmaze, int tmdbId, Locale locale, TVDoc.ProviderType source) : this(locale, source)
        {
            IsSearchResultOnly = false;
            TvMazeCode = tvmaze;
            TvdbCode = tvdb;
            TmdbCode = tmdbId;
        }

        protected CachedMediaInfo(TVDoc.ProviderType source)
        {
            Actors = new SafeList<Actor>();
            Crew = new SafeList<Crew>();
            Aliases = new SafeList<string>();
            Genres = new SafeList<string>();

            Dirty = false;
            Name = string.Empty;

            TvdbCode = -1;
            TvMazeCode = -1;
            TvRageCode = 0;
            TmdbCode = -1;

            Status = "Unknown";
            Source = source;
        }

        protected abstract MediaConfiguration.MediaType MediaType();

        public int IdCode(TVDoc.ProviderType selectedSource)
        {
            return selectedSource switch
            {
                TVDoc.ProviderType.libraryDefault => IdCode(MediaType() == MediaConfiguration.MediaType.movie
                    ? TVSettings.Instance.DefaultMovieProvider
                    : TVSettings.Instance.DefaultProvider),
                TVDoc.ProviderType.TVmaze => TvMazeCode,
                TVDoc.ProviderType.TheTVDB => TvdbCode,
                TVDoc.ProviderType.TMDB => TmdbCode,
                _ => throw new ArgumentOutOfRangeException(nameof(Source), selectedSource, null)
            };
        }

        private static Locale GetLocale(int? languageId, string? regionCode)
        {
            bool validLanguage = languageId.HasValue && Languages.Instance.GetLanguageFromId(languageId.Value) != null;
            bool validRegion = regionCode.HasValue() && Regions.Instance.RegionFromCode(regionCode!) != null;

            if (validLanguage && validRegion)
            {
                return new Locale(Regions.Instance.RegionFromCode(regionCode)!, Languages.Instance.GetLanguageFromId(languageId.Value)!);
            }

            if (validLanguage)
            {
                return new Locale(Languages.Instance.GetLanguageFromId(languageId.Value)!);
            }

            if (validRegion)
            {
                return new Locale(Regions.Instance.RegionFromCode(regionCode)!);
            }

            return new Locale();
        }

        public IEnumerable<string> GetAliases() => Aliases;

        public IEnumerable<Actor> GetActors() => Actors;

        public IEnumerable<string> GetActorNames() => GetActors().Select(x => x.ActorName);

        public void ClearActors()
        {
            Actors = new SafeList<Actor>();
        }

        public void AddActor(Actor actor)
        {
            Actors.Add(actor);
        }

        public IEnumerable<Crew> GetCrew() => Crew;

        public IEnumerable<string> GetCrewNames() => GetCrew().Select(x => x.Name);

        public void ClearCrew()
        {
            Crew = new SafeList<Crew>();
        }

        public void AddCrew(Crew crew)
        {
            Crew.Add(crew);
        }

        private static float GetSiteRating(XElement seriesXml)
        {
            string siteRatingString = seriesXml.ExtractStringOrNull("siteRating") ?? seriesXml.ExtractString("SiteRating");
            float.TryParse(siteRatingString,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                CultureInfo.CreateSpecificCulture("en-US"), out float x);

            return x;
        }

        protected string GenerateErrorMessage() => "Error processing data for a show. " + this + "\r\nLanguage: \"" + ActualLocale?.PreferredLanguage?.EnglishName + "\"";

        private void LoadActors(XElement seriesXml)
        {
            ClearActors();
            foreach (Actor a in seriesXml.Descendants("Actors").Descendants("Actor").Select(actorXml => new Actor(actorXml)))
            {
                AddActor(a);
            }
        }

        private void LoadCrew(XElement seriesXml)
        {
            ClearCrew();
            foreach (Crew c in seriesXml.Descendants("Crew").Descendants("CrewMember").Select(crewXml => new Crew(crewXml)))
            {
                AddCrew(c);
            }
        }

        private void LoadAliases(XElement seriesXml)
        {
            Aliases = new SafeList<string>();
            foreach (XElement aliasXml in seriesXml.Descendants("Aliases").Descendants("Alias"))
            {
                Aliases.Add(aliasXml.Value);
            }
        }

        private void LoadGenres(XElement seriesXml)
        {
            Genres = seriesXml
                .Descendants("Genres")
                .Descendants("Genre")
                .Select(g => g.Value.Trim()).Distinct()
                .ToSafeList();
        }

        public string GetImdbNumber() =>
            Imdb is null ? string.Empty
            : Imdb.StartsWith("tt", StringComparison.Ordinal) ? Imdb.RemoveFirst(2)
            : Imdb;

        public void AddAlias(string? s)
        {
            if (s.HasValue())
            {
                Aliases.Add(s);
            }
        }

        public override string ToString() => $"TMDB:{TmdbCode}/TVDB:{TvdbCode}/Maze:{TvMazeCode}/{Name}";

        public void UpgradeSearchResultToDirty()
        {
            if (IsSearchResultOnly)
            {
                Dirty = true;
                IsSearchResultOnly = false;
            }
        }

        protected static string ChooseBetter(string? encumbant, bool betterLanguage, string? newValue)
        {
            if (string.IsNullOrEmpty(encumbant))
            {
                return newValue?.Trim() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(newValue))
            {
                return encumbant.Trim();
            }

            return betterLanguage ? newValue.Trim() : encumbant.Trim();
        }

        private static string ChooseBetterStatus(string? encumbant, bool betterLanguage, string? newValue)
        {
            if (string.IsNullOrEmpty(encumbant) || encumbant.Equals("Unknown"))
            {
                return newValue?.Trim() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(newValue) || newValue.Equals("Unknown"))
            {
                return encumbant.Trim();
            }

            return betterLanguage ? newValue.Trim() : encumbant.Trim();
        }

        TVDoc.ProviderType ISeriesSpecifier.Provider => Source;

        public int TvdbId => TvdbCode;

        string ISeriesSpecifier.Name => Name;

        public MediaConfiguration.MediaType Media => MediaType();

        public int TvMazeId => TvMazeCode;

        public int TmdbId => TmdbCode;

        public string? ImdbCode => Imdb;

        public Locale TargetLocale => ActualLocale ?? new Locale();
        public abstract ProcessedSeason.SeasonType SeasonOrder { get; }

        public void UpdateId(int id, TVDoc.ProviderType source)
        {
            switch (source)
            {
                case TVDoc.ProviderType.TVmaze:
                    TvMazeCode = id;
                    break;

                case TVDoc.ProviderType.TheTVDB:
                    TvdbCode = id;
                    break;

                case TVDoc.ProviderType.TMDB:
                    TmdbCode = id;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        protected void WriteCommonFields(XmlWriter writer)
        {
            writer.WriteElement("id", TvdbCode);
            writer.WriteElement("mazeid", TvMazeCode);
            writer.WriteElement("TMDBCode", TmdbCode);
            writer.WriteElement("SeriesName", Name, true);
            writer.WriteElement("lastupdated", SrvLastUpdated);
            writer.WriteElement("LanguageId", ActualLocale?.PreferredLanguage?.TvdbId);
            writer.WriteElement("RegionCode", ActualLocale?.PreferredRegion?.Abbreviation);
            writer.WriteElement("TwitterId", TwitterId, true);
            writer.WriteElement("InstagramId", InstagramId, true);
            writer.WriteElement("FacebookId", FacebookId, true);
            writer.WriteElement("TagLine", TagLine, true);
            writer.WriteElement("Country", Country, true);
            writer.WriteElement("posterURL", PosterUrl);
            writer.WriteElement("FanartUrl", FanartUrl);
            writer.WriteElement("banner", BannerString, true);
            writer.WriteElement("TrailerUrl", TrailerUrl, true);
            writer.WriteElement("WebURL", WebUrl, true);
            writer.WriteElement("OfficialUrl", OfficialUrl, true);
            writer.WriteElement("ShowLanguage", ShowLanguage);
            writer.WriteElement("imdbId", Imdb, true);
            writer.WriteElement("rageid", TvRageCode, true);
            writer.WriteElement("network", Network, true);
            writer.WriteElement("overview", Overview, true);
            writer.WriteElement("rating", ContentRating);
            writer.WriteElement("runtime", Runtime, true);
            writer.WriteElement("seriesId", SeriesId, true);
            writer.WriteElement("status", Status);
            writer.WriteElement("siteRating", SiteRating, "0.##",CultureInfo.CreateSpecificCulture("en-US"));
            writer.WriteElement("siteRatingCount", SiteRatingVotes);
            writer.WriteElement("slug", Slug,true);
            writer.WriteElement("Popularity", Popularity, "0.##", CultureInfo.CreateSpecificCulture("en-US"));

            if (FirstAired != null && FirstAired > DateTime.Parse("1000-01-01"))
            {
                writer.WriteElement("FirstAired", FirstAired.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }

            writer.WriteStartElement("Actors");
            foreach (Actor aa in GetActors())
            {
                aa.WriteXml(writer);
            }
            writer.WriteEndElement(); //Actors
            writer.WriteStartElement("Crew");
            foreach (Crew aa in Crew)
            {
                aa.WriteXml(writer);
            }
            writer.WriteEndElement(); //Crew
            writer.WriteStartElement("Aliases");
            foreach (string a in Aliases)
            {
                writer.WriteElement("Alias", a);
            }
            writer.WriteEndElement(); //Aliases

            writer.WriteStartElement("Genres");
            foreach (string a in Genres)
            {
                writer.WriteElement("Genre", a);
            }
            writer.WriteEndElement(); //Genres
        }

        protected void LoadCommonXml(XElement seriesXml)
        {
            //<Data>
            // <Series>
            //  <id>...</id>
            //  etc.
            // </Series>
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            // ...
            //</Data>
            try
            {
                TvdbCode = seriesXml.ExtractInt("id") ?? throw new SourceConsistencyException("Error Extracting Id for Series", TVDoc.ProviderType.TheTVDB);
                TvMazeCode = seriesXml.ExtractInt("mazeid") ?? -1;
                TmdbCode = seriesXml.ExtractInt("TMDBCode") ?? -1;
                Name = System.Web.HttpUtility.HtmlDecode(
                    XmlHelper.ReadStringFixQuotesAndSpaces(seriesXml.ExtractStringOrNull("SeriesName") ?? seriesXml.ExtractString("seriesName")));
                SrvLastUpdated = seriesXml.ExtractLong("lastupdated") ?? seriesXml.ExtractLong("lastUpdated", 0);
                int? languageId = seriesXml.ExtractInt("LanguageId") ?? seriesXml.ExtractInt("languageId");
                string regionCode = seriesXml.ExtractString("RegionCode");
                ActualLocale = GetLocale(languageId, regionCode);
                Popularity = seriesXml.ExtractDouble("Popularity") ?? 0;
                TwitterId = seriesXml.ExtractStringOrNull("TwitterId");
                InstagramId = seriesXml.ExtractStringOrNull("InstagramId");
                FacebookId = seriesXml.ExtractStringOrNull("FacebookId");
                TagLine = seriesXml.ExtractStringOrNull("TagLine");
                Country = seriesXml.ExtractStringOrNull("Country");

                PosterUrl = seriesXml.ExtractString("posterURL");
                TrailerUrl = seriesXml.ExtractString("TrailerUrl");
                FanartUrl = seriesXml.ExtractString("FanartUrl");
                Imdb = seriesXml.ExtractStringOrNull("imdbId") ?? seriesXml.ExtractString("IMDB_ID");
                WebUrl = seriesXml.ExtractString("WebURL");
                OfficialUrl = seriesXml.ExtractString("OfficialUrl");
                ShowLanguage = seriesXml.ExtractString("ShowLanguage");
                TvRageCode = seriesXml.ExtractInt("rageid") ?? 0;
                Network = seriesXml.ExtractStringOrNull("network") ?? seriesXml.ExtractString("Network");
                Overview = seriesXml.ExtractStringOrNull("overview") ?? seriesXml.ExtractString("Overview");
                ContentRating = seriesXml.ExtractStringOrNull("rating") ?? seriesXml.ExtractString("Rating");
                Runtime = seriesXml.ExtractStringOrNull("runtime") ?? seriesXml.ExtractString("Runtime");
                SeriesId = seriesXml.ExtractStringOrNull("seriesId") ?? seriesXml.ExtractString("SeriesID");
                Status = seriesXml.ExtractStringOrNull("status") ?? seriesXml.ExtractString("Status");
                SiteRatingVotes = seriesXml.ExtractInt("siteRatingCount") ?? seriesXml.ExtractInt("SiteRatingCount", 0);
                Slug = seriesXml.ExtractString("slug");
                SiteRating = GetSiteRating(seriesXml);
                FirstAired = JsonHelper.ParseFirstAired(seriesXml.ExtractStringOrNull("FirstAired") ?? seriesXml.ExtractString("firstAired"));
                LoadActors(seriesXml);
                LoadCrew(seriesXml);
                LoadAliases(seriesXml);
                LoadGenres(seriesXml);
            }
            catch (SourceConsistencyException e)
            {
                LOGGER.Error(e, GenerateErrorMessage());
                // ReSharper disable once PossibleIntendedRethrow
                throw e;
            }
        }

        protected void MergeCommon(CachedMediaInfo o, bool useNewDataOverOld)
        {
            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            Name = ChooseBetter(Name, useNewDataOverOld, o.Name);
            Imdb = ChooseBetter(Imdb, useNewDataOverOld, o.Imdb);
            WebUrl = ChooseBetter(WebUrl, useNewDataOverOld, o.WebUrl);
            OfficialUrl = ChooseBetter(OfficialUrl, useNewDataOverOld, o.OfficialUrl);
            ShowLanguage = ChooseBetter(ShowLanguage, useNewDataOverOld, o.ShowLanguage);
            Overview = ChooseBetter(Overview, useNewDataOverOld, o.Overview);
            BannerString = ChooseBetter(BannerString, useNewDataOverOld, o.BannerString);
            PosterUrl = ChooseBetter(PosterUrl, useNewDataOverOld, o.PosterUrl);
            FanartUrl = ChooseBetter(FanartUrl, useNewDataOverOld, o.FanartUrl);
            TrailerUrl = ChooseBetter(TrailerUrl, useNewDataOverOld, o.TrailerUrl);
            Network = ChooseBetter(Network, useNewDataOverOld, o.Network);
            Runtime = ChooseBetter(Runtime, useNewDataOverOld, o.Runtime);
            SeriesId = ChooseBetter(SeriesId, useNewDataOverOld, o.SeriesId);
            Status = ChooseBetterStatus(Status, useNewDataOverOld, o.Status);
            ContentRating = ChooseBetter(ContentRating, useNewDataOverOld, o.ContentRating);
            Slug = ChooseBetter(Slug, useNewDataOverOld, o.Slug);
            TwitterId = ChooseBetter(TwitterId, useNewDataOverOld, o.TwitterId);
            InstagramId = ChooseBetter(InstagramId, useNewDataOverOld, o.InstagramId);
            FacebookId = ChooseBetter(FacebookId, useNewDataOverOld, o.FacebookId);
            TagLine = ChooseBetter(TagLine, useNewDataOverOld, o.TagLine);
            Country = ChooseBetter(Country, useNewDataOverOld, o.Country);

            if (o.FirstAired.HasValue && (useNewDataOverOld || !FirstAired.HasValue))
            {
                FirstAired = o.FirstAired;
            }

            if (useNewDataOverOld && o.SiteRating > 0)
            {
                SiteRating = o.SiteRating;
            }

            if (useNewDataOverOld && o.Popularity > 0)
            {
                Popularity = o.Popularity;
            }

            if (useNewDataOverOld && o.SiteRatingVotes > 0)
            {
                SiteRatingVotes = o.SiteRatingVotes;
            }

            bool useNewAliases = o.Aliases.HasAny() && useNewDataOverOld;
            if (!Aliases.HasAny() || useNewAliases)
            {
                Aliases = o.Aliases;
            }

            bool useNewGenres = o.Genres.HasAny() && useNewDataOverOld;
            if (!Genres.HasAny() || useNewGenres)
            {
                Genres = o.Genres;
            }

            bool useNewCrew = o.Crew.HasAny() && useNewDataOverOld;
            if (!Crew.HasAny() || useNewCrew)
            {
                Crew = o.Crew;
            }

            bool useNewActors = o.Actors.HasAny() && useNewDataOverOld;
            if (!Actors.HasAny() || useNewActors)
            {
                Actors = o.Actors;
            }

            if (useNewDataOverOld)
            {
                ActualLocale = o.ActualLocale;
            }

            Dirty = o.Dirty;
            IsSearchResultOnly = o.IsSearchResultOnly;
        }
    }
}
