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
        private FolderMonitor mFM;

        public FolderMonitorProgress(FolderMonitor thefm)
        {
            this.mFM = thefm;
            this.InitializeComponent();
            timer1_Tick(null, null); // force immediate initial update
        }

        public void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.mFM.FMPStopNow = true;
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (this.mFM == null)
                return;

            timer1.Stop();

            this.pbProgress.Value = this.mFM.FMPPercent;
            this.lbMessage.Text = this.mFM.FMPUpto;
            
            if (this.mFM.FMPStopNow)
                this.Close();

            timer1.Start();

        }

        private void FolderMonitorProgress_Load(object sender, System.EventArgs e)
        {
            this.Ready = true;
        }
    }
}