// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Windows.Forms;

namespace TVRename
{
    public partial class FolderMonitorProgress : Form
    {
        public bool StopNow = false;
        public bool Ready = false;
        private FolderMonitor _mFm;

        public FolderMonitorProgress(FolderMonitor thefm)
        {
            _mFm = thefm;
            InitializeComponent();
            timer1_Tick(null, null); // force immediate initial update
        }

        public void bnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            _mFm.FmpStopNow = true;
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (_mFm == null)
                return;

            timer1.Stop();

            BringToFront();

            pbProgress.Value = _mFm.FmpPercent;
            lbMessage.Text = _mFm.FmpUpto;
            
            if (_mFm.FmpStopNow)
                Close();

            timer1.Start();

        }

        private void FolderMonitorProgress_Load(object sender, System.EventArgs e)
        {
            Ready = true;
        }
    }
}
