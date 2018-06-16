// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using SourceGrid;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    /// <summary>
    /// Summary for AddEditSeasEpFinders
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class AddEditSeasEpFinders : Form
    {
        private readonly List<ShowItem> shows;
        public List<FilenameProcessorRE> OutputRegularExpressions { get; }

        public AddEditSeasEpFinders(List<FilenameProcessorRE> rex, List<ShowItem> sil, ShowItem initialShow,
            string initialFolder)
        {
            OutputRegularExpressions = rex;
            shows = sil;

            InitializeComponent();

            SetupGrid();
            FillGrid(OutputRegularExpressions);

            foreach (ShowItem si in shows)
            {
                cbShowList.Items.Add(si.ShowName);
                if (si == initialShow)
                    cbShowList.SelectedIndex = cbShowList.Items.Count - 1;
            }

            txtFolder.Text = initialFolder;
        }

        private void SetupGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
            {
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
            };

            SourceGrid.Cells.Views.Cell titleModelC = new SourceGrid.Cells.Views.Cell
            {
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment =
                    DevAge.Drawing.ContentAlignment.MiddleCenter
            };

            Grid1.Columns.Clear();
            Grid1.Rows.Clear();

            Grid1.RowsCount = 1;
            Grid1.ColumnsCount = 4;
            Grid1.FixedRows = 1;
            Grid1.FixedColumns = 0;
            Grid1.Selection.EnableMultiSelection = false;

            Grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            Grid1.Columns[0].Width = 60;
            Grid1.Columns[1].AutoSizeMode =
                SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            Grid1.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            Grid1.Columns[2].Width = 60;
            Grid1.Columns[3].AutoSizeMode =
                SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            Grid1.AutoStretchColumnsToFitWidth = true;
            Grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("Enabled") {AutomaticSortEnabled = false};
            Grid1[0, 0] = h;
            Grid1[0, 0].View = titleModelC;

            h = new SourceGrid.Cells.ColumnHeader("Regex") {AutomaticSortEnabled = false};
            Grid1[0, 1] = h;
            Grid1[0, 1].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("Full Path") {AutomaticSortEnabled = false};
            Grid1[0, 2] = h;
            Grid1[0, 2].View = titleModelC;

            h = new SourceGrid.Cells.ColumnHeader("Notes") {AutomaticSortEnabled = false};
            Grid1[0, 3] = h;
            Grid1[0, 3].View = titleModel;

            Grid1.Selection.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, RangeRegionChangedEventArgs e)
        {
            SelectionOnSelectionChanged(sender, e);
            StartTimer();
        }

        private void AddNewRow()
        {
            int r = Grid1.RowsCount;
            Grid1.RowsCount = r + 1;

            Grid1[r, 0] = new SourceGrid.Cells.CheckBox(null, true);
            Grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
            Grid1[r, 2] = new SourceGrid.Cells.CheckBox(null, false);
            Grid1[r, 3] = new SourceGrid.Cells.Cell("", typeof(string));

            ChangedCont changed = new ChangedCont(this);
            for (int c = 0; c < 4; c++)
                Grid1[r, c].AddController(changed);
        }

        private void FillGrid(List<FilenameProcessorRE> list)
        {
            while (Grid1.Rows.Count > 1) // leave header row
                Grid1.Rows.Remove(1);

            Grid1.RowsCount = list.Count + 1;

            int i = 1;
            foreach (FilenameProcessorRE re in list)
            {
                Grid1[i, 0] = new SourceGrid.Cells.CheckBox(null, re.Enabled);
                Grid1[i, 1] = new SourceGrid.Cells.Cell(re.RegExpression, typeof(string));
                Grid1[i, 2] = new SourceGrid.Cells.CheckBox(null, re.UseFullPath);
                Grid1[i, 3] = new SourceGrid.Cells.Cell(re.Notes, typeof(string));

                ChangedCont changed = new ChangedCont(this);

                for (int c = 0; c < 4; c++)
                    Grid1[i, c].AddController(changed);

                i++;
            }

            StartTimer();
        }

        private FilenameProcessorRE RegExForRow(int i)
        {
            if ((i < 1) || (i >= Grid1.RowsCount)) // row 0 is header
                return null;

            bool en = (bool) (Grid1[i, 0].Value);
            string regex = (string) (Grid1[i, 1].Value);
            bool fullPath = (bool) (Grid1[i, 2].Value);
            string notes = (string) (Grid1[i, 3].Value) ?? "";

            return string.IsNullOrEmpty(regex) ? null : new FilenameProcessorRE(en, regex, fullPath, notes);
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            OutputRegularExpressions.Clear();
            for (int i = 1; i < Grid1.RowsCount; i++) // skip header row
            {
                FilenameProcessorRE re = RegExForRow(i);
                if (re != null)
                    OutputRegularExpressions.Add(re);
            }
        }

        private void bnAdd_Click(object sender, EventArgs e)
        {
            AddNewRow();
            Grid1.Selection.Focus(new Position(Grid1.RowsCount - 1, 1), true);
            StartTimer();
        }

        private void bnDelete_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                Grid1.Rows.Remove(rowsIndex[0]);

            StartTimer();
        }

        private void bnBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFolder.Text))
                folderBrowser.SelectedPath = txtFolder.Text;

            if (folderBrowser.ShowDialog() == DialogResult.OK)
                txtFolder.Text = folderBrowser.SelectedPath;

            StartTimer();
        }

        private void cbShowList_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartTimer();
        }

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            StartTimer();
        }

        private void StartTimer()
        {
            lvPreview.Enabled = false;
            tmrFillPreview.Start();
        }

        private void tmrFillPreview_Tick(object sender, EventArgs e)
        {
            tmrFillPreview.Stop();
            FillPreview();
        }

        private void chkTestAll_CheckedChanged(object sender, EventArgs e)
        {
            StartTimer();
        }

        private void FillPreview()
        {
            lvPreview.Items.Clear();
            if ((string.IsNullOrEmpty(txtFolder.Text)) || (!Directory.Exists(txtFolder.Text)))
            {
                txtFolder.BackColor = Helpers.WarningColor();
                return;
            }
            else
                txtFolder.BackColor = SystemColors.Window;

            if (Grid1.RowsCount <= 1) // 1 for header
                return; // empty

            lvPreview.Enabled = true;

            List<FilenameProcessorRE> rel = new List<FilenameProcessorRE>();

            if (chkTestAll.Checked)
            {
                for (int i = 1; i < Grid1.RowsCount; i++)
                {
                    FilenameProcessorRE re = RegExForRow(i);
                    if (re != null)
                        rel.Add(re);
                }
            }
            else
            {
                int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();
                if (rowsIndex.Length == 0)
                    return;

                FilenameProcessorRE re2 = RegExForRow(rowsIndex[0]);
                if (re2 != null)
                    rel.Add(re2);
                else
                    return;
            }

            UpdatePreview(rel);
        }

        private void UpdatePreview(List<FilenameProcessorRE> rel)
        {
            lvPreview.BeginUpdate();
            DirectoryInfo d = new DirectoryInfo(txtFolder.Text);
            foreach (FileInfo fi in d.GetFiles())
            {
                if (!TVSettings.Instance.UsefulExtension(fi.Extension, true))
                    continue; // move on

                ShowItem si = cbShowList.SelectedIndex >= 0 ? shows[cbShowList.SelectedIndex] : null;
                bool r = TVDoc.FindSeasEp(fi, out int seas, out int ep, out int maxEp, si, rel, false,
                    out FilenameProcessorRE matchRex);

                ListViewItem lvi = new ListViewItem { Text = fi.Name };
                lvi.SubItems.Add((seas == -1) ? "-" : seas.ToString());
                lvi.SubItems.Add((ep == -1) ? "-" : ep + ((maxEp != -1) ? "-" + maxEp : ""));
                lvi.SubItems.Add((matchRex == null) ? "-" : matchRex.Notes);
                if (!r)
                    lvi.BackColor = Helpers.WarningColor();

                lvPreview.Items.Add(lvi);
            }

            lvPreview.EndUpdate();
        }

        private void bnDefaults_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Restore to default matching expressions?", "Filename Processors",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
                FillGrid(TVSettings.DefaultFNPList());
        }

        #region Nested type: ChangedCont

        private class ChangedCont : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly AddEditSeasEpFinders p;

            public ChangedCont(AddEditSeasEpFinders p)
            {
                this.p = p;
            }

            public override void OnValueChanged(CellContext sender, EventArgs e)
            {
                p.StartTimer();
            }
        }

        #endregion

        private void bnUp_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();

            if (rowsIndex.Length == 0) return;

            int recordToMoveUp = rowsIndex[0];

            if (recordToMoveUp < 2) return;

            Grid1.Rows.Swap(recordToMoveUp, recordToMoveUp - 1);
            Grid1.Selection.Focus(new Position(recordToMoveUp - 1, 1), true);
            StartTimer();
        }

        private void bnDown_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();

            if (rowsIndex.Length == 0) return;

            int recordToMoveDown = rowsIndex[0];

            if (recordToMoveDown > Grid1.RowsCount - 2) return;

            Grid1.Rows.Swap(recordToMoveDown, recordToMoveDown + 1);
            Grid1.Selection.Focus(new Position(recordToMoveDown + 1, 1), true);
            StartTimer();
        }

        private void SelectionOnSelectionChanged(object sender, RangeRegionChangedEventArgs rangeRegionChangedEventArgs)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();

            if (rowsIndex.Length == 0)
            {
                bnDelete.Enabled = false;
                bnUp.Enabled = false;
                bnDown.Enabled = false;
                return;
            }

            int selectedRow = rowsIndex[0];

            bnDelete.Enabled = true;

            bnUp.Enabled = (selectedRow > 1);
            bnDown.Enabled = (selectedRow < Grid1.RowsCount - 1);
        }
    }
}
