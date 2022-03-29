using JetBrains.Annotations;

namespace TVRename
{
    internal abstract class CustomTvShowCheck : TvShowCheck
    {
        protected CustomTvShowCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        [NotNull]
        public override string CheckName => "[TV] " + FieldName;
        protected abstract string FieldName { get; }
        protected abstract bool Field { get; }
        protected abstract string CustomFieldValue { get; }
        protected abstract string DefaultFieldValue { get; }

        public override bool Check() => Field;

        [NotNull]
        public override string Explain() => $"{FieldName} is enabled for this TV Show [{CustomFieldValue}], by default is is not [{DefaultFieldValue}].";
    }
}
