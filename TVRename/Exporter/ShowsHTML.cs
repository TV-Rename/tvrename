using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        internal override void Do()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
            {
                file.WriteLine(ShowHtmlHelper.HTMLHeader(8, Color.White));
                foreach (ShowItem si in Shows)
                {
                    file.WriteLine(CreateHtml(si));
                }

                file.WriteLine(ShowHtmlHelper.HTMLFooter());
            }
        }

        private static string CreateHtml(ShowItem si)
        {
            string posterUrl = TheTVDB.GetImageURL(si.TheSeries().GetImage(TVSettings.FolderJpgIsType.Poster));
            int minYear = si.TheSeries().MinYear;
            int maxYear = si.TheSeries().MaxYear;
            string yearRange = (minYear == maxYear) ? minYear.ToString() : minYear + "-" + maxYear;
            string episodeSummary = si.TheSeries().AiredSeasons.Sum(pair => pair.Value.Episodes.Count).ToString();
            string stars = ShowHtmlHelper.StarRating(si.TheSeries().SiteRating/2);
            string genreIcons = string.Join("&nbsp;", si.TheSeries().Genres().Select(ShowHtmlHelper.GenreIconHtml));
            string siteRating = si.TheSeries().SiteRating > 0 ? si.TheSeries().SiteRating + "/10" : "";

            return $@"<div class=""card card-body"">
            <div class=""row"">
            <div class=""col-md-4"">
                <img class=""show-poster rounded w-100"" src=""{posterUrl}"" alt=""{si.ShowName} Show Poster""></div>
            <div class=""col-md-8 d-flex flex-column"">
                <div class=""row"">
                    <div class=""col-md-8""><h1>{si.ShowName}</h1></div>
                    <div class=""col-md-4 text-right""><h6>{yearRange} ({si.TheSeries().Status})</h6><small class=""text-muted"">{episodeSummary} Episodes</small></div>
                </div>
            <div><blockquote>{si.TheSeries().Overview}</blockquote></div>
            <div><blockquote>{string.Join(", ", si.TheSeries().GetActorNames())}</blockquote></div>
            <div class=""row align-items-bottom flex-grow-1"">
                <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}</div>
                <div class=""col-md-4 align-self-end text-center"">{si.TheSeries().ContentRating}<br>{si.TheSeries().Network}</div>
                <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{string.Join(", ", si.TheSeries().Genres())}</div>
            </div>
            </div></div></div>";
        }
    }
}
