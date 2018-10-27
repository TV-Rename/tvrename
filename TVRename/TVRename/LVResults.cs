// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    using System.Windows.Forms;

    public class LVResults
    {
        #region WhichResults enum

        public enum WhichResults
        {
            Checked,
            Selected,
            All
        }

        #endregion

        public System.Collections.Generic.List<ActionCopyMoveRename> CopyMove;
        public int Count;
        public System.Collections.Generic.List<ActionDownloadImage> Download;
        public ItemList FlatList;
        public System.Collections.Generic.List<ItemMissing> Missing;
        public System.Collections.Generic.List<ActionNfo> NFO;
        public System.Collections.Generic.List<ActionPyTivoMeta> PyTivoMeta;
        public System.Collections.Generic.List<ActionTDownload> RSS;
        public System.Collections.Generic.List<ActionCopyMoveRename> Rename;

        public LVResults(ListView lv, bool isChecked) // if not checked, then selected items
        {
            Go(lv, isChecked ? WhichResults.Checked : WhichResults.Selected);
        }

        public LVResults(ListView lv, WhichResults which)
        {
            Go(lv, which);
        }

        public void Go(ListView lv, WhichResults which)
        {
            Missing = new System.Collections.Generic.List<ItemMissing>();
            RSS = new System.Collections.Generic.List<ActionTDownload>();
            CopyMove = new System.Collections.Generic.List<ActionCopyMoveRename>();
            Rename = new System.Collections.Generic.List<ActionCopyMoveRename>();
            Download = new System.Collections.Generic.List<ActionDownloadImage>();
            NFO = new System.Collections.Generic.List<ActionNfo>();
            PyTivoMeta = new System.Collections.Generic.List<ActionPyTivoMeta>();
            FlatList = new ItemList();

            System.Collections.Generic.List<ListViewItem> sel = new System.Collections.Generic.List<ListViewItem>();
            if (which == WhichResults.Checked)
            {
                ListView.CheckedListViewItemCollection ss = lv.CheckedItems;
                foreach (ListViewItem lvi in ss)
                    sel.Add(lvi);
            }
            else if (which == WhichResults.Selected)
            {
                ListView.SelectedListViewItemCollection ss = lv.SelectedItems;
                foreach (ListViewItem lvi in ss)
                    sel.Add(lvi);
            }
            else // all
            {
                foreach (ListViewItem lvi in lv.Items)
                    sel.Add(lvi);
            }

            Count = sel.Count;

            if (sel.Count == 0)
                return;

            System.Type firstType = ((Item) (sel[0].Tag)).GetType();

            foreach (ListViewItem lvi in sel)
            {
                if (lvi == null)
                    continue;

                Item action = (Item) (lvi.Tag);
                if (action != null)
                    FlatList.Add(action);

                if (action is ActionCopyMoveRename cmr)
                {
                    if (cmr.Operation == ActionCopyMoveRename.Op.rename)
                        Rename.Add(cmr);
                    else // copy/move
                        CopyMove.Add(cmr);
                }
                else if (action is ActionDownloadImage item)
                    Download.Add(item);
                else if (action is ActionTDownload rss)
                    RSS.Add(rss);
                else if (action is ItemMissing missing)
                    Missing.Add(missing);
                else if (action is ActionNfo nfo)
                    NFO.Add(nfo);
                else if (action is ActionPyTivoMeta meta)
                    PyTivoMeta.Add(meta);
            }
        }
    }
}
