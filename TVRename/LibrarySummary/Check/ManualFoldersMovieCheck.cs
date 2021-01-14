using System;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    class ManualFoldersMovieCheck : CustomMovieCheck
    {
        public ManualFoldersMovieCheck([NotNull] MovieConfiguration movie) : base(movie)
        {
        }

        protected override void FixInternal()
        {
            if (!Movie.AutomaticFolderRoot.HasValue())
            {
                throw new FixCheckException("Movie has no automatic folder root");
            }
            if (Movie.ManualLocations.Count > 1)
            {
                throw new FixCheckException("Movie has multiple manual locations - unclear which to copy across");
            }
            if (Movie.ManualLocations.Count ==0)
            {
                //no files to copy
                Movie.UseAutomaticFolders = true;
                Movie.UseManualLocations = false;
                return;
            }
            
            if (Movie.ManualLocations.Count == 1 )
            {
                DirectoryInfo source = new DirectoryInfo(Movie.ManualLocations.First());
                if (!source.Exists)
                {
                    Movie.UseManualLocations = false;
                    return;
                }

                if (!source.EnumerateFiles().Any() && !source.EnumerateDirectories().Any())
                {
                    //directory has nothing in it
                    FileHelper.RemoveDirectory(source.FullName);
                    Movie.UseManualLocations = false;
                    return;
                }

                //try to copy/move files
                if (Movie.AutomaticLocations().Count() > 1)
                {
                    throw new FixCheckException("Multiple target locations for the automated folders");
                }

                if (!Movie.AutomaticLocations().Any())
                {
                    throw new FixCheckException("No target automatic folders can be established");
                }

                if (source.FullName.Equals(Movie.AutomaticLocations().First(),StringComparison.CurrentCultureIgnoreCase))
                {
                    Movie.UseManualLocations = false;
                    return;

                }

                //we have one location
                CopyOrMove(Movie.ManualLocations[0], Movie.AutomaticLocations().First());
                Movie.UseAutomaticFolders = true;
                Movie.UseManualLocations = false;
            }
        }

        private void CopyOrMove(string fromDirectpry, string toDirectory)
        {
            DirectoryInfo target = new DirectoryInfo(toDirectory);

            if (target.Exists)
            {
                if (target.GetFiles().Any(x => x.IsMovieFile()))
                {
                    throw new FixCheckException("Target location has movie files - not copying just in case");
                }

                //Copy files
                foreach (string? file in Directory.EnumerateFiles(fromDirectpry))
                {
                    string destFile = Path.Combine(toDirectory, Path.GetFileName(file));
                    if (!File.Exists(destFile))
                    {
                        File.Move(file, destFile);
                    }
                    LOGGER.Info($"Moved {file} to {destFile}");

                }
                    
                if (Directory.IsEmpty(fromDirectpry))
                {
                    Directory.Delete(fromDirectpry,true);
                    LOGGER.Info($"Deleted empty directory {fromDirectpry}");
                }
                return;
            }

            Directory.Move(fromDirectpry, toDirectory);
            LOGGER.Info($"Moved whole directory {fromDirectpry} to {toDirectory}");
        }

        protected override string FieldName => "Use manual folders";

        protected override bool Field => Movie.UseManualLocations;
    }
}
