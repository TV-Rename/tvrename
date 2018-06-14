// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.mdr
//
// For more information see http://thetvdb.com/wiki/index.php/API:banners.xml
//  

using Newtonsoft.Json.Linq;
using System;
using System.Xml;

namespace TVRename
{
    public class Banner
    {
        public int BannerId;
        public int LanguageId;
        public string BannerPath;
        public string BannerType;
        public string Resolution;
        public double Rating;
        public int RatingCount;
        public int SeasonID;
        public int SeriesID;
        public string ThumbnailPath;

        public Season TheSeason;
        public SeriesInfo TheSeries;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Banner(Banner O)
        {

            BannerId = O.BannerId;
            BannerPath = O.BannerPath;
            BannerType = O.BannerType;
            LanguageId = O.LanguageId;
            Resolution = O.Resolution;
            Rating = O.Rating;
            RatingCount = O.RatingCount;
            SeasonID = O.SeasonID;
            SeriesID = O.SeriesID;

            ThumbnailPath = O.ThumbnailPath;
            
            TheSeason = O.TheSeason;
            TheSeries = O.TheSeries;

        }

        public Banner(SeriesInfo ser, Season seas)
        {
            SetDefaults(ser, seas);
        }

        public Banner(SeriesInfo ser, Season seas, int? codeHint, XmlReader r, CommandLineArgs args)
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
                SetDefaults(ser, seas);

                SeriesID = (int) codeHint;

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
                        SeriesID = r.ReadElementContentAsInt(); // thetvdb series id
                    else if (r.Name == "seasonid")
                        SeasonID = r.ReadElementContentAsInt();
                    else if (r.Name == "BannerPath")
                        BannerPath = XMLHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "BannerType")
                        BannerType = r.ReadElementContentAsString();
                    else if (r.Name == "LanguageId")
                        LanguageId = r.ReadElementContentAsInt();
                    else if (r.Name == "Resolution")
                        Resolution = r.ReadElementContentAsString();
                    else if (r.Name == "Rating")
                        {
                        String sn = r.ReadElementContentAsString();
                        double.TryParse(sn, out Rating);
                        }
                    else if (r.Name == "RatingCount")
                        RatingCount  = r.ReadElementContentAsInt();
                    else if (r.Name == "Season")
                        SeasonID = r.ReadElementContentAsInt();
                    else if (r.Name == "ThumbnailPath") ThumbnailPath = r.ReadElementContentAsString();
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
                if (SeriesID != -1)
                    message += "\r\nSeries ID: " + SeriesID;
                if (BannerId != -1)
                    message += "\r\nBanner ID: " + BannerId;
                if (!string.IsNullOrEmpty(BannerPath))
                    message += "\r\nBanner Path: " + BannerPath;

                logger.Error(e, message);
                
                throw new TheTVDB.TVDBException(e.Message);
            }
        }

        public Banner(int seriesId, JObject json, int LangId)
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

            SeriesID = seriesId;

            BannerPath = (string)json["fileName"];
            BannerId = (int)json["id"];
            BannerType = (string)json["keyType"];
            LanguageId = (json["languageId"] == null) ? LangId  : (int)json["languageId"];
            
            double.TryParse((string)(json["ratingsInfo"]["average"]), out Rating);
            RatingCount = (int)(json["ratingsInfo"]["count"]);

            Resolution = (string)json["resolution"];
            int.TryParse((string)json["subKey"], out SeasonID);
            ThumbnailPath = (string)json["thumbnail"];
        }


        public int SeasonNumber
        {
            get
            {
                if (TheSeason != null)
                    return TheSeason.SeasonNumber;
                return -1;
            }
        }

        public bool SameAs(Banner  o)
        {
            return (BannerId == o.BannerId);
        }

        public bool isSeriesPoster()
        {
            return ((BannerType == "poster"));
        }

        public bool isSeriesBanner()
        {
            return ((BannerType == "series"));
        }

        public bool isSeasonPoster()
        {
            return ((BannerType == "season") );
        }

        public bool isSeasonBanner()
        {
            return ((BannerType == "seasonwide") );
        }

        public bool isFanart()
        {
            return ((BannerType == "fanart") );
        }

        private void SetDefaults(SeriesInfo ser, Season seas)
        {
            TheSeason = seas;
            TheSeries = ser;


            BannerId = -1;
            BannerPath = "";
            BannerType = "";
            LanguageId = -1;
            Resolution = "";
            Rating = -1;
            RatingCount = 0;
            SeasonID = -1;
            SeriesID = -1;

            ThumbnailPath = "";

        }
                
        public void SetSeriesSeason(SeriesInfo ser, Season seas)
        {
            TheSeason = seas;
            TheSeries = ser;
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

            XMLHelper.WriteElementToXML(writer,"id",BannerId);
            XMLHelper.WriteElementToXML(writer,"BannerPath",BannerPath);
            XMLHelper.WriteElementToXML(writer,"BannerType",BannerType);
            XMLHelper.WriteElementToXML(writer, "LanguageId", LanguageId);
            XMLHelper.WriteElementToXML(writer,"Resolution",Resolution);
            XMLHelper.WriteElementToXML(writer,"Rating",Rating);
            XMLHelper.WriteElementToXML(writer,"RatingCount",RatingCount);
            XMLHelper.WriteElementToXML(writer,"Season",SeasonID);  
            XMLHelper.WriteElementToXML(writer,"ThumbnailPath",ThumbnailPath);

            writer.WriteEndElement(); //Banner
        }
    }
}

