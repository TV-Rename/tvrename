using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TVRename.Core.Models;
using TVRename.Core.Utility;
using TVRename.Windows.Configuration;
using TVRename.Windows.Models;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.Windows.Forms
{
    public partial class FilenameProcessors : Form
    {
        private readonly CellController controller;

        public FilenameProcessors()
        {
            InitializeComponent();

            this.controller = new CellController(this);

            this.grid.Selection.EnableMultiSelection = false;

            this.grid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            this.grid.Columns[0].Width = 70;
            this.grid.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            this.grid.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            this.grid.Columns[2].Width = 70;
            this.grid.Columns[3].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            this.grid.Columns.StretchToFit();

            this.grid[0, 0] = new SourceGrid.Cells.ColumnHeader("Enabled");
            this.grid[0, 1] = new SourceGrid.Cells.ColumnHeader("Pattern");
            this.grid[0, 2] = new SourceGrid.Cells.ColumnHeader("Full Path");
            this.grid[0, 3] = new SourceGrid.Cells.ColumnHeader("Notes");

            this.grid.Selection.SelectionChanged += grid_SelectionChanged;
        }

        private void FilenameProcessors_Load(object sender, EventArgs e)
        {
            FillGrid(Settings.Instance.FilenameProcessors);

            this.comboBoxShows.Items.AddRange(Settings.Instance.Shows.Select(s => s.ToString()).ToArray());
            this.comboBoxShows.SelectedIndex = 0;

            Preview();
        }

        private void grid_SelectionChanged(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            Preview();
        }

        private void Preview()
        {
            this.listViewPreview.Items.Clear();

            if (this.grid.RowsCount <= 1) return;

            this.listViewPreview.Enabled = true;

            int[] rowsIndex = this.grid.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length == 0) return;

            FilenameProcessor selected = this.SelectedRow(rowsIndex[0]);
            if (selected == null) return;

            List<FilenameProcessor> processors = new List<FilenameProcessor>
            {
                selected
            };

            Core.Models.Show show = Settings.Instance.Shows[this.comboBoxShows.SelectedIndex];

            this.listViewPreview.BeginUpdate();

            // TODO: Async
            foreach (FileInfo file in Helpers.GetFiles(show.Location, Settings.Instance.VideoFileExtensions.Select(e => $"*.{e}").ToArray(), SearchOption.AllDirectories).Select(f => new FileInfo(f)))
            {
                bool match = Scanner.MatchFile(file.DirectoryName, file.Name, out int matchedSeasonNumber, out int matchedEpisodeNumber, null, processors);

                this.listViewPreview.Items.Add(new ListViewItem(new[]
                {
                    file.Name,
                    matchedSeasonNumber == -1 ? "-" : matchedSeasonNumber.ToString(),
                    matchedEpisodeNumber == -1 ? "-" : matchedEpisodeNumber.ToString()
                })
                {
                    BackColor = !match ? Color.FromArgb(255, 210, 210) : Color.FromArgb(165, 245, 200)
                });
            }

            this.listViewPreview.EndUpdate();
        }

        private FilenameProcessor SelectedRow(int i)
        {
            if (i < 1 || i >= this.grid.RowsCount) return null;

            return new FilenameProcessor
            {
                Enabled = (bool)this.grid[i, 0].Value,
                Pattern = (string)this.grid[i, 1].Value ?? string.Empty,
                UseFullPath = (bool)this.grid[i, 2].Value,
                Notes = (string)this.grid[i, 3].Value ?? string.Empty
            };
        }

        public void FillGrid(List<FilenameProcessor> list)
        {
            while (this.grid.Rows.Count > 1) this.grid.Rows.Remove(1);

            this.grid.RowsCount = list.Count + 1;

            int i = 1;
            foreach (FilenameProcessor re in list)
            {
                this.grid[i, 0] = new SourceGrid.Cells.CheckBox(null, re.Enabled);
                this.grid[i, 1] = new SourceGrid.Cells.Cell(re.Pattern, typeof(string));
                this.grid[i, 2] = new SourceGrid.Cells.CheckBox(null, re.UseFullPath);
                this.grid[i, 3] = new SourceGrid.Cells.Cell(re.Notes, typeof(string));

                for (int c = 0; c < 4; c++) this.grid[i, c].AddController(this.controller);

                i++;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int r = this.grid.RowsCount;
            this.grid.RowsCount = r + 1;

            this.grid[r, 0] = new SourceGrid.Cells.CheckBox(null, true);
            this.grid[r, 1] = new SourceGrid.Cells.Cell(string.Empty, typeof(string));
            this.grid[r, 2] = new SourceGrid.Cells.CheckBox(null, false);
            this.grid[r, 3] = new SourceGrid.Cells.Cell(string.Empty, typeof(string));

            for (int c = 0; c < 4; c++) this.grid[r, c].AddController(this.controller);

            this.grid.Selection.Focus(new SourceGrid.Position(this.grid.RowsCount - 1, 1), true);

            Preview();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int[] rowsIndex = this.grid.Selection.GetSelectionRegion().GetRowsIndex();

            if (rowsIndex.Length > 0) this.grid.Rows.Remove(rowsIndex[0]);

            Preview();
        }

        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            // TODO
        }

        private void comboBoxShows_SelectedIndexChanged(object sender, EventArgs e)
        {
            Preview();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Settings.Instance.FilenameProcessors.Clear();
            Settings.Instance.FilenameProcessors.AddRange(this.grid.Rows.Skip(1).Select(r => SelectedRow(r.Index)));

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public class CellController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly FilenameProcessors form;

            public CellController(FilenameProcessors form)
            {
                this.form = form;
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                this.form.Preview();
            }
        }
    }
}
