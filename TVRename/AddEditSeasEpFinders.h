//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "Settings.h"
#include "ShowItem.h"
#include "TVDoc.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

    /// <summary>
    /// Summary for AddEditSeasEpFinders
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public ref class AddEditSeasEpFinders : public System::Windows::Forms::Form
    {
        FNPRegexList ^Rex;
    private: System::Windows::Forms::Label^  label2;
    private: System::Windows::Forms::TextBox^  txtFolder;
    private: System::Windows::Forms::Timer^  tmrFillPreview;
             ShowItemList ^SIL;
    private: System::Windows::Forms::CheckBox^  chkTestAll;
    private: System::Windows::Forms::Button^  bnDefaults;
	private: SourceGrid::Grid^  Grid1;

             TVSettings ^TheSettings;

    public:
        AddEditSeasEpFinders(FNPRegexList ^rex, ShowItemList ^sil, ShowItem ^initialShow, 
            String ^initialFolder, TVSettings ^s)
        {
            Rex = rex;
            SIL = sil;
            TheSettings = s;

            InitializeComponent();

			SetupGrid();
            FillGrid(Rex);
        
            for each (ShowItem ^si in SIL)
            {
                cbShowList->Items->Add(si->ShowName());
                if (si == initialShow)
                    cbShowList->SelectedIndex = cbShowList->Items->Count - 1;
            }
            txtFolder->Text = initialFolder;
        }

		
		void SetupGrid()
		{
			SourceGrid::Cells::Views::Cell ^titleModel = gcnew SourceGrid::Cells::Views::Cell();
			titleModel->BackColor = Color::SteelBlue;
			titleModel->ForeColor = Color::White;
			titleModel->TextAlignment = DevAge::Drawing::ContentAlignment::MiddleLeft;

			SourceGrid::Cells::Views::Cell ^titleModelC = gcnew SourceGrid::Cells::Views::Cell();
			titleModelC->BackColor = Color::SteelBlue;
			titleModelC->ForeColor = Color::White;
			titleModelC->TextAlignment = DevAge::Drawing::ContentAlignment::MiddleCenter;

			Grid1->Columns->Clear();
			Grid1->Rows->Clear();

			Grid1->RowsCount = 1;
			Grid1->ColumnsCount = 4;
			Grid1->FixedRows = 1;
			Grid1->FixedColumns = 0;
			Grid1->Selection->EnableMultiSelection = false;

			Grid1->Columns[0]->AutoSizeMode = SourceGrid::AutoSizeMode::None;
			Grid1->Columns[0]->Width = 60;
			Grid1->Columns[1]->AutoSizeMode = SourceGrid::AutoSizeMode::EnableAutoSize | SourceGrid::AutoSizeMode::EnableStretch;
			Grid1->Columns[2]->AutoSizeMode = SourceGrid::AutoSizeMode::None;
			Grid1->Columns[2]->Width = 60;
			Grid1->Columns[3]->AutoSizeMode = SourceGrid::AutoSizeMode::EnableAutoSize | SourceGrid::AutoSizeMode::EnableStretch;
			
			Grid1->AutoStretchColumnsToFitWidth = true;
			Grid1->Columns->StretchToFit();

			//////////////////////////////////////////////////////////////////////
			// header row

			SourceGrid::Cells::ColumnHeader ^h;
			h = gcnew SourceGrid::Cells::ColumnHeader("Enabled");
			h->AutomaticSortEnabled = false;
			Grid1[0,0] = h;
			Grid1[0,0]->View = titleModelC;
			

			h = gcnew SourceGrid::Cells::ColumnHeader("Regex");
			h->AutomaticSortEnabled = false;
			Grid1[0,1] = h;
			Grid1[0,1]->View = titleModel;

			h = gcnew SourceGrid::Cells::ColumnHeader("Full Path");
			h->AutomaticSortEnabled = false;
			Grid1[0,2] = h;
			Grid1[0,2]->View = titleModelC;

			h = gcnew SourceGrid::Cells::ColumnHeader("Notes");
			h->AutomaticSortEnabled = false;
			Grid1[0,3] = h;
			Grid1[0,3]->View = titleModel;

			Grid1->Selection->SelectionChanged += gcnew SourceGrid::RangeRegionChangedEventHandler(this, &AddEditSeasEpFinders::SelectionChanged);
		}

		void SelectionChanged(Object ^sender, SourceGrid::RangeRegionChangedEventArgs ^e)
		{
			StartTimer();
		}

		void AddNewRow()
		{
			int r = Grid1->RowsCount;
			Grid1->RowsCount = r + 1;

			Grid1[r, 0] = gcnew SourceGrid::Cells::CheckBox(nullptr, true);
			Grid1[r, 1] = gcnew SourceGrid::Cells::Cell("", (gcnew String(""))->GetType());
			Grid1[r, 2] = gcnew SourceGrid::Cells::CheckBox(nullptr, false);
			Grid1[r, 3] = gcnew SourceGrid::Cells::Cell("", (gcnew String(""))->GetType());

			ChangedCont ^changed = gcnew ChangedCont(this);
			for (int c=0;c<4;c++)
				Grid1[r,c]->AddController(changed);
		}

			public:
		ref class ChangedCont : 
		public SourceGrid::Cells::Controllers::ControllerBase
		{
		private:
			AddEditSeasEpFinders ^P;

		public: 
			ChangedCont(AddEditSeasEpFinders ^p)
			{
				P = p;
			}

			virtual void OnValueChanged(SourceGrid::CellContext sender, EventArgs ^e) override
			{
				P->StartTimer();
			}
		};


        void FillGrid(FNPRegexList ^list)
        {
			while (Grid1->Rows->Count > 1) // leave header row
				Grid1->Rows->Remove(1);

			Grid1->RowsCount = list->Count+1;

            int i=1;
            for each (FilenameProcessorRE ^re in list)
            {
				Grid1[i, 0] = gcnew SourceGrid::Cells::CheckBox(nullptr, re->Enabled);
				Grid1[i, 1] = gcnew SourceGrid::Cells::Cell(re->RE, (gcnew String(""))->GetType());
				Grid1[i, 2] = gcnew SourceGrid::Cells::CheckBox(nullptr, re->UseFullPath);
				Grid1[i, 3] = gcnew SourceGrid::Cells::Cell(re->Notes, (gcnew String(""))->GetType());

				ChangedCont ^changed = gcnew ChangedCont(this);

				for (int c=0;c<4;c++)
					Grid1[i,c]->AddController(changed);
				
				i++;
            }
            StartTimer();
        }

    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~AddEditSeasEpFinders()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::Button^  bnOK;
    protected: 
    private: System::Windows::Forms::Button^  bnCancel;
    private: System::Windows::Forms::Button^  bnDelete;
    private: System::Windows::Forms::Button^  bnAdd;
    private: System::Windows::Forms::ListView^  lvPreview;
    private: System::Windows::Forms::Button^  bnBrowse;
    private: System::Windows::Forms::ColumnHeader^  columnHeader1;
    private: System::Windows::Forms::FolderBrowserDialog^  folderBrowser;
    private: System::Windows::Forms::ColumnHeader^  columnHeader2;
    private: System::Windows::Forms::ColumnHeader^  columnHeader3;
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::ComboBox^  cbShowList;
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
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(AddEditSeasEpFinders::typeid));
			this->bnOK = (gcnew System::Windows::Forms::Button());
			this->bnCancel = (gcnew System::Windows::Forms::Button());
			this->bnDelete = (gcnew System::Windows::Forms::Button());
			this->bnAdd = (gcnew System::Windows::Forms::Button());
			this->lvPreview = (gcnew System::Windows::Forms::ListView());
			this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader3 = (gcnew System::Windows::Forms::ColumnHeader());
			this->bnBrowse = (gcnew System::Windows::Forms::Button());
			this->folderBrowser = (gcnew System::Windows::Forms::FolderBrowserDialog());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->cbShowList = (gcnew System::Windows::Forms::ComboBox());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->txtFolder = (gcnew System::Windows::Forms::TextBox());
			this->tmrFillPreview = (gcnew System::Windows::Forms::Timer(this->components));
			this->chkTestAll = (gcnew System::Windows::Forms::CheckBox());
			this->bnDefaults = (gcnew System::Windows::Forms::Button());
			this->Grid1 = (gcnew SourceGrid::Grid());
			this->SuspendLayout();
			// 
			// bnOK
			// 
			this->bnOK->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnOK->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->bnOK->Location = System::Drawing::Point(741, 560);
			this->bnOK->Name = L"bnOK";
			this->bnOK->Size = System::Drawing::Size(75, 23);
			this->bnOK->TabIndex = 6;
			this->bnOK->Text = L"OK";
			this->bnOK->UseVisualStyleBackColor = true;
			this->bnOK->Click += gcnew System::EventHandler(this, &AddEditSeasEpFinders::bnOK_Click);
			// 
			// bnCancel
			// 
			this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnCancel->Location = System::Drawing::Point(822, 560);
			this->bnCancel->Name = L"bnCancel";
			this->bnCancel->Size = System::Drawing::Size(75, 23);
			this->bnCancel->TabIndex = 7;
			this->bnCancel->Text = L"Cancel";
			this->bnCancel->UseVisualStyleBackColor = true;
			// 
			// bnDelete
			// 
			this->bnDelete->Location = System::Drawing::Point(93, 266);
			this->bnDelete->Name = L"bnDelete";
			this->bnDelete->Size = System::Drawing::Size(75, 23);
			this->bnDelete->TabIndex = 5;
			this->bnDelete->Text = L"&Delete";
			this->bnDelete->UseVisualStyleBackColor = true;
			this->bnDelete->Click += gcnew System::EventHandler(this, &AddEditSeasEpFinders::bnDelete_Click);
			// 
			// bnAdd
			// 
			this->bnAdd->Location = System::Drawing::Point(12, 266);
			this->bnAdd->Name = L"bnAdd";
			this->bnAdd->Size = System::Drawing::Size(75, 23);
			this->bnAdd->TabIndex = 4;
			this->bnAdd->Text = L"&Add";
			this->bnAdd->UseVisualStyleBackColor = true;
			this->bnAdd->Click += gcnew System::EventHandler(this, &AddEditSeasEpFinders::bnAdd_Click);
			// 
			// lvPreview
			// 
			this->lvPreview->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvPreview->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(3) {this->columnHeader1, this->columnHeader2, 
				this->columnHeader3});
			this->lvPreview->HeaderStyle = System::Windows::Forms::ColumnHeaderStyle::Nonclickable;
			this->lvPreview->Location = System::Drawing::Point(12, 295);
			this->lvPreview->Name = L"lvPreview";
			this->lvPreview->Size = System::Drawing::Size(885, 259);
			this->lvPreview->Sorting = System::Windows::Forms::SortOrder::Ascending;
			this->lvPreview->TabIndex = 8;
			this->lvPreview->UseCompatibleStateImageBehavior = false;
			this->lvPreview->View = System::Windows::Forms::View::Details;
			// 
			// columnHeader1
			// 
			this->columnHeader1->Text = L"Filename";
			this->columnHeader1->Width = 335;
			// 
			// columnHeader2
			// 
			this->columnHeader2->Text = L"Season";
			// 
			// columnHeader3
			// 
			this->columnHeader3->Text = L"Episode";
			// 
			// bnBrowse
			// 
			this->bnBrowse->Location = System::Drawing::Point(591, 266);
			this->bnBrowse->Name = L"bnBrowse";
			this->bnBrowse->Size = System::Drawing::Size(75, 23);
			this->bnBrowse->TabIndex = 9;
			this->bnBrowse->Text = L"&Browse...";
			this->bnBrowse->UseVisualStyleBackColor = true;
			this->bnBrowse->Click += gcnew System::EventHandler(this, &AddEditSeasEpFinders::bnBrowse_Click);
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(672, 271);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(37, 13);
			this->label1->TabIndex = 10;
			this->label1->Text = L"Show:";
			// 
			// cbShowList
			// 
			this->cbShowList->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
			this->cbShowList->FormattingEnabled = true;
			this->cbShowList->Location = System::Drawing::Point(715, 268);
			this->cbShowList->Name = L"cbShowList";
			this->cbShowList->Size = System::Drawing::Size(182, 21);
			this->cbShowList->TabIndex = 11;
			this->cbShowList->SelectedIndexChanged += gcnew System::EventHandler(this, &AddEditSeasEpFinders::cbShowList_SelectedIndexChanged);
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Location = System::Drawing::Point(322, 271);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(63, 13);
			this->label2->TabIndex = 10;
			this->label2->Text = L"Test Folder:";
			// 
			// txtFolder
			// 
			this->txtFolder->Location = System::Drawing::Point(391, 268);
			this->txtFolder->Name = L"txtFolder";
			this->txtFolder->Size = System::Drawing::Size(194, 20);
			this->txtFolder->TabIndex = 12;
			this->txtFolder->TextChanged += gcnew System::EventHandler(this, &AddEditSeasEpFinders::txtFolder_TextChanged);
			// 
			// tmrFillPreview
			// 
			this->tmrFillPreview->Interval = 500;
			this->tmrFillPreview->Tick += gcnew System::EventHandler(this, &AddEditSeasEpFinders::tmrFillPreview_Tick);
			// 
			// chkTestAll
			// 
			this->chkTestAll->AutoSize = true;
			this->chkTestAll->Checked = true;
			this->chkTestAll->CheckState = System::Windows::Forms::CheckState::Checked;
			this->chkTestAll->Location = System::Drawing::Point(255, 270);
			this->chkTestAll->Name = L"chkTestAll";
			this->chkTestAll->Size = System::Drawing::Size(61, 17);
			this->chkTestAll->TabIndex = 13;
			this->chkTestAll->Text = L"Test All";
			this->chkTestAll->UseVisualStyleBackColor = true;
			this->chkTestAll->CheckedChanged += gcnew System::EventHandler(this, &AddEditSeasEpFinders::chkTestAll_CheckedChanged);
			// 
			// bnDefaults
			// 
			this->bnDefaults->Location = System::Drawing::Point(174, 266);
			this->bnDefaults->Name = L"bnDefaults";
			this->bnDefaults->Size = System::Drawing::Size(75, 23);
			this->bnDefaults->TabIndex = 14;
			this->bnDefaults->Text = L"D&efaults";
			this->bnDefaults->UseVisualStyleBackColor = true;
			this->bnDefaults->Click += gcnew System::EventHandler(this, &AddEditSeasEpFinders::bnDefaults_Click);
			// 
			// Grid1
			// 
			this->Grid1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->Grid1->BackColor = System::Drawing::SystemColors::Window;
			this->Grid1->Location = System::Drawing::Point(13, 13);
			this->Grid1->Name = L"Grid1";
			this->Grid1->OptimizeMode = SourceGrid::CellOptimizeMode::ForRows;
			this->Grid1->SelectionMode = SourceGrid::GridSelectionMode::Cell;
			this->Grid1->Size = System::Drawing::Size(884, 247);
			this->Grid1->TabIndex = 15;
			this->Grid1->TabStop = true;
			this->Grid1->ToolTipText = L"";
			// 
			// AddEditSeasEpFinders
			// 
			this->AcceptButton = this->bnOK;
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnCancel;
			this->ClientSize = System::Drawing::Size(909, 595);
			this->Controls->Add(this->Grid1);
			this->Controls->Add(this->bnDefaults);
			this->Controls->Add(this->chkTestAll);
			this->Controls->Add(this->txtFolder);
			this->Controls->Add(this->cbShowList);
			this->Controls->Add(this->label2);
			this->Controls->Add(this->label1);
			this->Controls->Add(this->bnBrowse);
			this->Controls->Add(this->lvPreview);
			this->Controls->Add(this->bnOK);
			this->Controls->Add(this->bnCancel);
			this->Controls->Add(this->bnDelete);
			this->Controls->Add(this->bnAdd);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->MinimumSize = System::Drawing::Size(917, 430);
			this->ShowInTaskbar = false;
			this->Text = L"Filename Processors";
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
		FilenameProcessorRE ^REForRow(int i)
		{
			if ((i < 1) || (i >= Grid1->RowsCount)) // row 0 is header
				return nullptr;
			bool en = safe_cast<bool>(Grid1[i,0]->Value);
			String ^regex = safe_cast<String ^>(Grid1[i,1]->Value);
			bool fullPath = safe_cast<bool>(Grid1[i,2]->Value);
			String ^notes = safe_cast<String ^>(Grid1[i,3]->Value);
			if (notes == nullptr)
				notes = "";

			if (String::IsNullOrEmpty(regex))
				return nullptr;
			return gcnew FilenameProcessorRE(en, regex, fullPath, notes);
		}

    private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Rex->Clear();
				 for (int i=1;i<Grid1->RowsCount;i++) // skip header row
                 {
                     FilenameProcessorRE ^re = REForRow(i);
					 if (re != nullptr)
						 Rex->Add(re); 
                 }
             }
    private: System::Void bnAdd_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 AddNewRow();
				 Grid1->Selection->Focus(SourceGrid::Position(Grid1->RowsCount-1,1), true);
                 StartTimer();
             }
    private: System::Void bnDelete_Click(System::Object^  sender, System::EventArgs^  e) 
             {
				 // multiselection is off, so we can cheat...
				 array<int> ^rowsIndex = Grid1->Selection->GetSelectionRegion()->GetRowsIndex();
				 if (rowsIndex->Length)
					 Grid1->Rows->Remove(rowsIndex[0]);

                 StartTimer();
             }
    private: System::Void bnBrowse_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (!String::IsNullOrEmpty(txtFolder->Text))
                     folderBrowser->SelectedPath = txtFolder->Text;

                 if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
                     txtFolder->Text = folderBrowser->SelectedPath;

                 StartTimer();
             }
    private: System::Void cbShowList_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 StartTimer();
             }
    private: System::Void txtFolder_TextChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 StartTimer();
             }
             void StartTimer()
             {
                 lvPreview->Enabled = false;
                 tmrFillPreview->Start();
             }

    private: System::Void tmrFillPreview_Tick(System::Object^  sender, System::EventArgs^  e) 
             {
                 tmrFillPreview->Stop();
                 FillPreview();
             }
    private: System::Void chkTestAll_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 StartTimer();
             }

             void FillPreview()
             {
                 lvPreview->Items->Clear();
                 if ((String::IsNullOrEmpty(txtFolder->Text)) || (!DirectoryInfo(txtFolder->Text).Exists))
                 {
                     txtFolder->BackColor = WarningColor();
                     return;
                 }
                 else
                     txtFolder->BackColor = System::Drawing::SystemColors::Window;

                 if (Grid1->RowsCount <= 1) // 1 for header
                     return; // empty

                 lvPreview->Enabled = true;

                 FNPRegexList ^rel = gcnew FNPRegexList();

                 if (chkTestAll->Checked)
                 {
					 for (int i=1;i<Grid1->RowsCount;i++)
                     {
                         FilenameProcessorRE ^re = REForRow(i);
                         if (re != nullptr)
                             rel->Add(re); 
                     }
                 }
                 else
                 {
					 array<int> ^rowsIndex = Grid1->Selection->GetSelectionRegion()->GetRowsIndex();
					 if (!rowsIndex->Length)
						 return;

                     FilenameProcessorRE ^re2 = REForRow(rowsIndex[0]);
                     if (re2 != nullptr)
                         rel->Add(re2); 
                     else
                         return;
                 }

				 lvPreview->BeginUpdate();
                 for each (FileInfo ^fi in DirectoryInfo(txtFolder->Text).GetFiles())
                 {
                     int seas, ep;

                     if (!TheSettings->UsefulExtension(fi->Extension,true))
                         continue; // move on

                     bool r = TVDoc::FindSeasEp(fi, &seas, &ep, cbShowList->Text, rel);
                     ListViewItem ^lvi = gcnew ListViewItem();
                     lvi->Text = fi->Name;
                     lvi->SubItems->Add((seas == -1) ? "-" : seas.ToString());
                     lvi->SubItems->Add((ep == -1) ? "-" : ep.ToString());
                     if (!r)
                         lvi->BackColor = WarningColor();
                     lvPreview->Items->Add(lvi);
                 }
				 lvPreview->EndUpdate();

             }
    private: System::Void bnDefaults_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 ::DialogResult dr = MessageBox::Show("Restore to default matching expressions?",
                     "Filename Processors",
                     MessageBoxButtons::YesNo, 
                     MessageBoxIcon::Warning);
                 if (dr == ::DialogResult::Yes)
                     FillGrid(TVSettings::DefaultFNPList());
             }
};
}
