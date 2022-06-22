using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    internal class CleanUpEmptyLibraryFolders : PostScanActivity
    {
        public CleanUpEmptyLibraryFolders(TVDoc doc) : base(doc)
        {
        }

        public override string ActivityName() => "Clean up empty library folders";

        protected override bool Active() => true;

        protected override void DoCheck(System.Threading.CancellationToken token, PostScanProgressDelegate progress)
        {
            List<ShowConfiguration> libraryShows = MDoc.TvLibrary.GetSortedShowItems();
            List<MovieConfiguration> movieConfigurations = MDoc.FilmLibrary.GetSortedMovies();
            List<string> folders = TVSettings.Instance.LibraryFolders.Union(TVSettings.Instance.MovieLibraryFolders).ToList();

            int totalRecords = libraryShows.Count + movieConfigurations.Count + folders.Count;
            int n = 0;
            string lastUpdate = string.Empty;

            foreach (ShowConfiguration si in libraryShows)
            {
                progress(n++, totalRecords, si.ShowName,lastUpdate);

                foreach (string folderName in si.AllProposedFolderLocations().SelectMany(folderLocation => folderLocation.Value))
                {
                    string action = RemoveIfEmpty(si, folderName);
                    if (action.HasValue())
                    {
                        lastUpdate = action;
                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }

            foreach (MovieConfiguration mi in movieConfigurations)
            {
                progress(n++, totalRecords, mi.ShowName, lastUpdate);

                foreach (string folderName in mi.Locations)
                {
                    string action = RemoveIfEmpty(mi, folderName);
                    if (action.HasValue())
                    {
                        lastUpdate = action;
                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }

            foreach (string folder in folders)
            {
                progress(n++, totalRecords, folder, lastUpdate);
                DirectoryInfo directory = new DirectoryInfo(folder);
                foreach (DirectoryInfo testDirectory in  directory.EnumerateDirectories(DirectoryEnumerationOptions.Recursive | DirectoryEnumerationOptions.ContinueOnException).ToList())
                {
                    string action = RemoveIfEmpty(testDirectory.FullName);
                    if (action.HasValue())
                    {
                        lastUpdate = action;
                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }

        private string? RemoveIfEmpty(string folderName)
        {
            if (CanRemove(folderName))
            {
                //FileHelper.RemoveDirectory(folderName);
                if (!AlreadyDeleted(MDoc.TheActionList, folderName))
                {
                    MDoc.TheActionList.Add(new ActionDeleteDirectory(new DirectoryInfo(folderName)));
                    return $"Removed {folderName}";
                }
            }

            return null;
        }

        private static bool AlreadyDeleted(ItemList mDocTheActionList, string folderName)
        {
            return mDocTheActionList.OfType<ActionDeleteDirectory>().Any(deleteDirectory => deleteDirectory.IsFor(folderName));
        }

        private string? RemoveIfEmpty(MovieConfiguration mi, string folderName)
        {
            if (CanRemove(folderName))
            {
                //FileHelper.RemoveDirectory(folderName);
                MDoc.TheActionList.Add(new ActionDeleteDirectory(new DirectoryInfo(folderName),mi,TVSettings.Instance.Tidyup));
                return $"Removed {folderName}";
            }

            return null;
        }        private string? RemoveIfEmpty(ShowConfiguration si, string folderName)
        {
            if (CanRemove(folderName))
            {
                //FileHelper.RemoveDirectory(folderName);
                MDoc.TheActionList.Add(new ActionDeleteDirectory(new DirectoryInfo(folderName),si,TVSettings.Instance.Tidyup));
                return $"Removed {folderName}";
            }

            return null;
        }
        private static bool CanRemove(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                return false;
            }

            try
            {
                DirectoryInfo d = new(folderName);
                if (FileFinder.IsSubsFolder(d))
                {
                    return false;
                }
                //nothing at all
                bool noSubFolders = d.GetDirectories().Length == 0;
                if (d.GetFiles().Length == 0 && noSubFolders)
                {
                    return true;
                }

                bool containsMovieFiles = d.GetFiles("*",System.IO.SearchOption.AllDirectories).Any(s => s.IsMovieOrUsefulFile());

                if (!containsMovieFiles && noSubFolders)
                {
                    return true;
                }
            }
            catch (ArgumentException a)
            {
                LOGGER.Warn($"Could not determine whether {folderName} can be removed as we got as ArgumentException {a.Message}");
            }
            catch (FileReadOnlyException)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a FileReadOnlyException");
            }
            catch (DirectoryReadOnlyException)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a DirectoryReadOnlyException");
            }
            catch (UnauthorizedAccessException)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a UnauthorizedAccessException");
            }
            catch (System.IO.PathTooLongException)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a PathTooLongException");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                LOGGER.Info($"Could not find {folderName} as we got a DirectoryNotFoundException");
            }
            catch (DirectoryNotEmptyException)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a DirectoryNotEmptyException");
            }
            catch (OperationCanceledException)
            {
                LOGGER.Info($"Could not find {folderName} as we got a OperationCanceledException");
            }
            catch (System.IO.IOException i)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a OperationCanceledException: {i.Message}");
            }

            return false;
        }
    }
}
