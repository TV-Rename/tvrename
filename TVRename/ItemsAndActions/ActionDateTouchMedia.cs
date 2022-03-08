using Alphaleonis.Win32.Filesystem;
using System;

namespace TVRename
{
    internal class ActionDateTouchMedia : ActionDateTouchDirectory
    {
        private readonly MediaConfiguration show; // if for an entire show, rather than specific episode

        public ActionDateTouchMedia(DirectoryInfo dir, MediaConfiguration si, DateTime date) : base(dir, date)
        {
            show = si;
            if (si is MovieConfiguration m)
            {
                Movie = m;
            }
        }

        public override string SeriesName => show.ShowName;
        public override string SeasonNumber => string.Empty;
        public override int? SeasonNumberAsInt => null;
    }
}
