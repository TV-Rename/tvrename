//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TVRename.Forms;

internal static class ThemeHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    internal static Theme T => ThemeManager.Active ?? Theme.BuiltInDark;

    private static readonly ConditionalWeakTable<Control, object> Themed = new();
    private static readonly ConditionalWeakTable<ToolStripItem, object> AdaptedItems = new();
    private static readonly ConditionalWeakTable<ImageList, object> AdaptedImageLists = new();
    private static readonly ConditionalWeakTable<ImageList, ImageList> ThemedIconLists = new();
    private static readonly ConditionalWeakTable<ListView, ListViewColourer> ListColourers = new();
    private static readonly ConditionalWeakTable<SourceGrid.Cells.Views.IView, SourceGrid.Cells.Views.IView> HeaderReplacements = new();
    private static readonly ConditionalWeakTable<SourceGrid.Cells.Views.IView, object> CheckedViews = new();

    [ThreadStatic] private static bool changingColour;

    // Colour TV Rename uses for warnings in the light look (UiHelpers.WarningColor)
    private static readonly Color LightWarning = Color.FromArgb(255, 210, 210);

    // True the first time it is called for an object, so nothing is themed twice
    internal static bool FirstTime<TKey>(ConditionalWeakTable<TKey, object> done, TKey key) where TKey : class
    {
        if (done.TryGetValue(key, out _))
        {
            return false;
        }

        done.Add(key, true);
        return true;
    }

    internal static bool FirstTime(Control c) => FirstTime(Themed, c);

    #region Colours

    internal static void OnColourChanged(object? sender, EventArgs e)
    {
        if (sender is Control c && !changingColour && ThemeManager.IsThemed)
        {
            ApplyColourRules(c);
        }
    }

    // Colours chosen for the light look become theme colours
    internal static void ApplyColourRules(Control c)
    {
        try
        {
            changingColour = true;

            Color back = c.BackColor;
            Color? newBack = MapBackColour(c, back);
            if (newBack is { } nb && (nb.IsEmpty || nb.ToArgb() != back.ToArgb()))
            {
                c.BackColor = nb;
            }

            Color fore = c.ForeColor;
            Color? newFore = MapForeColour(c, fore);
            if (newFore is { } nf && (nf.IsEmpty || nf.ToArgb() != fore.ToArgb()))
            {
                c.ForeColor = nf;
            }
        }
        catch (Exception ex) when (ex is NotSupportedException or ArgumentException)
        {
            // Some hosted controls (web browser) do not allow their colours to be changed
        }
        finally
        {
            changingColour = false;
        }
    }

    private static Color? MapBackColour(Control c, Color back)
    {
        if (back.IsEmpty || back.A == 0)
        {
            return null;
        }

        int argb = back.ToArgb();
        if (ThemeManager.IsDark && !back.IsSystemColor && argb == Color.White.ToArgb())
        {
            // On dark an explicit white is left over from the light look
            return Color.Empty;
        }

        if (argb == LightWarning.ToArgb())
        {
            return T.Warning;
        }

        if (c is ButtonBase && argb == Color.LightGray.ToArgb())
        {
            return T.ActiveFilterButton;
        }

        // Only colours still equal to the standard ones change, so anything coloured on purpose keeps its colour
        if (argb == SystemColors.Control.ToArgb() && T.Background.ToArgb() != argb)
        {
            return T.Background;
        }

        if (argb == SystemColors.Window.ToArgb() && T.ListBackground.ToArgb() != argb)
        {
            return T.ListBackground;
        }

        return null;
    }

    private static Color? MapForeColour(Control c, Color fore)
    {
        if (fore.IsEmpty)
        {
            return null;
        }

        if (fore.IsKnownColor && fore.ToKnownColor() == KnownColor.ControlDark)
        {
            return T.SecondaryText;
        }

        int argb = fore.ToArgb();
        if (ThemeManager.IsDark && !fore.IsSystemColor && argb == Color.Black.ToArgb())
        {
            return Color.Empty;
        }

        if (!ThemeManager.IsDark && argb == Color.Black.ToArgb())
        {
            // In the light look normal text and list text are both black
            return IsListLike(c) ? T.ListText : T.Text;
        }

        if (argb == SystemColors.ControlText.ToArgb() && T.Text.ToArgb() != argb)
        {
            return T.Text;
        }

        if (argb == SystemColors.WindowText.ToArgb() && T.ListText.ToArgb() != argb)
        {
            return T.ListText;
        }

        return null;
    }

    private static bool IsListLike(Control c)
        => c is TextBoxBase or ListView or TreeView or ListBox or ComboBox or DataGridView or MonthCalendar or SourceGrid.GridVirtual;

    // The colour actually behind a control: its own, or the first parent with a solid colour
    internal static Color EffectiveBackColor(Control control)
    {
        for (Control? c = control; c is not null; c = c.Parent)
        {
            if (!c.BackColor.IsEmpty && c.BackColor.A == 255)
            {
                return c.BackColor;
            }
        }

        return T.Background;
    }

    // Relative luminance (WCAG)
    private static double Luminance(Color c)
    {
        static double Channel(int v)
        {
            double s = v / 255.0;
            return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
        }

        return (0.2126 * Channel(c.R)) + (0.7152 * Channel(c.G)) + (0.0722 * Channel(c.B));
    }

    private static double Contrast(Color a, Color b)
    {
        double la = Luminance(a), lb = Luminance(b);
        return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05);
    }

    // Moves the text colour just far enough to be readable, so coloured text keeps its colour
    internal static Color Readable(Color text, Color back, double minimum = 4.0)
    {
        if (Contrast(text, back) >= minimum)
        {
            return text;
        }

        Color target = Luminance(back) < 0.4 ? Color.White : Color.Black;
        for (int step = 1; step <= 10; step++)
        {
            double t = step / 10.0;
            Color mixed = Color.FromArgb(
                (int)Math.Round(text.R + ((target.R - text.R) * t)),
                (int)Math.Round(text.G + ((target.G - text.G) * t)),
                (int)Math.Round(text.B + ((target.B - text.B) * t)));
            if (Contrast(mixed, back) >= minimum)
            {
                return mixed;
            }
        }

        return target;
    }

    #endregion

    #region Link labels and disabled labels

    internal static void ThemeLinkLabel(LinkLabel link)
    {
        // Link labels use pure blue, which cannot be read on dark
        Color back = EffectiveBackColor(link);
        link.LinkColor = Readable(link.LinkColor, back, 4.5);
        link.ActiveLinkColor = Readable(link.ActiveLinkColor, back, 4.5);
        link.VisitedLinkColor = Readable(link.VisitedLinkColor, back, 4.5);
        link.DisabledLinkColor = T.DisabledText;
    }

    // Windows draws disabled label text embossed in two dark greys, which cannot be read on dark
    internal static void RepaintDisabledLabel(object? sender, PaintEventArgs e)
    {
        if (sender is not Label label || label.Enabled || !ThemeManager.IsDark || label.Image is not null || string.IsNullOrEmpty(label.Text))
        {
            return;
        }

        try
        {
            using (SolidBrush back = new(EffectiveBackColor(label)))
            {
                e.Graphics.FillRectangle(back, label.ClientRectangle);
            }

            Rectangle area = Rectangle.FromLTRB(
                label.Padding.Left,
                label.Padding.Top,
                label.ClientSize.Width - label.Padding.Right,
                label.ClientSize.Height - label.Padding.Bottom);

            TextRenderer.DrawText(e.Graphics, label.Text, label.Font, area, T.DisabledText, LabelTextFlags(label));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Could not draw label {label.Name}");
        }
    }

    private static TextFormatFlags LabelTextFlags(Label label)
    {
        TextFormatFlags flags = label.TextAlign switch
        {
            ContentAlignment.TopLeft => TextFormatFlags.Top | TextFormatFlags.Left,
            ContentAlignment.TopCenter => TextFormatFlags.Top | TextFormatFlags.HorizontalCenter,
            ContentAlignment.TopRight => TextFormatFlags.Top | TextFormatFlags.Right,
            ContentAlignment.MiddleLeft => TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            ContentAlignment.MiddleCenter => TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            ContentAlignment.MiddleRight => TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
            ContentAlignment.BottomLeft => TextFormatFlags.Bottom | TextFormatFlags.Left,
            ContentAlignment.BottomCenter => TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter,
            _ => TextFormatFlags.Bottom | TextFormatFlags.Right
        };

        flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;
        flags |= label.UseMnemonic ? TextFormatFlags.HidePrefix : TextFormatFlags.NoPrefix;
        if (label.AutoEllipsis)
        {
            flags |= TextFormatFlags.EndEllipsis;
        }

        if (label.RightToLeft == RightToLeft.Yes)
        {
            flags |= TextFormatFlags.RightToLeft;
        }

        return flags;
    }

    #endregion

    #region Borders

    private static readonly ConditionalWeakTable<Type, PropertyInfo> BorderStyleProperties = new();

    // Marker for "this type has no BorderStyle" (ConditionalWeakTable cannot hold null)
    private static readonly PropertyInfo NoBorderStyleProperty =
        typeof(ThemeHelper).GetProperty(nameof(T), BindingFlags.NonPublic | BindingFlags.Static)!;

    // Windows draws the sunken border with a white edge even in dark mode
    internal static void FlattenSunkenBorder(Control c)
    {
        PropertyInfo property = BorderStyleProperties.GetValue(c.GetType(), FindBorderStyleProperty);

        if (property == NoBorderStyleProperty || !property.CanWrite)
        {
            return;
        }

        try
        {
            if (property.GetValue(c) is BorderStyle.Fixed3D)
            {
                // A RichTextBox turns FixedSingle back into Fixed3D, so it gets no border
                property.SetValue(c, c is RichTextBox ? BorderStyle.None : BorderStyle.FixedSingle);
            }
        }
        catch (TargetInvocationException)
        {
            // A control that refuses the change keeps its border
        }
    }

    private static PropertyInfo FindBorderStyleProperty(Type type)
    {
        // Some controls (RichTextBox) redeclare BorderStyle, so take the most derived one
        PropertyInfo? found = null;
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.Name == "BorderStyle" && property.PropertyType == typeof(BorderStyle)
                && (found?.DeclaringType is null || found.DeclaringType.IsAssignableFrom(property.DeclaringType)))
            {
                found = property;
            }
        }

        return found ?? NoBorderStyleProperty;
    }

    #endregion

    #region Month calendar

    internal static void ThemeMonthCalendar(MonthCalendar calendar)
    {
        // The themed calendar ignores colours and stays white, so its visual style is switched off
        calendar.BackColor = T.ListBackground;
        calendar.ForeColor = T.ListText;
        calendar.TitleBackColor = T.CalendarTitle;
        calendar.TitleForeColor = Readable(Color.White, T.CalendarTitle);
        calendar.TrailingForeColor = T.DisabledText;
        WhenHandleCreated(calendar, () =>
        {
            SetWindowTheme(calendar.Handle, string.Empty, string.Empty);
            MakeRoomForCalendar(calendar);
        });
    }

    // The calendar is taller without visual styles, so grow its fixed table row
    private static void MakeRoomForCalendar(MonthCalendar calendar)
    {
        int needed = calendar.SingleMonthSize.Height + calendar.Margin.Vertical;
        Control child = calendar;

        while (child.Parent is TableLayoutPanel table)
        {
            TableLayoutPanelCellPosition position = table.GetPositionFromControl(child);
            if (position.Row >= 0 && position.Row < table.RowStyles.Count && table.GetRowSpan(child) == 1
                && table.RowStyles[position.Row] is { SizeType: SizeType.Absolute } row)
            {
                if (needed > row.Height)
                {
                    row.Height = needed;
                }

                return;
            }

            needed += table.Padding.Vertical + table.Margin.Vertical;
            child = table;
        }
    }

    #endregion

    #region Lists

    internal static void ThemeListView(ListView listView)
    {
        // ObjectListView hides SmallImageList with its own, which is the one it draws from
        if (listView is ObjectListView objectList)
        {
            if (objectList.SmallImageList is { ColorDepth: not ColorDepth.Depth32Bit } olvIcons)
            {
                objectList.SmallImageList = ThemedIconLists.GetValue(olvIcons, ThemedIcons);
            }
        }
        else if (listView.SmallImageList is { ColorDepth: not ColorDepth.Depth32Bit } icons)
        {
            listView.SmallImageList = ThemedIconLists.GetValue(icons, ThemedIcons);
        }

        if (ThemeManager.IsDark)
        {
            WhenHandleCreated(listView, () =>
            {
                // Dark scroll bars, and light header text
                SetWindowTheme(listView.Handle, "DarkMode_Explorer", null);
                IntPtr header = SendMessage(listView.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
                if (header != IntPtr.Zero)
                {
                    SetWindowTheme(header, "DarkMode_ItemsView", null);
                }
            });
        }

        if (!ListColourers.TryGetValue(listView, out _))
        {
            ListColourers.Add(listView, new ListViewColourer(listView));
        }

        if (listView is ObjectListView olv)
        {
            // ObjectListView draws its own header text
            HeaderStateStyle normal = new() { BackColor = T.HeaderBackground, ForeColor = T.Text, FrameColor = T.Border, FrameWidth = 1 };
            HeaderStateStyle hot = new() { BackColor = T.HeaderHot, ForeColor = T.Text, FrameColor = T.Border, FrameWidth = 1 };
            HeaderStateStyle pressed = new() { BackColor = T.HeaderPressed, ForeColor = T.Text, FrameColor = T.Border, FrameWidth = 1 };
            olv.HeaderFormatStyle = new HeaderFormatStyle { Normal = normal, Hot = hot, Pressed = pressed };

            // Swapped after TV Rename's own FormatRow, as ObjectListView draws cells from the row colours
            olv.FormatRow += (_, e) =>
            {
                if (ThemeManager.IsThemed && e.Item is not null)
                {
                    ThemeRow(e.Item, olv);
                }
            };

            if (!olv.VirtualMode)
            {
                foreach (ListViewItem item in olv.Items)
                {
                    ThemeRow(item, olv);
                }
            }
        }
    }

    private static void ThemeRow(ListViewItem item, ListView list)
    {
        Color? themedBack = ThemedRowBack(item.BackColor, list.BackColor);
        if (themedBack is { } newBack)
        {
            item.BackColor = newBack;
        }

        Color back = themedBack ?? item.BackColor;
        Color fore = item.ForeColor;
        Color wanted = themedBack is not null && fore.ToArgb() == Color.Black.ToArgb() ? T.ScheduleText : fore;
        Color readable = Readable(wanted, back);
        if (readable.ToArgb() != fore.ToArgb())
        {
            item.ForeColor = readable;
        }
    }

    // The theme colour for a row colour used in the light look, or null if it is not one of them
    internal static Color? ThemedRowBack(Color back, Color listBack)
    {
        int argb = back.ToArgb();
        if (argb == Color.LightYellow.ToArgb())
        {
            return T.ScheduleToday;
        }

        if (argb == Color.LightGray.ToArgb())
        {
            return T.ScheduleFuture;
        }

        if (argb == Color.LightBlue.ToArgb())
        {
            return T.SchedulePast;
        }

        if (argb == LightWarning.ToArgb())
        {
            return T.Warning;
        }

        if (argb == Color.White.ToArgb())
        {
            return listBack;
        }

        return null;
    }

    #endregion

    #region Grids

    // SourceGrid's cell views are made for the light look; call after the grid is filled
    public static void UpdateColorCells(this SourceGrid.Grid grid)
    {
        if (!ThemeManager.IsDark)
        {
            return;
        }

        try
        {
            Color gridBack = grid.BackColor;
            for (int r = 0; r < grid.RowsCount; r++)
            {
                for (int c = 0; c < grid.ColumnsCount; c++)
                {
                    if (grid[r, c] is SourceGrid.Cells.ICell { View: { } view } cell)
                    {
                        cell.View = ThemedView(view, gridBack);
                    }
                }
            }

            grid.Invalidate(true);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Could not theme grid {grid.Name}");
        }
    }

    // Each view is checked once; header views are swapped for one in the theme's header colours
    private static SourceGrid.Cells.Views.IView ThemedView(SourceGrid.Cells.Views.IView view, Color gridBack)
    {
        if (HeaderReplacements.TryGetValue(view, out SourceGrid.Cells.Views.IView? replacement))
        {
            return replacement;
        }

        if (!FirstTime(CheckedViews, view))
        {
            return view;
        }

        switch (view)
        {
            case SourceGrid.Cells.Views.Header header:
            {
                SourceGrid.Cells.Views.IView themed = new SourceGrid.Cells.Views.Cell
                {
                    BackColor = T.HeaderBackground,
                    ForeColor = Readable(IsUsable(header.ForeColor) ? header.ForeColor : T.Text, T.HeaderBackground),
                    Font = header.Font,
                    TextAlignment = header.TextAlignment,
                    WordWrap = header.WordWrap,
                    Padding = header.Padding
                };
                HeaderReplacements.Add(view, themed);
                CheckedViews.AddOrUpdate(themed, true);
                return themed;
            }

            case SourceGrid.Cells.Views.ViewBase plain:
            {
                Color back = IsUsable(plain.BackColor) ? plain.BackColor : gridBack;
                Color fore = IsUsable(plain.ForeColor) ? plain.ForeColor : T.ListText;

                // Black text becomes the theme's normal text, not a grey that would look deliberate
                Color wanted = fore.ToArgb() == Color.Black.ToArgb() ? T.ListText : fore;
                plain.ForeColor = Readable(wanted, back);
                return view;
            }

            default:
                return view;
        }
    }

    private static bool IsUsable(Color colour) => !colour.IsEmpty && colour.A == 255;

    #endregion

    #region Menus and toolbars

    private static ToolStripRenderer? themeRenderer;

    // Menus, toolbars and the status bar use Windows' own greys whatever their colour
    internal static void UseThemeRenderer(ToolStrip toolStrip)
    {
        if (T.Background.ToArgb() == SystemColors.Control.ToArgb())
        {
            return;
        }

        if (themeRenderer is null)
        {
            themeRenderer = new ToolStripProfessionalRenderer(new ThemeColourTable()) { RoundedEdges = false };
            ToolStripManager.Renderer = themeRenderer;
        }

        toolStrip.Renderer = themeRenderer;
    }

    internal static void AdaptToolStripItems(ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            if (ThemeManager.IsDark && FirstTime(AdaptedItems, item) && item.Image is { } image)
            {
                item.Image = AdaptImageForDark(image);
            }

            if (item is ToolStripDropDownItem { HasDropDownItems: true } dropDown)
            {
                UseThemeRenderer(dropDown.DropDown);
                AdaptToolStripItems(dropDown.DropDownItems);
            }
        }
    }

    private sealed class ThemeColourTable : ProfessionalColorTable
    {
        public ThemeColourTable() => UseSystemColors = false;

        private static Color Back => T.Background;
        private static Color Hot => T.SelectedSideTab;
        private static Color Pressed => T.HeaderPressed;
        private static Color Line => T.Border;

        public override Color ToolStripGradientBegin => Back;
        public override Color ToolStripGradientMiddle => Back;
        public override Color ToolStripGradientEnd => Back;
        public override Color ToolStripBorder => Back;
        public override Color ToolStripDropDownBackground => Back;
        public override Color ToolStripContentPanelGradientBegin => Back;
        public override Color ToolStripContentPanelGradientEnd => Back;
        public override Color ToolStripPanelGradientBegin => Back;
        public override Color ToolStripPanelGradientEnd => Back;
        public override Color MenuStripGradientBegin => Back;
        public override Color MenuStripGradientEnd => Back;
        public override Color StatusStripGradientBegin => Back;
        public override Color StatusStripGradientEnd => Back;
        public override Color ImageMarginGradientBegin => Back;
        public override Color ImageMarginGradientMiddle => Back;
        public override Color ImageMarginGradientEnd => Back;
        public override Color ImageMarginRevealedGradientBegin => Back;
        public override Color ImageMarginRevealedGradientMiddle => Back;
        public override Color ImageMarginRevealedGradientEnd => Back;
        public override Color OverflowButtonGradientBegin => Back;
        public override Color OverflowButtonGradientMiddle => Back;
        public override Color OverflowButtonGradientEnd => Back;
        public override Color RaftingContainerGradientBegin => Back;
        public override Color RaftingContainerGradientEnd => Back;

        public override Color MenuBorder => Line;
        public override Color MenuItemBorder => Line;
        public override Color SeparatorDark => Line;
        public override Color SeparatorLight => Back;
        public override Color GripDark => Line;
        public override Color GripLight => Back;

        public override Color MenuItemSelected => Hot;
        public override Color MenuItemSelectedGradientBegin => Hot;
        public override Color MenuItemSelectedGradientEnd => Hot;
        public override Color MenuItemPressedGradientBegin => Pressed;
        public override Color MenuItemPressedGradientMiddle => Pressed;
        public override Color MenuItemPressedGradientEnd => Pressed;

        public override Color ButtonSelectedHighlight => Hot;
        public override Color ButtonSelectedHighlightBorder => Line;
        public override Color ButtonSelectedBorder => Line;
        public override Color ButtonSelectedGradientBegin => Hot;
        public override Color ButtonSelectedGradientMiddle => Hot;
        public override Color ButtonSelectedGradientEnd => Hot;
        public override Color ButtonPressedHighlight => Pressed;
        public override Color ButtonPressedHighlightBorder => Line;
        public override Color ButtonPressedBorder => Line;
        public override Color ButtonPressedGradientBegin => Pressed;
        public override Color ButtonPressedGradientMiddle => Pressed;
        public override Color ButtonPressedGradientEnd => Pressed;
        public override Color ButtonCheckedHighlight => T.ActiveFilterButton;
        public override Color ButtonCheckedHighlightBorder => Line;
        public override Color ButtonCheckedGradientBegin => T.ActiveFilterButton;
        public override Color ButtonCheckedGradientMiddle => T.ActiveFilterButton;
        public override Color ButtonCheckedGradientEnd => T.ActiveFilterButton;
        public override Color CheckBackground => T.ActiveFilterButton;
        public override Color CheckSelectedBackground => Hot;
        public override Color CheckPressedBackground => Pressed;
    }

    #endregion

    #region Icons

    internal static void AdaptImageList(ImageList? imageList)
    {
        if (imageList is null || !ThemeManager.IsDark || !FirstTime(AdaptedImageLists, imageList))
        {
            return;
        }

        for (int i = 0; i < imageList.Images.Count; i++)
        {
            string? key = imageList.Images.Keys[i];
            imageList.Images[i] = AdaptImageForDark(imageList.Images[i]);
            if (!string.IsNullOrEmpty(key))
            {
                imageList.Images.SetKeyName(i, key);
            }
        }
    }

    // The list icons are drawn on white, which shows as a box on any other background
    private static ImageList ThemedIcons(ImageList icons)
    {
        ImageList themed = new() { ColorDepth = ColorDepth.Depth32Bit, ImageSize = icons.ImageSize };
        for (int i = 0; i < icons.Images.Count; i++)
        {
            using Bitmap drawn = new(icons.ImageSize.Width, icons.ImageSize.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(drawn))
            {
                g.Clear(Color.Magenta);
                icons.Draw(g, 0, 0, i);
            }

            Bitmap icon = new(drawn.Width, drawn.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(icon))
            {
                g.DrawImage(drawn, 0, 0, drawn.Width, drawn.Height);
            }

            ClearBackground(icon);
            themed.Images.Add(ThemeManager.IsDark ? AdaptImageForDark(icon) : icon);
            string? key = icons.Images.Keys[i];
            if (!string.IsNullOrEmpty(key))
            {
                themed.Images.SetKeyName(i, key);
            }
        }

        return themed;
    }

    // Clears the near white around an icon (not inside it) and the parts its mask leaves out
    private static void ClearBackground(Bitmap icon)
    {
        int magenta = Color.Magenta.ToArgb();
        bool[,] done = new bool[icon.Width, icon.Height];
        Stack<Point> todo = new();
        for (int x = 0; x < icon.Width; x++)
        {
            for (int y = 0; y < icon.Height; y++)
            {
                if (icon.GetPixel(x, y).ToArgb() == magenta)
                {
                    icon.SetPixel(x, y, Color.Transparent);
                }

                if (x == 0 || y == 0 || x == icon.Width - 1 || y == icon.Height - 1)
                {
                    todo.Push(new Point(x, y));
                }
            }
        }

        while (todo.Count > 0)
        {
            Point p = todo.Pop();
            if (p.X < 0 || p.Y < 0 || p.X >= icon.Width || p.Y >= icon.Height || done[p.X, p.Y])
            {
                continue;
            }

            done[p.X, p.Y] = true;
            Color c = icon.GetPixel(p.X, p.Y);
            if (c.A != 0 && Math.Min(c.R, Math.Min(c.G, c.B)) < 245)
            {
                continue;
            }

            icon.SetPixel(p.X, p.Y, Color.Transparent);
            todo.Push(new Point(p.X + 1, p.Y));
            todo.Push(new Point(p.X - 1, p.Y));
            todo.Push(new Point(p.X, p.Y + 1));
            todo.Push(new Point(p.X, p.Y - 1));
        }
    }

    private const int MIN_COLOURED_LIGHTNESS = 110;

    // Black line drawing icons disappear on dark, so their dark parts are turned light
    internal static Image AdaptImageForDark(Image source)
    {
        Bitmap bitmap = new(source.Width, source.Height, PixelFormat.Format32bppArgb);
        bitmap.SetResolution(source.HorizontalResolution, source.VerticalResolution);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height));
        }

        Rectangle all = new(0, 0, bitmap.Width, bitmap.Height);
        BitmapData data = bitmap.LockBits(all, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        try
        {
            int byteCount = Math.Abs(data.Stride) * bitmap.Height;
            byte[] pixels = new byte[byteCount];
            Marshal.Copy(data.Scan0, pixels, 0, byteCount);

            for (int i = 0; i + 3 < byteCount; i += 4)
            {
                if (pixels[i + 3] == 0)
                {
                    continue;
                }

                int b = pixels[i];
                int g = pixels[i + 1];
                int r = pixels[i + 2];
                int max = Math.Max(r, Math.Max(g, b));
                int min = Math.Min(r, Math.Min(g, b));
                int lightness = (max + min) / 2;
                double saturation = max == min
                    ? 0
                    : lightness <= 127
                        ? (max - min) / (double)(max + min)
                        : (max - min) / (double)(510 - max - min);

                if (saturation < 0.25 && lightness < 128)
                {
                    // Mirror the lightness: black -> white, dark grey -> light grey
                    int shift = 255 - (2 * lightness);
                    pixels[i] = (byte)Math.Min(255, b + shift);
                    pixels[i + 1] = (byte)Math.Min(255, g + shift);
                    pixels[i + 2] = (byte)Math.Min(255, r + shift);
                }
                else if (saturation >= 0.25 && lightness < MIN_COLOURED_LIGHTNESS)
                {
                    // Brighten dark colours (dark red, navy), keeping their hue
                    double scale = Math.Min(3.0, MIN_COLOURED_LIGHTNESS / (double)Math.Max(lightness, 1));
                    pixels[i] = (byte)Math.Min(255, (int)(b * scale));
                    pixels[i + 1] = (byte)Math.Min(255, (int)(g * scale));
                    pixels[i + 2] = (byte)Math.Min(255, (int)(r * scale));
                }
            }

            Marshal.Copy(pixels, 0, data.Scan0, byteCount);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        return bitmap;
    }

    #endregion

    #region Splash screen

    // Keeps the standard look when dark mode is switched on while the window is open (the splash screen)
    public static void KeepStandardLook(this Form form) => UseStandardColours(form);

    private static void UseStandardColours(Control c)
    {
        if (StandardColour(c.BackColor) is { } back)
        {
            c.BackColor = back;
        }

        if (StandardColour(c.ForeColor) is { } fore)
        {
            c.ForeColor = fore;
        }

        foreach (Control child in c.Controls)
        {
            UseStandardColours(child);
        }
    }

    // GetSysColor ignores the application's dark mode
    private static Color? StandardColour(Color colour)
    {
        if (!colour.IsSystemColor || !SysColorIndex.TryGetValue(colour.ToKnownColor(), out int index))
        {
            return null;
        }

        int bgr = GetSysColor(index);
        return Color.FromArgb(bgr & 0xFF, (bgr >> 8) & 0xFF, (bgr >> 16) & 0xFF);
    }

    private static readonly Dictionary<KnownColor, int> SysColorIndex = new()
    {
        [KnownColor.ScrollBar] = 0, [KnownColor.Desktop] = 1, [KnownColor.ActiveCaption] = 2,
        [KnownColor.InactiveCaption] = 3, [KnownColor.Menu] = 4, [KnownColor.Window] = 5,
        [KnownColor.WindowFrame] = 6, [KnownColor.MenuText] = 7, [KnownColor.WindowText] = 8,
        [KnownColor.ActiveCaptionText] = 9, [KnownColor.ActiveBorder] = 10, [KnownColor.InactiveBorder] = 11,
        [KnownColor.AppWorkspace] = 12, [KnownColor.Highlight] = 13, [KnownColor.HighlightText] = 14,
        [KnownColor.Control] = 15, [KnownColor.ButtonFace] = 15, [KnownColor.ControlDark] = 16,
        [KnownColor.ButtonShadow] = 16, [KnownColor.GrayText] = 17, [KnownColor.ControlText] = 18,
        [KnownColor.InactiveCaptionText] = 19, [KnownColor.ControlLightLight] = 20, [KnownColor.ButtonHighlight] = 20,
        [KnownColor.ControlDarkDark] = 21, [KnownColor.ControlLight] = 22, [KnownColor.InfoText] = 23,
        [KnownColor.Info] = 24, [KnownColor.HotTrack] = 26, [KnownColor.GradientActiveCaption] = 27,
        [KnownColor.GradientInactiveCaption] = 28, [KnownColor.MenuHighlight] = 29, [KnownColor.MenuBar] = 30
    };

    #endregion

    #region Native

    // Runs again after Windows Forms' own set up, so the theme it sets does not replace ours
    internal static void WhenHandleCreated(Control control, System.Action action)
    {
        void Run()
        {
            try
            {
                if (!control.IsDisposed && control.IsHandleCreated)
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Could not finish theming {control.Name}");
            }
        }

        void RunNowAndLater()
        {
            Run();
            control.BeginInvoke((System.Windows.Forms.MethodInvoker)Run);
        }

        control.HandleCreated += (_, _) => RunNowAndLater();
        if (control.IsHandleCreated)
        {
            RunNowAndLater();
        }
    }

    internal const int WM_PAINT = 0x000F;
    internal const int WM_ERASEBKGND = 0x0014;
    private const int LVM_GETHEADER = 0x1000 + 31;

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    internal static extern int SetWindowTheme(IntPtr hWnd, string? pszSubAppName, string? pszSubIdList);

    [DllImport("user32.dll")]
    private static extern int GetSysColor(int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    internal static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

    [StructLayout(LayoutKind.Sequential)]
    private struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public int fErase;
        public int left;
        public int top;
        public int right;
        public int bottom;
        public int fRestore;
        public int fIncUpdate;
        // rgbReserved: 32 bytes
        public long reserved0;
        public long reserved1;
        public long reserved2;
        public long reserved3;
    }

    // Handles WM_PAINT with our own painting (into the device context Windows gives, if any)
    internal static void HandlePaint(ref Message m, System.Action<Graphics> paint)
    {
        if (m.WParam != IntPtr.Zero)
        {
            using Graphics given = Graphics.FromHdc(m.WParam);
            paint(given);
        }
        else
        {
            PAINTSTRUCT ps = default;
            IntPtr hdc = BeginPaint(m.HWnd, ref ps);
            try
            {
                using Graphics g = Graphics.FromHdc(hdc);
                paint(g);
            }
            finally
            {
                EndPaint(m.HWnd, ref ps);
            }
        }

        m.Result = IntPtr.Zero;
    }

    #endregion
}
