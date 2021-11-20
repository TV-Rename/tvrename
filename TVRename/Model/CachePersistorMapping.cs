using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    internal static class CachePersistorMapping
    {
        public static void WriteEpisodeXml([NotNull] this XmlWriter writer, [NotNull] Episode e)
        {
            writer.WriteStartElement("Episode");

            writer.WriteElement("id", e.EpisodeId);
            writer.WriteElement("seriesid", e.SeriesId);
            writer.WriteElement("airedSeasonID", e.SeasonId);
            writer.WriteElement("airedEpisodeNumber", e.AiredEpNum);
            writer.WriteElement("SeasonNumber", e.AiredSeasonNumber);
            writer.WriteElement("dvdEpisodeNumber", e.DvdEpNum, true);
            writer.WriteElement("dvdSeason", e.DvdSeasonNumber, true);
            writer.WriteElement("lastupdated", e.SrvLastUpdated);
            writer.WriteElement("Overview", e.Overview?.Trim());
            writer.WriteElement("LinkURL", e.LinkUrl?.Trim());
            writer.WriteElement("Runtime", e.Runtime?.Trim());
            writer.WriteElement("Rating", e.EpisodeRating);
            writer.WriteElement("GuestStars", e.EpisodeGuestStars, true);
            writer.WriteElement("EpisodeDirector", e.EpisodeDirector, true);
            writer.WriteElement("Writer", e.Writer, true);
            writer.WriteElement("EpisodeName", e.MName, true);

            writer.WriteElement("FirstAired", e.FirstAired?.ToString("yyyy-MM-dd"), true);
            writer.WriteElement("AirTime", e.AirTime?.ToString("HH:mm"), true);
            writer.WriteElement("AirTime", e.AirStamp);

            writer.WriteElement("DvdChapter", e.DvdChapter);
            writer.WriteElement("DvdDiscId", e.DvdDiscId, true);
            writer.WriteElement("AirsBeforeSeason", e.AirsBeforeSeason);
            writer.WriteElement("AirsBeforeEpisode", e.AirsBeforeEpisode);
            writer.WriteElement("AirsAfterSeason", e.AirsAfterSeason);
            writer.WriteElement("SiteRatingCount", e.SiteRatingCount);
            writer.WriteElement("AbsoluteNumber", e.AbsoluteNumber);
            writer.WriteElement("ProductionCode", e.ProductionCode, true);
            writer.WriteElement("ImdbCode", e.ImdbCode, true);
            writer.WriteElement("ShowUrl", e.ShowUrl, true);
            writer.WriteElement("Filename", e.Filename, true);

            writer.WriteEndElement(); //Episode
        }
        [NotNull]
        public static Episode CreateEpisode([NotNull] this XElement r)
        {
            return new Episode
            {
                // </Episode>
                //  blah blah
                //  <id>...</id>
                // <Episode>
                SeriesId = r.ExtractInt("seriesid", -1), // key to the cachedSeries
                EpisodeId = r.ExtractInt("id", -1),
                SeasonId = r.ExtractInt("airedSeasonID") ?? r.ExtractInt("seasonid", -1),
                AiredEpNum = r.ExtractInt("airedEpisodeNumber") ?? r.ExtractInt("EpisodeNumber", -1),
                DvdEpNum = ExtractAndParse(r, "dvdEpisodeNumber"),
                ReadAiredSeasonNum = ExtractAndParse(r, "SeasonNumber"),
                ReadDvdSeasonNum = ExtractAndParse(r, "dvdSeason"),
                SrvLastUpdated = r.ExtractLong("lastupdated", -1),
                Overview = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Overview"))),
                LinkUrl = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("LinkURL")),
                Runtime = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Runtime")),
                EpisodeRating = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Rating")),
                EpisodeGuestStars = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("GuestStars")),
                EpisodeDirector = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("EpisodeDirector")),
                Writer = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Writer")),
                MName = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("EpisodeName"))),

                AirStamp = r.ExtractDateTime("AirStamp"),
                AirTime = JsonHelper.ParseAirTime(r.ExtractString("Airs_Time")),

                DvdDiscId = r.ExtractString("DvdDiscId"),
                Filename = r.ExtractStringOrNull("Filename") ?? r.ExtractString("filename"),
                ShowUrl = r.ExtractStringOrNull("ShowUrl") ?? r.ExtractString("showUrl"),
                ImdbCode = r.ExtractStringOrNull("ImdbCode") ?? r.ExtractStringOrNull("IMDB_ID") ?? r.ExtractString("imdbId"),
                ProductionCode = r.ExtractStringOrNull("ProductionCode") ?? r.ExtractString("productionCode"),

                DvdChapter = r.ExtractInt("DvdChapter") ?? r.ExtractInt("dvdChapter"),
                AirsBeforeSeason = r.ExtractInt("AirsBeforeSeason") ?? r.ExtractInt("airsBeforeSeason") ?? r.ExtractInt("airsbefore_season"),
                AirsBeforeEpisode = r.ExtractInt("AirsBeforeEpisode") ?? r.ExtractInt("airsBeforeEpisode") ?? r.ExtractInt("airsbefore_episode"),
                AirsAfterSeason = r.ExtractInt("AirsAfterSeason"),
                SiteRatingCount = r.ExtractInt("SiteRatingCount") ?? r.ExtractInt("siteRatingCount"),
                AbsoluteNumber = r.ExtractInt("AbsoluteNumber"),
                FirstAired = JsonHelper.ParseFirstAired(r.ExtractString("FirstAired")),
            };
        }

        private static int ExtractAndParse([NotNull] XElement r, string key)
        {
            string value = r.ExtractString(key);
            int.TryParse(value, out int intValue);
            return intValue;
        }
    }
}
