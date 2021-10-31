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
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    public partial class BulkAddMovie : Form
    {
        //public CancellationTokenSource TokenSource;
        private readonly TVDoc mDoc;

        private readonly BulkAddMovieManager engine;
        private readonly UI mainUi;

        //Thread safe counters to work out the progress
        //For auto id
        private static volatile int VolatileCounter;

        public BulkAddMovie(TVDoc doc, BulkAddMovieManager bam, UI mainUi)
        {
            mDoc = doc;
            engine = bam;
            this.mainUi = mainUi;
            InitializeComponent();
            FillFolderStringLists();
            tbResults.Parent = null;
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            if (!CanClose())
            {
                if (DialogResult.OK != MessageBox.Show("Close without adding identified shows to \"My Movies\"?", "Bulk Add Movies", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
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
            TVSettings.Instance.MovieLibraryFolders.Sort();
            TVSettings.Instance.IgnoreFolders.Sort();

            lstFMMonitorFolders.BeginUpdate();
            lstFMMonitorFolders.Items.Clear();

            foreach (string folder in TVSettings.Instance.MovieLibraryFolders)
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
            DeleteSelectedFolder(lstFMMonitorFolders, TVSettings.Instance.MovieLibraryFolders);
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
                Title = "Add New Movie Base Folder...",
                ShowEditbox = true,
                StartPosition = FormStartPosition.CenterParent
            };

            if (lstFMMonitorFolders.SelectedIndex != -1)
            {
                int n = lstFMMonitorFolders.SelectedIndex;
                searchFolderBrowser.SelectedPath = TVSettings.Instance.MovieLibraryFolders[n];
            }

            if (searchFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                if (Directory.Exists(searchFolderBrowser.SelectedPath))
                {
                    TVSettings.Instance.MovieLibraryFolders.Add(searchFolderBrowser.SelectedPath);
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
            OpenSelectedFolder();
        }

        private void OpenSelectedFolder()
        {
            if (lstFMMonitorFolders.SelectedIndex != -1)
            {
                Helpers.OpenFolder(TVSettings.Instance.MovieLibraryFolders[lstFMMonitorFolders.SelectedIndex]);
            }
        }

        private void bnOpenIgFolder_Click(object sender, System.EventArgs e)
        {
            if (lstFMIgnoreFolders.SelectedIndex != -1)
            {
                Helpers.OpenFolder(TVSettings.Instance.IgnoreFolders[lstFMIgnoreFolders.SelectedIndex]);
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

            bnFullAuto.Enabled = false;
            pbProgress.Visible = true;
            lblStatusLabel.Visible = true;

            bwRescan.RunWorkerAsync();
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
            AddDraggedFiles(e, TVSettings.Instance.MovieLibraryFolders);
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
                        engine.CheckFolderForMovies(di, true, true, true);
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
                DeleteSelectedFolder(lstFMMonitorFolders, TVSettings.Instance.MovieLibraryFolders);
            }
        }

        private void lstFMIgnoreFolders_KeyDown(object _, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedFolder(lstFMIgnoreFolders, TVSettings.Instance.IgnoreFolders);
            }
        }

        private void bnFullAuto_Click(object sender, System.EventArgs e)
        {
            if (engine.AddItems.Count == 0)
            {
                return;
            }

            bnFullAuto.Enabled = false;
            pbProgress.Visible = true;
            lblStatusLabel.Visible = true;
            bwIdentify.RunWorkerAsync();
        }

        private void AutoMatchMovie([NotNull] CancellationTokenSource cts, PossibleNewMovie ai, BackgroundWorker bw, int total)
        {
            if (cts.IsCancellationRequested)
            {
                return;
            }

            if (ai.CodeKnown)
            {
                return;
            }

            ai.GuessMovie(true);
            Interlocked.Increment(ref VolatileCounter);
            bw.ReportProgress((int)100.0 * VolatileCounter / total, ai);
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

            foreach (PossibleNewMovie ai in lvFMNewShows.SelectedItems.Cast<ListViewItem>()
                .Select(lvi => lvi.Tag).Cast<PossibleNewMovie>())
            {
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

            DialogResult res = MessageBox.Show("Add selected folders to the 'Bulk Add Movies' ignore folders list?", "Bulk Add Movies", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes)
            {
                return;
            }

            foreach (PossibleNewMovie ai in lvFMNewShows.SelectedItems.Cast<ListViewItem>()
                .Select(lvi => (PossibleNewMovie)lvi.Tag))
            {
                TVSettings.Instance.IgnoreFolders.Add(ai.Directory.FullName.ToLower());
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

            if (lvFMNewShows.SelectedItems[0].Tag is PossibleNewMovie ai)
            {
                Helpers.OpenFolder(ai.Directory.FullName);
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

            foreach (PossibleNewMovie ai in engine.AddItems)
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

        private static void UpdateResultEntry([NotNull] PossibleNewMovie ai, [NotNull] ListViewItem lvi)
        {
            lvi.SubItems.Clear();
            lvi.Text = ai.Directory.FullName;
            if (ai.CodeKnown)
            {
                CachedMovieInfo? x = ai.CachedMovie;
                lvi.SubItems.Add(x?.Name);
                string val = x?.FirstAired?.Year.ToString();
                lvi.SubItems.Add(val ?? string.Empty);
                lvi.SubItems.Add(ai.CodeString);
            }
            else
            {
                lvi.SubItems.Add(ai.RefinedHint);
                lvi.SubItems.Add(ai.PossibleYear.ToString());
                lvi.SubItems.Add(string.Empty);
            }
            lvi.Tag = ai;
            lvi.ImageIndex = ai.CodeKnown && !string.IsNullOrWhiteSpace(ai.MovieStub) ? 1 : 0;
        }

        private void UpdateListItem(PossibleNewMovie ai, bool makevis)
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
            int numberToAdd = engine.AddItems.Count(ai => !ai.CodeUnknown);
            if (numberToAdd > 0)
            {
                DialogResult res = MessageBox.Show($"Add {numberToAdd} identified movies to \"My Movies\"?", "Bulk Add Movies", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes)
                {
                    return;
                }

                engine.AddAllToMyMovies(mainUi);
            }

            Close();
        }

        private void bnVisitTVcom_Click(object sender, System.EventArgs e)
        {
            if (lvFMNewShows.SelectedItems.Count == 0)
            {
                return;
            }

            if (!(lvFMNewShows.SelectedItems[0].Tag is PossibleNewMovie fme))
            {
                return;
            }

            if (fme.CodeKnown)
            {
                switch (fme.SourceProvider)
                {
                    case TVDoc.ProviderType.TheTVDB:
                        Helpers.OpenUrl(TheTVDB.API.WebsiteMovieUrl(fme.ProviderCode));
                        break;

                    case TVDoc.ProviderType.TMDB:
                        Helpers.OpenUrl(TMDB.API.WebsiteMovieUrl(fme.ProviderCode));
                        break;
                }
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

            if (lvFMNewShows.SelectedItems[0].Tag is PossibleNewMovie fme)
            {
                EditEntry(fme);
                UpdateListItem(fme, true);
            }
            FillNewShowList(false);
        }

        private void EditEntry([NotNull] PossibleNewMovie fme)
        {
            BulkAddEditMovie ed = new(fme);
            if (ed.ShowDialog(this) != DialogResult.OK || ed.Code == -1)
            {
                return;
            }

            fme.SetId(ed.Code, ed.Provider);
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

        private void bwRescan_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name ??= "BulkAddMovie Scan Thread"; // Can only set it once

            CancellationTokenSource cts = new();
            engine.CheckFolders(cts.Token, (BackgroundWorker)sender, true, true);
            cts.Cancel();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name ??= "BulkAddMovie Identify Thread"; // Can only set it once
            IdentifyAll((BackgroundWorker)sender);
        }

        private void IdentifyAll(BackgroundWorker bw)
        {
            CancellationTokenSource cts = new();
            //TokenSource = cts;

            VolatileCounter = 0;

            Parallel.ForEach(engine.AddItems, movie =>
            {
                Thread.CurrentThread.Name ??= $" Identify {movie.Name}"; // Can only set it once
                AutoMatchMovie(cts, movie, bw, engine.AddItems.Count);
            });

            cts.Cancel();
        }

        private void bwIdentify_ProgressChanged(object sender, [NotNull] ProgressChangedEventArgs e)
        {
            lvFMNewShows.Update();

            pbProgress.Value = e.ProgressPercentage;
            lblStatusLabel.Text = ((PossibleNewMovie)e.UserState).RefinedHint;
            UpdateListItem((PossibleNewMovie)e.UserState, false);
        }

        private void bwIdentify_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bnFullAuto.Enabled = true;
            pbProgress.Visible = false;
            lblStatusLabel.Visible = false;
        }

        private void bwRescan_ProgressChanged(object sender, [NotNull] ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
            lblStatusLabel.Text = e.UserState.ToString();
        }

        private void bwRescan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bnFullAuto.Enabled = true;
            pbProgress.Visible = false;
            lblStatusLabel.Visible = false;
            FillNewShowList(false);
        }
    }
}
