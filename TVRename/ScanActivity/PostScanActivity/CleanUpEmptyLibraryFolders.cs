using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using Directory = Alphaleonis.Win32.Filesystem.Directory;

namespace TVRename
{
    internal class CleanUpEmptyLibraryFolders : PostScanActivity
    {
        public CleanUpEmptyLibraryFolders(TVDoc doc) : base(doc) {}

        protected override string ActivityName() => "Cleaned up empty library folders";

        protected override bool Active() => TVSettings.Instance.CleanLibraryAfterActions;

        protected override void DoCheck(SetProgressDelegate progress)
        {
            IEnumerable<ShowConfiguration> libraryShows = MDoc.TvLibrary.Shows.ToList();
            int totalRecords = libraryShows.Count();
            int n = 0;

            foreach (ShowConfiguration si in libraryShows)
            {
                UpdateStatus(n++, totalRecords, si.ShowName);

                foreach (string folderName in si.AllProposedFolderLocations().SelectMany(folderLocation => folderLocation.Value))
                {
                    RemoveIfEmpty(folderName);
                }
            }
        }

        private static void RemoveIfEmpty(string folderName)
        {
            if (CanRemove(folderName))
            {
                FileHelper.RemoveDirectory(folderName);
            }
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
                if (Directory.GetFiles(folderName).Length == 0 && Directory.GetDirectories(folderName).Length == 0)
                {
                    return true;
                }

                bool containsMovieFiles = Directory.GetFiles(folderName).Any(s => s.IsMovieFile());

                if (!containsMovieFiles)
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
