//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;

namespace TVRename
{
    public class FutureTorrentEntry : TorrentEntry
    {
        public FutureTorrentEntry([NotNull] string torrentFile, [NotNull] string to) : base(torrentFile, to, 0, false, string.Empty)
        {
        }
    }
}
