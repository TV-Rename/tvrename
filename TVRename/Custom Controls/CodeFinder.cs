using System;
using System.Windows.Forms;

namespace TVRename
{
    public abstract class CodeFinder : UserControl
    {
        public abstract void SetHint(string hint);

        public abstract event EventHandler<EventArgs>? SelectionChanged;

        public abstract CachedSeriesInfo? SelectedShow();

        public abstract CachedMovieInfo? SelectedMovie();

        public abstract int SelectedCode();
    }
}
