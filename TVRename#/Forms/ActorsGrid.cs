// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using DevAge.Drawing;
using DevAge.Drawing.VisualElements;
using SourceGrid;
using SourceGrid.Cells.Controllers;
using SourceGrid.Cells.Views;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;
using ContentAlignment = DevAge.Drawing.ContentAlignment;
using Image = SourceGrid.Exporter.Image;
using RowHeader = SourceGrid.Cells.RowHeader;

namespace TVRename
{
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
        private int _internal;
        private DataArr _theData;
        private TVDoc _mDoc;

        public ActorsGrid(TVDoc doc)
        {
            _internal = 0;

            InitializeComponent();

            _mDoc = doc;

            BuildData();
            DoSort();
        }

        private void BuildData()
        {
            // find actors that have been in more than one thing
            // Dictionary<String^, List<String> ^> ^whoInWhat = gcnew Dictionary<String^, List<String> ^>;
            TheTVDB.Instance.GetLock("Actors");
            _theData = new DataArr(TheTVDB.Instance.GetSeriesDict().Count);
            foreach (KeyValuePair<int, SeriesInfo> ser in TheTVDB.Instance.GetSeriesDict())
            {
                SeriesInfo si = ser.Value;
                foreach (string act in si.GetActors())
                {
                    string aa = act.Trim();
                    if (!string.IsNullOrEmpty(aa))
                        _theData.Set(si.Name, aa, true);
                }

                if (cbGuestStars.Checked)
                {
                    foreach (KeyValuePair<int, Season> kvp in si.Seasons)
                    {
                        foreach (Episode ep in kvp.Value.Episodes)
                        {
                                foreach (string g in ep.GetGuestStars())
                                {
                                    string aa = g.Trim();
                                    if (!string.IsNullOrEmpty(aa))
                                        _theData.Set(si.Name, aa, false);
                                }
                        }
                    }
                }
            }

            TheTVDB.Instance.Unlock("Actors");
            _theData.RemoveEmpties();
        }

        private void SortByName()
        {
            _internal++;
            rbName.Checked = true;
            _internal--;
            _theData.SortRows(false);
            _theData.SortCols(false);
        }

        private void SortByTotals()
        {
            _internal++;
            rbTotals.Checked = true;
            _internal--;
            _theData.SortRows(true);
            _theData.SortCols(true);
        }

        private void SortRowsByCount()
        {
            _internal++;
            rbCustom.Checked = true;
            _internal--;
            _theData.SortRows(true);
            FillGrid();
        }

        private void SortColsByCount()
        {
            _internal++;
            rbCustom.Checked = true;
            _internal--;
            _theData.SortCols(true);
            FillGrid();
        }

        private void ActorToTop(string a)
        {
            _internal++;
            rbCustom.Checked = true;
            _internal--;

            _theData.MoveColToTop(a);

            // also move the shows they've been in to the top, too
            int c = _theData.Cols.IndexOf(a);
            if (c != 0)
                return; // uh oh!
            int end = 0;
            for (int r = _theData.DataR - 1; r >= end; r--)
            {
                if (_theData.Data[r][0].HasValue)
                {
                    _theData.MoveRowToTop(_theData.Rows[r++]);
                    end++;
                }
            }
            FillGrid();
        }

        private void ShowToTop(string s)
        {
            _internal++;
            rbCustom.Checked = true;
            _internal--;

            _theData.MoveRowToTop(s);

            // also move the actors in this show to the top, too
            int r = _theData.Rows.IndexOf(s);
            if (r != 0)
                return; // uh oh!
            int end = 0;
            for (int c = _theData.DataC - 1; c >= end; c--)
            {
                if (_theData.Data[0][c].HasValue)
                {
                    _theData.MoveColToTop(_theData.Cols[c++]);
                    end++;
                }
            }

            FillGrid();
        }

        private void FillGrid()
        {
            Cell colTitleModel = new Cell
            {
                ElementText = new RotatedText(-90.0f),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = ContentAlignment.BottomCenter
            };

            Cell topleftTitleModel = new Cell
            {
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = ContentAlignment.BottomLeft
            };

            Cell isActorModel = new Cell
            {
                BackColor = Color.Green,
                ForeColor = Color.Green,
                TextAlignment = ContentAlignment.MiddleLeft
            };

            Cell isGuestModel = new Cell
            {
                BackColor = Color.LightGreen,
                ForeColor = Color.LightGreen,
                TextAlignment = ContentAlignment.MiddleLeft
            };

            grid1.Columns.Clear();
            grid1.Rows.Clear();

            int rows = _theData.DataR + 2; // title and total
            int cols = _theData.DataC + 2;

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

            for (int c = 0; c < _theData.DataC; c++)
            {
                h = new ColumnHeader(_theData.Cols[c])
                {
                    AutomaticSortEnabled = false,
                    ResizeEnabled = false
                }; // "<A HREF=\"http://www.imdb.com/find?s=nm&q="+kvp->Key+"\">"+kvp->Key+"</a>");

                // h->AddController(sortableController);
                // h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0); // TODO: remove?
                grid1[0, c + 1] = h;
                grid1[0, c + 1].View = colTitleModel;
                grid1[0, c + 1].AddController(new TopClickEvent(this, _theData.Cols[c]));
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

            for (int r = 0; r < _theData.DataR; r++)
            {
                grid1[r + 1, 0] = new RowHeader(_theData.Rows[r])
                {
                    ResizeEnabled = false
                };
                grid1[r + 1, 0].AddController(new SideClickEvent(this, _theData.Rows[r]));
            }

            grid1[_theData.DataR + 1, 0] = new RowHeader("Totals")
            {
                ResizeEnabled = false
            };
            grid1[_theData.DataR + 1, 0].AddController(new SortColsByCountEvent(this));

            for (int c = 0; c < _theData.DataC; c++)
            {
                for (int r = 0; r < _theData.DataR; r++)
                {
                    if (_theData.Data[r][c].HasValue)
                    {
                        grid1[r + 1, c + 1] = new SourceGrid.Cells.Cell("Y")
                        {
                            View = _theData.Data[r][c].Value ? isActorModel : isGuestModel
                        };
                        grid1[r + 1, c + 1].AddController(new CellClickEvent(_theData.Cols[c], _theData.Rows[r]));
                    }
                    else
                        grid1[r + 1, c + 1] = new SourceGrid.Cells.Cell("");
                }
            }

            for (int c = 0; c < _theData.DataC; c++)
                grid1[rows - 1, c + 1] = new SourceGrid.Cells.Cell(_theData.ColScore(c));

            for (int r = 0; r < _theData.DataR; r++)
                grid1[r + 1, cols - 1] = new SourceGrid.Cells.Cell(_theData.RowScore(r, null));

            grid1[_theData.DataR + 1, _theData.DataC + 1] = new SourceGrid.Cells.Cell("");

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

            Image image = new Image();
            Bitmap b = image.Export(grid1, grid1.CompleteRange);
            b.Save(saveFile.FileName, ImageFormat.Png);
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
            if (_internal != 0)
                return;
            DoSort();
        }

        private void rbTotals_CheckedChanged(object sender, EventArgs e)
        {
            if (_internal != 0)
                return;
            DoSort();
        }

        #region Nested type: CellClickEvent

        private class CellClickEvent : ControllerBase
        {
            private readonly string _show;
            private readonly string _who;

            public CellClickEvent(string who, string show)
            {
                _who = who;
                _show = show;
            }

            public override void OnClick(CellContext sender, EventArgs e)
            {
                Helpers.SysOpen("http://www.imdb.com/find?s=nm&q=" + _who);
            }
        }

        #endregion

        #region Nested type: DataArr

        private class DataArr
        {
            private int _allocC;
            private int _allocR;
            public readonly List<string> Cols;
            public bool?[][] Data;
            public int DataC;
            public int DataR;
            public readonly List<string> Rows;

            public DataArr(int rowCountPreAlloc)
            {
                Rows = new List<String>();
                Cols = new List<String>();
                _allocR = rowCountPreAlloc;
                _allocC = rowCountPreAlloc * 10;
                Data = new bool?[_allocR][];
                for (int i = 0; i < _allocR; i++)
                    Data[i] = new bool?[_allocC];
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
                if ((newr > _allocR) || (newc > _allocC)) // need to enlarge array
                {
                    if (newr > _allocR)
                        _allocR = newr * 2;
                    if (newc > _allocC)
                        _allocC = newc * 2;
                    bool?[][] newarr = new bool?[_allocR][];
                    for (int i = 0; i < _allocR; i++)
                        newarr[i] = new bool?[_allocC];

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
                _allocR = DataR = countR;
                _allocC = DataC = countC;
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

        public class RotatedText : TextGDI
        {
            public float Angle;

            public RotatedText(float angle)
            {
                Angle = angle;
            }

            protected override void OnDraw(GraphicsCache graphics, RectangleF area)
            {
                GraphicsState state = graphics.Graphics.Save();
                try
                {
                    float width2 = area.Width / 2;
                    float height2 = area.Height / 2;

                    //For a better drawing use the clear type rendering
                    graphics.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

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

        private class SideClickEvent : ControllerBase
        {
            private readonly ActorsGrid _g;
            private readonly string _show;

            public SideClickEvent(ActorsGrid g, string show)
            {
                _show = show;
                _g = g;
            }

            public override void OnClick(CellContext sender, EventArgs e)
            {
                if (_show == null)
                    _g.DoSort();
                else
                    _g.ShowToTop(_show);
            }
        }

        #endregion

        #region Nested type: SortColsByCountEvent

        private class SortColsByCountEvent : ControllerBase
        {
            private readonly ActorsGrid _g;

            public SortColsByCountEvent(ActorsGrid g)
            {
                _g = g;
            }

            public override void OnClick(CellContext sender, EventArgs e)
            {
                _g.SortColsByCount();
            }
        }

        #endregion

        #region Nested type: SortRowsByCountEvent

        private class SortRowsByCountEvent : ControllerBase
        {
            private readonly ActorsGrid _g;

            public SortRowsByCountEvent(ActorsGrid g)
            {
                _g = g;
            }

            public override void OnClick(CellContext sender, EventArgs e)
            {
                _g.SortRowsByCount();
            }
        }

        #endregion

        #region Nested type: TopClickEvent

        private class TopClickEvent : ControllerBase
        {
            private readonly string _actor;
            private readonly ActorsGrid _g;

            public TopClickEvent(ActorsGrid g, string act)
            {
                _g = g;
                _actor = act;
            }

            public override void OnClick(CellContext sender, EventArgs e)
            {
                _g.ActorToTop(_actor);
            }
        }

        #endregion
    }
}
