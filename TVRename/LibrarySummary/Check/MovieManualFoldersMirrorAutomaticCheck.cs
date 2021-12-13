using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    class MovieManualFoldersMirrorAutomaticCheck : MovieCheck
    {
        public MovieManualFoldersMirrorAutomaticCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        public override bool Check()
        {
            return Movie.UseManualLocations && Movie.ManualLocations.Any(loc => Movie.AutomaticLocations().Any(aloc=>LocationsMatch(aloc,loc)));
        }

        private static bool LocationsMatch(string aloc, string loc) => string.Equals(aloc, loc, StringComparison.Ordinal);

        [NotNull]
        public override string Explain()
        {
            IEnumerable<string> matchingLocations =
                Movie.ManualLocations.Where(loc => Movie.AutomaticLocations().Any(aloc => LocationsMatch(aloc, loc)));

            return $"{Movie.Name} has manual locations that match automatic ones {matchingLocations.ToCsv()}";
        }

        protected override void FixInternal()
        {
            IEnumerable<string> matchingLocations =
                Movie.ManualLocations.Where(loc => Movie.AutomaticLocations().Any(aloc => LocationsMatch(aloc, loc))).ToList();
            
            Movie.ManualLocations.RemoveNullableRange(matchingLocations);

            if (!Movie.ManualLocations.Any())
            {
                Movie.UseManualLocations = false;
            }
        }

        [NotNull]
        public override string CheckName => "[Movie] Manual Folders Mirror Automatic";
    }
}
