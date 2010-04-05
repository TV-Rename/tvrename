// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public class ActionCopyMoveRename : ActionItem
    {
        #region Op enum

        public enum Op
        {
            Copy,
            Move,
            Rename
        }

        #endregion

        public FileInfo From;

        public Op Operation;
        public FileInfo To;

        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep)
            : base(ActionType.kCopyMoveRename, ep)
        {
            this.Operation = operation;
            this.From = from;
            this.To = to;
        }

        public bool IsMoveRename() // same thing to the OS
        {
            return ((this.Operation == Op.Move) || (this.Operation == Op.Rename));
        }

        public override IgnoreItem GetIgnore()
        {
            if (this.To == null)
                return null;
            return new IgnoreItem(this.To.FullName);
        }

        public override int IconNumber()
        {
            return (this.IsMoveRename() ? 4 : 3);
        }

        public override string TargetFolder()
        {
            if (this.To == null)
                return null;
            return this.To.DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return this.To.Name;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            if (this.PE == null)
            {
                lvi.Text = "";
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
            }
            else
            {
                lvi.Text = this.PE.TheSeries.Name;
                lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
                lvi.SubItems.Add(this.PE.NumsAsString());
                DateTime? dt = this.PE.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }

            lvi.SubItems.Add(this.From.DirectoryName);
            lvi.SubItems.Add(this.From.Name);
            lvi.SubItems.Add(this.To.DirectoryName);
            lvi.SubItems.Add(this.To.Name);

            if (this.Operation == Op.Rename)
                lvi.Group = lv.Groups[1];
            else if (this.Operation == Op.Copy)
                lvi.Group = lv.Groups[2];
            else if (this.Operation == Op.Move)
                lvi.Group = lv.Groups[3];

            //lv->Items->Add(lvi);
            return lvi;
        }

        public bool SameSource(ActionCopyMoveRename o)
        {
            return (Helpers.Same(this.From, o.From));
        }

        public bool SameAs2(ActionCopyMoveRename o)
        {
            return ((this.Operation == o.Operation) && Helpers.Same(this.From, o.From) && Helpers.Same(this.To, o.To));
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionCopyMoveRename) (o));
        }

        public long FileSize()
        {
            try
            {
                return this.From.Length;
            }
            catch
            {
                return 1;
            }
        }
    }
}