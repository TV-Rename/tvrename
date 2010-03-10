//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "TVDoc.h"
#include "TheTVDBCodeFinder.h"
#include "AddItem.h"
#include "FolderMonitorProgress.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

namespace TVRename {

	/// <summary>
	/// Summary for FolderMonitor
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class FolderMonitor : public System::Windows::Forms::Form
	{
		TheTVDBCodeFinder ^mTCCF;
		TVDoc ^mDoc;
		int mInternalChange;

	public:
		FolderMonitor(TVDoc ^doc)
		{
			mDoc = doc;
			mInternalChange = 0;

			InitializeComponent();


			mTCCF = gcnew TheTVDBCodeFinder("", mDoc->GetTVDB(false,""));
			mTCCF->Dock = DockStyle::Fill;
			mTCCF->SelectionChanged += gcnew System::EventHandler(this, &FolderMonitor::lvMatches_ItemSelectionChanged);

			pnlCF->SuspendLayout();
			pnlCF->Controls->Add(mTCCF);
			pnlCF->ResumeLayout();

			FillFolderStringLists();
		}
	public:
		String ^FMPUpto;
		int FMPPercent;
		bool FMPStopNow;
		FolderMonitorProgress ^FMP;

		void FMPShower()
		{
			FMP = gcnew FolderMonitorProgress(this);
			FMP->ShowDialog();
			FMP = nullptr;
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~FolderMonitor()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::SplitContainer^  splitContainer3;
	protected: 
	private: System::Windows::Forms::TableLayoutPanel^  tableLayoutPanel1;
	private: System::Windows::Forms::Panel^  panel2;
	private: System::Windows::Forms::Button^  bnFMCheck;
	private: System::Windows::Forms::Button^  bnFMOpenMonFolder;
	private: System::Windows::Forms::Button^  bnFMRemoveMonFolder;
	private: System::Windows::Forms::Label^  label2;
	private: System::Windows::Forms::ListBox^  lstFMMonitorFolders;
	private: System::Windows::Forms::Button^  bnFMAddMonFolder;
	private: System::Windows::Forms::Panel^  panel3;
	private: System::Windows::Forms::Label^  label7;
	private: System::Windows::Forms::Button^  bnFMOpenIgFolder;
	private: System::Windows::Forms::Button^  bnFMAddIgFolder;
	private: System::Windows::Forms::Button^  bnFMRemoveIgFolder;
	private: System::Windows::Forms::ListBox^  lstFMIgnoreFolders;
	private: System::Windows::Forms::TextBox^  txtFMSpecificSeason;
	private: System::Windows::Forms::RadioButton^  rbSpecificSeason;
	private: System::Windows::Forms::RadioButton^  rbFlat;
	private: System::Windows::Forms::RadioButton^  rbFolderPerSeason;
	private: System::Windows::Forms::Button^  bnFMVisitTVcom;
	private: System::Windows::Forms::Panel^  pnlCF;
	private: System::Windows::Forms::Button^  bnFMFullAuto;
	private: System::Windows::Forms::ListView^  lvFMNewShows;
	private: System::Windows::Forms::ColumnHeader^  columnHeader42;
	private: System::Windows::Forms::ColumnHeader^  columnHeader43;
	private: System::Windows::Forms::ColumnHeader^  columnHeader44;
	private: System::Windows::Forms::ColumnHeader^  columnHeader45;
	private: System::Windows::Forms::Button^  bnAddThisOne;
	private: System::Windows::Forms::Button^  bnFolderMonitorDone;

	private: System::Windows::Forms::Label^  label6;
	private: System::Windows::Forms::Button^  bnFMRemoveNewFolder;
	private: System::Windows::Forms::Button^  bnFMNewFolderOpen;
	private: System::Windows::Forms::Button^  bnFMIgnoreAllNewFolders;
	private: System::Windows::Forms::Button^  bnFMIgnoreNewFolder;
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
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(FolderMonitor::typeid));
			this->splitContainer3 = (gcnew System::Windows::Forms::SplitContainer());
			this->tableLayoutPanel1 = (gcnew System::Windows::Forms::TableLayoutPanel());
			this->panel2 = (gcnew System::Windows::Forms::Panel());
			this->bnFMCheck = (gcnew System::Windows::Forms::Button());
			this->bnFMOpenMonFolder = (gcnew System::Windows::Forms::Button());
			this->bnFMRemoveMonFolder = (gcnew System::Windows::Forms::Button());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->lstFMMonitorFolders = (gcnew System::Windows::Forms::ListBox());
			this->bnFMAddMonFolder = (gcnew System::Windows::Forms::Button());
			this->panel3 = (gcnew System::Windows::Forms::Panel());
			this->label7 = (gcnew System::Windows::Forms::Label());
			this->bnFMOpenIgFolder = (gcnew System::Windows::Forms::Button());
			this->bnFMAddIgFolder = (gcnew System::Windows::Forms::Button());
			this->bnFMRemoveIgFolder = (gcnew System::Windows::Forms::Button());
			this->lstFMIgnoreFolders = (gcnew System::Windows::Forms::ListBox());
			this->bnClose = (gcnew System::Windows::Forms::Button());
			this->txtFMSpecificSeason = (gcnew System::Windows::Forms::TextBox());
			this->rbSpecificSeason = (gcnew System::Windows::Forms::RadioButton());
			this->rbFlat = (gcnew System::Windows::Forms::RadioButton());
			this->rbFolderPerSeason = (gcnew System::Windows::Forms::RadioButton());
			this->bnFMVisitTVcom = (gcnew System::Windows::Forms::Button());
			this->pnlCF = (gcnew System::Windows::Forms::Panel());
			this->bnFMFullAuto = (gcnew System::Windows::Forms::Button());
			this->lvFMNewShows = (gcnew System::Windows::Forms::ListView());
			this->columnHeader42 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader43 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader44 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader45 = (gcnew System::Windows::Forms::ColumnHeader());
			this->bnAddThisOne = (gcnew System::Windows::Forms::Button());
			this->bnFolderMonitorDone = (gcnew System::Windows::Forms::Button());
			this->label6 = (gcnew System::Windows::Forms::Label());
			this->bnFMRemoveNewFolder = (gcnew System::Windows::Forms::Button());
			this->bnFMNewFolderOpen = (gcnew System::Windows::Forms::Button());
			this->bnFMIgnoreAllNewFolders = (gcnew System::Windows::Forms::Button());
			this->bnFMIgnoreNewFolder = (gcnew System::Windows::Forms::Button());
			this->folderBrowser = (gcnew System::Windows::Forms::FolderBrowserDialog());
			this->splitContainer3->Panel1->SuspendLayout();
			this->splitContainer3->Panel2->SuspendLayout();
			this->splitContainer3->SuspendLayout();
			this->tableLayoutPanel1->SuspendLayout();
			this->panel2->SuspendLayout();
			this->panel3->SuspendLayout();
			this->SuspendLayout();
			// 
			// splitContainer3
			// 
			this->splitContainer3->Dock = System::Windows::Forms::DockStyle::Fill;
			this->splitContainer3->Location = System::Drawing::Point(0, 0);
			this->splitContainer3->Name = L"splitContainer3";
			this->splitContainer3->Orientation = System::Windows::Forms::Orientation::Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this->splitContainer3->Panel1->Controls->Add(this->tableLayoutPanel1);
			// 
			// splitContainer3.Panel2
			// 
			this->splitContainer3->Panel2->Controls->Add(this->bnClose);
			this->splitContainer3->Panel2->Controls->Add(this->txtFMSpecificSeason);
			this->splitContainer3->Panel2->Controls->Add(this->rbSpecificSeason);
			this->splitContainer3->Panel2->Controls->Add(this->rbFlat);
			this->splitContainer3->Panel2->Controls->Add(this->rbFolderPerSeason);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMVisitTVcom);
			this->splitContainer3->Panel2->Controls->Add(this->pnlCF);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMFullAuto);
			this->splitContainer3->Panel2->Controls->Add(this->lvFMNewShows);
			this->splitContainer3->Panel2->Controls->Add(this->bnAddThisOne);
			this->splitContainer3->Panel2->Controls->Add(this->bnFolderMonitorDone);
			this->splitContainer3->Panel2->Controls->Add(this->label6);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMRemoveNewFolder);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMNewFolderOpen);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMIgnoreAllNewFolders);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMIgnoreNewFolder);
			this->splitContainer3->Size = System::Drawing::Size(887, 634);
			this->splitContainer3->SplitterDistance = 190;
			this->splitContainer3->SplitterWidth = 5;
			this->splitContainer3->TabIndex = 12;
			// 
			// tableLayoutPanel1
			// 
			this->tableLayoutPanel1->ColumnCount = 2;
			this->tableLayoutPanel1->ColumnStyles->Add((gcnew System::Windows::Forms::ColumnStyle(System::Windows::Forms::SizeType::Percent, 
				50)));
			this->tableLayoutPanel1->ColumnStyles->Add((gcnew System::Windows::Forms::ColumnStyle(System::Windows::Forms::SizeType::Percent, 
				50)));
			this->tableLayoutPanel1->Controls->Add(this->panel2, 0, 0);
			this->tableLayoutPanel1->Controls->Add(this->panel3, 1, 0);
			this->tableLayoutPanel1->Dock = System::Windows::Forms::DockStyle::Fill;
			this->tableLayoutPanel1->Location = System::Drawing::Point(0, 0);
			this->tableLayoutPanel1->Name = L"tableLayoutPanel1";
			this->tableLayoutPanel1->RowCount = 1;
			this->tableLayoutPanel1->RowStyles->Add((gcnew System::Windows::Forms::RowStyle(System::Windows::Forms::SizeType::Percent, 100)));
			this->tableLayoutPanel1->RowStyles->Add((gcnew System::Windows::Forms::RowStyle(System::Windows::Forms::SizeType::Absolute, 190)));
			this->tableLayoutPanel1->RowStyles->Add((gcnew System::Windows::Forms::RowStyle(System::Windows::Forms::SizeType::Absolute, 190)));
			this->tableLayoutPanel1->Size = System::Drawing::Size(887, 190);
			this->tableLayoutPanel1->TabIndex = 0;
			// 
			// panel2
			// 
			this->panel2->Controls->Add(this->bnFMCheck);
			this->panel2->Controls->Add(this->bnFMOpenMonFolder);
			this->panel2->Controls->Add(this->bnFMRemoveMonFolder);
			this->panel2->Controls->Add(this->label2);
			this->panel2->Controls->Add(this->lstFMMonitorFolders);
			this->panel2->Controls->Add(this->bnFMAddMonFolder);
			this->panel2->Dock = System::Windows::Forms::DockStyle::Fill;
			this->panel2->Location = System::Drawing::Point(3, 3);
			this->panel2->Name = L"panel2";
			this->panel2->Size = System::Drawing::Size(437, 184);
			this->panel2->TabIndex = 0;
			// 
			// bnFMCheck
			// 
			this->bnFMCheck->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnFMCheck->Location = System::Drawing::Point(359, 158);
			this->bnFMCheck->Name = L"bnFMCheck";
			this->bnFMCheck->Size = System::Drawing::Size(75, 23);
			this->bnFMCheck->TabIndex = 10;
			this->bnFMCheck->Text = L"&Check";
			this->bnFMCheck->UseVisualStyleBackColor = true;
			this->bnFMCheck->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMCheck_Click);
			// 
			// bnFMOpenMonFolder
			// 
			this->bnFMOpenMonFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMOpenMonFolder->Location = System::Drawing::Point(165, 158);
			this->bnFMOpenMonFolder->Name = L"bnFMOpenMonFolder";
			this->bnFMOpenMonFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMOpenMonFolder->TabIndex = 9;
			this->bnFMOpenMonFolder->Text = L"&Open";
			this->bnFMOpenMonFolder->UseVisualStyleBackColor = true;
			this->bnFMOpenMonFolder->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMOpenMonFolder_Click);
			// 
			// bnFMRemoveMonFolder
			// 
			this->bnFMRemoveMonFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMRemoveMonFolder->Location = System::Drawing::Point(84, 158);
			this->bnFMRemoveMonFolder->Name = L"bnFMRemoveMonFolder";
			this->bnFMRemoveMonFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMRemoveMonFolder->TabIndex = 8;
			this->bnFMRemoveMonFolder->Text = L"&Remove";
			this->bnFMRemoveMonFolder->UseVisualStyleBackColor = true;
			this->bnFMRemoveMonFolder->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMRemoveMonFolder_Click);
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Location = System::Drawing::Point(0, 0);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(82, 13);
			this->label2->TabIndex = 5;
			this->label2->Text = L"&Monitor Folders:";
			// 
			// lstFMMonitorFolders
			// 
			this->lstFMMonitorFolders->AllowDrop = true;
			this->lstFMMonitorFolders->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lstFMMonitorFolders->FormattingEnabled = true;
			this->lstFMMonitorFolders->IntegralHeight = false;
			this->lstFMMonitorFolders->Location = System::Drawing::Point(3, 16);
			this->lstFMMonitorFolders->Name = L"lstFMMonitorFolders";
			this->lstFMMonitorFolders->ScrollAlwaysVisible = true;
			this->lstFMMonitorFolders->SelectionMode = System::Windows::Forms::SelectionMode::MultiExtended;
			this->lstFMMonitorFolders->Size = System::Drawing::Size(431, 136);
			this->lstFMMonitorFolders->TabIndex = 6;
			// 
			// bnFMAddMonFolder
			// 
			this->bnFMAddMonFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMAddMonFolder->Location = System::Drawing::Point(3, 158);
			this->bnFMAddMonFolder->Name = L"bnFMAddMonFolder";
			this->bnFMAddMonFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMAddMonFolder->TabIndex = 7;
			this->bnFMAddMonFolder->Text = L"&Add";
			this->bnFMAddMonFolder->UseVisualStyleBackColor = true;
			this->bnFMAddMonFolder->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMAddMonFolder_Click);
			// 
			// panel3
			// 
			this->panel3->Controls->Add(this->label7);
			this->panel3->Controls->Add(this->bnFMOpenIgFolder);
			this->panel3->Controls->Add(this->bnFMAddIgFolder);
			this->panel3->Controls->Add(this->bnFMRemoveIgFolder);
			this->panel3->Controls->Add(this->lstFMIgnoreFolders);
			this->panel3->Dock = System::Windows::Forms::DockStyle::Fill;
			this->panel3->Location = System::Drawing::Point(446, 3);
			this->panel3->Name = L"panel3";
			this->panel3->Size = System::Drawing::Size(438, 184);
			this->panel3->TabIndex = 0;
			// 
			// label7
			// 
			this->label7->AutoSize = true;
			this->label7->Location = System::Drawing::Point(3, 0);
			this->label7->Name = L"label7";
			this->label7->Size = System::Drawing::Size(77, 13);
			this->label7->TabIndex = 5;
			this->label7->Text = L"&Ignore Folders:";
			// 
			// bnFMOpenIgFolder
			// 
			this->bnFMOpenIgFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMOpenIgFolder->Location = System::Drawing::Point(165, 158);
			this->bnFMOpenIgFolder->Name = L"bnFMOpenIgFolder";
			this->bnFMOpenIgFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMOpenIgFolder->TabIndex = 9;
			this->bnFMOpenIgFolder->Text = L"O&pen";
			this->bnFMOpenIgFolder->UseVisualStyleBackColor = true;
			this->bnFMOpenIgFolder->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMOpenIgFolder_Click);
			// 
			// bnFMAddIgFolder
			// 
			this->bnFMAddIgFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMAddIgFolder->Location = System::Drawing::Point(2, 158);
			this->bnFMAddIgFolder->Name = L"bnFMAddIgFolder";
			this->bnFMAddIgFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMAddIgFolder->TabIndex = 7;
			this->bnFMAddIgFolder->Text = L"A&dd";
			this->bnFMAddIgFolder->UseVisualStyleBackColor = true;
			this->bnFMAddIgFolder->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMAddIgFolder_Click);
			// 
			// bnFMRemoveIgFolder
			// 
			this->bnFMRemoveIgFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMRemoveIgFolder->Location = System::Drawing::Point(84, 158);
			this->bnFMRemoveIgFolder->Name = L"bnFMRemoveIgFolder";
			this->bnFMRemoveIgFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMRemoveIgFolder->TabIndex = 8;
			this->bnFMRemoveIgFolder->Text = L"Remo&ve";
			this->bnFMRemoveIgFolder->UseVisualStyleBackColor = true;
			this->bnFMRemoveIgFolder->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMRemoveIgFolder_Click);
			// 
			// lstFMIgnoreFolders
			// 
			this->lstFMIgnoreFolders->AllowDrop = true;
			this->lstFMIgnoreFolders->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lstFMIgnoreFolders->FormattingEnabled = true;
			this->lstFMIgnoreFolders->IntegralHeight = false;
			this->lstFMIgnoreFolders->Location = System::Drawing::Point(0, 16);
			this->lstFMIgnoreFolders->Name = L"lstFMIgnoreFolders";
			this->lstFMIgnoreFolders->ScrollAlwaysVisible = true;
			this->lstFMIgnoreFolders->SelectionMode = System::Windows::Forms::SelectionMode::MultiExtended;
			this->lstFMIgnoreFolders->Size = System::Drawing::Size(438, 136);
			this->lstFMIgnoreFolders->TabIndex = 6;
			// 
			// bnClose
			// 
			this->bnClose->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnClose->Location = System::Drawing::Point(809, 413);
			this->bnClose->Name = L"bnClose";
			this->bnClose->Size = System::Drawing::Size(75, 23);
			this->bnClose->TabIndex = 30;
			this->bnClose->Text = L"Close";
			this->bnClose->UseVisualStyleBackColor = true;
			this->bnClose->Click += gcnew System::EventHandler(this, &FolderMonitor::bnClose_Click);
			// 
			// txtFMSpecificSeason
			// 
			this->txtFMSpecificSeason->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->txtFMSpecificSeason->Location = System::Drawing::Point(533, 235);
			this->txtFMSpecificSeason->Name = L"txtFMSpecificSeason";
			this->txtFMSpecificSeason->Size = System::Drawing::Size(53, 20);
			this->txtFMSpecificSeason->TabIndex = 29;
			// 
			// rbSpecificSeason
			// 
			this->rbSpecificSeason->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbSpecificSeason->AutoSize = true;
			this->rbSpecificSeason->Location = System::Drawing::Point(433, 236);
			this->rbSpecificSeason->Name = L"rbSpecificSeason";
			this->rbSpecificSeason->Size = System::Drawing::Size(100, 17);
			this->rbSpecificSeason->TabIndex = 28;
			this->rbSpecificSeason->TabStop = true;
			this->rbSpecificSeason->Text = L"Specific season";
			this->rbSpecificSeason->UseVisualStyleBackColor = true;
			this->rbSpecificSeason->CheckedChanged += gcnew System::EventHandler(this, &FolderMonitor::rbSpecificSeason_CheckedChanged);
			// 
			// rbFlat
			// 
			this->rbFlat->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbFlat->AutoSize = true;
			this->rbFlat->Location = System::Drawing::Point(433, 213);
			this->rbFlat->Name = L"rbFlat";
			this->rbFlat->Size = System::Drawing::Size(120, 17);
			this->rbFlat->TabIndex = 28;
			this->rbFlat->TabStop = true;
			this->rbFlat->Text = L"All seasons together";
			this->rbFlat->UseVisualStyleBackColor = true;
			this->rbFlat->CheckedChanged += gcnew System::EventHandler(this, &FolderMonitor::rbFlat_CheckedChanged);
			// 
			// rbFolderPerSeason
			// 
			this->rbFolderPerSeason->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbFolderPerSeason->AutoSize = true;
			this->rbFolderPerSeason->Location = System::Drawing::Point(433, 190);
			this->rbFolderPerSeason->Name = L"rbFolderPerSeason";
			this->rbFolderPerSeason->Size = System::Drawing::Size(109, 17);
			this->rbFolderPerSeason->TabIndex = 28;
			this->rbFolderPerSeason->TabStop = true;
			this->rbFolderPerSeason->Text = L"Folder per season";
			this->rbFolderPerSeason->UseVisualStyleBackColor = true;
			this->rbFolderPerSeason->CheckedChanged += gcnew System::EventHandler(this, &FolderMonitor::rbFolderPerSeason_CheckedChanged);
			// 
			// bnFMVisitTVcom
			// 
			this->bnFMVisitTVcom->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMVisitTVcom->Location = System::Drawing::Point(332, 383);
			this->bnFMVisitTVcom->Name = L"bnFMVisitTVcom";
			this->bnFMVisitTVcom->Size = System::Drawing::Size(75, 23);
			this->bnFMVisitTVcom->TabIndex = 26;
			this->bnFMVisitTVcom->Text = L"&Visit TVDB";
			this->bnFMVisitTVcom->UseVisualStyleBackColor = true;
			this->bnFMVisitTVcom->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMVisitTVcom_Click);
			// 
			// pnlCF
			// 
			this->pnlCF->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->pnlCF->Location = System::Drawing::Point(5, 190);
			this->pnlCF->Name = L"pnlCF";
			this->pnlCF->Size = System::Drawing::Size(407, 185);
			this->pnlCF->TabIndex = 25;
			// 
			// bnFMFullAuto
			// 
			this->bnFMFullAuto->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMFullAuto->Location = System::Drawing::Point(8, 413);
			this->bnFMFullAuto->Name = L"bnFMFullAuto";
			this->bnFMFullAuto->Size = System::Drawing::Size(75, 23);
			this->bnFMFullAuto->TabIndex = 24;
			this->bnFMFullAuto->Text = L"F&ull Auto";
			this->bnFMFullAuto->UseVisualStyleBackColor = true;
			this->bnFMFullAuto->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMFullAuto_Click);
			// 
			// lvFMNewShows
			// 
			this->lvFMNewShows->AllowDrop = true;
			this->lvFMNewShows->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvFMNewShows->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(4) {this->columnHeader42, 
				this->columnHeader43, this->columnHeader44, this->columnHeader45});
			this->lvFMNewShows->FullRowSelect = true;
			this->lvFMNewShows->HeaderStyle = System::Windows::Forms::ColumnHeaderStyle::Nonclickable;
			this->lvFMNewShows->HideSelection = false;
			this->lvFMNewShows->Location = System::Drawing::Point(6, 23);
			this->lvFMNewShows->Name = L"lvFMNewShows";
			this->lvFMNewShows->Size = System::Drawing::Size(881, 161);
			this->lvFMNewShows->TabIndex = 11;
			this->lvFMNewShows->UseCompatibleStateImageBehavior = false;
			this->lvFMNewShows->View = System::Windows::Forms::View::Details;
			// 
			// columnHeader42
			// 
			this->columnHeader42->Text = L"Folder";
			this->columnHeader42->Width = 240;
			// 
			// columnHeader43
			// 
			this->columnHeader43->Text = L"Show";
			this->columnHeader43->Width = 139;
			// 
			// columnHeader44
			// 
			this->columnHeader44->Text = L"Season";
			// 
			// columnHeader45
			// 
			this->columnHeader45->Text = L"thetvdb code";
			this->columnHeader45->Width = 94;
			// 
			// bnAddThisOne
			// 
			this->bnAddThisOne->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnAddThisOne->Location = System::Drawing::Point(433, 383);
			this->bnAddThisOne->Name = L"bnAddThisOne";
			this->bnAddThisOne->Size = System::Drawing::Size(75, 23);
			this->bnAddThisOne->TabIndex = 10;
			this->bnAddThisOne->Text = L"Add &This";
			this->bnAddThisOne->UseVisualStyleBackColor = true;
			this->bnAddThisOne->Click += gcnew System::EventHandler(this, &FolderMonitor::bnAddThisOne_Click);
			// 
			// bnFolderMonitorDone
			// 
			this->bnFolderMonitorDone->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFolderMonitorDone->Location = System::Drawing::Point(433, 413);
			this->bnFolderMonitorDone->Name = L"bnFolderMonitorDone";
			this->bnFolderMonitorDone->Size = System::Drawing::Size(75, 23);
			this->bnFolderMonitorDone->TabIndex = 10;
			this->bnFolderMonitorDone->Text = L"Do&ne";
			this->bnFolderMonitorDone->UseVisualStyleBackColor = true;
			this->bnFolderMonitorDone->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFolderMonitorDone_Click);
			// 
			// label6
			// 
			this->label6->AutoSize = true;
			this->label6->Location = System::Drawing::Point(3, 5);
			this->label6->Name = L"label6";
			this->label6->Size = System::Drawing::Size(64, 13);
			this->label6->TabIndex = 5;
			this->label6->Text = L"&New Shows";
			// 
			// bnFMRemoveNewFolder
			// 
			this->bnFMRemoveNewFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMRemoveNewFolder->Location = System::Drawing::Point(89, 414);
			this->bnFMRemoveNewFolder->Name = L"bnFMRemoveNewFolder";
			this->bnFMRemoveNewFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMRemoveNewFolder->TabIndex = 9;
			this->bnFMRemoveNewFolder->Text = L"Re&move";
			this->bnFMRemoveNewFolder->UseVisualStyleBackColor = true;
			this->bnFMRemoveNewFolder->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMRemoveNewFolder_Click);
			// 
			// bnFMNewFolderOpen
			// 
			this->bnFMNewFolderOpen->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMNewFolderOpen->Location = System::Drawing::Point(332, 414);
			this->bnFMNewFolderOpen->Name = L"bnFMNewFolderOpen";
			this->bnFMNewFolderOpen->Size = System::Drawing::Size(75, 23);
			this->bnFMNewFolderOpen->TabIndex = 9;
			this->bnFMNewFolderOpen->Text = L"Op&en";
			this->bnFMNewFolderOpen->UseVisualStyleBackColor = true;
			this->bnFMNewFolderOpen->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMNewFolderOpen_Click);
			// 
			// bnFMIgnoreAllNewFolders
			// 
			this->bnFMIgnoreAllNewFolders->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMIgnoreAllNewFolders->Location = System::Drawing::Point(251, 414);
			this->bnFMIgnoreAllNewFolders->Name = L"bnFMIgnoreAllNewFolders";
			this->bnFMIgnoreAllNewFolders->Size = System::Drawing::Size(75, 23);
			this->bnFMIgnoreAllNewFolders->TabIndex = 9;
			this->bnFMIgnoreAllNewFolders->Text = L"Ig&nore All";
			this->bnFMIgnoreAllNewFolders->UseVisualStyleBackColor = true;
			this->bnFMIgnoreAllNewFolders->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMIgnoreAllNewFolders_Click);
			// 
			// bnFMIgnoreNewFolder
			// 
			this->bnFMIgnoreNewFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMIgnoreNewFolder->Location = System::Drawing::Point(170, 414);
			this->bnFMIgnoreNewFolder->Name = L"bnFMIgnoreNewFolder";
			this->bnFMIgnoreNewFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMIgnoreNewFolder->TabIndex = 9;
			this->bnFMIgnoreNewFolder->Text = L"&Ignore";
			this->bnFMIgnoreNewFolder->UseVisualStyleBackColor = true;
			this->bnFMIgnoreNewFolder->Click += gcnew System::EventHandler(this, &FolderMonitor::bnFMIgnoreNewFolder_Click);
			// 
			// folderBrowser
			// 
			this->folderBrowser->ShowNewFolderButton = false;
			// 
			// FolderMonitor
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnClose;
			this->ClientSize = System::Drawing::Size(887, 634);
			this->Controls->Add(this->splitContainer3);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"FolderMonitor";
			this->ShowInTaskbar = false;
			this->Text = L"TVRename Folder Monitor";
			this->splitContainer3->Panel1->ResumeLayout(false);
			this->splitContainer3->Panel2->ResumeLayout(false);
			this->splitContainer3->Panel2->PerformLayout();
			this->splitContainer3->ResumeLayout(false);
			this->tableLayoutPanel1->ResumeLayout(false);
			this->panel2->ResumeLayout(false);
			this->panel2->PerformLayout();
			this->panel3->ResumeLayout(false);
			this->panel3->PerformLayout();
			this->ResumeLayout(false);

		}
#pragma endregion
	private: System::Void bnClose_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 this->Close();
			 }



			 void FillFolderStringLists()
			 {
				 lstFMIgnoreFolders->BeginUpdate();
				 lstFMMonitorFolders->BeginUpdate();

				 lstFMIgnoreFolders->Items->Clear();
				 lstFMMonitorFolders->Items->Clear();

				 mDoc->MonitorFolders->Sort();
				 mDoc->IgnoreFolders->Sort();

				 for each (String ^folder in mDoc->MonitorFolders)
					 lstFMMonitorFolders->Items->Add(folder);

				 for each (String ^folder in mDoc->IgnoreFolders)
					 lstFMIgnoreFolders->Items->Add(folder);

				 lstFMIgnoreFolders->EndUpdate();
				 lstFMMonitorFolders->EndUpdate();
			 }

			 System::Void bnFMRemoveMonFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 for (int i=lstFMMonitorFolders->SelectedIndices->Count-1;i>=0;i--)
				 {
					 int n = lstFMMonitorFolders->SelectedIndices[i];
					 mDoc->MonitorFolders->RemoveAt(n);
				 }
				 mDoc->SetDirty();
				 FillFolderStringLists();
			 }

			 System::Void bnFMRemoveIgFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 for (int i=lstFMIgnoreFolders->SelectedIndices->Count-1;i>=0;i--)
				 {
					 int n = lstFMIgnoreFolders->SelectedIndices[i];
					 mDoc->IgnoreFolders->RemoveAt(n);
				 }
				 mDoc->SetDirty();
				 FillFolderStringLists();
			 }
			 System::Void bnFMAddMonFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 folderBrowser->SelectedPath = "";
				 if (lstFMMonitorFolders->SelectedIndex != -1)
				 {
					 int n = lstFMMonitorFolders->SelectedIndex;
					 folderBrowser->SelectedPath = mDoc->MonitorFolders[n];
				 }

				 if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
				 {
					 mDoc->MonitorFolders->Add(folderBrowser->SelectedPath->ToLower());
					 mDoc->SetDirty();
					 FillFolderStringLists();
				 }
			 }
			 System::Void bnFMAddIgFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 folderBrowser->SelectedPath = "";
				 if (lstFMIgnoreFolders->SelectedIndex != -1)
					 folderBrowser->SelectedPath = mDoc->IgnoreFolders[lstFMIgnoreFolders->SelectedIndex];

				 if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
				 {
					 mDoc->IgnoreFolders->Add(folderBrowser->SelectedPath->ToLower());
					 mDoc->SetDirty();
					 FillFolderStringLists();
				 }
			 }
			 System::Void bnFMOpenMonFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (lstFMMonitorFolders->SelectedIndex != -1)
					 TVDoc::SysOpen(mDoc->MonitorFolders[lstFMMonitorFolders->SelectedIndex]);
			 }
			 System::Void bnFMOpenIgFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (lstFMIgnoreFolders->SelectedIndex != -1)
					 TVDoc::SysOpen(mDoc->MonitorFolders[lstFMIgnoreFolders->SelectedIndex]);
			 }
			 System::Void lstFMMonitorFolders_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
			 {
				 bnFMOpenMonFolder_Click(nullptr,nullptr);
			 }
			 System::Void lstFMIgnoreFolders_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
			 {
				 bnFMOpenIgFolder_Click(nullptr,nullptr);
			 }
			 System::Void bnFMCheck_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 mDoc->CheckMonitoredFolders();
				 GuessAll();
				 FillFMNewShowList(false);
				 //FillFMTVcomListCombo();
			 }

			 System::Void lstFMMonitorFolders_DragOver(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e)
			 {
				 if (  !e->Data->GetDataPresent( DataFormats::FileDrop ) )
					 e->Effect = DragDropEffects::None;
				 else
					 e->Effect = DragDropEffects::Copy;
			 }
			 System::Void lstFMIgnoreFolders_DragOver(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
			 {
				 if (  !e->Data->GetDataPresent( DataFormats::FileDrop ) )
					 e->Effect = DragDropEffects::None;
				 else
					 e->Effect = DragDropEffects::Copy;
			 }
			 System::Void lstFMMonitorFolders_DragDrop(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
			 {
				 array<String ^> ^files = safe_cast<array<String ^>^>(e->Data->GetData(DataFormats::FileDrop));
				 for (int i=0;i<files->Length;i++)
				 {
					 String ^path = files[i];
					 DirectoryInfo ^di;
					 try 
					 {
						 di = gcnew DirectoryInfo(path);
						 if (di->Exists)
							 mDoc->MonitorFolders->Add(path->ToLower());
					 }
					 catch (...)
					 {
					 }
				 }
				 mDoc->SetDirty();
				 FillFolderStringLists();
			 }
			 System::Void lstFMIgnoreFolders_DragDrop(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
			 {
				 array<String ^> ^files = safe_cast<array<String ^>^>(e->Data->GetData(DataFormats::FileDrop));
				 for (int i=0;i<files->Length;i++)
				 {
					 String ^path = files[i];
					 DirectoryInfo ^di;
					 try 
					 {
						 di = gcnew DirectoryInfo(path);
						 if (di->Exists)
							 mDoc->IgnoreFolders->Add(path->ToLower());
					 }
					 catch (...)
					 {
					 }
				 }
				 mDoc->SetDirty();
				 FillFolderStringLists();
			 }
			 System::Void lstFMMonitorFolders_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			 {
				 if (e->KeyCode == Keys::Delete)
					 bnFMRemoveMonFolder_Click(nullptr,nullptr);
			 }
			 System::Void lstFMIgnoreFolders_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			 {
				 if (e->KeyCode == Keys::Delete)
					 bnFMRemoveIgFolder_Click(nullptr,nullptr);
			 }

	private: System::Void bnFMFullAuto_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 FMPStopNow = false;
				 FMPUpto = "";
				 FMPPercent = 0;

				 Thread ^fmpshower = gcnew Thread(gcnew ThreadStart(this, &FolderMonitor::FMPShower));
				 fmpshower->Name = "Folder Monitor Progress";
				 fmpshower->Start();

				 int n = 0;
				 int n2 = mDoc->AddItems->Count;

				 for each (AddItem ^ai in mDoc->AddItems)
				 {
					 if (ai->TheSeries == nullptr)
					 {
						 // do search using folder name
						 mDoc->GuessShowName(ai);
						 if (!String::IsNullOrEmpty(ai->ShowName))
						 {
							 FMPUpto = ai->ShowName;
							 mDoc->GetTVDB(false,"")->Search(ai->ShowName);
							 GuessAI(ai);
							 UpdateFMListItem(ai, true);
							 lvFMNewShows->Update();
						 }
					 }
					 FMPPercent = (100*(n+(n2/2)))/n2;
					 n++;
					 if (FMPStopNow)
						 break;
				 }
				 FMPStopNow = true;
			 }

			 System::Void bnFMRemoveNewFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (lvFMNewShows->SelectedItems->Count == 0)
					 return;
				 for each (ListViewItem ^lvi in lvFMNewShows->SelectedItems)
				 {
					 AddItem ^ai = safe_cast<AddItem ^>(lvi->Tag);
					 mDoc->AddItems->Remove(ai);
				 }
				 FillFMNewShowList(false);
			 }
			 System::Void bnFMIgnoreNewFolder_Click(System::Object^  sender, System::EventArgs^  e)
			 {
				 if (lvFMNewShows->SelectedItems->Count == 0)
					 return;
				 for each (ListViewItem ^lvi in lvFMNewShows->SelectedItems)
				 {
					 AddItem ^ai = safe_cast<AddItem ^>(lvi->Tag);
					 mDoc->IgnoreFolders->Add(ai->Folder->ToLower());
					 mDoc->AddItems->Remove(ai);
				 }
				 mDoc->SetDirty();
				 FillFMNewShowList(false);
				 FillFolderStringLists();
			 }


			 System::Void bnFMIgnoreAllNewFolders_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 System::Windows::Forms::DialogResult dr = MessageBox::Show("Add everything in this list to the ignore list?",
					 "Ignore All",
					 Windows::Forms::MessageBoxButtons::OKCancel,
					 Windows::Forms::MessageBoxIcon::Exclamation);

				 if (dr != System::Windows::Forms::DialogResult::OK)
					 return;

				 for each (AddItem ^ai in mDoc->AddItems)
					 mDoc->IgnoreFolders->Add(ai->Folder->ToLower());

				 mDoc->AddItems->Clear();
				 mDoc->SetDirty();
				 FillFolderStringLists();
				 FillFMNewShowList(false);
			 }



			 System::Void lvFMNewShows_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			 {
				 /*
				 if (e->Button != System::Windows::Forms::MouseButtons::Right)
				 return;

				 lstFMNewFolders->ClearSelected();
				 lstFMNewFolders->SelectedIndex = lstFMNewFolders->IndexFromPoint(Point(e->X,e->Y));

				 int p;
				 if ((p = lstFMNewFolders->SelectedIndex) == -1)
				 return;

				 Point^ pt = lstFMNewFolders->PointToScreen(Point(e->X, e->Y));
				 RightClickOnFolder(lstFMNewFolders->Items[p]->ToString(),pt);

				 */
			 }
			 System::Void lvFMNewShows_DragOver(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
			 {
				 if (  !e->Data->GetDataPresent( DataFormats::FileDrop ) )
					 e->Effect = DragDropEffects::None;
				 else
					 e->Effect = DragDropEffects::Copy;
			 }
			 System::Void lvFMNewShows_DragDrop(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
			 {
				 array<String ^> ^files = safe_cast<array<String ^>^>(e->Data->GetData(DataFormats::FileDrop));
				 for (int i=0;i<files->Length;i++)
				 {
					 String ^path = files[i];
					 DirectoryInfo ^di;
					 try 
					 {
						 di = gcnew DirectoryInfo(path);
						 if (di->Exists)
						 {
							 // keep next line sync'd with ProcessAddItems, etc.
							 bool hasSeasonFolders = DirectoryInfo(path).GetDirectories("*Season *")->Length > 0; // todo - use non specific word
							 AddItem ^ai = gcnew AddItem(path, hasSeasonFolders ? kfmFolderPerSeason : kfmFlat, -1);
							 GuessAI(ai);
							 mDoc->AddItems->Add(ai);
						 }
					 }
					 catch (...)
					 {
					 }
				 }
				 FillFMNewShowList(true);
			 }
			 System::Void lvFMNewShows_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			 {
				 if (e->KeyCode == Keys::Delete)
					 bnFMRemoveNewFolder_Click(nullptr,nullptr);
			 }
			 System::Void bnFMNewFolderOpen_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (lvFMNewShows->SelectedItems->Count == 0)
					 return;
				 AddItem ^ai = safe_cast<AddItem ^>(lvFMNewShows->SelectedItems[0]->Tag);
				 TVDoc::SysOpen(ai->Folder);
			 }
			 System::Void lvFMNewShows_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
			 {
				 bnFMNewFolderOpen_Click(nullptr,nullptr);
			 }




			 System::Void lvMatches_ItemSelectionChanged(System::Object^  sender, System::EventArgs ^  e) 
			 {
				 if (mInternalChange)
					 return;

				 int code = mTCCF->SelectedCode();

				 SeriesInfo ^ser = mDoc->GetTVDB(false,"")->GetSeries(code);
				 if (ser == nullptr)
					 return;

				 for each (ListViewItem ^lvi in lvFMNewShows->SelectedItems)
				 {
					 AddItem ^ai = safe_cast<AddItem ^>(lvi->Tag);
					 ai->TheSeries = ser;
					 UpdateFMListItem(ai,false);
				 }
			 }

			 System::Void lvFMNewShows_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (lvFMNewShows->SelectedItems->Count == 0)
					 return;
				 AddItem ^ai = safe_cast<AddItem ^>(lvFMNewShows->SelectedItems[0]->Tag);
				 //txtTVComCode->Text = ai->TVcomCode == -1 ? "" : ai->TVcomCode.ToString();
				 //txtShowName->Text = ai->Show;
				 mInternalChange++;
				 mTCCF->SetHint(ai->TheSeries != nullptr ? ai->TheSeries->TVDBCode.ToString() : ai->ShowName);

				 if (ai->FolderMode == kfmFlat)
					 rbFlat->Checked = true;
				 else if (ai->FolderMode == kfmSpecificSeason)
				 {
					 rbSpecificSeason->Checked = true;
					 txtFMSpecificSeason->Text = ai->SpecificSeason.ToString();
				 }
				 else
					 rbFolderPerSeason->Checked = true;
				 rbSpecificSeason_CheckedChanged(nullptr,nullptr);

				 mInternalChange--;
			 }
			 void FillFMNewShowList(bool keepSel)
			 {
				 Generic::List<int> ^sel = gcnew Generic::List<int>;
				 if (keepSel)
					 for each (int i in lvFMNewShows->SelectedIndices)
						 sel->Add(i);

				 lvFMNewShows->BeginUpdate();
				 lvFMNewShows->Items->Clear();

				 for each (AddItem ^ai in mDoc->AddItems)
				 {
					 ListViewItem ^lvi = gcnew ListViewItem();
					 FMLVISet(ai, lvi);
					 lvFMNewShows->Items->Add(lvi);
				 }

				 if (keepSel)
					 for each (int i in sel)
						 if (i < lvFMNewShows->Items->Count)
							 lvFMNewShows->Items[i]->Selected = true;

				 lvFMNewShows->EndUpdate();
			 }
			 static void FMLVISet(AddItem ^ai, ListViewItem ^lvi)
			 {
				 lvi->SubItems->Clear();
				 lvi->Text = ai->Folder;
				 lvi->SubItems->Add(ai->TheSeries != nullptr ? ai->TheSeries->Name : "");
				 String ^fmode = "-";
				 if (ai->FolderMode == kfmFolderPerSeason)
					 fmode = "Per Seas";
				 else if (ai->FolderMode == kfmFlat)
					 fmode = "Flat";
				 else if (ai->FolderMode == kfmSpecificSeason)
					 fmode = ai->SpecificSeason.ToString();
				 lvi->SubItems->Add(fmode);
				 lvi->SubItems->Add(ai->TheSeries != nullptr ? ai->TheSeries->TVDBCode.ToString() : "");
				 lvi->Tag = ai;
			 }
			 void UpdateFMListItem(AddItem ^ai, bool makevis)
			 {
				 for each (ListViewItem ^lvi in lvFMNewShows->Items)
				 {
					 if (lvi->Tag == ai)
					 {
						 FMLVISet(ai, lvi);
						 if (makevis)
							 lvi->EnsureVisible();
						 break;
					 }
				 }
			 }


			 System::Void bnAddThisOne_Click(System::Object^  sender, System::EventArgs^  e)
			 {
				 if (lvFMNewShows->SelectedItems->Count == 0)
					 return;

				 System::Windows::Forms::DialogResult res = MessageBox::Show("Add the selected folders to My Shows?","Folder Monitor",MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
				 if (res != System::Windows::Forms::DialogResult::Yes)
					 return;

				 Generic::List<AddItem ^> ^toAdd = gcnew Generic::List<AddItem ^>();
				 for each (ListViewItem ^lvi in lvFMNewShows->SelectedItems)
				 {
					 AddItem ^ai = safe_cast<AddItem ^>(lvi->Tag);
					 toAdd->Add(ai);
					 mDoc->AddItems->Remove(ai);
				 }
				 ProcessAddItems(toAdd);
			 }

			 System::Void bnFolderMonitorDone_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 System::Windows::Forms::DialogResult res = MessageBox::Show("Add all of these to My Shows?","Folder Monitor",MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
				 if (res != System::Windows::Forms::DialogResult::Yes)
					 return;

				 ProcessAddItems(mDoc->AddItems);

				 this->Close();
			 }

			 void ProcessAddItems(Generic::List<AddItem ^> ^toAdd)
			 {
				 for each (AddItem ^ai in toAdd)
				 {
					 if (ai->TheSeries == nullptr)
						 continue; // skip

					 // see if there is a matching show item
					 ShowItem ^found = nullptr;
					 for each (ShowItem ^si in mDoc->GetShowItems(true))
					 {
						 if ( (ai->TheSeries != nullptr) &&
							 (ai->TheSeries->TVDBCode == si->TVDBCode) )
						 {
							 found = si;
							 break;
						 }
					 }
					 mDoc->UnlockShowItems();
					 if (found == nullptr)
					 {
						 ShowItem ^si = gcnew ShowItem(mDoc->GetTVDB(false,""));
						 si->TVDBCode = ai->TheSeries->TVDBCode;
						 //si->ShowName() = ai->TheSeries->Name;
						 mDoc->GetShowItems(true)->Add(si);
						 mDoc->UnlockShowItems();
						 mDoc->GenDict();
						 found = si;
					 }

					 if ((ai->FolderMode == kfmFolderPerSeason) || (ai->FolderMode == kfmFlat))
					 {
						 found->AutoAdd_FolderBase = ai->Folder;
						 found->AutoAdd_FolderPerSeason = ai->FolderMode == kfmFolderPerSeason;
						 String ^foldername = "Season ";

						 for each (DirectoryInfo ^di in DirectoryInfo(ai->Folder).GetDirectories("*Season *"))
						 {
							 String ^s = di->FullName;
							 String ^f = ai->Folder;
							 if (!f->EndsWith("\\"))
								 f = f + "\\";
							 f = Regex::Escape(f);
							 s = Regex::Replace(s, f+"(.*Season ).*", "$1",RegexOptions::IgnoreCase);
							 if (!String::IsNullOrEmpty(s))
							 {
								 foldername = s;
								 break;
							 }
						 }

						 found->AutoAdd_SeasonFolderName = foldername;
					 }

					 if ((ai->FolderMode == kfmSpecificSeason) && (ai->SpecificSeason != -1))
					 {
						 if (!found->ManualFolderLocations->ContainsKey(ai->SpecificSeason))
							 found->ManualFolderLocations[ai->SpecificSeason] = gcnew StringList();
						 found->ManualFolderLocations[ai->SpecificSeason]->Add(ai->Folder);
					 }

					 mDoc->Stats()->AutoAddedShows++;
				 }

				 mDoc->Dirty();
				 toAdd->Clear();

				 FillFMNewShowList(true);
			 }

			 void GuessAI(AddItem ^ai)
			 {
				 mDoc->GuessShowName(ai);
				 if (!String::IsNullOrEmpty(ai->ShowName))
				 {
					 TheTVDB ^db = mDoc->GetTVDB(true,"GuessAI");
					 for each (KeyValuePair<int, SeriesInfo ^> ^ser in db->GetSeriesDict())
					 {
						 String ^s;
						 s = ser->Value->Name->ToLower();
						 if (s == ai->ShowName->ToLower())
						 {
							 ai->TheSeries = ser->Value;
							 break;
						 }
					 }
					 db->Unlock("GuessAI");
				 }
			 }

			 void GuessAll() // not all -> selected only
			 {
				 for each (AddItem ^ai in mDoc->AddItems)
					 GuessAI(ai);
				 FillFMNewShowList(false);
			 }

			 System::Void FMControlLeave(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (lvFMNewShows->SelectedItems->Count != 0)
				 {
					 AddItem ^ai = safe_cast<AddItem ^>(lvFMNewShows->SelectedItems[0]->Tag);
					 UpdateFMListItem(ai,false);
				 }

			 }
			 System::Void bnFMVisitTVcom_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 int code = mTCCF->SelectedCode();
				 TVDoc::SysOpen(mDoc->GetTVDB(false,"")->WebsiteURL(code, -1, false));
			 }
			 void SetAllFolderModes(FolderModeEnum fm)
			 {
				 for each (ListViewItem ^lvi in lvFMNewShows->SelectedItems)
				 {
					 AddItem ^ai = safe_cast<AddItem ^>(lvi->Tag);

					 ai->FolderMode = fm;
					 UpdateFMListItem(ai,false);
				 }

			 }

	private: System::Void rbSpecificSeason_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 txtFMSpecificSeason->Enabled = rbSpecificSeason->Checked;

				 if (!mInternalChange)
					 SetAllFolderModes(kfmSpecificSeason);
			 }
	private: System::Void rbFlat_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (!mInternalChange)
					 SetAllFolderModes(kfmFlat);
			 }
	private: System::Void rbFolderPerSeason_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (!mInternalChange)
					 SetAllFolderModes(kfmFolderPerSeason);
			 }

			 System::Void lstFMMonitorFolders_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			 {
				 if (e->Button != System::Windows::Forms::MouseButtons::Right)
					 return;
/*

				 lstFMMonitorFolders->ClearSelected();
				 lstFMMonitorFolders->SelectedIndex = lstFMMonitorFolders->IndexFromPoint(Point(e->X,e->Y));

				 int p;
				 if ((p = lstFMMonitorFolders->SelectedIndex) == -1)
					 return;

				 Point^ pt = lstFMMonitorFolders->PointToScreen(Point(e->X, e->Y));
				 RightClickOnFolder(lstFMMonitorFolders->Items[p]->ToString(),pt);
				 */
			 }
			 System::Void lstFMIgnoreFolders_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			 {
				 if (e->Button != System::Windows::Forms::MouseButtons::Right)
					 return;
/*
				 lstFMIgnoreFolders->ClearSelected();
				 lstFMIgnoreFolders->SelectedIndex = lstFMIgnoreFolders->IndexFromPoint(Point(e->X,e->Y));

				 int p;
				 if ((p = lstFMIgnoreFolders->SelectedIndex) == -1)
					 return;

				 Point^ pt = lstFMIgnoreFolders->PointToScreen(Point(e->X, e->Y));
				 RightClickOnFolder(lstFMIgnoreFolders->Items[p]->ToString(),pt);
				 */
			 }

};
}
