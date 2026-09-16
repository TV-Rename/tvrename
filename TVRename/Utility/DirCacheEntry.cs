//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public class DirCacheEntry(FileInfo f)
{
    public readonly long Length = f.Length;
    public readonly FileInfo TheFile = f;
}
