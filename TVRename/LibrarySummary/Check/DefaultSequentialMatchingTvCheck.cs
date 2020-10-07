using JetBrains.Annotations;

namespace TVRename
{
    class DefaultSequentialMatchingTvCheck : DefaultTvShowCheck
    {
        public DefaultSequentialMatchingTvCheck([NotNull] ShowConfiguration movie) : base(movie) { }

        protected override string FieldName => "Do Sequential Matching Check";

        protected override bool Field => Show.UseSequentialMatch;

        protected override bool Default => TVSettings.Instance.DefShowSequentialMatching;

        protected override void FixInternal()
        {
            Show.UseSequentialMatch = Default;
        }
    }
}