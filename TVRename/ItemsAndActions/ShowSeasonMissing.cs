using System;
using System.Collections.Generic;

namespace TVRename;

public class ShowSeasonMissing : ItemMissing
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

    #region Item Members

    public override bool SameAs(Item o) => o is ShowSeasonMissing missing && CompareTo(missing) == 0;

    public override string Name => "Missing Season";

    public override int CompareTo(Item? o)
    {
        if (o is not ShowSeasonMissing miss)
        {
            return -1;
        }

        return !Show.ShowName.Equals(miss.Show.ShowName) ? string.Compare(Show.ShowName, miss.Show.ShowName, StringComparison.Ordinal) : seasonNumber.CompareTo(miss.seasonNumber);
    }

    #endregion Item Members

    public override bool DoRename => Episode?.Show.DoRename ?? true;

    public override MediaConfiguration Show => show;

    public override int? SeasonNumberAsInt => seasonNumber;

    public override string SeasonNumber => TVSettings.SeasonNameFor(seasonNumber);

    public override string SeriesName => show.Name ?? string.Empty;

    public override string ToString() => $"{Show.ShowName} Season:{SeasonNumber}";

    public override ShowConfiguration Series => show;
}
