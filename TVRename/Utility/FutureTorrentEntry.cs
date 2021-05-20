//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;

// ReSharper disable All

// Opens, understands, manipulates, and writes out BEncoded .torrent files, and uTorrent's resume.dat

namespace TVRename
{
    public class FutureTorrentEntry : TorrentEntry
    {
        public FutureTorrentEntry([NotNull] string torrentfile, [NotNull] string to) : base(torrentfile, to, 0, false, string.Empty)
        {
        }
    }

    // btcore

    // BTProcessor
}
