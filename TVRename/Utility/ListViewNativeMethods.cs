using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename.Utility
{
    static class ListViewNativeMethods
    {
        public static void HideCheckbox([NotNull] ListView list,[NotNull] ListViewItem item)
        {
            Lvitem lviItem = new Lvitem
            {
                iItem = item.Index,
                mask = LVIF_STATE,
                stateMask = LVIS_STATEIMAGEMASK,
                state = 0
            };

            SendMessage(list.Handle, LVM_SETITEM, IntPtr.Zero, ref lviItem);
        }

        private const int LVIF_STATE = 0x8;
        private const int LVIS_STATEIMAGEMASK = 0xF000;
        private const int LVM_SETITEM = LVM_FIRST + 76;

        private struct Lvitem
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String lpszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr iParam;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, ref Lvitem lParam);


        // The 'TopItem' function doesn't work in a ListView if groups are enabled. This is meant to be a workaround.
        // Problem is, it just doesn't work and I don't know why!
        // ReSharper disable once UnusedMember.Local
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int LVM_FIRST = 0x1000;
        private const int LVM_SCROLL = LVM_FIRST + 20;

        public static int GetScrollVerticalPos([NotNull] ListView list) => NativeMethods.GetScrollPos(list.Handle, SB_VERT);

        public static void SetScrollVerticalPos([NotNull] ListView list, int position)
        {
            int currentPos = NativeMethods.GetScrollPos(list.Handle, SB_VERT);
            int delta = -(currentPos - position);
            NativeMethods.SendMessage(list.Handle, LVM_SCROLL, IntPtr.Zero, (IntPtr)delta); // First param is horizontal scroll amount, second is vertical scroll amount
        }

    }
}
