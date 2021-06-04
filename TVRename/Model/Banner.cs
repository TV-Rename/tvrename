//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.mdr
//
// For more information see http://thetvdb.com/wiki/index.php/API:banners.xml
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public class MediaImage
    {
        public enum ImageType { poster, background, wideBanner }

        public enum ImageSubject { show, season, episode, movie }

        public int Id;
        public string? LanguageCode;
        public string? ImageUrl;
        public ImageType ImageStyle;
        public ImageSubject Subject;
        public string? Resolution;
        public double Rating;
        public int RatingCount;
        public string? ThumbnailUrl;

        protected MediaImage(XElement r)
        {
            Id = r.ExtractInt("Id") ?? -1;
            LanguageCode = r.ExtractString("LanguageCode");

            ImageUrl = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("ImageUrl"));
            ImageStyle = r.ExtractEnum("ImageStyle", ImageType.poster);
            Subject = r.ExtractEnum("Subject", ImageSubject.show);

            Resolution = r.ExtractString("Resolution");
            string sn = r.ExtractString("Rating");
            double.TryParse(sn, out Rating);
            RatingCount = r.ExtractInt("RatingCount", -1);
            ThumbnailUrl = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("ThumbnailUrl"));
        }

        protected MediaImage()
        {
        }

        protected void WriteCoreXml([NotNull] XmlWriter writer)
        {
            writer.WriteElement("id", Id);
            writer.WriteElement("ImageUrl", ImageUrl);
            writer.WriteElement("ImageStyle", (int)ImageStyle);
            writer.WriteElement("Subject", (int)Subject);
            writer.WriteElement("LanguageCode", LanguageCode);
            writer.WriteElement("Resolution", Resolution);
            writer.WriteElement("Rating", Rating);
            writer.WriteElement("RatingCount", RatingCount);
            writer.WriteElement("ThumbnailUrl", ThumbnailUrl);
        }
    }

    public class MovieImage : MediaImage
    {
        public int MovieId;
        public TVDoc.ProviderType MovieSource;

        public MovieImage()
        {
            Subject = ImageSubject.movie;
        }

        public MovieImage(int movieId, TVDoc.ProviderType source, [NotNull] XElement r) : base(r)
        {
            MovieId = r.ExtractInt("MovieId") ?? movieId; // thetvdb cachedSeries id
            MovieSource = source;
        }

        protected void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("MovieImage");
            WriteCoreXml(writer);
            writer.WriteElement("MovieId", MovieId);
            writer.WriteEndElement(); //MovieImage
        }
    }

    public class ShowImage : MediaImage
    {
        public int? SeasonId;
        public int SeriesId;
        public TVDoc.ProviderType SeriesSource;
        public int? SeasonNumber;

        public ShowImage(int seriesId, TVDoc.ProviderType source, [NotNull] XElement r) : base(r)
        {
            SeriesId = r.ExtractInt("SeriesId") ?? seriesId; // thetvdb cachedSeries id
            SeasonId = r.ExtractInt("SeasonId");
            SeasonNumber = r.ExtractInt("SeasonNumber");
            SeriesSource = source;
        }

        public ShowImage()
        {
        }

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("ShowImage");
            WriteCoreXml(writer);
            writer.WriteElement("SeriesId", SeriesId);
            writer.WriteElement("SeasonId", SeasonId);
            writer.WriteElement("SeasonNumber", SeasonNumber);
            writer.WriteEndElement(); //ShowImage
        }

        internal static ShowImage GenerateFromLegacyBannerXml(int seriesId, XElement r, TVDoc.ProviderType source)
        {
            // <Banner>
            //        <id>708811</id>
            //        <BannerPath>seasonswide/79488-5.jpg</BannerPath>
            //        <BannerType>season</BannerType>
            //        <BannerType2>seasonwide</BannerType2>
            //        <Language>en</Language>
            //        <Rating/>
            //        <RatingCount>0</RatingCount>
            //        <Season>5</Season>
            //  blah blah
            // </Banner>
            string sn = r.ExtractString("Rating");
            double.TryParse(sn, out double rating);

            ShowImage legacy = new ShowImage
            {
                Id = r.ExtractInt("id") ?? -1,
                SeriesId = r.ExtractInt("seriesid") ?? seriesId,
                SeasonId = r.ExtractInt("seasonid", -1),
                ImageUrl = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("BannerPath")),
                SeriesSource = source,
                ImageStyle = Convert(r.ExtractString("BannerType")),
                Subject = Convert2(r.ExtractString("BannerType")),
                LanguageCode = Languages.Instance.GetLanguageFromId(r.ExtractInt("LanguageId") ?? -1)?.Abbreviation,
                Resolution = r.ExtractString("Resolution"),
                Rating = rating,
                RatingCount = r.ExtractInt("RatingCount", -1)
            };

            // thetvdb cachedSeries id
            legacy.SeasonId = r.ExtractInt("Season", -1);
            legacy.ThumbnailUrl = r.ExtractString("ThumbnailPath");

            return legacy;
        }

        private static ImageSubject Convert2(string v)
        {
            if (v == "fanart") return ImageSubject.show;
            if (v == "poster") return ImageSubject.show;
            if (v == "season") return ImageSubject.season;
            if (v == "series") return ImageSubject.season;
            if (v == "seasonwide") return ImageSubject.show;
            throw new NotImplementedException();
        }

        private static ImageType Convert(string v)
        {
            if (v == "fanart") return ImageType.background;
            if (v == "poster") return ImageType.poster;
            if (v == "season") return ImageType.poster;
            if (v == "series") return ImageType.wideBanner;
            if (v == "seasonwide") return ImageType.wideBanner;
            throw new NotImplementedException();
        }
    }

    public class MovieImages : SafeList<MovieImage>
    {
    }

    public class ShowImages : SafeList<ShowImage>
    {
        internal ShowImage? GetSeasonWideBanner(int snum, Language lang)
        {
            return GetImage(snum, lang, MediaImage.ImageType.wideBanner);
        }

        internal ShowImage? GetSeasonBanner(int snum, Language lang)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            return GetImage(snum, lang, MediaImage.ImageType.poster);
        }

        internal ShowImage? GetImage(int snum, Language lang, MediaImage.ImageType type)
        {
            return GetBestSeasonLanguage(snum, lang, type)
                ?? GetBestSeason(snum, type)
                ?? GetShowImage(lang, type)
                ;
        }

        internal ShowImage? GetShowImage(Language lang, MediaImage.ImageType type)
        {
            return GetSeriesLangImage(lang, type)
                ?? GetSeriesImage(type);
        }

        private ShowImage? GetBestSeason(int snum, MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> validImages = this.Where(i => i.ImageStyle == type && i.SeasonNumber == snum);
            return BestFrom(validImages);
        }

        private ShowImage? GetSeriesLangImage(Language l, MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> validImages = this.Where(i => i.ImageStyle == type && i.LanguageCode == l.ThreeAbbreviation);
            return BestFrom(validImages);
        }

        private ShowImage? GetBestSeasonLanguage(int snum, Language l, MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> validImages = this.Where(i => i.ImageStyle == type && i.SeasonNumber == snum && i.LanguageCode == l.Abbreviation);
            return BestFrom(validImages);
        }

        private ShowImage? GetSeriesImage(MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> validImages = this.Where(i => i.ImageStyle == type);
            return BestFrom(validImages);
        }

        private static ShowImage? BestFrom(IEnumerable<ShowImage> validImages)
        {
            List<ShowImage> showImages = validImages.ToList();
            if (!showImages.Any())
            {
                return null;
            }
            double maxRating = showImages.Select(pair => pair.Rating).Max();

            return showImages.First(pair => Math.Abs(pair.Rating - maxRating) < 0.001);
        }

        public ShowImage? GetImage(TVSettings.FolderJpgIsType type, Language lang)
        {
            return type switch
            {
                TVSettings.FolderJpgIsType.Banner => GetShowImage(lang, MediaImage.ImageType.wideBanner),
                TVSettings.FolderJpgIsType.FanArt => GetShowImage(lang, MediaImage.ImageType.background),
                TVSettings.FolderJpgIsType.SeasonPoster => GetShowImage(lang, MediaImage.ImageType.poster),
                _ => GetShowImage(lang, MediaImage.ImageType.poster)
            };
        }

        internal void MergeImages(ShowImages images)
        {
            if (!this.Any())
            {
                Clear();
                AddRange(images);
                return;
            }
            foreach (ShowImage i in images)
            {
                if (this.All(si => si.Id != i.Id))
                {
                    Add(i);
                }
            }
        }
    }
}
