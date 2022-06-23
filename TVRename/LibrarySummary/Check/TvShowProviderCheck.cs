namespace TVRename;

internal class TvShowProviderCheck : TvShowCheck
{
    public TvShowProviderCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
    {
    }

    public override bool Check() => Show.ConfigurationProvider != TVDoc.ProviderType.libraryDefault;

    public override string Explain() => $"TV Show does not use the library default, ({TVSettings.Instance.DefaultProvider.PrettyPrint()}), it uses {Show.ConfigurationProvider.PrettyPrint()} (Hardcoded)";

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
