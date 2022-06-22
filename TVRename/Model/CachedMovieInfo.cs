using System;
using System.Collections.Generic;
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
        private readonly MovieImages images = new();

        public int? Year => FirstAired?.Year;

        protected override MediaConfiguration.MediaType MediaType() => MediaConfiguration.MediaType.movie;

        public override ProcessedSeason.SeasonType SeasonOrder => throw new InvalidOperationException();

        public bool InCollection => CollectionId.HasValue && CollectionName.HasValue();

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

        public CachedMovieInfo(XElement seriesXml, TVDoc.ProviderType source) : base(source)
        {
            DefaultValues();
            IsSearchResultOnly = false;
            LoadCommonXml(seriesXml);
            try
            {
                CollectionId = seriesXml.ExtractInt("CollectionId");
                CollectionName = seriesXml.ExtractStringOrNull("CollectionName");
                MovieType = seriesXml.ExtractString("Type");

                foreach (MovieImage s in seriesXml.Descendants("Images").Descendants("MovieImage").Select(xml => new MovieImage(IdCode(Source), Source, xml)))
                {
                    images.Add(s);
                }
            }
            catch (SourceConsistencyException e)
            {
                LOGGER.Error(e, GenerateErrorMessage());
                // ReSharper disable once PossibleIntendedRethrow
                throw e;
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void Merge(CachedMovieInfo o)
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

            MergeCommon(o,useNewDataOverOld);
            MovieType = ChooseBetter(MovieType, useNewDataOverOld, o.MovieType);
            CollectionName = ChooseBetter(CollectionName, useNewDataOverOld, o.CollectionName);

            if (useNewDataOverOld && o.CollectionId.HasValue)
            {
                CollectionId = o.CollectionId;
            }

            images.MergeImages(o.images);
        }

        public void WriteXml(XmlWriter writer)
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
            return images.Where(x => x.ImageStyle == type && x.LocationMatches(TargetLocale.LanguageToUse(Source)));
        }

        public IEnumerable<MovieImage> Images(MediaImage.ImageType type, MediaImage.ImageSubject subject)
        {
            return Images(type).Where(x => x.Subject == subject);
        }
    }
}
