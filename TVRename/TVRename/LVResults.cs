// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace TVRename
{
    using System.Windows.Forms;

    public class LvResults
    {
        #region WhichResults enum

        public enum WhichResults
        {
            Checked,
            selected,
            all
        }

        #endregion

        public System.Collections.Generic.List<ActionCopyMoveRename> CopyMove;
        public int Count;
        public System.Collections.Generic.List<ActionDownloadImage> Download;
        public ItemList FlatList;
        public System.Collections.Generic.List<ItemMissing> Missing;
        public System.Collections.Generic.List<ActionNfo> Nfo;
        public System.Collections.Generic.List<ActionPyTivoMeta> PyTivoMeta;
        public System.Collections.Generic.List<ActionTDownload> Rss;
        public System.Collections.Generic.List<ActionCopyMoveRename> Rename;

        public LvResults(ListView lv, bool isChecked) // if not checked, then selected items
        {
            Go(lv, isChecked ? WhichResults.Checked : WhichResults.selected);
        }

        public LvResults(ListView lv, WhichResults which)
        {
            Go(lv, which);
        }

        private void Go(ListView lv, WhichResults which)
        {
            Missing = new System.Collections.Generic.List<ItemMissing>();
            Rss = new System.Collections.Generic.List<ActionTDownload>();
            CopyMove = new System.Collections.Generic.List<ActionCopyMoveRename>();
            Rename = new System.Collections.Generic.List<ActionCopyMoveRename>();
            Download = new System.Collections.Generic.List<ActionDownloadImage>();
            Nfo = new System.Collections.Generic.List<ActionNfo>();
            PyTivoMeta = new System.Collections.Generic.List<ActionPyTivoMeta>();
            FlatList = new ItemList();

            System.Collections.Generic.List<ListViewItem> sel = new System.Collections.Generic.List<ListViewItem>();
            sel.AddRange(GetSelectionCollection(lv, which));

            Count = sel.Count;

            if (sel.Count == 0)
                return;

            foreach (ListViewItem lvi in sel)
            {
                if (lvi == null)
                    continue;

                Item action = (Item)(lvi.Tag);
                if (action != null)
                    FlatList.Add(action);

                switch (action)
                {
                    case ActionCopyMoveRename cmr when cmr.Operation == ActionCopyMoveRename.Op.rename:
                        Rename.Add(cmr);
                        break;
                    // copy/move
                    case ActionCopyMoveRename cmr:
                        CopyMove.Add(cmr);
                        break;
                    case ActionDownloadImage item:
                        Download.Add(item);
                        break;
                    case ActionTDownload rss:
                        Rss.Add(rss);
                        break;
                    case ItemMissing missing:
                        Missing.Add(missing);
                        break;
                    case ActionNfo nfo:
                        Nfo.Add(nfo);
                        break;
                    case ActionPyTivoMeta meta:
                        PyTivoMeta.Add(meta);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected value of action " + action?.GetType());
                }
            }
        }

        private static System.Collections.Generic.IEnumerable<ListViewItem> GetSelectionCollection(ListView lv, WhichResults which)
        {
            switch (which)
            {
                case WhichResults.Checked:
                    return lv.CheckedItems.Cast<ListViewItem>();
                case WhichResults.selected:
                    return lv.SelectedItems.Cast<ListViewItem>();
                case WhichResults.all:
                    return lv.Items.Cast<ListViewItem>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(which), which, null);
            }
        }
    }
}
