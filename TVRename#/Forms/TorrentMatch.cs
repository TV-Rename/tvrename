// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private SetProgressDelegate SetProgress;
        private TVDoc mDoc;

        public TorrentMatch(TVDoc doc, SetProgressDelegate prog)
        {
            this.SetProgress = prog;
            this.mDoc = doc;

            this.InitializeComponent();

            this.TabBTEnableDisable();
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void bnGo_Click(object sender, System.EventArgs e)
        {
            this.mDoc.RenameFilesToMatchTorrent(this.txtTorrentFile.Text, this.txtFolder.Text, this.tmatchTree, this.SetProgress, this.rbBTCopyTo.Checked, 
                                                this.txtBTSecondLocation.Text, mDoc.Args);
        }

        private void rbBTRenameFiles_CheckedChanged(object sender, System.EventArgs e)
        {
            this.TabBTEnableDisable();
        }

        private void TabBTEnableDisable()
        {
            bool e = this.rbBTCopyTo.Checked;

            this.txtBTSecondLocation.Enabled = e;
            this.bnBTSecondBrowse.Enabled = e;
            this.bnBTSecondOpen.Enabled = e;
        }

        private void rbBTCopyTo_CheckedChanged(object sender, System.EventArgs e)
        {
            this.TabBTEnableDisable();
        }

        private void bnBTOpenFolder_Click(object sender, System.EventArgs e)
        {
            Helpers.SysOpen(this.txtFolder.Text);
        }

        private void bnBrowseFolder_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtFolder.Text))
                this.folderBrowser.SelectedPath = this.txtFolder.Text;
            else if (!string.IsNullOrEmpty(this.txtTorrentFile.Text))
            {
                FileInfo fi = new FileInfo(this.txtTorrentFile.Text);
                if (fi != null)
                    this.folderBrowser.SelectedPath = fi.DirectoryName;
            }
            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtFolder.Text = this.folderBrowser.SelectedPath;
        }

        private void bnBrowseTorrent_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtTorrentFile.Text))
            {
                FileInfo fi = new FileInfo(this.txtTorrentFile.Text);
                if (fi != null)
                {
                    this.openFile.InitialDirectory = fi.DirectoryName;
                    this.openFile.FileName = fi.Name;
                }
            }
            else if (!string.IsNullOrEmpty(this.txtFolder.Text))
            {
                this.openFile.InitialDirectory = this.txtFolder.Text;
                this.openFile.FileName = "";
            }

            if (!string.IsNullOrEmpty(this.txtTorrentFile.Text))
            {
                FileInfo fi = new FileInfo(this.txtTorrentFile.Text);
                if (fi.Exists)
                    this.openFile.FileName = this.txtTorrentFile.Text;
            }

            if (this.openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtTorrentFile.Text = this.openFile.FileName;
        }

        private void bnBTSecondBrowse_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtBTSecondLocation.Text))
                this.folderBrowser.SelectedPath = this.txtBTSecondLocation.Text;
            else if (!string.IsNullOrEmpty(this.txtTorrentFile.Text))
            {
                FileInfo fi = new FileInfo(this.txtTorrentFile.Text);
                if (fi != null)
                    this.folderBrowser.SelectedPath = fi.DirectoryName;
            }
            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtFolder.Text = this.folderBrowser.SelectedPath;
        }

        private void bnBTSecondOpen_Click(object sender, System.EventArgs e)
        {
            Helpers.SysOpen(this.txtBTSecondLocation.Text);
        }
    }
}