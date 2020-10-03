using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using TVRename.TheTVDB;

namespace TVRename
{
    public class CachedMovieInfo : CachedMediaInfo
    {
        public DateTime? FirstAired;
        public readonly string? TargetLanguageCode; //The Language Code we'd like the Series in ; null if we want to use the system setting
        public int LanguageId; //The actual language obtained
        public string? Network;
        public string? Type;
        public string? ShowLanguage;
        public int? CollectionId;
        public string? CollectionName;
        public string? SeriesId;

        public string? Slug;

        public bool UseCustomLanguage => TargetLanguageCode != null;


        public string? Status { get; set; }

        public int? Year => FirstAired?.Year;

        // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        public CachedMovieInfo()
        {
            Actors = new List<Actor>();
            Aliases = new List<string>();
            Genres = new List<string>();
            Dirty = false;
            Name = string.Empty;
            TvdbCode = -1;
            TvMazeCode = -1;
            TmdbCode = -1;
            TvRageCode = 0;
            LanguageId = -1;
            Status = "Unknown";
        }

        public CachedMovieInfo(int tvdb, int tvmaze,int tmdbId) : this()
        {
            IsSearchResultOnly = false;
            TvMazeCode = tvmaze;
            TvdbCode = tvdb;
            TmdbCode = tmdbId;
        }

        public CachedMovieInfo(int tvdb, int tvmaze, int tmdbId, string langCode) : this(tvdb, tvmaze,tmdbId)
        {
            TargetLanguageCode = langCode;
        }

        public CachedMovieInfo([NotNull] XElement seriesXml) : this()
        {
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

            if (o.TvdbCode != TvdbCode && o.TvMazeCode != TvMazeCode)
            {
                return; // that's not us!
            }

            if (o.TvMazeCode != -1 && TvMazeCode != o.TvMazeCode)
            {
                TvMazeCode = o.TvMazeCode;
            }

            if (o.TvdbCode != -1 && TvdbCode != o.TvdbCode)
            {
                TvdbCode = o.TvdbCode;
            }

            if (o.TmdbCode != -1 && TmdbCode != o.TmdbCode)
            {
                TmdbCode = o.TmdbCode;
            }

            if (o.SrvLastUpdated != 0 && o.SrvLastUpdated < SrvLastUpdated)
            {
                return; // older!?
            }

            if (!o.IsSearchResultOnly)
            {
                IsSearchResultOnly = false;
            }
            bool currentLanguageNotSet = LanguageId == -1;
            string bestLanguageCode = TargetLanguageCode ?? TVSettings.Instance.PreferredLanguageCode;
            Language optimaLanguage = LocalCache.Instance.GetLanguageFromCode(bestLanguageCode);
            bool newLanguageOptimal = !(optimaLanguage is null) && o.LanguageId == optimaLanguage.Id;
            bool useNewDataOverOld = currentLanguageNotSet || newLanguageOptimal;

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

            if (useNewDataOverOld)
            {
                LanguageId = o.LanguageId;
            }

            Dirty = o.Dirty;
            IsSearchResultOnly = o.IsSearchResultOnly;
        }

        [NotNull]
        private static string ChooseBetter(string? encumbant, bool betterLanguage, string? newValue)
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

        [NotNull]
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
                LanguageId = seriesXml.ExtractInt("LanguageId") ?? seriesXml.ExtractInt("languageId") ?? throw new SourceConsistencyException("Error Extracting Language for Series", TVDoc.ProviderType.TheTVDB);

                CollectionId = seriesXml.ExtractInt("CollectionId") ;
                CollectionName = seriesXml.ExtractStringOrNull("CollectionName");
                TwitterId = seriesXml.ExtractStringOrNull("TwitterId") ;
                InstagramId = seriesXml.ExtractStringOrNull("InstagramId");
                FacebookId = seriesXml.ExtractStringOrNull("FacebookId") ;
                TagLine = seriesXml.ExtractStringOrNull("TagLine") ;
    
                PosterUrl = seriesXml.ExtractString("posterURL");
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

        private static float GetSiteRating([NotNull] XElement seriesXml)
        {
            string siteRatingString = seriesXml.ExtractStringOrNull("siteRating") ?? seriesXml.ExtractString("SiteRating");
            float.TryParse(siteRatingString,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                CultureInfo.CreateSpecificCulture("en-US"), out float x);

            return x;
        }

        [NotNull]
        private string GenerateErrorMessage() => "Error processing data from TheTVDB for a show. " + this + "\r\nLanguage: \"" + LanguageId + "\"";

        private void LoadActors([NotNull] XElement seriesXml)
        {
            ClearActors();
            foreach (Actor a in seriesXml.Descendants("Actors").Descendants("Actor").Select(actorXml => new Actor(actorXml)))
            {
                AddActor(a);
            }
        }
        private void LoadAliases([NotNull] XElement seriesXml)
        {
            Aliases = new List<string>();
            foreach (XElement aliasXml in seriesXml.Descendants("Aliases").Descendants("Alias"))
            {
                Aliases.Add(aliasXml.Value);
            }
        }

        private void LoadGenres([NotNull] XElement seriesXml)
        {
            Genres = seriesXml
                .Descendants("Genres")
                .Descendants("Genre")
                .Select(g => g.Value.Trim()).Distinct()
                .ToList();
        }
 
        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Movie");

            writer.WriteElement("id", TvdbCode);
            writer.WriteElement("mazeid", TvMazeCode);
            writer.WriteElement("TMDBCode", TmdbCode);
            writer.WriteElement("SeriesName", Name);
            writer.WriteElement("lastupdated", SrvLastUpdated);
            writer.WriteElement("LanguageId", LanguageId);
            writer.WriteElement("CollectionId", CollectionId);
            writer.WriteElement("CollectionName", CollectionName);
            writer.WriteElement("TwitterId", TwitterId);
            writer.WriteElement("InstagramId", InstagramId);
            writer.WriteElement("FacebookId", FacebookId);
            writer.WriteElement("TagLine", TagLine);
            writer.WriteElement("posterURL", PosterUrl);
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
    }
}
