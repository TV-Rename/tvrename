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
                Item = item.Index,
                Mask = LVIF_STATE,
                StateMask = LVIS_STATEIMAGEMASK,
                State = 0
            };

            SendMessage(list.Handle, LVM_SETITEM, IntPtr.Zero, ref lviItem);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LVGROUP
        {
            public int cbSize;
            public int mask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszHeader;
            public int cchHeader;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszFooter;
            public int cchFooter;
            public int iGroupId;
            public int stateMask;
            public int state;
            public int uAlign;
        }

        public enum GroupState
        {
            COLLAPSIBLE = 8,
            COLLAPSED = 1,
            EXPANDED = 0
        }

        [DllImport("user32.dll")]
        static extern int SendMessage2
            (IntPtr window, int message, int wParam, IntPtr lParam);

        public static void SetGroupCollapse([NotNull] this ListView list, GroupState state)
        {
            for (int i = 0; i <= list.Groups.Count; i++)
            {
                LVGROUP group = new LVGROUP();
                group.cbSize = Marshal.SizeOf(group);
                group.state = (int)state; // LVGS_COLLAPSIBLE 
                group.mask = 4; // LVGF_STATE 
                group.iGroupId = i;
                IntPtr ip = IntPtr.Zero;
                try
                {
                    ip = Marshal.AllocHGlobal(group.cbSize);
                    Marshal.StructureToPtr(group, ip, true);
                    SendMessage2(list.Handle, 0x1000 + 147, i, ip); // #define LVM_SETGROUPINFO(LVM_FIRST + 147)
               
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine
                        (ex.Message + Environment.NewLine + ex.StackTrace);
                }
                finally
                {
                    if (null != ip) Marshal.FreeHGlobal(ip);
                }
            }
        }

        private const int LVIF_STATE = 0x8;
        private const int LVIS_STATEIMAGEMASK = 0xF000;
        private const int LVM_SETITEM = LVM_FIRST + 76;

        private struct Lvitem
        {
            // ReSharper disable once NotAccessedField.Local
            public int Mask;
            // ReSharper disable once NotAccessedField.Local
            public int Item;
            public int SubItem;
            // ReSharper disable once NotAccessedField.Local
            public int State;
            // ReSharper disable once NotAccessedField.Local
            public int StateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string LpszText;
            public int CchTextMax;
            public int Image;
            public IntPtr Param;
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
