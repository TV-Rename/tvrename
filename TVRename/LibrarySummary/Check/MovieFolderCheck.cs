namespace TVRename
{
    internal class MovieFolderCheck : MovieCheck
    {
        public MovieFolderCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc) {}

        public override string CheckName => "[Movie] Use either manual or automatec folders";

        public override bool Check()
        {
            return !Movie.UseAutomaticFolders && !Movie.UseManualLocations;
        }

        public override string Explain()
        {
            return $"{Movie.Name} does not use automated nor manual folders";
        }

        protected override void FixInternal()
        {
            if (TVSettings.Instance.DefMovieUseutomaticFolders)
            {
                Movie.UseAutomaticFolders = true;
                return;
            }

            throw new FixCheckException($"Please manually assign automatic/manual directory for {Movie.Name}");
        }
    }
}
