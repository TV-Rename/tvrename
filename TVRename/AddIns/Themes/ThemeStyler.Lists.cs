//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TVRename.AddIns.Themes;

internal static partial class ThemeStyler
{
    private static readonly ConditionalWeakTable<ListView, ListViewColourer> ListColourers = new();

    private static void ThemeListView(ListView listView)
    {
        if (ThemeManager.IsDark)
        {
            WhenHandleCreated(listView, () =>
            {
                // Dark scroll bars for the list, and the dark items view theme for the header (light header text).
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
            // ObjectListView draws its own header text, so give it colours that work on a dark background.
            HeaderStateStyle normal = new() { BackColor = T.HeaderBackground, ForeColor = T.Text, FrameColor = T.Border, FrameWidth = 1 };
            HeaderStateStyle hot = new() { BackColor = T.HeaderHot, ForeColor = T.Text, FrameColor = T.Border, FrameWidth = 1 };
            HeaderStateStyle pressed = new() { BackColor = T.HeaderPressed, ForeColor = T.Text, FrameColor = T.Border, FrameWidth = 1 };
            olv.HeaderFormatStyle = new HeaderFormatStyle { Normal = normal, Hot = hot, Pressed = pressed };

            // ObjectListView draws its cells itself from each row's colours (the custom draw step below never sees
            // them), so the light look colours are swapped as each row is prepared, after TV Rename's own FormatRow.
            olv.FormatRow += (_, e) =>
            {
                if (ThemeManager.IsThemed && e.Item is not null)
                {
                    ThemeRow(e.Item, olv);
                }
            };

            // Rows that were filled before the window was shown.
            if (!olv.VirtualMode)
            {
                foreach (ListViewItem item in olv.Items)
                {
                    ThemeRow(item, olv);
                }
            }
        }
    }

    /// <summary>Swaps a row's light look colours for the theme's and keeps its text readable.</summary>
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

    /// <summary>The theme colour for a row colour chosen for the light look, or null if the row colour is not one of them.</summary>
    private static Color? ThemedRowBack(Color back, Color listBack)
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

    /// <summary>
    /// Sits in front of a list view's own custom draw handling and adjusts what it decided:
    /// <list type="bullet">
    /// <item>Row colours chosen for the light look (schedule yellow, grey and blue, warning pink) become the
    /// theme's colours, and row text is kept readable on whatever background a row ends up with.</item>
    /// <item>The dark list theme draws group titles in dark blue and ignores any text colour given for them, so
    /// after Windows has drawn a group title this repaints just the title text, where Windows reports it, in the
    /// theme's group title colour. The rest of the group (line, collapse button) is still drawn by Windows.</item>
    /// </list>
    /// </summary>
    private sealed class ListViewColourer : NativeWindow
    {
        private const int GroupTitleMaxChars = 512;
        private readonly ListView list;
        private readonly int offsetText = Marshal.OffsetOf<NMLVCUSTOMDRAW>(nameof(NMLVCUSTOMDRAW.clrText)).ToInt32();
        private readonly int offsetTextBack = Marshal.OffsetOf<NMLVCUSTOMDRAW>(nameof(NMLVCUSTOMDRAW.clrTextBk)).ToInt32();

        public ListViewColourer(ListView list)
        {
            this.list = list;
            // NativeWindow lets go of the handle itself when the window is destroyed.
            list.HandleCreated += (_, _) => Attach();
            if (list.IsHandleCreated)
            {
                Attach();
            }
        }

        private void Attach()
        {
            if (Handle != list.Handle)
            {
                AssignHandle(list.Handle);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != WM_REFLECT_NOTIFY || m.LParam == IntPtr.Zero
                || Marshal.PtrToStructure<NMHDR>(m.LParam).code != NM_CUSTOMDRAW)
            {
                base.WndProc(ref m);
                return;
            }

            NMLVCUSTOMDRAW draw = Marshal.PtrToStructure<NMLVCUSTOMDRAW>(m.LParam);
            base.WndProc(ref m);

            try
            {
                if (draw.dwItemType == LVCDI_GROUP)
                {
                    if (!ThemeManager.IsDark)
                    {
                        // The light look's blue group titles are readable as they are.
                        return;
                    }

                    if (draw.dwDrawStage == CDDS_PREPAINT)
                    {
                        // Ask to be told when the group title has been drawn.
                        m.Result = (IntPtr)((long)m.Result | CDRF_NOTIFYPOSTPAINT);
                    }
                    else if (draw.dwDrawStage == CDDS_POSTPAINT)
                    {
                        RepaintGroupTitle(draw);
                    }
                }
                else if ((draw.dwDrawStage & CDDS_ITEMPREPAINT) == CDDS_ITEMPREPAINT)
                {
                    AdjustRowColours(m.LParam);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Theme add-in: custom draw failed in {list.Name}");
            }
        }

        /// <summary>Reads the colours the list chose for this row (or cell) and swaps light look colours for theme ones.</summary>
        private void AdjustRowColours(IntPtr lParam)
        {
            int text = Marshal.ReadInt32(lParam, offsetText);
            int textBack = Marshal.ReadInt32(lParam, offsetTextBack);

            Color listBack = list.BackColor;
            Color back = IsSpecific(textBack) ? FromColorRef(textBack) : listBack;
            Color? themedBack = ThemedRowBack(back, listBack);
            Color finalBack = themedBack ?? back;

            if (themedBack is { } newBack)
            {
                Marshal.WriteInt32(lParam, offsetTextBack, ToColorRef(newBack));
            }

            if (!IsSpecific(text))
            {
                if (themedBack is null)
                {
                    return;
                }

                text = ToColorRef(list.ForeColor);
            }

            Color fore = FromColorRef(text);
            Color wanted = themedBack is not null && fore.ToArgb() == Color.Black.ToArgb() ? T.ScheduleText : fore;
            Color readable = Readable(wanted, finalBack);
            if (readable.ToArgb() != fore.ToArgb())
            {
                Marshal.WriteInt32(lParam, offsetText, ToColorRef(readable));
            }
        }

        private static bool IsSpecific(int colorRef) => colorRef is not CLR_DEFAULT and not CLR_NONE;

        private static Color FromColorRef(int colorRef) => Color.FromArgb(colorRef & 0xFF, (colorRef >> 8) & 0xFF, (colorRef >> 16) & 0xFF);

        private static int ToColorRef(Color c) => c.R | (c.G << 8) | (c.B << 16);

        private void RepaintGroupTitle(NMLVCUSTOMDRAW draw)
        {
            int groupId = (int)draw.dwItemSpec;

            RECT label = new() { top = LVGGR_LABEL };
            if (SendMessage(list.Handle, LVM_GETGROUPRECT, (IntPtr)groupId, ref label) == IntPtr.Zero
                || label.right <= label.left || label.bottom <= label.top)
            {
                return;
            }

            IntPtr buffer = Marshal.AllocHGlobal(GroupTitleMaxChars * sizeof(char));
            try
            {
                LVGROUP group = new()
                {
                    cbSize = Marshal.SizeOf<LVGROUP>(),
                    mask = LVGF_HEADER,
                    pszHeader = buffer,
                    cchHeader = GroupTitleMaxChars
                };
                if (SendMessage(list.Handle, LVM_GETGROUPINFO, (IntPtr)groupId, ref group) == (IntPtr)(-1))
                {
                    return;
                }

                string title = Marshal.PtrToStringUni(buffer) ?? string.Empty;
                Rectangle area = Rectangle.FromLTRB(label.left, label.top, label.right, label.bottom);

                using Graphics g = Graphics.FromHdc(draw.hdc);
                if ((draw.uItemState & CDIS_HOT) == 0)
                {
                    // Cover the dark blue title (a hot, hovered title keeps Windows' highlight behind it).
                    using SolidBrush back = new(list.BackColor);
                    g.FillRectangle(back, area);
                }

                // Default padding puts the text where Windows put it; a little extra width means a
                // marginally wider font is never cut off.
                Rectangle textArea = area with { Width = area.Width + area.Height };
                TextRenderer.DrawText(g, title, list.Font, textArea, Readable(T.GroupTitle, list.BackColor, 3.0),
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }

    private const int LVM_GETHEADER = 0x1000 + 31;
    private const int WM_REFLECT_NOTIFY = 0x2000 + 0x004E;
    private const int NM_CUSTOMDRAW = -12;
    private const int CDDS_PREPAINT = 0x1;
    private const int CDDS_POSTPAINT = 0x2;
    private const int CDDS_ITEMPREPAINT = 0x10001;
    private const int CDRF_NOTIFYPOSTPAINT = 0x10;
    private const int CDIS_HOT = 0x40;
    private const int LVCDI_GROUP = 0x1;
    private const int LVM_GETGROUPRECT = 0x1000 + 98;
    private const int LVGGR_LABEL = 2;
    private const int LVM_GETGROUPINFO = 0x1000 + 149;
    private const int LVGF_HEADER = 0x1;
    private const int CLR_NONE = unchecked((int)0xFFFFFFFF);
    private const int CLR_DEFAULT = unchecked((int)0xFF000000);

    [StructLayout(LayoutKind.Sequential)]
    private struct LVGROUP
    {
        public int cbSize;
        public int mask;
        public IntPtr pszHeader;
        public int cchHeader;
        public IntPtr pszFooter;
        public int cchFooter;
        public int iGroupId;
        public int stateMask;
        public int state;
        public int uAlign;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref RECT lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref LVGROUP lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct NMHDR
    {
        public IntPtr hwndFrom;
        public UIntPtr idFrom;
        public int code;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NMLVCUSTOMDRAW
    {
        public NMHDR hdr;
        public int dwDrawStage;
        public IntPtr hdc;
        public RECT rc;
        public UIntPtr dwItemSpec;
        public uint uItemState;
        public IntPtr lItemlParam;
        public int clrText;
        public int clrTextBk;
        public int iSubItem;
        public int dwItemType;
        public int clrFace;
        public int iIconEffect;
        public int iIconPhase;
        public int iPartId;
        public int iStateId;
        public RECT rcText;
        public uint uAlign;
    }
}
