using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class FindNewMoviesInDownloadFolders : ScanActivity
    {
        public FindNewMoviesInDownloadFolders(TVDoc doc) : base(doc)
        {
        }

        protected override string CheckName() => "Looked in the Search Folders for any new movies that need to be added to the library";

        protected override void DoCheck(SetProgressDelegate prog, TVDoc.ScanSettings settings)
        {
            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //does show match selected file?
            //if so add cachedSeries to list of cachedSeries scanned
            if (!Active())
            {
                LOGGER.Info("Not looking for new movies as 'Auto-Add' is turned off");
                return;
            }

            //Don't support unattended mode
            if (settings.Unattended || settings.Hidden)
            {
                LOGGER.Info("Not looking for new movies as app is unattended");
                return;
            }

            IEnumerable<string> possibleShowNames = GetPossibleShowNameStrings();
            List<MovieConfiguration> addedShows = FinderHelper.FindMovies(possibleShowNames, MDoc, settings.Owner);

            if (addedShows.Count <= 0)
            {
                return;
            }

            MDoc.FilmLibrary.AddRange(addedShows);
            MDoc.MovieAddedOrEdited(false, false, false, settings.Owner);
            MDoc.MovieAddedOrEdited(true, false, false, settings.Owner);

            LOGGER.Info("Added new shows called: {0}", addedShows.Select(s => s.ShowName).ToCsv());

            //add each new show into the shows being scanned
            foreach (MovieConfiguration si in addedShows)
            {
                settings.Movies.Add(si);
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
                    IEnumerable<string> filePaths = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                    foreach (string filePath in filePaths)
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

        public override bool Active() => TVSettings.Instance.AutoSearchForDownloadedFiles; //todo - split this into a separate setting

        private bool LookForSeries(FileSystemInfo name) => LookForSeries(name, MDoc.FilmLibrary.Values);

        private static bool LookForSeries(FileSystemInfo test, [NotNull] IEnumerable<MovieConfiguration> shows)
        {
            return shows.Any(si => si.NameMatch(test, TVSettings.Instance.UseFullPathNameToMatchSearchFolders));
        }
    }
}
