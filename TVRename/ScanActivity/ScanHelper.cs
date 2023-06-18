using Alphaleonis.Win32.Filesystem;
using NLog;
using System;

namespace TVRename;

public static class ScanHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static bool? AskUserAboutFileReplacement(FileInfo newFile, FileInfo existingFile, ProcessedEpisode pep, IDialogParent owner, TVDoc doc, ItemList returnActions)
    {
        try
        {
            using ChooseFile question = new(existingFile, newFile);

            owner.ShowChildDialog(question);
            ChooseFile.ChooseFileDialogResult result = question.Answer;

            switch (result)
            {
                case ChooseFile.ChooseFileDialogResult.ignore:
                    Logger.Info(
                        $"Keeping {newFile.FullName} as it might be better quality than {existingFile.FullName}");
                    return false;

                case ChooseFile.ChooseFileDialogResult.left:
                    Logger.Info(
                        $"User has elected to remove {newFile.FullName} as it is not as good quality than {existingFile.FullName}");
                    return true;

                case ChooseFile.ChooseFileDialogResult.right:
                    UpgradeFile(newFile, pep, existingFile, doc, returnActions);
                    return false;

                default:
                    throw new NotSupportedException($"result = {result} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}");
            }
        }
        catch (System.IO.FileNotFoundException)
        {
            return false;
        }
    }

    /// <summary>Asks user about whether to replace a file.</summary>
    /// <returns>false if the newFile is needed.</returns>
    public static bool? AskUserAboutFileReplacement(FileInfo newFile, FileInfo existingFile, MovieConfiguration pep, IDialogParent owner, TVDoc doc, ItemList returnActions)
    {
        try
        {
            using ChooseFile question = new(existingFile, newFile);

            owner.ShowChildDialog(question);
            ChooseFile.ChooseFileDialogResult result = question.Answer;

            switch (result)
            {
                case ChooseFile.ChooseFileDialogResult.ignore:
                    Logger.Info(
                        $"Keeping {newFile.FullName} as it might be better quality than {existingFile.FullName}");
                    return false;

                case ChooseFile.ChooseFileDialogResult.left:
                    Logger.Info(
                        $"User has elected to remove {newFile.FullName} as it is not as good quality than {existingFile.FullName}");
                    return true;

                case ChooseFile.ChooseFileDialogResult.right:
                    UpgradeFile(newFile, pep, existingFile, doc, returnActions);
                    return false;

                default:
                    throw new NotSupportedException($"result = {result} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}");
            }
        }
        catch (System.IO.FileNotFoundException)
        {
            return false;
        }
    }

    public static void UpgradeFile(FileInfo fi, ProcessedEpisode pep, FileInfo existingFile, TVDoc doc, ItemList actions)
    {
        if (existingFile.Extension != fi.Extension)
        {
            actions.Add(new ActionDeleteFile(existingFile, pep, null));
            actions.Add(new ActionCopyMoveRename(fi, existingFile.WithExtension(fi.Extension), pep, doc));
        }
        else
        {
            actions.Add(new ActionCopyMoveRename(fi, existingFile, pep, doc));
        }

        Logger.Info(
            $"Using {fi.FullName} to replace {existingFile.FullName} as it is better quality");
    }

    public static void UpgradeFile(FileInfo fi, MovieConfiguration pep, FileInfo existingFile, TVDoc doc, ItemList actions)
    {
        if (existingFile.Extension != fi.Extension)
        {
            actions.Add(new ActionDeleteFile(existingFile, pep, null));
            actions.Add(new ActionCopyMoveRename(fi, existingFile.WithExtension(fi.Extension), pep, doc));
        }
        else
        {
            actions.Add(new ActionCopyMoveRename(fi, existingFile, pep, doc));
        }

        Logger.Info(
            $"Using {fi.FullName} to replace {existingFile.FullName} as it is better quality");
    }
}
