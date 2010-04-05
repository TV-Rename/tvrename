// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

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
        private FNPRegexList Rex;
        private ShowItemList SIL;

        private TVSettings TheSettings;

        public AddEditSeasEpFinders(FNPRegexList rex, ShowItemList sil, ShowItem initialShow, string initialFolder, TVSettings s)
        {
            this.Rex = rex;
            this.SIL = sil;
            this.TheSettings = s;

            this.InitializeComponent();

            this.SetupGrid();
            this.FillGrid(this.Rex);

            foreach (ShowItem si in this.SIL)
            {
                this.cbShowList.Items.Add(si.ShowName());
                if (si == initialShow)
                    this.cbShowList.SelectedIndex = this.cbShowList.Items.Count - 1;
            }
            this.txtFolder.Text = initialFolder;
        }

        public void SetupGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell();
            titleModel.BackColor = Color.SteelBlue;
            titleModel.ForeColor = Color.White;
            titleModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

            SourceGrid.Cells.Views.Cell titleModelC = new SourceGrid.Cells.Views.Cell();
            titleModelC.BackColor = Color.SteelBlue;
            titleModelC.ForeColor = Color.White;
            titleModelC.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

            this.Grid1.Columns.Clear();
            this.Grid1.Rows.Clear();

            this.Grid1.RowsCount = 1;
            this.Grid1.ColumnsCount = 4;
            this.Grid1.FixedRows = 1;
            this.Grid1.FixedColumns = 0;
            this.Grid1.Selection.EnableMultiSelection = false;

            this.Grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            this.Grid1.Columns[0].Width = 60;
            this.Grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            this.Grid1.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            this.Grid1.Columns[2].Width = 60;
            this.Grid1.Columns[3].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            this.Grid1.AutoStretchColumnsToFitWidth = true;
            this.Grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("Enabled");
            h.AutomaticSortEnabled = false;
            this.Grid1[0, 0] = h;
            this.Grid1[0, 0].View = titleModelC;

            h = new SourceGrid.Cells.ColumnHeader("Regex");
            h.AutomaticSortEnabled = false;
            this.Grid1[0, 1] = h;
            this.Grid1[0, 1].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("Full Path");
            h.AutomaticSortEnabled = false;
            this.Grid1[0, 2] = h;
            this.Grid1[0, 2].View = titleModelC;

            h = new SourceGrid.Cells.ColumnHeader("Notes");
            h.AutomaticSortEnabled = false;
            this.Grid1[0, 3] = h;
            this.Grid1[0, 3].View = titleModel;

            this.Grid1.Selection.SelectionChanged += this.SelectionChanged;
        }

        public void SelectionChanged(Object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            this.StartTimer();
        }

        public void AddNewRow()
        {
            int r = this.Grid1.RowsCount;
            this.Grid1.RowsCount = r + 1;

            this.Grid1[r, 0] = new SourceGrid.Cells.CheckBox(null, true);
            this.Grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
            this.Grid1[r, 2] = new SourceGrid.Cells.CheckBox(null, false);
            this.Grid1[r, 3] = new SourceGrid.Cells.Cell("", typeof(string));

            ChangedCont changed = new ChangedCont(this);
            for (int c = 0; c < 4; c++)
                this.Grid1[r, c].AddController(changed);
        }

        public void FillGrid(FNPRegexList list)
        {
            while (this.Grid1.Rows.Count > 1) // leave header row
                this.Grid1.Rows.Remove(1);

            this.Grid1.RowsCount = list.Count + 1;

            int i = 1;
            foreach (FilenameProcessorRE re in list)
            {
                this.Grid1[i, 0] = new SourceGrid.Cells.CheckBox(null, re.Enabled);
                this.Grid1[i, 1] = new SourceGrid.Cells.Cell(re.RE, typeof(string));
                this.Grid1[i, 2] = new SourceGrid.Cells.CheckBox(null, re.UseFullPath);
                this.Grid1[i, 3] = new SourceGrid.Cells.Cell(re.Notes, typeof(string));

                ChangedCont changed = new ChangedCont(this);

                for (int c = 0; c < 4; c++)
                    this.Grid1[i, c].AddController(changed);

                i++;
            }
            this.StartTimer();
        }

        private FilenameProcessorRE REForRow(int i)
        {
            if ((i < 1) || (i >= this.Grid1.RowsCount)) // row 0 is header
                return null;
            bool en = (bool) (this.Grid1[i, 0].Value);
            string regex = (string) (this.Grid1[i, 1].Value);
            bool fullPath = (bool) (this.Grid1[i, 2].Value);
            string notes = (string) (this.Grid1[i, 3].Value);
            if (notes == null)
                notes = "";

            if (string.IsNullOrEmpty(regex))
                return null;
            return new FilenameProcessorRE(en, regex, fullPath, notes);
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            this.Rex.Clear();
            for (int i = 1; i < this.Grid1.RowsCount; i++) // skip header row
            {
                FilenameProcessorRE re = this.REForRow(i);
                if (re != null)
                    this.Rex.Add(re);
            }
        }

        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            this.AddNewRow();
            this.Grid1.Selection.Focus(new SourceGrid.Position(this.Grid1.RowsCount - 1, 1), true);
            this.StartTimer();
        }

        private void bnDelete_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = this.Grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                this.Grid1.Rows.Remove(rowsIndex[0]);

            this.StartTimer();
        }

        private void bnBrowse_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtFolder.Text))
                this.folderBrowser.SelectedPath = this.txtFolder.Text;

            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtFolder.Text = this.folderBrowser.SelectedPath;

            this.StartTimer();
        }

        private void cbShowList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.StartTimer();
        }

        private void txtFolder_TextChanged(object sender, System.EventArgs e)
        {
            this.StartTimer();
        }

        private void StartTimer()
        {
            this.lvPreview.Enabled = false;
            this.tmrFillPreview.Start();
        }

        private void tmrFillPreview_Tick(object sender, System.EventArgs e)
        {
            this.tmrFillPreview.Stop();
            this.FillPreview();
        }

        private void chkTestAll_CheckedChanged(object sender, System.EventArgs e)
        {
            this.StartTimer();
        }

        private void FillPreview()
        {
            this.lvPreview.Items.Clear();
            if ((string.IsNullOrEmpty(this.txtFolder.Text)) || (!Directory.Exists(this.txtFolder.Text)))
            {
                this.txtFolder.BackColor = Helpers.WarningColor();
                return;
            }
            else
                this.txtFolder.BackColor = System.Drawing.SystemColors.Window;

            if (this.Grid1.RowsCount <= 1) // 1 for header
                return; // empty

            this.lvPreview.Enabled = true;

            FNPRegexList rel = new FNPRegexList();

            if (this.chkTestAll.Checked)
            {
                for (int i = 1; i < this.Grid1.RowsCount; i++)
                {
                    FilenameProcessorRE re = this.REForRow(i);
                    if (re != null)
                        rel.Add(re);
                }
            }
            else
            {
                int[] rowsIndex = this.Grid1.Selection.GetSelectionRegion().GetRowsIndex();
                if (rowsIndex.Length == 0)
                    return;

                FilenameProcessorRE re2 = this.REForRow(rowsIndex[0]);
                if (re2 != null)
                    rel.Add(re2);
                else
                    return;
            }

            this.lvPreview.BeginUpdate();
            DirectoryInfo d = new DirectoryInfo(this.txtFolder.Text);
            foreach (FileInfo fi in d.GetFiles())
            {
                int seas;
                int ep;

                if (!this.TheSettings.UsefulExtension(fi.Extension, true))
                    continue; // move on

                bool r = TVDoc.FindSeasEp(fi, out seas, out ep, this.cbShowList.Text, rel);
                ListViewItem lvi = new ListViewItem();
                lvi.Text = fi.Name;
                lvi.SubItems.Add((seas == -1) ? "-" : seas.ToString());
                lvi.SubItems.Add((ep == -1) ? "-" : ep.ToString());
                if (!r)
                    lvi.BackColor = Helpers.WarningColor();
                this.lvPreview.Items.Add(lvi);
            }
            this.lvPreview.EndUpdate();
        }

        private void bnDefaults_Click(object sender, System.EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Restore to default matching expressions?", "Filename Processors", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
                this.FillGrid(TVSettings.DefaultFNPList());
        }

        #region Nested type: ChangedCont

        public class ChangedCont : SourceGrid.Cells.Controllers.ControllerBase
        {
            private AddEditSeasEpFinders P;

            public ChangedCont(AddEditSeasEpFinders p)
            {
                this.P = p;
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                this.P.StartTimer();
            }
        }

        #endregion
    }
}