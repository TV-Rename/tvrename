//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class PreviouslySeenMovies : List<int>
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
            try
            {
                EnsureAdded(XmlConvert.ToInt32(n.Value));
            }
            catch (OverflowException ex)
            {
                Logger.Fatal($"Could not add movie Id {n.Value} to previouslyseenmovies", ex);
            }
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
