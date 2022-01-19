using JetBrains.Annotations;

namespace TVRename
{
    internal class MovieFolderCheck : MovieCheck
    {
        public MovieFolderCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc) {}

        public override string CheckName => "[Movie] Use either manual or automatec folders";

        public override bool Check() => !Movie.UseAutomaticFolders && !Movie.UseManualLocations;

        [NotNull]
        public override string Explain() => $"{Movie.Name} does not use automated nor manual folders";

        protected override void FixInternal()
        {
            if (!TVSettings.Instance.DefMovieUseAutomaticFolders)
            {
                throw new FixCheckException($"Please manually assign automatic/manual directory for {Movie.Name}");
            }

            Movie.UseAutomaticFolders = true;
        }
    }
}
