using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class ManualFoldersMovieCheck : CustomMovieCheck
{
    public ManualFoldersMovieCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
    {
    }

    /// <exception cref="FixCheckException">Can't fix ManualFoldersMovieCheck</exception>
    protected override void FixInternal()
    {
        if (Movie.UseManualLocations && Movie.ManualLocations.Count == 1 && !Movie.AutomaticFolderRoot.HasValue())
        {
            string manualLocation = Movie.ManualLocations.First();
            if (TVSettings.Instance.MovieLibraryFolders.Any(root => manualLocation.StartsWith(root, StringComparison.Ordinal)))
            {
                Movie.AutomaticFolderRoot =
                    TVSettings.Instance.MovieLibraryFolders.FirstOrDefault(f => manualLocation.StartsWith(f, StringComparison.Ordinal)) ?? string.Empty;
            }
        }
        if (!Movie.AutomaticFolderRoot.HasValue())
        {
            throw new FixCheckException("Movie has no automatic folder root");
        }
        switch (Movie.ManualLocations.Count)
        {
            case > 1:
                throw new FixCheckException("Movie has multiple manual locations - unclear which to copy across");
            case 0:
                //no files to copy
                Movie.UseAutomaticFolders = true;
                Movie.UseManualLocations = false;
                return;
            case 1:
                {
                    DirectoryInfo source = new(Movie.ManualLocations.First());
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

                    //try to copy/move files
                    string automaticLocation = Movie.AutoFolderNameForMovie();

                    if (source.FullName.Equals(automaticLocation, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Movie.UseAutomaticFolders = true;
                        Movie.UseManualLocations = false;
                        return;
                    }

                    TryToMoveFiles(source, automaticLocation);
                    Movie.UseAutomaticFolders = true;
                    Movie.UseManualLocations = false;
                    break;
                }
        }
    }

    private void TryToMoveFiles(DirectoryInfo manualSource, string automaticDestination)
    {
        //Do we want to copy the whole folder or just some files from within?
        //we have one location to copy to

        bool manualLocationOnlyHasOneMovie = manualSource.EnumerateFiles().Where(f => f.IsMovieFile())
            .All(file => Movie.NameMatch(file, false));

        try
        {
            if (manualLocationOnlyHasOneMovie)
            {
                CopyOrMove(manualSource, automaticDestination);
            }
            else
            {
                MoveFiles(
                    manualSource.EnumerateFiles()
                        .Where(f => f.IsMovieFile())
                        .Where(file => Movie.NameMatch(file, false)),
                    automaticDestination);
            }
        }
        catch (System.IO.IOException ioe)
        {
            throw new FixCheckException(ioe.Message, ioe);
        }
    }

    private static void MoveFiles(IEnumerable<FileInfo> where, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (FileInfo? f in where)
        {
            string destinationPath = System.IO.Path.Combine(destination, f.Name);
            f.MoveTo(destinationPath);
            LOGGER.Info($"Moved {f.FullName} to {destinationPath}");
        }
    }

    private static void CopyOrMove(DirectoryInfo fromDirectory, string toDirectory)
    {
        DirectoryInfo target = new(toDirectory);

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

                    LOGGER.Info($"Moved {file.FullName} to {destFile}");
                }
            }

            FileHelper.DoTidyUp(fromDirectory, TVSettings.Instance.Tidyup);
            return;
        }

        fromDirectory.MoveTo(toDirectory);

        LOGGER.Info($"Moved whole directory {fromDirectory.FullName} to {toDirectory}");
    }

    protected override string FieldName => "Use manual folders";

    protected override bool Field => Movie.UseManualLocations;

    protected override string CustomFieldValue => Movie.ManualLocations.ToCsv();

    protected override string DefaultFieldValue => Movie.AutoFolderNameForMovie();
}
