//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class Language(int tvdbid, string abbreviation, string threeAbbreviation, string englishName, string localName, string isoDialectAbbreviation, bool isPrimary)
{
    public int TvdbId { get; set; } = tvdbid;
    public string Abbreviation { get; set; } = abbreviation;
    public string ThreeAbbreviation { get; set; } = threeAbbreviation;

    // ReSharper disable once InconsistentNaming
    public string ISODialectAbbreviation { get; set; } = isoDialectAbbreviation;

    public bool IsPrimary { get; set; } = isPrimary;
    public string LocalName { get; set; } = localName;
    public string EnglishName { get; set; } = englishName;
    public override string ToString() => $"{LocalName} ({EnglishName} - '{ISODialectAbbreviation}')";
}
