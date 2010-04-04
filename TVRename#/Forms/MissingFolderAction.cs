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
	/// Summary for MissingFolderAction
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public enum FAResult: int
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
		public FAResult Result;
		public string FolderName;

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
	         this.Close();
         }

        private void bnIgnoreAlways_Click(object sender, System.EventArgs e)
         {
	         Result = FAResult.kfaIgnoreAlways;
	         this.Close();
         }

        private void bnCreate_Click(object sender, System.EventArgs e)
         {
	         Result = FAResult.kfaCreate;
	         this.Close();
         }

        private void bnRetry_Click(object sender, System.EventArgs e)
         {
	         Result = FAResult.kfaRetry;
	         this.Close();
         }

        private void bnCancel_Click(object sender, System.EventArgs e)
         {
	         Result = FAResult.kfaCancel;
	         this.Close();
         }

        private void bnBrowse_Click(object sender, System.EventArgs e)
         {
	         folderBrowser.SelectedPath = FolderName;
	         if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
	         {
		         Result = FAResult.kfaDifferentFolder;
		         FolderName = folderBrowser.SelectedPath;
		         this.Close();
	         }
         }

        private void MissingFolderAction_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
         {
	         if (!e.Data.GetDataPresent(DataFormats.FileDrop))
		         e.Effect = DragDropEffects.None;
	         else
		         e.Effect = DragDropEffects.Copy;
         }

        private void MissingFolderAction_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
         {
	         string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop));
	         for (int i =0;i<files.Length;i++)
	         {
		         string path = files[i];
		         DirectoryInfo di;
		         try
		         {
			         di = new DirectoryInfo(path);
			         if (di.Exists)
			         {
				         FolderName = path;
				         Result = FAResult.kfaDifferentFolder;
				         this.Close();
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