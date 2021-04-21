using JetBrains.Annotations;

namespace TVRename
{
    internal class DefaultDoMissingTvCheck : DefaultTvShowCheck
    {
        public DefaultDoMissingTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc) { }

        protected override string FieldName => "Do Missing Check";

        protected override bool Field => Show.DoMissingCheck;

        protected override bool Default => TVSettings.Instance.DefShowDoMissingCheck;

        protected override void FixInternal()
        {
            Show.DoMissingCheck = Default;
        }
    }
}
