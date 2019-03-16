using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    class CheckAllFoldersExist : ScanShowActivity
    {
        public CheckAllFoldersExist(TVDoc doc) : base(doc) {}

        [NotNull]
        protected override string Checkname() => "Checked All Folders Exist";

        protected override void Check([NotNull] ShowItem si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (!si.DoMissingCheck && !si.DoRename)
            {
                return; // skip
            }

            Dictionary<int, List<string>> flocs = si.AllProposedFolderLocations();

            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
            foreach (int snum in numbers)
            {
                // show MissingFolderAction for any folders that are missing
                // throw Exception if user cancels

                if (si.IgnoreSeasons.Contains(snum))
                {
                    continue; // ignore this season
                }

                if ((snum == 0) && (si.CountSpecials))
                {
                    continue; // no specials season, they're merged into the seasons themselves
                }

                if ((snum == 0) && TVSettings.Instance.IgnoreAllSpecials)
                {
                    continue;
                }

                List<string> folders = new List<string>();

                if (flocs.ContainsKey(snum))
                {
                    folders = flocs[snum];
                }

                if (folders.Count == 0 && si.AutoAddNewSeasons())
                {
                    // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                    folders.Add(si.AutoFolderNameForSeason(snum));
                }

                if ((folders.Count == 0) && (!si.AutoAddNewSeasons()))
                {
                    // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                    folders.Add(string.Empty);
                }

                CreateSeasonFolders(si, snum, folders);
            } // for each snum
        }

        private void CreateSeasonFolders(ShowItem si, int snum, [NotNull] List<string> folders)
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

                    if (di != null && di.Exists)
                    {
                        continue;
                    }

                    string sn = si.ShowName;
                    string text = snum + " of " + si.MaxSeason();
                    string theFolder = folder;
                    string otherFolder = null;

                    FaResult whatToDo = GetDefaultAction();

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
                        throw new TVRenameOperationInterruptedException();
                    }
                    else if (whatToDo == FaResult.kfaCreate)
                    {
                        TryCreateDirectory(folder, sn, text);
                        goAgain = true;
                    }
                    else if (whatToDo == FaResult.kfaIgnoreAlways)
                    {
                        si.IgnoreSeasons.Add(snum);
                        Doc.SetDirty();
                        break;
                    }
                    else if (whatToDo == FaResult.kfaIgnoreOnce)
                    {
                        break;
                    }
                    else if (whatToDo == FaResult.kfaRetry)
                    {
                        goAgain = true;
                    }
                    else if (whatToDo == FaResult.kfaDifferentFolder)
                    {
                        folder = otherFolder;
                        goAgain = UpdateDirectory(si, snum, folder);
                    }
                } while (goAgain);
            } // for each folder
        }

        private  bool  UpdateDirectory(ShowItem si, int snum, string folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            bool goAgain = !di.Exists;
            if (di.Exists &&
                (!string.Equals(si.AutoFolderNameForSeason(snum), folder, StringComparison.CurrentCultureIgnoreCase)))
            {
                if (!si.ManualFolderLocations.ContainsKey(snum))
                {
                    si.ManualFolderLocations[snum] = new List<string>();
                }

                si.ManualFolderLocations[snum].Add(folder);
                Doc.SetDirty();
            }

            return goAgain;
        }

        private static void TryCreateDirectory([CanBeNull] string folder, string sn, string text)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                LOGGER.Warn($"Folder is not specified for {sn} {text}");
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
                    LOGGER.Warn($"Could not create directory: {folder} Error Message: {ioe.Message}");
                }
            }
        }

        private FaResult GetDefaultAction()
        {
            switch (Doc.Args.MissingFolder)
            {
                case CommandLineArgs.MissingFolderBehavior.create:
                    return FaResult.kfaCreate;
                case CommandLineArgs.MissingFolderBehavior.ignore:
                    return FaResult.kfaIgnoreOnce;
                case CommandLineArgs.MissingFolderBehavior.ask:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Doc.Args.Hide || !Environment.UserInteractive)
            {
                return FaResult.kfaIgnoreOnce; // default in /hide mode is to ignore
            }

            return FaResult.kfaNotSet;
        }

        protected override bool Active() => TVSettings.Instance.MissingCheck;
    }
}
