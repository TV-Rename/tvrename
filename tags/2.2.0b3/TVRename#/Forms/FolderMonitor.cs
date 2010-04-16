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
        public FolderMonitorProgress FMP;
        public int FMPPercent;
        public bool FMPStopNow;
        public string FMPUpto;
        private TVDoc mDoc;
        private int mInternalChange;
        private TheTVDBCodeFinder mTCCF;

        public FolderMonitor(TVDoc doc)
        {
            this.mDoc = doc;
            this.mInternalChange = 0;

            this.InitializeComponent();

            this.mTCCF = new TheTVDBCodeFinder("", this.mDoc.GetTVDB(false, ""));
            this.mTCCF.Dock = DockStyle.Fill;
            this.mTCCF.SelectionChanged += this.lvMatches_ItemSelectionChanged;

            this.pnlCF.SuspendLayout();
            this.pnlCF.Controls.Add(this.mTCCF);
            this.pnlCF.ResumeLayout();

            this.FillFolderStringLists();
        }

        public void FMPShower()
        {
            this.FMP = new FolderMonitorProgress(this);
            this.FMP.ShowDialog();
            this.FMP = null;
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void FillFolderStringLists()
        {
            this.lstFMIgnoreFolders.BeginUpdate();
            this.lstFMMonitorFolders.BeginUpdate();

            this.lstFMIgnoreFolders.Items.Clear();
            this.lstFMMonitorFolders.Items.Clear();

            this.mDoc.MonitorFolders.Sort();
            this.mDoc.IgnoreFolders.Sort();

            foreach (string folder in this.mDoc.MonitorFolders)
                this.lstFMMonitorFolders.Items.Add(folder);

            foreach (string folder in this.mDoc.IgnoreFolders)
                this.lstFMIgnoreFolders.Items.Add(folder);

            this.lstFMIgnoreFolders.EndUpdate();
            this.lstFMMonitorFolders.EndUpdate();
        }

        private void bnFMRemoveMonFolder_Click(object sender, System.EventArgs e)
        {
            for (int i = this.lstFMMonitorFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = this.lstFMMonitorFolders.SelectedIndices[i];
                this.mDoc.MonitorFolders.RemoveAt(n);
            }
            this.mDoc.SetDirty();
            this.FillFolderStringLists();
        }

        private void bnFMRemoveIgFolder_Click(object sender, System.EventArgs e)
        {
            for (int i = this.lstFMIgnoreFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = this.lstFMIgnoreFolders.SelectedIndices[i];
                this.mDoc.IgnoreFolders.RemoveAt(n);
            }
            this.mDoc.SetDirty();
            this.FillFolderStringLists();
        }

        private void bnFMAddMonFolder_Click(object sender, System.EventArgs e)
        {
            this.folderBrowser.SelectedPath = "";
            if (this.lstFMMonitorFolders.SelectedIndex != -1)
            {
                int n = this.lstFMMonitorFolders.SelectedIndex;
                this.folderBrowser.SelectedPath = this.mDoc.MonitorFolders[n];
            }

            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.mDoc.MonitorFolders.Add(this.folderBrowser.SelectedPath.ToLower());
                this.mDoc.SetDirty();
                this.FillFolderStringLists();
            }
        }

        private void bnFMAddIgFolder_Click(object sender, System.EventArgs e)
        {
            this.folderBrowser.SelectedPath = "";
            if (this.lstFMIgnoreFolders.SelectedIndex != -1)
                this.folderBrowser.SelectedPath = this.mDoc.IgnoreFolders[this.lstFMIgnoreFolders.SelectedIndex];

            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.mDoc.IgnoreFolders.Add(this.folderBrowser.SelectedPath.ToLower());
                this.mDoc.SetDirty();
                this.FillFolderStringLists();
            }
        }

        private void bnFMOpenMonFolder_Click(object sender, System.EventArgs e)
        {
            if (this.lstFMMonitorFolders.SelectedIndex != -1)
                TVDoc.SysOpen(this.mDoc.MonitorFolders[this.lstFMMonitorFolders.SelectedIndex]);
        }

        private void bnFMOpenIgFolder_Click(object sender, System.EventArgs e)
        {
            if (this.lstFMIgnoreFolders.SelectedIndex != -1)
                TVDoc.SysOpen(this.mDoc.MonitorFolders[this.lstFMIgnoreFolders.SelectedIndex]);
        }

        private void lstFMMonitorFolders_DoubleClick(object sender, System.EventArgs e)
        {
            this.bnFMOpenMonFolder_Click(null, null);
        }

        private void lstFMIgnoreFolders_DoubleClick(object sender, System.EventArgs e)
        {
            this.bnFMOpenIgFolder_Click(null, null);
        }

        private void bnFMCheck_Click(object sender, System.EventArgs e)
        {
            this.mDoc.CheckMonitoredFolders();
            this.GuessAll();
            this.FillFMNewShowList(false);
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
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                DirectoryInfo di;
                try
                {
                    di = new DirectoryInfo(path);
                    if (di.Exists)
                        this.mDoc.MonitorFolders.Add(path.ToLower());
                }
                catch
                {
                }
            }
            this.mDoc.SetDirty();
            this.FillFolderStringLists();
        }

        private void lstFMIgnoreFolders_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                DirectoryInfo di;
                try
                {
                    di = new DirectoryInfo(path);
                    if (di.Exists)
                        this.mDoc.IgnoreFolders.Add(path.ToLower());
                }
                catch
                {
                }
            }
            this.mDoc.SetDirty();
            this.FillFolderStringLists();
        }

        private void lstFMMonitorFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                this.bnFMRemoveMonFolder_Click(null, null);
        }

        private void lstFMIgnoreFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                this.bnFMRemoveIgFolder_Click(null, null);
        }

        private void bnFMFullAuto_Click(object sender, System.EventArgs e)
        {
            this.FMPStopNow = false;
            this.FMPUpto = "";
            this.FMPPercent = 0;

            Thread fmpshower = new Thread(this.FMPShower);
            fmpshower.Name = "Folder Monitor Progress";
            fmpshower.Start();

            int n = 0;
            int n2 = this.mDoc.AddItems.Count;

            foreach (FolderMonitorItem ai in this.mDoc.AddItems)
            {
                if (ai.TheSeries == null)
                {
                    // do search using folder name
                    TVDoc.GuessShowName(ai);
                    if (!string.IsNullOrEmpty(ai.ShowName))
                    {
                        this.FMPUpto = ai.ShowName;
                        this.mDoc.GetTVDB(false, "").Search(ai.ShowName);
                        this.GuessAI(ai);
                        this.UpdateFMListItem(ai, true);
                        this.lvFMNewShows.Update();
                    }
                }
                this.FMPPercent = (100 * (n + (n2 / 2))) / n2;
                n++;
                if (this.FMPStopNow)
                    break;
            }
            this.FMPStopNow = true;
        }

        private void bnFMRemoveNewFolder_Click(object sender, System.EventArgs e)
        {
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem lvi in this.lvFMNewShows.SelectedItems)
            {
                FolderMonitorItem ai = (FolderMonitorItem) (lvi.Tag);
                this.mDoc.AddItems.Remove(ai);
            }
            this.FillFMNewShowList(false);
        }

        private void bnFMIgnoreNewFolder_Click(object sender, System.EventArgs e)
        {
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem lvi in this.lvFMNewShows.SelectedItems)
            {
                FolderMonitorItem ai = (FolderMonitorItem) (lvi.Tag);
                this.mDoc.IgnoreFolders.Add(ai.Folder.ToLower());
                this.mDoc.AddItems.Remove(ai);
            }
            this.mDoc.SetDirty();
            this.FillFMNewShowList(false);
            this.FillFolderStringLists();
        }

        private void bnFMIgnoreAllNewFolders_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.DialogResult dr = MessageBox.Show("Add everything in this list to the ignore list?", "Ignore All", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if (dr != System.Windows.Forms.DialogResult.OK)
                return;

            foreach (FolderMonitorItem ai in this.mDoc.AddItems)
                this.mDoc.IgnoreFolders.Add(ai.Folder.ToLower());

            this.mDoc.AddItems.Clear();
            this.mDoc.SetDirty();
            this.FillFolderStringLists();
            this.FillFMNewShowList(false);
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
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
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
                        this.GuessAI(ai);
                        this.mDoc.AddItems.Add(ai);
                    }
                }
                catch
                {
                }
            }
            this.FillFMNewShowList(true);
        }

        private void lvFMNewShows_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                this.bnFMRemoveNewFolder_Click(null, null);
        }

        private void bnFMNewFolderOpen_Click(object sender, System.EventArgs e)
        {
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;
            FolderMonitorItem ai = (FolderMonitorItem) (this.lvFMNewShows.SelectedItems[0].Tag);
            TVDoc.SysOpen(ai.Folder);
        }

        private void lvFMNewShows_DoubleClick(object sender, System.EventArgs e)
        {
            this.bnFMNewFolderOpen_Click(null, null);
        }

        private void lvMatches_ItemSelectionChanged(object sender, System.EventArgs e)
        {
            if (this.mInternalChange != 0)
                return;

            int code = this.mTCCF.SelectedCode();

            SeriesInfo ser = this.mDoc.GetTVDB(false, "").GetSeries(code);
            if (ser == null)
                return;

            foreach (ListViewItem lvi in this.lvFMNewShows.SelectedItems)
            {
                FolderMonitorItem ai = (FolderMonitorItem) (lvi.Tag);
                ai.TheSeries = ser;
                this.UpdateFMListItem(ai, false);
            }
        }

        private void lvFMNewShows_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;
            FolderMonitorItem ai = (FolderMonitorItem) (this.lvFMNewShows.SelectedItems[0].Tag);
            //txtTVComCode->Text = ai->TVcomCode == -1 ? "" : ai->TVcomCode.ToString();
            //txtShowName->Text = ai->Show;
            this.mInternalChange++;
            this.mTCCF.SetHint(ai.TheSeries != null ? ai.TheSeries.TVDBCode.ToString() : ai.ShowName);

            if (ai.FolderMode == FolderModeEnum.kfmFlat)
                this.rbFlat.Checked = true;
            else if (ai.FolderMode == FolderModeEnum.kfmSpecificSeason)
            {
                this.rbSpecificSeason.Checked = true;
                this.txtFMSpecificSeason.Text = ai.SpecificSeason.ToString();
            }
            else
                this.rbFolderPerSeason.Checked = true;
            this.rbSpecificSeason_CheckedChanged(null, null);

            this.mInternalChange--;
        }

        private void FillFMNewShowList(bool keepSel)
        {
            System.Collections.Generic.List<int> sel = new System.Collections.Generic.List<int>();
            if (keepSel)
            {
                foreach (int i in this.lvFMNewShows.SelectedIndices)
                    sel.Add(i);
            }

            this.lvFMNewShows.BeginUpdate();
            this.lvFMNewShows.Items.Clear();

            foreach (FolderMonitorItem ai in this.mDoc.AddItems)
            {
                ListViewItem lvi = new ListViewItem();
                FMLVISet(ai, lvi);
                this.lvFMNewShows.Items.Add(lvi);
            }

            if (keepSel)
            {
                foreach (int i in sel)
                {
                    if (i < this.lvFMNewShows.Items.Count)
                        this.lvFMNewShows.Items[i].Selected = true;
                }
            }

            this.lvFMNewShows.EndUpdate();
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
            foreach (ListViewItem lvi in this.lvFMNewShows.Items)
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
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;

            System.Windows.Forms.DialogResult res = MessageBox.Show("Add the selected folders to My Shows?", "Folder Monitor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != System.Windows.Forms.DialogResult.Yes)
                return;

            System.Collections.Generic.List<FolderMonitorItem> toAdd = new System.Collections.Generic.List<FolderMonitorItem>();
            foreach (ListViewItem lvi in this.lvFMNewShows.SelectedItems)
            {
                FolderMonitorItem ai = (FolderMonitorItem) (lvi.Tag);
                toAdd.Add(ai);
                this.mDoc.AddItems.Remove(ai);
            }
            this.ProcessAddItems(toAdd);
        }

        private void bnFolderMonitorDone_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.DialogResult res = MessageBox.Show("Add all of these to My Shows?", "Folder Monitor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != System.Windows.Forms.DialogResult.Yes)
                return;

            this.ProcessAddItems(this.mDoc.AddItems);

            this.Close();
        }

        private void ProcessAddItems(System.Collections.Generic.List<FolderMonitorItem> toAdd)
        {
            foreach (FolderMonitorItem ai in toAdd)
            {
                if (ai.TheSeries == null)
                    continue; // skip

                // see if there is a matching show item
                ShowItem found = null;
                foreach (ShowItem si in this.mDoc.GetShowItems(true))
                {
                    if ((ai.TheSeries != null) && (ai.TheSeries.TVDBCode == si.TVDBCode))
                    {
                        found = si;
                        break;
                    }
                }
                this.mDoc.UnlockShowItems();
                if (found == null)
                {
                    ShowItem si = new ShowItem(this.mDoc.GetTVDB(false, ""));
                    si.TVDBCode = ai.TheSeries.TVDBCode;
                    //si->ShowName() = ai->TheSeries->Name;
                    this.mDoc.GetShowItems(true).Add(si);
                    this.mDoc.UnlockShowItems();
                    this.mDoc.GenDict();
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
                            f = f + System.IO.Path.DirectorySeparatorChar;
                        f = Regex.Escape(f);
                        s = Regex.Replace(s, f + "(.*Season ).*", "$1", RegexOptions.IgnoreCase);
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

                this.mDoc.Stats().AutoAddedShows++;
            }

            this.mDoc.Dirty();
            toAdd.Clear();

            this.FillFMNewShowList(true);
        }

        private void GuessAI(FolderMonitorItem ai)
        {
            TVDoc.GuessShowName(ai);
            if (!string.IsNullOrEmpty(ai.ShowName))
            {
                TheTVDB db = this.mDoc.GetTVDB(true, "GuessAI");
                foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> ser in db.GetSeriesDict())
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
            foreach (FolderMonitorItem ai in this.mDoc.AddItems)
                this.GuessAI(ai);
            this.FillFMNewShowList(false);
        }

        private void FMControlLeave(object sender, System.EventArgs e)
        {
            if (this.lvFMNewShows.SelectedItems.Count != 0)
            {
                FolderMonitorItem ai = (FolderMonitorItem) (this.lvFMNewShows.SelectedItems[0].Tag);
                this.UpdateFMListItem(ai, false);
            }
        }

        private void bnFMVisitTVcom_Click(object sender, System.EventArgs e)
        {
            int code = this.mTCCF.SelectedCode();
            TVDoc.SysOpen(this.mDoc.GetTVDB(false, "").WebsiteURL(code, -1, false));
        }

        private void SetAllFolderModes(FolderModeEnum fm)
        {
            foreach (ListViewItem lvi in this.lvFMNewShows.SelectedItems)
            {
                FolderMonitorItem ai = (FolderMonitorItem) (lvi.Tag);

                ai.FolderMode = fm;
                this.UpdateFMListItem(ai, false);
            }
        }

        private void rbSpecificSeason_CheckedChanged(object sender, System.EventArgs e)
        {
            this.txtFMSpecificSeason.Enabled = this.rbSpecificSeason.Checked;

            if (this.mInternalChange == 0)
                this.SetAllFolderModes(FolderModeEnum.kfmSpecificSeason);
        }

        private void rbFlat_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.mInternalChange == 0)
                this.SetAllFolderModes(FolderModeEnum.kfmFlat);
        }

        private void rbFolderPerSeason_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.mInternalChange == 0)
                this.SetAllFolderModes(FolderModeEnum.kfmFolderPerSeason);
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