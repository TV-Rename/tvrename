// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
        private bool _checkEnable;
        private bool _keyCheck;
        private readonly bool _menuCheck;
        private bool _onMouseDown;

        public MyListView()
        {
            _keyCheck = false;
            _checkEnable = true;
            _onMouseDown = false;
            _menuCheck = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _onMouseDown = true;
            base.OnMouseDown(e);
        }

        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            if (_onMouseDown)
                _checkEnable = false;
            base.OnItemSelectionChanged(e);
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (!_menuCheck && !_keyCheck && (false == _checkEnable)) //  || (!_keyCheck && _checkEnable && SelectedItems->Count > 1)
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
            _checkEnable = true;
            _onMouseDown = false;
            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                _keyCheck = true;
            else
                base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                _keyCheck = false;
        }

        // The 'TopItem' function doesn't work in a ListView if groups are enabled. This is meant to be a workaround.
        // Problem is, it just doesn't work and I don't know why!
        const Int32 LvmFirst = 0x1000;
        const Int32 LvmScroll = LvmFirst + 20;
        private const int SbHorz = 0;
        private const int SbVert = 1;

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public int GetScrollVerticalPos()
        {
            return GetScrollPos(Handle, SbVert);
        }

        public void SetScrollVerticalPos(int position)
        {
            var currentPos = GetScrollPos(Handle, SbVert);
            var delta = -(currentPos - position);
            SendMessage(Handle, LvmScroll, IntPtr.Zero, (IntPtr)delta); // First param is horizontal scroll amount, second is vertical scroll amount
        }
    }
}
