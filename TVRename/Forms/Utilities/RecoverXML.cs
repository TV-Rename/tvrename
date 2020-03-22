// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Windows.Forms;
using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

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
        public FileInfo TvDbFile;
        private FileInfo[] availableTvDbFiles;

        public FileInfo TvMazeFile;
        private FileInfo[] availableTvMazeFiles;

        public FileInfo SettingsFile;
        private FileInfo[] settingsList;

        public RecoverXml([CanBeNull] string hint)
        {
            InitializeComponent();
            SettingsFile = null;
            TvDbFile = null;
            TvMazeFile = null;
            if (!string.IsNullOrEmpty(hint))
            {
                lbHint.Text = hint + "\r\n ";
            }
        }

        private void RecoverXML_Load(object sender, System.EventArgs e)
        {
            settingsList = PathManager.GetPossibleSettingsHistory();
            availableTvDbFiles = PathManager.GetPossibleTvdbHistory();
            availableTvMazeFiles = PathManager.GetPossibleTvMazeHistory();

            lbSettings.Items.Add("Default settings");
            if (settingsList != null && settingsList.Length > 0)
            {
                foreach (FileInfo fi in settingsList)
                {
                    lbSettings.Items.Add(fi.LastWriteTime.ToString("g"));
                }

                lbSettings.SelectedIndex = 0;
            }

            lbTVDB.Items.Add("None");
            if (availableTvDbFiles != null && availableTvDbFiles.Length > 0)
            {
                foreach (FileInfo fi in availableTvDbFiles)
                {
                    lbTVDB.Items.Add(fi.LastWriteTime.ToString("g"));
                }

                lbTVDB.SelectedIndex = 0;
            }

            lbTvMaze.Items.Add("None");
            if (availableTvMazeFiles != null && availableTvMazeFiles.Length > 0)
            {
                foreach (FileInfo fi in availableTvMazeFiles)
                {
                    lbTvMaze.Items.Add(fi.LastWriteTime.ToString("g"));
                }

                lbTvMaze.SelectedIndex = 0;
            }
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            TvDbFile = GetFile(lbTVDB, availableTvDbFiles);
            TvMazeFile = GetFile(lbTvMaze, availableTvMazeFiles);
            SettingsFile = GetFile(lbSettings, settingsList);
            DialogResult = DialogResult.OK;
            Close();
        }

        [CanBeNull]
        private static FileInfo GetFile([NotNull] ListControl lb, IReadOnlyList<FileInfo> fileInfos)
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
    }
}
