// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using System.Linq;

namespace TVRename
{
    internal class FileFinder:Finder
    {
        public FileFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchLocally;

        public override FinderDisplayType DisplayType() => FinderDisplayType.local;

        public override void Check(SetProgressDelegate prog, int startpct, int totPct, ICollection<ShowItem> showList,
            TVDoc.ScanSettings settings)
        {
            prog.Invoke(startpct, "Starting searching through files");

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();

            int fileCount = CountFilesInDownloadDirs();

            DirCache dirCache = new DirCache();
            foreach (string s in TVSettings.Instance.DownloadFolders)
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                dirCache.AddFolder(prog, 0, fileCount, s, true);
            }

            int currentItem = 0;
            int totalN = ActionList.Count;
            foreach (ItemMissing me in ActionList.MissingItems())
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                prog.Invoke(startpct + ((totPct - startpct) * (++currentItem) / (totalN + 1)), me.Filename);

                ItemList thisRound = new ItemList();
                List<DirCacheEntry> matchedFiles = FindMatchedFiles(settings, dirCache, me, thisRound);

                ProcessMissingItem(settings, newList, toRemove, me, thisRound, matchedFiles);
            }

            if (TVSettings.Instance.KeepTogether)
                KeepTogether(newList);

            prog.Invoke(totPct, string.Empty);

            if (!TVSettings.Instance.LeaveOriginals)
            {
                ReorganiseToLeaveOriginals(newList);
            }

            foreach (Item i in toRemove)
                ActionList.Remove(i);

            foreach (Item i in newList)
                ActionList.Add(i);
        }

        private static void ReorganiseToLeaveOriginals(ItemList newList)
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
                    continue;

                bool last = i == (newList.Count - 1);
                ActionCopyMoveRename cmr2 = !last ? newList[i + 1] as ActionCopyMoveRename : null;
                bool ok2 = cmr2 != null;

                if (ok2)
                {
                    ActionCopyMoveRename a1 = cmr1;
                    ActionCopyMoveRename a2 = cmr2;
                    if (!FileHelper.Same(a1.From, a2.From))
                        a1.Operation = ActionCopyMoveRename.Op.move;
                }
                else
                {
                    // last item, or last copymoverename item in the list
                    ActionCopyMoveRename a1 = cmr1;
                    a1.Operation = ActionCopyMoveRename.Op.move;
                }
            }
        }

        private List<DirCacheEntry> FindMatchedFiles(TVDoc.ScanSettings settings, DirCache dirCache, ItemMissing me, ItemList thisRound)
        {
            List<DirCacheEntry> matchedFiles = new List<DirCacheEntry>();

            foreach (DirCacheEntry dce in dirCache)
            {
                if (!ReviewFile(me, thisRound, dce, settings)) continue;

                matchedFiles.Add(dce);
            }

            return matchedFiles;
        }

        private void ProcessMissingItem(TVDoc.ScanSettings settings, ItemList newList, ItemList toRemove, ItemMissing me, ItemList thisRound, List<DirCacheEntry> matchedFiles)
        {
            if (matchedFiles.Count == 1)
            {
                if (!OtherActionsMatch(matchedFiles[0], me, settings))
                {
                    toRemove.Add(me);
                    newList.AddRange(thisRound);
                }
                else
                {
                    LOGGER.Warn($"Ignoring potential match for {me.Episode.Show.ShowName} S{me.Episode.AppropriateSeasonNumber} E{me.Episode.AppropriateEpNum}: with file {matchedFiles[0]?.TheFile.FullName} as there are multiple actions for that file");
                    me.AddComment(
                        $"Ignoring potential match with file {matchedFiles[0]?.TheFile.FullName} as there are multiple actions for that file");
                }
            }
            else if (matchedFiles.Count > 1)
            {
                List<DirCacheEntry> bestMatchedFiles = IdentifyBestMatches(matchedFiles);

                if (bestMatchedFiles.Count == 1)
                {
                    //We have one file that is better, lets use it
                    toRemove.Add(me);
                    newList.AddRange(thisRound);
                }
                else
                {
                    foreach (DirCacheEntry matchedFile in matchedFiles)
                    {
                        LOGGER.Warn(
                            $"Ignoring potential match for {me.Episode.Show.ShowName} S{me.Episode.AppropriateSeasonNumber} E{me.Episode.AppropriateEpNum}: with file {matchedFile?.TheFile.FullName} as there are multiple files for that action");

                        me.AddComment(
                            $"Ignoring potential match with file {matchedFile?.TheFile.FullName} as there are multiple files for that action");
                    }
                }
            }
        }

        private static int CountFilesInDownloadDirs()
        {
            int fileCount = 0;
            foreach (string s in TVSettings.Instance.DownloadFolders)
                fileCount += DirCache.CountFiles(s, true);
            return fileCount;
        }

        private static List<DirCacheEntry> IdentifyBestMatches(List<DirCacheEntry> matchedFiles)
        {
            //See whether there are any of the matched files that stand out
            List<DirCacheEntry> bestMatchedFiles = new List<DirCacheEntry>();
            foreach (DirCacheEntry matchedFile in matchedFiles)
            {
                //test first file against all the others
                bool betterThanAllTheRest = true;
                foreach (DirCacheEntry compareAgainst in matchedFiles)
                {
                    if (matchedFile.TheFile.FullName == compareAgainst.TheFile.FullName) continue;
                    if (FileHelper.BetterQualityFile(matchedFile.TheFile, compareAgainst.TheFile) !=
                        FileHelper.VideoComparison.FirstFileBetter)
                    {
                        betterThanAllTheRest = false;
                    }
                }
                if (betterThanAllTheRest) bestMatchedFiles.Add(matchedFile);
            }

            return bestMatchedFiles;
        }

        private bool OtherActionsMatch(DirCacheEntry matchedFile, Item me,TVDoc.ScanSettings settings)
        //This is used to check whether the selected file may match any other files we are looking for
        {
            foreach (ItemMissing testMissingAction in ActionList.MissingItems())
            {
                if (testMissingAction.SameAs(me)) continue;

                if (ReviewFile(testMissingAction, new ItemList(), matchedFile,settings))
                {
                    //We have 2 options that match  me and testAction - See whether one is subset of the other
                    if (me.Episode.Show.ShowName.Contains(testMissingAction.Episode.Show.ShowName)) continue; //'me' is a better match, so don't worry about the new one

                    return true; 
                }
            }
            return false;
        }

        private void KeepTogether(ItemList actionlist)
        {
            // for each of the items in rcl, do the same copy/move if for other items with the same
            // base name, but different extensions
           ItemList extras = new ItemList();

            foreach (Item action1 in actionlist)
            {
                if (!(action1 is ActionCopyMoveRename))
                    continue;

                ActionCopyMoveRename action = (ActionCopyMoveRename)(action1);

                try
                {
                    DirectoryInfo sfdi = action.From.Directory;
                    string basename = action.From.Name;
                    int l = basename.Length;
                    basename = basename.Substring(0, l - action.From.Extension.Length);

                    string toname = action.To.Name;
                    int l2 = toname.Length;
                    toname = toname.Substring(0, l2 - action.To.Extension.Length);

                    FileInfo[] flist = sfdi.GetFiles(basename + ".*");
                    foreach (FileInfo fi in flist)
                    {
                        //check to see whether the file is one of the types we do/don't want to include
                        if (!TVSettings.Instance.KeepTogetherFilesWithType(fi.Extension)) continue;

                        // do case insensitive replace
                        string n = fi.Name;
                        int p = n.IndexOf(basename, StringComparison.OrdinalIgnoreCase);
                        string newName = n.Substring(0, p) + toname + n.Substring(p + basename.Length);
                        if ((TVSettings.Instance.RenameTxtToSub) && (newName.EndsWith(".txt")))
                            newName = newName.Substring(0, newName.Length - 4) + ".sub";

                        ActionCopyMoveRename newitem = new ActionCopyMoveRename(action.Operation, fi, FileHelper.FileInFolder(action.To.Directory, newName), action.Episode, false, null); // tidyup on main action, not this

                        // check this item isn't already in our to-do list
                        if (ActionListContains(actionlist, newitem)) continue;

                        if (!newitem.SameAs(action)) // don't re-add ourself
                            extras.Add(newitem);
                    }
                }
                catch (System.IO.PathTooLongException e)
                {
                    string t = "Path or filename too long. " + action.From.FullName + ", " + e.Message;
                    LOGGER.Warn(e, "Path or filename too long. " + action.From.FullName);

                    if ((!MDoc.Args.Unattended) && (!MDoc.Args.Hide)) MessageBox.Show(t, "Path or filename too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            foreach (Item action in extras)
            {
                // check we don't already have this in our list and, if we don't add it!
                bool have = AlreadyHaveAction(actionlist, action);

                if (!have)
                    actionlist.Insert(0, action); // put before other actions, so tidyup is run last
            }
        }

        private static bool AlreadyHaveAction(ItemList actionlist, Item action)
        {
            foreach (Item action2 in actionlist)
            {
                if (action2.SameAs(action))
                {
                    return true;
                }

                if ((action is Action a1) && (action2 is Action a2))
                {
                    if (a2.Produces == a1.Produces)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool ActionListContains(ItemList actionlist, ActionCopyMoveRename newitem)
        {
            return actionlist.CopyMoveItems().Any(cmAction => cmAction.SameSource(newitem));
        }

        private bool ReviewFile(ItemMissing me, ItemList addTo, DirCacheEntry dce,TVDoc.ScanSettings settings)
        {
            if (settings.Token.IsCancellationRequested) return true;
            
            int season = me.Episode.AppropriateSeasonNumber;
            int epnum = me.Episode.AppropriateEpNum;
            bool matched = false;

            try
            {
                if (dce.TheFile.IgnoreFile()) return false;

                //do any of the possible names for the series match the filename?
                matched = me.Episode.Show.NameMatch(dce);

                if (!matched) return false;

                bool regularMatch = FinderHelper.FindSeasEp(dce.TheFile, out int seasF, out int epF, out int maxEp, me.Episode.Show) &&
                         seasF == season &&
                         epF == epnum;

                bool sequentialMatch = me.Episode.Show.UseSequentialMatch &&
                          TVDoc.MatchesSequentialNumber(dce.TheFile.Name, ref seasF, ref epF, me.Episode) &&
                          seasF == season &&
                          epF == epnum;

                if (!regularMatch && !sequentialMatch) return false;

                if (maxEp != -1 && TVSettings.Instance.AutoMergeDownloadEpisodes)
                {
                    ShowRule sr = new ShowRule
                    {
                        DoWhatNow = RuleAction.kMerge,
                        First = epF,
                        Second = maxEp
                    };
                    me.Episode.Show?.AddSeasonRule(seasF, sr);

                    LOGGER.Info(
                        $"Looking at {me.Episode.Show.ShowName} and have identified that episode {epF} and {maxEp} of season {seasF} have been merged into one file {dce.TheFile.FullName}");
                    LOGGER.Info($"Added new rule automatically for {sr}");

                    //Regenerate the episodes with the new rule added
                    ShowLibrary.GenerateEpisodeDict(me.Episode.Show);

                    //Get the newly created processed episode we are after
                    // ReSharper disable once InconsistentNaming
                    ProcessedEpisode newPE = me.Episode;
                    foreach (ProcessedEpisode pe in me.Episode.Show.SeasonEpisodes[seasF])
                    {
                        if (pe.AppropriateEpNum == epF && pe.EpNum2 == maxEp) newPE = pe;
                    }

                    me = new ItemMissing(newPE, me.TargetFolder,
                        TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(newPE)));
                }
                FileInfo fi = new FileInfo(me.TheFileNoExt + dce.TheFile.Extension);

                if (TVSettings.Instance.PreventMove)
                {
                    //We do not want to move the file, just rename it
                    fi = new FileInfo(dce.TheFile.DirectoryName + System.IO.Path.DirectorySeparatorChar + me.Filename +
                                      dce.TheFile.Extension);
                }

                // don't remove the base search folders
                bool doTidyup = !TVSettings.Instance.DownloadFolders.Any(folder => folder.SameDirectoryLocation(fi.Directory.FullName));

                if (dce.TheFile.FullName != fi.FullName)
                    addTo.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.copy,  dce.TheFile, fi, me.Episode,doTidyup, me));

                DownloadIdentifiersController di = new DownloadIdentifiersController();

                // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                addTo.Add(di.ProcessEpisode(me.Episode, fi));

                return true;
            }
            catch (System.IO.PathTooLongException e)
            {
                string t = "Path too long. " + dce.TheFile.FullName + ", " + e.Message;
                LOGGER.Warn(e, "Path too long. " + dce.TheFile.FullName);

                t += ".  More information is available in the log file";
                if ((!MDoc.Args.Unattended) && (!MDoc.Args.Hide))
                    MessageBox.Show(t, "Path too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                t = "DirectoryName " + dce.TheFile.DirectoryName + ", File name: " + dce.TheFile.Name;
                t += matched ? ", matched.  " : ", no match.  ";
                if (matched)
                {
                    t += "Show: " + me.Episode.TheSeries.Name + ", Season " + season + ", Ep " + epnum + ".  ";
                    t += "To: " + me.TheFileNoExt;
                }
                LOGGER.Warn(t);
            }
            return false;
        }
    }
}
