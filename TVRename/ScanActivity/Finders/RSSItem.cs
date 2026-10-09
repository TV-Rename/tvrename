//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class RSSItem(string url, string title, int season, int episode, string showName, int seeders, long bytes,
    string source)
{
    public readonly int Episode = episode;
    public readonly int Season = season;
    public readonly string ShowName = showName;
    public readonly string Title = title;

    // ReSharper disable once InconsistentNaming
    public readonly string URL = url;

    public readonly int Seeders = seeders;
    public readonly long Bytes = bytes;
    public readonly string UpstreamSource = source;
}
