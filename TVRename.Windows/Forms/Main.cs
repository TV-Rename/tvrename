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
using TVRename.Core.Models;
using TVRename.Core.Models.Cache;
using TVRename.Core.TVDB;
using TVRename.Core.Utility;
using TVRename.Windows.Configuration;
using TVRename.Windows.Controls;
using TVRename.Windows.Utilities;
using Show = TVRename.Core.Models.Show;

namespace TVRename.Windows.Forms
{
    public partial class Main : Form
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Control view;

        public Main()
        {
            InitializeComponent();

            // Double buffer to reduce flicker
            Helpers.DoubleBuffer(this.listViewScan);
            Helpers.DoubleBuffer(this.listViewCalendar);

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

            BuildTree();
            BuildCalendar();

            this.Shown += (s, a) => Logger.Info("Main form started");
        }

        private void BuildTree()
        {
            this.treeViewShows.BeginUpdate();

            this.treeViewShows.Nodes.Clear();

            foreach (Show show in Settings.Instance.Shows.OrderBy(s => s.Metadata.Name))
            {
                TreeNode node = this.treeViewShows.Nodes.Add(show.TVDBId.ToString(), show.Metadata.Name);

                foreach (KeyValuePair<int, Season> season in show.Metadata.Seasons.OrderBy(s => s.Value.Number))
                {
                    node.Nodes.Add(new TreeNode
                    {
                        Text = season.Key == 0 ? Settings.Instance.SpecialsTemplate.Template(season.Value) : Settings.Instance.SeasonTemplate.Template(season.Value),
                        Name = season.Key.ToString()
                    });
                }
            }

            this.treeViewShows.EndUpdate();

            this.toolStripStatusLabelShows.Text = "show".ToQuantity(this.treeViewShows.Nodes.Count);
        }

        private void BuildCalendar()
        {
            List<DateTime> dates = new List<DateTime>();

            this.listViewCalendar.BeginUpdate();
            this.listViewCalendar.Groups["recent"].Header = $"Aired in the last {"day".ToQuantity(Settings.Instance.RecentDays)}";
            this.listViewCalendar.Items.Clear();

            foreach (Show show in Settings.Instance.Shows)
            {
                //if (!show.NextAirs.HasValue) continue;

                foreach (Season season in show.Metadata.Seasons.Values)
                {
                    if (show.IgnoredSeasons.Contains(season.Number)) continue;

                    foreach (Episode episode in season.Episodes.Values)
                    {
                        if (!episode.FirstAired.HasValue || episode.FirstAired?.CompareTo(DateTime.MaxValue) == 0) continue;

                        TimeSpan delta = episode.FirstAired.Value.Subtract(DateTime.UtcNow);

                        if (delta < -TimeSpan.FromDays(Settings.Instance.RecentDays)) continue;

                        ListViewItem lvi = new ListViewItem
                        {
                            Text = show.Metadata.Name,
                            SubItems =
                            {
                                season.Number.ToString(),
                                episode.Number.ToString(),
                                episode.FirstAired.Value.ToShortDateString(),
                                episode.FirstAired.Value.ToString("t"),
                                episode.FirstAired.Value.ToString("ddd"),
                                episode.FirstAired.Value < DateTime.UtcNow ? "Aired" : $"{delta.Days}d {delta.Hours}h",
                                episode.Name
                            },
                            Tag = episode
                        };

                        if (delta.TotalHours < 0)
                            lvi.Group = this.listViewCalendar.Groups["recent"];
                        else if (delta < TimeSpan.FromDays(Settings.Instance.RecentDays))
                            lvi.Group = this.listViewCalendar.Groups["soon"];
                        //else if (episode.NextToAir)
                        //    lvi.Group = this.listViewCalendar.Groups["future"];
                        else
                            lvi.Group = this.listViewCalendar.Groups["later"];

                        if (episode.FirstAired?.CompareTo(DateTime.Now) < 0) // has aired
                        {
                            List<FileInfo> files = new List<FileInfo>();

                            //this.FindEpOnDisk(episode);

                            if (files.Count > 0)
                            {
                                lvi.ImageIndex = 1;
                            }
                            else if (show.CheckMissing)
                            {
                                lvi.ImageIndex = 0;
                            }
                        }

                        this.listViewCalendar.Items.Add(lvi);

                        dates.Add(episode.FirstAired.Value);
                    }
                }
            }

            this.listViewCalendar.Sort();
            this.listViewCalendar.EndUpdate();

            this.calendar.BoldedDates = dates.ToArray();
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
            this.Text += $" {Helpers.DisplayVersion}";
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

                    BuildTree();
                    BuildCalendar();

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
            this.Close();
        }

        private void testErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new Exception("Test error");
        }

        private void mediaCenterFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MediaCenter().ShowDialog();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new Preferences().ShowDialog() != DialogResult.OK) return;

            BuildTree();
            BuildCalendar();
        }

        private async void refreshAllToolStripMenuItem_Click(object sender, EventArgs e)
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
            MessageBox.Show("TODO"); // TODO
        }

        private void toolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TODO"); // TODO
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

            BuildTree();
            BuildCalendar();
        }

        private async Task<List<ListViewItem>> Scan(CancellationToken ct)
        {
            // Directory|File.Exists has no async version so manually run in a new thread
            return await Task.Factory.StartNew(() =>
            {
                Dictionary<char, string> replacementChars = new Dictionary<char, string>
                {
                    {':', "-"},
                    {'"', "'"},
                    {'*', "#"},
                    {'?', ""},
                    {'>', ""},
                    {'<', ""},
                    {'/', "-"},
                    {'\\', "-"},
                    {'|', "-"},
                };

                List<ListViewItem> results = new List<ListViewItem>();

                foreach (Show show in Settings.Instance.Shows)
                {
                    ProcessedShow processedShow = new ProcessedShow(show);

                    if (!Directory.Exists(processedShow.Location))
                    {
                        // TODO: Missing show dir
                        continue;
                    }

                    foreach (IAction action in Settings.Instance.Identifiers.Select(i => i.ProcessShow(processedShow)).Where(s => s != null))
                    {
                        results.Add(new ListViewItem(new[]
                        {
                            processedShow.Name,
                            string.Empty,
                            string.Empty,
                            processedShow.LastUpdated.ToShortDateString(),
                            Path.GetDirectoryName(action.Produces),
                            Path.GetFileName(action.Produces),
                            string.Empty
                        })
                        {
                            Tag = action,
                            Checked = true,
                            Group = this.listViewScan.Groups[action.Type.ToLowerInvariant()]
                        });
                    }

                    foreach (Season season in show.Metadata.Seasons.Values)
                    {
                        ProcessedSeason processedSeason = new ProcessedSeason(season);

                        string seasonDir = (processedSeason.Number == 0 ? Settings.Instance.SpecialsTemplate : Settings.Instance.SeasonTemplate).Template(season);

                        foreach (KeyValuePair<char, string> singleChar in replacementChars)
                        {
                            seasonDir = seasonDir.Replace(singleChar.Key.ToString(), singleChar.Value);
                        }

                        processedSeason.Location = Path.Combine(processedShow.Location, seasonDir);

                        if (!Directory.Exists(processedSeason.Location))
                        {
                            // TODO: Missing season dir

                            Directory.CreateDirectory(processedSeason.Location);
                        }

                        foreach (IAction action in Settings.Instance.Identifiers.Select(i => i.ProcessSeason(processedShow, processedSeason)).Where(s => s != null))
                        {
                            results.Add(new ListViewItem(new[]
                            {
                                processedShow.Name,
                                processedSeason.Number == 0 ? Settings.Instance.SpecialsTemplate.Template(season) : processedSeason.Number.ToString(),
                                string.Empty,
                                processedShow.LastUpdated.ToShortDateString(),
                                Path.GetDirectoryName(action.Produces),
                                Path.GetFileName(action.Produces),
                                string.Empty
                            })
                            {
                                Tag = action,
                                Checked = true,
                                Group = this.listViewScan.Groups[action.Type.ToLowerInvariant()]
                            });
                        }

                        foreach (Episode episode in season.Episodes.Values)
                        {
                            ProcessedEpisode processedEpisode = new ProcessedEpisode(episode);

                            string episodeFile = Settings.Instance.EpisodeTemplate.Template(new Dictionary<string, object>
                            {
                                { "show", processedShow },
                                { "season", processedSeason },
                                { "episode", processedEpisode }
                            });

                            foreach (KeyValuePair<char, string> singleChar in replacementChars)
                            {
                                episodeFile = episodeFile.Replace(singleChar.Key.ToString(), singleChar.Value);
                            }

                            string episodePath = Path.Combine(processedSeason.Location, episodeFile);

                            // TODO: Video extentions

                            if (!File.Exists(episodePath))
                            {
                                results.Add(new ListViewItem(new[]
                                {
                                    processedShow.Name,
                                    processedSeason.Number == 0 ? Settings.Instance.SpecialsTemplate.Template(season) : processedSeason.Number.ToString(),
                                    processedEpisode.Number.ToString(),
                                    processedEpisode.FirstAired?.ToShortDateString(),
                                    Path.GetDirectoryName(episodePath),
                                    Path.GetFileName(episodePath),
                                    string.Empty
                                })
                                {
                                    Tag = "MISSING", // TODO
                                    Checked = false,
                                    Group = this.listViewScan.Groups["missing"]
                                });
                            }

                            processedEpisode.Location = processedSeason.Location;
                            processedEpisode.Filename = episodeFile;

                            foreach (IAction action in Settings.Instance.Identifiers.Select(i => i.ProcessEpisode(processedShow, processedSeason, processedEpisode)).Where(s => s != null))
                            {
                                results.Add(new ListViewItem(new[]
                                {
                                    processedShow.Name,
                                    processedSeason.Number == 0 ? Settings.Instance.SpecialsTemplate.Template(season) : processedSeason.Number.ToString(),
                                    processedEpisode.Number.ToString(),
                                    processedEpisode.LastUpdated.ToShortDateString(),
                                    Path.GetDirectoryName(action.Produces),
                                    Path.GetFileName(action.Produces),
                                    string.Empty
                                })
                                {
                                    Tag = action,
                                    Checked = true,
                                    Group = this.listViewScan.Groups[action.Type.ToLowerInvariant()]
                                });
                            }
                        }
                    }
                }

                return results;
            }, ct);
        }

        private async void toolStripButtonScan_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel.Text = "Scanning...";
            this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

            this.tabControl.SelectedIndex = 1;

            this.listViewScan.Items.Clear();

            try
            {
                List<ListViewItem> results = await Scan(CancellationToken.None);

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


        CancellationTokenSource cts = new CancellationTokenSource();

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

            if (episode.FirstAired.HasValue)
            {
                this.calendar.SelectionStart = episode.FirstAired.Value;
                this.calendar.SelectionEnd = episode.FirstAired.Value;
            }
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

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Settings.Instance.Dirty) return;

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
        }
    }
}
