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
    using System.Windows.Forms;

    public partial class FolderMonitorEdit : Form
    {
        public int Code;

        private TheTVDBCodeFinder mTCCF;

        public FolderMonitorEdit(TheTVDB db, FolderMonitorEntry hint)
        {
            this.InitializeComponent();

            this.mTCCF = new TheTVDBCodeFinder("", db);
            this.mTCCF.Dock = DockStyle.Fill;
            this.mTCCF.SelectionChanged += this.CodeChanged;

            this.pnlCF.SuspendLayout();
            this.pnlCF.Controls.Add(this.mTCCF);
            this.pnlCF.ResumeLayout();

            if (hint.CodeKnown)
                this.mTCCF.SetHint(hint.TVDBCode.ToString());
            else
            {
                string s = hint.Folder;
                int p = s.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                this.mTCCF.SetHint(s.Substring(p+1));
            }
            this.Code = -1;
        }

        private void CodeChanged(object sender, EventArgs e)
        {
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Code = this.mTCCF.SelectedCode();
            this.Close();
        }
    }
}