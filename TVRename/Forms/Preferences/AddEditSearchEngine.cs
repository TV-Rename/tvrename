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
    /// Summary for AddEditSearchEngineSe
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class AddEditSearchEngine : Form
    {
        private CustomNameTagsFloatingWindow cntfw;
        private SourceGrid.Grid grid1;
        
        private readonly ProcessedEpisode sampleEpisode;
        private readonly Searchers mSearchers;

        public AddEditSearchEngine(Searchers s, ProcessedEpisode pe)
        {
            sampleEpisode = pe;
            InitializeComponent();
            cntfw = null;

            SetupGrid();
            mSearchers = s;

            for (int i = 0; i < mSearchers.Count(); i++)
            {
                AddNewRow();
                grid1[i + 1, 0].Value = mSearchers.Name(i);
                grid1[i + 1, 1].Value = mSearchers.Url(i);
            }
        }

        private void SetupGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
                                                         };
            grid1.Columns.Clear();
            grid1.Rows.Clear();

            grid1.RowsCount = 1;
            grid1.ColumnsCount = 2;
            grid1.FixedRows = 1;
            grid1.FixedColumns = 0;
            grid1.Selection.EnableMultiSelection = false;

            grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            grid1.Columns[0].Width = 80;

            grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            grid1.AutoStretchColumnsToFitWidth = true;
            //Grid1->AutoSizeCells();
            grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("Name");
            h.AutomaticSortEnabled = false;
            grid1[0, 0] = h;
            grid1[0, 0].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("URL");
            h.AutomaticSortEnabled = false;
            grid1[0, 1] = h;
            grid1[0, 1].View = titleModel;
        }

        public void AddNewRow()
        {
            int r = grid1.RowsCount;
            grid1.RowsCount = r + 1;

            grid1[r, 0] = new SourceGrid.Cells.Cell("", typeof(string));
            grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
        }





        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            AddNewRow();
            grid1.Selection.Focus(new SourceGrid.Position(grid1.RowsCount - 1, 1), true);
        }

        private void bnDelete_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                grid1.Rows.Remove(rowsIndex[0]);
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            mSearchers.Clear();
            for (int i = 1; i < grid1.RowsCount; i++) // skip header row
            {
                string name = (string) (grid1[i, 0].Value);
                string url = (string) (grid1[i, 1].Value);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                    mSearchers.Add(name, url);
            }
        }

        private void bnTags_Click(object sender, System.EventArgs e)
        {
            cntfw = new CustomNameTagsFloatingWindow(sampleEpisode);
            cntfw.Show(this);
            Focus();
        }
    }
}
