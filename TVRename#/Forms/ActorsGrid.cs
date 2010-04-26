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
            this.Internal = 0;

            this.InitializeComponent();

            this.mDoc = doc;

            this.BuildData();
            this.DoSort();
        }

        private void BuildData()
        {
            // find actors that have been in more than one thing
            // Dictionary<String^, StringList ^> ^whoInWhat = gcnew Dictionary<String^, StringList ^>;
            TheTVDB db = this.mDoc.GetTVDB(true, "Actors");
            this.TheData = new DataArr(db.GetSeriesDict().Count);
            foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> ser in db.GetSeriesDict())
            {
                SeriesInfo si = ser.Value;
                string actors = si.GetItem("Actors");
                if (!string.IsNullOrEmpty(actors))
                {
                    foreach (string act in actors.Split('|'))
                    {
                        string aa = act.Trim();
                        if (!string.IsNullOrEmpty(aa))
                            this.TheData.Set(si.Name, aa, true);
                    }
                }

                if (this.cbGuestStars.Checked)
                {
                    foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp in si.Seasons)
                    {
                        foreach (Episode ep in kvp.Value.Episodes)
                        {
                            string guest = ep.GetItem("GuestStars");

                            if (!string.IsNullOrEmpty(guest))
                            {
                                foreach (string g in guest.Split('|'))
                                {
                                    string aa = g.Trim();
                                    if (!string.IsNullOrEmpty(aa))
                                        this.TheData.Set(si.Name, aa, false);
                                }
                            }
                        }
                    }
                }
            }

            db.Unlock("Actors");
            this.TheData.RemoveEmpties();
        }

        private void SortByName()
        {
            this.Internal++;
            this.rbName.Checked = true;
            this.Internal--;
            this.TheData.SortRows(false);
            this.TheData.SortCols(false);
        }

        private void SortByTotals()
        {
            this.Internal++;
            this.rbTotals.Checked = true;
            this.Internal--;
            this.TheData.SortRows(true);
            this.TheData.SortCols(true);
        }

        private void SortRowsByCount()
        {
            this.Internal++;
            this.rbCustom.Checked = true;
            this.Internal--;
            this.TheData.SortRows(true);
            this.FillGrid();
        }

        private void SortColsByCount()
        {
            this.Internal++;
            this.rbCustom.Checked = true;
            this.Internal--;
            this.TheData.SortCols(true);
            this.FillGrid();
        }

        private void ActorToTop(string a)
        {
            this.Internal++;
            this.rbCustom.Checked = true;
            this.Internal--;

            this.TheData.MoveColToTop(a);

            // also move the shows they've been in to the top, too
            int c = this.TheData.Cols.IndexOf(a);
            if (c != 0)
                return; // uh oh!
            int end = 0;
            for (int r = this.TheData.DataR - 1; r >= end; r--)
            {
                if (this.TheData.Data[r][0].HasValue)
                {
                    this.TheData.MoveRowToTop(this.TheData.Rows[r++]);
                    end++;
                }
            }
            this.FillGrid();
        }

        private void ShowToTop(string s)
        {
            this.Internal++;
            this.rbCustom.Checked = true;
            this.Internal--;

            this.TheData.MoveRowToTop(s);

            // also move the actors in this show to the top, too
            int r = this.TheData.Rows.IndexOf(s);
            if (r != 0)
                return; // uh oh!
            int end = 0;
            for (int c = this.TheData.DataC - 1; c >= end; c--)
            {
                if (this.TheData.Data[0][c].HasValue)
                {
                    this.TheData.MoveColToTop(this.TheData.Cols[c++]);
                    end++;
                }
            }

            this.FillGrid();
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

            this.grid1.Columns.Clear();
            this.grid1.Rows.Clear();

            int rows = this.TheData.DataR + 2; // title and total
            int cols = this.TheData.DataC + 2;

            this.grid1.ColumnsCount = cols;
            this.grid1.RowsCount = rows;
            this.grid1.FixedColumns = 1;
            this.grid1.FixedRows = 1;
            this.grid1.Selection.EnableMultiSelection = false;

            for (int i = 0; i < cols; i++)
            {
                this.grid1.Columns[i].AutoSizeMode = i == 0 ? SourceGrid.AutoSizeMode.Default : SourceGrid.AutoSizeMode.MinimumSize;
                if (i > 0)
                    this.grid1.Columns[i].Width = 24;
            }

            this.grid1.Rows[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize;
            this.grid1.Rows[0].Height = 100;

            ColumnHeader h = new SourceGrid.Cells.ColumnHeader("Show")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false
            };

            this.grid1[0, 0] = h;
            this.grid1[0, 0].View = topleftTitleModel;
            this.grid1[0, 0].AddController(new SideClickEvent(this, null)); // default sort

            for (int c = 0; c < this.TheData.DataC; c++)
            {
                h = new SourceGrid.Cells.ColumnHeader(this.TheData.Cols[c])
                {
                    AutomaticSortEnabled = false,
                    ResizeEnabled = false
                }; // "<A HREF=\"http://www.imdb.com/find?s=nm&q="+kvp->Key+"\">"+kvp->Key+"</a>");

                // h->AddController(sortableController);
                // h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0); // TODO: remove?
                this.grid1[0, c + 1] = h;
                this.grid1[0, c + 1].View = colTitleModel;
                this.grid1[0, c + 1].AddController(new TopClickEvent(this, this.TheData.Cols[c]));
            }

            int totalCol = this.grid1.ColumnsCount - 1;
            h = new SourceGrid.Cells.ColumnHeader("Totals")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false
            };
            //h->AddController(sortableController);
            // h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0);
            this.grid1.Columns[totalCol].Width = 48;
            this.grid1[0, totalCol] = h;
            this.grid1[0, totalCol].View = colTitleModel;
            this.grid1[0, totalCol].AddController(new SortRowsByCountEvent(this));

            for (int r = 0; r < this.TheData.DataR; r++)
            {
                this.grid1[r + 1, 0] = new SourceGrid.Cells.RowHeader(this.TheData.Rows[r])
                {
                    ResizeEnabled = false
                };
                this.grid1[r + 1, 0].AddController(new SideClickEvent(this, this.TheData.Rows[r]));
            }

            this.grid1[this.TheData.DataR + 1, 0] = new SourceGrid.Cells.RowHeader("Totals")
            {
                ResizeEnabled = false
            };
            this.grid1[this.TheData.DataR + 1, 0].AddController(new SortColsByCountEvent(this));

            for (int c = 0; c < this.TheData.DataC; c++)
            {
                for (int r = 0; r < this.TheData.DataR; r++)
                {
                    if (this.TheData.Data[r][c].HasValue)
                    {
                        this.grid1[r + 1, c + 1] = new SourceGrid.Cells.Cell("Y")
                        {
                            View = this.TheData.Data[r][c].Value ? isActorModel : isGuestModel
                        };
                        this.grid1[r + 1, c + 1].AddController(new CellClickEvent(this.TheData.Cols[c], this.TheData.Rows[r]));
                    }
                    else
                        this.grid1[r + 1, c + 1] = new SourceGrid.Cells.Cell("");
                }
            }

            for (int c = 0; c < this.TheData.DataC; c++)
                this.grid1[rows - 1, c + 1] = new SourceGrid.Cells.Cell(this.TheData.ColScore(c));

            for (int r = 0; r < this.TheData.DataR; r++)
                this.grid1[r + 1, cols - 1] = new SourceGrid.Cells.Cell(this.TheData.RowScore(r, null));

            this.grid1[this.TheData.DataR + 1, this.TheData.DataC + 1] = new SourceGrid.Cells.Cell("");

            this.grid1.AutoSizeCells();
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void cbGuestStars_CheckedChanged(object sender, System.EventArgs e)
        {
            this.cbGuestStars.Update();
            this.BuildData();
            this.DoSort();
        }

        private void bnSave_Click(object sender, System.EventArgs e)
        {
            this.saveFile.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
            if (this.saveFile.ShowDialog() != DialogResult.OK)
                return;

            SourceGrid.Exporter.Image image = new SourceGrid.Exporter.Image();
            Bitmap b = image.Export(this.grid1, this.grid1.CompleteRange);
            b.Save(this.saveFile.FileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void DoSort()
        {
            if (this.rbTotals.Checked)
                this.SortByTotals();
            else
                this.SortByName(); // will check name for us, too
            this.FillGrid();
        }

        private void rbName_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.Internal != 0)
                return;
            this.DoSort();
        }

        private void rbTotals_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.Internal != 0)
                return;
            this.DoSort();
        }

        #region Nested type: CellClickEvent

        private class CellClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly string Show;
            private readonly string Who;

            public CellClickEvent(string who, string show)
            {
                this.Who = who;
                this.Show = show;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                TVDoc.SysOpen("http://www.imdb.com/find?s=nm&q=" + this.Who);
            }
        }

        #endregion

        #region Nested type: DataArr

        private class DataArr
        {
            private int AllocC;
            private int AllocR;
            public readonly StringList Cols;
            public bool?[][] Data;
            public int DataC;
            public int DataR;
            public readonly StringList Rows;

            public DataArr(int rowCountPreAlloc)
            {
                this.Rows = new StringList();
                this.Cols = new StringList();
                this.AllocR = rowCountPreAlloc;
                this.AllocC = rowCountPreAlloc * 10;
                this.Data = new bool?[this.AllocR][];
                for (int i = 0; i < this.AllocR; i++)
                    this.Data[i] = new bool?[this.AllocC];
                this.DataR = this.DataC = 0;
            }

            private void SwapCols(int c1, int c2)
            {
                for (int r = 0; r < this.DataR; r++)
                {
                    bool? t = this.Data[r][c2];
                    this.Data[r][c2] = this.Data[r][c1];
                    this.Data[r][c1] = t;
                }
                string t2 = this.Cols[c1];
                this.Cols[c1] = this.Cols[c2];
                this.Cols[c2] = t2;
            }

            private void SwapRows(int r1, int r2)
            {
                for (int c = 0; c < this.DataC; c++)
                {
                    bool? t = this.Data[r2][c];
                    this.Data[r2][c] = this.Data[r1][c];
                    this.Data[r1][c] = t;
                }
                string t2 = this.Rows[r1];
                this.Rows[r1] = this.Rows[r2];
                this.Rows[r2] = t2;
            }

            public int RowScore(int r, bool[] onlyCols)
            {
                int t = 0;
                for (int c = 0; c < this.DataC; c++)
                {
                    if (((this.Data[r][c] != null) && ((onlyCols == null) || onlyCols[c])) && this.Data[r][c].HasValue)
                        t++;
                }
                return t;
            }

            public int ColScore(int c)
            {
                int t = 0;
                for (int r = 0; r < this.DataR; r++)
                {
                    if ((this.Data[r][c] != null) && this.Data[r][c].HasValue)
                        t++;
                }
                return t;
            }

            private void Resize()
            {
                int newr = this.Rows.Count;
                int newc = this.Cols.Count;
                if ((newr > this.AllocR) || (newc > this.AllocC)) // need to enlarge array
                {
                    if (newr > this.AllocR)
                        this.AllocR = newr * 2;
                    if (newc > this.AllocC)
                        this.AllocC = newc * 2;
                    bool?[][] newarr = new bool?[this.AllocR][];
                    for (int i = 0; i < this.AllocR; i++)
                        newarr[i] = new bool?[this.AllocC];

                    for (int r = 0; r < this.DataR; r++)
                    {
                        for (int c = 0; c < this.DataC; c++)
                        {
                            if ((r < newr) && (c < newc))
                                newarr[r][c] = this.Data[r][c];
                        }
                    }
                    this.Data = newarr;
                }
                this.DataR = newr;
                this.DataC = newc;
            }

            private int AddRow(string name)
            {
                this.Rows.Add(name);
                this.Resize();
                return this.Rows.Count - 1;
            }

            private int AddCol(string name)
            {
                this.Cols.Add(name);
                this.Resize();
                return this.Cols.Count - 1;
            }

            public void RemoveEmpties()
            {
                bool[] keepR = new bool[this.DataR];
                bool[] keepC = new bool[this.DataC];
                // trim by actor 
                int countR = 0;
                int countC = 0;

                for (int c = 0; c < this.DataC; c++)
                {
                    if (this.ColScore(c) >= 2)
                    {
                        keepC[c] = true;
                        countC++;
                    }
                }
                for (int r = 0; r < this.DataR; r++)
                {
                    if (this.RowScore(r, keepC) >= 1)
                    {
                        keepR[r] = true;
                        countR++;
                    }
                }

                bool?[][] newarr = new bool?[countR][];
                for (int r = 0, newR = 0; r < this.DataR; r++)
                {
                    if (keepR[r])
                    {
                        newarr[newR] = new bool?[countC];
                        for (int c = 0, newC = 0; c < this.DataC; c++)
                        {
                            if (keepR[r] && keepC[c])
                                newarr[newR][newC++] = this.Data[r][c];
                        }
                        newR++;
                    }
                }

                for (int r = 0, newR = 0; r < this.DataR; r++)
                {
                    if (keepR[r])
                        this.Rows[newR++] = this.Rows[r];
                }
                for (int c = 0, newC = 0; c < this.DataC; c++)
                {
                    if (keepC[c])
                        this.Cols[newC++] = this.Cols[c];
                }

                this.Rows.RemoveRange(countR, this.Rows.Count - countR);
                this.Cols.RemoveRange(countC, this.Cols.Count - countC);

                this.Data = newarr;
                this.AllocR = this.DataR = countR;
                this.AllocC = this.DataC = countC;
            }

            public void Set(string row, string col, bool isActor) // isActor = false means guest star
            {
                int r = this.Rows.IndexOf(row);
                int c = this.Cols.IndexOf(col);
                if (r == -1)
                    r = this.AddRow(row);
                if (c == -1)
                    c = this.AddCol(col);
                this.Data[r][c] = isActor;
            }

            public void SortCols(bool score) // false->name
            {
                for (int c2 = 0; c2 < (this.DataC - 1); c2++)
                {
                    int topscore = 0;
                    string topword = "";
                    int maxat = -1;
                    for (int c = c2; c < this.DataC; c++)
                    {
                        if (score)
                        {
                            int sc = this.ColScore(c);
                            if ((maxat == -1) || (sc > topscore))
                            {
                                maxat = c;
                                topscore = sc;
                            }
                        }
                        else
                        {
                            if ((maxat == -1) || (this.Cols[c].CompareTo(topword) < 0))
                            {
                                maxat = c;
                                topword = this.Cols[c];
                            }
                        }
                    }
                    if (maxat != c2)
                        this.SwapCols(c2, maxat);
                }
            }

            public void MoveColToTop(string col)
            {
                int n = this.Cols.IndexOf(col);
                if (n == -1)
                    return;

                for (int r = 0; r < this.DataR; r++)
                {
                    bool? t = this.Data[r][n];
                    for (int c = n; c > 0; c--)
                        this.Data[r][c] = this.Data[r][c - 1];
                    this.Data[r][0] = t;
                }

                string t2 = this.Cols[n];
                for (int c = n; c > 0; c--)
                    this.Cols[c] = this.Cols[c - 1];
                this.Cols[0] = t2;
            }

            public void MoveRowToTop(string row)
            {
                int n = this.Rows.IndexOf(row);
                if (n == -1)
                    return;

                for (int c = 0; c < this.DataC; c++)
                {
                    bool? t = this.Data[n][c];
                    for (int r = n; r > 0; r--)
                        this.Data[r][c] = this.Data[r - 1][c];
                    this.Data[0][c] = t;
                }

                string t2 = this.Rows[n];
                for (int r = n; r > 0; r--)
                    this.Rows[r] = this.Rows[r - 1];
                this.Rows[0] = t2;
            }

            public void SortRows(bool score) // false->name
            {
                for (int r2 = 0; r2 < (this.DataR - 1); r2++)
                {
                    int topscore = 0;
                    int maxat = -1;
                    string topword = "";
                    for (int r = r2; r < this.DataR; r++)
                    {
                        if (score)
                        {
                            int sc = this.RowScore(r, null);
                            if ((maxat == -1) || (sc > topscore))
                            {
                                maxat = r;
                                topscore = sc;
                            }
                        }
                        else
                        {
                            if ((maxat == -1) || (this.Rows[r].CompareTo(topword) < 0))
                            {
                                maxat = r;
                                topword = this.Rows[r];
                            }
                        }
                    }
                    if (maxat != r2)
                        this.SwapRows(r2, maxat);
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
                this.Angle = angle;
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

                    graphics.Graphics.RotateTransform(this.Angle);
                    graphics.Graphics.TranslateTransform(-height2, 0); //-(area.Y + height2));

                    this.StringFormat.Alignment = StringAlignment.Near;
                    this.StringFormat.LineAlignment = StringAlignment.Center;
                    graphics.Graphics.DrawString(this.Value, this.Font, graphics.BrushsCache.GetBrush(this.ForeColor), 0, 0, this.StringFormat);
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
                this.Show = show;
                this.G = g;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                if (this.Show == null)
                    this.G.DoSort();
                else
                    this.G.ShowToTop(this.Show);
            }
        }

        #endregion

        #region Nested type: SortColsByCountEvent

        private class SortColsByCountEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly ActorsGrid G;

            public SortColsByCountEvent(ActorsGrid g)
            {
                this.G = g;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                this.G.SortColsByCount();
            }
        }

        #endregion

        #region Nested type: SortRowsByCountEvent

        private class SortRowsByCountEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly ActorsGrid G;

            public SortRowsByCountEvent(ActorsGrid g)
            {
                this.G = g;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                this.G.SortRowsByCount();
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
                this.G = g;
                this.Actor = act;
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                this.G.ActorToTop(this.Actor);
            }
        }

        #endregion
    }
}