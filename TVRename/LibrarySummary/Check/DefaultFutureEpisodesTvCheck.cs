using JetBrains.Annotations;

namespace TVRename
{
    internal class DefaultFutureEpisodesTvCheck : DefaultTvShowCheck
    {
        public DefaultFutureEpisodesTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override string FieldName => "Do Future Episodes Check";

        protected override bool Field => Show.ForceCheckFuture;

        protected override bool Default => TVSettings.Instance.DefShowIncludeFuture;

        protected override void FixInternal()
        {
            Show.ForceCheckFuture = Default;
        }
    }
    internal class DefaultFutureMovieCheck : DefaultMovieCheck
    {
        public DefaultFutureMovieCheck([NotNull] MovieConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override string FieldName => "Do Future Movies Check";

        protected override bool Field => Movie.ForceCheckFuture;

        protected override bool Default => TVSettings.Instance.CheckFutureDatedMovies;

        protected override void FixInternal()
        {
            Movie.ForceCheckFuture = Default;
        }
    }
}
