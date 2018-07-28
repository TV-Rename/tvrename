using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal static class ShowHtmlHelper
    {
        public static string CreateOldPage(string body)
        {
            Color col = Color.FromName("ButtonFace");

            string css = "* { font-family: Tahoma, Arial; font-size 10pt; } " + "a:link { color: black } " +
                         "a:visited { color:black } " + "a:hover { color:#000080 } " + "a:active { color:black } " +
                         "a.search:link { color: #800000 } " + "a.search:visited { color:#800000 } " +
                         "a.search:hover { color:#000080 } " + "a.search:active { color:#800000 } " +
                         "* {background-color: #" + col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2") +
                         "}" + "* { color: black }";

            string html = "<html><head><meta charset=\"UTF-8\"><STYLE type=\"text/css\">" + css + "</style>";

            html += "</head><body>";
            html += body;
            html += "</body></html>";
            return html;
        }
        public static string GetShowHtmlOverview(this ShowItem si)
        {
            return TVSettings.Instance.IncludeBetaUpdates() ? si.GetShowHtmlOverviewNew() : CreateOldPage(si.GetShowHtmlOverviewOld());
        }

        public static string GetSeasonHtmlOverview(this ShowItem si, Season s)
        {
            return TVSettings.Instance.IncludeBetaUpdates() ? si.GetSeasonHtmlOverviewNew(s) : CreateOldPage(si.GetSeasonHtmlOverviewOld(s));
        }

        private static string GetShowHtmlOverviewNew(this ShowItem si)
        {
            Color col = Color.FromName("ButtonFace");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HTMLHeader(10,col));
            sb.AppendShow(si);
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        private static void AppendShow(this StringBuilder sb,ShowItem si)
        {
            SeriesInfo ser = si.TheSeries();
            string horizontalBanner = CreateHorizontalBannerHtml(ser);
            string poster = CreatePosterHtml(ser);
            int minYear = si.TheSeries().MinYear();
            int maxYear = si.TheSeries().MaxYear();
            string yearRange = (minYear == maxYear) ? minYear.ToString() : minYear + "-" + maxYear;
            string episodeSummary = si.TheSeries().AiredSeasons.Sum(pair => pair.Value.Episodes.Count).ToString();
            string stars = StarRating(si.TheSeries().GetSiteRating());
            string genreIcons = string.Join("&nbsp;", si.TheSeries().GetGenres().Select(GenreIconHtml));
            bool ratingIsNumber = float.TryParse(si.TheSeries().GetSiteRating(), out float rating);
            string siteRating = ratingIsNumber && rating > 0 ? rating + "/10" : "";
            string runTimeHtml = string.IsNullOrWhiteSpace(ser.GetRuntime()) ?string.Empty: $"<br/> {ser.GetRuntime()} min";
            string actorLinks = string.Join(", ", si.TheSeries().GetActors().Select(ActorLinkHtml));
            string tvdbLink = TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true);
            string airsTime = DateTime.Parse(ser.GetAirsTime()).ToString("h tt");
            string airsDay = ser.GetAirsDay();
            string dayTime = $"{airsDay} {airsTime}";

            string tvLink =  string.IsNullOrWhiteSpace(ser.GetSeriesId()) ?string.Empty: "http://www.tv.com/show/" + ser.GetSeriesId() + "/summary.html"; 
            string imdbLink = string.IsNullOrWhiteSpace(ser.GetImdb()) ?string.Empty: "http://www.imdb.com/title/" + ser.GetImdb();

            sb.AppendLine($@"<div class=""card card-body"">
            	<div class=""text-center"">
	             {horizontalBanner}
                  <div class=""row"">
                   <div class=""col-md-4"">
                    {poster}
                   </div>
                   <div class=""col-md-8 d-flex flex-column"">
                    <div class=""row"">
                     <div class=""col-md-8""><h1>{ser.Name}</h1></div>
                     <div class=""col-md-4 text-right""><h6>{yearRange} ({si.TheSeries().GetStatus()})</h6><small class=""text-muted"">{episodeSummary} Episodes{runTimeHtml}</small></div>
                    </div>
                    <div><p class=""lead"">{ser.GetOverview()}</p></div>
			        <div><blockquote>{actorLinks}</blockquote></div> 
		            <div>
			         {CreateButtonLink(tvdbLink, "TVDB.com")}
			         {CreateButtonLink(imdbLink, "IMDB.com")}
			         {CreateButtonLink(tvLink, "TV.com")}
			        </div>
		            <div class=""row align-items-bottom flex-grow-1"">
                     <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating} (From {ser.GetSiteRatingVotes()} Votes)</div>
                     <div class=""col-md-4 align-self-end text-center"">{si.TheSeries().GetContentRating()}<br>{si.TheSeries().GetNetwork()}, {dayTime}</div>
                     <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{string.Join(", ", si.TheSeries().GetGenres())}</div>
                    </div>
                   </div>
                  </div>
                 </div>");
        }

        private static string CreateButtonLink(string link, string label)
        {
            return string.IsNullOrWhiteSpace(link) ? string.Empty : $"<a href=\"{link}\" class=\"btn btn-outline-secondary\" role=\"button\" aria-disabled=\"true\">{label}</a>";
        }

        private static string CreateHorizontalBannerHtml(SeriesInfo ser)
        {
            if ((!string.IsNullOrEmpty(ser.GetSeriesWideBannerPath())) &&
                (!string.IsNullOrEmpty(TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()))))
                return  $"<img class=\"rounded\" src=\"{TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath())}\"><br/>&nbsp;</div>";

            return string.Empty;
        }

        private static string CreateHorizontalBannerHtml(this Season s)
        {
            if ((!string.IsNullOrEmpty(s.GetWideBannerPath())) &&
                (!string.IsNullOrEmpty(TheTVDB.GetImageURL(s.GetWideBannerPath()))))
                return $"<img class=\"rounded w-100\" src=\"{TheTVDB.GetImageURL(s.GetWideBannerPath())}\"><br/>";

            return string.Empty;
        }

        private static string CreatePosterHtml(SeriesInfo ser)
        {
            if ((!string.IsNullOrEmpty(ser.GetSeriesPosterPath())) &&
                (!string.IsNullOrEmpty(TheTVDB.GetImageURL(ser.GetSeriesPosterPath()))))
                return $"<img class=\"show-poster rounded w-100\" src=\"{TheTVDB.GetImageURL(ser.GetSeriesPosterPath())}\" alt=\"{ser.Name} Show Poster\">";
            return string.Empty;
        }

        private static string ScreenShotHtml(this ProcessedEpisode ei)
        {
            if (!TVSettings.Instance.ShowEpisodePictures) return string.Empty;
            if (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired") return string.Empty;
            if (string.IsNullOrEmpty(ei.Filename)) return string.Empty;
            if (string.IsNullOrWhiteSpace(TheTVDB.GetImageURL(ei.Filename))) return string.Empty;

            return $"<img class=\"rounded w-100\" src=\"{TheTVDB.GetImageURL(ei.Filename)}\" alt=\"{ei.Name} Screenshot\">";
        }

        private static string GetSeasonHtmlOverviewNew(this ShowItem si, Season s)
        {
            StringBuilder sb = new StringBuilder();
            DirFilesCache dfc = new DirFilesCache();
            Color col = Color.FromName("ButtonFace");
            sb.AppendLine(HTMLHeader(10,col));
            sb.AppendSeason(s,si);
            foreach (ProcessedEpisode ep in GetBestEpisodes(si,s))
            {
                List<FileInfo> fl = TVDoc.FindEpOnDisk(dfc, ep);
                sb.AppendEpisode(ep,fl);
            }
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        private static void AppendSeason(this StringBuilder sb, Season s, ShowItem si)
        {
            SeriesInfo ser = s.TheSeries;
            string seasonLink = TheTVDB.Instance.WebsiteUrl(ser.TVDBCode, s.SeasonId, false);
            string showLink = TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true);

            sb.AppendLine($@"<div class=""card card-body"">
				{s.CreateHorizontalBannerHtml()}
				<br/>
				<h1><A HREF=""{showLink}"">{ser.Name}</A> - <A HREF=""{seasonLink}"">{SeasonName(si, s.SeasonNumber)}</a></h1>
				</div>");
        }

        private static void AppendEpisode(this StringBuilder sb, ProcessedEpisode ep, IReadOnlyCollection<FileInfo> fl)
        {
            string stars = StarRating(ep.EpisodeRating);
            string episodeUrl = TheTVDB.Instance.WebsiteUrl(ep.SeriesId, ep.SeasonId, ep.EpisodeId);
            bool ratingIsNumber = float.TryParse(ep.EpisodeRating, out float rating);
            string siteRating = ratingIsNumber && rating > 0 ? rating + "/10" : "";
            if (!string.IsNullOrWhiteSpace(ep.SiteRatingCount))
                siteRating += $" (From {ep.SiteRatingCount} Votes)";

            string imdbLink = string.IsNullOrWhiteSpace(ep.ImdbCode) ? string.Empty : "http://www.imdb.com/title/" + ep.ImdbCode;
            string productionCode = string.IsNullOrWhiteSpace(ep.ProductionCode)
                ? string.Empty
                : "Production Code <br/>" + ep.ProductionCode;

            string episodeDescriptor =  CustomEpisodeName.NameForNoExt(ep, CustomEpisodeName.OldNStyle(6)); // may need to include (si.DVDOrder && snum == 0)? ep.Name:
            string writersHtml = string.IsNullOrWhiteSpace(ep.Writer)?string.Empty:"<b>Writers:</b> "+ string.Join(", ", ep.Writers);
            string directorsHtml = string.IsNullOrWhiteSpace(ep.EpisodeDirector) ? string.Empty : "<b>Directors:</b> " + string.Join(", ",ep.Directors);
            string possibleBreak = (string.IsNullOrWhiteSpace(writersHtml) || string.IsNullOrWhiteSpace(directorsHtml))
                ? string.Empty
                : "<br />";

            string searchButton = (fl == null)
                ? CreateButton(TVSettings.Instance.BTSearchURL(ep), "<i class=\"fas fa-search\"></i>")
                : string.Empty;

            string viewButton = string.Empty;
            string explorerButton = string.Empty;
            if (fl != null)
            { 
            foreach (FileInfo fi in fl)
                {
                    string urlFilename = HttpUtility.UrlEncode(fi.FullName);
                    viewButton += CreateButton($"watch://{urlFilename}", "<i class=\"far fa-eye\"></i>");
                    explorerButton += CreateButton($"explore://{urlFilename}", "<i class=\"far fa-folder-open\"></i>");
                }
            }

            string tvdbButton = CreateButton(episodeUrl, "TVDB.com");
            string imdbButton = CreateButton(imdbLink, "IMDB.com");
            string tvButton = CreateButton(ep.ShowUrl, "TV.com");

            sb.AppendLine($@"
                <div class=""card card-body"">
                 <div class=""row"">
                  <div class=""col-md-5"">{ep.ScreenShotHtml()}</div>
                   <div class=""col-md-7 d-flex flex-column"">
                    <div class=""row"">
                     <div class=""col-md-8""><h2>{episodeDescriptor}</h2></div>
                     <div class=""col-md-4 text-right"">{ep.DateDetailsHtml()}</div>
                    </div>
				   <div>
                    <blockquote>
                     {writersHtml}{possibleBreak}
                     {directorsHtml}
                    </blockquote>
                   </div>
                   <div><p class=""lead"">{ep.HiddenOverview()}</p></div>
                   <div>
                    {searchButton}{viewButton}{explorerButton}{tvdbButton}{imdbButton}{tvButton}
                   </div>
		           <div class=""row align-items-bottom flex-grow-1"">
                    <div class=""col-md-6 align-self-end"">{stars}<br>{siteRating}</div>
                    <div class=""col-md-6 align-self-end text-right"">{productionCode}</div>
                   </div>
                  </div>
                 </div>
                </div>");
        }

        private static string CreateButton(string link, string text)
        {
            if (string.IsNullOrWhiteSpace(link)) return string.Empty;

            return $"<a href=\"{link}\" class=\"btn btn-outline-secondary\" role=\"button\" aria-disabled=\"true\">{text}</i></a>";
        }

        private static string DateDetailsHtml(this ProcessedEpisode ei)
        {
            DateTime? dt = ei.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                return $"<h6>{dt.Value.ToShortDateString()}</h6><small class=\"text-muted\">({ei.HowLong()})</small>";

            return string.Empty;
        }

        private static string GetShowHtmlOverviewOld(this ShowItem si)
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

        private static string GetSeasonHtmlOverviewOld(this ShowItem si,Season s)
        {
            SeriesInfo ser = s.TheSeries;
            int snum = s.SeasonNumber;
            string body = "";

            if (!String.IsNullOrEmpty(ser.GetSeriesWideBannerPath()) &&
                !String.IsNullOrEmpty(TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath())))
                body += "<img width=758 height=140 src=\"" + TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()) +
                        "\"><br/>";

            List<ProcessedEpisode> eis = GetBestEpisodes(si, s);

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
                    body += "<b>" + HttpUtility.HtmlEncode(CustomEpisodeName.NameForNoExt(ei, CustomEpisodeName.OldNStyle(6))) +
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
                    if (!String.IsNullOrEmpty(ei.Filename))
                        body += "<img src=" + TheTVDB.GetImageURL(ei.Filename) + ">";

                    body += "</td></tr></table>";
                }
                else
                    body += GetOverview(ei);

                body += "<p><hr><p>";
            } // for each episode in this season

            return body;
        }

        private static List<ProcessedEpisode> GetBestEpisodes(ShowItem si, Season s)
        {
            return si.SeasonEpisodes.ContainsKey(s.SeasonNumber)
                ? si.SeasonEpisodes[s.SeasonNumber]
                : ShowItem.ProcessedListFromEpisodes(s.Episodes, si);
        }

        private static string GetOverview(ProcessedEpisode ei)
        {
            string overviewString = HiddenOverview(ei);

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

        private static string HiddenOverview(this ProcessedEpisode ei)
        {
            string overviewString = ei.Overview;

            if (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired")
                overviewString = "[Spoilers Hidden]";
            return overviewString;
        }

        public static string GetSeasonImagesHtmlOverview(this ShowItem si, Season s)
        {
            SeriesInfo ser = s.TheSeries;
            int snum = s.SeasonNumber;
            string body = "";

            // int snum = s.SeasonNumber;
            List<ProcessedEpisode> eis = si.SeasonEpisodes.ContainsKey(snum)
                ? si.SeasonEpisodes[snum]
                : ShowItem.ProcessedListFromEpisodes(s.Episodes, si);

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
                body += ImageSection("Series Banner", 758, 140, ser.GetSeasonWideBannerPath(snum));
                body += ImageSection("Series Poster", 350, 500, ser.GetSeasonBannerPath(snum));
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

        internal static string GenreIconHtml(string genre)
        {
            string[] availbleIcons =
            {
                "Action", "Adventure", "Animation", "Children", "Comedy", "Crime", "Documentary", "Drama", "Family",
                "Fantasy", "Food", "Horror", "Mini-Series", "Mystery", "Reality", "Romance", "Science-Fiction", "Soap",
                "Talk Show", "Thriller", "Travel", "War", "Western"
            };

            const string root = "https://www.tvrename.com/assets/images/GenreIcons/";

            return availbleIcons.Contains(genre)
                ? $@"<img width=""30"" height=""30"" src=""{root}{genre}.svg"" alt=""{genre}"">"
                : "";
        }

        private static string ActorLinkHtml(string actor)
        {
            return $@"<a href=""http://www.imdb.com/find?s=nm&q={actor}"">{actor}</a>";
        }

        internal static string StarRating(string rating)
        {
            return StarRating(float.TryParse(rating, out float f) ? f / 2 : 3);
        }

        private static string StarRating(float f)
        {
            const string star = @"<i class=""fas fa-star""></i>";
            const string halfstar = @"<i class=""fas fa-star-half""></i>";

            if (f < .25) return "";
            if (f <= .75) return halfstar;
            if (f > 1) return star + StarRating(f - 1);
            return star;
        }

        // ReSharper disable once InconsistentNaming
        internal static string HTMLHeader(int size,Color backgroundColour)
        {
            string hexColour = "#"+backgroundColour.R.ToString("X2") + backgroundColour.G.ToString("X2") + backgroundColour.B.ToString("X2");
            return @"<!DOCTYPE html>
                <html><head>
                <meta charset=""utf-8"">
                <meta name = ""viewport"" content = ""width=device-width, initial-scale=1.0"" >
                <title> TV Rename - Show Summary</title>
                <link rel = ""stylesheet"" href = ""http://maxcdn.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css"" />
                <link rel = ""stylesheet"" href = ""https://use.fontawesome.com/releases/v5.0.13/css/all.css"" />
                </head >"
                + $"<body style=\"background-color: {hexColour}\" ><div class=\"col-sm-{size} offset-sm-{(12 - size) / 2}\">";
        }

        // ReSharper disable once InconsistentNaming
        internal static string HTMLFooter()
        {
            return @"
                </div>
                <script src=""http://ajax.googleapis.com/ajax/libs/jquery/2.2.4/jquery.min.js""></script>
                <script src = ""http://cdnjs.cloudflare.com/ajax/libs/popper.js/1.13.0/umd/popper.min.js"" ></script>
                <script src = ""http://maxcdn.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js"" ></script>
                <script >$(document).ready(function(){ })</script>
                </body>
                </html>
                ";
        }
    }
}
