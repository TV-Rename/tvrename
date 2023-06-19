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

namespace TVRename;

internal class ShowsHtml : ShowsExporter
{
    public ShowsHtml(List<ShowConfiguration> shows) : base(shows)
    {
    }

    public override bool Active() => TVSettings.Instance.ExportShowsHTML;

    protected override string Location() => TVSettings.Instance.ExportShowsHTMLTo;

    protected override void Do()
    {
        using System.IO.StreamWriter file = new(Location());
        file.WriteLine(ShowHtmlHelper.HTMLHeader(8, Color.White));
        foreach (ShowConfiguration si in Shows)
        {
            try
            {
                file.WriteLine(CreateHtml(si));
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex,
                    $"Skipped adding {si.ShowName} to the output HTML as it is missing some data. Please try checking the settings and doing a force refresh on the show.");
            }
        }

        file.WriteLine(ShowHtmlHelper.HTMLFooter());
    }

    private static string CreateHtml(ShowConfiguration si)
    {
        CachedSeriesInfo? cachedSeries = si.CachedShow;
        if (cachedSeries is null)
        {
            return string.Empty;
        }

        string posterUrl = si.PosterUrl();
        string yearRange = ShowHtmlHelper.YearRange(cachedSeries);
        string episodeSummary = cachedSeries.Episodes.Count.ToString();
        string stars = ShowHtmlHelper.StarRating(cachedSeries.SiteRating / 2);
        string genreIcons = string.Join("&nbsp;", cachedSeries.Genres.Select(ShowHtmlHelper.GenreIconHtml));
        string siteRating = cachedSeries.SiteRating > 0 ? cachedSeries.SiteRating + "/10" : "";

        return $@"<div class=""card card-body"">
            <div class=""row"">
            <div class=""col-md-4"">
                <img class=""show-poster rounded w-100"" src=""{posterUrl}"" alt=""{si.ShowName} Show Poster""></div>
            <div class=""col-md-8 d-flex flex-column"">
                <div class=""row"">
                    <div class=""col-md-8""><h1>{si.ShowName}</h1></div>
                    <div class=""col-md-4 text-right""><h6>{yearRange} ({cachedSeries.Status})</h6><small class=""text-muted"">{episodeSummary} Episodes</small></div>
                </div>
            <div><blockquote>{cachedSeries.Overview}</blockquote></div>
            <div><blockquote>{cachedSeries.GetActorNames().ToCsv()}</blockquote></div>
            <div class=""row align-items-bottom flex-grow-1"">
                <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}</div>
                <div class=""col-md-4 align-self-end text-center"">{cachedSeries.ContentRating}<br>{cachedSeries.Networks.ToCsv()}</div>
                <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{cachedSeries.Genres.ToCsv()}</div>
            </div>
            </div></div></div>";
    }

    protected override string Name() => "Show HTML Exporter";
}
