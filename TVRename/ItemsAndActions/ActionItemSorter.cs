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
                ActionMoveRenameDirectory _ =>4,
                ActionTDownload _ => 5,
                ActionDownloadImage _ => 6,
                ActionMede8erViewXML _ => 7,
                ActionMede8erXML _ => 8,
                ActionNfo _ => 9,
                ActionPyTivoMeta _ => 10,
                ActionWdtvMeta _ => 11,
                ItemDownloading _ => 12,
                ActionDeleteFile _ => 13,
                ActionDeleteDirectory _ => 14,
                ActionDateTouchEpisode _ => 15,
                ActionDateTouchSeason _ => 16,
                ActionDateTouchMedia _ => 17,
                ActionDateTouchMovie _ => 18,
                ActionTRemove _ => 19,
                _ => throw new NotSupportedException()
            };
        }
    }
}
