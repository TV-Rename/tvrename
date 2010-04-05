//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace TVRename
{

	/// <summary>
	/// Summary for FolderMonitor
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public partial class FolderMonitor : Form
	{
		private TheTVDBCodeFinder mTCCF;
		private TVDoc mDoc;
		private int mInternalChange;

		public FolderMonitor(TVDoc doc)
		{
			mDoc = doc;
			mInternalChange = 0;

			InitializeComponent();


			mTCCF = new TheTVDBCodeFinder("", mDoc.GetTVDB(false,""));
			mTCCF.Dock = DockStyle.Fill;
			mTCCF.SelectionChanged += new System.EventHandler(lvMatches_ItemSelectionChanged);

			pnlCF.SuspendLayout();
			pnlCF.Controls.Add(mTCCF);
			pnlCF.ResumeLayout();

			FillFolderStringLists();
		}
		public string FMPUpto;
		public int FMPPercent;
		public bool FMPStopNow;
		public FolderMonitorProgress FMP;

		public void FMPShower()
		{
			FMP = new FolderMonitorProgress(this);
			FMP.ShowDialog();
			FMP = null;
		}


	    private void bnClose_Click(object sender, System.EventArgs e)
        {
			 this.Close();
        }

        private void FillFolderStringLists()
        {
            lstFMIgnoreFolders.BeginUpdate();
            lstFMMonitorFolders.BeginUpdate();

            lstFMIgnoreFolders.Items.Clear();
            lstFMMonitorFolders.Items.Clear();

            mDoc.MonitorFolders.Sort();
            mDoc.IgnoreFolders.Sort();

            foreach (string folder in mDoc.MonitorFolders)
             lstFMMonitorFolders.Items.Add(folder);

            foreach (string folder in mDoc.IgnoreFolders)
             lstFMIgnoreFolders.Items.Add(folder);

            lstFMIgnoreFolders.EndUpdate();
            lstFMMonitorFolders.EndUpdate();
        }

        private void bnFMRemoveMonFolder_Click(object sender, System.EventArgs e)
        {
			 for (int i =lstFMMonitorFolders.SelectedIndices.Count-1;i>=0;i--)
			 {
				 int n = lstFMMonitorFolders.SelectedIndices[i];
				 mDoc.MonitorFolders.RemoveAt(n);
			 }
			 mDoc.SetDirty();
			 FillFolderStringLists();
        }

         private void bnFMRemoveIgFolder_Click(object sender, System.EventArgs e)
         {
	         for (int i =lstFMIgnoreFolders.SelectedIndices.Count-1;i>=0;i--)
	         {
		         int n = lstFMIgnoreFolders.SelectedIndices[i];
		         mDoc.IgnoreFolders.RemoveAt(n);
	         }
	         mDoc.SetDirty();
	         FillFolderStringLists();
         }

         private void bnFMAddMonFolder_Click(object sender, System.EventArgs e)
         {
	         folderBrowser.SelectedPath = "";
	         if (lstFMMonitorFolders.SelectedIndex != -1)
	         {
		         int n = lstFMMonitorFolders.SelectedIndex;
		         folderBrowser.SelectedPath = mDoc.MonitorFolders[n];
	         }

	         if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
	         {
		         mDoc.MonitorFolders.Add(folderBrowser.SelectedPath.ToLower());
		         mDoc.SetDirty();
		         FillFolderStringLists();
	         }
         }

         private void bnFMAddIgFolder_Click(object sender, System.EventArgs e)
         {
	         folderBrowser.SelectedPath = "";
	         if (lstFMIgnoreFolders.SelectedIndex != -1)
		         folderBrowser.SelectedPath = mDoc.IgnoreFolders[lstFMIgnoreFolders.SelectedIndex];

	         if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
	         {
		         mDoc.IgnoreFolders.Add(folderBrowser.SelectedPath.ToLower());
		         mDoc.SetDirty();
		         FillFolderStringLists();
	         }
         }

         private void bnFMOpenMonFolder_Click(object sender, System.EventArgs e)
         {
	         if (lstFMMonitorFolders.SelectedIndex != -1)
		         TVDoc.SysOpen(mDoc.MonitorFolders[lstFMMonitorFolders.SelectedIndex]);
         }

         private void bnFMOpenIgFolder_Click(object sender, System.EventArgs e)
         {
	         if (lstFMIgnoreFolders.SelectedIndex != -1)
		         TVDoc.SysOpen(mDoc.MonitorFolders[lstFMIgnoreFolders.SelectedIndex]);
         }

         private void lstFMMonitorFolders_DoubleClick(object sender, System.EventArgs e)
         {
	         bnFMOpenMonFolder_Click(null, null);
         }

         private void lstFMIgnoreFolders_DoubleClick(object sender, System.EventArgs e)
         {
	         bnFMOpenIgFolder_Click(null, null);
         }

         private void bnFMCheck_Click(object sender, System.EventArgs e)
         {
	         mDoc.CheckMonitoredFolders();
	         GuessAll();
	         FillFMNewShowList(false);
	         //FillFMTVcomListCombo();
         }

         private void lstFMMonitorFolders_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
         {
	         if (!e.Data.GetDataPresent(DataFormats.FileDrop))
		         e.Effect = DragDropEffects.None;
	         else
		         e.Effect = DragDropEffects.Copy;
         }

         private void lstFMIgnoreFolders_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
         {
	         if (!e.Data.GetDataPresent(DataFormats.FileDrop))
		         e.Effect = DragDropEffects.None;
	         else
		         e.Effect = DragDropEffects.Copy;
         }

         private void lstFMMonitorFolders_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
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
				         mDoc.MonitorFolders.Add(path.ToLower());
		         }
		         catch
		         {
		         }
	         }
	         mDoc.SetDirty();
	         FillFolderStringLists();
         }

         private void lstFMIgnoreFolders_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
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
				         mDoc.IgnoreFolders.Add(path.ToLower());
		         }
		         catch
		         {
		         }
	         }
	         mDoc.SetDirty();
	         FillFolderStringLists();
         }

         private void lstFMMonitorFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
         {
	         if (e.KeyCode == Keys.Delete)
		         bnFMRemoveMonFolder_Click(null, null);
         }

         private void lstFMIgnoreFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
         {
	         if (e.KeyCode == Keys.Delete)
		         bnFMRemoveIgFolder_Click(null, null);
         }

    	private void bnFMFullAuto_Click(object sender, System.EventArgs e)
		 {
			 FMPStopNow = false;
			 FMPUpto = "";
			 FMPPercent = 0;

			 Thread fmpshower = new Thread(new ThreadStart(this.FMPShower));
			 fmpshower.Name = "Folder Monitor Progress";
			 fmpshower.Start();

			 int n = 0;
			 int n2 = mDoc.AddItems.Count;

			 foreach (FolderMonitorItem ai in mDoc.AddItems)
			 {
				 if (ai.TheSeries == null)
				 {
					 // do search using folder name
					 TVDoc.GuessShowName(ai);
					 if (!string.IsNullOrEmpty(ai.ShowName))
					 {
						 FMPUpto = ai.ShowName;
						 mDoc.GetTVDB(false,"").Search(ai.ShowName);
						 GuessAI(ai);
						 UpdateFMListItem(ai, true);
						 lvFMNewShows.Update();
					 }
				 }
				 FMPPercent = (100*(n+(n2/2)))/n2;
				 n++;
				 if (FMPStopNow)
					 break;
			 }
			 FMPStopNow = true;
		 }

		 private void bnFMRemoveNewFolder_Click(object sender, System.EventArgs e)
		 {
			 if (lvFMNewShows.SelectedItems.Count == 0)
				 return;
			 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
			 {
				 FolderMonitorItem ai = (FolderMonitorItem)(lvi.Tag);
				 mDoc.AddItems.Remove(ai);
			 }
			 FillFMNewShowList(false);
		 }

		 private void bnFMIgnoreNewFolder_Click(object sender, System.EventArgs e)
		 {
			 if (lvFMNewShows.SelectedItems.Count == 0)
				 return;
			 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
			 {
				 FolderMonitorItem ai = (FolderMonitorItem)(lvi.Tag);
				 mDoc.IgnoreFolders.Add(ai.Folder.ToLower());
				 mDoc.AddItems.Remove(ai);
			 }
			 mDoc.SetDirty();
			 FillFMNewShowList(false);
			 FillFolderStringLists();
		 }
              
		 private void bnFMIgnoreAllNewFolders_Click(object sender, System.EventArgs e)
		 {
			 System.Windows.Forms.DialogResult dr = MessageBox.Show("Add everything in this list to the ignore list?", "Ignore All", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

			 if (dr != System.Windows.Forms.DialogResult.OK)
				 return;

			 foreach (FolderMonitorItem ai in mDoc.AddItems)
				 mDoc.IgnoreFolders.Add(ai.Folder.ToLower());

			 mDoc.AddItems.Clear();
			 mDoc.SetDirty();
			 FillFolderStringLists();
			 FillFMNewShowList(false);
		 }

		 private void lvFMNewShows_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		 {
//                 
//				 if (e->Button != System::Windows::Forms::MouseButtons::Right)
//				 return;
//
//				 lstFMNewFolders->ClearSelected();
//				 lstFMNewFolders->SelectedIndex = lstFMNewFolders->IndexFromPoint(Point(e->X,e->Y));
//
//				 int p;
//				 if ((p = lstFMNewFolders->SelectedIndex) == -1)
//				 return;
//
//				 Point^ pt = lstFMNewFolders->PointToScreen(Point(e->X, e->Y));
//				 RightClickOnFolder(lstFMNewFolders->Items[p]->ToString(),pt);
//
//				 
		 }

		 private void lvFMNewShows_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		 {
			 if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				 e.Effect = DragDropEffects.None;
			 else
				 e.Effect = DragDropEffects.Copy;
		 }

		 private void lvFMNewShows_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
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
						 // keep next line sync'd with ProcessAddItems, etc.
						 bool hasSeasonFolders = Directory.GetDirectories(path, "*Season *").Length > 0; // todo - use non specific word
						 FolderMonitorItem ai = new FolderMonitorItem(path, hasSeasonFolders ? FolderModeEnum.kfmFolderPerSeason : FolderModeEnum.kfmFlat, -1);
						 GuessAI(ai);
						 mDoc.AddItems.Add(ai);
					 }
				 }
				 catch
				 {
				 }
			 }
			 FillFMNewShowList(true);
		 }

		 private void lvFMNewShows_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		 {
			 if (e.KeyCode == Keys.Delete)
				 bnFMRemoveNewFolder_Click(null, null);
		 }

		 private void bnFMNewFolderOpen_Click(object sender, System.EventArgs e)
		 {
			 if (lvFMNewShows.SelectedItems.Count == 0)
				 return;
			 FolderMonitorItem ai = (FolderMonitorItem)(lvFMNewShows.SelectedItems[0].Tag);
			 TVDoc.SysOpen(ai.Folder);
		 }

		 private void lvFMNewShows_DoubleClick(object sender, System.EventArgs e)
		 {
			 bnFMNewFolderOpen_Click(null, null);
		 }

         private void lvMatches_ItemSelectionChanged(object sender, System.EventArgs e)
		 {
			 if (mInternalChange != 0)
				 return;

			 int code = mTCCF.SelectedCode();

			 SeriesInfo ser = mDoc.GetTVDB(false,"").GetSeries(code);
			 if (ser == null)
				 return;

			 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
			 {
				 FolderMonitorItem ai = (FolderMonitorItem)(lvi.Tag);
				 ai.TheSeries = ser;
				 UpdateFMListItem(ai, false);
			 }
		 }

		 private void lvFMNewShows_SelectedIndexChanged(object sender, System.EventArgs e)
		 {
			 if (lvFMNewShows.SelectedItems.Count == 0)
				 return;
			 FolderMonitorItem ai = (FolderMonitorItem)(lvFMNewShows.SelectedItems[0].Tag);
			 //txtTVComCode->Text = ai->TVcomCode == -1 ? "" : ai->TVcomCode.ToString();
			 //txtShowName->Text = ai->Show;
			 mInternalChange++;
			 mTCCF.SetHint(ai.TheSeries != null ? ai.TheSeries.TVDBCode.ToString() : ai.ShowName);

			 if (ai.FolderMode == FolderModeEnum.kfmFlat)
				 rbFlat.Checked = true;
			 else if (ai.FolderMode == FolderModeEnum.kfmSpecificSeason)
			 {
				 rbSpecificSeason.Checked = true;
				 txtFMSpecificSeason.Text = ai.SpecificSeason.ToString();
			 }
			 else
				 rbFolderPerSeason.Checked = true;
			 rbSpecificSeason_CheckedChanged(null, null);

			 mInternalChange--;
		 }

		 private void FillFMNewShowList(bool keepSel)
		 {
             System.Collections.Generic.List<int> sel = new System.Collections.Generic.List<int>();
			 if (keepSel)
				 foreach (int i in lvFMNewShows.SelectedIndices)
					 sel.Add(i);

			 lvFMNewShows.BeginUpdate();
			 lvFMNewShows.Items.Clear();

			 foreach (FolderMonitorItem ai in mDoc.AddItems)
			 {
				 ListViewItem lvi = new ListViewItem();
				 FMLVISet(ai, lvi);
				 lvFMNewShows.Items.Add(lvi);
			 }

			 if (keepSel)
				 foreach (int i in sel)
					 if (i < lvFMNewShows.Items.Count)
						 lvFMNewShows.Items[i].Selected = true;

			 lvFMNewShows.EndUpdate();
		 }

		 private static void FMLVISet(FolderMonitorItem ai, ListViewItem lvi)
		 {
			 lvi.SubItems.Clear();
			 lvi.Text = ai.Folder;
			 lvi.SubItems.Add(ai.TheSeries != null ? ai.TheSeries.Name : "");
			 string fmode = "-";
			 if (ai.FolderMode == FolderModeEnum.kfmFolderPerSeason)
				 fmode = "Per Seas";
			 else if (ai.FolderMode == FolderModeEnum.kfmFlat)
				 fmode = "Flat";
			 else if (ai.FolderMode == FolderModeEnum.kfmSpecificSeason)
				 fmode = ai.SpecificSeason.ToString();
			 lvi.SubItems.Add(fmode);
			 lvi.SubItems.Add(ai.TheSeries != null ? ai.TheSeries.TVDBCode.ToString() : "");
			 lvi.Tag = ai;
		 }

		 private void UpdateFMListItem(FolderMonitorItem ai, bool makevis)
		 {
			 foreach (ListViewItem lvi in lvFMNewShows.Items)
			 {
				 if (lvi.Tag == ai)
				 {
					 FMLVISet(ai, lvi);
					 if (makevis)
						 lvi.EnsureVisible();
					 break;
				 }
			 }
		 }

		 private void bnAddThisOne_Click(object sender, System.EventArgs e)
		 {
			 if (lvFMNewShows.SelectedItems.Count == 0)
				 return;

			 System.Windows.Forms.DialogResult res = MessageBox.Show("Add the selected folders to My Shows?","Folder Monitor",MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			 if (res != System.Windows.Forms.DialogResult.Yes)
				 return;

			 System.Collections.Generic.List<FolderMonitorItem > toAdd = new System.Collections.Generic.List<FolderMonitorItem >();
			 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
			 {
				 FolderMonitorItem ai = (FolderMonitorItem)(lvi.Tag);
				 toAdd.Add(ai);
				 mDoc.AddItems.Remove(ai);
			 }
			 ProcessAddItems(toAdd);
		 }

		 private void bnFolderMonitorDone_Click(object sender, System.EventArgs e)
		 {
			 System.Windows.Forms.DialogResult res = MessageBox.Show("Add all of these to My Shows?","Folder Monitor",MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			 if (res != System.Windows.Forms.DialogResult.Yes)
				 return;

			 ProcessAddItems(mDoc.AddItems);

			 this.Close();
		 }

		 private void ProcessAddItems(System.Collections.Generic.List<FolderMonitorItem > toAdd)
		 {
			 foreach (FolderMonitorItem ai in toAdd)
			 {
				 if (ai.TheSeries == null)
					 continue; // skip

				 // see if there is a matching show item
				 ShowItem found = null;
				 foreach (ShowItem si in mDoc.GetShowItems(true))
				 {
					 if ((ai.TheSeries != null) && (ai.TheSeries.TVDBCode == si.TVDBCode))
					 {
						 found = si;
						 break;
					 }
				 }
				 mDoc.UnlockShowItems();
				 if (found == null)
				 {
					 ShowItem si = new ShowItem(mDoc.GetTVDB(false,""));
					 si.TVDBCode = ai.TheSeries.TVDBCode;
					 //si->ShowName() = ai->TheSeries->Name;
					 mDoc.GetShowItems(true).Add(si);
					 mDoc.UnlockShowItems();
					 mDoc.GenDict();
					 found = si;
				 }

				 if ((ai.FolderMode == FolderModeEnum.kfmFolderPerSeason) || (ai.FolderMode == FolderModeEnum.kfmFlat))
				 {
					 found.AutoAdd_FolderBase = ai.Folder;
					 found.AutoAdd_FolderPerSeason = ai.FolderMode == FolderModeEnum.kfmFolderPerSeason;
					 string foldername = "Season ";

					 foreach (DirectoryInfo di in new DirectoryInfo(ai.Folder).GetDirectories("*Season *"))
					 {
						 string s = di.FullName;
						 string f = ai.Folder;
						 if (!f.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
							 f = f + System.IO.Path.DirectorySeparatorChar.ToString();
						 f = Regex.Escape(f);
						 s = Regex.Replace(s, f+"(.*Season ).*", "$1",RegexOptions.IgnoreCase);
						 if (!string.IsNullOrEmpty(s))
						 {
							 foldername = s;
							 break;
						 }
					 }

					 found.AutoAdd_SeasonFolderName = foldername;
				 }

				 if ((ai.FolderMode == FolderModeEnum.kfmSpecificSeason) && (ai.SpecificSeason != -1))
				 {
					 if (!found.ManualFolderLocations.ContainsKey(ai.SpecificSeason))
						 found.ManualFolderLocations[ai.SpecificSeason] = new StringList();
					 found.ManualFolderLocations[ai.SpecificSeason].Add(ai.Folder);
				 }

				 mDoc.Stats().AutoAddedShows++;
			 }

			 mDoc.Dirty();
			 toAdd.Clear();

			 FillFMNewShowList(true);
		 }

		 private void GuessAI(FolderMonitorItem ai)
		 {
			 TVDoc.GuessShowName(ai);
			 if (!string.IsNullOrEmpty(ai.ShowName))
			 {
				 TheTVDB db = mDoc.GetTVDB(true,"GuessAI");
				 foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo > ser in db.GetSeriesDict())
				 {
					 string s;
					 s = ser.Value.Name.ToLower();
					 if (s == ai.ShowName.ToLower())
					 {
						 ai.TheSeries = ser.Value;
						 break;
					 }
				 }
				 db.Unlock("GuessAI");
			 }
		 }

		 private void GuessAll() // not all -> selected only
		 {
			 foreach (FolderMonitorItem ai in mDoc.AddItems)
				 GuessAI(ai);
			 FillFMNewShowList(false);
		 }

		 private void FMControlLeave(object sender, System.EventArgs e)
		 {
			 if (lvFMNewShows.SelectedItems.Count != 0)
			 {
				 FolderMonitorItem ai = (FolderMonitorItem)(lvFMNewShows.SelectedItems[0].Tag);
				 UpdateFMListItem(ai, false);
			 }

		 }
		 private void bnFMVisitTVcom_Click(object sender, System.EventArgs e)
		 {
			 int code = mTCCF.SelectedCode();
			 TVDoc.SysOpen(mDoc.GetTVDB(false,"").WebsiteURL(code, -1, false));
		 }

		 private void SetAllFolderModes(FolderModeEnum fm)
		 {
			 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
			 {
				 FolderMonitorItem ai = (FolderMonitorItem)(lvi.Tag);

				 ai.FolderMode = fm;
				 UpdateFMListItem(ai, false);
			 }

		 }

        private void rbSpecificSeason_CheckedChanged(object sender, System.EventArgs e)
        {
	         txtFMSpecificSeason.Enabled = rbSpecificSeason.Checked;

	         if (mInternalChange == 0)
		         SetAllFolderModes(FolderModeEnum.kfmSpecificSeason);
        }
        
        private void rbFlat_CheckedChanged(object sender, System.EventArgs e)
        {
	         if (mInternalChange == 0)
		         SetAllFolderModes(FolderModeEnum.kfmFlat);
        }

        private void rbFolderPerSeason_CheckedChanged(object sender, System.EventArgs e)
        {
	         if (mInternalChange == 0)
		         SetAllFolderModes(FolderModeEnum.kfmFolderPerSeason);
        }

		 private void lstFMMonitorFolders_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		 {
			 if (e.Button != System.Windows.Forms.MouseButtons.Right)
				 return;
//
//
//				 lstFMMonitorFolders->ClearSelected();
//				 lstFMMonitorFolders->SelectedIndex = lstFMMonitorFolders->IndexFromPoint(Point(e->X,e->Y));
//
//				 int p;
//				 if ((p = lstFMMonitorFolders->SelectedIndex) == -1)
//					 return;
//
//				 Point^ pt = lstFMMonitorFolders->PointToScreen(Point(e->X, e->Y));
//				 RightClickOnFolder(lstFMMonitorFolders->Items[p]->ToString(),pt);
//				 
		 }
		 private void lstFMIgnoreFolders_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		 {
			 if (e.Button != System.Windows.Forms.MouseButtons.Right)
				 return;
//
//				 lstFMIgnoreFolders->ClearSelected();
//				 lstFMIgnoreFolders->SelectedIndex = lstFMIgnoreFolders->IndexFromPoint(Point(e->X,e->Y));
//
//				 int p;
//				 if ((p = lstFMIgnoreFolders->SelectedIndex) == -1)
//					 return;
//
//				 Point^ pt = lstFMIgnoreFolders->PointToScreen(Point(e->X, e->Y));
//				 RightClickOnFolder(lstFMIgnoreFolders->Items[p]->ToString(),pt);
//				 
		 }
    }
}