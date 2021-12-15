﻿using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class MovieCheckEmptyManualFolders : MovieCheck
    {
        public MovieCheckEmptyManualFolders(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        public override bool Check()
        {
            if (!Movie.UseManualLocations)
            {
                return false;
            }

            if (Movie.ManualLocations.Count == 0)
            {
                return false;
            }

            return Movie.ManualLocations.Any(DirectoryIsMissingEmpty);
        }

        private static bool DirectoryIsMissingEmpty(string path) => !DirectoryHasContents(path);

        private static bool DirectoryHasContents(string path)
        {
            return path.HasValue()
                   && Directory.Exists(path)
                   && Directory.EnumerateFileSystemEntries(path).Any();
        }

        [NotNull]
        public override string Explain() => $"{Movie.Name} has manual folders set, these folders are missing or empty: {Movie.ManualLocations.Where(DirectoryIsMissingEmpty).ToCsv()}";

        protected override void FixInternal()
        {
            foreach (string directory in Movie.ManualLocations.Where(DirectoryIsMissingEmpty))
            {
                Movie.ManualLocations.Remove(directory);
                RemoveEmptyDirectory(directory);
            }

            if (!Movie.ManualLocations.Any())
            {
                Movie.UseManualLocations = false;
            }
        }

        private static void RemoveEmptyDirectory(string directory)
        {
            if (DirectoryIsMissingEmpty(directory))
            {
                Directory.Delete(directory);
            }
        }

        [NotNull]
        public override string CheckName => "[Movie] Movie has missing or empty manual folder";
    }
}