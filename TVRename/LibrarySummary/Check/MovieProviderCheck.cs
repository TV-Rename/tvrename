using JetBrains.Annotations;

namespace TVRename
{
    class MovieProviderCheck : MovieCheck
    {
        public MovieProviderCheck([NotNull] MovieConfiguration m) : base(m)
        {
        }

        public override bool Check() => Movie.ConfigurationProvider != TVDoc.ProviderType.libraryDefault;

        public override string Explain() => $"{MediaName} does not use the library default, it uses {Movie.ConfigurationProvider.PrettyPrint()}";

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