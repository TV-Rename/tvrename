using Alphaleonis.Win32.Filesystem;
using System;

namespace TVRename;

internal class ActionDateTouchSeason(DirectoryInfo dir, ProcessedSeason sn, DateTime date) : ActionDateTouchDirectory(dir, date)
{
    private readonly ProcessedSeason processedSeason = sn; // if for an entire show, rather than specific episode

    public override string SeriesName => processedSeason.Show.ShowName;
    public override string SeasonNumber => processedSeason.SeasonNumber != 0 ? processedSeason.SeasonNumber.ToString() : TVSettings.SpecialsListViewName;
    public override int? SeasonNumberAsInt => processedSeason.SeasonNumber;
    public override ShowConfiguration Series => processedSeason.Show;
}
