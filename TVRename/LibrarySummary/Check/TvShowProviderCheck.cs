using JetBrains.Annotations;

namespace TVRename
{
    class TvShowProviderCheck : TvShowCheck
    {
        public TvShowProviderCheck([NotNull] ShowConfiguration show) : base(show)
        {
        }

        public override bool Check() => Show.ConfigurationProvider!=TVDoc.ProviderType.libraryDefault;

        public override string Explain() => $"TV Show does not use the library default, it uses {Show.ConfigurationProvider.PrettyPrint()}";

        protected override void FixInternal()
        {
            if (Show.HasIdOfType(TVSettings.Instance.DefaultProvider))
            {
                Show.ConfigurationProvider = TVDoc.ProviderType.libraryDefault;
            }
            else
            {
                throw new FixCheckException($"Could not update provider for {MediaName}. It did not have an Id for {TVSettings.Instance.DefaultProvider}");
            }
        }
        public override string CheckName => "[TV] Use default source provider";
    }
}
