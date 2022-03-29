using JetBrains.Annotations;

namespace TVRename
{
    internal abstract class CustomMovieCheck : MovieCheck
    {
        protected CustomMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        [NotNull]
        protected override string MovieCheckName => FieldName;
        protected abstract string FieldName { get; }
        protected abstract bool Field { get; }

        public override bool Check() => Field;

        [NotNull]
        public override string Explain() => $"{FieldName} is enabled for this Movie, by default is is not.";
    }
}
