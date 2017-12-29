// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    /// <summary>
    /// Summary for TorrentMatch
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class TorrentMatch : Form
    {
        private SetProgressDelegate _setProgress;
        private TVDoc _mDoc;

        public TorrentMatch(TVDoc doc, SetProgressDelegate prog)
        {
            _setProgress = prog;
            _mDoc = doc;

            InitializeComponent();

            TabBtEnableDisable();
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void bnGo_Click(object sender, System.EventArgs e)
        {
            _mDoc.RenameFilesToMatchTorrent(txtTorrentFile.Text, txtFolder.Text, tmatchTree, _setProgress, rbBTCopyTo.Checked, 
                                                txtBTSecondLocation.Text, _mDoc.Args);
        }

        private void rbBTRenameFiles_CheckedChanged(object sender, System.EventArgs e)
        {
            TabBtEnableDisable();
        }

        private void TabBtEnableDisable()
        {
            bool e = rbBTCopyTo.Checked;

            txtBTSecondLocation.Enabled = e;
            bnBTSecondBrowse.Enabled = e;
            bnBTSecondOpen.Enabled = e;
        }

        private void rbBTCopyTo_CheckedChanged(object sender, System.EventArgs e)
        {
            TabBtEnableDisable();
        }

        private void bnBTOpenFolder_Click(object sender, System.EventArgs e)
        {
            Helpers.SysOpen(txtFolder.Text);
        }

        private void bnBrowseFolder_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFolder.Text))
                folderBrowser.SelectedPath = txtFolder.Text;
            else if (!string.IsNullOrEmpty(txtTorrentFile.Text))
            {
                FileInfo fi = new FileInfo(txtTorrentFile.Text);
                if (fi != null)
                    folderBrowser.SelectedPath = fi.DirectoryName;
            }
            if (folderBrowser.ShowDialog() == DialogResult.OK)
                txtFolder.Text = folderBrowser.SelectedPath;
        }

        private void bnBrowseTorrent_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTorrentFile.Text))
            {
                FileInfo fi = new FileInfo(txtTorrentFile.Text);
                if (fi != null)
                {
                    openFile.InitialDirectory = fi.DirectoryName;
                    openFile.FileName = fi.Name;
                }
            }
            else if (!string.IsNullOrEmpty(txtFolder.Text))
            {
                openFile.InitialDirectory = txtFolder.Text;
                openFile.FileName = "";
            }

            if (!string.IsNullOrEmpty(txtTorrentFile.Text))
            {
                FileInfo fi = new FileInfo(txtTorrentFile.Text);
                if (fi.Exists)
                    openFile.FileName = txtTorrentFile.Text;
            }

            if (openFile.ShowDialog() == DialogResult.OK)
                txtTorrentFile.Text = openFile.FileName;
        }

        private void bnBTSecondBrowse_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBTSecondLocation.Text))
                folderBrowser.SelectedPath = txtBTSecondLocation.Text;
            else if (!string.IsNullOrEmpty(txtTorrentFile.Text))
            {
                FileInfo fi = new FileInfo(txtTorrentFile.Text);
                if (fi != null)
                    folderBrowser.SelectedPath = fi.DirectoryName;
            }
            if (folderBrowser.ShowDialog() == DialogResult.OK)
                txtFolder.Text = folderBrowser.SelectedPath;
        }

        private void bnBTSecondOpen_Click(object sender, System.EventArgs e)
        {
            Helpers.SysOpen(txtBTSecondLocation.Text);
        }
    }
}
