#pragma once

#include "TVDoc.h"
//#include "Server.h"
#include "TheTVDBCodeFinder.h"
#include "AddEditShow.h"
#include "BuyMeADrink.h"
#include "UpcomingPopup.h"
#include "Preferences.h"
#include "StatsWindow.h"
#include "BugReport.h"
#include "EditRules.h"
#include "CustomNameDesigner.h"
#include "CustomName.h"
#include "AddEditSearchEngine.h"
#include "AddEditSeasEpFinders.h"
#include "Version.h"
#include "uTorrent.h"
#include "FolderMonitor.h"
#include "TorrentMatch.h"
#include "MyListView.h"
#include "ActorsGrid.h"
#include "RecoverXML.h"
#include "IgnoreEdit.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Xml;

namespace TVRename {
	// right click commands
	enum { kEpisodeGuideForShow = 1, kVisitTVDBEpisode, kVisitTVDBSeason, 
		kVisitTVDBSeries, kScanSpecificSeries, kWhenToWatchSeries, 
		kForceRefreshSeries, kBTSearchFor,
		kAIOIgnore, kAIOBrowseForFile, kAIOAction, kAIODelete,
		kWatchBase = 1000,
		kOpenFolderBase = 2000 };

	/// <summary>
	/// Summary for UI
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class UI : public System::Windows::Forms::Form
	{
	public:
		SetProgressDelegate ^SetProgress;

	private: System::Windows::Forms::Button^  bnMyShowsCollapse;
	private: System::Windows::Forms::TabPage^  tbAllInOne;
	private: System::Windows::Forms::Button^  bnAIOCheck;


	private: TVRename::MyListView^  lvAIO;



	private: System::Windows::Forms::Button^  bnAIOAction;
	private: System::Windows::Forms::ColumnHeader^  columnHeader48;
	private: System::Windows::Forms::ColumnHeader^  columnHeader49;

	private: System::Windows::Forms::ColumnHeader^  columnHeader51;
	private: System::Windows::Forms::ColumnHeader^  columnHeader52;
	private: System::Windows::Forms::ColumnHeader^  columnHeader53;
	private: System::Windows::Forms::ColumnHeader^  columnHeader54;
	private: System::Windows::Forms::ColumnHeader^  columnHeader55;
	private: System::Windows::Forms::ColumnHeader^  columnHeader56;




	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator3;
	private: System::Windows::Forms::ToolStripMenuItem^  folderMonitorToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator4;
	private: System::Windows::Forms::ToolStripMenuItem^  torrentMatchToolStripMenuItem;

	private: System::Windows::Forms::ColumnHeader^  columnHeader58;
	private: System::Windows::Forms::Button^  bnAIOWhichSearch;
	private: System::Windows::Forms::Button^  bnAIOBTSearch;
	private: System::Windows::Forms::Button^  bnAIOIgnore;
	private: System::Windows::Forms::Label^  label1;
	private: System::Windows::Forms::Button^  bnAIODownloads;

	private: System::Windows::Forms::Button^  bnAIORSS;

	private: System::Windows::Forms::Button^  bnAIOCopyMove;

	private: System::Windows::Forms::Button^  bnAIORename;










	private: System::Windows::Forms::Button^  bnAIOAllNone;
	private: System::Windows::Forms::Button^  bnAIONFO;
	private: System::Windows::Forms::Button^  bnAIOOptions;
	private: System::Windows::Forms::Button^  bnRemoveSel;
	private: System::Windows::Forms::ToolStripMenuItem^  ignoreListToolStripMenuItem;




	private: static bool ExperimentalFeatures;

	public: 

		static bool IncludeExperimentalStuff()
		{
			return ExperimentalFeatures;
		}

	protected: 
		TVDoc ^mDoc;
		System::Drawing::Size mLastNonMaximizedSize;
		Point mLastNonMaximizedLocation;
		int mInternalChange;
		int Busy;

		ProcessedEpisode ^mLastEpClicked;
		Season ^mLastSeasonClicked;
		AIOItem ^mLastAIOClicked;
		ShowItem ^mLastShowClicked;
		StringList ^mFoldersToOpen;
		FileList ^mLastFL;
		String ^mLastFolderClicked;

	private: System::Windows::Forms::MenuStrip^  menuStrip1;
	private: System::Windows::Forms::ToolStripMenuItem^  fileToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  exitToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  helpToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  visitWebsiteToolStripMenuItem;
	private: System::Windows::Forms::TabControl^  tabControl1;
	private: System::Windows::Forms::TabPage^  tbWTW;
	private: System::Windows::Forms::ColumnHeader^  columnHeader5;
	private: System::Windows::Forms::ColumnHeader^  columnHeader6;
	private: System::Windows::Forms::ColumnHeader^  columnHeader7;
	private: System::Windows::Forms::ColumnHeader^  columnHeader8;
	private: System::Windows::Forms::ListView^  lvWhenToWatch;
	private: System::Windows::Forms::ColumnHeader^  columnHeader29;
	private: System::Windows::Forms::ColumnHeader^  columnHeader30;
	private: System::Windows::Forms::ColumnHeader^  columnHeader31;
	private: System::Windows::Forms::ColumnHeader^  columnHeader32;
	private: System::Windows::Forms::ColumnHeader^  columnHeader33;
	private: System::Windows::Forms::ColumnHeader^  columnHeader34;
	private: System::Windows::Forms::ColumnHeader^  columnHeader35;
	private: System::Windows::Forms::ColumnHeader^  columnHeader25;
	private: System::Windows::Forms::ColumnHeader^  columnHeader26;
	private: System::Windows::Forms::ColumnHeader^  columnHeader27;
	private: System::Windows::Forms::ColumnHeader^  columnHeader28;
	private: System::Windows::Forms::TextBox^  txtWhenToWatchSynopsis;
	private: System::Windows::Forms::MonthCalendar^  calCalendar;
	private: System::Windows::Forms::OpenFileDialog^  openFile;
	private: System::Windows::Forms::FolderBrowserDialog^  folderBrowser;
	private: System::Windows::Forms::Button^  bnWhenToWatchCheck;
	private: System::Windows::Forms::ContextMenuStrip^  menuSearchSites;
	private: System::Windows::Forms::Timer^  refreshWTWTimer;
	private: System::Windows::Forms::Button^  bnWTWChooseSite;
	private: System::Windows::Forms::Button^  bnWTWBTSearch;
	private: System::Windows::Forms::NotifyIcon^  notifyIcon1;
	private: System::Windows::Forms::ColumnHeader^  columnHeader36;
	private: System::Windows::Forms::ToolStripMenuItem^  buyMeADrinkToolStripMenuItem;
	private: System::Windows::Forms::ContextMenuStrip^  showRightClickMenu;
	private: System::Windows::Forms::ContextMenuStrip^  folderRightClickMenu;
	private: System::Windows::Forms::ToolStripMenuItem^  saveToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator1;
	private: System::Windows::Forms::Timer^  statusTimer;
	private: System::Windows::Forms::ToolStripMenuItem^  optionsToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  backgroundDownloadToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^  toolStripSeparator2;
	private: System::Windows::Forms::ToolStripMenuItem^  preferencesToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  offlineOperationToolStripMenuItem;
	private: System::Windows::Forms::Label^  tsNextShowTxt;
	private: System::Windows::Forms::Label^  txtDLStatusLabel;
	private: System::Windows::Forms::ProgressBar^  pbProgressBarx;
	private: System::Windows::Forms::Timer^  BGDownloadTimer;
	private: System::Windows::Forms::ToolStripMenuItem^  bugReportToolStripMenuItem;
	private: System::Windows::Forms::ImageList^  ilIcons;
	private: System::Windows::Forms::ToolStripMenuItem^  exportToolStripMenuItem;
	private: System::Windows::Forms::SaveFileDialog^  saveFile;
	private: System::Windows::Forms::ToolStripMenuItem^  toolsToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  flushCacheToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  backgroundDownloadNowToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  statisticsToolStripMenuItem;
	private: System::Windows::Forms::TabPage^  tbMyShows;
	private: System::Windows::Forms::Button^  bnMyShowsAdd;
	private: System::Windows::Forms::TreeView^  MyShowTree;
	private: System::Windows::Forms::WebBrowser^  epGuideHTML;
	private: System::Windows::Forms::Button^  bnMyShowsRefresh;
	private: System::Windows::Forms::Button^  bnMyShowsDelete;
	private: System::Windows::Forms::Button^  bnMyShowsEdit;
	private: System::Windows::Forms::Button^  bnMyShowsVisitTVDB;
	private: System::Windows::Forms::Button^  bnMyShowsOpenFolder;
	private: System::Windows::Forms::ToolStripMenuItem^  quickstartGuideToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  filenameTemplateEditorToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  searchEnginesToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  filenameProcessorsToolStripMenuItem;
	private: System::Windows::Forms::TableLayoutPanel^  tableLayoutPanel2;

	private: System::Windows::Forms::Timer^  tmrShowUpcomingPopup;
	private: System::Windows::Forms::ToolStripMenuItem^  actorsToolStripMenuItem;
	private: System::Windows::Forms::Timer^  quickTimer;
	private: System::Windows::Forms::ToolStripMenuItem^  uTorrentToolStripMenuItem;
	private: System::ComponentModel::IContainer^  components;
	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>

	public:

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(UI::typeid));
			System::Windows::Forms::ListViewGroup^  listViewGroup13 = (gcnew System::Windows::Forms::ListViewGroup(L"Missing", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup14 = (gcnew System::Windows::Forms::ListViewGroup(L"Rename", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup15 = (gcnew System::Windows::Forms::ListViewGroup(L"Copy", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup16 = (gcnew System::Windows::Forms::ListViewGroup(L"Move", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup17 = (gcnew System::Windows::Forms::ListViewGroup(L"Download RSS", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup18 = (gcnew System::Windows::Forms::ListViewGroup(L"Download", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup19 = (gcnew System::Windows::Forms::ListViewGroup(L"NFO File", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup20 = (gcnew System::Windows::Forms::ListViewGroup(L"Downloading In µTorrent", 
				System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup1 = (gcnew System::Windows::Forms::ListViewGroup(L"Recently Aired", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup2 = (gcnew System::Windows::Forms::ListViewGroup(L"Next 7 Days", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup3 = (gcnew System::Windows::Forms::ListViewGroup(L"Later", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup4 = (gcnew System::Windows::Forms::ListViewGroup(L"Future Episodes", System::Windows::Forms::HorizontalAlignment::Left));
			this->menuStrip1 = (gcnew System::Windows::Forms::MenuStrip());
			this->fileToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->exportToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->saveToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator1 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->exitToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->optionsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->offlineOperationToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->backgroundDownloadToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator2 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->preferencesToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->filenameTemplateEditorToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->searchEnginesToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->filenameProcessorsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->flushCacheToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->backgroundDownloadNowToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator3 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->folderMonitorToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->actorsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator4 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->torrentMatchToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->uTorrentToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->helpToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->bugReportToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->buyMeADrinkToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->visitWebsiteToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->quickstartGuideToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->statisticsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->tabControl1 = (gcnew System::Windows::Forms::TabControl());
			this->tbMyShows = (gcnew System::Windows::Forms::TabPage());
			this->bnMyShowsCollapse = (gcnew System::Windows::Forms::Button());
			this->bnMyShowsVisitTVDB = (gcnew System::Windows::Forms::Button());
			this->bnMyShowsOpenFolder = (gcnew System::Windows::Forms::Button());
			this->bnMyShowsRefresh = (gcnew System::Windows::Forms::Button());
			this->epGuideHTML = (gcnew System::Windows::Forms::WebBrowser());
			this->MyShowTree = (gcnew System::Windows::Forms::TreeView());
			this->bnMyShowsDelete = (gcnew System::Windows::Forms::Button());
			this->bnMyShowsEdit = (gcnew System::Windows::Forms::Button());
			this->bnMyShowsAdd = (gcnew System::Windows::Forms::Button());
			this->tbAllInOne = (gcnew System::Windows::Forms::TabPage());
			this->bnAIOOptions = (gcnew System::Windows::Forms::Button());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->bnAIONFO = (gcnew System::Windows::Forms::Button());
			this->bnAIODownloads = (gcnew System::Windows::Forms::Button());
			this->bnAIORSS = (gcnew System::Windows::Forms::Button());
			this->bnAIOCopyMove = (gcnew System::Windows::Forms::Button());
			this->bnAIORename = (gcnew System::Windows::Forms::Button());
			this->bnAIOAllNone = (gcnew System::Windows::Forms::Button());
			this->bnRemoveSel = (gcnew System::Windows::Forms::Button());
			this->bnAIOIgnore = (gcnew System::Windows::Forms::Button());
			this->bnAIOWhichSearch = (gcnew System::Windows::Forms::Button());
			this->bnAIOBTSearch = (gcnew System::Windows::Forms::Button());
			this->lvAIO = (gcnew TVRename::MyListView());
			this->columnHeader48 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader49 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader51 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader52 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader53 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader54 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader55 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader56 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader58 = (gcnew System::Windows::Forms::ColumnHeader());
			this->ilIcons = (gcnew System::Windows::Forms::ImageList(this->components));
			this->bnAIOAction = (gcnew System::Windows::Forms::Button());
			this->bnAIOCheck = (gcnew System::Windows::Forms::Button());
			this->tbWTW = (gcnew System::Windows::Forms::TabPage());
			this->bnWTWChooseSite = (gcnew System::Windows::Forms::Button());
			this->bnWTWBTSearch = (gcnew System::Windows::Forms::Button());
			this->bnWhenToWatchCheck = (gcnew System::Windows::Forms::Button());
			this->txtWhenToWatchSynopsis = (gcnew System::Windows::Forms::TextBox());
			this->calCalendar = (gcnew System::Windows::Forms::MonthCalendar());
			this->lvWhenToWatch = (gcnew System::Windows::Forms::ListView());
			this->columnHeader29 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader30 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader31 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader32 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader36 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader33 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader34 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader35 = (gcnew System::Windows::Forms::ColumnHeader());
			this->tableLayoutPanel2 = (gcnew System::Windows::Forms::TableLayoutPanel());
			this->pbProgressBarx = (gcnew System::Windows::Forms::ProgressBar());
			this->txtDLStatusLabel = (gcnew System::Windows::Forms::Label());
			this->tsNextShowTxt = (gcnew System::Windows::Forms::Label());
			this->columnHeader5 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader6 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader7 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader8 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader25 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader26 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader27 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader28 = (gcnew System::Windows::Forms::ColumnHeader());
			this->openFile = (gcnew System::Windows::Forms::OpenFileDialog());
			this->folderBrowser = (gcnew System::Windows::Forms::FolderBrowserDialog());
			this->menuSearchSites = (gcnew System::Windows::Forms::ContextMenuStrip(this->components));
			this->refreshWTWTimer = (gcnew System::Windows::Forms::Timer(this->components));
			this->notifyIcon1 = (gcnew System::Windows::Forms::NotifyIcon(this->components));
			this->showRightClickMenu = (gcnew System::Windows::Forms::ContextMenuStrip(this->components));
			this->folderRightClickMenu = (gcnew System::Windows::Forms::ContextMenuStrip(this->components));
			this->statusTimer = (gcnew System::Windows::Forms::Timer(this->components));
			this->BGDownloadTimer = (gcnew System::Windows::Forms::Timer(this->components));
			this->saveFile = (gcnew System::Windows::Forms::SaveFileDialog());
			this->tmrShowUpcomingPopup = (gcnew System::Windows::Forms::Timer(this->components));
			this->quickTimer = (gcnew System::Windows::Forms::Timer(this->components));
			this->ignoreListToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->menuStrip1->SuspendLayout();
			this->tabControl1->SuspendLayout();
			this->tbMyShows->SuspendLayout();
			this->tbAllInOne->SuspendLayout();
			this->tbWTW->SuspendLayout();
			this->tableLayoutPanel2->SuspendLayout();
			this->SuspendLayout();
			// 
			// menuStrip1
			// 
			this->menuStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(4) {this->fileToolStripMenuItem, 
				this->optionsToolStripMenuItem, this->toolsToolStripMenuItem, this->helpToolStripMenuItem});
			this->menuStrip1->Location = System::Drawing::Point(0, 0);
			this->menuStrip1->Name = L"menuStrip1";
			this->menuStrip1->Size = System::Drawing::Size(931, 24);
			this->menuStrip1->TabIndex = 0;
			this->menuStrip1->Text = L"menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this->fileToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(4) {this->exportToolStripMenuItem, 
				this->saveToolStripMenuItem, this->toolStripSeparator1, this->exitToolStripMenuItem});
			this->fileToolStripMenuItem->Name = L"fileToolStripMenuItem";
			this->fileToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Alt | System::Windows::Forms::Keys::F4));
			this->fileToolStripMenuItem->Size = System::Drawing::Size(35, 20);
			this->fileToolStripMenuItem->Text = L"&File";
			// 
			// exportToolStripMenuItem
			// 
			this->exportToolStripMenuItem->Name = L"exportToolStripMenuItem";
			this->exportToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::E));
			this->exportToolStripMenuItem->Size = System::Drawing::Size(152, 22);
			this->exportToolStripMenuItem->Text = L"&Export";
			this->exportToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::exportToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this->saveToolStripMenuItem->Name = L"saveToolStripMenuItem";
			this->saveToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::S));
			this->saveToolStripMenuItem->Size = System::Drawing::Size(152, 22);
			this->saveToolStripMenuItem->Text = L"&Save";
			this->saveToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::saveToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this->toolStripSeparator1->Name = L"toolStripSeparator1";
			this->toolStripSeparator1->Size = System::Drawing::Size(149, 6);
			// 
			// exitToolStripMenuItem
			// 
			this->exitToolStripMenuItem->Name = L"exitToolStripMenuItem";
			this->exitToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Alt | System::Windows::Forms::Keys::F4));
			this->exitToolStripMenuItem->Size = System::Drawing::Size(152, 22);
			this->exitToolStripMenuItem->Text = L"E&xit";
			this->exitToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::exitToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this->optionsToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(8) {this->offlineOperationToolStripMenuItem, 
				this->backgroundDownloadToolStripMenuItem, this->toolStripSeparator2, this->preferencesToolStripMenuItem, this->ignoreListToolStripMenuItem, 
				this->filenameTemplateEditorToolStripMenuItem, this->searchEnginesToolStripMenuItem, this->filenameProcessorsToolStripMenuItem});
			this->optionsToolStripMenuItem->Name = L"optionsToolStripMenuItem";
			this->optionsToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::O));
			this->optionsToolStripMenuItem->Size = System::Drawing::Size(56, 20);
			this->optionsToolStripMenuItem->Text = L"&Options";
			// 
			// offlineOperationToolStripMenuItem
			// 
			this->offlineOperationToolStripMenuItem->Name = L"offlineOperationToolStripMenuItem";
			this->offlineOperationToolStripMenuItem->Size = System::Drawing::Size(232, 22);
			this->offlineOperationToolStripMenuItem->Text = L"&Offline Operation";
			this->offlineOperationToolStripMenuItem->ToolTipText = L"If you turn this on, TVRename will only use data it has locally, without download" 
				L"ing anything.";
			this->offlineOperationToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::offlineOperationToolStripMenuItem_Click);
			// 
			// backgroundDownloadToolStripMenuItem
			// 
			this->backgroundDownloadToolStripMenuItem->Name = L"backgroundDownloadToolStripMenuItem";
			this->backgroundDownloadToolStripMenuItem->Size = System::Drawing::Size(232, 22);
			this->backgroundDownloadToolStripMenuItem->Text = L"Automatic &Background Download";
			this->backgroundDownloadToolStripMenuItem->ToolTipText = L"Turn this on to let TVRename automatically download thetvdb.com data in the backr" 
				L"ground";
			this->backgroundDownloadToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::backgroundDownloadToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this->toolStripSeparator2->Name = L"toolStripSeparator2";
			this->toolStripSeparator2->Size = System::Drawing::Size(229, 6);
			// 
			// preferencesToolStripMenuItem
			// 
			this->preferencesToolStripMenuItem->Name = L"preferencesToolStripMenuItem";
			this->preferencesToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::P));
			this->preferencesToolStripMenuItem->Size = System::Drawing::Size(232, 22);
			this->preferencesToolStripMenuItem->Text = L"&Preferences";
			this->preferencesToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::preferencesToolStripMenuItem_Click);
			// 
			// filenameTemplateEditorToolStripMenuItem
			// 
			this->filenameTemplateEditorToolStripMenuItem->Name = L"filenameTemplateEditorToolStripMenuItem";
			this->filenameTemplateEditorToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::T));
			this->filenameTemplateEditorToolStripMenuItem->Size = System::Drawing::Size(232, 22);
			this->filenameTemplateEditorToolStripMenuItem->Text = L"&Filename Template Editor";
			this->filenameTemplateEditorToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::filenameTemplateEditorToolStripMenuItem_Click);
			// 
			// searchEnginesToolStripMenuItem
			// 
			this->searchEnginesToolStripMenuItem->Name = L"searchEnginesToolStripMenuItem";
			this->searchEnginesToolStripMenuItem->Size = System::Drawing::Size(232, 22);
			this->searchEnginesToolStripMenuItem->Text = L"&Search Engines";
			this->searchEnginesToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::searchEnginesToolStripMenuItem_Click);
			// 
			// filenameProcessorsToolStripMenuItem
			// 
			this->filenameProcessorsToolStripMenuItem->Name = L"filenameProcessorsToolStripMenuItem";
			this->filenameProcessorsToolStripMenuItem->Size = System::Drawing::Size(232, 22);
			this->filenameProcessorsToolStripMenuItem->Text = L"File&name Processors";
			this->filenameProcessorsToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::filenameProcessorsToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this->toolsToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(8) {this->flushCacheToolStripMenuItem, 
				this->backgroundDownloadNowToolStripMenuItem, this->toolStripSeparator3, this->folderMonitorToolStripMenuItem, this->actorsToolStripMenuItem, 
				this->toolStripSeparator4, this->torrentMatchToolStripMenuItem, this->uTorrentToolStripMenuItem});
			this->toolsToolStripMenuItem->Name = L"toolsToolStripMenuItem";
			this->toolsToolStripMenuItem->Size = System::Drawing::Size(44, 20);
			this->toolsToolStripMenuItem->Text = L"&Tools";
			// 
			// flushCacheToolStripMenuItem
			// 
			this->flushCacheToolStripMenuItem->Name = L"flushCacheToolStripMenuItem";
			this->flushCacheToolStripMenuItem->Size = System::Drawing::Size(242, 22);
			this->flushCacheToolStripMenuItem->Text = L"&Flush Cache";
			this->flushCacheToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::flushCacheToolStripMenuItem_Click);
			// 
			// backgroundDownloadNowToolStripMenuItem
			// 
			this->backgroundDownloadNowToolStripMenuItem->Name = L"backgroundDownloadNowToolStripMenuItem";
			this->backgroundDownloadNowToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::B));
			this->backgroundDownloadNowToolStripMenuItem->Size = System::Drawing::Size(242, 22);
			this->backgroundDownloadNowToolStripMenuItem->Text = L"&Background Download Now";
			this->backgroundDownloadNowToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::backgroundDownloadNowToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this->toolStripSeparator3->Name = L"toolStripSeparator3";
			this->toolStripSeparator3->Size = System::Drawing::Size(239, 6);
			// 
			// folderMonitorToolStripMenuItem
			// 
			this->folderMonitorToolStripMenuItem->Name = L"folderMonitorToolStripMenuItem";
			this->folderMonitorToolStripMenuItem->Size = System::Drawing::Size(242, 22);
			this->folderMonitorToolStripMenuItem->Text = L"Folder &Monitor";
			this->folderMonitorToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::folderMonitorToolStripMenuItem_Click);
			// 
			// actorsToolStripMenuItem
			// 
			this->actorsToolStripMenuItem->Name = L"actorsToolStripMenuItem";
			this->actorsToolStripMenuItem->Size = System::Drawing::Size(242, 22);
			this->actorsToolStripMenuItem->Text = L"&Actors Grid";
			this->actorsToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::actorsToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this->toolStripSeparator4->Name = L"toolStripSeparator4";
			this->toolStripSeparator4->Size = System::Drawing::Size(239, 6);
			// 
			// torrentMatchToolStripMenuItem
			// 
			this->torrentMatchToolStripMenuItem->Name = L"torrentMatchToolStripMenuItem";
			this->torrentMatchToolStripMenuItem->Size = System::Drawing::Size(242, 22);
			this->torrentMatchToolStripMenuItem->Text = L"&Torrent Match";
			this->torrentMatchToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::torrentMatchToolStripMenuItem_Click);
			// 
			// uTorrentToolStripMenuItem
			// 
			this->uTorrentToolStripMenuItem->Name = L"uTorrentToolStripMenuItem";
			this->uTorrentToolStripMenuItem->Size = System::Drawing::Size(242, 22);
			this->uTorrentToolStripMenuItem->Text = L"&uTorrent Save To";
			this->uTorrentToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::uTorrentToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this->helpToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(5) {this->bugReportToolStripMenuItem, 
				this->buyMeADrinkToolStripMenuItem, this->visitWebsiteToolStripMenuItem, this->quickstartGuideToolStripMenuItem, this->statisticsToolStripMenuItem});
			this->helpToolStripMenuItem->Name = L"helpToolStripMenuItem";
			this->helpToolStripMenuItem->Size = System::Drawing::Size(40, 20);
			this->helpToolStripMenuItem->Text = L"&Help";
			// 
			// bugReportToolStripMenuItem
			// 
			this->bugReportToolStripMenuItem->Name = L"bugReportToolStripMenuItem";
			this->bugReportToolStripMenuItem->Size = System::Drawing::Size(153, 22);
			this->bugReportToolStripMenuItem->Text = L"Bug &Report";
			this->bugReportToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::bugReportToolStripMenuItem_Click);
			// 
			// buyMeADrinkToolStripMenuItem
			// 
			this->buyMeADrinkToolStripMenuItem->Name = L"buyMeADrinkToolStripMenuItem";
			this->buyMeADrinkToolStripMenuItem->Size = System::Drawing::Size(153, 22);
			this->buyMeADrinkToolStripMenuItem->Text = L"&Buy Me A Drink";
			this->buyMeADrinkToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::buyMeADrinkToolStripMenuItem_Click);
			// 
			// visitWebsiteToolStripMenuItem
			// 
			this->visitWebsiteToolStripMenuItem->Name = L"visitWebsiteToolStripMenuItem";
			this->visitWebsiteToolStripMenuItem->Size = System::Drawing::Size(153, 22);
			this->visitWebsiteToolStripMenuItem->Text = L"&Visit Website";
			this->visitWebsiteToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::visitWebsiteToolStripMenuItem_Click);
			// 
			// quickstartGuideToolStripMenuItem
			// 
			this->quickstartGuideToolStripMenuItem->Name = L"quickstartGuideToolStripMenuItem";
			this->quickstartGuideToolStripMenuItem->Size = System::Drawing::Size(153, 22);
			this->quickstartGuideToolStripMenuItem->Text = L"&Quickstart Guide";
			this->quickstartGuideToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::quickstartGuideToolStripMenuItem_Click);
			// 
			// statisticsToolStripMenuItem
			// 
			this->statisticsToolStripMenuItem->Name = L"statisticsToolStripMenuItem";
			this->statisticsToolStripMenuItem->Size = System::Drawing::Size(153, 22);
			this->statisticsToolStripMenuItem->Text = L"&Statistics";
			this->statisticsToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::statisticsToolStripMenuItem_Click);
			// 
			// tabControl1
			// 
			this->tabControl1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->tabControl1->Controls->Add(this->tbMyShows);
			this->tabControl1->Controls->Add(this->tbAllInOne);
			this->tabControl1->Controls->Add(this->tbWTW);
			this->tabControl1->Location = System::Drawing::Point(0, 24);
			this->tabControl1->Name = L"tabControl1";
			this->tabControl1->SelectedIndex = 0;
			this->tabControl1->Size = System::Drawing::Size(931, 533);
			this->tabControl1->TabIndex = 0;
			this->tabControl1->DoubleClick += gcnew System::EventHandler(this, &UI::tabControl1_DoubleClick);
			this->tabControl1->SelectedIndexChanged += gcnew System::EventHandler(this, &UI::tabControl1_SelectedIndexChanged);
			// 
			// tbMyShows
			// 
			this->tbMyShows->Controls->Add(this->bnMyShowsCollapse);
			this->tbMyShows->Controls->Add(this->bnMyShowsVisitTVDB);
			this->tbMyShows->Controls->Add(this->bnMyShowsOpenFolder);
			this->tbMyShows->Controls->Add(this->bnMyShowsRefresh);
			this->tbMyShows->Controls->Add(this->epGuideHTML);
			this->tbMyShows->Controls->Add(this->MyShowTree);
			this->tbMyShows->Controls->Add(this->bnMyShowsDelete);
			this->tbMyShows->Controls->Add(this->bnMyShowsEdit);
			this->tbMyShows->Controls->Add(this->bnMyShowsAdd);
			this->tbMyShows->Location = System::Drawing::Point(4, 22);
			this->tbMyShows->Name = L"tbMyShows";
			this->tbMyShows->Padding = System::Windows::Forms::Padding(3);
			this->tbMyShows->Size = System::Drawing::Size(923, 507);
			this->tbMyShows->TabIndex = 9;
			this->tbMyShows->Text = L"My Shows";
			this->tbMyShows->UseVisualStyleBackColor = true;
			// 
			// bnMyShowsCollapse
			// 
			this->bnMyShowsCollapse->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnMyShowsCollapse->Location = System::Drawing::Point(251, 480);
			this->bnMyShowsCollapse->Name = L"bnMyShowsCollapse";
			this->bnMyShowsCollapse->Size = System::Drawing::Size(22, 23);
			this->bnMyShowsCollapse->TabIndex = 4;
			this->bnMyShowsCollapse->Text = L"-";
			this->bnMyShowsCollapse->UseVisualStyleBackColor = true;
			this->bnMyShowsCollapse->Click += gcnew System::EventHandler(this, &UI::bnMyShowsCollapse_Click);
			// 
			// bnMyShowsVisitTVDB
			// 
			this->bnMyShowsVisitTVDB->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnMyShowsVisitTVDB->Location = System::Drawing::Point(506, 480);
			this->bnMyShowsVisitTVDB->Name = L"bnMyShowsVisitTVDB";
			this->bnMyShowsVisitTVDB->Size = System::Drawing::Size(75, 23);
			this->bnMyShowsVisitTVDB->TabIndex = 7;
			this->bnMyShowsVisitTVDB->Text = L"&Visit TVDB";
			this->bnMyShowsVisitTVDB->UseVisualStyleBackColor = true;
			this->bnMyShowsVisitTVDB->Click += gcnew System::EventHandler(this, &UI::bnMyShowsVisitTVDB_Click);
			// 
			// bnMyShowsOpenFolder
			// 
			this->bnMyShowsOpenFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnMyShowsOpenFolder->Location = System::Drawing::Point(425, 480);
			this->bnMyShowsOpenFolder->Name = L"bnMyShowsOpenFolder";
			this->bnMyShowsOpenFolder->Size = System::Drawing::Size(75, 23);
			this->bnMyShowsOpenFolder->TabIndex = 6;
			this->bnMyShowsOpenFolder->Text = L"&Open";
			this->bnMyShowsOpenFolder->UseVisualStyleBackColor = true;
			this->bnMyShowsOpenFolder->Click += gcnew System::EventHandler(this, &UI::bnMyShowsOpenFolder_Click);
			// 
			// bnMyShowsRefresh
			// 
			this->bnMyShowsRefresh->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnMyShowsRefresh->Location = System::Drawing::Point(331, 480);
			this->bnMyShowsRefresh->Name = L"bnMyShowsRefresh";
			this->bnMyShowsRefresh->Size = System::Drawing::Size(75, 23);
			this->bnMyShowsRefresh->TabIndex = 5;
			this->bnMyShowsRefresh->Text = L"&Refresh";
			this->bnMyShowsRefresh->UseVisualStyleBackColor = true;
			this->bnMyShowsRefresh->Click += gcnew System::EventHandler(this, &UI::bnMyShowsRefresh_Click);
			// 
			// epGuideHTML
			// 
			this->epGuideHTML->AllowWebBrowserDrop = false;
			this->epGuideHTML->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->epGuideHTML->Location = System::Drawing::Point(279, 6);
			this->epGuideHTML->MinimumSize = System::Drawing::Size(20, 20);
			this->epGuideHTML->Name = L"epGuideHTML";
			this->epGuideHTML->Size = System::Drawing::Size(644, 468);
			this->epGuideHTML->TabIndex = 6;
			this->epGuideHTML->WebBrowserShortcutsEnabled = false;
			this->epGuideHTML->Navigating += gcnew System::Windows::Forms::WebBrowserNavigatingEventHandler(this, &UI::epGuideHTML_Navigating);
			// 
			// MyShowTree
			// 
			this->MyShowTree->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left));
			this->MyShowTree->HideSelection = false;
			this->MyShowTree->Location = System::Drawing::Point(3, 6);
			this->MyShowTree->Name = L"MyShowTree";
			this->MyShowTree->Size = System::Drawing::Size(270, 468);
			this->MyShowTree->TabIndex = 0;
			this->MyShowTree->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::MyShowTree_MouseClick);
			this->MyShowTree->AfterSelect += gcnew System::Windows::Forms::TreeViewEventHandler(this, &UI::MyShowTree_AfterSelect);
			// 
			// bnMyShowsDelete
			// 
			this->bnMyShowsDelete->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnMyShowsDelete->Location = System::Drawing::Point(170, 480);
			this->bnMyShowsDelete->Name = L"bnMyShowsDelete";
			this->bnMyShowsDelete->Size = System::Drawing::Size(75, 23);
			this->bnMyShowsDelete->TabIndex = 3;
			this->bnMyShowsDelete->Text = L"&Delete";
			this->bnMyShowsDelete->UseVisualStyleBackColor = true;
			this->bnMyShowsDelete->Click += gcnew System::EventHandler(this, &UI::bnMyShowsDelete_Click);
			// 
			// bnMyShowsEdit
			// 
			this->bnMyShowsEdit->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnMyShowsEdit->Location = System::Drawing::Point(89, 480);
			this->bnMyShowsEdit->Name = L"bnMyShowsEdit";
			this->bnMyShowsEdit->Size = System::Drawing::Size(75, 23);
			this->bnMyShowsEdit->TabIndex = 2;
			this->bnMyShowsEdit->Text = L"&Edit";
			this->bnMyShowsEdit->UseVisualStyleBackColor = true;
			this->bnMyShowsEdit->Click += gcnew System::EventHandler(this, &UI::bnMyShowsEdit_Click);
			// 
			// bnMyShowsAdd
			// 
			this->bnMyShowsAdd->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnMyShowsAdd->Location = System::Drawing::Point(8, 480);
			this->bnMyShowsAdd->Name = L"bnMyShowsAdd";
			this->bnMyShowsAdd->Size = System::Drawing::Size(75, 23);
			this->bnMyShowsAdd->TabIndex = 1;
			this->bnMyShowsAdd->Text = L"&Add";
			this->bnMyShowsAdd->UseVisualStyleBackColor = true;
			this->bnMyShowsAdd->Click += gcnew System::EventHandler(this, &UI::bnMyShowsAdd_Click);
			// 
			// tbAllInOne
			// 
			this->tbAllInOne->Controls->Add(this->bnAIOOptions);
			this->tbAllInOne->Controls->Add(this->label1);
			this->tbAllInOne->Controls->Add(this->bnAIONFO);
			this->tbAllInOne->Controls->Add(this->bnAIODownloads);
			this->tbAllInOne->Controls->Add(this->bnAIORSS);
			this->tbAllInOne->Controls->Add(this->bnAIOCopyMove);
			this->tbAllInOne->Controls->Add(this->bnAIORename);
			this->tbAllInOne->Controls->Add(this->bnAIOAllNone);
			this->tbAllInOne->Controls->Add(this->bnRemoveSel);
			this->tbAllInOne->Controls->Add(this->bnAIOIgnore);
			this->tbAllInOne->Controls->Add(this->bnAIOWhichSearch);
			this->tbAllInOne->Controls->Add(this->bnAIOBTSearch);
			this->tbAllInOne->Controls->Add(this->lvAIO);
			this->tbAllInOne->Controls->Add(this->bnAIOAction);
			this->tbAllInOne->Controls->Add(this->bnAIOCheck);
			this->tbAllInOne->Location = System::Drawing::Point(4, 22);
			this->tbAllInOne->Name = L"tbAllInOne";
			this->tbAllInOne->Padding = System::Windows::Forms::Padding(3);
			this->tbAllInOne->Size = System::Drawing::Size(923, 507);
			this->tbAllInOne->TabIndex = 11;
			this->tbAllInOne->Text = L"Scan";
			this->tbAllInOne->UseVisualStyleBackColor = true;
			// 
			// bnAIOOptions
			// 
			this->bnAIOOptions->Location = System::Drawing::Point(89, 6);
			this->bnAIOOptions->Name = L"bnAIOOptions";
			this->bnAIOOptions->Size = System::Drawing::Size(75, 23);
			this->bnAIOOptions->TabIndex = 8;
			this->bnAIOOptions->Text = L"&Options...";
			this->bnAIOOptions->UseVisualStyleBackColor = true;
			this->bnAIOOptions->Click += gcnew System::EventHandler(this, &UI::bnAIOOptions_Click);
			// 
			// label1
			// 
			this->label1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(393, 482);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(41, 13);
			this->label1->TabIndex = 7;
			this->label1->Text = L"Check:";
			// 
			// bnAIONFO
			// 
			this->bnAIONFO->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnAIONFO->Location = System::Drawing::Point(845, 477);
			this->bnAIONFO->Name = L"bnAIONFO";
			this->bnAIONFO->Size = System::Drawing::Size(75, 23);
			this->bnAIONFO->TabIndex = 6;
			this->bnAIONFO->Text = L"NFO";
			this->bnAIONFO->UseVisualStyleBackColor = true;
			this->bnAIONFO->Click += gcnew System::EventHandler(this, &UI::bnAIONFO_Click);
			// 
			// bnAIODownloads
			// 
			this->bnAIODownloads->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnAIODownloads->Location = System::Drawing::Point(764, 477);
			this->bnAIODownloads->Name = L"bnAIODownloads";
			this->bnAIODownloads->Size = System::Drawing::Size(75, 23);
			this->bnAIODownloads->TabIndex = 6;
			this->bnAIODownloads->Text = L"Downloads";
			this->bnAIODownloads->UseVisualStyleBackColor = true;
			this->bnAIODownloads->Click += gcnew System::EventHandler(this, &UI::bnAIODownloads_Click);
			// 
			// bnAIORSS
			// 
			this->bnAIORSS->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnAIORSS->Location = System::Drawing::Point(683, 477);
			this->bnAIORSS->Name = L"bnAIORSS";
			this->bnAIORSS->Size = System::Drawing::Size(75, 23);
			this->bnAIORSS->TabIndex = 6;
			this->bnAIORSS->Text = L"RSS";
			this->bnAIORSS->UseVisualStyleBackColor = true;
			this->bnAIORSS->Click += gcnew System::EventHandler(this, &UI::bnAIORSS_Click);
			// 
			// bnAIOCopyMove
			// 
			this->bnAIOCopyMove->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnAIOCopyMove->Location = System::Drawing::Point(602, 477);
			this->bnAIOCopyMove->Name = L"bnAIOCopyMove";
			this->bnAIOCopyMove->Size = System::Drawing::Size(75, 23);
			this->bnAIOCopyMove->TabIndex = 6;
			this->bnAIOCopyMove->Text = L"Copy/Move";
			this->bnAIOCopyMove->UseVisualStyleBackColor = true;
			this->bnAIOCopyMove->Click += gcnew System::EventHandler(this, &UI::bnAIOCopyMove_Click);
			// 
			// bnAIORename
			// 
			this->bnAIORename->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnAIORename->Location = System::Drawing::Point(521, 477);
			this->bnAIORename->Name = L"bnAIORename";
			this->bnAIORename->Size = System::Drawing::Size(75, 23);
			this->bnAIORename->TabIndex = 6;
			this->bnAIORename->Text = L"Rename";
			this->bnAIORename->UseVisualStyleBackColor = true;
			this->bnAIORename->Click += gcnew System::EventHandler(this, &UI::bnAIORename_Click);
			// 
			// bnAIOAllNone
			// 
			this->bnAIOAllNone->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnAIOAllNone->Location = System::Drawing::Point(440, 477);
			this->bnAIOAllNone->Name = L"bnAIOAllNone";
			this->bnAIOAllNone->Size = System::Drawing::Size(75, 23);
			this->bnAIOAllNone->TabIndex = 6;
			this->bnAIOAllNone->Text = L"All/None";
			this->bnAIOAllNone->UseVisualStyleBackColor = true;
			this->bnAIOAllNone->Click += gcnew System::EventHandler(this, &UI::bnAIOAllNone_Click);
			// 
			// bnRemoveSel
			// 
			this->bnRemoveSel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnRemoveSel->Location = System::Drawing::Point(269, 477);
			this->bnRemoveSel->Name = L"bnRemoveSel";
			this->bnRemoveSel->Size = System::Drawing::Size(75, 23);
			this->bnRemoveSel->TabIndex = 5;
			this->bnRemoveSel->Text = L"&Remove Sel";
			this->bnRemoveSel->UseVisualStyleBackColor = true;
			this->bnRemoveSel->Click += gcnew System::EventHandler(this, &UI::bnRemoveSel_Click);
			// 
			// bnAIOIgnore
			// 
			this->bnAIOIgnore->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnAIOIgnore->Location = System::Drawing::Point(188, 477);
			this->bnAIOIgnore->Name = L"bnAIOIgnore";
			this->bnAIOIgnore->Size = System::Drawing::Size(75, 23);
			this->bnAIOIgnore->TabIndex = 5;
			this->bnAIOIgnore->Text = L"&Ignore Sel";
			this->bnAIOIgnore->UseVisualStyleBackColor = true;
			this->bnAIOIgnore->Click += gcnew System::EventHandler(this, &UI::bnAIOIgnore_Click);
			// 
			// bnAIOWhichSearch
			// 
			this->bnAIOWhichSearch->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnAIOWhichSearch->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"bnAIOWhichSearch.Image")));
			this->bnAIOWhichSearch->Location = System::Drawing::Point(161, 477);
			this->bnAIOWhichSearch->Name = L"bnAIOWhichSearch";
			this->bnAIOWhichSearch->Size = System::Drawing::Size(19, 23);
			this->bnAIOWhichSearch->TabIndex = 4;
			this->bnAIOWhichSearch->UseVisualStyleBackColor = true;
			this->bnAIOWhichSearch->Click += gcnew System::EventHandler(this, &UI::bnAIOWhichSearch_Click);
			// 
			// bnAIOBTSearch
			// 
			this->bnAIOBTSearch->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnAIOBTSearch->Location = System::Drawing::Point(87, 477);
			this->bnAIOBTSearch->Name = L"bnAIOBTSearch";
			this->bnAIOBTSearch->Size = System::Drawing::Size(75, 23);
			this->bnAIOBTSearch->TabIndex = 3;
			this->bnAIOBTSearch->Text = L"BT S&earch";
			this->bnAIOBTSearch->UseVisualStyleBackColor = true;
			this->bnAIOBTSearch->Click += gcnew System::EventHandler(this, &UI::bnAIOBTSearch_Click);
			// 
			// lvAIO
			// 
			this->lvAIO->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvAIO->CheckBoxes = true;
			this->lvAIO->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(9) {this->columnHeader48, this->columnHeader49, 
				this->columnHeader51, this->columnHeader52, this->columnHeader53, this->columnHeader54, this->columnHeader55, this->columnHeader56, 
				this->columnHeader58});
			this->lvAIO->FullRowSelect = true;
			listViewGroup13->Header = L"Missing";
			listViewGroup13->Name = L"lvgAIOMissing";
			listViewGroup14->Header = L"Rename";
			listViewGroup14->Name = L"lvgAIORename";
			listViewGroup15->Header = L"Copy";
			listViewGroup15->Name = L"lvgAIOCopy";
			listViewGroup16->Header = L"Move";
			listViewGroup16->Name = L"lvgAIOMove";
			listViewGroup17->Header = L"Download RSS";
			listViewGroup17->Name = L"lvgAIODownloadRSS";
			listViewGroup18->Header = L"Download";
			listViewGroup18->Name = L"lvgAIODownload";
			listViewGroup19->Header = L"NFO File";
			listViewGroup19->Name = L"lvgAIONFO";
			listViewGroup20->Header = L"Downloading In µTorrent";
			listViewGroup20->Name = L"lngInuTorrent";
			this->lvAIO->Groups->AddRange(gcnew cli::array< System::Windows::Forms::ListViewGroup^  >(8) {listViewGroup13, listViewGroup14, 
				listViewGroup15, listViewGroup16, listViewGroup17, listViewGroup18, listViewGroup19, listViewGroup20});
			this->lvAIO->HeaderStyle = System::Windows::Forms::ColumnHeaderStyle::Nonclickable;
			this->lvAIO->HideSelection = false;
			this->lvAIO->Location = System::Drawing::Point(0, 35);
			this->lvAIO->Name = L"lvAIO";
			this->lvAIO->ShowItemToolTips = true;
			this->lvAIO->Size = System::Drawing::Size(920, 436);
			this->lvAIO->SmallImageList = this->ilIcons;
			this->lvAIO->TabIndex = 2;
			this->lvAIO->UseCompatibleStateImageBehavior = false;
			this->lvAIO->View = System::Windows::Forms::View::Details;
			this->lvAIO->MouseDoubleClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lvAIO_MouseDoubleClick);
			this->lvAIO->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lvAIO_MouseClick);
			this->lvAIO->SelectedIndexChanged += gcnew System::EventHandler(this, &UI::lvAIO_SelectedIndexChanged);
			this->lvAIO->ItemCheck += gcnew System::Windows::Forms::ItemCheckEventHandler(this, &UI::lvAIO_ItemCheck);
			this->lvAIO->RetrieveVirtualItem += gcnew System::Windows::Forms::RetrieveVirtualItemEventHandler(this, &UI::lvAIO_RetrieveVirtualItem);
			this->lvAIO->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &UI::lvAIO_KeyDown);
			// 
			// columnHeader48
			// 
			this->columnHeader48->Text = L"Show";
			this->columnHeader48->Width = 155;
			// 
			// columnHeader49
			// 
			this->columnHeader49->Text = L"Season";
			this->columnHeader49->Width = 50;
			// 
			// columnHeader51
			// 
			this->columnHeader51->Text = L"Episode";
			this->columnHeader51->Width = 50;
			// 
			// columnHeader52
			// 
			this->columnHeader52->Text = L"Date";
			this->columnHeader52->Width = 70;
			// 
			// columnHeader53
			// 
			this->columnHeader53->Text = L"Folder";
			this->columnHeader53->Width = 180;
			// 
			// columnHeader54
			// 
			this->columnHeader54->Text = L"Episode/Filename";
			this->columnHeader54->Width = 180;
			// 
			// columnHeader55
			// 
			this->columnHeader55->Text = L"Folder/Filename";
			this->columnHeader55->Width = 180;
			// 
			// columnHeader56
			// 
			this->columnHeader56->Text = L"Filename";
			this->columnHeader56->Width = 180;
			// 
			// columnHeader58
			// 
			this->columnHeader58->Text = L"Errors";
			this->columnHeader58->Width = 180;
			// 
			// ilIcons
			// 
			this->ilIcons->ImageStream = (cli::safe_cast<System::Windows::Forms::ImageListStreamer^  >(resources->GetObject(L"ilIcons.ImageStream")));
			this->ilIcons->TransparentColor = System::Drawing::Color::Transparent;
			this->ilIcons->Images->SetKeyName(0, L"OnDisk.bmp");
			this->ilIcons->Images->SetKeyName(1, L"MagGlass.bmp");
			this->ilIcons->Images->SetKeyName(2, L"uTorrent.bmp");
			this->ilIcons->Images->SetKeyName(3, L"copy.bmp");
			this->ilIcons->Images->SetKeyName(4, L"move.bmp");
			this->ilIcons->Images->SetKeyName(5, L"download.bmp");
			this->ilIcons->Images->SetKeyName(6, L"RSS.bmp");
			this->ilIcons->Images->SetKeyName(7, L"NFO.bmp");
			// 
			// bnAIOAction
			// 
			this->bnAIOAction->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnAIOAction->Location = System::Drawing::Point(6, 477);
			this->bnAIOAction->Name = L"bnAIOAction";
			this->bnAIOAction->Size = System::Drawing::Size(75, 23);
			this->bnAIOAction->TabIndex = 0;
			this->bnAIOAction->Text = L"&Do Checked";
			this->bnAIOAction->Click += gcnew System::EventHandler(this, &UI::bnAIOAction_Click);
			// 
			// bnAIOCheck
			// 
			this->bnAIOCheck->Location = System::Drawing::Point(8, 6);
			this->bnAIOCheck->Name = L"bnAIOCheck";
			this->bnAIOCheck->Size = System::Drawing::Size(75, 23);
			this->bnAIOCheck->TabIndex = 0;
			this->bnAIOCheck->Text = L"&Scan";
			this->bnAIOCheck->UseVisualStyleBackColor = true;
			this->bnAIOCheck->Click += gcnew System::EventHandler(this, &UI::bnAIOCheck_Click);
			// 
			// tbWTW
			// 
			this->tbWTW->Controls->Add(this->bnWTWChooseSite);
			this->tbWTW->Controls->Add(this->bnWTWBTSearch);
			this->tbWTW->Controls->Add(this->bnWhenToWatchCheck);
			this->tbWTW->Controls->Add(this->txtWhenToWatchSynopsis);
			this->tbWTW->Controls->Add(this->calCalendar);
			this->tbWTW->Controls->Add(this->lvWhenToWatch);
			this->tbWTW->Location = System::Drawing::Point(4, 22);
			this->tbWTW->Name = L"tbWTW";
			this->tbWTW->Size = System::Drawing::Size(923, 507);
			this->tbWTW->TabIndex = 4;
			this->tbWTW->Text = L"When to watch";
			this->tbWTW->UseVisualStyleBackColor = true;
			// 
			// bnWTWChooseSite
			// 
			this->bnWTWChooseSite->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"bnWTWChooseSite.Image")));
			this->bnWTWChooseSite->Location = System::Drawing::Point(163, 6);
			this->bnWTWChooseSite->Name = L"bnWTWChooseSite";
			this->bnWTWChooseSite->Size = System::Drawing::Size(19, 23);
			this->bnWTWChooseSite->TabIndex = 2;
			this->bnWTWChooseSite->UseVisualStyleBackColor = true;
			this->bnWTWChooseSite->Click += gcnew System::EventHandler(this, &UI::bnWTWChooseSite_Click);
			// 
			// bnWTWBTSearch
			// 
			this->bnWTWBTSearch->Location = System::Drawing::Point(89, 6);
			this->bnWTWBTSearch->Name = L"bnWTWBTSearch";
			this->bnWTWBTSearch->Size = System::Drawing::Size(75, 23);
			this->bnWTWBTSearch->TabIndex = 1;
			this->bnWTWBTSearch->Text = L"BT &Search";
			this->bnWTWBTSearch->UseVisualStyleBackColor = true;
			this->bnWTWBTSearch->Click += gcnew System::EventHandler(this, &UI::bnWTWBTSearch_Click);
			// 
			// bnWhenToWatchCheck
			// 
			this->bnWhenToWatchCheck->Location = System::Drawing::Point(8, 6);
			this->bnWhenToWatchCheck->Name = L"bnWhenToWatchCheck";
			this->bnWhenToWatchCheck->Size = System::Drawing::Size(75, 23);
			this->bnWhenToWatchCheck->TabIndex = 0;
			this->bnWhenToWatchCheck->Text = L"&Refresh";
			this->bnWhenToWatchCheck->UseVisualStyleBackColor = true;
			this->bnWhenToWatchCheck->Click += gcnew System::EventHandler(this, &UI::bnWhenToWatchCheck_Click);
			// 
			// txtWhenToWatchSynopsis
			// 
			this->txtWhenToWatchSynopsis->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->txtWhenToWatchSynopsis->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 10, System::Drawing::FontStyle::Regular, 
				System::Drawing::GraphicsUnit::Point, static_cast<System::Byte>(0)));
			this->txtWhenToWatchSynopsis->Location = System::Drawing::Point(0, 352);
			this->txtWhenToWatchSynopsis->Multiline = true;
			this->txtWhenToWatchSynopsis->Name = L"txtWhenToWatchSynopsis";
			this->txtWhenToWatchSynopsis->ReadOnly = true;
			this->txtWhenToWatchSynopsis->ScrollBars = System::Windows::Forms::ScrollBars::Vertical;
			this->txtWhenToWatchSynopsis->Size = System::Drawing::Size(733, 154);
			this->txtWhenToWatchSynopsis->TabIndex = 4;
			// 
			// calCalendar
			// 
			this->calCalendar->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->calCalendar->FirstDayOfWeek = System::Windows::Forms::Day::Sunday;
			this->calCalendar->Location = System::Drawing::Point(745, 352);
			this->calCalendar->MaxSelectionCount = 1;
			this->calCalendar->Name = L"calCalendar";
			this->calCalendar->TabIndex = 5;
			this->calCalendar->DateSelected += gcnew System::Windows::Forms::DateRangeEventHandler(this, &UI::calCalendar_DateSelected);
			// 
			// lvWhenToWatch
			// 
			this->lvWhenToWatch->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvWhenToWatch->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(8) {this->columnHeader29, 
				this->columnHeader30, this->columnHeader31, this->columnHeader32, this->columnHeader36, this->columnHeader33, this->columnHeader34, 
				this->columnHeader35});
			this->lvWhenToWatch->FullRowSelect = true;
			listViewGroup1->Header = L"Recently Aired";
			listViewGroup1->Name = L"justPassed";
			listViewGroup2->Header = L"Next 7 Days";
			listViewGroup2->Name = L"next7days";
			listViewGroup2->Tag = L"1";
			listViewGroup3->Header = L"Later";
			listViewGroup3->Name = L"later";
			listViewGroup3->Tag = L"2";
			listViewGroup4->Header = L"Future Episodes";
			listViewGroup4->Name = L"futureEps";
			this->lvWhenToWatch->Groups->AddRange(gcnew cli::array< System::Windows::Forms::ListViewGroup^  >(4) {listViewGroup1, listViewGroup2, 
				listViewGroup3, listViewGroup4});
			this->lvWhenToWatch->HideSelection = false;
			this->lvWhenToWatch->Location = System::Drawing::Point(0, 35);
			this->lvWhenToWatch->Name = L"lvWhenToWatch";
			this->lvWhenToWatch->ShowItemToolTips = true;
			this->lvWhenToWatch->Size = System::Drawing::Size(923, 311);
			this->lvWhenToWatch->SmallImageList = this->ilIcons;
			this->lvWhenToWatch->TabIndex = 3;
			this->lvWhenToWatch->UseCompatibleStateImageBehavior = false;
			this->lvWhenToWatch->View = System::Windows::Forms::View::Details;
			this->lvWhenToWatch->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lvWhenToWatch_MouseClick);
			this->lvWhenToWatch->SelectedIndexChanged += gcnew System::EventHandler(this, &UI::lvWhenToWatch_Click);
			this->lvWhenToWatch->DoubleClick += gcnew System::EventHandler(this, &UI::lvWhenToWatch_DoubleClick);
			this->lvWhenToWatch->ColumnClick += gcnew System::Windows::Forms::ColumnClickEventHandler(this, &UI::lvWhenToWatch_ColumnClick);
			// 
			// columnHeader29
			// 
			this->columnHeader29->Text = L"Show";
			this->columnHeader29->Width = 187;
			// 
			// columnHeader30
			// 
			this->columnHeader30->Text = L"Season";
			this->columnHeader30->Width = 51;
			// 
			// columnHeader31
			// 
			this->columnHeader31->Text = L"Episode";
			this->columnHeader31->Width = 55;
			// 
			// columnHeader32
			// 
			this->columnHeader32->Text = L"Air Date";
			this->columnHeader32->Width = 81;
			// 
			// columnHeader36
			// 
			this->columnHeader36->Text = L"Time";
			// 
			// columnHeader33
			// 
			this->columnHeader33->Text = L"Day";
			this->columnHeader33->Width = 42;
			// 
			// columnHeader34
			// 
			this->columnHeader34->Text = L"How Long";
			this->columnHeader34->Width = 69;
			// 
			// columnHeader35
			// 
			this->columnHeader35->Text = L"Episode Name";
			this->columnHeader35->Width = 360;
			// 
			// tableLayoutPanel2
			// 
			this->tableLayoutPanel2->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->tableLayoutPanel2->ColumnCount = 3;
			this->tableLayoutPanel2->ColumnStyles->Add((gcnew System::Windows::Forms::ColumnStyle(System::Windows::Forms::SizeType::Percent, 
				45)));
			this->tableLayoutPanel2->ColumnStyles->Add((gcnew System::Windows::Forms::ColumnStyle(System::Windows::Forms::SizeType::Percent, 
				40)));
			this->tableLayoutPanel2->ColumnStyles->Add((gcnew System::Windows::Forms::ColumnStyle(System::Windows::Forms::SizeType::Percent, 
				15)));
			this->tableLayoutPanel2->Controls->Add(this->pbProgressBarx, 2, 0);
			this->tableLayoutPanel2->Controls->Add(this->txtDLStatusLabel, 1, 0);
			this->tableLayoutPanel2->Controls->Add(this->tsNextShowTxt, 0, 0);
			this->tableLayoutPanel2->GrowStyle = System::Windows::Forms::TableLayoutPanelGrowStyle::FixedSize;
			this->tableLayoutPanel2->Location = System::Drawing::Point(0, 559);
			this->tableLayoutPanel2->Name = L"tableLayoutPanel2";
			this->tableLayoutPanel2->RowCount = 1;
			this->tableLayoutPanel2->RowStyles->Add((gcnew System::Windows::Forms::RowStyle(System::Windows::Forms::SizeType::Percent, 100)));
			this->tableLayoutPanel2->Size = System::Drawing::Size(919, 19);
			this->tableLayoutPanel2->TabIndex = 9;
			// 
			// pbProgressBarx
			// 
			this->pbProgressBarx->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->pbProgressBarx->Location = System::Drawing::Point(783, 3);
			this->pbProgressBarx->Name = L"pbProgressBarx";
			this->pbProgressBarx->Size = System::Drawing::Size(133, 13);
			this->pbProgressBarx->Step = 1;
			this->pbProgressBarx->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->pbProgressBarx->TabIndex = 0;
			// 
			// txtDLStatusLabel
			// 
			this->txtDLStatusLabel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->txtDLStatusLabel->Location = System::Drawing::Point(416, 6);
			this->txtDLStatusLabel->Name = L"txtDLStatusLabel";
			this->txtDLStatusLabel->Size = System::Drawing::Size(361, 13);
			this->txtDLStatusLabel->TabIndex = 1;
			this->txtDLStatusLabel->Text = L"Background Download: ---";
			this->txtDLStatusLabel->Visible = false;
			// 
			// tsNextShowTxt
			// 
			this->tsNextShowTxt->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->tsNextShowTxt->Location = System::Drawing::Point(3, 6);
			this->tsNextShowTxt->Name = L"tsNextShowTxt";
			this->tsNextShowTxt->Size = System::Drawing::Size(407, 13);
			this->tsNextShowTxt->TabIndex = 1;
			this->tsNextShowTxt->Text = L"---";
			this->tsNextShowTxt->UseMnemonic = false;
			// 
			// columnHeader5
			// 
			this->columnHeader5->Text = L"Show";
			this->columnHeader5->Width = 211;
			// 
			// columnHeader6
			// 
			this->columnHeader6->Text = L"Season";
			// 
			// columnHeader7
			// 
			this->columnHeader7->Text = L"thetvdb code";
			this->columnHeader7->Width = 82;
			// 
			// columnHeader8
			// 
			this->columnHeader8->Text = L"Show next airdate";
			this->columnHeader8->Width = 115;
			// 
			// columnHeader25
			// 
			this->columnHeader25->Text = L"From Folder";
			this->columnHeader25->Width = 187;
			// 
			// columnHeader26
			// 
			this->columnHeader26->Text = L"From Name";
			this->columnHeader26->Width = 163;
			// 
			// columnHeader27
			// 
			this->columnHeader27->Text = L"To Folder";
			this->columnHeader27->Width = 172;
			// 
			// columnHeader28
			// 
			this->columnHeader28->Text = L"To Name";
			this->columnHeader28->Width = 165;
			// 
			// openFile
			// 
			this->openFile->Filter = L"Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*";
			// 
			// folderBrowser
			// 
			this->folderBrowser->ShowNewFolderButton = false;
			// 
			// menuSearchSites
			// 
			this->menuSearchSites->Name = L"menuSearchSites";
			this->menuSearchSites->ShowImageMargin = false;
			this->menuSearchSites->Size = System::Drawing::Size(36, 4);
			this->menuSearchSites->ItemClicked += gcnew System::Windows::Forms::ToolStripItemClickedEventHandler(this, &UI::menuSearchSites_ItemClicked);
			// 
			// refreshWTWTimer
			// 
			this->refreshWTWTimer->Enabled = true;
			this->refreshWTWTimer->Interval = 600000;
			this->refreshWTWTimer->Tick += gcnew System::EventHandler(this, &UI::refreshWTWTimer_Tick);
			// 
			// notifyIcon1
			// 
			this->notifyIcon1->BalloonTipText = L"TV Rename is t3h r0x0r";
			this->notifyIcon1->BalloonTipTitle = L"TV Rename 2.1";
			this->notifyIcon1->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"notifyIcon1.Icon")));
			this->notifyIcon1->Text = L"TV Rename 2.1";
			this->notifyIcon1->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::notifyIcon1_Click);
			this->notifyIcon1->MouseDoubleClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::notifyIcon1_DoubleClick);
			// 
			// showRightClickMenu
			// 
			this->showRightClickMenu->Name = L"menuSearchSites";
			this->showRightClickMenu->ShowImageMargin = false;
			this->showRightClickMenu->Size = System::Drawing::Size(36, 4);
			this->showRightClickMenu->ItemClicked += gcnew System::Windows::Forms::ToolStripItemClickedEventHandler(this, &UI::showRightClickMenu_ItemClicked);
			// 
			// folderRightClickMenu
			// 
			this->folderRightClickMenu->Name = L"folderRightClickMenu";
			this->folderRightClickMenu->ShowImageMargin = false;
			this->folderRightClickMenu->Size = System::Drawing::Size(36, 4);
			this->folderRightClickMenu->ItemClicked += gcnew System::Windows::Forms::ToolStripItemClickedEventHandler(this, &UI::folderRightClickMenu_ItemClicked);
			// 
			// statusTimer
			// 
			this->statusTimer->Enabled = true;
			this->statusTimer->Interval = 250;
			this->statusTimer->Tick += gcnew System::EventHandler(this, &UI::statusTimer_Tick);
			// 
			// BGDownloadTimer
			// 
			this->BGDownloadTimer->Enabled = true;
			this->BGDownloadTimer->Interval = 10000;
			this->BGDownloadTimer->Tick += gcnew System::EventHandler(this, &UI::BGDownloadTimer_Tick);
			// 
			// tmrShowUpcomingPopup
			// 
			this->tmrShowUpcomingPopup->Interval = 250;
			this->tmrShowUpcomingPopup->Tick += gcnew System::EventHandler(this, &UI::tmrShowUpcomingPopup_Tick);
			// 
			// quickTimer
			// 
			this->quickTimer->Interval = 1;
			this->quickTimer->Tick += gcnew System::EventHandler(this, &UI::quickTimer_Tick);
			// 
			// ignoreListToolStripMenuItem
			// 
			this->ignoreListToolStripMenuItem->Name = L"ignoreListToolStripMenuItem";
			this->ignoreListToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::I));
			this->ignoreListToolStripMenuItem->Size = System::Drawing::Size(232, 22);
			this->ignoreListToolStripMenuItem->Text = L"&Ignore List";
			this->ignoreListToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::ignoreListToolStripMenuItem_Click);
			// 
			// UI
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(931, 582);
			this->Controls->Add(this->tableLayoutPanel2);
			this->Controls->Add(this->menuStrip1);
			this->Controls->Add(this->tabControl1);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->KeyPreview = true;
			this->MainMenuStrip = this->menuStrip1;
			this->Name = L"UI";
			this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Show;
			this->StartPosition = System::Windows::Forms::FormStartPosition::Manual;
			this->Text = L"TV Rename";
			this->Load += gcnew System::EventHandler(this, &UI::UI_Load);
			this->SizeChanged += gcnew System::EventHandler(this, &UI::UI_SizeChanged);
			this->FormClosing += gcnew System::Windows::Forms::FormClosingEventHandler(this, &UI::UI_FormClosing);
			this->LocationChanged += gcnew System::EventHandler(this, &UI::UI_LocationChanged);
			this->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &UI::UI_KeyDown);
			this->menuStrip1->ResumeLayout(false);
			this->menuStrip1->PerformLayout();
			this->tabControl1->ResumeLayout(false);
			this->tbMyShows->ResumeLayout(false);
			this->tbAllInOne->ResumeLayout(false);
			this->tbAllInOne->PerformLayout();
			this->tbWTW->ResumeLayout(false);
			this->tbWTW->PerformLayout();
			this->tableLayoutPanel2->ResumeLayout(false);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

		static int BGDLLongInterval() 
		{
			return 1000 * 60 * 60; // one hour
		}

	protected:
		void MoreBusy()
		{
			Interlocked::Increment(Busy);
		}
		void LessBusy()
		{
			Interlocked::Decrement(Busy);
		}

	private: static bool IsDebug()
			 {
#ifndef _DEBUG
				 return false;
#else
				 return true;
#endif
			 }

	private: void HideStuff()
			 {
				 if (IncludeExperimentalStuff() || IsDebug() || ForceExperimentalOn())
				 {
					 // debug stuff
					 // DirectoryInfo di(System::Windows::Forms::Application::UserAppDataPath+"\\..\\..\\..\\uTorrent");
				 }
				 else
				 {
					 // hide developmental things in release build
					 actorsToolStripMenuItem->Visible = false;
					 uTorrentToolStripMenuItem->Visible = false;

				 }
			 }

	public:
		UI(array<System::String ^> ^args)
		{        
			FileInfo ^tvdbOverride = nullptr;
			FileInfo ^settingsOverride = nullptr;

			bool ok = true;
			String ^hint = "";

			if ((args->Length == 1) && (args[0]->ToLower()=="/recover"))
			{
				ok = false; // force recover dialog
				hint = "Recover manually requested.";
			}
			do 
			{
				if (!ok)
				{
					RecoverXML ^rec = gcnew RecoverXML(hint);
					if (rec->ShowDialog() == ::DialogResult::OK)
				 {
					 tvdbOverride = rec->DBFile;
					 settingsOverride = rec->SettingsFile;
				 }
				}

				mDoc = gcnew TVDoc(args, settingsOverride, tvdbOverride);

				if (!ok)
					mDoc->SetDirty();

				ok = mDoc->LoadOK;
				if (!mDoc->LoadOK)
				{
					hint = "";
					if (!String::IsNullOrEmpty(mDoc->LoadErr))
						hint += mDoc->LoadErr;
					String ^h2 = mDoc->GetTVDB(false,"Recover")->LoadErr;
					if ( !String::IsNullOrEmpty(h2))
						hint += "\r\n"+h2;
				}
			} while (!ok);

			ExperimentalFeatures = mDoc->HasArg("/experimental");

			Busy = 0;
			mLastEpClicked = nullptr;
			mLastFolderClicked = nullptr;
			mLastSeasonClicked = nullptr;
			mLastShowClicked = nullptr;
			mLastAIOClicked = nullptr;

			mInternalChange = 0;
			mFoldersToOpen = gcnew StringList();

			InitializeComponent();


			try
			{
				LoadLayoutXML();
			}
			catch (...)
			{
				// silently fail, doesn't matter too much
			}

			SetProgress = gcnew SetProgressDelegate(this, &UI::SetProgressActual);

			lvWhenToWatch->ListViewItemSorter = gcnew DateSorterWTW();

			if (mDoc->HasArg("/hide"))
			{
				this->WindowState = FormWindowState::Minimized;
				this->Visible = false;
				this->Hide();
			}

			HideStuff();

			this->Text = this->Text + " " + DisplayVersionString();

			FillShowLists();
			UpdateSearchButton();
			SetGuideHTMLbody("");
			mDoc->DoWhenToWatch(true);
			FillWhenToWatchList();
			mDoc->WriteUpcomingRSS();
			ShowHideNotificationIcon();

			int t = mDoc->Settings->StartupTab;
			if (t < tabControl1->TabCount)
				tabControl1->SelectedIndex = mDoc->Settings->StartupTab;
			tabControl1_SelectedIndexChanged(nullptr,nullptr);			
		}

		void SetProgressActual(int p)
		{
			if (p < 0)
				p = 0;
			else if (p > 100)
				p = 100;

			pbProgressBarx->Value = p;
			pbProgressBarx->Update();
		}

		void ProcessArgs()
		{
			bool quit = mDoc->HasArg("/quit");

			if (mDoc->HasArg("/hide")) // /hide implies /quit
				quit = true;

			// process command line arguments, does not include application path
			//array<String ^> ^args = mDoc->GetArgs();
			//for (int i=0;i<args->Length;i++)
			//{
			//String ^arg = args[i]->ToLower();
			/*
			if (arg == "/missingcheck")
			{
			tabControl1->SelectedIndex = 2;
			bnDoMissingCheck_Click(nullptr,nullptr);
			}
			else if (arg == "/exportmissingxml")
			mDoc->ExportMissingXML(args[++i]);
			else if (arg == "/exportmissingcsv")
			mDoc->ExportMissingCSV(args[++i]);
			else if (arg == "/renamingcheck")
			{
			tabControl1->SelectedIndex = 1;
			bnRenameCheck_Click(nullptr,nullptr);
			}
			else if (arg == "/exportrenamingxml")
			mDoc->ExportRenamingXML(args[++i]);
			else if (arg == "/renamingdo")
			bnRenameDoRenaming_Click(nullptr,nullptr);
			else if (arg == "/fnocheck")
			{
			tabControl1->SelectedIndex = 2;
			bnFindMissingStuff_Click(nullptr,nullptr);
			}
			else if (arg == "exportfnoxml")
			mDoc->ExportFOXML(args[++i]);
			else if (arg == "/fnodo")
			bnDoMovingAndCopying_Click(nullptr,nullptr);
			*/
			//}

			if (quit)
				this->Close();
		}


		~UI()
		{
			//		mDoc->StopBGDownloadThread();  TODO
			mDoc = nullptr;

			if (components)
			{
				delete components;
			}
		}

		void UpdateSearchButton()
		{
			String ^name = mDoc->GetSearchers()->Name(mDoc->Settings->TheSearchers->CurrentSearchNum());
			bnWTWBTSearch->Text = name;
			bnAIOBTSearch->Text = name;
			FillEpGuideHTML();
		}

	private:
		System::Void exitToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			Close();
		}
		System::Void visitWebsiteToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			TVDoc::SysOpen("http://tvrename.com");
		}

		System::Void UI_Load(System::Object^  sender, System::EventArgs^  e) 
		{
			this->ShowInTaskbar = mDoc->Settings->ShowInTaskbar && !mDoc->HasArg("/hide");

			for each (TabPage ^tp in tabControl1->TabPages) // grr! TODO: why does it go white?
				tp->BackColor = System::Drawing::SystemColors::Control;

			this->Show();
			UI_LocationChanged(nullptr,nullptr);
			UI_SizeChanged(nullptr,nullptr);

			backgroundDownloadToolStripMenuItem->Checked = mDoc->Settings->BGDownload;
			offlineOperationToolStripMenuItem->Checked = mDoc->Settings->OfflineMode;
			BGDownloadTimer->Interval = 10000; // first time
			if (mDoc->Settings->BGDownload)
				BGDownloadTimer->Start();

			quickTimer->Start();
			//ProcessArgs();
		}

		ListView ^ListViewByName(String ^name)
		{
			if (name == "WhenToWatch") return lvWhenToWatch;
			if (name == "AllInOne") return lvAIO;
			return nullptr;
		}
		System::Void flushCacheToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			System::Windows::Forms::DialogResult res = MessageBox::Show("Are you sure you want to remove all "\
				"locally stored TheTVDB information?  This information will have to be downloaded again.  You "\
				"can force the refresh of a single show by holding down the \"Control\" key while clicking on "\
				"the \"Refresh\" button in the \"My Shows\" tab.",
				"Flush Web Cache",
				MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
			if (res == System::Windows::Forms::DialogResult::Yes)
			{
				mDoc->GetTVDB(false,"")->ForgetEverything();
				FillShowLists();
				FillEpGuideHTML();
				FillWhenToWatchList();
			}
		}

		bool LoadWidths(XmlReader ^xml)
		{
			String ^forwho = xml->GetAttribute("For");

			ListView ^lv = ListViewByName(forwho);
			if (lv == nullptr)
			{
				xml->ReadOuterXml();
				return true;
			}

			xml->Read();
			int c = 0;
			while (xml->Name == "Width")
			{
				if (c >= lv->Columns->Count)
					return false;
				lv->Columns[c++]->Width = xml->ReadElementContentAsInt();
			}
			xml->Read();
			return true;
		}


		bool LoadLayoutXML()
		{
			if (mDoc->HasArg("/hide"))
				return true;

			bool ok = true;
			XmlReaderSettings ^settings = gcnew XmlReaderSettings();
			settings->IgnoreComments = true;
			settings->IgnoreWhitespace = true;

			String ^fn = System::Windows::Forms::Application::UserAppDataPath+"\\Layout.xml";
			if (!FileInfo(fn).Exists)
				return true;

			XmlReader ^reader = XmlReader::Create(fn, settings);

			reader->Read();
			if (reader->Name != "xml")
				return false;

			reader->Read();
			if (reader->Name != "TVRename")
				return false;

			if (reader->GetAttribute("Version") != "2.1")
				return false;

			reader->Read();
			if (reader->Name != "Layout")
				return false;

			reader->Read();
			while (reader->Name != "Layout")
			{
				if (reader->Name == "Window")
				{
					reader->Read();
					while (reader->Name != "Window")
					{
						if (reader->Name == "Size")
						{
							int x = int::Parse(reader->GetAttribute("Width"));
							int y = int::Parse(reader->GetAttribute("Height"));
							this->Size = System::Drawing::Size(x,y);
							reader->Read();
						}
						else if (reader->Name == "Location")
						{
							int x = int::Parse(reader->GetAttribute("X"));
							int y = int::Parse(reader->GetAttribute("Y"));
							this->Location = Point(x,y);
							reader->Read();
						}
						else if (reader->Name == "Maximized")
							this->WindowState = ( reader->ReadElementContentAsBoolean() ? 
							FormWindowState::Maximized : 
						FormWindowState::Normal );
						else
							reader->ReadOuterXml();
					}
					reader->Read();
				} // window
				else if (reader->Name == "ColumnWidths")
					ok = LoadWidths(reader) && ok;
				else
					reader->ReadOuterXml();
			} // while

			reader->Close();
			return ok;
		}

		bool SaveLayoutXML()
		{
			if (mDoc->HasArg("/hide"))
				return true;

			XmlWriterSettings ^settings = gcnew XmlWriterSettings();
			settings->Indent = true;
			settings->NewLineOnAttributes = true;
			XmlWriter ^writer = XmlWriter::Create(System::Windows::Forms::Application::UserAppDataPath+"\\Layout.xml", settings);

			writer->WriteStartDocument();
			writer->WriteStartElement("TVRename");
			writer->WriteStartAttribute("Version");
			writer->WriteValue("2.1");
			writer->WriteEndAttribute(); // version
			writer->WriteStartElement("Layout");
			writer->WriteStartElement("Window");

			writer->WriteStartElement("Size");
			writer->WriteStartAttribute("Width");
			writer->WriteValue(mLastNonMaximizedSize.Width);
			writer->WriteEndAttribute();
			writer->WriteStartAttribute("Height");
			writer->WriteValue(mLastNonMaximizedSize.Height);
			writer->WriteEndAttribute();
			writer->WriteEndElement(); // size

			writer->WriteStartElement("Location");
			writer->WriteStartAttribute("X");
			writer->WriteValue(mLastNonMaximizedLocation.X);
			writer->WriteEndAttribute();
			writer->WriteStartAttribute("Y");
			writer->WriteValue(mLastNonMaximizedLocation.Y);
			writer->WriteEndAttribute();
			writer->WriteEndElement(); // Location

			writer->WriteStartElement("Maximized");
			writer->WriteValue(this->WindowState == FormWindowState::Maximized);
			writer->WriteEndElement(); // maximized

			writer->WriteEndElement(); // window

			WriteColWidthsXML("WhenToWatch", writer);
			WriteColWidthsXML("AllInOne", writer);

			writer->WriteEndElement(); // Layout
			writer->WriteEndElement(); // tvrename
			writer->WriteEndDocument();

			writer->Close();
			writer = nullptr;

			return true;
		}
		void WriteColWidthsXML(String ^thingName, XmlWriter ^writer)
		{
			ListView ^lv = ListViewByName(thingName);
			if (lv == nullptr)
				return;
			writer->WriteStartElement("ColumnWidths");
			writer->WriteStartAttribute("For");
			writer->WriteValue(thingName);
			writer->WriteEndAttribute();
			for each (ColumnHeader ^lvc in lv->Columns)
			{
				writer->WriteStartElement("Width");
				writer->WriteValue(lvc->Width);
				writer->WriteEndElement();
			}
			writer->WriteEndElement(); // columnwidths
		}

		System::Void UI_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e) 
		{

			if (mDoc->Dirty())
			{
				System::Windows::Forms::DialogResult res = MessageBox::Show("Your changes have not been saved.  Do you wish to save before quitting?","Unsaved data",MessageBoxButtons::YesNoCancel, MessageBoxIcon::Warning);
				if (res == System::Windows::Forms::DialogResult::Yes)
				{
					mDoc->WriteXMLSettings();
				} 
				else if (res == System::Windows::Forms::DialogResult::Cancel)
				{
					e->Cancel = true;
				} 
				else if (res == System::Windows::Forms::DialogResult::No)
				{
				}
			}
			if (!e->Cancel)
			{
				SaveLayoutXML();
				mDoc->TidyTVDB();
				mDoc->Closing();
			}

		}

		Windows::Forms::ContextMenuStrip ^BuildSearchMenu()
		{
			menuSearchSites->Items->Clear();
			for (int i=0;i<mDoc->GetSearchers()->Count();i++)
			{
				ToolStripMenuItem ^tsi = gcnew ToolStripMenuItem(mDoc->GetSearchers()->Name(i));
				tsi->Tag = i;
				menuSearchSites->Items->Add(tsi);
			}
			return menuSearchSites;
		}

		void ChooseSiteMenu(int n)
		{
			Windows::Forms::ContextMenuStrip ^sm = BuildSearchMenu();
			if (n == 1)
				sm->Show(bnWTWChooseSite,Point(0,0));
			else if (n == 0)
				sm->Show(bnAIOWhichSearch,Point(0,0));
		}

		System::Void bnWTWChooseSite_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			ChooseSiteMenu(1);
		}

		System::Void bnEpGuideChooseSearch_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			ChooseSiteMenu(2);
		}

		void FillShowLists()
		{
			Season ^currentSeas = TreeNodeToSeason(MyShowTree->SelectedNode);
			ShowItem ^currentSI = TreeNodeToShowItem(MyShowTree->SelectedNode);

			ShowItemList ^expanded = gcnew ShowItemList();
			for each (TreeNode ^n in MyShowTree->Nodes)
				if (n->IsExpanded)
					expanded->Add(TreeNodeToShowItem(n));

			MyShowTree->BeginUpdate();

			MyShowTree->Nodes->Clear();

			ShowItemList ^sil = mDoc->GetShowItems(true);
			for each (ShowItem ^si in sil)
			{
				TreeNode ^tvn = AddShowItemToTree(si);
				if (expanded->Contains(si))
					tvn->Expand();
			}
			mDoc->UnlockShowItems();

			for each (ShowItem ^si in expanded)
				for each (TreeNode ^n in MyShowTree->Nodes)
					if (TreeNodeToShowItem(n) == si)
						n->Expand();

			if (currentSeas != nullptr)
				SelectSeason(currentSeas);
			else
				if (currentSI != nullptr)
					SelectShow(currentSI);

			MyShowTree->EndUpdate();
		}

		static String ^QuickStartGuide()
		{
			return "http://tvrename.com/quickstart.html";
		}

		void ShowQuickStartGuide()
		{
			tabControl1->SelectTab(tbMyShows);
			epGuideHTML->Navigate(QuickStartGuide());
		}

		void FillEpGuideHTML()
		{
			if (MyShowTree->Nodes->Count == 0)
				ShowQuickStartGuide();
			else
			{
				TreeNode ^n = MyShowTree->SelectedNode;
				FillEpGuideHTML(n);
			}
		}

		ShowItem ^TreeNodeToShowItem(TreeNode ^n)
		{
			if (n == nullptr)
				return nullptr;

			ShowItem ^si = dynamic_cast<ShowItem ^>(n->Tag);
			if (si != nullptr)
				return si;

			ProcessedEpisode ^pe = dynamic_cast<ProcessedEpisode ^>(n->Tag);
			if (pe != nullptr)
				return pe->SI;

			Season ^seas = dynamic_cast<Season ^>(n->Tag);
			if (seas != nullptr)
			{
				if (seas->Episodes->Count)
				{
					int tvdbcode = seas->TheSeries->TVDBCode;
					for each (ShowItem ^si in mDoc->GetShowItems(true))
						if (si->TVDBCode == tvdbcode)
						{
							mDoc->UnlockShowItems();
							return si;
						}
						mDoc->UnlockShowItems();
				}
			}

			return nullptr;
		}

		static Season ^TreeNodeToSeason(TreeNode ^n)
		{
			if (n == nullptr)
				return nullptr;

			Season ^seas = dynamic_cast<Season ^>(n->Tag);
			return seas;

		}

		void FillEpGuideHTML(TreeNode ^n)
		{
			if (n == nullptr)
			{
				FillEpGuideHTML(nullptr, -1);
				return;
			}

			ProcessedEpisode ^pe = dynamic_cast<ProcessedEpisode ^>(n->Tag);
			if (pe != nullptr)
			{
				FillEpGuideHTML(pe->SI, pe->SeasonNumber);
				return;
			}

			Season ^seas = TreeNodeToSeason(n);
			if (seas != nullptr)
			{
				// we have a TVDB season, but need to find the equiavlent one in our local processed episode collection
				if (seas->Episodes->Count)
				{
					int tvdbcode = seas->TheSeries->TVDBCode;
					for each (ShowItem ^si in mDoc->GetShowItems(true))
					{
						if (si->TVDBCode == tvdbcode)
						{
							mDoc->UnlockShowItems();
							FillEpGuideHTML(si, seas->SeasonNumber);
							return;
						}
					}
					mDoc->UnlockShowItems();

					if (pe != nullptr)
					{
						FillEpGuideHTML(pe->SI, -1);
						return;
					}                        
				}
				FillEpGuideHTML(nullptr, -1);
				return;
			}

			FillEpGuideHTML(TreeNodeToShowItem(n), -1);
		}


		void FillEpGuideHTML(ShowItem ^si, int snum)
		{
			if (tabControl1->SelectedTab != tbMyShows)
				return;

			if (si == nullptr)
			{
				SetGuideHTMLbody("");
				return;
			}
			TheTVDB ^db = mDoc->GetTVDB(true,"FillEpGuideHTML");
			SeriesInfo ^ser =  db->GetSeries(si->TVDBCode);

			if (ser == nullptr)
			{
				SetGuideHTMLbody("Not downloaded, or not available");
				return;
			}

			String ^body = "";

			StringList ^skip = gcnew StringList();
			skip->Add("Actors");
			skip->Add("banner");
			skip->Add("Overview");
			skip->Add("Airs_Time");
			skip->Add("Airs_DayOfWeek");
			skip->Add("fanart");
			skip->Add("poster");
			skip->Add("zap2it_id");

			if ((snum >= 0) && (ser->Seasons->ContainsKey(snum)))
			{
				if (!String::IsNullOrEmpty(ser->GetItem("banner")) && !String::IsNullOrEmpty(db->BannerMirror) )
					body += "<img width=758 height=140 src=\"" + db->BannerMirror + "/banners/" + ser->GetItem("banner") +"\"><br/>";

				Season ^s = ser->Seasons[snum];

				ProcessedEpisodeList ^eis = nullptr;
				int snum = s->SeasonNumber;
				if (si->SeasonEpisodes->ContainsKey(snum))
					eis = si->SeasonEpisodes[snum]; // use processed episodes if they are available
				else
					eis = ShowItem::ProcessedListFromEpisodes(s->Episodes, si);


				String ^seasText = snum == 0 ? "Specials" : ("Season " + snum.ToString());
				if ((eis->Count > 0) && (eis[0]->SeasonID > 0))
					seasText = " - <A HREF=\""+db->WebsiteURL(si->TVDBCode, eis[0]->SeasonID, false)+"\">"+ seasText + "</a>";
				else
					seasText = " - " + seasText;

				body += "<h1><A HREF=\""+db->WebsiteURL(si->TVDBCode, -1, true)+"\">" + si->ShowName() + "</A>" + seasText + "</h1>";

				for each (ProcessedEpisode ^ei in eis)
				{
					String ^epl = ei->NumsAsString();

					// http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
					String ^episodeURL = "http://www.thetvdb.com/?tab=episode&seriesid="+ei->SeriesID+"&seasonid="+ei->SeasonID+"&id="+ei->EpisodeID.ToString();

					body += "<A href=\""+episodeURL+"\" name=\"ep"+epl+"\">"; // anchor
					body += "<b>" + CustomName::NameForNoExt(ei, CustomName::OldNStyle(6)) + "</b>";
					body += "</A>"; // anchor
					if (si->UseSequentialMatch && (ei->OverallNumber != -1))
						body += " (#" + ei->OverallNumber.ToString() + ")";

					body += " <A HREF=\"" + mDoc->Settings->BTSearchURL(ei) +"\" class=\"search\">Search</A>";

					FileList ^fl = mDoc->FindEpOnDisk(ei);
					if (fl != nullptr)
					{
						for each (FileInfo ^fi in fl)
							body += " <A HREF=\"file://" + fi->FullName +"\" class=\"search\">Watch</A>";
					}

					DateTime ^dt = ei->GetAirDateDT(true);
					if ((dt != nullptr)  && (dt->CompareTo(DateTime::MaxValue)))
						body += "<p>" + ei->GetAirDateDT(true)->ToShortDateString() + " (" + ei->HowLong() + ")";

					body += "<p><p>";

					if (mDoc->Settings->ShowEpisodePictures)
					{
						body += "<table><tr>";
						body += "<td width=100% valign=top>"+ei->Overview+"</td><td width=300 height=225>";
						// 300x168 / 300x225
						if (!String::IsNullOrEmpty(ei->GetItem("filename")))
							body += "<img src="+db->BannerMirror + "/banners/_cache/" + ei->GetItem("filename")+">";
						body += "</td></tr></table>";
					}
					else
						body += ei->Overview;

					body += "<p><hr><p>";
				} // for each episode in this season
			}
			else
			{
				// no epnum specified, just show an overview
				if ((!String::IsNullOrEmpty(ser->GetItem("banner"))) && (!String::IsNullOrEmpty(db->BannerMirror)))
					body += "<img width=758 height=140 src=\"" + db->BannerMirror + "/banners/" + ser->GetItem("banner") +"\"><br/>";

				body += "<h1><A HREF=\""+db->WebsiteURL(si->TVDBCode, -1, true)+"\">" + si->ShowName() + "</A> " + "</h1>";

				body += "<h2>Overview</h2>" + ser->GetItem("Overview");

				String ^actors = ser->GetItem("Actors");
				if (!String::IsNullOrEmpty(actors))
				{
					bool first = true;
					for each (String ^aa in actors->Split('|'))
					{
						if (!String::IsNullOrEmpty(aa))
						{
							if (!first)
								body += ", ";
							else
								body += "<h2>Actors</h2>";
							body += "<A HREF=\"http://www.imdb.com/find?s=nm&q="+aa+"\">"+aa+"</a>";
							first = false;
						}
					}
				}

				String ^airsTime = ser->GetItem("Airs_Time");
				String ^airsDay = ser->GetItem("Airs_DayOfWeek");
				if ((!String::IsNullOrEmpty(airsTime)) && (!String::IsNullOrEmpty(airsDay)))
				{
					body += "<h2>Airs</h2> " + airsTime + " " + airsDay;
					String ^net = ser->GetItem("Network");
					if (!String::IsNullOrEmpty(net))
					{
						skip->Add("Network");
						body += ", "+net;
					}
				}

				bool first = true;
				for each (KeyValuePair<String ^, String ^> ^kvp in ser->Items)
				{
					if (first)
					{
						body += "<h2>Information<table border=0>";
						first = false;
					}
					if (!skip->Contains(kvp->Key))
					{
						if (kvp->Key == "SeriesID")
							body += "<tr><td width=120px>tv.com</td><td><A HREF=\"http://www.tv.com/show/"+kvp->Value+"/summary.html\">Visit</a></td></tr>";
						else if (kvp->Key == "IMDB_ID")
							body += "<tr><td width=120px>imdb.com</td><td><A HREF=\"http://www.imdb.com/title/"+kvp->Value+"\">Visit</a></td></tr>";
						else
							body += "<tr><td width=120px>"+kvp->Key+"</td><td>" + kvp->Value +"</td></tr>";
					}
				}
				if (!first)
					body += "</table>";

			}
			db->Unlock("FillEpGuideHTML");
			SetGuideHTMLbody(body);
		} // FillEpGuideHTML

	public: static String ^EpGuidePath()
			{
				String ^tp = Path::GetTempPath();
				return tp+"tvrenameepguide.html";
			}

			static String ^EpGuideURLBase()
			{
				return "file://"+EpGuidePath();
			}

			void SetGuideHTMLbody(String ^body)
			{
				System::Drawing::Color col = System::Drawing::Color::FromName("ButtonFace");

				String ^css = "* { font-family: Tahoma, Arial; font-size 10pt; } " \
					"a:link { color: black } " \
					"a:visited { color:black } " \
					"a:hover { color:#000080 } " \
					"a:active { color:black } " \
					"a.search:link { color: #800000 } " \
					"a.search:visited { color:#800000 } " \
					"a.search:hover { color:#000080 } " \
					"a.search:active { color:#800000 } " \
					"* {background-color: #"+col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2") + "}" \
					"* { color: black }";

				String ^html = "<html><head><STYLE type=\"text/css\">"+css+"</style>";

				html += "</head><body>";
				html += body;
				html += "</body></html>";

				epGuideHTML->Navigate("about:blank"); // make it close any file it might have open

				String^ path = EpGuidePath();

				BinaryWriter ^bw = gcnew BinaryWriter(gcnew FileStream(path, FileMode::Create));
				bw->Write(System::Text::Encoding::GetEncoding("ISO-8859-1")->GetBytes(html));
				bw->Close();

				epGuideHTML->Navigate(EpGuideURLBase());
			}

			void TVDBFor(ProcessedEpisode ^e)
			{
				if (e == nullptr)
					return;

				TVDoc::SysOpen(mDoc->GetTVDB(false,"")->WebsiteURL(e->SI->TVDBCode, e->SeasonID, false));
			}
			void TVDBFor(Season ^seas)
			{
				if (seas == nullptr)
					return;

				TVDoc::SysOpen(mDoc->GetTVDB(false,"")->WebsiteURL(seas->TheSeries->TVDBCode, -1, false));
			}
			void TVDBFor(ShowItem ^si)
			{
				if (si == nullptr)
					return;

				TVDoc::SysOpen(mDoc->GetTVDB(false,"")->WebsiteURL(si->TVDBCode, -1, false));
			}
			/*
			void RenamingCheckSpecific(ShowItem ^si)
			{
			MoreBusy();
			mDoc->DoRenameCheck(this->SetProgress, si);
			FillShowLists();
			//FillRenameList();
			FillAIOList();
			LessBusy();
			tabControl1->SelectedIndex = 1;
			}
			void MissingCheckSpecific(ShowItem ^si)
			{
			MoreBusy();
			mDoc->DoMissingCheck(this->SetProgress, si);
			FillShowLists();
			//FillMissingList();
			FillAIOList();
			LessBusy();
			if (si != nullptr)
			tabControl1->SelectedIndex = 1;
			}
			*/
			System::Void menuSearchSites_ItemClicked(System::Object^  sender, System::Windows::Forms::ToolStripItemClickedEventArgs^  e) 
			{
				mDoc->SetSearcher(safe_cast<int>(e->ClickedItem->Tag));
				UpdateSearchButton();

			}
			System::Void bnWhenToWatchCheck_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				RefreshWTW(true);
			}

			void FillWhenToWatchList()
			{
				mInternalChange++;
				lvWhenToWatch->BeginUpdate();

				int dd = mDoc->Settings->WTWRecentDays;

				lvWhenToWatch->Groups[0]->Header = "Aired in the last " + dd.ToString() + " day" + ((dd == 1) ? "":"s");

				// try to maintain selections if we can
				ProcessedEpisodeList ^selections = gcnew ProcessedEpisodeList();
				for each (ListViewItem ^lvi in lvWhenToWatch->SelectedItems)
					selections->Add(safe_cast<ProcessedEpisode ^>(lvi->Tag));

				Season ^currentSeas = TreeNodeToSeason(MyShowTree->SelectedNode);
				ShowItem ^currentSI = TreeNodeToShowItem(MyShowTree->SelectedNode);


				lvWhenToWatch->Items->Clear();

				Generic::List<DateTime> ^bolded = gcnew Generic::List<DateTime>;

				for each (ShowItem ^si in mDoc->GetShowItems(true))
				{
					if (!si->ShowNextAirdate)
						continue;

					for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in si->SeasonEpisodes)
					{
						if (si->IgnoreSeasons->Contains(kvp->Key))
							continue; // ignore this season

						ProcessedEpisodeList ^eis = kvp->Value;

						bool nextToAirFound = false;

						for each (ProcessedEpisode ^ei in eis)
						{
							DateTime ^dt = ei->GetAirDateDT(true);
							if ((dt != nullptr) && (dt->CompareTo(DateTime::MaxValue)))
							{
								TimeSpan ^ts = dt->Subtract(DateTime::Now);
								if (ts->TotalHours >= (-24*dd)) // in the future (or fairly recent)
								{
									bolded->Add(*dt);
									if ((ts->TotalHours >= 0) && (!nextToAirFound))
									{
										nextToAirFound = true;
										ei->NextToAir = true;
									}
									else
										ei->NextToAir = false;

									ListViewItem ^lvi = gcnew System::Windows::Forms::ListViewItem();
									lvi->Text = "";
									for (int i=0;i<7;i++)
										lvi->SubItems->Add("");

									UpdateWTW(ei, lvi);

									lvWhenToWatch->Items->Add(lvi);

									for each (ProcessedEpisode ^pe in selections)
										if (pe->SameAs(ei))
										{
											lvi->Selected = true;
											break;
										}
								}
							}
						}
					}
				}
				mDoc->UnlockShowItems();
				lvWhenToWatch->Sort();

				lvWhenToWatch->EndUpdate();
				calCalendar->BoldedDates = bolded->ToArray();


				if (currentSeas != nullptr)
					SelectSeason(currentSeas);
				else
					if (currentSI != nullptr)
						SelectShow(currentSI);

				UpdateToolstripWTW();
				mInternalChange--;
			}

			System::Void lvWhenToWatch_ColumnClick(System::Object^  sender, System::Windows::Forms::ColumnClickEventArgs^  e) 
			{
				int col = e->Column;
				// 3 4, or 6 = do date sort on 3
				// 1 or 2 = number sort
				// 5 = day sort
				// all others, text sort

				if (col==6) // straight sort by date
				{
					lvWhenToWatch->ListViewItemSorter = gcnew DateSorterWTW();
					lvWhenToWatch->ShowGroups = false;
				}
				else if ((col==3)||(col==4))
				{
					lvWhenToWatch->ListViewItemSorter = gcnew DateSorterWTW();
					lvWhenToWatch->ShowGroups = true;
				}
				else
				{
					lvWhenToWatch->ShowGroups = false;
					if ((col == 1)||(col == 2))
						lvWhenToWatch->ListViewItemSorter = gcnew NumberAsTextSorter(col);
					else if (col == 5)
						lvWhenToWatch->ListViewItemSorter = gcnew DaySorter(col);
					else 
						lvWhenToWatch->ListViewItemSorter = gcnew TextSorter(col);
				}
				lvWhenToWatch->Sort();
			}

			System::Void lvWhenToWatch_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (lvWhenToWatch->SelectedIndices->Count == 0)
				{
					txtWhenToWatchSynopsis->Text = "";
					return;
				}
				int n = lvWhenToWatch->SelectedIndices[0];

				ProcessedEpisode ^ei = safe_cast<ProcessedEpisode ^>(lvWhenToWatch->Items[n]->Tag);
				txtWhenToWatchSynopsis->Text = ei->Overview;

				mInternalChange++;
				calCalendar->SelectionStart = *ei->GetAirDateDT(true);
				calCalendar->SelectionEnd = *ei->GetAirDateDT(true);
				mInternalChange--;

				if (mDoc->Settings->AutoSelectShowInMyShows)
					GotoEpguideFor(ei, false);
			}
			System::Void lvWhenToWatch_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
			{
				if (lvWhenToWatch->SelectedItems->Count > 0)
				{
					ProcessedEpisode ^ei = safe_cast<ProcessedEpisode ^>(lvWhenToWatch->SelectedItems[0]->Tag);
					FileList ^fl = mDoc->FindEpOnDisk(ei);
					if ((fl != nullptr) && (fl->Count > 0))
					{
						TVDoc::SysOpen(fl[0]->FullName);
						return;
					}
				}

				bnWTWBTSearch_Click(nullptr,nullptr);
			}
			System::Void calCalendar_DateSelected(System::Object^  sender, System::Windows::Forms::DateRangeEventArgs^  e) 
			{
				if (mInternalChange)
					return;

				DateTime ^dt = calCalendar->SelectionStart;
				for (int i=0;i<lvWhenToWatch->Items->Count;i++)
					lvWhenToWatch->Items[i]->Selected = false;

				bool first = true;

				for (int i=0;i<lvWhenToWatch->Items->Count;i++)
				{
					ListViewItem ^lvi = lvWhenToWatch->Items[i];
					ProcessedEpisode ^ei = safe_cast<ProcessedEpisode ^>(lvi->Tag);
					DateTime ^dt2 = ei->GetAirDateDT(true);
					double h = dt2->Subtract(*dt).TotalHours;
					if ((h >= 0) && (h < 24.0))
					{
						lvi->Selected = true;
						if (first)
						{
							lvi->EnsureVisible();
							first = false;
						}
					}
				}
				lvWhenToWatch->Focus();
			}

			System::Void bnEpGuideRefresh_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				bnWhenToWatchCheck_Click(nullptr,nullptr); // close enough!
				FillShowLists();
			}

			void RefreshWTW(bool doDownloads)
			{
				if (doDownloads)
					if (!mDoc->DoDownloadsFG())
						return;

				mInternalChange++;
				mDoc->DoWhenToWatch(true);
				FillShowLists();
				FillWhenToWatchList();
				mInternalChange--;

				mDoc->WriteUpcomingRSS();
			}

			System::Void refreshWTWTimer_Tick(System::Object^  sender, System::EventArgs^  e) 
			{
				if (!Busy)
					RefreshWTW(false);
			}
			void UpdateToolstripWTW()
			{
				// update toolstrip text too
				ProcessedEpisodeList ^next1 = mDoc->NextNShows(1, 36500);

				tsNextShowTxt->Text = "Next airing: ";
				if ((next1 != nullptr) && (next1->Count >= 1))
				{
					ProcessedEpisode ^ei = next1[0];
					tsNextShowTxt->Text += CustomName::NameForNoExt(ei, CustomName::OldNStyle(1)) + ", " + ei->HowLong() + " (" + ei->DayOfWeek() + ", " + ei->TimeOfDay() + ")";
				}
				else
					tsNextShowTxt->Text += "---";
			}
			System::Void bnWTWBTSearch_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				for each (ListViewItem ^lvi in lvWhenToWatch->SelectedItems)
					mDoc->DoBTSearch(safe_cast<ProcessedEpisode ^>(lvi->Tag));
			}
			System::Void epGuideHTML_Navigating(System::Object^  sender, System::Windows::Forms::WebBrowserNavigatingEventArgs^  e) 
			{
				String ^url = e->Url->AbsoluteUri;
				if (url->Contains("tvrenameepguide.html#ep"))
					return; // don't intercept
				if (url->EndsWith("tvrenameepguide.html"))
					return; // don't intercept
				if (!url->CompareTo("about:blank"))
					return; // don't intercept about:blank
				if (url == QuickStartGuide())
					return; // let the quickstartguide be shown

				if ( (!url->Substring(0,7)->CompareTo(gcnew String("http://"))) || 
					(!url->Substring(0,7)->CompareTo(gcnew String("file://"))) )
				{
					e->Cancel = true;
					TVDoc::SysOpen(e->Url->AbsoluteUri);
				}

			}
			System::Void notifyIcon1_Click(System::Object^  sender, MouseEventArgs^  e) 
			{
				// double-click of notification icon causes a click then doubleclick event, 
				// so we need to do a timeout before showing the single click's popup
				tmrShowUpcomingPopup->Start();
			}
			System::Void tmrShowUpcomingPopup_Tick(System::Object^  sender, System::EventArgs^  e) 
			{
				tmrShowUpcomingPopup->Stop();
				UpcomingPopup ^UP = gcnew UpcomingPopup(mDoc);
				UP->Show();

			}
			System::Void notifyIcon1_DoubleClick(System::Object^  sender, MouseEventArgs^  e) 
			{
				tmrShowUpcomingPopup->Stop();
				if (!mDoc->Settings->ShowInTaskbar)
					this->Show();
				if (this->WindowState == FormWindowState::Minimized)
					this->WindowState = FormWindowState::Normal;
				this->Activate();
			}
			System::Void buyMeADrinkToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				BuyMeADrink ^bmad = gcnew BuyMeADrink();
				bmad->ShowDialog();
			}

			void GotoEpguideFor(ShowItem ^si, bool changeTab)
			{
				if (changeTab)
					tabControl1->SelectTab(tbMyShows);
				FillEpGuideHTML(si, -1);
			}

			void GotoEpguideFor(Episode ^ep, bool changeTab)
			{
				if (changeTab)
					tabControl1->SelectTab(tbMyShows);

				SelectSeason(ep->TheSeason);
			}

			void RightClickOnMyShows(ShowItem ^si, Point ^pt)
			{
				mLastShowClicked = si;
				mLastEpClicked = nullptr;
				mLastSeasonClicked = nullptr;
				mLastAIOClicked = nullptr;
				BuildRightClickMenu(pt);

			}
			void RightClickOnMyShows(Season ^seas, Point ^pt)
			{
				mLastShowClicked = mDoc->GetShowItem(seas->TheSeries->TVDBCode);
				mLastEpClicked = nullptr;
				mLastSeasonClicked = seas;
				mLastAIOClicked = nullptr;
				BuildRightClickMenu(pt);
			}
			void RightClickOnShow(ProcessedEpisode ^ep, Point ^pt)
			{
				mLastEpClicked = ep;
				mLastShowClicked = ep != nullptr ? ep->SI: nullptr;
				mLastSeasonClicked = ep != nullptr ? ep->TheSeason : nullptr;
				mLastAIOClicked = nullptr;
				BuildRightClickMenu(pt);
			}

			void MenuGuideAndTVDB(bool addSep)
			{
				ShowItem ^si = mLastShowClicked;
				Season ^seas = mLastSeasonClicked;
				ProcessedEpisode ^ep = mLastEpClicked;
				ToolStripMenuItem ^tsi;

				if (si != nullptr)
				{
					if (addSep)
					{
						showRightClickMenu->Items->Add(gcnew ToolStripSeparator());
						addSep = false;
					}
					tsi = gcnew ToolStripMenuItem("Episode Guide");     tsi->Tag = (int)kEpisodeGuideForShow; showRightClickMenu->Items->Add(tsi);
				}

				if (ep != nullptr)
				{
					if (addSep)
					{
						showRightClickMenu->Items->Add(gcnew ToolStripSeparator());
						addSep = false;
					}
					tsi = gcnew ToolStripMenuItem("Visit thetvdb.com");     tsi->Tag = (int)kVisitTVDBEpisode; showRightClickMenu->Items->Add(tsi);
				}
				else if (seas != nullptr)
				{
					if (addSep)
					{
						showRightClickMenu->Items->Add(gcnew ToolStripSeparator());
						addSep = false;
					}
					tsi = gcnew ToolStripMenuItem("Visit thetvdb.com");     tsi->Tag = (int)kVisitTVDBSeason; showRightClickMenu->Items->Add(tsi);
				}
				else if (si != nullptr)
				{
					if (addSep)
					{
						showRightClickMenu->Items->Add(gcnew ToolStripSeparator());
						addSep = false;
					}
					tsi = gcnew ToolStripMenuItem("Visit thetvdb.com");     tsi->Tag = (int)kVisitTVDBSeries; showRightClickMenu->Items->Add(tsi);
				}

			}

			void MenuShowAndEpisodes()
			{
				ShowItem ^si = mLastShowClicked;
				Season ^seas = mLastSeasonClicked;
				ProcessedEpisode ^ep = mLastEpClicked;
				ToolStripMenuItem ^tsi;


				if (si != nullptr)
				{
					tsi = gcnew ToolStripMenuItem("Force Refresh");     tsi->Tag = (int)kForceRefreshSeries; showRightClickMenu->Items->Add(tsi);
					ToolStripSeparator ^tss = gcnew ToolStripSeparator(); showRightClickMenu->Items->Add(tss);
					tsi = gcnew ToolStripMenuItem("Scan");     tsi->Tag = (int)kScanSpecificSeries; showRightClickMenu->Items->Add(tsi);
					//tsi = gcnew ToolStripMenuItem("Renaming Check");     tsi->Tag = (int)kRenamingCheckSeries; showRightClickMenu->Items->Add(tsi);
					tsi = gcnew ToolStripMenuItem("When to Watch");     tsi->Tag = (int)kWhenToWatchSeries; showRightClickMenu->Items->Add(tsi);
				}

				if (ep != nullptr)
				{
					FileList ^fl = mDoc->FindEpOnDisk(ep);
					if (fl != nullptr)
					{
						if (fl->Count)
						{
							ToolStripSeparator ^tss = gcnew ToolStripSeparator(); showRightClickMenu->Items->Add(tss);

							int n = mLastFL->Count;
							for each (FileInfo ^fi in fl)
							{
								mLastFL->Add(fi);
								tsi = gcnew ToolStripMenuItem("Watch: "+fi->FullName);
								tsi->Tag = (int)kWatchBase + n;
								showRightClickMenu->Items->Add(tsi);
							}
						}
					}
				}
				else if ((seas != nullptr) && (si != nullptr))
				{
					// for each episode in season, find it on disk
					bool first = true;
					for each (ProcessedEpisode ^ep in si->SeasonEpisodes[seas->SeasonNumber])
					{
						FileList ^fl = mDoc->FindEpOnDisk(ep);
						if ((fl != nullptr) && (fl->Count > 0))
						{
							if (first)
							{
								ToolStripSeparator ^tss = gcnew ToolStripSeparator();
								showRightClickMenu->Items->Add(tss);
								first = false;
							}

							int n = mLastFL->Count;
							for each (FileInfo ^fi in fl)
							{
								mLastFL->Add(fi);
								tsi = gcnew ToolStripMenuItem("Watch: "+fi->FullName);
								tsi->Tag = (int)kWatchBase + n;
								showRightClickMenu->Items->Add(tsi);
							}
						}
					}
				}
			}

			void MenuFolders(LVResults ^lvr)
			{
				ShowItem ^si = mLastShowClicked;
				Season ^seas = mLastSeasonClicked;
				ProcessedEpisode ^ep = mLastEpClicked;
				ToolStripMenuItem ^tsi;
				Generic::List<String ^> ^added = gcnew Generic::List<String ^>();

				if (ep != nullptr)
				{
					if (ep->SI->AllFolderLocations(mDoc->Settings)->ContainsKey(ep->SeasonNumber))
					{
						int n = mFoldersToOpen->Count;
						bool first = true;
						for each (String ^folder in ep->SI->AllFolderLocations(mDoc->Settings)[ep->SeasonNumber])
						{
							if ((!String::IsNullOrEmpty(folder)) && DirectoryInfo(folder).Exists)
							{
								if (first)
								{
									ToolStripSeparator ^tss = gcnew ToolStripSeparator();  showRightClickMenu->Items->Add(tss);
									first = false;
								}

								tsi = gcnew ToolStripMenuItem("Open: "+folder);
								added->Add(folder);
								mFoldersToOpen->Add(folder);
								tsi->Tag = (int)kOpenFolderBase+n;
								n++;
								showRightClickMenu->Items->Add(tsi);
							}
						}
					}
				}
				else if ((seas != nullptr) && (si != nullptr) && 
					(si->AllFolderLocations(mDoc->Settings)->ContainsKey(seas->SeasonNumber)) )
				{
					int n = mFoldersToOpen->Count;
					bool first = true;
					for each (String ^folder in si->AllFolderLocations(mDoc->Settings)[seas->SeasonNumber])
					{
						if ((!String::IsNullOrEmpty(folder)) && DirectoryInfo(folder).Exists && !added->Contains(folder))
						{
							added->Add(folder); // don't show the same folder more than once
							if (first)
							{
								ToolStripSeparator ^tss = gcnew ToolStripSeparator();
								showRightClickMenu->Items->Add(tss);
								first = false;
							}

							tsi = gcnew ToolStripMenuItem("Open: "+folder);
							mFoldersToOpen->Add(folder);
							tsi->Tag = (int)kOpenFolderBase+n;
							n++;
							showRightClickMenu->Items->Add(tsi);
						}
					}
				}
				else if (si != nullptr) 
				{
					int n = mFoldersToOpen->Count;
					bool first = true;

					for each (KeyValuePair<int, StringList ^> ^kvp in si->AllFolderLocations(mDoc->Settings))
					{
						for each (String ^folder in kvp->Value)
						{
							if ((!String::IsNullOrEmpty(folder)) && DirectoryInfo(folder).Exists && !added->Contains(folder))
							{
								added->Add(folder); // don't show the same folder more than once
								if (first)
								{
									ToolStripSeparator ^tss = gcnew ToolStripSeparator();
									showRightClickMenu->Items->Add(tss);
									first = false;
								}

								tsi = gcnew ToolStripMenuItem("Open: "+folder);
								mFoldersToOpen->Add(folder);
								tsi->Tag = (int)kOpenFolderBase+n;
								n++;
								showRightClickMenu->Items->Add(tsi);
							}
						}
					}
				}

				if (lvr != nullptr) // add folders for selected Scan items
				{
					int n = mFoldersToOpen->Count;
					bool first = true;

					for each (AIOItem ^aio in lvr->FlatList)
					{
						String ^folder = aio->TargetFolder();
						if (!String::IsNullOrEmpty(folder) && DirectoryInfo(folder).Exists && !added->Contains(folder))
						{
							added->Add(folder); // don't show the same folder more than once
							if (first)
							{
								ToolStripSeparator ^tss = gcnew ToolStripSeparator();
								showRightClickMenu->Items->Add(tss);
								first = false;
							}

							tsi = gcnew ToolStripMenuItem("Open: "+folder);
							mFoldersToOpen->Add(folder);
							tsi->Tag = (int)kOpenFolderBase+n;
							n++;
							showRightClickMenu->Items->Add(tsi);
						}
					}
				}
			}

			void BuildRightClickMenu(Point ^pt)
			{
				showRightClickMenu->Items->Clear();
				mFoldersToOpen = gcnew StringList();
				mLastFL = gcnew FileList();

				MenuGuideAndTVDB(false);
				MenuShowAndEpisodes();
				MenuFolders(nullptr);

				showRightClickMenu->Show(*pt);
			}
			System::Void showRightClickMenu_ItemClicked(System::Object^  sender, System::Windows::Forms::ToolStripItemClickedEventArgs^  e) 
			{
				showRightClickMenu->Close();
				int n = safe_cast<int>(e->ClickedItem->Tag);
				switch (n)
				{
				case kEpisodeGuideForShow: // epguide
					if (mLastEpClicked != nullptr)
						GotoEpguideFor(mLastEpClicked, true);
					else
						GotoEpguideFor(mLastShowClicked, true);
					break;

				case kVisitTVDBEpisode: // thetvdb.com
					{
						TVDBFor(mLastEpClicked);
						break;
					}

				case kVisitTVDBSeason:
					{
						TVDBFor(mLastSeasonClicked);
						break;
					}

				case kVisitTVDBSeries:
					{
						TVDBFor(mLastShowClicked);
						break;
					}
				case kScanSpecificSeries:
					{
						if ( mLastShowClicked != nullptr )
						{
							Scan(mLastShowClicked);
							tabControl1->SelectTab(tbAllInOne);
						}
						break;
					}
					/*
					case kMissingCheckSeries:
					{
					if ( mLastShowClicked != nullptr)
					MissingCheckSpecific(mLastShowClicked);
					break;
					}
					case kRenamingCheckSeries:
					{
					if ( mLastShowClicked != nullptr)
					RenamingCheckSpecific(mLastShowClicked);
					break;
					}*/
				case kWhenToWatchSeries: // when to watch
					{
						int code = -1;
						if ( mLastEpClicked != nullptr )
							code = mLastEpClicked->TheSeries->TVDBCode;
						if ( mLastShowClicked != nullptr)
							code = mLastShowClicked->TVDBCode;

						if (code != -1)
						{
							tabControl1->SelectTab(tbWTW);

							for (int i=0;i<lvWhenToWatch->Items->Count;i++)
								lvWhenToWatch->Items[i]->Selected = false;

							for (int i=0;i<lvWhenToWatch->Items->Count;i++)
							{
								ListViewItem ^lvi = lvWhenToWatch->Items[i];
								ProcessedEpisode ^ei = safe_cast<ProcessedEpisode ^>(lvi->Tag);
								if ((ei != nullptr) && (ei->TheSeries->TVDBCode == code))
									lvi->Selected = true;
							}
							lvWhenToWatch->Focus();
						}
						break;
					}
				case kForceRefreshSeries:
					ForceRefresh(mLastShowClicked);
					break;
				case kBTSearchFor:
					{
						for each (ListViewItem ^lvi in lvAIO->SelectedItems)
						{
							AIOMissing ^m = safe_cast<AIOMissing ^>(lvi->Tag);
							if (m != nullptr)
								mDoc->DoBTSearch(m->PE);
						}
					}
					break;
				case kAIOAction:
					AIOAction(false);
					break;
				case kAIOBrowseForFile:
					{
						AIOMissing ^mi = safe_cast<AIOMissing ^>(mLastAIOClicked);
						if (mi != nullptr)
						{
							// browse for mLastAIOClicked
							openFile->Filter = "Video Files|"+mDoc->Settings->GetVideoExtensionsString()->Replace(".","*.")+"|All Files (*.*)|*.*";
							if (openFile->ShowDialog() == System::Windows::Forms::DialogResult::OK)
							{
								// make new AIOItem for copying/moving to specified location
								FileInfo ^from = gcnew FileInfo(openFile->FileName);
								mDoc->TheAIOList->Add(gcnew AIOCopyMoveRename(mDoc->Settings->LeaveOriginals ? AIOCopyMoveRename::Op::Copy : AIOCopyMoveRename::Op::Move,
									from,  
									gcnew FileInfo(mi->TheFileNoExt+from->Extension),
									mi->PE));
								// and remove old Missing item
								mDoc->TheAIOList->Remove(mLastAIOClicked);
								mLastAIOClicked = nullptr;
								FillAIOList();
							}
						}

						break;
					}
				case kAIOIgnore:
					IgnoreSelected();
					break;
				case kAIODelete:
					AIODeleteSelected();
					break;
				default:
					{
						if ((n >= kWatchBase) && (n < kOpenFolderBase))
						{
							int wn = n - kWatchBase;
							if ((mLastFL != nullptr) && (wn >= 0) && (wn < mLastFL->Count))
								TVDoc::SysOpen(mLastFL[wn]->FullName);
						}
						else if (n >= kOpenFolderBase)
						{
							int fnum = n - kOpenFolderBase;

							if (fnum < mFoldersToOpen->Count)
							{
								String ^folder = mFoldersToOpen[fnum];

								if (DirectoryInfo(folder).Exists)
									TVDoc::SysOpen(folder);
							}
							return;
						}
						else
						{
							Diagnostics::Debug::Fail("Unknown right-click action " + n.ToString());
						}
						break;
					}

				}

				mLastEpClicked = nullptr;
			}
			System::Void tabControl1_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
			{
				if (tabControl1->SelectedTab == tbMyShows)
					bnMyShowsRefresh_Click(nullptr,nullptr);
				else if (tabControl1->SelectedTab == tbWTW)
					bnWhenToWatchCheck_Click(nullptr,nullptr);
				else if (tabControl1->SelectedTab == tbAllInOne)
					bnAIOCheck_Click(nullptr,nullptr);
			}
			System::Void folderRightClickMenu_ItemClicked(System::Object^  sender, System::Windows::Forms::ToolStripItemClickedEventArgs^  e) 
			{
				switch (safe_cast<int>(e->ClickedItem->Tag))
				{
				case 0: // open folder
					TVDoc::SysOpen(mLastFolderClicked);
					break;
				default:
					break;
				}
			}
			void RightClickOnFolder(String ^folderPath, Point ^pt)
			{
				mLastFolderClicked = folderPath;
				folderRightClickMenu->Items->Clear();

				ToolStripMenuItem ^tsi;
				int n = 0;

				tsi = gcnew ToolStripMenuItem("Open: " + folderPath);     tsi->Tag = n++; folderRightClickMenu->Items->Add(tsi);

				folderRightClickMenu->Show(*pt);
			}

			System::Void lvWhenToWatch_MouseClick(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			{
				if (e->Button != System::Windows::Forms::MouseButtons::Right)
					return;
				if (lvWhenToWatch->SelectedItems->Count == 0)
					return;

				Point^ pt = lvWhenToWatch->PointToScreen(Point(e->X, e->Y));
				ProcessedEpisode ^ei = safe_cast<ProcessedEpisode ^>(lvWhenToWatch->SelectedItems[0]->Tag);
				RightClickOnShow(ei,pt);
			}


			System::Void preferencesToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				DoPrefs(false);
			}
			void DoPrefs(bool scanOptions)
			{
				Preferences ^pref = gcnew Preferences(mDoc, scanOptions);
				if (pref->ShowDialog() == ::DialogResult::OK)
				{
					mDoc->SetDirty();
					ShowHideNotificationIcon();
					FillWhenToWatchList();
					this->ShowInTaskbar = mDoc->Settings->ShowInTaskbar;
					FillEpGuideHTML();
				}
			}
			System::Void saveToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				mDoc->WriteXMLSettings();
				mDoc->GetTVDB(false,"")->SaveCache();
				SaveLayoutXML();
			}
			System::Void UI_SizeChanged(System::Object^  sender, System::EventArgs^  e) 
			{
				if (this->WindowState == FormWindowState::Normal )
					mLastNonMaximizedSize = this->Size;
				if ((this->WindowState == FormWindowState::Minimized ) && (!mDoc->Settings->ShowInTaskbar))
					this->Hide();
			}
			System::Void UI_LocationChanged(System::Object^  sender, System::EventArgs^  e) 
			{
				if (this->WindowState == FormWindowState::Normal )
					mLastNonMaximizedLocation = this->Location;
			}
			System::Void statusTimer_Tick(System::Object^  sender, System::EventArgs^  e) 
			{
				static int lastN = 0;
				int n = mDoc->DownloadDone ? 0 : mDoc->DownloadsRemaining;

				txtDLStatusLabel->Visible = (n || mDoc->Settings->BGDownload);
				if (n != 0)
				{
					txtDLStatusLabel->Text = "Background download: " + mDoc->GetTVDB(false,"")->CurrentDLTask;
					backgroundDownloadNowToolStripMenuItem->Enabled = false;
				}
				else
					txtDLStatusLabel->Text = "Background download: Idle";

				if (!Busy)
				{
					if ((n == 0) && (lastN > 0))
					{
						// we've just finished a bunch of background downloads
						mDoc->GetTVDB(false,"")->SaveCache();
						RefreshWTW(false);

						backgroundDownloadNowToolStripMenuItem->Enabled = true;
					}
					lastN = n;
				}
			}
			System::Void backgroundDownloadToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				mDoc->Settings->BGDownload = !mDoc->Settings->BGDownload;
				backgroundDownloadToolStripMenuItem->Checked = mDoc->Settings->BGDownload;
				statusTimer_Tick(nullptr,nullptr);
				mDoc->SetDirty();

				if (mDoc->Settings->BGDownload)
					BGDownloadTimer->Start();
				else
					BGDownloadTimer->Stop();
			}
			System::Void BGDownloadTimer_Tick(System::Object^  sender, System::EventArgs^  e) 
			{
				if (Busy)
				{
					BGDownloadTimer->Interval = 10000; // come back in 10 seconds
					BGDownloadTimer->Start();
					return;
				}
				BGDownloadTimer->Interval = BGDLLongInterval(); // after first time (10 seconds), put up to 60 minutes
				BGDownloadTimer->Start();

				if (mDoc->Settings->BGDownload && mDoc->DownloadDone) // only do auto-download if don't have stuff to do already
				{
					mDoc->StartBGDownloadThread(false);

					statusTimer_Tick(nullptr,nullptr);
				}
			}
			System::Void backgroundDownloadNowToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (mDoc->Settings->OfflineMode)
				{
					System::Windows::Forms::DialogResult res = MessageBox::Show("Ignore offline mode and download anyway?",
						"Background Download",MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
					if (res != System::Windows::Forms::DialogResult::Yes)
						return;
				}
				BGDownloadTimer->Stop();
				BGDownloadTimer->Start();

				mDoc->StartBGDownloadThread(false);

				statusTimer_Tick(nullptr,nullptr);
			}
			System::Void offlineOperationToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (!mDoc->Settings->OfflineMode)
					if (MessageBox::Show("Are you sure you wish to go offline?","TVRename",MessageBoxButtons::YesNo, MessageBoxIcon::Warning) == ::DialogResult::No)
						return;

				mDoc->Settings->OfflineMode = !mDoc->Settings->OfflineMode;
				offlineOperationToolStripMenuItem->Checked = mDoc->Settings->OfflineMode;
				mDoc->SetDirty();
			}

			System::Void tabControl1_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
			{
				if (tabControl1->SelectedTab == tbMyShows)
					FillEpGuideHTML();

				exportToolStripMenuItem->Enabled = false; /*( (tabControl1->SelectedTab == tbMissing) ||
														  (tabControl1->SelectedTab == tbFnO) ||
														  (tabControl1->SelectedTab == tbRenaming) );*/
			}

			System::Void bugReportToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				BugReport ^br = gcnew BugReport(mDoc);
				br->ShowDialog();
			}
			System::Void exportToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				/*
				if (tabControl1->SelectedTab == tbMissing)
				{
				if (!MissingListHasStuff())
				return;

				saveFile->Filter = "CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml";
				if (saveFile->ShowDialog() != ::DialogResult::OK)
				return;

				if (saveFile->FilterIndex == 1) // counts from 1
				mDoc->ExportMissingCSV(saveFile->FileName);
				else if (saveFile->FilterIndex == 2)
				mDoc->ExportMissingXML(saveFile->FileName);
				}
				else if (tabControl1->SelectedTab == tbFnO)
				{
				saveFile->Filter = "XML Files|*.xml";
				if (saveFile->ShowDialog() != ::DialogResult::OK)
				return;
				mDoc->ExportFOXML(saveFile->FileName);
				}
				else if (tabControl1->SelectedTab == tbRenaming)
				{
				saveFile->Filter = "XML Files|*.xml";
				if (saveFile->ShowDialog() != ::DialogResult::OK)
				return;
				mDoc->ExportRenamingXML(saveFile->FileName);
				}
				*/
			}


			void ShowHideNotificationIcon()
			{
				notifyIcon1->Visible = mDoc->Settings->NotificationAreaIcon && !mDoc->HasArg("/hide");
			}
			System::Void statisticsToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e)
			{
				StatsWindow ^sw = gcnew StatsWindow(mDoc->Stats());
				sw->ShowDialog();
			}


			////// //////// ////// //////// ////// //////// ////// //////// ////// //////// ////// //////// ////// //////// 


			TreeNode ^AddShowItemToTree(ShowItem ^si)
			{
				TheTVDB ^db = mDoc->GetTVDB(true,"AddShowItemToTree");
				String ^name = si->ShowName();

				SeriesInfo ^ser = db->GetSeries(si->TVDBCode);

				if (String::IsNullOrEmpty(name))
				{
					if (ser != nullptr)
						name = ser->Name;
					else
						name = "-- Unknown : " + si->TVDBCode + " --";
				}

				TreeNode ^n = gcnew TreeNode(name);
				n->Tag = si;


				if (ser != nullptr)
				{
					Generic::List<int> ^theKeys = gcnew Generic::List<int>;
					// now, go through and number them all sequentially
					for each (int snum in  ser->Seasons->Keys)
						theKeys->Add(snum);

					theKeys->Sort();

					for each (int snum in theKeys)
					{
						String ^nodeTitle = snum == 0 ? "Specials" : "Season " + snum.ToString();
						TreeNode ^n2 = gcnew TreeNode(nodeTitle);
						if (si->IgnoreSeasons->Contains(snum))
							n2->ForeColor = Color::Gray;
						n2->Tag = ser->Seasons[snum];
						n->Nodes->Add(n2);
					}
				}

				MyShowTree->Nodes->Add(n);

				db->Unlock("AddShowItemToTree");

				return n;
			}

			void UpdateWTW(ProcessedEpisode ^pe, ListViewItem ^lvi)
			{
				lvi->Tag = pe;

				// group 0 = just missed
				//       1 = this week
				//       2 = future / unknown

				DateTime ^dt = pe->GetAirDateDT(true);

				double ttn = (dt->Subtract(DateTime::Now)).TotalHours;

				if (ttn < 0)
					lvi->Group = lvWhenToWatch->Groups[0];
				else if (ttn < (7*24))
					lvi->Group = lvWhenToWatch->Groups[1];
				else if (!pe->NextToAir)
					lvi->Group = lvWhenToWatch->Groups[3];
				else
					lvi->Group = lvWhenToWatch->Groups[2];

				int n = 1;
				lvi->Text = pe->SI->ShowName();
				lvi->SubItems[n++]->Text = (pe->SeasonNumber != 0) ? pe->SeasonNumber.ToString() : "Special";
				String ^estr = (pe->EpNum > 0) ? pe->EpNum.ToString() : "";
				if ((pe->EpNum > 0) && (pe->EpNum2 != pe->EpNum) && (pe->EpNum2 > 0))
					estr += "-" + pe->EpNum2.ToString();
				lvi->SubItems[n++]->Text = estr;
				lvi->SubItems[n++]->Text = dt->ToShortDateString();
				lvi->SubItems[n++]->Text = dt->ToString("t");
				lvi->SubItems[n++]->Text = dt->ToString("ddd");
				lvi->SubItems[n++]->Text = pe->HowLong();
				lvi->SubItems[n++]->Text = pe->Name;

				// icon..
				if (pe->GetAirDateDT(true)->CompareTo(DateTime::Now) < 0) // has aired
				{
					FileList ^fl = mDoc->FindEpOnDisk(pe);
					if ((fl != nullptr) && (fl->Count > 0))
						lvi->ImageIndex = 0;
					else
						if (pe->SI->DoMissingCheck)
							lvi->ImageIndex = 1;
				}

			}

			void SelectSeason(Season ^seas)
			{
				for each (TreeNode ^n in MyShowTree->Nodes)
					for each (TreeNode ^n2 in n->Nodes)
						if (TreeNodeToSeason(n2) == seas)
						{
							n2->EnsureVisible();
							MyShowTree->SelectedNode = n2;
							return;
						}
						FillEpGuideHTML(nullptr);
			}
			void SelectShow(ShowItem ^si)
			{
				for each (TreeNode ^n in MyShowTree->Nodes)
					if (TreeNodeToShowItem(n) == si)
					{
						n->EnsureVisible();
						MyShowTree->SelectedNode = n;
						//FillEpGuideHTML();
						return;
					}
					FillEpGuideHTML(nullptr);
			}

	private: System::Void bnMyShowsAdd_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 MoreBusy();
				 ShowItem ^si = gcnew ShowItem(mDoc->GetTVDB(false,""));
				 TheTVDB ^db = mDoc->GetTVDB(true,"AddShow");
				 AddEditShow ^aes = gcnew AddEditShow(si, db, TZMagic::DefaultTZ());
				 System::Windows::Forms::DialogResult dr = aes->ShowDialog();
				 db->Unlock("AddShow");
				 if (dr == System::Windows::Forms::DialogResult::OK)
				 {
					 mDoc->GetShowItems(true)->Add(si);
					 mDoc->UnlockShowItems();
					 SeriesInfo ^ser = db->GetSeries(si->TVDBCode);
					 if (ser != nullptr)
						 ser->TimeZone = aes->TimeZone;
					 ShowAddedOrEdited(true);
					 SelectShow(si);
				 }
				 LessBusy();
			 }

			 void ShowAddedOrEdited(bool download)
			 {
				 mDoc->SetDirty();
				 RefreshWTW(download);
				 FillShowLists();
			 }

	private: System::Void bnMyShowsDelete_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 TreeNode ^n = MyShowTree->SelectedNode;
				 ShowItem ^si = TreeNodeToShowItem(n);
				 if (si == nullptr)
					 return;

				 System::Windows::Forms::DialogResult res = MessageBox::Show("Remove show \"" + si->ShowName() + "\".  Are you sure?","Confirmation",MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
				 if (res != System::Windows::Forms::DialogResult::Yes)
					 return;

				 mDoc->GetShowItems(true)->Remove(si);
				 mDoc->UnlockShowItems();
				 ShowAddedOrEdited(false);
			 }
	private: System::Void bnMyShowsEdit_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 TreeNode ^n = MyShowTree->SelectedNode;
				 if (n == nullptr)
					 return;
				 Season ^seas = TreeNodeToSeason(n);
				 if (seas != nullptr)
				 {
					 ShowItem ^si = TreeNodeToShowItem(n);
					 if (si != nullptr)
						 EditSeason(si, seas->SeasonNumber);
					 return;
				 }

				 ShowItem ^si = TreeNodeToShowItem(n);
				 if (si != nullptr)
				 {
					 EditShow(si);
					 return;
				 }
			 }

			 void EditSeason(ShowItem ^si, int seasnum)
			 {
				 MoreBusy();

				 TheTVDB ^db = mDoc->GetTVDB(true,"EditSeason");
				 SeriesInfo ^ser = db->GetSeries(si->TVDBCode);
				 ProcessedEpisodeList ^pel = TVDoc::GenerateEpisodes(si, ser, seasnum, false);

				 EditRules ^er = gcnew EditRules(si, pel, seasnum, mDoc->Settings->NamingStyle);
				 System::Windows::Forms::DialogResult dr = er->ShowDialog();
				 db->Unlock("EditSeason");
				 if (dr == ::DialogResult::OK)
				 {
					 ShowAddedOrEdited(false);
					 if (ser != nullptr)
						 SelectSeason(ser->Seasons[seasnum]);
				 }

				 LessBusy();
			 }

			 void EditShow(ShowItem ^si)
			 {
				 MoreBusy();
				 TheTVDB ^db = mDoc->GetTVDB(true,"EditShow");
				 SeriesInfo ^ser = db->GetSeries(si->TVDBCode);

				 int oldCode = si->TVDBCode;

				 AddEditShow ^aes = gcnew AddEditShow(si, db, ser != nullptr ? ser->TimeZone : TZMagic::DefaultTZ());

				 System::Windows::Forms::DialogResult dr = aes->ShowDialog();

				 db->Unlock("EditShow");

				 if (dr == System::Windows::Forms::DialogResult::OK)
				 {
					 if (ser != nullptr)
						 ser->TimeZone = aes->TimeZone; // TODO: move into AddEditShow

					 ShowAddedOrEdited(si->TVDBCode != oldCode);
					 SelectShow(si);
				 }
				 LessBusy();
			 }

			 void ForceRefresh(ShowItem ^si)
			 {
				 if (si != nullptr)
					 mDoc->GetTVDB(false,"")->ForgetShow(si->TVDBCode, true);
				 mDoc->DoDownloadsFG();
				 FillShowLists();
				 FillEpGuideHTML();
				 RefreshWTW(false);
			 }
	private: System::Void bnMyShowsRefresh_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (Control::ModifierKeys == Keys::Control)
				 {
					 // nuke currently selected show to force getting it fresh
					 TreeNode ^n = MyShowTree->SelectedNode;
					 ShowItem ^si = TreeNodeToShowItem(n);
					 ForceRefresh(si);
				 }
				 else
					 ForceRefresh(nullptr);
			 }
	private: System::Void MyShowTree_AfterSelect(System::Object^  sender, System::Windows::Forms::TreeViewEventArgs^  e) 
			 {
				 FillEpGuideHTML(e->Node);
			 }
	private: System::Void bnMyShowsVisitTVDB_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 TreeNode ^n = MyShowTree->SelectedNode;
				 ShowItem ^si = TreeNodeToShowItem(n);
				 if (si == nullptr)
					 return;
				 Season ^seas = TreeNodeToSeason(n);

				 int sid = -1;
				 if (seas != nullptr)
					 sid = seas->SeasonID;
				 TVDoc::SysOpen(mDoc->GetTVDB(false,"")->WebsiteURL(si->TVDBCode, sid , false));
			 }
	private: System::Void bnMyShowsOpenFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 TreeNode ^n = MyShowTree->SelectedNode;
				 ShowItem ^si = TreeNodeToShowItem(n);
				 if (si == nullptr)
					 return;

				 Season ^seas = TreeNodeToSeason(n);
				 FolderLocationDict ^afl = si->AllFolderLocations(mDoc->Settings);
				 cli::array<int> ^keys = gcnew cli::array<int>(afl->Count);
				 afl->Keys->CopyTo(keys, 0);
				 if ((seas == nullptr) && (keys->Length))
				 {
					 String ^f = si->AutoAdd_FolderBase;
					 if (String::IsNullOrEmpty(f))
					 {
						 int n = keys[0];
						 if (afl[n]->Count)
							 f = afl[n][0];
					 }
					 if (!String::IsNullOrEmpty(f))
					 {
						 try {
							 TVDoc::SysOpen(f);
							 return;
						 }
						 catch (...)
						 {
						 }
					 }
				 }

				 if ((seas != nullptr) && (afl->ContainsKey(seas->SeasonNumber)))
				 {
					 for each (String ^folder in afl[seas->SeasonNumber])
						 if (DirectoryInfo(folder).Exists)
						 {
							 TVDoc::SysOpen(folder);
							 return;
						 }
				 }
				 try {
					 if (!String::IsNullOrEmpty(si->AutoAdd_FolderBase) && (DirectoryInfo(si->AutoAdd_FolderBase).Exists))
						 TVDoc::SysOpen(si->AutoAdd_FolderBase);
				 }
				 catch (...)
				 {
				 }

			 }
	private: System::Void MyShowTree_MouseClick(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			 {
				 if (e->Button != System::Windows::Forms::MouseButtons::Right)
					 return;

				 MyShowTree->SelectedNode = MyShowTree->GetNodeAt(e->X, e->Y);

				 Point^ pt = MyShowTree->PointToScreen(Point(e->X, e->Y));
				 TreeNode ^n = MyShowTree->SelectedNode;

				 if (n == nullptr)
					 return;

				 ShowItem ^si = TreeNodeToShowItem(n);
				 Season ^seas = TreeNodeToSeason(n);

				 if (seas != nullptr)
					 RightClickOnMyShows(seas,pt);
				 else if (si != nullptr)
					 RightClickOnMyShows(si,pt);
			 }



	private: System::Void quickstartGuideToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 ShowQuickStartGuide();
			 }
			 ProcessedEpisodeList ^CurrentlySelectedPEL()
			 {
				 Season ^currentSeas = TreeNodeToSeason(MyShowTree->SelectedNode);
				 ShowItem ^currentSI = TreeNodeToShowItem(MyShowTree->SelectedNode);

				 int snum = (currentSeas != nullptr) ? currentSeas->SeasonNumber : 1;
				 ProcessedEpisodeList ^pel = nullptr;
				 if ((currentSI != nullptr) && (currentSI->SeasonEpisodes->ContainsKey(snum)))
					 pel = currentSI->SeasonEpisodes[snum];
				 else
				 {
					 for each (ShowItem ^si in mDoc->GetShowItems(true))
					 {
						 for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in si->SeasonEpisodes)
						 {
							 pel = kvp->Value;
							 break;
						 }
						 if (pel != nullptr)
							 break;
					 }
					 mDoc->UnlockShowItems();
				 }
				 return pel;
			 }
	private: System::Void filenameTemplateEditorToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 CustomName ^cn = gcnew CustomName(mDoc->Settings->NamingStyle->StyleString);
				 CustomNameDesigner ^cne = gcnew CustomNameDesigner(CurrentlySelectedPEL(), cn, mDoc);
				 ::DialogResult dr = cne->ShowDialog();
				 if (dr == ::DialogResult::OK)
				 {
					 mDoc->Settings->NamingStyle = cn;
					 mDoc->SetDirty();
				 }
			 }
	private: System::Void searchEnginesToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 ProcessedEpisodeList ^pel = CurrentlySelectedPEL();

				 AddEditSearchEngine ^aese = gcnew AddEditSearchEngine(mDoc->GetSearchers(), ((pel != nullptr) && (pel->Count)) ? pel[0] : nullptr);
				 ::DialogResult dr = aese->ShowDialog();
				 if (dr == ::DialogResult::OK)
				 {
					 mDoc->SetDirty();
					 UpdateSearchButton();
				 }
			 }
	private: System::Void filenameProcessorsToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 //Season ^currentSeas = TreeNodeToSeason(MyShowTree->SelectedNode);
				 ShowItem ^currentSI = TreeNodeToShowItem(MyShowTree->SelectedNode);
				 String ^theFolder = "";

				 if (currentSI != nullptr)
				 {
					 for each (KeyValuePair<int, StringList ^> ^kvp in currentSI->AllFolderLocations(mDoc->Settings))
					 {
						 for each (String ^folder in kvp->Value)
						 {
							 if ((!String::IsNullOrEmpty(folder)) && DirectoryInfo(folder).Exists)
							 {
								 theFolder = folder;
								 break;
							 }
						 }
						 if (!String::IsNullOrEmpty(theFolder))
							 break;
					 }
				 }

				 AddEditSeasEpFinders ^d = gcnew AddEditSeasEpFinders(mDoc->Settings->FNPRegexs, mDoc->GetShowItems(true), 
					 currentSI, theFolder, mDoc->Settings);
				 mDoc->UnlockShowItems();

				 ::DialogResult dr = d->ShowDialog();
				 if (dr == ::DialogResult::OK)
				 {
					 mDoc->SetDirty();
				 }


			 }
	private: System::Void actorsToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 ActorsGrid(mDoc).ShowDialog();
			 }
	private: System::Void quickTimer_Tick(System::Object^  sender, System::EventArgs^  e) 
			 {
				 quickTimer->Stop();
				 ProcessArgs();
			 }
	private: System::Void uTorrentToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 uTorrent ^ut = gcnew uTorrent(mDoc, this->SetProgress);
				 ut->ShowDialog();
				 tabControl1->SelectedIndex = 1; // go to all-in-one tab
			 }
	private: System::Void bnMyShowsCollapse_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 MyShowTree->CollapseAll();
			 }
	private: System::Void UI_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			 {
				 int t = -1;
				 if (e->Control && (e->KeyCode == Keys::D1))
					 t = 0;
				 else if (e->Control && (e->KeyCode == Keys::D2))
					 t = 1;
				 else if (e->Control && (e->KeyCode == Keys::D3))
					 t = 2;
				 else if (e->Control && (e->KeyCode == Keys::D4))
					 t = 3;
				 else if (e->Control && (e->KeyCode == Keys::D5))
					 t = 4;
				 else if (e->Control && (e->KeyCode == Keys::D6))
					 t = 5;
				 else if (e->Control && (e->KeyCode == Keys::D7))
					 t = 6;
				 else if (e->Control && (e->KeyCode == Keys::D8))
					 t = 7;
				 else if (e->Control && (e->KeyCode == Keys::D9))
					 t = 8;
				 else if (e->Control && (e->KeyCode == Keys::D0))
					 t = 9;
				 if ((t >= 0) && (t < tabControl1->TabCount))
				 {
					 tabControl1->SelectedIndex = t;
					 e->Handled = true;
				 }
			 }
	private: System::Void bnAIOCheck_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 Scan(nullptr);
			 }

			 void Scan(ShowItem ^s)
			 {
				 MoreBusy();
				 mDoc->AIOGo(this->SetProgress, s);
				 LessBusy();
				 FillAIOList();
			 }

			 String ^GBMB(__int64 size)
			 {
				 __int64 gb1 = (1024*1024*1024);
				 __int64 gb =  ((gb1/2) + size)/gb1;
				 if (gb > 1)
					 return gb.ToString() + " GB";
				 else
				 {
					 __int64 mb1 = 1024*1024;
					 __int64 mb  = ((mb1/2) + size)/mb1;
					 return mb.ToString() + " MB";
				 }
			 }
			 String ^itemitems(int n)
			 {
				 if (n == 1)
					 return "Item";
				 else
					 return "Items";
			 }
	private: System::Void lvAIO_RetrieveVirtualItem(System::Object^  sender, System::Windows::Forms::RetrieveVirtualItemEventArgs^  e) 
			 {
				 AIOItem ^aio = mDoc->TheAIOList[e->ItemIndex];
				 ListViewItem ^lvi = aio->GetLVI(lvAIO);
				 int n = aio->IconNumber();
				 if (n != -1)
					 lvi->ImageIndex = n;
				 lvi->Checked = true;
				 lvi->Tag = aio;

				 const int kErrCol = 8;
				 Diagnostics::Debug::Assert(lvi->SubItems->Count <= kErrCol);

				 while (lvi->SubItems->Count < kErrCol)
					 lvi->SubItems->Add(""); // pad our way to the error column

				 if (aio->HasError)
				 {

					 lvi->SubItems->Add(aio->ErrorText); // error text
					 lvi->BackColor = WarningColor();
					 if ((aio->Type == AIOType::kMissing)  || (aio->Type == AIOType::kuTorrenting))
						 lvi->Checked = false;
				 }
				 else
					 lvi->SubItems->Add("");

				 Diagnostics::Debug::Assert(lvi->SubItems->Count == lvAIO->Columns->Count);

				 e->Item = lvi;
			 }

			 void FillAIOList()
			 {
				 if (lvAIO->VirtualMode)
				 {
					 lvAIO->VirtualListSize = mDoc->TheAIOList->Count;
				 }
				 else
				 {
					 lvAIO->BeginUpdate();
					 lvAIO->Items->Clear();

					 for each (AIOItem ^aio in mDoc->TheAIOList)
					 {
						 ListViewItem ^lvi = aio->GetLVI(lvAIO);
						 lvi->Checked = true;
						 lvi->Tag = aio;
						 lvAIO->Items->Add(lvi);

						 int n = aio->IconNumber();
						 if (n != -1)
							 lvi->ImageIndex = n;

						 const int kErrCol = 8;
						 Diagnostics::Debug::Assert(lvi->SubItems->Count <= kErrCol);
						 if (aio->HasError)
						 {
							 while (lvi->SubItems->Count < kErrCol)
								 lvi->SubItems->Add(""); // pad our way to the error column
							 lvi->SubItems->Add(aio->ErrorText); // error text
							 lvi->BackColor = WarningColor();
							 if ((aio->Type == AIOType::kMissing)  || (aio->Type == AIOType::kuTorrenting))
								 lvi->Checked = false;
						 }
					 }
					 lvAIO->EndUpdate();
				 }

				 // do nice totals for each group
				 int missingCount = 0;
				 int renameCount = 0;
				 int copyCount = 0;
				 __int64 copySize = 0;
				 int moveCount = 0;
				 __int64 moveSize = 0;
				 int rssCount = 0;
				 int downloadCount = 0;
				 int nfoCount = 0;
				 int utCount = 0;

				 for each (AIOItem ^aio in mDoc->TheAIOList)
				 {
					 if (aio->Type == AIOType::kMissing)
						 missingCount++;
					 else if (aio->Type == AIOType::kCopyMoveRename)
					 {
						 AIOCopyMoveRename ^cmr = safe_cast<AIOCopyMoveRename ^>(aio);
						 AIOCopyMoveRename::Op op = cmr->Operation;
						 if (op == AIOCopyMoveRename::Op::Copy)
						 {
							 copyCount++;
							 if (cmr->From->Exists)
								 copySize += cmr->From->Length;
						 }
						 else if (op == AIOCopyMoveRename::Op::Move)
						 {
							 moveCount++;
							 if (cmr->From->Exists)
								 moveSize += cmr->From->Length;
						 }
						 else if (op == AIOCopyMoveRename::Op::Rename)
							 renameCount++;
					 }
					 else if (aio->Type == AIOType::kDownload)
						 downloadCount++;
					 else if (aio->Type == AIOType::kRSS)
						 rssCount++;
					 else if (aio->Type == AIOType::kNFO)
						 nfoCount++;
					 else if (aio->Type == AIOType::kuTorrenting)
						 utCount++;
				 }

				 lvAIO->Groups[0]->Header = "Missing (" + missingCount.ToString() + " " + itemitems(missingCount)+")";
				 lvAIO->Groups[1]->Header = "Rename (" + renameCount.ToString() + " " + itemitems(renameCount)+")";
				 lvAIO->Groups[2]->Header = "Copy (" + copyCount.ToString() + " " + itemitems(copyCount)+", "+GBMB(copySize)+")";
				 lvAIO->Groups[3]->Header = "Move (" + moveCount.ToString() + " " + itemitems(moveCount)+", "+GBMB(moveSize)+")";
				 lvAIO->Groups[4]->Header = "Download RSS (" + rssCount.ToString() + " " + itemitems(rssCount)+")";
				 lvAIO->Groups[5]->Header = "Download (" + downloadCount.ToString() + " " + itemitems(downloadCount)+")";
				 lvAIO->Groups[6]->Header = "NFO File (" + nfoCount.ToString() + " " + itemitems(nfoCount)+")";
				 lvAIO->Groups[7]->Header = "Downloading In µTorrent (" + utCount.ToString() + " " + itemitems(utCount)+")";


			 }

	private: System::Void bnAIOAction_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 AIOAction(true);
			 }
			 void AIOAction(bool checked)
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, checked);
				 mDoc->AIOAction(this->SetProgress,lvr->FlatList);
				 // remove items from master list, unless it had an error
				 for each (AIOItem ^i2 in (gcnew LVResults(lvAIO, checked))->FlatList)
					 if (!lvr->FlatList->Contains(i2))
						 mDoc->TheAIOList->Remove(i2);

				 FillAIOList();
				 RefreshWTW(false);
			 }
	private: System::Void folderMonitorToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 FolderMonitor ^fm = gcnew FolderMonitor(mDoc);
				 fm->ShowDialog();
				 FillShowLists();
			 }
	private: System::Void torrentMatchToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 TorrentMatch ^tm = gcnew TorrentMatch(mDoc, SetProgress);
				 tm->ShowDialog();			 
				 FillAIOList();
			 }
	private: System::Void bnAIOWhichSearch_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 ChooseSiteMenu(0);
			 }
	private: System::Void lvAIO_MouseClick(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			 {
				 if (e->Button != System::Windows::Forms::MouseButtons::Right)
					 return;

				 // build the right click menu for the _selected_ items, and types of items
				 LVResults ^lvr = gcnew LVResults(lvAIO, false);

				 if (!lvr->Count)
					 return; // nothing selected

				 Point^ pt = lvAIO->PointToScreen(Point(e->X, e->Y));

				 showRightClickMenu->Items->Clear();

				 // AIO related items
				 ToolStripMenuItem ^tsi;
				 if (lvr->Count > lvr->Missing->Count) // not just missing selected
				 {
					 tsi = gcnew ToolStripMenuItem("Action Selected");     tsi->Tag = (int)kAIOAction; showRightClickMenu->Items->Add(tsi);
				 }

				 tsi = gcnew ToolStripMenuItem("Ignore Selected");     tsi->Tag = (int)kAIOIgnore; showRightClickMenu->Items->Add(tsi);

				 tsi = gcnew ToolStripMenuItem("Remove Selected");     tsi->Tag = (int)kAIODelete; showRightClickMenu->Items->Add(tsi);

				 if (lvr->Count == lvr->Missing->Count) // only missing items selected?
				 {
					 showRightClickMenu->Items->Add(gcnew ToolStripSeparator());

					 tsi = gcnew ToolStripMenuItem("BT Search");     
					 tsi->Tag = (int)kBTSearchFor; 
					 showRightClickMenu->Items->Add(tsi);

					 if (lvr->Count == 1) // only one selected
					 {
						 tsi = gcnew ToolStripMenuItem("Browse For...");     
						 tsi->Tag = (int)kAIOBrowseForFile; 
						 showRightClickMenu->Items->Add(tsi);
					 }
				 }

				 MenuGuideAndTVDB(true);
				 MenuFolders(lvr);

				 showRightClickMenu->Show(*pt);
			 }
	private: System::Void lvAIO_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, false);

				 if (!lvr->Count)
				 {
					 // disable everything
					 bnAIOBTSearch->Enabled = false;
					 return;
				 }

				 if (lvr->Download->Count)
					 bnAIOBTSearch->Enabled = false;
				 else
					 bnAIOBTSearch->Enabled = true;

				 mLastShowClicked = nullptr;
				 mLastEpClicked = nullptr;
				 mLastSeasonClicked = nullptr;
				 mLastAIOClicked = nullptr;

				 showRightClickMenu->Items->Clear();
				 mFoldersToOpen = gcnew StringList();
				 mLastFL = gcnew FileList();

				 if ((lvr->Count == 1) && (lvAIO->FocusedItem != nullptr) && (lvAIO->FocusedItem->Tag != nullptr))
				 {
					 AIOItem ^aio = safe_cast<AIOItem ^>(lvAIO->FocusedItem->Tag);
					 mLastAIOClicked = aio;

					 mLastEpClicked = aio->PE;
					 if (aio->PE != nullptr)
					 {
						 mLastSeasonClicked = aio->PE->TheSeason;
						 mLastShowClicked = aio->PE->SI;
					 }
					 else
					 {
						 mLastSeasonClicked = nullptr;
						 mLastShowClicked = nullptr;
					 }


					 if ((mLastEpClicked != nullptr) && (mDoc->Settings->AutoSelectShowInMyShows))
						 GotoEpguideFor(mLastEpClicked, false);
				 }
			 }
			 void AIODeleteSelected()
			 {
				 ListView::SelectedListViewItemCollection ^sel = lvAIO->SelectedItems;
				 for each (ListViewItem ^lvi in sel)
					 mDoc->TheAIOList->Remove(safe_cast<AIOItem ^>(lvi->Tag));
				 FillAIOList();
			 }
	private: System::Void lvAIO_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			 {
				 if (e->KeyCode == Keys::Delete)
					 AIODeleteSelected();
			 }
	private: System::Void bnAIOIgnore_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 IgnoreSelected();
			 }
	private: System::Void bnAIOAllNone_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, true);
				 bool some = lvr->Count > 0;
				 for each (ListViewItem ^lvi in lvAIO->Items)
					 lvi->Checked = some ? false : true;
			 }
	private: System::Void bnAIORename_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, true);
				 bool some = lvr->Rename->Count > 0;
				 for each (ListViewItem ^lvi in lvAIO->Items)
				 {
					 AIOItem ^i = safe_cast<AIOItem ^>(lvi->Tag);
					 if ((i != nullptr) && (i->Type == AIOType::kCopyMoveRename) && 
						 (safe_cast<AIOCopyMoveRename ^>(i)->Operation == AIOCopyMoveRename::Op::Rename))
						 lvi->Checked = some ? false : true;
				 }
			 }
	private: System::Void bnAIOCopyMove_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, true);
				 bool some = lvr->CopyMove->Count > 0;
				 for each (ListViewItem ^lvi in lvAIO->Items)
				 {
					 AIOItem ^i = safe_cast<AIOItem ^>(lvi->Tag);
					 if ((i != nullptr) && (i->Type == AIOType::kCopyMoveRename) && 
						 (safe_cast<AIOCopyMoveRename ^>(i)->Operation != AIOCopyMoveRename::Op::Rename))
						 lvi->Checked = some ? false : true;
				 }
			 }
	private: System::Void bnAIONFO_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, true);
				 bool some = lvr->NFO->Count > 0;
				 for each (ListViewItem ^lvi in lvAIO->Items)
				 {
					 AIOItem ^i = safe_cast<AIOItem ^>(lvi->Tag);
					 if ((i != nullptr) && (i->Type == AIOType::kNFO))
						 lvi->Checked = some ? false : true;
				 }
			 }

	private: System::Void bnAIORSS_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, true);
				 bool some = lvr->RSS->Count > 0;
				 for each (ListViewItem ^lvi in lvAIO->Items)
				 {
					 AIOItem ^i = safe_cast<AIOItem ^>(lvi->Tag);
					 if ((i != nullptr) && (i->Type == AIOType::kRSS))
						 lvi->Checked = some ? false : true;
				 }
			 }
	private: System::Void bnAIODownloads_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, true);
				 bool some = lvr->Download->Count > 0;
				 for each (ListViewItem ^lvi in lvAIO->Items)
				 {
					 AIOItem ^i = safe_cast<AIOItem ^>(lvi->Tag);
					 if ((i != nullptr) && (i->Type == AIOType::kDownload))
						 lvi->Checked = some ? false : true;
				 }
			 }
	private: System::Void lvAIO_ItemCheck(System::Object^  sender, System::Windows::Forms::ItemCheckEventArgs^  e) 
			 {
				 if ((e->Index < 0) || (e->Index > lvAIO->Items->Count))
					 return;
				 AIOItem ^aio = safe_cast<AIOItem ^>(lvAIO->Items[e->Index]->Tag);
				 if ((aio != nullptr) && ( (aio->Type == AIOType::kMissing) || (aio->Type == AIOType::kuTorrenting)))
					 e->NewValue = CheckState::Unchecked;
			 }
	private: System::Void bnAIOOptions_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 DoPrefs(true);
			 }
	private: System::Void lvAIO_MouseDoubleClick(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			 {
				 // double-click on an item will search for missing, do nothing (for now) for anything else
				 for each (AIOMissing ^miss in LVResults(lvAIO, false).Missing)
					 if (miss->PE != nullptr)
						 mDoc->DoBTSearch(miss->PE);
			 }
	private: System::Void bnAIOBTSearch_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, false);

				 if (!lvr->Count)
					 return;

				 for each (AIOItem ^i in lvr->FlatList)
					 if (i->PE != nullptr)
						 mDoc->DoBTSearch(i->PE);
			 }

	private: System::Void bnRemoveSel_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 AIODeleteSelected();
			 }
			 void IgnoreSelected()
			 {
				 LVResults ^lvr = gcnew LVResults(lvAIO, false);
				 bool added = false;
				 for each (AIOItem ^aio in lvr->FlatList)
				 {
					 IgnoreItem ^ii = aio->GetIgnore();
					 if (ii != nullptr)
					 {
						 mDoc->Ignore->Add(ii);
						 added = true;
					 }
				 }
				 if (added)
				 {
					 mDoc->SetDirty();
					 mDoc->RemoveIgnored();
					 FillAIOList();
				 }
			 }
	private: System::Void ignoreListToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 IgnoreEdit ^ie = gcnew IgnoreEdit(mDoc);
				 ie->ShowDialog();
			 }
}; // UI class
} // namespace
