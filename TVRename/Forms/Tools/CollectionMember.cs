using System;

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
