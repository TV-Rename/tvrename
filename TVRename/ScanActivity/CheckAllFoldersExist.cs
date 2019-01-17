using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class CheckAllFoldersExist : ScanShowActivity
    {
        public CheckAllFoldersExist(TVDoc doc) : base(doc) {}

        protected override void Check(ShowItem si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (!si.DoMissingCheck && !si.DoRename)
                return; // skip

            Dictionary<int, List<string>> flocs = si.AllFolderLocations();

            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
            foreach (int snum in numbers)
            {
                // show MissingFolderAction for any folders that are missing
                // throw Exception if user cancels

                if (si.IgnoreSeasons.Contains(snum))
                    return; // ignore this season

                if ((snum == 0) && (si.CountSpecials))
                    return; // no specials season, they're merged into the seasons themselves

                List<string> folders = new List<string>();

                if (flocs.ContainsKey(snum))
                    folders = flocs[snum];

                if ((folders.Count == 0) && (!si.AutoAddNewSeasons()))
                    return; // no folders defined or found, autoadd off, so onto the next

                if (folders.Count == 0)
                {
                    // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                    folders.Add(si.AutoFolderNameForSeason(snum));
                }

                CreateSeasonFolders(si, snum, folders);
            } // for each snum
        }

        private void CreateSeasonFolders(ShowItem si, int snum, List<string> folders)
        {
            foreach (string folderExists in folders)
            {
                string folder = folderExists;

                // generate new filename info
                // ReSharper disable once RedundantAssignment
                bool goAgain = false;
                DirectoryInfo di = null;
                bool firstAttempt = true;
                do
                {
                    goAgain = false;
                    if (!string.IsNullOrEmpty(folder))
                    {
                        try
                        {
                            di = new DirectoryInfo(folder);
                        }
                        catch
                        {
                            break;
                        }
                    }

                    if (di != null && di.Exists) continue;

                    string sn = si.ShowName;
                    string text = snum + " of " + si.MaxSeason();
                    string theFolder = folder;
                    string otherFolder = null;

                    FaResult whatToDo = FaResult.kfaNotSet;

                    if (Doc.Args.MissingFolder == CommandLineArgs.MissingFolderBehavior.create)
                        whatToDo = FaResult.kfaCreate;
                    else if (Doc.Args.MissingFolder == CommandLineArgs.MissingFolderBehavior.ignore)
                        whatToDo = FaResult.kfaIgnoreOnce;

                    if (Doc.Args.Hide && (whatToDo == FaResult.kfaNotSet))
                        whatToDo = FaResult.kfaIgnoreOnce; // default in /hide mode is to ignore

                    if (TVSettings.Instance.AutoCreateFolders && firstAttempt)
                    {
                        whatToDo = FaResult.kfaCreate;
                        firstAttempt = false;
                    }

                    if (whatToDo == FaResult.kfaNotSet)
                    {
                        // no command line guidance, so ask the user
                        MissingFolderAction mfa = new MissingFolderAction(sn, text, theFolder);
                        mfa.ShowDialog();
                        whatToDo = mfa.Result;
                        otherFolder = mfa.FolderName;
                    }

                    if (whatToDo == FaResult.kfaCancel)
                    {
                        throw new TVRenameOperationInteruptedException();
                    }
                    else if (whatToDo == FaResult.kfaCreate)
                    {
                        if (string.IsNullOrWhiteSpace(folder))
                        {
                            LOGGER.Warn($"Folder is not specified for {sn} {text}");
                            goAgain = true;
                        }
                        else
                        {
                            try
                            {
                                LOGGER.Info("Creating directory as it is missing: {0}", folder);
                                Directory.CreateDirectory(folder);
                            }
                            catch (Exception ioe)
                            {
                                LOGGER.Info("Could not directory: {0}", folder);
                                LOGGER.Info(ioe);
                            }

                            goAgain = true;
                        }
                    }
                    else if (whatToDo == FaResult.kfaIgnoreAlways)
                    {
                        si.IgnoreSeasons.Add(snum);
                        Doc.SetDirty();
                        break;
                    }
                    else if (whatToDo == FaResult.kfaIgnoreOnce)
                        break;
                    else if (whatToDo == FaResult.kfaRetry)
                        goAgain = true;
                    else if (whatToDo == FaResult.kfaDifferentFolder)
                    {
                        folder = otherFolder;
                        di = new DirectoryInfo(folder);
                        goAgain = !di.Exists;
                        if (di.Exists && (!string.Equals(si.AutoFolderNameForSeason(snum), folder, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            if (!si.ManualFolderLocations.ContainsKey(snum))
                                si.ManualFolderLocations[snum] = new List<string>();

                            si.ManualFolderLocations[snum].Add(folder);
                            Doc.SetDirty();
                        }
                    }
                } while (goAgain);
            } // for each folder
        }

        protected override bool Active() => TVSettings.Instance.MissingCheck;
    }
}
