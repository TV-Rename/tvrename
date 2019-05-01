// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using JetBrains.Annotations;

namespace TVRename
{
    public class SeriesSpecifier
    {
        public readonly int SeriesId;
        public readonly bool UseCustomLanguage;
        public readonly string CustomLanguageCode;
        public readonly string Name;

        public SeriesSpecifier(int key, bool useCustomLanguage, [CanBeNull] string customLanguageCode,string name)
        {
            SeriesId = key;
            Name = name;

            if (string.IsNullOrWhiteSpace(customLanguageCode))
            {
                UseCustomLanguage = false;
                CustomLanguageCode = TVSettings.Instance.PreferredLanguageCode;
            }
            else
            {
                UseCustomLanguage = useCustomLanguage;
                CustomLanguageCode = customLanguageCode;
            }
        }
    }
}
