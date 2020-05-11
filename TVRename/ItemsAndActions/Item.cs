// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using JetBrains.Annotations;

namespace TVRename
{
 public abstract class Item :IComparable // something shown in the list on the Scan tab (not always an Action)
    {
        public abstract string TargetFolder { get; } // return a list of folders for right-click menu
        public abstract string ScanListViewGroup { get; } // which group name for the listview
        public abstract int IconNumber { get; } // which icon number to use in "ilIcons" (UI.cs). -1 for none
        public abstract IgnoreItem Ignore { get; } // what to add to the ignore list / compare against the ignore list
        public ProcessedEpisode Episode { get; protected set; } // associated episode
        public abstract int CompareTo(object o); // for sorting items in scan list (ActionItemSorter)
        public abstract bool SameAs(Item o); // are we the same thing as that other one?
        public abstract string Name { get; } // Name of this action, e.g. "Copy", "Move", "Download"

        [CanBeNull]
        protected static IgnoreItem GenerateIgnore([CanBeNull] string file) => string.IsNullOrEmpty(file) ? null : new IgnoreItem(file);

        [NotNull]
        public virtual string SeriesName => UI.GenerateShowUIName(Episode);
        [NotNull]
        public virtual string SeasonNumber => Episode?.SeasonNumberAsText ?? string.Empty;
        [NotNull]
        public virtual string EpisodeNumber => Episode?.EpNumsAsString() ?? string.Empty;
        [NotNull]
        public virtual string AirDateString => Episode?.GetAirDateDt(true).PrettyPrint() ?? string.Empty;

        public virtual DateTime? AirDate => Episode?.GetAirDateDt(true);
        public abstract string DestinationFolder { get; }
        public abstract string DestinationFile { get; }
        [NotNull]
        public virtual string SourceDetails => string.Empty;
        public string ErrorText { get; protected internal set; } // Human-readable error message, for when Error is true
    }
}
