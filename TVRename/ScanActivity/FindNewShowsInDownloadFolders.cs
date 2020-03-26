// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class FindNewShowsInDownloadFolders : ScanActivity
    {
        public FindNewShowsInDownloadFolders(TVDoc doc) : base(doc)
        {
        }

        [NotNull]
        protected override string CheckName() => "Looked in the Search Folders for any new shows that need to be added to the library";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //does show match selected file?
            //if so add series to list of series scanned
            if (!Active())
            {
                LOGGER.Info("Not looking for new shows as 'Auto-Add' is turned off");
                return;
            }

            //Don't support unattended mode
            if (settings.Unattended || settings.Hidden)
            {
                LOGGER.Info("Not looking for new shows as app is unattended");
                return;
            }

            IEnumerable<string> possibleShowNames = GetPossibleShowNameStrings();
            List<ShowItem> addedShows = FinderHelper.FindShows(possibleShowNames,MDoc);

            if (addedShows.Count <= 0)
            {
                return;
            }

            MDoc.Library.AddRange(addedShows);
            MDoc.ShowAddedOrEdited(false,false,false);
            MDoc.ShowAddedOrEdited(true,false,false);

            LOGGER.Info("Added new shows called: {0}", addedShows.Select(s => s.ShowName).ToCsv());

            //add each new show into the shows being scanned
            foreach (ShowItem si in addedShows)
            {
                showList.Add(si);
            }
        }

        [NotNull]
        private IEnumerable<string> GetPossibleShowNameStrings()
        {
            List<string> possibleShowNames = new List<string>();

            foreach (string dirPath in TVSettings.Instance.DownloadFolders.ToArray())
            {
                LOGGER.Info("Parsing {0} for new shows", dirPath);
                if (!Directory.Exists(dirPath))
                {
                    continue;
                }

                try
                {
                    foreach (string filePath in Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories))
                    {
                        if (!File.Exists(filePath))
                        {
                            continue;
                        }

                        FileInfo fi = new FileInfo(filePath);

                        if (fi.IgnoreFile())
                        {
                            continue;
                        }

                        if (!LookForSeries(fi))
                        {
                            possibleShowNames.Add(fi.RemoveExtension() + ".");
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    LOGGER.Warn(ex, $"Could not access files in {dirPath}");
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    LOGGER.Warn(ex, $"Could not access files in {dirPath}");
                }
                catch (System.IO.IOException ex)
                {
                    LOGGER.Warn(ex, $"Could not access files in {dirPath}");
                }
            }

            return possibleShowNames;
        }

        public override bool Active() => TVSettings.Instance.AutoSearchForDownloadedFiles;

        private bool LookForSeries(FileSystemInfo name) => LookForSeries(name, MDoc.Library.Values);

        private static bool LookForSeries(FileSystemInfo test, [NotNull] IEnumerable<ShowItem> shows)
        {
            return shows.Any(si => si.NameMatch(test, TVSettings.Instance.UseFullPathNameToMatchSearchFolders));
        }
    }
}
