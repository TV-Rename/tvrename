using System;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class ActionDateTouchMovie : ActionDateTouchFile
    {
        public ActionDateTouchMovie(FileInfo f, MovieConfiguration mov, DateTime date) : base(f, date)
        {
            Movie = mov;
        }
        public override bool SameAs(Item o)
        {
            return o is ActionDateTouchMovie touch && touch.WhereFile == WhereFile;
        }

        public override int CompareTo(Item o)
        {
            if (o is null || !(o is ActionDateTouchMovie nfo))
            {
                return -1;
            }

            if (Movie is null)
            {
                return 1;
            }

            if (nfo.Movie is null)
            {
                return -1;
            }

            return string.Compare(WhereFile.FullName + Movie.ShowName, nfo.WhereFile.FullName + nfo.Movie.ShowName, StringComparison.Ordinal);
        }
    }
}
