//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public static class TvRenameExtensions
{
    public static T LongestShowName<T>(this IEnumerable<T> media) where T : MediaConfiguration
    {
        IEnumerable<T> mediaConfigurations = media as T[] ?? [.. media];
        int longestName = mediaConfigurations.Max(configuration => configuration.ShowName.Length);
        return mediaConfigurations.First(config => config.ShowName.Length == longestName);
    }
}
