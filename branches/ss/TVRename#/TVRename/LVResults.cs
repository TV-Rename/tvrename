// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
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

        public bool AllSameType;

        public System.Collections.Generic.List<ActionCopyMoveRename> CopyMove;
        public int Count;
        public System.Collections.Generic.List<ActionDownload> Download;
        public ItemList FlatList;
        public System.Collections.Generic.List<ActionMissing> Missing;
        public System.Collections.Generic.List<ActionNFO> NFO;
        public System.Collections.Generic.List<ActionRSS> RSS;
        public System.Collections.Generic.List<ActionCopyMoveRename> Rename;
        public System.Collections.Generic.List<ActionuTorrenting> uTorrenting;

        public LVResults(ListView lv, bool @checked) // if not checked, then selected items
        {
            this.Go(lv, @checked ? WhichResults.Checked : WhichResults.Selected);
        }

        public LVResults(ListView lv, WhichResults which)
        {
            this.Go(lv, which);
        }

        public void Go(ListView lv, WhichResults which)
        {
            this.uTorrenting = new System.Collections.Generic.List<ActionuTorrenting>();
            this.Missing = new System.Collections.Generic.List<ActionMissing>();
            this.RSS = new System.Collections.Generic.List<ActionRSS>();
            this.CopyMove = new System.Collections.Generic.List<ActionCopyMoveRename>();
            this.Rename = new System.Collections.Generic.List<ActionCopyMoveRename>();
            this.Download = new System.Collections.Generic.List<ActionDownload>();
            this.NFO = new System.Collections.Generic.List<ActionNFO>();
            this.FlatList = new ItemList();

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

            this.Count = sel.Count;

            if (sel.Count == 0)
                return;

            ActionType t = ((ActionItem) (sel[0].Tag)).Type;

            this.AllSameType = true;
            foreach (ListViewItem lvi in sel)
            {
                if (lvi == null)
                    continue;

                ActionItem Action = (ActionItem) (lvi.Tag);
                this.FlatList.Add(Action);
                ActionType t2 = Action.Type;
                if (t2 != t)
                    this.AllSameType = false;

                switch (t2)
                {
                    case ActionType.kCopyMoveRename:
                        {
                            ActionCopyMoveRename cmr = (ActionCopyMoveRename) (Action);
                            if (cmr.Operation == ActionCopyMoveRename.Op.Rename)
                                this.Rename.Add(cmr);
                            else // copy/move
                                this.CopyMove.Add(cmr);
                            break;
                        }
                    case ActionType.kDownload:
                        this.Download.Add((ActionDownload) (Action));
                        break;
                    case ActionType.kRSS:
                        this.RSS.Add((ActionRSS) (Action));
                        break;
                    case ActionType.kMissing:
                        this.Missing.Add((ActionMissing) (Action));
                        break;
                    case ActionType.kNFO:
                        this.NFO.Add((ActionNFO) (Action));
                        break;
                    case ActionType.kuTorrenting:
                        this.uTorrenting.Add((ActionuTorrenting) (Action));
                        break;
                }
            }
        }
    }
}