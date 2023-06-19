using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

public class ShowSeasonMissing : ItemMissing, IEquatable<ShowSeasonMissing>
{
    private readonly int seasonNumber;
    private readonly ShowConfiguration show;
    public readonly List<ShowItemMissing> OriginalItems;

    public ShowSeasonMissing(ShowConfiguration si, int snum, string whereItShouldBeFolder, List<ShowItemMissing> originalItems)
        : base(string.Empty, string.Empty, whereItShouldBeFolder)
    {
        TheFileNoExt = whereItShouldBeFolder + System.IO.Path.DirectorySeparatorChar + Filename;
        seasonNumber = snum;
        OriginalItems = originalItems;
        show = si;
    }

    public override string Name => "Missing Season";
    public override bool DoRename => Episode?.Show.DoRename ?? true;
    public override MediaConfiguration Show => show;
    public override int? SeasonNumberAsInt => seasonNumber;
    public override string SeasonNumber => TVSettings.SeasonNameFor(seasonNumber);
    public override string SeriesName => show.Name ?? string.Empty;
    public override string ToString() => $"{Show.ShowName} Season:{SeasonNumber}";
    public override ShowConfiguration Series => show;
    public override string EpisodeString => ConvertEpNumsToText(show.ActiveSeasons.FirstOrDefault(s => s.Key == seasonNumber).Value);

    private static string ConvertEpNumsToText(IReadOnlyCollection<ProcessedEpisode> value)
    {
        int? min = value.Min(e => e.AppropriateEpNum);
        int? max = value.Max(e => e.EpNum2);
        return $"{min}-{max}";
    }

    #region Equality Stuff

    public override bool SameAs(Item o) => o is ShowSeasonMissing missing && CompareTo(missing) == 0;

    public override int CompareTo(Item? o)
    {
        if (o is not ShowSeasonMissing miss)
        {
            return -1;
        }

        return !Show.ShowName.Equals(miss.Show.ShowName) ? string.Compare(Show.ShowName, miss.Show.ShowName, StringComparison.Ordinal) : seasonNumber.CompareTo(miss.seasonNumber);
    }

    public bool Equals(ShowSeasonMissing? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && seasonNumber == other.seasonNumber && show.Equals(other.show);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((ShowSeasonMissing)obj);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), seasonNumber, show);

    #endregion Equality Stuff
}
