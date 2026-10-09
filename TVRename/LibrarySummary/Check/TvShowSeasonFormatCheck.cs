//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class TvShowSeasonFormatCheck(ShowConfiguration show, TVDoc doc) : TvShowCheck(show, doc)
{
    public override bool Check() => Show.AutoAddType == ShowConfiguration.AutomaticFolderType.customFolderFormat && TVSettings.Instance.DefShowUseSubFolders;

    public override string Explain() => $"TV Show does not use the library default for AutomaticFolder creation ({TVSettings.Instance.SeasonFolderFormat}), it uses {Show.AutoAddType}{(Show.AutoAddType == ShowConfiguration.AutomaticFolderType.customFolderFormat ? $" {Show.AutoAddCustomFolderFormat}" : "")}";

    protected override void FixInternal()
    {
        Show.AutoAddType = ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat;
        //TODO Should move files from the old location to the new one!!
    }

    public override string CheckName => "[TV] Use Custom season Folder Name Format";
}
