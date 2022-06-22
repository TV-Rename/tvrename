//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    public class Locale
    {
        public Region? PreferredRegion { get; }
        public Language? PreferredLanguage { get; }

        public Locale(Region preferredRegion, Language preferredLanguage)
        {
            PreferredRegion = preferredRegion;
            PreferredLanguage = preferredLanguage;
        }

        public Locale(Language preferredLanguage)
        {
            PreferredRegion = null;
            PreferredLanguage = preferredLanguage;
        }

        public Locale(Region preferredRegion)
        {
            PreferredRegion = preferredRegion;
            PreferredLanguage = null;
        }

        public Locale()
        {
            PreferredRegion = null;
            PreferredLanguage = null;
        }

        public Language LanguageToUse(TVDoc.ProviderType provider)
        {
            return PreferredLanguage ?? DefaultLanguage(provider);
        }

        private static Language DefaultLanguage(TVDoc.ProviderType provider)
        {
            return provider == TVDoc.ProviderType.TMDB ? TVSettings.Instance.TMDBLanguage : TVSettings.Instance.PreferredTVDBLanguage;
        }

        public Region RegionToUse(TVDoc.ProviderType provider)
        {
            return PreferredRegion ?? DefaultRegion(provider);
        }

        // ReSharper disable once UnusedParameter.Local
        private Region DefaultRegion(TVDoc.ProviderType provider) => TVSettings.Instance.TMDBRegion;

        public bool IsDifferentLanguageToDefaultFor(TVDoc.ProviderType provider)
        {
            if (PreferredLanguage == null)
            {
                return false;
            }

            if (PreferredLanguage!.ISODialectAbbreviation == DefaultLanguage(provider).ISODialectAbbreviation)
            {
                return false;
            }
            return true;
        }
    }
}
