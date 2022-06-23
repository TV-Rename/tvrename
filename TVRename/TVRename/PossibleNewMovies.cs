using System.Linq;

namespace TVRename;

public class PossibleNewMovies : SafeList<PossibleNewMovie>
{
    public void AddIfNew(PossibleNewMovie ai)
    {
        if (this.Any(m => m.Matches(ai)))
        {
            return;
        }
        Add(ai);
    }
}
