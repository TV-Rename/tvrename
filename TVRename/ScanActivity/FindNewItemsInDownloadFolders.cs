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
    internal class FindNewItemsInDownloadFolders : ScanActivity
    {
        public FindNewItemsInDownloadFolders(TVDoc doc) : base(doc)
        {
        }
        
        protected override string CheckName() => "Looked in the Search Folders for any new series/movies that need to be added to the library";

        protected override void DoCheck(SetProgressDelegate prog,
            TVDoc.ScanSettings settings)
        {
            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //does show match selected file?
            //if so add cachedSeries to list of cachedSeries scanned
            if (!Active())
            {
                LOGGER.Info("Not looking for new media as 'Auto-Add' is turned off");
                return;
            }

            //Don't support unattended mode
            if (settings.Unattended || settings.Hidden)
            {
                LOGGER.Info("Not looking for new media as app is unattended");
                return;
            }

            IEnumerable<FileInfo> possibleShowNames = GetPossibleShowNameStrings();
            List<MediaConfiguration> addedShows = FinderHelper.FindMedia(possibleShowNames,MDoc,settings.Owner);

            IEnumerable<ShowConfiguration> addedTvShows = addedShows.OfType<ShowConfiguration>();
            if (addedTvShows.Any())
            {
                MDoc.TvLibrary.AddRange(addedTvShows);
                MDoc.ShowAddedOrEdited(false, false, false, settings.Owner);
                MDoc.ShowAddedOrEdited(true, false, false, settings.Owner);
                //add each new show into the shows being scanned
                foreach (ShowConfiguration si in addedTvShows)
                {
                    settings.Shows.Add(si);
                }
                LOGGER.Info("Added new shows called: {0}", addedTvShows.Select(s => s.ShowName).ToCsv());
            }


            IEnumerable<MovieConfiguration> addedMovies = addedShows.OfType<MovieConfiguration>();
            if (addedMovies.Any())
            {
                MDoc.FilmLibrary.AddRange(addedMovies);
                MDoc.MovieAddedOrEdited(false, false, false, settings.Owner);
                MDoc.MovieAddedOrEdited(true, false, false, settings.Owner);

                foreach (MovieConfiguration si in addedMovies)
                {
                    settings.Movies.Add(si);
                }
                LOGGER.Info("Added new movies called: {0}", addedMovies.Select(s => s.ShowName).ToCsv());
            }
        }

        [NotNull]
        private IEnumerable<FileInfo> GetPossibleShowNameStrings()
        {
            List<FileInfo> possibleShowNames = new List<FileInfo>();

            foreach (string dirPath in TVSettings.Instance.DownloadFolders.ToArray())
            {
                LOGGER.Info("Parsing {0} for new shows", dirPath);
                if (!Directory.Exists(dirPath))
                {
                    continue;
                }

                try
                {
                    string[] array = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                    IOrderedEnumerable<string>? orderedFiles = from name in array orderby name select name;
                    foreach (string filePath in orderedFiles)
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

                        possibleShowNames.Add(fi);
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
    }
}
