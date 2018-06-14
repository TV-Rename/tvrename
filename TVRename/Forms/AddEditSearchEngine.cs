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

        private readonly ProcessedEpisode SampleEpisode;
        private readonly Searchers mSearchers;

        public AddEditSearchEngine(Searchers s, ProcessedEpisode pe)
        {
            SampleEpisode = pe;
            InitializeComponent();
            Cntfw = null;

            SetupGrid();
            mSearchers = s;

            for (int i = 0; i < mSearchers.Count(); i++)
            {
                AddNewRow();
                Grid1[i + 1, 0].Value = mSearchers.Name(i);
                Grid1[i + 1, 1].Value = mSearchers.URL(i);
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

            Grid1.Columns.Clear();
            Grid1.Rows.Clear();

            Grid1.RowsCount = 1;
            Grid1.ColumnsCount = 2;
            Grid1.FixedRows = 1;
            Grid1.FixedColumns = 0;
            Grid1.Selection.EnableMultiSelection = false;

            Grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            Grid1.Columns[0].Width = 80;

            Grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            Grid1.AutoStretchColumnsToFitWidth = true;
            //Grid1->AutoSizeCells();
            Grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("Name");
            h.AutomaticSortEnabled = false;
            Grid1[0, 0] = h;
            Grid1[0, 0].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("URL");
            h.AutomaticSortEnabled = false;
            Grid1[0, 1] = h;
            Grid1[0, 1].View = titleModel;
        }

        public void AddNewRow()
        {
            int r = Grid1.RowsCount;
            Grid1.RowsCount = r + 1;

            Grid1[r, 0] = new SourceGrid.Cells.Cell("", typeof(string));
            Grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
        }





        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            AddNewRow();
            Grid1.Selection.Focus(new SourceGrid.Position(Grid1.RowsCount - 1, 1), true);
        }

        private void bnDelete_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                Grid1.Rows.Remove(rowsIndex[0]);
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            mSearchers.Clear();
            for (int i = 1; i < Grid1.RowsCount; i++) // skip header row
            {
                string name = (string) (Grid1[i, 0].Value);
                string url = (string) (Grid1[i, 1].Value);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                    mSearchers.Add(name, url);
            }
        }

        private void bnTags_Click(object sender, System.EventArgs e)
        {
            Cntfw = new CustomNameTagsFloatingWindow(SampleEpisode);
            Cntfw.Show(this);
            Focus();
        }
    }
}
