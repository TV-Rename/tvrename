// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
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
    public enum FaResult
    {
        KfaNotSet,
        KfaRetry,
        KfaCancel,
        KfaCreate,
        KfaIgnoreOnce,
        KfaIgnoreAlways,
        KfaDifferentFolder
    }

    public partial class MissingFolderAction : Form
    {
        public string FolderName;
        public FaResult Result;

        public MissingFolderAction(string showName, string season, string folderName)
        {
            InitializeComponent();

            Result = FaResult.KfaCancel;
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

        private void bnIgnoreOnce_Click(object sender, EventArgs e)
        {
            Result = FaResult.KfaIgnoreOnce;
            Close();
        }

        private void bnIgnoreAlways_Click(object sender, EventArgs e)
        {
            Result = FaResult.KfaIgnoreAlways;
            Close();
        }

        private void bnCreate_Click(object sender, EventArgs e)
        {
            Result = FaResult.KfaCreate;
            Close();
        }

        private void bnRetry_Click(object sender, EventArgs e)
        {
            Result = FaResult.KfaRetry;
            Close();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Result = FaResult.KfaCancel;
            Close();
        }

        private void bnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowser.SelectedPath = FolderName;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                Result = FaResult.KfaDifferentFolder;
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
                        Result = FaResult.KfaDifferentFolder;
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
