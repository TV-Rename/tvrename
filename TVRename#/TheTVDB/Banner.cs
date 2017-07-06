// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.htmlr
//
// For more information see http://thetvdb.com/wiki/index.php/API:banners.xml
//  

using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
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
        
        public Banner(Banner O)
        {

            this.BannerId = O.BannerId;
            this.BannerPath = O.BannerPath;
            this.BannerType = O.BannerType;
            this.LanguageId = O.LanguageId;
            this.Resolution = O.Resolution;
            this.Rating = O.Rating;
            this.RatingCount = O.RatingCount;
            this.SeasonID = O.SeasonID;
            this.SeriesID = O.SeriesID;

            this.ThumbnailPath = O.ThumbnailPath;
            
            this.TheSeason = O.TheSeason;
            this.TheSeries = O.TheSeries;

        }

        public Banner(SeriesInfo ser, Season seas)
        {
            this.SetDefaults(ser, seas);
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
                this.SetDefaults(ser, seas);

                this.SeriesID = (int) codeHint;

                r.Read();
                if (r.Name != "Banner")
                    return;

                r.Read();
                while (!r.EOF)
                {
                    if ((r.Name == "Banner") && (!r.IsStartElement()))
                        break;

                    if (r.Name == "id")
                        this.BannerId = r.ReadElementContentAsInt();
                    else if (r.Name == "seriesid")
                        this.SeriesID = r.ReadElementContentAsInt(); // thetvdb series id
                    else if (r.Name == "seasonid")
                        this.SeasonID = r.ReadElementContentAsInt();
                    else if (r.Name == "BannerPath")
                        this.BannerPath = XMLHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "BannerType")
                        this.BannerType = r.ReadElementContentAsString();
                    else if (r.Name == "LanguageId")
                        this.LanguageId = r.ReadElementContentAsInt();
                    else if (r.Name == "Resolution")
                        this.Resolution = r.ReadElementContentAsString();
                    else if (r.Name == "Rating")
                        {
                        String sn = r.ReadElementContentAsString();
                        double.TryParse(sn, out this.Rating);
                        }
                    else if (r.Name == "RatingCount")
                        this.RatingCount  = r.ReadElementContentAsInt();
                    else if (r.Name == "Season")
                        this.SeasonID = r.ReadElementContentAsInt();
                    else if (r.Name == "ThumbnailPath") this.ThumbnailPath = r.ReadElementContentAsString();
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
                if (this.SeriesID != -1)
                    message += "\r\nSeries ID: " + this.SeriesID;
                if (this.BannerId != -1)
                    message += "\r\nBanner ID: " + this.BannerId;
                if (!string.IsNullOrEmpty(this.BannerPath))
                    message += "\r\nBanner Path: " + this.BannerPath;

                message += "\r\n" + e.Message;

                if (!args.Unattended) 
                    MessageBox.Show(message, "TVRename", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw new TVDBException(e.Message);
            }
        }

        public Banner(int seriesId, JObject json)
        {
            this.SetDefaults(null, null);
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

            this.SeriesID = seriesId;

            this.BannerPath = (string)json["fileName"];
            this.BannerId = (int)json["id"];
            this.BannerType = (string)json["keyType"];
            this.LanguageId = (json["languageId"] == null) ? -1 : (int)json["languageId"];
            
            double.TryParse((string)(json["ratingsInfo"]["average"]), out this.Rating);
            this.RatingCount = (int)(json["ratingsInfo"]["count"]);

            this.Resolution = (string)json["resolution"];
            int.TryParse((string)json["subKey"], out this.SeasonID);
            this.ThumbnailPath = (string)json["thumbnail"];
        }


        public int SeasonNumber
        {
            get
            {
                if (this.TheSeason != null)
                    return this.TheSeason.SeasonNumber;
                return -1;
            }
        }

        public bool SameAs(Banner  o)
        {
            return (this.BannerId == o.BannerId);
        }

        public bool isSeriesPoster()
        {
            return ((this.BannerType == "poster"));
        }

        public bool isSeriesBanner()
        {
            return ((this.BannerType == "series"));
        }

        public bool isSeasonPoster()
        {
            return ((this.BannerType == "season") );
        }

        public bool isSeasonBanner()
        {
            return ((this.BannerType == "seasonwide") );
        }

        public bool isFanart()
        {
            return ((this.BannerType == "fanart") );
        }

        public void SetDefaults(SeriesInfo ser, Season seas)
        {
            this.TheSeason = seas;
            this.TheSeries = ser;


            this.BannerId = -1;
            this.BannerPath = "";
            this.BannerType = "";
            this.LanguageId = -1;
            this.Resolution = "";
            this.Rating = -1;
            this.RatingCount = 0;
            this.SeasonID = -1;
            this.SeriesID = -1;

            this.ThumbnailPath = "";

        }
                
        public void SetSeriesSeason(SeriesInfo ser, Season seas)
        {
            this.TheSeason = seas;
            this.TheSeries = ser;
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

            XMLHelper.WriteElementToXML(writer,"id",this.BannerId);
            XMLHelper.WriteElementToXML(writer,"BannerPath",this.BannerPath);
            XMLHelper.WriteElementToXML(writer,"BannerType",this.BannerType);
            XMLHelper.WriteElementToXML(writer, "LanguageId", this.LanguageId);
            XMLHelper.WriteElementToXML(writer,"Resolution",this.Resolution);
            XMLHelper.WriteElementToXML(writer,"Rating",this.Rating);
            XMLHelper.WriteElementToXML(writer,"RatingCount",this.RatingCount);
            XMLHelper.WriteElementToXML(writer,"Season",this.SeasonID);  
            XMLHelper.WriteElementToXML(writer,"ThumbnailPath",this.ThumbnailPath);

            writer.WriteEndElement(); //Banner
        }
    }
}

