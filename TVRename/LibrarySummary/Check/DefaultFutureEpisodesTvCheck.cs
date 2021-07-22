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
}
