//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

internal abstract class MovieCheck(MovieConfiguration movie, TVDoc doc) : SettingsCheck(doc)
{
    public readonly MovieConfiguration Movie = movie;

    protected override async Task MarkMediaDirtyAsync()
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
