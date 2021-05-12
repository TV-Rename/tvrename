// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.Forms.ShowPreferences
{
    public partial class QuickLocateForm : Form
    {
        public string? DirectoryFullPath;
        public string? RootDirectory;
        private readonly string InitialFolder;
        public QuickLocateForm(string hint, MediaConfiguration.MediaType t)
        {
            InitializeComponent();

            cbDirectory.SuspendLayout();
            cbDirectory.Items.Clear();
            List<string> folders = t == MediaConfiguration.MediaType.tv ?  TVSettings.Instance.LibraryFolders : TVSettings.Instance.MovieLibraryFolders;
            foreach (string folder in folders)
            {
                cbDirectory.Items.Add(folder.TrimEnd(Path.DirectorySeparatorChar.ToString()));
            }
            cbDirectory.SelectedIndex = 0;
            cbDirectory.ResumeLayout();

            InitialFolder= Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(hint));
            txtShowFolder.Text = InitialFolder;
        }

        public bool FolderNameChanged => txtShowFolder.Text != InitialFolder;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DirectoryFullPath = cbDirectory.Text + txtShowFolder.Text;
            RootDirectory = cbDirectory.Text;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
