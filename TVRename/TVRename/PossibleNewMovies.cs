using System;
using System.Linq;

namespace TVRename
{
    public class PossibleNewMovies : SafeList<PossibleNewMovie>
    {
        public void AddIfNew(PossibleNewMovie ai)
        {
            if (this.Any(m => m.MovieStub.Equals(ai.MovieStub, StringComparison.CurrentCultureIgnoreCase)))
            {
                return;
            }
            Add(ai);
        }
    }
}
