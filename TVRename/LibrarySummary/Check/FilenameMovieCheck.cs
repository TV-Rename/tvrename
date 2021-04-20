using System;
using JetBrains.Annotations;

namespace TVRename
{
    internal class FilenameMovieCheck : MovieCheck
    {
        public FilenameMovieCheck([NotNull] MovieConfiguration movie) : base(movie) { }

        public override bool Check() => Movie.UseCustomNamingFormat;

        public override string Explain() => $"This movie does not use the standard file naming format '{TVSettings.Instance.MovieFilenameFormat}', it uses '{Movie.CustomNamingFormat}'";

        protected override void FixInternal()
        {
            //string currentLocation = 
            //todo - move files
            //throw new NotImplementedException();
        }
        public override string CheckName => "[Movie] Use Custom File Name Format";
    }
}
