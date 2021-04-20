using System;
using JetBrains.Annotations;

namespace TVRename
{
    public class MovieItemMissing : ItemMissing
    {
        public MovieItemMissing([NotNull] MovieConfiguration movie, [NotNull] string whereItShouldBeFolder)
        {
            Episode = null;
            Filename = TVSettings.Instance.FilenameFriendly(movie.ProposedFilename);
            TheFileNoExt = whereItShouldBeFolder + System.IO.Path.DirectorySeparatorChar + Filename;
            Folder = whereItShouldBeFolder;
            Movie = movie;
        }
        #region Item Members

        public override bool SameAs(Item o)
        {
            return o is MovieItemMissing missing && string.CompareOrdinal(missing.TheFileNoExt, TheFileNoExt) == 0;
        }

        public override string Name => "Missing Movie";

        public override int CompareTo(Item o)
        {
            if (!(o is MovieItemMissing miss))
            {
                return -1;
            }

            return string.Compare(TheFileNoExt, miss.TheFileNoExt, StringComparison.Ordinal);
        }

        public MovieConfiguration MovieConfig => Movie ?? throw new InvalidOperationException();
        #endregion

        public override bool DoRename => MovieConfig.DoRename;
        public override MediaConfiguration Show => MovieConfig;
        public override string ToString() => $"{MovieConfig.ShowName}";
    }
}
