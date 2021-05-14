using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using TVRename.Forms;
using TVRename.Properties;

namespace TVRename
{
    internal static class ShowHtmlHelper
    {
        [NotNull]
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

        [NotNull]
        public static string GetShowHtmlOverview(this ShowConfiguration si,bool includeDirectoryLinks)
        {
            Color col = Color.FromName("ButtonFace");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HTMLHeader(10,col));
            sb.AppendShow(si,col,includeDirectoryLinks);
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        public static string GetShowHtmlOverview(this CachedSeriesInfo series, RecommendationRow recommendation)
        {
            Color col = Color.FromName("ButtonFace");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HTMLHeader(10, col));
            sb.AppendShow(null,series, col, false);
            sb.AppendRecommendation(recommendation, col);
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        [NotNull]
        public static string GetMovieHtmlOverview(this MovieConfiguration si, bool includeDirectoryLinks)
        {
            Color col = Color.FromName("ButtonFace");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HTMLHeader(10, col));
            sb.AppendMovie(si, col, includeDirectoryLinks);
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        public static string GetMovieHtmlOverview(this CachedMovieInfo movie, RecommendationRow? recommendation)
        {
            Color col = Color.FromName("ButtonFace");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HTMLHeader(10, col));
            sb.AppendMovie(null,movie, col, false);
            sb.AppendRecommendation(recommendation, col);
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        [NotNull]
        public static string GetShowSummaryHtmlOverview([NotNull] this ShowConfiguration si, bool includeDirectoryLinks)
        {
            Color col = Color.FromName("ButtonFace");
            DirFilesCache dfc = new DirFilesCache();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(HTMLHeader(10, col));
            sb.AppendShowSummary(si,dfc, col,includeDirectoryLinks);
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        private static void AppendShowSummary(this StringBuilder sb, ShowConfiguration? si, DirFilesCache dfc, Color backgroundColour, bool includeDirectoryLinks)
        {
            CachedSeriesInfo ser = si?.CachedShow;
            if (ser is null)
            {
                return;
            }

            string horizontalBanner = CreateHorizontalBannerHtml(ser);
            string yearRange = YearRange(ser);
            string episodeSummary = ser.Episodes.Count.ToString();
            string imdbLink = string.IsNullOrWhiteSpace(ser.Imdb) ? string.Empty : $"http://www.imdb.com/title/{ser.Imdb}";
            string table = CreateEpisodeTableHeader(CreateTableRows(si, dfc, includeDirectoryLinks));

            sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                <div class=""text-center"">
	             {horizontalBanner}
                </div>
                    <div class=""row"">
                     <div class=""col-md-8""><h1>{si.ShowName}</h1><small class=""text-muted"">{ser.ShowLanguage} - {ser.Type}</small></div>
                     <div class=""col-md-4 text-right""><h6>{yearRange} ({ser.Status})</h6><small class=""text-muted"">{episodeSummary} Episodes</small></div>
                    </div>
		            <div class=""row"">
                    <table class=""w-100"">
                        <tr><td><p class=""lead"">{ser.Overview}</p></td><td class=""text-right align-text-top"">
			         {CreateButton(imdbLink, "IMDB.com", "View on IMDB")}
			         {CreateButton(ser.OfficialUrl, "Official Site", "View on Official Site")}
			        </td></tr></table>
                </div>
                {table}
                </div>");
        }

        private static string CreateTableRows(ShowConfiguration si, DirFilesCache dfc, bool includeDirectoryLinks)
        {
            StringBuilder tableRows = new StringBuilder();

            foreach (ProcessedSeason season in si.AppropriateSeasons().OrderBy(pair => pair.Key).Select(pair => pair.Value))
            {
                if (si.SeasonEpisodes.TryGetValue(season.SeasonNumber, out List<ProcessedEpisode> seasonEpisodes))
                {
                    tableRows.AppendSeasonShowSummary(dfc, si, season, includeDirectoryLinks, seasonEpisodes);
                }
            }

            return tableRows.ToString();
        }

        private static void AppendSeasonShowSummary([NotNull] this StringBuilder sb, DirFilesCache dfc, [NotNull] ShowConfiguration si, [NotNull] ProcessedSeason s, bool includeDirectoryLinks, IEnumerable<ProcessedEpisode> seasonEpisodes)
        {
            string explorerButton = string.Empty;
            if (includeDirectoryLinks)
            {
                string urlFilename = Uri.EscapeDataString(si.GetBestFolderLocationToOpen(s));
                explorerButton = CreateButton($"{UI.EXPLORE_PROXY}{urlFilename}",
                    "<i class=\"far fa-folder-open\"></i>", "Open Containing Folder");
            }

            string tableRows = seasonEpisodes.Select(episode => SeasonSummaryTableRow(episode, includeDirectoryLinks, dfc)).Concat();

            string? tvdbSLug = si.CachedShow?.Slug;
            string tvdbLink = !tvdbSLug.HasValue() ? string.Empty : TheTVDB.API.WebsiteSeasonUrl(s);
            string tvdbButton = CreateButton(tvdbLink, "TVDB.com", "View on TVDB");
            string tvMazeButton = CreateButton(s.Show.Provider!=TVDoc.ProviderType.TVmaze?string.Empty: s.WebsiteUrl, "TVmaze.com", "View on TV Maze");
            string episodeText = s.Episodes.Count > 0 ? $"<br/><small class=\"text-muted\">{s.Episodes.Count} Episodes</small>" : string.Empty;

            string seasonOverViewHtml = si.CachedShow?.Season(s.SeasonNumber)?.SeasonName.HasValue() ?? false
                ? $"<h4>{SeasonName(si, s.SeasonNumber)} - {si.CachedShow?.Season(s.SeasonNumber)?.SeasonName}</h4>{si.CachedShow?.Season(s.SeasonNumber)?.SeasonDescription}"
                : SeasonName(si, s.SeasonNumber);

            sb.AppendLine($@"     <tr class=""table-secondary"">
      <td scope=""row"" colspan=""4"">{seasonOverViewHtml}{episodeText}</td>
      <td class=""text-right"">
{CreateButton(EditSeasonUrl(si,s), "<i class=\"far fa-edit\"></i>", "Edit")}
{explorerButton}
            {tvdbButton}
            {tvMazeButton}</td>
    </tr>
{tableRows}");
        }

        private static void AppendShow(this StringBuilder sb, ShowConfiguration? si, Color backgroundColour,
            bool includeDirectoryLinks)
        {
            CachedSeriesInfo ser = si?.CachedShow;

            if (ser is null)
            {
                return;
            }
            AppendShow(sb,si,ser,backgroundColour,includeDirectoryLinks);
        }

        private static void AppendShow(this StringBuilder sb, ShowConfiguration? si, CachedSeriesInfo ser, Color backgroundColour, bool includeDirectoryLinks)
        {
            string horizontalBanner = CreateHorizontalBannerHtml(ser);
            string poster = CreatePosterHtml(ser);
            string yearRange = YearRange(ser);
            string episodeSummary = ser.Episodes.Count.ToString();
            string stars = StarRating(ser.SiteRating/2);
            string genreIcons = string.Join("&nbsp;", ser.Genres.Select(GenreIconHtml));
            string siteRating = ser.SiteRating > 0 ? ser.SiteRating+ "/10" : "";
            string runTimeHtml = string.IsNullOrWhiteSpace(ser.Runtime) ? string.Empty : $"<br/> {ser.Runtime} min";
            string actorLinks = ser.GetActors().Select(ActorLinkHtml).ToCsv();
            string airsTime = ParseAirsTime(ser);
            string airsDay = ser.AirsDay;
            string dayTime = $"{airsDay} {airsTime}";

            string tvLink = string.IsNullOrWhiteSpace(ser.SeriesId) ? string.Empty : $"http://www.tv.com/show/{ser.SeriesId}/summary.html";
            string imdbLink = string.IsNullOrWhiteSpace(ser.Imdb) ? string.Empty : $"http://www.imdb.com/title/{ser.Imdb}";
            string mazeLink = ser.TvMazeCode <=0 ? string.Empty : ser.WebUrl;
            string tmdbLink = ser.TmdbCode > 0 ? $"https://www.themoviedb.org/tv/{ser.TmdbCode}" : string.Empty;
            string tvdbLink = ser.TvdbCode>0?  TheTVDB.API.WebsiteShowUrl(ser) : string.Empty;

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
                     <div class=""col-md-8""><h1>{si?.ShowName ?? ser.Name}</h1><small class=""text-muted"">{ser.ShowLanguage} - {ser.Type}</small></div>
                     <div class=""col-md-4 text-right""><h6>{yearRange} ({ser.Status})</h6><small class=""text-muted"">{episodeSummary} Episodes{runTimeHtml}</small></div>
                    </div>
                    <div><p class=""lead"">{ser.Overview}</p></div>
			        <div><blockquote>{actorLinks}</blockquote></div> 
		            <div>
                    {CreateButton(EditTvSeriesUrl(ser), "<i class=\"far fa-edit\"></i>", "Edit")}
                     {explorerButton}
			         {CreateButton(tvdbLink, "TVDB.com", "View on TVDB")}
			         {CreateButton(imdbLink, "IMDB.com", "View on IMDB")}
                     {CreateButton(mazeLink, "TVmaze.com", "View on TVmaze")}
                     {CreateButton(tmdbLink, "TMDB.com", "View on TMDB")}
			         {CreateButton(tvLink, "TV.com", "View on TV.com")}
			         {CreateButton(ser.OfficialUrl, "Official Site", "View on Official Site")}
			        </div>
		            <div>
                        &nbsp;
			        </div>
		            <div class=""row align-items-bottom flex-grow-1"">
                     <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}{AddRatingCount(ser.SiteRatingVotes)}</div>
                     <div class=""col-md-4 align-self-end text-center"">{ser.ContentRating}<br>{ser.Network}, {dayTime}</div>
                     <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{ser.Genres.ToCsv()}</div>
                    </div>
                   </div>
                  </div>
                 </div>");
        }

        private static void AppendRecommendation(this StringBuilder sb, RecommendationRow recommendationRow, Color backgroundColour)
        {
            string top = recommendationRow.TopRated ? "TOP RATED" : string.Empty;
            string trending = recommendationRow.Trending ? "TRENDING" : string.Empty;

            sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                   <div class=""col-md-4"">
                    {top}
                    <BR />
                    {trending}
                   </div>
                   <div class=""col-md-8 d-flex flex-column"">
                    <div class=""row"">
                     <b>Because it's related to:</b> {recommendationRow.Related}
                    </div>
                    <div class=""row"">
                     <b>Because it's similar to:</b> {recommendationRow.Similar}
                    </div>
                   </div>
                  </div>
                 </div>");
        }


        private static void AppendMovie(this StringBuilder sb, MovieConfiguration? si, Color backgroundColour,
            bool includeDirectoryLinks)
        {
            CachedMovieInfo? ser = si?.CachedMovie;

            if (ser is null)
            {
                return;
            }
            AppendMovie(sb,si,ser,backgroundColour,includeDirectoryLinks);
        }

        private static void AppendMovie(this StringBuilder sb, MovieConfiguration? si, CachedMovieInfo ser,  Color backgroundColour, bool includeDirectoryLinks)
        {
            string poster = CreatePosterHtml(ser);
            string yearRange = ser.Year?.ToString() ?? "";
            string stars = StarRating(ser.SiteRating / 2);
            string genreIcons = string.Join("&nbsp;", ser.Genres.Select(GenreIconHtml));
            string siteRating = ser.SiteRating > 0 ? ser.SiteRating + "/10" : "";
            string runTimeHtml = string.IsNullOrWhiteSpace(ser.Runtime) ? string.Empty : $"<br/> {ser.Runtime} min";
            string actorLinks = ser.GetActors().Select(ActorLinkHtml).ToCsv();
            string tvdbLink = ser.TvdbCode>0?TheTVDB.API.WebsiteShowUrl(ser.TvdbCode):string.Empty; //todo -check this works - looks like the Show version

            string tvLink = string.IsNullOrWhiteSpace(ser.SeriesId) ? string.Empty : $"http://www.tv.com/show/{ser.SeriesId}/summary.html";
            string imdbLink = string.IsNullOrWhiteSpace(ser.Imdb) ? string.Empty : $"http://www.imdb.com/title/{ser.Imdb}";
            string tmdbLink = ser.TmdbCode > 0 ? $"https://www.themoviedb.org/movie/{ser.TmdbCode}" : string.Empty;
            string mazeLink = ser.TvMazeCode <= 0 ? string.Empty : ser.WebUrl;

            string urlFilename = includeDirectoryLinks ? Uri.EscapeDataString(si.Locations.FirstOrDefault() ?? string.Empty)                 : string.Empty;
            string explorerButton = includeDirectoryLinks                 ? CreateButton($"{UI.EXPLORE_PROXY}{urlFilename}", "<i class=\"far fa-folder-open\"></i>", "Open Containing Folder")                 : string.Empty;
            string viewButton = includeDirectoryLinks ? CreateButton($"{UI.WATCH_PROXY}{urlFilename}", "<i class=\"far fa-eye\"></i>", "Watch Now") : string.Empty;
            string facebookButton = ser.FacebookId.HasValue() ?CreateButton($"https://facebook.com/{ser.FacebookId}", "<i class=\"fab fa-facebook\"></i>", "Facebook"):string.Empty;
            string instaButton = ser.InstagramId.HasValue() ? CreateButton($"https://instagram.com/{ser.InstagramId}", "<i class=\"fab fa-instagram\"></i>", "Instagram"):string.Empty;
            string twitterButton = ser.TwitterId.HasValue() ? CreateButton($"https://twitter.com/{ser.TwitterId}", "<i class=\"fab fa-twitter\"></i>", "Twitter"):string.Empty;


            sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                   <div class=""col-md-4"">
                    {poster}
                   </div>
                   <div class=""col-md-8 d-flex flex-column"">
                    <div class=""row"">
                     <div class=""col-md-8""> <h1>{si?.ShowName ?? ser.Name}</h1><small class=""text-muted"">{ser.TagLine}</small></div>
                     <div class=""col-md-4 text-right""><h6>{yearRange} ({ser.Status})</h6>
                        <small class=""text-muted"">{ser.ShowLanguage} - {ser.Type}</small>
                        <small class=""text-muted"">{runTimeHtml}</small></div>
                    </div>
                    <div><p class=""lead"">{ser.Overview}</p></div>
			        <div></div> 
		            <div>
                    {CreateButton(EditMovieUrl(ser), "<i class=\"far fa-edit\"></i>", "Edit")}
                     {explorerButton}
                    {viewButton}
                     {CreateButton(tmdbLink, "TMDB.com", "View on TMDB")}
			         {CreateButton(tvdbLink, "TVDB.com", "View on TVDB")}
			         {CreateButton(imdbLink, "IMDB.com", "View on IMDB")}
                     {CreateButton(mazeLink, "TVmaze.com", "View on TVmaze")}
			         {CreateButton(tvLink, "TV.com", "View on TV.com")}
			         {CreateButton(ser.OfficialUrl, "Official Site", "View on Official Site")}
                    {facebookButton}
                    {instaButton}
                    {twitterButton}
                    
			        </div>
<div id=""accordion"">
  <div class=""card"">
    <div class=""card-header"" id=""headingOne"">
      <h5 class=""mb-0"">
        <button class=""btn btn-link"" data-toggle=""collapse"" data-target=""#collapseOne"" aria-expanded=""false"" aria-controls=""collapseOne"">
          Cast
        </button>
      </h5>
    </div>

    <div id = ""collapseOne"" class=""collapse show"" aria-labelledby=""headingOne"" data-parent=""#accordion"">
      <div class=""card-body"">
        <blockquote>{actorLinks}</blockquote>
        
      </div>
    </div>
  </div>
  <div class=""card"">
    <div class=""card-header"" id=""headingTwo"">
      <h5 class=""mb-0"">
        <button class=""btn btn-link collapsed"" data-toggle=""collapse"" data-target=""#collapseTwo"" aria-expanded=""false"" aria-controls=""collapseTwo"">
          Crew
        </button>
      </h5>
    </div>
    <div id = ""collapseTwo"" class=""collapse"" aria-labelledby=""headingTwo"" data-parent=""#accordion"">
      <div class=""card-body"">
        {ser.GetCrew().Select(c => $"{c.Name} as {c.Job}").ToCsv()}
      </div>
    </div>
  </div>
</div>
		            <div>
                        &nbsp;
			        </div>
		            <div class=""row align-items-bottom flex-grow-1"">
                     <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}{AddRatingCount(ser.SiteRatingVotes)}</div>
                     <div class=""col-md-4 align-self-end text-center"">{ser.ContentRating}<br>{ser.Network}</div>
                     <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{ser.Genres.ToCsv()}</div>
                    </div>
                   </div>
                  </div>
                 </div>");
        }

        private static string? EditMovieUrl(MovieConfiguration si)
        {
            switch (si.Provider)
            {
                case TVDoc.ProviderType.TheTVDB:
                    //tofo reenable when TVDB has movies
                    //if (si.TVDBSlug > 0)
                    //{
                        //return $"https://thetvdb.com/movies/{TVDBSlug}/edit";
                    //}

                    return null;

                case TVDoc.ProviderType.TMDB:
                    if (si.TmdbCode > 0)
                    {
                        return $"https://www.themoviedb.org/movie/{si.TmdbCode}/edit?active_nav_item=primary_facts";
                    }

                    return null;

                case TVDoc.ProviderType.TVmaze:
                case TVDoc.ProviderType.libraryDefault:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string? EditMovieUrl(CachedMovieInfo si)
        {
            if (si.TmdbCode > 0)
            {
                return $"https://www.themoviedb.org/movie/{si.TmdbCode}/edit?active_nav_item=primary_facts";
            }
            if (si.Slug.HasValue())
            {
                return $"https://thetvdb.com/movies/{si.Slug}/edit";
            }

            return null;

        }

        private static string? EditSeasonUrl(ShowConfiguration si, ProcessedSeason s)
        {
            switch (si.Provider)
            {
                case TVDoc.ProviderType.TheTVDB:
                    //todo reenable when TVDB has movies
                    //if (si.TVDBSlug > 0)
                    //{
                    //return $"https://thetvdb.com/movies/{TVDBSlug}/edit";
                    //}

                    return null;

                case TVDoc.ProviderType.TMDB:
                    if (si.TmdbCode > 0)
                    {
                        return $"https://www.themoviedb.org/tv/{si.TmdbCode}/edit?active_nav_item=seasons";
                    }

                    return null;

                case TVDoc.ProviderType.TVmaze:
                    if (si.TVmazeCode > 0 && s.SeasonId>0)
                    {
                        return $"https://www.tvmaze.com/season/update?id={s.SeasonId}";
                    }

                    return null;
                case TVDoc.ProviderType.libraryDefault:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string? EditTvEpisodeUrl(ProcessedEpisode ep)
        {
            switch (ep.Show.Provider)
            {
                case TVDoc.ProviderType.TheTVDB:
                    if (ep.Show.CachedShow?.Slug.HasValue()??false)
                    {
                        return $"https://thetvdb.com/series/{ep.Show.CachedShow?.Slug}/episodes/{ep.EpisodeId}";
                    }

                    return null;

                case TVDoc.ProviderType.TMDB:
                    if (ep.Show.TmdbCode > 0)
                    {
                        return $"https://www.themoviedb.org/tv/{ep.Show.TmdbCode}/season/{ep.AppropriateSeasonNumber}/episode/{ep.AppropriateEpNum}/edit?active_nav_item=primary_facts";
                    }

                    return null;

                case TVDoc.ProviderType.TVmaze:
                    if (ep.EpisodeId > 0)
                    {
                        return $"https://www.tvmaze.com/episode/update?id={ep.EpisodeId}";
                    }

                    return null;
                
                case TVDoc.ProviderType.libraryDefault:
                default:
                    return null;
            }
        }

        private static string? EditTvSeriesUrl(ShowConfiguration si)
        {
            switch (si.Provider)
            {
                case TVDoc.ProviderType.TheTVDB:
                    
                    if (si.CachedShow?.Slug.HasValue() ?? false)
                    {
                        return $"https://thetvdb.com/series/{si.CachedShow.Slug}/edit";
                    }

                    return null;

                case TVDoc.ProviderType.TMDB:
                    if (si.TmdbCode > 0)
                    {
                        return $"https://www.themoviedb.org/tv/{si.TmdbCode}/edit?active_nav_item=primary_facts";
                    }

                    return null;

                case TVDoc.ProviderType.TVmaze:
                    if (si.TVmazeCode > 0)
                    {
                        return $" https://www.tvmaze.com/show/update?id={si.TVmazeCode}";
                    }

                    return null;
                case TVDoc.ProviderType.libraryDefault:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string? EditTvSeriesUrl(CachedSeriesInfo si)
        {
            if (si.TmdbCode > 0)
            {
                return $"https://www.themoviedb.org/tv/{si.TmdbCode}/edit?active_nav_item=primary_facts";
            }
            if (si.Slug.HasValue())
            {
                return $"https://thetvdb.com/series/{si.Slug}/edit";
            }
            if (si.TvMazeCode > 0)
            {
                return $" https://www.tvmaze.com/show/update?id={si.TvMazeCode}";
            }

            return null;
        }

        [NotNull]
        public static string YearRange(CachedSeriesInfo? ser)
        {
            if (ser is null)
            {
                return string.Empty;
            }
            int? minYear = ser.MinYear;
            int? maxYear = ser.MaxYear;

            if (minYear.HasValue && maxYear.HasValue)
            {
                return minYear.Value == maxYear.Value ? minYear.Value.ToString() : minYear.Value + "-" + maxYear.Value;
            }

            if (minYear.HasValue)
            {
                return minYear.Value + "-";
            }

            if (maxYear.HasValue)
            {
                return "-" + maxYear.Value;
            }

            return string.Empty;
        }

        [NotNull]
        private static string ParseAirsTime([NotNull] CachedSeriesInfo ser)
        {
            return ser.AirsTime?.ToString("h tt")?? string.Empty;
        }

        [NotNull]
        private static string GetBestFolderLocationToOpen([NotNull] this ShowConfiguration si)
        {
            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && Directory.Exists(si.AutoAddFolderBase))
            {
                return si.AutoAddFolderBase;
            }

            foreach (string folder  in si.AllExistngFolderLocations().Values.SelectMany(a=>a))
            {
                if (folder.HasValue() && Directory.Exists(folder))
                {
                    return folder;
                }
            }
            return string.Empty;
        }

        [NotNull]
        private static string CreateHorizontalBannerHtml([NotNull] CachedSeriesInfo ser)
        {
            string path = ser.GetSeriesWideBannerPath();
            if (!string.IsNullOrEmpty(path) &&
                !string.IsNullOrEmpty(TheTVDB.API.GetImageURL(path)))
            {
                return  $"<img class=\"rounded\" src=\"{TheTVDB.API.GetImageURL(path)}\"><br/>&nbsp;";
            }

            return string.Empty;
        }

        [NotNull]
        private static string CreateHorizontalBannerHtml([NotNull] this ProcessedSeason s)
        {
            if (!string.IsNullOrEmpty(s.GetWideBannerPath()) &&
                !string.IsNullOrEmpty(TheTVDB.API.GetImageURL(s.GetWideBannerPath())))
            {
                return $"<img class=\"rounded w-100\" src=\"{TheTVDB.API.GetImageURL(s.GetWideBannerPath())}\"><br/>";
            }

            return string.Empty;
        }

        [NotNull]
        private static string CreatePosterHtml([NotNull] CachedSeriesInfo ser)
        {
            string url = ser.GetSeriesPosterPath();
            if (url.HasValue() && !url.IsWebLink() && TheTVDB.API.GetImageURL(url).HasValue())
            {
                url =TheTVDB.API.GetImageURL(url);
            }
            if (url.HasValue() && url.IsWebLink())
            {
                return $"<img class=\"show-poster rounded w-100\" src=\"{url}\" alt=\"{ser.Name} Show Poster\">";
            }

            return string.Empty;
        }

        [NotNull]
        public static string CreatePosterHtml([NotNull] CachedMovieInfo ser)
        {
            string url = ser.PosterUrl;
            if (url.HasValue() && !url!.IsWebLink() && TheTVDB.API.GetImageURL(url).HasValue())
            {
                url = TheTVDB.API.GetImageURL(url);
            }
            if (url.HasValue() && url!.IsWebLink())
            {
                return $"<img class=\"show-poster rounded w-100\" src=\"{url}\" alt=\"{ser.Name} Movie Poster\">";
            }

            return string.Empty;
        }

        [NotNull]
        private static string CreateSeasonPosterHtml([NotNull] ShowConfiguration si,int snum)
        {
            string url = si.CachedShow?.GetSeasonBannerPath(snum);
            if (url is null)
            {
                return string.Empty;
            }
            if (url.HasValue() && !url.IsWebLink() && TheTVDB.API.GetImageURL(url).HasValue())
            {
                url = TheTVDB.API.GetImageURL(url);
            }
            if (url.HasValue() && url.IsWebLink())
            {
                return $"<img class=\"show-poster rounded w-100\" src=\"{url}\" alt=\"{si.ShowName} Show Poster\">";
            }

            return string.Empty;
        }

        [NotNull]
        private static string ScreenShotHtml(this ProcessedEpisode ei)
        {
            if (!TVSettings.Instance.ShowEpisodePictures)
            {
                return string.Empty;
            }

            if (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired")
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(ei.Filename))
            {
                return string.Empty;
            }

            string url = ei.Show.Provider == TVDoc.ProviderType.TheTVDB
                ? TheTVDB.API.GetImageURL(ei.Filename)
                : ei.Filename;

            if (url.HasValue()) 
            {
                return $"<img class=\"rounded w-100\" src=\"{url}\" alt=\"{ei.Name} Screenshot\">";
            }
            return string.Empty;
        }

        [NotNull]
        public static string GetSeasonHtmlOverview([NotNull] this ShowConfiguration si, [NotNull] ProcessedSeason s, bool includeDirectoryLinks)
        {
            StringBuilder sb = new StringBuilder();
            DirFilesCache dfc = new DirFilesCache();
            Color col = Color.FromName("ButtonFace");
            sb.AppendLine(HTMLHeader(10,col));
            sb.AppendSeason(s,si,col,includeDirectoryLinks);

            if (si.SeasonEpisodes.TryGetValue(s.SeasonNumber, out List<ProcessedEpisode> siSeasonEpisode))
            {
                foreach (ProcessedEpisode ep in siSeasonEpisode)
                {
                    List<FileInfo> fl = includeDirectoryLinks ? dfc.FindEpOnDisk(ep) : null;
                    sb.AppendEpisode(ep, fl, col);
                }
            }

            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        [NotNull]
        public static string GetSeasonSummaryHtmlOverview([NotNull] this ShowConfiguration si, [NotNull] ProcessedSeason s, bool includeDirectoryLinks)
        {
            StringBuilder sb = new StringBuilder();
            Color col = Color.FromName("ButtonFace");
            sb.AppendLine(HTMLHeader(10, col));
            sb.AppendSeasonSummary(si, s, col, includeDirectoryLinks);
            sb.AppendLine(HTMLFooter());
            return sb.ToString();
        }

        [NotNull]
        private static string SeasonSummaryTableRow([NotNull]ProcessedEpisode ep, bool includeDirectoryLinks, DirFilesCache dfc)
        {
            List<FileInfo>? fl = includeDirectoryLinks ? dfc.FindEpOnDisk(ep) : null;
            string status = GetEpisodeStatus(ep, includeDirectoryLinks, fl);

            string searchButton = (fl is null || fl.Count == 0) && ep.HasAired()
                ? CreateButton(TVSettings.Instance.BTSearchURL(ep), "<i class=\"fas fa-search\"></i>", "Search for Torrent...")
                : string.Empty;

            string viewButton = string.Empty;
            if (fl != null)
            {
                foreach (string urlFilename in fl.Select(fi => Uri.EscapeDataString(fi.FullName)))
                {
                    viewButton += CreateButton($"{UI.WATCH_PROXY}{urlFilename}", "<i class=\"far fa-eye\"></i>", "Watch Now");
                }
            }

            string airedText = ep.HasAired() ? " (Aired)" : string.Empty;
            return
                $"<tr><th scope=\"row\">{ep.AppropriateEpNum}</th><td>{ep.GetAirDateDt(true):d}{airedText}</td><td>{ep.Name}</td><td>{status}</td><td class=\"text-right\">{searchButton}{viewButton}</td></tr>";
        }

        [NotNull]
        private static string GetEpisodeStatus([NotNull] ProcessedEpisode ep, bool includeDirectoryLinks, List<FileInfo>? fl)
        {
            bool filesExist = fl != null && fl.Count > 0;

            if (includeDirectoryLinks && filesExist)
            {
                return "On Disk";
            }
            
            if (ep.Show.IgnoreSeasons.Contains(ep.AppropriateSeasonNumber))
            {
                return "Ignored Season";
            }
            if (TVSettings.Instance.IgnoreAllSpecials && ep.AppropriateSeasonNumber == 0)
            {
                return "All Specials Ignored";
            }
            if (TVSettings.Instance.IgnorePreviouslySeen && ep.PreviouslySeen)
            {
                return "Previously Seen";
            }
            if (TVSettings.Instance.Ignore.Any(item => item.MatchesEpisode(ep.Show.AutoAddFolderBase, ep)))
            {
                return "Episode Ignored";
            }

            if (includeDirectoryLinks && ep.HasAired())
            {
                return "Missing";
            }

            return string.Empty;
        }

        private static void AppendSeasonSummary(this StringBuilder sb, ShowConfiguration? si, ProcessedSeason s, Color backgroundColour, bool includeDirectoryLinks)
        {
            DirFilesCache dfc = new DirFilesCache();
            if (si is null)
            {
                return;
            }

            string tablerows = si.SeasonEpisodes[s.SeasonNumber].ToList().Select(episode => SeasonSummaryTableRow(episode,includeDirectoryLinks,dfc)).Concat();

            string seasonHeaderDiv = CreateSeasonHeaderDiv(si,s,includeDirectoryLinks);
            string table = CreateEpisodeTableHeader(tablerows);

            sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">{seasonHeaderDiv}
                {table}
                </div>");
        }

        [NotNull]
        private static string CreateEpisodeTableHeader(string rows)
        {
            return $@"<div class=""row"">
<table class=""table  table-hover"">
  <thead>
    <tr>
      <th scope=""col"">#</th>
      <th scope=""col"">Date</th>
      <th scope=""col"">Name</th>
      <th scope=""col"">Status</th>
      <th class=""text-right"" scope=""col"">Action</th>
    </tr>
  </thead>
  <tbody>
    {rows}
  </tbody>
</table>
</div>";
        }

        [NotNull]
        private static string CreateSeasonHeaderDiv([NotNull] ShowConfiguration si, [NotNull] ProcessedSeason s, bool includeDirectoryLinks)
        {
            string explorerButton = string.Empty;
            if (includeDirectoryLinks)
            {
                string urlFilename = Uri.EscapeDataString(si.GetBestFolderLocationToOpen(s));
                explorerButton = CreateButton($"{UI.EXPLORE_PROXY}{urlFilename}",
                    "<i class=\"far fa-folder-open\"></i>", "Open Containing Folder");
            }

            string tvdbSLug = si.CachedShow?.Slug;
            string tvdbLink = !tvdbSLug.HasValue() ? string.Empty : TheTVDB.API.WebsiteSeasonUrl(s);
            string tvdbButton = CreateButton(tvdbLink, "TVDB.com", "View on TVDB");
            string tvMazeButton = CreateButton(s.Show.Provider != TVDoc.ProviderType.TVmaze ? string.Empty : s.WebsiteUrl, "TVmaze.com", "View on TV Maze");

            string episodeText = s.Episodes.Count > 0 ? $"<br/><small class=\"text-muted\">{s.Episodes.Count} Episodes</small>" : string.Empty;

            string seasonOverViewHtml = si.CachedShow?.Season(s.SeasonNumber)?.SeasonName.HasValue() ?? false
                ? $"<h2>{si.CachedShow?.Season(s.SeasonNumber)?.SeasonName}</h3><p>{si.CachedShow?.Season(s.SeasonNumber)?.SeasonDescription}</p>"
                : string.Empty;

            return $@"<div class=""row"">
                    <div class=""col-8""><h1>{si.ShowName} - {SeasonName(si, s.SeasonNumber)}</h1>
                    {seasonOverViewHtml}
                    </div>
                    <div class=""col-4 text-right"">
                        {CreateButton(EditSeasonUrl(si,s), "<i class=\"far fa-edit\"></i>", "Edit")}
                        {explorerButton}
                        {tvdbButton}
                        {tvMazeButton}
                        {episodeText}
                    </div>
                </div>";
        }

        private static void AppendSeason(this StringBuilder sb, ProcessedSeason s, ShowConfiguration? si,Color backgroundColour, bool includeDirectoryLinks)
        {
            if (si is null)
            {
                return;
            }

            string seasonHeaderDiv = CreateSeasonHeaderDiv(si, s, includeDirectoryLinks);

            sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
				{s.CreateHorizontalBannerHtml()}
				<br/>
                {seasonHeaderDiv}
				</div>");
        }

        private static string GetBestFolderLocationToOpen([NotNull] this ShowConfiguration si,[NotNull] ProcessedSeason s )
        {
            Dictionary<int, SafeList<string>> afl = si.AllExistngFolderLocations();

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
            {
                return si.AutoAddFolderBase;
            }

            return string.Empty;
        }

        private static void AppendEpisode([NotNull] this StringBuilder sb, [NotNull] ProcessedEpisode ep, IReadOnlyCollection<FileInfo>? fl,Color backgroundColour)
        {
            string stars = StarRating(ep.EpisodeRating);
            string tvdbEpisodeUrl = ep.Show.Provider == TVDoc.ProviderType.TheTVDB ? ep.TVDBWebsiteUrl : string.Empty;
            bool ratingIsNumber = float.TryParse(ep.EpisodeRating, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out float rating);
            string siteRating = ratingIsNumber && rating > 0
                ? rating + "/10" + AddRatingCount(ep.SiteRatingCount??0)
                : "";

            string imdbLink = string.IsNullOrWhiteSpace(ep.ImdbCode) ? string.Empty : "http://www.imdb.com/title/" + ep.ImdbCode;
            string productionCode = string.IsNullOrWhiteSpace(ep.ProductionCode)
                ? string.Empty
                : "Production Code <br/>" + ep.ProductionCode;

            string episodeDescriptor = CustomEpisodeName.NameForNoExt(ep, CustomEpisodeName.OldNStyle(6)); // may need to include (si.DVDOrder && snum == 0)? ep.Name:
            string writersHtml = string.IsNullOrWhiteSpace(ep.Writer) ? string.Empty : "<b>Writers:</b> " + ep.Writers.ToCsv();
            string directorsHtml = string.IsNullOrWhiteSpace(ep.EpisodeDirector) ? string.Empty : "<b>Directors:</b> " + ep.Directors.ToCsv();
            string guestHtml = string.IsNullOrWhiteSpace(ep.EpisodeGuestStars) ? string.Empty : "<b>Guest Stars:</b> " + ep.GuestStars.ToCsv();

            bool directorsIsBlank = string.IsNullOrWhiteSpace(directorsHtml);
            bool writersIsBlank = string.IsNullOrWhiteSpace(writersHtml);
            string possibleBreak1 = writersIsBlank || directorsIsBlank
                ? string.Empty
                : "<br />";

            bool writersAndDirectorsBlank = writersIsBlank && directorsIsBlank;
            string possibleBreak2 = writersAndDirectorsBlank || string.IsNullOrWhiteSpace(guestHtml)
                ? string.Empty
                : "<br />";

            string searchButton = (fl is null || fl.Count==0) && ep.HasAired()
                ? CreateButton(TVSettings.Instance.BTSearchURL(ep), "<i class=\"fas fa-search\"></i>","Search for Torrent...")
                : string.Empty;

            string viewButton = string.Empty;
            string explorerButton = string.Empty;
            if (fl != null)
            {
                foreach (string urlFilename in fl.Select(fi => Uri.EscapeDataString(fi.FullName)))
                {
                    viewButton += CreateButton($"{UI.WATCH_PROXY}{urlFilename}", "<i class=\"far fa-eye\"></i>","Watch Now");
                    explorerButton += CreateButton($"{UI.EXPLORE_PROXY}{urlFilename}", "<i class=\"far fa-folder-open\"></i>","Open Containing Folder");
                }
            }

            string tvdbButton = CreateButton(tvdbEpisodeUrl, "TVDB.com","View on TVDB");
            string tvMazeButton = CreateButton(ep.Show.Provider==TVDoc.ProviderType.TVmaze? ep.LinkUrl:null, "TVmaze.com", "View on TV maze");
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
                    {CreateButton(EditTvEpisodeUrl(ep), "<i class=\"far fa-edit\"></i>", "Edit")}
                    {searchButton}
                    {viewButton}
                    {explorerButton}
                    {tvdbButton}
                    {tvMazeButton}
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

        [NotNull]
        public static string AddRatingCount(int siteRatingCount)
        {
            return siteRatingCount > 0 ? $" (From {siteRatingCount} Vote{(siteRatingCount == 1 ? "" : "s")})" : string.Empty;
        }

        [NotNull]
        private static string CreateButton(string? link, string text, string tooltip)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return string.Empty;
            }

            return $"<a href=\"{link}\" class=\"btn btn-outline-secondary\" role=\"button\" aria-disabled=\"true\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"{tooltip}\">{text}</a>";
        }

        [NotNull]
        private static string DateDetailsHtml([NotNull] this ProcessedEpisode ei)
        {
            DateTime? dt = ei.GetAirDateDt(true);
            if (dt != null && dt.Value.CompareTo(DateTime.MaxValue) != 0)
            {
                return $"<h6>{dt.Value.ToShortDateString()}</h6><small class=\"text-muted\">({ei.HowLong()})</small>";
            }

            return string.Empty;
        }

        [NotNull]
        public static string GetShowImagesHtmlOverview([NotNull] this ShowConfiguration si)
        {
            string body =
                $"<h1><A HREF=\"{TheTVDB.API.WebsiteShowUrl(si)}\">{si.ShowName}</A> </h1>";

            CachedSeriesInfo ser = si.CachedShow;
            if (ser is null)
            {
                return body;
            }

            body += ImageSection("Show Banner", 758, 140, ser.GetSeriesWideBannerPath(),si.Provider);
            body += ImageSection("Show Poster", 350, 500, ser.GetSeriesPosterPath(), si.Provider);
            body += ImageSection("Show Fanart", 960, 540, ser.GetSeriesFanartPath(), si.Provider);
            return body;
        }

        [NotNull]
        private static string ImageSection(string title, int width, int height, string? bannerPath,TVDoc.ProviderType p)
        {
            if (string.IsNullOrEmpty(bannerPath))
            {
                return "";
            }

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            string url = p switch
            {
                TVDoc.ProviderType.TVmaze => bannerPath,
                TVDoc.ProviderType.TheTVDB => TheTVDB.API.GetImageURL(bannerPath),
                TVDoc.ProviderType.TMDB => bannerPath,
                _ => throw new ArgumentOutOfRangeException(nameof(p), p, null)
            };

            return string.IsNullOrEmpty(url) ? "" : $"<h2>{title}</h2><img width={width} height={height} src=\"{url}\"><br/>";
        }

        private static string HiddenOverview([NotNull] this ProcessedEpisode ei)
        {
            if (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired")
            {
                return Resources.Spoilers_Hidden_Text;
            }

            if(ei.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
            {
                return string.Join("<p/><hr/><p class=\"lead\" />", ei.SourceEpisodes.Select(e => e.Overview));
            }

            return ei.Overview ??string.Empty;
        }

        [NotNull]
        public static string GetSeasonImagesHtmlOverview([NotNull] this ShowConfiguration si, [NotNull] ProcessedSeason s)
        {
            int snum = s.SeasonNumber;
            string body = $"<h1>{si.ShowName} - {ProcessedSeason.UIFullSeasonWord(snum)}</h1>";

            if (TVSettings.Instance.NeedToDownloadBannerFile())
            {
                body += ImageSection("Series Banner", 758, 140, si.CachedShow?.GetSeasonWideBannerPath(snum),si.Provider);
                body += ImageSection("Series Poster", 350, 500, si.CachedShow?.GetSeasonBannerPath(snum), si.Provider);
            }
            else
            {
                body +=
                    "<h2>Images are not being downloaded for this cachedSeries. Please see Options -> Preferences -> Media Center to reconfigure.</h2>";
            }

            return body;
        }

        [NotNull]
        public static string GetMovieImagesHtmlOverview([NotNull] this MovieConfiguration si)
        {
            string body = $"<h1>{si.ShowName} - Images</h1>";

            if (TVSettings.Instance.NeedToDownloadBannerFile())
            {
                body += ImageSection("Poster", 350, 500, si.CachedMovie?.PosterUrl, si.Provider);
                body += ImageSection("Fanart", 960, 540, si.CachedMovie?.FanartUrl, si.Provider);
            }
            else
            {
                body +=
                    "<h2>Images are not being downloaded for this cachedMovie. Please see Options -> Preferences -> Media Center to reconfigure.</h2>";
            }

            return body;
        }

        public static string SeasonName([NotNull] ShowConfiguration si, int snum)
        {
            switch (si.Order)
            {
                case ProcessedSeason.SeasonType.dvd:
                    return snum == 0
                        ? "Not Available on DVD"
                        : "DVD " + ProcessedSeason.UISeasonWord(snum);

                case ProcessedSeason.SeasonType.aired:
                    return ProcessedSeason.UIFullSeasonWord(snum);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [NotNull]
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

        [NotNull]
        private static string ActorLinkHtml([NotNull] Actor actor)
        {
            string asText = actor.AsSelf()?string.Empty
                : string.IsNullOrWhiteSpace(actor.ActorRole) ? string.Empty
                : " as " + actor.ActorRole;
            string tryText =
                $@"<a href=""http://www.imdb.com/find?s=nm&q={actor.ActorName}"">{actor.ActorName}</a>{asText}";
            return tryText;
        }

        [NotNull]
        private static string StarRating(string? rating)
        {
            try
            {
                if (!rating.HasValue())
                {
                    return StarRating(0);
                }
                float f = float.Parse(rating!, CultureInfo.CreateSpecificCulture("en-US"));

                return StarRating(f / 2);
            }
            catch (FormatException)
            {
                return StarRating(0);
            }
        }

        [NotNull]
        internal static string StarRating(float f)
        {
            const string STAR = @"<i class=""fas fa-star""></i>";
            const string HALFSTAR = @"<i class=""fas fa-star-half""></i>";

            if (f < .25)
            {
                return "";
            }

            if (f <= .75)
            {
                return HALFSTAR;
            }

            if (f > 1)
            {
                return STAR + StarRating(f - 1);
            }

            return STAR;
        }

        // ReSharper disable once InconsistentNaming
        [NotNull]
        internal static string HTMLHeader(int size,Color backgroundColour)
        {
            return @"<!DOCTYPE html>
                <html><head>
                <meta charset=""utf-8"">
                <meta name = ""viewport"" content = ""width=device-width, initial-scale=1.0"" >
                <title> TV Rename - Show Summary</title>
                <link rel = ""stylesheet"" href = ""http://maxcdn.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css"" />
                <link rel = ""stylesheet"" href = ""https://use.fontawesome.com/releases/v5.14.0/css/all.css"" />
                </head >"
                + $"<body style=\"background-color: {backgroundColour.HexColour()}\" ><div class=\"col-sm-{size} offset-sm-{(12 - size) / 2}\">";
        }

        [NotNull]
        private static string HexColour(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        // ReSharper disable once InconsistentNaming
        [NotNull]
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

        [NotNull]
        public static string GetShowHtmlOverviewOffline([NotNull] this ShowConfiguration si)
        {
            string body = string.Empty;
            CachedSeriesInfo ser = si.CachedShow;

            if (!(ser is null) &&
                !string.IsNullOrEmpty(ser.GetSeriesWideBannerPath()) &&
                !string.IsNullOrEmpty(TheTVDB.API.GetImageURL(ser.GetSeriesWideBannerPath())))
            {
                body += "<img width=758 height=140 src=\"" + TheTVDB.API.GetImageURL(ser.GetSeriesWideBannerPath()) +
                        "\"><br/>";
            }

            body += $"<h1><A HREF=\"{TheTVDB.API.WebsiteShowUrl(si)}\">{si.ShowName}</A> </h1>";

            body += "<h2>Overview</h2>" + ser?.Overview; //get overview in either format

            bool first = true;
            foreach (Actor aa in si.Actors.Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
            {
                body += first ? "<h2>Actors</h2>" : ", ";

                body += "<A HREF=\"http://www.imdb.com/find?s=nm&q=" + aa.ActorName + "\">" + aa.ActorName + $"</a> as {aa.ActorRole}";
                first = false;
            }

            string airsTime = ser?.AirsTime.PrettyPrint();
            string? airsDay = ser?.AirsDay;
            if (!string.IsNullOrEmpty(airsTime) && !string.IsNullOrEmpty(airsDay))
            {
                body += "<h2>Airs</h2> " + airsTime + " " + airsDay;
                string net = ser.Network;
                if (!string.IsNullOrEmpty(net))
                {
                    body += ", " + net;
                }
            }

            string yearRange = YearRange(ser);

            string siteRating = (ser?.SiteRating??0) > 0 ? ser.SiteRating + "/10" : string.Empty;
            string tvdbLink = si.TvdbCode > 0 ? TheTVDB.API.WebsiteShowUrl(si) : string.Empty;

            string tableHtml = string.Empty;

            tableHtml += GetOverviewPart("thetvdb.com", $"<A HREF=\"{tvdbLink}\">Visit</a>");
            tableHtml += GetOverviewPart("imdb.com", "<A HREF=\"http://www.imdb.com/title/" + ser?.Imdb + "\">Visit</a>");
            tableHtml += GetOverviewPart("tv.com", "<A HREF=\"http://www.tv.com/show/" + ser?.SeriesId + "/summary.html\">Visit</a>");
            tableHtml += GetOverviewPart("Runtime", ser?.Runtime);
            tableHtml += GetOverviewPart("Aliases", si.AliasNames.ToCsv());
            tableHtml += GetOverviewPart("Genres", si.Genres.ToCsv());
            tableHtml += GetOverviewPart("Rating", ser?.ContentRating);
            tableHtml += GetOverviewPart("User Rating", $"{siteRating}{AddRatingCount(ser?.SiteRatingVotes??0)}");
            tableHtml += GetOverviewPart("Active From", yearRange);
            tableHtml += GetOverviewPart("Status", ser?.Status);

            if (tableHtml.HasValue())
            {
                body += "<h2>Information<table border=0>"+tableHtml+"</table>";
            }

            return body;
        }

        [NotNull]
        public static string GetSeasonHtmlOverviewOffline([NotNull] this ShowConfiguration si, [NotNull] ProcessedSeason s)
        {
            CachedSeriesInfo ser = si.CachedShow;
            int snum = s.SeasonNumber;
            string body = string.Empty;

            if (!string.IsNullOrEmpty(ser?.GetSeriesWideBannerPath()) &&
                !string.IsNullOrEmpty(TheTVDB.API.GetImageURL(ser.GetSeriesWideBannerPath())))
            {
                body += "<img width=758 height=140 src=\"" + TheTVDB.API.GetImageURL(ser.GetSeriesWideBannerPath()) +
                        "\"><br/>";
            }

            List<ProcessedEpisode> eis = si.SeasonEpisodes[snum];

            string seasText = SeasonName(si, snum);

            if (eis.Count > 0 && eis[0].SeasonId > 0)
            {
                seasText = " - <A HREF=\"" + TheTVDB.API.WebsiteSeasonUrl(s) + "\">" +
                           seasText + "</a>";
            }
            else
            {
                seasText = " - " + seasText;
            }

            body += "<h1><A HREF=\"" + TheTVDB.API.WebsiteShowUrl(si) + "\">" + si.ShowName +
                    "</A>" + seasText + "</h1>";

            DirFilesCache dfc = new DirFilesCache();
            foreach (ProcessedEpisode ei in eis)
            {
                string epl = ei.EpNumsAsString();

                string episodeUrl = TheTVDB.API.WebsiteEpisodeUrl(ei);

                body += "<A href=\"" + episodeUrl + "\" name=\"ep" + epl + "\">"; // anchor
                body += "<b>" + EpisodeName(si, snum, ei) + "</b>";
                body += "</A>"; // anchor
                if (si.UseSequentialMatch && ei.OverallNumber != -1)
                {
                    body += " (#" + ei.OverallNumber + ")";
                }

                List<FileInfo> fl = dfc.FindEpOnDisk(ei);
                if (fl.Count>0)
                {
                    foreach (FileInfo fi in fl)
                    {
                        string urlFilename = HttpUtility.UrlEncode(fi.FullName);
                        body += $" <A HREF=\"watch://{urlFilename}\" class=\"search\">Watch</A>";
                        body += $" <A HREF=\"explore://{urlFilename}\" class=\"search\">Show in Explorer</A>";
                    }
                }
                else
                {
                    body += " <A HREF=\"" + TVSettings.Instance.BTSearchURL(ei) + "\" class=\"search\">Search</A>";
                }

                DateTime? dt = ei.GetAirDateDt(true);
                if (dt != null && dt.Value.CompareTo(DateTime.MaxValue) != 0)
                {
                    body += "<p>" + dt.Value.ToShortDateString() + " (" + ei.HowLong() + ")";
                }

                body += "<p><p>";

                bool hideMyShowsSpoilersNow = TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired";
                if (TVSettings.Instance.ShowEpisodePictures || hideMyShowsSpoilersNow)
                {
                    body += "<table><tr>";
                    body += "<td width=100% valign=top>" + GetOverview(ei) + "</td><td width=300 height=225>";
                    // 300x168 / 300x225
                    if (!string.IsNullOrEmpty(ei.Filename))
                    {
                        body += "<img src=" + TheTVDB.API.GetImageURL(ei.Filename) + ">";
                    }

                    body += "</td></tr></table>";
                }
                else
                {
                    body += GetOverview(ei);
                }

                body += "<p><hr><p>";
            } // for each episode in this season

            return body;
        }

        public static string GetMovieHtmlOverviewOffline([NotNull] this MovieConfiguration si)
        {
            string body = string.Empty;
            CachedMovieInfo? ser = si.CachedMovie;

            body += $"<h1><A HREF=\"{ser?.OfficialUrl}\">{si.ShowName}</A> </h1>";

            body += "<h2>Overview</h2>" + ser?.Overview; //get overview in either format

            bool first = true;
            foreach (Actor aa in si.Actors.Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
            {
                body += first ? "<h2>Actors</h2>" : ", ";

                body += "<A HREF=\"http://www.imdb.com/find?s=nm&q=" + aa.ActorName + "\">" + aa.ActorName + $"</a> as {aa.ActorRole}";
                first = false;
            }

            string siteRating = (ser?.SiteRating ?? 0) > 0 ? ser?.SiteRating + "/10" : string.Empty;
            string tvdbLink = si.TvdbCode > 0 ? TheTVDB.API.WebsiteShowUrl(ser.TvdbCode) : string.Empty;
            string tmdbLink = si.TmdbCode > 0 ? $"https://www.themoviedb.org/movie/{si.TmdbCode}" : string.Empty;
            string mazeLink = ser?.TvMazeCode > 0 ? ser.WebUrl : string.Empty;
            string facebookButton = ser.FacebookId.HasValue() ? $"https://facebook.com/{ser.FacebookId}" : string.Empty;
            string instaButton = ser.InstagramId.HasValue() ? $"https://instagram.com/{ser.InstagramId}" : string.Empty;
            string twitterButton = ser.TwitterId.HasValue() ? $"https://twitter.com/{ser.TwitterId}" : string.Empty;

            string tableHtml = string.Empty;

            tableHtml += GetOverviewPart("thetvdb.com", $"<A HREF=\"{tvdbLink}\">Visit</a>");
            tableHtml += GetOverviewPart("imdb.com", "<A HREF=\"http://www.imdb.com/title/" + ser?.Imdb + "\">Visit</a>");
            tableHtml += GetOverviewPart("tv.com", "<A HREF=\"http://www.tv.com/show/" + ser?.SeriesId + "/summary.html\">Visit</a>");
            if (tmdbLink.HasValue()) tableHtml += GetOverviewPart("MovieDB", $"<A HREF=\"{tmdbLink}\">Visit</a>");
            if (mazeLink.HasValue()) tableHtml += GetOverviewPart("TV Maze", $"<A HREF=\"{mazeLink}\">Visit</a>");
            if (facebookButton.HasValue()) tableHtml += GetOverviewPart("Facebook", $"<A HREF=\"{facebookButton}\">Visit</a>");
            if (instaButton.HasValue()) tableHtml += GetOverviewPart("Instagram", $"<A HREF=\"{instaButton}\">Visit</a>");
            if (twitterButton.HasValue()) tableHtml += GetOverviewPart("Twitter", $"<A HREF=\"{twitterButton}\">Visit</a>");

            tableHtml += GetOverviewPart("Runtime", ser?.Runtime);
            tableHtml += GetOverviewPart("Aliases", si.AliasNames.ToCsv());
            tableHtml += GetOverviewPart("Genres", si.Genres.ToCsv());
            tableHtml += GetOverviewPart("Rating", ser?.ContentRating);
            tableHtml += GetOverviewPart("Network", ser?.Network);
            tableHtml += GetOverviewPart("Network", ser?.ContentRating);
            tableHtml += GetOverviewPart("User Rating", $"{siteRating}{AddRatingCount(ser?.SiteRatingVotes ?? 0)}");
            tableHtml += GetOverviewPart("Released", ser?.Year?.ToString());
            tableHtml += GetOverviewPart("Tagline", ser?.TagLine);
            tableHtml += GetOverviewPart("Status", ser?.Status);
            tableHtml += GetOverviewPart("Type", ser?.Type);

            if (!string.IsNullOrWhiteSpace(tableHtml))
            {
                body += "<h2>Information<table border=0>" + tableHtml + "</table>";
            }
                    
            return body;
        }

        [NotNull]
        private static string EpisodeName([NotNull] ShowConfiguration si, int snum,  [NotNull] ProcessedEpisode ei)
        {
            if (si.Order == ProcessedSeason.SeasonType.dvd && snum == 0)
            {
                return ei.Name;
            }
            return HttpUtility.HtmlEncode(CustomEpisodeName.NameForNoExt(ei, CustomEpisodeName.OldNStyle(6)));
        }

        private static string GetOverview([NotNull] ProcessedEpisode ei)
        {
            string overviewString=string.Empty;

            overviewString += GetOverviewPart("imdb.com", "<A HREF=\"http://www.imdb.com/title/" + ei.ImdbCode + "\">Visit</a>");
            overviewString += GetOverviewPart("Link", "<A HREF=\"" +ei.ShowUrl+ "\">Visit</a>");
            overviewString += GetOverviewPart("Director", ei.EpisodeDirector);
            overviewString += GetOverviewPart("Guest Stars", ei.EpisodeGuestStars);
            overviewString += GetOverviewPart("Production Code", ei.ProductionCode);
            overviewString += GetOverviewPart("Writer", ei.Writer);

            if (!string.IsNullOrWhiteSpace(overviewString))
            {
                return ei.HiddenOverview() + "<table border=0>" + overviewString + "</table>";
            }

            return ei.HiddenOverview();
        }

        [NotNull]
        private static string GetOverviewPart(string name, string? value)
        {
            return string.IsNullOrWhiteSpace(value)? string.Empty: "<tr><td width=120px>" + name + "</td><td>" + value + "</td></tr>";
        }

        public static string? YoutubeTrailer(CachedMediaInfo? si)
        {
            return si?.TrailerUrl?.Replace("/watch?v=", "/embed/");
        }
    }
}
