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
            Rex = rex;
            SIL = sil;
            TheSettings = s;

            InitializeComponent();

            SetupGrid();
            FillGrid(Rex);

            foreach (ShowItem si in SIL)
            {
                cbShowList.Items.Add(si.ShowName());
                if (si == initialShow)
                    cbShowList.SelectedIndex = cbShowList.Items.Count - 1;
            }
            txtFolder.Text = initialFolder;
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

            Grid1.Columns.Clear();
            Grid1.Rows.Clear();

            Grid1.RowsCount = 1;
            Grid1.ColumnsCount = 4;
            Grid1.FixedRows = 1;
            Grid1.FixedColumns = 0;
            Grid1.Selection.EnableMultiSelection = false;

            Grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            Grid1.Columns[0].Width = 60;
            Grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            Grid1.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            Grid1.Columns[2].Width = 60;
            Grid1.Columns[3].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            Grid1.AutoStretchColumnsToFitWidth = true;
            Grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("Enabled");
            h.AutomaticSortEnabled = false;
            Grid1[0, 0] = h;
            Grid1[0, 0].View = titleModelC;


            h = new SourceGrid.Cells.ColumnHeader("Regex");
            h.AutomaticSortEnabled = false;
            Grid1[0, 1] = h;
            Grid1[0, 1].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("Full Path");
            h.AutomaticSortEnabled = false;
            Grid1[0, 2] = h;
            Grid1[0, 2].View = titleModelC;

            h = new SourceGrid.Cells.ColumnHeader("Notes");
            h.AutomaticSortEnabled = false;
            Grid1[0, 3] = h;
            Grid1[0, 3].View = titleModel;

            Grid1.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(SelectionChanged);
        }

        public void SelectionChanged(Object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            StartTimer();
        }

        public void AddNewRow()
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

        public class ChangedCont : SourceGrid.Cells.Controllers.ControllerBase
        {
            private AddEditSeasEpFinders P;

            public ChangedCont(AddEditSeasEpFinders p)
            {
                P = p;
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                P.StartTimer();
            }
        }


        public void FillGrid(FNPRegexList list)
        {
            while (Grid1.Rows.Count > 1) // leave header row
                Grid1.Rows.Remove(1);

            Grid1.RowsCount = list.Count + 1;

            int i = 1;
            foreach (FilenameProcessorRE re in list)
            {
                Grid1[i, 0] = new SourceGrid.Cells.CheckBox(null, re.Enabled);
                Grid1[i, 1] = new SourceGrid.Cells.Cell(re.RE, typeof(string));
                Grid1[i, 2] = new SourceGrid.Cells.CheckBox(null, re.UseFullPath);
                Grid1[i, 3] = new SourceGrid.Cells.Cell(re.Notes, typeof(string));

                ChangedCont changed = new ChangedCont(this);

                for (int c = 0; c < 4; c++)
                    Grid1[i, c].AddController(changed);

                i++;
            }
            StartTimer();
        }

        private FilenameProcessorRE REForRow(int i)
        {
            if ((i < 1) || (i >= Grid1.RowsCount)) // row 0 is header
                return null;
            bool en = (bool)(Grid1[i, 0].Value);
            string regex = (string)(Grid1[i, 1].Value);
            bool fullPath = (bool)(Grid1[i, 2].Value);
            string notes = (string)(Grid1[i, 3].Value);
            if (notes == null)
                notes = "";

            if (string.IsNullOrEmpty(regex))
                return null;
            return new FilenameProcessorRE(en, regex, fullPath, notes);
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            Rex.Clear();
            for (int i = 1; i < Grid1.RowsCount; i++) // skip header row
            {
                FilenameProcessorRE re = REForRow(i);
                if (re != null)
                    Rex.Add(re);
            }
        }
        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            AddNewRow();
            Grid1.Selection.Focus(new SourceGrid.Position(Grid1.RowsCount - 1, 1), true);
            StartTimer();
        }
        private void bnDelete_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                Grid1.Rows.Remove(rowsIndex[0]);

            StartTimer();
        }
        private void bnBrowse_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFolder.Text))
                folderBrowser.SelectedPath = txtFolder.Text;

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtFolder.Text = folderBrowser.SelectedPath;

            StartTimer();
        }
        private void cbShowList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            StartTimer();
        }
        private void txtFolder_TextChanged(object sender, System.EventArgs e)
        {
            StartTimer();
        }
        private void StartTimer()
        {
            lvPreview.Enabled = false;
            tmrFillPreview.Start();
        }

        private void tmrFillPreview_Tick(object sender, System.EventArgs e)
        {
            tmrFillPreview.Stop();
            FillPreview();
        }
        private void chkTestAll_CheckedChanged(object sender, System.EventArgs e)
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
                txtFolder.BackColor = System.Drawing.SystemColors.Window;

            if (Grid1.RowsCount <= 1) // 1 for header
                return; // empty

            lvPreview.Enabled = true;

            FNPRegexList rel = new FNPRegexList();

            if (chkTestAll.Checked)
            {
                for (int i = 1; i < Grid1.RowsCount; i++)
                {
                    FilenameProcessorRE re = REForRow(i);
                    if (re != null)
                        rel.Add(re);
                }
            }
            else
            {
                int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();
                if (rowsIndex.Length == 0)
                    return;

                FilenameProcessorRE re2 = REForRow(rowsIndex[0]);
                if (re2 != null)
                    rel.Add(re2);
                else
                    return;
            }

            lvPreview.BeginUpdate();
            DirectoryInfo d = new DirectoryInfo(txtFolder.Text);
            foreach (FileInfo fi in d.GetFiles())
            {
                int seas;
                int ep;

                if (!TheSettings.UsefulExtension(fi.Extension, true))
                    continue; // move on

                bool r = TVDoc.FindSeasEp(fi, out seas, out ep, cbShowList.Text, rel);
                ListViewItem lvi = new ListViewItem();
                lvi.Text = fi.Name;
                lvi.SubItems.Add((seas == -1) ? "-" : seas.ToString());
                lvi.SubItems.Add((ep == -1) ? "-" : ep.ToString());
                if (!r)
                    lvi.BackColor = Helpers.WarningColor();
                lvPreview.Items.Add(lvi);
            }
            lvPreview.EndUpdate();

        }
        private void bnDefaults_Click(object sender, System.EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Restore to default matching expressions?", "Filename Processors", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
                FillGrid(TVSettings.DefaultFNPList());
        }
    }
}


