//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
namespace TVRename;

public class ExtraEp(int series, int episode, ProcessedSeason.SeasonType st)
{
    public bool Done = false;
    public readonly int EpisodeId = episode;
    public readonly int SeriesId = series;
    public readonly ProcessedSeason.SeasonType Order = st;
}
