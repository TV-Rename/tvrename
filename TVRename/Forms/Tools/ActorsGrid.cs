//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TVRename;

using SourceGrid.Cells;
using Forms;

/// <summary>
/// Summary for ActorsGrid
///
/// WARNING: If you change the name of this class, you will need to change the
///          'Resource File Name' property for the managed resource compiler tool
///          associated with all .resx files this class depends on.  Otherwise,
///          the designers will not be able to interact properly with localized
///          resources associated with this form.
/// </summary>
public partial class ActorsGrid : Form
{
    private int @internal;
    private DataArr theData;
    private readonly TVDoc mDoc;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public ActorsGrid(TVDoc doc)
    {
        @internal = 0;

        InitializeComponent();

        mDoc = doc;
        Cursor.Current = Cursors.WaitCursor;
        BuildData();
        DoSort();
        Cursor.Current = Cursors.Default;
        theData = new DataArr(mDoc.TvLibrary.Shows.Count());
    }

    private void BuildData()
    {
        // find actors that have been in more than one thing
        theData = new DataArr(mDoc.TvLibrary.Shows.Count());
        foreach (ShowConfiguration ser in mDoc.TvLibrary.Shows)
        {
            CachedSeriesInfo? si = ser.CachedShow;
            foreach (string aa in ser.Actors.Select(act => act.ActorName.Trim()).Where(aa => !string.IsNullOrEmpty(aa)))
            {
                theData.Set(ser.ShowName, aa, true);
            }

            if (cbGuestStars.Checked && si != null)
            {
                foreach (Episode ep in si.Episodes)
                {
                    foreach (string g in ep.GuestStars)
                    {
                        theData.Set(ser.ShowName, g.Trim(), false);
                    }
                }
            }
        }

        theData.RemoveEmpties();
    }

    private void SortByName()
    {
        @internal++;
        rbName.Checked = true;
        @internal--;
        theData.SortRows(false);
        theData.SortCols(false);
    }

    private void SortByTotals()
    {
        @internal++;
        rbTotals.Checked = true;
        @internal--;
        theData.SortRows(true);
        theData.SortCols(true);
    }

    private void SortRowsByCount()
    {
        @internal++;
        rbCustom.Checked = true;
        @internal--;
        theData.SortRows(true);
        FillGrid();
    }

    private void SortColsByCount()
    {
        @internal++;
        rbCustom.Checked = true;
        @internal--;
        theData.SortCols(true);
        FillGrid();
    }

    private void ActorToTop(string a)
    {
        @internal++;
        rbCustom.Checked = true;
        @internal--;

        theData.MoveColToTop(a);

        // also move the shows they've been in to the top, too
        int c = theData.Cols.IndexOf(a);
        if (c != 0)
        {
            return; // uh oh!
        }

        int end = 0;
        for (int r = theData.DataR - 1; r >= end; r--)
        {
            if (theData.Data[r][0].HasValue)
            {
                theData.MoveRowToTop(theData.Rows[r++]);
                end++;
            }
        }
        FillGrid();
    }

    private void ShowToTop(string s)
    {
        @internal++;
        rbCustom.Checked = true;
        @internal--;

        theData.MoveRowToTop(s);

        // also move the actors in this show to the top, too
        int r = theData.Rows.IndexOf(s);
        if (r != 0)
        {
            return; // uh oh!
        }

        int end = 0;
        for (int c = theData.DataC - 1; c >= end; c--)
        {
            if (theData.Data[0][c].HasValue)
            {
                theData.MoveColToTop(theData.Cols[c++]);
                end++;
            }
        }

        FillGrid();
    }

    private void FillGrid()
    {
        SourceGrid.Cells.Views.Cell colTitleModel = new()
        {
            ElementText = new RotatedText(-90.0f),
            BackColor = Color.SteelBlue,
            ForeColor = Color.White,
            TextAlignment = DevAge.Drawing.ContentAlignment.BottomCenter
        };

        SourceGrid.Cells.Views.Cell topleftTitleModel = new()
        {
            BackColor = Color.SteelBlue,
            ForeColor = Color.White,
            TextAlignment = DevAge.Drawing.ContentAlignment.BottomLeft
        };

        SourceGrid.Cells.Views.Cell isActorModel = new()
        {
            BackColor = Color.Green,
            ForeColor = Color.Green,
            TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
        };

        SourceGrid.Cells.Views.Cell isGuestModel = new()
        {
            BackColor = Color.LightGreen,
            ForeColor = Color.LightGreen,
            TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
        };

        grid1.Columns.Clear();
        grid1.Rows.Clear();

        int rows = theData.DataR + 2; // title and total
        int cols = theData.DataC + 2;

        grid1.ColumnsCount = cols;
        grid1.RowsCount = rows;
        grid1.FixedColumns = 1;
        grid1.FixedRows = 1;
        grid1.Selection.EnableMultiSelection = false;

        for (int i = 0; i < cols; i++)
        {
            grid1.Columns[i].AutoSizeMode = i == 0 ? SourceGrid.AutoSizeMode.Default : SourceGrid.AutoSizeMode.MinimumSize;
            if (i > 0)
            {
                grid1.Columns[i].Width = 24;
            }
        }

        grid1.Rows[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize;
        grid1.Rows[0].Height = 100;

        ColumnHeader h = new("Show")
        {
            AutomaticSortEnabled = false,
            ResizeEnabled = false
        };

        grid1[0, 0] = h;
        grid1[0, 0].View = topleftTitleModel;
        grid1[0, 0].AddController(new SideClickEvent(this, null)); // default sort

        for (int c = 0; c < theData.DataC; c++)
        {
            h = new ColumnHeader(theData.Cols[c])
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false
            };

            grid1[0, c + 1] = h;
            grid1[0, c + 1].View = colTitleModel;
            grid1[0, c + 1].AddController(new TopClickEvent(this, theData.Cols[c]));
        }

        int totalCol = grid1.ColumnsCount - 1;
        h = new ColumnHeader("Totals")
        {
            AutomaticSortEnabled = false,
            ResizeEnabled = false
        };
        grid1.Columns[totalCol].Width = 48;
        grid1[0, totalCol] = h;
        grid1[0, totalCol].View = colTitleModel;
        grid1[0, totalCol].AddController(new SortRowsByCountEvent(this));

        for (int r = 0; r < theData.DataR; r++)
        {
            grid1[r + 1, 0] = new RowHeader(theData.Rows[r])
            {
                ResizeEnabled = false
            };
            grid1[r + 1, 0].AddController(new SideClickEvent(this, theData.Rows[r]));
        }

        grid1[theData.DataR + 1, 0] = new RowHeader("Totals")
        {
            ResizeEnabled = false
        };
        grid1[theData.DataR + 1, 0].AddController(new SortColsByCountEvent(this));

        for (int c = 0; c < theData.DataC; c++)
        {
            for (int r = 0; r < theData.DataR; r++)
            {
                if (theData.Data[r][c].HasValue)
                {
                    grid1[r + 1, c + 1] = new Cell("Y");
                    {
                        bool? x = theData.Data[r][c];
                        if (x.HasValue)
                        {
                            grid1[r + 1, c + 1].View = x.Value ? isActorModel : isGuestModel;
                        }
                        else
                        {
                            grid1[r + 1, c + 1].View = isActorModel;
                        }
                    }
                    grid1[r + 1, c + 1].AddController(new CellClickEvent(theData.Cols[c]));
                }
                else
                {
                    grid1[r + 1, c + 1] = new Cell("");
                }
            }
        }

        for (int c = 0; c < theData.DataC; c++)
        {
            grid1[rows - 1, c + 1] = new Cell(theData.ColScore(c));
        }

        for (int r = 0; r < theData.DataR; r++)
        {
            grid1[r + 1, cols - 1] = new Cell(theData.RowScore(r, null));
        }

        grid1[theData.DataR + 1, theData.DataC + 1] = new Cell("");

        grid1.AutoSizeCells();
    }

    private void bnClose_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void cbGuestStars_CheckedChanged(object sender, EventArgs e)
    {
        cbGuestStars.Update();
        BuildData();
        DoSort();
    }

    private void bnSave_Click(object sender, EventArgs e)
    {
        saveFile.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
        if (UiHelpers.ShowDialogAndOk(saveFile,this))
        {
            try
            {
                SourceGrid.Exporter.Image image = new();
                Bitmap b = image.Export(grid1, grid1.CompleteRange);
                b.Save(saveFile.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, $"Failed to save {saveFile.FileName}.");
            }
        }
    }

    private void DoSort()
    {
        if (rbTotals.Checked)
        {
            SortByTotals();
        }
        else
        {
            SortByName(); // will check name for us, too
        }

        FillGrid();
    }

    private void rbName_CheckedChanged(object sender, EventArgs e)
    {
        if (@internal != 0)
        {
            return;
        }

        DoSort();
    }

    private void rbTotals_CheckedChanged(object sender, EventArgs e)
    {
        if (@internal != 0)
        {
            return;
        }

        DoSort();
    }

    #region Nested type: CellClickEvent

    private class CellClickEvent : SourceGrid.Cells.Controllers.ControllerBase
    {
        private readonly string who;

        public CellClickEvent(string who)
        {
            this.who = who;
        }

        public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
        {
            who.ToImdbActorLink().OpenUrlInBrowser();
        }
    }

    #endregion Nested type: CellClickEvent

    #region Nested type: DataArr

    private class DataArr
    {
        private int allocC;
        private int allocR;
        public readonly List<string> Cols;
        public bool?[][] Data;
        public int DataC;
        public int DataR;
        public readonly List<string> Rows;

        public DataArr(int rowCountPreAlloc)
        {
            Rows = [];
            Cols = [];
            allocR = rowCountPreAlloc;
            allocC = rowCountPreAlloc * 10;
            Data = new bool?[allocR][];
            for (int i = 0; i < allocR; i++)
            {
                Data[i] = new bool?[allocC];
            }

            DataR = 0;
            DataC = 0;
        }

        private void SwapCols(int c1, int c2)
        {
            for (int r = 0; r < DataR; r++)
            {
                (Data[r][c2], Data[r][c1]) = (Data[r][c1], Data[r][c2]);
            }
            (Cols[c1], Cols[c2]) = (Cols[c2], Cols[c1]);
        }

        private void SwapRows(int r1, int r2)
        {
            for (int c = 0; c < DataC; c++)
            {
                (Data[r2][c], Data[r1][c]) = (Data[r1][c], Data[r2][c]);
            }
            (Rows[r1], Rows[r2]) = (Rows[r2], Rows[r1]);
        }

        public int RowScore(int r, IReadOnlyList<bool>? onlyCols)
        {
            int t = 0;
            for (int c = 0; c < DataC; c++)
            {
                if (Data[r][c] != null && (onlyCols is null || onlyCols[c]))
                {
                    t++;
                }
            }
            return t;
        }

        public int ColScore(int c)
        {
            int t = 0;
            for (int r = 0; r < DataR; r++)
            {
                if (Data[r][c] != null)
                {
                    t++;
                }
            }
            return t;
        }

        private void Resize()
        {
            int newr = Rows.Count;
            int newc = Cols.Count;
            if (newr > allocR || newc > allocC) // need to enlarge array
            {
                if (newr > allocR)
                {
                    allocR = newr * 2;
                }

                if (newc > allocC)
                {
                    allocC = newc * 2;
                }

                bool?[][] newarr = new bool?[allocR][];
                for (int i = 0; i < allocR; i++)
                {
                    newarr[i] = new bool?[allocC];
                }

                for (int r = 0; r < DataR; r++)
                {
                    for (int c = 0; c < DataC; c++)
                    {
                        if (r < newr && c < newc)
                        {
                            newarr[r][c] = Data[r][c];
                        }
                    }
                }
                Data = newarr;
            }
            DataR = newr;
            DataC = newc;
        }

        private int AddRow(string name)
        {
            Rows.Add(name);
            Resize();
            return Rows.Count - 1;
        }

        private int AddCol(string name)
        {
            Cols.Add(name);
            Resize();
            return Cols.Count - 1;
        }

        public void RemoveEmpties()
        {
            bool[] keepR = new bool[DataR];
            bool[] keepC = new bool[DataC];
            // trim by actor
            int countR = 0;
            int countC = 0;

            for (int c = 0; c < DataC; c++)
            {
                if (ColScore(c) >= 2)
                {
                    keepC[c] = true;
                    countC++;
                }
            }
            for (int r = 0; r < DataR; r++)
            {
                if (RowScore(r, keepC) >= 1)
                {
                    keepR[r] = true;
                    countR++;
                }
            }

            bool?[][] newarr = new bool?[countR][];
            for (int r = 0, newR = 0; r < DataR; r++)
            {
                if (keepR[r])
                {
                    newarr[newR] = new bool?[countC];
                    for (int c = 0, newC = 0; c < DataC; c++)
                    {
                        if (keepR[r] && keepC[c])
                        {
                            newarr[newR][newC++] = Data[r][c];
                        }
                    }
                    newR++;
                }
            }

            for (int r = 0, newR = 0; r < DataR; r++)
            {
                if (keepR[r])
                {
                    Rows[newR++] = Rows[r];
                }
            }
            for (int c = 0, newC = 0; c < DataC; c++)
            {
                if (keepC[c])
                {
                    Cols[newC++] = Cols[c];
                }
            }

            Rows.RemoveRange(countR, Rows.Count - countR);
            Cols.RemoveRange(countC, Cols.Count - countC);

            Data = newarr;
            allocR = countR;
            DataR = countR;
            allocC = countC;
            DataC = countC;
        }

        public void Set(string row, string col, bool isActor) // isActor = false means guest star
        {
            int r = Rows.IndexOf(row);
            int c = Cols.IndexOf(col);
            if (r == -1)
            {
                r = AddRow(row);
            }

            if (c == -1)
            {
                c = AddCol(col);
            }

            Data[r][c] = isActor;
        }

        public void SortCols(bool score)
        {
            for (int c2 = 0; c2 < DataC - 1; c2++)
            {
                int topscore = 0;
                string topword = string.Empty;
                int maxat = -1;
                for (int c = c2; c < DataC; c++)
                {
                    if (score)
                    {
                        int sc = ColScore(c);
                        if (maxat == -1 || sc > topscore)
                        {
                            maxat = c;
                            topscore = sc;
                        }
                    }
                    else
                    {
                        if (maxat == -1 || string.Compare(Cols[c], topword, StringComparison.Ordinal) < 0)
                        {
                            maxat = c;
                            topword = Cols[c];
                        }
                    }
                }
                if (maxat != c2)
                {
                    SwapCols(c2, maxat);
                }
            }
        }

        public void MoveColToTop(string col)
        {
            int n = Cols.IndexOf(col);
            if (n == -1)
            {
                return;
            }

            for (int r = 0; r < DataR; r++)
            {
                bool? t = Data[r][n];
                for (int c = n; c > 0; c--)
                {
                    Data[r][c] = Data[r][c - 1];
                }

                Data[r][0] = t;
            }

            string t2 = Cols[n];
            for (int c = n; c > 0; c--)
            {
                Cols[c] = Cols[c - 1];
            }

            Cols[0] = t2;
        }

        public void MoveRowToTop(string row)
        {
            int n = Rows.IndexOf(row);
            if (n == -1)
            {
                return;
            }

            for (int c = 0; c < DataC; c++)
            {
                bool? t = Data[n][c];
                for (int r = n; r > 0; r--)
                {
                    Data[r][c] = Data[r - 1][c];
                }

                Data[0][c] = t;
            }

            string t2 = Rows[n];
            for (int r = n; r > 0; r--)
            {
                Rows[r] = Rows[r - 1];
            }

            Rows[0] = t2;
        }

        public void SortRows(bool score)
        {
            for (int r2 = 0; r2 < DataR - 1; r2++)
            {
                int topscore = 0;
                int maxat = -1;
                string topword = string.Empty;
                for (int r = r2; r < DataR; r++)
                {
                    if (score)
                    {
                        int sc = RowScore(r, null);
                        if (maxat == -1 || sc > topscore)
                        {
                            maxat = r;
                            topscore = sc;
                        }
                    }
                    else
                    {
                        if (maxat == -1 || string.Compare(Rows[r], topword, StringComparison.Ordinal) < 0)
                        {
                            maxat = r;
                            topword = Rows[r];
                        }
                    }
                }
                if (maxat != r2)
                {
                    SwapRows(r2, maxat);
                }
            }
        }
    }

    #endregion Nested type: DataArr

    #region Nested type: RotatedText

    public class RotatedText : DevAge.Drawing.VisualElements.TextGDI
    {
        private readonly float angle;

        public RotatedText(float angle)
        {
            this.angle = angle;
        }

        protected override void OnDraw(DevAge.Drawing.GraphicsCache graphics, RectangleF area)
        {
            System.Drawing.Drawing2D.GraphicsState state = graphics.Graphics.Save();
            try
            {
                float width2 = area.Width / 2;
                float height2 = area.Height / 2;

                //For a better drawing use the clear type rendering
                graphics.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                //Move the origin to the center of the cell (for a more easy rotation)
                graphics.Graphics.TranslateTransform(area.X + width2, area.Y + height2);

                graphics.Graphics.RotateTransform(angle);
                graphics.Graphics.TranslateTransform(-height2, 0); //-(area.Y + height2))

                StringFormat.Alignment = StringAlignment.Near;
                StringFormat.LineAlignment = StringAlignment.Center;
                graphics.Graphics.DrawString(Value, Font, graphics.BrushsCache.GetBrush(ForeColor), 0, 0, StringFormat);
            }
            finally
            {
                graphics.Graphics.Restore(state);
            }
        }
    }

    #endregion Nested type: RotatedText

    #region Nested type: SideClickEvent

    private class SideClickEvent : SourceGrid.Cells.Controllers.ControllerBase
    {
        private readonly ActorsGrid g;
        private readonly string? show;

        public SideClickEvent(ActorsGrid g, string? show)
        {
            this.show = show;
            this.g = g;
        }

        public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
        {
            if (show is null)
            {
                g.DoSort();
            }
            else
            {
                g.ShowToTop(show);
            }
        }
    }

    #endregion Nested type: SideClickEvent

    #region Nested type: SortColsByCountEvent

    private class SortColsByCountEvent : SourceGrid.Cells.Controllers.ControllerBase
    {
        private readonly ActorsGrid g;

        public SortColsByCountEvent(ActorsGrid g)
        {
            this.g = g;
        }

        public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
        {
            g.SortColsByCount();
        }
    }

    #endregion Nested type: SortColsByCountEvent

    #region Nested type: SortRowsByCountEvent

    private class SortRowsByCountEvent : SourceGrid.Cells.Controllers.ControllerBase
    {
        private readonly ActorsGrid grid;

        public SortRowsByCountEvent(ActorsGrid g)
        {
            grid = g;
        }

        public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
        {
            grid.SortRowsByCount();
        }
    }

    #endregion Nested type: SortRowsByCountEvent

    #region Nested type: TopClickEvent

    private class TopClickEvent : SourceGrid.Cells.Controllers.ControllerBase
    {
        private readonly string actor;
        private readonly ActorsGrid g;

        public TopClickEvent(ActorsGrid g, string act)
        {
            this.g = g;
            actor = act;
        }

        public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
        {
            g.ActorToTop(actor);
        }
    }

    #endregion Nested type: TopClickEvent
}
