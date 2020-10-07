using JetBrains.Annotations;

namespace TVRename
{
    class DefaultFutureEpisodesTvCheck : DefaultTvShowCheck
    {
        public DefaultFutureEpisodesTvCheck([NotNull] ShowConfiguration movie) : base(movie) { }

        protected override string FieldName => "Do Future Episodes Check";

        protected override bool Field => Show.ForceCheckFuture;

        protected override bool Default => TVSettings.Instance.DefShowIncludeFuture;

        protected override void FixInternal()
        {
            Show.ForceCheckFuture = Default;
        }
    }
}