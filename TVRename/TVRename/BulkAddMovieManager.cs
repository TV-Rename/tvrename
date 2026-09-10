
using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TVRename.Forms;

namespace TVRename;

public class BulkAddMovieManager(TVDoc doc)
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public PossibleNewMovies AddItems = [];
    private readonly TVDoc mDoc = doc;

    //Thread safe counters to work out the progress
    //for scanning
    private static ThreadSafeCounter CurrentPhaseDirectory = new();
    private static ThreadSafeCounter CurrentPhaseTotalDirectory = new();
    private static ThreadSafeCounter CurrentPhase = new();
    private static ThreadSafeCounter CurrentPhaseTotal = new();

    private static DirectoryInfo[]? GetValidDirectories(DirectoryInfo di)
    {
        try
        {
            return [.. di.GetDirectories().Where(d => d.IsImportant())];
        }
        catch (UnauthorizedAccessException)
        {
            // e.g. recycle bin, system volume information
            Logger.Warn($"Could not access {di.FullName} (or a subdir), may not be an issue as could be expected e.g. recycle bin, system volume information");
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            // e.g. recycle bin, system volume information
            Logger.Warn($"Could not access {di.FullName} (or a subdir), it is no longer found");
        }
        catch (System.IO.IOException)
        {
            Logger.Warn($"Could not access {di.FullName} (or a subdir), got an IO Exception");
        }
        return null;
    }

    public async Task<(bool finished, DirectoryInfo[]? subDirs)> CheckFolderForMoviesAsync(DirectoryInfo di2, bool andGuess, bool fullLogging, bool showErrorMsgBox)
    {
        CurrentPhaseDirectory.Increment();
        try
        {
            // ..and not already a folder for one of our shows
            string theFolder = di2.FullName.ToLower();
            foreach (MovieConfiguration? si in mDoc.FilmLibrary.Movies)
            {
                if (RejectFolderIfIncludedInShow(fullLogging, si, theFolder))
                {
                    return (true, null);
                }
            } // for each showitem

            //We don't have it already
            DirectoryInfo[]? subDirectories = GetValidDirectories(di2);

            //This is an indication that something is wrong
            if (subDirectories is null)
            {
                return (false, null);
            }

            /*
            bool hasSubFolders = subDirectories.Length > 0;
            if (hasSubFolders)
            {
                return (false, subDirectories);
            }*/

            if (TVSettings.Instance.BulkAddIgnoreRecycleBin && di2.IsRecycleBin())
            {
                return (false, subDirectories);
            }

            List<FileInfo> films = FilmFiles(di2);
            if (TVSettings.Instance.BulkAddCompareNoVideoFolders && !films.Any())
            {
                return (false, subDirectories);
            }
            if (!films.Any())
            {
                Logger.Warn($"Checked {di2.FullName} and it had no movie files.");
                if (!di2.GetFiles().Any() && !di2.GetDirectories().Any())
                {
                    di2.Delete(false);
                }
            }

            foreach (FileInfo newFilm in films)
            {
                // ....its good!
                Logger.Info($"Adding {newFilm.FullName} as a new Movie");
                PossibleNewMovie ai = new(newFilm, showErrorMsgBox);
                if (andGuess)
                {
                    await ai.GuessMovieAsync(showErrorMsgBox);
                }
                AddItems.AddIfNew(ai);
            }

            return (false, subDirectories);
        }
        catch (UnauthorizedAccessException)
        {
            Logger.Info($"Can't access {di2.FullName}, so ignoring it");
            return (true, null);
        }
    }

    private static bool RejectFolderIfIncludedInShow(bool fullLogging, MovieConfiguration si, string theFolder)
    {
        foreach (string dir in si.Locations)
        {
            if (!string.IsNullOrEmpty(dir) && theFolder.IsSubfolderOf(dir))
            {
                // we're looking at a folder that is a subfolder of an existing show
                if (fullLogging)
                {
                    Logger.Info($"Rejecting {theFolder} as it's already part of {si.ShowName}.");
                }

                return true;
            }
        }

        return false;
    }

    private static List<FileInfo> FilmFiles(DirectoryInfo directory)
    {
        return [.. directory.GetFiles("*", System.IO.SearchOption.TopDirectoryOnly).Where(file => file.IsMovieFile())];
    }

    private async Task CheckFolderForShowsAsync(DirectoryInfo di, BackgroundWorker bw, bool fullLogging, bool showErrorMsgBox, CancellationToken token)
    {
        int percentComplete = (int)(100.0 / CurrentPhaseTotal.Value * (1.0 * CurrentPhase.Value + 1.0 * CurrentPhaseDirectory.Value / CurrentPhaseTotalDirectory.Value ));
        bw.ReportProgress(percentComplete.Between(0, 100), di.Name);

        if (!di.Exists)
        {
            return;
        }

        if (token.IsCancellationRequested)
        {
            return;
        }

        // is it on the ''Bulk Add' ignore list?
        if (TVSettings.Instance.IgnoreFolders.Contains(di.FullName.ToLower()))
        {
            if (fullLogging)
            {
                Logger.Info($"Rejecting {di.FullName} as it's on the ignore list.");
            }

            return;
        }

        (bool finished, DirectoryInfo[]? subDirs) = await CheckFolderForMoviesAsync(di, false, fullLogging, showErrorMsgBox);

        if (finished)
        {
            return; // done.
        }

        if (subDirs is null)
        {
            return; //indication we could not access the sub-directory
        }

        // recursively check a folder for new shows
        CurrentPhaseTotalDirectory.Increment(subDirs.Length);

        foreach (DirectoryInfo di2 in subDirs)
        {
            CheckFolderForShowsAsync(di2, bw, fullLogging, showErrorMsgBox, token); // not a season folder.. recurse!
        } // for each directory
    }

    public async Task AddAllToMyMoviesAsync(UI ui)
    {
        List<MovieConfiguration> movies = AddToLibrary(AddItems.Where(ai => ai.CodeKnown));

        await mDoc.MoviesAddedOrEditedAsync(true, false, false, ui, movies);
        AddItems.Clear();
    }

    private List<MovieConfiguration> AddToLibrary(IEnumerable<PossibleNewMovie> ais)
    {
        List<MovieConfiguration> movies = [];
        foreach (PossibleNewMovie ai in ais.Where(a=>a.CodeKnown))
        {
            // see if there is a matching show item
            MovieConfiguration? found = mDoc.FilmLibrary.GetMovie(ai);
            if (found is null)
            {
                MovieConfiguration newMovie = GenerateConfiguration(ai);
                mDoc.Add(newMovie.AsList(), true);
                mDoc.Stats().AutoAddedMovies++;
                movies.Add(newMovie);
                continue;
            }

            //We are updating an existing record
            movies.Add(found);
            string targetDirectoryName = CustomMovieName.DirectoryNameFor(found, TVSettings.Instance.MovieFolderFormat);
            bool inDefaultPath = ai.Directory.Name.Equals(
                targetDirectoryName,
                StringComparison.CurrentCultureIgnoreCase);

            bool existingLocationIsDefaultToo = found.UseAutomaticFolders && found.AutomaticFolderRoot.In([.. TVSettings.Instance.MovieLibraryFolders]);
            string? matchingRoot = TVSettings.Instance.MovieLibraryFolders.FirstOrDefault(s => ai.Directory.FullName.IsSubfolderOf(s));
            bool isInLibraryFolderFileFinder = matchingRoot.HasValue();

            if (inDefaultPath && isInLibraryFolderFileFinder && !existingLocationIsDefaultToo)
            {
                found.UseAutomaticFolders = true;
                found.UseCustomFolderNameFormat = false;
                found.AutomaticFolderRoot = matchingRoot!;
                //leave found.UseManualLocations alone to retain any existing manual locations
                continue;
            }

            //we have an existing record that we need to add manual folders to

            if (isInLibraryFolderFileFinder && !found.AutomaticFolderRoot.HasValue())
            {
                //Probably in the library
                found.AutomaticFolderRoot = matchingRoot!;
            }

            found.UseManualLocations = true;
            if (!found.ManualLocations.Contains(ai.Directory.FullName))
            {
                found.ManualLocations.Add(ai.Directory.FullName);
            }
        }

        return movies;
    }

    private static MovieConfiguration GenerateConfiguration(PossibleNewMovie ai)
    {
        // need to add a new showitem
        MovieConfiguration found = new(ai)
        {
            UseCustomFolderNameFormat = false
        };

        string? matchingLibraryRoot = TVSettings.Instance.MovieLibraryFolders.FirstOrDefault(s => ai.Directory.FullName.IsSubfolderOf(s));
        bool isUnderLibraryFolder = matchingLibraryRoot.HasValue();

        if (isUnderLibraryFolder)
        {
            found.AutomaticFolderRoot = matchingLibraryRoot!;
        }

        if (!TVSettings.Instance.DefMovieUseAutomaticFolders)
        {
            found.ManualLocations.Add(ai.Directory.FullName);
            found.UseManualLocations = true;
            found.UseAutomaticFolders = false;
            return found;
        }

        bool inAutomaticFolder = isUnderLibraryFolder && found.AutoFolderNameForMovie().EnsureEndsWithSeparator().Equals(ai.Directory.FullName.EnsureEndsWithSeparator(), StringComparison.CurrentCulture);
        found.UseAutomaticFolders = inAutomaticFolder;
        found.UseManualLocations = !inAutomaticFolder;

        if (!inAutomaticFolder)
        {
            found.ManualLocations.Add(ai.Directory.FullName);
        }

        return found;
    }

    public void CheckFolders(BackgroundWorker bw, bool detailedLogging, bool showErrorMsgBox, CancellationToken token)
    {
        // Check the  folder list, and build up a new "AddItems" list.
        // guessing what the shows actually are isn't done here.  That is done by
        // calls to "GuessShowItem"
        Logger.Info("*********************************************************************");
        Logger.Info("*Starting to find folders that contain files, but are not in library*");

        AddItems = [];

        CurrentPhaseTotal.Reset(1);
        if (TVSettings.Instance.MovieLibraryFolders.Any())
        {
            CurrentPhaseTotal.Reset(TVSettings.Instance.MovieLibraryFolders.Count);
        }

        CurrentPhase.Reset();

        foreach (string folder in TVSettings.Instance.MovieLibraryFolders)
        {
            CurrentPhaseDirectory.Reset();
            CurrentPhaseTotalDirectory.Reset(1);

            DirectoryInfo di = new(folder);
            if (TVSettings.Instance.LibraryFolders.Contains(folder))
            {
                Logger.Warn($"Not loading {folder} as it is both a movie folder and a tv folder");
                continue;
            }
            CheckFolderForShowsAsync(di, bw, detailedLogging, showErrorMsgBox, token);

            if (token.IsCancellationRequested)
            {
                break;
            }
            CurrentPhase.Increment();
        }
    }
}
