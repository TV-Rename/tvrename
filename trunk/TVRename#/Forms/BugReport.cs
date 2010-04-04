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
	/// Summary for BugReport
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public partial class BugReport : Form
	{
		private TVDoc mDoc;

		public BugReport(TVDoc doc)
		{
			mDoc = doc;
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

	    private void bnCreate_Click(object sender, System.EventArgs e)
	    {
		    txtEmailText.Text = "Working... This may take a while.";
		    txtEmailText.Update();

		    string txt = "";
		    txt += "From: " + txtName.Text + " <" + txtEmail.Text + ">" + "\r\n";
		    txt += "Subject: TVRename bug report" + "\r\n";
		    txt += "\r\n";
		    txt += "TVRename version: " + Version.DisplayVersionString() + "\r\n";
		    txt += "UserAppDataPath is " + System.Windows.Forms.Application.UserAppDataPath + "\r\n";
		    txt += "EpGuidePath is " + UI.EpGuidePath() + "\r\n";
		    txt += "\r\n";
		    txt += "==== Brief Description ====" + "\r\n";
		    txt += txtDesc1.Text + "\r\n";
		    txt += "\r\n";
		    txt += "==== Description ====" + "\r\n";
		    txt += txtDesc2.Text + "\r\n";
		    txt += "\r\n";
		    txt += "==== Frequency ====" + "\r\n";
		    txt += txtFreq.Text + "\r\n";
		    txt += "\r\n";
		    txt += "==== Notes and Comments ====" + "\r\n";
		    txt += txtComments.Text + "\r\n";
		    txt += "\r\n";


		    if (cbSettings.Checked)
		    {
			    txt += "==== Settings Files ====" + "\r\n";
			    txt += "\r\n";
			    txt += "---- TVRenameSettings.xml" + "\r\n";
			    txt += "\r\n";
			    try
			    {
				    StreamReader sr = new StreamReader(System.Windows.Forms.Application.UserAppDataPath+System.IO.Path.DirectorySeparatorChar.ToString()+"TVRenameSettings.xml");
				    txt += sr.ReadToEnd();
				    sr.Close();
				    txt += "\r\n";
			    }
			    catch
			    {
				    txt += "Error reading TVRenameSettings.xml\r\n";
			    }
			    txt += "\r\n";
		    }

		    if (cbFOScan.Checked || cbFolderScan.Checked)
		    {
			    txt += "==== Filename processors ====\r\n";
			    foreach (FilenameProcessorRE s in mDoc.Settings.FNPRegexs)
				    txt += (s.Enabled ? "Enabled":"Disabled") + " \""+s.RE+"\" "+(s.UseFullPath?"(FullPath)":"")+"\r\n";
			    txt += "\r\n";
		    }

		    if (cbFOScan.Checked)
		    {
			    txt += "==== Finding & Organising Directory Scan ====" + "\r\n";
			    txt += "\r\n";

                DirCache dirC = new DirCache();
                foreach (string efi in mDoc.SearchFolders)
                    dirC.AddFolder(null, 0, 0, efi, true, mDoc.Settings);

			    foreach (DirCacheEntry fi in dirC)
			    {
				    int seas;
				    int ep;
				    bool r = mDoc.FindSeasEp(fi.TheFile, out seas, out ep, null);
				    bool useful = fi.HasUsefulExtension_NotOthersToo;
				    txt += fi.TheFile.FullName + " ("+(r?"OK":"No")+" " + seas.ToString()+","+ep.ToString()+" "+(useful?fi.TheFile.Extension:"-")+")" + "\r\n";
			    }
			    txt += "\r\n";
		    }

		    if (cbFolderScan.Checked)
		    {
			    txt += "==== Media Folders Directory Scan ====" + "\r\n";

			    foreach (ShowItem si in mDoc.GetShowItems(true))
			    {
				    foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> kvp in si.SeasonEpisodes)
				    {
					    int snum = kvp.Key;
					    if (((snum == 0) && (si.CountSpecials)) || !si.AllFolderLocations(mDoc.Settings).ContainsKey(snum))
						    continue; // skip specials


					    foreach (string folder in si.AllFolderLocations(mDoc.Settings)[snum])
					    {
						    txt += si.TVDBCode + " : " + si.ShowName() + " : S" + snum.ToString() + "\r\n";
						    txt += "Folder: " + folder;
						    txt += "\r\n";
                            DirCache files = new DirCache();
                            if (Directory.Exists(folder))
                                files.AddFolder(null, 0, 0, folder, true, mDoc.Settings);
						    foreach (DirCacheEntry fi in files)
						    {
							    int seas;
							    int ep;
							    bool r = mDoc.FindSeasEp(fi.TheFile, out seas, out ep, si.ShowName());
							    bool useful = fi.HasUsefulExtension_NotOthersToo;
							    txt += fi.TheFile.FullName + " ("+(r?"OK":"No")+" " + seas.ToString()+","+ep.ToString()+" "+(useful?fi.TheFile.Extension:"-")+")" + "\r\n";
						    }
						    txt += "\r\n";
					    }
				    }
				    txt += "\r\n";
			    }
			    mDoc.UnlockShowItems();

			    txt += "\r\n";
		    }

		    txtEmailText.Text = txt;
	    }

	    private void bnCopy_Click(object sender, System.EventArgs e)
		{
				 Clipboard.SetDataObject(txtEmailText.Text);
		}
	}
}