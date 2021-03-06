//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    public class DirCacheEntry
    {
        public readonly long Length;
        public readonly FileInfo TheFile;

        public DirCacheEntry([NotNull] FileInfo f)
        {
            TheFile = f;
            Length = f.Length;
        }
    }
}