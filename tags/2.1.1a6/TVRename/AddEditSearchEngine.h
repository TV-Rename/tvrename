#pragma once

#include "Searchers.h"
#include "CustomNameTagsFloatingWindow.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


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
            ProcessedEpisode ^SampleEpisode;

	public:
		AddEditSearchEngine(Searchers ^s, ProcessedEpisode ^pe)
		{
                    SampleEpisode = pe;
			InitializeComponent();
                        Cntfw = nullptr;
			
                        mSearchers = s;

                        dgvURLs->Rows->Add(mSearchers->Count());
                        for (int i=0;i<mSearchers->Count();i++)
                        {
                            dgvURLs->Rows[i]->Cells[0]->Value = mSearchers->Name(i);
                            dgvURLs->Rows[i]->Cells[1]->Value = mSearchers->URL(i);
                        }
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
        private: System::Windows::Forms::DataGridView^  dgvURLs;
        private: System::Windows::Forms::Button^  bnAdd;
        private: System::Windows::Forms::Button^  bnDelete;
        private: System::Windows::Forms::DataGridViewTextBoxColumn^  Name;
        private: System::Windows::Forms::DataGridViewTextBoxColumn^  URL;
        private: System::Windows::Forms::Button^  bnCancel;
        private: System::Windows::Forms::Button^  bnOK;
        private: System::Windows::Forms::DataGridViewTextBoxColumn^  TheName;
        private: System::Windows::Forms::DataGridViewTextBoxColumn^  TheURL;
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
                    this->dgvURLs = (gcnew System::Windows::Forms::DataGridView());
                    this->TheName = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
                    this->TheURL = (gcnew System::Windows::Forms::DataGridViewTextBoxColumn());
                    this->bnAdd = (gcnew System::Windows::Forms::Button());
                    this->bnDelete = (gcnew System::Windows::Forms::Button());
                    this->bnCancel = (gcnew System::Windows::Forms::Button());
                    this->bnOK = (gcnew System::Windows::Forms::Button());
                    this->bnTags = (gcnew System::Windows::Forms::Button());
                    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dgvURLs))->BeginInit();
                    this->SuspendLayout();
                    // 
                    // dgvURLs
                    // 
                    this->dgvURLs->AllowUserToAddRows = false;
                    this->dgvURLs->AllowUserToDeleteRows = false;
                    this->dgvURLs->AllowUserToResizeRows = false;
                    this->dgvURLs->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
                        | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->dgvURLs->AutoSizeColumnsMode = System::Windows::Forms::DataGridViewAutoSizeColumnsMode::AllCells;
                    this->dgvURLs->ColumnHeadersHeightSizeMode = System::Windows::Forms::DataGridViewColumnHeadersHeightSizeMode::AutoSize;
                    this->dgvURLs->Columns->AddRange(gcnew cli::array< System::Windows::Forms::DataGridViewColumn^  >(2) {this->TheName, this->TheURL});
                    this->dgvURLs->Location = System::Drawing::Point(12, 12);
                    this->dgvURLs->MultiSelect = false;
                    this->dgvURLs->Name = L"dgvURLs";
                    this->dgvURLs->RowHeadersVisible = false;
                    this->dgvURLs->Size = System::Drawing::Size(664, 325);
                    this->dgvURLs->TabIndex = 0;
                    // 
                    // TheName
                    // 
                    this->TheName->AutoSizeMode = System::Windows::Forms::DataGridViewAutoSizeColumnMode::Fill;
                    this->TheName->FillWeight = 150;
                    this->TheName->HeaderText = L"Name";
                    this->TheName->Name = L"TheName";
                    // 
                    // TheURL
                    // 
                    this->TheURL->AutoSizeMode = System::Windows::Forms::DataGridViewAutoSizeColumnMode::Fill;
                    this->TheURL->FillWeight = 300;
                    this->TheURL->HeaderText = L"URL";
                    this->TheURL->Name = L"TheURL";
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
                    this->bnTags->Location = System::Drawing::Point(189, 343);
                    this->bnTags->Name = L"bnTags";
                    this->bnTags->Size = System::Drawing::Size(75, 23);
                    this->bnTags->TabIndex = 3;
                    this->bnTags->Text = L"Tags...";
                    this->bnTags->UseVisualStyleBackColor = true;
                    this->bnTags->Click += gcnew System::EventHandler(this, &AddEditSearchEngine::bnTags_Click);
                    // 
                    // AddEditSearchEngine
                    // 
                    this->AcceptButton = this->bnOK;
                    this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
                    this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
                    this->CancelButton = this->bnCancel;
                    this->ClientSize = System::Drawing::Size(688, 378);
                    this->Controls->Add(this->bnTags);
                    this->Controls->Add(this->bnOK);
                    this->Controls->Add(this->bnCancel);
                    this->Controls->Add(this->bnDelete);
                    this->Controls->Add(this->bnAdd);
                    this->Controls->Add(this->dgvURLs);
                    this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
                    this->ShowInTaskbar = false;
                    this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
                    this->Text = L"Modify Search Engines";
                    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->dgvURLs))->EndInit();
                    this->ResumeLayout(false);

                }
#pragma endregion
        private: System::Void bnAdd_Click(System::Object^  sender, System::EventArgs^  e) 
                 {
                     dgvURLs->Rows->Add();
                 }
private: System::Void bnDelete_Click(System::Object^  sender, System::EventArgs^  e) 
         {
             dgvURLs->Rows->Remove(dgvURLs->CurrentRow);
         }
private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
         {
             mSearchers->Clear();
             for each (DataGridViewRow ^r in dgvURLs->Rows)
                 mSearchers->Add(r->Cells[0]->Value->ToString(), r->Cells[1]->Value->ToString());
         }
private: System::Void bnTags_Click(System::Object^  sender, System::EventArgs^  e) 
         {
             Cntfw = gcnew CustomNameTagsFloatingWindow(SampleEpisode);
             Cntfw->Show(this);
             this->Focus();
         }
};
}
