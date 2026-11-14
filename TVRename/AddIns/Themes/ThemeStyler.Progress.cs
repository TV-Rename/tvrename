//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Runtime.CompilerServices;

namespace TVRename.AddIns.Themes;

internal static partial class ThemeStyler
{
    /// <summary>
    /// In dark mode Windows draws a progress bar without its visual style: a dark track that disappears into a
    /// dark window and a blue bar. This paints progress bars in the theme's colours instead: a track with a border
    /// that is always visible (also at 0%), and the filled part in the theme's progress colour.
    /// Marquee (busy) progress bars keep Windows' own drawing because they animate themselves.
    /// </summary>
    private sealed class ProgressBarPainter : NativeWindow
    {
        private static readonly ConditionalWeakTable<ProgressBar, ProgressBarPainter> Painters = new();

        private const int PBM_SETRANGE = 0x0401;
        private const int PBM_SETPOS = 0x0402;
        private const int PBM_DELTAPOS = 0x0403;
        private const int PBM_SETSTEP = 0x0404;
        private const int PBM_STEPIT = 0x0405;
        private const int PBM_SETRANGE32 = 0x0406;
        private const int PBM_SETSTATE = 0x0410;

        private readonly ProgressBar bar;

        public static void Attach(ProgressBar bar)
        {
            if (!Painters.TryGetValue(bar, out _))
            {
                Painters.Add(bar, new ProgressBarPainter(bar));
            }
        }

        private ProgressBarPainter(ProgressBar bar)
        {
            this.bar = bar;
            bar.HandleCreated += (_, _) => AssignTo();
            if (bar.IsHandleCreated)
            {
                AssignTo();
            }
        }

        private void AssignTo()
        {
            if (Handle != bar.Handle)
            {
                AssignHandle(bar.Handle);
            }

            bar.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            if (bar.Style == ProgressBarStyle.Marquee)
            {
                base.WndProc(ref m);
                return;
            }

            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    m.Result = 1;
                    return;

                case WM_PAINT:
                    HandlePaint(ref m, Paint);
                    return;

                case PBM_SETRANGE:
                case PBM_SETPOS:
                case PBM_DELTAPOS:
                case PBM_SETSTEP:
                case PBM_STEPIT:
                case PBM_SETRANGE32:
                case PBM_SETSTATE:
                    // The control may draw straight away when its value changes; make sure ours is what shows.
                    base.WndProc(ref m);
                    InvalidateRect(m.HWnd, IntPtr.Zero, false);
                    return;

                default:
                    base.WndProc(ref m);
                    return;
            }
        }

        private void Paint(Graphics g)
        {
            try
            {
                Rectangle area = bar.ClientRectangle;
                if (area.Width <= 0 || area.Height <= 0)
                {
                    return;
                }

                using (SolidBrush track = new(T.ProgressTrack))
                {
                    g.FillRectangle(track, area);
                }

                int range = bar.Maximum - bar.Minimum;
                double fraction = range <= 0 ? 0 : Math.Clamp((bar.Value - bar.Minimum) / (double)range, 0, 1);
                Rectangle inner = Rectangle.Inflate(area, -1, -1);
                int filled = (int)Math.Round(inner.Width * fraction);
                if (filled > 0 && inner.Height > 0)
                {
                    Rectangle fill = bar.RightToLeft == RightToLeft.Yes && bar.RightToLeftLayout
                        ? new Rectangle(inner.Right - filled, inner.Top, filled, inner.Height)
                        : new Rectangle(inner.Left, inner.Top, filled, inner.Height);
                    using SolidBrush brush = new(T.ProgressBar);
                    g.FillRectangle(brush, fill);
                }

                using Pen border = new(T.Border);
                g.DrawRectangle(border, area.X, area.Y, area.Width - 1, area.Height - 1);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Theme add-in: could not paint progress bar {bar.Name}");
            }
        }
    }
}
