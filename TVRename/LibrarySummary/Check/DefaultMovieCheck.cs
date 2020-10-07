using JetBrains.Annotations;

namespace TVRename
{
    abstract class DefaultMovieCheck : MovieCheck
    {
        protected DefaultMovieCheck([NotNull] MovieConfiguration movie) : base(movie)
        {
        }
        public override string CheckName => "[Movie] "+FieldName;
        protected abstract string FieldName { get; }
        protected abstract bool Field { get; }
        protected abstract bool Default { get; }

        public override bool Check()
        {
            return Field != Default;
        }

        public override string Explain()
        {
            return $"Default value for {FieldName} is {Default}. For {Movie.ShowName} it is {Field}.";
        }
    }
}