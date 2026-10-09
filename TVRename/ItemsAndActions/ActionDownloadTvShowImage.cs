//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

public class ActionDownloadTvShowImage(ShowConfiguration si, FileInfo dest, string path, bool shrink) : ActionDownloadImage(si, null, dest, path, shrink)
{
    public ActionDownloadTvShowImage(ShowConfiguration si, FileInfo dest, string path)
        : this(si, dest, path, false)
    {
    }

    public override ShowConfiguration? Series => Si as ShowConfiguration;
}