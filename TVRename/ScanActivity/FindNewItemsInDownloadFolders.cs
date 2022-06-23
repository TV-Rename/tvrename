//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class FindNewItemsInDownloadFolders : ScanActivity
{
    public FindNewItemsInDownloadFolders(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
    }

    protected override string CheckName() => "Looked in the Search Folders for any new series/movies that need to be added to the library";

    protected override void DoCheck(SetProgressDelegate progress)
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
        if (Settings.Unattended || Settings.Hidden)
        {
            LOGGER.Info("Not looking for new media as app is unattended");
            return;
        }

        IEnumerable<FileInfo> possibleShowNames = GetPossibleFiles();
        IEnumerable<MediaConfiguration> addedShows = FinderHelper.FindMedia(possibleShowNames, MDoc, Settings.Owner);
        List<MediaConfiguration> addedShowsUnique = RemoveExistingAndDups(addedShows);

        List<ShowConfiguration> addedTvShows = addedShowsUnique.OfType<ShowConfiguration>().Distinct().ToList();
        if (addedTvShows.Any())
        {
            MDoc.Add(addedTvShows, true);
            MDoc.TvAddedOrEdited(true, false, false, Settings.Owner, addedTvShows);
            //add each new show into the shows being scanned
            Settings.Shows.AddRange(addedTvShows);
            LOGGER.Info($"Added new shows called: {addedTvShows.Select(s => s.ShowName).ToCsv()}" );
        }

        List<MovieConfiguration> addedMovies = addedShowsUnique.OfType<MovieConfiguration>().Distinct().ToList();
        if (addedMovies.Any())
        {
            MDoc.Add(addedMovies, true);
            MDoc.MoviesAddedOrEdited(true, false, false, Settings.Owner, addedMovies);
            Settings.Movies.AddRange(addedMovies);
            LOGGER.Info($"Added new movies called: {addedMovies.Select(s => s.ShowName).ToCsv()}");
        }
    }

    private List<MediaConfiguration> RemoveExistingAndDups(IEnumerable<MediaConfiguration> addedShows)
    {
        List<MediaConfiguration> returnList = new();
        foreach (MediaConfiguration testMedia in addedShows)
        {
            if (MDoc.AlreadyContains(testMedia))
            {
                continue;
            }
            if (TVDoc.ContainsMedia(returnList, testMedia))
            {
                continue;
            }
            returnList.Add(testMedia);
        }

        return returnList;
    }

    private IEnumerable<FileInfo> GetPossibleFiles()
    {
        List<FileInfo> possibleShowNames = new();

        foreach (string dirPath in TVSettings.Instance.DownloadFolders.ToArray())
        {
            LOGGER.Info($"Parsing {dirPath} for new shows" );
            if (!Directory.Exists(dirPath))
            {
                continue;
            }

            try
            {
                string[] array = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                IOrderedEnumerable<string> orderedFiles = array.OrderBy(name => name);
                foreach (string filePath in orderedFiles)
                {
                    if (!File.Exists(filePath))
                    {
                        continue;
                    }

                    FileInfo fi = new(filePath);

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
