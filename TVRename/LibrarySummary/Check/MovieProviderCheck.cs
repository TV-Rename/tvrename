using JetBrains.Annotations;

namespace TVRename
{
    internal class MovieProviderCheck : MovieCheck
    {
        public MovieProviderCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        public override bool Check() => Movie.ConfigurationProvider != TVDoc.ProviderType.libraryDefault;

        public override string Explain() => $"This movie does not use the library default ({TVSettings.Instance.DefaultMovieProvider.PrettyPrint()}), it uses {Movie.ConfigurationProvider.PrettyPrint()} (Hardcoded)";

        protected override void FixInternal()
        {
            if (Movie.HasIdOfType(TVSettings.Instance.DefaultMovieProvider))
            {
                Movie.ConfigurationProvider = TVDoc.ProviderType.libraryDefault;
            }
            else
            {
                throw new FixCheckException($"Could not update provider for {MediaName}. It did not have an Id for {TVSettings.Instance.DefaultMovieProvider}");
            }
        }
        public override string CheckName => "[Movie] Use default source provider";
    }
}
