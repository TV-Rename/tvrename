using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class CheckAllMovieFoldersExist : ScanMovieActivity
{
    public CheckAllMovieFoldersExist(TVDoc doc) : base(doc)
    {
    }

    protected override string ActivityName() => "Checked All movie Folders Exist";

    protected override void Check(MovieConfiguration movie, DirFilesCache dfc, TVDoc.ScanSettings settings)
    {
        if (!movie.DoMissingCheck && !movie.DoRename)
        {
            return; // skip
        }

        if (TVSettings.Instance.IgnorePreviouslySeenMovies)
        {
            if (TVSettings.Instance.PreviouslySeenMovies.Includes(movie))
            {
                return;
            }
        }

        List<string> folders = movie.Locations.ToList();

        List<string> ignoredLocations = [];

        foreach (string folderExists in folders)
        {
            CreateFolder(movie, ignoredLocations, folderExists, settings.Owner);
            if (movie.Format == MovieConfiguration.MovieFolderFormat.dvd)
            {
                CreateFolder(movie, ignoredLocations, folderExists, "AUDIO_TS", settings.Owner);
                CreateFolder(movie, ignoredLocations, folderExists, "VIDEO_TS", settings.Owner);
            }
            if (movie.Format == MovieConfiguration.MovieFolderFormat.bluray)
            {
                CreateFolder(movie, ignoredLocations, folderExists, "BDMV", settings.Owner);
                CreateFolder(movie, ignoredLocations, folderExists, "CERTIFICATE", settings.Owner);
            }
        } // for each folder
    }

    private void CreateFolder(MovieConfiguration si, ICollection<string> ignoredLocations,
        string proposedFolderName, string subFolder, IDialogParent owner)
    {
        CreateFolder(si, ignoredLocations, Path.Combine(proposedFolderName, subFolder), owner);
    }

    private void CreateFolder(MovieConfiguration si, ICollection<string> ignoredLocations, string proposedFolderName, IDialogParent owner)
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
                    LOGGER.Warn($"Could not create Folder {folder} as {e.ErrorText()}.");
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
                using MissingFolderAction mfa = new(si.ShowName, "", folder);

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
                    goAgain = UpdateDirectory(si, folder);
                    break;

                case FaResult.kfaNotSet:
                    break;

                case FaResult.kfaCancel:
                    throw new TVRenameOperationInterruptedException();

                case FaResult.kfaCreate:
                    TryCreateDirectory(folder, si.ShowName);
                    goAgain = true;
                    break;

                case FaResult.kfaIgnoreOnce:
                    ignoredLocations.Add(folder);
                    break;

                case FaResult.kfaIgnoreAlways:
                    if (si.AutomaticLocations().Contains(folder))
                    {
                        si.UseAutomaticFolders = false;
                    }
                    if (si.ManualLocations.Contains(folder))
                    {
                        if (si.ManualLocations.Count == 1)
                        {
                            si.UseManualLocations = false;
                        }
                        else
                        {
                            si.ManualLocations.Remove(folder);
                        }
                    }
                    Doc.SetDirty();
                    break;

                default:
                    throw new NotSupportedException($"whatToDo = {whatToDo} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}");
            }
        } while (goAgain);
    }

    private bool UpdateDirectory(MovieConfiguration si, string folder)
    {
        DirectoryInfo di = new(folder);
        bool goAgain = !di.Exists;

        if (di.Exists)
        {
            si.ManualLocations.Add(folder);
            Doc.SetDirty();
        }

        return goAgain;
    }

    private static void TryCreateDirectory(string? folder, string text)
    {
        if (string.IsNullOrWhiteSpace(folder))
        {
            LOGGER.Warn($"Folder is not specified for {text}");
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
