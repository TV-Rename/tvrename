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

        private FolderMonitor mFM;
        public bool StopNow;

        public FolderMonitorProgress(FolderMonitor thefm)
        {
            mFM = thefm;

            InitializeComponent();
        }

        public void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            mFM.FMPStopNow = true;
        }


        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (mFM.FMPStopNow)
                this.Close();
        }
    }
}