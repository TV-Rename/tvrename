// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.IO;
using System.Windows.Forms;

namespace TVRename
{
    public partial class FolderMonitorEdit : Form
    {
        public int Code;

        private readonly TheTVDBCodeFinder _mTccf;

        public FolderMonitorEdit(FolderMonitorEntry hint)
        {
            InitializeComponent();

            _mTccf = new TheTVDBCodeFinder("") {Dock = DockStyle.Fill};
            _mTccf.SelectionChanged += CodeChanged;
            _mTccf.lvMatches.DoubleClick += MatchDoubleClick;


            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(_mTccf);
            pnlCF.ResumeLayout();

            if (hint.CodeKnown)
                _mTccf.SetHint(hint.TVDBCode.ToString());
            else
            {
                string s = hint.Folder;
                int p = s.LastIndexOf(Path.DirectorySeparatorChar);
                _mTccf.SetHint(s.Substring(p+1));
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
            Code = _mTccf.SelectedCode();
            Close();
        }
    }
}
