using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Models.Cache;
using TVRename.Core.Models.Settings;
using TVRename.Core.TVDB;
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

            BuildTree();

            this.Shown += (sender, args) =>
            {
                Logger.Info("Main form started");
            };
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
                        Text = season.Key == 0 ? "Specials" : $"Season {season.Key}",
                        Name = season.Key.ToString()
                    });
                }
            }

            this.treeViewShows.EndUpdate();

            this.toolStripStatusLabelShows.Text = $"{this.treeViewShows.Nodes.Count} shows";
        }

        private async Task Save()
        {
            this.toolStripButtonSave.Enabled = false;
            this.addToolStripMenuItem.Enabled = false;
            this.toolStripStatusLabel.Text = "Saving settings...";

            Settings.Save();

            await TVDB.Save(Path.Combine(ApplicationBase.BasePath, "tvdb.json"));

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
                    this.toolStripStatusLabel.Text = $"Downloading new show...";
                    this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

                    await TVDB.Instance.Add(show.TVDBId, CancellationToken.None);
                    Settings.Instance.Shows.Add(show);

                    this.toolStripProgressBar.Style = ProgressBarStyle.Continuous;
                    this.toolStripStatusLabel.Text = "Ready";

                    BuildTree();
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

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            addToolStripMenuItem_Click(sender, e);
        }

        private async void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            await Save();
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

                    await TVDB.Instance.Refresh(id, CancellationToken.None);

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

                int number = int.Parse(e.Node.Name);

                this.view = new Label
                {
                    Text = $"TODO: Season {number}",
                    Font = new Font("Microsoft Sans Serif,", 20),
                    Dock = DockStyle.Fill
                };
            }

            this.splitContainer.Panel2.Controls.Add(this.view);
        }
    }
}
