using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class CheckAllFoldersExist : ScanShowActivity
{
    public CheckAllFoldersExist(TVDoc doc) : base(doc)
    {
    }

    protected override string ActivityName() => "Checked All Folders Exist";

    protected override void Check(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
    {
        if (!si.DoMissingCheck && !si.DoRename)
        {
            return; // skip
        }

        Dictionary<int, SafeList<string>> flocs = si.AllProposedFolderLocations();

        List<string> ignoredLocations = [];

        foreach (int snum in si.GetSeasonKeys())
        {
            // show MissingFolderAction for any folders that are missing
            // throw Exception if user cancels

            if (si.IgnoreSeasons.Contains(snum))
            {
                continue; // ignore this season
            }

            if (snum == 0 && si.CountSpecials)
            {
                continue; // no specials season, they're merged into the seasons themselves
            }

            if (snum == 0 && TVSettings.Instance.IgnoreAllSpecials)
            {
                continue;
            }

            SafeList<string> folders = [];

            if (flocs.TryGetValue(snum, out SafeList<string>? floc))
            {
                folders = floc;
            }

            if (si.SeasonEpisodes[snum].All(episode => !MightWeProcess(episode, folders)))
            {
                //All episodes in this season are ignored
                continue;
            }

            if (folders.Count == 0 && si.AutoAddNewSeasons())
            {
                // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                folders.Add(si.AutoFolderNameForSeason(snum));
            }

            if (folders.Count == 0 && !si.AutoAddNewSeasons())
            {
                // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                folders.Add(string.Empty);
            }

            CreateSeasonFolders(si, snum, folders, ignoredLocations, settings.Owner);
        } // for each snum
    }

    private static bool MightWeProcess(ProcessedEpisode episode, IEnumerable<string> folders)
    {
        foreach (string folder in folders)
        {
            if (TVSettings.Instance.Ignore.Any(ii => ii.MatchesEpisode(folder, episode)))
            {
                return false;
            }
        }

        if (TVSettings.Instance.IgnorePreviouslySeen)
        {
            if (TVSettings.Instance.PreviouslySeenEpisodes.Includes(episode))
            {
                return false;
            }
        }

        if (!episode.Show.ForceCheckNoAirdate && episode.GetAirDateDt() == null)
        {
            return false;
        }

        return true;
    }

    private void CreateSeasonFolders(ShowConfiguration si, int snum, IEnumerable<string> folders,
        ICollection<string> ignoredLocations, IDialogParent owner)
    {
        foreach (string folderExists in folders)
        {
            CreateSeasonFolder(si, snum, ignoredLocations, folderExists, owner);
        } // for each folder
    }

    private void CreateSeasonFolder(ShowConfiguration si, int snum, ICollection<string> ignoredLocations, string proposedFolderName, IDialogParent owner)
    {
        string folder = proposedFolderName;
        DirectoryInfo? di = null;
        bool firstAttempt = true;

        // generate new filename info
        // ReSharper disable once RedundantAssignment
        bool goAgain;
        do
        {
            goAgain = false;
            if (!string.IsNullOrEmpty(folder))
            {
                try
                {
                    di = new DirectoryInfo(folder);
                }
                catch (Exception e)
                {
                    LOGGER.Warn($"Could not create Season Folder {folder} as {e.ErrorText()}.");
                    break;
                }
            }

            if (ignoredLocations.Contains(folder))
            {
                break;
            }

            if (di != null && di.Exists)
            {
                continue;
            }

            string? otherFolder = null;

            FaResult whatToDo = GetDefaultAction();

            if (TVSettings.Instance.AutoCreateFolders && firstAttempt)
            {
                whatToDo = FaResult.kfaCreate;
                firstAttempt = false;
            }

            if (whatToDo == FaResult.kfaNotSet)
            {
                // no command line guidance, so ask the user
                using MissingFolderAction mfa = new(si.ShowName, snum + " of " + si.MaxSeason(), folder);

                owner.ShowChildDialog(mfa);
                whatToDo = mfa.Result;
                otherFolder = mfa.FolderName;
            }

            switch (whatToDo)
            {
                case FaResult.kfaRetry:
                    goAgain = true;
                    break;

                case FaResult.kfaDifferentFolder:
                    if (otherFolder != null)
                    {
                        folder = otherFolder;
                    }
                    goAgain = UpdateDirectory(si, snum, folder);
                    break;

                case FaResult.kfaNotSet:
                    break;

                case FaResult.kfaCancel:
                    throw new TVRenameOperationInterruptedException();

                case FaResult.kfaCreate:
                    TryCreateDirectory(folder, si.ShowName, snum + " of " + si.MaxSeason());
                    goAgain = true;
                    break;

                case FaResult.kfaIgnoreOnce:
                    ignoredLocations.Add(folder);
                    break;

                case FaResult.kfaIgnoreAlways:
                    si.IgnoreSeasons.Add(snum);
                    Doc.SetDirty();
                    break;

                default:
                    throw new NotSupportedException($"whatToDo = {whatToDo} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}");
            }
        } while (goAgain);
    }

    private bool UpdateDirectory(ShowConfiguration si, int snum, string folder)
    {
        DirectoryInfo di = new(folder);
        bool goAgain = !di.Exists;
        if (di.Exists &&
            !string.Equals(si.AutoFolderNameForSeason(snum), folder, StringComparison.CurrentCultureIgnoreCase))
        {
            if (!si.ManualFolderLocations.ContainsKey(snum))
            {
                si.ManualFolderLocations[snum] = [];
            }

            si.ManualFolderLocations[snum].Add(folder);
            Doc.SetDirty();
        }

        return goAgain;
    }

    private static void TryCreateDirectory(string? folder, string sn, string text)
    {
        if (string.IsNullOrWhiteSpace(folder))
        {
            LOGGER.Warn($"Folder is not specified for {sn} {text}");
        }
        else
        {
            try
            {
                LOGGER.Info($"Creating directory as it is missing: {folder}");
                Directory.CreateDirectory(folder);
            }
            catch (Exception ioe)
            {
                LOGGER.Warn($"Could not create directory: {folder} Error Message: {ioe.ErrorText()}");
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
