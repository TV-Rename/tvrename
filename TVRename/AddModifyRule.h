#pragma once

#include "ShowItem.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

    /// <summary>
    /// Summary for AddModifyRule
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public ref class AddModifyRule : public System::Windows::Forms::Form
    {
    private:
        ShowRule ^mRule;

    public:
        AddModifyRule(ShowRule ^rule) :
          mRule(rule)
          {
              InitializeComponent();

              FillDialog();
          }

    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~AddModifyRule()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::RadioButton^  rbRemove;
    protected: 

    private: System::Windows::Forms::RadioButton^  rbSwap;
    protected: 


    private: System::Windows::Forms::RadioButton^  rbMerge;
    private: System::Windows::Forms::RadioButton^  rbInsert;
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::Label^  txtLabel1;
    private: System::Windows::Forms::Label^  txtLabel2;
    private: System::Windows::Forms::TextBox^  txtValue1;
    private: System::Windows::Forms::TextBox^  txtValue2;
    private: System::Windows::Forms::Label^  txtWithNameLabel;







    private: System::Windows::Forms::TextBox^  txtUserText;

    private: System::Windows::Forms::Button^  bnOK;
    private: System::Windows::Forms::Button^  bnCancel;
    private: System::Windows::Forms::RadioButton^  rbIgnore;
    private: System::Windows::Forms::RadioButton^  rbRename;
    private: System::Windows::Forms::Label^  txtLeaveBlank;
    private: System::Windows::Forms::RadioButton^  rbSplit;
    private: System::Windows::Forms::RadioButton^  rbCollapse;

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
        void InitializeComponent(void)
        {
            System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(AddModifyRule::typeid));
            this->rbRemove = (gcnew System::Windows::Forms::RadioButton());
            this->rbSwap = (gcnew System::Windows::Forms::RadioButton());
            this->rbMerge = (gcnew System::Windows::Forms::RadioButton());
            this->rbInsert = (gcnew System::Windows::Forms::RadioButton());
            this->label1 = (gcnew System::Windows::Forms::Label());
            this->txtLabel1 = (gcnew System::Windows::Forms::Label());
            this->txtLabel2 = (gcnew System::Windows::Forms::Label());
            this->txtValue1 = (gcnew System::Windows::Forms::TextBox());
            this->txtValue2 = (gcnew System::Windows::Forms::TextBox());
            this->txtWithNameLabel = (gcnew System::Windows::Forms::Label());
            this->txtUserText = (gcnew System::Windows::Forms::TextBox());
            this->bnOK = (gcnew System::Windows::Forms::Button());
            this->bnCancel = (gcnew System::Windows::Forms::Button());
            this->rbIgnore = (gcnew System::Windows::Forms::RadioButton());
            this->rbRename = (gcnew System::Windows::Forms::RadioButton());
            this->txtLeaveBlank = (gcnew System::Windows::Forms::Label());
            this->rbSplit = (gcnew System::Windows::Forms::RadioButton());
            this->rbCollapse = (gcnew System::Windows::Forms::RadioButton());
            this->SuspendLayout();
            // 
            // rbRemove
            // 
            this->rbRemove->AutoSize = true;
            this->rbRemove->Location = System::Drawing::Point(61, 58);
            this->rbRemove->Name = L"rbRemove";
            this->rbRemove->Size = System::Drawing::Size(236, 17);
            this->rbRemove->TabIndex = 2;
            this->rbRemove->TabStop = true;
            this->rbRemove->Text = L"R&emove : Remove episode(s) from the series";
            this->rbRemove->UseVisualStyleBackColor = true;
            this->rbRemove->Click += gcnew System::EventHandler(this, &AddModifyRule::rb_Click);
            // 
            // rbSwap
            // 
            this->rbSwap->AutoSize = true;
            this->rbSwap->Location = System::Drawing::Point(61, 81);
            this->rbSwap->Name = L"rbSwap";
            this->rbSwap->Size = System::Drawing::Size(204, 17);
            this->rbSwap->TabIndex = 3;
            this->rbSwap->TabStop = true;
            this->rbSwap->Text = L"&Swap : Swap position of two episodes";
            this->rbSwap->UseVisualStyleBackColor = true;
            this->rbSwap->Click += gcnew System::EventHandler(this, &AddModifyRule::rb_Click);
            // 
            // rbMerge
            // 
            this->rbMerge->AutoSize = true;
            this->rbMerge->Location = System::Drawing::Point(61, 104);
            this->rbMerge->Name = L"rbMerge";
            this->rbMerge->Size = System::Drawing::Size(239, 17);
            this->rbMerge->TabIndex = 4;
            this->rbMerge->TabStop = true;
            this->rbMerge->Text = L"&Merge : Merge episodes into multi-episode file";
            this->rbMerge->UseVisualStyleBackColor = true;
            this->rbMerge->Click += gcnew System::EventHandler(this, &AddModifyRule::rb_Click);
            // 
            // rbInsert
            // 
            this->rbInsert->AutoSize = true;
            this->rbInsert->Location = System::Drawing::Point(61, 127);
            this->rbInsert->Name = L"rbInsert";
            this->rbInsert->Size = System::Drawing::Size(240, 17);
            this->rbInsert->TabIndex = 5;
            this->rbInsert->TabStop = true;
            this->rbInsert->Text = L"Inser&t : Insert another episode into the season";
            this->rbInsert->UseVisualStyleBackColor = true;
            this->rbInsert->Click += gcnew System::EventHandler(this, &AddModifyRule::rb_Click);
            // 
            // label1
            // 
            this->label1->AutoSize = true;
            this->label1->Location = System::Drawing::Point(8, 14);
            this->label1->Name = L"label1";
            this->label1->Size = System::Drawing::Size(40, 13);
            this->label1->TabIndex = 0;
            this->label1->Text = L"Action:";
            // 
            // txtLabel1
            // 
            this->txtLabel1->AutoSize = true;
            this->txtLabel1->Location = System::Drawing::Point(13, 217);
            this->txtLabel1->Name = L"txtLabel1";
            this->txtLabel1->Size = System::Drawing::Size(42, 13);
            this->txtLabel1->TabIndex = 6;
            this->txtLabel1->Text = L"Label1:";
            // 
            // txtLabel2
            // 
            this->txtLabel2->AutoSize = true;
            this->txtLabel2->Location = System::Drawing::Point(13, 242);
            this->txtLabel2->Name = L"txtLabel2";
            this->txtLabel2->Size = System::Drawing::Size(42, 13);
            this->txtLabel2->TabIndex = 8;
            this->txtLabel2->Text = L"Label2:";
            // 
            // txtValue1
            // 
            this->txtValue1->Location = System::Drawing::Point(80, 213);
            this->txtValue1->Name = L"txtValue1";
            this->txtValue1->Size = System::Drawing::Size(100, 20);
            this->txtValue1->TabIndex = 7;
            // 
            // txtValue2
            // 
            this->txtValue2->Location = System::Drawing::Point(80, 239);
            this->txtValue2->Name = L"txtValue2";
            this->txtValue2->Size = System::Drawing::Size(100, 20);
            this->txtValue2->TabIndex = 9;
            // 
            // txtWithNameLabel
            // 
            this->txtWithNameLabel->AutoSize = true;
            this->txtWithNameLabel->Location = System::Drawing::Point(13, 269);
            this->txtWithNameLabel->Name = L"txtWithNameLabel";
            this->txtWithNameLabel->Size = System::Drawing::Size(63, 13);
            this->txtWithNameLabel->TabIndex = 10;
            this->txtWithNameLabel->Text = L"New Name:";
            // 
            // txtUserText
            // 
            this->txtUserText->Location = System::Drawing::Point(80, 266);
            this->txtUserText->Name = L"txtUserText";
            this->txtUserText->Size = System::Drawing::Size(254, 20);
            this->txtUserText->TabIndex = 11;
            // 
            // bnOK
            // 
            this->bnOK->DialogResult = System::Windows::Forms::DialogResult::OK;
            this->bnOK->Location = System::Drawing::Point(175, 308);
            this->bnOK->Name = L"bnOK";
            this->bnOK->Size = System::Drawing::Size(75, 23);
            this->bnOK->TabIndex = 12;
            this->bnOK->Text = L"OK";
            this->bnOK->UseVisualStyleBackColor = true;
            this->bnOK->Click += gcnew System::EventHandler(this, &AddModifyRule::bnOK_Click);
            // 
            // bnCancel
            // 
            this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
            this->bnCancel->Location = System::Drawing::Point(256, 308);
            this->bnCancel->Name = L"bnCancel";
            this->bnCancel->Size = System::Drawing::Size(75, 23);
            this->bnCancel->TabIndex = 13;
            this->bnCancel->Text = L"Cancel";
            this->bnCancel->UseVisualStyleBackColor = true;
            // 
            // rbIgnore
            // 
            this->rbIgnore->AutoSize = true;
            this->rbIgnore->Location = System::Drawing::Point(61, 12);
            this->rbIgnore->Name = L"rbIgnore";
            this->rbIgnore->Size = System::Drawing::Size(246, 17);
            this->rbIgnore->TabIndex = 1;
            this->rbIgnore->TabStop = true;
            this->rbIgnore->Text = L"&Ignore : Don\'t rename or check for episode(s)";
            this->rbIgnore->UseVisualStyleBackColor = true;
            this->rbIgnore->Click += gcnew System::EventHandler(this, &AddModifyRule::rb_Click);
            // 
            // rbRename
            // 
            this->rbRename->AutoSize = true;
            this->rbRename->Location = System::Drawing::Point(61, 35);
            this->rbRename->Name = L"rbRename";
            this->rbRename->Size = System::Drawing::Size(203, 17);
            this->rbRename->TabIndex = 1;
            this->rbRename->TabStop = true;
            this->rbRename->Text = L"&Rename : Set episode name manually";
            this->rbRename->UseVisualStyleBackColor = true;
            this->rbRename->Click += gcnew System::EventHandler(this, &AddModifyRule::rb_Click);
            // 
            // txtLeaveBlank
            // 
            this->txtLeaveBlank->AutoSize = true;
            this->txtLeaveBlank->Location = System::Drawing::Point(77, 289);
            this->txtLeaveBlank->Name = L"txtLeaveBlank";
            this->txtLeaveBlank->Size = System::Drawing::Size(173, 13);
            this->txtLeaveBlank->TabIndex = 10;
            this->txtLeaveBlank->Text = L"(Leave blank for automatic naming)";
            // 
            // rbSplit
            // 
            this->rbSplit->AutoSize = true;
            this->rbSplit->Location = System::Drawing::Point(61, 150);
            this->rbSplit->Name = L"rbSplit";
            this->rbSplit->Size = System::Drawing::Size(221, 17);
            this->rbSplit->TabIndex = 5;
            this->rbSplit->TabStop = true;
            this->rbSplit->Text = L"S&plit: Make one episode count as multiple";
            this->rbSplit->UseVisualStyleBackColor = true;
            this->rbSplit->Click += gcnew System::EventHandler(this, &AddModifyRule::rb_Click);
            // 
            // rbCollapse
            // 
            this->rbCollapse->AutoSize = true;
            this->rbCollapse->Location = System::Drawing::Point(61, 173);
            this->rbCollapse->Name = L"rbCollapse";
            this->rbCollapse->Size = System::Drawing::Size(217, 17);
            this->rbCollapse->TabIndex = 5;
            this->rbCollapse->TabStop = true;
            this->rbCollapse->Text = L"&Collapse: Merge episodes, and renumber";
            this->rbCollapse->UseVisualStyleBackColor = true;
            this->rbCollapse->Click += gcnew System::EventHandler(this, &AddModifyRule::rb_Click);
            // 
            // AddModifyRule
            // 
            this->AcceptButton = this->bnOK;
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->CancelButton = this->bnCancel;
            this->ClientSize = System::Drawing::Size(341, 341);
            this->Controls->Add(this->bnCancel);
            this->Controls->Add(this->bnOK);
            this->Controls->Add(this->txtUserText);
            this->Controls->Add(this->txtValue2);
            this->Controls->Add(this->txtValue1);
            this->Controls->Add(this->txtLeaveBlank);
            this->Controls->Add(this->txtWithNameLabel);
            this->Controls->Add(this->txtLabel2);
            this->Controls->Add(this->txtLabel1);
            this->Controls->Add(this->label1);
            this->Controls->Add(this->rbRename);
            this->Controls->Add(this->rbIgnore);
            this->Controls->Add(this->rbCollapse);
            this->Controls->Add(this->rbSplit);
            this->Controls->Add(this->rbInsert);
            this->Controls->Add(this->rbMerge);
            this->Controls->Add(this->rbSwap);
            this->Controls->Add(this->rbRemove);
            this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
            this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
            this->Name = L"AddModifyRule";
            this->ShowInTaskbar = false;
            this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Hide;
            this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
            this->Text = L"Add/Modify Rule";
            this->ResumeLayout(false);
            this->PerformLayout();

        }
#pragma endregion
    private: System::Void rb_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 EnableDisableAndLabels();
             }
             void FillDialog()
             {
                 switch (mRule->DoWhatNow)
                 {
                 case kRename:
                     rbRename->Checked = true;
                     break;

                 case kCollapse:
                     rbCollapse->Checked = true;
                     break;

                 case kRemove:
                     rbRemove->Checked = true;
                     break;

                 case kSwap:
                     rbSwap->Checked = true;
                     break;

                 case kMerge:
                     rbMerge->Checked = true;
                     break;

                 case kSplit:
                     rbSplit->Checked = true;
                     break;

                 case kInsert:
                     rbInsert->Checked = true;
                     break;

                 default:
                 case kIgnoreEp:
                     rbIgnore->Checked = true;
                     break;
                 }

                 txtUserText->Text = mRule->UserSuppliedText;
                 if (mRule->First != -1)
                     txtValue1->Text = mRule->First.ToString();
                 if (mRule->Second != -1)
                     txtValue2->Text = mRule->Second.ToString();

                 EnableDisableAndLabels();

             }

             void EnableDisableAndLabels()
             {
                 if (rbRemove->Checked)
                 {
                     txtLabel1->Text = "&From/Number:";
                     txtLabel2->Text = "T&o:";
                     txtLeaveBlank->Visible = false;
                     txtLabel2->Enabled = true;
                     txtValue1->Enabled = true;
                     txtValue2->Enabled = true;
                     txtUserText->Enabled = false;
                     txtWithNameLabel->Enabled = false;
                 }
                 else if (rbSwap->Checked)
                 {
                     txtLabel1->Text = "&Number:";
                     txtLabel2->Text = "N&umber:";
                     txtLeaveBlank->Visible = false;
                     txtLabel2->Enabled = true;
                     txtValue1->Enabled = true;
                     txtValue2->Enabled = true;
                     txtUserText->Enabled = false;
                     txtWithNameLabel->Enabled = false;
                 }
                 else if (rbMerge->Checked || rbCollapse->Checked)
                 {
                     txtLabel1->Text = "&From:";
                     txtLabel2->Text = "T&o:";
                     txtLeaveBlank->Visible = true;
                     txtLabel2->Enabled = true;
                     txtValue1->Enabled = true;
                     txtValue2->Enabled = true;
                     txtUserText->Enabled = true;
                     txtWithNameLabel->Enabled = true;
                 }
                 else if (rbInsert->Checked)
                 {
                     txtLabel1->Text = "&At:";
                     txtLabel2->Text = "N&umber:";
                     txtLeaveBlank->Visible = false;
                     txtLabel2->Enabled = false;
                     txtValue1->Enabled = true;
                     txtValue2->Enabled = false;
                     txtUserText->Enabled = true;
                     txtWithNameLabel->Enabled = true;
                 } 
                 else if(rbIgnore->Checked)
                 {
                     txtLabel1->Text = "&From/Number:";
                     txtLabel2->Text = "T&o:";
                     txtLeaveBlank->Visible = false;
                     txtLabel2->Enabled = true;
                     txtValue1->Enabled = true;
                     txtValue2->Enabled = true;
                     txtUserText->Enabled = false;
                     txtWithNameLabel->Enabled = false;
                 }
                 else if (rbRename->Checked)
                 {
                     txtLabel1->Text = "&Number:";
                     txtLabel2->Text = "N&umber:";
                     txtLeaveBlank->Visible = false;
                     txtLabel2->Enabled = false;
                     txtValue1->Enabled = true;
                     txtValue2->Enabled = false;
                     txtUserText->Enabled = true;
                     txtWithNameLabel->Enabled = true;
                 }
                 else if (rbSplit->Checked)
                 {
                     txtLabel1->Text = "&Number:";
                     txtLabel2->Text = "Int&o:";
                     txtLeaveBlank->Visible = false;
                     txtLabel2->Enabled = true;
                     txtValue1->Enabled = true;
                     txtValue2->Enabled = true;
                     txtUserText->Enabled = false;
                     txtWithNameLabel->Enabled = false;
                 }
             }
    private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 RuleAction dwn;

                 if (rbIgnore->Checked) dwn = kIgnoreEp;
                 else if (rbSwap->Checked) dwn = kSwap;
                 else if (rbMerge->Checked) dwn = kMerge;
                 else if (rbInsert->Checked) dwn = kInsert;
                 else if (rbRemove->Checked) dwn = kRemove;
                 else if (rbCollapse->Checked) dwn = kCollapse;
                 else if (rbRename->Checked) dwn = kRename;
                 else if (rbSplit->Checked) dwn = kSplit;

                 mRule->DoWhatNow = dwn;
                 mRule->UserSuppliedText = txtUserText->Enabled ? txtUserText->Text : "";

                 try 
                 {
                     mRule->First = txtValue1->Enabled ? Convert::ToInt32(txtValue1->Text) : -1;
                 }
                 catch (...)
                 {
                     mRule->First = -1;
                 }

                 try 
                 {
                     mRule->Second = txtValue2->Enabled ? Convert::ToInt32(txtValue2->Text) : -1;
                 }
                 catch (...)
                 {
                     mRule->Second = -1;
                 }
             }
    };
}
