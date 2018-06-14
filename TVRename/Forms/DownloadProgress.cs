// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private readonly CacheUpdater mDoc;

        public DownloadProgress(CacheUpdater doc)
        {
            InitializeComponent();

            mDoc = doc;
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            tmrUpdate.Stop();
            mDoc.StopBGDownloadThread();
            DialogResult = DialogResult.Abort;
        }

        private void tmrUpdate_Tick(object sender, System.EventArgs e)
        {
            if (mDoc.DownloadDone)
                Close();
            else
                UpdateStuff();
        }

        private void DownloadProgress_Load(object sender, System.EventArgs e)
        {
            //UpdateStuff();
        }

        private void UpdateStuff()
        {
            txtCurrent.Text = TheTVDB.Instance.CurrentDLTask;
            pbProgressBar.Value = mDoc.DownloadPct;
        }
    }
}
