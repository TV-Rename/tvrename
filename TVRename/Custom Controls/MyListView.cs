// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

// Starting from:
// http://social.msdn.microsoft.com/Forums/ja-JP/csharpexpressja/thread/67475927-015c-4206-b5e7-d67504edb3a1
//
// Standard windows listview has a very weird behaviour when shift-clicking, if it is a multicolumn, 
// full-row-select, and checkboxes on.. and you click in anything other than the first column.

// This is the list view used on the "Scan" tab

using System.Windows.Forms;

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
    }
}
