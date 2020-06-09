using System;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class ActionDateTouchShow : ActionDateTouchDirectory
    {
        private readonly ShowItem show; // if for an entire show, rather than specific episode
        public ActionDateTouchShow(DirectoryInfo dir, ShowItem si, DateTime date) :base(dir,date)
        {
            show = si;
        }
        public override string SeriesName => show.ShowName;
        public override string SeasonNumber => string.Empty;
    }
}
