//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

public abstract class ActionFileOperation : Action
{
    protected TVSettings.TidySettings? Tidyup;

    protected void DoTidyUp(DirectoryInfo? di) => FileHelper.DoTidyUp(di, Tidyup);

    protected void DeleteOrRecycleFolder(DirectoryInfo? di) => FileHelper.DeleteOrRecycleFolder(di, Tidyup);
}
