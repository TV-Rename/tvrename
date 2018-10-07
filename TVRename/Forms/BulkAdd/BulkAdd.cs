// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
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
        private FolderMonitorProgress progressDialog;
        public int FmpPercent;
        public bool FmpStopNow;
        public string FmpUpto;
        private readonly TVDoc mDoc;
        private readonly BulkAddManager engine;

        public FolderMonitor(TVDoc doc,BulkAddManager bam)
        {
            mDoc = doc;
            engine = bam;

            InitializeComponent();

            FillFolderStringLists();
            tbResults.Parent = null;
        }

        private void FmpShower()
        {
            progressDialog = new FolderMonitorProgress(this);
            progressDialog.ShowDialog();
            progressDialog = null;
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
            foreach (FoundFolder fme in engine.AddItems)
            {
                if (fme.CodeKnown) {return false;}
            }

            return true;
        }

        private void FillFolderStringLists()
        {
            TVSettings.Instance.LibraryFolders.Sort();
            TVSettings.Instance.IgnoreFolders.Sort();
            
            lstFMMonitorFolders.BeginUpdate();
            lstFMMonitorFolders.Items.Clear();
            
            foreach (string folder in TVSettings.Instance.LibraryFolders)
                lstFMMonitorFolders.Items.Add(folder);

            lstFMMonitorFolders.EndUpdate();

            lstFMIgnoreFolders.BeginUpdate();
            lstFMIgnoreFolders.Items.Clear();

            foreach (string folder in TVSettings.Instance.IgnoreFolders)
                lstFMIgnoreFolders.Items.Add(folder);

            lstFMIgnoreFolders.EndUpdate();
        }

        private void bnRemoveMonFolder_Click(object sender, System.EventArgs e)
        {
            for (int i = lstFMMonitorFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = lstFMMonitorFolders.SelectedIndices[i];
                TVSettings.Instance.LibraryFolders.RemoveAt(n);
            }
            mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void bnRemoveIgFolder_Click(object sender, System.EventArgs e)
        {
            for (int i = lstFMIgnoreFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = lstFMIgnoreFolders.SelectedIndices[i];
                TVSettings.Instance.IgnoreFolders.RemoveAt(n);
            }
            mDoc.SetDirty();
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

            if (lstFMMonitorFolders.SelectedIndex != -1)
            {
                int n = lstFMMonitorFolders.SelectedIndex;
                searchFolderBrowser.SelectedPath = TVSettings.Instance.LibraryFolders[n];
            }

            if (searchFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                TVSettings.Instance.LibraryFolders.Add(searchFolderBrowser.SelectedPath.ToLower());
                mDoc.SetDirty();
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

            if (lstFMIgnoreFolders.SelectedIndex != -1)
            {
                ignoreFolderBrowser.SelectedPath = TVSettings.Instance.IgnoreFolders[lstFMIgnoreFolders.SelectedIndex];
            }

            if (ignoreFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                TVSettings.Instance.IgnoreFolders.Add(ignoreFolderBrowser.SelectedPath.ToLower());
                mDoc.SetDirty();
                FillFolderStringLists();
            }
        }

        private void bnOpenMonFolder_Click(object sender, System.EventArgs e)
        {
            if (lstFMMonitorFolders.SelectedIndex != -1)
                Helpers.SysOpen(TVSettings.Instance.LibraryFolders[lstFMMonitorFolders.SelectedIndex]);
        }

        private void bnOpenIgFolder_Click(object sender, System.EventArgs e)
        {
            if (lstFMIgnoreFolders.SelectedIndex != -1)
                Helpers.SysOpen(TVSettings.Instance.LibraryFolders[lstFMIgnoreFolders.SelectedIndex]);
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
            tbResults.Parent = tabControl1;

            tabControl1.SelectedTab = tbResults;
            tabControl1.Update();

            FmpStopNow = false;
            FmpUpto = "Checking folders";
            FmpPercent = 0;

            Thread fmpshower = new Thread(FmpShower) {Name = "'Bulk Add Shows' Progress (Folder Check)"};
            fmpshower.Start();

            while (progressDialog == null || !progressDialog.Ready)
            {
                Thread.Sleep(10);
            }

            engine.CheckFolders(ref FmpStopNow, ref FmpPercent);
            
            FmpStopNow = true;

            FillNewShowList(false);
        }

        private static void lstFMMonitorFolders_DragOver(object _, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private static void lstFMIgnoreFolders_DragOver(object _, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lstFMMonitorFolders_DragDrop(object _, DragEventArgs e)
        {
            AddDraggedFiles(e, TVSettings.Instance.LibraryFolders);
        }

        private void lvFMNewShows_DragDrop(object _, DragEventArgs e)
        {
            string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop));
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                    {
                        engine.CheckFolderForShows(di, true, out DirectoryInfo[] _);
                        FillNewShowList(true);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void lstFMIgnoreFolders_DragDrop(object _, DragEventArgs e)
        {
            AddDraggedFiles(e, TVSettings.Instance.IgnoreFolders);
        }

        private void AddDraggedFiles(DragEventArgs e, List<string> strings)
        {
            string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop));
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                        strings.Add(path.ToLower());
                }
                catch
                {
                    // ignored
                }
            }
            mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void lstFMMonitorFolders_KeyDown(object _, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                bnRemoveMonFolder_Click(null, null);
        }

        private void lstFMIgnoreFolders_KeyDown(object _, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                bnRemoveIgFolder_Click(null, null);
        }

        private void bnFullAuto_Click(object _, System.EventArgs e)
        {
            if (engine.AddItems.Count == 0)
                return;

            FmpStopNow = false;
            FmpUpto = "Identifying shows";
            FmpPercent = 0;

            Thread fmpshower = new Thread(FmpShower) {Name = "Bulk Add Shows Progress (Full Auto)"};
            fmpshower.Start();

            while ((progressDialog == null) || (!progressDialog.Ready))
            {
                Thread.Sleep(10);
            }

            int n = 0;
            int n2 = engine.AddItems.Count;

            foreach (FoundFolder ai in engine.AddItems)
            {
                FmpPercent = 100 * n++ / n2;

               if (FmpStopNow)
                   break;

                if (ai.CodeKnown)
                    continue;

                int p = ai.Folder.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                FmpUpto = ai.Folder.Substring(p + 1); // +1 makes -1 "not found" result ok
                
                BulkAddManager.GuessShowItem(ai,mDoc.Library);
                
                // update our display
                UpdateListItem(ai, true);
                lvFMNewShows.Update();
                Update();
            }
            FmpStopNow = true;
        }

        private void bnRemoveNewFolder_Click(object _, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
            {
                FoundFolder ai = (FoundFolder)(lvi.Tag);
                engine.AddItems.Remove(ai);
            }
            FillNewShowList(false);
        }

        private void bnIgnoreNewFolder_Click(object _, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
                return;

            DialogResult res = MessageBox.Show("Add selected folders to the 'Bulk Add Shows' ignore folders list?", "Bulk Add Shows", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes)
                return;

            foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
            {
                FoundFolder ai = (FoundFolder)(lvi.Tag);
                TVSettings.Instance.IgnoreFolders.Add(ai.Folder.ToLower());
                engine.AddItems.Remove(ai);
            }
            mDoc.SetDirty();
            FillNewShowList(false);
            FillFolderStringLists();
        }

        private void lvFMNewShows_DragOver(object _, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lvFMNewShows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                bnRemoveNewFolder_Click(null, null);
            }
        }

        private void bnNewFolderOpen_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
            {
                return;
            }

            if (lvFMNewShows.SelectedItems[0].Tag is FoundFolder ai)
            {
                Helpers.SysOpen(ai.Folder);
            }
        }

        private void FillNewShowList(bool keepSel)
        {
            List<int> sel = new List<int>();
            if (keepSel)
            {
                foreach (int i in lvFMNewShows.SelectedIndices)
                    sel.Add(i);
            }

            lvFMNewShows.BeginUpdate();
            lvFMNewShows.Items.Clear();

            foreach (FoundFolder ai in engine.AddItems)
            {
                ListViewItem lvi = new ListViewItem();
                UpdateResultEntry(ai, lvi);
                lvFMNewShows.Items.Add(lvi);
                lvi.ImageIndex = 0;
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

        private void UpdateResultEntry(FoundFolder ai, ListViewItem lvi)
        {
            lvi.SubItems.Clear();
            lvi.Text = ai.Folder;
            lvi.SubItems.Add(ai.CodeKnown ? TheTVDB.Instance.GetSeries(ai.TVDBCode).Name : "");
            lvi.SubItems.Add(ai.HasSeasonFoldersGuess ? "Folder per season" : "Flat");
            lvi.SubItems.Add(ai.CodeKnown ? ai.TVDBCode.ToString() : "");
            lvi.Tag = ai;
            lvi.ImageIndex=(ai.CodeKnown&&!string.IsNullOrWhiteSpace(ai.Folder))?1:0;
        }

        private void UpdateListItem(FoundFolder ai, bool makevis)
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
            if (engine.AddItems.Count > 0)
            {
                DialogResult res = MessageBox.Show("Add identified shows to \"My Shows\"?", "Bulk Add Shows", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes)
                    return;

                engine.AddAllToMyShows();
            }

            Close();
        }

        private void bnVisitTVcom_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
                return;

            FoundFolder fme = lvFMNewShows.SelectedItems[0].Tag as FoundFolder;
            if (fme == null)
                return;

            int code = fme.TVDBCode;
            if (code != -1)
                Helpers.SysOpen(TheTVDB.Instance.WebsiteUrl(code, -1, false));
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

            FoundFolder fme = lvFMNewShows.SelectedItems[0].Tag as FoundFolder;
            EditEntry(fme);
            UpdateListItem(fme, true);
        }

        private void EditEntry(FoundFolder fme)
        {
            FolderMonitorEdit ed = new FolderMonitorEdit(fme);
            if ((ed.ShowDialog() != DialogResult.OK )|| (ed.Code == -1))
                return;

            fme.TVDBCode = ed.Code;
        }

        private void lstFMMonitorFolders_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bnRemoveMonFolder.Enabled = (lstFMMonitorFolders.SelectedIndices.Count > 0);
            bnOpenMonFolder.Enabled = (lstFMMonitorFolders.SelectedIndices.Count > 0);
        }

        private void lstFMIgnoreFolders_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bnRemoveIgFolder.Enabled = (lstFMIgnoreFolders.SelectedIndices.Count > 0);
            bnOpenIgFolder.Enabled = (lstFMIgnoreFolders.SelectedIndices.Count > 0);
        }

        private void lvFMNewShows_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bool somethingSelected = (lvFMNewShows.SelectedItems.Count > 0);
            bnEditEntry.Enabled = somethingSelected;
            bnRemoveNewFolder.Enabled = somethingSelected;
            bnIgnoreNewFolder.Enabled = somethingSelected;
            bnVisitTVcom.Enabled = somethingSelected;
            bnNewFolderOpen.Enabled = somethingSelected;
        }
    }
}
