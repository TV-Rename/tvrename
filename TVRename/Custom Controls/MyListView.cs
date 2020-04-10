// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;

// Starting from:
// http://social.msdn.microsoft.com/Forums/ja-JP/csharpexpressja/thread/67475927-015c-4206-b5e7-d67504edb3a1
//
// Standard windows listview has a very weird behaviour when shift-clicking, if it is a multicolumn, 
// full-row-select, and checkboxes on.. and you click in anything other than the first column.

// This is the list view used on the "Scan" tab

namespace TVRename
{
    /// <summary>
    /// Summary for MyListView
    /// </summary>
    public class MyListView : ListViewFlickerFree
    {
        private bool checkEnable;
        private bool keyCheck;
        private readonly bool menuCheck;
        private bool onMouseDown;

        public MyListView()
        {
            keyCheck = false;
            checkEnable = true;
            onMouseDown = false;
            menuCheck = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            onMouseDown = true;
            base.OnMouseDown(e);
        }

        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            if (onMouseDown)
            {
                checkEnable = false;
            }

            base.OnItemSelectionChanged(e);
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (!menuCheck && !keyCheck && !checkEnable)
            {
                ice.NewValue = ice.CurrentValue;
                return;
            }
            base.OnItemCheck(ice);
            if (SelectedItems.Count == 1)
            {
                Items[SelectedIndices[0]].Selected = false;
                Items[ice.Index].Selected = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            checkEnable = true;
            onMouseDown = false;
            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                keyCheck = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                keyCheck = false;
            }
        }

        // The 'TopItem' function doesn't work in a ListView if groups are enabled. This is meant to be a workaround.
        // Problem is, it just doesn't work and I don't know why!
        // ReSharper disable once UnusedMember.Local
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int LVM_FIRST = 0x1000;
        private const int LVM_SCROLL = LVM_FIRST + 20;

        public int GetScrollVerticalPos() => NativeMethods.GetScrollPos(Handle, SB_VERT);

        public void SetScrollVerticalPos(int position)
        {
            int currentPos = NativeMethods.GetScrollPos(Handle, SB_VERT);
            int delta = -(currentPos - position);
            NativeMethods.SendMessage(Handle, LVM_SCROLL, IntPtr.Zero, (IntPtr)delta); // First param is horizontal scroll amount, second is vertical scroll amount
        }

        public void HideCheckbox([NotNull] ListViewItem item)
        {
            Lvitem lviItem = new Lvitem
            {
                iItem = item.Index, mask = LVIF_STATE, stateMask = LVIS_STATEIMAGEMASK, state = 0
            };

            SendMessage(Handle, LVM_SETITEM, IntPtr.Zero, ref lviItem);
        }

        private const int LVIF_STATE = 0x8;
        private const int LVIS_STATEIMAGEMASK = 0xF000;
        private const int LVM_SETITEM = LVM_FIRST + 76;

        // suppress warnings for interop
#pragma warning disable 0649
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
#pragma warning restore 0649

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, ref Lvitem lParam);
    }
}
