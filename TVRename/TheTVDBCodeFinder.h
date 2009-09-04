#pragma once

#include "TheTVDB.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Text::RegularExpressions;

namespace TVRename {

    /// <summary>
    /// Summary for TheTVDBCodeFinder
    /// </summary>
    public ref class TheTVDBCodeFinder : public System::Windows::Forms::UserControl
    {
        TheTVDB ^mTVDB;
        bool mInternal;

    public:
        event EventHandler ^SelectionChanged;

        TheTVDBCodeFinder(String ^initialHint, TheTVDB ^db)
        {
            mInternal = false;
            mTVDB = db;

            InitializeComponent();

            txtFindThis->Text = initialHint;
        }

        void SetHint(String ^s)
        {
            mInternal = true;
            txtFindThis->Text = s;
            mInternal = false;
            DoFind(true);
        }

        int SelectedCode()
        {
            try
            {
                if (lvMatches->SelectedItems->Count == 0)
                    return int::Parse(txtFindThis->Text);

                return safe_cast<int>(lvMatches->SelectedItems[0]->Tag);
            }
            catch (...)
            {
                return -1;
            }
        }


    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~TheTVDBCodeFinder()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::Label^  txtSearchStatus;
    protected: 
    private: System::Windows::Forms::Button^  bnGoSearch;
    private: System::Windows::Forms::TextBox^  txtFindThis;
    private: System::Windows::Forms::ListView^  lvMatches;
    private: System::Windows::Forms::ColumnHeader^  columnHeader1;
    private: System::Windows::Forms::ColumnHeader^  columnHeader2;
    private: System::Windows::Forms::Label^  label3;

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
            this->txtSearchStatus = (gcnew System::Windows::Forms::Label());
            this->bnGoSearch = (gcnew System::Windows::Forms::Button());
            this->txtFindThis = (gcnew System::Windows::Forms::TextBox());
            this->lvMatches = (gcnew System::Windows::Forms::ListView());
            this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
            this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
            this->label3 = (gcnew System::Windows::Forms::Label());
            this->SuspendLayout();
            // 
            // txtSearchStatus
            // 
            this->txtSearchStatus->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
                | System::Windows::Forms::AnchorStyles::Right));
            this->txtSearchStatus->BorderStyle = System::Windows::Forms::BorderStyle::Fixed3D;
            this->txtSearchStatus->Location = System::Drawing::Point(2, 153);
            this->txtSearchStatus->Name = L"txtSearchStatus";
            this->txtSearchStatus->Size = System::Drawing::Size(397, 15);
            this->txtSearchStatus->TabIndex = 9;
            this->txtSearchStatus->Text = L"                    ";
            // 
            // bnGoSearch
            // 
            this->bnGoSearch->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
            this->bnGoSearch->Location = System::Drawing::Point(324, -1);
            this->bnGoSearch->Name = L"bnGoSearch";
            this->bnGoSearch->Size = System::Drawing::Size(75, 23);
            this->bnGoSearch->TabIndex = 7;
            this->bnGoSearch->Text = L"&Search";
            this->bnGoSearch->UseVisualStyleBackColor = true;
            this->bnGoSearch->Click += gcnew System::EventHandler(this, &TheTVDBCodeFinder::bnGoSearch_Click);
            // 
            // txtFindThis
            // 
            this->txtFindThis->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                | System::Windows::Forms::AnchorStyles::Right));
            this->txtFindThis->Location = System::Drawing::Point(90, 1);
            this->txtFindThis->Name = L"txtFindThis";
            this->txtFindThis->Size = System::Drawing::Size(228, 20);
            this->txtFindThis->TabIndex = 6;
            this->txtFindThis->TextChanged += gcnew System::EventHandler(this, &TheTVDBCodeFinder::txtFindThis_TextChanged);
            // 
            // lvMatches
            // 
            this->lvMatches->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
                | System::Windows::Forms::AnchorStyles::Left) 
                | System::Windows::Forms::AnchorStyles::Right));
            this->lvMatches->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(2) {this->columnHeader1, this->columnHeader2});
            this->lvMatches->FullRowSelect = true;
            this->lvMatches->HideSelection = false;
            this->lvMatches->Location = System::Drawing::Point(1, 27);
            this->lvMatches->MultiSelect = false;
            this->lvMatches->Name = L"lvMatches";
            this->lvMatches->ShowItemToolTips = true;
            this->lvMatches->Size = System::Drawing::Size(397, 123);
            this->lvMatches->TabIndex = 8;
            this->lvMatches->UseCompatibleStateImageBehavior = false;
            this->lvMatches->View = System::Windows::Forms::View::Details;
            this->lvMatches->SelectedIndexChanged += gcnew System::EventHandler(this, &TheTVDBCodeFinder::lvMatches_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this->columnHeader1->Text = L"Code";
            this->columnHeader1->Width = 44;
            // 
            // columnHeader2
            // 
            this->columnHeader2->Text = L"Show Name";
            this->columnHeader2->Width = 334;
            // 
            // label3
            // 
            this->label3->AutoSize = true;
            this->label3->Location = System::Drawing::Point(-1, 4);
            this->label3->Name = L"label3";
            this->label3->Size = System::Drawing::Size(85, 13);
            this->label3->TabIndex = 5;
            this->label3->Text = L"TheTVDB &code:";
            this->label3->TextAlign = System::Drawing::ContentAlignment::TopRight;
            // 
            // TheTVDBCodeFinder
            // 
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->Controls->Add(this->txtSearchStatus);
            this->Controls->Add(this->bnGoSearch);
            this->Controls->Add(this->txtFindThis);
            this->Controls->Add(this->lvMatches);
            this->Controls->Add(this->label3);
            this->Name = L"TheTVDBCodeFinder";
            this->Size = System::Drawing::Size(403, 170);
            this->ResumeLayout(false);
            this->PerformLayout();

        }
#pragma endregion
    private: System::Void txtFindThis_TextChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (!mInternal)
                     DoFind(false);
             }
             void DoFind(bool chooseOnlyMatch)
             {
                 if (mInternal)
                     return;

                 lvMatches->BeginUpdate();

                 String ^what = txtFindThis->Text;

                 lvMatches->Items->Clear();
                 if (what != "")
                 {				 
                     what = what->ToLower();

                     bool numeric = Regex::Match(what,"^[0-9]+$")->Success;
                     int matchnum = 0;
                     try 
                     {
                         matchnum = numeric ? int::Parse(what) : -1;
                     }
                     catch (OverflowException ^)
                     {
                     } 

                     mTVDB->GetLock("DoFind");
                     for each (KeyValuePair<int, SeriesInfo^> ^kvp in mTVDB->GetSeriesDict())
                     {
                         int num = kvp->Key;
                         String ^show = kvp->Value->Name;
                         String ^s = num.ToString() + " " + show;

                         String ^simpleS = Regex::Replace(s->ToLower(),"[^\\w ]","");

                         bool numberMatch = numeric && num == matchnum;

                         if ( numberMatch ||
                             (!numeric && (simpleS->Contains(Regex::Replace(what,"[^\\w ]","")))) ||
                             (numeric && show->Contains(what)) )
                         {
                             ListViewItem ^lvi = gcnew ListViewItem;
                             lvi->Text = num.ToString();
                             lvi->SubItems->Add(show);
                             lvi->Tag = num;
                             if (numberMatch)
                                 lvi->Selected = true;
                             lvMatches->Items->Add(lvi);
                         }
                     }
                     mTVDB->Unlock("DoFind");

                     if ((lvMatches->Items->Count == 1) && numeric)
                         lvMatches->Items[0]->Selected = true;

                     int n = lvMatches->Items->Count;
                     txtSearchStatus->Text = "Found " + n + " show" + ((n!=1) ? "s":"");
                 }
                 else
                     txtSearchStatus->Text = "";

                 lvMatches->EndUpdate();

                 if (lvMatches->Items->Count == 1)
                     lvMatches->Items[0]->Selected = true;
             }
    private: System::Void bnGoSearch_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 // search on thetvdb.com site
                 txtSearchStatus->Text = "Searching on TheTVDB.com";
                 txtSearchStatus->Update();

                 //String ^url = "http://www.tv.com/search.php?stype=program&qs="+txtFindThis->Text+"&type=11&stype=search&tag=search%3Bbutton";

                 mTVDB->Search(txtFindThis->Text);

                 DoFind(true);
             }
    private: System::Void lvMatches_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 this->SelectionChanged(sender,e);
             }
    };
}
