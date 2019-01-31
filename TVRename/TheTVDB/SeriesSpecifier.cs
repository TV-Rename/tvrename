// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    public class SeriesSpecifier
    {
        public readonly int SeriesId;
        public readonly bool UseCustomLanguage;
        public readonly string CustomLanguageCode;
        public readonly string Name;

        public SeriesSpecifier(int key, bool useCustomLanguage, string customLanguageCode,string name)
        {
            SeriesId = key;
            UseCustomLanguage = useCustomLanguage;
            CustomLanguageCode = customLanguageCode;
            Name = name;
        }
    }
}
