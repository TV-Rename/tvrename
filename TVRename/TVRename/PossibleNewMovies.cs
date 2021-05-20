using System;
using System.Linq;

namespace TVRename
{
    public class PossibleNewMovies : System.Collections.Generic.List<PossibleNewMovie>
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