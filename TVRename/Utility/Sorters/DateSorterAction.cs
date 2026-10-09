//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


using BrightIdeasSoftware;

namespace TVRename
{
    public class DateSorterAction : ObjectListViewComparer<DateTime>
    {
        public DateSorterAction(int column) : base(column)
        {
        }

        protected override DateTime GetValue(OLVListItem lvi, int columnId)
        {
            try
            {
                if (lvi.RowObject is Item a)
                {
                    return a.AirDate ?? DateTime.Now;
                }

                return DateTime.Now;
            }
            catch
            {
                return DateTime.Now;
            }
        }
    }
}
