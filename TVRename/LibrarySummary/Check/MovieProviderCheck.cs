//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class MovieProviderCheck(MovieConfiguration movie, TVDoc doc) : MovieCheck(movie, doc)
{
    public override bool Check() => Movie.ConfigurationProvider != TVDoc.ProviderType.libraryDefault;

    public override string Explain() => $"This movie does not use the library default ({TVSettings.Instance.DefaultMovieProvider.PrettyPrint()}), it uses {Movie.ConfigurationProvider.PrettyPrint()} (Hardcoded)";

    /// <exception cref="FixCheckException">Condition.</exception>
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

    protected override string MovieCheckName => "Use default source provider";
}
