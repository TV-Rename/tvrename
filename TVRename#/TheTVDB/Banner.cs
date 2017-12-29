// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.htmlr
//
// For more information see http://thetvdb.com/wiki/index.php/API:banners.xml
//  

using System;
using System.Xml;
using Newtonsoft.Json.Linq;
using NLog;

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
        public int SeasonId;
        public int SeriesId;
        public string ThumbnailPath;

        public Season TheSeason;
        public SeriesInfo TheSeries;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Banner(Banner o)
        {

            BannerId = o.BannerId;
            BannerPath = o.BannerPath;
            BannerType = o.BannerType;
            LanguageId = o.LanguageId;
            Resolution = o.Resolution;
            Rating = o.Rating;
            RatingCount = o.RatingCount;
            SeasonId = o.SeasonId;
            SeriesId = o.SeriesId;

            ThumbnailPath = o.ThumbnailPath;
            
            TheSeason = o.TheSeason;
            TheSeries = o.TheSeries;

        }

        public Banner(SeriesInfo ser, Season seas)
        {
            SetDefaults(ser, seas);
        }

        public Banner(SeriesInfo ser, Season seas, int codeHint, XmlReader r, CommandLineArgs args)
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

                SeriesId = codeHint;

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
                        SeasonId = r.ReadElementContentAsInt();
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
                if (SeriesId != -1)
                    message += "\r\nSeries ID: " + SeriesId;
                if (BannerId != -1)
                    message += "\r\nBanner ID: " + BannerId;
                if (!string.IsNullOrEmpty(BannerPath))
                    message += "\r\nBanner Path: " + BannerPath;

                Logger.Error(e, message);
                
                throw new TVDBException(e.Message);
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
            BannerType = (string)json["keyType"];
            LanguageId = langId;//(json["languageId"] == null) ? -1 : (int)json["languageId"];
            
            double.TryParse((string)(json["ratingsInfo"]["average"]), out Rating);
            RatingCount = (int)(json["ratingsInfo"]["count"]);

            Resolution = (string)json["resolution"];
            int.TryParse((string)json["subKey"], out SeasonId);
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

        public bool IsSeriesPoster()
        {
            return ((BannerType == "poster"));
        }

        public bool IsSeriesBanner()
        {
            return ((BannerType == "series"));
        }

        public bool IsSeasonPoster()
        {
            return ((BannerType == "season") );
        }

        public bool IsSeasonBanner()
        {
            return ((BannerType == "seasonwide") );
        }

        public bool IsFanart()
        {
            return ((BannerType == "fanart") );
        }

        public void SetDefaults(SeriesInfo ser, Season seas)
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
            SeasonId = -1;
            SeriesId = -1;

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
            XMLHelper.WriteElementToXML(writer,"Season",SeasonId);  
            XMLHelper.WriteElementToXML(writer,"ThumbnailPath",ThumbnailPath);

            writer.WriteEndElement(); //Banner
        }
    }
}

