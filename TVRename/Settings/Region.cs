//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class Region
{
    public Region(int id, string abbreviation, string threeAbbreviation, string localName, string? englishName)
    {
        Id = id;
        Abbreviation = abbreviation;
        ThreeAbbreviation = threeAbbreviation;
        LocalName = localName;
        EnglishName = englishName;
    }

    public int Id { get; set; }
    public string Abbreviation { get; set; }
    public string ThreeAbbreviation { get; set; }
    public string LocalName { get; set; }
    public string? EnglishName { get; set; }
}
