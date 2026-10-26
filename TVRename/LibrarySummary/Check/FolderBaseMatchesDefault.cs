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
    public override string Explain() => $"This TV show has a base folder that does not match the default. {Show.AutoAddFolderBase} is current value, default is {DefaultFolder(Show.AutoAddFolderBase)}";

    public override bool Check()
    {
        string currentFolder = Show.AutoAddFolderBase;
        string newlyCreatedSettingsFolder = DefaultFolder(currentFolder);

        return currentFolder != newlyCreatedSettingsFolder;
    }

    private string DefaultFolder(string currentFolder)
    {
        string libraryFolder = FindBestLibraryFolder(currentFolder, TVSettings.Instance.LibraryFolders);
        string newlyCreatedSettingsFolder = libraryFolder.EnsureEndsWithSeparator()
                                 + TVSettings.Instance.DefaultTVShowFolder(Show);
        return newlyCreatedSettingsFolder;
    }

    private static string FindBestLibraryFolder(string currentFolder, SafeList<string> libraryFolders)
    {
        foreach(string libFolder in libraryFolders)
        {
            if (currentFolder.StartsWith(libFolder))
            {
                return libFolder;
            }
        }
        return libraryFolders.FirstOrDefault() ?? string.Empty;
    }

    /// <exception cref="FixCheckException">Can't fix movie</exception>
    protected override void FixInternal()
    {
        //Store the old folder
        string oldFolder = Show.AutoAddFolderBase;

        try
        {
            //Update the folder to the new value
            Show.AutoAddFolderBase = DefaultFolder(Show.AutoAddFolderBase);

            Directory.CreateDirectory(Show.AutoAddFolderBase);

            //Create the new location if needed and move files
            Directory.Move(oldFolder, Show.AutoAddFolderBase,MoveOptions.CopyAllowed);
        }
        catch (Exception ex)
        {
            LOGGER.Error(ex, $"Could not move from {oldFolder} to {DefaultFolder(Show.AutoAddFolderBase)}");
        }
    }
}
