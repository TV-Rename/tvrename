using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Humanizer;
using TVRename.Core.Actions;
using TVRename.Core.Models.Cache;
using TVRename.Core.TVDB;
using TVRename.Core.Utility;
using TVRename.Windows.Configuration;
using TVRename.Windows.Controls;
using static TVRename.Windows.Utilities.Helpers;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Show = TVRename.Core.Models.Show;

namespace TVRename.Windows.Forms
{
    public partial class Main : Form
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Control view;
        private CancellationTokenSource cts;

        public Main()
        {
            InitializeComponent();

            // Double buffer to reduce flicker
            DoubleBuffer(this.listViewScan);
            DoubleBuffer(this.listViewCalendar);

            // Position window
            this.Size = Configuration.Layout.Instance.Window.Size;
            this.Location = Configuration.Layout.Instance.Window.Location;
            this.WindowState = Configuration.Layout.Instance.Window.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
            this.splitContainer.SplitterDistance = Configuration.Layout.Instance.Window.Splitter;

            // Image lists
            this.imageListTabs.Images.Add(Properties.Resources.Tv);
            this.imageListTabs.Images.Add(Properties.Resources.Zoom);
            this.imageListTabs.Images.Add(Properties.Resources.Calendar);

            this.tabControl.TabPages[0].ImageIndex = 0;
            this.tabControl.TabPages[1].ImageIndex = 1;
            this.tabControl.TabPages[2].ImageIndex = 2;

            this.imageListCalendar.Images.Add(Properties.Resources.Scan);
            this.imageListCalendar.Images.Add(Properties.Resources.Disk);

            BuildTree().GetAwaiter().GetResult();
            BuildCalendar().GetAwaiter().GetResult();

            Shown += (s, a) => Logger.Info("Main form started");
        }

        private async Task BuildTree()
        {
            this.treeViewShows.BeginUpdate();
            this.treeViewShows.Nodes.Clear();

            List<TreeNode> results = await Scanner.BuildTree();

            this.treeViewShows.Nodes.AddRange(results.ToArray());

            this.treeViewShows.EndUpdate();

            this.toolStripStatusLabelShows.Text = "show".ToQuantity(this.treeViewShows.Nodes.Count);
        }

        private async Task BuildCalendar()
        {
            this.listViewCalendar.BeginUpdate();
            this.listViewCalendar.Groups["recent"].Header = $"Aired in the last {"day".ToQuantity(Settings.Instance.RecentDays)}";
            this.listViewCalendar.Items.Clear();

            List<ListViewItem> results = await Scanner.BuildCalendar(this.listViewCalendar.Groups);

            this.listViewCalendar.Items.AddRange(results.ToArray());

            this.listViewCalendar.Sort(); // TODO
            this.listViewCalendar.EndUpdate();

            this.calendar.BoldedDates = results.Select(l => (Episode)l.Tag).Where(e => e.FirstAired.HasValue).Select(e => e.FirstAired.Value).Distinct().ToArray();
        }

        private async Task Save()
        {
            this.toolStripButtonSave.Enabled = false;
            this.addToolStripMenuItem.Enabled = false;
            this.toolStripStatusLabel.Text = "Saving settings...";

            Settings.Save();

            await TVDB.Save(Path.Combine(ApplicationBase.BasePath, "tvdb.json"));

            Settings.Instance.Dirty = false;

            this.toolStripStatusLabel.Text = "Ready";
            this.toolStripButtonSave.Enabled = true;
            this.addToolStripMenuItem.Enabled = true;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Text += $" {DisplayVersion}";
        }

        private async void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel.Text = "Adding a show...";

            Add add = new Add();

            if (add.ShowDialog() == DialogResult.OK)
            {
                Show show = add.NewShow;

                if (Settings.Instance.Shows.All(s => s.TVDBId != show.TVDBId))
                {
                    this.toolStripStatusLabel.Text = "Downloading new show...";
                    this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

                    await TVDB.Instance.Add(show.TVDBId, CancellationToken.None);
                    Settings.Instance.Shows.Add(show);

                    this.toolStripProgressBar.Style = ProgressBarStyle.Continuous;
                    this.toolStripStatusLabel.Text = "Ready";

                    await BuildTree();
                    await BuildCalendar();

                    Settings.Instance.Dirty = true;
                }
            }

            this.toolStripStatusLabel.Text = "Ready";
        }

        private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void testErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new Exception("Test error");
        }

        private void mediaCenterFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MediaCenter().ShowDialog();
        }

        private async void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new Preferences().ShowDialog() != DialogResult.OK) return;

            await BuildTree();
            await BuildCalendar();
        }

        private void filenameProcessorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new FilenameProcessors().ShowDialog() != DialogResult.OK) return;

            // TODO
        }

        private void refreshAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TODO"); // TODO

            //await TVDB.Instance.Refresh(CancellationToken.None); // TODO: Warning, UI
        }

        private void bulkAddShowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TODO"); // TODO
        }

        private void onlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.tvrename.com/userguide");
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.tvrename.com/");
        }

        private void toolStripMenuItemStatistics_Click(object sender, EventArgs e)
        {
            new Statistics().ShowDialog();
        }

        private void toolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            addToolStripMenuItem_Click(sender, e);
        }

        private async void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            await Save();
        }

        private async void toolStripButtonUpdate_Click(object sender, EventArgs e)
        {
            this.toolStripButtonUpdate.Enabled = false;
            this.toolStripStatusLabel.Text = "Downloading show updates...";
            this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

            await TVDB.Instance.Update(CancellationToken.None);

            this.toolStripProgressBar.Style = ProgressBarStyle.Continuous;
            this.toolStripStatusLabel.Text = "Ready";
            this.toolStripButtonUpdate.Enabled = true;

            await BuildTree();
            await BuildCalendar();
        }

        private static async Task<bool> CheckMissingDirectories(IEnumerable<Show> shows)
        {
            foreach (Show show in shows)
            {
                if (!show.CheckMissing) continue;

                foreach (Season season in show.Metadata.Seasons.Values)
                {
                    if (show.IgnoredSeasons.Contains(season.Number)) continue;

                    string seasonDir = (season.Number == 0 ? Settings.Instance.SpecialsTemplate : Settings.Instance.SeasonTemplate).Template(season);
                    seasonDir = EscapeTemplatePath(seasonDir);

                    string seasonPath = Path.Combine(show.Location, seasonDir);

                    if (Settings.Instance.AutoCreateFolders) Directory.CreateDirectory(seasonPath);

                    bool retry;

                    do
                    {
                        if (Directory.Exists(seasonPath)) break;

                        retry = false;

                        using (MissingFolderAction form = new MissingFolderAction(show, season, seasonPath))
                        {
                            form.ShowDialog();

                            if (form.Result == MissingFolderAction.ActionResult.Retry)
                            {
                                retry = true;
                            }
                            else if (form.Result == MissingFolderAction.ActionResult.Cancel)
                            {
                                return false;
                            }
                            else if (form.Result == MissingFolderAction.ActionResult.Create)
                            {
                                await Task.Factory.StartNew(() => Directory.CreateDirectory(seasonPath));

                                retry = true;
                            }
                            else if (form.Result == MissingFolderAction.ActionResult.IgnoreOnce)
                            {
                                break;
                            }
                            else if (form.Result == MissingFolderAction.ActionResult.IgnoreAlways)
                            {
                                show.IgnoredSeasons.Add(season.Number);

                                break;
                            }
                            else if (form.Result == MissingFolderAction.ActionResult.Location)
                            {
                                // TODO

                                retry = true;
                            }
                        }
                    } while (retry);
                }
            }

            return true;
        }

        private async void toolStripButtonScan_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel.Text = "Scanning...";
            this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

            this.tabControl.SelectedIndex = 1;

            this.listViewScan.Items.Clear();

            try
            {
                // Iterate all directories, prompting user to create or ignore missing
                if (!await CheckMissingDirectories(Settings.Instance.Shows)) return;

                List<ListViewItem> results = await Scanner.Scan(this.listViewScan, CancellationToken.None);

                this.listViewScan.Items.AddRange(results.ToArray());

                this.listViewScan.Groups["missing"].Header = $"Missing ({"Item".ToQuantity(this.listViewScan.Groups["missing"].Items.Count)})";
                this.listViewScan.Groups["metadata"].Header = $"Metadata ({"Item".ToQuantity(this.listViewScan.Groups["metadata"].Items.Count)})";
                this.listViewScan.Groups["download"].Header = $"Download ({"Item".ToQuantity(this.listViewScan.Groups["download"].Items.Count)})";

                this.listViewScan.Sort();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            this.toolStripProgressBar.Style = ProgressBarStyle.Continuous;
            this.toolStripStatusLabel.Text = "Ready";

            this.buttonScanProcess.Enabled = this.listViewScan.CheckedItems.Count > 0;
        }

        private void treeViewShows_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.view?.Dispose();

            if (e.Node.Level == 0)
            {
                // Show

                int id = int.Parse(e.Node.Name);
                Show show = Settings.Instance.Shows.First(s => s.TVDBId == id);

                ShowView showView = new ShowView(show)
                {
                    Dock = DockStyle.Fill
                };

                showView.RefreshClicked += async (s, a) =>
                {
                    showView.RefreshEnabled = false;
                    this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                    this.toolStripStatusLabel.Text = $"Refreshing show {show.Metadata.Name}...";

                    await TVDB.Instance.Refresh(show.TVDBId, CancellationToken.None);

                    showView.Item = show;

                    this.toolStripStatusLabel.Text = "Ready";
                    this.toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                    showView.RefreshEnabled = true;
                };

                this.view = showView;
            }
            else
            {
                // Season

                int id = int.Parse(e.Node.Parent.Name);
                Show show = Settings.Instance.Shows.First(s => s.TVDBId == id);
                int number = int.Parse(e.Node.Name);
                Season season = show.Metadata.Seasons[number];

                this.view = new SeasonView(season)
                {
                    Dock = DockStyle.Fill
                };
            }

            this.splitContainer.Panel2.Controls.Add(this.view);
        }

        private async void buttonScanProcess_Click(object sender, EventArgs e)
        {
            if (this.buttonScanProcess.Text == "&Process")
            {
                this.buttonScanProcess.Text = "&Cancel";

                this.cts = new CancellationTokenSource();

                try
                {
                    this.toolStripStatusLabel.Text = "Processing...";
                    this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

                    foreach (ListViewItem lvi in this.listViewScan.CheckedItems)
                    {
                        if (!(lvi.Tag is IAction action)) continue;

                        try
                        {
                            await action.Run(this.cts.Token);

                            if (action is FileAction fileAction)
                            {
                                switch (fileAction.Operation)
                                {
                                    case FileAction.FileOperation.Copy:
                                        Stats.Instance.FilesCopied.Increment();

                                        break;
                                    case FileAction.FileOperation.Move:
                                        Stats.Instance.FilesMoved.Increment();

                                        break;
                                    case FileAction.FileOperation.Rename:
                                        Stats.Instance.FilesRenamed.Increment();

                                        break;
                                }
                            }

                            this.listViewScan.Items.Remove(lvi);
                        }
                        catch (TaskCanceledException exception)
                        {
                            Logger.Error(exception);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);

                            lvi.ForeColor = Color.Red;
                            lvi.SubItems[6].Text = ex.Message;
                        }
                    }

                    this.toolStripProgressBar.Style = ProgressBarStyle.Continuous;
                    this.toolStripStatusLabel.Text = "Ready";

                    this.buttonScanProcess.Text = "&Process";
                    this.buttonScanProcess.Enabled = this.listViewScan.CheckedItems.Count > 0;
                }
                catch (TaskCanceledException exception)
                {
                    Logger.Error(exception);
                }
            }
            else
            {
                this.buttonScanProcess.Enabled = false;
                this.toolStripStatusLabel.Text = "Canceling...";

                this.cts.Cancel();
            }
        }

        private void listViewCalendar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewCalendar.SelectedItems.Count == 0)
            {
                this.textBoxSynopsis.Clear();
                return;
            }

            Episode episode = (Episode)this.listViewCalendar.SelectedItems[0].Tag;

            this.textBoxSynopsis.Text = episode.Overview;

            if (!episode.FirstAired.HasValue) return;

            this.calendar.SelectionStart = episode.FirstAired.Value;
            this.calendar.SelectionEnd = episode.FirstAired.Value;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Settings.Instance.Dirty) return;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (MessageBox.Show("Your TV Rename settings have changed, do you want to save before exiting?", "TV Rename", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;

                case DialogResult.Yes:
                    Settings.Save();
                    break;
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Configuration.Layout.Instance.Window.Size = this.WindowState == FormWindowState.Maximized ? this.RestoreBounds.Size : this.Size;
            Configuration.Layout.Instance.Window.Location = this.WindowState == FormWindowState.Maximized ? this.RestoreBounds.Location : this.Location;
            Configuration.Layout.Instance.Window.Maximized = this.WindowState == FormWindowState.Maximized;
            Configuration.Layout.Instance.Window.Splitter = this.splitContainer.SplitterDistance;
            Configuration.Layout.Save();

            Stats.Save();
        }
    }
}
