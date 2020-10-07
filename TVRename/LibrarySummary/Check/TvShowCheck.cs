namespace TVRename
{
    internal abstract class TvShowCheck : SettingsCheck
    {
        public readonly ShowConfiguration Show;
        protected TvShowCheck(ShowConfiguration show)
        {
            Show = show;
        }
        public override MediaConfiguration.MediaType Type() => MediaConfiguration.MediaType.tv;
        public override string MediaName => Show.ShowName;
    }
}