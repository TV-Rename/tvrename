// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public class PreviouslySeenEpisodes : List<int>
    {
        public PreviouslySeenEpisodes()
        {
        }

        public PreviouslySeenEpisodes([CanBeNull] XElement xml)
        {
            if (xml is null)
            {
                return;
            }

            foreach (XElement n in xml.Descendants("Episode"))
            {
                EnsureAdded(XmlConvert.ToInt32(n.Value));
            }
        }

        private void EnsureAdded(int epId)
        {
            if (Contains(epId))
            {
                return;
            }

            Add(epId);
        }

        public void EnsureAdded([NotNull] ProcessedEpisode dbep) => EnsureAdded(dbep.EpisodeId);

        public bool Includes([NotNull] Item item)
        {
            return item.Episode != null && Contains(item.Episode.EpisodeId);
        }
    }
}
