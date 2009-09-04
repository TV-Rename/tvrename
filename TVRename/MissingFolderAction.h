#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::IO;

namespace TVRename {

    /// <summary>
    /// Summary for MissingFolderAction
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    enum FAResult { kfaNotSet, kfaRetry, kfaCancel, kfaCreate, kfaIgnoreOnce, kfaIgnoreAlways, kfaDifferentFolder };

    public ref class MissingFolderAction : public System::Windows::Forms::Form
    {
    public:
        FAResult Result;
        String ^FolderName;

        MissingFolderAction(String ^showName, String ^season, String ^folderName)
        {
            InitializeComponent();

            Result = kfaCancel;
            FolderName = folderName;
            txtShow->Text = showName;
            txtSeason->Text = season;
            txtFolder->Text = FolderName;

            if (String::IsNullOrEmpty(FolderName))
            {
                txtFolder->Text = "Click Browse..., or Drag+Drop a folder onto this window.";
                bnCreate->Enabled = false;
                bnRetry->Enabled = false;
            }
        }

    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~MissingFolderAction()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::Label^  label2;
    private: System::Windows::Forms::Label^  label3;
    private: System::Windows::Forms::Button^  bnIgnoreOnce;
    private: System::Windows::Forms::Button^  bnIgnoreAlways;
    private: System::Windows::Forms::Button^  bnCreate;
    private: System::Windows::Forms::Label^  txtShow;
    private: System::Windows::Forms::Label^  txtSeason;
    private: System::Windows::Forms::Label^  txtFolder;
    private: System::Windows::Forms::Button^  bnRetry;
    private: System::Windows::Forms::Button^  bnCancel;
    private: System::Windows::Forms::Button^  bnBrowse;
    private: System::Windows::Forms::FolderBrowserDialog^  folderBrowser;

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
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(MissingFolderAction::typeid));
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->bnIgnoreOnce = (gcnew System::Windows::Forms::Button());
			this->bnIgnoreAlways = (gcnew System::Windows::Forms::Button());
			this->bnCreate = (gcnew System::Windows::Forms::Button());
			this->txtShow = (gcnew System::Windows::Forms::Label());
			this->txtSeason = (gcnew System::Windows::Forms::Label());
			this->txtFolder = (gcnew System::Windows::Forms::Label());
			this->bnRetry = (gcnew System::Windows::Forms::Button());
			this->bnCancel = (gcnew System::Windows::Forms::Button());
			this->bnBrowse = (gcnew System::Windows::Forms::Button());
			this->folderBrowser = (gcnew System::Windows::Forms::FolderBrowserDialog());
			this->SuspendLayout();
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(12, 9);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(37, 13);
			this->label1->TabIndex = 0;
			this->label1->Text = L"Show:";
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Location = System::Drawing::Point(12, 31);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(46, 13);
			this->label2->TabIndex = 2;
			this->label2->Text = L"Season:";
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Location = System::Drawing::Point(12, 55);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(39, 13);
			this->label3->TabIndex = 4;
			this->label3->Text = L"Folder:";
			// 
			// bnIgnoreOnce
			// 
			this->bnIgnoreOnce->Location = System::Drawing::Point(15, 84);
			this->bnIgnoreOnce->Name = L"bnIgnoreOnce";
			this->bnIgnoreOnce->Size = System::Drawing::Size(92, 23);
			this->bnIgnoreOnce->TabIndex = 7;
			this->bnIgnoreOnce->Text = L"Ig&nore Once";
			this->bnIgnoreOnce->UseVisualStyleBackColor = true;
			this->bnIgnoreOnce->Click += gcnew System::EventHandler(this, &MissingFolderAction::bnIgnoreOnce_Click);
			// 
			// bnIgnoreAlways
			// 
			this->bnIgnoreAlways->Location = System::Drawing::Point(113, 84);
			this->bnIgnoreAlways->Name = L"bnIgnoreAlways";
			this->bnIgnoreAlways->Size = System::Drawing::Size(92, 23);
			this->bnIgnoreAlways->TabIndex = 8;
			this->bnIgnoreAlways->Text = L"Ignore &Always";
			this->bnIgnoreAlways->UseVisualStyleBackColor = true;
			this->bnIgnoreAlways->Click += gcnew System::EventHandler(this, &MissingFolderAction::bnIgnoreAlways_Click);
			// 
			// bnCreate
			// 
			this->bnCreate->Location = System::Drawing::Point(211, 84);
			this->bnCreate->Name = L"bnCreate";
			this->bnCreate->Size = System::Drawing::Size(92, 23);
			this->bnCreate->TabIndex = 9;
			this->bnCreate->Text = L"&Create";
			this->bnCreate->UseVisualStyleBackColor = true;
			this->bnCreate->Click += gcnew System::EventHandler(this, &MissingFolderAction::bnCreate_Click);
			// 
			// txtShow
			// 
			this->txtShow->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->txtShow->Location = System::Drawing::Point(70, 9);
			this->txtShow->Name = L"txtShow";
			this->txtShow->Size = System::Drawing::Size(431, 13);
			this->txtShow->TabIndex = 1;
			this->txtShow->Text = L"---";
			// 
			// txtSeason
			// 
			this->txtSeason->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->txtSeason->Location = System::Drawing::Point(70, 31);
			this->txtSeason->Name = L"txtSeason";
			this->txtSeason->Size = System::Drawing::Size(431, 13);
			this->txtSeason->TabIndex = 3;
			this->txtSeason->Text = L"---";
			// 
			// txtFolder
			// 
			this->txtFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->txtFolder->Location = System::Drawing::Point(70, 55);
			this->txtFolder->Name = L"txtFolder";
			this->txtFolder->Size = System::Drawing::Size(331, 13);
			this->txtFolder->TabIndex = 5;
			this->txtFolder->Text = L"---";
			// 
			// bnRetry
			// 
			this->bnRetry->Location = System::Drawing::Point(309, 84);
			this->bnRetry->Name = L"bnRetry";
			this->bnRetry->Size = System::Drawing::Size(92, 23);
			this->bnRetry->TabIndex = 10;
			this->bnRetry->Text = L"&Retry";
			this->bnRetry->UseVisualStyleBackColor = true;
			this->bnRetry->Click += gcnew System::EventHandler(this, &MissingFolderAction::bnRetry_Click);
			// 
			// bnCancel
			// 
			this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnCancel->Location = System::Drawing::Point(407, 84);
			this->bnCancel->Name = L"bnCancel";
			this->bnCancel->Size = System::Drawing::Size(92, 23);
			this->bnCancel->TabIndex = 11;
			this->bnCancel->Text = L"Canc&el";
			this->bnCancel->UseVisualStyleBackColor = true;
			this->bnCancel->Click += gcnew System::EventHandler(this, &MissingFolderAction::bnCancel_Click);
			// 
			// bnBrowse
			// 
			this->bnBrowse->Location = System::Drawing::Point(407, 50);
			this->bnBrowse->Name = L"bnBrowse";
			this->bnBrowse->Size = System::Drawing::Size(92, 23);
			this->bnBrowse->TabIndex = 6;
			this->bnBrowse->Text = L"&Browse...";
			this->bnBrowse->UseVisualStyleBackColor = true;
			this->bnBrowse->Click += gcnew System::EventHandler(this, &MissingFolderAction::bnBrowse_Click);
			// 
			// MissingFolderAction
			// 
			this->AllowDrop = true;
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnCancel;
			this->ClientSize = System::Drawing::Size(516, 121);
			this->Controls->Add(this->bnBrowse);
			this->Controls->Add(this->bnCancel);
			this->Controls->Add(this->bnRetry);
			this->Controls->Add(this->bnCreate);
			this->Controls->Add(this->bnIgnoreAlways);
			this->Controls->Add(this->bnIgnoreOnce);
			this->Controls->Add(this->txtFolder);
			this->Controls->Add(this->label3);
			this->Controls->Add(this->txtSeason);
			this->Controls->Add(this->label2);
			this->Controls->Add(this->txtShow);
			this->Controls->Add(this->label1);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"MissingFolderAction";
			this->ShowInTaskbar = false;
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = L"Missing Folder";
			this->DragDrop += gcnew System::Windows::Forms::DragEventHandler(this, &MissingFolderAction::MissingFolderAction_DragDrop);
			this->DragOver += gcnew System::Windows::Forms::DragEventHandler(this, &MissingFolderAction::MissingFolderAction_DragOver);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
    private: System::Void bnIgnoreOnce_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Result = kfaIgnoreOnce;
                 this->Close();
             }
    private: System::Void bnIgnoreAlways_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Result = kfaIgnoreAlways;
                 this->Close();
             }
    private: System::Void bnCreate_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Result = kfaCreate;
                 this->Close();
             }
    private: System::Void bnRetry_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Result = kfaRetry;
                 this->Close();
             }
    private: System::Void bnCancel_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Result = kfaCancel;
                 this->Close();
             }
    private: System::Void bnBrowse_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 folderBrowser->SelectedPath = FolderName;
                 if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
                 {
                     Result = kfaDifferentFolder;
                     FolderName = folderBrowser->SelectedPath;
                     this->Close();
                 }
             }

    private: System::Void MissingFolderAction_DragOver(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
             {
                 if (  !e->Data->GetDataPresent( DataFormats::FileDrop ) )
                     e->Effect = DragDropEffects::None;
                 else
                     e->Effect = DragDropEffects::Copy;
             }
    private: System::Void MissingFolderAction_DragDrop(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
             {
                 array<String ^> ^files = safe_cast<array<String ^>^>(e->Data->GetData(DataFormats::FileDrop));
                 for (int i=0;i<files->Length;i++)
                 {
                     String ^path = files[i];
                     DirectoryInfo ^di;
                     try 
                     {
                         di = gcnew DirectoryInfo(path);
                         if (di->Exists)
                         {
                             FolderName = path;
                             Result = kfaDifferentFolder;
                             this->Close();
                             return;
                         }
                     }
                     catch (...)
                     {
                     }
                 }
             }
    };
}
