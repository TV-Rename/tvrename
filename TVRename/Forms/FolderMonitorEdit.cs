// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    using System;
    using System.Windows.Forms;

    public partial class FolderMonitorEdit : Form
    {
        public int Code;

        private TheTVDBCodeFinder mTCCF;

        public FolderMonitorEdit(FolderMonitorEntry hint)
        {
            this.InitializeComponent();

            this.mTCCF = new TheTVDBCodeFinder("");
            this.mTCCF.Dock = DockStyle.Fill;
            this.mTCCF.SelectionChanged += this.CodeChanged;
            this.mTCCF.lvMatches.DoubleClick += this.MatchDoubleClick;


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

        private void MatchDoubleClick(object sender, EventArgs e)
        {
            this.bnOK_Click(null, null);
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
