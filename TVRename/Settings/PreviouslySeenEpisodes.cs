//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace TVRename;

public class PreviouslySeenEpisodes : List<int>
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public PreviouslySeenEpisodes()
    {
    }

    public PreviouslySeenEpisodes(XElement? xml)
    {
        if (xml is null)
        {
            return;
        }

        foreach (XElement n in xml.Descendants("Episode"))
        {
            try
            {
                EnsureAdded(XmlConvert.ToInt32(n.Value));
            }
            catch (OverflowException ex)
            {
                Logger.Fatal($"Could not add episode Id {n.Value} to previouslyseenepisodes",ex);
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

    public void EnsureAdded(ProcessedEpisode episode) => EnsureAdded(episode.EpisodeId);

    public bool Includes(Item item) => Includes(item.Episode);

    public bool Includes(ProcessedEpisode? episode) => episode is { EpisodeId: > 0 } && Contains(episode.EpisodeId);
}
