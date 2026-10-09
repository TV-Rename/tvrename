//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class uTorrentFinder(TVDoc doc, TVDoc.ScanSettings settings) : DownloadingProviderFinder(doc, new uTorrent(), settings)
{
    public override bool Active() => TVSettings.Instance.CheckuTorrent;
}
