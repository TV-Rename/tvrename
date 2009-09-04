#pragma once

#include "TVDoc.h"
#include "BT.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace TVRename {

	/// <summary>
	/// Summary for TorrentMatch
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class TorrentMatch : public System::Windows::Forms::Form
	{
		TVDoc ^mDoc;
	private: System::Windows::Forms::OpenFileDialog^  openFile;
			 SetProgressDelegate ^SetProgress;

	public:
		TorrentMatch(TVDoc ^doc, SetProgressDelegate ^prog)
		{
			SetProgress = prog;
			mDoc = doc;

			InitializeComponent();

			TabBTEnableDisable();
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~TorrentMatch()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::RadioButton^  rbBTRenameFiles;
	protected: 
	private: System::Windows::Forms::RadioButton^  rbBTCopyTo;
	private: System::Windows::Forms::Button^  bnBTSecondOpen;
	private: System::Windows::Forms::Button^  bnBTOpenFolder;
	private: System::Windows::Forms::TreeView^  tmatchTree;
	private: System::Windows::Forms::Button^  bnGo;
	private: System::Windows::Forms::Button^  bnBTSecondBrowse;
	private: System::Windows::Forms::Button^  bnBrowseFolder;
	private: System::Windows::Forms::TextBox^  txtBTSecondLocation;
	private: System::Windows::Forms::TextBox^  txtFolder;
	private: System::Windows::Forms::Button^  bnBrowseTorrent;
	private: System::Windows::Forms::Label^  label14;
	private: System::Windows::Forms::Label^  label3;
	private: System::Windows::Forms::TextBox^  txtTorrentFile;
	private: System::Windows::Forms::Label^  label4;
	private: System::Windows::Forms::Button^  bnClose;
	private: System::Windows::Forms::FolderBrowserDialog^  folderBrowser;

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
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(TorrentMatch::typeid));
			this->rbBTRenameFiles = (gcnew System::Windows::Forms::RadioButton());
			this->rbBTCopyTo = (gcnew System::Windows::Forms::RadioButton());
			this->bnBTSecondOpen = (gcnew System::Windows::Forms::Button());
			this->bnBTOpenFolder = (gcnew System::Windows::Forms::Button());
			this->tmatchTree = (gcnew System::Windows::Forms::TreeView());
			this->bnGo = (gcnew System::Windows::Forms::Button());
			this->bnBTSecondBrowse = (gcnew System::Windows::Forms::Button());
			this->bnBrowseFolder = (gcnew System::Windows::Forms::Button());
			this->txtBTSecondLocation = (gcnew System::Windows::Forms::TextBox());
			this->txtFolder = (gcnew System::Windows::Forms::TextBox());
			this->bnBrowseTorrent = (gcnew System::Windows::Forms::Button());
			this->label14 = (gcnew System::Windows::Forms::Label());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->txtTorrentFile = (gcnew System::Windows::Forms::TextBox());
			this->label4 = (gcnew System::Windows::Forms::Label());
			this->bnClose = (gcnew System::Windows::Forms::Button());
			this->folderBrowser = (gcnew System::Windows::Forms::FolderBrowserDialog());
			this->openFile = (gcnew System::Windows::Forms::OpenFileDialog());
			this->SuspendLayout();
			// 
			// rbBTRenameFiles
			// 
			this->rbBTRenameFiles->AutoSize = true;
			this->rbBTRenameFiles->Checked = true;
			this->rbBTRenameFiles->Location = System::Drawing::Point(113, 67);
			this->rbBTRenameFiles->Name = L"rbBTRenameFiles";
			this->rbBTRenameFiles->Size = System::Drawing::Size(86, 17);
			this->rbBTRenameFiles->TabIndex = 24;
			this->rbBTRenameFiles->TabStop = true;
			this->rbBTRenameFiles->Text = L"Rename files";
			this->rbBTRenameFiles->UseVisualStyleBackColor = true;
			this->rbBTRenameFiles->CheckedChanged += gcnew System::EventHandler(this, &TorrentMatch::rbBTRenameFiles_CheckedChanged);
			// 
			// rbBTCopyTo
			// 
			this->rbBTCopyTo->AutoSize = true;
			this->rbBTCopyTo->Location = System::Drawing::Point(205, 67);
			this->rbBTCopyTo->Name = L"rbBTCopyTo";
			this->rbBTCopyTo->Size = System::Drawing::Size(65, 17);
			this->rbBTCopyTo->TabIndex = 25;
			this->rbBTCopyTo->Text = L"Copy To";
			this->rbBTCopyTo->UseVisualStyleBackColor = true;
			this->rbBTCopyTo->CheckedChanged += gcnew System::EventHandler(this, &TorrentMatch::rbBTCopyTo_CheckedChanged);
			// 
			// bnBTSecondOpen
			// 
			this->bnBTSecondOpen->Location = System::Drawing::Point(502, 91);
			this->bnBTSecondOpen->Name = L"bnBTSecondOpen";
			this->bnBTSecondOpen->Size = System::Drawing::Size(75, 23);
			this->bnBTSecondOpen->TabIndex = 21;
			this->bnBTSecondOpen->Text = L"&Open";
			this->bnBTSecondOpen->UseVisualStyleBackColor = true;
			this->bnBTSecondOpen->Click += gcnew System::EventHandler(this, &TorrentMatch::bnBTSecondOpen_Click);
			// 
			// bnBTOpenFolder
			// 
			this->bnBTOpenFolder->Location = System::Drawing::Point(502, 39);
			this->bnBTOpenFolder->Name = L"bnBTOpenFolder";
			this->bnBTOpenFolder->Size = System::Drawing::Size(75, 23);
			this->bnBTOpenFolder->TabIndex = 20;
			this->bnBTOpenFolder->Text = L"&Open";
			this->bnBTOpenFolder->UseVisualStyleBackColor = true;
			this->bnBTOpenFolder->Click += gcnew System::EventHandler(this, &TorrentMatch::bnBTOpenFolder_Click);
			// 
			// tmatchTree
			// 
			this->tmatchTree->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->tmatchTree->Location = System::Drawing::Point(14, 148);
			this->tmatchTree->Name = L"tmatchTree";
			this->tmatchTree->Size = System::Drawing::Size(565, 220);
			this->tmatchTree->TabIndex = 23;
			// 
			// bnGo
			// 
			this->bnGo->Location = System::Drawing::Point(113, 119);
			this->bnGo->Name = L"bnGo";
			this->bnGo->Size = System::Drawing::Size(75, 23);
			this->bnGo->TabIndex = 22;
			this->bnGo->Text = L"&Go";
			this->bnGo->UseVisualStyleBackColor = true;
			this->bnGo->Click += gcnew System::EventHandler(this, &TorrentMatch::bnGo_Click);
			// 
			// bnBTSecondBrowse
			// 
			this->bnBTSecondBrowse->Location = System::Drawing::Point(421, 91);
			this->bnBTSecondBrowse->Name = L"bnBTSecondBrowse";
			this->bnBTSecondBrowse->Size = System::Drawing::Size(75, 23);
			this->bnBTSecondBrowse->TabIndex = 19;
			this->bnBTSecondBrowse->Text = L"B&rowse...";
			this->bnBTSecondBrowse->UseVisualStyleBackColor = true;
			this->bnBTSecondBrowse->Click += gcnew System::EventHandler(this, &TorrentMatch::bnBTSecondBrowse_Click);
			// 
			// bnBrowseFolder
			// 
			this->bnBrowseFolder->Location = System::Drawing::Point(421, 39);
			this->bnBrowseFolder->Name = L"bnBrowseFolder";
			this->bnBrowseFolder->Size = System::Drawing::Size(75, 23);
			this->bnBrowseFolder->TabIndex = 18;
			this->bnBrowseFolder->Text = L"B&rowse...";
			this->bnBrowseFolder->UseVisualStyleBackColor = true;
			this->bnBrowseFolder->Click += gcnew System::EventHandler(this, &TorrentMatch::bnBrowseFolder_Click);
			// 
			// txtBTSecondLocation
			// 
			this->txtBTSecondLocation->Location = System::Drawing::Point(113, 93);
			this->txtBTSecondLocation->Name = L"txtBTSecondLocation";
			this->txtBTSecondLocation->Size = System::Drawing::Size(296, 20);
			this->txtBTSecondLocation->TabIndex = 16;
			// 
			// txtFolder
			// 
			this->txtFolder->Location = System::Drawing::Point(113, 41);
			this->txtFolder->Name = L"txtFolder";
			this->txtFolder->Size = System::Drawing::Size(296, 20);
			this->txtFolder->TabIndex = 17;
			// 
			// bnBrowseTorrent
			// 
			this->bnBrowseTorrent->Location = System::Drawing::Point(421, 10);
			this->bnBrowseTorrent->Name = L"bnBrowseTorrent";
			this->bnBrowseTorrent->Size = System::Drawing::Size(75, 23);
			this->bnBrowseTorrent->TabIndex = 13;
			this->bnBrowseTorrent->Text = L"&Browse...";
			this->bnBrowseTorrent->UseVisualStyleBackColor = true;
			this->bnBrowseTorrent->Click += gcnew System::EventHandler(this, &TorrentMatch::bnBrowseTorrent_Click);
			// 
			// label14
			// 
			this->label14->AutoSize = true;
			this->label14->Location = System::Drawing::Point(63, 69);
			this->label14->Name = L"label14";
			this->label14->Size = System::Drawing::Size(40, 13);
			this->label14->TabIndex = 14;
			this->label14->Text = L"Action:";
			this->label14->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Location = System::Drawing::Point(63, 44);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(39, 13);
			this->label3->TabIndex = 15;
			this->label3->Text = L"&Folder:";
			this->label3->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// txtTorrentFile
			// 
			this->txtTorrentFile->Location = System::Drawing::Point(113, 12);
			this->txtTorrentFile->Name = L"txtTorrentFile";
			this->txtTorrentFile->Size = System::Drawing::Size(296, 20);
			this->txtTorrentFile->TabIndex = 12;
			// 
			// label4
			// 
			this->label4->AutoSize = true;
			this->label4->Location = System::Drawing::Point(43, 15);
			this->label4->Name = L"label4";
			this->label4->Size = System::Drawing::Size(59, 13);
			this->label4->TabIndex = 11;
			this->label4->Text = L".&torrent file:";
			this->label4->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// bnClose
			// 
			this->bnClose->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnClose->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnClose->Location = System::Drawing::Point(504, 374);
			this->bnClose->Name = L"bnClose";
			this->bnClose->Size = System::Drawing::Size(75, 23);
			this->bnClose->TabIndex = 26;
			this->bnClose->Text = L"Close";
			this->bnClose->UseVisualStyleBackColor = true;
			this->bnClose->Click += gcnew System::EventHandler(this, &TorrentMatch::bnClose_Click);
			// 
			// folderBrowser
			// 
			this->folderBrowser->ShowNewFolderButton = false;
			// 
			// openFile
			// 
			this->openFile->Filter = L"Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*";
			// 
			// TorrentMatch
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnClose;
			this->ClientSize = System::Drawing::Size(591, 409);
			this->Controls->Add(this->bnClose);
			this->Controls->Add(this->rbBTRenameFiles);
			this->Controls->Add(this->rbBTCopyTo);
			this->Controls->Add(this->bnBTSecondOpen);
			this->Controls->Add(this->bnBTOpenFolder);
			this->Controls->Add(this->tmatchTree);
			this->Controls->Add(this->bnGo);
			this->Controls->Add(this->bnBTSecondBrowse);
			this->Controls->Add(this->bnBrowseFolder);
			this->Controls->Add(this->txtBTSecondLocation);
			this->Controls->Add(this->txtFolder);
			this->Controls->Add(this->bnBrowseTorrent);
			this->Controls->Add(this->label14);
			this->Controls->Add(this->label3);
			this->Controls->Add(this->txtTorrentFile);
			this->Controls->Add(this->label4);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"TorrentMatch";
			this->ShowInTaskbar = false;
			this->Text = L"Torrent Match";
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
	private: System::Void bnClose_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 this->Close();
			 }

			System::Void bnGo_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				mDoc->RenameFilesToMatchTorrent(txtTorrentFile->Text, 
					txtFolder->Text, 
					tmatchTree, 
					SetProgress,
					rbBTCopyTo->Checked,
					txtBTSecondLocation->Text);
			}


			
			System::Void bnBTCopyToBrowse_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (!String::IsNullOrEmpty(txtBTSecondLocation->Text))
					folderBrowser->SelectedPath = txtBTSecondLocation->Text;
				else if (!String::IsNullOrEmpty(txtTorrentFile->Text))
				{
					FileInfo ^fi = gcnew FileInfo(txtTorrentFile->Text);
					if (fi != nullptr)
						folderBrowser->SelectedPath = fi->DirectoryName;
				}
				if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
					txtBTSecondLocation->Text = folderBrowser->SelectedPath;
			}

	private: System::Void rbBTRenameFiles_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
	  {
	    TabBTEnableDisable();
	  }
	  void TabBTEnableDisable()
	  {
	    bool e = rbBTCopyTo->Checked;

	    txtBTSecondLocation->Enabled = e;
	    bnBTSecondBrowse->Enabled = e;
	    bnBTSecondOpen->Enabled = e;
	  }
	private: System::Void rbBTCopyTo_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
	  {
	    TabBTEnableDisable();
	  }
			System::Void bnBTCopyToOpen_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				TVDoc::SysOpen(txtBTSecondLocation->Text);
			}


			System::Void bnBTOpenFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				TVDoc::SysOpen(txtFolder->Text);
			}

			
			System::Void bnBrowseFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (!String::IsNullOrEmpty(txtFolder->Text))
					folderBrowser->SelectedPath = txtFolder->Text;
				else if (!String::IsNullOrEmpty(txtTorrentFile->Text))
				{
					FileInfo ^fi = gcnew FileInfo(txtTorrentFile->Text);
					if (fi != nullptr)
						folderBrowser->SelectedPath = fi->DirectoryName;
				}
				if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
					txtFolder->Text = folderBrowser->SelectedPath;
			}
			System::Void bnBrowseTorrent_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (!String::IsNullOrEmpty(txtTorrentFile->Text))
				{
					FileInfo ^fi = gcnew FileInfo(txtTorrentFile->Text);
					if (fi != nullptr)
					{
						openFile->InitialDirectory = fi->DirectoryName;
						openFile->FileName = fi->Name;
					}
				}
				else if (!String::IsNullOrEmpty(txtFolder->Text))
				{
					openFile->InitialDirectory = txtFolder->Text;
					openFile->FileName = "";
				}


				if (!String::IsNullOrEmpty(txtTorrentFile->Text))
				{
					FileInfo ^fi = gcnew FileInfo(txtTorrentFile->Text);
					if (fi->Exists)
						openFile->FileName = txtTorrentFile->Text;
				}

				if (openFile->ShowDialog() == System::Windows::Forms::DialogResult::OK)
					txtTorrentFile->Text = openFile->FileName;
			}


private: System::Void bnBTSecondBrowse_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 if (!String::IsNullOrEmpty(txtBTSecondLocation->Text))
					folderBrowser->SelectedPath = txtBTSecondLocation->Text;
				else if (!String::IsNullOrEmpty(txtTorrentFile->Text))
				{
					FileInfo ^fi = gcnew FileInfo(txtTorrentFile->Text);
					if (fi != nullptr)
						folderBrowser->SelectedPath = fi->DirectoryName;
				}
				if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
					txtFolder->Text = folderBrowser->SelectedPath;
		 }
private: System::Void bnBTSecondOpen_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 TVDoc::SysOpen(txtBTSecondLocation->Text);
		 }
};
}
