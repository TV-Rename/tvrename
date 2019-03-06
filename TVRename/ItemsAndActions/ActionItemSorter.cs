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
            if (x is null)
            {
                return -1;
            }

            if (y is null)
            {
                return 1;
            }

            return (x.GetType() == y.GetType()) ? x.Compare(y) : (TypeNumber(x) - TypeNumber(y));
        }

        #endregion

        private static int TypeNumber(Item a)
        {
            if (a is ItemMissing)
            {
                return 1;
            }

            if (a is ActionCopyMoveRename)
            {
                return 2;
            }

            if (a is ActionTDownload)
            {
                return 3;
            }

            if (a is ActionDownloadImage)
            {
                return 4;
            }

            if (a is ActionMede8erViewXML)
            {
                return 5;
            }

            if (a is ActionMede8erXML)
            {
                return 6;
            }

            if (a is ActionNfo)
            {
                return 7;
            }

            if (a is ActionPyTivoMeta)
            {
                return 8;
            }

            if (a is ActionWdtvMeta)
            {
                return 9;
            }

            if (a is ItemDownloading)
            {
                return 10;
            }

            if (a is ActionDeleteFile)
            {
                return 11;
            }

            if (a is ActionDeleteDirectory)
            {
                return 12;
            }

            if (a is ActionDateTouch)
            {
                return 13;
            }

            return 14;
        }
    }
}
