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
        public bool StopNow;
        private FolderMonitor mFM;

        public FolderMonitorProgress(FolderMonitor thefm)
        {
            this.mFM = thefm;

            this.InitializeComponent();
        }

        public void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.mFM.FMPStopNow = true;
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (this.mFM.FMPStopNow)
                this.Close();
        }
    }
}