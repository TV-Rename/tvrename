// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
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
                ItemMissing _ => 1,
                ActionCopyMoveRename _ => 2,
                ActionTDownload _ => 3,
                ActionDownloadImage _ => 4,
                ActionMede8erViewXML _ => 5,
                ActionMede8erXML _ => 6,
                ActionNfo _ => 7,
                ActionPyTivoMeta _ => 8,
                ActionWdtvMeta _ => 9,
                ItemDownloading _ => 10,
                ActionDeleteFile _ => 11,
                ActionDeleteDirectory _ => 12,
                ActionDateTouchEpisode _ => 13,
                ActionDateTouchSeason _ => 14,
                ActionDateTouchShow _ => 15,
                ActionTRemove _ => 16
            };
        }
    }
}
