using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    public class BulkAddMovieManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public PossibleNewMovies AddItems;
        private readonly TVDoc mDoc;

        //Thread safe counters to work out the progress
        //for scanning
        private static volatile int CurrentPhaseDirectory;
        private static volatile int CurrentPhaseTotalDirectory;
        private static volatile int CurrentPhase;
        private static volatile int CurrentPhaseTotal;

        public BulkAddMovieManager(TVDoc doc)
        {
            AddItems = new PossibleNewMovies();
            mDoc = doc;
        }

        private DirectoryInfo[]? GetValidDirectories([NotNull] DirectoryInfo di)
        {
            try
            {
                return  di.GetDirectories().Where(d => d.IsImportant()).ToArray();
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

        public (bool finished, DirectoryInfo[] subDirs) CheckFolderForMovies([NotNull] DirectoryInfo di2, bool andGuess, bool fullLogging, bool showErrorMsgBox)
        {
            CurrentPhaseDirectory++;
            try
            {
                // ..and not already a folder for one of our shows
                string theFolder = di2.FullName.ToLower();
                foreach (var si in mDoc.FilmLibrary.Movies)
                {
                    if (RejectFolderIfIncludedInShow(fullLogging, si, theFolder))
                    {
                        return (true, null);
                    }
                } // for each showitem

                //We don't have it already
                DirectoryInfo[] subDirectories = GetValidDirectories(di2);

                //This is an indication that something is wrong
                if (subDirectories is null)
                {
                    return (false, null);
                }

                bool hasSubFolders = subDirectories.Length > 0;
                if (hasSubFolders)
                {
                    return (false, subDirectories);
                }

                if (TVSettings.Instance.BulkAddIgnoreRecycleBin && IsRecycleBin(di2))
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
                    Logger.Info("Adding {0} as a new folder", theFolder);
                    PossibleNewMovie ai = new PossibleNewMovie(newFilm, andGuess, showErrorMsgBox);
                    AddItems.AddIfNew(ai);
                }

                return (false, subDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Info("Can't access {0}, so ignoring it", di2.FullName);
                return (true, null);
            }
        }

        private static bool IsRecycleBin([NotNull] DirectoryInfo di2)
        {
            bool endsWith = di2.FullName.Contains("$RECYCLE.BIN", StringComparison.OrdinalIgnoreCase)
                            || di2.FullName.Contains("\\@Recycle\\", StringComparison.OrdinalIgnoreCase)
                            || di2.FullName.EndsWith("\\@Recycle", StringComparison.OrdinalIgnoreCase);

            return endsWith;
        }

        private static bool RejectFolderIfIncludedInShow(bool fullLogging, [NotNull] MovieConfiguration si, string theFolder)
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

        private static List<FileInfo> FilmFiles([NotNull] DirectoryInfo directory)
        {
            return directory.GetFiles("*", System.IO.SearchOption.TopDirectoryOnly).Where(file => file.IsMovieFile()).ToList();
        }

        private void CheckFolderForShows([NotNull] DirectoryInfo di, CancellationToken token, BackgroundWorker bw, bool fullLogging, bool showErrorMsgBox)
        {
            int percentComplete = (int)(100.0 / CurrentPhaseTotal * ((1.0 * CurrentPhase) + (1.0 * CurrentPhaseDirectory / CurrentPhaseTotalDirectory)));
            if (percentComplete > 100)
            {
                percentComplete = 100;
            }
            bw.ReportProgress(percentComplete,di.Name);
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
                    Logger.Info("Rejecting {0} as it's on the ignore list.", di.FullName);
                }

                return;
            }

            (bool finished, DirectoryInfo[] subDirs) = CheckFolderForMovies(di, false, fullLogging, showErrorMsgBox);

            if (finished)
            {
                return; // done.
            }

            if (subDirs is null)
            {
                return; //indication we could not access the sub-directory
            }

            // recursively check a folder for new shows
            CurrentPhaseTotalDirectory += subDirs.Length;

            foreach (DirectoryInfo di2 in subDirs)
            {
                CheckFolderForShows(di2, token,bw, fullLogging, showErrorMsgBox); // not a season folder.. recurse!
            } // for each directory
        }

        public void AddAllToMyMovies()
        {
            foreach (PossibleNewMovie ai in AddItems.Where(ai => ai.CodeKnown))
            {
                AddToLibrary(ai);
            }

            mDoc.SetDirty();
            AddItems.Clear();
            mDoc.ExportMovieInfo();
        }

        private void AddToLibrary([NotNull] PossibleNewMovie ai)
        {
            if (ai.CodeUnknown)
            {
                return;
            }

            string matchingRoot = TVSettings.Instance.MovieLibraryFolders.FirstOrDefault(s =>  ai.Directory.FullName.IsSubfolderOf(s));
            bool isInLibraryFolderFileFinder = matchingRoot.HasValue();

            // see if there is a matching show item
            MovieConfiguration found = mDoc.FilmLibrary.GetMovie(ai);
            if (found is null)
            {
                AddNewMovieToLibrary(ai, isInLibraryFolderFileFinder, matchingRoot);
                return;
            }

            //We are updating an existing record

            string targetDirectoryName = CustomMovieName.NameFor(found, TVSettings.Instance.MovieFolderFormat);
            bool inDefaultPath = ai.Directory.Name.Equals(
                targetDirectoryName,
                StringComparison.CurrentCultureIgnoreCase);

            bool existingLocationIsDefaultToo = found.UseAutomaticFolders && found.AutomaticFolderRoot.In(TVSettings.Instance.MovieLibraryFolders.ToArray());

            if (inDefaultPath && isInLibraryFolderFileFinder && !existingLocationIsDefaultToo)
            {
                found.UseAutomaticFolders = true;
                found.UseCustomFolderNameFormat = false;
                found.AutomaticFolderRoot = matchingRoot!;
                //leave found.UseManualLocations alone to retain any existing manual locations
                return;
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

        private void AddNewMovieToLibrary(PossibleNewMovie ai, bool isInLibraryFolderFileFinder, string? matchingRoot)
        {
            // need to add a new showitem
            MovieConfiguration found = new MovieConfiguration(ai);

            mDoc.FilmLibrary.Add(found);

            mDoc.Stats().AutoAddedMovies++;

            bool inDefaultPath = ai.Directory.Name.Equals(
                CustomMovieName.NameFor(found, TVSettings.Instance.MovieFolderFormat),
                StringComparison.CurrentCultureIgnoreCase);

            if (inDefaultPath && isInLibraryFolderFileFinder)
            {
                found.UseAutomaticFolders = true;
                found.UseCustomFolderNameFormat = false;
                found.AutomaticFolderRoot = matchingRoot!;
                found.UseManualLocations = false;
                return;
            }

            if (isInLibraryFolderFileFinder)
            {
                found.AutomaticFolderRoot = matchingRoot!;
            }

            found.UseAutomaticFolders = false;
            found.UseCustomFolderNameFormat = false;
            found.UseManualLocations = true;
            found.ManualLocations.Add(ai.Directory.FullName);
        }

        public void CheckFolders(CancellationToken token,BackgroundWorker bw,  bool detailedLogging, bool showErrorMsgBox)
        {
            // Check the  folder list, and build up a new "AddItems" list.
            // guessing what the shows actually are isn't done here.  That is done by
            // calls to "GuessShowItem"
            Logger.Info("*********************************************************************");
            Logger.Info("*Starting to find folders that contain files, but are not in library*");

            AddItems = new PossibleNewMovies();

            CurrentPhaseTotal = 1;
            if (TVSettings.Instance.MovieLibraryFolders.Count > 0)
            {
                CurrentPhaseTotal = TVSettings.Instance.MovieLibraryFolders.Count;
            }

            CurrentPhase = 0;

            foreach (string folder in TVSettings.Instance.MovieLibraryFolders)
            {
                CurrentPhaseDirectory = 0;
                CurrentPhaseTotalDirectory = 1;

                DirectoryInfo di = new DirectoryInfo(folder);
                if (TVSettings.Instance.LibraryFolders.Contains(folder))
                {
                    Logger.Warn($"Not loading {folder} as it is both a movie folder and a tv folder");
                    continue;
                }
                CheckFolderForShows(di, token,bw, detailedLogging, showErrorMsgBox);

                if (token.IsCancellationRequested)
                {
                    break;
                }
                Interlocked.Increment(ref CurrentPhase);

            }
        }
    }
}
