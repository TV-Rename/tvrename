using System;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public class ShowImage : MediaImage
    {
        public int? SeasonId;
        public int SeriesId;
        public TVDoc.ProviderType SeriesSource;
        public int? SeasonNumber;

        public ShowImage(int seriesId, TVDoc.ProviderType source, XElement r) : base(r)
        {
            SeriesId = r.ExtractInt("SeriesId") ?? seriesId; // thetvdb cachedSeries id
            SeasonId = r.ExtractInt("SeasonId");
            SeasonNumber = r.ExtractInt("SeasonNumber");
            SeriesSource = source;
        }

        public ShowImage()
        {
        }

        public void WriteXml(XmlWriter writer)
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

            ShowImage legacy = new()
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

            static ImageSubject Convert2(string v)
            {
                return v switch
                {
                    "fanart" => ImageSubject.show,
                    "poster" => ImageSubject.show,
                    "season" => ImageSubject.season,
                    "series" => ImageSubject.season,
                    "seasonwide" => ImageSubject.show,
                    _ => throw new ArgumentException()
                };
            }

            static ImageType Convert(string v)
            {
                return v switch
                {
                    "fanart" => ImageType.background,
                    "poster" => ImageType.poster,
                    "season" => ImageType.poster,
                    "series" => ImageType.wideBanner,
                    "seasonwide" => ImageType.wideBanner,
                    _ => throw new ArgumentException()
                };
            }
        }
    }
}
