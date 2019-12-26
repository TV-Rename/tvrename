using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class CleanUpEmptyLibraryFolders : PostScanActivity
    {
        public CleanUpEmptyLibraryFolders(TVDoc doc) : base(doc) {}

        [NotNull]
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
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(folderName,
                    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            catch (FileReadOnlyException)
            {
                LOGGER.Info($"Could not recycle {folderName} as we got a FileReadOnlyException");
            }
        }

        private static bool CanRemove(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                return false;
            }

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

            return false;
        }
    }
}
