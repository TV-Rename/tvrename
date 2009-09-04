#pragma once

#include "TheTVDB.h"
#include "Settings.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

	/// <summary>
	/// Summary for PreferredLanguage
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class PreferredLanguage : public System::Windows::Forms::Form
	{
            LanguageListType ^ourList;
            TheTVDB ^DB;

	public:
		PreferredLanguage(TheTVDB ^db)
		{
                    DB = db;

			InitializeComponent();

                        if (!db->Connected)
                        {
                            lbLangs->Items->Add("Please Wait");
                            lbLangs->Items->Add("Connecting...");
                            lbLangs->Update();
                            db->Connect();
                        }

                        // make our list
                        // add already prioritised items (that still exist)
                        ourList = gcnew LanguageListType();
                        for each (String ^s in db->LanguagePriorityList)
                            if (db->LanguageList->ContainsKey(s))
                                ourList->Add(s);

                        // add items that haven't been prioritised
                        for each (KeyValuePair<String ^, String ^> ^k in DB->LanguageList)
                            if (!ourList->Contains(k->Key))
                                ourList->Add(k->Key);

                        FillList();
                }

                void FillList()
                {
                    lbLangs->BeginUpdate();
                        lbLangs->Items->Clear();
                        for each (String ^l in ourList)
                         lbLangs->Items->Add(DB->LanguageList[l]);
                        lbLangs->EndUpdate();
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~PreferredLanguage()
		{
			if (components)
			{
				delete components;
			}
		}
        private: System::Windows::Forms::Label^  label1;
        private: System::Windows::Forms::ListBox^  lbLangs;
        protected: 

        private: System::Windows::Forms::Button^  bnOK;
        private: System::Windows::Forms::Button^  bnCancel;
        private: System::Windows::Forms::Button^  bnUp;

        private: System::Windows::Forms::Button^  bnDown;
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
                    System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(PreferredLanguage::typeid));
                    this->label1 = (gcnew System::Windows::Forms::Label());
                    this->lbLangs = (gcnew System::Windows::Forms::ListBox());
                    this->bnOK = (gcnew System::Windows::Forms::Button());
                    this->bnCancel = (gcnew System::Windows::Forms::Button());
                    this->bnUp = (gcnew System::Windows::Forms::Button());
                    this->bnDown = (gcnew System::Windows::Forms::Button());
                    this->SuspendLayout();
                    // 
                    // label1
                    // 
                    this->label1->AutoSize = true;
                    this->label1->Location = System::Drawing::Point(12, 9);
                    this->label1->Name = L"label1";
                    this->label1->Size = System::Drawing::Size(179, 13);
                    this->label1->TabIndex = 0;
                    this->label1->Text = L"Preferred languages for thetvdb.com";
                    // 
                    // lbLangs
                    // 
                    this->lbLangs->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
                        | System::Windows::Forms::AnchorStyles::Left) 
                        | System::Windows::Forms::AnchorStyles::Right));
                    this->lbLangs->FormattingEnabled = true;
                    this->lbLangs->Location = System::Drawing::Point(15, 25);
                    this->lbLangs->Name = L"lbLangs";
                    this->lbLangs->Size = System::Drawing::Size(183, 277);
                    this->lbLangs->TabIndex = 1;
                    // 
                    // bnOK
                    // 
                    this->bnOK->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
                    this->bnOK->DialogResult = System::Windows::Forms::DialogResult::OK;
                    this->bnOK->Location = System::Drawing::Point(204, 250);
                    this->bnOK->Name = L"bnOK";
                    this->bnOK->Size = System::Drawing::Size(75, 23);
                    this->bnOK->TabIndex = 2;
                    this->bnOK->Text = L"OK";
                    this->bnOK->UseVisualStyleBackColor = true;
                    this->bnOK->Click += gcnew System::EventHandler(this, &PreferredLanguage::bnOK_Click);
                    // 
                    // bnCancel
                    // 
                    this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
                    this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
                    this->bnCancel->Location = System::Drawing::Point(204, 279);
                    this->bnCancel->Name = L"bnCancel";
                    this->bnCancel->Size = System::Drawing::Size(75, 23);
                    this->bnCancel->TabIndex = 3;
                    this->bnCancel->Text = L"Cancel";
                    this->bnCancel->UseVisualStyleBackColor = true;
                    // 
                    // bnUp
                    // 
                    this->bnUp->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
                    this->bnUp->Location = System::Drawing::Point(204, 25);
                    this->bnUp->Name = L"bnUp";
                    this->bnUp->Size = System::Drawing::Size(75, 23);
                    this->bnUp->TabIndex = 4;
                    this->bnUp->Text = L"Move &up";
                    this->bnUp->UseVisualStyleBackColor = true;
                    this->bnUp->Click += gcnew System::EventHandler(this, &PreferredLanguage::bnUp_Click);
                    // 
                    // bnDown
                    // 
                    this->bnDown->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
                    this->bnDown->Location = System::Drawing::Point(204, 54);
                    this->bnDown->Name = L"bnDown";
                    this->bnDown->Size = System::Drawing::Size(75, 23);
                    this->bnDown->TabIndex = 4;
                    this->bnDown->Text = L"Move &down";
                    this->bnDown->UseVisualStyleBackColor = true;
                    this->bnDown->Click += gcnew System::EventHandler(this, &PreferredLanguage::bnDown_Click);
                    // 
                    // PreferredLanguage
                    // 
                    this->AcceptButton = this->bnOK;
                    this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
                    this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
                    this->CancelButton = this->bnCancel;
                    this->ClientSize = System::Drawing::Size(291, 314);
                    this->Controls->Add(this->bnDown);
                    this->Controls->Add(this->bnUp);
                    this->Controls->Add(this->bnCancel);
                    this->Controls->Add(this->bnOK);
                    this->Controls->Add(this->lbLangs);
                    this->Controls->Add(this->label1);
                    this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
                    this->Name = L"PreferredLanguage";
                    this->ShowInTaskbar = false;
                    this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
                    this->Text = L"Language Selection";
                    this->ResumeLayout(false);
                    this->PerformLayout();

                }
#pragma endregion

        private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
                 {
                     DB->LanguagePriorityList = ourList;
                     DB->SaveCache();

                     this->Close();
                 }
private: System::Void bnDown_Click(System::Object^  sender, System::EventArgs^  e) 
         {
             int n = lbLangs->SelectedIndex;
             if (n == -1)
                 return;

             if (n < (ourList->Count - 1))
             {
                 ourList->Reverse(n,2);
                 FillList();
                 lbLangs->SelectedIndex = n+1;
             }
         }
private: System::Void bnUp_Click(System::Object^  sender, System::EventArgs^  e) 
         {
             int n = lbLangs->SelectedIndex;
             if (n == -1)
                 return;
             if (n > 0)
             {
                 ourList->Reverse(n-1,2);
                 FillList();
                 lbLangs->SelectedIndex = n-1;
             }

         }
};
}
