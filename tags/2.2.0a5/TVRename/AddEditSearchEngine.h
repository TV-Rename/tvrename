#pragma once

#include "Searchers.h"
#include "CustomNameTagsFloatingWindow.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace SourceGrid;

namespace TVRename {

	/// <summary>
	/// Summary for AddEditSearchEngine
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class AddEditSearchEngine : public System::Windows::Forms::Form
	{
		Searchers  ^mSearchers;
		CustomNameTagsFloatingWindow ^Cntfw;
		SourceGrid::Grid^ Grid1;
		//array<SourceGrid::Cells::Editors::EditorBase ^> ^MyEditors;

		ProcessedEpisode ^SampleEpisode;

	public:
		AddEditSearchEngine(Searchers ^s, ProcessedEpisode ^pe)
		{
			SampleEpisode = pe;
			InitializeComponent();
			Cntfw = nullptr;

			SetupGrid();
			mSearchers = s;

			for (int i=0;i<mSearchers->Count();i++)
			{
				AddNewRow();
				Grid1[i+1,0]->Value = mSearchers->Name(i);
				Grid1[i+1,1]->Value = mSearchers->URL(i);
			}
		}

		void SetupGrid()
		{
			SourceGrid::Cells::Views::Cell ^titleModel = gcnew SourceGrid::Cells::Views::Cell();
			titleModel->BackColor = Color::SteelBlue;
			titleModel->ForeColor = Color::White;
			titleModel->TextAlignment = DevAge::Drawing::ContentAlignment::MiddleLeft;

			Grid1->Columns->Clear();
			Grid1->Rows->Clear();

			Grid1->RowsCount = 1;
			Grid1->ColumnsCount = 2;
			Grid1->FixedRows = 1;
			Grid1->FixedColumns = 0;
			Grid1->Selection->EnableMultiSelection = false;

			Grid1->Columns[0]->AutoSizeMode = SourceGrid::AutoSizeMode::None;
			Grid1->Columns[0]->Width = 80;

			Grid1->Columns[1]->AutoSizeMode = SourceGrid::AutoSizeMode::EnableAutoSize | SourceGrid::AutoSizeMode::EnableStretch;

			Grid1->AutoStretchColumnsToFitWidth = true;
			//Grid1->AutoSizeCells();
			Grid1->Columns->StretchToFit();

			//////////////////////////////////////////////////////////////////////
			// header row

			SourceGrid::Cells::ColumnHeader ^h;
			h = gcnew SourceGrid::Cells::ColumnHeader("Name");
			h->AutomaticSortEnabled = false;
			Grid1[0,0] = h;
			Grid1[0,0]->View = titleModel;

			h = gcnew SourceGrid::Cells::ColumnHeader("URL");
			h->AutomaticSortEnabled = false;
			Grid1[0,1] = h;
			Grid1[0,1]->View = titleModel;
		}

		void AddNewRow()
		{
			int r = Grid1->RowsCount;
			Grid1->RowsCount = r + 1;

			Grid1[r, 0] = gcnew SourceGrid::Cells::Cell("", (gcnew String(""))->GetType());
			Grid1[r, 1] = gcnew SourceGrid::Cells::Cell("", (gcnew String(""))->GetType());
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~AddEditSearchEngine()
		{
			if (Cntfw != nullptr)
				Cntfw->Close();
			if (components)
			{
				delete components;
			}
		}

	private: System::Windows::Forms::Button^  bnAdd;
	private: System::Windows::Forms::Button^  bnDelete;
	private: System::Windows::Forms::Button^  bnCancel;
	private: System::Windows::Forms::Button^  bnOK;
	private: System::Windows::Forms::Button^  bnTags;

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
		void InitializeComponent(void)
		{
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(AddEditSearchEngine::typeid));
			this->bnAdd = (gcnew System::Windows::Forms::Button());
			this->bnDelete = (gcnew System::Windows::Forms::Button());
			this->bnCancel = (gcnew System::Windows::Forms::Button());
			this->bnOK = (gcnew System::Windows::Forms::Button());
			this->bnTags = (gcnew System::Windows::Forms::Button());
			this->Grid1 = (gcnew SourceGrid::Grid());
			this->SuspendLayout();
			// 
			// bnAdd
			// 
			this->bnAdd->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnAdd->Location = System::Drawing::Point(12, 343);
			this->bnAdd->Name = L"bnAdd";
			this->bnAdd->Size = System::Drawing::Size(75, 23);
			this->bnAdd->TabIndex = 1;
			this->bnAdd->Text = L"&Add";
			this->bnAdd->UseVisualStyleBackColor = true;
			this->bnAdd->Click += gcnew System::EventHandler(this, &AddEditSearchEngine::bnAdd_Click);
			// 
			// bnDelete
			// 
			this->bnDelete->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnDelete->Location = System::Drawing::Point(93, 343);
			this->bnDelete->Name = L"bnDelete";
			this->bnDelete->Size = System::Drawing::Size(75, 23);
			this->bnDelete->TabIndex = 1;
			this->bnDelete->Text = L"&Delete";
			this->bnDelete->UseVisualStyleBackColor = true;
			this->bnDelete->Click += gcnew System::EventHandler(this, &AddEditSearchEngine::bnDelete_Click);
			// 
			// bnCancel
			// 
			this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnCancel->Location = System::Drawing::Point(601, 343);
			this->bnCancel->Name = L"bnCancel";
			this->bnCancel->Size = System::Drawing::Size(75, 23);
			this->bnCancel->TabIndex = 2;
			this->bnCancel->Text = L"Cancel";
			this->bnCancel->UseVisualStyleBackColor = true;
			// 
			// bnOK
			// 
			this->bnOK->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnOK->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->bnOK->Location = System::Drawing::Point(520, 343);
			this->bnOK->Name = L"bnOK";
			this->bnOK->Size = System::Drawing::Size(75, 23);
			this->bnOK->TabIndex = 2;
			this->bnOK->Text = L"OK";
			this->bnOK->UseVisualStyleBackColor = true;
			this->bnOK->Click += gcnew System::EventHandler(this, &AddEditSearchEngine::bnOK_Click);
			// 
			// bnTags
			// 
			this->bnTags->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnTags->Location = System::Drawing::Point(189, 343);
			this->bnTags->Name = L"bnTags";
			this->bnTags->Size = System::Drawing::Size(75, 23);
			this->bnTags->TabIndex = 3;
			this->bnTags->Text = L"Tags...";
			this->bnTags->UseVisualStyleBackColor = true;
			this->bnTags->Click += gcnew System::EventHandler(this, &AddEditSearchEngine::bnTags_Click);
			// 
			// Grid1
			// 
			this->Grid1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->Grid1->BackColor = System::Drawing::SystemColors::Window;
			this->Grid1->Location = System::Drawing::Point(12, 12);
			this->Grid1->Name = L"Grid1";
			this->Grid1->OptimizeMode = SourceGrid::CellOptimizeMode::ForRows;
			this->Grid1->SelectionMode = SourceGrid::GridSelectionMode::Cell;
			this->Grid1->Size = System::Drawing::Size(664, 321);
			this->Grid1->TabIndex = 4;
			this->Grid1->TabStop = true;
			this->Grid1->ToolTipText = L"";
			// 
			// AddEditSearchEngine
			// 
			this->AcceptButton = this->bnOK;
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnCancel;
			this->ClientSize = System::Drawing::Size(688, 378);
			this->Controls->Add(this->Grid1);
			this->Controls->Add(this->bnTags);
			this->Controls->Add(this->bnOK);
			this->Controls->Add(this->bnCancel);
			this->Controls->Add(this->bnDelete);
			this->Controls->Add(this->bnAdd);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"AddEditSearchEngine";
			this->ShowInTaskbar = false;
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = L"Modify Search Engines";
			this->ResumeLayout(false);

		}
#pragma endregion
	private: System::Void bnAdd_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 AddNewRow();
				 Grid1->Selection->Focus(SourceGrid::Position(Grid1->RowsCount-1,1), true);
			 }
	private: System::Void bnDelete_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 // multiselection is off, so we can cheat...
				 array<int> ^rowsIndex = Grid1->Selection->GetSelectionRegion()->GetRowsIndex();
				 if (rowsIndex->Length)
					 Grid1->Rows->Remove(rowsIndex[0]);
			 }
	private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 mSearchers->Clear();
				 for (int i=1;i<Grid1->RowsCount;i++) // skip header row
				 {
					 String ^name = safe_cast<String ^>(Grid1[i,0]->Value);
					 String ^url = safe_cast<String ^>(Grid1[i,1]->Value);
					 if (!String::IsNullOrEmpty(name) && !String::IsNullOrEmpty(url))
						 mSearchers->Add(name, url);
				 }
			 }
	private: System::Void bnTags_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 Cntfw = gcnew CustomNameTagsFloatingWindow(SampleEpisode);
				 Cntfw->Show(this);
				 this->Focus();
			 }
	};
}
