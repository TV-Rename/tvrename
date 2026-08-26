//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    internal class PossibleMedia(MediaConfiguration configuration, string hint)
    {
        internal readonly MediaConfiguration Configuration = configuration;
        internal readonly string Hint = hint;
    }
}
