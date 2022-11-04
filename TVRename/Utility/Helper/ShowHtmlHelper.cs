using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using TVRename.Forms;
using TVRename.Properties;

namespace TVRename;

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

    public static string GetShowHtmlOverview(this ShowConfiguration si, bool includeDirectoryLinks)
    {
        Color col = Color.FromName("ButtonFace");
        StringBuilder sb = new();
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendShow(si, col, includeDirectoryLinks);
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    public static string GetSeasonImagesOverview(this ProcessedSeason season)
    {
        Color col = Color.FromName("ButtonFace");
        StringBuilder sb = new();
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendSeasonImages(col, season);
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    public static string GetShowImagesOverview(this ShowConfiguration si)
    {
        Color col = Color.FromName("ButtonFace");
        StringBuilder sb = new();
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendShowImages(si, col);
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    public static string GetMovieImagesOverview(this MovieConfiguration si)
    {
        Color col = Color.FromName("ButtonFace");
        StringBuilder sb = new();
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendMovieImages(si, col);
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }
    public static string GetShowHtmlOverview(this CachedSeriesInfo series, RecommendationRow recommendation)
    {
        Color col = Color.FromName("ButtonFace");
        StringBuilder sb = new();
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendShow(null, series, col, false);
        sb.AppendRecommendation(recommendation, col);
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    public static string GetMovieHtmlOverview(this MovieConfiguration si, bool includeDirectoryLinks)
    {
        Color col = Color.FromName("ButtonFace");
        StringBuilder sb = new();
        DirFilesCache dfc = new();
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendMovie(si, col, includeDirectoryLinks,dfc);
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    public static string GetMovieHtmlOverview(this CachedMovieInfo movie, RecommendationRow? recommendation)
    {
        Color col = Color.FromName("ButtonFace");
        DirFilesCache dfc = new();
        StringBuilder sb = new();
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendMovie(null, movie, col, false,dfc);
        if (recommendation != null)
        {
            sb.AppendRecommendation(recommendation, col);
        }
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    public static string GetShowSummaryHtmlOverview(this ShowConfiguration si, bool includeDirectoryLinks)
    {
        Color col = Color.FromName("ButtonFace");
        DirFilesCache dfc = new();
        StringBuilder sb = new();

        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendShowSummary(si, dfc, col, includeDirectoryLinks);
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    private static string ToImdbLink(this string? imdbId)
    {
        return string.IsNullOrWhiteSpace(imdbId) ? string.Empty : $"https://www.imdb.com/title/{imdbId}";
    }

    private static void AppendShowSummary(this StringBuilder sb, ShowConfiguration? si, DirFilesCache dfc, Color backgroundColour, bool includeDirectoryLinks)
    {
        CachedSeriesInfo? ser = si?.CachedShow;
        if (ser is null)
        {
            return;
        }

        string horizontalBanner = CreateHorizontalBannerHtml(si);
        string yearRange = YearRange(ser);
        string episodeSummary = ser.Episodes.Count.ToString();
        string imdbLink = ser.Imdb.ToImdbLink();
        string table = CreateEpisodeTableHeader(CreateTableRows(si!, dfc, includeDirectoryLinks));

        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                <div class=""text-center"">
	             {horizontalBanner}
                </div>
                    <div class=""row"">
                     <div class=""col-md-8""><h1>{si!.ShowName}</h1><small class=""text-muted"">{ser.ShowLanguage} - {ser.SeriesType}</small></div>
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
        StringBuilder tableRows = new();

        foreach (ProcessedSeason season in si.AppropriateSeasons().OrderBy(pair => pair.Key).Select(pair => pair.Value))
        {
            if (si.SeasonEpisodes.TryGetValue(season.SeasonNumber, out List<ProcessedEpisode>? seasonEpisodes))
            {
                tableRows.AppendSeasonShowSummary(dfc, si, season, includeDirectoryLinks, seasonEpisodes);
            }
        }

        return tableRows.ToString();
    }

    private static void AppendSeasonShowSummary(this StringBuilder sb, DirFilesCache dfc, ShowConfiguration si, ProcessedSeason s, bool includeDirectoryLinks, IEnumerable<ProcessedEpisode> seasonEpisodes)
    {
        string explorerButton = string.Empty;
        if (includeDirectoryLinks)
        {
            string urlFilename = Uri.EscapeDataString(si.GetBestFolderLocationToOpen(s));
            explorerButton = CreateExploreButton(urlFilename);
        }

        string tableRows = seasonEpisodes.Select(episode => SeasonSummaryTableRow(episode, includeDirectoryLinks, dfc)).Concat();

        string? tvdbSLug = si.CachedShow?.Slug;
        string tvdbLink = !tvdbSLug.HasValue() ? string.Empty : TheTVDB.API.WebsiteSeasonUrl(s);
        string tvdbButton = CreateButton(tvdbLink, "TVDB.com", "View on TVDB");
        string tvMazeButton = CreateButton(s.Show.Provider != TVDoc.ProviderType.TVmaze ? string.Empty : s.WebsiteUrl, "TVmaze.com", "View on TV Maze");
        string episodeText = !s.Episodes.IsEmpty ? $"<br/><small class=\"text-muted\">{s.Episodes.Count} Episodes</small>" : string.Empty;

        Season? season = si.CachedShow?.Season(s.SeasonNumber);
        string seasonOverViewHtml = HasCustomNameOrDescription(season)
            ? $"<h4>{SeasonName(si, s.SeasonNumber)} - {season?.SeasonName}</h4>{season?.SeasonDescription}"
            : SeasonName(si, s.SeasonNumber);

        sb.AppendLine($@"     <tr class=""table-secondary"">
      <td scope=""row"" colspan=""4"">{seasonOverViewHtml}{episodeText}</td>
      <td class=""text-right"">
{CreateButton(EditSeasonUrl(si, s), "<i class=\"far fa-edit\"></i>", "Edit")}
{explorerButton}
            {tvdbButton}
            {tvMazeButton}</td>
    </tr>
{tableRows}");
    }

    private static bool HasCustomNameOrDescription(Season? season)
    {
        if (season is null)
        {
            return false;
        }

        if (season.SeasonDescription.HasValue())
        {
            return true;
        }

        if (season.SeasonName.IsNullOrWhitespace())
        {
            return false;
        }

        return season.SeasonName is null || season.SeasonName.Trim().ToInt() != season.SeasonNumber;
    }

    private static void AppendShow(this StringBuilder sb, ShowConfiguration si, Color backgroundColour,
        bool includeDirectoryLinks)
    {
        CachedSeriesInfo? ser = si.CachedShow;

        if (ser is null)
        {
            return;
        }
        AppendShow(sb, si, ser, backgroundColour, includeDirectoryLinks);
    }

    private static void AppendShowImages(this StringBuilder sb, ShowConfiguration? si, Color backgroundColour)
    {
        CachedSeriesInfo? ser = si?.CachedShow;

        if (ser is null)
        {
            return;
        }

        string posterHtml = GenerateImageCarousel("Posters", ser.Images(MediaImage.ImageType.poster, MediaImage.ImageSubject.show).ToList(), "MyPosterCarousel", "w-50");
        string bannerHtml = GenerateImageCarousel("Banners", ser.Images(MediaImage.ImageType.wideBanner, MediaImage.ImageSubject.show).ToList(), "MyBannerCarousel", "w-100");
        string fanartHtml = GenerateImageCarousel("Backgrounds", ser.Images(MediaImage.ImageType.background).ToList(), "MyBackgroundCarousel", "w-100");
        string iconHtml = GenerateImageCarousel("Icons", ser.Images(MediaImage.ImageType.icon).ToList(), "MyIconCarousel", "w-100");
        string clearArtHtml = GenerateImageCarousel("Clear Art", ser.Images(MediaImage.ImageType.clearArt).ToList(), "MyClearArtCarousel", "w-100");
        string clearLogoHtml = GenerateImageCarousel("Clear Logo", ser.Images(MediaImage.ImageType.clearLogo).ToList(), "MyClearLogoCarousel", "w-100");

        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {posterHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {bannerHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {fanartHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {iconHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {clearArtHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {clearLogoHtml}
                  </div>

                </div>");
    }

    private static void AppendSeasonImages(this StringBuilder sb, Color backgroundColour,ProcessedSeason season)
    {
        CachedSeriesInfo? ser = season.Show.CachedShow;

        if (ser is null)
        {
            return;
        }

        string posterHtml = GenerateImageCarousel("Posters", season.Images(MediaImage.ImageType.poster, MediaImage.ImageSubject.season)?.ToList(), "MyPosterCarousel", "w-50");
        string bannerHtml = GenerateImageCarousel("Banners", season.Images(MediaImage.ImageType.wideBanner, MediaImage.ImageSubject.season)?.ToList(), "MyBannerCarousel", "w-100");
        string fanartHtml = GenerateImageCarousel("Backgrounds", season.Images(MediaImage.ImageType.background)?.ToList(), "MyBackgroundCarousel", "w-100");
        string iconHtml = GenerateImageCarousel("Icons", season.Images(MediaImage.ImageType.icon)?.ToList(), "MyIconCarousel", "w-100");

        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {posterHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {bannerHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {fanartHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {iconHtml}
                  </div>

                </div>");
    }

    private static void AppendMovieImages(this StringBuilder sb, MovieConfiguration? si, Color backgroundColour)
    {
        CachedMovieInfo? ser = si?.CachedMovie;

        if (ser is null)
        {
            return;
        }

        string posterHtml = GenerateImageCarousel("Posters", ser.Images(MediaImage.ImageType.poster, MediaImage.ImageSubject.movie).ToList(), "MyPosterCarousel", "w-50");
        string bannerHtml = GenerateImageCarousel("Banners", ser.Images(MediaImage.ImageType.wideBanner, MediaImage.ImageSubject.show).ToList(), "MyBannerCarousel", "w-100");
        string fanartHtml = GenerateImageCarousel("Backgrounds", ser.Images(MediaImage.ImageType.background).ToList(), "MyBackgroundCarousel", "w-100");
        string iconHtml = GenerateImageCarousel("Icons", ser.Images(MediaImage.ImageType.icon).ToList(), "MyIconCarousel", "w-100");
        string clearArtHtml = GenerateImageCarousel("Clear Art", ser.Images(MediaImage.ImageType.clearArt).ToList(), "MyClearArtCarousel", "w-100");
        string clearLogoHtml = GenerateImageCarousel("Clear Logo", ser.Images(MediaImage.ImageType.clearLogo).ToList(), "MyClearLogoCarousel", "w-100");

        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {posterHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {fanartHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {bannerHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {iconHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {clearArtHtml}
                  </div>

                </div>");
        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                    {clearLogoHtml}
                  </div>

                </div>");
    }

    private static string GenerateImageCarousel(string title, IReadOnlyCollection<MediaImage>? images, string id, string imageTag)
    {
        if (images is null || images.Count == 0)
        {
            return $@" <h2>{title}</h2><div>No Images Found</div>";
        }

        return $@" <h2>{title}</h2>
<div id=""{id}"" class=""carousel slide"" data-bs-ride=""carousel"">
                <div class=""carousel-indicators"">
{GenerateCarouselIndicators(images, id)}

    </div>
    <div class=""carousel-inner"">
      {GenerateCarouselBlocks(images, imageTag)}
    </div>
    <button class=""carousel-control-prev"" type=""button"" data-bs-target=""#{id}"" data-bs-slide=""prev"">
      <span class=""carousel-control-prev-icon"" aria-hidden=""true""></span>
      <span class=""visually-hidden"">Previous</span>
    </button>
    <button class=""carousel-control-next"" type=""button"" data-bs-target=""#{id}"" data-bs-slide=""next"">
      <span class=""carousel-control-next-icon"" aria-hidden=""true""></span>
      <span class=""visually-hidden"">Next</span>
    </button>
  </div>";
    }

    private static string GenerateCarouselBlocks(IEnumerable<MediaImage> images, string imageTag)
    {
        StringBuilder sb = new();
        bool isFirst = true;
        foreach (MediaImage i in images)
        {
            if (isFirst)
            {
                sb.Append("<div class=\"carousel-item active\">");
                isFirst = false;
            }
            else
            {
                sb.Append("<div class=\"carousel-item\">");
            }

            sb.AppendLine(
                $"<div class=\"container\"><img class=\"show-poster rounded {imageTag}\" src=\"{i.ImageUrl}\"></div></div>");
        }

        return sb.ToString();
    }

    private static string GenerateCarouselIndicators(IEnumerable<MediaImage> images, string id)
    {
        StringBuilder sb = new();
        bool isFirst = true;
        int c = 0;
        foreach (MediaImage? _ in images)
        {
            sb.Append($"<button type=\"button\" data-bs-target=\"#{id}\" data-bs-slide-to=\"{c++}\" ");
            if (isFirst)
            {
                sb.Append("class=\"active\" aria-current=\"true\" ");
                isFirst = false;
            }

            sb.AppendLine("aria-label=\"Slide {c}\"></button>");
        }

        return sb.ToString();
    }

    private static void AppendShow(this StringBuilder sb, ShowConfiguration? si, CachedSeriesInfo ser, Color backgroundColour, bool includeDirectoryLinks)
    {
        string horizontalBanner = CreateHorizontalBannerHtml(si);
        string poster = CreatePosterHtml(ser);
        string yearRange = YearRange(ser);
        int episodeSummary = ser.Episodes.Count;
        string episodeText = episodeSummary > 0 ? $"{episodeSummary} Episodes" : string.Empty;
        string stars = StarRating(ser.SiteRating / 2);
        string genreIcons = string.Join("&nbsp;", ser.Genres.Select(GenreIconHtml));
        string siteRating = PrettyPrint(ser.SiteRating);
        string runTimeHtml = string.IsNullOrWhiteSpace(ser.Runtime) ? string.Empty : $"<br/> {ser.Runtime} min";
        string actorLinks = ser.GetActors().Select(ActorLinkHtml).ToCsv();
        string dayTime = $"{ser.AirsDay} {ParseAirsTime(ser)}";

        string imdbLink = ser.Imdb.ToImdbLink();
        string? mazeLink = ser.TvMazeCode <= 0 ? string.Empty : ser.WebUrl;
        string tmdbLink = ser.TmdbCode > 0 ? TMDB.API.WebsiteShowUrl(ser) : string.Empty;
        string tvdbLink = ser.TvdbCode > 0 ? TheTVDB.API.WebsiteShowUrl(ser) : string.Empty;

        string urlFilename = includeDirectoryLinks
            ? Uri.EscapeDataString(si?.GetBestFolderLocationToOpen() ?? string.Empty)
            : string.Empty;
        string explorerButton = includeDirectoryLinks
            ? CreateExploreButton(urlFilename)
            : string.Empty;

        string facebookButton = ser.FacebookId.HasValue() ? CreateButton($"https://facebook.com/{ser.FacebookId}", "<i class=\"fab fa-facebook\"></i>", "Facebook") : string.Empty;
        string instagramButton = ser.InstagramId.HasValue() ? CreateButton($"https://instagram.com/{ser.InstagramId}", "<i class=\"fab fa-instagram\"></i>", "Instagram") : string.Empty;
        string twitterButton = ser.TwitterId.HasValue() ? CreateButton($"https://twitter.com/{ser.TwitterId}", "<i class=\"fab fa-twitter\"></i>", "Twitter") : string.Empty;

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
                     <div class=""col-md-8""><h1>{si?.ShowName ?? ser.Name}</h1><small class=""text-muted"">{ser.ShowLanguage} - {ser.SeriesType}</small></div>
                     <div class=""col-md-4 text-right""><h6>{yearRange} ({ser.Status})</h6><small class=""text-muted"">{episodeText}{runTimeHtml}</small></div>
                    </div>
                    <div><p class=""lead"">{ser.Overview}</p></div>");
        if(ser.GetCrew().Any() || ser.GetActors().Any() )
        {
            sb.AppendLine($@"<div class=""accordion accordion-flush"" id=""accordionCastCrew"">
  <div class=""accordion-item"" style=""background-color:#F0F0F0"">
    <h2 class=""accordion-header"" id=""flush-headingOne"" style=""background-color:#F0F0F0"">
      <button class=""accordion-button collapsed"" type=""button"" data-bs-toggle=""collapse"" data-bs-target=""#flush-collapseOne"" aria-expanded=""false"" aria-controls=""flush-collapseOne"" style=""background-color:#F0F0F0"">
        Cast
      </button>
    </h2>
    <div id=""flush-collapseOne"" class=""accordion-collapse collapse"" aria-labelledby=""flush-headingOne"" data-bs-parent=""#accordionCastCrew"">
	  <div class=""accordion-body"">
	  <blockquote>{actorLinks}</blockquote>
	  </div>
    </div>
  </div>
  <div class=""accordion-item"" style=""background-color:#F0F0F0"">
    <h2 class=""accordion-header"" id=""flush-headingTwo"" style=""background-color:#F0F0F0"">
      <button class=""accordion-button collapsed"" type=""button"" data-bs-toggle=""collapse"" data-bs-target=""#flush-collapseTwo"" aria-expanded=""false"" aria-controls=""flush-collapseTwo"" style=""background-color:#F0F0F0"">
        Crew
      </button>
    </h2>
    <div id=""flush-collapseTwo"" class=""accordion-collapse collapse"" aria-labelledby=""flush-headingTwo"" data-bs-parent=""#accordionCastCrew"">
      <div class=""accordion-body"">
	  {ser.GetCrew().Select(c => $"{c.Name} as {c.Job}").ToCsv()}
	  </div>
    </div>
  </div>
</div>");
        }
        sb.AppendLine($@"<div><br/></div>
		            <div>
                    {CreateButton(EditTvSeriesUrl(ser), "<i class=\"far fa-edit\"></i>", "Edit")}
                     {explorerButton}
			         {CreateButton(tvdbLink, "TVDB.com", "View on TVDB")}
			         {CreateButton(imdbLink, "IMDB.com", "View on IMDB")}
                     {CreateButton(mazeLink, "TVmaze.com", "View on TVmaze")}
                     {CreateButton(tmdbLink, "TMDB.com", "View on TMDB")}
			         {CreateButton(ser.OfficialUrl, "Official Site", "View on Official Site")}
                    {facebookButton}
                    {instagramButton}
                    {twitterButton}
			        </div>
		            <div>
                        &nbsp;
			        </div>
		            <div class=""row align-items-bottom flex-grow-1"">
                     <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}{AddRatingCount(ser.SiteRatingVotes)}</div>
                     <div class=""col-md-4 align-self-end text-center"">{ser.ContentRating}<br>{ser.Networks.ToCsv()}, {dayTime}</div>
                     <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{ser.Genres.ToCsv()}</div>
                    </div>
                   </div>
                  </div>
                 </div>");
    }

    private static string PrettyPrint(float? rating) => rating is > 0 ? rating.Value + "/10" : string.Empty;
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
        bool includeDirectoryLinks, DirFilesCache dfc)
    {
        CachedMovieInfo? ser = si?.CachedMovie;

        if (ser is null)
        {
            return;
        }
        AppendMovie(sb, si, ser, backgroundColour, includeDirectoryLinks,dfc);
    }

    // ReSharper disable once FunctionComplexityOverflow
    private static void AppendMovie(this StringBuilder sb, MovieConfiguration? si, CachedMovieInfo ser, Color backgroundColour, bool includeDirectoryLinks, DirFilesCache dfc)
    {
        string poster = CreatePosterHtml(ser);
        string yearRange = ser.Year?.ToString() ?? "";
        string stars = StarRating(ser.SiteRating / 2);
        string genreIcons = string.Join("&nbsp;", ser.Genres.Select(GenreIconHtml));
        string siteRating = PrettyPrint(ser.SiteRating);
        string runTimeHtml = string.IsNullOrWhiteSpace(ser.Runtime) ? string.Empty : $"<br/> {ser.Runtime} min";
        string actorLinks = ser.GetActors().Select(ActorLinkHtml).ToCsv();
        string tvdbLink = ser.Slug.HasValue() ? TheTVDB.API.WebsiteMovieUrl(ser.Slug) : string.Empty;
        string imdbLink = ser.Imdb.ToImdbLink();
        string tmdbLink = ser.TmdbCode > 0 ? TMDB.API.WebsiteMovieUrl(ser.TmdbCode) : string.Empty;
        string? mazeLink = ser.TvMazeCode <= 0 ? string.Empty : ser.WebUrl;

        string urlFilename = includeDirectoryLinks && (si !=null) ? Uri.EscapeDataString(dfc.FindMovieOnDisk(si).FirstOrDefault()?.FullName ?? string.Empty) : string.Empty;
        string explorerButton = includeDirectoryLinks ? CreateExploreButton(urlFilename) : string.Empty;
        string viewButton = includeDirectoryLinks ? CreateWatchButton(urlFilename) : string.Empty;
        string facebookButton = ser.FacebookId.HasValue() ? CreateButton($"https://facebook.com/{ser.FacebookId}", "<i class=\"fab fa-facebook\"></i>", "Facebook") : string.Empty;
        string instagramButton = ser.InstagramId.HasValue() ? CreateButton($"https://instagram.com/{ser.InstagramId}", "<i class=\"fab fa-instagram\"></i>", "Instagram") : string.Empty;
        string twitterButton = ser.TwitterId.HasValue() ? CreateButton($"https://twitter.com/{ser.TwitterId}", "<i class=\"fab fa-twitter\"></i>", "Twitter") : string.Empty;

        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">
                  <div class=""row"">
                   <div class=""col-md-4"">
                    {poster}
                   </div>
                   <div class=""col-md-8 d-flex flex-column"">
                    <div class=""row"">
                     <div class=""col-md-8""> <h1>{si?.ShowName ?? ser.Name}</h1><small class=""text-muted"">{ser.TagLine}</small></div>
                     <div class=""col-md-4 text-right""><h6>{yearRange} ({ser.Status})</h6>
                        <small class=""text-muted"">{ser.ShowLanguage} - {ser.MovieType} - {ser.Country}</small>
                        <small class=""text-muted"">{runTimeHtml}</small></div>
                    </div>
                    <div><p class=""lead"">{ser.Overview}</p></div>
			        <div></div>");
        if (ser.GetCrew().Any() || ser.GetActors().Any())
        {
            sb.AppendLine($@"<div class=""accordion accordion-flush"" id=""accordionCastCrew"">
  <div class=""accordion-item"" style=""background-color:#F0F0F0"">
    <h2 class=""accordion-header"" id=""flush-headingOne"" style=""background-color:#F0F0F0"">
      <button class=""accordion-button collapsed"" type=""button"" data-bs-toggle=""collapse"" data-bs-target=""#flush-collapseOne"" aria-expanded=""false"" aria-controls=""flush-collapseOne"" style=""background-color:#F0F0F0"">
        Cast
      </button>
    </h2>
    <div id=""flush-collapseOne"" class=""accordion-collapse collapse"" aria-labelledby=""flush-headingOne"" data-bs-parent=""#accordionCastCrew"">
	  <div class=""accordion-body"">
	  <blockquote>{actorLinks}</blockquote>
	  </div>
    </div>
  </div>
  <div class=""accordion-item"" style=""background-color:#F0F0F0"">
    <h2 class=""accordion-header"" id=""flush-headingTwo"" style=""background-color:#F0F0F0"">
      <button class=""accordion-button collapsed"" type=""button"" data-bs-toggle=""collapse"" data-bs-target=""#flush-collapseTwo"" aria-expanded=""false"" aria-controls=""flush-collapseTwo"" style=""background-color:#F0F0F0"">
        Crew
      </button>
    </h2>
    <div id=""flush-collapseTwo"" class=""accordion-collapse collapse"" aria-labelledby=""flush-headingTwo"" data-bs-parent=""#accordionCastCrew"">
      <div class=""accordion-body"">
	  {ser.GetCrew().Select(c => $"{c.Name} as {c.Job}").ToCsv()}
	  </div>
    </div>
  </div>
</div>");
        }
        sb.AppendLine($@"<div><br/></div>
		            <div>
                    {CreateButton(EditMovieUrl(ser), "<i class=\"far fa-edit\"></i>", "Edit")}
                     {explorerButton}
                    {viewButton}
                     {CreateButton(tmdbLink, "TMDB.com", "View on TMDB")}
			         {CreateButton(tvdbLink, "TVDB.com", "View on TVDB")}
			         {CreateButton(imdbLink, "IMDB.com", "View on IMDB")}
                     {CreateButton(mazeLink, "TVmaze.com", "View on TVmaze")}
			         {CreateButton(ser.OfficialUrl, "Official Site", "View on Official Site")}
                    {facebookButton}
                    {instagramButton}
                    {twitterButton}
			        </div>
		            <div>
                        &nbsp;
			        </div>
		            <div class=""row align-items-bottom flex-grow-1"">
                     <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}{AddRatingCount(ser.SiteRatingVotes)}</div>
                     <div class=""col-md-4 align-self-end text-center"">{ser.ContentRating}<br>{ser.Networks.ToCsv()}</div>
                     <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{ser.Genres.ToCsv()}</div>
                    </div>
                   </div>
                  </div>
                 </div>");
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static string? EditMovieUrl(CachedMovieInfo si)
    {
        if (si.TmdbCode > 0)
        {
            return $"https://www.themoviedb.org/movie/{si.TmdbCode}/edit?active_nav_item=primary_facts";
        }
        return si.Slug.HasValue() ? $"https://thetvdb.com/movies/{si.Slug}/edit" : null;
    }

    private static string? EditSeasonUrl(ShowConfiguration si, ProcessedSeason s)
    {
        switch (si.Provider)
        {
            case TVDoc.ProviderType.TheTVDB:
                if (si.CachedShow?.Slug.HasValue() ?? false)
                {
                    return $"https://thetvdb.com/series/{si.CachedShow.Slug}/seasons/{s.SeasonStyle.PrettyPrint()}/{s.SeasonNumber}/edit";
                }

                return null;

            case TVDoc.ProviderType.TMDB:
                if (si.TmdbCode > 0)
                {
                    return $"https://www.themoviedb.org/tv/{si.TmdbCode}/edit?active_nav_item=seasons";
                }

                return null;

            case TVDoc.ProviderType.TVmaze:
                if (si.TVmazeCode > 0 && s.SeasonId > 0)
                {
                    return $"https://www.tvmaze.com/season/update?id={s.SeasonId}";
                }

                return null;

            case TVDoc.ProviderType.libraryDefault:
            default:
                throw new NotSupportedException($"si.Provider = {si.Provider} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()?.ToString()}");
        }
    }

    private static string? EditTvEpisodeUrl(ProcessedEpisode ep)
    {
        switch (ep.Show.Provider)
        {
            case TVDoc.ProviderType.TheTVDB:
                if (ep.Show.CachedShow?.Slug.HasValue() ?? false)
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
    private static string? EditTvSeriesUrl(CachedSeriesInfo si)
    {
        return EditTvSeriesUrl(si, si.Source);
    }

    private static string? EditTvSeriesUrl(CachedSeriesInfo si,TVDoc.ProviderType source)
    {
        switch (source)
        {
            case TVDoc.ProviderType.TheTVDB:

                if (si.Slug.HasValue())
                {
                    return $"https://thetvdb.com/series/{si.Slug}/edit";
                }

                return null;

            case TVDoc.ProviderType.TMDB:
                if (si.TmdbCode > 0)
                {
                    return $"https://www.themoviedb.org/tv/{si.TmdbCode}/edit?active_nav_item=primary_facts";
                }

                return null;

            case TVDoc.ProviderType.TVmaze:
                if (si.TvMazeCode > 0)
                {
                    return $" https://www.tvmaze.com/show/update?id={si.TvMazeCode}";
                }

                return null;

            case TVDoc.ProviderType.libraryDefault:
                return EditTvSeriesUrl(si, TVSettings.Instance.DefaultProvider);
            default:
                throw new ArgumentOutOfRangeException(nameof(source),$"TV Url asked tobe created for {source.PrettyPrint()}");
        }
    }

    public static string YearRange(CachedSeriesInfo? ser)
    {
        int? minYear = ser?.MinYear;
        int? maxYear = ser?.MaxYear;

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

    private static string ParseAirsTime(CachedSeriesInfo ser)
    {
        return ser.AirsTime?.ToString("h tt") ?? string.Empty;
    }

    private static string GetBestFolderLocationToOpen(this ShowConfiguration si)
    {
        if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && Directory.Exists(si.AutoAddFolderBase))
        {
            return si.AutoAddFolderBase;
        }

        return si.AllExistngFolderLocations()
            .Values
            .SelectMany(a => a)
            .Where(folder => folder.HasValue())
            .FirstOrDefault(Directory.Exists) ?? string.Empty;
    }

    private static string CreateHorizontalBannerHtml(ShowConfiguration? series)
    {
        string? url = series?.WideBannerUrl();
        if (url.HasValue())
        {
            return $"<img class=\"rounded\" src=\"{url}\"><br/>&nbsp;";
        }

        return string.Empty;
    }

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static string CreateHorizontalBannerHtml(this ProcessedSeason s)
    {
        string url = s.WideBannerUrl();
        if (url.HasValue())
        {
            return $"<img class=\"rounded w-100\" src=\"{TheTVDB.API.GetImageURL(s.GetWideBannerPath())}\"><br/>";
        }

        return string.Empty;
    }

    public static string PosterUrl(this ShowConfiguration series)
    {
        string? url = series.CachedShow?.GetSeriesPosterPath();

        if (url.HasValue() && !url.IsWebLink() && series.Provider == TVDoc.ProviderType.TheTVDB && TheTVDB.API.GetImageURL(url).HasValue())
        {
            return TheTVDB.API.GetImageURL(url);
        }
        return url ?? string.Empty;
    }

    public static string? ThumbnailUrl(this ProcessedEpisode episode)
    {
        string? url = episode.Filename;

        if (url.HasValue() && !url.IsWebLink() && episode.Show.Provider == TVDoc.ProviderType.TheTVDB && TheTVDB.API.GetImageURL(url).HasValue())
        {
            return TheTVDB.API.GetImageURL(url);
        }
        if (url.HasValue() && url.IsWebLink())
        {
            return url;
        }
        return episode.Filename;
    }

    public static string CreatePosterHtml(CachedMovieInfo ser)
    {
        string? url = ser.PosterUrl;
        if (url.HasValue() && !url.IsWebLink() && TheTVDB.API.GetImageURL(url).HasValue())
        {
            url = TheTVDB.API.GetImageURL(url);
        }
        if (url.HasValue() && url.IsWebLink())
        {
            return $"<img class=\"show-poster rounded w-100\" src=\"{url}\" alt=\"{ser.Name} Movie Poster\">";
        }

        return string.Empty;
    }

    private static string CreatePosterHtml(CachedSeriesInfo ser)
    {
        string? url = ser.GetSeriesPosterPath();
        if (url.HasValue() && !url.IsWebLink() && TheTVDB.API.GetImageURL(url).HasValue())
        {
            url = TheTVDB.API.GetImageURL(url);
        }
        if (url.HasValue() && url.IsWebLink())
        {
            return $"<img class=\"show-poster rounded w-100\" src=\"{url}\" alt=\"{ser.Name} Show Poster\">";
        }

        return string.Empty;
    }

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

        if (string.IsNullOrEmpty(ei.ThumbnailUrl()))
        {
            return string.Empty;
        }

        string? url = ei.ThumbnailUrl();

        if (url.HasValue())
        {
            return $"<img class=\"rounded w-100\" src=\"{url}\" alt=\"{ei.Name} Screenshot\">";
        }
        return string.Empty;
    }

    public static string GetSeasonHtmlOverview(this ShowConfiguration si, ProcessedSeason s, bool includeDirectoryLinks)
    {
        StringBuilder sb = new();
        DirFilesCache dfc = new();
        Color col = Color.FromName("ButtonFace");
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendSeason(s, si, col, includeDirectoryLinks);

        if (si.SeasonEpisodes.TryGetValue(s.SeasonNumber, out List<ProcessedEpisode>? siSeasonEpisode))
        {
            foreach (ProcessedEpisode ep in siSeasonEpisode)
            {
                List<FileInfo>? fl = includeDirectoryLinks ? dfc.FindEpOnDisk(ep) : null;
                sb.AppendEpisode(ep, fl, col);
            }
        }

        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    public static string GetSeasonSummaryHtmlOverview(this ShowConfiguration si, ProcessedSeason s, bool includeDirectoryLinks)
    {
        StringBuilder sb = new();
        Color col = Color.FromName("ButtonFace");
        sb.AppendLine(HTMLHeader(10, col));
        sb.AppendSeasonSummary(si, s, col, includeDirectoryLinks);
        sb.AppendLine(HTMLFooter());
        return sb.ToString();
    }

    private static string SeasonSummaryTableRow(ProcessedEpisode ep, bool includeDirectoryLinks, DirFilesCache dfc)
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
                viewButton += CreateWatchButton(urlFilename);
            }
        }

        string airedText = ep.HasAired() ? " (Aired)" : string.Empty;
        DateTime? airDateDt = ep.GetAirDateDt(true);
        string airDateString = airDateDt.HasValue && airDateDt > DateTime.MinValue
            ? airDateDt.Value.ToString("d", DateTimeFormatInfo.CurrentInfo)
            : string.Empty;
        return
            $"<tr><th scope=\"row\">{ep.AppropriateEpNum}</th><td>{airDateString}{airedText}</td><td>{ep.Name}</td><td>{status}</td><td class=\"text-right\">{searchButton}{viewButton}</td></tr>";
    }

    private static string GetEpisodeStatus(ProcessedEpisode ep, bool includeDirectoryLinks, List<FileInfo>? fl)
    {
        bool filesExist = fl is { Count: > 0 };

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
        DirFilesCache dfc = new();
        if (si is null)
        {
            return;
        }

        string tableRows = si.SeasonEpisodes[s.SeasonNumber].ToList().Select(episode => SeasonSummaryTableRow(episode, includeDirectoryLinks, dfc)).Concat();

        string seasonHeaderDiv = CreateSeasonHeaderDiv(si, s, includeDirectoryLinks);
        string table = CreateEpisodeTableHeader(tableRows);

        sb.AppendLine($@"<div class=""card card-body"" style=""background-color:{backgroundColour.HexColour()}"">{seasonHeaderDiv}
                {table}
                </div>");
    }

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

    private static string CreateSeasonHeaderDiv(ShowConfiguration si, ProcessedSeason s, bool includeDirectoryLinks)
    {
        string explorerButton = string.Empty;
        if (includeDirectoryLinks)
        {
            string urlFilename = Uri.EscapeDataString(si.GetBestFolderLocationToOpen(s));
            explorerButton = CreateExploreButton(urlFilename);
        }

        string? tvdbSLug = si.CachedShow?.Slug;
        string tvdbLink = !tvdbSLug.HasValue() ? string.Empty : TheTVDB.API.WebsiteSeasonUrl(s);
        string tvdbButton = CreateButton(tvdbLink, "TVDB.com", "View on TVDB");
        string tvMazeButton = CreateButton(s.Show.Provider != TVDoc.ProviderType.TVmaze ? string.Empty : s.WebsiteUrl, "TVmaze.com", "View on TV Maze");

        string episodeText = !s.Episodes.IsEmpty ? $"<br/><small class=\"text-muted\">{s.Episodes.Count} Episodes</small>" : string.Empty;

        Season? season = si.CachedShow?.Season(s.SeasonNumber);
        string seasonOverViewHtml = HasCustomNameOrDescription(season)
            ? $"<h3>{season?.SeasonName}</h3><p>{season?.SeasonDescription}</p>"
            : string.Empty;

        return $@"<div class=""row"">
                    <div class=""col-8"">
                        <h1>{si.ShowName} - {SeasonName(si, s.SeasonNumber)}</h1>
                        {seasonOverViewHtml}
                    </div>
                    <div class=""col-4 text-right"">
                        {CreateButton(EditSeasonUrl(si, s), "<i class=\"far fa-edit\"></i>", "Edit")}
                        {explorerButton}
                        {tvdbButton}
                        {tvMazeButton}
                        {episodeText}
                    </div>
                </div>";
    }

    private static void AppendSeason(this StringBuilder sb, ProcessedSeason s, ShowConfiguration? si, Color backgroundColour, bool includeDirectoryLinks)
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

    private static string GetBestFolderLocationToOpen(this ShowConfiguration si, ProcessedSeason s)
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

    private static void AppendEpisode(this StringBuilder sb, ProcessedEpisode ep, IReadOnlyCollection<FileInfo>? fl, Color backgroundColour)
    {
        string stars = StarRating(ep.EpisodeRating);
        string tvdbEpisodeUrl = ep.Show.Provider == TVDoc.ProviderType.TheTVDB ? ep.TVDBWebsiteUrl : string.Empty;
        bool ratingIsNumber = float.TryParse(ep.EpisodeRating, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out float rating);
        string siteRating = ratingIsNumber && rating > 0
            ? rating + "/10" + AddRatingCount(ep.SiteRatingCount ?? 0)
            : "";

        string imdbLink = ep.ImdbCode.ToImdbLink();
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

        string searchButton = (fl is null || fl.Count == 0) && ep.HasAired()
            ? CreateButton(TVSettings.Instance.BTSearchURL(ep), "<i class=\"fas fa-search\"></i>", "Search for Torrent...")
            : string.Empty;

        string viewButton = string.Empty;
        string explorerButton = string.Empty;
        if (fl != null)
        {
            foreach (string urlFilename in fl.Select(fi => Uri.EscapeDataString(fi.FullName)))
            {
                viewButton += CreateWatchButton(urlFilename);
                explorerButton += CreateExploreButton(urlFilename);
            }
        }

        string tvdbButton = CreateButton(tvdbEpisodeUrl, "TVDB.com", "View on TVDB");
        string tvMazeButton = CreateButton(ep.Show.Provider == TVDoc.ProviderType.TVmaze ? ep.LinkUrl : null, "TVmaze.com", "View on TV maze");
        string imdbButton = CreateButton(imdbLink, "IMDB.com", "View on IMDB");
        string tmdbButton = ep.Show.Provider == TVDoc.ProviderType.TMDB ? CreateButton(ep.WebsiteUrl, "TMDB.com", "View on TMDB.com"):string.Empty;

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
                    {tmdbButton}
                    {tvMazeButton}
                    {imdbButton}
                   </div>
		           <div class=""row align-items-bottom flex-grow-1"">
                    <div class=""col-md-6 align-self-end"">{stars}<br>{siteRating}</div>
                    <div class=""col-md-6 align-self-end text-right"">{productionCode}</div>
                   </div>
                  </div>
                 </div>
                </div>");
    }

    public static string AddRatingCount(int siteRatingCount)
    {
        return siteRatingCount > 0 ? $" (From {siteRatingCount} Vote{(siteRatingCount == 1 ? "" : "s")})" : string.Empty;
    }

    private static string CreateButton(string? link, string text, string tooltip)
    {
        if (string.IsNullOrWhiteSpace(link))
        {
            return string.Empty;
        }

        return $"<a href=\"{link}\" class=\"btn btn-outline-secondary\" role=\"button\" aria-disabled=\"true\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"{tooltip}\">{text}</a>";
    }

    private static string CreateExploreButton(string? urlFilename)
    {
        if (string.IsNullOrWhiteSpace(urlFilename))
        {
            return string.Empty;
        }

        return CreateButton($"{UI.EXPLORE_PROXY}{urlFilename}",
            "<i class=\"far fa-folder-open\"></i>", "Open Containing Folder");
    }

    private static string CreateWatchButton(string? urlFilename)
    {
        if (string.IsNullOrWhiteSpace(urlFilename))
        {
            return string.Empty;
        }

        return CreateButton($"{UI.WATCH_PROXY}{urlFilename}", "<i class=\"far fa-eye\"></i>", "Watch Now");
    }

    private static string DateDetailsHtml(this ProcessedEpisode ei)
    {
        try
        {
            DateTime? dt = ei.GetAirDateDt(true);
            if (dt != null && dt.Value.CompareTo(DateTime.MaxValue) != 0)
            {
                return $"<h6>{dt.Value.ToShortDateString()}</h6><small class=\"text-muted\">({ei.HowLong()})</small>";
            }
        }
        catch (Exception e)
        {
            Logger.Error(e,$"Could not display date from {ei.Show.Name} S{ei.AppropriateSeasonNumber}E{ei.AppropriateEpNum}");
        }
        return string.Empty;
    }

    public static string GetShowImagesHtmlOverview(this ShowConfiguration si)
    {
        string body =
            $"<h1><A HREF=\"{si.ProviderShowUrl()}\">{si.ShowName}</A> </h1>";

        CachedSeriesInfo? ser = si.CachedShow;
        if (ser is null)
        {
            return body;
        }

        body += ImageSection("Show Banner", 758, 140, ser.GetSeriesWideBannerPath(), si.Provider);
        body += ImageSection("Show Poster", 350, 500, ser.GetSeriesPosterPath(), si.Provider);
        body += ImageSection("Show Fanart", 960, 540, ser.GetSeriesFanartPath(), si.Provider);
        return body;
    }

    private static string ImageSection(string title, int width, int height, string? bannerPath, TVDoc.ProviderType p)
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

    private static string HiddenOverview(this ProcessedEpisode ei)
    {
        if (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired")
        {
            return Resources.Spoilers_Hidden_Text;
        }

        if (ei.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
        {
            return string.Join("<p/><hr/><p class=\"lead\" />", ei.SourceEpisodes.Select(e => e.Overview));
        }

        return ei.Overview ?? string.Empty;
    }

    public static string GetSeasonImagesHtmlOverview(this ShowConfiguration si, ProcessedSeason s)
    {
        int snum = s.SeasonNumber;
        string body = $"<h1>{si.ShowName} - {ProcessedSeason.UIFullSeasonWord(snum)}</h1>";

        if (TVSettings.Instance.NeedToDownloadBannerFile())
        {
            body += ImageSection("Series Banner", 758, 140, si.CachedShow?.GetSeasonWideBannerPath(snum), si.Provider);
            body += ImageSection("Series Poster", 350, 500, si.CachedShow?.GetSeasonBannerPath(snum), si.Provider);
        }
        else
        {
            body +=
                "<h2>Images are not being downloaded for this cachedSeries. Please see Options -> Preferences -> Media Center to reconfigure.</h2>";
        }

        return body;
    }

    public static string GetMovieImagesHtmlOverview(this MovieConfiguration si)
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

    public static string SeasonName(ShowConfiguration si, int snum)
    {
        return si.Order switch
        {
            ProcessedSeason.SeasonType.dvd => snum == 0
                ? "Not Available on DVD"
                : "DVD " + ProcessedSeason.UISeasonWord(snum),
            ProcessedSeason.SeasonType.aired => ProcessedSeason.UIFullSeasonWord(snum),
            ProcessedSeason.SeasonType.alternate => snum == 0
                ? "Not in alternate season"
                : "Alternate " + ProcessedSeason.UISeasonWord(snum),
            _ => throw new ArgumentOutOfRangeException($"ShowConfig {si} has invalid Season Order {si.Order.PrettyPrint()}.")
        };
    }

    internal static string GenreIconHtml(string genre)
    {
        string[] availableIcons =
        {
            "Action", "Adventure", "Animation", "Children", "Comedy", "Crime", "Documentary", "Drama", "Family",
            "Fantasy", "Food", "Horror", "Mini-Series", "Mystery", "Reality", "Romance", "Science-Fiction", "Soap",
            "Talk Show", "Thriller", "Travel", "War", "Western"
        };

        const string ROOT = "https://www.tvrename.com/assets/images/GenreIcons/";

        return availableIcons.Contains(genre)
            ? $@"<img width=""30"" height=""30"" src=""{ROOT}{genre}.svg"" alt=""{genre}"">"
            : "";
    }

    private static string ActorLinkHtml(this Actor actor)
    {
        string asText = actor.AsSelf() ? string.Empty
            : string.IsNullOrWhiteSpace(actor.ActorRole) ? string.Empty
            : " as " + actor.ActorRole;
        string tryText =
            $@"<a href=""{actor.ActorName.ToImdbActorLink()}"">{actor.ActorName}</a>{asText}";
        return tryText;
    }

    public static string ToImdbActorLink(this string name)
    {
        return $@"https://www.imdb.com/find?s=nm&q={name}";
    }

    private static string StarRating(string? rating)
    {
        try
        {
            if (!rating.HasValue())
            {
                return StarRating(0);
            }
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
        const string HALF_STAR = @"<i class=""fas fa-star-half""></i>";

        if (f < .25)
        {
            return string.Empty;
        }

        if (f <= .75)
        {
            return HALF_STAR;
        }

        if (f > 1)
        {
            return STAR + StarRating(f - 1);
        }

        return STAR;
    }

    // ReSharper disable once InconsistentNaming
    internal static string HTMLHeader(int size, Color backgroundColour)
    {
        return @"<!DOCTYPE html>
                <html><head>
                <meta charset=""utf-8"">
                <meta name = ""viewport"" content = ""width=device-width, initial-scale=1.0"" >
                <title> TV Rename - Show Summary</title>
                 <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC"" crossorigin=""anonymous"">
                <link rel=""stylesheet"" href=""https://pro.fontawesome.com/releases/v5.10.0/css/all.css"" integrity=""sha384-AYmEC3Yw5cVb3ZcuHtOA93w35dYTsvhLPVnYs9eStHfGJvOvKxVfELGroGkvsg+p"" crossorigin=""anonymous""/>
                </head>"
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
                 <script src=""https://code.jquery.com/jquery-3.6.0.min.js"" integrity=""sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4="" crossorigin=""anonymous""></script>
				<script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js"" integrity=""sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM"" crossorigin=""anonymous""></script>
                    <script>$(document).ready(function(){ })</script>
                     </body>
                </html>
                ";
    }

    public static string GetShowHtmlOverviewOffline(this ShowConfiguration si)
    {
        string body = string.Empty;
        CachedSeriesInfo? ser = si.CachedShow;

        string bannerUrl = si.WideBannerUrl();
        if (bannerUrl.HasValue())
        {
            body += $"<img width=758 height=140 src=\"{bannerUrl}\"><br/>";
        }

        body += $"<h1><A HREF=\"{si.ProviderShowUrl()}\">{si.ShowName}</A> </h1>";

        body += "<h2>Overview</h2>" + ser?.Overview; //get overview in either format

        bool first = true;
        foreach (Actor aa in si.Actors.Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
        {
            body += first ? "<h2>Actors</h2>" : ", ";

            body += aa.ActorLinkHtml();
            first = false;
        }

        if (ser != null)
        {
            body += $"<h2>Airs</h2>{ser.AirsDay} {ParseAirsTime(ser)}";
            string? net = ser.Network;
            if (!string.IsNullOrEmpty(net))
            {
                body += ", " + ser.Networks.ToCsv();
            }
        }

        string yearRange = YearRange(ser);
        string siteRating = PrettyPrint(ser?.SiteRating);
        string tvdbLink = si.TvdbCode > 0 ? TheTVDB.API.WebsiteShowUrl(si) : string.Empty;

        string tableHtml = string.Empty;

        tableHtml += GetOverviewLinkPart("thetvdb.com", tvdbLink);
        tableHtml += GetOverviewLinkPart("imdb.com", ser?.Imdb.ToImdbLink());
        tableHtml += GetOverviewPart("Runtime", ser?.Runtime);
        tableHtml += GetOverviewPart("Aliases", si.AliasNames.ToCsv());
        tableHtml += GetOverviewPart("Genres", si.Genres.ToCsv());
        tableHtml += GetOverviewPart("Rating", ser?.ContentRating);
        tableHtml += GetOverviewPart("User Rating", $"{siteRating}{AddRatingCount(ser?.SiteRatingVotes ?? 0)}");
        tableHtml += GetOverviewPart("Active From", yearRange);
        tableHtml += GetOverviewPart("Status", ser?.Status);

        if (tableHtml.HasValue())
        {
            body += "<h2>Information<table border=0>" + tableHtml + "</table>";
        }

        return body;
    }

    private static string WideBannerUrl(this ShowConfiguration series)
    {
        string? url = series.CachedShow?.GetSeriesWideBannerPath();

        if (url.HasValue() && !url.IsWebLink() && series.Provider == TVDoc.ProviderType.TheTVDB && TheTVDB.API.GetImageURL(url).HasValue())
        {
            return TheTVDB.API.GetImageURL(url);
        }
        return url ?? string.Empty;
    }

    private static string WideBannerUrl(this ProcessedSeason season)
    {
        string? url = season.GetWideBannerPath();

        if (url.HasValue() && !url.IsWebLink() && season.Show.Provider == TVDoc.ProviderType.TheTVDB && TheTVDB.API.GetImageURL(url).HasValue())
        {
            return TheTVDB.API.GetImageURL(url);
        }
        return url ?? string.Empty;
    }

    public static string GetSeasonHtmlOverviewOffline(this ShowConfiguration si, ProcessedSeason s)
    {
        int snum = s.SeasonNumber;
        string body = string.Empty;

        string widePosterUrl = si.WideBannerUrl();
        if (widePosterUrl.HasValue())
        {
            body += $"<img width=758 height=140 src=\"{widePosterUrl}\"><br/>";
        }

        List<ProcessedEpisode> eis = si.SeasonEpisodes[snum];

        string seasText = SeasonName(si, snum);
        string? seasonUrl = s.WebsiteUrl;
        if (eis.Any() && eis[0].SeasonId > 0 && seasonUrl.HasValue())
        {
            seasText = $" - <A HREF=\"{seasonUrl}\">{seasText}</a>";
        }
        else
        {
            seasText = " - " + seasText;
        }

        body += "<h1><A HREF=\"" + si.ProviderShowUrl() + "\">" + si.ShowName +
                "</A>" + seasText + "</h1>";

        DirFilesCache dfc = new();
        foreach (ProcessedEpisode ei in eis)
        {
            string epl = ei.EpNumsAsString();

            string episodeUrl = ei.ProviderWebUrl();

            body += "<A href=\"" + episodeUrl + "\" name=\"ep" + epl + "\">"; // anchor
            body += "<b>" + EpisodeName(si, snum, ei) + "</b>";
            body += "</A>"; // anchor
            if (si.UseSequentialMatch && ei.OverallNumber != -1)
            {
                body += " (#" + ei.OverallNumber + ")";
            }

            List<FileInfo> fl = dfc.FindEpOnDisk(ei);
            if (fl.Any())
            {
                foreach (FileInfo fi in fl)
                {
                    string urlFilename = System.Web.HttpUtility.UrlEncode(fi.FullName);
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
                if (ei.ThumbnailUrl().HasValue())
                {
                    body += "<img src=" + ei.ThumbnailUrl() + ">";
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

    public static string ProviderWebUrl(this ProcessedEpisode ei)
    {
        return (ei.Show.Provider switch
        {
            TVDoc.ProviderType.TheTVDB => TheTVDB.API.WebsiteEpisodeUrl(ei),
            TVDoc.ProviderType.TMDB => ei.LinkUrl.HasValue() ? ei.LinkUrl! :
                $"https://www.themoviedb.org/tv/{ei.TheCachedSeries.TmdbId}/season/{ei.AppropriateSeasonNumber}/episode/{ei.AppropriateEpNum}",
            TVDoc.ProviderType.TVmaze => ei.LinkUrl.HasValue() ? ei.LinkUrl! : $"https://www.tvmaze.com/episodes/{ei.EpisodeId}",
            _ => string.Empty
        });
    }

    private static string ProviderShowUrl(this ShowConfiguration show)
    {
        return show.Provider switch
        {
            TVDoc.ProviderType.TheTVDB => TheTVDB.API.WebsiteShowUrl(show),
            TVDoc.ProviderType.TMDB => show.WebsiteUrl ?? string.Empty,
            TVDoc.ProviderType.libraryDefault => show.WebsiteUrl ?? string.Empty,
            TVDoc.ProviderType.TVmaze => show.WebsiteUrl ?? string.Empty,
            _ => throw new ArgumentOutOfRangeException($"ShowConfig {show} has invalid Provider.")
        };
    }

    public static string GetMovieHtmlOverviewOffline(this MovieConfiguration si)
    {
        string body = string.Empty;
        CachedMovieInfo? ser = si.CachedMovie;

        body += $"<h1><A HREF=\"{ser?.OfficialUrl}\">{si.ShowName}</A> </h1>";

        body += "<h2>Overview</h2>" + ser?.Overview; //get overview in either format

        bool first = true;
        foreach (Actor aa in si.Actors.Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
        {
            body += first ? "<h2>Actors</h2>" : ", ";

            body += aa.ActorLinkHtml();
            first = false;
        }

        string siteRating = PrettyPrint(ser?.SiteRating);
        string tvdbLink = si.TvdbCode > 0 ? TheTVDB.API.WebsiteShowUrl(si.TvdbCode) : string.Empty;
        string tmdbLink = si.TmdbCode > 0 ? TMDB.API.WebsiteMovieUrl(si.TmdbCode)   : string.Empty;
        string? mazeLink = ser?.TvMazeCode > 0 ? ser.WebUrl : string.Empty;
        string facebookButton = ser?.FacebookId.HasValue() ?? false ? $"https://facebook.com/{ser.FacebookId}" : string.Empty;
        string instagramButton = ser?.InstagramId.HasValue() ?? false ? $"https://instagram.com/{ser.InstagramId}" : string.Empty;
        string twitterButton = ser?.TwitterId.HasValue() ?? false ? $"https://twitter.com/{ser.TwitterId}" : string.Empty;

        string tableHtml = string.Empty;

        tableHtml += GetOverviewLinkPart("thetvdb.com", tvdbLink);
        tableHtml += GetOverviewLinkPart("imdb.com", ser?.Imdb.ToImdbLink());
        tableHtml += GetOverviewLinkPart("MovieDB", tmdbLink);
        tableHtml += GetOverviewLinkPart("TV Maze", mazeLink);
        tableHtml += GetOverviewLinkPart("Facebook", facebookButton);
        tableHtml += GetOverviewLinkPart("Instagram", instagramButton);
        tableHtml += GetOverviewLinkPart("Twitter", twitterButton);
        tableHtml += GetOverviewPart("Runtime", ser?.Runtime);
        tableHtml += GetOverviewPart("Aliases", si.AliasNames.ToCsv());
        tableHtml += GetOverviewPart("Genres", si.Genres.ToCsv());
        tableHtml += GetOverviewPart("Rating", ser?.ContentRating);
        tableHtml += GetOverviewPart("Network", ser?.Networks.ToCsv());
        tableHtml += GetOverviewPart("Content Rating", ser?.ContentRating);
        tableHtml += GetOverviewPart("User Rating", $"{siteRating}{AddRatingCount(ser?.SiteRatingVotes ?? 0)}");
        tableHtml += GetOverviewPart("Released", ser?.Year?.ToString());
        tableHtml += GetOverviewPart("Tagline", ser?.TagLine);
        tableHtml += GetOverviewPart("Country", ser?.Country);
        tableHtml += GetOverviewPart("Status", ser?.Status);
        tableHtml += GetOverviewPart("Type", ser?.MovieType);

        if (!string.IsNullOrWhiteSpace(tableHtml))
        {
            body += "<h2>Information<table border=0>" + tableHtml + "</table>";
        }

        return body;
    }

    private static string EpisodeName(ShowConfiguration si, int snum, ProcessedEpisode ei)
    {
        if (si.Order == ProcessedSeason.SeasonType.dvd && snum == 0)
        {
            return ei.Name;
        }
        return System.Web.HttpUtility.HtmlEncode(CustomEpisodeName.NameForNoExt(ei, CustomEpisodeName.OldNStyle(6)));
    }

    private static string GetOverview(ProcessedEpisode ei)
    {
        string overviewString = string.Empty;

        overviewString += GetOverviewLinkPart("imdb.com", ei.ImdbCode);
        overviewString += GetOverviewLinkPart("Link", ei.ShowUrl);
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

    private static string GetOverviewPart(string name, string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : "<tr><td width=120px>" + name + "</td><td>" + value + "</td></tr>";
    }
    private static string GetOverviewLinkPart(string name, string? link)
    {
        return string.IsNullOrWhiteSpace(link) ? string.Empty : GetOverviewPart(name, $"<A HREF=\"{link}\">Visit</a>");
    }

    public static string? YoutubeTrailer(CachedMediaInfo? si)
    {
        return si?.TrailerUrl?.Replace("/watch?v=", "/embed/");
    }
}
