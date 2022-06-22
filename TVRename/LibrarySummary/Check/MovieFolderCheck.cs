namespace TVRename
{
    internal class MovieFolderCheck : MovieCheck
    {
        public MovieFolderCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc) {}

        protected override string MovieCheckName => "Use either manual or automatic folders";

        public override bool Check() => !Movie.UseAutomaticFolders && !Movie.UseManualLocations;

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
