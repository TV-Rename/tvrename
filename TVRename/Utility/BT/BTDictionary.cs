//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//




namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTDictionary : BTItem
{
    public readonly List<BTDictionaryItem> Items;

    public BTDictionary()
        : base(BTChunk.kDictionary)
    {
        Items = [];
    }

    public BTItem? GetItem(string key) => GetItem(key, false);

    public BTItem? GetItem(string key, bool ignoreCase)
    {
        foreach (BTDictionaryItem t in Items)
        {
            if (string.Equals(t.Key, key, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
            {
                return t.Data;
            }
        }

        return null;
    }
}
