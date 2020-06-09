// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private CustomNameTagsFloatingWindow? cntfw;
        
        private readonly ProcessedEpisode? sampleEpisode;
        private readonly Searchers mSearchers;

        public AddEditSearchEngine(Searchers s, ProcessedEpisode? pe)
        {
            sampleEpisode = pe;
            InitializeComponent();
            cntfw = null;

            SetupGrid();
            mSearchers = s;

            int row = 1;
            foreach (SearchEngine engine in mSearchers)
            {
                AddNewRow();
                grid1[row, 0].Value = engine.Name;
                grid1[row, 1].Value = engine.Url;
                row++;
            }
        }

        private void SetupGrid()
        {
            Cell titleModel = new Cell
                 {
                     BackColor = Color.SteelBlue,
                     ForeColor = Color.White,
                     TextAlignment = ContentAlignment.MiddleLeft
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
            grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            ColumnHeader h = new ColumnHeader("Name") {AutomaticSortEnabled = false};
            grid1[0, 0] = h;
            grid1[0, 0].View = titleModel;

            h = new ColumnHeader("URL") {AutomaticSortEnabled = false};
            grid1[0, 1] = h;
            grid1[0, 1].View = titleModel;
        }

        private void AddNewRow()
        {
            int r = grid1.RowsCount;
            grid1.RowsCount = r + 1;

            grid1[r, 0] = new SourceGrid.Cells.Cell("", typeof(string));
            grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
        }

        private void bnAdd_Click(object sender, EventArgs e)
        {
            AddNewRow();
            grid1.Selection.Focus(new Position(grid1.RowsCount - 1, 1), true);
        }

        private void bnDelete_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
            {
                grid1.Rows.Remove(rowsIndex[0]);
            }
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            mSearchers.Clear();
            for (int i = 1; i < grid1.RowsCount; i++) // skip header row
            {
                string name = (string) grid1[i, 0].Value;
                string url = (string) grid1[i, 1].Value;
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                {
                    mSearchers.Add(new SearchEngine{ Name=name,Url= url });
                }
            }
        }

        private void bnTags_Click(object sender, EventArgs e)
        {
            cntfw = new CustomNameTagsFloatingWindow(sampleEpisode);
            cntfw.Show(this);
            Focus();
        }
    }
}
