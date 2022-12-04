namespace TVRename;

internal class CustomRegionMovieCheck : CustomMovieCheck
{
    public CustomRegionMovieCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
    {
    }

    protected override void FixInternal()
    {
        Movie.UseCustomRegion = false;
    }

    protected override string FieldName => "Use Custom Region";

    protected override bool Field => Movie.UseCustomRegion;

    protected override string CustomFieldValue => Movie.CustomRegionCode ?? string.Empty;

    protected override string DefaultFieldValue => Movie.Provider == TVDoc.ProviderType.TMDB ? TVSettings.Instance.TMDBRegion.ThreeAbbreviation : string.Empty;
}
