using System;
using JetBrains.Annotations;

namespace TVRename
{
    class FilenameMovieCheck : MovieCheck
    {
        public FilenameMovieCheck([NotNull] MovieConfiguration movie) : base(movie) { }

        public override bool Check() => Movie.UseCustomNamingFormat;

        public override string Explain() => $"{MediaName} does not use the standard Folder naming format {TVSettings.Instance.MovieFilenameFormat}, it uses {Movie.CustomNamingFormat}";

        protected override void FixInternal()
        {
            //todo - move files
            throw new NotImplementedException();
        }
        public override string CheckName => "[Movie] Use Custom File Name Format";
    }
}