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
using Path = System.IO.Path;

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
    public partial class RecoverXML : Form
    {
        public FileInfo DbFile;
        private FileInfo[] _dbList;

        public FileInfo SettingsFile;
        private FileInfo[] _settingsList;

        public RecoverXML(string hint)
        {
            InitializeComponent();
            SettingsFile = null;
            DbFile = null;
            if (!string.IsNullOrEmpty(hint))
                lbHint.Text = hint + "\r\n ";
        }

        private void RecoverXML_Load(object sender, EventArgs e)
        {
            _settingsList = new DirectoryInfo(Path.GetDirectoryName(PathManager.TVDocSettingsFile.FullName)).GetFiles(PathManager.SettingsFileName + "*");
            _dbList = new DirectoryInfo(Path.GetDirectoryName(PathManager.TVDBFile.FullName)).GetFiles(PathManager.TVDBFileName + "*");

            lbSettings.Items.Add("Default settings");
            if ((_settingsList != null) && _settingsList.Length > 0)
            {
                foreach (FileInfo fi in _settingsList)
                    lbSettings.Items.Add(fi.LastWriteTime.ToString("g"));
                lbSettings.SelectedIndex = 0;
            }

            lbDB.Items.Add("None");
            if ((_dbList != null) && _dbList.Length > 0)
            {
                foreach (FileInfo fi in _dbList)
                    lbDB.Items.Add(fi.LastWriteTime.ToString("g"));
                lbDB.SelectedIndex = 0;
            }
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            // we added a 'none' item at the top of the list, so adjust for that

            int n = lbDB.SelectedIndex;
            if (n == -1)
                n = 0;
            DbFile = (n == 0) ? null : _dbList[n - 1];

            n = lbSettings.SelectedIndex;
            if (n == -1)
                n = 0;
            SettingsFile = (n == 0) ? null : _settingsList[n];

            DialogResult = DialogResult.OK;
            Close();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
