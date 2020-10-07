namespace TVRename
{
    abstract class MovieCheck : SettingsCheck
    {
        public readonly MovieConfiguration Movie;
        protected MovieCheck(MovieConfiguration movie)
        {
            Movie = movie;
        }

        public override MediaConfiguration.MediaType Type() => MediaConfiguration.MediaType.movie;
        public override string MediaName => Movie.ShowName;
    }
}