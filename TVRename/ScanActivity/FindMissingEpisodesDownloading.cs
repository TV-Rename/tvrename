//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class FindMissingEpisodesDownloading(TVDoc doc, TVDoc.ScanSettings settings) : FindMissingEpisodes(doc, settings)
{
    protected override string CheckName() => "Looked in download applications for the missing files";

    protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.downloading;
}
