using JetBrains.Annotations;

namespace TVRename
{
    internal class DefaultDoMissingMovieCheck : DefaultMovieCheck
    {
        public DefaultDoMissingMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        [NotNull]
        protected override string FieldName => "Do Missing Check";

        protected override bool Field => Movie.DoMissingCheck;

        protected override bool Default => TVSettings.Instance.DefMovieDoMissingCheck;

        protected override void FixInternal()
        {
            Movie.DoMissingCheck = Default;
        }
    }
}
