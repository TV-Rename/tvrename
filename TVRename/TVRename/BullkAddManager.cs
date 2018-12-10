using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    /// <summary>
    /// Handles the logic behind the Bulk Add Function
    /// Works in conjunction with a UI to show outcomes to the user
    /// </summary>
    public class BulkAddManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public FolderMonitorEntryList AddItems;
        private readonly TVDoc mDoc;

        public BulkAddManager(TVDoc doc)
        {
            AddItems = new FolderMonitorEntryList();
            mDoc = doc;
        }

        public static void GuessShowItem(FoundFolder ai, ShowLibrary library)
        {
            string showName = GuessShowName(ai, library);

            int tvdbId = FindShowCode(ai);

            if (string.IsNullOrEmpty(showName)  && tvdbId == -1)
                return;

            TheTVDB.Instance.GetLock("GuessShowItem");

            if (tvdbId != -1)
            {
                TheTVDB.Instance.Unlock("GuessShowItem");
                SeriesInfo series = TheTVDB.Instance.GetSeriesAndDownload(tvdbId);
                if (series != null)
                {
                    ai.TVDBCode = tvdbId;
                    return;
                }
            }

            SeriesInfo ser = TheTVDB.Instance.GetSeries(showName);
            if (ser != null)
            {
                ai.TVDBCode = ser.TvdbCode;
                TheTVDB.Instance.Unlock("GuessShowItem");
                return;
            }

            //Try removing any year
            string showNameNoYear = showName == null ? string.Empty:Regex.Replace(showName, @"\(\d{4}\)", "").Trim();
            ser = TheTVDB.Instance.GetSeries(showNameNoYear);
            if (ser != null)
                ai.TVDBCode = ser.TvdbCode;

            TheTVDB.Instance.Unlock("GuessShowItem");
        }

        private static int FindShowCode(FoundFolder ai)
        {
            List<string> possibleFilenames = new List<string> {"series.xml", "tvshow.nfo"};
            foreach (string fileName in possibleFilenames)
            {
                IEnumerable<FileInfo> files = ai.Folder.EnumerateFiles(fileName).ToList();
                if (files.Any())
                {
                    foreach (FileInfo file in files)
                    {
                        int x = FindShowCode(file);
                        if (x != -1) return x;
                    }
                }
            }
            //Can't find it
            return -1;
        }

        private static int FindShowCode(FileInfo file)
        {
            using (XmlReader reader = XmlReader.Create(file.OpenText()))
            {
                while (reader.Read())
                {
                    if ((reader.Name == "tvdbid") && reader.IsStartElement())
                    {
                        int x = reader.ReadElementContentAsInt();
                        if (x != -1) return x;
                    }
                }
            }
            return -1;
        }

        private static string GuessShowName(FoundFolder ai, ShowLibrary library)
        {
            // see if we can guess a season number and show name, too
            // Assume is blah\blah\blah\show\season X
            string showName = ai.Folder.FullName;

            foreach (string seasonWord in library.SeasonWords())
            {
                string seasonFinder = ".*" + seasonWord + "[ _\\.]+([0-9]+).*";
                if (Regex.Matches(showName, seasonFinder, RegexOptions.IgnoreCase).Count == 0)
                    continue;

                try
                {
                    // remove season folder from end of the path
                    showName = Regex.Replace(showName, "(.*)\\\\" + seasonFinder, "$1", RegexOptions.IgnoreCase);
                    break;
                }
                catch (ArgumentException)
                {
                }
            }

            // assume last folder element is the show name
            showName = showName.Substring(showName.LastIndexOf(System.IO.Path.DirectorySeparatorChar.ToString(),
                                              StringComparison.Ordinal) + 1);

            return showName;
        }

        private bool HasSeasonFolders(DirectoryInfo di, out DirectoryInfo[] subDirs, out string folderFormat)
        {
            try
            {
                subDirs = di.GetDirectories();
                // keep in sync with ProcessAddItems, etc.
                foreach (string sw in mDoc.Library.SeasonWords())
                {
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        string regex = "^(?<folderName>" + sw + "\\s*)(?<number>\\d+)$";
                        Match m = Regex.Match(subDir.Name, regex, RegexOptions.IgnoreCase);
                        if (!m.Success) continue;

                        //We have a match!
                        folderFormat = m.Groups["folderName"] + (m.Groups["number"].ToString().StartsWith("0") ? "{Season:2}" : "{Season}");

                        Logger.Info("Assuming {0} contains a show because pattern '{1}' is found in subdirectory {2}",
                            di.FullName, folderFormat, subDir.FullName);

                        return true;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // e.g. recycle bin, system volume information
                Logger.Warn(
                    "Could not access {0} (or a subdir), may not be an issue as could be expected e.g. recycle bin, system volume information",
                    di.FullName);

                subDirs = null;
            }
            folderFormat=string.Empty;
            return false;
        }

        public bool CheckFolderForShows(DirectoryInfo di2, bool andGuess, out DirectoryInfo[] subDirs,bool  fullLogging )
        {
            // ..and not already a folder for one of our shows
            string theFolder = di2.FullName.ToLower();
            foreach (ShowItem si in mDoc.Library.GetShowItems())
            {
                if (si.AutoAddNewSeasons() && !string.IsNullOrEmpty(si.AutoAddFolderBase) &&
                    theFolder.IsSubfolderOf(si.AutoAddFolderBase))
                {
                    // we're looking at a folder that is a subfolder of an existing show
                    if (fullLogging) Logger.Info("Rejecting {0} as it's already part of {1}.", theFolder, si.ShowName);
                    subDirs = null;
                    return true;
                }

                if (si.UsesManualFolders())
                {
                    Dictionary<int, List<string>> afl = si.AllFolderLocations();
                    foreach (KeyValuePair<int, List<string>> kvp in afl)
                    {
                        foreach (string folder in kvp.Value)
                        {
                            if (!string.Equals(theFolder, folder, StringComparison.CurrentCultureIgnoreCase))
                                continue;

                            if (fullLogging) Logger.Info("Rejecting {0} as it's already part of {1}:{2}.", theFolder, si.ShowName,
                                folder);

                            subDirs = null;
                            return true;
                        }
                    }
                }
            } // for each showitem

            //We don't have it already
            bool hasSeasonFolders;
            try
            {
                hasSeasonFolders = HasSeasonFolders(di2, out DirectoryInfo[] subDirectories, out string folderFormat);

                subDirs = subDirectories;

                //This is an indication that something is wrong
                if (subDirectories is null) return false;

                bool hasSubFolders = subDirectories.Length > 0;
                if (!hasSubFolders || hasSeasonFolders)
                {
                    if (TVSettings.Instance.BulkAddCompareNoVideoFolders && !HasFilmFiles(di2)) return false;

                    if (TVSettings.Instance.BulkAddIgnoreRecycleBin &&
                        di2.FullName.Contains("$RECYCLE.BIN", StringComparison.OrdinalIgnoreCase))
                        return true;

                    if (TVSettings.Instance.BulkAddIgnoreRecycleBin &&
                        di2.FullName.Contains("\\@Recycle\\", StringComparison.OrdinalIgnoreCase))
                        return true;

                    // ....its good!
                    FoundFolder ai =
                        new FoundFolder(di2, hasSeasonFolders, folderFormat);

                    AddItems.Add(ai);
                    Logger.Info("Adding {0} as a new folder", theFolder);
                    if (andGuess)
                        GuessShowItem(ai, mDoc.Library);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Info("Can't access {0}, so ignoring it", di2.FullName);
                subDirs = null;
                return true;
            }

            return hasSeasonFolders;
        }

        private static bool HasFilmFiles(DirectoryInfo directory)
        {
            return directory.GetFiles("*", System.IO.SearchOption.TopDirectoryOnly).Any(file => file.IsMovieFile());
        }

        private void CheckFolderForShows(DirectoryInfo di, CancellationToken token,bool fullLogging)
        {
            if (!di.Exists)
                return;

            if (token.IsCancellationRequested)
                return;

            // is it on the ''Bulk Add Shows' ignore list?
            if (TVSettings.Instance.IgnoreFolders.Contains(di.FullName.ToLower()))
            {
                if (fullLogging) Logger.Info("Rejecting {0} as it's on the ignore list.", di.FullName);
                return;
            }

            if (CheckFolderForShows(di, false, out DirectoryInfo[] subDirs,fullLogging))
                return; // done.

            if (subDirs == null) return; //indication we could not access the subdirectory

            // recursively check a folder for new shows

            foreach (DirectoryInfo di2 in subDirs)
            {
                CheckFolderForShows(di2, token,fullLogging); // not a season folder.. recurse!
            } // for each directory
        }

        public void AddAllToMyShows()
        {
            foreach (FoundFolder ai in AddItems)
            {
                if (ai.CodeUnknown)
                    continue; // skip

                // see if there is a matching show item
                ShowItem found = mDoc.Library.ShowItem(ai.TVDBCode);
                if (found == null)
                {
                    // need to add a new showitem
                    found = new ShowItem(ai.TVDBCode);
                    mDoc.Library.Add(found);
                }

                found.AutoAddFolderBase = ai.Folder.FullName;

                if (ai.HasSeasonFoldersGuess)
                {
                    found.AutoAddType = (ai.SeasonFolderFormat == TVSettings.Instance.SeasonFolderFormat)
                        ? ShowItem.AutomaticFolderType.libraryDefault
                        : ShowItem.AutomaticFolderType.custom;

                    found.AutoAddCustomFolderFormat = ai.SeasonFolderFormat;
                }
                else
                {
                    found.AutoAddType = ShowItem.AutomaticFolderType.baseOnly;
                }
                mDoc.Stats().AutoAddedShows++;
            }

            mDoc.Library.GenDict();
            mDoc.Dirty();
            AddItems.Clear();
            mDoc.ExportShowInfo();
        }

        public void CheckFolders(CancellationToken token, SetProgressDelegate prog,bool detailedLogging)
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

                CheckFolderForShows(di,token, detailedLogging);

                if (token.IsCancellationRequested)
                    break;
            }
            prog.Invoke(100 , string.Empty);
        }
    }
}
