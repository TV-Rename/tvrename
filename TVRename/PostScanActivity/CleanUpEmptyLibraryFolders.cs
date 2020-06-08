using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using Microsoft.VisualBasic.FileIO;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class CleanUpEmptyLibraryFolders : PostScanActivity
    {
        public CleanUpEmptyLibraryFolders(TVDoc doc) : base(doc) {}

        protected override string Checkname() => "Cleaned up empty library folders";

        protected override bool Active() => TVSettings.Instance.CleanLibraryAfterActions;

        protected override void DoCheck(SetProgressDelegate prog)
        {
            foreach (ShowItem si in MDoc.Library.Shows)
            {
                foreach (KeyValuePair<int, List<string>> folderLocation in si.AllProposedFolderLocations())
                {
                    foreach (string folderName in folderLocation.Value)
                    {
                        RemoveIfEmpty(folderName);
                    }
                }
            }
        }

        private static void RemoveIfEmpty(string folderName)
        {
            if (CanRemove(folderName))
            {
                RemoveDirectory(folderName);
            }
        }

        private static void RemoveDirectory([NotNull] string folderName)
        {
            try
            {
                LOGGER.Info($"Removing {folderName} as part of the library clean up");
                foreach (string file in Directory.GetFiles(folderName))
                {
                    LOGGER.Info($"    Folder contains {file}");
                }

                LOGGER.Info($"Recycling {folderName}");
                FileSystem.DeleteDirectory(folderName,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);
            }
            catch (FileReadOnlyException)
            {
                LOGGER.Warn($"Could not recycle {folderName} as we got a FileReadOnlyException");
            }
            catch (DirectoryReadOnlyException)
            {
                LOGGER.Warn($"Could not recycle {folderName} as we got a DirectoryReadOnlyException");
            }
            catch (UnauthorizedAccessException)
            {
                LOGGER.Warn($"Could not recycle {folderName} as we got a UnauthorizedAccessException");
            }
            catch (PathTooLongException)
            {
                LOGGER.Warn($"Could not recycle {folderName} as we got a PathTooLongException");
            }
            catch (DirectoryNotFoundException)
            {
                LOGGER.Info($"Could not recycle {folderName} as we got a DirectoryNotFoundException");
            }
            catch (DirectoryNotEmptyException)
            {
                LOGGER.Warn($"Could not recycle {folderName} as we got a DirectoryNotEmptyException");
            }
            catch (OperationCanceledException)
            {
                LOGGER.Info($"Could not recycle {folderName} as we got a OperationCanceledException");
            }
            catch (IOException i)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a OperationCanceledException: {i.Message}");
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

                bool containsMovieFiles = Directory.GetFiles(folderName).ToList().Select(s => new FileInfo(s))
                    .Any(info => info.IsMovieFile());

                if (!containsMovieFiles)
                {
                    return true;
                }
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
            catch (PathTooLongException)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a PathTooLongException");
            }
            catch (DirectoryNotFoundException)
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
            catch (IOException i)
            {
                LOGGER.Warn($"Could not find {folderName} as we got a OperationCanceledException: {i.Message}");
            }

            return false;
        }
    }
}
