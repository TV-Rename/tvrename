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
	private: System::Windows::Forms::Button^  bnBrowseWTWRSS;
	private: System::Windows::Forms::TextBox^  txtWTWRSS;







	private: System::Windows::Forms::CheckBox^  cbWTWRSS;
	private: System::Windows::Forms::SaveFileDialog^  saveFile;

	private: System::Windows::Forms::TextBox^  txtWTWDays;
	private: System::Windows::Forms::Label^  label2;
	private: System::Windows::Forms::Label^  label1;
	private: System::Windows::Forms::ComboBox^  cbStartupTab;
	private: System::Windows::Forms::Label^  label6;



	private: System::Windows::Forms::CheckBox^  cbNotificationIcon;
	private: System::Windows::Forms::TextBox^  txtVideoExtensions;
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
	private: System::Windows::Forms::TabControl^  tabControl1;
	private: System::Windows::Forms::TabPage^  tabPage1;
	private: System::Windows::Forms::TabPage^  tabPage2;
	private: System::Windows::Forms::TabPage^  tabPage3;
	private: System::Windows::Forms::GroupBox^  groupBox3;
	private: System::Windows::Forms::GroupBox^  groupBox5;
	private: System::Windows::Forms::GroupBox^  groupBox4;
	private: System::Windows::Forms::Button^  bnBrowseMissingCSV;
	private: System::Windows::Forms::CheckBox^  cbMissingCSV;
	private: System::Windows::Forms::TextBox^  txtMissingCSV;
	private: System::Windows::Forms::Button^  bnBrowseMissingXML;
	private: System::Windows::Forms::CheckBox^  cbMissingXML;
	private: System::Windows::Forms::TextBox^  txtMissingXML;
	private: System::Windows::Forms::Button^  bnBrowseFOXML;

	private: System::Windows::Forms::CheckBox^  cbFOXML;
	private: System::Windows::Forms::TextBox^  txtFOXML;


	private: System::Windows::Forms::Button^  bnBrowseRenamingXML;
	private: System::Windows::Forms::CheckBox^  cbRenamingXML;
	private: System::Windows::Forms::TextBox^  txtRenamingXML;
private: System::Windows::Forms::CheckBox^  cbIgnoreSamples;
private: System::Windows::Forms::TextBox^  txtRSpace;
private: System::Windows::Forms::Label^  label18;
private: System::Windows::Forms::CheckBox^  cbForceLower;
private: System::Windows::Forms::Label^  label19;
private: System::Windows::Forms::TextBox^  txtMaxSampleSize;
private: System::Windows::Forms::Label^  label21;
private: System::Windows::Forms::Label^  label20;
private: System::Windows::Forms::TextBox^  txtParallelDownloads;
private: System::Windows::Forms::Label^  label22;
private: System::Windows::Forms::TextBox^  txtOtherExtensions;













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
			this->txtRSpace = (gcnew System::Windows::Forms::TextBox());
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
			this->label18 = (gcnew System::Windows::Forms::Label());
			this->label10 = (gcnew System::Windows::Forms::Label());
			this->label9 = (gcnew System::Windows::Forms::Label());
			this->label8 = (gcnew System::Windows::Forms::Label());
			this->label7 = (gcnew System::Windows::Forms::Label());
			this->label5 = (gcnew System::Windows::Forms::Label());
			this->label4 = (gcnew System::Windows::Forms::Label());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->groupBox2 = (gcnew System::Windows::Forms::GroupBox());
			this->bnBrowseWTWRSS = (gcnew System::Windows::Forms::Button());
			this->txtWTWRSS = (gcnew System::Windows::Forms::TextBox());
			this->cbWTWRSS = (gcnew System::Windows::Forms::CheckBox());
			this->label17 = (gcnew System::Windows::Forms::Label());
			this->label16 = (gcnew System::Windows::Forms::Label());
			this->label15 = (gcnew System::Windows::Forms::Label());
			this->txtExportRSSMaxDays = (gcnew System::Windows::Forms::TextBox());
			this->txtExportRSSMaxShows = (gcnew System::Windows::Forms::TextBox());
			this->saveFile = (gcnew System::Windows::Forms::SaveFileDialog());
			this->cbTxtToSub = (gcnew System::Windows::Forms::CheckBox());
			this->txtSpecialsFolderName = (gcnew System::Windows::Forms::TextBox());
			this->txtVideoExtensions = (gcnew System::Windows::Forms::TextBox());
			this->cbStartupTab = (gcnew System::Windows::Forms::ComboBox());
			this->cbShowEpisodePictures = (gcnew System::Windows::Forms::CheckBox());
			this->cbLeadingZero = (gcnew System::Windows::Forms::CheckBox());
			this->cbKeepTogether = (gcnew System::Windows::Forms::CheckBox());
			this->chkShowInTaskbar = (gcnew System::Windows::Forms::CheckBox());
			this->cbNotificationIcon = (gcnew System::Windows::Forms::CheckBox());
			this->txtWTWDays = (gcnew System::Windows::Forms::TextBox());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->label13 = (gcnew System::Windows::Forms::Label());
			this->label14 = (gcnew System::Windows::Forms::Label());
			this->label6 = (gcnew System::Windows::Forms::Label());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->tabControl1 = (gcnew System::Windows::Forms::TabControl());
			this->tabPage1 = (gcnew System::Windows::Forms::TabPage());
			this->label21 = (gcnew System::Windows::Forms::Label());
			this->label20 = (gcnew System::Windows::Forms::Label());
			this->txtParallelDownloads = (gcnew System::Windows::Forms::TextBox());
			this->tabPage2 = (gcnew System::Windows::Forms::TabPage());
			this->label19 = (gcnew System::Windows::Forms::Label());
			this->txtMaxSampleSize = (gcnew System::Windows::Forms::TextBox());
			this->cbForceLower = (gcnew System::Windows::Forms::CheckBox());
			this->cbIgnoreSamples = (gcnew System::Windows::Forms::CheckBox());
			this->tabPage3 = (gcnew System::Windows::Forms::TabPage());
			this->groupBox5 = (gcnew System::Windows::Forms::GroupBox());
			this->bnBrowseFOXML = (gcnew System::Windows::Forms::Button());
			this->cbFOXML = (gcnew System::Windows::Forms::CheckBox());
			this->txtFOXML = (gcnew System::Windows::Forms::TextBox());
			this->groupBox4 = (gcnew System::Windows::Forms::GroupBox());
			this->bnBrowseRenamingXML = (gcnew System::Windows::Forms::Button());
			this->cbRenamingXML = (gcnew System::Windows::Forms::CheckBox());
			this->txtRenamingXML = (gcnew System::Windows::Forms::TextBox());
			this->groupBox3 = (gcnew System::Windows::Forms::GroupBox());
			this->bnBrowseMissingXML = (gcnew System::Windows::Forms::Button());
			this->cbMissingXML = (gcnew System::Windows::Forms::CheckBox());
			this->bnBrowseMissingCSV = (gcnew System::Windows::Forms::Button());
			this->txtMissingXML = (gcnew System::Windows::Forms::TextBox());
			this->cbMissingCSV = (gcnew System::Windows::Forms::CheckBox());
			this->txtMissingCSV = (gcnew System::Windows::Forms::TextBox());
			this->txtOtherExtensions = (gcnew System::Windows::Forms::TextBox());
			this->label22 = (gcnew System::Windows::Forms::Label());
			this->groupBox1->SuspendLayout();
			this->groupBox2->SuspendLayout();
			this->tabControl1->SuspendLayout();
			this->tabPage1->SuspendLayout();
			this->tabPage2->SuspendLayout();
			this->tabPage3->SuspendLayout();
			this->groupBox5->SuspendLayout();
			this->groupBox4->SuspendLayout();
			this->groupBox3->SuspendLayout();
			this->SuspendLayout();
			// 
			// OKButton
			// 
			this->OKButton->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->OKButton->Location = System::Drawing::Point(279, 445);
			this->OKButton->Name = L"OKButton";
			this->OKButton->Size = System::Drawing::Size(75, 23);
			this->OKButton->TabIndex = 0;
			this->OKButton->Text = L"OK";
			this->OKButton->UseVisualStyleBackColor = true;
			this->OKButton->Click += gcnew System::EventHandler(this, &Preferences::OKButton_Click);
			// 
			// bnCancel
			// 
			this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnCancel->Location = System::Drawing::Point(360, 445);
			this->bnCancel->Name = L"bnCancel";
			this->bnCancel->Size = System::Drawing::Size(75, 23);
			this->bnCancel->TabIndex = 1;
			this->bnCancel->Text = L"Cancel";
			this->bnCancel->UseVisualStyleBackColor = true;
			this->bnCancel->Click += gcnew System::EventHandler(this, &Preferences::CancelButton_Click);
			// 
			// groupBox1
			// 
			this->groupBox1->Controls->Add(this->txtRSpace);
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
			this->groupBox1->Controls->Add(this->label18);
			this->groupBox1->Controls->Add(this->label10);
			this->groupBox1->Controls->Add(this->label9);
			this->groupBox1->Controls->Add(this->label8);
			this->groupBox1->Controls->Add(this->label7);
			this->groupBox1->Controls->Add(this->label5);
			this->groupBox1->Controls->Add(this->label4);
			this->groupBox1->Controls->Add(this->label3);
			this->groupBox1->Location = System::Drawing::Point(6, 6);
			this->groupBox1->Name = L"groupBox1";
			this->groupBox1->Size = System::Drawing::Size(403, 82);
			this->groupBox1->TabIndex = 0;
			this->groupBox1->TabStop = false;
			this->groupBox1->Text = L"Filename Character Replacements";
			// 
			// txtRSpace
			// 
			this->txtRSpace->Location = System::Drawing::Point(358, 50);
			this->txtRSpace->Name = L"txtRSpace";
			this->txtRSpace->Size = System::Drawing::Size(28, 20);
			this->txtRSpace->TabIndex = 19;
			this->txtRSpace->Text = L" ";
			// 
			// txtR9
			// 
			this->txtR9->Location = System::Drawing::Point(278, 50);
			this->txtR9->Name = L"txtR9";
			this->txtR9->Size = System::Drawing::Size(28, 20);
			this->txtR9->TabIndex = 17;
			// 
			// txtR6
			// 
			this->txtR6->Location = System::Drawing::Point(197, 50);
			this->txtR6->Name = L"txtR6";
			this->txtR6->Size = System::Drawing::Size(28, 20);
			this->txtR6->TabIndex = 15;
			// 
			// txtR3
			// 
			this->txtR3->Location = System::Drawing::Point(117, 50);
			this->txtR3->Name = L"txtR3";
			this->txtR3->Size = System::Drawing::Size(28, 20);
			this->txtR3->TabIndex = 13;
			// 
			// txtR8
			// 
			this->txtR8->Location = System::Drawing::Point(37, 50);
			this->txtR8->Name = L"txtR8";
			this->txtR8->Size = System::Drawing::Size(28, 20);
			this->txtR8->TabIndex = 11;
			// 
			// txtR5
			// 
			this->txtR5->Location = System::Drawing::Point(358, 24);
			this->txtR5->Name = L"txtR5";
			this->txtR5->Size = System::Drawing::Size(28, 20);
			this->txtR5->TabIndex = 9;
			// 
			// txtR2
			// 
			this->txtR2->Location = System::Drawing::Point(278, 24);
			this->txtR2->Name = L"txtR2";
			this->txtR2->Size = System::Drawing::Size(28, 20);
			this->txtR2->TabIndex = 7;
			// 
			// txtR7
			// 
			this->txtR7->Location = System::Drawing::Point(198, 24);
			this->txtR7->Name = L"txtR7";
			this->txtR7->Size = System::Drawing::Size(28, 20);
			this->txtR7->TabIndex = 5;
			// 
			// txtR4
			// 
			this->txtR4->Location = System::Drawing::Point(117, 24);
			this->txtR4->Name = L"txtR4";
			this->txtR4->Size = System::Drawing::Size(28, 20);
			this->txtR4->TabIndex = 3;
			// 
			// txtR1
			// 
			this->txtR1->Location = System::Drawing::Point(37, 24);
			this->txtR1->Name = L"txtR1";
			this->txtR1->Size = System::Drawing::Size(28, 20);
			this->txtR1->TabIndex = 1;
			// 
			// label12
			// 
			this->label12->AutoSize = true;
			this->label12->Location = System::Drawing::Point(179, 53);
			this->label12->Name = L"label12";
			this->label12->Size = System::Drawing::Size(12, 13);
			this->label12->TabIndex = 14;
			this->label12->Text = L"\"";
			// 
			// label11
			// 
			this->label11->AutoSize = true;
			this->label11->Location = System::Drawing::Point(183, 27);
			this->label11->Name = L"label11";
			this->label11->Size = System::Drawing::Size(9, 13);
			this->label11->TabIndex = 4;
			this->label11->Text = L"|";
			// 
			// label18
			// 
			this->label18->AutoSize = true;
			this->label18->Location = System::Drawing::Point(309, 53);
			this->label18->Name = L"label18";
			this->label18->Size = System::Drawing::Size(48, 13);
			this->label18->TabIndex = 18;
			this->label18->Text = L"<space>";
			// 
			// label10
			// 
			this->label10->AutoSize = true;
			this->label10->Location = System::Drawing::Point(22, 53);
			this->label10->Name = L"label10";
			this->label10->Size = System::Drawing::Size(12, 13);
			this->label10->TabIndex = 10;
			this->label10->Text = L"\\";
			// 
			// label9
			// 
			this->label9->AutoSize = true;
			this->label9->Location = System::Drawing::Point(263, 53);
			this->label9->Name = L"label9";
			this->label9->Size = System::Drawing::Size(12, 13);
			this->label9->TabIndex = 16;
			this->label9->Text = L"/";
			// 
			// label8
			// 
			this->label8->AutoSize = true;
			this->label8->Location = System::Drawing::Point(339, 27);
			this->label8->Name = L"label8";
			this->label8->Size = System::Drawing::Size(13, 13);
			this->label8->TabIndex = 8;
			this->label8->Text = L">";
			// 
			// label7
			// 
			this->label7->AutoSize = true;
			this->label7->Location = System::Drawing::Point(98, 27);
			this->label7->Name = L"label7";
			this->label7->Size = System::Drawing::Size(13, 13);
			this->label7->TabIndex = 2;
			this->label7->Text = L"<";
			// 
			// label5
			// 
			this->label5->AutoSize = true;
			this->label5->Location = System::Drawing::Point(101, 53);
			this->label5->Name = L"label5";
			this->label5->Size = System::Drawing::Size(11, 13);
			this->label5->TabIndex = 12;
			this->label5->Text = L"*";
			// 
			// label4
			// 
			this->label4->AutoSize = true;
			this->label4->Location = System::Drawing::Point(262, 25);
			this->label4->Name = L"label4";
			this->label4->Size = System::Drawing::Size(10, 13);
			this->label4->TabIndex = 6;
			this->label4->Text = L":";
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Location = System::Drawing::Point(18, 27);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(13, 13);
			this->label3->TabIndex = 0;
			this->label3->Text = L"\?";
			// 
			// groupBox2
			// 
			this->groupBox2->Controls->Add(this->bnBrowseWTWRSS);
			this->groupBox2->Controls->Add(this->txtWTWRSS);
			this->groupBox2->Controls->Add(this->cbWTWRSS);
			this->groupBox2->Controls->Add(this->label17);
			this->groupBox2->Controls->Add(this->label16);
			this->groupBox2->Controls->Add(this->label15);
			this->groupBox2->Controls->Add(this->txtExportRSSMaxDays);
			this->groupBox2->Controls->Add(this->txtExportRSSMaxShows);
			this->groupBox2->Location = System::Drawing::Point(6, 6);
			this->groupBox2->Name = L"groupBox2";
			this->groupBox2->Size = System::Drawing::Size(403, 84);
			this->groupBox2->TabIndex = 0;
			this->groupBox2->TabStop = false;
			this->groupBox2->Text = L"When to Watch";
			// 
			// bnBrowseWTWRSS
			// 
			this->bnBrowseWTWRSS->Location = System::Drawing::Point(321, 18);
			this->bnBrowseWTWRSS->Name = L"bnBrowseWTWRSS";
			this->bnBrowseWTWRSS->Size = System::Drawing::Size(75, 23);
			this->bnBrowseWTWRSS->TabIndex = 2;
			this->bnBrowseWTWRSS->Text = L"Browse...";
			this->bnBrowseWTWRSS->UseVisualStyleBackColor = true;
			this->bnBrowseWTWRSS->Click += gcnew System::EventHandler(this, &Preferences::bnBrowseWTWRSS_Click);
			// 
			// txtWTWRSS
			// 
			this->txtWTWRSS->Location = System::Drawing::Point(64, 20);
			this->txtWTWRSS->Name = L"txtWTWRSS";
			this->txtWTWRSS->Size = System::Drawing::Size(251, 20);
			this->txtWTWRSS->TabIndex = 1;
			// 
			// cbWTWRSS
			// 
			this->cbWTWRSS->AutoSize = true;
			this->cbWTWRSS->Location = System::Drawing::Point(10, 22);
			this->cbWTWRSS->Name = L"cbWTWRSS";
			this->cbWTWRSS->Size = System::Drawing::Size(48, 17);
			this->cbWTWRSS->TabIndex = 0;
			this->cbWTWRSS->Text = L"RSS";
			this->cbWTWRSS->UseVisualStyleBackColor = true;
			this->cbWTWRSS->CheckedChanged += gcnew System::EventHandler(this, &Preferences::EnableDisable);
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
			// cbTxtToSub
			// 
			this->cbTxtToSub->AutoSize = true;
			this->cbTxtToSub->Location = System::Drawing::Point(9, 169);
			this->cbTxtToSub->Name = L"cbTxtToSub";
			this->cbTxtToSub->Size = System::Drawing::Size(118, 17);
			this->cbTxtToSub->TabIndex = 12;
			this->cbTxtToSub->Text = L"Rename .txt to .sub";
			this->cbTxtToSub->UseVisualStyleBackColor = true;
			// 
			// txtSpecialsFolderName
			// 
			this->txtSpecialsFolderName->Location = System::Drawing::Point(116, 216);
			this->txtSpecialsFolderName->Name = L"txtSpecialsFolderName";
			this->txtSpecialsFolderName->Size = System::Drawing::Size(279, 20);
			this->txtSpecialsFolderName->TabIndex = 15;
			// 
			// txtVideoExtensions
			// 
			this->txtVideoExtensions->Location = System::Drawing::Point(102, 96);
			this->txtVideoExtensions->Name = L"txtVideoExtensions";
			this->txtVideoExtensions->Size = System::Drawing::Size(293, 20);
			this->txtVideoExtensions->TabIndex = 8;
			// 
			// cbStartupTab
			// 
			this->cbStartupTab->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
			this->cbStartupTab->FormattingEnabled = true;
			this->cbStartupTab->Items->AddRange(gcnew cli::array< System::Object^  >(7) {L"My Shows", L"Rename", L"Missing", L"Finding and Organising", 
				L"Folder Monitor", L"Torrent Match", L"When to Watch"});
			this->cbStartupTab->Location = System::Drawing::Point(103, 35);
			this->cbStartupTab->Name = L"cbStartupTab";
			this->cbStartupTab->Size = System::Drawing::Size(135, 21);
			this->cbStartupTab->TabIndex = 4;
			// 
			// cbShowEpisodePictures
			// 
			this->cbShowEpisodePictures->AutoSize = true;
			this->cbShowEpisodePictures->Location = System::Drawing::Point(9, 85);
			this->cbShowEpisodePictures->Name = L"cbShowEpisodePictures";
			this->cbShowEpisodePictures->Size = System::Drawing::Size(218, 17);
			this->cbShowEpisodePictures->TabIndex = 12;
			this->cbShowEpisodePictures->Text = L"Show episode pictures in episode guides";
			this->cbShowEpisodePictures->UseVisualStyleBackColor = true;
			// 
			// cbLeadingZero
			// 
			this->cbLeadingZero->AutoSize = true;
			this->cbLeadingZero->Location = System::Drawing::Point(9, 192);
			this->cbLeadingZero->Name = L"cbLeadingZero";
			this->cbLeadingZero->Size = System::Drawing::Size(170, 17);
			this->cbLeadingZero->TabIndex = 13;
			this->cbLeadingZero->Text = L"Leading 0 on Season numbers";
			this->cbLeadingZero->UseVisualStyleBackColor = true;
			// 
			// cbKeepTogether
			// 
			this->cbKeepTogether->AutoSize = true;
			this->cbKeepTogether->Location = System::Drawing::Point(9, 148);
			this->cbKeepTogether->Name = L"cbKeepTogether";
			this->cbKeepTogether->Size = System::Drawing::Size(251, 17);
			this->cbKeepTogether->TabIndex = 11;
			this->cbKeepTogether->Text = L"Copy/Move files with same base name as video";
			this->cbKeepTogether->UseVisualStyleBackColor = true;
			this->cbKeepTogether->CheckedChanged += gcnew System::EventHandler(this, &Preferences::cbKeepTogether_CheckedChanged);
			// 
			// chkShowInTaskbar
			// 
			this->chkShowInTaskbar->AutoSize = true;
			this->chkShowInTaskbar->Location = System::Drawing::Point(169, 62);
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
			this->cbNotificationIcon->Location = System::Drawing::Point(9, 62);
			this->cbNotificationIcon->Name = L"cbNotificationIcon";
			this->cbNotificationIcon->Size = System::Drawing::Size(154, 17);
			this->cbNotificationIcon->TabIndex = 5;
			this->cbNotificationIcon->Text = L"Show notification area icon";
			this->cbNotificationIcon->UseVisualStyleBackColor = true;
			this->cbNotificationIcon->CheckedChanged += gcnew System::EventHandler(this, &Preferences::cbNotificationIcon_CheckedChanged);
			// 
			// txtWTWDays
			// 
			this->txtWTWDays->Location = System::Drawing::Point(92, 9);
			this->txtWTWDays->Name = L"txtWTWDays";
			this->txtWTWDays->Size = System::Drawing::Size(28, 20);
			this->txtWTWDays->TabIndex = 1;
			this->txtWTWDays->KeyPress += gcnew System::Windows::Forms::KeyPressEventHandler(this, &Preferences::txtNumberOnlyKeyPress);
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Location = System::Drawing::Point(126, 12);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(111, 13);
			this->label2->TabIndex = 2;
			this->label2->Text = L"days counts as recent";
			// 
			// label13
			// 
			this->label13->AutoSize = true;
			this->label13->Location = System::Drawing::Point(9, 219);
			this->label13->Name = L"label13";
			this->label13->Size = System::Drawing::Size(108, 13);
			this->label13->TabIndex = 14;
			this->label13->Text = L"Specials folder name:";
			// 
			// label14
			// 
			this->label14->AutoSize = true;
			this->label14->Location = System::Drawing::Point(6, 99);
			this->label14->Name = L"label14";
			this->label14->Size = System::Drawing::Size(90, 13);
			this->label14->TabIndex = 7;
			this->label14->Text = L"Video extensions:";
			// 
			// label6
			// 
			this->label6->AutoSize = true;
			this->label6->Location = System::Drawing::Point(6, 38);
			this->label6->Name = L"label6";
			this->label6->Size = System::Drawing::Size(62, 13);
			this->label6->TabIndex = 3;
			this->label6->Text = L"Startup tab:";
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(6, 12);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(80, 13);
			this->label1->TabIndex = 0;
			this->label1->Text = L"When to watch";
			// 
			// tabControl1
			// 
			this->tabControl1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->tabControl1->Controls->Add(this->tabPage1);
			this->tabControl1->Controls->Add(this->tabPage2);
			this->tabControl1->Controls->Add(this->tabPage3);
			this->tabControl1->Location = System::Drawing::Point(12, 12);
			this->tabControl1->Name = L"tabControl1";
			this->tabControl1->SelectedIndex = 0;
			this->tabControl1->Size = System::Drawing::Size(423, 421);
			this->tabControl1->TabIndex = 5;
			// 
			// tabPage1
			// 
			this->tabPage1->Controls->Add(this->cbStartupTab);
			this->tabPage1->Controls->Add(this->label21);
			this->tabPage1->Controls->Add(this->label1);
			this->tabPage1->Controls->Add(this->cbShowEpisodePictures);
			this->tabPage1->Controls->Add(this->label6);
			this->tabPage1->Controls->Add(this->chkShowInTaskbar);
			this->tabPage1->Controls->Add(this->label20);
			this->tabPage1->Controls->Add(this->label2);
			this->tabPage1->Controls->Add(this->cbNotificationIcon);
			this->tabPage1->Controls->Add(this->txtParallelDownloads);
			this->tabPage1->Controls->Add(this->txtWTWDays);
			this->tabPage1->Location = System::Drawing::Point(4, 22);
			this->tabPage1->Name = L"tabPage1";
			this->tabPage1->Padding = System::Windows::Forms::Padding(3);
			this->tabPage1->Size = System::Drawing::Size(415, 395);
			this->tabPage1->TabIndex = 0;
			this->tabPage1->Text = L"General";
			this->tabPage1->UseVisualStyleBackColor = true;
			// 
			// label21
			// 
			this->label21->AutoSize = true;
			this->label21->Location = System::Drawing::Point(6, 111);
			this->label21->Name = L"label21";
			this->label21->Size = System::Drawing::Size(82, 13);
			this->label21->TabIndex = 0;
			this->label21->Text = L"Download up to";
			// 
			// label20
			// 
			this->label20->AutoSize = true;
			this->label20->Location = System::Drawing::Point(126, 111);
			this->label20->Name = L"label20";
			this->label20->Size = System::Drawing::Size(170, 13);
			this->label20->TabIndex = 2;
			this->label20->Text = L"shows simultaneously from thetvdb";
			// 
			// txtParallelDownloads
			// 
			this->txtParallelDownloads->Location = System::Drawing::Point(92, 108);
			this->txtParallelDownloads->Name = L"txtParallelDownloads";
			this->txtParallelDownloads->Size = System::Drawing::Size(28, 20);
			this->txtParallelDownloads->TabIndex = 1;
			this->txtParallelDownloads->KeyPress += gcnew System::Windows::Forms::KeyPressEventHandler(this, &Preferences::txtNumberOnlyKeyPress);
			// 
			// tabPage2
			// 
			this->tabPage2->Controls->Add(this->label19);
			this->tabPage2->Controls->Add(this->txtMaxSampleSize);
			this->tabPage2->Controls->Add(this->cbTxtToSub);
			this->tabPage2->Controls->Add(this->label22);
			this->tabPage2->Controls->Add(this->label14);
			this->tabPage2->Controls->Add(this->groupBox1);
			this->tabPage2->Controls->Add(this->txtSpecialsFolderName);
			this->tabPage2->Controls->Add(this->label13);
			this->tabPage2->Controls->Add(this->txtOtherExtensions);
			this->tabPage2->Controls->Add(this->txtVideoExtensions);
			this->tabPage2->Controls->Add(this->cbKeepTogether);
			this->tabPage2->Controls->Add(this->cbForceLower);
			this->tabPage2->Controls->Add(this->cbIgnoreSamples);
			this->tabPage2->Controls->Add(this->cbLeadingZero);
			this->tabPage2->Location = System::Drawing::Point(4, 22);
			this->tabPage2->Name = L"tabPage2";
			this->tabPage2->Padding = System::Windows::Forms::Padding(3);
			this->tabPage2->Size = System::Drawing::Size(415, 395);
			this->tabPage2->TabIndex = 1;
			this->tabPage2->Text = L"Files and Folders";
			this->tabPage2->UseVisualStyleBackColor = true;
			// 
			// label19
			// 
			this->label19->AutoSize = true;
			this->label19->Location = System::Drawing::Point(231, 245);
			this->label19->Name = L"label19";
			this->label19->Size = System::Drawing::Size(55, 13);
			this->label19->TabIndex = 18;
			this->label19->Text = L"MB in size";
			// 
			// txtMaxSampleSize
			// 
			this->txtMaxSampleSize->Location = System::Drawing::Point(175, 242);
			this->txtMaxSampleSize->Name = L"txtMaxSampleSize";
			this->txtMaxSampleSize->Size = System::Drawing::Size(53, 20);
			this->txtMaxSampleSize->TabIndex = 17;
			this->txtMaxSampleSize->KeyPress += gcnew System::Windows::Forms::KeyPressEventHandler(this, &Preferences::txtNumberOnlyKeyPress);
			// 
			// cbForceLower
			// 
			this->cbForceLower->AutoSize = true;
			this->cbForceLower->Location = System::Drawing::Point(9, 267);
			this->cbForceLower->Name = L"cbForceLower";
			this->cbForceLower->Size = System::Drawing::Size(167, 17);
			this->cbForceLower->TabIndex = 19;
			this->cbForceLower->Text = L"Make all filenames lower case";
			this->cbForceLower->UseVisualStyleBackColor = true;
			// 
			// cbIgnoreSamples
			// 
			this->cbIgnoreSamples->AutoSize = true;
			this->cbIgnoreSamples->Location = System::Drawing::Point(9, 244);
			this->cbIgnoreSamples->Name = L"cbIgnoreSamples";
			this->cbIgnoreSamples->Size = System::Drawing::Size(166, 17);
			this->cbIgnoreSamples->TabIndex = 16;
			this->cbIgnoreSamples->Text = L"Ignore \"sample\" videos, up to";
			this->cbIgnoreSamples->UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			this->tabPage3->Controls->Add(this->groupBox5);
			this->tabPage3->Controls->Add(this->groupBox4);
			this->tabPage3->Controls->Add(this->groupBox3);
			this->tabPage3->Controls->Add(this->groupBox2);
			this->tabPage3->Location = System::Drawing::Point(4, 22);
			this->tabPage3->Name = L"tabPage3";
			this->tabPage3->Padding = System::Windows::Forms::Padding(3);
			this->tabPage3->Size = System::Drawing::Size(415, 395);
			this->tabPage3->TabIndex = 2;
			this->tabPage3->Text = L"Automatic Export";
			this->tabPage3->UseVisualStyleBackColor = true;
			// 
			// groupBox5
			// 
			this->groupBox5->Controls->Add(this->bnBrowseFOXML);
			this->groupBox5->Controls->Add(this->cbFOXML);
			this->groupBox5->Controls->Add(this->txtFOXML);
			this->groupBox5->Location = System::Drawing::Point(6, 244);
			this->groupBox5->Name = L"groupBox5";
			this->groupBox5->Size = System::Drawing::Size(402, 55);
			this->groupBox5->TabIndex = 3;
			this->groupBox5->TabStop = false;
			this->groupBox5->Text = L"Finding and Organising";
			// 
			// bnBrowseFOXML
			// 
			this->bnBrowseFOXML->Location = System::Drawing::Point(321, 19);
			this->bnBrowseFOXML->Name = L"bnBrowseFOXML";
			this->bnBrowseFOXML->Size = System::Drawing::Size(75, 23);
			this->bnBrowseFOXML->TabIndex = 2;
			this->bnBrowseFOXML->Text = L"Browse...";
			this->bnBrowseFOXML->UseVisualStyleBackColor = true;
			this->bnBrowseFOXML->Click += gcnew System::EventHandler(this, &Preferences::bnBrowseFOXML_Click);
			// 
			// cbFOXML
			// 
			this->cbFOXML->AutoSize = true;
			this->cbFOXML->Location = System::Drawing::Point(10, 23);
			this->cbFOXML->Name = L"cbFOXML";
			this->cbFOXML->Size = System::Drawing::Size(48, 17);
			this->cbFOXML->TabIndex = 0;
			this->cbFOXML->Text = L"XML";
			this->cbFOXML->UseVisualStyleBackColor = true;
			this->cbFOXML->CheckedChanged += gcnew System::EventHandler(this, &Preferences::EnableDisable);
			// 
			// txtFOXML
			// 
			this->txtFOXML->Location = System::Drawing::Point(64, 21);
			this->txtFOXML->Name = L"txtFOXML";
			this->txtFOXML->Size = System::Drawing::Size(251, 20);
			this->txtFOXML->TabIndex = 1;
			// 
			// groupBox4
			// 
			this->groupBox4->Controls->Add(this->bnBrowseRenamingXML);
			this->groupBox4->Controls->Add(this->cbRenamingXML);
			this->groupBox4->Controls->Add(this->txtRenamingXML);
			this->groupBox4->Location = System::Drawing::Point(6, 181);
			this->groupBox4->Name = L"groupBox4";
			this->groupBox4->Size = System::Drawing::Size(402, 57);
			this->groupBox4->TabIndex = 2;
			this->groupBox4->TabStop = false;
			this->groupBox4->Text = L"Renaming";
			// 
			// bnBrowseRenamingXML
			// 
			this->bnBrowseRenamingXML->Location = System::Drawing::Point(321, 19);
			this->bnBrowseRenamingXML->Name = L"bnBrowseRenamingXML";
			this->bnBrowseRenamingXML->Size = System::Drawing::Size(75, 23);
			this->bnBrowseRenamingXML->TabIndex = 2;
			this->bnBrowseRenamingXML->Text = L"Browse...";
			this->bnBrowseRenamingXML->UseVisualStyleBackColor = true;
			this->bnBrowseRenamingXML->Click += gcnew System::EventHandler(this, &Preferences::bnBrowseRenamingXML_Click);
			// 
			// cbRenamingXML
			// 
			this->cbRenamingXML->AutoSize = true;
			this->cbRenamingXML->Location = System::Drawing::Point(10, 23);
			this->cbRenamingXML->Name = L"cbRenamingXML";
			this->cbRenamingXML->Size = System::Drawing::Size(48, 17);
			this->cbRenamingXML->TabIndex = 0;
			this->cbRenamingXML->Text = L"XML";
			this->cbRenamingXML->UseVisualStyleBackColor = true;
			this->cbRenamingXML->CheckedChanged += gcnew System::EventHandler(this, &Preferences::EnableDisable);
			// 
			// txtRenamingXML
			// 
			this->txtRenamingXML->Location = System::Drawing::Point(64, 21);
			this->txtRenamingXML->Name = L"txtRenamingXML";
			this->txtRenamingXML->Size = System::Drawing::Size(251, 20);
			this->txtRenamingXML->TabIndex = 1;
			// 
			// groupBox3
			// 
			this->groupBox3->Controls->Add(this->bnBrowseMissingXML);
			this->groupBox3->Controls->Add(this->cbMissingXML);
			this->groupBox3->Controls->Add(this->bnBrowseMissingCSV);
			this->groupBox3->Controls->Add(this->txtMissingXML);
			this->groupBox3->Controls->Add(this->cbMissingCSV);
			this->groupBox3->Controls->Add(this->txtMissingCSV);
			this->groupBox3->Location = System::Drawing::Point(6, 96);
			this->groupBox3->Name = L"groupBox3";
			this->groupBox3->Size = System::Drawing::Size(402, 79);
			this->groupBox3->TabIndex = 1;
			this->groupBox3->TabStop = false;
			this->groupBox3->Text = L"Missing";
			// 
			// bnBrowseMissingXML
			// 
			this->bnBrowseMissingXML->Location = System::Drawing::Point(321, 44);
			this->bnBrowseMissingXML->Name = L"bnBrowseMissingXML";
			this->bnBrowseMissingXML->Size = System::Drawing::Size(75, 23);
			this->bnBrowseMissingXML->TabIndex = 5;
			this->bnBrowseMissingXML->Text = L"Browse...";
			this->bnBrowseMissingXML->UseVisualStyleBackColor = true;
			this->bnBrowseMissingXML->Click += gcnew System::EventHandler(this, &Preferences::bnBrowseMissingXML_Click);
			// 
			// cbMissingXML
			// 
			this->cbMissingXML->AutoSize = true;
			this->cbMissingXML->Location = System::Drawing::Point(10, 48);
			this->cbMissingXML->Name = L"cbMissingXML";
			this->cbMissingXML->Size = System::Drawing::Size(48, 17);
			this->cbMissingXML->TabIndex = 3;
			this->cbMissingXML->Text = L"XML";
			this->cbMissingXML->UseVisualStyleBackColor = true;
			this->cbMissingXML->CheckedChanged += gcnew System::EventHandler(this, &Preferences::EnableDisable);
			// 
			// bnBrowseMissingCSV
			// 
			this->bnBrowseMissingCSV->Location = System::Drawing::Point(321, 14);
			this->bnBrowseMissingCSV->Name = L"bnBrowseMissingCSV";
			this->bnBrowseMissingCSV->Size = System::Drawing::Size(75, 23);
			this->bnBrowseMissingCSV->TabIndex = 2;
			this->bnBrowseMissingCSV->Text = L"Browse...";
			this->bnBrowseMissingCSV->UseVisualStyleBackColor = true;
			this->bnBrowseMissingCSV->Click += gcnew System::EventHandler(this, &Preferences::bnBrowseMissingCSV_Click);
			// 
			// txtMissingXML
			// 
			this->txtMissingXML->Location = System::Drawing::Point(64, 46);
			this->txtMissingXML->Name = L"txtMissingXML";
			this->txtMissingXML->Size = System::Drawing::Size(251, 20);
			this->txtMissingXML->TabIndex = 4;
			// 
			// cbMissingCSV
			// 
			this->cbMissingCSV->AutoSize = true;
			this->cbMissingCSV->Location = System::Drawing::Point(10, 19);
			this->cbMissingCSV->Name = L"cbMissingCSV";
			this->cbMissingCSV->Size = System::Drawing::Size(47, 17);
			this->cbMissingCSV->TabIndex = 0;
			this->cbMissingCSV->Text = L"CSV";
			this->cbMissingCSV->UseVisualStyleBackColor = true;
			this->cbMissingCSV->CheckedChanged += gcnew System::EventHandler(this, &Preferences::EnableDisable);
			// 
			// txtMissingCSV
			// 
			this->txtMissingCSV->Location = System::Drawing::Point(64, 17);
			this->txtMissingCSV->Name = L"txtMissingCSV";
			this->txtMissingCSV->Size = System::Drawing::Size(251, 20);
			this->txtMissingCSV->TabIndex = 1;
			// 
			// txtOtherExtensions
			// 
			this->txtOtherExtensions->Location = System::Drawing::Point(102, 122);
			this->txtOtherExtensions->Name = L"txtOtherExtensions";
			this->txtOtherExtensions->Size = System::Drawing::Size(293, 20);
			this->txtOtherExtensions->TabIndex = 10;
			// 
			// label22
			// 
			this->label22->AutoSize = true;
			this->label22->Location = System::Drawing::Point(6, 125);
			this->label22->Name = L"label22";
			this->label22->Size = System::Drawing::Size(89, 13);
			this->label22->TabIndex = 9;
			this->label22->Text = L"Other extensions:";
			// 
			// Preferences
			// 
			this->AcceptButton = this->OKButton;
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnCancel;
			this->ClientSize = System::Drawing::Size(447, 480);
			this->Controls->Add(this->tabControl1);
			this->Controls->Add(this->bnCancel);
			this->Controls->Add(this->OKButton);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->MaximizeBox = false;
			this->Name = L"Preferences";
			this->ShowInTaskbar = false;
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = L"Preferences";
			this->Load += gcnew System::EventHandler(this, &Preferences::Preferences_Load);
			this->groupBox1->ResumeLayout(false);
			this->groupBox1->PerformLayout();
			this->groupBox2->ResumeLayout(false);
			this->groupBox2->PerformLayout();
			this->tabControl1->ResumeLayout(false);
			this->tabPage1->ResumeLayout(false);
			this->tabPage1->PerformLayout();
			this->tabPage2->ResumeLayout(false);
			this->tabPage2->PerformLayout();
			this->tabPage3->ResumeLayout(false);
			this->groupBox5->ResumeLayout(false);
			this->groupBox5->PerformLayout();
			this->groupBox4->ResumeLayout(false);
			this->groupBox4->PerformLayout();
			this->groupBox3->ResumeLayout(false);
			this->groupBox3->PerformLayout();
			this->ResumeLayout(false);

		}
#pragma endregion
	private: System::Void OKButton_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (!TVSettings::OKExtensionsString(txtVideoExtensions->Text))
				 {
					 ::DialogResult res = MessageBox::Show("Extensions list must be separated by semicolons, and each extension must start with a dot.","Preferences",MessageBoxButtons::OK, MessageBoxIcon::Warning);
					 tabControl1->SelectedIndex = 1;
					 txtVideoExtensions->Focus();
					 return;
				 }
				 if (!TVSettings::OKExtensionsString(txtOtherExtensions->Text))
				 {
					 ::DialogResult res = MessageBox::Show("Extensions list must be separated by semicolons, and each extension must start with a dot.","Preferences",MessageBoxButtons::OK, MessageBoxIcon::Warning);
					 tabControl1->SelectedIndex = 1;
					 txtOtherExtensions->Focus();
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
				 S->Replacements->Add(gcnew Replacement(" ",txtRSpace->Text));

				 S->ExportWTWRSS = cbWTWRSS->Checked;
				 S->ExportWTWRSSTo = txtWTWRSS->Text;

				 S->ExportMissingXML= cbMissingXML->Checked;
				 S->ExportMissingXMLTo = txtMissingXML->Text; 
				 S->ExportMissingCSV = cbMissingCSV->Checked; 
				 S->ExportMissingCSVTo = txtMissingCSV->Text; 
				 S->ExportRenamingXML = cbRenamingXML->Checked; 
				 S->ExportRenamingXMLTo = txtRenamingXML->Text; 
				 S->ExportFOXML = cbFOXML->Checked; 
				 S->ExportFOXMLTo = txtFOXML->Text; 

				 S->WTWRecentDays = Convert::ToInt32(txtWTWDays->Text);
				 S->StartupTab = cbStartupTab->SelectedIndex;
				 S->NotificationAreaIcon = cbNotificationIcon->Checked;
				 S->SetVideoExtensionsString(txtVideoExtensions->Text);
				 S->SetOtherExtensionsString(txtOtherExtensions->Text);
				 S->ExportRSSMaxDays = Convert::ToInt32(txtExportRSSMaxDays->Text);
				 S->ExportRSSMaxShows =  Convert::ToInt32(txtExportRSSMaxShows->Text);
				 S->KeepTogether = cbKeepTogether->Checked;
				 S->LeadingZeroOnSeason = cbLeadingZero->Checked;
				 S->ShowInTaskbar = chkShowInTaskbar->Checked;
				 S->RenameTxtToSub = cbTxtToSub->Checked;
				 S->ShowEpisodePictures = cbShowEpisodePictures->Checked;
				 S->SpecialsFolderName = txtSpecialsFolderName->Text;

				 S->ForceLowercaseFilenames = cbForceLower->Checked;
				 S->IgnoreSamples = cbIgnoreSamples->Checked;

				 try
				 {
				 S->SampleFileMaxSizeMB = int::Parse(txtMaxSampleSize->Text);
				 }
				 catch (...)
				 {
					 S->SampleFileMaxSizeMB = 50;
				 }

				 try
				 {
				 S->ParallelDownloads = int::Parse(txtParallelDownloads->Text);
				 }
				 catch (...)
				 {
					 S->ParallelDownloads = 4;
				 }

				 if (S->ParallelDownloads < 1)
					 S->ParallelDownloads = 1;
				 else if (S->ParallelDownloads > 8)
					 S->ParallelDownloads = 8;

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
					 else if (R->This == " ") txtRSpace->Text = R->That;
				 }

				 txtMaxSampleSize->Text = S->SampleFileMaxSizeMB.ToString();

				 cbWTWRSS->Checked = S->ExportWTWRSS;
				 txtWTWRSS->Text = S->ExportWTWRSSTo;
				 txtWTWDays->Text = S->WTWRecentDays.ToString();
				 txtExportRSSMaxDays->Text = S->ExportRSSMaxDays.ToString();
				 txtExportRSSMaxShows->Text = S->ExportRSSMaxShows.ToString();

				 cbMissingXML->Checked = S->ExportMissingXML;
				 txtMissingXML->Text = S->ExportMissingXMLTo;
				 cbMissingCSV->Checked = S->ExportMissingCSV;
				 txtMissingCSV->Text = S->ExportMissingCSVTo;

				 cbRenamingXML->Checked = S->ExportRenamingXML;
				 txtRenamingXML->Text = S->ExportRenamingXMLTo;

				 cbFOXML->Checked = S->ExportFOXML;
				 txtFOXML->Text = S->ExportFOXMLTo;

				 cbStartupTab->SelectedIndex = S->StartupTab;
				 cbNotificationIcon->Checked = S->NotificationAreaIcon;
				 txtVideoExtensions->Text = S->GetVideoExtensionsString();
				 txtOtherExtensions->Text = S->GetOtherExtensionsString();
				 
				 cbKeepTogether->Checked = S->KeepTogether;
				 cbKeepTogether_CheckedChanged(nullptr,nullptr);

				 cbLeadingZero->Checked = S->LeadingZeroOnSeason;
				 chkShowInTaskbar->Checked = S->ShowInTaskbar;
				 cbTxtToSub->Checked = S->RenameTxtToSub;
				 cbShowEpisodePictures->Checked = S->ShowEpisodePictures;
				 txtSpecialsFolderName->Text = S->SpecialsFolderName;
				 cbForceLower->Checked = S->ForceLowercaseFilenames;
				 cbIgnoreSamples->Checked = S->IgnoreSamples;

				 txtParallelDownloads->Text = S->ParallelDownloads.ToString();

				 EnableDisable(nullptr,nullptr);
			 }
			 void Browse(TextBox ^txt)
			 {
				 saveFile->FileName = txt->Text;
				 if (saveFile->ShowDialog() == System::Windows::Forms::DialogResult::OK)
					 txt->Text = saveFile->FileName;

			 }
	private: System::Void bnBrowseWTWRSS_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 Browse(txtWTWRSS);
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
	private: System::Void bnBrowseMissingCSV_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 Browse(txtMissingCSV);
			 }
	private: System::Void bnBrowseMissingXML_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 Browse(txtMissingXML);
			 }
	private: System::Void bnBrowseRenamingXML_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 Browse(txtRenamingXML);
			 }
	private: System::Void bnBrowseFOXML_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 Browse(txtFOXML);
			 }
	private: System::Void EnableDisable(System::Object^  sender, System::EventArgs^  e) 
			 {
				 bool wtw = cbWTWRSS->Checked;
				 txtWTWRSS->Enabled = wtw;
				 bnBrowseWTWRSS->Enabled = wtw;
				 label15->Enabled = wtw;
				 label16->Enabled = wtw;
				 label17->Enabled = wtw;
				 txtExportRSSMaxDays->Enabled = wtw;
				 txtExportRSSMaxShows->Enabled = wtw;

				 bool fo = cbFOXML->Checked;
				 txtFOXML->Enabled = fo;
				 bnBrowseFOXML->Enabled = fo;

				 bool ren = cbRenamingXML->Checked;
				 txtRenamingXML->Enabled = ren;
				 bnBrowseRenamingXML->Enabled = ren;

				 bool misx = cbMissingXML->Checked;
				 txtMissingXML->Enabled = misx;
				 bnBrowseMissingXML->Enabled = misx;

				 bool misc = cbMissingCSV->Checked;
				 txtMissingCSV->Enabled = misc;
				 bnBrowseMissingCSV->Enabled = misc;
			 }

};
}
