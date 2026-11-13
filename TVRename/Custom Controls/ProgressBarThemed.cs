//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using TVRename.Forms;

namespace TVRename;

// In dark mode the track disappears into the window, so paint it in the theme's colours (not marquee bars)
public class ProgressBarThemed : ProgressBar
{
    private const int PBM_SETRANGE = 0x0401;
    private const int PBM_SETPOS = 0x0402;
    private const int PBM_DELTAPOS = 0x0403;
    private const int PBM_SETSTEP = 0x0404;
    private const int PBM_STEPIT = 0x0405;
    private const int PBM_SETRANGE32 = 0x0406;
    private const int PBM_SETSTATE = 0x0410;

    private static Theme T => ThemeHelper.T;

    protected override void WndProc(ref Message m)
    {
        if (!ThemeManager.IsThemed || Style == ProgressBarStyle.Marquee)
        {
            base.WndProc(ref m);
            return;
        }

        switch (m.Msg)
        {
            case ThemeHelper.WM_ERASEBKGND:
                m.Result = 1;
                return;

            case ThemeHelper.WM_PAINT:
                ThemeHelper.HandlePaint(ref m, PaintBar);
                return;

            case PBM_SETRANGE:
            case PBM_SETPOS:
            case PBM_DELTAPOS:
            case PBM_SETSTEP:
            case PBM_STEPIT:
            case PBM_SETRANGE32:
            case PBM_SETSTATE:
                // The control may draw straight away when its value changes, so make sure ours is what shows
                base.WndProc(ref m);
                ThemeHelper.InvalidateRect(m.HWnd, IntPtr.Zero, false);
                return;

            default:
                base.WndProc(ref m);
                return;
        }
    }

    private void PaintBar(Graphics g)
    {
        try
        {
            Rectangle area = ClientRectangle;
            if (area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            using (SolidBrush track = new(T.ProgressTrack))
            {
                g.FillRectangle(track, area);
            }

            int range = Maximum - Minimum;
            double fraction = range <= 0 ? 0 : Math.Clamp((Value - Minimum) / (double)range, 0, 1);
            Rectangle inner = Rectangle.Inflate(area, -1, -1);
            int filled = (int)Math.Round(inner.Width * fraction);
            if (filled > 0 && inner.Height > 0)
            {
                Rectangle fill = RightToLeft == RightToLeft.Yes && RightToLeftLayout
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
            LogManager.GetCurrentClassLogger().Error(ex, $"Could not paint progress bar {Name}");
        }
    }
}
