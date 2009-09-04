#pragma once

#include "TVDoc.h"
#include "Settings.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

    /// <summary>
    /// Summary for Preferences
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public ref class Preferences : public System::Windows::Forms::Form
    {
    private:
        TVDoc ^mDoc;

    public:
        Preferences(TVDoc ^doc)
        {
            InitializeComponent();

            mDoc = doc;
        }

    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~Preferences()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::Button^  OKButton;
    private: System::Windows::Forms::Button^  bnCancel;
    protected: 



    private: System::Windows::Forms::GroupBox^  groupBox1;

    private: System::Windows::Forms::Label^  label5;
    private: System::Windows::Forms::Label^  label4;
    private: System::Windows::Forms::Label^  label3;
    private: System::Windows::Forms::Label^  label10;
    private: System::Windows::Forms::Label^  label9;
    private: System::Windows::Forms::Label^  label8;
    private: System::Windows::Forms::Label^  label7;
    private: System::Windows::Forms::Label^  label11;
    private: System::Windows::Forms::TextBox^  txtR3;


    private: System::Windows::Forms::TextBox^  txtR2;

    private: System::Windows::Forms::TextBox^  txtR1;

    private: System::Windows::Forms::Label^  label12;
    private: System::Windows::Forms::TextBox^  txtR9;

    private: System::Windows::Forms::TextBox^  txtR6;
    private: System::Windows::Forms::TextBox^  txtR8;


    private: System::Windows::Forms::TextBox^  txtR5;
    private: System::Windows::Forms::TextBox^  txtR7;


    private: System::Windows::Forms::TextBox^  txtR4;

    private: System::Windows::Forms::GroupBox^  groupBox2;
    private: System::Windows::Forms::Button^  bnBrowseRSSExportWTW;



    private: System::Windows::Forms::TextBox^  txtExportTo;



    private: System::Windows::Forms::CheckBox^  chkExportWTW;
    private: System::Windows::Forms::SaveFileDialog^  saveFile;
    private: System::Windows::Forms::GroupBox^  groupBox3;
    private: System::Windows::Forms::TextBox^  txtWTWDays;
    private: System::Windows::Forms::Label^  label2;
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::ComboBox^  cbStartupTab;
    private: System::Windows::Forms::Label^  label6;



    private: System::Windows::Forms::CheckBox^  cbNotificationIcon;
    private: System::Windows::Forms::TextBox^  txtExtensions;
    private: System::Windows::Forms::Label^  label14;
    private: System::Windows::Forms::Label^  label16;
    private: System::Windows::Forms::Label^  label15;
    private: System::Windows::Forms::TextBox^  txtExportRSSMaxDays;

    private: System::Windows::Forms::TextBox^  txtExportRSSMaxShows;
    private: System::Windows::Forms::Label^  label17;
    private: System::Windows::Forms::CheckBox^  cbKeepTogether;
    private: System::Windows::Forms::CheckBox^  cbLeadingZero;
    private: System::Windows::Forms::CheckBox^  chkShowInTaskbar;
    private: System::Windows::Forms::CheckBox^  cbTxtToSub;
    private: System::Windows::Forms::CheckBox^  cbShowEpisodePictures;
    private: System::Windows::Forms::TextBox^  txtSpecialsFolderName;
    private: System::Windows::Forms::Label^  label13;








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
            System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(Preferences::typeid));
            this->OKButton = (gcnew System::Windows::Forms::Button());
            this->bnCancel = (gcnew System::Windows::Forms::Button());
            this->groupBox1 = (gcnew System::Windows::Forms::GroupBox());
            this->txtR9 = (gcnew System::Windows::Forms::TextBox());
            this->txtR6 = (gcnew System::Windows::Forms::TextBox());
            this->txtR3 = (gcnew System::Windows::Forms::TextBox());
            this->txtR8 = (gcnew System::Windows::Forms::TextBox());
            this->txtR5 = (gcnew System::Windows::Forms::TextBox());
            this->txtR2 = (gcnew System::Windows::Forms::TextBox());
            this->txtR7 = (gcnew System::Windows::Forms::TextBox());
            this->txtR4 = (gcnew System::Windows::Forms::TextBox());
            this->txtR1 = (gcnew System::Windows::Forms::TextBox());
            this->label12 = (gcnew System::Windows::Forms::Label());
            this->label11 = (gcnew System::Windows::Forms::Label());
            this->label10 = (gcnew System::Windows::Forms::Label());
            this->label9 = (gcnew System::Windows::Forms::Label());
            this->label8 = (gcnew System::Windows::Forms::Label());
            this->label7 = (gcnew System::Windows::Forms::Label());
            this->label5 = (gcnew System::Windows::Forms::Label());
            this->label4 = (gcnew System::Windows::Forms::Label());
            this->label3 = (gcnew System::Windows::Forms::Label());
            this->groupBox2 = (gcnew System::Windows::Forms::GroupBox());
            this->bnBrowseRSSExportWTW = (gcnew System::Windows::Forms::Button());
            this->txtExportTo = (gcnew System::Windows::Forms::TextBox());
            this->chkExportWTW = (gcnew System::Windows::Forms::CheckBox());
            this->label17 = (gcnew System::Windows::Forms::Label());
            this->label16 = (gcnew System::Windows::Forms::Label());
            this->label15 = (gcnew System::Windows::Forms::Label());
            this->txtExportRSSMaxDays = (gcnew System::Windows::Forms::TextBox());
            this->txtExportRSSMaxShows = (gcnew System::Windows::Forms::TextBox());
            this->saveFile = (gcnew System::Windows::Forms::SaveFileDialog());
            this->groupBox3 = (gcnew System::Windows::Forms::GroupBox());
            this->cbTxtToSub = (gcnew System::Windows::Forms::CheckBox());
            this->txtExtensions = (gcnew System::Windows::Forms::TextBox());
            this->cbStartupTab = (gcnew System::Windows::Forms::ComboBox());
            this->cbShowEpisodePictures = (gcnew System::Windows::Forms::CheckBox());
            this->cbLeadingZero = (gcnew System::Windows::Forms::CheckBox());
            this->cbKeepTogether = (gcnew System::Windows::Forms::CheckBox());
            this->chkShowInTaskbar = (gcnew System::Windows::Forms::CheckBox());
            this->cbNotificationIcon = (gcnew System::Windows::Forms::CheckBox());
            this->txtWTWDays = (gcnew System::Windows::Forms::TextBox());
            this->label2 = (gcnew System::Windows::Forms::Label());
            this->label14 = (gcnew System::Windows::Forms::Label());
            this->label6 = (gcnew System::Windows::Forms::Label());
            this->label1 = (gcnew System::Windows::Forms::Label());
            this->label13 = (gcnew System::Windows::Forms::Label());
            this->txtSpecialsFolderName = (gcnew System::Windows::Forms::TextBox());
            this->groupBox1->SuspendLayout();
            this->groupBox2->SuspendLayout();
            this->groupBox3->SuspendLayout();
            this->SuspendLayout();
            // 
            // OKButton
            // 
            this->OKButton->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
            this->OKButton->Location = System::Drawing::Point(259, 415);
            this->OKButton->Name = L"OKButton";
            this->OKButton->Size = System::Drawing::Size(75, 23);
            this->OKButton->TabIndex = 3;
            this->OKButton->Text = L"OK";
            this->OKButton->UseVisualStyleBackColor = true;
            this->OKButton->Click += gcnew System::EventHandler(this, &Preferences::OKButton_Click);
            // 
            // bnCancel
            // 
            this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
            this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
            this->bnCancel->Location = System::Drawing::Point(340, 415);
            this->bnCancel->Name = L"bnCancel";
            this->bnCancel->Size = System::Drawing::Size(75, 23);
            this->bnCancel->TabIndex = 4;
            this->bnCancel->Text = L"Cancel";
            this->bnCancel->UseVisualStyleBackColor = true;
            this->bnCancel->Click += gcnew System::EventHandler(this, &Preferences::CancelButton_Click);
            // 
            // groupBox1
            // 
            this->groupBox1->Controls->Add(this->txtR9);
            this->groupBox1->Controls->Add(this->txtR6);
            this->groupBox1->Controls->Add(this->txtR3);
            this->groupBox1->Controls->Add(this->txtR8);
            this->groupBox1->Controls->Add(this->txtR5);
            this->groupBox1->Controls->Add(this->txtR2);
            this->groupBox1->Controls->Add(this->txtR7);
            this->groupBox1->Controls->Add(this->txtR4);
            this->groupBox1->Controls->Add(this->txtR1);
            this->groupBox1->Controls->Add(this->label12);
            this->groupBox1->Controls->Add(this->label11);
            this->groupBox1->Controls->Add(this->label10);
            this->groupBox1->Controls->Add(this->label9);
            this->groupBox1->Controls->Add(this->label8);
            this->groupBox1->Controls->Add(this->label7);
            this->groupBox1->Controls->Add(this->label5);
            this->groupBox1->Controls->Add(this->label4);
            this->groupBox1->Controls->Add(this->label3);
            this->groupBox1->Location = System::Drawing::Point(12, 12);
            this->groupBox1->Name = L"groupBox1";
            this->groupBox1->Size = System::Drawing::Size(404, 82);
            this->groupBox1->TabIndex = 0;
            this->groupBox1->TabStop = false;
            this->groupBox1->Text = L"Filename Character Replacements";
            // 
            // txtR9
            // 
            this->txtR9->Location = System::Drawing::Point(278, 50);
            this->txtR9->Name = L"txtR9";
            this->txtR9->Size = System::Drawing::Size(28, 20);
            this->txtR9->TabIndex = 19;
            // 
            // txtR6
            // 
            this->txtR6->Location = System::Drawing::Point(197, 50);
            this->txtR6->Name = L"txtR6";
            this->txtR6->Size = System::Drawing::Size(28, 20);
            this->txtR6->TabIndex = 17;
            // 
            // txtR3
            // 
            this->txtR3->Location = System::Drawing::Point(117, 50);
            this->txtR3->Name = L"txtR3";
            this->txtR3->Size = System::Drawing::Size(28, 20);
            this->txtR3->TabIndex = 15;
            // 
            // txtR8
            // 
            this->txtR8->Location = System::Drawing::Point(37, 50);
            this->txtR8->Name = L"txtR8";
            this->txtR8->Size = System::Drawing::Size(28, 20);
            this->txtR8->TabIndex = 13;
            // 
            // txtR5
            // 
            this->txtR5->Location = System::Drawing::Point(358, 24);
            this->txtR5->Name = L"txtR5";
            this->txtR5->Size = System::Drawing::Size(28, 20);
            this->txtR5->TabIndex = 11;
            // 
            // txtR2
            // 
            this->txtR2->Location = System::Drawing::Point(278, 24);
            this->txtR2->Name = L"txtR2";
            this->txtR2->Size = System::Drawing::Size(28, 20);
            this->txtR2->TabIndex = 9;
            // 
            // txtR7
            // 
            this->txtR7->Location = System::Drawing::Point(198, 24);
            this->txtR7->Name = L"txtR7";
            this->txtR7->Size = System::Drawing::Size(28, 20);
            this->txtR7->TabIndex = 7;
            // 
            // txtR4
            // 
            this->txtR4->Location = System::Drawing::Point(117, 24);
            this->txtR4->Name = L"txtR4";
            this->txtR4->Size = System::Drawing::Size(28, 20);
            this->txtR4->TabIndex = 5;
            // 
            // txtR1
            // 
            this->txtR1->Location = System::Drawing::Point(37, 24);
            this->txtR1->Name = L"txtR1";
            this->txtR1->Size = System::Drawing::Size(28, 20);
            this->txtR1->TabIndex = 3;
            // 
            // label12
            // 
            this->label12->AutoSize = true;
            this->label12->Location = System::Drawing::Point(179, 53);
            this->label12->Name = L"label12";
            this->label12->Size = System::Drawing::Size(12, 13);
            this->label12->TabIndex = 16;
            this->label12->Text = L"\"";
            // 
            // label11
            // 
            this->label11->AutoSize = true;
            this->label11->Location = System::Drawing::Point(183, 27);
            this->label11->Name = L"label11";
            this->label11->Size = System::Drawing::Size(9, 13);
            this->label11->TabIndex = 6;
            this->label11->Text = L"|";
            // 
            // label10
            // 
            this->label10->AutoSize = true;
            this->label10->Location = System::Drawing::Point(22, 53);
            this->label10->Name = L"label10";
            this->label10->Size = System::Drawing::Size(12, 13);
            this->label10->TabIndex = 12;
            this->label10->Text = L"\\";
            // 
            // label9
            // 
            this->label9->AutoSize = true;
            this->label9->Location = System::Drawing::Point(263, 53);
            this->label9->Name = L"label9";
            this->label9->Size = System::Drawing::Size(12, 13);
            this->label9->TabIndex = 18;
            this->label9->Text = L"/";
            // 
            // label8
            // 
            this->label8->AutoSize = true;
            this->label8->Location = System::Drawing::Point(339, 27);
            this->label8->Name = L"label8";
            this->label8->Size = System::Drawing::Size(13, 13);
            this->label8->TabIndex = 10;
            this->label8->Text = L">";
            // 
            // label7
            // 
            this->label7->AutoSize = true;
            this->label7->Location = System::Drawing::Point(98, 27);
            this->label7->Name = L"label7";
            this->label7->Size = System::Drawing::Size(13, 13);
            this->label7->TabIndex = 4;
            this->label7->Text = L"<";
            // 
            // label5
            // 
            this->label5->AutoSize = true;
            this->label5->Location = System::Drawing::Point(101, 53);
            this->label5->Name = L"label5";
            this->label5->Size = System::Drawing::Size(11, 13);
            this->label5->TabIndex = 14;
            this->label5->Text = L"*";
            // 
            // label4
            // 
            this->label4->AutoSize = true;
            this->label4->Location = System::Drawing::Point(262, 25);
            this->label4->Name = L"label4";
            this->label4->Size = System::Drawing::Size(10, 13);
            this->label4->TabIndex = 8;
            this->label4->Text = L":";
            // 
            // label3
            // 
            this->label3->AutoSize = true;
            this->label3->Location = System::Drawing::Point(18, 27);
            this->label3->Name = L"label3";
            this->label3->Size = System::Drawing::Size(13, 13);
            this->label3->TabIndex = 2;
            this->label3->Text = L"\?";
            // 
            // groupBox2
            // 
            this->groupBox2->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
            this->groupBox2->Controls->Add(this->bnBrowseRSSExportWTW);
            this->groupBox2->Controls->Add(this->txtExportTo);
            this->groupBox2->Controls->Add(this->chkExportWTW);
            this->groupBox2->Controls->Add(this->label17);
            this->groupBox2->Controls->Add(this->label16);
            this->groupBox2->Controls->Add(this->label15);
            this->groupBox2->Controls->Add(this->txtExportRSSMaxDays);
            this->groupBox2->Controls->Add(this->txtExportRSSMaxShows);
            this->groupBox2->Location = System::Drawing::Point(13, 322);
            this->groupBox2->Name = L"groupBox2";
            this->groupBox2->Size = System::Drawing::Size(403, 84);
            this->groupBox2->TabIndex = 2;
            this->groupBox2->TabStop = false;
            this->groupBox2->Text = L"RSS Export";
            // 
            // bnBrowseRSSExportWTW
            // 
            this->bnBrowseRSSExportWTW->Location = System::Drawing::Point(321, 18);
            this->bnBrowseRSSExportWTW->Name = L"bnBrowseRSSExportWTW";
            this->bnBrowseRSSExportWTW->Size = System::Drawing::Size(75, 23);
            this->bnBrowseRSSExportWTW->TabIndex = 2;
            this->bnBrowseRSSExportWTW->Text = L"Browse...";
            this->bnBrowseRSSExportWTW->UseVisualStyleBackColor = true;
            this->bnBrowseRSSExportWTW->Click += gcnew System::EventHandler(this, &Preferences::bnBrowseRSSExportWTW_Click);
            // 
            // txtExportTo
            // 
            this->txtExportTo->Location = System::Drawing::Point(114, 20);
            this->txtExportTo->Name = L"txtExportTo";
            this->txtExportTo->Size = System::Drawing::Size(201, 20);
            this->txtExportTo->TabIndex = 1;
            // 
            // chkExportWTW
            // 
            this->chkExportWTW->AutoSize = true;
            this->chkExportWTW->Location = System::Drawing::Point(10, 22);
            this->chkExportWTW->Name = L"chkExportWTW";
            this->chkExportWTW->Size = System::Drawing::Size(102, 17);
            this->chkExportWTW->TabIndex = 0;
            this->chkExportWTW->Text = L"When to Watch";
            this->chkExportWTW->UseVisualStyleBackColor = true;
            this->chkExportWTW->CheckedChanged += gcnew System::EventHandler(this, &Preferences::chkExportWTW_CheckedChanged);
            // 
            // label17
            // 
            this->label17->AutoSize = true;
            this->label17->Location = System::Drawing::Point(212, 53);
            this->label17->Name = L"label17";
            this->label17->Size = System::Drawing::Size(61, 13);
            this->label17->TabIndex = 7;
            this->label17->Text = L"days worth.";
            // 
            // label16
            // 
            this->label16->AutoSize = true;
            this->label16->Location = System::Drawing::Point(120, 53);
            this->label16->Name = L"label16";
            this->label16->Size = System::Drawing::Size(52, 13);
            this->label16->TabIndex = 5;
            this->label16->Text = L"shows, or";
            // 
            // label15
            // 
            this->label15->AutoSize = true;
            this->label15->Location = System::Drawing::Point(9, 53);
            this->label15->Name = L"label15";
            this->label15->Size = System::Drawing::Size(71, 13);
            this->label15->TabIndex = 3;
            this->label15->Text = L"No more than";
            // 
            // txtExportRSSMaxDays
            // 
            this->txtExportRSSMaxDays->Location = System::Drawing::Point(178, 50);
            this->txtExportRSSMaxDays->Name = L"txtExportRSSMaxDays";
            this->txtExportRSSMaxDays->Size = System::Drawing::Size(28, 20);
            this->txtExportRSSMaxDays->TabIndex = 6;
            this->txtExportRSSMaxDays->KeyPress += gcnew System::Windows::Forms::KeyPressEventHandler(this, &Preferences::txtNumberOnlyKeyPress);
            // 
            // txtExportRSSMaxShows
            // 
            this->txtExportRSSMaxShows->Location = System::Drawing::Point(86, 50);
            this->txtExportRSSMaxShows->Name = L"txtExportRSSMaxShows";
            this->txtExportRSSMaxShows->Size = System::Drawing::Size(28, 20);
            this->txtExportRSSMaxShows->TabIndex = 4;
            this->txtExportRSSMaxShows->KeyPress += gcnew System::Windows::Forms::KeyPressEventHandler(this, &Preferences::txtNumberOnlyKeyPress);
            // 
            // saveFile
            // 
            this->saveFile->DefaultExt = L"rss";
            this->saveFile->Filter = L"RSS files (*.rss)|*.rss|XML files (*.xml)|*.xml|All files (*.*)|*.*";
            // 
            // groupBox3
            // 
            this->groupBox3->Controls->Add(this->cbTxtToSub);
            this->groupBox3->Controls->Add(this->txtSpecialsFolderName);
            this->groupBox3->Controls->Add(this->txtExtensions);
            this->groupBox3->Controls->Add(this->cbStartupTab);
            this->groupBox3->Controls->Add(this->cbShowEpisodePictures);
            this->groupBox3->Controls->Add(this->cbLeadingZero);
            this->groupBox3->Controls->Add(this->cbKeepTogether);
            this->groupBox3->Controls->Add(this->chkShowInTaskbar);
            this->groupBox3->Controls->Add(this->cbNotificationIcon);
            this->groupBox3->Controls->Add(this->txtWTWDays);
            this->groupBox3->Controls->Add(this->label2);
            this->groupBox3->Controls->Add(this->label13);
            this->groupBox3->Controls->Add(this->label14);
            this->groupBox3->Controls->Add(this->label6);
            this->groupBox3->Controls->Add(this->label1);
            this->groupBox3->Location = System::Drawing::Point(13, 100);
            this->groupBox3->Name = L"groupBox3";
            this->groupBox3->Size = System::Drawing::Size(403, 218);
            this->groupBox3->TabIndex = 1;
            this->groupBox3->TabStop = false;
            this->groupBox3->Text = L"Miscellaneous";
            // 
            // cbTxtToSub
            // 
            this->cbTxtToSub->AutoSize = true;
            this->cbTxtToSub->Location = System::Drawing::Point(264, 121);
            this->cbTxtToSub->Name = L"cbTxtToSub";
            this->cbTxtToSub->Size = System::Drawing::Size(118, 17);
            this->cbTxtToSub->TabIndex = 10;
            this->cbTxtToSub->Text = L"Rename .txt to .sub";
            this->cbTxtToSub->UseVisualStyleBackColor = true;
            // 
            // txtExtensions
            // 
            this->txtExtensions->Location = System::Drawing::Point(95, 95);
            this->txtExtensions->Name = L"txtExtensions";
            this->txtExtensions->Size = System::Drawing::Size(300, 20);
            this->txtExtensions->TabIndex = 8;
            // 
            // cbStartupTab
            // 
            this->cbStartupTab->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
            this->cbStartupTab->FormattingEnabled = true;
            this->cbStartupTab->Items->AddRange(gcnew cli::array< System::Object^  >(7) {L"My Shows", L"Rename", L"Missing", L"Finding and Organising", 
                L"Folder Monitor", L"Torrent Match", L"When to Watch"});
            this->cbStartupTab->Location = System::Drawing::Point(103, 47);
            this->cbStartupTab->Name = L"cbStartupTab";
            this->cbStartupTab->Size = System::Drawing::Size(135, 21);
            this->cbStartupTab->TabIndex = 4;
            // 
            // cbShowEpisodePictures
            // 
            this->cbShowEpisodePictures->AutoSize = true;
            this->cbShowEpisodePictures->Location = System::Drawing::Point(9, 167);
            this->cbShowEpisodePictures->Name = L"cbShowEpisodePictures";
            this->cbShowEpisodePictures->Size = System::Drawing::Size(218, 17);
            this->cbShowEpisodePictures->TabIndex = 12;
            this->cbShowEpisodePictures->Text = L"Show episode pictures in episode guides";
            this->cbShowEpisodePictures->UseVisualStyleBackColor = true;
            // 
            // cbLeadingZero
            // 
            this->cbLeadingZero->AutoSize = true;
            this->cbLeadingZero->Location = System::Drawing::Point(9, 144);
            this->cbLeadingZero->Name = L"cbLeadingZero";
            this->cbLeadingZero->Size = System::Drawing::Size(170, 17);
            this->cbLeadingZero->TabIndex = 11;
            this->cbLeadingZero->Text = L"Leading 0 on Season numbers";
            this->cbLeadingZero->UseVisualStyleBackColor = true;
            // 
            // cbKeepTogether
            // 
            this->cbKeepTogether->AutoSize = true;
            this->cbKeepTogether->Location = System::Drawing::Point(9, 121);
            this->cbKeepTogether->Name = L"cbKeepTogether";
            this->cbKeepTogether->Size = System::Drawing::Size(239, 17);
            this->cbKeepTogether->TabIndex = 9;
            this->cbKeepTogether->Text = L"Copy/Move files with same base name as avi";
            this->cbKeepTogether->UseVisualStyleBackColor = true;
            this->cbKeepTogether->CheckedChanged += gcnew System::EventHandler(this, &Preferences::cbKeepTogether_CheckedChanged);
            // 
            // chkShowInTaskbar
            // 
            this->chkShowInTaskbar->AutoSize = true;
            this->chkShowInTaskbar->Location = System::Drawing::Point(169, 74);
            this->chkShowInTaskbar->Name = L"chkShowInTaskbar";
            this->chkShowInTaskbar->Size = System::Drawing::Size(102, 17);
            this->chkShowInTaskbar->TabIndex = 6;
            this->chkShowInTaskbar->Text = L"Show in taskbar";
            this->chkShowInTaskbar->UseVisualStyleBackColor = true;
            this->chkShowInTaskbar->CheckedChanged += gcnew System::EventHandler(this, &Preferences::chkShowInTaskbar_CheckedChanged);
            // 
            // cbNotificationIcon
            // 
            this->cbNotificationIcon->AutoSize = true;
            this->cbNotificationIcon->Location = System::Drawing::Point(9, 74);
            this->cbNotificationIcon->Name = L"cbNotificationIcon";
            this->cbNotificationIcon->Size = System::Drawing::Size(154, 17);
            this->cbNotificationIcon->TabIndex = 5;
            this->cbNotificationIcon->Text = L"Show notification area icon";
            this->cbNotificationIcon->UseVisualStyleBackColor = true;
            this->cbNotificationIcon->CheckedChanged += gcnew System::EventHandler(this, &Preferences::cbNotificationIcon_CheckedChanged);
            // 
            // txtWTWDays
            // 
            this->txtWTWDays->Location = System::Drawing::Point(92, 21);
            this->txtWTWDays->Name = L"txtWTWDays";
            this->txtWTWDays->Size = System::Drawing::Size(28, 20);
            this->txtWTWDays->TabIndex = 1;
            this->txtWTWDays->KeyPress += gcnew System::Windows::Forms::KeyPressEventHandler(this, &Preferences::txtNumberOnlyKeyPress);
            // 
            // label2
            // 
            this->label2->AutoSize = true;
            this->label2->Location = System::Drawing::Point(126, 24);
            this->label2->Name = L"label2";
            this->label2->Size = System::Drawing::Size(111, 13);
            this->label2->TabIndex = 2;
            this->label2->Text = L"days counts as recent";
            // 
            // label14
            // 
            this->label14->AutoSize = true;
            this->label14->Location = System::Drawing::Point(6, 98);
            this->label14->Name = L"label14";
            this->label14->Size = System::Drawing::Size(83, 13);
            this->label14->TabIndex = 7;
            this->label14->Text = L"Find extensions:";
            // 
            // label6
            // 
            this->label6->AutoSize = true;
            this->label6->Location = System::Drawing::Point(6, 50);
            this->label6->Name = L"label6";
            this->label6->Size = System::Drawing::Size(62, 13);
            this->label6->TabIndex = 3;
            this->label6->Text = L"Startup tab:";
            // 
            // label1
            // 
            this->label1->AutoSize = true;
            this->label1->Location = System::Drawing::Point(6, 24);
            this->label1->Name = L"label1";
            this->label1->Size = System::Drawing::Size(80, 13);
            this->label1->TabIndex = 0;
            this->label1->Text = L"When to watch";
            // 
            // label13
            // 
            this->label13->AutoSize = true;
            this->label13->Location = System::Drawing::Point(9, 191);
            this->label13->Name = L"label13";
            this->label13->Size = System::Drawing::Size(108, 13);
            this->label13->TabIndex = 13;
            this->label13->Text = L"Specials folder name:";
            // 
            // txtSpecialsFolderName
            // 
            this->txtSpecialsFolderName->Location = System::Drawing::Point(116, 188);
            this->txtSpecialsFolderName->Name = L"txtSpecialsFolderName";
            this->txtSpecialsFolderName->Size = System::Drawing::Size(279, 20);
            this->txtSpecialsFolderName->TabIndex = 14;
            // 
            // Preferences
            // 
            this->AcceptButton = this->OKButton;
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->CancelButton = this->bnCancel;
            this->ClientSize = System::Drawing::Size(427, 450);
            this->Controls->Add(this->groupBox3);
            this->Controls->Add(this->groupBox2);
            this->Controls->Add(this->groupBox1);
            this->Controls->Add(this->bnCancel);
            this->Controls->Add(this->OKButton);
            this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
            this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
            this->Name = L"Preferences";
            this->ShowInTaskbar = false;
            this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
            this->Text = L"Preferences";
            this->Load += gcnew System::EventHandler(this, &Preferences::Preferences_Load);
            this->groupBox1->ResumeLayout(false);
            this->groupBox1->PerformLayout();
            this->groupBox2->ResumeLayout(false);
            this->groupBox2->PerformLayout();
            this->groupBox3->ResumeLayout(false);
            this->groupBox3->PerformLayout();
            this->ResumeLayout(false);

        }
#pragma endregion
    private: System::Void OKButton_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (!TVSettings::OKExtensionsString(txtExtensions->Text))
                 {
                     ::DialogResult res = MessageBox::Show("Extensions list must be separated by semicolons, and each extension must start with a dot.  Ignore changes to this setting?","Preferences",MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
                     if (res == ::DialogResult::No)
                         return;
                 }
                 TVSettings ^S = mDoc->Settings;
                 S->Replacements->Clear();
                 S->Replacements->Add(gcnew Replacement("?",txtR1->Text));
                 S->Replacements->Add(gcnew Replacement(":",txtR2->Text));
                 S->Replacements->Add(gcnew Replacement("*",txtR3->Text));
                 S->Replacements->Add(gcnew Replacement("<",txtR4->Text));
                 S->Replacements->Add(gcnew Replacement(">",txtR5->Text));
                 S->Replacements->Add(gcnew Replacement("\"",txtR6->Text));
                 S->Replacements->Add(gcnew Replacement("|",txtR7->Text));
                 S->Replacements->Add(gcnew Replacement("\\",txtR8->Text));
                 S->Replacements->Add(gcnew Replacement("/",txtR9->Text));

                 S->ExportWTWRSS = chkExportWTW->Checked;
                 S->ExportWTWRSSTo = txtExportTo->Text;

                 S->WTWRecentDays = Convert::ToInt32(txtWTWDays->Text);
                 S->StartupTab = cbStartupTab->SelectedIndex;
                 S->NotificationAreaIcon = cbNotificationIcon->Checked;
                 S->SetGoodExtensionsString(txtExtensions->Text);
                 S->ExportRSSMaxDays = Convert::ToInt32(txtExportRSSMaxDays->Text);
                 S->ExportRSSMaxShows =  Convert::ToInt32(txtExportRSSMaxShows->Text);
                 S->KeepTogether = cbKeepTogether->Checked;
                 S->LeadingZeroOnSeason = cbLeadingZero->Checked;
                 S->ShowInTaskbar = chkShowInTaskbar->Checked;
                 S->RenameTxtToSub = cbTxtToSub->Checked;
                 S->ShowEpisodePictures = cbShowEpisodePictures->Checked;
                 S->SpecialsFolderName = txtSpecialsFolderName->Text;

                 mDoc->SetDirty();
                 this->DialogResult = ::DialogResult::OK;
                 this->Close();
             }
    private: System::Void Preferences_Load(System::Object^  sender, System::EventArgs^  e) 
             {
                 TVSettings ^S = mDoc->Settings;
                 for each (Replacement ^R in S->Replacements)
                 {
                     if (R->This == "?") txtR1->Text = R->That;
                     else if (R->This == ":") txtR2->Text = R->That;
                     else if (R->This == "*") txtR3->Text = R->That;
                     else if (R->This == "<") txtR4->Text = R->That;
                     else if (R->This == ">") txtR5->Text = R->That;
                     else if (R->This == "\"") txtR6->Text = R->That;
                     else if (R->This == "|") txtR7->Text = R->That;
                     else if (R->This == "\\") txtR8->Text = R->That;
                     else if (R->This == "/") txtR9->Text = R->That;
                 }

                 chkExportWTW->Checked = S->ExportWTWRSS;
                 txtExportTo->Text = S->ExportWTWRSSTo;
                 txtExportTo->Enabled = chkExportWTW->Checked;
                 txtWTWDays->Text = S->WTWRecentDays.ToString();
                 cbStartupTab->SelectedIndex = S->StartupTab;
                 cbNotificationIcon->Checked = S->NotificationAreaIcon;
                 txtExtensions->Text = S->GetGoodExtensionsString();
                 txtExportRSSMaxDays->Text = S->ExportRSSMaxDays.ToString();
                 txtExportRSSMaxShows->Text = S->ExportRSSMaxShows.ToString();
                 
                 cbKeepTogether->Checked = S->KeepTogether;
                 cbKeepTogether_CheckedChanged(nullptr,nullptr);

                 cbLeadingZero->Checked = S->LeadingZeroOnSeason;
                 chkShowInTaskbar->Checked = S->ShowInTaskbar;
                 cbTxtToSub->Checked = S->RenameTxtToSub;
                 cbShowEpisodePictures->Checked = S->ShowEpisodePictures;
                 txtSpecialsFolderName->Text = S->SpecialsFolderName;
             }
    private: System::Void bnBrowseRSSExportWTW_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 saveFile->FileName = txtExportTo->Text;
                 if (saveFile->ShowDialog() == System::Windows::Forms::DialogResult::OK)
                     txtExportTo->Text = saveFile->FileName;
             }
    private: System::Void chkExportWTW_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 txtExportTo->Enabled = chkExportWTW->Checked;
             }
    private: System::Void txtNumberOnlyKeyPress(System::Object^  sender, System::Windows::Forms::KeyPressEventArgs^  e) 
             {
                 // digits only
                 if ((e->KeyChar >= 32) && (!Char::IsDigit(e->KeyChar)) )
                     e->Handled = true;
             }
    private: System::Void CancelButton_Click(System::Object^  sender, System::EventArgs^  e) 
             {
                 this->Close();
             }
    private: System::Void cbNotificationIcon_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (!cbNotificationIcon->Checked)
                     chkShowInTaskbar->Checked = true;
             }
private: System::Void chkShowInTaskbar_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
         {                 
             if (!chkShowInTaskbar->Checked)
                     cbNotificationIcon->Checked = true;
         }
private: System::Void cbKeepTogether_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
         {
             cbTxtToSub->Enabled = cbKeepTogether->Checked;
         }
};
}
