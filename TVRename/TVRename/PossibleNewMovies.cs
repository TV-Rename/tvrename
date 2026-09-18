using System;
using System.Collections.Concurrent;
using System.Linq;

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
