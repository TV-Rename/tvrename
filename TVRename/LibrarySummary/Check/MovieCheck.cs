namespace TVRename
{
    internal abstract class MovieCheck : SettingsCheck
    {
        public readonly MovieConfiguration Movie;

        protected MovieCheck(MovieConfiguration movie, TVDoc doc) : base(doc)
        {
            Movie = movie;
        }

        protected override void MarkMediaDirty()
        {
            Movie.CachedMovie.Dirty = true;
        }

        public override MediaConfiguration.MediaType Type() => MediaConfiguration.MediaType.movie;

        public override string MediaName => Movie.ShowName;
    }
}
