//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

using System.Windows.Forms;
using System.IO;


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
		private FileInfo[] SettingsList;
		private FileInfo[] DBList;

		public FileInfo SettingsFile;

		public FileInfo DBFile;

		public RecoverXML(string hint)
		{
			InitializeComponent();
			SettingsFile = null;
			DBFile = null;
			if (!string.IsNullOrEmpty(hint))
				lbHint.Text = hint+"\r\n ";
		}

        private void RecoverXML_Load(object sender, System.EventArgs e)
         {
	         SettingsList = new DirectoryInfo(System.Windows.Forms.Application.UserAppDataPath).GetFiles("TVRenameSettings.xml*");
	         DBList = new DirectoryInfo(System.Windows.Forms.Application.UserAppDataPath).GetFiles("TheTVDB.xml*");

	         lbSettings.Items.Add("Default settings");
	         if ((SettingsList != null) && SettingsList.Length > 0)
	         {
		         foreach (FileInfo fi in SettingsList)
			         lbSettings.Items.Add(fi.LastWriteTime.ToString("g"));
		         lbSettings.SelectedIndex = 0;
	         }

	         lbDB.Items.Add("None");
	         if ((DBList != null) && DBList.Length>0)
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
	         DBFile = (n == 0) ? null : DBList[n-1];

	         n = lbSettings.SelectedIndex;
	         if (n == -1)
		         n = 0;
	         SettingsFile = (n == 0) ? null : SettingsList[n];

	         this.DialogResult =DialogResult.OK;
	         this.Close();
         }

        private void bnCancel_Click(object sender, System.EventArgs e)
         {
	         this.DialogResult =DialogResult.Cancel;
	         this.Close();
         }
    }
}