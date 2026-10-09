//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename.Forms;

internal class CollectionMember(string collectionName, CachedMovieInfo neededShowValue)
{
    public readonly string CollectionName = collectionName;
    public readonly CachedMovieInfo Movie = neededShowValue;

    // ReSharper disable once UnusedMember.Global - Used by UI component
    public string MovieName => Movie.Name;

    public int TmdbCode => Movie.TmdbCode;

    public bool IsInLibrary;

    public int? MovieYear => Movie.Year;
    public DateTime? ReleaseDate => Movie.FirstAired;
}
