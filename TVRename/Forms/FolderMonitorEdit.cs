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
            InitializeComponent();

            mTCCF = new TheTVDBCodeFinder("");
            mTCCF.Dock = DockStyle.Fill;
            mTCCF.SelectionChanged += CodeChanged;
            mTCCF.lvMatches.DoubleClick += MatchDoubleClick;


            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(mTCCF);
            pnlCF.ResumeLayout();

            if (hint.CodeKnown)
                mTCCF.SetHint(hint.TVDBCode.ToString());
            else
            {
                string s = hint.Folder;
                int p = s.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                mTCCF.SetHint(s.Substring(p+1));
            }
            Code = -1;
        }

        private void MatchDoubleClick(object sender, EventArgs e)
        {
            bnOK_Click(null, null);
        }

        private void CodeChanged(object sender, EventArgs e)
        {
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Code = mTCCF.SelectedCode();
            Close();
        }
    }
}
