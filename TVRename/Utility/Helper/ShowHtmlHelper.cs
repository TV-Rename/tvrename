using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
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

        public static string GetShowHtmlOverview(this ShowItem si,bool includeDirectoryLinks)
        {
            Color col = Color.FromName("ButtonFace");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HTMLHeader(10,col));
            sb.AppendShow(si,col,includeDirectoryLinks);
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        private static void AppendShow(this StringBuilder sb,ShowItem si, Color backgroundColour, bool includeDirectoryLinks)
        {
            if (si == null) return;

            SeriesInfo ser = si.TheSeries();
            string horizontalBanner = CreateHorizontalBannerHtml(ser);
            string poster = CreatePosterHtml(ser);
            int minYear = si.TheSeries().MinYear;
            int maxYear = si.TheSeries().MaxYear;
            string yearRange = (minYear == maxYear) ? minYear.ToString() : minYear + "-" + maxYear;
            string episodeSummary = si.TheSeries().AiredSeasons.Sum(pair => pair.Value.Episodes.Count).ToString();
            string stars = StarRating(si.TheSeries().SiteRating/2);
            string genreIcons = string.Join("&nbsp;", si.TheSeries().Genres().Select(GenreIconHtml));
            string siteRating = si.TheSeries().SiteRating > 0 ? si.TheSeries().SiteRating+ "/10" : "";
            string runTimeHtml = string.IsNullOrWhiteSpace(ser.Runtime) ? string.Empty : $"<br/> {ser.Runtime} min";
            string actorLinks = string.Join(", ", si.TheSeries().GetActors().Select(ActorLinkHtml));
            string tvdbLink = TheTVDB.Instance.WebsiteUrl(si.TvdbCode, -1, true);
            string airsTime = ParseAirsTime(ser);
            string airsDay = ser.AirsDay;
            string dayTime = $"{airsDay} {airsTime}";

            string tvLink = string.IsNullOrWhiteSpace(ser.SeriesId) ? string.Empty : "http://www.tv.com/show/" + ser.SeriesId+ "/summary.html";
            string imdbLink = string.IsNullOrWhiteSpace(ser.Imdb) ? string.Empty : "http://www.imdb.com/title/" + ser.Imdb;

            string urlFilename = includeDirectoryLinks
                ? Uri.EscapeDataString(si.GetBestFolderLocationToOpen())
                : string.Empty;
            string explorerButton = includeDirectoryLinks
                ? CreateButton($"{UI.EXPLORE_PROXY}{urlFilename}", "<i class=\"far fa-folder-open\"></i>", "Open Containing Folder")
                : string.Empty;

            sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                <div class=""text-center"">
	             {horizontalBanner}
                </div>
                  <div class=""row"">
                   <div class=""col-md-4"">
                    {poster}
                   </div>
                   <div class=""col-md-8 d-flex flex-column"">
                    <div class=""row"">
                     <div class=""col-md-8""><h1>{ser.Name}</h1></div>
                     <div class=""col-md-4 text-right""><h6>{yearRange} ({si.TheSeries().Status})</h6><small class=""text-muted"">{episodeSummary} Episodes{runTimeHtml}</small></div>
                    </div>
                    <div><p class=""lead"">{ser.Overview}</p></div>
			        <div><blockquote>{actorLinks}</blockquote></div> 
		            <div>
                     {explorerButton}
			         {CreateButton(tvdbLink, "TVDB.com", "View on TVDB")}
			         {CreateButton(imdbLink, "IMDB.com", "View on IMDB")}
			         {CreateButton(tvLink, "TV.com", "View on TV.com")}
			        </div>
		            <div>
                        &nbsp;
			        </div>
		            <div class=""row align-items-bottom"">
                     <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}{AddRatingCount(ser.SiteRatingVotes)}</div>
                     <div class=""col-md-4 align-self-end text-center"">{si.TheSeries().ContentRating}<br>{si.TheSeries().Network}, {dayTime}</div>
                     <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{string.Join(", ", si.TheSeries().Genres())}</div>
                    </div>
                   </div>
                  </div>
                 </div>");
            //Ideally we'd have <div class=""row align-items-bottom flex-grow-1""> in there as it looks better, but a bug in IE prevents it from looking correct
        }

        private static string ParseAirsTime(SeriesInfo ser)
        {
            return ser.AirsTime?.ToString("h tt")?? string.Empty;
        }

        private static string GetBestFolderLocationToOpen(this ShowItem si)
        {
            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && Directory.Exists(si.AutoAddFolderBase))
                return si.AutoAddFolderBase;

            Dictionary<int, List<string>> afl = si.AllFolderLocations();

            foreach (KeyValuePair<int, List<string>> season in afl)
            {
                foreach (string folder in season.Value)
                {
                    if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder)) return folder;
                }
            }
            return string.Empty;
        }

        private static string CreateHorizontalBannerHtml(SeriesInfo ser)
        {
            if ((!string.IsNullOrEmpty(ser.GetSeriesWideBannerPath())) &&
                (!string.IsNullOrEmpty(TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()))))
                return  $"<img class=\"rounded\" src=\"{TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath())}\"><br/>&nbsp;";

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

        public static string GetSeasonHtmlOverview(this ShowItem si, Season s, bool includeDirectoryLinks)
        {
            StringBuilder sb = new StringBuilder();
            DirFilesCache dfc = new DirFilesCache();
            Color col = Color.FromName("ButtonFace");
            sb.AppendLine(HTMLHeader(10,col));
            sb.AppendSeason(s,si,col,includeDirectoryLinks);
            foreach (ProcessedEpisode ep in GetBestEpisodes(si,s))
            {
                List<FileInfo> fl = dfc.FindEpOnDisk(ep);
                sb.AppendEpisode(ep,fl,col);
            }
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        private static void AppendSeason(this StringBuilder sb, Season s, ShowItem si,Color backgroundColour, bool includeDirectoryLinks)
        {
            if (si == null)
                return;

            SeriesInfo ser = s.TheSeries;
            string seasonLink = TheTVDB.Instance.WebsiteUrl(ser.TvdbCode, s.SeasonId, false);
            string showLink = TheTVDB.Instance.WebsiteUrl(si.TvdbCode, -1, true);
            string urlFilename = Uri.EscapeDataString(si.GetBestFolderLocationToOpen(s));

            string explorerButton = includeDirectoryLinks
                ? CreateButton($"{UI.EXPLORE_PROXY}{urlFilename}", "<i class=\"far fa-folder-open\"></i>", "Open Containing Folder")
                : string.Empty;
            string tvdbButton = CreateButton(seasonLink, "TVDB.com", "View on TVDB");

            sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
				{s.CreateHorizontalBannerHtml()}
				<br/>
                <div class=""row"">
                    <div class=""col-8""><h1><A HREF=""{showLink}"">{ser.Name}</A> - <A HREF=""{seasonLink}"">{SeasonName(si, s.SeasonNumber)}</a></h1></div>
                    <div class=""col-4"">
                        {explorerButton}
                        {tvdbButton}
                    </div>
                </div>
				</div>");
        }

        private static string GetBestFolderLocationToOpen(this ShowItem si,Season s )
        {
            Dictionary<int, List<string>> afl = si.AllFolderLocations();
            int[] keys = new int[afl.Count];
            afl.Keys.CopyTo(keys, 0);

            if (afl.ContainsKey(s.SeasonNumber))
            {
                foreach (string folder in afl[s.SeasonNumber])
                {
                    if (Directory.Exists(folder))
                    {
                        return folder;
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && Directory.Exists(si.AutoAddFolderBase))
                return si.AutoAddFolderBase;

            return string.Empty;
        }

        private static void AppendEpisode(this StringBuilder sb, ProcessedEpisode ep, IReadOnlyCollection<FileInfo> fl,Color backgroundColour)
        {
            string stars = StarRating(ep.EpisodeRating);
            string episodeUrl = TheTVDB.Instance.WebsiteUrl(ep.SeriesId, ep.SeasonId, ep.EpisodeId);
            bool ratingIsNumber = float.TryParse(ep.EpisodeRating, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out float rating);
            string siteRating = ratingIsNumber && rating > 0
                ? rating + "/10" + AddRatingCount(ep.SiteRatingCount??0)
                : "";

            string imdbLink = string.IsNullOrWhiteSpace(ep.ImdbCode) ? string.Empty : "http://www.imdb.com/title/" + ep.ImdbCode;
            string productionCode = string.IsNullOrWhiteSpace(ep.ProductionCode)
                ? string.Empty
                : "Production Code <br/>" + ep.ProductionCode;

            string episodeDescriptor = CustomEpisodeName.NameForNoExt(ep, CustomEpisodeName.OldNStyle(6)); // may need to include (si.DVDOrder && snum == 0)? ep.Name:
            string writersHtml = string.IsNullOrWhiteSpace(ep.Writer) ? string.Empty : "<b>Writers:</b> " + string.Join(", ", ep.Writers);
            string directorsHtml = string.IsNullOrWhiteSpace(ep.EpisodeDirector) ? string.Empty : "<b>Directors:</b> " + string.Join(", ", ep.Directors);
            string guestHtml = string.IsNullOrWhiteSpace(ep.EpisodeGuestStars) ? string.Empty : "<b>Guest Stars:</b> " + string.Join(", ", ep.GuestStars);
            string possibleBreak1 = (string.IsNullOrWhiteSpace(writersHtml) || string.IsNullOrWhiteSpace(directorsHtml))
                ? string.Empty
                : "<br />";
            string possibleBreak2 = ((string.IsNullOrWhiteSpace(writersHtml)&&string.IsNullOrWhiteSpace(directorsHtml)) || string.IsNullOrWhiteSpace(guestHtml))
                ? string.Empty
                : "<br />";

            string searchButton = (fl == null || fl.Count==0) && ep.HasAired()
                ? CreateButton(TVSettings.Instance.BTSearchURL(ep), "<i class=\"fas fa-search\"></i>","Search for Torrent...")
                : string.Empty;

            string viewButton = string.Empty;
            string explorerButton = string.Empty;
            if (fl != null)
            {
                foreach (FileInfo fi in fl)
                {
                    string urlFilename = Uri.EscapeDataString(fi.FullName);
                    viewButton += CreateButton($"{UI.WATCH_PROXY}{urlFilename}", "<i class=\"far fa-eye\"></i>","Watch Now");
                    explorerButton += CreateButton($"{UI.EXPLORE_PROXY}{urlFilename}", "<i class=\"far fa-folder-open\"></i>","Open Containing Folder");
                }
            }

            string tvdbButton = CreateButton(episodeUrl, "TVDB.com","View on TVDB");
            string imdbButton = CreateButton(imdbLink, "IMDB.com","View on IMDB");
            string tvButton = CreateButton(ep.ShowUrl, "TV.com","View on TV.com");

            sb.AppendLine($@"
                <div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                 <div class=""row"">
                  <div class=""col-md-5"">{ep.ScreenShotHtml()}</div>
                   <div class=""col-md-7 d-flex flex-column"">
                    <div class=""row"">
                     <div class=""col-md-8""><h2>{episodeDescriptor}</h2></div>
                     <div class=""col-md-4 text-right"">{ep.DateDetailsHtml()}</div>
                    </div>
				   <div>
                    <blockquote>
                     {writersHtml}{possibleBreak1}
                     {directorsHtml}{possibleBreak2}
                     {guestHtml}
                    </blockquote>
                   </div>
                   <div><p class=""lead"">{ep.HiddenOverview()}</p></div>
                   <div>
                    {searchButton}
                    {viewButton}
                    {explorerButton}
                    {tvdbButton}
                    {imdbButton}
                    {tvButton}
                   </div>
		           <div class=""row align-items-bottom flex-grow-1"">
                    <div class=""col-md-6 align-self-end"">{stars}<br>{siteRating}</div>
                    <div class=""col-md-6 align-self-end text-right"">{productionCode}</div>
                   </div>
                  </div>
                 </div>
                </div>");
        }

        private static string AddRatingCount(int siteRatingCount)
        {
            return siteRatingCount > 0 ? $" (From {siteRatingCount} Vote{(siteRatingCount == 1 ? "" : "s")})" : string.Empty;
        }

        private static string CreateButton(string link, string text, string tooltip)
        {
            if (string.IsNullOrWhiteSpace(link)) return string.Empty;

            return $"<a href=\"{link}\" class=\"btn btn-outline-secondary\" role=\"button\" aria-disabled=\"true\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"{tooltip}\">{text}</i></a>";
        }

        private static string DateDetailsHtml(this ProcessedEpisode ei)
        {
            DateTime? dt = ei.GetAirDateDt(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                return $"<h6>{dt.Value.ToShortDateString()}</h6><small class=\"text-muted\">({ei.HowLong()})</small>";

            return string.Empty;
        }

        public static string GetShowImagesHtmlOverview(this ShowItem si)
        {
            SeriesInfo ser = si.TheSeries();

            string body =
                $"<h1><A HREF=\"{TheTVDB.Instance.WebsiteUrl(si.TvdbCode, -1, true)}\">{si.ShowName}</A> </h1>";

            body += ImageSection("Show Banner", 758, 140, ser.GetSeriesWideBannerPath());
            body += ImageSection("Show Poster", 350, 500, ser.GetSeriesPosterPath());
            body += ImageSection("Show Fanart", 960, 540, ser.GetSeriesFanartPath());
            return body;
        }

        private static string ImageSection(string title, int width, int height, string bannerPath)
        {
            if (string.IsNullOrEmpty(bannerPath)) return "";

            string url = TheTVDB.GetImageURL(bannerPath);

            return string.IsNullOrEmpty(url) ? "" : $"<h2>{title}</h2><img width={width} height={height} src=\"{url}\"><br/>";
        }

        private static List<ProcessedEpisode> GetBestEpisodes(ShowItem si, Season s)
        {
            return si.SeasonEpisodes.ContainsKey(s.SeasonNumber)
                ? si.SeasonEpisodes[s.SeasonNumber]
                : ShowItem.ProcessedListFromEpisodes(s.Episodes.Values, si);
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

            List<ProcessedEpisode> eis = si.SeasonEpisodes.ContainsKey(snum)
                ? si.SeasonEpisodes[snum]
                : ShowItem.ProcessedListFromEpisodes(s.Episodes.Values, si);

            string seasText = Season.UIFullSeasonWord(snum);

            if ((eis.Count > 0) && (eis[0].SeasonId > 0))
                seasText = " - <A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TvdbCode, eis[0].SeasonId, false) + "\">" +
                           seasText + "</a>";
            else
                seasText = " - " + seasText;

            body += "<h1><A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TvdbCode, -1, true) + "\">" + si.ShowName +
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
            if (si.DvdOrder)
            {
                return (snum == 0)
                    ? "Not Available on DVD"
                    : "DVD " + Season.UISeasonWord(snum);
            }
            else
            {
                return Season.UIFullSeasonWord(snum);
            }
        }

        internal static string GenreIconHtml(string genre)
        {
            string[] availbleIcons =
            {
                "Action", "Adventure", "Animation", "Children", "Comedy", "Crime", "Documentary", "Drama", "Family",
                "Fantasy", "Food", "Horror", "Mini-Series", "Mystery", "Reality", "Romance", "Science-Fiction", "Soap",
                "Talk Show", "Thriller", "Travel", "War", "Western"
            };

            const string ROOT = "https://www.tvrename.com/assets/images/GenreIcons/";

            return availbleIcons.Contains(genre)
                ? $@"<img width=""30"" height=""30"" src=""{ROOT}{genre}.svg"" alt=""{genre}"">"
                : "";
        }

        private static string ActorLinkHtml(Actor actor)
        {
            string asText = string.IsNullOrWhiteSpace(actor.ActorRole) ? string.Empty : (" as " + actor.ActorRole);
            string tryText =
                $@"<a href=""http://www.imdb.com/find?s=nm&q={actor.ActorName}"">{actor.ActorName}</a>{asText}";
            return tryText;
        }

        private static string StarRating(string rating)
        {
            try
            {
                float f = float.Parse(rating, CultureInfo.CreateSpecificCulture("en-US"));

                return StarRating(f / 2);
            }
            catch (FormatException)
            {
                return StarRating(0);
            }
        }

        internal static string StarRating(float f)
        {
            const string STAR = @"<i class=""fas fa-star""></i>";
            const string HALFSTAR = @"<i class=""fas fa-star-half""></i>";

            if (f < .25) return "";
            if (f <= .75) return HALFSTAR;
            if (f > 1) return STAR + StarRating(f - 1);
            return STAR;
        }

        // ReSharper disable once InconsistentNaming
        internal static string HTMLHeader(int size,Color backgroundColour)
        {
            return @"<!DOCTYPE html>
                <html><head>
                <meta charset=""utf-8"">
                <meta name = ""viewport"" content = ""width=device-width, initial-scale=1.0"" >
                <title> TV Rename - Show Summary</title>
                <link rel = ""stylesheet"" href = ""http://maxcdn.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css"" />
                <link rel = ""stylesheet"" href = ""https://use.fontawesome.com/releases/v5.0.13/css/all.css"" />
                </head >"
                + $"<body style=\"background-color: {backgroundColour.HexColour()}\" ><div class=\"col-sm-{size} offset-sm-{(12 - size) / 2}\">";
        }

        private static string HexColour(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
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
