using JetBrains.Annotations;

namespace TVRename
{
    abstract class CustomTvShowCheck : TvShowCheck
    {
        protected CustomTvShowCheck([NotNull] ShowConfiguration show) : base(show)
        {
        }
        public override string CheckName => "[TV] " + FieldName;
        protected abstract string FieldName { get; }
        protected abstract bool Field { get; }
        public override bool Check()
        {
            return Field;
        }

        public override string Explain()
        {
            return $"{FieldName} is enabled for {MediaName}, by default is is not.";
        }
    }
}