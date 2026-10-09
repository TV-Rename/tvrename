//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class MovieManualFoldersMirrorAutomaticCheck(MovieConfiguration movie, TVDoc doc) : MovieCheck(movie, doc)
{
    public override bool Check()
    {
        return Movie.UseManualLocations && Movie.ManualLocations.Any(loc => LocationsMatch(Movie.AutoFolderNameForMovie(), loc));
    }

    private static bool LocationsMatch(string aloc, string loc) => string.Equals(aloc, loc, StringComparison.Ordinal);

    public override string Explain()
    {
        IEnumerable<string> matchingLocations =
            Movie.ManualLocations.Where(loc => LocationsMatch(Movie.AutoFolderNameForMovie(), loc));

        return $"{Movie.Name} has manual locations that match automatic ones {matchingLocations.ToCsv()}";
    }

    protected override void FixInternal()
    {
        IEnumerable<string> matchingLocations =
            [.. Movie.ManualLocations.Where(loc => LocationsMatch(Movie.AutoFolderNameForMovie(), loc))];

        Movie.ManualLocations.RemoveNullableRange(matchingLocations);

        if (!Movie.ManualLocations.IsAny())
        {
            Movie.UseManualLocations = false;
        }
    }

    protected override string MovieCheckName => "Manual Movie Folders Mirror Automatic";
}
