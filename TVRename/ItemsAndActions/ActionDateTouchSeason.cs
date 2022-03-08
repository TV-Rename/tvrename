using Alphaleonis.Win32.Filesystem;
using System;

namespace TVRename
{
    internal class ActionDateTouchSeason : ActionDateTouchDirectory
    {
        private readonly ProcessedSeason processedSeason; // if for an entire show, rather than specific episode

        public ActionDateTouchSeason(DirectoryInfo dir, ProcessedSeason sn, DateTime date) : base(dir, date)
        {
            processedSeason = sn;
        }

        public override string SeriesName => processedSeason.Show.ShowName;
        public override string SeasonNumber => processedSeason.SeasonNumber != 0 ? processedSeason.SeasonNumber.ToString() : TVSettings.SpecialsListViewName;
        public override int? SeasonNumberAsInt => processedSeason.SeasonNumber;
    }
}
