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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TVRename.Forms;

namespace TVRename;

/// <summary>
/// Handles the logic behind the Bulk Add Function
/// Works in conjunction with a UI to show outcomes to the user
/// </summary>
public class BulkAddSeriesManager(TVDoc doc)
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public FolderMonitorEntryList AddItems = [];
    private readonly TVDoc mDoc = doc;

    public static async Task GuessShowItemAsync(PossibleNewTvShow ai, ShowLibrary library, bool showErrorMsgBox)
    {
        Language languageToUse = TVSettings.Instance.DefaultProvider == TVDoc.ProviderType.TMDB
            ? TVSettings.Instance.TMDBLanguage
            : TVSettings.Instance.PreferredTVDBLanguage;

        Locale localeToUse = new(languageToUse);

        string showName = GuessShowName(ai, library);
        //todo - (BulkAdd Manager needs to work for new providers)
        int tvdbId = FindTvdbShowCode(ai);

        if (string.IsNullOrEmpty(showName) && tvdbId == -1)
        {
            return;
        }

        if (tvdbId != -1)
        {
            try
            {
                ai.UpdateId(tvdbId, TVDoc.ProviderType.TheTVDB);
                CachedSeriesInfo? cachedSeries = await TheTVDB.LocalCache.Instance.GetSeriesOrDownloadAsync(ai, showErrorMsgBox);
                if (cachedSeries != null)
                {
                    return;
                }
            }
            catch (MediaNotFoundException)
            {
                //continue to try the next method
            }
        }

        CachedSeriesInfo? ser = await TheTVDB.LocalCache.Instance.GetSeriesAsync(showName, showErrorMsgBox, localeToUse);
        if (ser != null)
        {
            ai.UpdateId(ser.TvdbId, TVDoc.ProviderType.TheTVDB);
            return;
        }

        //Try removing any year
        string showNameNoYear = showName.RemoveBracketedYear();

        //Remove anything we can from hint to make it cleaner and hence more likely to match
        string refinedHint = FinderHelper.RemoveSeriesEpisodeIndicators(showNameNoYear, library.SeasonWords());

        if (string.IsNullOrWhiteSpace(refinedHint))
        {
            Logger.Info($"Ignoring {showName} as it refines to nothing.");
        }

        ser = await TheTVDB.LocalCache.Instance.GetSeriesAsync(refinedHint, showErrorMsgBox, localeToUse);

        ai.RefinedHint = refinedHint;
        if (ser != null)
        {
            ai.UpdateId(tvdbId, TVDoc.ProviderType.TheTVDB);
        }
    }

    private static int FindTvdbShowCode(PossibleNewTvShow ai)
    {
        List<string> possibleFilenames = ["series.xml", "tvshow.nfo"];
        foreach (string fileName in possibleFilenames)
        {
            try
            {
                IEnumerable<FileInfo> files = ai.FindFiles(fileName);
                if (files.IsAny())
                {
                    foreach (int x in files.Select(FindTvdbShowCode).Where(x => x != -1))
                    {
                        return x;
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (NotSupportedException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (System.IO.IOException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
        }
        //Can't find it
        return -1;
    }

    private static int FindTvdbShowCode(FileInfo file)
    {
        try
        {
            using XmlReader reader = XmlReader.Create(file.OpenText());
            while (reader.Read())
            {
                if (reader.Name == "tvdbid" && reader.IsStartElement())
                {
                    string s = reader.ReadElementContentAsString();
                    bool success = int.TryParse(s, out int x);
                    if (success && x != -1)
                    {
                        return x;
                    }
                }

                if (reader.Name == "uniqueid" && reader.IsStartElement() && reader.GetAttribute("type") == "tvdb")
                {
                    string s = reader.ReadElementContentAsString();
                    bool success = int.TryParse(s, out int x);
                    if (success && x != -1)
                    {
                        return x;
                    }
                }
            }
        }
        catch (XmlException xe)
        {
            Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any TVDB Ids inside, got {xe.ErrorText()}");
            return -1;
        }
        catch (System.IO.IOException xe)
        {
            Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any TVDB Ids inside, got {xe.ErrorText()}");
            return -1;
        }
        catch (UnauthorizedAccessException xe)
        {
            Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any TVDB Ids inside, got {xe.ErrorText()}");
            return -1;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Could not parse {file.FullName} to try and see whether there is any TVDB Ids inside.");
        }

        return -1;
    }

    private static string GuessShowName(PossibleNewTvShow ai, ShowLibrary library)
    {
        // see if we can guess a season number and show name, too
        // Assume is blah\blah\blah\show\season X
        string showName = ai.FolderName;

        foreach (string seasonWord in library.SeasonWords())
        {
            string seasonFinder = ".*" + seasonWord + "[ _\\.]+([0-9]+).*";
            if (Regex.Count(showName, seasonFinder, RegexOptions.IgnoreCase) == 0)
            {
                continue;
            }

            try
            {
                // remove season folder from end of the path
                showName = Regex.Replace(showName, "(.*)\\\\" + seasonFinder, "$1", RegexOptions.IgnoreCase);
                break;
            }
            catch (ArgumentException)
            {
                //Ignore this Exception
            }
        }

        // assume last folder element is the show name
        showName = showName[(showName.LastIndexOf(Path.DirectorySeparatorChar.ToString(),
            StringComparison.Ordinal) + 1)..];

        return showName;
    }

    private async Task<(bool, DirectoryInfo[]?, string)> HasSeasonFoldersAsync(DirectoryInfo di)
    {
        var folderFormat = string.Empty;

        try
        {
            var subDirs = await di.GetDirectoriesAsync();

            // keep in sync with ProcessAddItems, etc.
            foreach (string sw in mDoc.TvLibrary.SeasonWords())
            {
                foreach (DirectoryInfo subDir in subDirs)
                {
                    string regex = "^(?<prefix>.*)(?<folderName>" + sw + "\\s*)(?<number>\\d+)$";
                    try
                    {
                        Match m = Regex.Match(subDir.Name, regex, RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            continue;
                        }
                        //We have a match!
                        folderFormat = m.Groups["prefix"].Value + m.Groups["folderName"] + (m.Groups["number"].ToString().StartsWith('0') ? "{Season:2}" : "{Season}");

                        Logger.Info($"Assuming {di.FullName} contains a show because pattern '{folderFormat}' is found in subdirectory {subDir.FullName}");
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, $"Could not parse {regex} to tell whether {subDir.Name} is a subfolder - ignoring this regex.");
                        continue;
                    }
                    return (true, subDirs, folderFormat);
                }
            }

            return (false, subDirs, string.Empty);
        }
        catch (UnauthorizedAccessException)
        {
            // e.g. recycle bin, system volume information
            Logger.Warn($"Could not access {di.FullName} (or a subdir), may not be an issue as could be expected e.g. recycle bin, system volume information");
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            // e.g. recycle bin, system volume information
            Logger.Warn($"Could not access {di.FullName} (or a subdir), it is no longer found");
        }
        catch (System.IO.IOException)
        {
            Logger.Warn($"Could not access {di.FullName} (or a subdir), got an IO Exception");
        }
        return (false, null, string.Empty);
    }

    public async Task<(bool finished, DirectoryInfo[]? subDirs)> CheckFolderForShowsAsync(DirectoryInfo di2, bool andGuess, bool fullLogging, bool showErrorMsgBox)
    {
        try
        {
            // ..and not already a folder for one of our shows
            string theFolder = di2.FullName.ToLower();
            foreach (ShowConfiguration si in mDoc.TvLibrary.GetSortedShows())
            {
                if (await RejectFolderIfIncludedInShowAsync(fullLogging, si, theFolder))
                {
                    return (true, null);
                }
            } // for each showitem

            //We don't have it already
            (bool hasSeasonFolders, DirectoryInfo[]? subDirectories, string folderFormat) = await HasSeasonFoldersAsync(di2);

            //This is an indication that something is wrong
            if (subDirectories is null)
            {
                return (false, null);
            }

            bool hasSubFolders = subDirectories.Length > 0;
            if (hasSubFolders && !hasSeasonFolders)
            {
                return (false, subDirectories);
            }

            if (TVSettings.Instance.BulkAddCompareNoVideoFolders && !hasSeasonFolders && !HasFilmFiles(di2))
            {
                return (false, subDirectories);
            }

            if (TVSettings.Instance.BulkAddIgnoreRecycleBin && di2.IsRecycleBin())
            {
                return (false, subDirectories);
            }

            // ....its good!
            PossibleNewTvShow ai = new(di2, hasSeasonFolders, folderFormat);

            AddItems.Add(ai);
            Logger.Info($"Adding {theFolder} as a new folder");
            if (andGuess)
            {
                await GuessShowItemAsync(ai, mDoc.TvLibrary, showErrorMsgBox);
            }
            return (hasSeasonFolders, subDirectories);
        }
        catch (UnauthorizedAccessException)
        {
            Logger.Info($"Can't access {di2.FullName}, so ignoring it");
            return (true, null);
        }
    }

    private static async Task<bool> RejectFolderIfIncludedInShowAsync(bool fullLogging, ShowConfiguration si, string theFolder)
    {
        if (si.AutoAddNewSeasons() && !string.IsNullOrEmpty(si.AutoAddFolderBase) &&
            theFolder.IsSubfolderOf(si.AutoAddFolderBase))
        {
            // we're looking at a folder that is a subfolder of an existing show
            if (fullLogging)
            {
                Logger.Info($"Rejecting {theFolder} as it's already part of {si.ShowName}.");
            }

            return true;
        }

        if (si.UsesManualFolders())
        {
            Dictionary<int, SafeList<string>> afl = await si.AllExistngFolderLocationsAsync();
            foreach (KeyValuePair<int, SafeList<string>> kvp in afl)
            {
                foreach (string folder in kvp.Value)
                {
                    if (!string.Equals(theFolder, folder, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    if (fullLogging)
                    {
                        Logger.Info($"Rejecting {theFolder} as it's already part of {si.ShowName}:{folder}.");
                    }

                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasFilmFiles(DirectoryInfo directory)
    {
        return directory.GetFiles("*", System.IO.SearchOption.TopDirectoryOnly).Any(file => file.IsMovieFile());
    }

    private async Task CheckFolderForShowsAsync(IProgress<ScanProgressReport>? handler, DirectoryInfo di, bool fullLogging, bool showErrorMsgBox, CancellationToken token)
    {
        if (!di.Exists)
        {
            return;
        }

        if (token.IsCancellationRequested)
        {
            return;
        }

        // is it on the ''Bulk Add TV Shows' ignore list?
        if (TVSettings.Instance.IgnoreFolders.Contains(di.FullName.ToLower()))
        {
            if (fullLogging)
            {
                Logger.Info($"Rejecting {di.FullName} as it's on the ignore list.");
            }

            return;
        }

        (bool finished, DirectoryInfo[]? subDirs) = await CheckFolderForShowsAsync(di, false, fullLogging, showErrorMsgBox);

        if (finished)
        {
            return; // done.
        }

        if (subDirs is null)
        {
            return; //indication we could not access the sub-directory
        }

        // recursively check a folder for new shows

        ThreadSafeCounter c = new();
        int total = subDirs.Length;
        foreach (DirectoryInfo di2 in subDirs)
        {
            handler?.Report(new ScanProgressReport
            {
                ProgressPercentage = (int)((100 * c.Increment()) / total),
                UpdateText = di2.Name
            });

            await CheckFolderForShowsAsync(handler, di2, fullLogging, showErrorMsgBox, token); // not a season folder.. recurse!
        } // for each directory
    }

    public async Task AddAllToMyShowsAsync(UI ui)
    {
        List<ShowConfiguration> shows = await AddToLibraryAsync(AddItems.Where(ai => ai.CodeKnown));

        await mDoc.TvAddedOrEditedAsync(true, false, false, ui, shows);
        AddItems.Clear();
    }

    private async Task<List<ShowConfiguration>> AddToLibraryAsync(IEnumerable<PossibleNewTvShow> ais)
    {
        List<ShowConfiguration> touchedShows = [];
        foreach (PossibleNewTvShow ai in ais)
        {
            // see if there is a matching show item
            ShowConfiguration? found = mDoc.TvLibrary.GetShowItem(ai);
            if (found is null)
            {
                // need to add a new showitem
                found = new ShowConfiguration(ai.ProviderCode, ai.Provider);
                await mDoc.AddAsync(found.AsList(), true);
            }

            found.AutoAddFolderBase = ai.FolderName;

            if (ai.HasSeasonFoldersGuess)
            {
                found.AutoAddType = ai.SeasonFolderFormat == TVSettings.Instance.SeasonFolderFormat
                    ? ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat
                    : ShowConfiguration.AutomaticFolderType.customFolderFormat;

                found.AutoAddCustomFolderFormat = ai.SeasonFolderFormat;
            }
            else
            {
                found.AutoAddType = ShowConfiguration.AutomaticFolderType.baseOnly;
            }

            touchedShows.Add(found);
            mDoc.Stats().AutoAddedShows++;
        }

        return touchedShows;
    }

    public async Task CheckFoldersAsync(IProgress<ScanProgressReport>? handler, bool detailedLogging, bool showErrorMsgBox, CancellationToken token)
    {
        // Check the  folder list, and build up a new "AddItems" list.
        // guessing what the shows actually are isn't done here.  That is done by
        // calls to "GuessShowItem"
        Logger.Info("*********************************************************************");
        Logger.Info("*Starting to find folders that contain files, but are not in library*");

        AddItems = [];

        int c = TVSettings.Instance.LibraryFolders.Count;

        int c2 = 0;
        foreach (string folder in TVSettings.Instance.LibraryFolders)
        {
            handler?.Report(new ScanProgressReport
            {
                ProgressPercentage = (int)((100 * c2++) / c),
                UpdateText = folder
            });

            DirectoryInfo di = new(folder);
            if (TVSettings.Instance.MovieLibraryFolders.Contains(folder))
            {
                Logger.Warn($"Not loading {folder} as it is both a movie folder and a tv folder");
                continue;
            }

            await CheckFolderForShowsAsync(handler, di, detailedLogging, showErrorMsgBox, token);

            if (token.IsCancellationRequested)
            {
                break;
            }
        }
        handler?.Report(new ScanProgressReport
        {
            ProgressPercentage = 100,
            UpdateText = "Complete"
        });
    }
}
