//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using DaveChambers.FolderBrowserDialogEx;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

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
    public partial class BulkAddShow : Form
    {
        private FolderMonitorProgress? progressDialog;
        public int FmpPercent;
        public string FmpUpto;
        public CancellationTokenSource TokenSource;
        private readonly TVDoc mDoc;
        private readonly BulkAddSeriesManager engine;
        private readonly UI mainUi;

        public BulkAddShow(TVDoc doc, BulkAddSeriesManager bam,UI mainUi)
        {
            mDoc = doc;
            engine = bam;
            this.mainUi = mainUi;

            InitializeComponent();

            FillFolderStringLists();
            TokenSource = new CancellationTokenSource();
            tbResults.Parent = null;
            FmpUpto = string.Empty;
        }

        private void FmpShower()
        {
            progressDialog = new FolderMonitorProgress(this);
            if (!progressDialog.IsDisposed)
            {
                progressDialog?.ShowDialog();
            }

            progressDialog = null;
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            if (!CanClose())
            {
                if (DialogResult.OK != MessageBox.Show("Close without adding identified shows to \"TV Shows\"?", "Bulk Add Shows", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    return;
                }
            }

            Close();
        }

        private bool CanClose()
        {
            return engine.AddItems.All(fme => !fme.CodeKnown);
        }

        private void FillFolderStringLists()
        {
            TVSettings.Instance.LibraryFolders.Sort();
            TVSettings.Instance.IgnoreFolders.Sort();

            lstFMMonitorFolders.BeginUpdate();
            lstFMMonitorFolders.Items.Clear();

            foreach (string folder in TVSettings.Instance.LibraryFolders)
            {
                lstFMMonitorFolders.Items.Add(folder);
            }

            lstFMMonitorFolders.EndUpdate();

            lstFMIgnoreFolders.BeginUpdate();
            lstFMIgnoreFolders.Items.Clear();

            foreach (string folder in TVSettings.Instance.IgnoreFolders)
            {
                lstFMIgnoreFolders.Items.Add(folder);
            }

            lstFMIgnoreFolders.EndUpdate();
        }

        private void bnRemoveMonFolder_Click(object sender, System.EventArgs e)
        {
            DeleteSelectedFolder(lstFMMonitorFolders, TVSettings.Instance.LibraryFolders);
        }

        private void DeleteSelectedFolder([NotNull] ListBox lb, SafeList<string> folders)
        {
            for (int i = lb.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = lb.SelectedIndices[i];
                folders.RemoveAt(n);
            }

            mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void bnRemoveIgFolder_Click(object sender, System.EventArgs e)
        {
            DeleteSelectedFolder(lstFMIgnoreFolders, TVSettings.Instance.IgnoreFolders);
        }

        private void bnAddMonFolder_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialogEx searchFolderBrowser = new()
            {
                SelectedPath = "",
                Title = "Add New Monitor Folder...",
                ShowEditbox = true,
                StartPosition = FormStartPosition.CenterParent
            };

            if (lstFMMonitorFolders.SelectedIndex != -1)
            {
                int n = lstFMMonitorFolders.SelectedIndex;
                searchFolderBrowser.SelectedPath = TVSettings.Instance.LibraryFolders[n];
            }

            if (searchFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                if (Directory.Exists(searchFolderBrowser.SelectedPath))
                {
                    TVSettings.Instance.LibraryFolders.Add(searchFolderBrowser.SelectedPath);
                    mDoc.SetDirty();
                    FillFolderStringLists();
                }
            }
        }

        private void bnAddIgFolder_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialogEx ignoreFolderBrowser = new()
            {
                SelectedPath = "",
                Title = "Add New Ignore Folder...",
                ShowEditbox = true,
                StartPosition = FormStartPosition.CenterParent
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
            OpenSelectedFolder();
        }

        private void OpenSelectedFolder()
        {
            if (lstFMMonitorFolders.SelectedIndex != -1)
            {
                Helpers.OpenFolder(TVSettings.Instance.LibraryFolders[lstFMMonitorFolders.SelectedIndex]);
            }
        }

        private void bnOpenIgFolder_Click(object sender, System.EventArgs e)
        {
            if (lstFMIgnoreFolders.SelectedIndex != -1)
            {
                Helpers.OpenFolder(TVSettings.Instance.LibraryFolders[lstFMIgnoreFolders.SelectedIndex]);
            }
        }

        private void lstFMMonitorFolders_DoubleClick(object sender, System.EventArgs e)
        {
            OpenSelectedFolder();
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

            CancellationTokenSource cts = new();
            TokenSource = cts;

            FmpUpto = "Checking folders";
            FmpPercent = 0;

            Thread fmpshower = new(FmpShower) { Name = "'Bulk Add Shows' Progress (Folder Check)" };
            fmpshower.Start();

            while (progressDialog is null || !progressDialog.Ready)
            {
                Thread.Sleep(10);
            }

            engine.CheckFolders(cts.Token, UpdateProgress, true, true);
            cts.Cancel();
            FillNewShowList(false);
        }

        private void UpdateProgress(int percent, string message, string lastUpdated)
        {
            FmpPercent = percent;
        }

        private void lstFMMonitorFolders_DragOver(object _, [NotNull] DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lstFMIgnoreFolders_DragOver(object _, [NotNull] DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lstFMMonitorFolders_DragDrop(object _, [NotNull] DragEventArgs e)
        {
            AddDraggedFiles(e, TVSettings.Instance.LibraryFolders);
        }

        private void lvFMNewShows_DragDrop(object _, [NotNull] DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new(path);
                    if (di.Exists)
                    {
                        engine.CheckFolderForShows(di, true, true, true);
                        FillNewShowList(true);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void lstFMIgnoreFolders_DragDrop(object _, [NotNull] DragEventArgs e)
        {
            AddDraggedFiles(e, TVSettings.Instance.IgnoreFolders);
        }

        private void AddDraggedFiles([NotNull] DragEventArgs e, SafeList<string> strings)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new(path);
                    if (di.Exists)
                    {
                        strings.Add(path.ToLower());
                    }
                }
                catch
                {
                    // ignored
                }
            }
            mDoc.SetDirty();
            FillFolderStringLists();
        }

        private void lstFMMonitorFolders_KeyDown(object _, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedFolder(lstFMMonitorFolders, TVSettings.Instance.LibraryFolders);
            }
        }

        private void lstFMIgnoreFolders_KeyDown(object _, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedFolder(lstFMIgnoreFolders, TVSettings.Instance.IgnoreFolders);
            }
        }

        private void bnFullAuto_Click(object _, System.EventArgs e)
        {
            if (engine.AddItems.Count == 0)
            {
                return;
            }

            CancellationTokenSource cts = new();
            TokenSource = cts;

            FmpUpto = "Identifying shows";
            FmpPercent = 0;

            Thread fmpshower = new(FmpShower) { Name = "Bulk Add Shows Progress (Full Auto)" };
            fmpshower.Start();

            while (progressDialog is null || !progressDialog.Ready)
            {
                Thread.Sleep(10);
            }

            int n = 0;
            int n2 = engine.AddItems.Count;

            foreach (PossibleNewTvShow ai in engine.AddItems)
            {
                FmpPercent = 100 * n++ / n2;

                if (cts.IsCancellationRequested)
                {
                    break;
                }

                if (ai.CodeKnown)
                {
                    continue;
                }

                BulkAddSeriesManager.GuessShowItem(ai, mDoc.TvLibrary, true);

                // update our display
                UpdateListItem(ai, true);
                lvFMNewShows.Update();
                Update();
            }
            cts.Cancel();
        }

        private void bnRemoveNewFolder_Click(object _, System.EventArgs e)
        {
            RemoveNewFolder();
        }

        private void RemoveNewFolder()
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
            {
                return;
            }

            foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
            {
                PossibleNewTvShow ai = (PossibleNewTvShow)lvi.Tag;
                engine.AddItems.Remove(ai);
            }

            FillNewShowList(false);
        }

        private void bnIgnoreNewFolder_Click(object _, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
            {
                return;
            }

            DialogResult res = MessageBox.Show("Add selected folders to the 'Bulk Add Shows' ignore folders list?", "Bulk Add Shows", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes)
            {
                return;
            }

            foreach (PossibleNewTvShow ai in lvFMNewShows.SelectedItems.Cast<ListViewItem>()
                .Select(lvi => (PossibleNewTvShow)lvi.Tag))
            {
                TVSettings.Instance.IgnoreFolders.Add(ai.Folder.FullName.ToLower());
                engine.AddItems.Remove(ai);
            }
            mDoc.SetDirty();
            FillNewShowList(false);
            FillFolderStringLists();
        }

        private void lvFMNewShows_DragOver(object _, [NotNull] DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lvFMNewShows_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveNewFolder();
            }
        }

        private void bnNewFolderOpen_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
            {
                return;
            }

            if (lvFMNewShows.SelectedItems[0].Tag is PossibleNewTvShow ai)
            {
                Helpers.OpenFolder(ai.Folder.FullName);
            }
        }

        private void FillNewShowList(bool keepSel)
        {
            List<int> sel = new();
            if (keepSel)
            {
                foreach (int i in lvFMNewShows.SelectedIndices)
                {
                    sel.Add(i);
                }
            }

            lvFMNewShows.BeginUpdate();
            lvFMNewShows.Items.Clear();

            foreach (PossibleNewTvShow ai in engine.AddItems)
            {
                ListViewItem lvi = new();
                UpdateResultEntry(ai, lvi);
                lvFMNewShows.Items.Add(lvi);
                lvi.ImageIndex = 0;
            }

            if (keepSel)
            {
                foreach (int i in sel)
                {
                    if (i < lvFMNewShows.Items.Count)
                    {
                        lvFMNewShows.Items[i].Selected = true;
                    }
                }
            }

            lvFMNewShows.EndUpdate();
            lvFMNewShows.Update();
        }

        private static void UpdateResultEntry([NotNull] PossibleNewTvShow ai, [NotNull] ListViewItem lvi)
        {
            lvi.SubItems.Clear();
            lvi.Text = ai.Folder.FullName;
            if (ai.CodeKnown)
            {
                CachedSeriesInfo? x = ai.CachedSeries;

                lvi.SubItems.Add(x?.Name);
                lvi.SubItems.Add(ai.HasSeasonFoldersGuess ? "Folder per season" : "Flat");
                lvi.SubItems.Add(ai.CodeKnown ? ai.ProviderCode.ToString() : string.Empty);
            }
            else
            {
                lvi.SubItems.Add(ai.RefinedHint);
                lvi.SubItems.Add(ai.HasSeasonFoldersGuess ? "Folder per season" : "Flat");
                lvi.SubItems.Add(string.Empty);
            }

            lvi.Tag = ai;
            lvi.ImageIndex = ai.CodeKnown && !string.IsNullOrWhiteSpace(ai.Folder.FullName) ? 1 : 0;
        }

        private void UpdateListItem(PossibleNewTvShow ai, bool makevis)
        {
            foreach (ListViewItem lvi in lvFMNewShows.Items.Cast<ListViewItem>().Where(lvi => lvi.Tag == ai))
            {
                UpdateResultEntry(ai, lvi);

                if (makevis)
                {
                    lvi.EnsureVisible();
                }
            }
        }

        private void bnFolderMonitorDone_Click(object sender, System.EventArgs e)
        {
            if (engine.AddItems.Count > 0)
            {
                DialogResult res = MessageBox.Show("Add identified shows to \"TV Shows\"?", "Bulk Add Shows", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes)
                {
                    return;
                }

                engine.AddAllToMyShows(mainUi);
            }

            Close();
        }

        private void bnVisitTVcom_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
            {
                return;
            }

            if (lvFMNewShows.SelectedItems[0].Tag is not PossibleNewTvShow fme)
            {
                return;
            }

            if (!fme.CodeKnown)
            {
                return;
            }

            switch (fme.Provider)
            {
                case TVDoc.ProviderType.TheTVDB:
                    Helpers.OpenUrl(TheTVDB.API.WebsiteShowUrl(fme.ProviderCode));
                    break;

                case TVDoc.ProviderType.TVmaze:
                    Helpers.OpenUrl(TVmaze.LocalCache.Instance.GetSeries(fme.ProviderCode)?.WebUrl ?? string.Empty);
                    break;

                case TVDoc.ProviderType.TMDB:
                    Helpers.OpenUrl(TMDB.API.WebsiteShowUrl(fme.ProviderCode));
                    break;
            }
        }

        private void bnCheck2_Click(object sender, System.EventArgs e)
        {
            DoCheck();
        }

        private void lvFMNewShows_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditEntry();
        }

        private void bnEditEntry_Click(object sender, System.EventArgs e)
        {
            EditEntry();
        }

        private void EditEntry()
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
            {
                return;
            }

            if (lvFMNewShows.SelectedItems[0].Tag is PossibleNewTvShow fme)
            {
                EditEntry(fme);
                UpdateListItem(fme, true);
            }
        }

        private void EditEntry([NotNull] PossibleNewTvShow fme)
        {
            BulkAddEditShow ed = new(fme);
            if (ed.ShowDialog(this) != DialogResult.OK || ed.Code == -1)
            {
                return;
            }

            fme.UpdateId(ed.Code, ed.ProviderType);
        }

        private void lstFMMonitorFolders_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bnRemoveMonFolder.Enabled = lstFMMonitorFolders.SelectedIndices.Count > 0;
            bnOpenMonFolder.Enabled = lstFMMonitorFolders.SelectedIndices.Count > 0;
        }

        private void lstFMIgnoreFolders_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bnRemoveIgFolder.Enabled = lstFMIgnoreFolders.SelectedIndices.Count > 0;
            bnOpenIgFolder.Enabled = lstFMIgnoreFolders.SelectedIndices.Count > 0;
        }

        private void lvFMNewShows_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bool somethingSelected = lvFMNewShows.SelectedItems.Count > 0;
            bnEditEntry.Enabled = somethingSelected;
            bnRemoveNewFolder.Enabled = somethingSelected;
            bnIgnoreNewFolder.Enabled = somethingSelected;
            bnVisitTVcom.Enabled = somethingSelected;
            bnNewFolderOpen.Enabled = somethingSelected;
        }
    }
}
