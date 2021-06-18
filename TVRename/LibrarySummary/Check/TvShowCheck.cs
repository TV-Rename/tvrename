namespace TVRename
{
    internal abstract class TvShowCheck : SettingsCheck
    {
        public readonly ShowConfiguration Show;

        protected TvShowCheck(ShowConfiguration show, TVDoc doc) : base(doc)
        {
            Show = show;
        }

        protected override void MarkMediaDirty()
        {
            if (Show.CachedShow != null)
            {
                Show.CachedShow.Dirty = true;
                Doc.TvAddedOrEdited(false,true,true,null, Show);
            }
        }

        public override MediaConfiguration.MediaType Type() => MediaConfiguration.MediaType.tv;

        public override string MediaName => Show.ShowName;
    }
}
