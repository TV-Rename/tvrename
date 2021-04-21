using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class ManualFoldersMovieCheck : CustomMovieCheck
    {
        public ManualFoldersMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
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
                    Movie.UseAutomaticFolders = true;
                    return;
                }

                if (!source.EnumerateFiles().Any() && !source.EnumerateDirectories().Any())
                {
                    //directory has nothing in it
                    FileHelper.RemoveDirectory(source.FullName);
                    Movie.UseManualLocations = false;
                    Movie.UseAutomaticFolders = true;
                    return;
                }

                Movie.UseAutomaticFolders = true;
                //try to copy/move files
                System.Collections.Generic.List<string> automaticLocations = Movie.AutomaticLocations().ToList();

                if (automaticLocations.Count > 1)
                {
                    throw new FixCheckException("Multiple target locations for the automated folders");
                }

                if (!automaticLocations.Any())
                {
                    throw new FixCheckException("No target automatic folders can be established");
                }

                if (source.FullName.Equals(automaticLocations.First(),StringComparison.CurrentCultureIgnoreCase))
                {
                    Movie.UseManualLocations = false;
                    return;
                }

                //Do we want to copy the whole folder or just some files from witin?
                //we have one location to copy to

                bool manualLocationOnlyHasOneMovie = source.EnumerateFiles().Where(f=>f.IsMovieFile()).All(file => Movie.NameMatch(file, false));

                if (manualLocationOnlyHasOneMovie)
                {
                    CopyOrMove(source, automaticLocations.First());
                }
                else
                {
                    MoveFiles(source.EnumerateFiles().Where(f => f.IsMovieFile()).Where(file => Movie.NameMatch(file, false)), automaticLocations.First());
                }

                Movie.UseManualLocations = false;
            }
        }

        private void MoveFiles(IEnumerable<FileInfo> @where, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (FileInfo? f in where)
            {
                f.MoveTo(System.IO.Path.Combine(destination, f.Name));
            }
        }

        private void CopyOrMove(DirectoryInfo fromDirectory, string toDirectory)
        {
            DirectoryInfo target = new DirectoryInfo(toDirectory);

            if (target.Exists)
            {
                if (target.GetFiles().Any(x => x.IsMovieFile()))
                {
                    throw new FixCheckException("Target location has movie files - not copying just in case");
                }

                //Copy files
                foreach (FileInfo file in fromDirectory.EnumerateFiles())
                {
                    string destFile = Path.Combine(toDirectory, file.Name);
                    if (!File.Exists(destFile))
                    {
                        file.MoveTo(destFile);
                        //File.Move(file, destFile);

                        LOGGER.Info($"Moved {file.FullName} to {destFile}");
                    }
                }

                if (Directory.IsEmpty(fromDirectory.FullName))
                {
                    fromDirectory.Delete(false);
                    LOGGER.Info($"Deleted empty directory {fromDirectory.FullName}");
                }
                return;
            }

            fromDirectory.MoveTo(toDirectory);
            //Directory.Move(fromDirectpry, toDirectory);
            LOGGER.Info($"Moved whole directory {fromDirectory.FullName } to {toDirectory}");
        }

        protected override string FieldName => "Use manual folders";

        protected override bool Field => Movie.UseManualLocations;
    }
}
