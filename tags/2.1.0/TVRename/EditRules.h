#pragma once

#include "ShowItem.h"
#include "AddModifyRule.h"
#include "TVDoc.h"
#include "CustomName.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

    /// <summary>
    /// Summary for EditRules
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public ref class EditRules : public System::Windows::Forms::Form
    {
        RuleSet ^WorkingRuleSet;
        ShowItem ^mSI;
        int mSeasonNumber;
        ProcessedEpisodeList ^mOriginalEps;
        CustomName ^NameStyle;

    public:
        EditRules(ShowItem ^si, ProcessedEpisodeList ^originalEpList, int seasonNumber, CustomName ^style)
        {
            NameStyle = style;
            InitializeComponent();

            mSI = si;
            mOriginalEps = originalEpList;
            mSeasonNumber = seasonNumber;

            if (si->SeasonRules->ContainsKey(seasonNumber))
                WorkingRuleSet = gcnew RuleSet(si->SeasonRules[seasonNumber]);
            else
                WorkingRuleSet = gcnew RuleSet();

            txtShowName->Text = si->ShowName();
            txtSeasonNumber->Text = seasonNumber.ToString();

            FillRuleList(false, 0);
        }

    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~EditRules()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::Button^  bnRuleUp;
    protected: 
    private: System::Windows::Forms::Button^  bnRuleDown;
    private: System::Windows::Forms::ListView^  lvRuleList;
    private: System::Windows::Forms::ColumnHeader^  columnHeader3;
    private: System::Windows::Forms::ColumnHeader^  columnHeader4;
    private: System::Windows::Forms::ColumnHeader^  columnHeader5;
    private: System::Windows::Forms::ColumnHeader^  columnHeader6;
    private: System::Windows::Forms::Button^  bnDelRule;
    private: System::Windows::Forms::Button^  bnEdit;
    private: System::Windows::Forms::Button^  bnAddRule;
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::Label^  label2;
    private: System::Windows::Forms::Label^  label3;
    private: System::Windows::Forms::ListBox^  lbEpsPreview;

    private: System::Windows::Forms::Label^  label4;
    private: System::Windows::Forms::Button^  bnCancel;

    private: System::Windows::Forms::Button^  bnOK;

    private: System::Windows::Forms::Label^  txtShowName;
    private: System::Windows::Forms::Label^  txtSeasonNumber;

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
            System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(EditRules::typeid));
            this->bnRuleUp = (gcnew System::Windows::Forms::Button());
            this->bnRuleDown = (gcnew System::Windows::Forms::Button());
            this->lvRuleList = (gcnew System::Windows::Forms::ListView());
            this->columnHeader3 = (gcnew System::Windows::Forms::ColumnHeader());
            this->columnHeader4 = (gcnew System::Windows::Forms::ColumnHeader());
            this->columnHeader5 = (gcnew System::Windows::Forms::ColumnHeader());
            this->columnHeader6 = (gcnew System::Windows::Forms::ColumnHeader());
            this->bnDelRule = (gcnew System::Windows::Forms::Button());
            this->bnEdit = (gcnew System::Windows::Forms::Button());
            this->bnAddRule = (gcnew System::Windows::Forms::Button());
            this->label1 = (gcnew System::Windows::Forms::Label());
            this->label2 = (gcnew System::Windows::Forms::Label());
            this->label3 = (gcnew System::Windows::Forms::Label());
            this->lbEpsPreview = (gcnew System::Windows::Forms::ListBox());
            this->label4 = (gcnew System::Windows::Forms::Label());
            this->bnCancel = (gcnew System::Windows::Forms::Button());
            this->bnOK = (gcnew System::Windows::Forms::Button());
            this->txtShowName = (gcnew System::Windows::Forms::Label());
            this->txtSeasonNumber = (gcnew System::Windows::Forms::Label());
            this->SuspendLayout();
            // 
            // bnRuleUp
            // 
            this->bnRuleUp->Location = System::Drawing::Point(255, 226);
            this->bnRuleUp->Name = L"bnRuleUp";
            this->bnRuleUp->Size = System::Drawing::Size(75, 23);
            this->bnRuleUp->TabIndex = 9;
            this->bnRuleUp->Text = L"&Up";
            this->bnRuleUp->UseVisualStyleBackColor = true;
            this->bnRuleUp->Click += gcnew System::EventHandler(this, &EditRules::bnRuleUp_Click);
            // 
            // bnRuleDown
            // 
            this->bnRuleDown->Location = System::Drawing::Point(336, 226);
            this->bnRuleDown->Name = L"bnRuleDown";
            this->bnRuleDown->Size = System::Drawing::Size(75, 23);
            this->bnRuleDown->TabIndex = 10;
            this->bnRuleDown->Text = L"Do&wn";
            this->bnRuleDown->UseVisualStyleBackColor = true;
            this->bnRuleDown->Click += gcnew System::EventHandler(this, &EditRules::bnRuleDown_Click);
            // 
            // lvRuleList
            // 
            this->lvRuleList->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(4) {this->columnHeader3, this->columnHeader4, 
                this->columnHeader5, this->columnHeader6});
            this->lvRuleList->FullRowSelect = true;
            this->lvRuleList->HideSelection = false;
            this->lvRuleList->Location = System::Drawing::Point(12, 73);
            this->lvRuleList->MultiSelect = false;
            this->lvRuleList->Name = L"lvRuleList";
            this->lvRuleList->Size = System::Drawing::Size(400, 147);
            this->lvRuleList->TabIndex = 5;
            this->lvRuleList->UseCompatibleStateImageBehavior = false;
            this->lvRuleList->View = System::Windows::Forms::View::Details;
            this->lvRuleList->DoubleClick += gcnew System::EventHandler(this, &EditRules::lvRuleList_DoubleClick);
            // 
            // columnHeader3
            // 
            this->columnHeader3->Text = L"Action";
            // 
            // columnHeader4
            // 
            this->columnHeader4->Text = L"Episode";
            // 
            // columnHeader5
            // 
            this->columnHeader5->Text = L"Episode";
            // 
            // columnHeader6
            // 
            this->columnHeader6->Text = L"Name";
            this->columnHeader6->Width = 205;
            // 
            // bnDelRule
            // 
            this->bnDelRule->Location = System::Drawing::Point(174, 226);
            this->bnDelRule->Name = L"bnDelRule";
            this->bnDelRule->Size = System::Drawing::Size(75, 23);
            this->bnDelRule->TabIndex = 8;
            this->bnDelRule->Text = L"&Delete";
            this->bnDelRule->UseVisualStyleBackColor = true;
            this->bnDelRule->Click += gcnew System::EventHandler(this, &EditRules::bnDelRule_Click);
            // 
            // bnEdit
            // 
            this->bnEdit->Location = System::Drawing::Point(93, 226);
            this->bnEdit->Name = L"bnEdit";
            this->bnEdit->Size = System::Drawing::Size(75, 23);
            this->bnEdit->TabIndex = 7;
            this->bnEdit->Text = L"&Edit";
            this->bnEdit->UseVisualStyleBackColor = true;
            this->bnEdit->Click += gcnew System::EventHandler(this, &EditRules::bnEdit_Click);
            // 
            // bnAddRule
            // 
            this->bnAddRule->Location = System::Drawing::Point(12, 226);
            this->bnAddRule->Name = L"bnAddRule";
            this->bnAddRule->Size = System::Drawing::Size(75, 23);
            this->bnAddRule->TabIndex = 6;
            this->bnAddRule->Text = L"&Add";
            this->bnAddRule->UseVisualStyleBackColor = true;
            this->bnAddRule->Click += gcnew System::EventHandler(this, &EditRules::bnAddRule_Click);
            // 
            // label1
            // 
            this->label1->AutoSize = true;
            this->label1->Location = System::Drawing::Point(9, 54);
            this->label1->Name = L"label1";
            this->label1->Size = System::Drawing::Size(37, 13);
            this->label1->TabIndex = 4;
            this->label1->Text = L"&Rules:";
            this->label1->TextAlign = System::Drawing::ContentAlignment::TopRight;
            // 
            // label2
            // 
            this->label2->AutoSize = true;
            this->label2->Location = System::Drawing::Point(9, 6);
            this->label2->Name = L"label2";
            this->label2->Size = System::Drawing::Size(37, 13);
            this->label2->TabIndex = 0;
            this->label2->Text = L"Show:";
            this->label2->TextAlign = System::Drawing::ContentAlignment::TopRight;
            // 
            // label3
            // 
            this->label3->AutoSize = true;
            this->label3->Location = System::Drawing::Point(9, 29);
            this->label3->Name = L"label3";
            this->label3->Size = System::Drawing::Size(46, 13);
            this->label3->TabIndex = 2;
            this->label3->Text = L"Season:";
            this->label3->TextAlign = System::Drawing::ContentAlignment::TopRight;
            // 
            // lbEpsPreview
            // 
            this->lbEpsPreview->FormattingEnabled = true;
            this->lbEpsPreview->Location = System::Drawing::Point(12, 276);
            this->lbEpsPreview->Name = L"lbEpsPreview";
            this->lbEpsPreview->ScrollAlwaysVisible = true;
            this->lbEpsPreview->SelectionMode = System::Windows::Forms::SelectionMode::None;
            this->lbEpsPreview->Size = System::Drawing::Size(400, 160);
            this->lbEpsPreview->TabIndex = 12;
            // 
            // label4
            // 
            this->label4->AutoSize = true;
            this->label4->Location = System::Drawing::Point(9, 257);
            this->label4->Name = L"label4";
            this->label4->Size = System::Drawing::Size(48, 13);
            this->label4->TabIndex = 11;
            this->label4->Text = L"Preview:";
            this->label4->TextAlign = System::Drawing::ContentAlignment::TopRight;
            // 
            // bnCancel
            // 
            this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
            this->bnCancel->Location = System::Drawing::Point(337, 442);
            this->bnCancel->Name = L"bnCancel";
            this->bnCancel->Size = System::Drawing::Size(75, 23);
            this->bnCancel->TabIndex = 14;
            this->bnCancel->Text = L"Cancel";
            this->bnCancel->UseVisualStyleBackColor = true;
            this->bnCancel->Click += gcnew System::EventHandler(this, &EditRules::bnCancel_Click);
            // 
            // bnOK
            // 
            this->bnOK->DialogResult = System::Windows::Forms::DialogResult::OK;
            this->bnOK->Location = System::Drawing::Point(256, 442);
            this->bnOK->Name = L"bnOK";
            this->bnOK->Size = System::Drawing::Size(75, 23);
            this->bnOK->TabIndex = 13;
            this->bnOK->Text = L"OK";
            this->bnOK->UseVisualStyleBackColor = true;
            this->bnOK->Click += gcnew System::EventHandler(this, &EditRules::bnOK_Click);
            // 
            // txtShowName
            // 
            this->txtShowName->AutoSize = true;
            this->txtShowName->Location = System::Drawing::Point(67, 6);
            this->txtShowName->Name = L"txtShowName";
            this->txtShowName->Size = System::Drawing::Size(16, 13);
            this->txtShowName->TabIndex = 1;
            this->txtShowName->Text = L"---";
            this->txtShowName->TextAlign = System::Drawing::ContentAlignment::TopRight;
            // 
            // txtSeasonNumber
            // 
            this->txtSeasonNumber->AutoSize = true;
            this->txtSeasonNumber->Location = System::Drawing::Point(67, 29);
            this->txtSeasonNumber->Name = L"txtSeasonNumber";
            this->txtSeasonNumber->Size = System::Drawing::Size(16, 13);
            this->txtSeasonNumber->TabIndex = 3;
            this->txtSeasonNumber->Text = L"---";
            this->txtSeasonNumber->TextAlign = System::Drawing::ContentAlignment::TopRight;
            // 
            // EditRules
            // 
            this->AcceptButton = this->bnOK;
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->CancelButton = this->bnCancel;
            this->ClientSize = System::Drawing::Size(424, 478);
            this->Controls->Add(this->lbEpsPreview);
            this->Controls->Add(this->bnOK);
            this->Controls->Add(this->bnRuleUp);
            this->Controls->Add(this->bnCancel);
            this->Controls->Add(this->bnRuleDown);
            this->Controls->Add(this->lvRuleList);
            this->Controls->Add(this->bnDelRule);
            this->Controls->Add(this->bnEdit);
            this->Controls->Add(this->bnAddRule);
            this->Controls->Add(this->txtSeasonNumber);
            this->Controls->Add(this->label3);
            this->Controls->Add(this->txtShowName);
            this->Controls->Add(this->label2);
            this->Controls->Add(this->label4);
            this->Controls->Add(this->label1);
            this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
            this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
            this->Name = L"EditRules";
            this->ShowInTaskbar = false;
            this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
            this->Text = L"Edit Season Rules";
            this->ResumeLayout(false);
            this->PerformLayout();

        }
#pragma endregion
    private: System::Void bnAddRule_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 ShowRule ^sr = gcnew ShowRule();
                 AddModifyRule ^ar = gcnew AddModifyRule(sr);

                 bool res = ar->ShowDialog() == System::Windows::Forms::DialogResult::OK;
                 if (res)
                 WorkingRuleSet->Add(sr);

                 FillRuleList(false, 0);
             }
             void FillRuleList(bool keepSel, int adj)
             {
                 Generic::List<int> ^sel = gcnew Generic::List<int>;
                 if (keepSel)
                     for each (int i in lvRuleList->SelectedIndices)
                         sel->Add(i);

                 lvRuleList->Items->Clear();
                 for each (ShowRule ^sr in WorkingRuleSet)
                 {
                     ListViewItem ^lvi = gcnew ListViewItem();
                     lvi->Text = sr->ActionInWords();
                     lvi->SubItems->Add(sr->First == -1 ? "" : sr->First.ToString());
                     lvi->SubItems->Add(sr->Second == -1 ? "" : sr->Second.ToString());
                     lvi->SubItems->Add(sr->UserSuppliedText);
                     lvi->Tag = sr;
                     lvRuleList->Items->Add(lvi);
                 }

                 if (keepSel)
                     for each (int i in sel)
                         if (i < lvRuleList->Items->Count)
                         {
                             int n = i+adj;
                             if ((n >= 0) && (n < lvRuleList->Items->Count))
                                 lvRuleList->Items[n]->Selected = true;
                         }

                 FillPreview();

             }

    private: System::Void bnEdit_Click(System::Object^  sender, System::EventArgs^  e) 
             {

                 if (lvRuleList->SelectedItems->Count == 0)
                     return;
                 ShowRule ^sr = safe_cast<ShowRule ^>(lvRuleList->SelectedItems[0]->Tag);
                 AddModifyRule ^ar = gcnew AddModifyRule(sr);
                 ar->ShowDialog(); // modifies rule in-place if OK'd
                 FillRuleList(false, 0);

             }
    private: System::Void bnDelRule_Click(System::Object^  sender, System::EventArgs^  e) 
             {

                 if (lvRuleList->SelectedItems->Count == 0)
                     return;
                 ShowRule ^sr = safe_cast<ShowRule ^>(lvRuleList->SelectedItems[0]->Tag);

                 WorkingRuleSet->Remove(sr);
                 FillRuleList(false, 0);

             }

    private: System::Void bnRuleUp_Click(System::Object^  sender, System::EventArgs^  e) 
             {                             
                 if (lvRuleList->SelectedIndices->Count != 1)
                     return;
                 int p = lvRuleList->SelectedIndices[0];
                 if (p <= 0)
                     return;

                 ShowRule ^sr = WorkingRuleSet[p];
                 WorkingRuleSet->RemoveAt(p);
                 WorkingRuleSet->Insert(p-1, sr);

                 FillRuleList(true, -1);

             }
    private: System::Void bnRuleDown_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (lvRuleList->SelectedIndices->Count != 1)
                     return;
                 int p = lvRuleList->SelectedIndices[0];
                 if (p >= (lvRuleList->Items->Count - 1))
                     return;

                 ShowRule ^sr = WorkingRuleSet[p];
                 WorkingRuleSet->RemoveAt(p);
                 WorkingRuleSet->Insert(p+1, sr);
                 FillRuleList(true, +1);
             }


private: System::Void lvRuleList_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
         {
                 bnEdit_Click(nullptr,nullptr);
         }
private: System::Void bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
         {
             mSI->SeasonRules[mSeasonNumber] = WorkingRuleSet;
             this->Close();
         }
private: System::Void bnCancel_Click(System::Object^  sender, System::EventArgs^  e) 
         {
             this->Close();
         }
         void FillPreview()
         {
             ProcessedEpisodeList ^pel = gcnew ProcessedEpisodeList();
             
             if (mOriginalEps != nullptr)
             {
                 for each (ProcessedEpisode ^pe in mOriginalEps)
                     pel->Add(gcnew ProcessedEpisode(pe));

                 TVDoc::ApplyRules(pel, WorkingRuleSet, mSI);
             }

             lbEpsPreview->BeginUpdate();
             lbEpsPreview->Items->Clear();
             for each (ProcessedEpisode ^pe in pel)
                 lbEpsPreview->Items->Add(NameStyle->NameFor(pe));
             lbEpsPreview->EndUpdate();
         }
};
}
