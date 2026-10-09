//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class ActionDateTouchSeason(DirectoryInfo dir, ProcessedSeason sn, DateTime date) : ActionDateTouchDirectory(dir, date)
{
    private readonly ProcessedSeason processedSeason = sn; // if for an entire show, rather than specific episode

    public override string SeriesName => processedSeason.Show.ShowName;
    public override string SeasonNumber => processedSeason.SeasonNumber != 0 ? processedSeason.SeasonNumber.ToString() : TVSettings.SpecialsListViewName;
    public override int? SeasonNumberAsInt => processedSeason.SeasonNumber;
    public override ShowConfiguration Series => processedSeason.Show;
}
