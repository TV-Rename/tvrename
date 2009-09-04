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
	private: System::Windows::Forms::DataGridViewCheckBoxColumn^  colEnabled;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  TheName;
	private: System::Windows::Forms::DataGridViewCheckBoxColumn^  Column1;
	private: System::Windows::Forms::DataGridViewTextBoxColumn^  Column4;

             TVSettings ^TheSettings;

    public:
        AddEditSeasEpFinders(FNPRegexList ^rex, ShowItemList ^sil, ShowItem ^initialShow, 
            String ^initialFolder, TVSettings ^s)
        {
            Rex = rex;
            SIL = sil;
            TheSettings = s;

            InitializeComponent();

            FillGrid(Rex);
        
            for each (ShowItem ^si in SIL)
            {
                cbShowList->Items->Add(si->ShowName());
                if (si == initialShow)
                    cbShowList->SelectedIndex = cbShowList->Items->Count - 1;
            }
            txtFolder->Text = initialFolder;
        }

        void FillGrid(FNPRegexList ^list)
        {
            dgv->Rows->Clear();
            dgv->Rows->Add(list->Count);
            int i=0;
            for each (FilenameProcessorRE ^re in list)
            {
				dgv->Rows[i]->Cells[0]->Value = re->Enabled;
                dgv->Rows[i]->Cells[1]->Value = re->RE;
                dgv->Rows[i]->Cells[2]->Value = re->UseFullPath;
                dgv->Rows[i]->Cells[3]->Value = re->Notes;
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


    private: System::Windows::Forms::DataGridView^  dgv;
    private: System::Windows::Forms::DataGridViewTextBoxColumn^  Name;
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
			this->dgv = (gcnew System::Windows::Forms::DataGridView());
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
			this->colEnabled = (gcnew System::Windows::Forms::DataGridViewCheckBoxColumn());
			this->TheName = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			this->Column1 = (gcnew System::Windows::Forms::DataGridViewCheckBoxColumn());
			this->Column4 = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dgv))->BeginInit();
			this->SuspendLayout();
			// 
			// bnOK
			// 
			this->bnOK->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnOK->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->bnOK->Location = System::Drawing::Point(749, 534);
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
			this->bnCancel->Location = System::Drawing::Point(830, 534);
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
			// dgv
			// 
			this->dgv->AllowUserToAddRows = false;
			this->dgv->AllowUserToDeleteRows = false;
			this->dgv->AllowUserToResizeRows = false;
			this->dgv->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->dgv->AutoSizeColumnsMode = System::Windows::Forms::DataGridViewAutoSizeColumnsMode::AllCells;
			this->dgv->ColumnHeadersHeightSizeMode = System::Windows::Forms::DataGridViewColumnHeadersHeightSizeMode::AutoSize;
			this->dgv->Columns->AddRange(gcnew cli::array< System::Windows::Forms::DataGridViewColumn^  >(4) {this->colEnabled, this->TheName, 
				this->Column1, this->Column4});
			this->dgv->Location = System::Drawing::Point(12, 12);
			this->dgv->MultiSelect = false;
			this->dgv->Name = L"dgv";
			this->dgv->RowHeadersVisible = false;
			this->dgv->Size = System::Drawing::Size(893, 248);
			this->dgv->TabIndex = 3;
			this->dgv->CellValueChanged += gcnew System::Windows::Forms::DataGridViewCellEventHandler(this, &AddEditSeasEpFinders::dgv_CellValueChanged);
			this->dgv->SelectionChanged += gcnew System::EventHandler(this, &AddEditSeasEpFinders::dgv_SelectionChanged);
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
			this->lvPreview->Size = System::Drawing::Size(893, 233);
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
			this->bnBrowse->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->bnBrowse->Location = System::Drawing::Point(598, 266);
			this->bnBrowse->Name = L"bnBrowse";
			this->bnBrowse->Size = System::Drawing::Size(75, 23);
			this->bnBrowse->TabIndex = 9;
			this->bnBrowse->Text = L"&Browse...";
			this->bnBrowse->UseVisualStyleBackColor = true;
			this->bnBrowse->Click += gcnew System::EventHandler(this, &AddEditSeasEpFinders::bnBrowse_Click);
			// 
			// label1
			// 
			this->label1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(679, 271);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(37, 13);
			this->label1->TabIndex = 10;
			this->label1->Text = L"Show:";
			// 
			// cbShowList
			// 
			this->cbShowList->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->cbShowList->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
			this->cbShowList->FormattingEnabled = true;
			this->cbShowList->Location = System::Drawing::Point(722, 268);
			this->cbShowList->Name = L"cbShowList";
			this->cbShowList->Size = System::Drawing::Size(183, 21);
			this->cbShowList->TabIndex = 11;
			this->cbShowList->SelectedIndexChanged += gcnew System::EventHandler(this, &AddEditSeasEpFinders::cbShowList_SelectedIndexChanged);
			// 
			// label2
			// 
			this->label2->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->label2->AutoSize = true;
			this->label2->Location = System::Drawing::Point(329, 271);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(63, 13);
			this->label2->TabIndex = 10;
			this->label2->Text = L"Test Folder:";
			// 
			// txtFolder
			// 
			this->txtFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->txtFolder->Location = System::Drawing::Point(398, 268);
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
			this->chkTestAll->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->chkTestAll->AutoSize = true;
			this->chkTestAll->Checked = true;
			this->chkTestAll->CheckState = System::Windows::Forms::CheckState::Checked;
			this->chkTestAll->Location = System::Drawing::Point(262, 270);
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
			// colEnabled
			// 
			this->colEnabled->HeaderText = L"Enabled";
			this->colEnabled->Name = L"colEnabled";
			this->colEnabled->Width = 52;
			// 
			// TheName
			// 
			this->TheName->AutoSizeMode = System::Windows::Forms::DataGridViewAutoSizeColumnMode::Fill;
			this->TheName->FillWeight = 300;
			this->TheName->HeaderText = L"Regex";
			this->TheName->Name = L"TheName";
			// 
			// Column1
			// 
			this->Column1->AutoSizeMode = System::Windows::Forms::DataGridViewAutoSizeColumnMode::Fill;
			this->Column1->HeaderText = L"Use Full Path";
			this->Column1->Name = L"Column1";
			this->Column1->Resizable = System::Windows::Forms::DataGridViewTriState::True;
			// 
			// Column4
			// 
			this->Column4->AutoSizeMode = System::Windows::Forms::DataGridViewAutoSizeColumnMode::Fill;
			this->Column4->FillWeight = 300;
			this->Column4->HeaderText = L"Notes";
			this->Column4->Name = L"Column4";
			// 
			// AddEditSeasEpFinders
			// 
			this->AcceptButton = this->bnOK;
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(917, 569);
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
			this->Controls->Add(this->dgv);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->ShowInTaskbar = false;
			this->Text = L"Filename Processors";
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dgv))->EndInit();
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
    private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Rex->Clear();
                 for each (DataGridViewRow ^dgvr in dgv->Rows)
                 {
                     FilenameProcessorRE ^re = REFor(dgvr);
                     if (re != nullptr)
                         Rex->Add(re); 
                 }
             }
    private: System::Void bnAdd_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 dgv->Rows->Add();
                 StartTimer();
             }
    private: System::Void bnDelete_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 dgv->Rows->Remove(dgv->CurrentRow);
                 StartTimer();
             }
    private: System::Void bnBrowse_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (txtFolder->Text != "")
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
    private: System::Void dgv_SelectionChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 StartTimer();
             }
    private: System::Void chkTestAll_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 StartTimer();
             }
             FilenameProcessorRE ^REFor(DataGridViewRow ^dgvr)
             {
                 if (dgvr->Cells[1]->Value == nullptr) 
                     return nullptr;

				 bool enabled = (dgvr->Cells[0]->Value == nullptr) ? false : (bool)dgvr->Cells[0]->Value;
                 bool useFolders = (dgvr->Cells[2]->Value == nullptr) ? false : (bool)dgvr->Cells[2]->Value;
                 String ^notes = (dgvr->Cells[3]->Value == nullptr) ? "" : dgvr->Cells[3]->Value->ToString();

                 String ^re = dgvr->Cells[1]->Value->ToString();
                 if (re != "")
                     return gcnew FilenameProcessorRE(enabled, re, useFolders, notes);
                 else
                     return nullptr;
             }
             void FillPreview()
             {
                 lvPreview->Items->Clear();
                 if ((txtFolder->Text == "") || (!DirectoryInfo(txtFolder->Text).Exists))
                 {
                     txtFolder->BackColor = WarningColor();
                     return;
                 }
                 else
                     txtFolder->BackColor = System::Drawing::SystemColors::Window;

                 if (dgv->Rows->Count == 0)
                     return;

                 lvPreview->Enabled = true;

                 FNPRegexList ^rel = gcnew FNPRegexList();

                 if (chkTestAll->Checked)
                 {
                     for each (DataGridViewRow ^dgvr in dgv->Rows)
                     {
                         FilenameProcessorRE ^re = REFor(dgvr);
                         if (re != nullptr)
                             rel->Add(re); 
                     }
                 }
                 else
                 {
                     DataGridViewRow ^dgvr = dgv->CurrentRow;
                     if (dgvr == nullptr)
                         dgvr = dgv->Rows[0];

                     FilenameProcessorRE ^re2 = REFor(dgvr);
                     if (re2 != nullptr)
                         rel->Add(re2); 
                     else
                         return;
                 }

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

             }
    private: System::Void dgv_CellValueChanged(System::Object^  sender, System::Windows::Forms::DataGridViewCellEventArgs^  e) 
             {
                 StartTimer();
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
