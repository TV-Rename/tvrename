//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TVRename.AddIns.Themes;

internal static partial class ThemeStyler
{
    private static readonly ConditionalWeakTable<TabControl, TabControlPainter> TabPainters = new();

    private static void ThemeTabControl(TabControl tabControl)
    {
        AdaptImageList(tabControl.ImageList);

        if (!TabPainters.TryGetValue(tabControl, out _))
        {
            TabPainters.Add(tabControl, new TabControlPainter(tabControl));
        }
    }

    /// <summary>
    /// The native tab control always paints its tab strip and border light. This takes over its painting:
    /// background, border and tabs. The tab pages are separate windows and paint themselves.
    /// Owner drawn tab controls (the side tab strips) still draw their own tabs; they paint the selected tab white,
    /// so the selected tab is drawn as unselected and its background is then changed to the theme colour.
    /// </summary>
    private sealed class TabControlPainter : NativeWindow
    {
        private static readonly MethodInfo? OnDrawItemMethod = typeof(TabControl).GetMethod(
            "OnDrawItem", BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(DrawItemEventArgs)], null);

        private readonly TabControl tabs;

        public TabControlPainter(TabControl tabs)
        {
            this.tabs = tabs;
            // NativeWindow lets go of the handle itself when the window is destroyed.
            tabs.HandleCreated += (_, _) => Attach();
            if (tabs.IsHandleCreated)
            {
                Attach();
            }
        }

        private void Attach()
        {
            if (Handle != tabs.Handle)
            {
                AssignHandle(tabs.Handle);
            }
            tabs.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    m.Result = 1;
                    return;

                case WM_PAINT:
                    HandlePaint(ref m, PaintClipped);
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
                Rectangle client = tabs.ClientRectangle;
                if (client.Width <= 0 || client.Height <= 0)
                {
                    return;
                }

                // Never paint over the selected page: it paints itself.
                if (tabs.SelectedTab is { Visible: true } page)
                {
                    g.ExcludeClip(page.Bounds);
                }

                Paint(g, client);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Theme add-in: could not paint tab control {tabs.Name}");
            }
        }

        private void Paint(Graphics g, Rectangle client)
        {
            Color stripBack = tabs.Parent?.BackColor ?? T.Background;
            Color pageBack = tabs.SelectedTab?.BackColor ?? T.Background;

            using (SolidBrush strip = new(stripBack))
            {
                g.FillRectangle(strip, client);
            }

            if (tabs.TabCount == 0)
            {
                return;
            }

            // The area the tabs take up, and the page area next to it.
            Rectangle tabArea = tabs.GetTabRect(0);
            for (int i = 1; i < tabs.TabCount; i++)
            {
                tabArea = Rectangle.Union(tabArea, tabs.GetTabRect(i));
            }

            Rectangle pane = tabs.Alignment switch
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

            bool ownerDrawn = tabs.DrawMode == TabDrawMode.OwnerDrawFixed && OnDrawItemMethod is not null;
            for (int i = 0; i < tabs.TabCount; i++)
            {
                Rectangle tab = tabs.GetTabRect(i);
                bool selected = i == tabs.SelectedIndex;

                if (ownerDrawn)
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

                TabPage page = tabs.TabPages[i];
                Rectangle textArea = tab;
                Image? image = TabImage(page);
                if (image is not null)
                {
                    int y = tab.Top + ((tab.Height - image.Height) / 2);
                    g.DrawImage(image, tab.Left + 6, y, image.Width, image.Height);
                    textArea = Rectangle.FromLTRB(tab.Left + 6 + image.Width, tab.Top, tab.Right, tab.Bottom);
                }

                Color text = page.Enabled ? T.Text : T.DisabledText;
                TextRenderer.DrawText(g, page.Text, tabs.Font, textArea, text,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
            }
        }

        private void JoinSelectedTab(Graphics g, Rectangle tab, Rectangle pane, Color pageBack)
        {
            using Pen joint = new(pageBack);
            switch (tabs.Alignment)
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
                    // TabAlignment has only the four values above.
                    break;
            }
        }

        /// <summary>
        /// Lets the tab control's own DrawItem code draw the tab (always as unselected, because it paints a selected
        /// tab white), into a bitmap. For the selected tab the background is then moved to the theme colour: each
        /// grey pixel is treated as a mix of the old background and white text, and remixed with the new background,
        /// so the text and its anti-aliased edges look as if drawn on the new colour. Coloured pixels are left alone.
        /// </summary>
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
                DrawItemEventArgs e = new(bg, tabs.Font, tab, index, DrawItemState.None);
                OnDrawItemMethod!.Invoke(tabs, [e]);
            }

            if (selected)
            {
                Rebackground(bitmap, T.SelectedSideTab, tabs.FindForm()?.ForeColor ?? T.Text);
            }

            g.DrawImageUnscaled(bitmap, tab.X, tab.Y);
        }

        private static void Rebackground(Bitmap bitmap, Color newBack, Color text)
        {
            // The corner is background in every TV Rename side tab.
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
                        continue; // coloured (part of an icon)
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

        /// <summary>
        /// A pixel that is a mix of the old background and the text colour becomes the same mix of the new background
        /// and the text colour (works for light text on dark and dark text on light).
        /// </summary>
        private static byte Remix(int value, int oldBack, int newBack, int text)
        {
            if (Math.Abs(text - oldBack) < 8)
            {
                // Text and background (almost) the same in this channel: nothing to tell apart.
                return (byte)(value == oldBack ? newBack : value);
            }

            double t = Math.Clamp((value - oldBack) / (double)(text - oldBack), 0, 1);
            return (byte)Math.Round(newBack + ((text - newBack) * t));
        }

        private Image? TabImage(TabPage page)
        {
            ImageList? list = tabs.ImageList;
            if (list is null)
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
}
