//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;

namespace TVRename;

public class ActionItemSorter : System.Collections.Generic.IComparer<Item>
{
    #region IComparer<Item> Members

    public int Compare(Item? x, Item? y)
    {
        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        return TypeNumber(x) == TypeNumber(y) ? x.CompareTo(y) : TypeNumber(x) - TypeNumber(y);
    }

    #endregion IComparer<Item> Members

    private static int TypeNumber(Item a)
    {
        return a switch
        {
            ShowItemMissing => 1,
            MovieItemMissing => 2,
            ShowSeasonMissing =>3,
            ActionCopyMoveRename => 4,
            ActionMoveRenameDirectory => 5,
            ActionTDownload => 6,
            ActionDownloadImage => 7,
            ActionMede8erViewXML => 8,
            ActionMede8erXML => 9,
            ActionNfo => 10,
            ActionPyTivoMeta => 11,
            ActionWdtvMeta => 12,
            ItemDownloading => 13,
            ActionDeleteFile => 14,
            ActionDeleteDirectory => 15,
            ActionDateTouchEpisode => 16,
            ActionDateTouchSeason => 17,
            ActionDateTouchMedia => 18,
            ActionDateTouchMovie => 19,
            ActionTRemove => 20,
            ActionUnArchive =>21,
            _ => throw new NotSupportedException()
        };
    }
}
