//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

public class ActionDownloadMovieImage(MovieConfiguration si, FileInfo dest, string path, bool shrink) : ActionDownloadImage(si, null, dest, path, shrink)
{
    public ActionDownloadMovieImage(MovieConfiguration si, FileInfo dest, string path)
        : this(si, dest, path, false)
    {
    }
}