using JetBrains.Annotations;

namespace TVRename
{
    abstract class DefaultTvShowCheck : TvShowCheck
    {
        protected DefaultTvShowCheck([NotNull] ShowConfiguration movie) : base(movie)
        {
        }
        public override string CheckName => "[TV] " + FieldName;
        protected abstract string FieldName { get; }
        protected abstract bool Field { get; }
        protected abstract bool Default { get; }

        public override bool Check()
        {
            return Field != Default;
        }

        public override string Explain()
        {
            return $"Default value for {FieldName} is {Default}. For {MediaName} it is {Field}.";
        }
    }
}