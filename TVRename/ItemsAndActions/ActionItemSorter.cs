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

            return x.GetType() == y.GetType() ? x.Compare(y) : TypeNumber(x) - TypeNumber(y);
        }

        #endregion

        private static int TypeNumber(Item a)
        {
            switch (a)
            {
                case ItemMissing _:
                    return 1;

                case ActionCopyMoveRename _:
                    return 2;

                case ActionTDownload _:
                    return 3;

                case ActionDownloadImage _:
                    return 4;

                case ActionMede8erViewXML _:
                    return 5;

                case ActionMede8erXML _:
                    return 6;

                case ActionNfo _:
                    return 7;

                case ActionPyTivoMeta _:
                    return 8;

                case ActionWdtvMeta _:
                    return 9;

                case ItemDownloading _:
                    return 10;

                case ActionDeleteFile _:
                    return 11;

                case ActionDeleteDirectory _:
                    return 12;

                case ActionDateTouch _:
                    return 13;

                default:
                    return 14;
            }
        }
    }
}
