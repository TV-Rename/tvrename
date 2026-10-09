//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class UseManualFoldersTvShowCheck(ShowConfiguration show, TVDoc doc) : CustomTvShowCheck(show, doc)
{
    protected override string FieldName => "[TV] Use Manual season Folders for TV Show";
    protected override bool Field => Show.UsesManualFolders();

    protected override void FixInternal()
    {
        Show.ManualFolderLocations.Clear();
        Show.AutoAddType = ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat;
    }
    protected override string CustomFieldValue => Show.ManualFolderLocations.Values.SelectMany(x => x).ToCsv();

    protected override string DefaultFieldValue => Show.AutoAddFolderBase;
}
