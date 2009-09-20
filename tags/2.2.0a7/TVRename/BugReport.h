//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "TVDoc.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

    /// <summary>
    /// Summary for BugReport
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public ref class BugReport : public System::Windows::Forms::Form
    {
    private:
        TVDoc ^mDoc;

    public:
        BugReport(TVDoc ^doc)
        {
            mDoc = doc;
            InitializeComponent();
            //
            //TODO: Add the constructor code here
            //
        }

    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~BugReport()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::TextBox^  txtName;
    protected: 

    protected: 
    private: System::Windows::Forms::Label^  label2;
    private: System::Windows::Forms::Label^  label3;
    private: System::Windows::Forms::TextBox^  txtEmail;

    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::TextBox^  txtDesc1;

    private: System::Windows::Forms::Label^  label4;
    private: System::Windows::Forms::TextBox^  txtDesc2;

    private: System::Windows::Forms::Label^  label5;
    private: System::Windows::Forms::TextBox^  txtFreq;
    private: System::Windows::Forms::TextBox^  txtComments;


    private: System::Windows::Forms::Label^  label6;
    private: System::Windows::Forms::Label^  label7;
    private: System::Windows::Forms::CheckBox^  cbSettings;
    private: System::Windows::Forms::CheckBox^  cbFOScan;



    private: System::Windows::Forms::Button^  bnCreate;
    private: System::Windows::Forms::TextBox^  txtEmailText;


    private: System::Windows::Forms::Label^  label8;
    private: System::Windows::Forms::Button^  bnClose;
    private: System::Windows::Forms::CheckBox^  cbFolderScan;
    private: System::Windows::Forms::Button^  bnCopy;


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
            System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(BugReport::typeid));
            this->txtName = (gcnew System::Windows::Forms::TextBox());
            this->label2 = (gcnew System::Windows::Forms::Label());
            this->label3 = (gcnew System::Windows::Forms::Label());
            this->txtEmail = (gcnew System::Windows::Forms::TextBox());
            this->label1 = (gcnew System::Windows::Forms::Label());
            this->txtDesc1 = (gcnew System::Windows::Forms::TextBox());
            this->label4 = (gcnew System::Windows::Forms::Label());
            this->txtDesc2 = (gcnew System::Windows::Forms::TextBox());
            this->label5 = (gcnew System::Windows::Forms::Label());
            this->txtFreq = (gcnew System::Windows::Forms::TextBox());
            this->txtComments = (gcnew System::Windows::Forms::TextBox());
            this->label6 = (gcnew System::Windows::Forms::Label());
            this->label7 = (gcnew System::Windows::Forms::Label());
            this->cbSettings = (gcnew System::Windows::Forms::CheckBox());
            this->cbFOScan = (gcnew System::Windows::Forms::CheckBox());
            this->bnCreate = (gcnew System::Windows::Forms::Button());
            this->txtEmailText = (gcnew System::Windows::Forms::TextBox());
            this->label8 = (gcnew System::Windows::Forms::Label());
            this->bnClose = (gcnew System::Windows::Forms::Button());
            this->cbFolderScan = (gcnew System::Windows::Forms::CheckBox());
            this->bnCopy = (gcnew System::Windows::Forms::Button());
            this->SuspendLayout();
            // 
            // txtName
            // 
            this->txtName->Location = System::Drawing::Point(56, 12);
            this->txtName->Name = L"txtName";
            this->txtName->Size = System::Drawing::Size(299, 20);
            this->txtName->TabIndex = 1;
            // 
            // label2
            // 
            this->label2->AutoSize = true;
            this->label2->Location = System::Drawing::Point(12, 15);
            this->label2->Name = L"label2";
            this->label2->Size = System::Drawing::Size(38, 13);
            this->label2->TabIndex = 0;
            this->label2->Text = L"Name:";
            // 
            // label3
            // 
            this->label3->AutoSize = true;
            this->label3->Location = System::Drawing::Point(12, 41);
            this->label3->Name = L"label3";
            this->label3->Size = System::Drawing::Size(35, 13);
            this->label3->TabIndex = 0;
            this->label3->Text = L"Email:";
            // 
            // txtEmail
            // 
            this->txtEmail->Location = System::Drawing::Point(56, 38);
            this->txtEmail->Name = L"txtEmail";
            this->txtEmail->Size = System::Drawing::Size(299, 20);
            this->txtEmail->TabIndex = 1;
            // 
            // label1
            // 
            this->label1->AutoSize = true;
            this->label1->Location = System::Drawing::Point(12, 67);
            this->label1->Name = L"label1";
            this->label1->Size = System::Drawing::Size(155, 13);
            this->label1->TabIndex = 0;
            this->label1->Text = L"Brief description of the problem:";
            // 
            // txtDesc1
            // 
            this->txtDesc1->Location = System::Drawing::Point(56, 83);
            this->txtDesc1->Multiline = true;
            this->txtDesc1->Name = L"txtDesc1";
            this->txtDesc1->Size = System::Drawing::Size(299, 40);
            this->txtDesc1->TabIndex = 1;
            // 
            // label4
            // 
            this->label4->AutoSize = true;
            this->label4->Location = System::Drawing::Point(12, 129);
            this->label4->Name = L"label4";
            this->label4->Size = System::Drawing::Size(258, 13);
            this->label4->TabIndex = 0;
            this->label4->Text = L"Detailed description, and steps to repeat the problem:";
            // 
            // txtDesc2
            // 
            this->txtDesc2->Location = System::Drawing::Point(56, 145);
            this->txtDesc2->Multiline = true;
            this->txtDesc2->Name = L"txtDesc2";
            this->txtDesc2->ScrollBars = System::Windows::Forms::ScrollBars::Vertical;
            this->txtDesc2->Size = System::Drawing::Size(299, 142);
            this->txtDesc2->TabIndex = 1;
            // 
            // label5
            // 
            this->label5->AutoSize = true;
            this->label5->Location = System::Drawing::Point(12, 290);
            this->label5->Name = L"label5";
            this->label5->Size = System::Drawing::Size(183, 13);
            this->label5->TabIndex = 0;
            this->label5->Text = L"How often does this problem happen:";
            // 
            // txtFreq
            // 
            this->txtFreq->Location = System::Drawing::Point(56, 310);
            this->txtFreq->Name = L"txtFreq";
            this->txtFreq->Size = System::Drawing::Size(299, 20);
            this->txtFreq->TabIndex = 1;
            // 
            // txtComments
            // 
            this->txtComments->Location = System::Drawing::Point(56, 356);
            this->txtComments->Multiline = true;
            this->txtComments->Name = L"txtComments";
            this->txtComments->ScrollBars = System::Windows::Forms::ScrollBars::Vertical;
            this->txtComments->Size = System::Drawing::Size(299, 112);
            this->txtComments->TabIndex = 3;
            // 
            // label6
            // 
            this->label6->AutoSize = true;
            this->label6->Location = System::Drawing::Point(12, 340);
            this->label6->Name = L"label6";
            this->label6->Size = System::Drawing::Size(147, 13);
            this->label6->TabIndex = 2;
            this->label6->Text = L"Any other comments or notes:";
            // 
            // label7
            // 
            this->label7->AutoSize = true;
            this->label7->Location = System::Drawing::Point(12, 488);
            this->label7->Name = L"label7";
            this->label7->Size = System::Drawing::Size(45, 13);
            this->label7->TabIndex = 2;
            this->label7->Text = L"Include:";
            // 
            // cbSettings
            // 
            this->cbSettings->AutoSize = true;
            this->cbSettings->Checked = true;
            this->cbSettings->CheckState = System::Windows::Forms::CheckState::Checked;
            this->cbSettings->Location = System::Drawing::Point(56, 504);
            this->cbSettings->Name = L"cbSettings";
            this->cbSettings->Size = System::Drawing::Size(88, 17);
            this->cbSettings->TabIndex = 4;
            this->cbSettings->Text = L"Settings Files";
            this->cbSettings->UseVisualStyleBackColor = true;
            // 
            // cbFOScan
            // 
            this->cbFOScan->AutoSize = true;
            this->cbFOScan->Checked = true;
            this->cbFOScan->CheckState = System::Windows::Forms::CheckState::Checked;
            this->cbFOScan->Location = System::Drawing::Point(150, 504);
            this->cbFOScan->Name = L"cbFOScan";
            this->cbFOScan->Size = System::Drawing::Size(74, 17);
            this->cbFOScan->TabIndex = 4;
            this->cbFOScan->Text = L"F&&O Scan";
            this->cbFOScan->UseVisualStyleBackColor = true;
            // 
            // bnCreate
            // 
            this->bnCreate->Location = System::Drawing::Point(15, 532);
            this->bnCreate->Name = L"bnCreate";
            this->bnCreate->Size = System::Drawing::Size(75, 23);
            this->bnCreate->TabIndex = 5;
            this->bnCreate->Text = L"Create";
            this->bnCreate->UseVisualStyleBackColor = true;
            this->bnCreate->Click += gcnew System::EventHandler(this, &BugReport::bnCreate_Click);
            // 
            // txtEmailText
            // 
            this->txtEmailText->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
                | System::Windows::Forms::AnchorStyles::Left));
            this->txtEmailText->Location = System::Drawing::Point(399, 83);
            this->txtEmailText->Multiline = true;
            this->txtEmailText->Name = L"txtEmailText";
            this->txtEmailText->ScrollBars = System::Windows::Forms::ScrollBars::Vertical;
            this->txtEmailText->Size = System::Drawing::Size(461, 441);
            this->txtEmailText->TabIndex = 1;
            // 
            // label8
            // 
            this->label8->Location = System::Drawing::Point(396, 12);
            this->label8->Name = L"label8";
            this->label8->Size = System::Drawing::Size(464, 68);
            this->label8->TabIndex = 0;
            this->label8->Text = resources->GetString(L"label8.Text");
            // 
            // bnClose
            // 
            this->bnClose->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
            this->bnClose->DialogResult = System::Windows::Forms::DialogResult::Cancel;
            this->bnClose->Location = System::Drawing::Point(785, 530);
            this->bnClose->Name = L"bnClose";
            this->bnClose->Size = System::Drawing::Size(75, 23);
            this->bnClose->TabIndex = 5;
            this->bnClose->Text = L"Close";
            this->bnClose->UseVisualStyleBackColor = true;
            // 
            // cbFolderScan
            // 
            this->cbFolderScan->AutoSize = true;
            this->cbFolderScan->Checked = true;
            this->cbFolderScan->CheckState = System::Windows::Forms::CheckState::Checked;
            this->cbFolderScan->Location = System::Drawing::Point(230, 504);
            this->cbFolderScan->Name = L"cbFolderScan";
            this->cbFolderScan->Size = System::Drawing::Size(83, 17);
            this->cbFolderScan->TabIndex = 4;
            this->cbFolderScan->Text = L"Folder Scan";
            this->cbFolderScan->UseVisualStyleBackColor = true;
            // 
            // bnCopy
            // 
            this->bnCopy->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
            this->bnCopy->Location = System::Drawing::Point(399, 530);
            this->bnCopy->Name = L"bnCopy";
            this->bnCopy->Size = System::Drawing::Size(75, 23);
            this->bnCopy->TabIndex = 5;
            this->bnCopy->Text = L"Copy";
            this->bnCopy->UseVisualStyleBackColor = true;
            this->bnCopy->Click += gcnew System::EventHandler(this, &BugReport::bnCopy_Click);
            // 
            // BugReport
            // 
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->CancelButton = this->bnClose;
            this->ClientSize = System::Drawing::Size(875, 567);
            this->Controls->Add(this->bnCopy);
            this->Controls->Add(this->bnClose);
            this->Controls->Add(this->bnCreate);
            this->Controls->Add(this->cbFolderScan);
            this->Controls->Add(this->cbFOScan);
            this->Controls->Add(this->cbSettings);
            this->Controls->Add(this->txtComments);
            this->Controls->Add(this->label7);
            this->Controls->Add(this->label6);
            this->Controls->Add(this->txtEmailText);
            this->Controls->Add(this->txtDesc2);
            this->Controls->Add(this->label8);
            this->Controls->Add(this->label4);
            this->Controls->Add(this->txtDesc1);
            this->Controls->Add(this->label1);
            this->Controls->Add(this->txtFreq);
            this->Controls->Add(this->label5);
            this->Controls->Add(this->txtEmail);
            this->Controls->Add(this->label3);
            this->Controls->Add(this->txtName);
            this->Controls->Add(this->label2);
            this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
            this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
            this->Name = L"BugReport";
            this->ShowInTaskbar = false;
            this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Hide;
            this->Text = L"BugReport";
            this->ResumeLayout(false);
            this->PerformLayout();

        }
#pragma endregion

    private: System::Void bnCreate_Click(System::Object^  sender, System::EventArgs^  e) ;
    private: System::Void bnCopy_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Clipboard::SetDataObject(txtEmailText->Text);
             }
    };
}
