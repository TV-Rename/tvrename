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
using System.Xml;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    /// <summary>
    /// Handles the logic behind the Bulk Add Function
    /// Works in conjunction with a UI to show outcomes to the user
    /// </summary>
    public class BulkAddSeriesManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public FolderMonitorEntryList AddItems;
        private readonly TVDoc mDoc;

        public BulkAddSeriesManager(TVDoc doc)
        {
            AddItems = new FolderMonitorEntryList();
            mDoc = doc;
        }

        public static void GuessShowItem([NotNull] PossibleNewTvShow ai, [NotNull] ShowLibrary library, bool showErrorMsgBox)
        {
            string languageCode = TVSettings.Instance.DefaultProvider == TVDoc.ProviderType.TMDB
                ? TVSettings.Instance.TMDBLanguage
                : TVSettings.Instance.PreferredLanguageCode;

            string showName = GuessShowName(ai, library);
            //todo - (BulkAdd Manager needs to work for new providers)
            int tvdbId = FindTVDBShowCode(ai);

            if (string.IsNullOrEmpty(showName)  && tvdbId == -1)
            {
                return;
            }

            if (tvdbId != -1)
            {
                try
                {
                    CachedSeriesInfo cachedSeries = TheTVDB.LocalCache.Instance.GetSeriesAndDownload(tvdbId,showErrorMsgBox);
                    if (cachedSeries != null)
                    {
                        ai.SetId(tvdbId,TVDoc.ProviderType.TheTVDB);
                        return;
                    }
                }
                catch (MediaNotFoundException)
                {
                    //continue to try the next method
                }
            }

            CachedSeriesInfo ser = TheTVDB.LocalCache.Instance.GetSeries(showName,showErrorMsgBox, languageCode);
            if (ser != null)
            {
                ai.SetId(tvdbId, TVDoc.ProviderType.TheTVDB);
                return;
            }

            //Try removing any year
            string showNameNoYear =
                Regex.Replace(showName, @"\(\d{4}\)", "").Trim();

            //Remove anything we can from hint to make it cleaner and hence more likely to match
            string refinedHint = FinderHelper.RemoveSeriesEpisodeIndicators(showNameNoYear, library.SeasonWords());

            if (string.IsNullOrWhiteSpace(refinedHint))
            {
                Logger.Info($"Ignoring {showName} as it refines to nothing.");
            }

            ser = TheTVDB.LocalCache.Instance.GetSeries(refinedHint,showErrorMsgBox, languageCode);

            ai.RefinedHint = refinedHint;
            if (ser != null)
            {
                ai.SetId(tvdbId, TVDoc.ProviderType.TheTVDB);
            }
        }

        private static int FindTVDBShowCode(PossibleNewTvShow ai)
        {
            List<string> possibleFilenames = new List<string> {"cachedSeries.xml", "tvshow.nfo"};
            foreach (string fileName in possibleFilenames)
            {
                try
                {
                    IEnumerable<FileInfo> files = ai.Folder.EnumerateFiles(fileName).ToList();
                    if (files.Any())
                    {
                        foreach (int x in files.Select(FindTVDBShowCode).Where(x => x != -1))
                        {
                            return x;
                        }
                    }
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.Message}");
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.Message}");
                }
                catch (NotSupportedException e)
                {
                    Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.Message}");
                }
                catch (System.IO.IOException e)
                {
                    Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.Message}");
                }
            }
            //Can't find it
            return -1;
        }

        private static int FindTVDBShowCode([NotNull] FileInfo file)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(file.OpenText()))
                {
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
            }
            catch (XmlException xe)
            {
                Logger.Warn( $"Could not parse {file.FullName} to try and see whether there is any TVDB Ids inside, got {xe.Message}");
                return -1;
            }
            catch (System.IO.IOException xe)
            {
                Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any TVDB Ids inside, got {xe.Message}");
                return -1;
            }
            catch (UnauthorizedAccessException xe)
            {
                Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any TVDB Ids inside, got {xe.Message}");
                return -1;
            }
            catch (Exception e)
            {
                Logger.Error(e,$"Could not parse {file.FullName} to try and see whether there is any TVDB Ids inside.");
            }

            return -1;
        }

        [NotNull]
        private static string GuessShowName([NotNull] PossibleNewTvShow ai, [NotNull] ShowLibrary library)
        {
            // see if we can guess a season number and show name, too
            // Assume is blah\blah\blah\show\season X
            string showName = ai.Folder.FullName;

            foreach (string seasonWord in library.SeasonWords())
            {
                string seasonFinder = ".*" + seasonWord + "[ _\\.]+([0-9]+).*";
                if (Regex.Matches(showName, seasonFinder, RegexOptions.IgnoreCase).Count == 0)
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
            showName = showName.Substring(showName.LastIndexOf(Path.DirectorySeparatorChar.ToString(),
                                              StringComparison.Ordinal) + 1);

            return showName;
        }

        private bool HasSeasonFolders([NotNull] DirectoryInfo di, out DirectoryInfo[]? subDirs, [NotNull] out string folderFormat)
        {
            try
            {
                subDirs = di.GetDirectories();
                // keep in sync with ProcessAddItems, etc.
                foreach (string sw in mDoc.TvLibrary.SeasonWords())
                {
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        string regex = "^(?<prefix>.*)(?<folderName>" + sw + "\\s*)(?<number>\\d+)$";
                        Match m = Regex.Match(subDir.Name, regex, RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            continue;
                        }

                        //We have a match!
                        folderFormat = m.Groups["prefix"].Value + m.Groups["folderName"] + (m.Groups["number"].ToString().StartsWith("0", StringComparison.Ordinal) ? "{Season:2}" : "{Season}");

                        Logger.Info($"Assuming {di.FullName} contains a show because pattern '{folderFormat}' is found in subdirectory { subDir.FullName}");

                        return true;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // e.g. recycle bin, system volume information
                Logger.Warn($"Could not access {di.FullName} (or a subdir), may not be an issue as could be expected e.g. recycle bin, system volume information");

                subDirs = null;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                // e.g. recycle bin, system volume information
                Logger.Warn($"Could not access {di.FullName} (or a subdir), it is no longer found");

                subDirs = null;
            }
            catch (System.IO.IOException)
            {
                Logger.Warn($"Could not access {di.FullName} (or a subdir), got an IO Exception");

                subDirs = null;
            }
            folderFormat =string.Empty;
            return false;
        }

        public (bool finished, DirectoryInfo[] subDirs) CheckFolderForShows([NotNull] DirectoryInfo di2, bool andGuess,bool  fullLogging, bool showErrorMsgBox)
        {
            try
            {
                // ..and not already a folder for one of our shows
                string theFolder = di2.FullName.ToLower();
                foreach (ShowConfiguration si in mDoc.TvLibrary.GetSortedShowItems())
                {
                    if (RejectFolderIfIncludedInShow(fullLogging, si, theFolder))
                    {
                        return (true,null);
                    }
                } // for each showitem

                //We don't have it already
                bool hasSeasonFolders = HasSeasonFolders(di2, out DirectoryInfo[] subDirectories, out string folderFormat);

                //This is an indication that something is wrong
                if (subDirectories is null)
                {
                    return (false,null);
                }

                bool hasSubFolders = subDirectories.Length > 0;
                if (hasSubFolders && !hasSeasonFolders)
                {
                    return (false, subDirectories);
                }

                if (TVSettings.Instance.BulkAddCompareNoVideoFolders && !HasFilmFiles(di2))
                {
                    return (false, subDirectories);
                }

                if (TVSettings.Instance.BulkAddIgnoreRecycleBin && IsRecycleBin(di2))
                {
                    return (false, subDirectories);
                }

                // ....its good!
                PossibleNewTvShow ai = new PossibleNewTvShow(di2, hasSeasonFolders, folderFormat);

                AddItems.Add(ai);
                Logger.Info("Adding {0} as a new folder", theFolder);
                if (andGuess)
                {
                    GuessShowItem(ai, mDoc.TvLibrary,showErrorMsgBox);
                }
                return (hasSeasonFolders, subDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Info("Can't access {0}, so ignoring it", di2.FullName);
                return (true,null);
            }
        }

        private static bool IsRecycleBin([NotNull] DirectoryInfo di2)
        {
            bool endsWith = di2.FullName.Contains("$RECYCLE.BIN", StringComparison.OrdinalIgnoreCase)
                            || di2.FullName.Contains("\\@Recycle\\", StringComparison.OrdinalIgnoreCase)
                            || di2.FullName.EndsWith("\\@Recycle", StringComparison.OrdinalIgnoreCase);

            return endsWith;
        }

        private static bool RejectFolderIfIncludedInShow(bool fullLogging, [NotNull] ShowConfiguration si,string theFolder)
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
                Dictionary<int, SafeList<string>> afl = si.AllExistngFolderLocations();
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

        private static bool HasFilmFiles([NotNull] DirectoryInfo directory)
        {
            return directory.GetFiles("*", System.IO.SearchOption.TopDirectoryOnly).Any(file => file.IsMovieFile());
        }

        private void CheckFolderForShows([NotNull] DirectoryInfo di, CancellationToken token,bool fullLogging, bool showErrorMsgBox)
        {
            if (!di.Exists)
            {
                return;
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            // is it on the ''Bulk Add Shows' ignore list?
            if (TVSettings.Instance.IgnoreFolders.Contains(di.FullName.ToLower()))
            {
                if (fullLogging)
                {
                    Logger.Info("Rejecting {0} as it's on the ignore list.", di.FullName);
                }

                return;
            }

            (bool finished, DirectoryInfo[]? subDirs) =CheckFolderForShows(di, false, fullLogging, showErrorMsgBox);

            if (finished)
            {
                return; // done.
            }

            if (subDirs is null)
            {
                return; //indication we could not access the sub-directory
            }

            // recursively check a folder for new shows

            foreach (DirectoryInfo di2 in subDirs)
            {
                CheckFolderForShows(di2, token,fullLogging,showErrorMsgBox); // not a season folder.. recurse!
            } // for each directory
        }

        public void AddAllToMyShows()
        {
            foreach (PossibleNewTvShow ai in AddItems.Where(ai=>!ai.CodeUnknown))
            {
                AddToLibrary(ai);
            }

            mDoc.TvLibrary.GenDict();
            mDoc.SetDirty();
            AddItems.Clear();
            mDoc.ExportShowInfo();
        }

        private void AddToLibrary([NotNull] PossibleNewTvShow ai)
        {
            // see if there is a matching show item
            ShowConfiguration found = mDoc.TvLibrary.GetShowItem(ai.ProviderCode, ai.Provider);
            if (found is null)
            {
                // need to add a new showitem
                found = new ShowConfiguration(ai.ProviderCode, ai.Provider);
                mDoc.Add(found);
            }

            found.AutoAddFolderBase = ai.Folder.FullName;

            if (ai.HasSeasonFoldersGuess)
            {
                found.AutoAddType = ai.SeasonFolderFormat == TVSettings.Instance.SeasonFolderFormat
                    ? ShowConfiguration.AutomaticFolderType.libraryDefault
                    : ShowConfiguration.AutomaticFolderType.custom;

                found.AutoAddCustomFolderFormat = ai.SeasonFolderFormat;
            }
            else
            {
                found.AutoAddType = ShowConfiguration.AutomaticFolderType.baseOnly;
            }

            mDoc.Stats().AutoAddedShows++;
        }

        public void CheckFolders(CancellationToken token, [NotNull] SetProgressDelegate prog,bool detailedLogging, bool showErrorMsgBox)
        {
            // Check the  folder list, and build up a new "AddItems" list.
            // guessing what the shows actually are isn't done here.  That is done by
            // calls to "GuessShowItem"
            Logger.Info("*********************************************************************");
            Logger.Info("*Starting to find folders that contain files, but are not in library*");

            AddItems = new FolderMonitorEntryList();

            int c = TVSettings.Instance.LibraryFolders.Count;

            int c2 = 0;
            foreach (string folder in TVSettings.Instance.LibraryFolders)
            {
                prog.Invoke(100 * c2++ / c,folder);
                DirectoryInfo di = new DirectoryInfo(folder);
                if (TVSettings.Instance.MovieLibraryFolders.Contains(folder))
                {
                    Logger.Warn($"Not loading {folder} as it is both a movie folder and a tv folder");
                    continue;
                }

                CheckFolderForShows(di,token, detailedLogging,showErrorMsgBox);

                if (token.IsCancellationRequested)
                {
                    break;
                }
            }
            prog.Invoke(100 , string.Empty);
        }
    }
}
