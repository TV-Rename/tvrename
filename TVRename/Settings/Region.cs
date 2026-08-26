//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class Region(int id, string abbreviation, string threeAbbreviation, string localName, string? englishName)
{
    public int Id { get; set; } = id;
    public string Abbreviation { get; set; } = abbreviation;
    public string ThreeAbbreviation { get; set; } = threeAbbreviation;
    public string LocalName { get; set; } = localName;
    public string? EnglishName { get; set; } = englishName;
}
