// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class ShowsHTML : ShowsExporter
    {
        public ShowsHTML(List<ShowItem> shows) : base(shows)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportShowsHTML;
        protected override string Location() => TVSettings.Instance.ExportShowsHTMLTo;

        protected override void Do()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
            {
                file.WriteLine(ShowHtmlHelper.HTMLHeader(8, Color.White));
                foreach (ShowItem si in Shows)
                {
                    try
                    {
                        file.WriteLine(CreateHtml(si));
                    }
                    catch (Exception ex)
                    {
                        LOGGER.Error(ex,
                            $"Skipped adding {si.ShowName} to the outpur HTML as it is missing some data. Please try checking the settings and doing a force refresh on the show.");
                    }
                }

                file.WriteLine(ShowHtmlHelper.HTMLFooter());
            }
        }

        [NotNull]
        private static string CreateHtml([NotNull] ShowItem si)
        {
            SeriesInfo series = si.TheSeries();
            if (series is null)
            {
                return string.Empty;
            }

            string posterUrl = TheTVDBAPI.GetImageURL(series.GetImage(TVSettings.FolderJpgIsType.Poster));
            string yearRange = ShowHtmlHelper.YearRange(series);
            string episodeSummary = series.Episodes.Count.ToString();
            string stars = ShowHtmlHelper.StarRating(series.SiteRating/2);
            string genreIcons = string.Join("&nbsp;", series.Genres().Select(ShowHtmlHelper.GenreIconHtml));
            string siteRating = series.SiteRating > 0 ? series.SiteRating + "/10" : "";

            return $@"<div class=""card card-body"">
            <div class=""row"">
            <div class=""col-md-4"">
                <img class=""show-poster rounded w-100"" src=""{posterUrl}"" alt=""{si.ShowName} Show Poster""></div>
            <div class=""col-md-8 d-flex flex-column"">
                <div class=""row"">
                    <div class=""col-md-8""><h1>{si.ShowName}</h1></div>
                    <div class=""col-md-4 text-right""><h6>{yearRange} ({series.Status})</h6><small class=""text-muted"">{episodeSummary} Episodes</small></div>
                </div>
            <div><blockquote>{series.Overview}</blockquote></div>
            <div><blockquote>{string.Join(", ", series.GetActorNames())}</blockquote></div>
            <div class=""row align-items-bottom flex-grow-1"">
                <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}</div>
                <div class=""col-md-4 align-self-end text-center"">{series.ContentRating}<br>{series.Network}</div>
                <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{string.Join(", ", series.Genres())}</div>
            </div>
            </div></div></div>";
        }
    }
}
