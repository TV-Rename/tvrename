using JetBrains.Annotations;

namespace TVRename
{
    internal class DefaultDoRenameTvCheck : DefaultTvShowCheck
    {
        public DefaultDoRenameTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override string FieldName => "Rename Check";

        protected override bool Field => Show.DoRename;

        protected override bool Default => TVSettings.Instance.DefShowDoRenaming;

        protected override void FixInternal()
        {
            Show.DoRename = Default;
        }
    }
}
