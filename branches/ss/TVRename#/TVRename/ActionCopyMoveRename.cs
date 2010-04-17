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

    public class ActionCopyMoveRename : Item, Action, ScanListItem
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

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }
        public int IconNumber { get { return this.IsMoveRename() ? 4 : 3; } }
        public string ProgressText
        {
            get { return this.To.Name; }
        }
        public ProcessedEpisode Episode { get; set; }
        public IgnoreItem Ignore
        {
            get
            {
                if (this.To == null)
                    return null;
                return new IgnoreItem(this.To.FullName);
            }
        }
        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                if (this.Episode == null)
                {
                    lvi.Text = "";
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                }
                else
                {
                    lvi.Text = this.Episode.TheSeries.Name;
                    lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
                    lvi.SubItems.Add(this.Episode.NumsAsString());
                    DateTime? dt = this.Episode.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }

                lvi.SubItems.Add(this.From.DirectoryName);
                lvi.SubItems.Add(this.From.Name);
                lvi.SubItems.Add(this.To.DirectoryName);
                lvi.SubItems.Add(this.To.Name);

                return lvi;
            }
        }
        public int ScanListViewGroup
        {
            get
            {
                if (this.Operation == Op.Rename)
                    return 1;
                if (this.Operation == Op.Copy)
                    return 2;
                if (this.Operation == Op.Move)
                    return 3;
                return 2;
            }
        }
        public string TargetFolder
        {
            get
            {
                if (this.To == null)
                    return null;
                return this.To.DirectoryName;
            }
        }
        public int PercentDone { get { return Done ? 100 : 0; } } // 0 to 100
        public long SizeOfWork { get { return SourceFileSize(); } } // for file copy/move, number of bytes in file.  for simple tasks, 1.
        public bool SameAs(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;

            return ((cmr != null) && 
                    (this.Operation == cmr.Operation) && 
                    Helpers.Same(this.From, cmr.From) && 
                    Helpers.Same(this.To, cmr.To));
        }
        public bool Go(TVSettings settings)
        {
            // TODO: move non-UI code from CopyMoveProgressDialog to here
            throw new NotImplementedException();
        }
        public bool Stop()
        {
            return false;
        }
        // --------------------------------------------------------------------------------------------------------
        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep)
        {
            this.Episode = ep;

            this.Operation = operation;
            this.From = from;
            this.To = to;
        }
        
        public bool IsMoveRename() // same thing to the OS
        {
            return ((this.Operation == Op.Move) || (this.Operation == Op.Rename));
        }

        public bool SameSource(ActionCopyMoveRename o)
        {
            return (Helpers.Same(this.From, o.From));
        }

        // ========================================================================================================

        private long SourceFileSize()
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
        public int Compare(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;
            
            if (cmr == null)
                return 0;

            string s1 = this.From.FullName + (this.From.Directory.Root.FullName != this.To.Directory.Root.FullName ? "0" : "1");
            string s2 = cmr.From.FullName + (cmr.From.Directory.Root.FullName != cmr.To.Directory.Root.FullName ? "0" : "1");

            return s1.CompareTo(s2);
        }
    }
}