//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class TvShowProviderCheck(ShowConfiguration show, TVDoc doc) : TvShowCheck(show, doc)
{
    public override bool Check() => Show.ConfigurationProvider != TVDoc.ProviderType.libraryDefault;

    public override string Explain() => $"TV Show does not use the library default, ({TVSettings.Instance.DefaultProvider.PrettyPrint()}), it uses {Show.ConfigurationProvider.PrettyPrint()} (Hardcoded)";

    /// <exception cref="FixCheckException">Condition.</exception>
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
