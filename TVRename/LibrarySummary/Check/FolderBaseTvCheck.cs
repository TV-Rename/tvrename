//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class FolderBaseTvCheck(ShowConfiguration show, TVDoc doc) : TvShowCheck(show, doc)
{
    public override bool Check() => !Show.AutoAddFolderBase.HasValue() && Show.AutoAddNewSeasons();

    public override string Explain() => "This TV show does not have an automatic folder base specified.";

    /// <exception cref="FixCheckException">Can't fix movie</exception>
    protected override void FixInternal()
    {
        if (TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation && TVSettings.Instance.DefShowLocation.HasValue())
        {
            Show.AutoAddFolderBase = TVSettings.Instance.DefShowLocation.EnsureEndsWithSeparator() + TVSettings.Instance.DefaultTVShowFolder(Show);

            return;
        }

        if (TVSettings.Instance.LibraryFolders.Count > 1)
        {
            throw new FixCheckException("Can't fix movie as multiple TV Show Library Folders are specified");
        }

        if (TVSettings.Instance.LibraryFolders.Count == 0)
        {
            throw new FixCheckException("Can't fix movie as no TV Show Library Folders are specified");
        }

        Show.AutoAddFolderBase = TVSettings.Instance.LibraryFolders.First().EnsureEndsWithSeparator()
                                 + TVSettings.Instance.DefaultTVShowFolder(Show);
    }

    public override string CheckName => "[TV] Has an automatic base folder supplied";
}


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

        //Update the folder to the new value
        Show.AutoAddFolderBase = DefaultFolder(Show.AutoAddFolderBase);

        //Create the new location if needed and move files
        Directory.Move(oldFolder, Show.AutoAddFolderBase);
    }
}
