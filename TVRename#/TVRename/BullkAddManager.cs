using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public class BulkAddManager
    {
        public BulkAddManager(TVDoc doc)
        {
            this.AddItems = new FolderMonitorEntryList();
            this.mDoc = doc;
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public FolderMonitorEntryList AddItems;
        private TVDoc mDoc;


        public static void GuessShowItem(FolderMonitorEntry ai, ShowLibrary library)
        {
            string showName = GuessShowName(ai,library);

            if (string.IsNullOrEmpty(showName))
                return;

            TheTVDB.Instance.GetLock("GuessShowItem");

            SeriesInfo ser = TheTVDB.Instance.FindSeriesForName(showName);
            if (ser != null)
                ai.TVDBCode = ser.TVDBCode;

            TheTVDB.Instance.Unlock("GuessShowItem");
        }

        private static string GuessShowName(FolderMonitorEntry ai,ShowLibrary library)
        {
            // see if we can guess a season number and show name, too
            // Assume is blah\blah\blah\show\season X
            string showName = ai.Folder;

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
            showName = showName.Substring(showName.LastIndexOf(System.IO.Path.DirectorySeparatorChar.ToString()) + 1);

            return showName;
        }

        private bool HasSeasonFolders(DirectoryInfo di, out string folderName, out DirectoryInfo[] subDirs)
        {
            try
            {
                subDirs = di.GetDirectories();
                // keep in sync with ProcessAddItems, etc.
                foreach (string sw in mDoc.Library.SeasonWords())
                {
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        string regex = "^(?<folderName>" + sw + "\\s*)\\d+$";
                        Match m = Regex.Match(subDir.Name, regex, RegexOptions.IgnoreCase);
                        if (!m.Success) continue;

                        //We have a match!
                        folderName = m.Groups["folderName"].ToString();
                        logger.Info("Assuming {0} contains a show because keyword '{1}' is found in subdirectory {2}", di.FullName, folderName, subDir.FullName);
                        return true;
                    }
                }

            }
            catch (UnauthorizedAccessException uae)
            {
                // e.g. recycle bin, system volume information
                logger.Warn("Could not access {0} (or a subdir), may not be an issue as could be expected e.g. recycle bin, system volume information", di.FullName);
                subDirs = null;
            }


            folderName = null;
            return false;
        }

        public bool CheckFolderForShows(DirectoryInfo di2, bool andGuess, out DirectoryInfo[] subDirs)
        {
            // ..and not already a folder for one of our shows
            string theFolder = di2.FullName.ToLower();
            foreach (ShowItem si in this.mDoc.Library.GetShowItems())
            {
                if (si.AutoAddNewSeasons && !string.IsNullOrEmpty(si.AutoAdd_FolderBase) &&
                    theFolder.IsSubfolderOf(si.AutoAdd_FolderBase))
                {
                    // we're looking at a folder that is a subfolder of an existing show
                    logger.Info("Rejecting {0} as it's already part of {1}.", theFolder, si.ShowName);
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
                            if (theFolder.ToLower() != folder.ToLower())
                                continue;

                            logger.Info("Rejecting {0} as it's already part of {1}:{2}.", theFolder, si.ShowName, folder);
                            subDirs = null;
                            return true;
                        }
                    }
                }
            } // for each showitem


            //We don't have it already
            bool hasSeasonFolders = false;
            try
            {
                hasSeasonFolders = HasSeasonFolders(di2, out string folderName, out DirectoryInfo[] subDirectories);

                subDirs = subDirectories;

                //This is an indication that something is wrong
                if (subDirectories is null) return false;

                bool hasSubFolders = subDirectories?.Length > 0;
                if (!hasSubFolders || hasSeasonFolders)
                {
                    if (TVSettings.Instance.BulkAddCompareNoVideoFolders && !HasFilmFiles(di2)) return false;

                    if (TVSettings.Instance.BulkAddIgnoreRecycleBin && di2.FullName.Contains("$RECYCLE.BIN", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (TVSettings.Instance.BulkAddIgnoreRecycleBin && di2.FullName.Contains("\\@Recycle\\", StringComparison.OrdinalIgnoreCase))
                        return true;



                    // ....its good!
                    FolderMonitorEntry ai = new FolderMonitorEntry(di2.FullName, hasSeasonFolders, folderName);
                    this.AddItems.Add(ai);
                    logger.Info("Adding {0} as a new folder", theFolder);
                    if (andGuess)
                        GuessShowItem(ai,this.mDoc.Library);
                }

            }
            catch (UnauthorizedAccessException)
            {
                logger.Info("Can't access {0}, so ignoring it", di2.FullName);
                subDirs = null;
                return true;
            }

            return hasSeasonFolders;
        }

        private static bool HasFilmFiles(DirectoryInfo directory)
        {
            return directory.GetFiles("*", System.IO.SearchOption.TopDirectoryOnly).Any(file => TVSettings.Instance.UsefulExtension(file.Extension, false));
        }

        public void CheckFolderForShows(DirectoryInfo di, ref bool stop)
        {
            // is it on the ''Bulk Add Shows' ignore list?
            if (TVSettings.Instance.IgnoreFolders.Contains(di.FullName.ToLower()))
            {
                logger.Info("Rejecting {0} as it's on the ignore list.", di.FullName);
                return;
            }

            if (CheckFolderForShows(di, false, out DirectoryInfo[] subDirs))
                return; // done.

            if (subDirs == null) return; //indication we could not access the subdirectory

            // recursively check a folder for new shows

            foreach (DirectoryInfo di2 in subDirs)
            {
                if (stop)
                    return;

                this.CheckFolderForShows(di2, ref stop); // not a season folder.. recurse!
            } // for each directory
        }

        public void AddAllToMyShows()
        {
            foreach (FolderMonitorEntry ai in this.AddItems)
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

                found.AutoAdd_FolderBase = ai.Folder;
                found.AutoAdd_FolderPerSeason = ai.HasSeasonFoldersGuess;

                found.AutoAdd_SeasonFolderName = ai.SeasonFolderName;
                mDoc.Stats().AutoAddedShows++;
            }

            this.mDoc.Library.GenDict();
            this.mDoc.Dirty();
            this.AddItems.Clear();
            this.mDoc.ExportShowInfo();
        }



        public void CheckFolders(ref bool stop, ref int percentDone)
        {
            // Check the  folder list, and build up a new "AddItems" list.
            // guessing what the shows actually are isn't done here.  That is done by
            // calls to "GuessShowItem"
            logger.Info("*********************************************************************");
            logger.Info("*Starting to find folders that contain files, but are not in library*");

            this.AddItems = new FolderMonitorEntryList();

            int c = TVSettings.Instance.LibraryFolders.Count;

            int c2 = 0;
            foreach (string folder in TVSettings.Instance.LibraryFolders)
            {
                percentDone = 100 * c2++ / c;
                DirectoryInfo di = new DirectoryInfo(folder);
                if (!di.Exists)
                    continue;

                this.CheckFolderForShows(di, ref stop);

                if (stop)
                    break;
            }

        }

    }
}
