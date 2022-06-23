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
            ActionCopyMoveRename => 3,
            ActionMoveRenameDirectory => 4,
            ActionTDownload => 5,
            ActionDownloadImage => 6,
            ActionMede8erViewXML => 7,
            ActionMede8erXML => 8,
            ActionNfo => 9,
            ActionPyTivoMeta => 10,
            ActionWdtvMeta => 11,
            ItemDownloading => 12,
            ActionDeleteFile => 13,
            ActionDeleteDirectory => 14,
            ActionDateTouchEpisode => 15,
            ActionDateTouchSeason => 16,
            ActionDateTouchMedia => 17,
            ActionDateTouchMovie => 18,
            ActionTRemove => 19,
            ActionUnArchive =>20,
            _ => throw new NotSupportedException()
        };
    }
}
