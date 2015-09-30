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
    /// <summary>
    /// Summary for DownloadProgress
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class DownloadProgress : Form
    {
        private TVDoc mDoc;

        public DownloadProgress(TVDoc doc)
        {
            this.InitializeComponent();

            this.mDoc = doc;
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.tmrUpdate.Stop();
            this.mDoc.StopBGDownloadThread();
            this.DialogResult = DialogResult.Abort;
        }

        private void tmrUpdate_Tick(object sender, System.EventArgs e)
        {
            if (this.mDoc.DownloadDone)
                this.Close();
            else
                this.UpdateStuff();
        }

        private void DownloadProgress_Load(object sender, System.EventArgs e)
        {
            //UpdateStuff();
        }

        private void UpdateStuff()
        {
            this.txtCurrent.Text = TheTVDB.Instance.CurrentDLTask;
            this.pbProgressBar.Value = this.mDoc.DownloadPct;
        }
    }
}