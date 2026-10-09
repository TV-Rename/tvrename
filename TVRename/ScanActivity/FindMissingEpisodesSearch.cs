//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class FindMissingEpisodesSearch(TVDoc doc, TVDoc.ScanSettings settings) : FindMissingEpisodes(doc, settings)
{
    protected override string CheckName() => "Looked online for the missing files to see if they can be downloaded";

    protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.search;
}
