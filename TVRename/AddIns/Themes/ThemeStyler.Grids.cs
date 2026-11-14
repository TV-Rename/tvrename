//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Runtime.CompilerServices;
using SourceGrid.Cells;
using SourceGrid.Cells.Views;

namespace TVRename.AddIns.Themes;

internal static partial class ThemeStyler
{
    /// <summary>
    /// SourceGrid grids (Show Summary, Actors grid) are filled in code after their window opens, cell by cell, with
    /// cell styles chosen for the light look: standard headers that are always drawn light, and black or white text.
    /// While a grid is shown, its cells are checked when the application is idle: light headers get a dark header
    /// style and text that cannot be read on its cell's background is made readable (a coloured text keeps its
    /// colour, just lighter or darker). Each cell style is only checked once.
    /// </summary>
    private static class GridStyler
    {
        private static readonly List<WeakReference<SourceGrid.Grid>> Grids = [];
        private static readonly ConditionalWeakTable<IView, object> CheckedViews = new();
        private static readonly ConditionalWeakTable<IView, IView> HeaderReplacements = new();
        private static long nextSweep;
        private const int SweepIntervalMs = 300;

        public static void Track(SourceGrid.GridVirtual grid)
        {
            if (grid is SourceGrid.Grid g)
            {
                Grids.Add(new WeakReference<SourceGrid.Grid>(g));
            }
        }

        public static void Sweep()
        {
            long now = Environment.TickCount64;
            if (now < nextSweep || Grids.Count == 0)
            {
                return;
            }

            nextSweep = now + SweepIntervalMs;
            Grids.RemoveAll(r => !r.TryGetTarget(out SourceGrid.Grid? g) || g.IsDisposed);

            foreach (WeakReference<SourceGrid.Grid> reference in Grids)
            {
                if (reference.TryGetTarget(out SourceGrid.Grid? grid) && grid.Visible)
                {
                    try
                    {
                        if (StyleCells(grid))
                        {
                            grid.Invalidate(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"Theme add-in: could not style grid {grid.Name}");
                    }
                }
            }
        }

        private static bool StyleCells(SourceGrid.Grid grid)
        {
            bool changed = false;
            Color gridBack = grid.BackColor;

            for (int r = 0; r < grid.RowsCount; r++)
            {
                for (int c = 0; c < grid.ColumnsCount; c++)
                {
                    if (grid[r, c] is not ICell cell || cell.View is not { } view)
                    {
                        continue;
                    }

                    if (HeaderReplacements.TryGetValue(view, out IView? replacement))
                    {
                        cell.View = replacement;
                        changed = true;
                        continue;
                    }

                    if (CheckedViews.TryGetValue(view, out _))
                    {
                        continue;
                    }

                    if (view is SourceGrid.Cells.Views.Header header)
                    {
                        IView dark = DarkHeaderFor(header);
                        HeaderReplacements.Add(view, dark);
                        CheckedViews.AddOrUpdate(dark, true);
                        cell.View = dark;
                        changed = true;
                        continue;
                    }

                    CheckedViews.Add(view, true);
                    if (view is SourceGrid.Cells.Views.ViewBase plain)
                    {
                        changed |= MakeReadable(plain, gridBack);
                    }
                }
            }

            return changed;
        }

        private static IView DarkHeaderFor(SourceGrid.Cells.Views.Header header)
        {
            return new SourceGrid.Cells.Views.Cell
            {
                BackColor = T.HeaderBackground,
                ForeColor = Readable(IsUsable(header.ForeColor) ? header.ForeColor : T.Text, T.HeaderBackground),
                Font = header.Font,
                TextAlignment = header.TextAlignment,
                WordWrap = header.WordWrap,
                Padding = header.Padding
            };
        }

        private static bool MakeReadable(SourceGrid.Cells.Views.ViewBase view, Color gridBack)
        {
            Color back = IsUsable(view.BackColor) ? view.BackColor : gridBack;
            Color fore = IsUsable(view.ForeColor) ? view.ForeColor : T.ListText;

            // Black is the light look's normal text, so it becomes the theme's normal text (not a dimmer grey,
            // which would look like the grey TV Rename uses on purpose, for example for shows that are not scanned).
            Color wanted = fore.ToArgb() == Color.Black.ToArgb() ? T.ListText : fore;
            Color readable = Readable(wanted, back);
            if (readable.ToArgb() == fore.ToArgb())
            {
                return false;
            }

            view.ForeColor = readable;
            return true;
        }

        private static bool IsUsable(Color colour) => !colour.IsEmpty && colour.A == 255;
    }
}
