//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "ShowItem.h"
#include "TheTVDBCodeFinder.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


//            this->txtCustomShowName->TextChanged += gcnew System::EventHandler(this, &AddEditShow::txtCustomShowName_TextChanged);

namespace TVRename {

    /// <summary>
    /// Summary for AddEditShow
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public ref class AddEditShow : public System::Windows::Forms::Form
    {
        ShowItem ^mSI;
        TheTVDB ^mTVDB;
        TheTVDBCodeFinder ^mTCCF;
        //bool mShowNameBlank;

    public: 
        String ^TimeZone;


    private: System::Windows::Forms::Panel^  pnlCF;
    private: System::Windows::Forms::Label^  label3;
    private: System::Windows::Forms::CheckBox^  chkFolderPerSeason;
    private: System::Windows::Forms::TextBox^  txtSeasonFolderName;
    private: System::Windows::Forms::Label^  txtHash;

    private: System::Windows::Forms::TextBox^  txtBaseFolder;
    private: System::Windows::Forms::Button^  bnBrowse;
    private: System::Windows::Forms::CheckBox^  chkAutoFolders;
    private: System::Windows::Forms::CheckBox^  cbDoRenaming;
    private: System::Windows::Forms::CheckBox^  cbDoMissingCheck;
    private: System::Windows::Forms::CheckBox^  chkThumbnailsAndStuff;
    private: System::Windows::Forms::FolderBrowserDialog^  folderBrowser;
    private: System::Windows::Forms::Label^  label5;
    private: System::Windows::Forms::CheckBox^  chkDVDOrder;
    private: System::Windows::Forms::CheckBox^  chkForceCheckAll;
    private: System::Windows::Forms::GroupBox^  gbAutoFolders;
    private: System::Windows::Forms::ListView^  lvSeasonFolders;
    private: System::Windows::Forms::ColumnHeader^  columnHeader1;
    private: System::Windows::Forms::ColumnHeader^  columnHeader2;
    private: System::Windows::Forms::TextBox^  txtSeasonNumber;
    private: System::Windows::Forms::TextBox^  txtFolder;
    private: System::Windows::Forms::Button^  bnBrowseFolder;
    private: System::Windows::Forms::Button^  bnAdd;
    private: System::Windows::Forms::Button^  bnRemove;
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::Label^  label7;
    private: System::Windows::Forms::GroupBox^  groupBox1;
    private: System::Windows::Forms::CheckBox^  cbSequentialMatching;
    private: System::Windows::Forms::CheckBox^  chkCustomShowName;
    private: System::Windows::Forms::CheckBox^  chkPadTwoDigits;

    private: System::Windows::Forms::TextBox^  txtIgnoreSeasons;


    public:
        AddEditShow(ShowItem ^si, TheTVDB ^db, String ^timezone) :
          mSI(si)
          {
              mTVDB = db;
              InitializeComponent();

              cbTimeZone->BeginUpdate();
              cbTimeZone->Items->Clear();

              RegistryKey ^rk = Registry::LocalMachine->OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones");
              array<String ^> ^skn = rk->GetSubKeyNames();

              for (int i=0;i<skn->Length;i++)
                  cbTimeZone->Items->Add(skn[i]);

              cbTimeZone->EndUpdate();


              mTCCF = gcnew TheTVDBCodeFinder(si->TVDBCode != -1 ? si->TVDBCode.ToString() : "", mTVDB);
              mTCCF->Dock = DockStyle::Fill;
              //mTCCF->SelectionChanged += gcnew System::EventHandler(this, &AddEditShow::lvMatches_ItemSelectionChanged);

              pnlCF->SuspendLayout();
              pnlCF->Controls->Add(mTCCF);
              pnlCF->ResumeLayout();

              chkCustomShowName->Checked = si->UseCustomShowName;
              if (chkCustomShowName->Checked)
                  txtCustomShowName->Text = si->CustomShowName;
              chkCustomShowName_CheckedChanged(nullptr,nullptr);

              cbSequentialMatching->Checked = si->UseSequentialMatch;
              chkShowNextAirdate->Checked = si->ShowNextAirdate;
              chkSpecialsCount->Checked = si->CountSpecials;
              chkFolderPerSeason->Checked = si->AutoAdd_FolderPerSeason;
              txtSeasonFolderName->Text = si->AutoAdd_SeasonFolderName;
              txtBaseFolder->Text = si->AutoAdd_FolderBase;
              chkAutoFolders->Checked = si->AutoAddNewSeasons;
              chkAutoFolders_CheckedChanged(nullptr,nullptr);
              chkFolderPerSeason_CheckedChanged(nullptr,nullptr);

              chkThumbnailsAndStuff->Checked = false; // TODO
              cbDoRenaming->Checked = si->DoRename;
              cbDoMissingCheck->Checked = si->DoMissingCheck;
              cbDoMissingCheck_CheckedChanged(nullptr,nullptr);

              chkPadTwoDigits->Checked = si->PadSeasonToTwoDigits;
              SetRightNumberOfHashes();

              TimeZone = (!String::IsNullOrEmpty(timezone)) ? timezone : TZMagic::DefaultTZ();
              cbTimeZone->Text = TimeZone;
              chkDVDOrder->Checked = si->DVDOrder;
              chkForceCheckAll->Checked = si->ForceCheckAll;

              bool first = true;
              si->IgnoreSeasons->Sort();
              for each (int i in si->IgnoreSeasons)
              {
                  if (!first)
                      txtIgnoreSeasons->Text += " ";
                  txtIgnoreSeasons->Text += i.ToString();
                  first = false;
              }

              for each (KeyValuePair<int, StringList ^> ^kvp in si->ManualFolderLocations)
              {
                  for each (String ^s in kvp->Value)
                  {
                      ListViewItem ^lvi = gcnew ListViewItem();
                      lvi->Text = kvp->Key.ToString();
                      lvi->SubItems->Add(s);

                      lvSeasonFolders->Items->Add(lvi);
                  }
              }
              lvSeasonFolders->Sort();

              txtSeasonNumber_TextChanged(nullptr,nullptr);
              txtFolder_TextChanged(nullptr,nullptr);

          }


    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~AddEditShow()
        {
            if (components)
            {
                delete components;
            }
        }

    protected: 



    private: System::Windows::Forms::TextBox^  txtCustomShowName;

    private: System::Windows::Forms::ComboBox^  cbTimeZone;
    private: System::Windows::Forms::Label^  label6;




    private: System::Windows::Forms::Button^  bnCancel;
    private: System::Windows::Forms::Button^  buttonOK;

    private: System::Windows::Forms::CheckBox^  chkSpecialsCount;



    private: System::Windows::Forms::CheckBox^  chkShowNextAirdate;







    private:
        /// <summary>
        /// Required designer variable.
        /// </summary>
        System::ComponentModel::Container ^components;



        // ShowItemRuleSet ^mWorkingRuleSet;


#pragma region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent(void)
        {
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(AddEditShow::typeid));
			this->txtCustomShowName = (gcnew System::Windows::Forms::TextBox());
			this->cbTimeZone = (gcnew System::Windows::Forms::ComboBox());
			this->label6 = (gcnew System::Windows::Forms::Label());
			this->bnCancel = (gcnew System::Windows::Forms::Button());
			this->buttonOK = (gcnew System::Windows::Forms::Button());
			this->chkSpecialsCount = (gcnew System::Windows::Forms::CheckBox());
			this->chkShowNextAirdate = (gcnew System::Windows::Forms::CheckBox());
			this->pnlCF = (gcnew System::Windows::Forms::Panel());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->chkFolderPerSeason = (gcnew System::Windows::Forms::CheckBox());
			this->txtSeasonFolderName = (gcnew System::Windows::Forms::TextBox());
			this->txtHash = (gcnew System::Windows::Forms::Label());
			this->txtBaseFolder = (gcnew System::Windows::Forms::TextBox());
			this->bnBrowse = (gcnew System::Windows::Forms::Button());
			this->chkAutoFolders = (gcnew System::Windows::Forms::CheckBox());
			this->cbDoRenaming = (gcnew System::Windows::Forms::CheckBox());
			this->cbDoMissingCheck = (gcnew System::Windows::Forms::CheckBox());
			this->chkThumbnailsAndStuff = (gcnew System::Windows::Forms::CheckBox());
			this->folderBrowser = (gcnew System::Windows::Forms::FolderBrowserDialog());
			this->label5 = (gcnew System::Windows::Forms::Label());
			this->txtIgnoreSeasons = (gcnew System::Windows::Forms::TextBox());
			this->chkDVDOrder = (gcnew System::Windows::Forms::CheckBox());
			this->chkForceCheckAll = (gcnew System::Windows::Forms::CheckBox());
			this->gbAutoFolders = (gcnew System::Windows::Forms::GroupBox());
			this->chkPadTwoDigits = (gcnew System::Windows::Forms::CheckBox());
			this->lvSeasonFolders = (gcnew System::Windows::Forms::ListView());
			this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
			this->txtSeasonNumber = (gcnew System::Windows::Forms::TextBox());
			this->txtFolder = (gcnew System::Windows::Forms::TextBox());
			this->bnBrowseFolder = (gcnew System::Windows::Forms::Button());
			this->bnAdd = (gcnew System::Windows::Forms::Button());
			this->bnRemove = (gcnew System::Windows::Forms::Button());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->label7 = (gcnew System::Windows::Forms::Label());
			this->groupBox1 = (gcnew System::Windows::Forms::GroupBox());
			this->cbSequentialMatching = (gcnew System::Windows::Forms::CheckBox());
			this->chkCustomShowName = (gcnew System::Windows::Forms::CheckBox());
			this->gbAutoFolders->SuspendLayout();
			this->groupBox1->SuspendLayout();
			this->SuspendLayout();
			// 
			// txtCustomShowName
			// 
			this->txtCustomShowName->Location = System::Drawing::Point(137, 175);
			this->txtCustomShowName->Name = L"txtCustomShowName";
			this->txtCustomShowName->Size = System::Drawing::Size(283, 20);
			this->txtCustomShowName->TabIndex = 2;
			// 
			// cbTimeZone
			// 
			this->cbTimeZone->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
			this->cbTimeZone->FormattingEnabled = true;
			this->cbTimeZone->Location = System::Drawing::Point(103, 201);
			this->cbTimeZone->Name = L"cbTimeZone";
			this->cbTimeZone->Size = System::Drawing::Size(200, 21);
			this->cbTimeZone->TabIndex = 7;
			// 
			// label6
			// 
			this->label6->AutoSize = true;
			this->label6->Location = System::Drawing::Point(8, 204);
			this->label6->Name = L"label6";
			this->label6->Size = System::Drawing::Size(87, 13);
			this->label6->TabIndex = 6;
			this->label6->Text = L"Airs in &Timezone:";
			this->label6->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// bnCancel
			// 
			this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnCancel->Location = System::Drawing::Point(347, 615);
			this->bnCancel->Name = L"bnCancel";
			this->bnCancel->Size = System::Drawing::Size(75, 23);
			this->bnCancel->TabIndex = 21;
			this->bnCancel->Text = L"Cancel";
			this->bnCancel->UseVisualStyleBackColor = true;
			this->bnCancel->Click += gcnew System::EventHandler(this, &AddEditShow::bnCancel_Click);
			// 
			// buttonOK
			// 
			this->buttonOK->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->buttonOK->DialogResult = System::Windows::Forms::DialogResult::OK;
			this->buttonOK->Location = System::Drawing::Point(266, 615);
			this->buttonOK->Name = L"buttonOK";
			this->buttonOK->Size = System::Drawing::Size(75, 23);
			this->buttonOK->TabIndex = 20;
			this->buttonOK->Text = L"OK";
			this->buttonOK->UseVisualStyleBackColor = true;
			this->buttonOK->Click += gcnew System::EventHandler(this, &AddEditShow::buttonOK_Click);
			// 
			// chkSpecialsCount
			// 
			this->chkSpecialsCount->AutoSize = true;
			this->chkSpecialsCount->Location = System::Drawing::Point(12, 251);
			this->chkSpecialsCount->Name = L"chkSpecialsCount";
			this->chkSpecialsCount->Size = System::Drawing::Size(155, 17);
			this->chkSpecialsCount->TabIndex = 11;
			this->chkSpecialsCount->Text = L"S&pecials count as episodes";
			this->chkSpecialsCount->UseVisualStyleBackColor = true;
			// 
			// chkShowNextAirdate
			// 
			this->chkShowNextAirdate->AutoSize = true;
			this->chkShowNextAirdate->Location = System::Drawing::Point(12, 228);
			this->chkShowNextAirdate->Name = L"chkShowNextAirdate";
			this->chkShowNextAirdate->Size = System::Drawing::Size(111, 17);
			this->chkShowNextAirdate->TabIndex = 8;
			this->chkShowNextAirdate->Text = L"Show &next airdate";
			this->chkShowNextAirdate->UseVisualStyleBackColor = true;
			// 
			// pnlCF
			// 
			this->pnlCF->Location = System::Drawing::Point(11, 7);
			this->pnlCF->Name = L"pnlCF";
			this->pnlCF->Size = System::Drawing::Size(409, 160);
			this->pnlCF->TabIndex = 0;
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Location = System::Drawing::Point(6, 20);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(63, 13);
			this->label3->TabIndex = 1;
			this->label3->Text = L"Base &Folder";
			this->label3->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// chkFolderPerSeason
			// 
			this->chkFolderPerSeason->AutoSize = true;
			this->chkFolderPerSeason->Location = System::Drawing::Point(9, 45);
			this->chkFolderPerSeason->Name = L"chkFolderPerSeason";
			this->chkFolderPerSeason->Size = System::Drawing::Size(110, 17);
			this->chkFolderPerSeason->TabIndex = 22;
			this->chkFolderPerSeason->Text = L"&Folder per season";
			this->chkFolderPerSeason->UseVisualStyleBackColor = true;
			this->chkFolderPerSeason->CheckedChanged += gcnew System::EventHandler(this, &AddEditShow::chkFolderPerSeason_CheckedChanged);
			// 
			// txtSeasonFolderName
			// 
			this->txtSeasonFolderName->Location = System::Drawing::Point(125, 43);
			this->txtSeasonFolderName->Name = L"txtSeasonFolderName";
			this->txtSeasonFolderName->Size = System::Drawing::Size(120, 20);
			this->txtSeasonFolderName->TabIndex = 23;
			// 
			// txtHash
			// 
			this->txtHash->AutoSize = true;
			this->txtHash->Enabled = false;
			this->txtHash->Location = System::Drawing::Point(251, 46);
			this->txtHash->Name = L"txtHash";
			this->txtHash->Size = System::Drawing::Size(14, 13);
			this->txtHash->TabIndex = 6;
			this->txtHash->Text = L"#";
			this->txtHash->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// txtBaseFolder
			// 
			this->txtBaseFolder->Location = System::Drawing::Point(75, 17);
			this->txtBaseFolder->Name = L"txtBaseFolder";
			this->txtBaseFolder->Size = System::Drawing::Size(170, 20);
			this->txtBaseFolder->TabIndex = 23;
			// 
			// bnBrowse
			// 
			this->bnBrowse->Location = System::Drawing::Point(252, 15);
			this->bnBrowse->Name = L"bnBrowse";
			this->bnBrowse->Size = System::Drawing::Size(75, 23);
			this->bnBrowse->TabIndex = 24;
			this->bnBrowse->Text = L"&Browse...";
			this->bnBrowse->UseVisualStyleBackColor = true;
			this->bnBrowse->Click += gcnew System::EventHandler(this, &AddEditShow::bnBrowse_Click);
			// 
			// chkAutoFolders
			// 
			this->chkAutoFolders->AutoSize = true;
			this->chkAutoFolders->Checked = true;
			this->chkAutoFolders->CheckState = System::Windows::Forms::CheckState::Checked;
			this->chkAutoFolders->Location = System::Drawing::Point(21, 340);
			this->chkAutoFolders->Name = L"chkAutoFolders";
			this->chkAutoFolders->Size = System::Drawing::Size(110, 17);
			this->chkAutoFolders->TabIndex = 22;
			this->chkAutoFolders->Text = L"Automatic Folders";
			this->chkAutoFolders->UseVisualStyleBackColor = true;
			this->chkAutoFolders->CheckedChanged += gcnew System::EventHandler(this, &AddEditShow::chkAutoFolders_CheckedChanged);
			// 
			// cbDoRenaming
			// 
			this->cbDoRenaming->AutoSize = true;
			this->cbDoRenaming->Location = System::Drawing::Point(12, 274);
			this->cbDoRenaming->Name = L"cbDoRenaming";
			this->cbDoRenaming->Size = System::Drawing::Size(86, 17);
			this->cbDoRenaming->TabIndex = 8;
			this->cbDoRenaming->Text = L"Do &renaming";
			this->cbDoRenaming->UseVisualStyleBackColor = true;
			// 
			// cbDoMissingCheck
			// 
			this->cbDoMissingCheck->AutoSize = true;
			this->cbDoMissingCheck->Location = System::Drawing::Point(107, 274);
			this->cbDoMissingCheck->Name = L"cbDoMissingCheck";
			this->cbDoMissingCheck->Size = System::Drawing::Size(110, 17);
			this->cbDoMissingCheck->TabIndex = 8;
			this->cbDoMissingCheck->Text = L"Do &missing check";
			this->cbDoMissingCheck->UseVisualStyleBackColor = true;
			this->cbDoMissingCheck->CheckedChanged += gcnew System::EventHandler(this, &AddEditShow::cbDoMissingCheck_CheckedChanged);
			// 
			// chkThumbnailsAndStuff
			// 
			this->chkThumbnailsAndStuff->AutoSize = true;
			this->chkThumbnailsAndStuff->Enabled = false;
			this->chkThumbnailsAndStuff->Location = System::Drawing::Point(256, 251);
			this->chkThumbnailsAndStuff->Name = L"chkThumbnailsAndStuff";
			this->chkThumbnailsAndStuff->Size = System::Drawing::Size(166, 17);
			this->chkThumbnailsAndStuff->TabIndex = 8;
			this->chkThumbnailsAndStuff->Text = L"Thumbnail, banners, and stuff";
			this->chkThumbnailsAndStuff->UseVisualStyleBackColor = true;
			this->chkThumbnailsAndStuff->Visible = false;
			// 
			// label5
			// 
			this->label5->AutoSize = true;
			this->label5->Location = System::Drawing::Point(11, 319);
			this->label5->Name = L"label5";
			this->label5->Size = System::Drawing::Size(84, 13);
			this->label5->TabIndex = 1;
			this->label5->Text = L"Ign&ore Seasons:";
			this->label5->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// txtIgnoreSeasons
			// 
			this->txtIgnoreSeasons->Location = System::Drawing::Point(101, 316);
			this->txtIgnoreSeasons->Name = L"txtIgnoreSeasons";
			this->txtIgnoreSeasons->Size = System::Drawing::Size(156, 20);
			this->txtIgnoreSeasons->TabIndex = 23;
			// 
			// chkDVDOrder
			// 
			this->chkDVDOrder->AutoSize = true;
			this->chkDVDOrder->Location = System::Drawing::Point(256, 228);
			this->chkDVDOrder->Name = L"chkDVDOrder";
			this->chkDVDOrder->Size = System::Drawing::Size(78, 17);
			this->chkDVDOrder->TabIndex = 25;
			this->chkDVDOrder->Text = L"&DVD Order";
			this->chkDVDOrder->UseVisualStyleBackColor = true;
			// 
			// chkForceCheckAll
			// 
			this->chkForceCheckAll->AutoSize = true;
			this->chkForceCheckAll->Location = System::Drawing::Point(223, 274);
			this->chkForceCheckAll->Name = L"chkForceCheckAll";
			this->chkForceCheckAll->Size = System::Drawing::Size(167, 17);
			this->chkForceCheckAll->TabIndex = 8;
			this->chkForceCheckAll->Text = L"M&issing check for all episodes";
			this->chkForceCheckAll->UseVisualStyleBackColor = true;
			// 
			// gbAutoFolders
			// 
			this->gbAutoFolders->Controls->Add(this->chkPadTwoDigits);
			this->gbAutoFolders->Controls->Add(this->txtBaseFolder);
			this->gbAutoFolders->Controls->Add(this->bnBrowse);
			this->gbAutoFolders->Controls->Add(this->txtSeasonFolderName);
			this->gbAutoFolders->Controls->Add(this->label3);
			this->gbAutoFolders->Controls->Add(this->chkFolderPerSeason);
			this->gbAutoFolders->Controls->Add(this->txtHash);
			this->gbAutoFolders->Location = System::Drawing::Point(12, 341);
			this->gbAutoFolders->Name = L"gbAutoFolders";
			this->gbAutoFolders->Size = System::Drawing::Size(387, 75);
			this->gbAutoFolders->TabIndex = 26;
			this->gbAutoFolders->TabStop = false;
			// 
			// chkPadTwoDigits
			// 
			this->chkPadTwoDigits->AutoSize = true;
			this->chkPadTwoDigits->Location = System::Drawing::Point(278, 45);
			this->chkPadTwoDigits->Name = L"chkPadTwoDigits";
			this->chkPadTwoDigits->Size = System::Drawing::Size(104, 17);
			this->chkPadTwoDigits->TabIndex = 25;
			this->chkPadTwoDigits->Text = L"Pad to two digits";
			this->chkPadTwoDigits->UseVisualStyleBackColor = true;
			this->chkPadTwoDigits->CheckedChanged += gcnew System::EventHandler(this, &AddEditShow::chkPadTwoDigits_CheckedChanged);
			// 
			// lvSeasonFolders
			// 
			this->lvSeasonFolders->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(2) {this->columnHeader1, 
				this->columnHeader2});
			this->lvSeasonFolders->FullRowSelect = true;
			this->lvSeasonFolders->HeaderStyle = System::Windows::Forms::ColumnHeaderStyle::Nonclickable;
			this->lvSeasonFolders->Location = System::Drawing::Point(11, 77);
			this->lvSeasonFolders->Name = L"lvSeasonFolders";
			this->lvSeasonFolders->Size = System::Drawing::Size(311, 97);
			this->lvSeasonFolders->TabIndex = 27;
			this->lvSeasonFolders->UseCompatibleStateImageBehavior = false;
			this->lvSeasonFolders->View = System::Windows::Forms::View::Details;
			// 
			// columnHeader1
			// 
			this->columnHeader1->Text = L"Season";
			this->columnHeader1->Width = 52;
			// 
			// columnHeader2
			// 
			this->columnHeader2->Text = L"Folder";
			this->columnHeader2->Width = 250;
			// 
			// txtSeasonNumber
			// 
			this->txtSeasonNumber->Location = System::Drawing::Point(61, 19);
			this->txtSeasonNumber->Name = L"txtSeasonNumber";
			this->txtSeasonNumber->Size = System::Drawing::Size(52, 20);
			this->txtSeasonNumber->TabIndex = 28;
			this->txtSeasonNumber->TextChanged += gcnew System::EventHandler(this, &AddEditShow::txtSeasonNumber_TextChanged);
			// 
			// txtFolder
			// 
			this->txtFolder->Location = System::Drawing::Point(61, 50);
			this->txtFolder->Name = L"txtFolder";
			this->txtFolder->Size = System::Drawing::Size(180, 20);
			this->txtFolder->TabIndex = 28;
			this->txtFolder->TextChanged += gcnew System::EventHandler(this, &AddEditShow::txtFolder_TextChanged);
			// 
			// bnBrowseFolder
			// 
			this->bnBrowseFolder->Location = System::Drawing::Point(247, 48);
			this->bnBrowseFolder->Name = L"bnBrowseFolder";
			this->bnBrowseFolder->Size = System::Drawing::Size(75, 23);
			this->bnBrowseFolder->TabIndex = 29;
			this->bnBrowseFolder->Text = L"B&rowse...";
			this->bnBrowseFolder->UseVisualStyleBackColor = true;
			this->bnBrowseFolder->Click += gcnew System::EventHandler(this, &AddEditShow::bnBrowseFolder_Click);
			// 
			// bnAdd
			// 
			this->bnAdd->Location = System::Drawing::Point(328, 48);
			this->bnAdd->Name = L"bnAdd";
			this->bnAdd->Size = System::Drawing::Size(75, 23);
			this->bnAdd->TabIndex = 29;
			this->bnAdd->Text = L"&Add";
			this->bnAdd->UseVisualStyleBackColor = true;
			this->bnAdd->Click += gcnew System::EventHandler(this, &AddEditShow::bnAdd_Click);
			// 
			// bnRemove
			// 
			this->bnRemove->Location = System::Drawing::Point(328, 151);
			this->bnRemove->Name = L"bnRemove";
			this->bnRemove->Size = System::Drawing::Size(75, 23);
			this->bnRemove->TabIndex = 29;
			this->bnRemove->Text = L"Remo&ve";
			this->bnRemove->UseVisualStyleBackColor = true;
			this->bnRemove->Click += gcnew System::EventHandler(this, &AddEditShow::bnRemove_Click);
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(9, 22);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(46, 13);
			this->label1->TabIndex = 30;
			this->label1->Text = L"Season:";
			// 
			// label7
			// 
			this->label7->AutoSize = true;
			this->label7->Location = System::Drawing::Point(9, 53);
			this->label7->Name = L"label7";
			this->label7->Size = System::Drawing::Size(39, 13);
			this->label7->TabIndex = 30;
			this->label7->Text = L"Folder:";
			// 
			// groupBox1
			// 
			this->groupBox1->Controls->Add(this->label7);
			this->groupBox1->Controls->Add(this->label1);
			this->groupBox1->Controls->Add(this->bnRemove);
			this->groupBox1->Controls->Add(this->bnAdd);
			this->groupBox1->Controls->Add(this->bnBrowseFolder);
			this->groupBox1->Controls->Add(this->txtFolder);
			this->groupBox1->Controls->Add(this->txtSeasonNumber);
			this->groupBox1->Controls->Add(this->lvSeasonFolders);
			this->groupBox1->Location = System::Drawing::Point(12, 422);
			this->groupBox1->Name = L"groupBox1";
			this->groupBox1->Size = System::Drawing::Size(411, 186);
			this->groupBox1->TabIndex = 31;
			this->groupBox1->TabStop = false;
			this->groupBox1->Text = L"Manual/Additional Folders";
			// 
			// cbSequentialMatching
			// 
			this->cbSequentialMatching->AutoSize = true;
			this->cbSequentialMatching->Location = System::Drawing::Point(12, 297);
			this->cbSequentialMatching->Name = L"cbSequentialMatching";
			this->cbSequentialMatching->Size = System::Drawing::Size(290, 17);
			this->cbSequentialMatching->TabIndex = 8;
			this->cbSequentialMatching->Text = L"Use sequential number matching (missing episodes only)";
			this->cbSequentialMatching->UseVisualStyleBackColor = true;
			// 
			// chkCustomShowName
			// 
			this->chkCustomShowName->AutoSize = true;
			this->chkCustomShowName->Location = System::Drawing::Point(11, 177);
			this->chkCustomShowName->Name = L"chkCustomShowName";
			this->chkCustomShowName->Size = System::Drawing::Size(121, 17);
			this->chkCustomShowName->TabIndex = 32;
			this->chkCustomShowName->Text = L"Custom s&how name:";
			this->chkCustomShowName->UseVisualStyleBackColor = true;
			this->chkCustomShowName->CheckedChanged += gcnew System::EventHandler(this, &AddEditShow::chkCustomShowName_CheckedChanged);
			// 
			// AddEditShow
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnCancel;
			this->ClientSize = System::Drawing::Size(432, 650);
			this->Controls->Add(this->chkCustomShowName);
			this->Controls->Add(this->groupBox1);
			this->Controls->Add(this->chkAutoFolders);
			this->Controls->Add(this->gbAutoFolders);
			this->Controls->Add(this->chkDVDOrder);
			this->Controls->Add(this->txtIgnoreSeasons);
			this->Controls->Add(this->pnlCF);
			this->Controls->Add(this->chkSpecialsCount);
			this->Controls->Add(this->chkThumbnailsAndStuff);
			this->Controls->Add(this->chkForceCheckAll);
			this->Controls->Add(this->cbDoMissingCheck);
			this->Controls->Add(this->cbSequentialMatching);
			this->Controls->Add(this->cbDoRenaming);
			this->Controls->Add(this->chkShowNextAirdate);
			this->Controls->Add(this->bnCancel);
			this->Controls->Add(this->buttonOK);
			this->Controls->Add(this->txtCustomShowName);
			this->Controls->Add(this->label5);
			this->Controls->Add(this->cbTimeZone);
			this->Controls->Add(this->label6);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"AddEditShow";
			this->ShowInTaskbar = false;
			this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Hide;
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = L"Add/Edit Show";
			this->gbAutoFolders->ResumeLayout(false);
			this->gbAutoFolders->PerformLayout();
			this->groupBox1->ResumeLayout(false);
			this->groupBox1->PerformLayout();
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

    private: System::Void buttonOK_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (!OKToClose())
                 {
                     this->DialogResult = ::DialogResult::None;
                     return;
                 }

                 SetmSI();
                 this->DialogResult = System::Windows::Forms::DialogResult::OK;
                 Close();
             }


             bool OKToClose()
             {
                 if (!mTVDB->HasSeries(mTCCF->SelectedCode()))
                 {
                     ::DialogResult dr = ::MessageBox::Show("tvdb code unknown, close anyway?","TVRename Add/Edit Show",MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
                     if (dr == ::DialogResult::No)
                         return false;
                 }

                 return true;
             }

             void SetmSI()
             {
                 int code = mTCCF->SelectedCode();

                 String ^tz = (cbTimeZone->SelectedItem != nullptr) ? cbTimeZone->SelectedItem->ToString() : "";

                 mSI->CustomShowName = txtCustomShowName->Text;
                 mSI->UseCustomShowName = chkCustomShowName->Checked;
                 TimeZone = tz; //TODO: move to somewhere else.  make timezone manager for tvdb?
                 mSI->ShowNextAirdate = chkShowNextAirdate->Checked;
                 mSI->PadSeasonToTwoDigits = chkPadTwoDigits->Checked;
                 mSI->TVDBCode = code;
                 //todo mSI->SeasonNumber = seasnum;
                 mSI->CountSpecials = chkSpecialsCount->Checked;
                 //                                 mSI->Rules = mWorkingRuleSet;  // TODO
                 mSI->DoRename = cbDoRenaming->Checked;
                 mSI->DoMissingCheck = cbDoMissingCheck->Checked;

                 mSI->AutoAddNewSeasons = chkAutoFolders->Checked;
                 mSI->AutoAdd_FolderPerSeason = chkFolderPerSeason->Checked;
                 mSI->AutoAdd_SeasonFolderName = txtSeasonFolderName->Text;
                 mSI->AutoAdd_FolderBase = txtBaseFolder->Text;

                 mSI->DVDOrder = chkDVDOrder->Checked;
                 mSI->ForceCheckAll = chkForceCheckAll->Checked;
                 mSI->UseSequentialMatch = cbSequentialMatching->Checked;

                 String ^slist = txtIgnoreSeasons->Text;
                 mSI->IgnoreSeasons->Clear();
                 for each (Match^ match in Regex::Matches(slist, "\\b[0-9]+\\b"))
                     mSI->IgnoreSeasons->Add(int::Parse(match->Value));

                 mSI->ManualFolderLocations->Clear();
                 for each (ListViewItem ^lvi in lvSeasonFolders->Items)
                 {
                     try
                     {
                         int seas = int::Parse(lvi->Text);
                         if (!mSI->ManualFolderLocations->ContainsKey(seas))
                             mSI->ManualFolderLocations->Add(seas, gcnew StringList());

                         mSI->ManualFolderLocations[seas]->Add(lvi->SubItems[1]->Text);
                     }
                     catch (...)
                     {
                     }
                 }
             }


    private: System::Void bnCancel_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 Close();
             }
    private: System::Void chkFolderPerSeason_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 txtSeasonFolderName->Enabled = chkFolderPerSeason->Checked;
             }
    private: System::Void bnBrowse_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (!String::IsNullOrEmpty(txtBaseFolder->Text))
                     folderBrowser->SelectedPath = txtBaseFolder->Text;

                 if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
                     txtBaseFolder->Text = folderBrowser->SelectedPath;
             }
    private: System::Void chkAutoFolders_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 gbAutoFolders->Enabled = chkAutoFolders->Checked;
             }
    private: System::Void cbDoMissingCheck_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 chkForceCheckAll->Enabled = cbDoMissingCheck->Checked;
             }
    private: System::Void bnRemove_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (lvSeasonFolders->SelectedItems->Count)
                     for each (ListViewItem ^lvi in lvSeasonFolders->SelectedItems)
                         lvSeasonFolders->Items->Remove(lvi);
             }
    private: System::Void bnAdd_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 ListViewItem ^lvi = gcnew ListViewItem();
                 lvi->Text = txtSeasonNumber->Text;
                 lvi->SubItems->Add(txtFolder->Text);

                 lvSeasonFolders->Items->Add(lvi);

                 txtSeasonNumber->Text = "";
                 txtFolder->Text = "";

                 lvSeasonFolders->Sort();
             }
    private: System::Void bnBrowseFolder_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 folderBrowser->SelectedPath = txtFolder->Text;
                 if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
                     txtFolder->Text = folderBrowser->SelectedPath;
             }
    private: System::Void txtSeasonNumber_TextChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 bool isNumber = Regex::Match(txtSeasonNumber->Text,"^[0-9]+$")->Success;
                 bnAdd->Enabled = isNumber && (!String::IsNullOrEmpty(txtSeasonNumber->Text));
             }
    private: System::Void txtFolder_TextChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 bool ok = true;
                 if (!String::IsNullOrEmpty(txtFolder->Text))
                 {
                     try
                     {
                         ok = DirectoryInfo(txtFolder->Text).Exists;
                     }
                     catch (...)
                     {
                     }
                 }
                 if (ok)
                     txtFolder->BackColor = System::Drawing::SystemColors::Window;
                 else
                     txtFolder->BackColor = WarningColor();

             }
    private: System::Void chkCustomShowName_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 txtCustomShowName->Enabled = chkCustomShowName->Checked;
             }
private: System::Void chkPadTwoDigits_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
         {
             SetRightNumberOfHashes();
         }

         void SetRightNumberOfHashes()
         {
                 txtHash->Text = chkPadTwoDigits->Checked ? "##" : "#"; 
         }
};
}


