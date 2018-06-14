// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TVRename
{
    using SourceGrid.Cells;

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
        private int Internal;
        private DataArr TheData;
        private TVDoc mDoc;

        public ActorsGrid(TVDoc doc)
        {
            Internal = 0;

            InitializeComponent();

            mDoc = doc;

            BuildData();
            DoSort();
        }

        private void BuildData()
        {
            // find actors that have been in more than one thing
            // Dictionary<String^, List<String> ^> ^whoInWhat = gcnew Dictionary<String^, List<String> ^>;
            TheTVDB.Instance.GetLock("Actors");
            TheData = new DataArr(mDoc.Library.Count);
            foreach (ShowItem ser in mDoc.Library.Shows)
            {
                SeriesInfo si = TheTVDB.Instance.GetSeries(ser.TVDBCode);
                foreach (string act in si.GetActors())
                {
                    string aa = act.Trim();
                    if (!string.IsNullOrEmpty(aa))
                        TheData.Set(si.Name, aa, true);
                }

                if (cbGuestStars.Checked)
                {
                    foreach (KeyValuePair<int, Season> kvp in si.AiredSeasons) //We can use AiredSeasons as it does not matter which order we do this in Aired or DVD
                    {
                        foreach (Episode ep in kvp.Value.Episodes)
                        {
                                foreach (string g in ep.GetGuestStars())
                                {
                                    string aa = g.Trim();
                                    if (!string.IsNullOrEmpty(aa))
                                        TheData.Set(si.Name, aa, false);
                                }
                        }
                    }
                }
            }

            TheTVDB.Instance.Unlock("Actors");
            TheData.RemoveEmpties();
        }

        private void SortByName()
        {
            Internal++;
            rbName.Checked = true;
            Internal--;
            TheData.SortRows(false);
            TheData.SortCols(false);
        }

        private void SortByTotals()
        {
            Internal++;
            rbTotals.Checked = true;
            Internal--;
            TheData.SortRows(true);
            TheData.SortCols(true);
        }

        private void SortRowsByCount()
        {
            Internal++;
            rbCustom.Checked = true;
            Internal--;
            TheData.SortRows(true);
            FillGrid();
        }

        private void SortColsByCount()
        {
            Internal++;
            rbCustom.Checked = true;
            Internal--;
            TheData.SortCols(true);
            FillGrid();
        }

        private void ActorToTop(string a)
        {
            Internal++;
            rbCustom.Checked = true;
            Internal--;

            TheData.MoveColToTop(a);

            // also move the shows they've been in to the top, too
            int c = TheData.Cols.IndexOf(a);
            if (c != 0)
                return; // uh oh!
            int end = 0;
            for (int r = TheData.DataR - 1; r >= end; r--)
            {
                if (TheData.Data[r][0].HasValue)
                {
                    TheData.MoveRowToTop(TheData.Rows[r++]);
                    end++;
                }
            }
            FillGrid();
        }

        private void ShowToTop(string s)
        {
            Internal++;
            rbCustom.Checked = true;
            Internal--;

            TheData.MoveRowToTop(s);

            // also move the actors in this show to the top, too
            int r = TheData.Rows.IndexOf(s);
            if (r != 0)
                return; // uh oh!
            int end = 0;
            for (int c = TheData.DataC - 1; c >= end; c--)
            {
                if (TheData.Data[0][c].HasValue)
                {
                    TheData.MoveColToTop(TheData.Cols[c++]);
                    end++;
                }
            }

            FillGrid();
        }

        private void FillGrid()
        {
            SourceGrid.Cells.Views.Cell colTitleModel = new SourceGrid.Cells.Views.Cell
            {
                ElementText = new RotatedText(-90.0f),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = DevAge.Drawing.ContentAlignment.BottomCenter
            };

            SourceGrid.Cells.Views.Cell topleftTitleModel = new SourceGrid.Cells.Views.Cell
            {
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = DevAge.Drawing.ContentAlignment.BottomLeft
            };

            SourceGrid.Cells.Views.Cell isActorModel = new SourceGrid.Cells.Views.Cell
            {
                BackColor = Color.Green,
                ForeColor = Color.Green,
                TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
            };

            SourceGrid.Cells.Views.Cell isGuestModel = new SourceGrid.Cells.Views.Cell
            {
                BackColor = Color.LightGreen,
                ForeColor = Color.LightGreen,
                TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
            };

            grid1.Columns.Clear();
            grid1.Rows.Clear();

            int rows = TheData.DataR + 2; // title and total
            int cols = TheData.DataC + 2;

            grid1.ColumnsCount = cols;
            grid1.RowsCount = rows;
            grid1.FixedColumns = 1;
            grid1.FixedRows = 1;
            grid1.Selection.EnableMultiSelection = false;

            for (int i = 0; i < cols; i++)
            {
                grid1.Columns[i].AutoSizeMode = i == 0 ? SourceGrid.AutoSizeMode.Default : SourceGrid.AutoSizeMode.MinimumSize;
                if (i > 0)
                    grid1.Columns[i].Width = 24;
            }

            grid1.Rows[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize;
            grid1.Rows[0].Height = 100;

            ColumnHeader h = new ColumnHeader("Show")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false
            };

            grid1[0, 0] = h;
            grid1[0, 0].View = topleftTitleModel;
            grid1[0, 0].AddController(new SideClickEvent(this, null)); // default sort

            for (int c = 0; c < TheData.DataC; c++)
            {
                h = new ColumnHeader(TheData.Cols[c])
                {
                    AutomaticSortEnabled = false,
                    ResizeEnabled = false
                }; // "<A HREF=\"http://www.imdb.com/find?s=nm&q="+kvp->Key+"\">"+kvp->Key+"</a>");

                // h->AddController(sortableController);
                // h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0); // TODO: remove?
                grid1[0, c + 1] = h;
                grid1[0, c + 1].View = colTitleModel;
                grid1[0, c + 1].AddController(new TopClickEvent(this, TheData.Cols[c]));
            }

            int totalCol = grid1.ColumnsCount - 1;
            h = new ColumnHeader("Totals")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false
            };
            //h->AddController(sortableController);
            // h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0);
            grid1.Columns[totalCol].Width = 48;
            grid1[0, totalCol] = h;
            grid1[0, totalCol].View = colTitleModel;
            grid1[0, totalCol].AddController(new SortRowsByCountEvent(this));

            for (int r = 0; r < TheData.DataR; r++)
            {
                grid1[r + 1, 0] = new RowHeader(TheData.Rows[r])
                {
                    ResizeEnabled = false
                };
                grid1[r + 1, 0].AddController(new SideClickEvent(this, TheData.Rows[r]));
            }

            grid1[TheData.DataR + 1, 0] = new RowHeader("Totals")
            {
                ResizeEnabled = false
            };
            grid1[TheData.DataR + 1, 0].AddController(new SortColsByCountEvent(this));

            for (int c = 0; c < TheData.DataC; c++)
            {
                for (int r = 0; r < TheData.DataR; r++)
                {
                    if (TheData.Data[r][c].HasValue)
                    {
                        grid1[r + 1, c + 1] = new Cell("Y")
                        {
                            View = TheData.Data[r][c].Value ? isActorModel : isGuestModel
                        };
                        grid1[r + 1, c + 1].AddController(new CellClickEvent(TheData.Cols[c], TheData.Rows[r]));
                    }
                    else
                        grid1[r + 1, c + 1] = new Cell("");
                }
            }

            for (int c = 0; c < TheData.DataC; c++)
                grid1[rows - 1, c + 1] = new Cell(TheData.ColScore(c));

            for (int r = 0; r < TheData.DataR; r++)
                grid1[r + 1, cols - 1] = new Cell(TheData.RowScore(r, null));

            grid1[TheData.DataR + 1, TheData.DataC + 1] = new Cell("");

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
            if (saveFile.ShowDialog() != DialogResult.OK)
                return;

            SourceGrid.Exporter.Image image = new SourceGrid.Exporter.Image();
            Bitmap b = image.Export(grid1, grid1.CompleteRange);
            b.Save(saveFile.FileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void DoSort()
        {
            if (rbTotals.Checked)
                SortByTotals();
            else
                SortByName(); // will check name for us, too
            FillGrid();
        }

        private void rbName_CheckedChanged(object sender, EventArgs e)
        {
            if (Internal != 0)
                return;
            DoSort();
        }

        private void rbTotals_CheckedChanged(object sender, EventArgs e)
        {
            if (Internal != 0)
                return;
            DoSort();
        }

        #region Nested type: CellClickEvent

        private class CellClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly string Show;
            private readonly string Who;

            public CellClickEvent(string who, string show)
            {
                Who = who;
                Show = show;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                Helpers.SysOpen("http://www.imdb.com/find?s=nm&q=" + Who);
            }
        }

        #endregion

        #region Nested type: DataArr

        private class DataArr
        {
            private int AllocC;
            private int AllocR;
            public readonly List<string> Cols;
            public bool?[][] Data;
            public int DataC;
            public int DataR;
            public readonly List<string> Rows;

            public DataArr(int rowCountPreAlloc)
            {
                Rows = new List<String>();
                Cols = new List<String>();
                AllocR = rowCountPreAlloc;
                AllocC = rowCountPreAlloc * 10;
                Data = new bool?[AllocR][];
                for (int i = 0; i < AllocR; i++)
                    Data[i] = new bool?[AllocC];
                DataR = DataC = 0;
            }

            private void SwapCols(int c1, int c2)
            {
                for (int r = 0; r < DataR; r++)
                {
                    bool? t = Data[r][c2];
                    Data[r][c2] = Data[r][c1];
                    Data[r][c1] = t;
                }
                string t2 = Cols[c1];
                Cols[c1] = Cols[c2];
                Cols[c2] = t2;
            }

            private void SwapRows(int r1, int r2)
            {
                for (int c = 0; c < DataC; c++)
                {
                    bool? t = Data[r2][c];
                    Data[r2][c] = Data[r1][c];
                    Data[r1][c] = t;
                }
                string t2 = Rows[r1];
                Rows[r1] = Rows[r2];
                Rows[r2] = t2;
            }

            public int RowScore(int r, bool[] onlyCols)
            {
                int t = 0;
                for (int c = 0; c < DataC; c++)
                {
                    if (((Data[r][c] != null) && ((onlyCols == null) || onlyCols[c])) && Data[r][c].HasValue)
                        t++;
                }
                return t;
            }

            public int ColScore(int c)
            {
                int t = 0;
                for (int r = 0; r < DataR; r++)
                {
                    if ((Data[r][c] != null) && Data[r][c].HasValue)
                        t++;
                }
                return t;
            }

            private void Resize()
            {
                int newr = Rows.Count;
                int newc = Cols.Count;
                if ((newr > AllocR) || (newc > AllocC)) // need to enlarge array
                {
                    if (newr > AllocR)
                        AllocR = newr * 2;
                    if (newc > AllocC)
                        AllocC = newc * 2;
                    bool?[][] newarr = new bool?[AllocR][];
                    for (int i = 0; i < AllocR; i++)
                        newarr[i] = new bool?[AllocC];

                    for (int r = 0; r < DataR; r++)
                    {
                        for (int c = 0; c < DataC; c++)
                        {
                            if ((r < newr) && (c < newc))
                                newarr[r][c] = Data[r][c];
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
                                newarr[newR][newC++] = Data[r][c];
                        }
                        newR++;
                    }
                }

                for (int r = 0, newR = 0; r < DataR; r++)
                {
                    if (keepR[r])
                        Rows[newR++] = Rows[r];
                }
                for (int c = 0, newC = 0; c < DataC; c++)
                {
                    if (keepC[c])
                        Cols[newC++] = Cols[c];
                }

                Rows.RemoveRange(countR, Rows.Count - countR);
                Cols.RemoveRange(countC, Cols.Count - countC);

                Data = newarr;
                AllocR = DataR = countR;
                AllocC = DataC = countC;
            }

            public void Set(string row, string col, bool isActor) // isActor = false means guest star
            {
                int r = Rows.IndexOf(row);
                int c = Cols.IndexOf(col);
                if (r == -1)
                    r = AddRow(row);
                if (c == -1)
                    c = AddCol(col);
                Data[r][c] = isActor;
            }

            public void SortCols(bool score) // false->name
            {
                for (int c2 = 0; c2 < (DataC - 1); c2++)
                {
                    int topscore = 0;
                    string topword = "";
                    int maxat = -1;
                    for (int c = c2; c < DataC; c++)
                    {
                        if (score)
                        {
                            int sc = ColScore(c);
                            if ((maxat == -1) || (sc > topscore))
                            {
                                maxat = c;
                                topscore = sc;
                            }
                        }
                        else
                        {
                            if ((maxat == -1) || (Cols[c].CompareTo(topword) < 0))
                            {
                                maxat = c;
                                topword = Cols[c];
                            }
                        }
                    }
                    if (maxat != c2)
                        SwapCols(c2, maxat);
                }
            }

            public void MoveColToTop(string col)
            {
                int n = Cols.IndexOf(col);
                if (n == -1)
                    return;

                for (int r = 0; r < DataR; r++)
                {
                    bool? t = Data[r][n];
                    for (int c = n; c > 0; c--)
                        Data[r][c] = Data[r][c - 1];
                    Data[r][0] = t;
                }

                string t2 = Cols[n];
                for (int c = n; c > 0; c--)
                    Cols[c] = Cols[c - 1];
                Cols[0] = t2;
            }

            public void MoveRowToTop(string row)
            {
                int n = Rows.IndexOf(row);
                if (n == -1)
                    return;

                for (int c = 0; c < DataC; c++)
                {
                    bool? t = Data[n][c];
                    for (int r = n; r > 0; r--)
                        Data[r][c] = Data[r - 1][c];
                    Data[0][c] = t;
                }

                string t2 = Rows[n];
                for (int r = n; r > 0; r--)
                    Rows[r] = Rows[r - 1];
                Rows[0] = t2;
            }

            public void SortRows(bool score) // false->name
            {
                for (int r2 = 0; r2 < (DataR - 1); r2++)
                {
                    int topscore = 0;
                    int maxat = -1;
                    string topword = "";
                    for (int r = r2; r < DataR; r++)
                    {
                        if (score)
                        {
                            int sc = RowScore(r, null);
                            if ((maxat == -1) || (sc > topscore))
                            {
                                maxat = r;
                                topscore = sc;
                            }
                        }
                        else
                        {
                            if ((maxat == -1) || (Rows[r].CompareTo(topword) < 0))
                            {
                                maxat = r;
                                topword = Rows[r];
                            }
                        }
                    }
                    if (maxat != r2)
                        SwapRows(r2, maxat);
                }
            }
        }

        #endregion

        #region Nested type: RotatedText

        public class RotatedText : DevAge.Drawing.VisualElements.TextGDI
        {
            public float Angle;

            public RotatedText(float angle)
            {
                Angle = angle;
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

                    graphics.Graphics.RotateTransform(Angle);
                    graphics.Graphics.TranslateTransform(-height2, 0); //-(area.Y + height2));

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

        #endregion

        #region Nested type: SideClickEvent

        private class SideClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly ActorsGrid G;
            private readonly string Show;

            public SideClickEvent(ActorsGrid g, string show)
            {
                Show = show;
                G = g;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                if (Show == null)
                    G.DoSort();
                else
                    G.ShowToTop(Show);
            }
        }

        #endregion

        #region Nested type: SortColsByCountEvent

        private class SortColsByCountEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly ActorsGrid G;

            public SortColsByCountEvent(ActorsGrid g)
            {
                G = g;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                G.SortColsByCount();
            }
        }

        #endregion

        #region Nested type: SortRowsByCountEvent

        private class SortRowsByCountEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly ActorsGrid G;

            public SortRowsByCountEvent(ActorsGrid g)
            {
                G = g;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                G.SortRowsByCount();
            }
        }

        #endregion

        #region Nested type: TopClickEvent

        private class TopClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly string Actor;
            private readonly ActorsGrid G;

            public TopClickEvent(ActorsGrid g, string act)
            {
                G = g;
                Actor = act;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                G.ActorToTop(Actor);
            }
        }

        #endregion
    }
}
