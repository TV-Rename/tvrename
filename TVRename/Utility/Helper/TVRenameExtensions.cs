using System.Collections.Generic;
using System.Linq;

namespace TVRename;

public static class TvRenameExtensions
{
    public static T LongestShowName<T>(this IEnumerable<T> media) where T : MediaConfiguration
    {
        IEnumerable<T> mediaConfigurations = media as T[] ?? media.ToArray();
        int longestName = mediaConfigurations.Select(configuration => configuration.ShowName.Length).Max();
        return mediaConfigurations.First(config => config.ShowName.Length == longestName);
    }
}
