using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public class CachedMovieInfo : CachedMediaInfo
    {
        public string? MovieType;
        public int? CollectionId;
        public string? CollectionName;
        private MovieImages images = new MovieImages();

        public int? Year => FirstAired?.Year;

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
            bool useNewDataOverOld = currentLanguageNotSet || o.SrvLastUpdated >= SrvLastUpdated; //TODO - work out cached language and see what's best || newLanguageOptimal;

            SrvLastUpdated = o.SrvLastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            Name = ChooseBetter(Name, useNewDataOverOld, o.Name);
            Imdb = ChooseBetter(Imdb, useNewDataOverOld, o.Imdb);
            WebUrl = ChooseBetter(WebUrl, useNewDataOverOld, o.WebUrl);
            OfficialUrl = ChooseBetter(OfficialUrl, useNewDataOverOld, o.OfficialUrl);
            ShowLanguage = ChooseBetter(ShowLanguage, useNewDataOverOld, o.ShowLanguage);
            MovieType = ChooseBetter(MovieType, useNewDataOverOld, o.MovieType);
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
            Country = ChooseBetter(Country, useNewDataOverOld, o.Country);

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
            images.MergeImages(o.images);

            Dirty = o.Dirty;
            IsSearchResultOnly = o.IsSearchResultOnly;
        }

        private void LoadXml([NotNull] XElement seriesXml)
        {
            LoadCommonXml(seriesXml);
            try
            {
                CollectionId = seriesXml.ExtractInt("CollectionId");
                CollectionName = seriesXml.ExtractStringOrNull("CollectionName");
                MovieType = seriesXml.ExtractString("Type");
                LoadImages(seriesXml);
            }
            catch (SourceConsistencyException e)
            {
                LOGGER.Error(e, GenerateErrorMessage());
                // ReSharper disable once PossibleIntendedRethrow
                throw e;
            }
        }

        private void LoadImages([NotNull] XElement seriesXml)
        {
            images = new MovieImages();
            foreach (MovieImage s in seriesXml.Descendants("Images").Descendants("MovieImage").Select(xml => new MovieImage(IdCode(Source), Source, xml)))
            {
                images.Add(s);
            }
        }

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Movie");
            WriteCommonFields(writer);

            writer.WriteElement("CollectionId", CollectionId);
            writer.WriteElement("CollectionName", CollectionName, true);
            writer.WriteElement("Type", MovieType);

            writer.WriteStartElement("Images");
            foreach (MovieImage i in images)
            {
                i.WriteXml(writer);
            }
            writer.WriteEndElement(); //Images

            writer.WriteEndElement(); // cachedSeries
        }

        public void AddOrUpdateImage(MovieImage image)
        {
            images.RemoveAll(s => s.Id == image.Id);
            images.Add(image);
        }

        public IEnumerable<MovieImage> Images(MediaImage.ImageType type)
        {
            return images.Where(x => x.ImageStyle == type && (x.LanguageCode ?? TargetLocale.LanguageToUse(Source).Abbreviation) == TargetLocale.LanguageToUse(Source).Abbreviation);
        }

        public IEnumerable<MovieImage> Images(MediaImage.ImageType type, MediaImage.ImageSubject subject)
        {
            return images.Where(x => x.ImageStyle == type && x.Subject == subject && (x.LanguageCode ?? TargetLocale.LanguageToUse(Source).Abbreviation ) == TargetLocale.LanguageToUse(Source).Abbreviation);
        }
    }
}
