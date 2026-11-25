//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class FolderBaseMatchesDefault(ShowConfiguration show, TVDoc doc) : TvShowCheck(show, doc)
{
    public override string CheckName => "[TV] Base folder is not consistent with setting for new shows";
    public override string Explain() => $"This TV show has a base folder that does not match the default. {OldValue} is current value, default is {NewValue}";
    public override bool Check() => !string.Equals(OldValue, NewValue, StringComparison.OrdinalIgnoreCase);
    private string OldValue => Show.AutoAddFolderBase.EnsureEndsWithSeparator();
    private string NewValue => DefaultFolder(Show.AutoAddFolderBase);

    private string DefaultFolder(string currentFolder)
    {
        return FindBestLibraryFolder(currentFolder, TVSettings.Instance.LibraryFolders)
            + TVSettings.Instance.DefaultTVShowFolder(Show).EnsureEndsWithSeparator();
    }

    private static string FindBestLibraryFolder(string currentFolder, SafeList<string> libraryFolders)
    {
        foreach(string libFolder in libraryFolders)
        {
            if (currentFolder.StartsWith(libFolder,StringComparison.OrdinalIgnoreCase))
            {
                return libFolder.EnsureEndsWithSeparator();
            }
        }
        return libraryFolders.FirstOrDefault()?.EnsureEndsWithSeparator() ?? string.Empty;
    }

    /// <exception cref="FixCheckException">Can't fix show</exception>
    protected override async Task FixInternal()
    {
        //Store the old folder
        string oldFolderValue = Show.AutoAddFolderBase;
        string newFolderValue = NewValue;

        //Create the new location if needed and move files
        if (Directory.Exists(newFolderValue))
        {
            await FileHelper.MergeDirectoriesAsync(oldFolderValue, newFolderValue);
        }
        else
        {
            string message = $"Could not move files for {Show.ShowName}. Would have liked to move files from [{oldFolderValue}] to '{newFolderValue}'";

            try
            {
                CopyMoveResult result = Directory.Move(oldFolderValue, newFolderValue, MoveOptions.CopyAllowed);
                if (result.ErrorCode != 0)
                {
                    Show.AutoAddFolderBase = oldFolderValue; // revert to old value if move fails
                    throw new FixCheckException($"Could not move from {oldFolderValue} to {newFolderValue} Error was:{result.ErrorMessage}");
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                throw new FixCheckException(message + ": " + uae.Message, uae);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                //the source was not present anyway, so do nothing
            }
            catch (System.IO.PathTooLongException ptle)
            {
                throw new FixCheckException(message + ": " + ptle.Message, ptle);
            }
            catch (System.IO.IOException ex)
            {
                throw new FixCheckException(message + ": " + ex.Message, ex);
            }
        }

        //Update the folder to the new value
        Show.AutoAddFolderBase = NewValue;

        LOGGER.Warn($"Updated folder for {Show.ShowName}. Updated from [{oldFolderValue}] to '{newFolderValue}'");
    }
}
