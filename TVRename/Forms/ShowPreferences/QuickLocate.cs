using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.Forms.ShowPreferences
{
    public partial class frmQuickLocate : Form
    {
        public string DirectoryFullPath;

        public frmQuickLocate(string hint)
        {
            InitializeComponent();

            cbDirectory.SuspendLayout();
            cbDirectory.Items.Clear();
            cbDirectory.Items.AddRange(TVSettings.Instance.LibraryFolders.ToArray());
            cbDirectory.SelectedIndex = 0;
            cbDirectory.ResumeLayout();

            txtShowFolder.Text = Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(hint)) + Path.DirectorySeparatorChar;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
              SetDirectory();
                DialogResult = DialogResult.OK;
                Close();
        }

        private void SetDirectory()
        {
            DirectoryFullPath = cbDirectory.Text + txtShowFolder.Text;
        }
    }
}
