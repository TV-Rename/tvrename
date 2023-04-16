//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    internal class PossibleMedia
    {
        internal readonly MediaConfiguration Configuration;
        internal readonly string hint;

        public PossibleMedia(MediaConfiguration configuration, string hint)
        {
            Configuration = configuration;
            this.hint = hint;
        }
    }
}
