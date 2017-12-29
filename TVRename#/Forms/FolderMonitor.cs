// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Windows.Forms;
using System.Threading;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;

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
        private readonly TVDoc mDoc;
        // private int mInternalChange;
        // private TheTVDBCodeFinder mTCCF;

        public FolderMonitor(TVDoc doc)
        {
            mDoc = doc;

            InitializeComponent();

            FillFolderStringLists();
        }

        public void FMPShower()
        {
            FMP = new FolderMonitorProgress(this);
            FMP.ShowDialog();
            FMP = null;
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            bool confirmClose = false;
            foreach (FolderMonitorEntry fme in mDoc.AddItems)
            {
                if (fme.CodeKnown)
                {
                    confirmClose = true;
                    break;
                }
            }
            if (confirmClose)
            {
                if (DialogResult.OK != MessageBox.Show("Close without adding identified shows to \"My Shows\"?", "Folder Monitor", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    return;
                }
            }

            Close();
        }

        private void FillFolderStringLists()
        {
            mDoc.MonitorFolders.Sort();
            mDoc.IgnoreFolders.Sort();
            
            lstFMMonitorFolders.BeginUpdate();
            lstFMMonitorFolders.Items.Clear();
            
            foreach (string folder in mDoc.MonitorFolders)
                lstFMMonitorFolders.Items.Add(folder);

            lstFMMonitorFolders.EndUpdate();

            lstFMIgnoreFolders.BeginUpdate();
            lstFMIgnoreFolders.Items.Clear();

            foreach (string folder in mDoc.IgnoreFolders)
                lstFMIgnoreFolders.Items.Add(folder);

            lstFMIgnoreFolders.EndUpdate();
        }

        private void bnRemoveMonFolder_Click(object sender, System.EventArgs e)
        {
            for (int i = lstFMMonitorFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = lstFMMonitorFolders.SelectedIndices[i];
                mDoc.MonitorFolders.RemoveAt(n);
            }
            mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void bnRemoveIgFolder_Click(object sender, System.EventArgs e)
        {
            for (int i = lstFMIgnoreFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = lstFMIgnoreFolders.SelectedIndices[i];
                mDoc.IgnoreFolders.RemoveAt(n);
            }
            mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void bnAddMonFolder_Click(object sender, System.EventArgs e)
        {
            folderBrowser.SelectedPath = "";
            if (lstFMMonitorFolders.SelectedIndex != -1)
            {
                int n = lstFMMonitorFolders.SelectedIndex;
                folderBrowser.SelectedPath = mDoc.MonitorFolders[n];
            }

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                mDoc.MonitorFolders.Add(folderBrowser.SelectedPath.ToLower());
                mDoc.SetDirty();
                FillFolderStringLists();
            }
        }

        private void bnAddIgFolder_Click(object sender, System.EventArgs e)
        {
            folderBrowser.SelectedPath = "";
            if (lstFMIgnoreFolders.SelectedIndex != -1)
                folderBrowser.SelectedPath = mDoc.IgnoreFolders[lstFMIgnoreFolders.SelectedIndex];

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                mDoc.IgnoreFolders.Add(folderBrowser.SelectedPath.ToLower());
                mDoc.SetDirty();
                FillFolderStringLists();
            }
        }

        private void bnOpenMonFolder_Click(object sender, System.EventArgs e)
        {
            if (lstFMMonitorFolders.SelectedIndex != -1)
                Helpers.SysOpen(mDoc.MonitorFolders[lstFMMonitorFolders.SelectedIndex]);
        }

        private void bnOpenIgFolder_Click(object sender, System.EventArgs e)
        {
            if (lstFMIgnoreFolders.SelectedIndex != -1)
                Helpers.SysOpen(mDoc.MonitorFolders[lstFMIgnoreFolders.SelectedIndex]);
        }

        private void lstFMMonitorFolders_DoubleClick(object sender, System.EventArgs e)
        {
            bnOpenMonFolder_Click(null, null);
        }

        private void lstFMIgnoreFolders_DoubleClick(object sender, System.EventArgs e)
        {

        }

        private void bnCheck_Click(object sender, System.EventArgs e)
        {
            DoCheck();
        }

        private void DoCheck()
        {
            tabControl1.SelectedTab = tbResults;
            tabControl1.Update();

            FMPStopNow = false;
            FMPUpto = "Checking folders";
            FMPPercent = 0;

            Thread fmpshower = new Thread(FMPShower);
            fmpshower.Name = "Folder Monitor Progress (Folder Check)";
            fmpshower.Start();

            while ((FMP == null) || (!FMP.Ready))
                Thread.Sleep(10);

            mDoc.MonitorCheckFolders(ref FMPStopNow, ref FMPPercent);
            
            FMPStopNow = true;

            FillFMNewShowList(false);

        }

        private void lstFMMonitorFolders_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lstFMIgnoreFolders_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lstFMMonitorFolders_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
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

        private void lstFMIgnoreFolders_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
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

        private void lstFMMonitorFolders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                bnRemoveMonFolder_Click(null, null);
        }

        private void lstFMIgnoreFolders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                bnRemoveIgFolder_Click(null, null);
        }

        private void bnFullAuto_Click(object sender, System.EventArgs e)
        {
            if (mDoc.AddItems.Count == 0)
                return;

            FMPStopNow = false;
            FMPUpto = "Identifying shows";
            FMPPercent = 0;

            Thread fmpshower = new Thread(FMPShower) {Name = "Folder Monitor Progress (Full Auto)"};
            fmpshower.Start();

            while ((FMP == null) || (!FMP.Ready))
                Thread.Sleep(10);

            int n = 0;
            int n2 = mDoc.AddItems.Count;

            foreach (FolderMonitorEntry ai in mDoc.AddItems)
            {
                FMPPercent = 100 * n++ / n2;

               if (FMPStopNow)
                   break;

                if (ai.CodeKnown)
                    continue;

                int p = ai.Folder.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                FMPUpto = ai.Folder.Substring(p + 1); // +1 makes -1 "not found" result ok
                
                mDoc.MonitorGuessShowItem(ai);
                
                // update our display
                UpdateFMListItem(ai, true);
                lvFMNewShows.Update();
                Update();
            }
            FMPStopNow = true;
        }

        private void bnRemoveNewFolder_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
            {
                FolderMonitorEntry ai = (FolderMonitorEntry)(lvi.Tag);
                mDoc.AddItems.Remove(ai);
            }
            FillFMNewShowList(false);
        }

        private void bnIgnoreNewFolder_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
                return;

            DialogResult res = MessageBox.Show("Add selected folders to the folder monitor ignore list?", "Folder Monitor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes)
                return;

            foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
            {
                FolderMonitorEntry ai = (FolderMonitorEntry)(lvi.Tag);
                mDoc.IgnoreFolders.Add(ai.Folder.ToLower());
                mDoc.AddItems.Remove(ai);
            }
            mDoc.SetDirty();
            FillFMNewShowList(false);
            FillFolderStringLists();
        }

        private void lvFMNewShows_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lvFMNewShows_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                    {
                        mDoc.MonitorAddSingleFolder(di, true);
                        FillFMNewShowList(true);
                    }
                }
                catch
                {
                }
            }
        }

        private void lvFMNewShows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                bnRemoveNewFolder_Click(null, null);
        }

        private void bnNewFolderOpen_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
                return;
            FolderMonitorEntry ai = lvFMNewShows.SelectedItems[0].Tag as FolderMonitorEntry;
            if (ai != null)
                Helpers.SysOpen(ai.Folder);
        }

        private void FillFMNewShowList(bool keepSel)
        {
            System.Collections.Generic.List<int> sel = new System.Collections.Generic.List<int>();
            if (keepSel)
            {
                foreach (int i in lvFMNewShows.SelectedIndices)
                    sel.Add(i);
            }

            lvFMNewShows.BeginUpdate();
            lvFMNewShows.Items.Clear();

            foreach (FolderMonitorEntry ai in mDoc.AddItems)
            {
                ListViewItem lvi = new ListViewItem();
                UpdateResultEntry(ai, lvi);
                lvFMNewShows.Items.Add(lvi);
            }

            if (keepSel)
            {
                foreach (int i in sel)
                {
                    if (i < lvFMNewShows.Items.Count)
                        lvFMNewShows.Items[i].Selected = true;
                }
            }

            lvFMNewShows.EndUpdate();
            lvFMNewShows.Update();
        }

        private void UpdateResultEntry(FolderMonitorEntry ai, ListViewItem lvi)
        {
            lvi.SubItems.Clear();
            lvi.Text = ai.Folder;
            lvi.SubItems.Add(ai.CodeKnown ? TheTVDB.Instance.GetSeries(ai.TVDBCode).Name : "");
            lvi.SubItems.Add(ai.HasSeasonFoldersGuess ? "Folder per season" : "Flat");
            lvi.SubItems.Add(ai.CodeKnown ? ai.TVDBCode.ToString() : "");
            lvi.Tag = ai;
        }

        private void UpdateFMListItem(FolderMonitorEntry ai, bool makevis)
        {
            foreach (ListViewItem lvi in lvFMNewShows.Items)
            {
                if (lvi.Tag != ai) // haven't found the entry yet
                    continue;

                UpdateResultEntry(ai, lvi);

                if (makevis)
                    lvi.EnsureVisible();

                break;
            }
        }

        private void bnFolderMonitorDone_Click(object sender, System.EventArgs e)
        {
            if (mDoc.AddItems.Count > 0)
            {
                DialogResult res = MessageBox.Show("Add identified shows to \"My Shows\"?", "Folder Monitor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes)
                    return;

                mDoc.MonitorAddAllToMyShows();
            }

            Close();
        }

        //private void ProcessAddItems(FolderMonitorEntryList toAdd)
        //{
        //    foreach (FolderMonitorEntry ai in toAdd)
        //    {
        //        if (ai.TheSeries == null)
        //            continue; // skip

        //        // see if there is a matching show item
        //        ShowItem found = null;
        //        foreach (ShowItem si in this.mDoc.GetShowItems(true))
        //        {
        //            if ((ai.TheSeries != null) && (ai.TheSeries.TVDBCode == si.TVDBCode))
        //            {
        //                found = si;
        //                break;
        //            }
        //        }
        //        this.mDoc.UnlockShowItems();
        //        if (found == null)
        //        {
        //           xxx 
        //               ShowItem si = new ShowItem(this.mDoc.GetTVDB(false, ""));
        //            si.TVDBCode = ai.TheSeries.TVDBCode;
        //            //si->ShowName = ai->TheSeries->Name;
        //            this.mDoc.GetShowItems(true).Add(si);
        //            this.mDoc.UnlockShowItems();
        //            this.mDoc.GenDict();
        //            found = si;
        //        }

        //        if ((ai.FolderMode == FolderModeEnum.kfmFolderPerSeason) || (ai.FolderMode == FolderModeEnum.kfmFlat))
        //        {
        //            found.AutoAdd_FolderBase = ai.Folder;
        //            found.AutoAdd_FolderPerSeason = ai.FolderMode == FolderModeEnum.kfmFolderPerSeason;
        //            string foldername = "Season ";

        //            foreach (DirectoryInfo di in new DirectoryInfo(ai.Folder).GetDirectories("*Season *"))
        //            {
        //                string s = di.FullName;
        //                string f = ai.Folder;
        //                if (!f.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
        //                    f = f + System.IO.Path.DirectorySeparatorChar;
        //                f = Regex.Escape(f);
        //                s = Regex.Replace(s, f + "(.*Season ).*", "$1", RegexOptions.IgnoreCase);
        //                if (!string.IsNullOrEmpty(s))
        //                {
        //                    foldername = s;
        //                    break;
        //                }
        //            }

        //            found.AutoAdd_SeasonFolderName = foldername;
        //        }

        //        if ((ai.FolderMode == FolderModeEnum.kfmSpecificSeason) && (ai.SpecificSeason != -1))
        //        {
        //            if (!found.ManualFolderLocations.ContainsKey(ai.SpecificSeason))
        //                found.ManualFolderLocations[ai.SpecificSeason] = new StringList();
        //            found.ManualFolderLocations[ai.SpecificSeason].Add(ai.Folder);
        //        }

        //        this.mDoc.Stats().AutoAddedShows++;
        //    }

        //    this.mDoc.Dirty();
        //    toAdd.Clear();

        //    this.FillFMNewShowList(true);
        //}

        //private void GuessAll() // not all -> selected only
        //{
        //    foreach (FolderMonitorEntry ai in this.mDoc.AddItems)
        //        this.mDoc.MonitorGuessShowItem(ai);
        //    this.FillFMNewShowList(false);
        //}

        private void bnVisitTVcom_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
                return;

            FolderMonitorEntry fme = lvFMNewShows.SelectedItems[0].Tag as FolderMonitorEntry;
            if (fme == null)
                return;

            int code = fme.TVDBCode;
            if (code != -1)
                Helpers.SysOpen(TheTVDB.Instance.WebsiteURL(code, -1, false));
        }

        private void bnCheck2_Click(object sender, System.EventArgs e)
        {
            DoCheck();
        }

        private void lvFMNewShows_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bnEditEntry_Click(null, null);
        }

        private void bnEditEntry_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
                return;

            FolderMonitorEntry fme = lvFMNewShows.SelectedItems[0].Tag as FolderMonitorEntry;
            EditEntry(fme);
            UpdateFMListItem(fme, true);
        }

        private void EditEntry(FolderMonitorEntry fme)
        {
            FolderMonitorEdit ed = new FolderMonitorEdit(fme);
            if ((ed.ShowDialog() != DialogResult.OK )|| (ed.Code == -1))
                return;

            fme.TVDBCode = ed.Code;
        }

    }
}
