using System.Linq;

namespace TVRename
{
    public class Languages : SafeList<Language>
    {
        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile Languages? InternalInstance;
        private static readonly object SyncRoot = new();

        public static Languages Instance
        {
            get
            {
                if (InternalInstance is null)
                {
                    lock (SyncRoot)
                    {
                        // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                        if (InternalInstance is null)
                        {
                            InternalInstance = new Languages();
                        }
                    }
                }

                return InternalInstance;
            }
        }

        public Language FallbackLanguage => GetLanguageFromCode("en")!;

        private Languages()
        {
            Add(new Language(106, "ar", "ara", "Arabic", "العربية", "ar-AE", true));
            Add(new Language(115, "be", "bel", "Belarusian", "беларуская мова", "be-BY", true));
            Add(new Language(122, "bg", "bul", "Bulgarian", "български език", "bg-BG", true));
            Add(new Language(116, "bn", "ben", "Bangla (India)", "বাংলা", "bn-BD", true));
            Add(new Language(123, "ca", "cat", "Catalan (Spain)", "català", "ca-ES", true));
            Add(new Language(28, "cs", "ces", "Czech", "čeština", "cs-CZ", true));
            Add(new Language(10, "da", "dan", "Danish", "dansk", "da-DK", true));
            Add(new Language(14, "de", "deu", "German (Austrian)", "Deutsch", "de-AT", false));
            Add(new Language(14, "de", "deu", "German (Swiss)", "Deutsch", "de-CH", false));
            Add(new Language(14, "de", "deu", "German", "Deutsch", "de-DE", true));
            Add(new Language(20, "el", "ell", "Greek", "ελληνική γλώσσα", "el-GR", true));
            Add(new Language(7, "en", "eng", "English (Australian)", "English", "en-AU", false));
            Add(new Language(7, "en", "eng", "English (Canadian)", "English", "en-CA", false));
            Add(new Language(7, "en", "eng", "English", "English", "en-GB", true));
            Add(new Language(7, "en", "eng", "English (Ireland)", "English", "en-IE", false));
            Add(new Language(7, "en", "eng", "English (New Zealand)", "English", "en-NZ", false));
            Add(new Language(7, "en", "eng", "English (United States)", "English", "en-US", false));
            Add(new Language(134, "eo", "epo", "Esperanto ", "Esperanto", "eo-EO", true));
            Add(new Language(16, "es", "spa", "Spanish", "español", "es-ES", true));
            Add(new Language(16, "es", "spa", "Spanish (Mexico)", "español", "es-MX", false));
            Add(new Language(135, "et", "est", "Estonian ", "eesti", "et-EE", true));
            Add(new Language(136, "eu", "eus", "Basque (Spain)", "euskara", "eu-ES", true));
            Add(new Language(139, "fa", "fas", "Farsi", "فارسی", "fa-IR", true));
            Add(new Language(11, "fi", "fin", "Finnish", "suomi", "fi-FI", true));
            Add(new Language(17, "fr", "fra", "French (Canada)", "français", "fr-CA", false));
            Add(new Language(17, "fr", "fra", "French", "français", "fr-FR", true));
            Add(new Language(145, "gl", "glg", "Galician (Spain)", "galego", "gl-ES", true));
            Add(new Language(24, "he", "heb", "Hebrew (Israel)", "עברית", "he-IL", true));
            Add(new Language(152, "hi", "hin", "Hindi (India)", "हिन्दी", "hi-IN", true));
            Add(new Language(19, "hu", "hun", "Hungarian", "Magyar", "hu-HU", true));
            Add(new Language(161, "id", "ind", "Indonesian", "Bahasa Indonesia", "id-ID", true));
            Add(new Language(15, "it", "ita", "Italian", "italiano", "it-IT", true));
            Add(new Language(25, "ja", "jpn", "Japanese", "日本語", "ja-JP", true));
            Add(new Language(168, "ka", "kat", "Georgian", "ქართული", "ka-GE", true));
            Add(new Language(170, "kk", "kaz", "Kazakh", "қазақ тілі", "kk-KZ", true));
            Add(new Language(166, "kn", "kan", "Kannada (India)", "ಕನ್ನಡ", "kn-IN", true));
            Add(new Language(32, "ko", "kor", "Korean", "한국어", "ko-KR", true));
            Add(new Language(184, "lt", "lit", "Lithuanian", "lietuvių kalba", "lt-LT", true));
            Add(new Language(181, "lv", "lav", "Latvian", "latviešu valoda", "lv-LV", true));
            Add(new Language(189, "ml", "mal", "Malayalam (India)", "മലയാളം", "ml-IN", false));
            Add(new Language(189, "ml", "mal", "Malay (Malaysia)", "മലയാളം", "ms-MY", true));
            Add(new Language(189, "ml", "mal", "Malay (Singapore)", "മലയാളം", "ms-SG", false));
            Add(new Language(13, "nl", "nld", "Dutch (Netherlands)", "Nederlands", "nl-NL", true));
            Add(new Language(9, "no", "nor", "Norwegian", "Norsk bokmål", "no-NO", true));
            Add(new Language(18, "pl", "pol", "Polish", "język polski", "pl-PL", true));
            Add(new Language(26, "pt", "por", "Portuguese (Brazil)", "Português - Brasil", "pt-BR", false));
            Add(new Language(214, "pt", "por", "Portuguese", "Português - Portugal", "pt-PT", true));
            Add(new Language(218, "ro", "ron", "Romanian", "limba română", "ro-RO", true));
            Add(new Language(22, "ru", "rus", "Russian", "русский язык", "ru-RU", true));
            Add(new Language(222, "si", "sin", "Sinhala (Sri Lanka)", "සිංහල", "si-LK", true));
            Add(new Language(30, "sk", "slk", "Slovak", "slovenčina", "sk-SK", true));
            Add(new Language(223, "sl", "slv", "Slovenian", "slovenski jezik", "sl-SI", true));
            Add(new Language(230, "sq", "sqi", "Albanian", "gjuha shqipe", "sq-AL", true));
            Add(new Language(232, "sr", "srp", "Serbian", "српски језик", "sr-RS", true));
            Add(new Language(8, "sv", "swe", "Swedish", "svenska", "sv-SE", true));
            Add(new Language(237, "ta", "tam", "Tamil (India)", "தமிழ்", "ta-IN", true));
            Add(new Language(239, "te", "tel", "Telugu (India)", "తెలుగు", "te-IN", true));
            Add(new Language(242, "th", "tha", "Thai", "ไทย", "th-TH", true));
            Add(new Language(241, "tl", "tgl", "Tagalog (Philippines)", "Wikang Tagalog", "tl-PH", true));
            Add(new Language(21, "tr", "tur", "Turkish", "Türkçe", "tr-TR", true));
            Add(new Language(250, "uk", "ukr", "Ukrainian", "українська мова", "uk-UA", true));
            Add(new Language(254, "vi", "vie", "Vietnamese", "Tiếng Việt", "vi-VN", true));
            Add(new Language(27, "zh", "zho", "Chinese", "大陆简体", "zh-CN", true));
            Add(new Language(27, "zh", "zho", "Chinese (Hong Kong)", "大陆简体", "zh-HK", false));
            Add(new Language(27, "zh", "zho", "Chinese (Taiwan)", "大陆简体", "zh-TW", false));
            Add(new Language(262, "zu", "zul", "Zulu (South Africa)", "isiZulu", "zu-ZA", true));
        }

        public Language? GetLanguageFromCode(string? languageAbbreviation)
        {
            return this.SingleOrDefault(l => l.Abbreviation == languageAbbreviation && l.IsPrimary);
        }

        public Language? GetLanguageFromLocalName(string? language)
        {
            return this.SingleOrDefault(l => l.LocalName == language && l.IsPrimary);
        }

        // ReSharper disable once UnusedMember.Global
        public Language? GetLanguageFromId(int languageId)
        {
            return this.SingleOrDefault(l => l.TvdbId == languageId && l.IsPrimary);
        }

        public Language? LanguageFromDialectCode(string iso)
        {
            return this.SingleOrDefault(l => l.ISODialectAbbreviation == iso);
        }

        public Language? GetLanguageFromThreeCode(string threeLetterIsoCode)
        {
            return this.SingleOrDefault(l => l.ThreeAbbreviation == threeLetterIsoCode && l.IsPrimary);
        }
    }
}
