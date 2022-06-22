//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Generic;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    /// <summary>
    /// Summary for RecoverXML
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class RecoverXml : Form
    {
        public FileInfo? TvDbFile;
        private readonly FileInfo[] availableTvDbFiles;

        public FileInfo? TvMazeFile;
        private readonly FileInfo[] availableTvMazeFiles;

        public FileInfo? TmdbFile;
        private readonly FileInfo[] availableTmdbFiles;

        public FileInfo? SettingsFile;
        private readonly FileInfo[] settingsList;

        public RecoverXml(string? hint)
        {
            InitializeComponent();
            settingsList = PathManager.GetPossibleSettingsHistory();
            availableTvDbFiles = PathManager.GetPossibleTvdbHistory();
            availableTvMazeFiles = PathManager.GetPossibleTvMazeHistory();
            availableTmdbFiles = PathManager.GetPossibleTmdbHistory();

            if (!string.IsNullOrEmpty(hint))
            {
                lbHint.Text = hint + "\r\n ";
            }
        }

        private void RecoverXML_Load(object sender, System.EventArgs e)
        {
            Setup(lbSettings, "Default settings", settingsList);
            Setup(lbTVDB, "No Cache", availableTvDbFiles);
            Setup(lbTvMaze, "No Cache", availableTvMazeFiles);
            Setup(lbTMDB, "No Cache", availableTmdbFiles);
        }

        private static void Setup(ListBox lb, string defaultValue, IReadOnlyCollection<FileInfo> files)
        {
            lb.Items.Add(defaultValue);
            if (files.Count > 0)
            {
                foreach (FileInfo fi in files)
                {
                    lb.Items.Add(fi.LastWriteTime.ToString("g"));
                }

                lb.SelectedIndex = 0;
            }
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            TvDbFile = GetFile(lbTVDB, availableTvDbFiles);
            TvMazeFile = GetFile(lbTvMaze, availableTvMazeFiles);
            TmdbFile = GetFile(lbTMDB, availableTmdbFiles);
            SettingsFile = GetFile(lbSettings, settingsList);
            DialogResult = DialogResult.OK;
            Close();
        }

        private static FileInfo? GetFile(ListControl lb, IReadOnlyList<FileInfo> fileInfos)
        {
            // we added a 'none' item at the top of the list, so adjust for that
            int n = lb.SelectedIndex;
            if (n == -1)
            {
                n = 0;
            }
            return n == 0 ? null : fileInfos[n - 1];
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void label5_Click(object sender, System.EventArgs e)
        {
        }
    }
}
