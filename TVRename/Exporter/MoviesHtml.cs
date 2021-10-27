using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TVRename
{
    internal class MoviesHtml : MoviesExporter
    {
        public MoviesHtml(List<MovieConfiguration> shows) : base(shows)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportMoviesHTML;

        protected override string Location() => TVSettings.Instance.ExportMoviesHTMLTo;
        [NotNull]
        protected override string Name() => "Movies HTML Exporter";
        protected override void Do()
        {
            using (System.IO.StreamWriter file = new(Location()))
            {
                file.WriteLine(ShowHtmlHelper.HTMLHeader(8, Color.White));
                foreach (MovieConfiguration si in Shows)
                {
                    try
                    {
                        file.WriteLine(CreateHtml(si));
                    }
                    catch (NullReferenceException ex)
                    {
                        LOGGER.Error(ex,
                            $"Skipped adding {si.ShowName} to the output HTML as it is missing some data. Please try checking the settings and doing a force refresh on the show.");
                    }
                }

                file.WriteLine(ShowHtmlHelper.HTMLFooter());
            }
        }

        [NotNull]
        private static string CreateHtml([NotNull] MovieConfiguration si)
        {
            CachedMovieInfo cachedSeries = si.CachedMovie;
            if (cachedSeries is null)
            {
                return string.Empty;
            }

            string yearRange = cachedSeries.Year?.ToString() ?? "";
            string stars = ShowHtmlHelper.StarRating(cachedSeries.SiteRating / 2);
            string genreIcons = string.Join("&nbsp;", cachedSeries.Genres.Select(ShowHtmlHelper.GenreIconHtml));
            string siteRating = cachedSeries.SiteRating > 0 ? cachedSeries.SiteRating + "/10" : "";

            string poster = ShowHtmlHelper.CreatePosterHtml(cachedSeries);
            string runTimeHtml = string.IsNullOrWhiteSpace(cachedSeries.Runtime) ? string.Empty : $"<br/> {cachedSeries.Runtime} min";

            return $@"<div class=""card card-body"">
            <div class=""row"">
            <div class=""col-md-4"">
                {poster}
</div>
            <div class=""col-md-8 d-flex flex-column"">
                <div class=""row"">
                    <div class=""col-md-8""><h1>{si.ShowName}</h1><small class=""text-muted"">{cachedSeries.TagLine}</small></div>
                    <div class=""col-md-4 text-right""><h6>{yearRange} ({cachedSeries.Status})</h6>
<small class=""text-muted"">{cachedSeries.ShowLanguage} - {cachedSeries.MovieType} - {cachedSeries.Country}</small>
                        <small class=""text-muted"">{runTimeHtml}</small></div>
</div>

            <div><blockquote>{cachedSeries.Overview}</blockquote></div>
            <div><blockquote>{cachedSeries.GetActorNames().ToCsv()}</blockquote></div>
            <div class=""row align-items-bottom flex-grow-1"">
                <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}{ShowHtmlHelper.AddRatingCount(cachedSeries.SiteRatingVotes)}</div>
                <div class=""col-md-4 align-self-end text-center"">{cachedSeries.ContentRating}<br>{cachedSeries.Networks.ToCsv()}</div>
                <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{cachedSeries.Genres.ToCsv()}</div>
            </div>
            </div></div></div>";
        }
    }
}
