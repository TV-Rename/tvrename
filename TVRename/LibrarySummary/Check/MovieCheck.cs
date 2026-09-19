namespace TVRename;

internal abstract class MovieCheck(MovieConfiguration movie, TVDoc doc) : SettingsCheck(doc)
{
    public readonly MovieConfiguration Movie = movie;

    protected override async void MarkMediaDirtyAsync()
    {
        if (Movie.CachedMovie == null)
        {
            return;
        }

        Movie.CachedMovie.Dirty = true;
        await Doc.MoviesAddedOrEditedAsync(false, true, true, null, Movie);
    }

    public override MediaConfiguration.MediaType Type() => MediaConfiguration.MediaType.movie;

    public override string MediaName => Movie.ShowName;

    public sealed override string CheckName => "[Movie] " + MovieCheckName;

    protected abstract string MovieCheckName { get; }
}
