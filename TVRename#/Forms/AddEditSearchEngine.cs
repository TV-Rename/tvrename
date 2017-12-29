// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Drawing;
using System.Windows.Forms;
using SourceGrid;
using SourceGrid.Cells.Views;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;
using ContentAlignment = DevAge.Drawing.ContentAlignment;

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
        private CustomNameTagsFloatingWindow _cntfw;
        private Grid _grid1;
        //array<SourceGrid::Cells::Editors::EditorBase ^> ^MyEditors;

        private readonly ProcessedEpisode _sampleEpisode;
        private readonly Searchers _mSearchers;

        public AddEditSearchEngine(Searchers s, ProcessedEpisode pe)
        {
            _sampleEpisode = pe;
            InitializeComponent();
            _cntfw = null;

            SetupGrid();
            _mSearchers = s;

            for (int i = 0; i < _mSearchers.Count(); i++)
            {
                AddNewRow();
                _grid1[i + 1, 0].Value = _mSearchers.Name(i);
                _grid1[i + 1, 1].Value = _mSearchers.Url(i);
            }
        }

        public void SetupGrid()
        {
            Cell titleModel = new Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = ContentAlignment.MiddleLeft
                                                         };

            _grid1.Columns.Clear();
            _grid1.Rows.Clear();

            _grid1.RowsCount = 1;
            _grid1.ColumnsCount = 2;
            _grid1.FixedRows = 1;
            _grid1.FixedColumns = 0;
            _grid1.Selection.EnableMultiSelection = false;

            _grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            _grid1.Columns[0].Width = 80;

            _grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            _grid1.AutoStretchColumnsToFitWidth = true;
            //Grid1->AutoSizeCells();
            _grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            ColumnHeader h;
            h = new ColumnHeader("Name");
            h.AutomaticSortEnabled = false;
            _grid1[0, 0] = h;
            _grid1[0, 0].View = titleModel;

            h = new ColumnHeader("URL");
            h.AutomaticSortEnabled = false;
            _grid1[0, 1] = h;
            _grid1[0, 1].View = titleModel;
        }

        public void AddNewRow()
        {
            int r = _grid1.RowsCount;
            _grid1.RowsCount = r + 1;

            _grid1[r, 0] = new SourceGrid.Cells.Cell("", typeof(string));
            _grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~AddEditSearchEngine()
        {
            if (_cntfw != null)
                _cntfw.Close();
        }

        private void bnAdd_Click(object sender, EventArgs e)
        {
            AddNewRow();
            _grid1.Selection.Focus(new Position(_grid1.RowsCount - 1, 1), true);
        }

        private void bnDelete_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = _grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                _grid1.Rows.Remove(rowsIndex[0]);
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            _mSearchers.Clear();
            for (int i = 1; i < _grid1.RowsCount; i++) // skip header row
            {
                string name = (string) (_grid1[i, 0].Value);
                string url = (string) (_grid1[i, 1].Value);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                    _mSearchers.Add(name, url);
            }
        }

        private void bnTags_Click(object sender, EventArgs e)
        {
            _cntfw = new CustomNameTagsFloatingWindow(_sampleEpisode);
            _cntfw.Show(this);
            Focus();
        }
    }
}
