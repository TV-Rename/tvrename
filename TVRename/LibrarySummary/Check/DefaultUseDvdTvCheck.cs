using JetBrains.Annotations;

namespace TVRename
{
    internal class DefaultUseDvdTvCheck : DefaultTvShowCheck
    {
        public DefaultUseDvdTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        [NotNull]
        protected override string FieldName => "Use DVD Order Check";

        protected override bool Field => Show.DvdOrder;

        protected override bool Default => TVSettings.Instance.DefShowDVDOrder;

        protected override void FixInternal()
        {
            Show.DvdOrder = Default;
        }
    }

    internal class DefaultUseAlternateTvCheck : DefaultTvShowCheck
    {
        public DefaultUseAlternateTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        [NotNull]
        protected override string FieldName => "Use Alternate Order Check";

        protected override bool Field => Show.AlternateOrder;

        protected override bool Default => TVSettings.Instance.DefShowAlternateOrder;

        protected override void FixInternal()
        {
            Show.AlternateOrder = Default;
        }
    }
}
