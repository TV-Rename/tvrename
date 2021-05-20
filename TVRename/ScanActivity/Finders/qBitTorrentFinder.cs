//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class qBitTorrentFinder : DownloadingProviderFinder
    {
        public qBitTorrentFinder(TVDoc i) : base(i, new qBitTorrent())
        {
        }

        public override bool Active() => TVSettings.Instance.CheckqBitTorrent;
    }
}