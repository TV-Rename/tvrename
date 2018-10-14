namespace TVRename
{
    public class SeriesSpecifier
    {
        public readonly int seriesId;
        public readonly bool useCustomLanguage;
        public readonly string customLanguageCode;

        public SeriesSpecifier(int key, bool useCustomLanguage, string customLanguageCode)
        {
            this.seriesId = key;
            this.useCustomLanguage = useCustomLanguage;
            this.customLanguageCode = customLanguageCode;
        }
    }
}
