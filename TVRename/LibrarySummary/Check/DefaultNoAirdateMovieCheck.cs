namespace TVRename;

internal class DefaultNoAirdateMovieCheck(MovieConfiguration show, TVDoc doc) : DefaultMovieCheck(show, doc)
{
    protected override string FieldName => "No Airdate Movie Check";

    protected override bool Field => Movie.ForceCheckNoAirdate;

    protected override bool Default => TVSettings.Instance.DefMovieCheckNoDatedMovies;

    protected override void FixInternal()
    {
        Movie.ForceCheckNoAirdate = Default;
    }
}
