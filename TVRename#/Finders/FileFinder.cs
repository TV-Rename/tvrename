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

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public override bool Active() => TVSettings.Instance.SearchLocally;

        public override FinderDisplayType DisplayType() => FinderDisplayType.Local;

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            prog.Invoke(startpct);

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();

            int fileCount = 0;
            foreach (string s in this.Doc.SearchFolders)
                fileCount += DirCache.CountFiles(s, true);

            int c = 0;

            DirCache dirCache = new DirCache();
            foreach (string s in this.Doc.SearchFolders)
            {
                if (this.ActionCancel)
                    return;

                dirCache.AddFolder(prog, c, fileCount, s, true);
            }

            int currentItem = 0;
            int totalN = this.ActionList.Count;
            foreach (Item action1 in this.ActionList)
            {
                if (this.ActionCancel)
                    return;

                prog.Invoke(startpct + (totPct-startpct) * (++currentItem) / (totalN + 1));

                if (!(action1 is ItemMissing me)) continue;

                int numberMatched = 0;
                ItemList thisRound = new ItemList();
                DirCacheEntry matchedFile=null;

                foreach (DirCacheEntry dce in dirCache)
                {
                    if (ReviewFile(me, thisRound, dce)) {numberMatched++;
                        matchedFile = dce;
                    }
                }

                if ((numberMatched == 1) &&(!OtherActionsMatch(matchedFile,me,this.ActionList)))
                {
                    toRemove.Add(action1);
                    newList.AddRange(thisRound);
                }
            }

            if (TVSettings.Instance.KeepTogether)
                KeepTogether(newList);

            prog.Invoke(totPct);

            if (!TVSettings.Instance.LeaveOriginals)
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
                            a1.Operation = ActionCopyMoveRename.Op.Move;
                    }
                    else
                    {
                        // last item, or last copymoverename item in the list
                        ActionCopyMoveRename a1 = cmr1;
                        a1.Operation = ActionCopyMoveRename.Op.Move;
                    }
                }
            }

            foreach (Item i in toRemove)
                this.ActionList.Remove(i);

            foreach (Item i in newList)
                this.ActionList.Add(i);
        }

        private bool OtherActionsMatch(DirCacheEntry matchedFile, ItemMissing me, ItemList actionList)
        //This is used to check whether the selected file may match any other files we are looking for
        {
            foreach (Item testAction in actionList)
            {
                if (!(testAction is ItemMissing testMissingAction)) continue;
                if (testMissingAction.SameAs(me)) continue;

                if (ReviewFile(testMissingAction, new ItemList(), matchedFile)) return true;

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

                        ActionCopyMoveRename newitem = new ActionCopyMoveRename(action.Operation, fi, FileHelper.FileInFolder(action.To.Directory, newName), action.Episode, null,null); // tidyup on main action, not this

                        

                        // check this item isn't already in our to-do list
                        bool doNotAdd = false;
                        foreach (Item ai2 in actionlist)
                        {
                            if (!(ai2 is ActionCopyMoveRename))
                                continue;

                            if (((ActionCopyMoveRename)(ai2)).SameSource(newitem))
                            {
                                doNotAdd = true;
                                break;
                            }
                        }

                        if (!doNotAdd)
                        {
                            if (!newitem.SameAs(action)) // don't re-add ourself
                                extras.Add(newitem);
                        }
                    }
                }
                catch (System.IO.PathTooLongException e)
                {
                    string t = "Path or filename too long. " + action.From.FullName + ", " + e.Message;
                    logger.Warn(e, "Path or filename too long. " + action.From.FullName);

                    if ((!this.Doc.Args.Unattended) && (!this.Doc.Args.Hide)) MessageBox.Show(t, "Path or filename too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    
                }
            }

            foreach (Item action in extras)
            {
                // check we don't already have this in our list and, if we don't add it!
                bool have = false;
                foreach (Item action2 in actionlist)
                {
                    if (action2.SameAs(action))
                    {
                        have = true;
                        break;
                    }

                    if ((action is Action a1) && (action2 is Action) )
                    {
                        Action a2 = (Action)action2;
                        if (a2.Produces == a1.Produces)
                        {
                            have = true;
                            break;
                        }
                    }
                }

                if (!have)
                    actionlist.Insert(0, action); // put before other actions, so tidyup is run last
            }
        }
        // consider each of the files, see if it is suitable for series "ser" and episode "epi"
        // if so, add a rcitem for copy to "fi"


        private bool ReviewFile(ItemMissing me, ItemList addTo, DirCacheEntry dce)
        {
            int season = me.Episode.AppropriateSeasonNumber;

            int epnum = me.Episode.AppropriateEpNum;

            if (this.ActionCancel)
                return true;

            bool matched = false;

            try
            {
                if (!dce.HasUsefulExtension_NotOthersToo) // not a useful file extension
                    return false;
                if (TVSettings.Instance.IgnoreSamples && dce.LowerName.Contains("sample") &&
                    ((dce.Length / (1024 * 1024)) < TVSettings.Instance.SampleFileMaxSizeMB))
                    return false;

                //do any of the possible names for the series match the filename?
                matched = (me.Episode.SI.getSimplifiedPossibleShowNames()
                    .Any(name => FileHelper.SimplifyAndCheckFilename(dce.SimplifiedFullName, name)));

                if (matched)
                {
                    if (dce.LowerName.StartsWith("-.") && (dce.Length / 1024 < 10))
                    {
                        logger.Info($"Ignoring matched file {dce.TheFile.FullName} as it looks like a Mac Temp File");
                        return false;
                    }

                    if ((TVDoc.FindSeasEp(dce.TheFile, out int seasF, out int epF, out int maxEp, me.Episode.SI) && (seasF == season) &&
                         (epF == epnum)) ||
                        (me.Episode.SI.UseSequentialMatch &&
                         TVDoc.MatchesSequentialNumber(dce.TheFile.Name, ref seasF, ref epF, me.Episode) && (seasF == season) &&
                         (epF == epnum)))
                    {
                        if (maxEp != -1 && TVSettings.Instance.AutoMergeEpisodes)
                        {
                            ShowRule sr = new ShowRule
                            {
                                DoWhatNow = RuleAction.kMerge,
                                First = epF,
                                Second = maxEp
                            };
                            if (!me.Episode.SI.SeasonRules.ContainsKey(seasF)) me.Episode.SI.SeasonRules[seasF] = new List<ShowRule>();

                            me.Episode.SI.SeasonRules[seasF].Add(sr);
                            logger.Info(
                                $"Looking at {me.Episode.SI.ShowName} and have identified that episode {epF} and {maxEp} of season {seasF} have been merged into one file {dce.TheFile.FullName}");
                            logger.Info($"Added new rule automatically for {sr}");

                            //Regenerate the episodes with the new rule added
                            this.Doc.GenerateEpisodeDict(me.Episode.SI);

                            //Get the newly created processed episode we are after
                            List<ProcessedEpisode> newSeason = this.Doc.GetShowItem(me.Episode.SI.TVDBCode).SeasonEpisodes[seasF];
                            // ReSharper disable once InconsistentNaming
                            ProcessedEpisode newPE = me.Episode;

                            foreach (ProcessedEpisode pe in newSeason)
                            {
                                if (pe.AppropriateEpNum == epF && pe.EpNum2 == maxEp) newPE = pe;
                            }


                            me = new ItemMissing(newPE, me.TargetFolder,
                                TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameForExt(newPE)));
                        }

                        FileInfo fi = new FileInfo(me.TheFileNoExt + dce.TheFile.Extension);


                        if (TVSettings.Instance.PreventMove)
                        {
                            //We do not want to move the file, just rename it
                            fi = new FileInfo(dce.TheFile.DirectoryName + System.IO.Path.DirectorySeparatorChar + me.Filename +
                                              dce.TheFile.Extension);
                        }

                        // don't remove the base search folders
                        bool doTidyup = true;
                        foreach (string folder in this.Doc.SearchFolders)
                        {
                            if (folder.SameDirectoryLocation(fi.Directory.FullName))


                            {
                                doTidyup = false;
                                break;
                            }
                        }

                        if (dce.TheFile.FullName != fi.FullName)
                            addTo.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.Copy,  dce.TheFile, fi, me.Episode,
                                doTidyup ? TVSettings.Instance.Tidyup : null, me));

                        DownloadIdentifiersController di = new DownloadIdentifiersController();

                        // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                        addTo.Add(di.ProcessEpisode(me.Episode, fi));

                        return true;
                    }
                }
            }
            catch (System.IO.PathTooLongException e)
            {
                string t = "Path too long. " + dce.TheFile.FullName + ", " + e.Message;
                logger.Warn(e, "Path too long. " + dce.TheFile.FullName);

                t += ".  More information is available in the log file";
                if ((!this.Doc.Args.Unattended) && (!this.Doc.Args.Hide))
                    MessageBox.Show(t, "Path too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                t = "DirectoryName " + dce.TheFile.DirectoryName + ", File name: " + dce.TheFile.Name;
                t += matched ? ", matched.  " : ", no match.  ";
                if (matched)
                {
                    t += "Show: " + me.Episode.TheSeries.Name + ", Season " + season + ", Ep " + epnum + ".  ";
                    t += "To: " + me.TheFileNoExt;
                }

                logger.Warn(t);
            }

            return false;
        }
    }
}


