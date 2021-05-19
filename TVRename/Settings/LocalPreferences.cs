// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    public class LocalPreferences
    {
        public Region PreferredRegion { get; }
        public Language PreferredLanguage { get; }

        public LocalPreferences(Region preferredRegion, Language preferredLanguage)
        {
            PreferredRegion = preferredRegion;
            PreferredLanguage = preferredLanguage;
        }
    }

}
