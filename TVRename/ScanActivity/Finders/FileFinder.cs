//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal abstract class FileFinder : Finder
    {
        protected FileFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
        {
        }

        public override FinderDisplayType DisplayType() => FinderDisplayType.local;

        // ReSharper disable once FunctionComplexityOverflow
        protected bool ReviewFile(ShowItemMissing me, ItemList addTo, FileInfo dce, bool addMergeRules, bool preventMove, bool doExtraFiles, bool useFullPath)
        {
            if (Settings.Token.IsCancellationRequested)
            {
                return false;
            }

            bool matched = false;

            try
            {
                if (dce.IgnoreFile())
                {
                    return false;
                }

                //do any of the possible names for the cachedSeries match the filename?
                matched = me.MissingEpisode.Show.NameMatch(dce, useFullPath);

                if (!matched)
                {
                    return false;
                }

                (bool identifySuccess, int seasF, int epF, int maxEp) = IdentifyFile(me, dce);

                if (!identifySuccess)
                {
                    return false;
                }

                if (maxEp != -1 && addMergeRules)
                {
                    me = UpdateMissingItem(me, dce, epF, maxEp, seasF);
                }

                FileInfo fi = FinderHelper.GenerateTargetName(me, dce);

                if (preventMove)
                {
                    //We do not want to move the file, just rename it
                    fi = new FileInfo(dce.DirectoryName.EnsureEndsWithSeparator() + me.Filename + dce.Extension);
                }

                if (dce.FullName != fi.FullName && !FindExistingActionFor(addTo, dce))
                {
                    // don't remove the base search folders
                    bool doTidyup =
                        !TVSettings.Instance.DownloadFolders.Any(folder =>
                            folder.SameDirectoryLocation(fi.Directory.FullName));

                    addTo.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.copy, dce, fi, me.MissingEpisode, doTidyup, me, MDoc));
                }

                if (doExtraFiles)
                {
                    DownloadIdentifiersController di = new();

                    // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                    addTo.Add(di.ProcessEpisode(me.Episode, fi));
                }

                return true;
            }
            catch (System.IO.PathTooLongException e)
            {
                WarnPathTooLong(me, dce, e, matched);
            }
            return false;
        }

        protected bool ReviewFile(MovieItemMissing me, ItemList addTo, FileInfo dce, bool preventMove, bool doExtraFiles, bool useFullPath)
        {
            if (Settings.Token.IsCancellationRequested)
            {
                return false;
            }

            bool matched = false;

            try
            {
                if (dce.IgnoreFile() || me.Movie is null)
                {
                    return false;
                }

                //do any of the possible names for the cachedSeries match the filename?
                matched = me.Show.NameMatch(dce, useFullPath);

                if (!matched)
                {
                    return false;
                }

                if (me.Show.LengthNameMatch(dce, useFullPath) != MDoc.FilmLibrary.Movies.MaxOrDefault(m => m.LengthNameMatch(dce, useFullPath), 0))
                {
                    return false;
                }

                FileInfo fi = FinderHelper.GenerateTargetName(me, dce);

                if (preventMove)
                {
                    //We do not want to move the file, just rename it
                    fi = new FileInfo(dce.DirectoryName.EnsureEndsWithSeparator() + me.Filename + dce.Extension);
                }

                if (dce.FullName != fi.FullName && !FindExistingActionFor(addTo, dce))
                {
                    // don't remove the base search folders
                    bool doTidyup =
                        !TVSettings.Instance.DownloadFolders.Any(folder =>
                            folder.SameDirectoryLocation(fi.Directory.FullName));

                    addTo.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.copy, dce, fi, me.Movie, doTidyup, me, MDoc));
                }

                if (doExtraFiles)
                {
                    DownloadIdentifiersController di = new();

                    // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                    addTo.Add(di.ProcessMovie(me.MovieConfig, fi));
                }

                return true;
            }
            catch (System.IO.PathTooLongException e)
            {
                WarnPathTooLong(me, dce, e, matched);
            }
            return false;
        }

        private static (bool identifysuccess, int foundSeason, int foundEpisode, int maxEp) IdentifyFile([NotNull] ShowItemMissing me, [NotNull] FileInfo dce)
        {
            int season = me.MissingEpisode.AppropriateSeasonNumber;
            int epnum = me.MissingEpisode.AppropriateEpNum;

            bool fileMatchesFilenameProcessors = FinderHelper.FindSeasEp(dce, out int foundSeason, out int foundEpisode, out int maxEp, me.MissingEpisode.Show);

            if (fileMatchesFilenameProcessors)
            {
                if (foundSeason == season && foundEpisode == epnum)
                {
                    return (true, foundSeason, foundEpisode, maxEp);
                }
                else
                {
                    return (false, 0, 0, 0);
                }
            }

            if (me.MissingEpisode.Show.UseSequentialMatch)
            {
                if (FinderHelper.MatchesSequentialNumber(dce.RemoveExtension(false), me.MissingEpisode))
                {
                    return (true, season, epnum, me.MissingEpisode.EpNum2);
                }
            }

            if (me.MissingEpisode.Show.UseAirDateMatch)
            {
                if (FinderHelper.FindSeasEpDateCheck(dce.Name, out foundSeason, out foundEpisode, me.MissingEpisode.Show))
                {
                    if (foundEpisode == epnum && foundSeason == season)
                    {
                        return (true, foundSeason, foundEpisode, -1);
                    }
                }
            }

            if (me.MissingEpisode.Show.UseEpNameMatch)
            {
                if (FinderHelper.FindSeasEpNameCheck(dce, me.MissingEpisode.Show, out foundSeason, out foundEpisode))
                {
                    if (foundEpisode == epnum && foundSeason == season)
                    {
                        return (true, foundSeason, foundEpisode, -1);
                    }
                }
            }

            return (false, 0, 0, 0);
        }

        private void WarnPathTooLong([NotNull] ShowItemMissing me, [NotNull] FileInfo dce, [NotNull] Exception e, bool matched)
        {
            int season = me.MissingEpisode.AppropriateSeasonNumber;
            int epnum = me.MissingEpisode.AppropriateEpNum;

            string t = "Path too long. " + dce.FullName + ", " + e.Message;
            LOGGER.Error(e, "Path too long. " + dce.FullName);

            t += ".  More information is available in the log file";
            if (!MDoc.Args.Unattended && !MDoc.Args.Hide && Environment.UserInteractive)
            {
                MessageBox.Show(t, "Path too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            t = "DirectoryName " + dce.DirectoryName + ", File name: " + dce.Name;
            t += matched ? ", matched.  " : ", no match.  ";
            if (matched)
            {
                t += "Show: " + me.MissingEpisode.TheCachedSeries.Name + ", Season " + season + ", Ep " + epnum + ".  ";
                t += "To: " + me.TheFileNoExt;
            }

            LOGGER.Warn(t);
        }

        private void WarnPathTooLong([NotNull] MovieItemMissing me, [NotNull] FileInfo dce, [NotNull] Exception e, bool matched)
        {
            string t = "Path too long. " + dce.FullName + ", " + e.Message;
            LOGGER.Error(e, "Path too long. " + dce.FullName);

            t += ".  More information is available in the log file";
            if (!MDoc.Args.Unattended && !MDoc.Args.Hide && Environment.UserInteractive)
            {
                MessageBox.Show(t, "Path too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            t = "DirectoryName " + dce.DirectoryName + ", File name: " + dce.Name;
            t += matched ? ", matched.  " : ", no match.  ";
            if (matched)
            {
                t += "Show: " + me.MovieConfig.ShowName + ".  ";
                t += "To: " + me.TheFileNoExt;
            }

            LOGGER.Warn(t);
        }

        [NotNull]
        private static ShowItemMissing UpdateMissingItem([NotNull] ShowItemMissing me, [NotNull] FileInfo dce, int epF, int maxEp, int seasF)
        {
            ShowRule sr = new()
            {
                DoWhatNow = RuleAction.kMerge,
                First = epF,
                Second = maxEp
            };

            me.MissingEpisode.Show.AddSeasonRule(seasF, sr);

            LOGGER.Info(
                $"Looking at {me.MissingEpisode.Show.ShowName} and have identified that episode {epF} and {maxEp} of season {seasF} have been merged into one file {dce.FullName}");

            LOGGER.Info($"Added new rule automatically for {sr}");

            //Regenerate the episodes with the new rule added
            ShowLibrary.GenerateEpisodeDict(me.MissingEpisode.Show);

            //Get the newly created processed episode we are after
            // ReSharper disable once InconsistentNaming
            ProcessedEpisode newPE = me.MissingEpisode;
            foreach (ProcessedEpisode pe in me.MissingEpisode.Show.SeasonEpisodes[seasF])
            {
                if (pe.AppropriateEpNum == epF && pe.EpNum2 == maxEp)
                {
                    newPE = pe;
                }
            }

            return new ShowItemMissing(newPE, me.TargetFolder);
        }

        private static bool FindExistingActionFor([NotNull] ItemList addTo, [NotNull] FileSystemInfo fi)
        {
            foreach (ActionCopyMoveRename existingFileOperation in addTo.CopyMoveRename)
            {
                if (string.Compare(existingFileOperation.From.FullName, fi.FullName,
                        StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            return false;
        }
        protected void CopySubsFolders([NotNull] ItemList actionlist)
        {
            CopySubsFolders(actionlist,  !MDoc.Args.Unattended && !MDoc.Args.Hide, MDoc);
        }

        public static void CopySubsFolders([NotNull] ItemList actionlist, bool showErrors, TVDoc d)
        {
            // for each of the items in rcl, do the same copy/move if for other items with the same
            // base name, but different extensions
            ItemList extras = new();

            foreach (ActionCopyMoveRename action in actionlist.CopyMoveRename)
            {
                try
                {
                    IEnumerable<DirectoryInfo> suitableSubFolders = action.SourceDirectory.GetDirectories().Where(IsSubsFolder);
                    foreach (DirectoryInfo subtitleFolder in suitableSubFolders)
                    {
                        extras.Add(CopySubFolderForAction(d, action, subtitleFolder));
                    }
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    LOGGER.Warn($"Could not find {action.SourceDirectory}, so not copying any subtitles from it.");
                }
                catch (System.IO.IOException io)
                {
                    LOGGER.Warn(io,$"Could not access {action.SourceDirectory}, so not copying any subtitles from it.");
                }
                catch (UnauthorizedAccessException)
                {
                    LOGGER.Warn($"Could not access {action.SourceDirectory}, so not copying any subtitles from it.");
                }
            }

            UpdateActionList(actionlist, extras);
        }

        [CanBeNull]
        private static Item CopySubFolderForAction(TVDoc doc, [NotNull] ActionCopyMoveRename action, [NotNull] DirectoryInfo subtitleFolder)
        {
            if (action.Episode != null)
            {
                //Does not really make sense for shows (multiple episodes in one directory).
                //If we only have one file we can rename it
                List<FileInfo> subFiles = subtitleFolder.GetFiles().Where(IsSubTitleFile).ToList();
                if (subFiles.Count == 1)
                {
                    FileInfo fi = subFiles.Single();
                    string newName = action.DestinationBaseName + fi.Extension;
                    return new ActionCopyMoveRename(action.Operation, fi, FileHelper.FileInFolder(action.To.Directory, newName),
                        action.SourceEpisode, true, null, doc);
                }
            }
            else
            {
                bool hasSubtitleFiles = subtitleFolder.GetFiles().Any(IsSubTitleFile);

                if (!hasSubtitleFiles)
                {
                    return null;
                }

                string newlocation = Path.Combine(action.DestinationFolder, subtitleFolder.Name);

                if (newlocation == subtitleFolder.FullName)
                {
                    return null;
                }

                return new ActionMoveRenameDirectory(subtitleFolder.FullName, newlocation, action.Movie!);
            }

            return null;
        }

        private static bool IsSubTitleFile([NotNull] FileInfo file)
        {
            return TVSettings.Instance.subtitleExtensionsArray.Contains(file.Extension);
        }

        public static bool IsSubsFolder(DirectoryInfo folder)
        {
            return TVSettings.Instance.SubsFolderNames.Any(x=>string.Equals(x,folder.Name,StringComparison.CurrentCultureIgnoreCase));
        }

        protected void KeepTogether([NotNull] ItemList actionlist, bool fromLibrary)
        {
            KeepTogether(actionlist, fromLibrary, !MDoc.Args.Unattended && !MDoc.Args.Hide, MDoc);
        }

        public static void KeepTogether([NotNull] ItemList actionlist, bool fromLibrary, bool showErrors, TVDoc d)
        {
            // for each of the items in rcl, do the same copy/move if for other items with the same
            // base name, but different extensions
            ItemList extras = new();

            foreach (ActionCopyMoveRename action in actionlist.CopyMoveRename)
            {
                KeepTogetherForItem(actionlist, fromLibrary, action, extras, showErrors, d);
            }

            UpdateActionList(actionlist, extras);
        }

        private static void KeepTogetherForItem(ItemList actionlist, bool fromLibrary, [NotNull] ActionCopyMoveRename action, ItemList extras, bool showErrors, TVDoc d)
        {
            try
            {
                string basename = action.From.RemoveExtension();
                string toname = action.To.RemoveExtension();

                //We have the arbitary >9 limit to make sure that a file with a short name does not drag a whole load of other files with it
                FileInfo[] flist = basename.Length >9
                    ? action.From.Directory.GetFiles(basename + "*.*")
                    : action.From.Directory.GetFiles(basename + ".*");
                foreach (FileInfo fi in flist)
                {
                    //check to see whether the file is one of the types we do/don't want to include
                    //If we are copying from outside the library we use the 'Keep Together' Logic
                    if (!fromLibrary && !TVSettings.Instance.KeepTogetherFilesWithType(fi.Extension))
                    {
                        continue;
                    }

                    //If we are with in the library we use the 'Other Extensions'
                    if (fromLibrary && !TVSettings.Instance.FileHasUsefulExtension(fi, true))
                    {
                        continue;
                    }

                    string newName = GetFilename(fi.Name, basename, toname);

                    ActionCopyMoveRename newitem = action.Episode is null ?
                        new ActionCopyMoveRename(action.Operation, fi,
                            FileHelper.FileInFolder(action.To.Directory, newName), action.Movie!, true,
                            null, d)
                        : new ActionCopyMoveRename(action.Operation, fi,
                        FileHelper.FileInFolder(action.To.Directory, newName), action.SourceEpisode, true,
                        null, d);

                    // check this item isn't already in our to-do list
                    if (ActionListContains(actionlist, newitem))
                    {
                        continue;
                    }

                    if (!newitem.SameAs(action)) // don't re-add ourself
                    {
                        extras.Add(newitem);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                LOGGER.Warn("Could not access: " + action.From.FullName);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                LOGGER.Warn("Could not find: " + action.From.FullName);
            }
            catch (System.IO.PathTooLongException e)
            {
                string t = "Path or filename too long. " + action.From.FullName + ", " + e.Message;
                LOGGER.Warn(e, "Path or filename too long. " + action.From.FullName);

                if (showErrors && Environment.UserInteractive)
                {
                    MessageBox.Show(t, "Path or filename too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (System.IO.IOException ioe)
            {
                LOGGER.Warn($"IOException Occured accessing: {action.From.FullName} nessage:{ioe.Message}");
            }
        }

        [NotNull]
        private static string GetFilename([NotNull] string filename, [NotNull] string basename, string toname)
        {
            // do case insensitive replace
            int p = filename.IndexOf(basename, StringComparison.OrdinalIgnoreCase);

            string newName = filename.Substring(0, p) + toname + filename.Substring(p + basename.Length);
            if (TVSettings.Instance.RenameTxtToSub && newName.EndsWith(".txt", StringComparison.Ordinal))
            {
                return newName.RemoveLast(4) + ".sub";
            }

            return newName;
        }

        private static void UpdateActionList(ItemList actionlist, [NotNull] ItemList extras)
        {
            foreach (Item action in extras)
            {
                // check we don't already have this in our list and, if we don't add it!
                bool have = AlreadyHaveAction(actionlist, action);

                if (!have)
                {
                    actionlist.Insert(0, action); // put before other actions, so tidyup is run last
                }
            }
        }

        protected static void ReorganiseToLeaveOriginals([NotNull] ItemList newList)
        {
            // go through and change last of each operation on a given source file to a 'Move'
            // ideally do that move within same filesystem

            // sort based on source file, and destination drive, putting last if destdrive == sourcedrive
            newList.Sort(new ActionItemSorter());

            // sort puts all the CopyMoveRenames together
            // then set the last of each source file to be a move
            for (int i = 0; i < newList.Count; i++)
            {
                ActionCopyMoveRename cmr1 = newList[i] as ActionCopyMoveRename;
                bool ok1 = cmr1 != null;

                if (!ok1)
                {
                    continue;
                }

                bool last = i == newList.Count - 1;
                ActionCopyMoveRename cmr2 = !last ? newList[i + 1] as ActionCopyMoveRename : null;
                bool ok2 = cmr2 != null;

                if (ok2)
                {
                    ActionCopyMoveRename a1 = cmr1;
                    ActionCopyMoveRename a2 = cmr2;
                    if (!FileHelper.Same(a1.From, a2.From))
                    {
                        a1.Operation = ActionCopyMoveRename.Op.move;
                    }
                }
                else
                {
                    // last item, or last copymoverename item in the list
                    ActionCopyMoveRename a1 = cmr1;
                    a1.Operation = ActionCopyMoveRename.Op.move;
                }
            }
        }

        private static bool AlreadyHaveAction([NotNull] ItemList actionList, Item action)
        {
            foreach (Item action2 in actionList)
            {
                if (action2.SameAs(action))
                {
                    return true;
                }

                if (action is Action a1 && action2 is Action a2 && a2.Produces == a1.Produces)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ActionListContains([NotNull] ItemList actionlist, ActionCopyMoveRename newItem)
        {
            return actionlist.CopyMoveRename.Any(cmAction => cmAction.SameSource(newItem));
        }

        protected void ProcessMissingItem(ItemList newList, ItemList toRemove, ItemMissing me, Dictionary<FileInfo, ItemList> thisRound, [NotNull] List<FileInfo> matchedFiles, bool useFullPath)
        {
            if (matchedFiles.Count == 1)
            {
                if (!OtherActionsMatch(matchedFiles[0], me, useFullPath))
                {
                    if (!FinderHelper.BetterShowsMatch(matchedFiles[0], me.Show, useFullPath, MDoc))
                    {
                        toRemove.Add(me);
                        newList.AddRange(thisRound[matchedFiles[0]]);
                    }
                    else
                    {
                        LOGGER.Warn($"Ignoring potential match for {me}: with file {matchedFiles[0].FullName} as there are multiple shows that match for that file");
                        me.AddComment(
                            $"Ignoring potential match with file {matchedFiles[0].FullName} as there are multiple shows for that file");
                    }
                }
                else
                {
                    LOGGER.Warn($"Ignoring potential match for {me}: with file {matchedFiles[0].FullName} as there are multiple actions for that file");
                    me.AddComment(
                        $"Ignoring potential match with file {matchedFiles[0].FullName} as there are multiple actions for that file");
                }
            }

            if (matchedFiles.Count > 1)
            {
                List<FileInfo> bestMatchedFiles = IdentifyBestMatches(matchedFiles);

                if (bestMatchedFiles.Count == 1)
                {
                    //We have one file that is better, lets use it
                    toRemove.Add(me);
                    newList.AddRange(thisRound[bestMatchedFiles[0]]);
                }
                else
                {
                    foreach (FileInfo matchedFile in matchedFiles)
                    {
                        LOGGER.Warn(
                            $"Ignoring potential match for {me}: with file {matchedFile.FullName} as there are multiple files for that action");

                        me.AddComment(
                            $"Ignoring potential match with file {matchedFile.FullName} as there are multiple files for that action");
                    }
                }
            }
        }

        [NotNull]
        private static List<FileInfo> IdentifyBestMatches([NotNull] List<FileInfo> matchedFiles)
        {
            //See whether there are any of the matched files that stand out
            List<FileInfo> bestMatchedFiles = new();
            foreach (FileInfo matchedFile in matchedFiles)
            {
                //test first file against all the others
                bool betterThanAllTheRest = true;
                foreach (FileInfo compareAgainst in matchedFiles)
                {
                    if (matchedFile.FullName == compareAgainst.FullName)
                    {
                        continue;
                    }

                    if (FileHelper.BetterQualityFile(matchedFile, compareAgainst) !=
                        FileHelper.VideoComparison.firstFileBetter)
                    {
                        betterThanAllTheRest = false;
                    }
                }
                if (betterThanAllTheRest)
                {
                    bestMatchedFiles.Add(matchedFile);
                }
            }

            return bestMatchedFiles;
        }

        private bool OtherActionsMatch(FileInfo matchedFile, Item me, bool useFullPath)
        //This is used to check whether the selected file may match any other files we are looking for
        {
            foreach (ShowItemMissing testMissingAction in ActionList.MissingEpisodes)
            {
                if (testMissingAction.SameAs(me))
                {
                    continue;
                }

                if (ReviewFile(testMissingAction, new ItemList(), matchedFile, false, false, false, useFullPath))
                {
                    //We have 2 options that match  me and testAction - See whether one is subset of the other
                    if (me.Episode != null && me.Episode.Show.ShowName.Contains(testMissingAction.MissingEpisode.Show.ShowName))
                    {
                        continue; //'me' is a better match, so don't worry about the new one
                    }

                    return true;
                }
            }
            return false;
        }
    }
}
