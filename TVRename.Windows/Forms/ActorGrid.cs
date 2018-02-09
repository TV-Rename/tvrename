using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SourceGrid.Cells.Views;
using TVRename.Windows.Configuration;

namespace TVRename.Windows.Forms
{
    public partial class ActorGrid : Form
    {
        public ActorGrid()
        {
            InitializeComponent();
        }

        private void ActorGrid_Load(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void FillGrid()
        {
            // Actor data
            Dictionary<int, List<string>> data = Settings.Instance.Shows.ToDictionary(s => s.TVDBId, s => s.Metadata.Actors);

            // Cell views
            Cell colTitleCell = new Cell
            {
                ElementText = new RotatedText(-90.0f),
                TextAlignment = DevAge.Drawing.ContentAlignment.BottomCenter
            };

            // Grid
            this.grid.Columns.Clear();
            this.grid.Rows.Clear();

            this.grid.ColumnsCount = data.Values.Sum(a => a.Count) + 2; // title and total
            this.grid.RowsCount = data.Count + 2; // title and total
            this.grid.Selection.EnableMultiSelection = false;

            // First row
            this.grid.Rows[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize;
            this.grid.Rows[0].Height = 100;

            // First cell
            this.grid[0, 0] = new SourceGrid.Cells.ColumnHeader("Show")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false,
                View = new Cell
                {
                    TextAlignment = DevAge.Drawing.ContentAlignment.BottomLeft
                }
            };

            // Columns
            int c = 1;
            foreach (string actor in data.Values.SelectMany(a => a).Distinct())
            {
                this.grid[0, c++] = new SourceGrid.Cells.ColumnHeader(actor)
                {
                    AutomaticSortEnabled = false,
                    ResizeEnabled = false,
                    View = colTitleCell
                };
            }

            // Last column
            this.grid[0, this.grid.ColumnsCount - 1] = new SourceGrid.Cells.ColumnHeader("Totals")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false,
                View = new Cell
                {
                    TextAlignment = DevAge.Drawing.ContentAlignment.BottomRight
                }
            };

            // Rows
            int r = 1;
            foreach (int id in data.Keys)
            {
                this.grid[r++, 0] = new SourceGrid.Cells.RowHeader(Settings.Instance.Shows.First(s => s.TVDBId == id).Metadata.Name)
                {
                    ResizeEnabled = false
                };
            }

            // Last row
            this.grid[this.grid.RowsCount - 1, 0] = new SourceGrid.Cells.RowHeader("Totals")
            {
                ResizeEnabled = false
            };

            // Cells
            for (c = 1; c < this.grid.ColumnsCount - 1; c++)
            {
                for (r = 1; r < this.grid.RowsCount - 1; r++)
                {
                    this.grid[r, c] = new SourceGrid.Cells.Cell(string.Empty); // TODO
                }
            }

            // Last row totals
            for (c = 1; c < this.grid.ColumnsCount; c++)
            {
                this.grid[this.grid.RowsCount - 1, c] = new SourceGrid.Cells.Cell(0); // TODO
            }

            // Last column totals
            for (r = 1; r < this.grid.RowsCount; r++)
            {
                this.grid[r, this.grid.ColumnsCount - 1] = new SourceGrid.Cells.Cell(0); // TODO
            }

            // Bottom right cell
            this.grid[this.grid.RowsCount - 1, this.grid.ColumnsCount - 1] = new SourceGrid.Cells.Cell(string.Empty);

            this.grid.AutoSizeCells();
        }

        public class RotatedText : DevAge.Drawing.VisualElements.TextGDI
        {
            public float Angle { get; set; }

            public RotatedText(float angle = 90)
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

                    // For a better drawing use the clear type rendering
                    graphics.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    // Move the origin to the center of the cell (for a more easy rotation)
                    graphics.Graphics.TranslateTransform(area.X + width2, area.Y + height2);

                    graphics.Graphics.RotateTransform(this.Angle);
                    graphics.Graphics.TranslateTransform(-height2, 0);

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
    }
}
