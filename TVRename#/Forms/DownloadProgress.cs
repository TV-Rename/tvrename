// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
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
        private readonly TVDoc _mDoc;

        public DownloadProgress(TVDoc doc)
        {
            InitializeComponent();

            _mDoc = doc;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            tmrUpdate.Stop();
            _mDoc.StopBgDownloadThread();
            DialogResult = DialogResult.Abort;
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (_mDoc.DownloadDone)
                Close();
            else
                UpdateStuff();
        }

        private void DownloadProgress_Load(object sender, EventArgs e)
        {
            //UpdateStuff();
        }

        private void UpdateStuff()
        {
            txtCurrent.Text = TheTVDB.Instance.CurrentDlTask;
            pbProgressBar.Value = _mDoc.DownloadPct;
        }
    }
}
