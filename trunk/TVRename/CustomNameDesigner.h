#pragma once

#include "CustomName.h"
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
    /// Summary for CustomNameDesigner
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public ref class CustomNameDesigner : public System::Windows::Forms::Form
    {
        ProcessedEpisodeList ^Eps;
        CustomName ^CN;
        TVDoc ^mDoc;

    public:
        CustomNameDesigner(ProcessedEpisodeList ^pel, CustomName ^cn, TVDoc ^doc)
        {
            Eps = pel;
            CN = cn;
            mDoc = doc;

            InitializeComponent();

            if (Eps == nullptr)
                lvTest->Enabled = false;
            txtTemplate->Text = CN->StyleString;

            FillExamples();
            FillCombos();
        }

    private:
        void FillCombos()
        {
            cbTags->Items->Clear();
            cbPresets->Items->Clear();
            ProcessedEpisode ^pe = nullptr;
            if (lvTest->SelectedItems->Count == 0)
                pe = ((Eps != nullptr) && (Eps->Count)) ? Eps[0] : nullptr;
            else
                pe = safe_cast<ProcessedEpisode ^>(lvTest->SelectedItems[0]->Tag);

            for each (String ^s in CustomName::Tags())
            {
                String ^txt = s;
                if (pe != nullptr)
                    txt += " - " + CustomName::NameFor(pe, s);
                cbTags->Items->Add(txt);
            }

            for each (String ^s in CustomName::Presets())
            {
                if (pe != nullptr)
                    cbPresets->Items->Add(CustomName::NameFor(pe, s));
                else
                    cbPresets->Items->Add(s);
            }

        }

    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~CustomNameDesigner()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::TextBox^  txtTemplate;
    protected: 

    private: System::Windows::Forms::ComboBox^  cbPresets;
    private: System::Windows::Forms::Label^  label2;
    private: System::Windows::Forms::Label^  label3;
    private: System::Windows::Forms::ListView^  lvTest;
    private: System::Windows::Forms::ColumnHeader^  columnHeader1;
    private: System::Windows::Forms::ColumnHeader^  columnHeader2;
    private: System::Windows::Forms::ColumnHeader^  columnHeader3;
    private: System::Windows::Forms::Button^  bnOK;

    private: System::Windows::Forms::Button^  bnCancel;

    private: System::Windows::Forms::Label^  label4;
    private: System::Windows::Forms::ComboBox^  cbTags;


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
            System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(CustomNameDesigner::typeid));
            this->label1 = (gcnew System::Windows::Forms::Label());
            this->txtTemplate = (gcnew System::Windows::Forms::TextBox());
            this->cbPresets = (gcnew System::Windows::Forms::ComboBox());
            this->label2 = (gcnew System::Windows::Forms::Label());
            this->label3 = (gcnew System::Windows::Forms::Label());
            this->lvTest = (gcnew System::Windows::Forms::ListView());
            this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
            this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
            this->columnHeader3 = (gcnew System::Windows::Forms::ColumnHeader());
            this->bnOK = (gcnew System::Windows::Forms::Button());
            this->bnCancel = (gcnew System::Windows::Forms::Button());
            this->label4 = (gcnew System::Windows::Forms::Label());
            this->cbTags = (gcnew System::Windows::Forms::ComboBox());
            this->SuspendLayout();
            // 
            // label1
            // 
            this->label1->AutoSize = true;
            this->label1->Location = System::Drawing::Point(12, 13);
            this->label1->Name = L"label1";
            this->label1->Size = System::Drawing::Size(89, 13);
            this->label1->TabIndex = 0;
            this->label1->Text = L"Naming template:";
            // 
            // txtTemplate
            // 
            this->txtTemplate->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
                | System::Windows::Forms::AnchorStyles::Right));
            this->txtTemplate->Location = System::Drawing::Point(107, 10);
            this->txtTemplate->Name = L"txtTemplate";
            this->txtTemplate->Size = System::Drawing::Size(590, 20);
            this->txtTemplate->TabIndex = 1;
            this->txtTemplate->TextChanged += gcnew System::EventHandler(this, &CustomNameDesigner::txtTemplate_TextChanged);
            // 
            // cbPresets
            // 
            this->cbPresets->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
            this->cbPresets->FormattingEnabled = true;
            this->cbPresets->Location = System::Drawing::Point(107, 36);
            this->cbPresets->Name = L"cbPresets";
            this->cbPresets->Size = System::Drawing::Size(381, 21);
            this->cbPresets->TabIndex = 2;
            this->cbPresets->SelectedIndexChanged += gcnew System::EventHandler(this, &CustomNameDesigner::cbPresets_SelectedIndexChanged);
            // 
            // label2
            // 
            this->label2->AutoSize = true;
            this->label2->Location = System::Drawing::Point(12, 40);
            this->label2->Name = L"label2";
            this->label2->Size = System::Drawing::Size(45, 13);
            this->label2->TabIndex = 0;
            this->label2->Text = L"Presets:";
            // 
            // label3
            // 
            this->label3->AutoSize = true;
            this->label3->Location = System::Drawing::Point(12, 98);
            this->label3->Name = L"label3";
            this->label3->Size = System::Drawing::Size(90, 13);
            this->label3->TabIndex = 0;
            this->label3->Text = L"Sample and Test:";
            // 
            // lvTest
            // 
            this->lvTest->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
                | System::Windows::Forms::AnchorStyles::Left) 
                | System::Windows::Forms::AnchorStyles::Right));
            this->lvTest->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(3) {this->columnHeader1, this->columnHeader2, 
                this->columnHeader3});
            this->lvTest->HeaderStyle = System::Windows::Forms::ColumnHeaderStyle::Nonclickable;
            this->lvTest->Location = System::Drawing::Point(15, 114);
            this->lvTest->Name = L"lvTest";
            this->lvTest->Size = System::Drawing::Size(682, 265);
            this->lvTest->TabIndex = 3;
            this->lvTest->UseCompatibleStateImageBehavior = false;
            this->lvTest->View = System::Windows::Forms::View::Details;
            this->lvTest->SelectedIndexChanged += gcnew System::EventHandler(this, &CustomNameDesigner::lvTest_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this->columnHeader1->Text = L"Example";
            this->columnHeader1->Width = 456;
            // 
            // columnHeader2
            // 
            this->columnHeader2->Text = L"Season";
            // 
            // columnHeader3
            // 
            this->columnHeader3->Text = L"Episode";
            // 
            // bnOK
            // 
            this->bnOK->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
            this->bnOK->DialogResult = System::Windows::Forms::DialogResult::OK;
            this->bnOK->Location = System::Drawing::Point(541, 385);
            this->bnOK->Name = L"bnOK";
            this->bnOK->Size = System::Drawing::Size(75, 23);
            this->bnOK->TabIndex = 4;
            this->bnOK->Text = L"OK";
            this->bnOK->UseVisualStyleBackColor = true;
            // 
            // bnCancel
            // 
            this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
            this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
            this->bnCancel->Location = System::Drawing::Point(622, 385);
            this->bnCancel->Name = L"bnCancel";
            this->bnCancel->Size = System::Drawing::Size(75, 23);
            this->bnCancel->TabIndex = 5;
            this->bnCancel->Text = L"Cancel";
            this->bnCancel->UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this->label4->AutoSize = true;
            this->label4->Location = System::Drawing::Point(12, 66);
            this->label4->Name = L"label4";
            this->label4->Size = System::Drawing::Size(34, 13);
            this->label4->TabIndex = 0;
            this->label4->Text = L"Tags:";
            // 
            // cbTags
            // 
            this->cbTags->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
            this->cbTags->FormattingEnabled = true;
            this->cbTags->Location = System::Drawing::Point(107, 63);
            this->cbTags->Name = L"cbTags";
            this->cbTags->Size = System::Drawing::Size(381, 21);
            this->cbTags->TabIndex = 2;
            this->cbTags->SelectedIndexChanged += gcnew System::EventHandler(this, &CustomNameDesigner::cbTags_SelectedIndexChanged);
            // 
            // CustomNameDesigner
            // 
            this->AcceptButton = this->bnOK;
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->CancelButton = this->bnCancel;
            this->ClientSize = System::Drawing::Size(709, 420);
            this->Controls->Add(this->bnCancel);
            this->Controls->Add(this->bnOK);
            this->Controls->Add(this->lvTest);
            this->Controls->Add(this->cbTags);
            this->Controls->Add(this->cbPresets);
            this->Controls->Add(this->label3);
            this->Controls->Add(this->label4);
            this->Controls->Add(this->label2);
            this->Controls->Add(this->txtTemplate);
            this->Controls->Add(this->label1);
            this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
            this->Name = L"CustomNameDesigner";
            this->ShowInTaskbar = false;
            this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
            this->Text = L"Filename Template Editor";
            this->ResumeLayout(false);
            this->PerformLayout();

        }
#pragma endregion

        void FillExamples()
        {
            if (Eps == nullptr)
                return;

            lvTest->Items->Clear();
            for each (ProcessedEpisode ^pe in Eps)
            {
                ListViewItem ^lvi = gcnew ListViewItem();
                String ^fn = mDoc->FilenameFriendly(CN->NameFor(pe));
                lvi->Text = fn;

                bool ok = false, ok1 = false, ok2 = false;
                if (fn->Length < 100)
                {
                    int seas, ep;
                    ok = mDoc->FindSeasEp(gcnew FileInfo(fn+".avi"), &seas, &ep, pe->SI->ShowName());
                    ok1 = ok && (seas == pe->SeasonNumber);
                    ok2 = ok && (ep == pe->EpNum);
                    String ^pre1 = ok1 ? "" : "* ";
                    String ^pre2 = ok2 ? "" : "* ";

                    lvi->SubItems->Add(pre1 + ((seas != -1) ? seas.ToString() : ""));
                    lvi->SubItems->Add(pre2 + ((ep != -1) ? ep.ToString() : ""));
                    lvi->Tag = pe;
                }
                if (!ok || !ok1 || !ok2)
                    lvi->BackColor = WarningColor();
                lvTest->Items->Add(lvi);
            }
        }
    private: System::Void cbPresets_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 int n = cbPresets->SelectedIndex;
                 if (n == -1)
                     return;

                 txtTemplate->Text = CustomName::Presets()[n];
                 cbPresets->SelectedIndex = -1;
             }
    private: System::Void txtTemplate_TextChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 CN->StyleString = txtTemplate->Text;
                 FillExamples();
             }
    private: System::Void cbTags_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 int n = cbTags->SelectedIndex;
                 if (n == -1)
                     return;

                 int p = txtTemplate->SelectionStart;
                 String ^s = txtTemplate->Text;
                 txtTemplate->Text = s->Substring(0, p) + CustomName::Tags()[cbTags->SelectedIndex] + s->Substring(p);

                 cbTags->SelectedIndex = -1;
             }
    private: System::Void lvTest_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 FillCombos();
             }
    };
}
