//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

// Starting from:
// http://social.msdn.microsoft.com/Forums/ja-JP/csharpexpressja/thread/67475927-015c-4206-b5e7-d67504edb3a1
//
// Standard windows listview has a very weird behaviour when shift-clicking, if it is a multicolumn, 
// full-row-select, and checkboxes on.. and you click in anything other than the first column.

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

	/// <summary>
	/// Summary for MyListView
	/// </summary>
	public ref class MyListView : public System::Windows::Forms::ListView
	{
	private:
		bool _checkEnable;
		bool _onMouseDown;
		bool _menuCheck;
		bool _keyCheck;
	public:
		MyListView(void)
		{
			_keyCheck = false;
			_checkEnable = true;
			_onMouseDown = false;
			_menuCheck = false;
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~MyListView()
		{
			if (components)
			{
				delete components;
			}
		}
	protected: 

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
#pragma endregion

	protected:
		virtual void OnMouseDown (MouseEventArgs ^e) override 
		{
			_onMouseDown = true ;
			System::Windows::Forms::ListView::OnMouseDown(e);
		}
		virtual void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs ^e) override 
		{
			if ( true == _onMouseDown) 
			{
				_checkEnable = false ; 
			}
			System::Windows::Forms::ListView::OnItemSelectionChanged(e); 
		}
		virtual void OnItemCheck(ItemCheckEventArgs ^ice) override 
		{
			if (!_menuCheck && !_keyCheck && ( false == _checkEnable /*|| (!_keyCheck && _checkEnable && SelectedItems->Count > 1)*/)) 
			{
				ice->NewValue = ice->CurrentValue;
				return;
			}
			System::Windows::Forms::ListView::OnItemCheck(ice);
			if (SelectedItems->Count == 1)
			{
				this->Items[this->SelectedIndices[0]]->Selected = false;
				this->Items[ice->Index]->Selected = true;
			}
		}
		virtual void OnMouseUp(MouseEventArgs ^e) override
		{
			_checkEnable = true ; 
			_onMouseDown = false ; 
			System::Windows::Forms::ListView::OnMouseUp(e); 
		}
		virtual void OnKeyDown(KeyEventArgs ^e) override
		{
			if (e->KeyCode == Keys::Space)
			{
				_keyCheck = true;
			}
			else
				ListView::OnKeyDown(e);
		}
		virtual void OnKeyUp(KeyEventArgs ^e) override
		{
			if (e->KeyCode == Keys::Space)
			{
				_keyCheck = false;
			}
		}

	}; // mylistview
}
