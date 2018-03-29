// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System.Windows.Forms;
using System.Drawing;

namespace TVRename
{
    /// <summary>
    /// Summary for AddEditSearchEngine
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class AddEditSearchEngine : Form
    {
        private CustomNameTagsFloatingWindow Cntfw;
        private SourceGrid.Grid Grid1;
        //array<SourceGrid::Cells::Editors::EditorBase ^> ^MyEditors;

        private ProcessedEpisode SampleEpisode;
        private Searchers mSearchers;

        public AddEditSearchEngine(Searchers s, ProcessedEpisode pe)
        {
            this.SampleEpisode = pe;
            this.InitializeComponent();
            this.Cntfw = null;

            this.SetupGrid();
            this.mSearchers = s;

            for (int i = 0; i < this.mSearchers.Count(); i++)
            {
                this.AddNewRow();
                this.Grid1[i + 1, 0].Value = this.mSearchers.Name(i);
                this.Grid1[i + 1, 1].Value = this.mSearchers.URL(i);
            }
        }

        public void SetupGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
                                                         };

            this.Grid1.Columns.Clear();
            this.Grid1.Rows.Clear();

            this.Grid1.RowsCount = 1;
            this.Grid1.ColumnsCount = 2;
            this.Grid1.FixedRows = 1;
            this.Grid1.FixedColumns = 0;
            this.Grid1.Selection.EnableMultiSelection = false;

            this.Grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            this.Grid1.Columns[0].Width = 80;

            this.Grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            this.Grid1.AutoStretchColumnsToFitWidth = true;
            //Grid1->AutoSizeCells();
            this.Grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("Name");
            h.AutomaticSortEnabled = false;
            this.Grid1[0, 0] = h;
            this.Grid1[0, 0].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("URL");
            h.AutomaticSortEnabled = false;
            this.Grid1[0, 1] = h;
            this.Grid1[0, 1].View = titleModel;
        }

        public void AddNewRow()
        {
            int r = this.Grid1.RowsCount;
            this.Grid1.RowsCount = r + 1;

            this.Grid1[r, 0] = new SourceGrid.Cells.Cell("", typeof(string));
            this.Grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
        }





        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            this.AddNewRow();
            this.Grid1.Selection.Focus(new SourceGrid.Position(this.Grid1.RowsCount - 1, 1), true);
        }

        private void bnDelete_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = this.Grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                this.Grid1.Rows.Remove(rowsIndex[0]);
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            this.mSearchers.Clear();
            for (int i = 1; i < this.Grid1.RowsCount; i++) // skip header row
            {
                string name = (string) (this.Grid1[i, 0].Value);
                string url = (string) (this.Grid1[i, 1].Value);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                    this.mSearchers.Add(name, url);
            }
        }

        private void bnTags_Click(object sender, System.EventArgs e)
        {
            this.Cntfw = new CustomNameTagsFloatingWindow(this.SampleEpisode);
            this.Cntfw.Show(this);
            this.Focus();
        }
    }
}
