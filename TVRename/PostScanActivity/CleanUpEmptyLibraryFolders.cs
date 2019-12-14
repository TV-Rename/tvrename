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

        public override bool Active() => TVSettings.Instance.CleanLibraryAfterActions;

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

        private void RemoveIfEmpty(string folderName)
        {
            if (CanRemove(folderName))
            {
                RemoveDirectory(folderName);
            }
        }

        private void RemoveDirectory(string folderName)
        {
            LOGGER.Info($"Removing {folderName} as part of the library clean up");
            foreach (string file in Directory.GetFiles(folderName))
            {
                LOGGER.Info($"    Folder contains {file}");
            }
            Directory.Delete(folderName,true);
        }

        private bool CanRemove(string folderName)
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
