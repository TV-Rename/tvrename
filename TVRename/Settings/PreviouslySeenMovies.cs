using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace TVRename;

public class PreviouslySeenMovies : List<int>
{
    public PreviouslySeenMovies()
    {
    }

    public PreviouslySeenMovies(XElement? xml)
    {
        if (xml is null)
        {
            return;
        }

        foreach (XElement n in xml.Descendants("Movie"))
        {
            EnsureAdded(XmlConvert.ToInt32(n.Value));
        }
    }

    private void EnsureAdded(int epId)
    {
        if (!Contains(epId) && epId > 0)
        {
            Add(epId);
        }
    }

    public void EnsureAdded(MovieConfiguration m) => EnsureAdded(m.Code);

    public bool Includes(MovieConfiguration? m) => m is { Code: > 0 } && Contains(m.Code);

    public bool Includes(Item item) => Includes(item.Movie);

    //TODO fix this class to make it work with multi sources
}
