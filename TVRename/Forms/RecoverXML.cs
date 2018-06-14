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
        public FileInfo DBFile;
        private FileInfo[] DBList;

        public FileInfo SettingsFile;
        private FileInfo[] SettingsList;

        public RecoverXML(string hint)
        {
            InitializeComponent();
            SettingsFile = null;
            DBFile = null;
            if (!string.IsNullOrEmpty(hint))
                lbHint.Text = hint + "\r\n ";
        }

        private void RecoverXML_Load(object sender, System.EventArgs e)
        {
            SettingsList = new DirectoryInfo(System.IO.Path.GetDirectoryName(PathManager.TVDocSettingsFile.FullName)).GetFiles(PathManager.SettingsFileName + "*");
            DBList = new DirectoryInfo(System.IO.Path.GetDirectoryName(PathManager.TVDBFile.FullName)).GetFiles(PathManager.TVDBFileName + "*");

            lbSettings.Items.Add("Default settings");
            if ((SettingsList != null) && SettingsList.Length > 0)
            {
                foreach (FileInfo fi in SettingsList)
                    lbSettings.Items.Add(fi.LastWriteTime.ToString("g"));
                lbSettings.SelectedIndex = 0;
            }

            lbDB.Items.Add("None");
            if ((DBList != null) && DBList.Length > 0)
            {
                foreach (FileInfo fi in DBList)
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
            DBFile = (n == 0) ? null : DBList[n - 1];

            n = lbSettings.SelectedIndex;
            if (n == -1)
                n = 0;
            SettingsFile = (n == 0) ? null : SettingsList[n];

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
