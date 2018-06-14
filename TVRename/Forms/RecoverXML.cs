// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System.Windows.Forms;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo ;
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
        private FileInfo[] availableFiles;

        public FileInfo SettingsFile;
        private FileInfo[] settingsList;

        public RecoverXML(string hint)
        {
            InitializeComponent();
            SettingsFile = null;
            DbFile = null;
            if (!string.IsNullOrEmpty(hint))
                lbHint.Text = hint + "\r\n ";
        }

        private void RecoverXML_Load(object sender, System.EventArgs e)
        {
            settingsList = new DirectoryInfo(System.IO.Path.GetDirectoryName(PathManager.TVDocSettingsFile.FullName)).GetFiles(PathManager.SettingsFileName + "*");
            availableFiles = new DirectoryInfo(System.IO.Path.GetDirectoryName(PathManager.TVDBFile.FullName)).GetFiles(PathManager.TvdbFileName + "*");

            lbSettings.Items.Add("Default settings");
            if ((settingsList != null) && settingsList.Length > 0)
            {
                foreach (FileInfo fi in settingsList)
                    lbSettings.Items.Add(fi.LastWriteTime.ToString("g"));
                lbSettings.SelectedIndex = 0;
            }

            lbDB.Items.Add("None");
            if ((availableFiles != null) && availableFiles.Length > 0)
            {
                foreach (FileInfo fi in availableFiles)
                    lbDB.Items.Add(fi.LastWriteTime.ToString("g"));
                lbDB.SelectedIndex = 0;
            }
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            // we added a 'none' item at the top of the list, so adjust for that

            int n = lbDB.SelectedIndex;
            if (n == -1)
                n = 0;
            DbFile = (n == 0) ? null : availableFiles[n - 1];

            n = lbSettings.SelectedIndex;
            if (n == -1)
                n = 0;
            SettingsFile = (n == 0) ? null : settingsList[n];

            DialogResult = DialogResult.OK;
            Close();
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
