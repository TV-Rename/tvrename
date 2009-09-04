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
#include "AddItem.h"
#include "EditRules.h"
#include "FolderMonitorProgress.h"
#include "CustomNameDesigner.h"
#include "CustomName.h"
#include "AddEditSearchEngine.h"
#include "AddEditSeasEpFinders.h"
#include "PreferredLanguage.h"
#include "Version.h"
#include "uTorrent.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Xml;
using namespace System::Threading;

// using namespace System::Threading;

namespace TVRename {
	// right click commands
	enum { kEpisodeGuideForShow = 1, kVisitTVDBEpisode, kVisitTVDBSeason, kVisitTVDBSeries,
		   kMissingCheckSeries, kWhenToWatchSeries, kForceRefreshSeries, kRenamingCheckSeries,
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
	public: 

	public: 
		static bool ExperimentalFeatures;

		static bool IncludeExperimentalStuff()
		{
			return ExperimentalFeatures;
		}

	protected: 
		TheTVDBCodeFinder ^mTCCF;
		TVDoc ^mDoc;
		System::Drawing::Size mLastNonMaximizedSize;
		Point mLastNonMaximizedLocation;
		int mInternalChange;
		int Busy;

		ProcessedEpisode ^mLastEpClicked;
		Season ^mLastSeasonClicked;
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
	private: System::Windows::Forms::TabPage^  tbRenaming;
	private: System::Windows::Forms::TabPage^  tbFnO;
	private: System::Windows::Forms::TabPage^  tbWTW;
	private: System::Windows::Forms::ColumnHeader^  columnHeader5;
	private: System::Windows::Forms::ColumnHeader^  columnHeader6;
	private: System::Windows::Forms::ColumnHeader^  columnHeader7;
	private: System::Windows::Forms::ColumnHeader^  columnHeader8;
	private: System::Windows::Forms::Button^  bnRenameCheck;
	private: System::Windows::Forms::Button^  bnRenameDoRenaming;
	private: System::Windows::Forms::ListView^  lvRenameList;
	private: System::Windows::Forms::ColumnHeader^  columnHeader9;
	private: System::Windows::Forms::ColumnHeader^  columnHeader10;
	private: System::Windows::Forms::ColumnHeader^  columnHeader11;
	private: System::Windows::Forms::ColumnHeader^  columnHeader12;
	private: System::Windows::Forms::TabPage^  tbMissing;
	private: System::Windows::Forms::ListView^  lvMissingList;
	private: System::Windows::Forms::ColumnHeader^  columnHeader16;
	private: System::Windows::Forms::ColumnHeader^  columnHeader17;
	private: System::Windows::Forms::ColumnHeader^  columnHeader18;
	private: System::Windows::Forms::ColumnHeader^  columnHeader19;
	private: System::Windows::Forms::ColumnHeader^  columnHeader20;
	private: System::Windows::Forms::CheckBox^  cbLeaveOriginals;
	private: System::Windows::Forms::Button^  bnFindMissingStuff;
	private: System::Windows::Forms::ColumnHeader^  columnHeader21;
	private: System::Windows::Forms::ColumnHeader^  columnHeader22;
	private: System::Windows::Forms::ColumnHeader^  columnHeader23;
	private: System::Windows::Forms::ColumnHeader^  columnHeader24;
	private: System::Windows::Forms::Button^  bnDoMissingCheck;
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
	private: System::Windows::Forms::Button^  bnChooseSite;
	private: System::Windows::Forms::Button^  bnBTSearch;
	private: System::Windows::Forms::TextBox^  txtWhenToWatchSynopsis;
	private: System::Windows::Forms::MonthCalendar^  calCalendar;
	private: System::Windows::Forms::TabPage^  tbTorrentMatch;
	private: System::Windows::Forms::Button^  bnBTOpenFolder;
	private: System::Windows::Forms::TreeView^  tmatchTree;
	private: System::Windows::Forms::Button^  bnGo;
	private: System::Windows::Forms::Button^  bnBrowseFolder;
	private: System::Windows::Forms::TextBox^  txtFolder;
	private: System::Windows::Forms::Button^  bnBrowseTorrent;
	private: System::Windows::Forms::Label^  label3;
	private: System::Windows::Forms::TextBox^  txtTorrentFile;
	private: System::Windows::Forms::Label^  label4;
	private: System::Windows::Forms::OpenFileDialog^  openFile;
	private: System::Windows::Forms::FolderBrowserDialog^  folderBrowser;
	private: System::Windows::Forms::Button^  bnWhenToWatchCheck;
	private: System::Windows::Forms::ColumnHeader^  columnHeader39;
	private: System::Windows::Forms::ColumnHeader^  columnHeader40;
	private: System::Windows::Forms::ListView^  lvCopyMoveList;
	private: System::Windows::Forms::ContextMenuStrip^  menuSearchSites;
	private: System::Windows::Forms::Button^  bnDoMovingAndCopying;
	private: System::Windows::Forms::Button^  bnOpenSearchFolder;
	private: System::Windows::Forms::Button^  bnRemoveSearchFolder;
	private: System::Windows::Forms::Button^  bnAddSearchFolder;
	private: System::Windows::Forms::ListBox^  lbSearchFolders;
	private: System::Windows::Forms::Label^  label5;
	private: System::Windows::Forms::Timer^  refreshWTWTimer;
	private: System::Windows::Forms::Button^  bnWTWChooseSite;
	private: System::Windows::Forms::Button^  bnWTWBTSearch;
	private: System::Windows::Forms::NotifyIcon^  notifyIcon1;
	private: System::Windows::Forms::ColumnHeader^  columnHeader36;
	private: System::Windows::Forms::ToolStripMenuItem^  buyMeADrinkToolStripMenuItem;
	private: System::Windows::Forms::TabPage^  tbFM;
	private: System::Windows::Forms::Button^  bnFMCheck;
	private: System::Windows::Forms::Button^  bnFMOpenMonFolder;
	private: System::Windows::Forms::Button^  bnFMRemoveMonFolder;
	private: System::Windows::Forms::Button^  bnFMAddMonFolder;
	private: System::Windows::Forms::ListBox^  lstFMMonitorFolders;
	private: System::Windows::Forms::Label^  label2;
	private: System::Windows::Forms::Label^  label6;
	private: System::Windows::Forms::SplitContainer^  splitContainer3;
	private: System::Windows::Forms::ListBox^  lstFMIgnoreFolders;
	private: System::Windows::Forms::Label^  label7;
	private: System::Windows::Forms::Button^  bnFMOpenIgFolder;
	private: System::Windows::Forms::Button^  bnFMAddIgFolder;
	private: System::Windows::Forms::Button^  bnFMRemoveIgFolder;
	private: System::Windows::Forms::Button^  bnFMIgnoreNewFolder;
	private: System::Windows::Forms::Button^  bnFMIgnoreAllNewFolders;
	private: System::Windows::Forms::Button^  bnFMRemoveNewFolder;
	private: System::Windows::Forms::Button^  bnFMNewFolderOpen;
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
	private: System::Windows::Forms::Button^  bnBTSecondOpen;
	private: System::Windows::Forms::Button^  bnBTSecondBrowse;
	private: System::Windows::Forms::TextBox^  txtBTSecondLocation;
	private: System::Windows::Forms::ToolStripMenuItem^  bugReportToolStripMenuItem;
	private: System::Windows::Forms::ImageList^  ilWTWIcons;
	private: System::Windows::Forms::ToolStripMenuItem^  exportToolStripMenuItem;
	private: System::Windows::Forms::SaveFileDialog^  saveFile;
	private: System::Windows::Forms::TableLayoutPanel^  tableLayoutPanel1;
	private: System::Windows::Forms::Panel^  panel2;
	private: System::Windows::Forms::Panel^  panel3;
	private: System::Windows::Forms::ListView^  lvFMNewShows;
	private: System::Windows::Forms::ColumnHeader^  columnHeader42;
	private: System::Windows::Forms::Button^  A;
	private: System::Windows::Forms::ColumnHeader^  columnHeader43;
	private: System::Windows::Forms::ColumnHeader^  columnHeader44;
	private: System::Windows::Forms::ColumnHeader^  columnHeader45;
	private: System::Windows::Forms::Panel^  pnlCF;
	private: System::Windows::Forms::Button^  bnFMVisitTVcom;
	private: System::Windows::Forms::Button^  bnFMFullAuto;
	private: System::Windows::Forms::ToolStripMenuItem^  toolsToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  flushCacheToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  backgroundDownloadNowToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  statisticsToolStripMenuItem;
	private: System::Windows::Forms::Button^  bnAddThisOne;
	private: System::Windows::Forms::TabPage^  tbMyShows;
	private: System::Windows::Forms::Button^  bnMyShowsAdd;
	private: System::Windows::Forms::TreeView^  MyShowTree;
	private: System::Windows::Forms::WebBrowser^  epGuideHTML;
	private: System::Windows::Forms::Button^  bnMyShowsRefresh;
	private: System::Windows::Forms::Button^  bnMyShowsDelete;
	private: System::Windows::Forms::Button^  bnMyShowsEdit;
	private: System::Windows::Forms::Button^  bnMyShowsVisitTVDB;
	private: System::Windows::Forms::Button^  bnMyShowsOpenFolder;
	private: System::Windows::Forms::Label^  txtFOCopyMoveStats;
	private: System::Windows::Forms::Label^  txtMissingStats;
	private: System::Windows::Forms::Label^  txtRenamingCount;
	private: System::Windows::Forms::RadioButton^  rbSpecificSeason;
	private: System::Windows::Forms::RadioButton^  rbFlat;
	private: System::Windows::Forms::RadioButton^  rbFolderPerSeason;
	private: System::Windows::Forms::TextBox^  txtFMSpecificSeason;
	private: System::Windows::Forms::ToolStripMenuItem^  quickstartGuideToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  filenameTemplateEditorToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  searchEnginesToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^  filenameProcessorsToolStripMenuItem;
	private: System::Windows::Forms::TableLayoutPanel^  tableLayoutPanel2;
	private: System::Windows::Forms::TabPage^  tbRSS;
	private: System::Windows::Forms::Button^  bnRSSGo;
	private: System::Windows::Forms::ListView^  lvRSSItems;
	private: System::Windows::Forms::ColumnHeader^  columnHeader1;
	private: System::Windows::Forms::ColumnHeader^  columnHeader2;
	private: System::Windows::Forms::ColumnHeader^  columnHeader3;
	private: System::Windows::Forms::TextBox^  txtRSSURL;
	private: System::Windows::Forms::Label^  label8;
	private: System::Windows::Forms::Label^  label1;
	private: System::Windows::Forms::ColumnHeader^  columnHeader4;
	private: System::Windows::Forms::ListView^  lvRSSMatchingMissing;
	private: System::Windows::Forms::ColumnHeader^  columnHeader13;
	private: System::Windows::Forms::ColumnHeader^  columnHeader14;
	private: System::Windows::Forms::ColumnHeader^  columnHeader15;
	private: System::Windows::Forms::Label^  label9;
	private: System::Windows::Forms::ColumnHeader^  columnHeader38;
	private: System::Windows::Forms::ColumnHeader^  columnHeader41;
	private: System::Windows::Forms::ColumnHeader^  columnHeader46;
	private: System::Windows::Forms::Button^  bnRSSDownload;
	private: System::Windows::Forms::ColumnHeader^  columnHeader37;
	private: System::Windows::Forms::ColumnHeader^  columnHeader47;
	private: System::Windows::Forms::ToolStripMenuItem^  languagePreferenceToolStripMenuItem;
	private: System::Windows::Forms::Timer^  tmrShowUpcomingPopup;
	private: System::Windows::Forms::Button^  bnRSSBrowseuTorrent;
	private: System::Windows::Forms::TextBox^  txtRSSuTorrentPath;
	private: System::Windows::Forms::Label^  label10;

	private: System::Windows::Forms::RadioButton^  rbBTRenameFiles;
	private: System::Windows::Forms::RadioButton^  rbBTCopyTo;
private: System::Windows::Forms::Label^  label14;
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
			System::Windows::Forms::ListViewGroup^  listViewGroup5 = (gcnew System::Windows::Forms::ListViewGroup(L"Recently Aired", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup6 = (gcnew System::Windows::Forms::ListViewGroup(L"Next 7 Days", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup7 = (gcnew System::Windows::Forms::ListViewGroup(L"Later", System::Windows::Forms::HorizontalAlignment::Left));
			System::Windows::Forms::ListViewGroup^  listViewGroup8 = (gcnew System::Windows::Forms::ListViewGroup(L"Future Episodes", System::Windows::Forms::HorizontalAlignment::Left));
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
			this->languagePreferenceToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->flushCacheToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->backgroundDownloadNowToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->actorsToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
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
			this->tbRenaming = (gcnew System::Windows::Forms::TabPage());
			this->txtRenamingCount = (gcnew System::Windows::Forms::Label());
			this->bnRenameCheck = (gcnew System::Windows::Forms::Button());
			this->bnRenameDoRenaming = (gcnew System::Windows::Forms::Button());
			this->lvRenameList = (gcnew System::Windows::Forms::ListView());
			this->columnHeader9 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader10 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader11 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader12 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader37 = (gcnew System::Windows::Forms::ColumnHeader());
			this->tbMissing = (gcnew System::Windows::Forms::TabPage());
			this->txtMissingStats = (gcnew System::Windows::Forms::Label());
			this->bnChooseSite = (gcnew System::Windows::Forms::Button());
			this->bnBTSearch = (gcnew System::Windows::Forms::Button());
			this->bnDoMissingCheck = (gcnew System::Windows::Forms::Button());
			this->lvMissingList = (gcnew System::Windows::Forms::ListView());
			this->columnHeader16 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader17 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader40 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader18 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader19 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader20 = (gcnew System::Windows::Forms::ColumnHeader());
			this->tbFnO = (gcnew System::Windows::Forms::TabPage());
			this->txtFOCopyMoveStats = (gcnew System::Windows::Forms::Label());
			this->bnOpenSearchFolder = (gcnew System::Windows::Forms::Button());
			this->bnRemoveSearchFolder = (gcnew System::Windows::Forms::Button());
			this->bnAddSearchFolder = (gcnew System::Windows::Forms::Button());
			this->lbSearchFolders = (gcnew System::Windows::Forms::ListBox());
			this->label5 = (gcnew System::Windows::Forms::Label());
			this->bnDoMovingAndCopying = (gcnew System::Windows::Forms::Button());
			this->cbLeaveOriginals = (gcnew System::Windows::Forms::CheckBox());
			this->bnFindMissingStuff = (gcnew System::Windows::Forms::Button());
			this->lvCopyMoveList = (gcnew System::Windows::Forms::ListView());
			this->columnHeader39 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader21 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader23 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader22 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader24 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader47 = (gcnew System::Windows::Forms::ColumnHeader());
			this->tbFM = (gcnew System::Windows::Forms::TabPage());
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
			this->A = (gcnew System::Windows::Forms::Button());
			this->label6 = (gcnew System::Windows::Forms::Label());
			this->bnFMRemoveNewFolder = (gcnew System::Windows::Forms::Button());
			this->bnFMNewFolderOpen = (gcnew System::Windows::Forms::Button());
			this->bnFMIgnoreAllNewFolders = (gcnew System::Windows::Forms::Button());
			this->bnFMIgnoreNewFolder = (gcnew System::Windows::Forms::Button());
			this->tbTorrentMatch = (gcnew System::Windows::Forms::TabPage());
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
			this->ilWTWIcons = (gcnew System::Windows::Forms::ImageList(this->components));
			this->tbRSS = (gcnew System::Windows::Forms::TabPage());
			this->bnRSSBrowseuTorrent = (gcnew System::Windows::Forms::Button());
			this->bnRSSDownload = (gcnew System::Windows::Forms::Button());
			this->bnRSSGo = (gcnew System::Windows::Forms::Button());
			this->lvRSSMatchingMissing = (gcnew System::Windows::Forms::ListView());
			this->columnHeader13 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader14 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader15 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader38 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader41 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader46 = (gcnew System::Windows::Forms::ColumnHeader());
			this->lvRSSItems = (gcnew System::Windows::Forms::ListView());
			this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader3 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader4 = (gcnew System::Windows::Forms::ColumnHeader());
			this->txtRSSuTorrentPath = (gcnew System::Windows::Forms::TextBox());
			this->label9 = (gcnew System::Windows::Forms::Label());
			this->txtRSSURL = (gcnew System::Windows::Forms::TextBox());
			this->label10 = (gcnew System::Windows::Forms::Label());
			this->label8 = (gcnew System::Windows::Forms::Label());
			this->label1 = (gcnew System::Windows::Forms::Label());
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
			this->menuStrip1->SuspendLayout();
			this->tabControl1->SuspendLayout();
			this->tbMyShows->SuspendLayout();
			this->tbRenaming->SuspendLayout();
			this->tbMissing->SuspendLayout();
			this->tbFnO->SuspendLayout();
			this->tbFM->SuspendLayout();
			this->splitContainer3->Panel1->SuspendLayout();
			this->splitContainer3->Panel2->SuspendLayout();
			this->splitContainer3->SuspendLayout();
			this->tableLayoutPanel1->SuspendLayout();
			this->panel2->SuspendLayout();
			this->panel3->SuspendLayout();
			this->tbTorrentMatch->SuspendLayout();
			this->tbWTW->SuspendLayout();
			this->tbRSS->SuspendLayout();
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
			this->exportToolStripMenuItem->Size = System::Drawing::Size(155, 22);
			this->exportToolStripMenuItem->Text = L"&Export";
			this->exportToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::exportToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this->saveToolStripMenuItem->Name = L"saveToolStripMenuItem";
			this->saveToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::S));
			this->saveToolStripMenuItem->Size = System::Drawing::Size(155, 22);
			this->saveToolStripMenuItem->Text = L"&Save";
			this->saveToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::saveToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this->toolStripSeparator1->Name = L"toolStripSeparator1";
			this->toolStripSeparator1->Size = System::Drawing::Size(152, 6);
			// 
			// exitToolStripMenuItem
			// 
			this->exitToolStripMenuItem->Name = L"exitToolStripMenuItem";
			this->exitToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Alt | System::Windows::Forms::Keys::F4));
			this->exitToolStripMenuItem->Size = System::Drawing::Size(155, 22);
			this->exitToolStripMenuItem->Text = L"E&xit";
			this->exitToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::exitToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this->optionsToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(8) {this->offlineOperationToolStripMenuItem, 
				this->backgroundDownloadToolStripMenuItem, this->toolStripSeparator2, this->preferencesToolStripMenuItem, this->filenameTemplateEditorToolStripMenuItem, 
				this->searchEnginesToolStripMenuItem, this->filenameProcessorsToolStripMenuItem, this->languagePreferenceToolStripMenuItem});
			this->optionsToolStripMenuItem->Name = L"optionsToolStripMenuItem";
			this->optionsToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::O));
			this->optionsToolStripMenuItem->Size = System::Drawing::Size(56, 20);
			this->optionsToolStripMenuItem->Text = L"&Options";
			// 
			// offlineOperationToolStripMenuItem
			// 
			this->offlineOperationToolStripMenuItem->Name = L"offlineOperationToolStripMenuItem";
			this->offlineOperationToolStripMenuItem->Size = System::Drawing::Size(243, 22);
			this->offlineOperationToolStripMenuItem->Text = L"&Offline Operation";
			this->offlineOperationToolStripMenuItem->ToolTipText = L"If you turn this on, TVRename will only use data it has locally, without download" 
				L"ing anything.";
			this->offlineOperationToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::offlineOperationToolStripMenuItem_Click);
			// 
			// backgroundDownloadToolStripMenuItem
			// 
			this->backgroundDownloadToolStripMenuItem->Name = L"backgroundDownloadToolStripMenuItem";
			this->backgroundDownloadToolStripMenuItem->Size = System::Drawing::Size(243, 22);
			this->backgroundDownloadToolStripMenuItem->Text = L"Automatic &Background Download";
			this->backgroundDownloadToolStripMenuItem->ToolTipText = L"Turn this on to let TVRename automatically download thetvdb.com data in the backr" 
				L"ground";
			this->backgroundDownloadToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::backgroundDownloadToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this->toolStripSeparator2->Name = L"toolStripSeparator2";
			this->toolStripSeparator2->Size = System::Drawing::Size(240, 6);
			this->toolStripSeparator2->Visible = false;
			// 
			// preferencesToolStripMenuItem
			// 
			this->preferencesToolStripMenuItem->Name = L"preferencesToolStripMenuItem";
			this->preferencesToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::P));
			this->preferencesToolStripMenuItem->Size = System::Drawing::Size(243, 22);
			this->preferencesToolStripMenuItem->Text = L"&Preferences";
			this->preferencesToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::preferencesToolStripMenuItem_Click);
			// 
			// filenameTemplateEditorToolStripMenuItem
			// 
			this->filenameTemplateEditorToolStripMenuItem->Name = L"filenameTemplateEditorToolStripMenuItem";
			this->filenameTemplateEditorToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::T));
			this->filenameTemplateEditorToolStripMenuItem->Size = System::Drawing::Size(243, 22);
			this->filenameTemplateEditorToolStripMenuItem->Text = L"&Filename Template Editor";
			this->filenameTemplateEditorToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::filenameTemplateEditorToolStripMenuItem_Click);
			// 
			// searchEnginesToolStripMenuItem
			// 
			this->searchEnginesToolStripMenuItem->Name = L"searchEnginesToolStripMenuItem";
			this->searchEnginesToolStripMenuItem->Size = System::Drawing::Size(243, 22);
			this->searchEnginesToolStripMenuItem->Text = L"&Search Engines";
			this->searchEnginesToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::searchEnginesToolStripMenuItem_Click);
			// 
			// filenameProcessorsToolStripMenuItem
			// 
			this->filenameProcessorsToolStripMenuItem->Name = L"filenameProcessorsToolStripMenuItem";
			this->filenameProcessorsToolStripMenuItem->Size = System::Drawing::Size(243, 22);
			this->filenameProcessorsToolStripMenuItem->Text = L"File&name Processors";
			this->filenameProcessorsToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::filenameProcessorsToolStripMenuItem_Click);
			// 
			// languagePreferenceToolStripMenuItem
			// 
			this->languagePreferenceToolStripMenuItem->Name = L"languagePreferenceToolStripMenuItem";
			this->languagePreferenceToolStripMenuItem->Size = System::Drawing::Size(243, 22);
			this->languagePreferenceToolStripMenuItem->Text = L"&Language Selection";
			this->languagePreferenceToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::languagePreferenceToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this->toolsToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(4) {this->flushCacheToolStripMenuItem, 
				this->backgroundDownloadNowToolStripMenuItem, this->actorsToolStripMenuItem, this->uTorrentToolStripMenuItem});
			this->toolsToolStripMenuItem->Name = L"toolsToolStripMenuItem";
			this->toolsToolStripMenuItem->Size = System::Drawing::Size(44, 20);
			this->toolsToolStripMenuItem->Text = L"&Tools";
			// 
			// flushCacheToolStripMenuItem
			// 
			this->flushCacheToolStripMenuItem->Name = L"flushCacheToolStripMenuItem";
			this->flushCacheToolStripMenuItem->Size = System::Drawing::Size(253, 22);
			this->flushCacheToolStripMenuItem->Text = L"&Flush Cache";
			this->flushCacheToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::flushCacheToolStripMenuItem_Click);
			// 
			// backgroundDownloadNowToolStripMenuItem
			// 
			this->backgroundDownloadNowToolStripMenuItem->Name = L"backgroundDownloadNowToolStripMenuItem";
			this->backgroundDownloadNowToolStripMenuItem->ShortcutKeys = static_cast<System::Windows::Forms::Keys>((System::Windows::Forms::Keys::Control | System::Windows::Forms::Keys::B));
			this->backgroundDownloadNowToolStripMenuItem->Size = System::Drawing::Size(253, 22);
			this->backgroundDownloadNowToolStripMenuItem->Text = L"&Background Download Now";
			this->backgroundDownloadNowToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::backgroundDownloadNowToolStripMenuItem_Click);
			// 
			// actorsToolStripMenuItem
			// 
			this->actorsToolStripMenuItem->Name = L"actorsToolStripMenuItem";
			this->actorsToolStripMenuItem->Size = System::Drawing::Size(253, 22);
			this->actorsToolStripMenuItem->Text = L"&Actors";
			this->actorsToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::actorsToolStripMenuItem_Click);
			// 
			// uTorrentToolStripMenuItem
			// 
			this->uTorrentToolStripMenuItem->Name = L"uTorrentToolStripMenuItem";
			this->uTorrentToolStripMenuItem->Size = System::Drawing::Size(253, 22);
			this->uTorrentToolStripMenuItem->Text = L"&uTorrent";
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
			this->bugReportToolStripMenuItem->Size = System::Drawing::Size(164, 22);
			this->bugReportToolStripMenuItem->Text = L"Bug &Report";
			this->bugReportToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::bugReportToolStripMenuItem_Click);
			// 
			// buyMeADrinkToolStripMenuItem
			// 
			this->buyMeADrinkToolStripMenuItem->Name = L"buyMeADrinkToolStripMenuItem";
			this->buyMeADrinkToolStripMenuItem->Size = System::Drawing::Size(164, 22);
			this->buyMeADrinkToolStripMenuItem->Text = L"&Buy Me A Drink";
			this->buyMeADrinkToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::buyMeADrinkToolStripMenuItem_Click);
			// 
			// visitWebsiteToolStripMenuItem
			// 
			this->visitWebsiteToolStripMenuItem->Name = L"visitWebsiteToolStripMenuItem";
			this->visitWebsiteToolStripMenuItem->Size = System::Drawing::Size(164, 22);
			this->visitWebsiteToolStripMenuItem->Text = L"&Visit Website";
			this->visitWebsiteToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::visitWebsiteToolStripMenuItem_Click);
			// 
			// quickstartGuideToolStripMenuItem
			// 
			this->quickstartGuideToolStripMenuItem->Name = L"quickstartGuideToolStripMenuItem";
			this->quickstartGuideToolStripMenuItem->Size = System::Drawing::Size(164, 22);
			this->quickstartGuideToolStripMenuItem->Text = L"&Quickstart Guide";
			this->quickstartGuideToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::quickstartGuideToolStripMenuItem_Click);
			// 
			// statisticsToolStripMenuItem
			// 
			this->statisticsToolStripMenuItem->Name = L"statisticsToolStripMenuItem";
			this->statisticsToolStripMenuItem->Size = System::Drawing::Size(164, 22);
			this->statisticsToolStripMenuItem->Text = L"&Statistics";
			this->statisticsToolStripMenuItem->Click += gcnew System::EventHandler(this, &UI::statisticsToolStripMenuItem_Click);
			// 
			// tabControl1
			// 
			this->tabControl1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->tabControl1->Controls->Add(this->tbMyShows);
			this->tabControl1->Controls->Add(this->tbRenaming);
			this->tabControl1->Controls->Add(this->tbMissing);
			this->tabControl1->Controls->Add(this->tbFnO);
			this->tabControl1->Controls->Add(this->tbFM);
			this->tabControl1->Controls->Add(this->tbTorrentMatch);
			this->tabControl1->Controls->Add(this->tbWTW);
			this->tabControl1->Controls->Add(this->tbRSS);
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
			// tbRenaming
			// 
			this->tbRenaming->Controls->Add(this->txtRenamingCount);
			this->tbRenaming->Controls->Add(this->bnRenameCheck);
			this->tbRenaming->Controls->Add(this->bnRenameDoRenaming);
			this->tbRenaming->Controls->Add(this->lvRenameList);
			this->tbRenaming->Location = System::Drawing::Point(4, 22);
			this->tbRenaming->Name = L"tbRenaming";
			this->tbRenaming->Size = System::Drawing::Size(923, 507);
			this->tbRenaming->TabIndex = 2;
			this->tbRenaming->Text = L"Renaming";
			this->tbRenaming->UseVisualStyleBackColor = true;
			// 
			// txtRenamingCount
			// 
			this->txtRenamingCount->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->txtRenamingCount->AutoSize = true;
			this->txtRenamingCount->Location = System::Drawing::Point(89, 486);
			this->txtRenamingCount->Name = L"txtRenamingCount";
			this->txtRenamingCount->Size = System::Drawing::Size(67, 13);
			this->txtRenamingCount->TabIndex = 3;
			this->txtRenamingCount->Text = L"                    ";
			// 
			// bnRenameCheck
			// 
			this->bnRenameCheck->Location = System::Drawing::Point(8, 6);
			this->bnRenameCheck->Name = L"bnRenameCheck";
			this->bnRenameCheck->Size = System::Drawing::Size(75, 23);
			this->bnRenameCheck->TabIndex = 0;
			this->bnRenameCheck->Text = L"&Check";
			this->bnRenameCheck->UseVisualStyleBackColor = true;
			this->bnRenameCheck->Click += gcnew System::EventHandler(this, &UI::bnRenameCheck_Click);
			// 
			// bnRenameDoRenaming
			// 
			this->bnRenameDoRenaming->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnRenameDoRenaming->Location = System::Drawing::Point(8, 481);
			this->bnRenameDoRenaming->Name = L"bnRenameDoRenaming";
			this->bnRenameDoRenaming->Size = System::Drawing::Size(75, 23);
			this->bnRenameDoRenaming->TabIndex = 2;
			this->bnRenameDoRenaming->Text = L"&Rename";
			this->bnRenameDoRenaming->UseVisualStyleBackColor = true;
			this->bnRenameDoRenaming->Click += gcnew System::EventHandler(this, &UI::bnRenameDoRenaming_Click);
			// 
			// lvRenameList
			// 
			this->lvRenameList->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvRenameList->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(5) {this->columnHeader9, 
				this->columnHeader10, this->columnHeader11, this->columnHeader12, this->columnHeader37});
			this->lvRenameList->FullRowSelect = true;
			this->lvRenameList->HideSelection = false;
			this->lvRenameList->Location = System::Drawing::Point(3, 35);
			this->lvRenameList->Name = L"lvRenameList";
			this->lvRenameList->ShowItemToolTips = true;
			this->lvRenameList->Size = System::Drawing::Size(920, 440);
			this->lvRenameList->TabIndex = 1;
			this->lvRenameList->UseCompatibleStateImageBehavior = false;
			this->lvRenameList->View = System::Windows::Forms::View::Details;
			this->lvRenameList->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lvRenameList_MouseClick);
			this->lvRenameList->ColumnClick += gcnew System::Windows::Forms::ColumnClickEventHandler(this, &UI::lvRenameList_ColumnClick);
			this->lvRenameList->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &UI::lvRenameList_KeyDown);
			// 
			// columnHeader9
			// 
			this->columnHeader9->Text = L"Show";
			this->columnHeader9->Width = 187;
			// 
			// columnHeader10
			// 
			this->columnHeader10->Text = L"Folder";
			this->columnHeader10->Width = 161;
			// 
			// columnHeader11
			// 
			this->columnHeader11->Text = L"From Name";
			this->columnHeader11->Width = 206;
			// 
			// columnHeader12
			// 
			this->columnHeader12->Text = L"To Name";
			this->columnHeader12->Width = 199;
			// 
			// columnHeader37
			// 
			this->columnHeader37->Text = L"Errors";
			this->columnHeader37->Width = 132;
			// 
			// tbMissing
			// 
			this->tbMissing->Controls->Add(this->txtMissingStats);
			this->tbMissing->Controls->Add(this->bnChooseSite);
			this->tbMissing->Controls->Add(this->bnBTSearch);
			this->tbMissing->Controls->Add(this->bnDoMissingCheck);
			this->tbMissing->Controls->Add(this->lvMissingList);
			this->tbMissing->Location = System::Drawing::Point(4, 22);
			this->tbMissing->Name = L"tbMissing";
			this->tbMissing->Size = System::Drawing::Size(923, 507);
			this->tbMissing->TabIndex = 6;
			this->tbMissing->Text = L"Missing";
			this->tbMissing->UseVisualStyleBackColor = true;
			// 
			// txtMissingStats
			// 
			this->txtMissingStats->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->txtMissingStats->AutoSize = true;
			this->txtMissingStats->Location = System::Drawing::Point(107, 486);
			this->txtMissingStats->Name = L"txtMissingStats";
			this->txtMissingStats->Size = System::Drawing::Size(70, 13);
			this->txtMissingStats->TabIndex = 4;
			this->txtMissingStats->Text = L"                     ";
			// 
			// bnChooseSite
			// 
			this->bnChooseSite->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnChooseSite->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"bnChooseSite.Image")));
			this->bnChooseSite->Location = System::Drawing::Point(82, 481);
			this->bnChooseSite->Name = L"bnChooseSite";
			this->bnChooseSite->Size = System::Drawing::Size(19, 23);
			this->bnChooseSite->TabIndex = 3;
			this->bnChooseSite->UseVisualStyleBackColor = true;
			this->bnChooseSite->Click += gcnew System::EventHandler(this, &UI::bnChooseSite_Click);
			// 
			// bnBTSearch
			// 
			this->bnBTSearch->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnBTSearch->Location = System::Drawing::Point(8, 481);
			this->bnBTSearch->Name = L"bnBTSearch";
			this->bnBTSearch->Size = System::Drawing::Size(75, 23);
			this->bnBTSearch->TabIndex = 2;
			this->bnBTSearch->Text = L"BT &Search";
			this->bnBTSearch->UseVisualStyleBackColor = true;
			this->bnBTSearch->Click += gcnew System::EventHandler(this, &UI::bnBTSearch_Click);
			// 
			// bnDoMissingCheck
			// 
			this->bnDoMissingCheck->Location = System::Drawing::Point(8, 6);
			this->bnDoMissingCheck->Name = L"bnDoMissingCheck";
			this->bnDoMissingCheck->Size = System::Drawing::Size(75, 23);
			this->bnDoMissingCheck->TabIndex = 0;
			this->bnDoMissingCheck->Text = L"&Check";
			this->bnDoMissingCheck->UseVisualStyleBackColor = true;
			this->bnDoMissingCheck->Click += gcnew System::EventHandler(this, &UI::bnDoMissingCheck_Click);
			// 
			// lvMissingList
			// 
			this->lvMissingList->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvMissingList->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(6) {this->columnHeader16, 
				this->columnHeader17, this->columnHeader40, this->columnHeader18, this->columnHeader19, this->columnHeader20});
			this->lvMissingList->FullRowSelect = true;
			this->lvMissingList->HideSelection = false;
			this->lvMissingList->Location = System::Drawing::Point(3, 35);
			this->lvMissingList->Name = L"lvMissingList";
			this->lvMissingList->ShowItemToolTips = true;
			this->lvMissingList->Size = System::Drawing::Size(920, 440);
			this->lvMissingList->TabIndex = 1;
			this->lvMissingList->UseCompatibleStateImageBehavior = false;
			this->lvMissingList->View = System::Windows::Forms::View::Details;
			this->lvMissingList->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lvMissingList_MouseClick);
			this->lvMissingList->SelectedIndexChanged += gcnew System::EventHandler(this, &UI::lvMissingList_SelectedIndexChanged);
			this->lvMissingList->DoubleClick += gcnew System::EventHandler(this, &UI::lvMissingList_DoubleClick);
			this->lvMissingList->ColumnClick += gcnew System::Windows::Forms::ColumnClickEventHandler(this, &UI::lvMissingList_ColumnClick);
			this->lvMissingList->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &UI::lvMissingList_KeyDown);
			// 
			// columnHeader16
			// 
			this->columnHeader16->Text = L"Show";
			this->columnHeader16->Width = 187;
			// 
			// columnHeader17
			// 
			this->columnHeader17->Text = L"Folder";
			this->columnHeader17->Width = 189;
			// 
			// columnHeader40
			// 
			this->columnHeader40->Text = L"Season";
			// 
			// columnHeader18
			// 
			this->columnHeader18->Text = L"Episode";
			this->columnHeader18->Width = 66;
			// 
			// columnHeader19
			// 
			this->columnHeader19->Text = L"Name";
			this->columnHeader19->Width = 165;
			// 
			// columnHeader20
			// 
			this->columnHeader20->Text = L"When Aired";
			this->columnHeader20->Width = 76;
			// 
			// tbFnO
			// 
			this->tbFnO->Controls->Add(this->txtFOCopyMoveStats);
			this->tbFnO->Controls->Add(this->bnOpenSearchFolder);
			this->tbFnO->Controls->Add(this->bnRemoveSearchFolder);
			this->tbFnO->Controls->Add(this->bnAddSearchFolder);
			this->tbFnO->Controls->Add(this->lbSearchFolders);
			this->tbFnO->Controls->Add(this->label5);
			this->tbFnO->Controls->Add(this->bnDoMovingAndCopying);
			this->tbFnO->Controls->Add(this->cbLeaveOriginals);
			this->tbFnO->Controls->Add(this->bnFindMissingStuff);
			this->tbFnO->Controls->Add(this->lvCopyMoveList);
			this->tbFnO->Location = System::Drawing::Point(4, 22);
			this->tbFnO->Name = L"tbFnO";
			this->tbFnO->Size = System::Drawing::Size(923, 507);
			this->tbFnO->TabIndex = 3;
			this->tbFnO->Text = L"Finding and Organising";
			this->tbFnO->UseVisualStyleBackColor = true;
			// 
			// txtFOCopyMoveStats
			// 
			this->txtFOCopyMoveStats->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->txtFOCopyMoveStats->AutoSize = true;
			this->txtFOCopyMoveStats->Location = System::Drawing::Point(89, 486);
			this->txtFOCopyMoveStats->Name = L"txtFOCopyMoveStats";
			this->txtFOCopyMoveStats->Size = System::Drawing::Size(82, 13);
			this->txtFOCopyMoveStats->TabIndex = 9;
			this->txtFOCopyMoveStats->Text = L"                         ";
			// 
			// bnOpenSearchFolder
			// 
			this->bnOpenSearchFolder->Location = System::Drawing::Point(173, 167);
			this->bnOpenSearchFolder->Name = L"bnOpenSearchFolder";
			this->bnOpenSearchFolder->Size = System::Drawing::Size(75, 23);
			this->bnOpenSearchFolder->TabIndex = 4;
			this->bnOpenSearchFolder->Text = L"&Open";
			this->bnOpenSearchFolder->UseVisualStyleBackColor = true;
			this->bnOpenSearchFolder->Click += gcnew System::EventHandler(this, &UI::bnOpenSearchFolder_Click);
			// 
			// bnRemoveSearchFolder
			// 
			this->bnRemoveSearchFolder->Location = System::Drawing::Point(92, 167);
			this->bnRemoveSearchFolder->Name = L"bnRemoveSearchFolder";
			this->bnRemoveSearchFolder->Size = System::Drawing::Size(75, 23);
			this->bnRemoveSearchFolder->TabIndex = 3;
			this->bnRemoveSearchFolder->Text = L"&Remove";
			this->bnRemoveSearchFolder->UseVisualStyleBackColor = true;
			this->bnRemoveSearchFolder->Click += gcnew System::EventHandler(this, &UI::bnRemoveSearchFolder_Click);
			// 
			// bnAddSearchFolder
			// 
			this->bnAddSearchFolder->Location = System::Drawing::Point(8, 167);
			this->bnAddSearchFolder->Name = L"bnAddSearchFolder";
			this->bnAddSearchFolder->Size = System::Drawing::Size(75, 23);
			this->bnAddSearchFolder->TabIndex = 2;
			this->bnAddSearchFolder->Text = L"&Add";
			this->bnAddSearchFolder->UseVisualStyleBackColor = true;
			this->bnAddSearchFolder->Click += gcnew System::EventHandler(this, &UI::bnAddSearchFolder_Click);
			// 
			// lbSearchFolders
			// 
			this->lbSearchFolders->AllowDrop = true;
			this->lbSearchFolders->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lbSearchFolders->FormattingEnabled = true;
			this->lbSearchFolders->Location = System::Drawing::Point(3, 27);
			this->lbSearchFolders->Name = L"lbSearchFolders";
			this->lbSearchFolders->ScrollAlwaysVisible = true;
			this->lbSearchFolders->Size = System::Drawing::Size(920, 134);
			this->lbSearchFolders->TabIndex = 1;
			this->lbSearchFolders->DragOver += gcnew System::Windows::Forms::DragEventHandler(this, &UI::lbSearchFolders_DragOver);
			this->lbSearchFolders->DragDrop += gcnew System::Windows::Forms::DragEventHandler(this, &UI::lbSearchFolders_DragDrop);
			this->lbSearchFolders->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lbSearchFolders_MouseDown);
			this->lbSearchFolders->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &UI::lbSearchFolders_KeyDown);
			// 
			// label5
			// 
			this->label5->AutoSize = true;
			this->label5->Location = System::Drawing::Point(5, 7);
			this->label5->Name = L"label5";
			this->label5->Size = System::Drawing::Size(78, 13);
			this->label5->TabIndex = 0;
			this->label5->Text = L"&Search Folders";
			// 
			// bnDoMovingAndCopying
			// 
			this->bnDoMovingAndCopying->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnDoMovingAndCopying->Location = System::Drawing::Point(8, 481);
			this->bnDoMovingAndCopying->Name = L"bnDoMovingAndCopying";
			this->bnDoMovingAndCopying->Size = System::Drawing::Size(75, 23);
			this->bnDoMovingAndCopying->TabIndex = 8;
			this->bnDoMovingAndCopying->Text = L"&Move/Copy";
			this->bnDoMovingAndCopying->UseVisualStyleBackColor = true;
			this->bnDoMovingAndCopying->Click += gcnew System::EventHandler(this, &UI::bnDoMovingAndCopying_Click);
			// 
			// cbLeaveOriginals
			// 
			this->cbLeaveOriginals->AutoSize = true;
			this->cbLeaveOriginals->Location = System::Drawing::Point(92, 200);
			this->cbLeaveOriginals->Name = L"cbLeaveOriginals";
			this->cbLeaveOriginals->Size = System::Drawing::Size(99, 17);
			this->cbLeaveOriginals->TabIndex = 6;
			this->cbLeaveOriginals->Text = L"&Leave Originals";
			this->cbLeaveOriginals->UseVisualStyleBackColor = true;
			// 
			// bnFindMissingStuff
			// 
			this->bnFindMissingStuff->Location = System::Drawing::Point(8, 196);
			this->bnFindMissingStuff->Name = L"bnFindMissingStuff";
			this->bnFindMissingStuff->Size = System::Drawing::Size(75, 23);
			this->bnFindMissingStuff->TabIndex = 5;
			this->bnFindMissingStuff->Text = L"Fin&d";
			this->bnFindMissingStuff->UseVisualStyleBackColor = true;
			this->bnFindMissingStuff->Click += gcnew System::EventHandler(this, &UI::bnFindMissingStuff_Click);
			// 
			// lvCopyMoveList
			// 
			this->lvCopyMoveList->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvCopyMoveList->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(6) {this->columnHeader39, 
				this->columnHeader21, this->columnHeader23, this->columnHeader22, this->columnHeader24, this->columnHeader47});
			this->lvCopyMoveList->FullRowSelect = true;
			this->lvCopyMoveList->HideSelection = false;
			this->lvCopyMoveList->Location = System::Drawing::Point(3, 225);
			this->lvCopyMoveList->Name = L"lvCopyMoveList";
			this->lvCopyMoveList->ShowItemToolTips = true;
			this->lvCopyMoveList->Size = System::Drawing::Size(920, 250);
			this->lvCopyMoveList->TabIndex = 7;
			this->lvCopyMoveList->UseCompatibleStateImageBehavior = false;
			this->lvCopyMoveList->View = System::Windows::Forms::View::Details;
			this->lvCopyMoveList->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lvCopyMoveList_MouseClick);
			this->lvCopyMoveList->ColumnClick += gcnew System::Windows::Forms::ColumnClickEventHandler(this, &UI::lvCopyMoveList_ColumnClick);
			this->lvCopyMoveList->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &UI::lvCopyMoveList_KeyDown);
			// 
			// columnHeader39
			// 
			this->columnHeader39->Text = L"Operation";
			// 
			// columnHeader21
			// 
			this->columnHeader21->Text = L"From Folder";
			this->columnHeader21->Width = 187;
			// 
			// columnHeader23
			// 
			this->columnHeader23->Text = L"From Name";
			this->columnHeader23->Width = 163;
			// 
			// columnHeader22
			// 
			this->columnHeader22->Text = L"To Folder";
			this->columnHeader22->Width = 172;
			// 
			// columnHeader24
			// 
			this->columnHeader24->Text = L"To Name";
			this->columnHeader24->Width = 165;
			// 
			// columnHeader47
			// 
			this->columnHeader47->Text = L"Errors";
			this->columnHeader47->Width = 162;
			// 
			// tbFM
			// 
			this->tbFM->Controls->Add(this->splitContainer3);
			this->tbFM->Location = System::Drawing::Point(4, 22);
			this->tbFM->Name = L"tbFM";
			this->tbFM->Size = System::Drawing::Size(923, 507);
			this->tbFM->TabIndex = 8;
			this->tbFM->Text = L"Folder Monitor";
			this->tbFM->UseVisualStyleBackColor = true;
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
			this->splitContainer3->Panel2->Controls->Add(this->txtFMSpecificSeason);
			this->splitContainer3->Panel2->Controls->Add(this->rbSpecificSeason);
			this->splitContainer3->Panel2->Controls->Add(this->rbFlat);
			this->splitContainer3->Panel2->Controls->Add(this->rbFolderPerSeason);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMVisitTVcom);
			this->splitContainer3->Panel2->Controls->Add(this->pnlCF);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMFullAuto);
			this->splitContainer3->Panel2->Controls->Add(this->lvFMNewShows);
			this->splitContainer3->Panel2->Controls->Add(this->bnAddThisOne);
			this->splitContainer3->Panel2->Controls->Add(this->A);
			this->splitContainer3->Panel2->Controls->Add(this->label6);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMRemoveNewFolder);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMNewFolderOpen);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMIgnoreAllNewFolders);
			this->splitContainer3->Panel2->Controls->Add(this->bnFMIgnoreNewFolder);
			this->splitContainer3->Size = System::Drawing::Size(923, 507);
			this->splitContainer3->SplitterDistance = 152;
			this->splitContainer3->SplitterWidth = 5;
			this->splitContainer3->TabIndex = 11;
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
			this->tableLayoutPanel1->RowStyles->Add((gcnew System::Windows::Forms::RowStyle(System::Windows::Forms::SizeType::Absolute, 152)));
			this->tableLayoutPanel1->RowStyles->Add((gcnew System::Windows::Forms::RowStyle(System::Windows::Forms::SizeType::Absolute, 152)));
			this->tableLayoutPanel1->Size = System::Drawing::Size(923, 152);
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
			this->panel2->Size = System::Drawing::Size(455, 146);
			this->panel2->TabIndex = 0;
			// 
			// bnFMCheck
			// 
			this->bnFMCheck->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnFMCheck->Location = System::Drawing::Point(377, 120);
			this->bnFMCheck->Name = L"bnFMCheck";
			this->bnFMCheck->Size = System::Drawing::Size(75, 23);
			this->bnFMCheck->TabIndex = 10;
			this->bnFMCheck->Text = L"&Check";
			this->bnFMCheck->UseVisualStyleBackColor = true;
			this->bnFMCheck->Click += gcnew System::EventHandler(this, &UI::bnFMCheck_Click);
			// 
			// bnFMOpenMonFolder
			// 
			this->bnFMOpenMonFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMOpenMonFolder->Location = System::Drawing::Point(165, 120);
			this->bnFMOpenMonFolder->Name = L"bnFMOpenMonFolder";
			this->bnFMOpenMonFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMOpenMonFolder->TabIndex = 9;
			this->bnFMOpenMonFolder->Text = L"&Open";
			this->bnFMOpenMonFolder->UseVisualStyleBackColor = true;
			this->bnFMOpenMonFolder->Click += gcnew System::EventHandler(this, &UI::bnFMOpenMonFolder_Click);
			// 
			// bnFMRemoveMonFolder
			// 
			this->bnFMRemoveMonFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMRemoveMonFolder->Location = System::Drawing::Point(84, 120);
			this->bnFMRemoveMonFolder->Name = L"bnFMRemoveMonFolder";
			this->bnFMRemoveMonFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMRemoveMonFolder->TabIndex = 8;
			this->bnFMRemoveMonFolder->Text = L"&Remove";
			this->bnFMRemoveMonFolder->UseVisualStyleBackColor = true;
			this->bnFMRemoveMonFolder->Click += gcnew System::EventHandler(this, &UI::bnFMRemoveMonFolder_Click);
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
			this->lstFMMonitorFolders->Size = System::Drawing::Size(449, 98);
			this->lstFMMonitorFolders->TabIndex = 6;
			this->lstFMMonitorFolders->DragOver += gcnew System::Windows::Forms::DragEventHandler(this, &UI::lstFMMonitorFolders_DragOver);
			this->lstFMMonitorFolders->DragDrop += gcnew System::Windows::Forms::DragEventHandler(this, &UI::lstFMMonitorFolders_DragDrop);
			this->lstFMMonitorFolders->DoubleClick += gcnew System::EventHandler(this, &UI::lstFMMonitorFolders_DoubleClick);
			this->lstFMMonitorFolders->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lstFMMonitorFolders_MouseDown);
			this->lstFMMonitorFolders->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &UI::lstFMMonitorFolders_KeyDown);
			// 
			// bnFMAddMonFolder
			// 
			this->bnFMAddMonFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMAddMonFolder->Location = System::Drawing::Point(3, 120);
			this->bnFMAddMonFolder->Name = L"bnFMAddMonFolder";
			this->bnFMAddMonFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMAddMonFolder->TabIndex = 7;
			this->bnFMAddMonFolder->Text = L"&Add";
			this->bnFMAddMonFolder->UseVisualStyleBackColor = true;
			this->bnFMAddMonFolder->Click += gcnew System::EventHandler(this, &UI::bnFMAddMonFolder_Click);
			// 
			// panel3
			// 
			this->panel3->Controls->Add(this->label7);
			this->panel3->Controls->Add(this->bnFMOpenIgFolder);
			this->panel3->Controls->Add(this->bnFMAddIgFolder);
			this->panel3->Controls->Add(this->bnFMRemoveIgFolder);
			this->panel3->Controls->Add(this->lstFMIgnoreFolders);
			this->panel3->Dock = System::Windows::Forms::DockStyle::Fill;
			this->panel3->Location = System::Drawing::Point(464, 3);
			this->panel3->Name = L"panel3";
			this->panel3->Size = System::Drawing::Size(456, 146);
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
			this->bnFMOpenIgFolder->Location = System::Drawing::Point(165, 120);
			this->bnFMOpenIgFolder->Name = L"bnFMOpenIgFolder";
			this->bnFMOpenIgFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMOpenIgFolder->TabIndex = 9;
			this->bnFMOpenIgFolder->Text = L"O&pen";
			this->bnFMOpenIgFolder->UseVisualStyleBackColor = true;
			this->bnFMOpenIgFolder->Click += gcnew System::EventHandler(this, &UI::bnFMOpenIgFolder_Click);
			// 
			// bnFMAddIgFolder
			// 
			this->bnFMAddIgFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMAddIgFolder->Location = System::Drawing::Point(2, 120);
			this->bnFMAddIgFolder->Name = L"bnFMAddIgFolder";
			this->bnFMAddIgFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMAddIgFolder->TabIndex = 7;
			this->bnFMAddIgFolder->Text = L"A&dd";
			this->bnFMAddIgFolder->UseVisualStyleBackColor = true;
			this->bnFMAddIgFolder->Click += gcnew System::EventHandler(this, &UI::bnFMAddIgFolder_Click);
			// 
			// bnFMRemoveIgFolder
			// 
			this->bnFMRemoveIgFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMRemoveIgFolder->Location = System::Drawing::Point(84, 120);
			this->bnFMRemoveIgFolder->Name = L"bnFMRemoveIgFolder";
			this->bnFMRemoveIgFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMRemoveIgFolder->TabIndex = 8;
			this->bnFMRemoveIgFolder->Text = L"Remo&ve";
			this->bnFMRemoveIgFolder->UseVisualStyleBackColor = true;
			this->bnFMRemoveIgFolder->Click += gcnew System::EventHandler(this, &UI::bnFMRemoveIgFolder_Click);
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
			this->lstFMIgnoreFolders->Size = System::Drawing::Size(456, 98);
			this->lstFMIgnoreFolders->TabIndex = 6;
			this->lstFMIgnoreFolders->DragOver += gcnew System::Windows::Forms::DragEventHandler(this, &UI::lstFMIgnoreFolders_DragOver);
			this->lstFMIgnoreFolders->DragDrop += gcnew System::Windows::Forms::DragEventHandler(this, &UI::lstFMIgnoreFolders_DragDrop);
			this->lstFMIgnoreFolders->DoubleClick += gcnew System::EventHandler(this, &UI::lstFMIgnoreFolders_DoubleClick);
			this->lstFMIgnoreFolders->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lstFMIgnoreFolders_MouseDown);
			// 
			// txtFMSpecificSeason
			// 
			this->txtFMSpecificSeason->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->txtFMSpecificSeason->Location = System::Drawing::Point(533, 146);
			this->txtFMSpecificSeason->Name = L"txtFMSpecificSeason";
			this->txtFMSpecificSeason->Size = System::Drawing::Size(53, 20);
			this->txtFMSpecificSeason->TabIndex = 29;
			// 
			// rbSpecificSeason
			// 
			this->rbSpecificSeason->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbSpecificSeason->AutoSize = true;
			this->rbSpecificSeason->Location = System::Drawing::Point(433, 147);
			this->rbSpecificSeason->Name = L"rbSpecificSeason";
			this->rbSpecificSeason->Size = System::Drawing::Size(100, 17);
			this->rbSpecificSeason->TabIndex = 28;
			this->rbSpecificSeason->TabStop = true;
			this->rbSpecificSeason->Text = L"Specific season";
			this->rbSpecificSeason->UseVisualStyleBackColor = true;
			this->rbSpecificSeason->CheckedChanged += gcnew System::EventHandler(this, &UI::rbSpecificSeason_CheckedChanged);
			// 
			// rbFlat
			// 
			this->rbFlat->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbFlat->AutoSize = true;
			this->rbFlat->Location = System::Drawing::Point(433, 124);
			this->rbFlat->Name = L"rbFlat";
			this->rbFlat->Size = System::Drawing::Size(120, 17);
			this->rbFlat->TabIndex = 28;
			this->rbFlat->TabStop = true;
			this->rbFlat->Text = L"All seasons together";
			this->rbFlat->UseVisualStyleBackColor = true;
			this->rbFlat->CheckedChanged += gcnew System::EventHandler(this, &UI::rbFlat_CheckedChanged);
			// 
			// rbFolderPerSeason
			// 
			this->rbFolderPerSeason->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbFolderPerSeason->AutoSize = true;
			this->rbFolderPerSeason->Location = System::Drawing::Point(433, 101);
			this->rbFolderPerSeason->Name = L"rbFolderPerSeason";
			this->rbFolderPerSeason->Size = System::Drawing::Size(109, 17);
			this->rbFolderPerSeason->TabIndex = 28;
			this->rbFolderPerSeason->TabStop = true;
			this->rbFolderPerSeason->Text = L"Folder per season";
			this->rbFolderPerSeason->UseVisualStyleBackColor = true;
			this->rbFolderPerSeason->CheckedChanged += gcnew System::EventHandler(this, &UI::rbFolderPerSeason_CheckedChanged);
			// 
			// bnFMVisitTVcom
			// 
			this->bnFMVisitTVcom->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMVisitTVcom->Location = System::Drawing::Point(332, 294);
			this->bnFMVisitTVcom->Name = L"bnFMVisitTVcom";
			this->bnFMVisitTVcom->Size = System::Drawing::Size(75, 23);
			this->bnFMVisitTVcom->TabIndex = 26;
			this->bnFMVisitTVcom->Text = L"&Visit TVDB";
			this->bnFMVisitTVcom->UseVisualStyleBackColor = true;
			this->bnFMVisitTVcom->Click += gcnew System::EventHandler(this, &UI::bnFMVisitTVcom_Click);
			// 
			// pnlCF
			// 
			this->pnlCF->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->pnlCF->Location = System::Drawing::Point(5, 101);
			this->pnlCF->Name = L"pnlCF";
			this->pnlCF->Size = System::Drawing::Size(407, 185);
			this->pnlCF->TabIndex = 25;
			// 
			// bnFMFullAuto
			// 
			this->bnFMFullAuto->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMFullAuto->Location = System::Drawing::Point(8, 324);
			this->bnFMFullAuto->Name = L"bnFMFullAuto";
			this->bnFMFullAuto->Size = System::Drawing::Size(75, 23);
			this->bnFMFullAuto->TabIndex = 24;
			this->bnFMFullAuto->Text = L"F&ull Auto";
			this->bnFMFullAuto->UseVisualStyleBackColor = true;
			this->bnFMFullAuto->Click += gcnew System::EventHandler(this, &UI::bnFMFullAuto_Click);
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
			this->lvFMNewShows->Size = System::Drawing::Size(917, 72);
			this->lvFMNewShows->TabIndex = 11;
			this->lvFMNewShows->UseCompatibleStateImageBehavior = false;
			this->lvFMNewShows->View = System::Windows::Forms::View::Details;
			this->lvFMNewShows->SelectedIndexChanged += gcnew System::EventHandler(this, &UI::lvFMNewShows_SelectedIndexChanged);
			this->lvFMNewShows->DoubleClick += gcnew System::EventHandler(this, &UI::lvFMNewShows_DoubleClick);
			this->lvFMNewShows->DragDrop += gcnew System::Windows::Forms::DragEventHandler(this, &UI::lvFMNewShows_DragDrop);
			this->lvFMNewShows->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &UI::lvFMNewShows_MouseDown);
			this->lvFMNewShows->KeyDown += gcnew System::Windows::Forms::KeyEventHandler(this, &UI::lvFMNewShows_KeyDown);
			this->lvFMNewShows->DragOver += gcnew System::Windows::Forms::DragEventHandler(this, &UI::lvFMNewShows_DragOver);
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
			this->bnAddThisOne->Location = System::Drawing::Point(433, 294);
			this->bnAddThisOne->Name = L"bnAddThisOne";
			this->bnAddThisOne->Size = System::Drawing::Size(75, 23);
			this->bnAddThisOne->TabIndex = 10;
			this->bnAddThisOne->Text = L"Add &This";
			this->bnAddThisOne->UseVisualStyleBackColor = true;
			this->bnAddThisOne->Click += gcnew System::EventHandler(this, &UI::bnAddThisOne_Click);
			// 
			// A
			// 
			this->A->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->A->Location = System::Drawing::Point(433, 324);
			this->A->Name = L"A";
			this->A->Size = System::Drawing::Size(75, 23);
			this->A->TabIndex = 10;
			this->A->Text = L"Do&ne";
			this->A->UseVisualStyleBackColor = true;
			this->A->Click += gcnew System::EventHandler(this, &UI::bnFolderMonitorDone_Click);
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
			this->bnFMRemoveNewFolder->Location = System::Drawing::Point(89, 325);
			this->bnFMRemoveNewFolder->Name = L"bnFMRemoveNewFolder";
			this->bnFMRemoveNewFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMRemoveNewFolder->TabIndex = 9;
			this->bnFMRemoveNewFolder->Text = L"Re&move";
			this->bnFMRemoveNewFolder->UseVisualStyleBackColor = true;
			this->bnFMRemoveNewFolder->Click += gcnew System::EventHandler(this, &UI::bnFMRemoveNewFolder_Click);
			// 
			// bnFMNewFolderOpen
			// 
			this->bnFMNewFolderOpen->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMNewFolderOpen->Location = System::Drawing::Point(332, 325);
			this->bnFMNewFolderOpen->Name = L"bnFMNewFolderOpen";
			this->bnFMNewFolderOpen->Size = System::Drawing::Size(75, 23);
			this->bnFMNewFolderOpen->TabIndex = 9;
			this->bnFMNewFolderOpen->Text = L"Op&en";
			this->bnFMNewFolderOpen->UseVisualStyleBackColor = true;
			this->bnFMNewFolderOpen->Click += gcnew System::EventHandler(this, &UI::bnFMNewFolderOpen_Click);
			// 
			// bnFMIgnoreAllNewFolders
			// 
			this->bnFMIgnoreAllNewFolders->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMIgnoreAllNewFolders->Location = System::Drawing::Point(251, 325);
			this->bnFMIgnoreAllNewFolders->Name = L"bnFMIgnoreAllNewFolders";
			this->bnFMIgnoreAllNewFolders->Size = System::Drawing::Size(75, 23);
			this->bnFMIgnoreAllNewFolders->TabIndex = 9;
			this->bnFMIgnoreAllNewFolders->Text = L"Ig&nore All";
			this->bnFMIgnoreAllNewFolders->UseVisualStyleBackColor = true;
			this->bnFMIgnoreAllNewFolders->Click += gcnew System::EventHandler(this, &UI::bnFMIgnoreAllNewFolders_Click);
			// 
			// bnFMIgnoreNewFolder
			// 
			this->bnFMIgnoreNewFolder->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnFMIgnoreNewFolder->Location = System::Drawing::Point(170, 325);
			this->bnFMIgnoreNewFolder->Name = L"bnFMIgnoreNewFolder";
			this->bnFMIgnoreNewFolder->Size = System::Drawing::Size(75, 23);
			this->bnFMIgnoreNewFolder->TabIndex = 9;
			this->bnFMIgnoreNewFolder->Text = L"&Ignore";
			this->bnFMIgnoreNewFolder->UseVisualStyleBackColor = true;
			this->bnFMIgnoreNewFolder->Click += gcnew System::EventHandler(this, &UI::bnFMIgnoreNewFolder_Click);
			// 
			// tbTorrentMatch
			// 
			this->tbTorrentMatch->Controls->Add(this->rbBTRenameFiles);
			this->tbTorrentMatch->Controls->Add(this->rbBTCopyTo);
			this->tbTorrentMatch->Controls->Add(this->bnBTSecondOpen);
			this->tbTorrentMatch->Controls->Add(this->bnBTOpenFolder);
			this->tbTorrentMatch->Controls->Add(this->tmatchTree);
			this->tbTorrentMatch->Controls->Add(this->bnGo);
			this->tbTorrentMatch->Controls->Add(this->bnBTSecondBrowse);
			this->tbTorrentMatch->Controls->Add(this->bnBrowseFolder);
			this->tbTorrentMatch->Controls->Add(this->txtBTSecondLocation);
			this->tbTorrentMatch->Controls->Add(this->txtFolder);
			this->tbTorrentMatch->Controls->Add(this->bnBrowseTorrent);
			this->tbTorrentMatch->Controls->Add(this->label14);
			this->tbTorrentMatch->Controls->Add(this->label3);
			this->tbTorrentMatch->Controls->Add(this->txtTorrentFile);
			this->tbTorrentMatch->Controls->Add(this->label4);
			this->tbTorrentMatch->Location = System::Drawing::Point(4, 22);
			this->tbTorrentMatch->Name = L"tbTorrentMatch";
			this->tbTorrentMatch->Size = System::Drawing::Size(923, 507);
			this->tbTorrentMatch->TabIndex = 7;
			this->tbTorrentMatch->Text = L"Torrent Match";
			this->tbTorrentMatch->UseVisualStyleBackColor = true;
			// 
			// rbBTRenameFiles
			// 
			this->rbBTRenameFiles->AutoSize = true;
			this->rbBTRenameFiles->Checked = true;
			this->rbBTRenameFiles->Location = System::Drawing::Point(102, 65);
			this->rbBTRenameFiles->Name = L"rbBTRenameFiles";
			this->rbBTRenameFiles->Size = System::Drawing::Size(86, 17);
			this->rbBTRenameFiles->TabIndex = 10;
			this->rbBTRenameFiles->TabStop = true;
			this->rbBTRenameFiles->Text = L"Rename files";
			this->rbBTRenameFiles->UseVisualStyleBackColor = true;
			this->rbBTRenameFiles->CheckedChanged += gcnew System::EventHandler(this, &UI::rbBTRenameFiles_CheckedChanged);
			// 
			// rbBTCopyTo
			// 
			this->rbBTCopyTo->AutoSize = true;
			this->rbBTCopyTo->Location = System::Drawing::Point(194, 65);
			this->rbBTCopyTo->Name = L"rbBTCopyTo";
			this->rbBTCopyTo->Size = System::Drawing::Size(65, 17);
			this->rbBTCopyTo->TabIndex = 10;
			this->rbBTCopyTo->Text = L"Copy To";
			this->rbBTCopyTo->UseVisualStyleBackColor = true;
			this->rbBTCopyTo->CheckedChanged += gcnew System::EventHandler(this, &UI::rbBTCopyTo_CheckedChanged);
			// 
			// bnBTSecondOpen
			// 
			this->bnBTSecondOpen->Location = System::Drawing::Point(491, 89);
			this->bnBTSecondOpen->Name = L"bnBTSecondOpen";
			this->bnBTSecondOpen->Size = System::Drawing::Size(75, 23);
			this->bnBTSecondOpen->TabIndex = 6;
			this->bnBTSecondOpen->Text = L"&Open";
			this->bnBTSecondOpen->UseVisualStyleBackColor = true;
			this->bnBTSecondOpen->Click += gcnew System::EventHandler(this, &UI::bnBTCopyToOpen_Click);
			// 
			// bnBTOpenFolder
			// 
			this->bnBTOpenFolder->Location = System::Drawing::Point(491, 37);
			this->bnBTOpenFolder->Name = L"bnBTOpenFolder";
			this->bnBTOpenFolder->Size = System::Drawing::Size(75, 23);
			this->bnBTOpenFolder->TabIndex = 6;
			this->bnBTOpenFolder->Text = L"&Open";
			this->bnBTOpenFolder->UseVisualStyleBackColor = true;
			this->bnBTOpenFolder->Click += gcnew System::EventHandler(this, &UI::bnBTOpenFolder_Click);
			// 
			// tmatchTree
			// 
			this->tmatchTree->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->tmatchTree->Location = System::Drawing::Point(3, 146);
			this->tmatchTree->Name = L"tmatchTree";
			this->tmatchTree->Size = System::Drawing::Size(920, 359);
			this->tmatchTree->TabIndex = 8;
			// 
			// bnGo
			// 
			this->bnGo->Location = System::Drawing::Point(102, 117);
			this->bnGo->Name = L"bnGo";
			this->bnGo->Size = System::Drawing::Size(75, 23);
			this->bnGo->TabIndex = 7;
			this->bnGo->Text = L"&Go";
			this->bnGo->UseVisualStyleBackColor = true;
			this->bnGo->Click += gcnew System::EventHandler(this, &UI::bnGo_Click);
			// 
			// bnBTSecondBrowse
			// 
			this->bnBTSecondBrowse->Location = System::Drawing::Point(410, 89);
			this->bnBTSecondBrowse->Name = L"bnBTSecondBrowse";
			this->bnBTSecondBrowse->Size = System::Drawing::Size(75, 23);
			this->bnBTSecondBrowse->TabIndex = 5;
			this->bnBTSecondBrowse->Text = L"B&rowse...";
			this->bnBTSecondBrowse->UseVisualStyleBackColor = true;
			this->bnBTSecondBrowse->Click += gcnew System::EventHandler(this, &UI::bnBTCopyToBrowse_Click);
			// 
			// bnBrowseFolder
			// 
			this->bnBrowseFolder->Location = System::Drawing::Point(410, 37);
			this->bnBrowseFolder->Name = L"bnBrowseFolder";
			this->bnBrowseFolder->Size = System::Drawing::Size(75, 23);
			this->bnBrowseFolder->TabIndex = 5;
			this->bnBrowseFolder->Text = L"B&rowse...";
			this->bnBrowseFolder->UseVisualStyleBackColor = true;
			this->bnBrowseFolder->Click += gcnew System::EventHandler(this, &UI::bnBrowseFolder_Click);
			// 
			// txtBTSecondLocation
			// 
			this->txtBTSecondLocation->Location = System::Drawing::Point(102, 91);
			this->txtBTSecondLocation->Name = L"txtBTSecondLocation";
			this->txtBTSecondLocation->Size = System::Drawing::Size(296, 20);
			this->txtBTSecondLocation->TabIndex = 4;
			// 
			// txtFolder
			// 
			this->txtFolder->Location = System::Drawing::Point(102, 39);
			this->txtFolder->Name = L"txtFolder";
			this->txtFolder->Size = System::Drawing::Size(296, 20);
			this->txtFolder->TabIndex = 4;
			// 
			// bnBrowseTorrent
			// 
			this->bnBrowseTorrent->Location = System::Drawing::Point(410, 8);
			this->bnBrowseTorrent->Name = L"bnBrowseTorrent";
			this->bnBrowseTorrent->Size = System::Drawing::Size(75, 23);
			this->bnBrowseTorrent->TabIndex = 2;
			this->bnBrowseTorrent->Text = L"&Browse...";
			this->bnBrowseTorrent->UseVisualStyleBackColor = true;
			this->bnBrowseTorrent->Click += gcnew System::EventHandler(this, &UI::bnBrowseTorrent_Click);
			// 
			// label14
			// 
			this->label14->AutoSize = true;
			this->label14->Location = System::Drawing::Point(52, 67);
			this->label14->Name = L"label14";
			this->label14->Size = System::Drawing::Size(40, 13);
			this->label14->TabIndex = 3;
			this->label14->Text = L"Action:";
			this->label14->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Location = System::Drawing::Point(52, 42);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(39, 13);
			this->label3->TabIndex = 3;
			this->label3->Text = L"&Folder:";
			this->label3->TextAlign = System::Drawing::ContentAlignment::TopRight;
			// 
			// txtTorrentFile
			// 
			this->txtTorrentFile->Location = System::Drawing::Point(102, 10);
			this->txtTorrentFile->Name = L"txtTorrentFile";
			this->txtTorrentFile->Size = System::Drawing::Size(296, 20);
			this->txtTorrentFile->TabIndex = 1;
			// 
			// label4
			// 
			this->label4->AutoSize = true;
			this->label4->Location = System::Drawing::Point(32, 13);
			this->label4->Name = L"label4";
			this->label4->Size = System::Drawing::Size(59, 13);
			this->label4->TabIndex = 0;
			this->label4->Text = L".&torrent file:";
			this->label4->TextAlign = System::Drawing::ContentAlignment::TopRight;
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
			this->bnWTWChooseSite->Location = System::Drawing::Point(163, 9);
			this->bnWTWChooseSite->Name = L"bnWTWChooseSite";
			this->bnWTWChooseSite->Size = System::Drawing::Size(19, 23);
			this->bnWTWChooseSite->TabIndex = 2;
			this->bnWTWChooseSite->UseVisualStyleBackColor = true;
			this->bnWTWChooseSite->Click += gcnew System::EventHandler(this, &UI::bnWTWChooseSite_Click);
			// 
			// bnWTWBTSearch
			// 
			this->bnWTWBTSearch->Location = System::Drawing::Point(89, 9);
			this->bnWTWBTSearch->Name = L"bnWTWBTSearch";
			this->bnWTWBTSearch->Size = System::Drawing::Size(75, 23);
			this->bnWTWBTSearch->TabIndex = 1;
			this->bnWTWBTSearch->Text = L"BT &Search";
			this->bnWTWBTSearch->UseVisualStyleBackColor = true;
			this->bnWTWBTSearch->Click += gcnew System::EventHandler(this, &UI::bnWTWBTSearch_Click);
			// 
			// bnWhenToWatchCheck
			// 
			this->bnWhenToWatchCheck->Location = System::Drawing::Point(8, 9);
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
			this->txtWhenToWatchSynopsis->Location = System::Drawing::Point(3, 343);
			this->txtWhenToWatchSynopsis->Multiline = true;
			this->txtWhenToWatchSynopsis->Name = L"txtWhenToWatchSynopsis";
			this->txtWhenToWatchSynopsis->ReadOnly = true;
			this->txtWhenToWatchSynopsis->ScrollBars = System::Windows::Forms::ScrollBars::Vertical;
			this->txtWhenToWatchSynopsis->Size = System::Drawing::Size(681, 163);
			this->txtWhenToWatchSynopsis->TabIndex = 4;
			// 
			// calCalendar
			// 
			this->calCalendar->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->calCalendar->FirstDayOfWeek = System::Windows::Forms::Day::Sunday;
			this->calCalendar->Location = System::Drawing::Point(696, 343);
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
			listViewGroup5->Header = L"Recently Aired";
			listViewGroup5->Name = L"justPassed";
			listViewGroup6->Header = L"Next 7 Days";
			listViewGroup6->Name = L"next7days";
			listViewGroup6->Tag = L"1";
			listViewGroup7->Header = L"Later";
			listViewGroup7->Name = L"later";
			listViewGroup7->Tag = L"2";
			listViewGroup8->Header = L"Future Episodes";
			listViewGroup8->Name = L"futureEps";
			this->lvWhenToWatch->Groups->AddRange(gcnew cli::array< System::Windows::Forms::ListViewGroup^  >(4) {listViewGroup5, listViewGroup6, 
				listViewGroup7, listViewGroup8});
			this->lvWhenToWatch->HideSelection = false;
			this->lvWhenToWatch->Location = System::Drawing::Point(3, 41);
			this->lvWhenToWatch->Name = L"lvWhenToWatch";
			this->lvWhenToWatch->ShowItemToolTips = true;
			this->lvWhenToWatch->Size = System::Drawing::Size(920, 296);
			this->lvWhenToWatch->SmallImageList = this->ilWTWIcons;
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
			// ilWTWIcons
			// 
			this->ilWTWIcons->ImageStream = (cli::safe_cast<System::Windows::Forms::ImageListStreamer^  >(resources->GetObject(L"ilWTWIcons.ImageStream")));
			this->ilWTWIcons->TransparentColor = System::Drawing::Color::Transparent;
			this->ilWTWIcons->Images->SetKeyName(0, L"OnDisk.bmp");
			this->ilWTWIcons->Images->SetKeyName(1, L"MagGlass.bmp");
			// 
			// tbRSS
			// 
			this->tbRSS->Controls->Add(this->bnRSSBrowseuTorrent);
			this->tbRSS->Controls->Add(this->bnRSSDownload);
			this->tbRSS->Controls->Add(this->bnRSSGo);
			this->tbRSS->Controls->Add(this->lvRSSMatchingMissing);
			this->tbRSS->Controls->Add(this->lvRSSItems);
			this->tbRSS->Controls->Add(this->txtRSSuTorrentPath);
			this->tbRSS->Controls->Add(this->label9);
			this->tbRSS->Controls->Add(this->txtRSSURL);
			this->tbRSS->Controls->Add(this->label10);
			this->tbRSS->Controls->Add(this->label8);
			this->tbRSS->Controls->Add(this->label1);
			this->tbRSS->Location = System::Drawing::Point(4, 22);
			this->tbRSS->Name = L"tbRSS";
			this->tbRSS->Padding = System::Windows::Forms::Padding(3);
			this->tbRSS->Size = System::Drawing::Size(923, 507);
			this->tbRSS->TabIndex = 10;
			this->tbRSS->Text = L"RSS";
			this->tbRSS->UseVisualStyleBackColor = true;
			// 
			// bnRSSBrowseuTorrent
			// 
			this->bnRSSBrowseuTorrent->Location = System::Drawing::Point(412, 39);
			this->bnRSSBrowseuTorrent->Name = L"bnRSSBrowseuTorrent";
			this->bnRSSBrowseuTorrent->Size = System::Drawing::Size(75, 23);
			this->bnRSSBrowseuTorrent->TabIndex = 6;
			this->bnRSSBrowseuTorrent->Text = L"&Browse...";
			this->bnRSSBrowseuTorrent->UseVisualStyleBackColor = true;
			// 
			// bnRSSDownload
			// 
			this->bnRSSDownload->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->bnRSSDownload->Location = System::Drawing::Point(11, 473);
			this->bnRSSDownload->Name = L"bnRSSDownload";
			this->bnRSSDownload->Size = System::Drawing::Size(75, 23);
			this->bnRSSDownload->TabIndex = 5;
			this->bnRSSDownload->Text = L"Download";
			this->bnRSSDownload->UseVisualStyleBackColor = true;
			this->bnRSSDownload->Click += gcnew System::EventHandler(this, &UI::bnRSSDownload_Click);
			// 
			// bnRSSGo
			// 
			this->bnRSSGo->Location = System::Drawing::Point(412, 10);
			this->bnRSSGo->Name = L"bnRSSGo";
			this->bnRSSGo->Size = System::Drawing::Size(75, 23);
			this->bnRSSGo->TabIndex = 3;
			this->bnRSSGo->Text = L"Go";
			this->bnRSSGo->UseVisualStyleBackColor = true;
			this->bnRSSGo->Click += gcnew System::EventHandler(this, &UI::bnRSSGo_Click);
			// 
			// lvRSSMatchingMissing
			// 
			this->lvRSSMatchingMissing->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvRSSMatchingMissing->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(6) {this->columnHeader13, 
				this->columnHeader14, this->columnHeader15, this->columnHeader38, this->columnHeader41, this->columnHeader46});
			this->lvRSSMatchingMissing->FullRowSelect = true;
			this->lvRSSMatchingMissing->Location = System::Drawing::Point(11, 255);
			this->lvRSSMatchingMissing->Name = L"lvRSSMatchingMissing";
			this->lvRSSMatchingMissing->Size = System::Drawing::Size(901, 212);
			this->lvRSSMatchingMissing->TabIndex = 2;
			this->lvRSSMatchingMissing->UseCompatibleStateImageBehavior = false;
			this->lvRSSMatchingMissing->View = System::Windows::Forms::View::Details;
			// 
			// columnHeader13
			// 
			this->columnHeader13->Text = L"Title";
			this->columnHeader13->Width = 252;
			// 
			// columnHeader14
			// 
			this->columnHeader14->Text = L"Season";
			this->columnHeader14->Width = 51;
			// 
			// columnHeader15
			// 
			this->columnHeader15->Text = L"Episode";
			this->columnHeader15->Width = 61;
			// 
			// columnHeader38
			// 
			this->columnHeader38->Text = L"Show";
			this->columnHeader38->Width = 118;
			// 
			// columnHeader41
			// 
			this->columnHeader41->Text = L"Folder";
			this->columnHeader41->Width = 172;
			// 
			// columnHeader46
			// 
			this->columnHeader46->Text = L"Filename";
			this->columnHeader46->Width = 222;
			// 
			// lvRSSItems
			// 
			this->lvRSSItems->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->lvRSSItems->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(4) {this->columnHeader1, this->columnHeader2, 
				this->columnHeader3, this->columnHeader4});
			this->lvRSSItems->FullRowSelect = true;
			this->lvRSSItems->Location = System::Drawing::Point(11, 96);
			this->lvRSSItems->MultiSelect = false;
			this->lvRSSItems->Name = L"lvRSSItems";
			this->lvRSSItems->Size = System::Drawing::Size(901, 140);
			this->lvRSSItems->TabIndex = 2;
			this->lvRSSItems->UseCompatibleStateImageBehavior = false;
			this->lvRSSItems->View = System::Windows::Forms::View::Details;
			// 
			// columnHeader1
			// 
			this->columnHeader1->Text = L"Title";
			this->columnHeader1->Width = 252;
			// 
			// columnHeader2
			// 
			this->columnHeader2->Text = L"Season";
			this->columnHeader2->Width = 51;
			// 
			// columnHeader3
			// 
			this->columnHeader3->Text = L"Episode";
			this->columnHeader3->Width = 61;
			// 
			// columnHeader4
			// 
			this->columnHeader4->Text = L"Show Name";
			this->columnHeader4->Width = 160;
			// 
			// txtRSSuTorrentPath
			// 
			this->txtRSSuTorrentPath->Location = System::Drawing::Point(83, 41);
			this->txtRSSuTorrentPath->Name = L"txtRSSuTorrentPath";
			this->txtRSSuTorrentPath->Size = System::Drawing::Size(323, 20);
			this->txtRSSuTorrentPath->TabIndex = 1;
			this->txtRSSuTorrentPath->Text = L"c:\\Program Files\\uTorrent\\uTorrent.exe";
			// 
			// label9
			// 
			this->label9->AutoSize = true;
			this->label9->Location = System::Drawing::Point(8, 239);
			this->label9->Name = L"label9";
			this->label9->Size = System::Drawing::Size(118, 13);
			this->label9->TabIndex = 0;
			this->label9->Text = L"Matching missing items:";
			// 
			// txtRSSURL
			// 
			this->txtRSSURL->Location = System::Drawing::Point(83, 12);
			this->txtRSSURL->Name = L"txtRSSURL";
			this->txtRSSURL->Size = System::Drawing::Size(323, 20);
			this->txtRSSURL->TabIndex = 1;
			this->txtRSSURL->Text = L"http://tvrss.net/feed/eztv/";
			// 
			// label10
			// 
			this->label10->AutoSize = true;
			this->label10->Location = System::Drawing::Point(8, 44);
			this->label10->Name = L"label10";
			this->label10->Size = System::Drawing::Size(75, 13);
			this->label10->TabIndex = 0;
			this->label10->Text = L"µTorrent Path:";
			// 
			// label8
			// 
			this->label8->AutoSize = true;
			this->label8->Location = System::Drawing::Point(8, 80);
			this->label8->Name = L"label8";
			this->label8->Size = System::Drawing::Size(60, 13);
			this->label8->TabIndex = 0;
			this->label8->Text = L"RSS Items:";
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(8, 15);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(69, 13);
			this->label1->TabIndex = 0;
			this->label1->Text = L"Source URL:";
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
			this->tbRenaming->ResumeLayout(false);
			this->tbRenaming->PerformLayout();
			this->tbMissing->ResumeLayout(false);
			this->tbMissing->PerformLayout();
			this->tbFnO->ResumeLayout(false);
			this->tbFnO->PerformLayout();
			this->tbFM->ResumeLayout(false);
			this->splitContainer3->Panel1->ResumeLayout(false);
			this->splitContainer3->Panel2->ResumeLayout(false);
			this->splitContainer3->Panel2->PerformLayout();
			this->splitContainer3->ResumeLayout(false);
			this->tableLayoutPanel1->ResumeLayout(false);
			this->panel2->ResumeLayout(false);
			this->panel2->PerformLayout();
			this->panel3->ResumeLayout(false);
			this->panel3->PerformLayout();
			this->tbTorrentMatch->ResumeLayout(false);
			this->tbTorrentMatch->PerformLayout();
			this->tbWTW->ResumeLayout(false);
			this->tbWTW->PerformLayout();
			this->tbRSS->ResumeLayout(false);
			this->tbRSS->PerformLayout();
			this->tableLayoutPanel2->ResumeLayout(false);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

		int BGDLLongInterval()
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

	private: bool IsDebug()
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
					 DirectoryInfo di(System::Windows::Forms::Application::UserAppDataPath+"\\..\\..\\..\\uTorrent");
				 }
				 else
				 {
					 // hide developmental things in release build
					 actorsToolStripMenuItem->Visible = false;
					 uTorrentToolStripMenuItem->Visible = false;
					 tabControl1->Controls->Remove(tbRSS);
				 }
			 }

	public:
		UI(array<System::String ^> ^args)
		{            
			mDoc = gcnew TVDoc(args);

			ExperimentalFeatures = mDoc->HasArg("/experimental");

			Busy = 0;
			mLastEpClicked = nullptr;
			mLastFolderClicked = nullptr;
			mLastSeasonClicked = nullptr;
			mLastShowClicked = nullptr;

			mInternalChange = 0;
			mFoldersToOpen = gcnew StringList();

			InitializeComponent();
			SetProgress = gcnew SetProgressDelegate(this, &UI::SetProgressActual);

			lvWhenToWatch->ListViewItemSorter = gcnew DateSorterWTW(3);

			if (mDoc->HasArg("/hide"))
			{
				this->WindowState = FormWindowState::Minimized;
				this->Visible = false;
				this->Hide();
			}

			HideStuff();

			this->Text = this->Text + " " + DisplayVersionString();

			mTCCF = gcnew TheTVDBCodeFinder("", mDoc->GetTVDB(false,""));
			mTCCF->Dock = DockStyle::Fill;
			mTCCF->SelectionChanged += gcnew System::EventHandler(this, &UI::lvMatches_ItemSelectionChanged);

			pnlCF->SuspendLayout();
			pnlCF->Controls->Add(mTCCF);
			pnlCF->ResumeLayout();

			FillShowLists();
			UpdateSearchButton();
			FillSearchFolderList();
			SetGuideHTMLbody("");
			mDoc->DoWhenToWatch(true, false);
			FillWhenToWatchList();
			mDoc->WriteUpcomingRSS();
			FillFolderStringLists();
			ShowHideNotificationIcon();

			tabControl1->SelectedIndex = mDoc->Settings->StartupTab;
			tabControl1_SelectedIndexChanged(nullptr,nullptr);

			TabBTEnableDisable();

			
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
			array<String ^> ^args = mDoc->GetArgs();
			for (int i=0;i<args->Length;i++)
			{
				String ^arg = args[i]->ToLower();
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
			}

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
			bnBTSearch->Text = name;
			bnWTWBTSearch->Text = name;
			//            bnEpGuideChooseSearch->Text = name;
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

			LoadLayoutXML();
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
			if (name == "Renaming") return lvRenameList;
			if (name == "Missing") return lvMissingList;
			if (name == "CopyMove") return lvCopyMoveList;
			if (name == "WhenToWatch") return lvWhenToWatch;
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
				return false;


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

			WriteColWidthsXML("Renaming", writer);
			WriteColWidthsXML("Missing", writer);
			WriteColWidthsXML("CopyMove", writer);
			WriteColWidthsXML("WhenToWatch", writer);

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
			if (n == 0)
				sm->Show(bnChooseSite,Point(0,0));
			else if (n == 1)
				sm->Show(bnWTWChooseSite,Point(0,0));
			//            else if (n == 2)
			//                sm->Show(bnEpGuideChooseSearch,Point(0,0));
		}

		System::Void bnWTWChooseSite_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			ChooseSiteMenu(1);
		}

		System::Void bnEpGuideChooseSearch_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			ChooseSiteMenu(2);
		}


		System::Void bnChooseSite_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			ChooseSiteMenu(0);
		}


		System::Void bnBTSearch_Click(System::Object^  sender, System::EventArgs^  e) 
		{
			for each (ListViewItem ^lvi in lvMissingList->SelectedItems)
				mDoc->DoBTSearch(safe_cast<MissingEpisode ^>(lvi->Tag));
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

		String ^QuickStartGuide()
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

		Season ^TreeNodeToSeason(TreeNode ^n)
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
				if ((ser->GetItem("banner") != "") && (db->BannerMirror != ""))
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
					String ^epl = ei->EpNum.ToString();
					if (ei->EpNum != ei->EpNum2)
						epl += "-" + ei->EpNum2.ToString();

					// http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
					String ^episodeURL = "http://www.thetvdb.com/?tab=episode&seriesid="+ei->SeriesID+"&seasonid="+ei->SeasonID+"&id="+ei->EpisodeID.ToString();

					body += "<A href=\""+episodeURL+"\" name=\"ep"+epl+"\">"; // anchor
					body += "<b>" + CustomName::NameFor(ei, CustomName::OldNStyle(6)) + "</b>";
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
						if (ei->GetItem("filename") != "")
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
				if ((ser->GetItem("banner") != "") && (db->BannerMirror != ""))
					body += "<img width=758 height=140 src=\"" + db->BannerMirror + "/banners/" + ser->GetItem("banner") +"\"><br/>";

				body += "<h1><A HREF=\""+db->WebsiteURL(si->TVDBCode, -1, true)+"\">" + si->ShowName() + "</A> " + "</h1>";

				body += "<h2>Overview</h2>" + ser->GetItem("Overview");

				String ^actors = ser->GetItem("Actors");
				if (actors != "")
				{
					bool first = true;
					for each (String ^aa in actors->Split('|'))
					{
						if (aa != "")
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
				if ((airsTime != "") && (airsDay != ""))
				{
					body += "<h2>Airs</h2> " + airsTime + " " + airsDay;
					String ^net = ser->GetItem("Network");
					if (net != "")
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

			String ^EpGuideURLBase()
			{
				return "file://"+EpGuidePath();
			}

			void SetGuideHTMLbody(String ^body)
			{
				System::Drawing::Color col = System::Drawing::Color::FromName("ButtonFace");
				String ^bgcolor = col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2");

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
			System::Void bnEpGuideVistTVCom_Click(System::Object^  sender, System::EventArgs^  e) 
			{
			int n = lstEpGuideShows->SelectedIndex;
			if (n == -1)
			return;

			ShowItem ^si = mDoc->GetShowItems()[n];
			TVDBFor(si);
			}
			*/
			System::Void bnRenameCheck_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				RenamingCheckSpecific(nullptr);
			}
			void RenamingCheckSpecific(ShowItem ^si)
			{
				MoreBusy();
				mDoc->DoRenameCheck(this->SetProgress, si);
				FillShowLists();
				FillRenameList();
				LessBusy();
				tabControl1->SelectedIndex = 1;
			}
			System::Void bnRenameDoRenaming_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				MoreBusy();
				mDoc->DoRenaming();
				FillRenameList();
				LessBusy();
			}
			void MissingCheckSpecific(ShowItem ^si)
			{
				MoreBusy();
				mDoc->DoMissingCheck(this->SetProgress, si);
				FillShowLists();
				FillMissingList();
				LessBusy();
				if (si != nullptr)
					tabControl1->SelectedIndex = 2;
			}
			System::Void bnDoMissingCheck_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				MissingCheckSpecific(nullptr);
			}
			void FillRenameList()
			{
				lvRenameList->Items->Clear();

				for each (RCItem ^ri in mDoc->RenameList)
					AddRCItem(ri, lvRenameList, true);

				int n = mDoc->RenameList->Count;
				if (n == 0)
					txtRenamingCount->Text = "";
				else
					txtRenamingCount->Text = n.ToString() + " item"+ ( n != 1 ? "s":"") + ".";
			}
			void FillMissingList()
			{
				lvMissingList->Items->Clear();
				for each (MissingEpisode ^mi in mDoc->MissingEpisodes)
					AddMissingEpisode(mi, lvMissingList);
				int n = mDoc->MissingEpisodes->Count;
				if (n == 0)
					txtMissingStats->Text = "";
				else
					txtMissingStats->Text = n.ToString() + " item"+ ( n != 1 ? "s":"") + ".";
			}

			System::Void lvMissingList_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) {
			}
			System::Void menuSearchSites_ItemClicked(System::Object^  sender, System::Windows::Forms::ToolStripItemClickedEventArgs^  e) 
			{
				mDoc->SetSearcher(safe_cast<int>(e->ClickedItem->Tag));
				UpdateSearchButton();

			}
			System::Void bnDoMovingAndCopying_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				mDoc->DoCopyMoving();
				FillCopyMoveList();
				FillMissingList();
				RefreshWTW(false, true);
			}
			bool MissingListHasStuff()
			{
				if (mDoc->MissingEpisodes->Count == 0)
				{
					System::Windows::Forms::DialogResult res = MessageBox::Show("There are no missing episodes on the \"Missing\" tab.  Do a missing check first?","TVRename Finding and Organising",MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
					if (res == ::DialogResult::Yes)
						bnDoMissingCheck_Click(nullptr,nullptr);
				}
				return (mDoc->MissingEpisodes->Count > 0);
			}
			System::Void bnFindMissingStuff_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (!MissingListHasStuff())
					return;
				mDoc->LookForMissingEps(this->SetProgress, cbLeaveOriginals->Checked);
				FillCopyMoveList();
			}

			void FillCopyMoveList()
			{
				__int64 totalSize = 0;
				lvCopyMoveList->Items->Clear();
				for each (RCItem ^ri in mDoc->CopyMoveList)
				{
					AddRCItem(ri, lvCopyMoveList, false);
					FileInfo ^fi = gcnew FileInfo(ri->FullFromName());
					totalSize += fi->Exists ? fi->Length : 0;
				}
				int n = mDoc->CopyMoveList->Count;
				if (n == 0)
					txtFOCopyMoveStats->Text = "";
				else
				{  
					txtFOCopyMoveStats->Text = n.ToString() + " item";
					if (n != 1)
						txtFOCopyMoveStats->Text += "s";
					txtFOCopyMoveStats->Text += ", ";
					int gb = (int)(0.5 + (double)totalSize/(1024*1024*1024));
					if (gb > 1)
						txtFOCopyMoveStats->Text += gb.ToString() + " GB.";
					else
					{
						int mb = (int)(0.5 + (double)totalSize/(1024*1024));
						txtFOCopyMoveStats->Text += mb.ToString() + " MB.";
					}
				}
			}

			void FillSearchFolderList()
			{
				lbSearchFolders->Items->Clear();
				mDoc->SearchFolders->Sort();
				for each (String ^efi in mDoc->SearchFolders)
					lbSearchFolders->Items->Add(efi);
			}


			System::Void bnAddSearchFolder_Click(System::Object^  sender, System::EventArgs^  e)
			{
				int n = lbSearchFolders->SelectedIndex;
				if (n != -1)
					folderBrowser->SelectedPath = mDoc->SearchFolders[n];
				else
					folderBrowser->SelectedPath = "";

				if (folderBrowser->ShowDialog() == System::Windows::Forms::DialogResult::OK)
				{
					mDoc->SearchFolders->Add(folderBrowser->SelectedPath);
					mDoc->SetDirty();
				}

				FillSearchFolderList();
			}

			System::Void bnRemoveSearchFolder_Click(System::Object^  sender, System::EventArgs^  e)
			{
				int n = lbSearchFolders->SelectedIndex;
				if (n == -1)
					return;

				mDoc->SearchFolders->RemoveAt(n);
				mDoc->SetDirty();

				FillSearchFolderList();

			}

			System::Void bnOpenSearchFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				int n = lbSearchFolders->SelectedIndex;
				if (n == -1)
					return;
				TVDoc::SysOpen(mDoc->SearchFolders[n]);
			}
			System::Void bnWhenToWatchCheck_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				RefreshWTW(true, false);
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

									ListViewItem ^lvi = AddWTW(ei);

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
					lvWhenToWatch->ListViewItemSorter = gcnew DateSorterWTW(3);
					lvWhenToWatch->ShowGroups = false;
				}
				else if ((col==3)||(col==4))
				{
					lvWhenToWatch->ListViewItemSorter = gcnew DateSorterWTW(3);
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

			void RefreshWTW(bool doDownloads, bool suppressErrors)
			{
				if (doDownloads)
					if (!mDoc->DoDownloadsFG())
						return;

				mInternalChange++;
				mDoc->DoWhenToWatch(true, suppressErrors);
				FillShowLists();
				FillWhenToWatchList();
				mInternalChange--;

				mDoc->WriteUpcomingRSS();
			}

			System::Void refreshWTWTimer_Tick(System::Object^  sender, System::EventArgs^  e) 
			{
				if (!Busy)
					RefreshWTW(false, true);
			}
			void UpdateToolstripWTW()
			{
				// update toolstrip text too
				ProcessedEpisodeList ^next1 = mDoc->NextNShows(1, 36500);

				tsNextShowTxt->Text = "Next airing: ";
				if ((next1 != nullptr) && (next1->Count >= 1))
				{
					ProcessedEpisode ^ei = next1[0];
					tsNextShowTxt->Text += CustomName::NameFor(ei, CustomName::OldNStyle(1)) + ", " + ei->HowLong() + " (" + ei->DayOfWeek() + ", " + ei->TimeOfDay() + ")";
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
				if (!url->CompareTo(gcnew String("about:blank")))
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
			System::Void lvCopyMoveList_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			{
				if (e->KeyCode == Keys::Delete)
				{
					// delete selected items

					for each (ListViewItem ^lvi in lvCopyMoveList->SelectedItems)
					{
						RCItem ^rci = safe_cast<RCItem ^>(lvi->Tag);
						mDoc->CopyMoveList->Remove(rci);
					}
					FillCopyMoveList();
				}
			}

			System::Void lvRenameList_ColumnClick(System::Object^  sender, System::Windows::Forms::ColumnClickEventArgs^  e) 
			{
				lvRenameList->ListViewItemSorter = gcnew TextSorter(e->Column);
			}
			System::Void lvMissingList_ColumnClick(System::Object^  sender, System::Windows::Forms::ColumnClickEventArgs^  e) 
			{
				int col = e->Column;
				if ((col == 2)||(col == 3))
					lvMissingList->ListViewItemSorter = gcnew NumberAsTextSorter(col);
				else 
					if (col == 5)
						lvMissingList->ListViewItemSorter = gcnew DateSorterML(col);
					else
						lvMissingList->ListViewItemSorter = gcnew TextSorter(col);
			}
			System::Void lvCopyMoveList_ColumnClick(System::Object^  sender, System::Windows::Forms::ColumnClickEventArgs^  e) 
			{
				lvCopyMoveList->ListViewItemSorter = gcnew TextSorter(e->Column);
			}

			System::Void lvRenameList_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			{
				if (e->KeyCode == Keys::Delete)
				{
					// delete selected items

					for each (ListViewItem ^lvi in lvRenameList->SelectedItems)
					{
						RCItem ^rci = safe_cast<RCItem ^>(lvi->Tag);
						mDoc->RenameList->Remove(rci);
					}
					FillRenameList();
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
			System::Void lvMissingList_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
			{
				bnBTSearch_Click(nullptr,nullptr);
			}
			System::Void bnBTOpenFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				TVDoc::SysOpen(txtFolder->Text);
			}
			System::Void bnBrowseFolder_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (txtFolder->Text != "")
					folderBrowser->SelectedPath = txtFolder->Text;
				else if (txtTorrentFile->Text != "")
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
				if (txtTorrentFile->Text != "")
				{
					FileInfo ^fi = gcnew FileInfo(txtTorrentFile->Text);
					if (fi != nullptr)
					{
						openFile->InitialDirectory = fi->DirectoryName;
						openFile->FileName = fi->Name;
					}
				}
				else if (txtFolder->Text != "")
				{
					openFile->InitialDirectory = txtFolder->Text;
					openFile->FileName = "";
				}


				if (txtTorrentFile->Text != "")
				{
					FileInfo ^fi = gcnew FileInfo(txtTorrentFile->Text);
					if (fi->Exists)
						openFile->FileName = txtTorrentFile->Text;
				}

				if (openFile->ShowDialog() == System::Windows::Forms::DialogResult::OK)
					txtTorrentFile->Text = openFile->FileName;
			}
			System::Void bnGo_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				int action = actNone;
				if (rbBTCopyTo->Checked)
					action = actCopy;
				else if (rbBTRenameFiles->Checked)
					action = actRename;

				bool ok = mDoc->ProcessTorrent(txtTorrentFile->Text, 
					txtFolder->Text, 
					tmatchTree, 
					this->SetProgress,
					action,
					txtBTSecondLocation->Text);

				if (ok)
				{
					FillRenameList();
					FillCopyMoveList();
					if (action & actCopy)
						tabControl1->SelectTab(tbFnO);
					else
						tabControl1->SelectTab(tbRenaming);
				}
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
			/*
			MonitorItem ^nthMonitorItem(bool ignored, int n)
			{
			int c = 0;
			for each (MonitorItem ^mi in mDoc->GetStringList())
			if ((mi->Ignore == ignored) && (c++ == n))
			return mi;
			return nullptr;
			}
			*/
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
			System::Void lbSearchFolders_DragOver(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
			{
				if (  !e->Data->GetDataPresent( DataFormats::FileDrop ) )
					e->Effect = DragDropEffects::None;
				else
					e->Effect = DragDropEffects::Copy;
			}
			System::Void lbSearchFolders_DragDrop(System::Object^  sender, System::Windows::Forms::DragEventArgs^  e) 
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
							mDoc->SearchFolders->Add(path->ToLower());
						}
					}
					catch (...)
					{
					}
				}
				mDoc->SetDirty();
				FillSearchFolderList();
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
			System::Void lbSearchFolders_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			{
				if (e->KeyCode == Keys::Delete)
					bnRemoveSearchFolder_Click(nullptr,nullptr);
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
			System::Void lvMissingList_KeyDown(System::Object^  sender, System::Windows::Forms::KeyEventArgs^  e) 
			{
				if (e->KeyCode == Keys::Delete)
				{
					// delete selected items

					for each (ListViewItem ^lvi in lvMissingList->SelectedItems)
					{
						MissingEpisode ^mi = safe_cast<MissingEpisode ^>(lvi->Tag);
						mDoc->MissingEpisodes->Remove(mi);
					}
					FillMissingList();
				}
			}
			void GotoEpguideFor(ShowItem ^si, bool changeTab)
			{
				if (changeTab)
					tabControl1->SelectTab(tbMyShows);
				FillEpGuideHTML(si, -1);
			}

			void GotoEpguideFor(ProcessedEpisode ^pe, bool changeTab)
			{
				if (changeTab)
					tabControl1->SelectTab(tbMyShows);

				SelectSeason(pe->TheSeason);

				//FillEpGuideHTML(pe->SI, pe->SeasonNumber);
			}

			void RightClickOnMyShows(ShowItem ^si, Point ^pt)
			{
				mLastShowClicked = si;
				mLastEpClicked = nullptr;
				mLastSeasonClicked = nullptr;
				BuildRightClickMenu(pt);

			}
			void RightClickOnMyShows(Season ^seas, Point ^pt)
			{
				mLastShowClicked = mDoc->GetShowItem(seas->TheSeries->TVDBCode);
				mLastEpClicked = nullptr;
				mLastSeasonClicked = seas;
				BuildRightClickMenu(pt);
			}
			void RightClickOnShow(ProcessedEpisode ^ep, Point ^pt)
			{
				mLastEpClicked = ep;
				mLastShowClicked = ep != nullptr ? ep->SI: nullptr;
				mLastSeasonClicked = ep != nullptr ? ep->TheSeason : nullptr;
				BuildRightClickMenu(pt);
			}

			void BuildRightClickMenu( Point ^pt)
			{
				ShowItem ^si = mLastShowClicked;
				Season ^seas = mLastSeasonClicked;
				ProcessedEpisode ^ep = mLastEpClicked;
				//showRightClickMenu->T
				showRightClickMenu->Items->Clear();
				mFoldersToOpen->Clear();
				mLastFL = gcnew FileList();

				ToolStripMenuItem ^tsi;

				if (si != nullptr)
				{
					tsi = gcnew ToolStripMenuItem("Episode Guide");     tsi->Tag = (int)kEpisodeGuideForShow; showRightClickMenu->Items->Add(tsi);
				}

				if (ep != nullptr)
				{
					tsi = gcnew ToolStripMenuItem("Visit thetvdb.com");     tsi->Tag = (int)kVisitTVDBEpisode; showRightClickMenu->Items->Add(tsi);
				}
				else if (seas != nullptr)
				{
					tsi = gcnew ToolStripMenuItem("Visit thetvdb.com");     tsi->Tag = (int)kVisitTVDBSeason; showRightClickMenu->Items->Add(tsi);
				}
				else if (si != nullptr)
				{
					tsi = gcnew ToolStripMenuItem("Visit thetvdb.com");     tsi->Tag = (int)kVisitTVDBSeries; showRightClickMenu->Items->Add(tsi);
				}

				if (si != nullptr)
				{
					tsi = gcnew ToolStripMenuItem("Force Refresh");     tsi->Tag = (int)kForceRefreshSeries; showRightClickMenu->Items->Add(tsi);
					ToolStripSeparator ^tss = gcnew ToolStripSeparator(); showRightClickMenu->Items->Add(tss);
					tsi = gcnew ToolStripMenuItem("Missing Check");     tsi->Tag = (int)kMissingCheckSeries; showRightClickMenu->Items->Add(tsi);
					tsi = gcnew ToolStripMenuItem("Renaming Check");     tsi->Tag = (int)kRenamingCheckSeries; showRightClickMenu->Items->Add(tsi);
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

				if (ep != nullptr)
				{
					if (ep->SI->AllFolderLocations(mDoc->Settings)->ContainsKey(ep->SeasonNumber))
					{
						int n = mFoldersToOpen->Count;
						bool first = true;
						for each (String ^folder in ep->SI->AllFolderLocations(mDoc->Settings)[ep->SeasonNumber])
						{
							if ((folder != "") && DirectoryInfo(folder).Exists)
							{
								if (first)
								{
									ToolStripSeparator ^tss = gcnew ToolStripSeparator();  showRightClickMenu->Items->Add(tss);
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
				else if ((seas != nullptr) && (si != nullptr) && 
					(si->AllFolderLocations(mDoc->Settings)->ContainsKey(seas->SeasonNumber)) )
				{
					int n = mFoldersToOpen->Count;
					bool first = true;
					Generic::List<String ^> ^added = gcnew Generic::List<String ^>();
					for each (String ^folder in si->AllFolderLocations(mDoc->Settings)[seas->SeasonNumber])
					{
						if ((folder != "") && DirectoryInfo(folder).Exists && !added->Contains(folder))
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

					Generic::List<String ^> ^added = gcnew Generic::List<String ^>();
					for each (KeyValuePair<int, StringList ^> ^kvp in si->AllFolderLocations(mDoc->Settings))
					{
						for each (String ^folder in kvp->Value)
						{
							if ((folder != "") && DirectoryInfo(folder).Exists && !added->Contains(folder))
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
				showRightClickMenu->Show(*pt);
			}
			System::Void showRightClickMenu_ItemClicked(System::Object^  sender, System::Windows::Forms::ToolStripItemClickedEventArgs^  e) 
			{
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
					}
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
					if (mLastShowClicked != nullptr)
						ForceRefresh(mLastShowClicked);
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
						break;
					}

				}

				mLastEpClicked = nullptr;
			}
			System::Void tabControl1_DoubleClick(System::Object^  sender, System::EventArgs^  e) 
			{
				if (tabControl1->SelectedTab == tbMyShows)
					bnMyShowsRefresh_Click(nullptr,nullptr);
				else if (tabControl1->SelectedTab == tbRenaming)
					bnRenameCheck_Click(nullptr,nullptr);
				else if (tabControl1->SelectedTab == tbMissing)
					bnDoMissingCheck_Click(nullptr,nullptr);
				else if (tabControl1->SelectedTab == tbFnO)
					bnFindMissingStuff_Click(nullptr,nullptr);
				else if (tabControl1->SelectedTab == tbFM)
					bnFMCheck_Click(nullptr,nullptr);
				else if (tabControl1->SelectedTab == tbWTW)
					bnWhenToWatchCheck_Click(nullptr,nullptr);
			}
			System::Void lvMissingList_MouseClick(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			{
				if (e->Button != System::Windows::Forms::MouseButtons::Right)
					return;

				if (lvMissingList->SelectedItems->Count == 0)
					return;

				Point^ pt = lvMissingList->PointToScreen(Point(e->X, e->Y));
				RightClickOnShow(safe_cast<MissingEpisode ^>(lvMissingList->SelectedItems[0]->Tag),pt);
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

			System::Void lvRenameList_MouseClick(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			{
				if (e->Button != System::Windows::Forms::MouseButtons::Right)
					return;

				if (lvRenameList->SelectedItems->Count == 0)
					return;

				RCItem ^rci = safe_cast<RCItem ^>(lvRenameList->SelectedItems[0]->Tag);

				Point^ pt = lvRenameList->PointToScreen(Point(e->X, e->Y));
				RightClickOnFolder(rci->FromFolder,pt);
			}

			System::Void lstFMMonitorFolders_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			{
				if (e->Button != System::Windows::Forms::MouseButtons::Right)
					return;

				lstFMMonitorFolders->ClearSelected();
				lstFMMonitorFolders->SelectedIndex = lstFMMonitorFolders->IndexFromPoint(Point(e->X,e->Y));

				int p;
				if ((p = lstFMMonitorFolders->SelectedIndex) == -1)
					return;

				Point^ pt = lstFMMonitorFolders->PointToScreen(Point(e->X, e->Y));
				RightClickOnFolder(lstFMMonitorFolders->Items[p]->ToString(),pt);
			}
			System::Void lstFMIgnoreFolders_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			{
				if (e->Button != System::Windows::Forms::MouseButtons::Right)
					return;

				lstFMIgnoreFolders->ClearSelected();
				lstFMIgnoreFolders->SelectedIndex = lstFMIgnoreFolders->IndexFromPoint(Point(e->X,e->Y));

				int p;
				if ((p = lstFMIgnoreFolders->SelectedIndex) == -1)
					return;

				Point^ pt = lstFMIgnoreFolders->PointToScreen(Point(e->X, e->Y));
				RightClickOnFolder(lstFMIgnoreFolders->Items[p]->ToString(),pt);
			}
			System::Void lbSearchFolders_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			{
				if (e->Button != System::Windows::Forms::MouseButtons::Right)
					return;

				lbSearchFolders->ClearSelected();
				lbSearchFolders->SelectedIndex = lbSearchFolders->IndexFromPoint(Point(e->X,e->Y));

				int p;
				if ((p = lbSearchFolders->SelectedIndex) == -1)
					return;

				Point^ pt = lbSearchFolders->PointToScreen(Point(e->X, e->Y));
				RightClickOnFolder(lbSearchFolders->Items[p]->ToString(),pt);
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
				Preferences ^pref = gcnew Preferences(mDoc);
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
			System::Void lvCopyMoveList_MouseClick(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
			{
				if (e->Button != System::Windows::Forms::MouseButtons::Right)
					return;

				if (lvCopyMoveList->SelectedItems->Count == 0)
					return;

				RCItem ^ri = safe_cast<RCItem ^>(lvCopyMoveList->SelectedItems[0]->Tag);

				Point^ pt = lvCopyMoveList->PointToScreen(Point(e->X, e->Y));
				RightClickOnFolder(ri->FromFolder,pt);
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
						RefreshWTW(false, true);

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

			System::Void bnBTCopyToBrowse_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (txtBTSecondLocation->Text != "")
					folderBrowser->SelectedPath = txtBTSecondLocation->Text;
				else if (txtTorrentFile->Text != "")
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

			System::Void tabControl1_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) 
			{
				if (tabControl1->SelectedTab == tbMyShows)
					FillEpGuideHTML();
				exportToolStripMenuItem->Enabled = ( (tabControl1->SelectedTab == tbMissing) ||
					(tabControl1->SelectedTab == tbFnO) ||
					(tabControl1->SelectedTab == tbRenaming) );
			}

			System::Void bugReportToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				BugReport ^br = gcnew BugReport(mDoc);
				br->ShowDialog();
			}
			System::Void exportToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			{
				if (tabControl1->SelectedTab == tbMissing)
				{
					if (!MissingListHasStuff())
						return;

					saveFile->Filter = "CSV Files|*.csv|XML Files|*.xml";
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
			void FMLVISet(AddItem ^ai, ListViewItem ^lvi)
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
						mDoc->GenDict(true, true);
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
							if (s != "")
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
				FillShowLists();
			}

			void GuessAI(AddItem ^ai)
			{
				mDoc->GuessShowName(ai);
				if (ai->ShowName != "")
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

				if (name == "")
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
						n2->Tag = ser->Seasons[snum];
						n->Nodes->Add(n2);
					}
				}

				MyShowTree->Nodes->Add(n);

				db->Unlock("AddShowItemToTree");

				return n;
			}


			void AddMissingEpisode(MissingEpisode ^mi, System::Windows::Forms::ListView ^lv)
			{
				System::Windows::Forms::ListViewItem ^lvi = gcnew System::Windows::Forms::ListViewItem;

				lvi->Text = mi->SI->ShowName();

				int snum = mi->SeasonNumber;
				lvi->SubItems->Add(mi->WhereItBelongs);
				lvi->SubItems->Add(snum.ToString());

				String ^epl = mi->EpNum.ToString();
				if (mi->EpNum != mi->EpNum2)
					epl += "-" + mi->EpNum2.ToString();

				lvi->SubItems->Add(epl);

				lvi->SubItems->Add(mi->Name);

				DateTime ^dt = mi->GetAirDateDT(true);
				if ((dt != nullptr) && (dt->CompareTo(DateTime::MaxValue)))
					lvi->SubItems->Add(mi->GetAirDateDT(true)->ToShortDateString());
				else
					lvi->SubItems->Add("");

				lvi->Tag = mi;

				lv->Items->Add(lvi);
			}

			void AddRCItem(RCItem ^rci, System::Windows::Forms::ListView ^lv, bool renameListStyle)
			{
				System::Windows::Forms::ListViewItem ^lvi = gcnew System::Windows::Forms::ListViewItem;
				if (renameListStyle)
				{
					lvi->Text = rci->ShowName;
					lvi->SubItems->Add(rci->FromFolder);
					lvi->SubItems->Add(rci->FromName);
					lvi->SubItems->Add(rci->ToName);
					lvi->SubItems->Add(rci->LastError);
				}
				else
				{
					lvi->Text = rci->GetOperationName();
					lvi->SubItems->Add(rci->FromFolder);
					lvi->SubItems->Add(rci->FromName);
					lvi->SubItems->Add(rci->ToFolder);
					lvi->SubItems->Add(rci->ToName);
					lvi->SubItems->Add(rci->LastError);
				}

				if ((rci->LastError == nullptr) || (rci->LastError != ""))
					lvi->BackColor = WarningColor();
				lvi->Tag = rci;
				lv->Items->Add(lvi);
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

			ListViewItem ^AddWTW(ProcessedEpisode ^pe)
			{
				System::Windows::Forms::ListViewItem ^lvi = gcnew System::Windows::Forms::ListViewItem();
				lvi->Text = "";
				for (int i=0;i<7;i++)
					lvi->SubItems->Add("");

				UpdateWTW(pe, lvi);

				lvWhenToWatch->Items->Add(lvi);

				return lvi;

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
					 ShowAddedOrEdited(si);
					 SelectShow(si);
				 }
				 LessBusy();
			 }

			 void ShowAddedOrEdited(ShowItem ^si)
			 {
				 mDoc->SetDirty();
				 RefreshWTW(true, false);
				 FillShowLists();
				 // FillEpGuideHTML(si, -1);
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
				 ShowAddedOrEdited(nullptr);
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
				 ProcessedEpisodeList ^pel = TVDoc::GenerateEpisodes(si, ser, seasnum, true, false);

				 EditRules ^er = gcnew EditRules(si, pel, seasnum, mDoc->Settings->NamingStyle);
				 System::Windows::Forms::DialogResult dr = er->ShowDialog();
				 db->Unlock("EditSeason");
				 if (dr == ::DialogResult::OK)
				 {
					 ShowAddedOrEdited(si);
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

				 AddEditShow ^aes = gcnew AddEditShow(si, db, ser != nullptr ? ser->TimeZone : TZMagic::DefaultTZ());

				 System::Windows::Forms::DialogResult dr = aes->ShowDialog();

				 db->Unlock("EditShow");

				 if (dr == System::Windows::Forms::DialogResult::OK)
				 {
					 if (ser != nullptr)
						 ser->TimeZone = aes->TimeZone; // TODO: move into AddEditShow

					 ShowAddedOrEdited(si);
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
				 RefreshWTW(false, true);
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
					 int n = keys[0];
					 if (afl[n]->Count)
					 {
						 try {
							 TVDoc::SysOpen(afl[n][0]);
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
					 if ((si->AutoAdd_FolderBase != "") && (DirectoryInfo(si->AutoAdd_FolderBase).Exists))
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
				 //if (si == nullptr)
				 //    return;

				 Season ^seas = TreeNodeToSeason(n);
				 /*int snum = -1;
				 if (seas == nullptr)
				 {
				 int lastSeas = -1;
				 for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in si->SeasonEpisodes)
				 {
				 if ((lastSeas == -1) || (kvp->Key > lastSeas))
				 lastSeas = kvp->Key;
				 }
				 snum = lastSeas;
				 }
				 else
				 snum = seas->SeasonNumber;

				 if ((snum == -1) || (si->SeasonEpisodes[snum]->Count == 0))
				 return;*/

				 if (seas != nullptr)
					 RightClickOnMyShows(seas,pt); // ->SeasonEpisodes[snum][0]
				 else if (si != nullptr)
					 RightClickOnMyShows(si,pt); // ->SeasonEpisodes[snum][0]
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

	public:
		String ^FMPUpto;
		int FMPPercent;
		bool FMPStopNow;
		FolderMonitorProgress ^FMP;

		void FMPShower()
		{
			FMP = gcnew FolderMonitorProgress(this);
			::DialogResult dr = FMP->ShowDialog();
			FMP = nullptr;
		}
	private: System::Void bnFMFullAuto_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 FMPStopNow = false;
				 FMPUpto = "";
				 FMPPercent = 0;

				 Thread ^fmpshower = gcnew Thread(gcnew ThreadStart(this, &UI::FMPShower));
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
						 if (ai->ShowName != "")
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
				 Season ^currentSeas = TreeNodeToSeason(MyShowTree->SelectedNode);
				 ShowItem ^currentSI = TreeNodeToShowItem(MyShowTree->SelectedNode);
				 String ^theFolder = "";

				 if (currentSI != nullptr)
				 {
					 for each (KeyValuePair<int, StringList ^> ^kvp in currentSI->AllFolderLocations(mDoc->Settings))
					 {
						 for each (String ^folder in kvp->Value)
						 {
							 if ((folder != "") && DirectoryInfo(folder).Exists)
							 {
								 theFolder = folder;
								 break;
							 }
						 }
						 if (theFolder != "")
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
	private: System::Void bnRSSGo_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 lvRSSItems->Items->Clear();

				 mDoc->RSSList = gcnew RSSItemList();
				 mDoc->RSSList->DownloadRSS(txtRSSURL->Text, mDoc->Settings->FNPRegexs);
				 for each (RSSItem ^it in mDoc->RSSList)
				 {
					 ListViewItem ^lvi = gcnew ListViewItem();
					 lvi->Text = it->Title;
					 lvi->SubItems->Add(it->Season.ToString());
					 lvi->SubItems->Add(it->Episode.ToString());
					 lvi->SubItems->Add(it->ShowName);
					 lvRSSItems->Items->Add(lvi);
				 }
				 mDoc->MatchRSSToMissing();

				 lvRSSMatchingMissing->Items->Clear();
				 for each (RSSMissingItem ^mi in mDoc->RSSMissingList)
				 {
					 ListViewItem ^lvi = gcnew ListViewItem();
					 lvi->Text = mi->RSS->Title;
					 lvi->SubItems->Add(mi->Episode->Season.ToString());
					 lvi->SubItems->Add(mi->Episode->EpNum.ToString());
					 lvi->SubItems->Add(mi->Episode->TheSeries->Name);
					 lvi->SubItems->Add(mi->Episode->WhereItBelongs);
					 lvi->SubItems->Add(mDoc->FilenameFriendly(mDoc->Settings->NamingStyle->NameFor(mi->Episode)));
					 lvi->Tag = mi;
					 lvRSSMatchingMissing->Items->Add(lvi);
				 }
			 }
	private: System::Void bnRSSDownload_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 int c = lvRSSMatchingMissing->SelectedItems->Count;
				 int n =0;
				 for each (ListViewItem ^lvi in lvRSSMatchingMissing->SelectedItems)
				 {
					 SetProgressActual(100*(n+1)/(c+1));
					 RSSMissingItem ^mi = safe_cast<RSSMissingItem ^>(lvi->Tag);
					 mDoc->DownloadRSS(mi, txtRSSuTorrentPath->Text);
					 n++;
				 }
				 SetProgressActual(0);
			 }
	private: System::Void languagePreferenceToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 TheTVDB ^db = mDoc->GetTVDB(true,"LanguagePreference");
				 PreferredLanguage ^pl = gcnew PreferredLanguage(db);
				 if (pl->ShowDialog() == ::DialogResult::OK)
					 mDoc->SetDirty();
				 db->Unlock("LanguagePreference");
			 }
	private: System::Void actorsToolStripMenuItem_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 // find actors that have been in more than one thing
				 Dictionary<String^, StringList ^> ^whoInWhat = gcnew Dictionary<String^, StringList ^>;
				 TheTVDB ^db = mDoc->GetTVDB(true,"Actors");
				 for each (KeyValuePair<int, SeriesInfo ^> ^ser in db->GetSeriesDict())
				 {
					 SeriesInfo ^si = ser->Value;
					 String ^actors = si->GetItem("Actors");
					 if (actors != "")
					 {
						 for each (String ^aa in actors->Split('|'))
						 {
							 if (aa != "")
							 {
								 if (!whoInWhat->ContainsKey(aa))
									 whoInWhat[aa] = gcnew StringList();
								 if (!whoInWhat[aa]->Contains(si->Name))
									 whoInWhat[aa]->Add(si->Name);
							 }
						 }
					 }
					 /*
					 for each (KeyValuePair<int, Season ^> ^kvp in si->Seasons)
					 {
						 for each (Episode ^ep in kvp->Value->Episodes)
						 {
							 String ^guest = ep->GetItem("GuestStars");

							 if (guest != "")
							 {
								 for each (String ^aa in guest->Split('|'))
								 {
									 if (aa != "")
									 {
										 if (!whoInWhat->ContainsKey(aa))
											 whoInWhat[aa] = gcnew StringList();
										 if (!whoInWhat[aa]->Contains(si->Name))
											 whoInWhat[aa]->Add(si->Name);
									 }
								 }
							 }

						 }
					 }
					 */
				 }

				 StringList ^shows = gcnew StringList();
				 for each (KeyValuePair<String ^,StringList ^> ^kvp in whoInWhat)
				 {
					 if (kvp->Value->Count > 1)
					 {
						 for each (String ^s in kvp->Value)
							 if (!shows->Contains(s))
								 shows->Add(s);
					 }
				 }

				 shows->Sort();

				 // make an awesome HTML table

				 String ^html = "<font size=8pt>";
				 html += "<table border=1>";
				 html += "<tr><th>Show/Actor</th>";
				 int actorCount = 0;
				 for each (KeyValuePair<String ^,StringList ^> ^kvp in whoInWhat)
					 if (kvp->Value->Count > 1)
					 {
						 html += "<th><A HREF=\"http://www.imdb.com/find?s=nm&q="+kvp->Key+"\">"+kvp->Key+"</a></th>";
						 actorCount++;
					 }
				 html += "<th>Total</th>";
				 html += "</tr>";

				 array<int> ^colTotals = gcnew array<int>(actorCount);
				 int i;
				 for (i=0;i<actorCount;i++)
					 colTotals[i] = 0;

				 for each (String ^s in shows)
				 {
					 //String ^u = db->WebsiteURL(si->TVDBCode, 0, true)
					 //html += "<tr><td><A HREF=\""+u+"\">"+s+"</a></td>";
					 html += "<tr><td>"+s+"</td>";

					 int rowtotal = 0;
					 i = 0;
					 for each (KeyValuePair<String ^,StringList ^> ^kvp in whoInWhat)
					 {
						 if (kvp->Value->Count > 1)
						 {
							 if (kvp->Value->Contains(s))
							 {
								 html += "<td style=\"background-color: #00AA00;\">Y</td>";
								 colTotals[i]++;
								 rowtotal++;
							 }
							 else
								 html += "<td>&nbsp;</td>";
							i++;
						 }
					 }
					 html += "<td>"+rowtotal+"</td>";
					 html += "</tr>";
				 }

				 html += "<tr><th>Totals</td>";
				 for (i=0;i<actorCount;i++)
					 html += "<td>"+colTotals[i]+"</td>";
				 html += "</tr>";


				 html += "</table></font>";

				 tabControl1->SelectTab(tbMyShows);

				 SetGuideHTMLbody(html);

				 db->Unlock("Actors");
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
}; // UI class
} // namespace
