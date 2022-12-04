using System.Collections.Generic;
using TVRename.YTS;

namespace TVRename.Forms;

public class YtsViewerRow
{
    private readonly API.YtsMovie result;
    public MovieConfiguration? Movie;
    public YtsViewerRow(API.YtsMovie x, TVDoc library)
    {
        result = x;
        Movie = library.FilmLibrary.GetMovieFromImdb(ImdbCode);
    }

    public string Name => Movie?.Name ?? result.Name;
    public string Overview => Movie?.CachedMovie?.Overview ?? result.Overview;
    public string Year => Movie?.CachedMovie?.Year.ToString() ?? result.Year;
    public int Id => result.Id;
    public string YtsUrl => result.YtsUrl;
    public string Runtime => result.Runtime;
    public IEnumerable<string> Genres => result.Genres;
    public string Language => Movie?.CachedMovie?.ShowLanguage ?? result.Language;

    //Star score is out of 5 stars, we produce a 'normlised' result by adding a top mark 10/10 and a bottom mark 1/10 and recalculating
    //this is to stop a show with one 10/10 vote looking too good, this normalises it back if the number of votes is small
    public float StarScore => result.StarScore;
    public string ContentRating => result.ContentRating;
    public string ImdbCode => result.ImdbCode;
    public string? PosterUrl => result.PosterUrl;
    public string BackgroundUrl => result.BackgroundUrl;
    public string TrailerUrl => result.TrailerUrl;
    public IEnumerable<API.YtsDownload> Downloads => result.Downloads;
    public API.YtsMovie YtsMovie => result;
    public void SetShow(MovieConfiguration found)
    {
        Movie = found;
    }
}
