// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename
{
    public class LvResults
    {
        #region WhichResults enum

        public enum WhichResults
        {
            Checked,
            Selected,
            All
        }

        #endregion

        public bool AllSameType;

        public List<ActionCopyMoveRename> CopyMove;
        public int Count;
        public List<ActionDownload> Download;
        public ScanListItemList FlatList;
        public List<ItemMissing> Missing;
        public List<ActionNfo> Nfo;
        public List<ActionPyTivoMeta> PyTivoMeta;
        public List<ActionRss> Rss;
        public List<ActionCopyMoveRename> Rename;
        //public System.Collections.Generic.List<ItemuTorrenting> uTorrenting;

        public LvResults(ListView lv, bool isChecked) // if not checked, then selected items
        {
            Go(lv, isChecked ? WhichResults.Checked : WhichResults.Selected);
        }

        public LvResults(ListView lv, WhichResults which)
        {
            Go(lv, which);
        }

        public void Go(ListView lv, WhichResults which)
        {
            //this.uTorrenting = new System.Collections.Generic.List<ItemuTorrenting>();
            Missing = new List<ItemMissing>();
            Rss = new List<ActionRss>();
            CopyMove = new List<ActionCopyMoveRename>();
            Rename = new List<ActionCopyMoveRename>();
            Download = new List<ActionDownload>();
            Nfo = new List<ActionNfo>();
            PyTivoMeta = new List<ActionPyTivoMeta>();
            FlatList = new ScanListItemList();

            List<ListViewItem> sel = new List<ListViewItem>();
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

            Type firstType = ((Item) (sel[0].Tag)).GetType();

            AllSameType = true;
            foreach (ListViewItem lvi in sel)
            {
                if (lvi == null)
                    continue;

                Item action = (Item) (lvi.Tag);
                if (action is IScanListItem)
                    FlatList.Add(action as IScanListItem);

                if (action.GetType() != firstType)
                    AllSameType = false;

                if (action is ActionCopyMoveRename)
                {
                    ActionCopyMoveRename cmr = action as ActionCopyMoveRename;
                    if (cmr.Operation == ActionCopyMoveRename.Op.Rename)
                        Rename.Add(cmr);
                    else // copy/move
                        CopyMove.Add(cmr);
                }
                else if (action is ActionDownload)
                    Download.Add((ActionDownload) (action));
                else if (action is ActionRss)
                    Rss.Add((ActionRss) (action));
                else if (action is ItemMissing)
                    Missing.Add((ItemMissing) (action));
                else if (action is ActionNfo)
                    Nfo.Add((ActionNfo) (action));
                else if (action is ActionPyTivoMeta)
                    PyTivoMeta.Add((ActionPyTivoMeta) (action));
                //else if (action is ItemuTorrenting)
                //    this.uTorrenting.Add((ItemuTorrenting) (action));
            }
        }
    }
}
