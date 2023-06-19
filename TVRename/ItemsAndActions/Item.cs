//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TVRename;

public abstract class Item : IComparable<Item>, INotifyPropertyChanged, IEquatable<Item> // something shown in the list on the Scan tab (not always an Action)
{
    protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    public abstract string? TargetFolder { get; } // return a list of folders for right-click menu
    public abstract string ScanListViewGroup { get; } // which group name for the listview
    public abstract int IconNumber { get; } // which icon number to use in "ilIcons" (UI.cs). -1 for none
    public abstract IgnoreItem? Ignore { get; } // what to add to the ignore list / compare against the ignore list
    public ProcessedEpisode? Episode { get; protected init; } // associated episode
    public MovieConfiguration? Movie { get; protected init; } // associated movie

    public abstract int CompareTo(Item? obj); // for sorting items in scan list (ActionItemSorter)

    public abstract bool SameAs(Item o); // are we the same thing as that other one?

    public abstract string Name { get; } // Name of this action, e.g. "Copy", "Move", "Download"

    public ItemMissing? UndoItemMissing; //Item to revert to if we have to cancel this action

    public event PropertyChangedEventHandler? PropertyChanged;

    // This method is called by the Set accessor of each property.
    // The CallerMemberName attribute that is applied to the optional propertyName
    // parameter causes the property name of the caller to be substituted as an argument.
    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected static IgnoreItem? GenerateIgnore(string? file) => file.HasValue() ? new IgnoreItem(file) : null;

    public virtual string SeriesName => Movie?.ShowName ?? Episode?.Show.ShowName ?? string.Empty;
    public virtual ShowConfiguration? Series => Episode?.Show;
    public virtual string SeasonNumber => Episode?.SeasonNumberAsText ?? string.Empty;

    public virtual string EpisodeString => Episode?.EpisodeNumbersAsText ?? string.Empty;

    // ReSharper disable once UnusedMember.Global
    public int? EpisodeNumber => Episode?.AppropriateEpNum;
    public virtual int? SeasonNumberAsInt => Episode?.AppropriateSeasonNumber;

    public string OrderKey => SeasonNumberAsInt?.Pad(2) + "-" +
                                      Episode?.AppropriateEpNum.Pad(4) + "-" + AirDateString;

    public virtual string AirDateString => Episode?.GetAirDateDt(true).PrettyPrint() ?? Movie?.CachedMovie?.FirstAired.PrettyPrint() ?? string.Empty;

    public virtual DateTime? AirDate => Episode?.GetAirDateDt(true) ?? Movie?.CachedMovie?.FirstAired;
    public abstract string? DestinationFolder { get; }
    public abstract string? DestinationFile { get; }

    public virtual string SourceDetails => string.Empty;

    private string? errorTextValue;
    public string? ErrorText
    {
        get => errorTextValue;
        protected internal set
        {
            errorTextValue = value;
            NotifyPropertyChanged();
        }
    } // Human-readable error message, for when Error is true

    public int CompareTo(object obj) => CompareTo(obj as Item);

    public override bool Equals(object? obj) => obj is Item i && SameAs(i);

    public bool Equals(Item? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || SameAs(other);
    }

    public override int GetHashCode() => HashCode.Combine(Episode, Movie, DestinationFolder, DestinationFile);

    public abstract bool CheckedItem
    {
        get;
        set;
    }
}
