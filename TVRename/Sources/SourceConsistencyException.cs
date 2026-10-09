//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

[Serializable]
// ReSharper disable once InconsistentNaming
public class SourceConsistencyException : Exception
{
    // Thrown if an error occurs in the XML when reading TheTVDB.xml
    public SourceConsistencyException(string message, TVDoc.ProviderType provider)
        : base(provider.PrettyPrint() + ": " + message)
    {
    }

    public SourceConsistencyException(string message, TVDoc.ProviderType provider, Exception ex)
        : base(provider.PrettyPrint() + ": " + message, ex)
    {
    }
}
