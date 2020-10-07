using JetBrains.Annotations;

namespace TVRename
{
    class DefaultSpecialsAsEpisodesTvCheck : DefaultTvShowCheck
    {
        public DefaultSpecialsAsEpisodesTvCheck([NotNull] ShowConfiguration movie) : base(movie) { }

        protected override string FieldName => "Count Specials As Episodes Check";

        protected override bool Field => Show.CountSpecials;

        protected override bool Default => TVSettings.Instance.DefShowSpecialsCount;

        protected override void FixInternal()
        {
            Show.CountSpecials = Default;
        }
    }
}