using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class MovieManualFoldersMirrorAutomaticCheck : MovieCheck
{
    public MovieManualFoldersMirrorAutomaticCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
    {
    }

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

        if (!Movie.ManualLocations.Any())
        {
            Movie.UseManualLocations = false;
        }
    }

    protected override string MovieCheckName => "Manual Movie Folders Mirror Automatic";
}
