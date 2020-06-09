using System;
using Alphaleonis.Win32.Filesystem;

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
        public override string SeasonNumber => processedSeason.SeasonNumber.ToString();
    }
}
