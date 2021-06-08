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
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public class MediaImage
    {
        //see https://fanart.tv/movie-fanart/ for some details
        public enum ImageType
        {
            poster,
            background, //or fanart
            wideBanner,
            icon,
            clearLogo,
            clearArt,
            cdArt,
            thumbs,
        }

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
            ImageUrl = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("ImageUrl"));
            ImageStyle = r.ExtractEnum("ImageStyle", ImageType.poster);
            Subject = r.ExtractEnum("Subject", ImageSubject.show);
            LanguageCode = r.ExtractStringOrNull("LanguageCode");
            Resolution = r.ExtractStringOrNull("Resolution");
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
            writer.WriteElement("Id", Id);
            writer.WriteElement("ImageUrl", ImageUrl, true);
            writer.WriteElement("ImageStyle", (int)ImageStyle);
            writer.WriteElement("Subject", (int)Subject);
            writer.WriteElement("LanguageCode", LanguageCode, true);
            writer.WriteElement("Resolution", Resolution, true);
            writer.WriteElement("Rating", Rating);
            writer.WriteElement("RatingCount", RatingCount);
            writer.WriteElement("ThumbnailUrl", ThumbnailUrl, true);
        }
    }
}
