#pragma once

#include "RenameItem.h"
#include "MissingEpisode.h"
#include "bt.h"
#include "TVDoc.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

using namespace System::IO;

namespace TVRename {

	/// <summary>
	/// Summary for uTorrent
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class uTorrent : public System::Windows::Forms::Form
	{
	private: 
		SetProgressDelegate ^SetProg;

	private: System::Windows::Forms::Button^  bnClose;
	private: System::Windows::Forms::Label^  lbDPMatch;
	private: System::Windows::Forms::Label^  lbDPMissing;
	private: System::IO::FileSystemWatcher^  watcher;


			 TVDoc ^mDoc;


	public:
		uTorrent(TVDoc ^doc, SetProgressDelegate ^progdel)
		{
			mDoc = doc;
			SetProg = progdel;

			InitializeComponent();
			
			watcher->Error += gcnew ErrorEventHandler(this, &uTorrent::WatcherError);


			cbUTMatchMissing->Enabled = mDoc->MissingEpisodes->Count > 0;
			EnableDisable();

			FileInfo ^f = gcnew FileInfo(System::Windows::Forms::Application::UserAppDataPath+"\\..\\..\\..\\uTorrent\\resume.dat");
			if (f->Exists)
			{
				txtUTresumedatFolder->Text = f->Directory->FullName;
				bnUTRefresh_Click(nullptr,nullptr);
			}
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~uTorrent()
		{
			if (components)
			{
				delete components;
			}
		}

	private: System::Windows::Forms::Button^  bnUTHelp;
	private: System::Windows::Forms::CheckBox^  cbUTSetPrio;
	private: System::Windows::Forms::CheckBox^  cbUTMatchMissing;
	private: System::Windows::Forms::CheckBox^  cbUTUseHashing;
	private: System::Windows::Forms::CheckBox^  chkUTTest;
	private: System::Windows::Forms::CheckBox^  chkUTSearchSubfolders;
	private: System::Windows::Forms::Button^  bnUTNone;
	private: System::Windows::Forms::Button^  bnUTGo;
	private: System::Windows::Forms::Button^  bnUTRefresh;
	private: System::Windows::Forms::Button^  bnUTAll;
	private: System::Windows::Forms::Button^  bnUTBrowseSearchFolder;
	private: System::Windows::Forms::Button^  bnUTBrowseResumeDat;
	private: System::Windows::Forms::TextBox^  txtUTSearchFolder;
	private: System::Windows::Forms::Label^  label15;
	private: System::Windows::Forms::Label^  label13;
	private: System::Windows::Forms::TextBox^  txtUTresumedatFolder;
	private: System::Windows::Forms::Label^  label12;
	private: System::Windows::Forms::Label^  label11;
	private: System::Windows::Forms::CheckedListBox^  lbUTTorrents;
	private: System::Windows::Forms::ListView^  lvUTResults;
	private: System::Windows::Forms::ColumnHeader^  columnHeader50;
	private: System::Windows::Forms::ColumnHeader^  columnHeader48;
	private: System::Windows::Forms::ColumnHeader^  columnHeader49;
	private: System::Windows::Forms::ColumnHeader^  columnHeader51;
	private: System::Windows::Forms::ColumnHeader^  columnHeader52;

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
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(uTorrent::typeid));
			this->bnUTBrowseResumeDat = (gcnew System::Windows::Forms::Button());
			this->bnUTBrowseSearchFolder = (gcnew System::Windows::Forms::Button());
			this->bnUTAll = (gcnew System::Windows::Forms::Button());
			this->bnUTRefresh = (gcnew System::Windows::Forms::Button());
			this->bnUTGo = (gcnew System::Windows::Forms::Button());
			this->bnUTNone = (gcnew System::Windows::Forms::Button());
			this->chkUTSearchSubfolders = (gcnew System::Windows::Forms::CheckBox());
			this->chkUTTest = (gcnew System::Windows::Forms::CheckBox());
			this->cbUTUseHashing = (gcnew System::Windows::Forms::CheckBox());
			this->cbUTMatchMissing = (gcnew System::Windows::Forms::CheckBox());
			this->cbUTSetPrio = (gcnew System::Windows::Forms::CheckBox());
			this->bnUTHelp = (gcnew System::Windows::Forms::Button());
			this->lbUTTorrents = (gcnew System::Windows::Forms::CheckedListBox());
			this->label11 = (gcnew System::Windows::Forms::Label());
			this->label12 = (gcnew System::Windows::Forms::Label());
			this->txtUTresumedatFolder = (gcnew System::Windows::Forms::TextBox());
			this->label13 = (gcnew System::Windows::Forms::Label());
			this->label15 = (gcnew System::Windows::Forms::Label());
			this->txtUTSearchFolder = (gcnew System::Windows::Forms::TextBox());
			this->columnHeader50 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader48 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader49 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader51 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader52 = (gcnew System::Windows::Forms::ColumnHeader());
			this->lvUTResults = (gcnew System::Windows::Forms::ListView());
			this->bnClose = (gcnew System::Windows::Forms::Button());
			this->lbDPMatch = (gcnew System::Windows::Forms::Label());
			this->lbDPMissing = (gcnew System::Windows::Forms::Label());
			this->watcher = (gcnew System::IO::FileSystemWatcher());
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->watcher))->BeginInit();
			this->SuspendLayout();
			// 
			// bnUTBrowseResumeDat
			// 
			this->bnUTBrowseResumeDat->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->bnUTBrowseResumeDat->Location = System::Drawing::Point(735, 3);
			this->bnUTBrowseResumeDat->Name = L"bnUTBrowseResumeDat";
			this->bnUTBrowseResumeDat->Size = System::Drawing::Size(75, 23);
			this->bnUTBrowseResumeDat->TabIndex = 2;
			this->bnUTBrowseResumeDat->Text = L"&Browse...";
			this->bnUTBrowseResumeDat->UseVisualStyleBackColor = true;
			// 
			// bnUTBrowseSearchFolder
			// 
			this->bnUTBrowseSearchFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->bnUTBrowseSearchFolder->Location = System::Drawing::Point(735, 284);
			this->bnUTBrowseSearchFolder->Name = L"bnUTBrowseSearchFolder";
			this->bnUTBrowseSearchFolder->Size = System::Drawing::Size(75, 23);
			this->bnUTBrowseSearchFolder->TabIndex = 10;
			this->bnUTBrowseSearchFolder->Text = L"B&rowse...";
			this->bnUTBrowseSearchFolder->UseVisualStyleBackColor = true;
			// 
			// bnUTAll
			// 
			this->bnUTAll->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->bnUTAll->Location = System::Drawing::Point(816, 32);
			this->bnUTAll->Name = L"bnUTAll";
			this->bnUTAll->Size = System::Drawing::Size(75, 23);
			this->bnUTAll->TabIndex = 6;
			this->bnUTAll->Text = L"&All";
			this->bnUTAll->UseVisualStyleBackColor = true;
			this->bnUTAll->Click += gcnew System::EventHandler(this, &uTorrent::bnUTAll_Click);
			// 
			// bnUTRefresh
			// 
			this->bnUTRefresh->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->bnUTRefresh->Location = System::Drawing::Point(816, 3);
			this->bnUTRefresh->Name = L"bnUTRefresh";
			this->bnUTRefresh->Size = System::Drawing::Size(75, 23);
			this->bnUTRefresh->TabIndex = 3;
			this->bnUTRefresh->Text = L"Refres&h";
			this->bnUTRefresh->UseVisualStyleBackColor = true;
			this->bnUTRefresh->Click += gcnew System::EventHandler(this, &uTorrent::bnUTRefresh_Click);
			// 
			// bnUTGo
			// 
			this->bnUTGo->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnUTGo->Location = System::Drawing::Point(123, 588);
			this->bnUTGo->Name = L"bnUTGo";
			this->bnUTGo->Size = System::Drawing::Size(75, 23);
			this->bnUTGo->TabIndex = 11;
			this->bnUTGo->Text = L"&Go";
			this->bnUTGo->UseVisualStyleBackColor = true;
			this->bnUTGo->Click += gcnew System::EventHandler(this, &uTorrent::bnUTGo_Click);
			// 
			// bnUTNone
			// 
			this->bnUTNone->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->bnUTNone->Location = System::Drawing::Point(816, 61);
			this->bnUTNone->Name = L"bnUTNone";
			this->bnUTNone->Size = System::Drawing::Size(75, 23);
			this->bnUTNone->TabIndex = 7;
			this->bnUTNone->Text = L"&None";
			this->bnUTNone->UseVisualStyleBackColor = true;
			this->bnUTNone->Click += gcnew System::EventHandler(this, &uTorrent::bnUTNone_Click);
			// 
			// chkUTSearchSubfolders
			// 
			this->chkUTSearchSubfolders->AutoSize = true;
			this->chkUTSearchSubfolders->Location = System::Drawing::Point(142, 359);
			this->chkUTSearchSubfolders->Name = L"chkUTSearchSubfolders";
			this->chkUTSearchSubfolders->Size = System::Drawing::Size(132, 17);
			this->chkUTSearchSubfolders->TabIndex = 14;
			this->chkUTSearchSubfolders->Text = L"S&earch subfolders, too";
			this->chkUTSearchSubfolders->UseVisualStyleBackColor = true;
			// 
			// chkUTTest
			// 
			this->chkUTTest->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->chkUTTest->AutoSize = true;
			this->chkUTTest->Checked = true;
			this->chkUTTest->CheckState = System::Windows::Forms::CheckState::Checked;
			this->chkUTTest->Location = System::Drawing::Point(204, 592);
			this->chkUTTest->Name = L"chkUTTest";
			this->chkUTTest->Size = System::Drawing::Size(70, 17);
			this->chkUTTest->TabIndex = 12;
			this->chkUTTest->Text = L"Te&st Run";
			this->chkUTTest->UseVisualStyleBackColor = true;
			// 
			// cbUTUseHashing
			// 
			this->cbUTUseHashing->AutoSize = true;
			this->cbUTUseHashing->Location = System::Drawing::Point(123, 336);
			this->cbUTUseHashing->Name = L"cbUTUseHashing";
			this->cbUTUseHashing->Size = System::Drawing::Size(189, 17);
			this->cbUTUseHashing->TabIndex = 13;
			this->cbUTUseHashing->Text = L"Match against files in search folder";
			this->cbUTUseHashing->UseVisualStyleBackColor = true;
			this->cbUTUseHashing->CheckedChanged += gcnew System::EventHandler(this, &uTorrent::cbUTUseHashing_CheckedChanged);
			// 
			// cbUTMatchMissing
			// 
			this->cbUTMatchMissing->AutoSize = true;
			this->cbUTMatchMissing->Enabled = false;
			this->cbUTMatchMissing->Location = System::Drawing::Point(123, 382);
			this->cbUTMatchMissing->Name = L"cbUTMatchMissing";
			this->cbUTMatchMissing->Size = System::Drawing::Size(185, 17);
			this->cbUTMatchMissing->TabIndex = 15;
			this->cbUTMatchMissing->Text = L"Match against missing episode list";
			this->cbUTMatchMissing->UseVisualStyleBackColor = true;
			this->cbUTMatchMissing->CheckedChanged += gcnew System::EventHandler(this, &uTorrent::cbUTMatchMissing_CheckedChanged);
			// 
			// cbUTSetPrio
			// 
			this->cbUTSetPrio->AutoSize = true;
			this->cbUTSetPrio->Location = System::Drawing::Point(364, 314);
			this->cbUTSetPrio->Name = L"cbUTSetPrio";
			this->cbUTSetPrio->Size = System::Drawing::Size(132, 17);
			this->cbUTSetPrio->TabIndex = 16;
			this->cbUTSetPrio->Text = L"Set download priorities";
			this->cbUTSetPrio->UseVisualStyleBackColor = true;
			this->cbUTSetPrio->CheckedChanged += gcnew System::EventHandler(this, &uTorrent::cbUTSetPrio_CheckedChanged);
			// 
			// bnUTHelp
			// 
			this->bnUTHelp->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnUTHelp->Location = System::Drawing::Point(816, 559);
			this->bnUTHelp->Name = L"bnUTHelp";
			this->bnUTHelp->Size = System::Drawing::Size(75, 23);
			this->bnUTHelp->TabIndex = 19;
			this->bnUTHelp->Text = L"Help";
			this->bnUTHelp->UseVisualStyleBackColor = true;
			this->bnUTHelp->Click += gcnew System::EventHandler(this, &uTorrent::bnUTHelp_Click);
			// 
			// lbUTTorrents
			// 
			this->lbUTTorrents->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lbUTTorrents->CheckOnClick = true;
			this->lbUTTorrents->FormattingEnabled = true;
			this->lbUTTorrents->IntegralHeight = false;
			this->lbUTTorrents->Location = System::Drawing::Point(123, 33);
			this->lbUTTorrents->Name = L"lbUTTorrents";
			this->lbUTTorrents->ScrollAlwaysVisible = true;
			this->lbUTTorrents->Size = System::Drawing::Size(687, 244);
			this->lbUTTorrents->TabIndex = 5;
			// 
			// label11
			// 
			this->label11->AutoSize = true;
			this->label11->Location = System::Drawing::Point(33, 33);
			this->label11->Name = L"label11";
			this->label11->Size = System::Drawing::Size(84, 13);
			this->label11->TabIndex = 4;
			this->label11->Text = L"Choose &torrents:";
			// 
			// label12
			// 
			this->label12->AutoSize = true;
			this->label12->Location = System::Drawing::Point(12, 9);
			this->label12->Name = L"label12";
			this->label12->Size = System::Drawing::Size(105, 13);
			this->label12->TabIndex = 0;
			this->label12->Text = L"µTorrent resume.&dat:";
			// 
			// txtUTresumedatFolder
			// 
			this->txtUTresumedatFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->txtUTresumedatFolder->Location = System::Drawing::Point(123, 6);
			this->txtUTresumedatFolder->Name = L"txtUTresumedatFolder";
			this->txtUTresumedatFolder->Size = System::Drawing::Size(606, 20);
			this->txtUTresumedatFolder->TabIndex = 1;
			this->txtUTresumedatFolder->TextChanged += gcnew System::EventHandler(this, &uTorrent::txtUTresumedatFolder_TextChanged);
			// 
			// label13
			// 
			this->label13->AutoSize = true;
			this->label13->Location = System::Drawing::Point(41, 289);
			this->label13->Name = L"label13";
			this->label13->Size = System::Drawing::Size(76, 13);
			this->label13->TabIndex = 8;
			this->label13->Text = L"Sear&ch Folder:";
			// 
			// label15
			// 
			this->label15->AutoSize = true;
			this->label15->Location = System::Drawing::Point(77, 410);
			this->label15->Name = L"label15";
			this->label15->Size = System::Drawing::Size(40, 13);
			this->label15->TabIndex = 17;
			this->label15->Text = L"Status:";
			// 
			// txtUTSearchFolder
			// 
			this->txtUTSearchFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->txtUTSearchFolder->Location = System::Drawing::Point(123, 286);
			this->txtUTSearchFolder->Name = L"txtUTSearchFolder";
			this->txtUTSearchFolder->Size = System::Drawing::Size(606, 20);
			this->txtUTSearchFolder->TabIndex = 9;
			// 
			// columnHeader50
			// 
			this->columnHeader50->Text = L"Reason";
			// 
			// columnHeader48
			// 
			this->columnHeader48->Text = L"Torrent";
			this->columnHeader48->Width = 230;
			// 
			// columnHeader49
			// 
			this->columnHeader49->Text = L"#";
			this->columnHeader49->Width = 28;
			// 
			// columnHeader51
			// 
			this->columnHeader51->Text = L"Priority";
			// 
			// columnHeader52
			// 
			this->columnHeader52->Text = L"Location";
			this->columnHeader52->Width = 280;
			// 
			// lvUTResults
			// 
			this->lvUTResults->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvUTResults->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(5) {this->columnHeader50, 
				this->columnHeader48, this->columnHeader49, this->columnHeader51, this->columnHeader52});
			this->lvUTResults->FullRowSelect = true;
			this->lvUTResults->HeaderStyle = System::Windows::Forms::ColumnHeaderStyle::Nonclickable;
			this->lvUTResults->Location = System::Drawing::Point(123, 410);
			this->lvUTResults->Name = L"lvUTResults";
			this->lvUTResults->ShowItemToolTips = true;
			this->lvUTResults->Size = System::Drawing::Size(686, 172);
			this->lvUTResults->TabIndex = 18;
			this->lvUTResults->UseCompatibleStateImageBehavior = false;
			this->lvUTResults->View = System::Windows::Forms::View::Details;
			// 
			// bnClose
			// 
			this->bnClose->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnClose->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnClose->Location = System::Drawing::Point(816, 588);
			this->bnClose->Name = L"bnClose";
			this->bnClose->Size = System::Drawing::Size(75, 23);
			this->bnClose->TabIndex = 20;
			this->bnClose->Text = L"Close";
			this->bnClose->UseVisualStyleBackColor = true;
			this->bnClose->Click += gcnew System::EventHandler(this, &uTorrent::bnClose_Click);
			// 
			// lbDPMatch
			// 
			this->lbDPMatch->AutoSize = true;
			this->lbDPMatch->Location = System::Drawing::Point(361, 337);
			this->lbDPMatch->Name = L"lbDPMatch";
			this->lbDPMatch->Size = System::Drawing::Size(155, 13);
			this->lbDPMatch->TabIndex = 8;
			this->lbDPMatch->Text = L"Enable matched, disable others";
			// 
			// lbDPMissing
			// 
			this->lbDPMissing->AutoSize = true;
			this->lbDPMissing->Location = System::Drawing::Point(361, 383);
			this->lbDPMissing->Name = L"lbDPMissing";
			this->lbDPMissing->Size = System::Drawing::Size(328, 13);
			this->lbDPMissing->TabIndex = 8;
			this->lbDPMissing->Text = L"Enable missing episodes, disable others, unless also matched above";
			// 
			// watcher
			// 
			this->watcher->EnableRaisingEvents = true;
			this->watcher->Filter = L"resume.dat";
			this->watcher->NotifyFilter = static_cast<System::IO::NotifyFilters>(((System::IO::NotifyFilters::LastWrite | System::IO::NotifyFilters::LastAccess) 
				| System::IO::NotifyFilters::CreationTime));
			this->watcher->SynchronizingObject = this;
			this->watcher->Created += gcnew System::IO::FileSystemEventHandler(this, &uTorrent::watcher_Created);
			this->watcher->Changed += gcnew System::IO::FileSystemEventHandler(this, &uTorrent::watcher_Changed);
			// 
			// uTorrent
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnClose;
			this->ClientSize = System::Drawing::Size(903, 623);
			this->Controls->Add(this->bnClose);
			this->Controls->Add(this->lvUTResults);
			this->Controls->Add(this->bnUTHelp);
			this->Controls->Add(this->cbUTSetPrio);
			this->Controls->Add(this->cbUTMatchMissing);
			this->Controls->Add(this->cbUTUseHashing);
			this->Controls->Add(this->chkUTTest);
			this->Controls->Add(this->chkUTSearchSubfolders);
			this->Controls->Add(this->bnUTNone);
			this->Controls->Add(this->bnUTGo);
			this->Controls->Add(this->bnUTRefresh);
			this->Controls->Add(this->bnUTAll);
			this->Controls->Add(this->bnUTBrowseSearchFolder);
			this->Controls->Add(this->bnUTBrowseResumeDat);
			this->Controls->Add(this->txtUTSearchFolder);
			this->Controls->Add(this->label15);
			this->Controls->Add(this->lbDPMissing);
			this->Controls->Add(this->lbDPMatch);
			this->Controls->Add(this->label13);
			this->Controls->Add(this->txtUTresumedatFolder);
			this->Controls->Add(this->label12);
			this->Controls->Add(this->label11);
			this->Controls->Add(this->lbUTTorrents);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"uTorrent";
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = L"TVRename - µTorrent";
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->watcher))->EndInit();
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

	private: System::Void bnUTRefresh_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 RefreshResumeDat();
			 }
			 void UTSelectNone()
			 {
				 for (int i=0;i<lbUTTorrents->Items->Count;i++)
					 lbUTTorrents->SetItemChecked(i, false);
			 }
			 void UTSelectAll()
			 {
				 for (int i=0;i<lbUTTorrents->Items->Count;i++)
					 lbUTTorrents->SetItemChecked(i, true);
			 }
	private: System::Void bnUTAll_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 UTSelectAll();
			 }
	private: System::Void bnUTNone_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 UTSelectNone();
			 }
			 void AddUTStatusLine(String ^s)
			 {
				 //txtUTStatus->Text += "\r\n"+s;
				 //txtUTStatus->SelectionStart = txtUTStatus->Text->Length;
				 //txtUTStatus->ScrollToCaret();
				 //txtUTStatus->Update();
			 }
			 void RefreshResumeDat()
			 {
				 Generic::List<String ^> ^checkedItems = gcnew Generic::List<String ^>;
				 for each (String ^torrent in lbUTTorrents->CheckedItems)
					 checkedItems->Add(torrent);

				 lbUTTorrents->Items->Clear();
				 // open resume.dat file, fill checked list box with torrents available to choose from
				 String ^file = txtUTresumedatFolder->Text+"\\resume.dat";
				 if (!File::Exists(file))
				 {
					 AddUTStatusLine(file+" does not exist.");
					 return;
				 }
				 BTFile ^resumeDat = BEncodeLoader::Load(file);
				 if (resumeDat == nullptr)
				 {
					 AddUTStatusLine("Error loading "+file);
					 return;
				 }
				 AddUTStatusLine("Loaded resume.dat");
				 BTDictionary ^dict = resumeDat->GetDict();
				 for (int i=0;i<dict->Items->Count;i++)
				 {
					 BTItem ^it = dict->Items[i];
					 if (it->Type == kDictionaryItem)
					 {
						 BTDictionaryItem ^d2 = safe_cast<BTDictionaryItem ^>(it);
						 if ((d2->Key != ".fileguard") && (d2->Data->Type == kDictionary))
							 lbUTTorrents->Items->Add(d2->Key);
					 }
				 }
				 AddUTStatusLine("Found "+lbUTTorrents->Items->Count.ToString()+" torrents");

				 for each (String ^torrent in checkedItems)
					 for (int i=0;i<lbUTTorrents->Items->Count;i++)
						 if (lbUTTorrents->Items[i]->ToString() == torrent)
							 lbUTTorrents->SetItemChecked(i, true);
			 }
	private: System::Void bnUTGo_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 String ^searchFolder = txtUTSearchFolder->Text;
				 String ^resumeDatFolder = txtUTresumedatFolder->Text;
				 String ^resumeDatFile = resumeDatFolder + "\\resume.dat";
				 bool testMode = chkUTTest->Checked;

				 if (!File::Exists(resumeDatFile))
				 {
					 AddUTStatusLine("couldn't find "+resumeDatFile);
					 return;
				 }
				 AddUTStatusLine("");
				 if (testMode)
					 AddUTStatusLine("******** Test mode : No changes will be made to resume.dat ********");
				 else
					 if (!CheckUTorrentClosed())
						 return;

				 int action = actNone;
				 if (chkUTSearchSubfolders->Checked)
					 action |=  actSearchSubfolders;
				 if (cbUTUseHashing->Checked)
					 action |= actHashSearch;
				 if (cbUTSetPrio->Checked)
					 action |= actSetPrios;
				 if (testMode)
					 action |= actTestMode;
				 if (cbUTMatchMissing->Checked)
					 action |= actMatchMissing;

				 if ( (action & (actRename | actCopy | actMatchMissing | actHashSearch)) == 0 )
					 return;

				 lvUTResults->Items->Clear();
				 AddUTStatusLine("Using "+resumeDatFile);

				 BTProcessor ^btp = gcnew BTProcessor();

				 for each (String ^torrent in lbUTTorrents->CheckedItems)
				 {
					 AddUTStatusLine("");
					 AddUTStatusLine("Processing "+ torrent);
					 btp->Setup(torrent, searchFolder, nullptr, lvUTResults, nullptr, SetProg,
						 action, resumeDatFolder, mDoc->MissingEpisodes, mDoc->Settings->FNPRegexs);
					 btp->Go();
				 }
				 AddUTStatusLine(btp->CacheStats());
				 if (!testMode)
					 RestartUTorrent();
				 else
					 AddUTStatusLine("******** Test mode : No changes will be made to resume.dat ********");
			 }
	private: System::Void cbUTUseHashing_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 EnableDisable();
			 }
			 void EnableDisable()
			 {
				 bool en = cbUTUseHashing->Checked;
				 txtUTSearchFolder->Enabled = en;
				 bnUTBrowseSearchFolder->Enabled = en;
				 chkUTSearchSubfolders->Enabled = en;

				 lbDPMatch->Enabled = cbUTSetPrio->Checked && cbUTUseHashing->Checked;
				 lbDPMissing->Enabled = cbUTSetPrio->Checked && cbUTMatchMissing->Checked;

			 }
	private: System::Void bnUTHelp_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 MessageBox::Show("- Select the folder containing µTorrent's \"resume.dat\" file.\r\n" \
					 "- Select the torrents you want to operate on.\r\n" \
					 "- \"Use Hashes\" will search through your media collection to find files that match those inside the torrents.\r\n" \
					 "  - Choose the folder that has the completed files in it (or the root of a tree to search).\r\n" \
					 "  - \"Search subfolders\" will do a recursive search from the search folder.\r\n" \
					 "  - \"Set download priority\" will set missing files to \"Skip\", and found files to \"Normal\",\r\n" \
					 "- \"Match missing\" will compare files in the rorrents, to what it currently in your \"Missing List\"\r\n" \
					 "  - \"Set download priority\" will set missing files to download, and others off.\r\n" \
					 "- Turn on \"Test\" to show what would happen, but not make any changes to resume.dat.\r\n" \
					 "- Click \"Go\".\r\n" \
					 "- If not in test mode, quit and restart µTorrent when instructed.\r\n" \
					 "- You will need to do a \"Force Check\" in µTorrent on any torrents that now have completed files.",
					 "TVRename µTorrent",
					 MessageBoxButtons::OK);
			 }

			 bool CheckUTorrentClosed()
			 {
				 ::DialogResult dr = MessageBox::Show("Make sure µTorrent is not running, then click OK.","TVRename",MessageBoxButtons::OKCancel,MessageBoxIcon::Warning);
				 return (dr == ::DialogResult::OK);
			 }

			 bool RestartUTorrent()
			 {
				 ::DialogResult dr = MessageBox::Show("You may now restart µTorrent.","TVRename",MessageBoxButtons::OK,MessageBoxIcon::Warning);
				 return true;
			 }


	private: System::Void bnClose_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 this->Close();
			 }
	private: System::Void cbUTMatchMissing_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 EnableDisable();
			 }
	private: System::Void cbUTSetPrio_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 EnableDisable();
			 }
	private: System::Void watcher_Changed(System::Object^  sender, System::IO::FileSystemEventArgs^  e) 
			 {
				 RefreshResumeDat();
			 }
			 void WatcherError(System::Object^, System::IO::ErrorEventArgs^)
			 {
				 while (!watcher->EnableRaisingEvents)
				 {
					 try 
					 {
						 StartWatching();
						 RefreshResumeDat();
					 }
					 catch (...)
					 {
						 Threading::Thread::Sleep(500);
					 }
				 }
			 }

	private: System::Void txtUTresumedatFolder_TextChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 StartWatching();
			 }
			 void StartWatching()
			 {
				 String ^p = txtUTresumedatFolder->Text;
				 bool en = DirectoryInfo(p).Exists;
				 if (en)
				 {
					 watcher->Path = p;
					 watcher->Filter = "resume.dat";
				 }
				 watcher->EnableRaisingEvents = en;
			 }
	private: System::Void watcher_Created(System::Object^  sender, System::IO::FileSystemEventArgs^  e) 
			 {
				 RefreshResumeDat();
			 }
};
}
