using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Humanizer;
using Microsoft.WindowsAPICodePack.Dialogs;
using TVRename.Core.Extensions;
using TVRename.Core.Models;
using TVRename.Core.TVDB;
using TVRename.Windows.Configuration;
using TVRename.Windows.Models;

namespace TVRename.Windows.Forms
{
    /// <summary>
    /// Form allowing users to search for and import multiple shows from folders.
    /// </summary>
    public partial class BulkAdd : Form
    {
        public BulkAdd()
        {
            InitializeComponent();
        }

        private void BulkAdd_Load(object sender, EventArgs e)
        {
            this.imageListResults.Images.Add(Properties.Resources.Fail);
            this.imageListResults.Images.Add(Properties.Resources.Pass);

            this.listBoxFolders.BeginUpdate();
            Settings.Instance.SearchDirectories.ForEach(d => this.listBoxFolders.Items.Add(d));
            this.listBoxFolders.EndUpdate();
            if (this.listBoxFolders.Items.Count > 0) this.listBoxFolders.SelectedIndex = 0;
            this.buttonFoldersScan.Enabled = this.listBoxFolders.Items.Count > 0;

            this.listBoxIgnored.BeginUpdate();
            Settings.Instance.IgnoreDirectories.ForEach(d => this.listBoxIgnored.Items.Add(d));
            this.listBoxIgnored.EndUpdate();
            if (this.listBoxIgnored.Items.Count > 0) this.listBoxIgnored.SelectedIndex = 0;

            EnableTab(this.tabPageResults, false);
        }

        private void listBoxFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonFoldersRemove.Enabled = this.listBoxFolders.SelectedItems.Count > 0;
            this.buttonFoldersOpen.Enabled = this.listBoxFolders.SelectedItems.Count > 0;
        }

        private void listBoxFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listBoxFolders.IndexFromPoint(e.Location) == ListBox.NoMatches) return;

            buttonFoldersOpen_Click(sender, e);
        }

        private void buttonFoldersAdd_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false,
                RestoreDirectory = true,
                EnsureValidNames = true,
                EnsurePathExists = true,
                EnsureFileExists = true
            };

            if (ofd.ShowDialog() != CommonFileDialogResult.Ok) return;
            if (this.listBoxFolders.Items.Cast<string>().Any(i => string.Equals(i, ofd.FileName, StringComparison.InvariantCultureIgnoreCase))) return;

            this.listBoxFolders.Items.Add(ofd.FileName);

            this.buttonFoldersScan.Enabled = this.listBoxFolders.Items.Count > 0;
        }

        private void buttonFoldersRemove_Click(object sender, EventArgs e)
        {
            if (this.listBoxFolders.SelectedIndex == ListBox.NoMatches) return;

            this.listBoxFolders.Items.RemoveAt(this.listBoxFolders.SelectedIndex);

            this.buttonFoldersScan.Enabled = this.listBoxFolders.Items.Count > 0;
        }

        private void buttonFoldersOpen_Click(object sender, EventArgs e)
        {
            if (this.listBoxFolders.SelectedIndex == ListBox.NoMatches) return;

            Process.Start(this.listBoxFolders.SelectedItem.ToString());
        }

        private void buttonFoldersScan_Click(object sender, EventArgs e)
        {
            if (this.listViewResults.Items.Count > 0 && MessageBox.Show("Scanning again will clear your previous scan results, do you want to continue?", "TV Rename", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            // Folders tab
            EnableTab(this.tabPageFolders, false);

            // Ignored tag
            EnableTab(this.tabPageIgnored, false);

            // Results tab
            this.buttonResultsId.Enabled = false;
            this.buttonResultsEdit.Enabled = false;
            this.buttonResultsRemove.Enabled = false;
            this.buttonResultsIgnore.Enabled = false;
            this.buttonResultsAdd.Enabled = false;
            this.listViewResults.Items.Clear();
            this.listViewResults.SelectedIndexChanged -= listViewResults_SelectedIndexChanged;

            this.tabControl.SelectedIndex = 1; // Results tab

            this.toolStripStatusLabel.Text = "Scanning folders...";
            this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

            // TODO: Async, full scan
            this.listViewResults.BeginUpdate();
            this.listViewResults.Items.AddRange(SearchAllFolders(this.listBoxFolders.Items.Cast<string>()).ToArray());
            this.listViewResults.EndUpdate();

            // Finished
            this.toolStripStatusLabel.Text = $"Found {"potential show".ToQuantity(this.listViewResults.Items.Count)}";
            this.toolStripProgressBar.Style = ProgressBarStyle.Continuous;

            // Folders tab
            EnableTab(this.tabPageFolders, true);

            // Ignored tag
            EnableTab(this.tabPageIgnored, true);

            // Results tab
            EnableTab(this.tabPageResults, true);
            this.buttonResultsId.Enabled = this.listViewResults.Items.Count > 0;
            this.buttonResultsEdit.Enabled = false;
            this.buttonResultsRemove.Enabled = false;
            this.buttonResultsIgnore.Enabled = false;
            this.buttonResultsAdd.Enabled = false;
            this.listViewResults.SelectedIndexChanged += listViewResults_SelectedIndexChanged;
            if (this.listViewResults.Items.Count > 0) this.listViewResults.Items[0].Selected = true;
        }

        private void listViewResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonResultsEdit.Enabled = this.listViewResults.SelectedItems.Count > 0;
            this.buttonResultsRemove.Enabled = this.listViewResults.SelectedItems.Count > 0;
            this.buttonResultsIgnore.Enabled = this.listViewResults.SelectedItems.Count > 0;
        }

        private void listViewResults_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listViewResults.HitTest(e.Location).Item == null) return;

            buttonResultsEdit_Click(sender, e);
        }

        private async void buttonResultsId_Click(object sender, EventArgs e)
        {
            // Folders tab
            EnableTab(this.tabPageFolders, false);

            // Ignored tag
            EnableTab(this.tabPageIgnored, false);

            // Results tab
            this.buttonResultsId.Text = "&Cancel"; // TODO
            this.buttonResultsId.Enabled = true;
            this.buttonResultsEdit.Enabled = false;
            this.buttonResultsRemove.Enabled = false;
            this.buttonResultsIgnore.Enabled = false;
            this.buttonResultsAdd.Enabled = false;
            this.listViewResults.SelectedIndexChanged -= listViewResults_SelectedIndexChanged;

            List<ListViewItem> items = this.listViewResults.Items.Cast<ListViewItem>().Where(l => l.ImageIndex != 1).ToList();

            this.toolStripStatusLabel.Text = $"Identifing {"show".ToQuantity(items.Count)}...";
            this.toolStripProgressBar.Value = 0;
            this.toolStripProgressBar.Minimum = 0;
            this.toolStripProgressBar.Maximum = items.Count * 2;

            await items.ForEachAsync(async lvi =>
            {
                this.toolStripProgressBar.Value++;

                FoundShowFolder match = await AutoId(lvi.Tag as FoundShowFolder);

                this.toolStripProgressBar.Value++;

                if (match == null) return;

                lvi.ImageIndex = 1;
                lvi.SubItems[1].Text = match.Name;
                lvi.SubItems[3].Text = match.TvdbId.ToString();
            }, TVDB.Instance.Threads, true);

            // Finished
            this.toolStripStatusLabel.Text = $"Identified {this.listViewResults.Items.Cast<ListViewItem>().Count(l => l.ImageIndex == 1)} of {"show".ToQuantity(this.listViewResults.Items.Count)}";
            this.toolStripProgressBar.Value = 0;

            // Folders tab
            EnableTab(this.tabPageFolders, true);

            // Ignored tag
            EnableTab(this.tabPageIgnored, true);

            // Results tab
            this.buttonResultsId.Text = "Auto &ID All";
            this.buttonResultsId.Enabled = true;
            this.buttonResultsEdit.Enabled = false;
            this.buttonResultsRemove.Enabled = false;
            this.buttonResultsIgnore.Enabled = false;
            this.buttonResultsAdd.Enabled = this.listViewResults.Items.Cast<ListViewItem>().Any(l => l.ImageIndex == 1); // Any IDed
            this.listViewResults.SelectedIndexChanged += listViewResults_SelectedIndexChanged;
            if (this.listViewResults.Items.Count > 0) this.listViewResults.Items[0].Selected = true;
        }

        private void buttonResultsEdit_Click(object sender, EventArgs e)
        {

        }

        private void buttonResultsRemove_Click(object sender, EventArgs e)
        {

        }

        private void buttonResultsIgnore_Click(object sender, EventArgs e)
        {

        }

        private void buttonResultsAdd_Click(object sender, EventArgs e)
        {
            IEnumerable<FoundShowFolder> add = this.listViewResults.Items.Cast<ListViewItem>().Where(l => l.ImageIndex == 1).Select(l => l.Tag as FoundShowFolder);

            // TODO: Add

            this.Close();
        }

        private void listBoxIgnored_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonIgnoredRemove.Enabled = this.listBoxIgnored.SelectedItems.Count > 0;
            this.buttonIgnoredOpen.Enabled = this.listBoxIgnored.SelectedItems.Count > 0;
        }

        private void listBoxIgnored_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listBoxIgnored.IndexFromPoint(e.Location) == ListBox.NoMatches) return;

            buttonIgnoredOpen_Click(sender, e);
        }

        private void buttonIgnoredAdd_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false,
                RestoreDirectory = true,
                EnsureValidNames = true,
                EnsurePathExists = true,
                EnsureFileExists = true
            };

            if (ofd.ShowDialog() != CommonFileDialogResult.Ok) return;
            if (this.listBoxIgnored.Items.Cast<string>().Any(i => string.Equals(i, ofd.FileName, StringComparison.InvariantCultureIgnoreCase))) return;

            this.listBoxIgnored.Items.Add(ofd.FileName);
        }

        private void buttonIgnoredRemove_Click(object sender, EventArgs e)
        {
            if (this.listBoxIgnored.SelectedIndex == ListBox.NoMatches) return;

            this.listBoxIgnored.Items.RemoveAt(this.listBoxIgnored.SelectedIndex);
        }

        private void buttonIgnoredOpen_Click(object sender, EventArgs e)
        {
            if (this.listBoxIgnored.SelectedIndex == ListBox.NoMatches) return;

            Process.Start(this.listBoxIgnored.SelectedItem.ToString());
        }

        private void BulkAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO: Warn users to save results
        }

        private static void EnableTab(TabPage page, bool enable)
        {
            foreach (Control control in page.Controls) control.Enabled = enable;
        }

        public async Task<FoundShowFolder> AutoId(FoundShowFolder folder)
        {
            var name = Path.GetFileName(folder.Location);
            var s = await TVDB.Instance.Search(name, CancellationToken.None);

            if (s.Count < 1) return null;

            if (s.Count == 1) return new FoundShowFolder
            {
                Location = folder.Location,
                Structure = folder.Structure,
                TvdbId = s[0].Id,
                Name = s[0].Name
            };

            SearchResult match = s.FirstOrDefault(m => m.Name == name);

            if (match != null) return new FoundShowFolder
            {
                Location = folder.Location,
                Structure = folder.Structure,
                TvdbId = match.Id,
                Name = match.Name
            };

            return null;
        }

        public IEnumerable<ListViewItem> SearchAllFolders(IEnumerable<string> directories)
        {
            IEnumerable<DirectoryInfo> tlds = directories.Select(d => new DirectoryInfo(d)).Where(d => d.Exists).SelectMany(d => d.GetDirectories());

            foreach (DirectoryInfo directory in tlds)
            {
                foreach (FoundShowFolder r in this.SearchFolder(directory))
                {
                    ListViewItem lvi = new ListViewItem(new[]
                    {
                        r.Location,
                        string.Empty,
                        r.Structure == FoundShowFolder.FolderStructure.Seasons ? "Folder per season" : "Flat",
                        string.Empty
                    })
                    {
                        //Checked = true,
                        ImageIndex = 0,
                        Tag = r
                    };

                    yield return lvi;
                }
            }
        }

        public IEnumerable<FoundShowFolder> SearchFolder(DirectoryInfo directory)
        {
            if (Settings.Instance.IgnoreDirectories.Contains(directory.FullName.ToLower())) yield break; // TODO: Better comp

            bool hasSeasonFolders = directory.GetDirectories().Length > 0 && directory.GetDirectories().SelectMany(d => d.GetFiles()).Any();

            yield return new FoundShowFolder
            {
                Location = directory.FullName,
                Structure = hasSeasonFolders ? FoundShowFolder.FolderStructure.Seasons : FoundShowFolder.FolderStructure.Flat
            };
        }
    }
}
