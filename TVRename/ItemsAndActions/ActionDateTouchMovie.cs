//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

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

    public override int CompareTo(Item? o)
    {
        if (o is not ActionDateTouchMovie nfo)
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
