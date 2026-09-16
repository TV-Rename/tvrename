//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TVRename;

internal class UrlCache
{
    private readonly ConcurrentDictionary<string, string> internalCache = new();

    public async Task<string> GetUrlAsync(string s, bool instanceSearchJsonUseCloudflare)
    {
        if (internalCache.TryGetValue(s, out string? value))
        {
            return value;
        }

        string newValue = await HttpHelper.GetUrlAsync(s, instanceSearchJsonUseCloudflare);
        try
        {
            internalCache.TryAdd(s, newValue);
        }
        catch (OverflowException)
        {
            return newValue;
        }
        return newValue;
    }
}
