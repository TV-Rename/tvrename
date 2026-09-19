using System.Collections.Generic;
using System.Linq;

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
