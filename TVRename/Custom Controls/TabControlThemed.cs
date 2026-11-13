//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TVRename.Forms;

namespace TVRename;

// The native tab control always paints its tab strip and border light
public class TabControlThemed : TabControl
{
    private static Theme T => ThemeHelper.T;

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (ThemeManager.IsThemed)
        {
            Invalidate();
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (!ThemeManager.IsThemed)
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
                ThemeHelper.HandlePaint(ref m, PaintClipped);
                return;

            default:
                base.WndProc(ref m);
                return;
        }
    }

    private void PaintClipped(Graphics g)
    {
        try
        {
            Rectangle client = ClientRectangle;
            if (client.Width <= 0 || client.Height <= 0)
            {
                return;
            }

            // Never paint over the selected page
            if (SelectedTab is { Visible: true } page)
            {
                g.ExcludeClip(page.Bounds);
            }

            PaintTabs(g, client);
        }
        catch (Exception ex)
        {
            LogManager.GetCurrentClassLogger().Error(ex, $"Could not paint tab control {Name}");
        }
    }

    private void PaintTabs(Graphics g, Rectangle client)
    {
        Color stripBack = Parent?.BackColor ?? T.Background;
        Color pageBack = SelectedTab?.BackColor ?? T.Background;

        using (SolidBrush strip = new(stripBack))
        {
            g.FillRectangle(strip, client);
        }

        if (TabCount == 0)
        {
            return;
        }

        Rectangle tabArea = GetTabRect(0);
        for (int i = 1; i < TabCount; i++)
        {
            tabArea = Rectangle.Union(tabArea, GetTabRect(i));
        }

        Rectangle pane = Alignment switch
        {
            TabAlignment.Bottom => Rectangle.FromLTRB(client.Left, client.Top, client.Right, tabArea.Top),
            TabAlignment.Left => Rectangle.FromLTRB(tabArea.Right, client.Top, client.Right, client.Bottom),
            TabAlignment.Right => Rectangle.FromLTRB(client.Left, client.Top, tabArea.Left, client.Bottom),
            _ => Rectangle.FromLTRB(client.Left, tabArea.Bottom, client.Right, client.Bottom)
        };

        using (SolidBrush paneBrush = new(pageBack))
        {
            g.FillRectangle(paneBrush, pane);
        }

        using Pen border = new(T.Border);
        g.DrawRectangle(border, pane.X, pane.Y, pane.Width - 1, pane.Height - 1);

        for (int i = 0; i < TabCount; i++)
        {
            Rectangle tab = GetTabRect(i);
            bool selected = i == SelectedIndex;

            if (DrawMode == TabDrawMode.OwnerDrawFixed)
            {
                DrawOwnerDrawnTab(g, tab, i, selected);
                continue;
            }

            using (SolidBrush tabBrush = new(selected ? pageBack : T.TabStrip))
            {
                g.FillRectangle(tabBrush, tab);
            }

            g.DrawRectangle(border, tab.X, tab.Y, tab.Width - 1, tab.Height - 1);

            if (selected)
            {
                JoinSelectedTab(g, tab, pane, pageBack);
            }

            TabPage page = TabPages[i];
            Rectangle textArea = tab;
            if (TabImage(page) is { } image)
            {
                int y = tab.Top + ((tab.Height - image.Height) / 2);
                g.DrawImage(image, tab.Left + 6, y, image.Width, image.Height);
                textArea = Rectangle.FromLTRB(tab.Left + 6 + image.Width, tab.Top, tab.Right, tab.Bottom);
            }

            Color text = page.Enabled ? T.Text : T.DisabledText;
            TextRenderer.DrawText(g, page.Text, Font, textArea, text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }
    }

    private void JoinSelectedTab(Graphics g, Rectangle tab, Rectangle pane, Color pageBack)
    {
        using Pen joint = new(pageBack);
        switch (Alignment)
        {
            case TabAlignment.Top:
                g.DrawLine(joint, tab.Left + 1, tab.Bottom - 1, tab.Right - 2, tab.Bottom - 1);
                g.DrawLine(joint, tab.Left + 1, pane.Top, tab.Right - 2, pane.Top);
                break;
            case TabAlignment.Bottom:
                g.DrawLine(joint, tab.Left + 1, tab.Top, tab.Right - 2, tab.Top);
                g.DrawLine(joint, tab.Left + 1, pane.Bottom - 1, tab.Right - 2, pane.Bottom - 1);
                break;
            case TabAlignment.Left:
                g.DrawLine(joint, tab.Right - 1, tab.Top + 1, tab.Right - 1, tab.Bottom - 2);
                g.DrawLine(joint, pane.Left, tab.Top + 1, pane.Left, tab.Bottom - 2);
                break;
            case TabAlignment.Right:
                g.DrawLine(joint, tab.Left, tab.Top + 1, tab.Left, tab.Bottom - 2);
                g.DrawLine(joint, pane.Right - 1, tab.Top + 1, pane.Right - 1, tab.Bottom - 2);
                break;
            default:
                break;
        }
    }

    // Owner drawn tabs paint the selected tab white, so draw it as unselected and move its background to the theme
    private void DrawOwnerDrawnTab(Graphics g, Rectangle tab, int index, bool selected)
    {
        if (tab.Width <= 0 || tab.Height <= 0)
        {
            return;
        }

        using Bitmap bitmap = new(tab.Width, tab.Height, PixelFormat.Format32bppArgb);
        using (Graphics bg = Graphics.FromImage(bitmap))
        {
            bg.TranslateTransform(-tab.X, -tab.Y);
            OnDrawItem(new DrawItemEventArgs(bg, Font, tab, index, DrawItemState.None));
        }

        if (selected)
        {
            Rebackground(bitmap, T.SelectedSideTab, FindForm()?.ForeColor ?? T.Text);
        }

        g.DrawImageUnscaled(bitmap, tab.X, tab.Y);
    }

    private static void Rebackground(Bitmap bitmap, Color newBack, Color text)
    {
        // The corner is background in every TV Rename side tab
        Color oldBack = bitmap.GetPixel(0, 0);

        Rectangle all = new(0, 0, bitmap.Width, bitmap.Height);
        BitmapData data = bitmap.LockBits(all, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        try
        {
            int byteCount = Math.Abs(data.Stride) * bitmap.Height;
            byte[] pixels = new byte[byteCount];
            Marshal.Copy(data.Scan0, pixels, 0, byteCount);

            for (int i = 0; i + 3 < byteCount; i += 4)
            {
                int b = pixels[i], g = pixels[i + 1], r = pixels[i + 2];
                int max = Math.Max(r, Math.Max(g, b));
                int min = Math.Min(r, Math.Min(g, b));
                if (max - min > 24)
                {
                    continue; // coloured, part of an icon
                }

                pixels[i] = Remix(b, oldBack.B, newBack.B, text.B);
                pixels[i + 1] = Remix(g, oldBack.G, newBack.G, text.G);
                pixels[i + 2] = Remix(r, oldBack.R, newBack.R, text.R);
                pixels[i + 3] = 255;
            }

            Marshal.Copy(pixels, 0, data.Scan0, byteCount);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
    }

    private static byte Remix(int value, int oldBack, int newBack, int text)
    {
        if (Math.Abs(text - oldBack) < 8)
        {
            return (byte)(value == oldBack ? newBack : value);
        }

        double t = Math.Clamp((value - oldBack) / (double)(text - oldBack), 0, 1);
        return (byte)Math.Round(newBack + ((text - newBack) * t));
    }

    private Image? TabImage(TabPage page)
    {
        if (ImageList is not { } list)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(page.ImageKey) && list.Images.ContainsKey(page.ImageKey))
        {
            return list.Images[page.ImageKey];
        }

        return page.ImageIndex >= 0 && page.ImageIndex < list.Images.Count ? list.Images[page.ImageIndex] : null;
    }
}
