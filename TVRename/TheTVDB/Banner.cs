// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.mdr
//
// For more information see http://thetvdb.com/wiki/index.php/API:banners.xml
//  

using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace TVRename
{
    public class Banner
    {
        public int BannerId;
        public int LanguageId;
        public string BannerPath;
        private string bannerType;
        private string resolution;
        public double Rating;
        private int ratingCount;
        public int SeasonId;
        public int SeriesId;
        private string thumbnailPath;
        private Season theSeason;
        private SeriesInfo theSeries;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Banner(int seriesId, XmlReader r)
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
            try
            {
                SetDefaults(null, null);

                SeriesId = seriesId;

                r.Read();
                if (r.Name != "Banner")
                    return;

                r.Read();
                while (!r.EOF)
                {
                    if ((r.Name == "Banner") && (!r.IsStartElement()))
                        break;

                    if (r.Name == "id")
                        BannerId = r.ReadElementContentAsInt();
                    else if (r.Name == "seriesid")
                        SeriesId = r.ReadElementContentAsInt(); // thetvdb series id
                    else if (r.Name == "seasonid")
                        SeasonId = r.ReadElementContentAsInt();
                    else if (r.Name == "BannerPath")
                        BannerPath = XmlHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "BannerType")
                        bannerType = r.ReadElementContentAsString();
                    else if (r.Name == "LanguageId")
                        LanguageId = r.ReadElementContentAsInt();
                    else if (r.Name == "Resolution")
                        resolution = r.ReadElementContentAsString();
                    else if (r.Name == "Rating")
                        {
                        string sn = r.ReadElementContentAsString();
                        double.TryParse(sn, out Rating);
                        }
                    else if (r.Name == "RatingCount")
                        ratingCount  = r.ReadElementContentAsInt();
                    else if (r.Name == "Season")
                        SeasonId = r.ReadElementContentAsInt();
                    else if (r.Name == "ThumbnailPath") thumbnailPath = r.ReadElementContentAsString();
                    else
                    {
                        if ((r.IsEmptyElement) || !r.IsStartElement())
                            r.ReadOuterXml();
                        else
                            r.Read();
                    }
                }
            }
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for a banner.";
                if (SeriesId != -1)
                    message += "\r\nSeries ID: " + SeriesId;
                if (BannerId != -1)
                    message += "\r\nBanner ID: " + BannerId;
                if (!string.IsNullOrEmpty(BannerPath))
                    message += "\r\nBanner Path: " + BannerPath;

                Logger.Error(e, message);
                
                throw new TheTVDB.TVDBException(e.Message);
            }
        }

        public Banner(int seriesId, JObject json, int langId)
        {
            SetDefaults(null, null);
            // {
            //  "fileName": "string",
            //  "id": 0,
            //  "keyType": "string",
            //  "languageId": 0,
            //  "ratingsInfo": {
            //      "average": 0,
            //      "count": 0
            //      },
            //  "resolution": "string",
            //  "subKey": "string",         //May Contain Season Number
            //  "thumbnail": "string"
            //  }

            SeriesId = seriesId;

            BannerPath = (string)json["fileName"];
            BannerId = (int)json["id"];
            bannerType = (string)json["keyType"];
            LanguageId = (json["languageId"] == null) ? langId  : (int)json["languageId"];
            
            double.TryParse((string)(json["ratingsInfo"]["average"]), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out Rating);
            ratingCount = (int)(json["ratingsInfo"]["count"]);

            resolution = (string)json["resolution"];
            int.TryParse((string)json["subKey"], out SeasonId);
            thumbnailPath = (string)json["thumbnail"];
        }

        public bool SameAs(Banner  o) => (BannerId == o.BannerId);

        public bool IsSeriesPoster() => (bannerType == "poster");

        public bool IsSeriesBanner() => (bannerType == "series");

        public bool IsSeasonPoster() => (bannerType == "season");

        public bool IsSeasonBanner() => (bannerType == "seasonwide" );

        public bool IsFanart() => (bannerType == "fanart");

        private void SetDefaults(SeriesInfo ser, Season seas)
        {
            theSeason = seas;
            theSeries = ser;
            BannerId = -1;
            BannerPath = "";
            bannerType = "";
            LanguageId = -1;
            resolution = "";
            Rating = -1;
            ratingCount = 0;
            SeasonId = -1;
            SeriesId = -1;

            thumbnailPath = "";
        }

        public void WriteXml(XmlWriter writer)
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

            writer.WriteStartElement("Banner");

            XmlHelper.WriteElementToXml(writer,"id",BannerId);
            XmlHelper.WriteElementToXml(writer,"BannerPath",BannerPath);
            XmlHelper.WriteElementToXml(writer,"BannerType",bannerType);
            XmlHelper.WriteElementToXml(writer, "LanguageId", LanguageId);
            XmlHelper.WriteElementToXml(writer,"Resolution",resolution);
            XmlHelper.WriteElementToXml(writer,"Rating",Rating);
            XmlHelper.WriteElementToXml(writer,"RatingCount",ratingCount);
            XmlHelper.WriteElementToXml(writer,"Season",SeasonId);  
            XmlHelper.WriteElementToXml(writer,"ThumbnailPath",thumbnailPath);

            writer.WriteEndElement(); //Banner
        }
    }
}
