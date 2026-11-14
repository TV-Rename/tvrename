//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Runtime.InteropServices;
using TVRename.Forms;

namespace TVRename;

// Theme colours for a list's rows, group titles, grid lines and the header past the last column
internal sealed class ListViewColourer : NativeWindow
{
    private const int GROUP_TITLE_MAX_CHARS = 512;
    private readonly ListView list;
    private readonly int offsetText = Marshal.OffsetOf<NMLVCUSTOMDRAW>(nameof(NMLVCUSTOMDRAW.clrText)).ToInt32();
    private readonly int offsetTextBack = Marshal.OffsetOf<NMLVCUSTOMDRAW>(nameof(NMLVCUSTOMDRAW.clrTextBk)).ToInt32();

    private static Theme T => ThemeHelper.T;

    public ListViewColourer(ListView list)
    {
        this.list = list;
        // NativeWindow lets go of the handle itself when the window is destroyed
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
        if (!ThemeManager.IsThemed)
        {
            base.WndProc(ref m);
            return;
        }

        if (m.Msg == WM_PAINT && ThemeManager.IsDark && list.GridLines && list.View == View.Details)
        {
            base.WndProc(ref m);
            RecolourGridLines();
            return;
        }

        if (m.Msg == WM_NOTIFY && m.LParam != IntPtr.Zero && list is BrightIdeasSoftware.ObjectListView)
        {
            NMHDR header = Marshal.PtrToStructure<NMHDR>(m.LParam);
            if (header.code == NM_CUSTOMDRAW && header.hwndFrom == SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero))
            {
                HeaderCustomDraw(ref m);
                return;
            }
        }

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
                    return; // the light look's blue group titles are readable
                }

                if (draw.dwDrawStage == CDDS_PREPAINT)
                {
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
            LogManager.GetCurrentClassLogger().Error(ex, $"Custom draw failed in {list.Name}");
        }
    }

    private void AdjustRowColours(IntPtr lParam)
    {
        int text = Marshal.ReadInt32(lParam, offsetText);
        int textBack = Marshal.ReadInt32(lParam, offsetTextBack);

        Color listBack = list.BackColor;
        Color back = IsSpecific(textBack) ? FromColorRef(textBack) : listBack;
        Color? themedBack = ThemeHelper.ThemedRowBack(back, listBack);
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
        Color readable = ThemeHelper.Readable(wanted, finalBack);
        if (readable.ToArgb() != fore.ToArgb())
        {
            Marshal.WriteInt32(lParam, offsetText, ToColorRef(readable));
        }
    }

    private void HeaderCustomDraw(ref Message m)
    {
        NMCUSTOMDRAW draw = Marshal.PtrToStructure<NMCUSTOMDRAW>(m.LParam);
        base.WndProc(ref m);

        try
        {
            if (draw.dwDrawStage == CDDS_PREPAINT)
            {
                m.Result = (IntPtr)((long)m.Result | CDRF_NOTIFYPOSTPAINT);
            }
            else if (draw.dwDrawStage == CDDS_POSTPAINT)
            {
                int right = 0;
                int count = (int)SendMessage(draw.hdr.hwndFrom, HDM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
                for (int i = 0; i < count; i++)
                {
                    RECT item = new();
                    if (SendMessage(draw.hdr.hwndFrom, HDM_GETITEMRECT, (IntPtr)i, ref item) != IntPtr.Zero)
                    {
                        right = Math.Max(right, item.right);
                    }
                }

                if (right < draw.rc.right)
                {
                    Rectangle strip = Rectangle.FromLTRB(right, draw.rc.top, draw.rc.right, draw.rc.bottom);
                    using Graphics g = Graphics.FromHdc(draw.hdc);
                    using SolidBrush back = new(T.HeaderBackground);
                    using Pen line = new(T.Border);
                    g.FillRectangle(back, strip);
                    g.DrawLine(line, strip.Left, strip.Bottom - 1, strip.Right, strip.Bottom - 1);
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.GetCurrentClassLogger().Error(ex, $"Header custom draw failed in {list.Name}");
        }
    }

    private void RecolourGridLines()
    {
        try
        {
            // The header moves left when the list is scrolled sideways, so its items are offset by where it sits
            IntPtr header = SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            int headerLeft = 0, headerBottom = 0;
            if (header != IntPtr.Zero && GetWindowRect(header, out RECT headerRect))
            {
                Point topLeft = list.PointToClient(new Point(headerRect.left, headerRect.top));
                headerLeft = topLeft.X;
                headerBottom = topLeft.Y + headerRect.bottom - headerRect.top;
            }

            int rowHeight = (int)(((long)SendMessage(list.Handle, LVM_APPROXIMATEVIEWRECT, (IntPtr)2, (IntPtr)(-1)) >> 16) & 0xFFFF)
                            - (int)(((long)SendMessage(list.Handle, LVM_APPROXIMATEVIEWRECT, (IntPtr)1, (IntPtr)(-1)) >> 16) & 0xFFFF);
            if (rowHeight <= 0)
            {
                return;
            }

            int top = list.TopItem is { } first ? list.GetItemRect(first.Index).Top : headerBottom;
            Rectangle client = list.ClientRectangle;

            using Graphics g = list.CreateGraphics();
            using Pen line = new(T.Border);
            for (int y = top + rowHeight - 1; y < client.Bottom; y += rowHeight)
            {
                g.DrawLine(line, client.Left, y, client.Right, y);
            }

            int count = header == IntPtr.Zero ? 0 : (int)SendMessage(header, HDM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
            for (int i = 0; i < count; i++)
            {
                RECT item = new();
                if (SendMessage(header, HDM_GETITEMRECT, (IntPtr)i, ref item) != IntPtr.Zero)
                {
                    int x = headerLeft + item.right;
                    g.DrawLine(line, x, headerBottom, x, client.Bottom);
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.GetCurrentClassLogger().Error(ex, $"Could not draw the grid lines of {list.Name}");
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

        IntPtr buffer = Marshal.AllocHGlobal(GROUP_TITLE_MAX_CHARS * sizeof(char));
        try
        {
            LVGROUP group = new()
            {
                cbSize = Marshal.SizeOf<LVGROUP>(),
                mask = LVGF_HEADER,
                pszHeader = buffer,
                cchHeader = GROUP_TITLE_MAX_CHARS
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
                // Cover the dark blue title (a hovered title keeps Windows' highlight behind it)
                using SolidBrush back = new(list.BackColor);
                g.FillRectangle(back, area);
            }

            // A little extra width so a marginally wider font is never cut off
            Rectangle textArea = area with { Width = area.Width + area.Height };
            TextRenderer.DrawText(g, title, list.Font, textArea, ThemeHelper.Readable(T.GroupTitle, list.BackColor, 3.0),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private const int WM_PAINT = 0x000F;
    private const int WM_NOTIFY = 0x004E;
    private const int WM_REFLECT_NOTIFY = 0x2000 + WM_NOTIFY;
    private const int LVM_GETHEADER = 0x1000 + 31;
    private const int LVM_APPROXIMATEVIEWRECT = 0x1000 + 64;
    private const int HDM_GETITEMCOUNT = 0x1200;
    private const int HDM_GETITEMRECT = 0x1200 + 7;
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
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

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

    [StructLayout(LayoutKind.Sequential)]
    private struct NMHDR
    {
        public IntPtr hwndFrom;
        public UIntPtr idFrom;
        public int code;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NMCUSTOMDRAW
    {
        public NMHDR hdr;
        public int dwDrawStage;
        public IntPtr hdc;
        public RECT rc;
        public UIntPtr dwItemSpec;
        public uint uItemState;
        public IntPtr lItemlParam;
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

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref RECT lParam);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref LVGROUP lParam);
}
