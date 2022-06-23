//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Concurrent;

namespace TVRename;

internal class UrlCache
{
    private readonly ConcurrentDictionary<string, string> internalCache = new();

    public string GetUrl(string s, bool instanceSearchJsonUseCloudflare)
    {
        if (internalCache.TryGetValue(s, out string value))
        {
            return value;
        }

        string newValue = HttpHelper.GetUrl(s, instanceSearchJsonUseCloudflare);
        internalCache.TryAdd(s, newValue);
        return newValue;
    }
}
