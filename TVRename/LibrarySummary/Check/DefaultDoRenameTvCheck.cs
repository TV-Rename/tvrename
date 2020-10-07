using JetBrains.Annotations;

namespace TVRename
{
    class DefaultDoRenameTvCheck : DefaultTvShowCheck
    {
        public DefaultDoRenameTvCheck([NotNull] ShowConfiguration movie) : base(movie)         { }

        protected override string FieldName => "Rename Check";

        protected override bool Field => Show.DoRename;

        protected override bool Default => TVSettings.Instance.DefShowDoRenaming;

        protected override void FixInternal()
        {
            Show.DoRename = Default;
        }
    }
}