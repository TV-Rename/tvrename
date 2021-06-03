using JetBrains.Annotations;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public class CachedMovieInfo : CachedMediaInfo
    {
        public string? Network;
        public string? Type;
        public int? CollectionId;
        public string? CollectionName;
        private MovieImages images = new MovieImages();

        public int? Year => FirstAired?.Year;

        public string? FanartUrl;

        protected override MediaConfiguration.MediaType MediaType() => MediaConfiguration.MediaType.movie;

        private void DefaultValues()
        {
        }

        public CachedMovieInfo(Locale locale, TVDoc.ProviderType source) : base(locale, source)
        {
            DefaultValues();
        }

        public CachedMovieInfo(int tvdb, int tvmaze, int tmdb, Locale locale, TVDoc.ProviderType source) : base(tvdb, tvmaze, tmdb, locale, source)
        {
            DefaultValues();
        }

        public CachedMovieInfo([NotNull] XElement seriesXml, TVDoc.ProviderType source) : base(source)
        {
            DefaultValues();
            LoadXml(seriesXml);
            IsSearchResultOnly = false;
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void Merge([NotNull] CachedMovieInfo o)
        {
            if (o.IsSearchResultOnly && !IsSearchResultOnly)
            {
                return;
            }

            if (o.TvdbCode != TvdbCode && o.TvMazeCode != TvMazeCode && o.TmdbCode != TmdbCode)
            {
                return; // that's not us!
            }

            if (o.TvMazeCode != -1 && TvMazeCode != o.TvMazeCode)
            {
                TvMazeCode = o.TvMazeCode;
            }

            if (o.TmdbCode != -1 && TmdbCode != o.TmdbCode)
            {
                TmdbCode = o.TmdbCode;
            }

            if (o.TvdbCode != -1 && TvdbCode != o.TvdbCode)
            {
                TvdbCode = o.TvdbCode;
            }

            if (o.SrvLastUpdated != 0 && o.SrvLastUpdated < SrvLastUpdated)
            {
                return; // older!?
            }

            if (!o.IsSearchResultOnly)
            {
                IsSearchResultOnly = false;
            }
            bool currentLanguageNotSet = ActualLocale is null;
            //            Language optimaLanguage = config o.ActualLocale ?? TVSettings.Instance.PreferredTVDBLanguage;
            // bool newLanguageOptimal = o.ActualLocale.PreferredLanguage == optimaLanguage;
            bool useNewDataOverOld = currentLanguageNotSet || (o.SrvLastUpdated >= SrvLastUpdated); //TODO - work out cached language and see what's best || newLanguageOptimal;

            SrvLastUpdated = o.SrvLastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            Name = ChooseBetter(Name, useNewDataOverOld, o.Name);
            Imdb = ChooseBetter(Imdb, useNewDataOverOld, o.Imdb);
            WebUrl = ChooseBetter(WebUrl, useNewDataOverOld, o.WebUrl);
            OfficialUrl = ChooseBetter(OfficialUrl, useNewDataOverOld, o.OfficialUrl);
            ShowLanguage = ChooseBetter(ShowLanguage, useNewDataOverOld, o.ShowLanguage);
            Type = ChooseBetter(Type, useNewDataOverOld, o.Type);
            Overview = ChooseBetter(Overview, useNewDataOverOld, o.Overview);
            PosterUrl = ChooseBetter(PosterUrl, useNewDataOverOld, o.PosterUrl);
            FanartUrl = ChooseBetter(FanartUrl, useNewDataOverOld, o.FanartUrl);
            TrailerUrl = ChooseBetter(TrailerUrl, useNewDataOverOld, o.TrailerUrl);
            Network = ChooseBetter(Network, useNewDataOverOld, o.Network);
            Runtime = ChooseBetter(Runtime, useNewDataOverOld, o.Runtime);
            SeriesId = ChooseBetter(SeriesId, useNewDataOverOld, o.SeriesId);
            Status = ChooseBetterStatus(Status, useNewDataOverOld, o.Status);
            ContentRating = ChooseBetter(ContentRating, useNewDataOverOld, o.ContentRating);
            Slug = ChooseBetter(Slug, useNewDataOverOld, o.Slug);
            CollectionName = ChooseBetter(CollectionName, useNewDataOverOld, o.CollectionName);
            TwitterId = ChooseBetter(TwitterId, useNewDataOverOld, o.TwitterId);
            InstagramId = ChooseBetter(InstagramId, useNewDataOverOld, o.InstagramId);
            FacebookId = ChooseBetter(FacebookId, useNewDataOverOld, o.FacebookId);
            TagLine = ChooseBetter(TagLine, useNewDataOverOld, o.TagLine);

            if (useNewDataOverOld && o.CollectionId.HasValue)
            {
                CollectionId = o.CollectionId;
            }

            if (o.FirstAired.HasValue && (useNewDataOverOld || !FirstAired.HasValue))
            {
                FirstAired = o.FirstAired;
            }

            if (useNewDataOverOld && o.SiteRating > 0)
            {
                SiteRating = o.SiteRating;
            }

            if (useNewDataOverOld && o.SiteRatingVotes > 0)
            {
                SiteRatingVotes = o.SiteRatingVotes;
            }

            if (useNewDataOverOld && o.Popularity > 0)
            {
                Popularity = o.Popularity;
            }

            bool useNewAliases = o.Aliases.Any() && useNewDataOverOld;
            if (!Aliases.Any() || useNewAliases)
            {
                Aliases = o.Aliases;
            }

            bool useNewGenres = o.Genres.Any() && useNewDataOverOld;
            if (!Genres.Any() || useNewGenres)
            {
                Genres = o.Genres;
            }

            bool useNewCrew = o.Crew.Any() && useNewDataOverOld;
            if (!Crew.Any() || useNewCrew)
            {
                Crew = o.Crew;
            }

            bool useNewActors = o.Actors.Any() && useNewDataOverOld;
            if (!Actors.Any() || useNewActors)
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

        private void LoadXml([NotNull] XElement seriesXml)
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

                CollectionId = seriesXml.ExtractInt("CollectionId");
                Popularity = seriesXml.ExtractDouble("Popularity") ?? 0;
                CollectionName = seriesXml.ExtractStringOrNull("CollectionName");
                TwitterId = seriesXml.ExtractStringOrNull("TwitterId");
                InstagramId = seriesXml.ExtractStringOrNull("InstagramId");
                FacebookId = seriesXml.ExtractStringOrNull("FacebookId");
                TagLine = seriesXml.ExtractStringOrNull("TagLine");

                PosterUrl = seriesXml.ExtractString("posterURL");
                TrailerUrl = seriesXml.ExtractString("TrailerUrl");
                FanartUrl = seriesXml.ExtractString("FanartUrl");
                Imdb = seriesXml.ExtractStringOrNull("imdbId") ?? seriesXml.ExtractString("IMDB_ID");
                WebUrl = seriesXml.ExtractString("WebURL");
                OfficialUrl = seriesXml.ExtractString("OfficialUrl");
                Type = seriesXml.ExtractString("Type");
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

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Movie");

            writer.WriteElement("id", TvdbCode);
            writer.WriteElement("mazeid", TvMazeCode);
            writer.WriteElement("TMDBCode", TmdbCode);
            writer.WriteElement("SeriesName", Name);
            writer.WriteElement("lastupdated", SrvLastUpdated);
            writer.WriteElement("LanguageId", ActualLocale?.PreferredLanguage?.TvdbId);
            writer.WriteElement("RegionCode", ActualLocale?.PreferredRegion?.Abbreviation);
            writer.WriteElement("CollectionId", CollectionId);
            writer.WriteElement("CollectionName", CollectionName);
            writer.WriteElement("TwitterId", TwitterId);
            writer.WriteElement("InstagramId", InstagramId);
            writer.WriteElement("FacebookId", FacebookId);
            writer.WriteElement("TagLine", TagLine);
            writer.WriteElement("posterURL", PosterUrl);
            writer.WriteElement("FanartUrl", FanartUrl);
            writer.WriteElement("TrailerUrl", TrailerUrl);
            writer.WriteElement("WebURL", WebUrl);
            writer.WriteElement("OfficialUrl", OfficialUrl);
            writer.WriteElement("ShowLanguage", ShowLanguage);
            writer.WriteElement("Type", Type);
            writer.WriteElement("imdbId", Imdb);
            writer.WriteElement("rageid", TvRageCode);
            writer.WriteElement("network", Network);
            writer.WriteElement("overview", Overview);
            writer.WriteElement("rating", ContentRating);
            writer.WriteElement("runtime", Runtime);
            writer.WriteElement("seriesId", SeriesId);
            writer.WriteElement("status", Status);
            writer.WriteElement("siteRating", SiteRating, "0.##");
            writer.WriteElement("siteRatingCount", SiteRatingVotes);
            writer.WriteElement("slug", Slug);
            writer.WriteElement("Popularity", Popularity, "0.##");

            if (FirstAired != null)
            {
                writer.WriteElement("FirstAired", FirstAired.Value.ToString("yyyy-MM-dd"));
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

            writer.WriteEndElement(); // cachedSeries
        }

        public void AddOrUpdateImage(MovieImage image)
        {
            images.RemoveAll(s => s.Id == image.Id);
            images.Add(image);
        }
    }
}
