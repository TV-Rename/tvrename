// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;

namespace TVRename
{
    public class ActionItemSorter : System.Collections.Generic.IComparer<Item>
    {
        #region IComparer<Item> Members

        public int Compare(Item x, Item y)
        {
            return TypeNumber(x) == TypeNumber(y) ? x.CompareTo(y) : TypeNumber(x) - TypeNumber(y);
        }

        #endregion

        private static int TypeNumber(Item a)
        {
            return a switch
            { 
                ShowItemMissing _ => 1,
                MovieItemMissing _ => 2,
                ActionCopyMoveRename _ => 3,
                ActionTDownload _ => 4,
                ActionDownloadImage _ => 5,
                ActionMede8erViewXML _ => 6,
                ActionMede8erXML _ => 7,
                ActionNfo _ => 8,
                ActionPyTivoMeta _ => 9,
                ActionWdtvMeta _ => 10,
                ItemDownloading _ => 11,
                ActionDeleteFile _ => 12,
                ActionDeleteDirectory _ => 13,
                ActionDateTouchEpisode _ => 14,
                ActionDateTouchSeason _ => 15,
                ActionDateTouchMedia _ => 16,
                ActionDateTouchMovie _ => 17,
                ActionTRemove _ => 18,
                _ => throw new NotSupportedException()
            };
        }
    }
}
