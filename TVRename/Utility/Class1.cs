using System;
using System.Collections.Generic;
using System.Web;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.Utility
{
    internal static class ShowHtmlHelper
    {
        public static string GetShowHtmlOverview(this ShowItem si)
        {
            string body = "";
            SeriesInfo ser = si.TheSeries();

            List<string> skip = new List<string>
            {
                "Actors",
                "banner",
                "Overview",
                "overview",
                "Airs_Time",
                "airsTime",
                "Airs_DayOfWeek",
                "airsDayOfWeek",
                "fanart",
                "poster",
                "zap2it_id",
                "zap2itId",
                "id",
                "seriesName",
                "lastUpdated",
                "updatedBy"
            };

            if ((!String.IsNullOrEmpty(ser.GetSeriesWideBannerPath())) &&
                (!String.IsNullOrEmpty(TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()))))
                body += "<img width=758 height=140 src=\"" + TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()) +
                        "\"><br/>";

            body += $"<h1><A HREF=\"{TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true)}\">{si.ShowName}</A> </h1>";

            body += "<h2>Overview</h2>" + ser.GetOverview(); //get overview in either format

            bool first = true;
            foreach (string aa in ser.GetActors())
            {
                if (String.IsNullOrEmpty(aa)) continue;
                if (!first)
                    body += ", ";
                else
                    body += "<h2>Actors</h2>";

                body += "<A HREF=\"http://www.imdb.com/find?s=nm&q=" + aa + "\">" + aa + "</a>";
                first = false;
            }

            string airsTime = ser.GetAirsTime();
            string airsDay = ser.GetAirsDay();
            if ((!String.IsNullOrEmpty(airsTime)) && (!String.IsNullOrEmpty(airsDay)))
            {
                body += "<h2>Airs</h2> " + airsTime + " " + airsDay;
                string net = ser.GetNetwork();
                if (!String.IsNullOrEmpty(net))
                {
                    skip.Add("Network");
                    skip.Add("network");
                    body += ", " + net;
                }
            }

            bool firstInfo = true;
            foreach (KeyValuePair<string, string> kvp in ser.Items)
            {
                if (firstInfo)
                {
                    body += "<h2>Information<table border=0>";
                    firstInfo = false;
                }

                if (skip.Contains(kvp.Key)) continue;

                if (((kvp.Key == "SeriesID") || (kvp.Key == "seriesId")) & (kvp.Value != ""))
                    body += "<tr><td width=120px>tv.com</td><td><A HREF=\"http://www.tv.com/show/" + kvp.Value +
                            "/summary.html\">Visit</a></td></tr>";
                else if ((kvp.Key == "IMDB_ID") || (kvp.Key == "imdbId"))
                    body += "<tr><td width=120px>imdb.com</td><td><A HREF=\"http://www.imdb.com/title/" +
                            kvp.Value + "\">Visit</a></td></tr>";
                else if (kvp.Value != "")
                    body += "<tr><td width=120px>" + kvp.Key + "</td><td>" + kvp.Value + "</td></tr>";
            }

            if (!firstInfo)
                body += "</table>";

            return body;
        }

        public static string GetShowImagesHtmlOverview(this ShowItem si)
        {
            SeriesInfo ser = si.TheSeries();

            string body =
                $"<h1><A HREF=\"{TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true)}\">{si.ShowName}</A> </h1>";

            body += ImageSection("Show Banner", 758, 140, ser.GetSeriesWideBannerPath());
            body += ImageSection("Show Poster", 350, 500, ser.GetSeriesPosterPath());
            body += ImageSection("Show Fanart", 960, 540, ser.GetSeriesFanartPath());
            return body;
        }

        private static string ImageSection(string title, int width, int height, string bannerPath)
        {
            if (String.IsNullOrEmpty(bannerPath)) return "";

            string url = TheTVDB.GetImageURL(bannerPath);

            return String.IsNullOrEmpty(url) ? "" : $"<h2>{title}</h2><img width={width} height={height} src=\"{url}\"><br/>";
        }

        public static string GetSeasonHtmlOverview(this ShowItem si,Season s)
        {
            SeriesInfo ser = s.TheSeries;
            int snum = s.SeasonNumber;
            string body = "";

            if (!String.IsNullOrEmpty(ser.GetSeriesWideBannerPath()) &&
                !String.IsNullOrEmpty(TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath())))
                body += "<img width=758 height=140 src=\"" + TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()) +
                        "\"><br/>";

            List<ProcessedEpisode> eis;
            // int snum = s.SeasonNumber;
            if (si.SeasonEpisodes.ContainsKey(snum))
                eis = si.SeasonEpisodes[snum]; // use processed episodes if they are available
            else
                eis = ShowItem.ProcessedListFromEpisodes(s.Episodes, si);

            string seasText = SeasonName(si, snum);

            if ((eis.Count > 0) && (eis[0].SeasonId > 0))
                seasText = " - <A HREF=\"" + TheTVDB.Instance.WebsiteUrl(ser.TVDBCode, eis[0].SeasonId, false) + "\">" +
                           seasText + "</a>";
            else
                seasText = " - " + seasText;

            body += "<h1><A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true) + "\">" + si.ShowName +
                    "</A>" + seasText + "</h1>";

            DirFilesCache dfc = new DirFilesCache();
            foreach (ProcessedEpisode ei in eis)
            {
                string epl = ei.NumsAsString();

                string episodeUrl = TheTVDB.Instance.WebsiteUrl(ei.SeriesId, ei.SeasonId, ei.EpisodeId);

                body += "<A href=\"" + episodeUrl + "\" name=\"ep" + epl + "\">"; // anchor
                if (si.DVDOrder && snum == 0)
                {
                    body += "<b>" + ei.Name + "</b>";
                }
                else
                    body += "<b>" + HttpUtility.HtmlEncode(CustomName.NameForNoExt(ei, CustomName.OldNStyle(6))) +
                            "</b>";

                body += "</A>"; // anchor
                if (si.UseSequentialMatch && (ei.OverallNumber != -1))
                    body += " (#" + ei.OverallNumber + ")";

                List<FileInfo> fl = TVDoc.FindEpOnDisk(dfc, ei);
                if (fl != null)
                {
                    foreach (FileInfo fi in fl)
                    {
                        string urlFilename = HttpUtility.UrlEncode(fi.FullName);
                        body += $" <A HREF=\"watch://{urlFilename}\" class=\"search\">Watch</A>";
                        body += $" <A HREF=\"explore://{urlFilename}\" class=\"search\">Show in Explorer</A>";
                    }
                }
                else body += " <A HREF=\"" + TVSettings.Instance.BTSearchURL(ei) + "\" class=\"search\">Search</A>";

                DateTime? dt = ei.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    body += "<p>" + dt.Value.ToShortDateString() + " (" + ei.HowLong() + ")";

                body += "<p><p>";

                if ((TVSettings.Instance.ShowEpisodePictures) ||
                    (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired"))
                {
                    body += "<table><tr>";
                    body += "<td width=100% valign=top>" + GetOverview(ei) + "</td><td width=300 height=225>";
                    // 300x168 / 300x225
                    if (!String.IsNullOrEmpty(ei.GetFilename()))
                        body += "<img src=" + TheTVDB.GetImageURL(ei.GetFilename()) + ">";

                    body += "</td></tr></table>";
                }
                else
                    body += GetOverview(ei);

                body += "<p><hr><p>";
            } // for each episode in this season

            return body;
        }

        private static string GetOverview(ProcessedEpisode ei)
        {
            string overviewString = ei.Overview;

            if (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired")
                overviewString = "[Spoilers Hidden]";

            bool firstInfo = true;
            foreach (KeyValuePair<string, string> kvp in ei.OtherItems())
            {
                if (firstInfo)
                {
                    overviewString += "<table border=0>";
                    firstInfo = false;
                }

                if ((kvp.Value != "") && kvp.Value != "0")
                {
                    if (((kvp.Key == "IMDB_ID") || (kvp.Key == "imdbId")))
                        overviewString += "<tr><td width=120px>imdb.com</td><td><A HREF=\"http://www.imdb.com/title/" +
                                          kvp.Value + "\">Visit</a></td></tr>";
                    else if (((kvp.Key == "showUrl")))
                        overviewString += "<tr><td width=120px>Link</td><td><A HREF=\"" + kvp.Value +
                                          "\">Visit</a></td></tr>";
                    else
                        overviewString += "<tr><td width=120px>" + kvp.Key + "</td><td>" + kvp.Value + "</td></tr>";
                }
            }

            if (!firstInfo)
                overviewString += "</table>";

            return overviewString;
        }

        public static string GetSeasonImagesHTMLOverview(this ShowItem si, Season s)
        {
            SeriesInfo ser = s.TheSeries;
            int snum = s.SeasonNumber;
            string body = "";

            List<ProcessedEpisode> eis = null;
            // int snum = s.SeasonNumber;
            if (si.SeasonEpisodes.ContainsKey(snum))
                eis = si.SeasonEpisodes[snum]; // use processed episodes if they are available
            else
                eis = ShowItem.ProcessedListFromEpisodes(s.Episodes, si);

            string seasText = snum == 0
                ? TVSettings.Instance.SpecialsFolderName
                : (TVSettings.Instance.defaultSeasonWord + " " + snum);

            if ((eis.Count > 0) && (eis[0].SeasonId > 0))
                seasText = " - <A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TVDBCode, eis[0].SeasonId, false) + "\">" +
                           seasText + "</a>";
            else
                seasText = " - " + seasText;

            body += "<h1><A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true) + "\">" + si.ShowName +
                    "</A>" + seasText + "</h1>";

            if (TVSettings.Instance.NeedToDownloadBannerFile())
            {
                body += ShowHtmlHelper.ImageSection("Series Banner", 758, 140, ser.GetSeasonWideBannerPath(snum));
                body += ShowHtmlHelper.ImageSection("Series Poster", 350, 500, ser.GetSeasonBannerPath(snum));
            }
            else
            {
                body +=
                    "<h2>Images are not being downloaded for this series. Please see Options -> Preferences -> Media Center to reconfigure.</h2>";
            }

            return body;
        }

        public static string SeasonName(ShowItem si, int snum)
        {
            string nodeTitle;
            if (si.DVDOrder)
            {
                nodeTitle = snum == 0
                    ? "Not Available on DVD"
                    : "DVD " + TVSettings.Instance.defaultSeasonWord + " " + snum;
            }
            else
            {
                nodeTitle = snum == 0
                    ? TVSettings.Instance.SpecialsFolderName
                    : TVSettings.Instance.defaultSeasonWord + " " + snum;
            }

            return nodeTitle;
        }
    }
}
