// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System.Windows.Forms;
using System.Threading;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using DaveChambers.FolderBrowserDialogEx;

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
        private FolderMonitorProgress ProgressDialog;
        public int FMPPercent;
        public bool FMPStopNow;
        public string FMPUpto;
        private readonly TVDoc mDoc;
        private readonly BulkAddManager engine;

        public FolderMonitor(TVDoc doc,BulkAddManager bam)
        {
            this.mDoc = doc;
            this.engine = bam;

            InitializeComponent();

            FillFolderStringLists();
            this.tbResults.Parent = null;
        }

        public void FMPShower()
        {
            this.ProgressDialog = new FolderMonitorProgress(this);
            this.ProgressDialog.ShowDialog();
            this.ProgressDialog = null;
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            if (!CanClose())
            {
                if (DialogResult.OK != MessageBox.Show("Close without adding identified shows to \"My Shows\"?", "Bulk Add Shows", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    return;
                }
            }

            Close();
        }

        private bool CanClose()
        {
            foreach (FolderMonitorEntry fme in this.engine.AddItems)
            {
                if (fme.CodeKnown) {return false;}
            }

            return true;
        }

        private void FillFolderStringLists()
        {
            TVSettings.Instance.LibraryFolders.Sort();
            TVSettings.Instance.IgnoreFolders.Sort();
            
            this.lstFMMonitorFolders.BeginUpdate();
            this.lstFMMonitorFolders.Items.Clear();
            
            foreach (string folder in TVSettings.Instance.LibraryFolders)
                this.lstFMMonitorFolders.Items.Add(folder);

            this.lstFMMonitorFolders.EndUpdate();

            this.lstFMIgnoreFolders.BeginUpdate();
            this.lstFMIgnoreFolders.Items.Clear();

            foreach (string folder in TVSettings.Instance.IgnoreFolders)
                this.lstFMIgnoreFolders.Items.Add(folder);

            this.lstFMIgnoreFolders.EndUpdate();
        }

        private void bnRemoveMonFolder_Click(object sender, System.EventArgs e)
        {
            for (int i = this.lstFMMonitorFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = this.lstFMMonitorFolders.SelectedIndices[i];
                TVSettings.Instance.LibraryFolders.RemoveAt(n);
            }
            this.mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void bnRemoveIgFolder_Click(object sender, System.EventArgs e)
        {
            for (int i = this.lstFMIgnoreFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = this.lstFMIgnoreFolders.SelectedIndices[i];
                TVSettings.Instance.IgnoreFolders.RemoveAt(n);
            }
            this.mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void bnAddMonFolder_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialogEx searchFolderBrowser = new FolderBrowserDialogEx
            {
                SelectedPath = "",
                Title = "Add New Monitor Folder...",
                ShowEditbox = true,
                StartPosition = FormStartPosition.CenterScreen
            };



            if (this.lstFMMonitorFolders.SelectedIndex != -1)
            {
                int n = this.lstFMMonitorFolders.SelectedIndex;
                searchFolderBrowser.SelectedPath = TVSettings.Instance.LibraryFolders[n];
            }

            if (searchFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                TVSettings.Instance.LibraryFolders.Add(searchFolderBrowser.SelectedPath.ToLower());
                this.mDoc.SetDirty();
                FillFolderStringLists();
            }
        }

        private void bnAddIgFolder_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialogEx ignoreFolderBrowser = new FolderBrowserDialogEx
            {
                SelectedPath = "",
                Title = "Add New Ignore Folder...",
                ShowEditbox = true,
                StartPosition = FormStartPosition.CenterScreen
            };


            if (this.lstFMIgnoreFolders.SelectedIndex != -1)
                ignoreFolderBrowser.SelectedPath = TVSettings.Instance.IgnoreFolders[this.lstFMIgnoreFolders.SelectedIndex];

            if (ignoreFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                TVSettings.Instance.IgnoreFolders.Add(ignoreFolderBrowser.SelectedPath.ToLower());
                this.mDoc.SetDirty();
                FillFolderStringLists();
            }
        }

        private void bnOpenMonFolder_Click(object sender, System.EventArgs e)
        {
            if (this.lstFMMonitorFolders.SelectedIndex != -1)
                Helpers.SysOpen(TVSettings.Instance.LibraryFolders[this.lstFMMonitorFolders.SelectedIndex]);
        }

        private void bnOpenIgFolder_Click(object sender, System.EventArgs e)
        {
            if (this.lstFMIgnoreFolders.SelectedIndex != -1)
                Helpers.SysOpen(TVSettings.Instance.LibraryFolders[this.lstFMIgnoreFolders.SelectedIndex]);
        }

        private void lstFMMonitorFolders_DoubleClick(object sender, System.EventArgs e)
        {
            bnOpenMonFolder_Click(null, null);
        }

        private void bnCheck_Click(object sender, System.EventArgs e)
        {
            DoCheck();
        }

        private void DoCheck()
        {
            this.tbResults.Parent = this.tabControl1;

            this.tabControl1.SelectedTab = this.tbResults;
            this.tabControl1.Update();

            this.FMPStopNow = false;
            this.FMPUpto = "Checking folders";
            this.FMPPercent = 0;

            Thread fmpshower = new Thread(FMPShower) {Name = "'Bulk Add Shows' Progress (Folder Check)"};
            fmpshower.Start();

            while (this.ProgressDialog == null || !this.ProgressDialog.Ready)
            {
                Thread.Sleep(10);
            }

            this.engine.CheckFolders(ref this.FMPStopNow, ref this.FMPPercent);
            
            this.FMPStopNow = true;

            FillNewShowList(false);

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
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                        TVSettings.Instance.LibraryFolders.Add(path.ToLower());
                }
                catch
                {
                    // ignored
                }
            }
            this.mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void lstFMIgnoreFolders_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                        TVSettings.Instance.IgnoreFolders.Add(path.ToLower());
                }
                catch
                {
                    // ignored
                }
            }
            this.mDoc.SetDirty();
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
            if (this.engine.AddItems.Count == 0)
                return;

            this.FMPStopNow = false;
            this.FMPUpto = "Identifying shows";
            this.FMPPercent = 0;

            Thread fmpshower = new Thread(FMPShower) {Name = "Bulk Add Shows Progress (Full Auto)"};
            fmpshower.Start();

            while ((this.ProgressDialog == null) || (!this.ProgressDialog.Ready))
                Thread.Sleep(10);

            int n = 0;
            int n2 = this.engine.AddItems.Count;

            foreach (FolderMonitorEntry ai in this.engine.AddItems)
            {
                this.FMPPercent = 100 * n++ / n2;

               if (this.FMPStopNow)
                   break;

                if (ai.CodeKnown)
                    continue;

                int p = ai.Folder.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                this.FMPUpto = ai.Folder.Substring(p + 1); // +1 makes -1 "not found" result ok
                
                BulkAddManager.GuessShowItem(ai,this.mDoc.Library);
                
                // update our display
                UpdateListItem(ai, true);
                this.lvFMNewShows.Update();
                Update();
            }
            this.FMPStopNow = true;
        }

        private void bnRemoveNewFolder_Click(object sender, System.EventArgs e)
        {
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem lvi in this.lvFMNewShows.SelectedItems)
            {
                FolderMonitorEntry ai = (FolderMonitorEntry)(lvi.Tag);
                this.engine.AddItems.Remove(ai);
            }
            FillNewShowList(false);
        }

        private void bnIgnoreNewFolder_Click(object sender, System.EventArgs e)
        {
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;

            DialogResult res = MessageBox.Show("Add selected folders to the 'Bulk Add Shows' ignore folders list?", "Bulk Add Shows", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes)
                return;

            foreach (ListViewItem lvi in this.lvFMNewShows.SelectedItems)
            {
                FolderMonitorEntry ai = (FolderMonitorEntry)(lvi.Tag);
                TVSettings.Instance.IgnoreFolders.Add(ai.Folder.ToLower());
                this.engine.AddItems.Remove(ai);
            }
            this.mDoc.SetDirty();
            FillNewShowList(false);
            FillFolderStringLists();
        }

        private void lvFMNewShows_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lvFMNewShows_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                    {
                        this.engine.CheckFolderForShows(di, true,out DirectoryInfo[] _);
                        FillNewShowList(true);
                    }
                }
                catch
                {
                    // ignored
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
            if (this.lvFMNewShows.SelectedItems.Count == 0)
            {
                return;
            }

            if (this.lvFMNewShows.SelectedItems[0].Tag is FolderMonitorEntry ai)
            {

                Helpers.SysOpen(ai.Folder);
            }
        }

        private void FillNewShowList(bool keepSel)
        {
            System.Collections.Generic.List<int> sel = new System.Collections.Generic.List<int>();
            if (keepSel)
            {
                foreach (int i in this.lvFMNewShows.SelectedIndices)
                    sel.Add(i);
            }

            this.lvFMNewShows.BeginUpdate();
            this.lvFMNewShows.Items.Clear();

            foreach (FolderMonitorEntry ai in this.engine.AddItems)
            {
                ListViewItem lvi = new ListViewItem();
                UpdateResultEntry(ai, lvi);
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
            this.lvFMNewShows.Update();
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

        private void UpdateListItem(FolderMonitorEntry ai, bool makevis)
        {
            foreach (ListViewItem lvi in this.lvFMNewShows.Items)
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
            if (this.engine.AddItems.Count > 0)
            {
                DialogResult res = MessageBox.Show("Add identified shows to \"My Shows\"?", "Bulk Add Shows", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes)
                    return;

                this.engine.AddAllToMyShows();
            }

            Close();
        }

        private void bnVisitTVcom_Click(object sender, System.EventArgs e)
        {
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;

            FolderMonitorEntry fme = this.lvFMNewShows.SelectedItems[0].Tag as FolderMonitorEntry;
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
            if (this.lvFMNewShows.SelectedItems.Count == 0)
                return;

            FolderMonitorEntry fme = this.lvFMNewShows.SelectedItems[0].Tag as FolderMonitorEntry;
            EditEntry(fme);
            UpdateListItem(fme, true);
        }

        private void EditEntry(FolderMonitorEntry fme)
        {
            FolderMonitorEdit ed = new FolderMonitorEdit(fme);
            if ((ed.ShowDialog() != DialogResult.OK )|| (ed.Code == -1))
                return;

            fme.TVDBCode = ed.Code;
        }

        private void lstFMMonitorFolders_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.bnRemoveMonFolder.Enabled = (this.lstFMMonitorFolders.SelectedIndices.Count > 0);
            this.bnOpenMonFolder.Enabled = (this.lstFMMonitorFolders.SelectedIndices.Count > 0);
        }

        private void lstFMIgnoreFolders_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.bnRemoveIgFolder.Enabled = (this.lstFMIgnoreFolders.SelectedIndices.Count > 0);
            this.bnOpenIgFolder.Enabled = (this.lstFMIgnoreFolders.SelectedIndices.Count > 0);
        }

        private void lvFMNewShows_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bool somethingSelected = (this.lvFMNewShows.SelectedItems.Count > 0);
            this.bnEditEntry.Enabled = somethingSelected;
            this.bnRemoveNewFolder.Enabled = somethingSelected;
            this.bnIgnoreNewFolder.Enabled = somethingSelected;
            this.bnVisitTVcom.Enabled = somethingSelected;
            this.bnNewFolderOpen.Enabled = somethingSelected;
        }
    }
}
