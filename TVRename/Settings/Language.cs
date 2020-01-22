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

        //Note Keep the Setters on these properties as they are needed for XML Serialisation (Codacity will complain though)
        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }
}
