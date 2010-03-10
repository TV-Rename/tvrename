//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
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
	public class MyListView : System.Windows.Forms.ListView
	{
		private bool _checkEnable;
		private bool _onMouseDown;
		private bool _menuCheck;
		private bool _keyCheck;
		public MyListView()
		{
			_keyCheck = false;
			_checkEnable = true;
			_onMouseDown = false;
			_menuCheck = false;
		}

#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>

#endregion
		protected override void OnMouseDown (MouseEventArgs e)
		{
			_onMouseDown = true;
			base.OnMouseDown(e);
		}
		protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
		{
			if (true == _onMouseDown)
			{
				_checkEnable = false;
			}
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
				this.Items[this.SelectedIndices[0]].Selected = false;
				this.Items[ice.Index].Selected = true;
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
			{
				_keyCheck = true;
			}
			else
				base.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				_keyCheck = false;
			}
		}

	} // mylistview
}