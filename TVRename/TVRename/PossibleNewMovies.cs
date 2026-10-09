//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class PossibleNewMovies : ConcurrentBag<PossibleNewMovie>
{
    public void AddIfNew(PossibleNewMovie ai)
    {
        if (this.Any(m => m.Matches(ai)))
        {
            return;
        }
        Add(ai);
    }

    internal void Remove(PossibleNewMovie ai)
    {
        TryTake(out PossibleNewMovie? removed);
    }
}
