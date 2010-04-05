// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
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
    public class MyListView : ListView
    {
        private bool _checkEnable;
        private bool _keyCheck;
        private bool _menuCheck;
        private bool _onMouseDown;

        public MyListView()
        {
            this._keyCheck = false;
            this._checkEnable = true;
            this._onMouseDown = false;
            this._menuCheck = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this._onMouseDown = true;
            base.OnMouseDown(e);
        }

        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            if (this._onMouseDown)
                this._checkEnable = false;
            base.OnItemSelectionChanged(e);
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (!this._menuCheck && !this._keyCheck && (false == this._checkEnable)) //  || (!_keyCheck && _checkEnable && SelectedItems->Count > 1)
            {
                ice.NewValue = ice.CurrentValue;
                return;
            }
            base.OnItemCheck(ice);
            if (this.SelectedItems.Count == 1)
            {
                this.Items[this.SelectedIndices[0]].Selected = false;
                this.Items[ice.Index].Selected = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._checkEnable = true;
            this._onMouseDown = false;
            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                this._keyCheck = true;
            else
                base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                this._keyCheck = false;
        }
    }
}