#pragma once

#include "AIOItems.h"
#include "TVDoc.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

	/// <summary>
	/// Summary for IgnoreEdit
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class IgnoreEdit : public System::Windows::Forms::Form
	{
		TVDoc ^mDoc;
		IgnoreList ^Ignore;
		IgnoreList ^DisplayedSet;


	public:
		IgnoreEdit(TVDoc ^doc)
		{
			mDoc = doc;
			Ignore = gcnew IgnoreList();

			for each (IgnoreItem ^ii in mDoc->Ignore)
				Ignore->Add(ii);

			InitializeComponent();

			FillList();
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~IgnoreEdit()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::Button^  bnOK;
	protected: 
	private: System::Windows::Forms::Button^  bnCancel;
	private: System::Windows::Forms::ListBox^  lbItems;

	private: System::Windows::Forms::Button^  bnRemove;
	private: System::Windows::Forms::Label^  label1;
	private: System::Windows::Forms::TextBox^  txtFilter;
	private: System::Windows::Forms::Timer^  timer1;
	private: System::ComponentModel::IContainer^  components;


	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(IgnoreEdit::typeid));
			this->bnOK = (gcnew System::Windows::Forms::Button());
			this->bnCancel = (gcnew System::Windows::Forms::Button());
			this->lbItems = (gcnew System::Windows::Forms::ListBox());
			this->bnRemove = (gcnew System::Windows::Forms::Button());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->txtFilter = (gcnew System::Windows::Forms::TextBox());
			this->timer1 = (gcnew System::Windows::Forms::Timer(this->components));
			this->SuspendLayout();
			// 
			// bnOK
			// 
			this->bnOK->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnOK->Location = System::Drawing::Point(216, 425);
			this->bnOK->Name = L"bnOK";
			this->bnOK->Size = System::Drawing::Size(75, 23);
			this->bnOK->TabIndex = 0;
			this->bnOK->Text = L"OK";
			this->bnOK->UseVisualStyleBackColor = true;
			this->bnOK->Click += gcnew System::EventHandler(this, &IgnoreEdit::bnOK_Click);
			// 
			// bnCancel
			// 
			this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnCancel->Location = System::Drawing::Point(297, 425);
			this->bnCancel->Name = L"bnCancel";
			this->bnCancel->Size = System::Drawing::Size(75, 23);
			this->bnCancel->TabIndex = 1;
			this->bnCancel->Text = L"Cancel";
			this->bnCancel->UseVisualStyleBackColor = true;
			// 
			// lbItems
			// 
			this->lbItems->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lbItems->FormattingEnabled = true;
			this->lbItems->IntegralHeight = false;
			this->lbItems->Location = System::Drawing::Point(9, 38);
			this->lbItems->Name = L"lbItems";
			this->lbItems->ScrollAlwaysVisible = true;
			this->lbItems->SelectionMode = System::Windows::Forms::SelectionMode::MultiExtended;
			this->lbItems->Size = System::Drawing::Size(363, 381);
			this->lbItems->Sorted = true;
			this->lbItems->TabIndex = 2;
			// 
			// bnRemove
			// 
			this->bnRemove->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnRemove->Location = System::Drawing::Point(9, 425);
			this->bnRemove->Name = L"bnRemove";
			this->bnRemove->Size = System::Drawing::Size(75, 23);
			this->bnRemove->TabIndex = 0;
			this->bnRemove->Text = L"&Remove";
			this->bnRemove->UseVisualStyleBackColor = true;
			this->bnRemove->Click += gcnew System::EventHandler(this, &IgnoreEdit::bnRemove_Click);
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(6, 15);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(32, 13);
			this->label1->TabIndex = 3;
			this->label1->Text = L"Filter:";
			// 
			// txtFilter
			// 
			this->txtFilter->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->txtFilter->Location = System::Drawing::Point(44, 12);
			this->txtFilter->Name = L"txtFilter";
			this->txtFilter->Size = System::Drawing::Size(328, 20);
			this->txtFilter->TabIndex = 4;
			this->txtFilter->TextChanged += gcnew System::EventHandler(this, &IgnoreEdit::txtFilter_TextChanged);
			// 
			// timer1
			// 
			this->timer1->Interval = 500;
			this->timer1->Tick += gcnew System::EventHandler(this, &IgnoreEdit::timer1_Tick);
			// 
			// IgnoreEdit
			// 
			this->AcceptButton = this->bnOK;
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnCancel;
			this->ClientSize = System::Drawing::Size(384, 460);
			this->Controls->Add(this->txtFilter);
			this->Controls->Add(this->label1);
			this->Controls->Add(this->lbItems);
			this->Controls->Add(this->bnCancel);
			this->Controls->Add(this->bnRemove);
			this->Controls->Add(this->bnOK);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"IgnoreEdit";
			this->ShowInTaskbar = false;
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = L"Edit Ignore List";
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
	private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 mDoc->Ignore = Ignore;
				 mDoc->SetDirty();
				 this->Close();
			 }
private: System::Void bnRemove_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 for each (int i in lbItems->SelectedIndices)
				 Ignore->Remove(DisplayedSet[i]);
			 FillList();
		 }
		 void FillList()
		 {
			 lbItems->BeginUpdate();
			 lbItems->Items->Clear();

			 String ^f = txtFilter->Text->ToLower();
			 bool all = String::IsNullOrEmpty(f);

			 DisplayedSet = gcnew IgnoreList();

			 for each (IgnoreItem ^ii in Ignore)
			 {
				 String ^s = ii->FileAndPath;
				 if (all || s->ToLower()->Contains(f))
				 {
					 lbItems->Items->Add(s);
					 DisplayedSet->Add(ii);
				 }
			 }

			 lbItems->EndUpdate();
		 }
private: System::Void txtFilter_TextChanged(System::Object^  sender, System::EventArgs^  e) 
		 {
			 timer1->Start();
		 }
private: System::Void timer1_Tick(System::Object^  sender, System::EventArgs^  e) 
		 {
			 timer1->Stop();
			 FillList();
		 }
};
}
