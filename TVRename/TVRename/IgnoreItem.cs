// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

// An "IgnoreItem" represents a file/episode to never ask the user about again. (Right-click->Ignore Selected / Options->Ignore List)

using System;
using JetBrains.Annotations;

namespace TVRename
{
    public class IgnoreItem
    {
        public readonly string FileAndPath;

        public IgnoreItem(string fileAndPath)
        {
            FileAndPath = fileAndPath;
        }

        public bool SameFileAs(IgnoreItem? o)
        {
            if (string.IsNullOrEmpty(FileAndPath) || string.IsNullOrEmpty(o?.FileAndPath))
            {
                return false;
            }

            return FileAndPath == o.FileAndPath;
        }

        public bool MatchesEpisode([NotNull] string folder,[NotNull] ProcessedEpisode episode)
        {
            if (!FileAndPath.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string plannedFilename = TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(episode));

            return FileAndPath.EndsWith(plannedFilename,StringComparison.OrdinalIgnoreCase);
        }
    }
}
