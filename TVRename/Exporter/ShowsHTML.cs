using System;
using System.Collections.Generic;
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

        public override void Run()
        {
            if (!Active()) return;

            try
            {

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
                {
                    file.WriteLine(HTMLHeader());
                    foreach (ShowItem si in Shows)
                    {
                        file.WriteLine(CreateHTML(si));
                    }
                    file.WriteLine(HTMLFooter());

                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static string CreateHTML(ShowItem si)
        {
            string posterURL = TheTVDB.GetImageURL(si.TheSeries().GetImage(TVSettings.FolderJpgIsType.Poster));
            int minYear = si.TheSeries().MinYear(); 
            int maxYear = si.TheSeries().MaxYear();
            string yearRange = (minYear==maxYear) ? minYear.ToString() : minYear + "-" + maxYear;
            string episodeSummary = si.TheSeries().AiredSeasons.Sum(pair => pair.Value.Episodes.Count).ToString();
            string stars = StarRating(si.TheSeries().GetSiteRating());
            string genreIcons = string.Join("&nbsp;",si.TheSeries().GetGenres().Select(GenreIconHTML));
            bool ratingIsNumber = float.TryParse(si.TheSeries().GetSiteRating(), out float rating);
            string siteRating = ratingIsNumber && rating>0? rating+"/10":"";

            return $@"<div class=""card card-body"">
            <div class=""row"">
            <div class=""col-md-4"">
                <img class=""show-poster rounded w-100"" src=""{posterURL}"" alt=""{si.ShowName} Show Poster""></div>
            <div class=""col-md-8 d-flex flex-column"">
                <div class=""row"">
                    <div class=""col-md-8""><h1>{si.ShowName}</h1></div>
                    <div class=""col-md-4 text-right""><h6>{yearRange} ({si.TheSeries().getStatus()})</h6><small class=""text-muted"">{episodeSummary } Episodes</small></div>
                </div>
            <div><blockquote>{si.TheSeries().GetOverview()}</blockquote></div>
            <div><blockquote>{string.Join(", ",si.TheSeries().GetActors())}</blockquote></div>
            <div class=""row align-items-bottom flex-grow-1"">
                <div class=""col-md-4 align-self-end"">{stars}<br>{siteRating}</div>
                <div class=""col-md-4 align-self-end text-center"">{si.TheSeries().GetContentRating()}<br>{si.TheSeries().getNetwork()}</div>
                <div class=""col-md-4 align-self-end text-right"">{genreIcons}<br>{string.Join(", ",si.TheSeries().GetGenres()) }</div>
            </div>
            </div></div></div>";
        }

        private static string GenreIconHTML(string genre)
        {
            string[] availbleIcons = {"Action","Adventure","Animation","Children","Comedy","Crime","Documentary","Drama","Family","Fantasy","Food","Horror","Mini-Series","Mystery","Reality","Romance","Science-Fiction","Soap","Talk Show","Thriller","Travel","War","Western"};
            const string root = "https://www.tvrename.com/assets/images/GenreIcons/";

            return availbleIcons.Contains(genre)
                ? $@"<img width=""30"" height=""30"" src=""{root}{genre}.svg"" alt=""{genre}"">"
                : "";
        }

        

        private static string StarRating(string rating)
        {
            return StarRating(float.TryParse(rating, out float f) ? f/2 :3);
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
        private static string HTMLHeader()
        {
            return @"<!DOCTYPE html>
                <html><head>
                <meta charset=""utf-8"">
                <meta name = ""viewport"" content = ""width=device-width, initial-scale=1.0"" >
                <title> TV Rename - Show Summary</title>
                <link rel = ""stylesheet"" href = ""http://maxcdn.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css"" />
                <link rel = ""stylesheet"" href = ""https://use.fontawesome.com/releases/v5.0.13/css/all.css"" />
                </head >
                <body >
                <div class=""col-sm-8 offset-sm-2"">";
        }
        // ReSharper disable once InconsistentNaming
        private static string HTMLFooter()
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
