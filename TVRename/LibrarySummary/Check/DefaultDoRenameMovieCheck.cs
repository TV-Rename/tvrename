using JetBrains.Annotations;

namespace TVRename
{
    internal class DefaultDoRenameMovieCheck : DefaultMovieCheck
    {
        public DefaultDoRenameMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        [NotNull]
        protected override string FieldName => "Rename Check";

        protected override bool Field => Movie.DoRename;

        protected override bool Default => TVSettings.Instance.DefMovieDoRenaming;

        protected override void FixInternal()
        {
            Movie.DoRename = Default;
        }
    }
}
