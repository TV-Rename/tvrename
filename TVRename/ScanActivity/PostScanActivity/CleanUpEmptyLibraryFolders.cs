using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    internal class CleanUpEmptyLibraryFolders : PostScanActivity
    {
        public CleanUpEmptyLibraryFolders(TVDoc doc) : base(doc)
        {
        }

        [NotNull]
        public override string ActivityName() => "Clean up empty library folders";

        protected override bool Active() => true;

        protected override void DoCheck(System.Threading.CancellationToken token, PostScanProgressDelegate progress)
        {
            List<ShowConfiguration> libraryShows = MDoc.TvLibrary.GetSortedShowItems();
            List<MovieConfiguration> movieConfigurations = MDoc.FilmLibrary.GetSortedMovies();
            int totalRecords = libraryShows.Count + movieConfigurations.Count;
            int n = 0;
            string lastUpdate = string.Empty;

            foreach (ShowConfiguration si in libraryShows)
            {
                progress(n++, totalRecords, si.ShowName,lastUpdate);

                foreach (string folderName in si.AllProposedFolderLocations().SelectMany(folderLocation => folderLocation.Value))
                {
                    string action = RemoveIfEmpty(folderName);
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
                    string action = RemoveIfEmpty(folderName);
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

        [CanBeNull]
        private static string RemoveIfEmpty(string folderName)
        {
            if (CanRemove(folderName))
            {
                FileHelper.RemoveDirectory(folderName);
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
                //nothing at all
                bool noSubFolders = Directory.GetDirectories(folderName).Length == 0;
                if (Directory.GetFiles(folderName).Length == 0 && noSubFolders)
                {
                    return true;
                }

                bool containsMovieFiles = Directory.GetFiles(folderName,"*",System.IO.SearchOption.AllDirectories).Any(s => s.IsMovieFile());

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
