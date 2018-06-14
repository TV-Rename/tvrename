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
    /// Summary for MissingFolderAction
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public enum FAResult
    {
        kfaNotSet,
        kfaRetry,
        kfaCancel,
        kfaCreate,
        kfaIgnoreOnce,
        kfaIgnoreAlways,
        kfaDifferentFolder
    }

    public partial class MissingFolderAction : Form
    {
        public string FolderName;
        public FAResult Result;

        public MissingFolderAction(string showName, string season, string folderName)
        {
            InitializeComponent();

            Result = FAResult.kfaCancel;
            FolderName = folderName;
            txtShow.Text = showName;
            txtSeason.Text = season;
            txtFolder.Text = FolderName;

            if (string.IsNullOrEmpty(FolderName))
            {
                txtFolder.Text = "Click Browse..., or Drag+Drop a folder onto this window.";
                bnCreate.Enabled = false;
                bnRetry.Enabled = false;
            }
        }

        private void bnIgnoreOnce_Click(object sender, System.EventArgs e)
        {
            Result = FAResult.kfaIgnoreOnce;
            Close();
        }

        private void bnIgnoreAlways_Click(object sender, System.EventArgs e)
        {
            Result = FAResult.kfaIgnoreAlways;
            Close();
        }

        private void bnCreate_Click(object sender, System.EventArgs e)
        {
            Result = FAResult.kfaCreate;
            Close();
        }

        private void bnRetry_Click(object sender, System.EventArgs e)
        {
            Result = FAResult.kfaRetry;
            Close();
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            Result = FAResult.kfaCancel;
            Close();
        }

        private void bnBrowse_Click(object sender, System.EventArgs e)
        {
            folderBrowser.SelectedPath = FolderName;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                Result = FAResult.kfaDifferentFolder;
                FolderName = folderBrowser.SelectedPath;
                Close();
            }
        }

        private void MissingFolderAction_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void MissingFolderAction_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                    {
                        FolderName = path;
                        Result = FAResult.kfaDifferentFolder;
                        Close();
                        return;
                    }
                }
                catch
                {
                }
            }
        }
    }
}
