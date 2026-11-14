//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using CefSharp.WinForms;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TVRename.AddIns.Themes;

/// <summary>
/// Applies the active theme to a window and everything on it. Application.SetColorMode(Dark) themes most standard
/// controls; this fills the gaps (tab strips, list headers and groups, calendar, progress bars, grids, web pages,
/// black icons, colours that are hard coded for the light look) using the colours of the theme file.
/// Nothing here does anything unless a theme is in use, so the standard look is never changed.
/// </summary>
internal static partial class ThemeStyler
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static Theme T => ThemeManager.Active ?? Theme.BuiltInDark;

    private static readonly ConditionalWeakTable<Control, object> Themed = new();
    private static readonly ConditionalWeakTable<Form, object> StandardLookWindows = new();
    private static readonly ConditionalWeakTable<ToolStripItem, object> AdaptedItems = new();
    private static readonly ConditionalWeakTable<ImageList, object> AdaptedImageLists = new();

    [ThreadStatic] private static bool changingColour;

    /// <summary>True the first time it is called for a given object (so nothing is themed twice).</summary>
    private static bool FirstTime<TKey>(ConditionalWeakTable<TKey, object> done, TKey key) where TKey : class
    {
        if (done.TryGetValue(key, out _))
        {
            return false;
        }

        done.Add(key, true);
        return true;
    }

    internal static bool IsStandardLookWindow(Form form) => StandardLookWindows.TryGetValue(form, out _);

    /// <summary>Applies the theme to a control and everything in it, including controls added later.</summary>
    internal static void ApplyTheme(Control control)
    {
        if (!ThemeManager.IsThemed)
        {
            return;
        }

        try
        {
            ThemeControl(control);
        }
        catch (Exception ex)
        {
            // Theming must never stop a window from opening.
            Logger.Error(ex, $"Theme add-in: could not theme {control.Name}");
        }
    }

    /// <summary>Called when the application is idle: work that has to wait until a window has filled itself.</summary>
    internal static void OnIdle()
    {
        if (ThemeManager.IsDark)
        {
            GridStyler.Sweep();
        }
    }

    private static void ThemeControl(Control c)
    {
        if (!FirstTime(Themed, c))
        {
            return;
        }

        // Most fixes are for what Windows' dark look gets wrong; a theme built on the light look only needs its
        // colours applied (backgrounds, text, tab strips, headers, calendar, progress bars, menus, web pages).
        bool dark = ThemeManager.IsDark;

        ApplyColourRules(c);
        if (dark)
        {
            FlattenSunkenBorder(c);
        }

        switch (c)
        {
            case ChromiumWebBrowser web:
                WebPageStyler.Attach(web);
                break;

            case SourceGrid.GridVirtual grid when dark:
                GridStyler.Track(grid);
                break;

            case TabControl tabControl:
                ThemeTabControl(tabControl);
                break;

            case TabPage tabPage:
                // A tab page that uses the visual style back colour is painted white whatever the colour mode.
                tabPage.UseVisualStyleBackColor = false;
                tabPage.BackColor = T.Background;
                break;

            case ListView listView:
                ThemeListView(listView);
                break;

            case TreeView treeView when dark:
                WhenHandleCreated(treeView, () => SetWindowTheme(treeView.Handle, "DarkMode_Explorer", null));
                break;

            case MonthCalendar calendar:
                ThemeMonthCalendar(calendar);
                break;

            case ProgressBar progressBar:
                ProgressBarPainter.Attach(progressBar);
                break;

            case ToolStrip toolStrip:
                UseThemeRenderer(toolStrip);
                AdaptToolStripItems(toolStrip.Items);
                break;

            case ComboBox when dark:
                // Without this a drop down list keeps its light look. This is the dark theme Windows' own
                // dark file dialogs use for combo boxes.
                WhenHandleCreated(c, () => SetWindowTheme(c.Handle, "DarkMode_CFD", null));
                break;

            case RichTextBox { Enabled: false, Parent.Enabled: true } infoText when dark:
                // Windows draws a disabled rich text box grey on grey whatever colours it is given, which is hard
                // to read on dark. Read only gives the same "look but do not edit" box in the theme's colours.
                infoText.ReadOnly = true;
                infoText.TabStop = false;
                infoText.Enabled = true;
                infoText.ForeColor = T.ListText;
                WhenHandleCreated(c, () => SetWindowTheme(c.Handle, "DarkMode_Explorer", null));
                break;

            case TextBoxBase { Multiline: true } when dark:
            case ListBox when dark:
            case ScrollableControl { AutoScroll: true } when dark:
                // Dark scroll bars.
                WhenHandleCreated(c, () => SetWindowTheme(c.Handle, "DarkMode_Explorer", null));
                break;

            case Button button when dark:
                // Buttons need the visual style back colour to get the Windows 11 dark look.
                button.UseVisualStyleBackColor = true;
                if (button.Image is { } buttonImage)
                {
                    button.Image = AdaptImageForDark(buttonImage);
                }
                break;

            case LinkLabel link:
                // Link labels use pure blue, which cannot be read on dark. Keep the hue, make it light enough.
                Color linkBack = EffectiveBackColor(link);
                link.LinkColor = Readable(link.LinkColor, linkBack, 4.5);
                link.ActiveLinkColor = Readable(link.ActiveLinkColor, linkBack, 4.5);
                link.VisitedLinkColor = Readable(link.VisitedLinkColor, linkBack, 4.5);
                link.DisabledLinkColor = T.DisabledText;
                break;

            case Label label when dark:
                // Windows draws disabled label text embossed in two dark greys, which cannot be read on dark.
                label.Paint += RepaintDisabledLabel;
                break;

            default:
                // Every other control already looks right with the colours set above.
                break;
        }

        if (c.ContextMenuStrip is { } menu)
        {
            UseThemeRenderer(menu);
            AdaptToolStripItems(menu.Items);
        }

        c.BackColorChanged += OnColourChanged;
        c.ForeColorChanged += OnColourChanged;

        foreach (Control child in c.Controls)
        {
            ThemeControl(child);
        }

        c.ControlAdded += (_, e) =>
        {
            if (e.Control is not null)
            {
                ApplyTheme(e.Control);
            }
        };
    }

    #region Colour rules

    // Colours TV Rename uses for the light look, and what they become in a theme.
    private static readonly Color LightWarning = Color.FromArgb(255, 210, 210);

    private static void OnColourChanged(object? sender, EventArgs e)
    {
        if (sender is Control c && !changingColour && ThemeManager.IsThemed)
        {
            ApplyColourRules(c);
        }
    }

    /// <summary>
    /// Turns colours chosen for the light look into theme colours: an explicit white background or black text,
    /// the light warning pink, the light grey of an active filter button, the grey "less important" text, and the
    /// theme's own window, list and text colours where a control uses the standard ones.
    /// </summary>
    private static void ApplyColourRules(Control c)
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
            // Some hosted controls (web browser) do not allow their colours to be changed.
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
            // On dark an explicit white is a leftover of the light look. (On the light look white is the standard
            // list colour, which the rule below handles.)
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

        // A theme can change the standard background colours. Only colours still equal to the standard ones are
        // changed, so anything TV Rename colours on purpose keeps its colour.
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
            // On the light look normal text and list text are both black, so tell them apart by the control.
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

    /// <summary>Controls that use the list (window) colours rather than the window background colours.</summary>
    private static bool IsListLike(Control c)
        => c is TextBoxBase or ListView or TreeView or ListBox or ComboBox or DataGridView or MonthCalendar or SourceGrid.GridVirtual;

    /// <summary>The colour actually behind a control (its own, or the first ancestor with a solid colour).</summary>
    private static Color EffectiveBackColor(Control control)
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

    /// <summary>Relative luminance (WCAG).</summary>
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

    /// <summary>
    /// Text that can be read on the given background: the same colour if it already contrasts enough, otherwise
    /// the colour moved towards white (dark background) or black (light background) just far enough, so a coloured
    /// text stays recognisably the same colour.
    /// </summary>
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

    #region Labels

    private static void RepaintDisabledLabel(object? sender, PaintEventArgs e)
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
            Logger.Error(ex, $"Theme add-in: could not draw label {label.Name}");
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

    /// <summary>
    /// Windows draws the sunken 3D border (BorderStyle.Fixed3D) with a white highlight edge whatever the colour
    /// mode, which outlines lists, trees and text boxes in white. A single line border looks right in the dark theme.
    /// </summary>
    private static void FlattenSunkenBorder(Control c)
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
                // A RichTextBox turns FixedSingle back into Fixed3D (documented), so it gets no border instead.
                property.SetValue(c, c is RichTextBox ? BorderStyle.None : BorderStyle.FixedSingle);
            }
        }
        catch (TargetInvocationException)
        {
            // A control that refuses the change keeps its border.
        }
    }

    private static PropertyInfo FindBorderStyleProperty(Type type)
    {
        // Some controls (RichTextBox) redeclare BorderStyle, so take the most derived declaration.
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

    // Marker for "this control type has no BorderStyle property" (ConditionalWeakTable cannot hold null).
    private static readonly PropertyInfo NoBorderStyleProperty =
        typeof(ThemeStyler).GetProperty(nameof(T), BindingFlags.NonPublic | BindingFlags.Static)!;

    #endregion

    #region Month calendar

    private static void ThemeMonthCalendar(MonthCalendar calendar)
    {
        // The themed calendar ignores colours and stays white, so switch its visual style off and colour it.
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

    /// <summary>
    /// Without visual styles the calendar is a few pixels taller, so in a fixed height table row its bottom row
    /// (today's date) is cut off. Grow the fixed row that holds it, if it is too small.
    /// </summary>
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

    #region Icons

    private static void AdaptToolStripItems(ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            // Only the dark look needs lighter icons.
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

    private static void AdaptImageList(ImageList? imageList)
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

    private const int MinColouredLightness = 110;

    /// <summary>
    /// Many icons are black line drawings that disappear on a dark background. Turns the dark, grey parts of an
    /// icon light (black becomes white) and brightens very dark colours, so coloured icons keep their colours.
    /// </summary>
    private static Image AdaptImageForDark(Image source)
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
                    // Mirror the lightness: black -> white, dark grey -> light grey, keeping any slight tint.
                    int shift = 255 - (2 * lightness);
                    pixels[i] = (byte)Math.Min(255, b + shift);
                    pixels[i + 1] = (byte)Math.Min(255, g + shift);
                    pixels[i + 2] = (byte)Math.Min(255, r + shift);
                }
                else if (saturation >= 0.25 && lightness < MinColouredLightness)
                {
                    // Brighten dark colours (dark red, navy) keeping their hue, so they show on a dark background.
                    double scale = Math.Min(3.0, MinColouredLightness / (double)Math.Max(lightness, 1));
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

    #region Standard look (splash screen)

    /// <summary>
    /// Makes a window look exactly as it does without a theme: every system colour it uses is replaced by the
    /// colour Windows uses for it in the standard look, and progress bars get their standard visual style back.
    /// </summary>
    internal static void KeepStandardLook(Form form)
    {
        if (!ThemeManager.IsDark)
        {
            return;
        }

        StandardLookWindows.AddOrUpdate(form, true);
        UseStandardColours(form);
    }

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

        if (c is ProgressBar bar)
        {
            // Removing the dark mode theme association gives the progress bar its normal (green) visual style.
            WhenHandleCreated(bar, () => SetWindowTheme(bar.Handle, null, null));
        }

        foreach (Control child in c.Controls)
        {
            UseStandardColours(child);
        }
    }

    /// <summary>The standard look's value of a system colour (GetSysColor ignores the application's dark mode).</summary>
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

    /// <summary>
    /// Runs an action that needs the window handle: now if it exists, and again whenever it is (re)created.
    /// It also runs once more after the framework has finished its own handle set up, so a theme set by
    /// Windows Forms itself does not replace ours.
    /// </summary>
    private static void WhenHandleCreated(Control control, System.Action action)
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
                Logger.Error(ex, $"Theme add-in: could not finish theming {control.Name}");
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

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string? pszSubAppName, string? pszSubIdList);

    [DllImport("user32.dll")]
    private static extern int GetSysColor(int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

    private const int WM_PAINT = 0x000F;
    private const int WM_ERASEBKGND = 0x0014;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

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
        // rgbReserved: 32 reserved bytes, kept as plain fields so the struct needs no array marshalling.
        public long reserved0;
        public long reserved1;
        public long reserved2;
        public long reserved3;
    }

    /// <summary>Runs a WM_PAINT with our own painting (and the device context Windows gives, if any).</summary>
    private static void HandlePaint(ref Message m, System.Action<Graphics> paint)
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
