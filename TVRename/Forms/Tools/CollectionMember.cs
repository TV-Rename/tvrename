using System;

namespace TVRename.Forms;

internal class CollectionMember
{
    public string CollectionName;
    public CachedMovieInfo Movie;

    // ReSharper disable once UnusedMember.Global - Used by UI component
    public string MovieName => Movie.Name;

    public int TmdbCode => Movie.TmdbCode;

    public bool IsInLibrary;

    public CollectionMember(string collectionName, CachedMovieInfo neededShowValue)
    {
        CollectionName = collectionName;
        Movie = neededShowValue;
    }

    public int? MovieYear => Movie.Year;
    public DateTime? ReleaseDate => Movie.FirstAired;
}
