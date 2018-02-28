using System;
using System.Windows.Forms;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using System.Linq;

namespace TVRename
{
    class FileFinder:Finder
    {
        public FileFinder(TVDoc i) : base(i) { }

        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public override bool Active()
        {
            return TVSettings.Instance.SearchLocally;
        }

        public override Finder.FinderDisplayType DisplayType()
        {
            return FinderDisplayType.Local;
        }

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            prog.Invoke(0);

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();

            int fileCount = 0;
            foreach (string s in this.mDoc.SearchFolders)
                fileCount += DirCache.CountFiles(s, true);

            int c = 0;

            DirCache dirCache = new DirCache();
            foreach (String s in this.mDoc.SearchFolders)
            {
                if (this.ActionCancel)
                    return;

                c = dirCache.AddFolder(prog, c, fileCount, s, true);
            }

            c = 0;
            int totalN = this.TheActionList.Count;
            foreach (Item action1 in this.TheActionList)
            {
                if (this.ActionCancel)
                    return;

                prog.Invoke(50 + 50 * (++c) / (totalN + 1)); // second 50% of progress bar

                if (action1 is ItemMissing)
                {
                    if (this.FindMissingEp(dirCache, (ItemMissing)(action1), newList, ActionCopyMoveRename.Op.Copy))
                        toRemove.Add(action1);
                }
            }

            if (TVSettings.Instance.KeepTogether)
                this.KeepTogether(newList);

            prog.Invoke(100);

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
                this.TheActionList.Remove(i);

            foreach (Item i in newList)
                this.TheActionList.Add(i);

            //                 if (Settings->ExportFOXML)
            //				ExportFOXML(Settings->ExportFOXMLTo);

        }

        public void KeepTogether(ItemList Actionlist)
        {
            // for each of the items in rcl, do the same copy/move if for other items with the same
            // base name, but different extensions

            ItemList extras = new ItemList();

            foreach (Item Action1 in Actionlist)
            {
                if (!(Action1 is ActionCopyMoveRename))
                    continue;

                ActionCopyMoveRename Action = (ActionCopyMoveRename)(Action1);

                try
                {
                    DirectoryInfo sfdi = Action.From.Directory;
                    string basename = Action.From.Name;
                    int l = basename.Length;
                    basename = basename.Substring(0, l - Action.From.Extension.Length);

                    string toname = Action.To.Name;
                    int l2 = toname.Length;
                    toname = toname.Substring(0, l2 - Action.To.Extension.Length);

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

                        ActionCopyMoveRename newitem = new ActionCopyMoveRename(Action.Operation, fi, FileHelper.FileInFolder(Action.To.Directory, newName), Action.Episode, null); // tidyup on main action, not this

                        

                        // check this item isn't already in our to-do list
                        bool doNotAdd = false;
                        foreach (Item ai2 in Actionlist)
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
                            if (!newitem.SameAs(Action)) // don't re-add ourself
                                extras.Add(newitem);
                        }
                    }
                }
                catch (System.IO.PathTooLongException e)
                {
                    string t = "Path or filename too long. " + Action.From.FullName + ", " + e.Message;
                    logger.Warn(e, "Path or filename too long. " + Action.From.FullName);

                    if ((!this.mDoc.Args.Unattended) && (!this.mDoc.Args.Hide)) MessageBox.Show(t, "Path or filename too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    
                }
            }

            foreach (Item action in extras)
            {
                // check we don't already have this in our list and, if we don't add it!
                bool have = false;
                foreach (Item action2 in Actionlist)
                {
                    if (action2.SameAs(action))
                    {
                        have = true;
                        break;
                    }

                    if ((action is Action) && (action2 is Action) )
                    {
                        Action a1 = (Action)action;
                        Action a2 = (Action)action2;
                        if (a2.produces == a1.produces)
                        {
                            have = true;
                            break;
                        }
                    }
                }

                if (!have)
                    Actionlist.Insert(0, action); // put before other actions, so tidyup is run last
            }
        }
        // consider each of the files, see if it is suitable for series "ser" and episode "epi"
        // if so, add a rcitem for copy to "fi"
        public bool FindMissingEp(DirCache dirCache, ItemMissing me, ItemList addTo, ActionCopyMoveRename.Op whichOp)
        {
            int season = me.Episode.SeasonNumber;

            //String ^toName = FilenameFriendly(Settings->NamingStyle->NameFor(me->PE));
            int epnum = me.Episode.EpNum;

            // TODO: find a 'best match', or use first ?

            foreach (DirCacheEntry dce in dirCache)
            {
                if (this.ActionCancel)
                    return true;

                bool matched = false;

                try
                {
                    if (!dce.HasUsefulExtension_NotOthersToo) // not a usefile file extension
                        continue;
                    if (TVSettings.Instance.IgnoreSamples && dce.LowerName.Contains("sample") && ((dce.Length / (1024 * 1024)) < TVSettings.Instance.SampleFileMaxSizeMB))
                        continue;

                    //do any of the possible names for the series match the filename?
                    matched = (me.Episode.SI.getSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(dce.SimplifiedFullName,name)));

                    if (matched)
                    {
                        if ((TVDoc.FindSeasEp(dce.TheFile, out int seasF, out int epF, me.Episode.SI) && (seasF == season) && (epF == epnum)) || (me.Episode.SI.UseSequentialMatch && TVDoc.MatchesSequentialNumber(dce.TheFile.Name, ref seasF, ref epF, me.Episode) && (seasF == season) && (epF == epnum)))
                        {
                            FileInfo fi = new FileInfo(me.TheFileNoExt + dce.TheFile.Extension);

                            // don't remove the base search folders
                            bool doTidyup = true;
                            foreach (String folder in this.mDoc.SearchFolders)
                            {
                                // http://stackoverflow.com/questions/1794025/how-to-check-whether-2-directoryinfo-objects-are-pointing-to-the-same-directory
                                if (String.Compare(folder.ToLower().TrimEnd('\\'), fi.Directory.FullName.ToLower().TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    doTidyup = false;
                                    break;
                                }
                            }
                            addTo.Add(new ActionCopyMoveRename(whichOp, dce.TheFile, fi, me.Episode, doTidyup ? TVSettings.Instance.Tidyup : null));

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
                    if ((!this.mDoc.Args.Unattended) && (!this.mDoc.Args.Hide)) MessageBox.Show(t, "Path too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    t = "DirectoryName " + dce.TheFile.DirectoryName + ", File name: " + dce.TheFile.Name;
                    t += matched ? ", matched.  " : ", no match.  ";
                    if (matched)
                    {
                        t += "Show: " + me.Episode.TheSeries.Name + ", Season " + season + ", Ep " + epnum + ".  ";
                        t += "To: " + me.TheFileNoExt;
                    }
                    logger.Warn(t);
                }
            }

            return false;
        }

    
    }
}


