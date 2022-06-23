//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class Language
{
    public Language(int tvdbid, string abbreviation, string threeAbbreviation, string localName, string? englishName, string isoDialectAbbreviation, bool isPrimary)
    {
        TvdbId = tvdbid;
        Abbreviation = abbreviation;
        ThreeAbbreviation = threeAbbreviation;
        LocalName = localName;
        EnglishName = englishName;
        ISODialectAbbreviation = isoDialectAbbreviation;
        IsPrimary = isPrimary;
    }

    public int TvdbId { get; set; }
    public string Abbreviation { get; set; }
    public string ThreeAbbreviation { get; set; }

    // ReSharper disable once InconsistentNaming
    public string ISODialectAbbreviation { get; set; }

    public bool IsPrimary { get; set; }
    public string LocalName { get; set; }
    public string? EnglishName { get; set; }
}
