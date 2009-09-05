//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

namespace TVRename {

	/// <summary>
	/// Summary for ScanProgress
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>



public ref class ScanProgress : public System::Windows::Forms::Form
	{
	public:
		bool Ready;
		bool Finished;

		int pctRename;
		int pctMissing;
		int pctLocalSearch;
		int pctRSS;
		int pctuTorrent;
		int pctFolderThumbs;

	private: System::Windows::Forms::Timer^  timer1;
	public: 

		ScanProgress(bool e1, bool e2, bool e3, bool e4, bool e5, bool e6)
		{
			Ready = false;
			Finished = false;
			InitializeComponent();

			lb1->Enabled = e1;
			lb2->Enabled = e2;
			lb3->Enabled = e3;
			lb4->Enabled = e4;
			lb5->Enabled = e5;
			lb6->Enabled = e6;


		}

	  void Update()
	  {
		  pbRename->Value = ((pctRename<0) ? 0 : ((pctRename>100) ? 100 : pctRename));
		  pbRename->Update();
		  pbMissing->Value = ((pctMissing<0) ? 0 : ((pctMissing>100) ? 100 : pctMissing));
		  pbMissing->Update();
		  pbLocalSearch->Value = ((pctLocalSearch<0) ? 0 : ((pctLocalSearch>100) ? 100 : pctLocalSearch));
		  pbLocalSearch->Update();
		  pbRSS->Value = ((pctRSS<0) ? 0 : ((pctRSS>100) ? 100 : pctRSS));
		  pbRSS->Update();
		  pbuTorrent->Value = ((pctuTorrent<0) ? 0 : ((pctuTorrent>100) ? 100 : pctuTorrent));
		  pbuTorrent->Update();
		  pbFolderThumbs->Value = ((pctFolderThumbs<0) ? 0 : ((pctFolderThumbs>100) ? 100 : pctFolderThumbs));
		  pbFolderThumbs->Update();
	  }

	  void RenameProg(int p)
	  {
		  pctRename = p;
	  }

	  void MissingProg(int p)
	  {
		  pctMissing = p;
	  }

	  void LocalSearchProg(int p)
	  {
		  pctLocalSearch = p;
	  }

	  void RSSProg(int p)
	  {
		  pctRSS = p;
	  }
	  void uTorrentProg(int p)
	  {
		  pctuTorrent = p;
	  }

	  void FolderThumbsProg(int p)
	  {
		  pctFolderThumbs = p;
	  }

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~ScanProgress()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::Button^  bnCancel;
	private: System::Windows::Forms::Label^  lb1;
	protected: 

	private: System::Windows::Forms::ProgressBar^  pbRename;
	private: System::Windows::Forms::Label^  lb2;


	private: System::Windows::Forms::ProgressBar^  pbMissing;
	private: System::Windows::Forms::Label^  lb3;


	private: System::Windows::Forms::ProgressBar^  pbLocalSearch;
	private: System::Windows::Forms::Label^  lb5;


	private: System::Windows::Forms::ProgressBar^  pbRSS;
	private: System::Windows::Forms::Label^  lb4;


	private: System::Windows::Forms::ProgressBar^  pbuTorrent;
private: System::Windows::Forms::Label^  lb6;





	private: System::Windows::Forms::ProgressBar^  pbFolderThumbs;
	private: System::ComponentModel::IContainer^  components;






	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(ScanProgress::typeid));
			this->bnCancel = (gcnew System::Windows::Forms::Button());
			this->lb1 = (gcnew System::Windows::Forms::Label());
			this->pbRename = (gcnew System::Windows::Forms::ProgressBar());
			this->lb2 = (gcnew System::Windows::Forms::Label());
			this->pbMissing = (gcnew System::Windows::Forms::ProgressBar());
			this->lb3 = (gcnew System::Windows::Forms::Label());
			this->pbLocalSearch = (gcnew System::Windows::Forms::ProgressBar());
			this->lb5 = (gcnew System::Windows::Forms::Label());
			this->pbRSS = (gcnew System::Windows::Forms::ProgressBar());
			this->lb4 = (gcnew System::Windows::Forms::Label());
			this->pbuTorrent = (gcnew System::Windows::Forms::ProgressBar());
			this->lb6 = (gcnew System::Windows::Forms::Label());
			this->pbFolderThumbs = (gcnew System::Windows::Forms::ProgressBar());
			this->timer1 = (gcnew System::Windows::Forms::Timer(this->components));
			this->SuspendLayout();
			// 
			// bnCancel
			// 
			this->bnCancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnCancel->Location = System::Drawing::Point(289, 127);
			this->bnCancel->Name = L"bnCancel";
			this->bnCancel->Size = System::Drawing::Size(75, 23);
			this->bnCancel->TabIndex = 0;
			this->bnCancel->Text = L"Cancel";
			this->bnCancel->UseVisualStyleBackColor = true;
			// 
			// lb1
			// 
			this->lb1->AutoSize = true;
			this->lb1->Location = System::Drawing::Point(12, 9);
			this->lb1->Name = L"lb1";
			this->lb1->Size = System::Drawing::Size(81, 13);
			this->lb1->TabIndex = 1;
			this->lb1->Text = L"Rename Check";
			// 
			// pbRename
			// 
			this->pbRename->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->pbRename->Location = System::Drawing::Point(141, 9);
			this->pbRename->Name = L"pbRename";
			this->pbRename->Size = System::Drawing::Size(223, 13);
			this->pbRename->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbRename->TabIndex = 2;
			// 
			// lb2
			// 
			this->lb2->AutoSize = true;
			this->lb2->Location = System::Drawing::Point(12, 28);
			this->lb2->Name = L"lb2";
			this->lb2->Size = System::Drawing::Size(117, 13);
			this->lb2->TabIndex = 1;
			this->lb2->Text = L"Missing Episode Check";
			// 
			// pbMissing
			// 
			this->pbMissing->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->pbMissing->Location = System::Drawing::Point(141, 28);
			this->pbMissing->Name = L"pbMissing";
			this->pbMissing->Size = System::Drawing::Size(223, 13);
			this->pbMissing->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbMissing->TabIndex = 2;
			// 
			// lb3
			// 
			this->lb3->AutoSize = true;
			this->lb3->Location = System::Drawing::Point(12, 47);
			this->lb3->Name = L"lb3";
			this->lb3->Size = System::Drawing::Size(77, 13);
			this->lb3->TabIndex = 1;
			this->lb3->Text = L"Search Locally";
			// 
			// pbLocalSearch
			// 
			this->pbLocalSearch->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->pbLocalSearch->Location = System::Drawing::Point(141, 47);
			this->pbLocalSearch->Name = L"pbLocalSearch";
			this->pbLocalSearch->Size = System::Drawing::Size(223, 13);
			this->pbLocalSearch->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbLocalSearch->TabIndex = 2;
			// 
			// lb5
			// 
			this->lb5->AutoSize = true;
			this->lb5->Location = System::Drawing::Point(12, 85);
			this->lb5->Name = L"lb5";
			this->lb5->Size = System::Drawing::Size(66, 13);
			this->lb5->TabIndex = 1;
			this->lb5->Text = L"Search RSS";
			// 
			// pbRSS
			// 
			this->pbRSS->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->pbRSS->Location = System::Drawing::Point(141, 85);
			this->pbRSS->Name = L"pbRSS";
			this->pbRSS->Size = System::Drawing::Size(223, 13);
			this->pbRSS->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbRSS->TabIndex = 2;
			// 
			// lb4
			// 
			this->lb4->AutoSize = true;
			this->lb4->Location = System::Drawing::Point(12, 66);
			this->lb4->Name = L"lb4";
			this->lb4->Size = System::Drawing::Size(81, 13);
			this->lb4->TabIndex = 1;
			this->lb4->Text = L"Check µTorrent";
			// 
			// pbuTorrent
			// 
			this->pbuTorrent->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->pbuTorrent->Location = System::Drawing::Point(141, 66);
			this->pbuTorrent->Name = L"pbuTorrent";
			this->pbuTorrent->Size = System::Drawing::Size(223, 13);
			this->pbuTorrent->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbuTorrent->TabIndex = 2;
			// 
			// lb6
			// 
			this->lb6->AutoSize = true;
			this->lb6->Location = System::Drawing::Point(12, 104);
			this->lb6->Name = L"lb6";
			this->lb6->Size = System::Drawing::Size(93, 13);
			this->lb6->TabIndex = 1;
			this->lb6->Text = L"Folder Thumbnails";
			// 
			// pbFolderThumbs
			// 
			this->pbFolderThumbs->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->pbFolderThumbs->Location = System::Drawing::Point(141, 104);
			this->pbFolderThumbs->Name = L"pbFolderThumbs";
			this->pbFolderThumbs->Size = System::Drawing::Size(223, 13);
			this->pbFolderThumbs->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbFolderThumbs->TabIndex = 2;
			// 
			// timer1
			// 
			this->timer1->Interval = 100;
			this->timer1->Tick += gcnew System::EventHandler(this, &ScanProgress::timer1_Tick);
			// 
			// ScanProgress
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnCancel;
			this->ClientSize = System::Drawing::Size(376, 162);
			this->Controls->Add(this->pbFolderThumbs);
			this->Controls->Add(this->lb6);
			this->Controls->Add(this->pbuTorrent);
			this->Controls->Add(this->lb4);
			this->Controls->Add(this->pbRSS);
			this->Controls->Add(this->lb5);
			this->Controls->Add(this->pbLocalSearch);
			this->Controls->Add(this->lb3);
			this->Controls->Add(this->pbMissing);
			this->Controls->Add(this->lb2);
			this->Controls->Add(this->pbRename);
			this->Controls->Add(this->lb1);
			this->Controls->Add(this->bnCancel);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"ScanProgress";
			this->ShowInTaskbar = false;
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterParent;
			this->Text = L"Scan Progress";
			this->Load += gcnew System::EventHandler(this, &ScanProgress::ScanProgress_Load);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

private: System::Void ScanProgress_Load(System::Object^  sender, System::EventArgs^  e) 
		 {
			 Ready = true;
			 timer1->Start();
		 }
private: System::Void timer1_Tick(System::Object^  sender, System::EventArgs^  e) 
		 {
			 Update();
			 timer1->Start();
			 if (Finished)
				 this->Close();
		 }
public: void Done()
		 {
			 Finished = true;
		 }
};
}
