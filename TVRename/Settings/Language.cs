// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    public class Language
    {
        public Language() {
        }

        public Language(int id, string abbreviation, string name, string englishName)
        {
            Id = id;
            Abbreviation = abbreviation;
            Name = name;
            EnglishName = englishName;
        }

        public int Id { get; }
        public string Abbreviation { get;}
        public string Name { get;}
        public string EnglishName { get;}
    }
}
