#pragma once

#include "TheTVDB.h"
#include "RenameItem.h"
#include "Sorters.h"
#include "Searchers.h"
#include "MissingEpisode.h"
#include "BT.h"
#include "ShowItem.h"
#include "Settings.h"
#include "AddItem.h"
#include "Server.h"
#include "Statistics.h"
#include "CopyMoveProgress.h"
#include "DownloadProgress.h"
#include "MissingFolderAction.h"
#include "RSS.h"

namespace TVRename {
    using namespace System;
    using namespace System::ComponentModel;
    using namespace System::Collections;
    using namespace System::Windows::Forms;
    using namespace System::Data;
    using namespace System::Drawing;
    using namespace System::IO;
    using namespace System::Threading;
    using namespace System::Net;
    using namespace System::IO::Compression;

    typedef Collections::Generic::List<String ^> StringList;
   
    public ref class TVDoc
    {
    private:
        TVRenameStats ^mStats;
        bool mDirty;
        ShowItemList ^ShowItems;

        System::Threading::Thread ^mDownloaderThread;
        bool DownloadOK;

        TheTVDB ^mTVDB;

    public:
        bool DownloadDone;
        bool DownloadStopOnError;
        int DownloadPct;
        int DownloadsRemaining;

        TheTVDB ^GetTVDB(bool lock, String ^whoFor)
        {
            if (lock)
            {
                if (whoFor == "")
                    whoFor = "unknown";

                mTVDB->GetLock("GetTVDB : " + whoFor);
            }
            return mTVDB;
        }

        AddItemList ^AddItems;
        TVSettings ^Settings;
        StringList ^MonitorFolders;
        StringList ^IgnoreFolders;
        StringList ^SearchFolders;
        RCList ^RenameList;
        RCList ^CopyMoveList;
        MissingEpisodeList ^MissingEpisodes;
        RSSItemList ^RSSList;
        RSSMissingItemList ^RSSMissingList;

    public:
        TVDoc()
        {
            RSSMissingList = nullptr;
            mTVDB = gcnew TheTVDB();
            mStats = gcnew TVRenameStats();
            mDirty = false;
            //mLayout = gcnew Layout();

            Settings = gcnew TVSettings();
            MissingEpisodes = gcnew MissingEpisodeList();

            MonitorFolders = gcnew StringList();
            IgnoreFolders = gcnew StringList();
            SearchFolders = gcnew StringList();
            ShowItems = gcnew ShowItemList();
            RenameList = gcnew RCList();
            CopyMoveList = gcnew RCList();
            AddItems = gcnew AddItemList();

            DownloadDone = true;
            DownloadOK = true;

            LoadXMLSettings();
            //    StartServer();
        }

        ~TVDoc()
        {
            StopBGDownloadThread();
        }

    private: void LockShowItems()
        {
            return;
            Monitor::Enter(ShowItems);
        }
    public: void UnlockShowItems()
        {
            return;
            Monitor::Exit(ShowItems);
        }

    public: TVRenameStats ^Stats()
        {
            mStats->NS_NumberOfShows = ShowItems->Count;
            mStats->NS_NumberOfSeasons = 0;
            mStats->NS_NumberOfEpisodesExpected = 0;

            LockShowItems();
            for each (ShowItem ^si in ShowItems)
            {
                for each (KeyValuePair<int, ProcessedEpisodeList ^> ^k in si->SeasonEpisodes)
                    mStats->NS_NumberOfEpisodesExpected += k->Value->Count;
                mStats->NS_NumberOfSeasons += si->SeasonEpisodes->Count;
            }
            UnlockShowItems();

            return mStats;
        }
        void SetDirty()
        {
            mDirty = true;
        }
        bool Dirty()
        {
            return mDirty;
        }
        ShowItemList ^GetShowItems(bool lock)
        {
            if (lock)
               LockShowItems();

            ShowItems->Sort(gcnew Comparison<ShowItem^>(CompareShowItemNames));
            return ShowItems;
        }
        ShowItem ^GetShowItem(int id)
        {
            LockShowItems();
            for each (ShowItem ^si in ShowItems)
            {
                if (si->TVDBCode == id)
                {
                    UnlockShowItems();
                    return si;
                }
            }
            UnlockShowItems();
            return nullptr;
        }


        /*
        void WebServer()
        {
        TVRenameServer ^serve = gcnew TVRenameServer(this); // all work is done in constructor which never returns
        }
        void StartServer()
        {
        mServerThread = gcnew System::Threading::Thread(gcnew System::Threading::ThreadStart(this, &TVDoc::WebServer));
        mServerThread->Name = "Web Server";
        mServerThread->Start();
        }
        void StopServer()
        {
        if (mServerThread != nullptr)
        {
        mServerThread->Abort();
        mServerThread = nullptr;

        }
        }
        */



        void DoCopyMoving(System::Windows::Forms::ProgressBar ^pbProgress)
        {
            CopyMoveList = ProcessRCList(pbProgress, CopyMoveList);
        }


        void DoRenaming(System::Windows::Forms::ProgressBar ^pbProgress)
        {
            RenameList = ProcessRCList(pbProgress, RenameList);
        }

        RCList ^ProcessRCList(System::Windows::Forms::ProgressBar ^pbProgress, RCList ^list)
        {
            int c = 0;
            pbProgress->Value = 0;

            CopyMoveResult res;
            CopyMoveProgress ^cmp = gcnew CopyMoveProgress(list, res, pbProgress, MissingEpisodes, Stats());
            cmp->ShowDialog();

            if ((res != kCopyMoveOk) && (res != kUserCancelled))
                MessageBox::Show(cmp->ErrorText(), "Errors", MessageBoxButtons::OK, MessageBoxIcon::Exclamation);

            //mMDI->UpdateRenamingWindow();
            pbProgress->Value = 0;
            RCList ^errFiles = cmp->ErrFiles();
            cmp = nullptr;
            return errFiles;
        }

        void DoRenameCheck(System::Windows::Forms::ProgressBar ^pbProgress)
        {
            Stats()->RenameChecksDone++;

            if (!DoDownloadsFG(pbProgress))
                return;

            RenameList->Clear();

            pbProgress->Value = 0;
            //lvStatus->Items->Clear();
            int c = 0;
            int totalN = 0;

            LockShowItems();

            for each (ShowItem ^si in ShowItems)
                if (si->DoRename)
                    totalN += si->SeasonEpisodes->Count;

            for each (ShowItem ^si in ShowItems)
            {
                if (!si->DoRename)
                    continue;

                if (si->AllFolderLocations(Settings)->Count == 0)
                    continue; // skip

                c += si->SeasonEpisodes->Count;

                pbProgress->Value = 100*(c+1) / (totalN+1); // +1 to always have a bit of activity in the bar when we're working
                pbProgress->Update();

                BuildRenameList(si, RenameList);
            }

            if (Settings->KeepTogether)
                KeepTogether(RenameList);

            UnlockShowItems();

            pbProgress->Value = 0;
        }

        void BuildRenameList(ShowItem ^si, RCList ^rl)
        {
            //            GenerateEpisodeDict(si, true, false);

            for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in si->SeasonEpisodes)
            {
                int snum = kvp->Key;
                if ((si->IgnoreSeasons->Contains(kvp->Key)) || (!si->AllFolderLocations(Settings)->ContainsKey(snum)))
                    continue; // ignore/skip this season

                // Season ^seas = kvp->Value;
                if ((snum == 0) && (si->CountSpecials)) 
                    continue; // skip specials

                StringList ^folders = si->AllFolderLocations(Settings)[snum];
                for each (String ^folder in folders)
                {
                    // generate new filename info
                    DirectoryInfo^ di;
                    try {
                        di = gcnew DirectoryInfo(folder);
                    }
                    catch (...)
                    {
                        return;
                    }

                    if (!di->Exists)
                        return; // we don't care. a missing check will pick up missing folders if necessary.

                    array<FileInfo^>^fi = di->GetFiles();

                    // fill renameItems
                    for each (FileInfo^ fiTemp in fi)
                    {
                        int seas, ep;

                        if (!Settings->UsefulExtension(fiTemp->Extension))
                            continue; // move on

                        if (FindSeasEp(fiTemp, &seas, &ep, si->ShowName()))
                        {
                            // see if we have an epinfo for this one
                            if (seas == -1)
                                seas = snum;

                            for each (ProcessedEpisode ^pe in si->SeasonEpisodes[snum])
                            {
                                if ((pe->EpNum == ep) && (pe->SeasonNumber == seas))
                                {
                                    //SeriesInfo ^ser = GetTVDB(false, "")->GetSeries(si->TVDBCode);

                                    String ^nname = FilenameFriendly(Settings->NamingStyle->NameFor(pe) + fiTemp->Extension);

                                    if (nname != fiTemp->Name)
                                    {
                                        rl->Add(gcnew RCItem(si->ShowName(), rcRename, folder, 
                                            fiTemp->Name, 
                                            folder,
                                            nname,
                                            pe));
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        void DoMissingCheck(System::Windows::Forms::ProgressBar ^pbProgress)
        {
            Stats()->MissingChecksDone++;
            mStats->NS_NumberOfEpisodes = 0;

            if (!DoDownloadsFG(pbProgress))
                return;

            MissingEpisodes->Clear();

            pbProgress->Value = 0;
            //lvStatus->Items->Clear();
            int c = 0;
            int totalN = 0;
            LockShowItems();

            for each (ShowItem ^si in ShowItems)
                if (si->DoMissingCheck)
                    totalN += si->AllFolderLocations(Settings)->Count;

            for each (ShowItem ^si in ShowItems)
            {
                if (!si->DoMissingCheck)
                    continue; // skip

                pbProgress->Value = 100*(c+1) / (totalN+1); // +1 to always have a bit of activity in the bar when we're working

                c += si->AllFolderLocations(Settings)->Count;
                pbProgress->Update();

                if (!UpToDateCheck(si))
                    break;
            }
            UnlockShowItems();

            pbProgress->Value = 0;
        }

        void SetSearcher(int n)
        {
            Settings->TheSearchers->SetToNumber(n);
            SetDirty();
        }

        void CheckFolder(DirectoryInfo ^di, AddItemList ^addList) // check a monitored folder for new shows
        {
            for each (DirectoryInfo ^di2 in di->GetDirectories())
            {
                String ^seasonWord = "Season ";
                bool hasSeasonFolders = false;
                try
                {
                    // keep in sync with ProcessAddItems, etc.
                    hasSeasonFolders = di2->GetDirectories("*" + seasonWord + "*")->Length > 0; // todo - use non specific word
                }
                catch (UnauthorizedAccessException ^)
                {
                    // e.g. recycle bin, system volume information
                    continue;
                }

                if (!hasSeasonFolders)
                    CheckFolder(di2, addList); // not a season folder.. recurse!

                String ^theFolder = di2->FullName->ToLower();

                // if its not in the ignore list
                if (IgnoreFolders->Contains(theFolder))
                    continue;

                // ..and not already a folder for one of our shows
                bool bzzt = false;
                for each (ShowItem ^si in ShowItems)
                {
                    if (si->AutoAddNewSeasons && (si->AutoAdd_FolderBase!=""))
                    {
                        int l = si->AutoAdd_FolderBase->Length;
                        if ((theFolder->Length >= l) && (theFolder->Substring(0,l)->ToLower() == si->AutoAdd_FolderBase->ToLower()))
                        {
                            bzzt = true;
                            break;
                        }
                    }
                    if (!bzzt)
                    {
                        FolderLocationDict ^afl = si->AllFolderLocations(Settings);
                        for each (KeyValuePair<int, StringList ^> ^kvp in afl)
                        {
                            for each (String ^folder in kvp->Value)
                            {
                                if (theFolder->ToLower() == folder->ToLower())
                                {
                                    bzzt = true;
                                    break;
                                }
                                if (bzzt)
                                    break;
                            }
                        }
                    }
                    else
                        break;
                } // for each showitem
                if (!bzzt)
                {
                    // ....its good!
                    addList->Add(gcnew AddItem(di2->FullName, hasSeasonFolders ? kfmFolderPerSeason : kfmFlat, -1));
                }
            }
        }

        void CheckMonitoredFolders()
        {
            AddItems->Clear();

            LockShowItems();
            for each (String ^folder in MonitorFolders)
            {
                //try {
                DirectoryInfo ^di = gcnew DirectoryInfo(folder);
                if (!di->Exists)
                    continue;

                CheckFolder(di, AddItems);
                /*  }
                catch (...)
                {
                }*/
            }
            UnlockShowItems();
        }

        bool ProcessTorrent(String ^torrent, 
            String ^folder, 
            TreeView ^tvTree, 
            ProgressBar ^pbProgress,
            int action,
            String ^secondLocation)
        {
            if (action == 0)
                return false;

            if (folder == "") 
                return false;
            if (torrent == "")
                return false;

            if (action & actCopy)
            {
                if (secondLocation == "")
                    return false;
                if (!Directory::Exists(secondLocation))
                    return false;
            }

            Stats()->TorrentsMatched++;
            RenameList->Clear();
            RCList ^theList = nullptr;
            
            if (action & actCopy)
                theList = CopyMoveList;
            else if (action & actRename)
                theList = RenameList;

            BTProcessor ^btp = gcnew BTProcessor();
            btp->Setup(torrent, folder, tvTree, nullptr, theList, pbProgress, action, secondLocation, nullptr, Settings->FNPRegexs);
            bool r = btp->Go();
            return r;
        }


        String ^FilenameFriendly(String ^fn)
        {
            for each (Replacement ^R in Settings->Replacements)
                fn = fn->Replace(R->This, R->That);
            return fn;
        }



        // consider each of files, see if it is suitable for series "ser" and episode "epi"
        // if so, add a rcitem for move to "fi"
        bool FindMissingEp(FileList ^files, MissingEpisode ^me)
        {
            String ^showname = me->SI->ShowName();
            int season = me->SeasonNumber;
            //DateTime ^airdate = epi->GetAirDateDT();
            String ^toFolder = me->WhereItBelongs;
            //(NStyle::Style)me->SI->NamingStyle
            String ^toName = FilenameFriendly(Settings->NamingStyle->NameFor(me));
            int epnum = me->EpNum;

            // TODO: find a 'best match', or use first ?

            showname = SimplifyName(showname);

            for each (FileInfo ^file in files)
            {
                bool matched = false;

                try
                {
                    if (!Settings->UsefulExtension(file->Extension)) // not a usefile file extension
                        continue;

                    String ^simplifiedfname = SimplifyName(file->DirectoryName+"\\"+file->Name);

                    matched = Regex::Match(simplifiedfname,"\\b"+showname+"\\b",RegexOptions::IgnoreCase)->Success;

                    if (matched)
                    {
                        int seasF, epF;
                        String ^fn = file->Name;

                        if ( ( FindSeasEp(file, &seasF, &epF, me->TheSeries->Name) && (seasF == season) && (epF == epnum) ) ||
                            ( me->SI->UseSequentialMatch && 
                            MatchesSequentialNumber(file->Name, &seasF, &epF, me) && (seasF == season) && (epF == epnum) )
                            )
                        {
                            CopyMoveList->Add(gcnew RCItem(showname, rcCopy, file->DirectoryName, file->Name, 
                                toFolder, toName+file->Extension, me));
                            return true;
                        }
                    }
                }
                catch (System::IO::PathTooLongException ^e)
                {
                    String ^t = "Path too long. " + file->DirectoryName+"\\"+file->Name + ", " + e->Message;
                    t += ".  Try to display more info?";
                    ::DialogResult dr = MessageBox::Show(t,"Path too long", MessageBoxButtons::YesNo, MessageBoxIcon::Exclamation);
                    if (dr == ::DialogResult::Yes)
                    {
                        t = "DirectoryName " + file->DirectoryName + ", File name: " + file->Name;
                        t +=  matched ? ", matched.  " : ", no match.  ";
                        if (matched)
                        {
                            t += "Show: " + me->TheSeries->Name + ", Season " + season.ToString() + ", Ep " + epnum.ToString() + ".  ";
                            t += "To folder: " + toFolder + ", to name: " + toName;
                        }

                        MessageBox::Show(t, "Path too long", MessageBoxButtons::OK, MessageBoxIcon::Exclamation);
                    }
                }
            }

            return false;
        }

        void KeepTogether(RCList ^rcl)
        {
            // for each of the items in rcl, do the same copy/move if for other items with the same
            // base name, but different extensions

            RCList ^extras = gcnew RCList();

            for each (RCItem ^ri in rcl)
            {
                try
                {
                    DirectoryInfo ^sfdi = gcnew DirectoryInfo(ri->FromFolder);
                    FileInfo ^ffi = gcnew FileInfo(ri->FullFromName());
                    String ^basename = ffi->Name;
                    int l = basename->Length;
                    basename = basename->Substring(0, l - ffi->Extension->Length);

                    String ^ftn = ri->FullToName();
                    FileInfo ^tfi = gcnew FileInfo(ftn);
                    String ^toname = tfi->Name;
                    int l2 = toname->Length;
                    toname = toname->Substring(0, l2 - tfi->Extension->Length);

                    cli::array<FileInfo ^> ^flist = sfdi->GetFiles(basename+".*");
                    for each (FileInfo ^fi in flist)
                    {
                        String ^newName = fi->Name->Replace(basename,toname);//toname + fi->Extension;
                        if (Settings->RenameTxtToSub)
                        {
                            if (newName->EndsWith(".txt"))
                                newName = newName->Substring(0, newName->Length -4) + ".sub";
                        }

                        RCItem ^newrci = gcnew RCItem(ri->ShowName, ri->Operation, ri->FromFolder,
                            fi->Name, ri->ToFolder, newName, ri->TheEpisode);

                        // check this item isn't already in our to-do list
                        bool doNotAdd = false;
                        for each (RCItem ^ri2 in rcl)
                        {
                            if (ri->SameSource(newrci))
                            {
                                doNotAdd = true;
                                break;
                            }
                        }


                            if (!doNotAdd)
                            {

                                if (!newrci->SameAs(ri)) // don't re-add ourself
                                    extras->Add(newrci);
                            }
                    }
                }
                catch (System::IO::PathTooLongException ^e)
                {
                    String ^t = "Path or filename too long. " + ri->FullFromName() + ", " + e->Message;
                    ::DialogResult dr = MessageBox::Show(t,"Path or filename too long", MessageBoxButtons::OK, MessageBoxIcon::Exclamation);
                }
            }

            for each (RCItem ^ri in extras)
            {
                // check we don't already have this in our list and, if we don't add it!
                bool have = false;
                for each (RCItem ^ri2 in rcl)
                {
                    if (ri2->SameAs(ri))
                    {
                        have = true;
                        break;
                    }
                }

                if (!have)
                    rcl->Add(ri);
            }
        }

        void LookForMissingEps(System::Windows::Forms::ProgressBar ^pbProgress, bool leaveOriginals)
        {
            // for each ep we have noticed as being missing
            // look through the monitored folders for it

            Stats()->FindAndOrganisesDone++;

            CopyMoveList->Clear();

            int totalN = MissingEpisodes->Count + SearchFolders->Count;
            int c = 0;

            FileList ^files = gcnew FileList;
            for (int k=0;k<SearchFolders->Count;k++)
            {
                pbProgress->Value = 100*(++c) / (totalN+1);
                BuildDirCache(files, SearchFolders[k], true);
            }

            for (int j=0;j<MissingEpisodes->Count;j++)
            {
                pbProgress->Value = 100*(++c) / (totalN+1);
                FindMissingEp(files, MissingEpisodes[j]);
            }

            if (Settings->KeepTogether)
                KeepTogether(CopyMoveList);

            pbProgress->Value = 0;

            if (!leaveOriginals)
            {
                // go through and change last of each operation on a given source file to a 'Move'
                // ideally do that move within same filesystem

                // sort based on source file, and destination drive, putting last if destdrive == sourcedrive
                SortSmartly(CopyMoveList);

                // then put last of each source file to be a move
                for (int i=0;i<CopyMoveList->Count;i++)
                {
                    if ( (i == (CopyMoveList->Count-1)) || (CopyMoveList[i]->FullFromName() != CopyMoveList[i+1]->FullFromName()))
                        CopyMoveList[i]->Operation = rcMove;
                }
            }

            //if (Settings->KeepTogether && Settings->RenameTxtToSub)
            //{
            //    for each (RCItem ^rci in CopyMoveList)
            //    {
            //        if (rci->FromName->EndsWith(".txt"))
            //            rci->ToName = rci->ToName->Substring(0, rci->ToName->Length -4) + ".sub";
            //    }

            //}
        }


        // -----------------------------------------------------------------------------

        void Downloader()
        {
            // do background downloads of webpages

            try
            {
                if (ShowItems->Count == 0)
                {
                    DownloadDone = true;
                    DownloadOK = true;
                    return;
                }

                if (!GetTVDB(false,"")->GetUpdates())
                {
                    DownloadDone = true;
                    DownloadOK = false;
                    return;
                }

                // for eachs of the ShowItems, make sure we've got downloaded data for it

                int n2 = ShowItems->Count;
                int n = 0;
                Generic::List<int> ^codes = gcnew Generic::List<int>;
                LockShowItems();
                for each (ShowItem ^si in ShowItems)
                    codes->Add(si->TVDBCode);
                UnlockShowItems();

                for each (int code in codes)
                {
                    DownloadPct = 100*(n+1)/(n2+1);
                    DownloadsRemaining = n2-n;
                    n++;

                    bool r = GetTVDB(false,"")->EnsureUpdated(code);

                    if (!r)
                    {
                        DownloadOK = false;
                        if (DownloadStopOnError)
                            DownloadDone = true;
                        return;
                    }
                }

                GetTVDB(false,"")->UpdatesDoneOK();
                DownloadDone = true;
                DownloadOK = true;
                return;
            }
            catch (ThreadAbortException ^)
            {
                DownloadDone = true;
                DownloadOK = false;
                return;
            }

            DownloadOK = false;
            DownloadDone = true;
            return;
        } 


        void StartBGDownloadThread(bool stopOnError)
        {
            if (!DownloadDone)
                return;
            DownloadStopOnError = stopOnError;
            DownloadPct = 0;
            DownloadDone = false;
            DownloadOK = true;
            mDownloaderThread = gcnew System::Threading::Thread(gcnew System::Threading::ThreadStart(this, &TVDoc::Downloader));
            mDownloaderThread->Name = "Downloader";
            mDownloaderThread->Start();
        }

        void StopBGDownloadThread()
        {
            if (mDownloaderThread != nullptr)
            {
                mDownloaderThread->Abort();
                mDownloaderThread = nullptr;
            }
        }

        bool DoDownloadsFG(System::Windows::Forms::ProgressBar ^pbProgress)
        {
            if (Settings->OfflineMode)
            	return true; // don't do internet in offline mode!

            pbProgress->Value = 0;
            
            StartBGDownloadThread(true);

            int delay1 = 50; // half a second
            while ((delay1--) && (!DownloadDone))
                Threading::Thread::Sleep(10);

            if (!DownloadDone) // downloading still going on, so time to show the dialog
            {
                DownloadProgress ^dp = gcnew DownloadProgress(this);
                ::DialogResult dr = dp->ShowDialog();
            }

            while (!DownloadDone)
                Threading::Thread::Sleep(100);

            GetTVDB(false,"")->SaveCache();

            GenDict(false,false);

            if (!DownloadOK)
            {
                MessageBox::Show(mTVDB->LastError, "Error while downloading", MessageBoxButtons::OK, MessageBoxIcon::Error);
                mTVDB->LastError = "";
            }

            return DownloadOK;
        }

        bool GenDict(bool cachedOnly, bool suppressErrors)
        {
            bool res = true;
            LockShowItems();
            for each (ShowItem ^si in ShowItems)
                if (!GenerateEpisodeDict(si, cachedOnly, suppressErrors))
                    res = false;
            UnlockShowItems();
            return res;
        }

        Searchers ^GetSearchers()
        {
            return Settings->TheSearchers;
        }

        void TidyTVDB()
        {
            // remove any shows from thetvdb that aren't in My Shows
            TheTVDB ^db = GetTVDB(true, "TidyTVDB");
            Generic::List<int> ^removeList = gcnew Generic::List<int>;

            for each (KeyValuePair<int, SeriesInfo^> ^kvp in mTVDB->GetSeriesDict())
            {
                bool found = false;
                for each (ShowItem ^si in ShowItems)
                {
                    if (si->TVDBCode == kvp->Key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    removeList->Add(kvp->Key);
            }

            for each (int i in removeList)
                db->ForgetShow(i, false);

            db->Unlock("TheTVDB");
            db->SaveCache();
        }
        void Closing()
        {
            StopBGDownloadThread();
            Stats()->Save();
        }

        void DoBTSearch(ProcessedEpisode ^ep)
        {
            TVDoc::SysOpen(Settings->BTSearchURL(ep));
        }

        void DoWhenToWatch(System::Windows::Forms::ProgressBar ^pbProgress, bool cachedOnly, bool suppressErrors)
        {
            if (!cachedOnly && !DoDownloadsFG(pbProgress))
            {
                return;
            }
            if (cachedOnly)
                GenDict(true, suppressErrors);
            /*
            int c = 0;
            int totalN = ShowItems->Count;
            for each (ShowItem ^si in ShowItems)
            {
            pbProgress->Value = 100*(c+1) / (totalN+1);

            GenerateEpisodeDict(si, cachedOnly, suppressErrors);
            c++;
            }
            // SaveCacheState();
            pbProgress->Value = 0;*/
        }

        FileList ^FindEpOnDisk(ProcessedEpisode ^pe)
        {
            return FindEpOnDisk(pe->SI, pe);
        }
        FileList ^FindEpOnDisk(ShowItem ^si, Episode ^epi)
        {
            FileList ^ret = gcnew FileList;

            int seasWanted = epi->TheSeason->SeasonNumber;
            int epWanted = epi->EpNum;

            int snum = epi->TheSeason->SeasonNumber;

            if (!si->AllFolderLocations(Settings)->ContainsKey(snum))
                return ret;

            for each (String ^folder in si->AllFolderLocations(Settings)[snum])
            {
                DirectoryInfo^ di;
                try
                {
                    di = gcnew DirectoryInfo(folder);
                }
                catch (...)
                {
                    return ret;
                }
                if (!di->Exists)
                    return ret;

                array<FileInfo^>^files = di->GetFiles();
                for each (FileInfo^ fiTemp in files)
                {
                    int seasFound, epFound;

                    if (!Settings->UsefulExtension(fiTemp->Extension))
                        continue; // move on

                    if (FindSeasEp(fiTemp, &seasFound, &epFound, si->ShowName()))
                    {
                        if (seasFound == -1)
                            seasFound = seasWanted;
                        if ((seasFound == seasWanted) && (epFound == epWanted))
                            ret->Add(fiTemp);
                    }
                }
            }

            return ret;
        }

        bool HasAnyAirdates(ShowItem ^si, int snum)
        {
            bool r = false;
            TheTVDB ^db = GetTVDB(false,"");
            SeriesInfo ^ser = db->GetSeries(si->TVDBCode);
            if ((ser != nullptr) && (ser->Seasons->ContainsKey(snum)))
            {
                for each (Episode ^e in ser->Seasons[snum]->Episodes)
                    if (e->FirstAired != nullptr)
                    {
                        r = true;
                        break;
                    }
            }
            return r;
        }

        bool UpToDateCheck(ShowItem ^si) 
        {
            int maxEpNum = 0;
            int missingCount = -1;

            if (!si->DoMissingCheck) 
                return true;
        
            FolderLocationDict ^flocs = si->AllFolderLocations(Settings);

            for each (int snum in si->SeasonEpisodes->Keys)
            {
            //for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in si->SeasonEpisodes)
            //{

                if (si->IgnoreSeasons->Contains(snum/*kvp->Key*/))
                    continue; // ignore this season

                //int snum = kvp->Key;
                if ((snum == 0) && (si->CountSpecials)) 
                    continue;  // no specials season, they're merged into the seasons themselves

                StringList ^folders = gcnew StringList();

                if (flocs->ContainsKey(snum))
                    folders = flocs[snum];

                if (folders->Count == 0)
                {
                    // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                    folders->Add(si->AutoFolderNameForSeason(snum, Settings));
                }

                for each (String ^folder in folders)
                {
                    // generate new filename info
                    bool goAgain = false;
                    bool nextSeas = false;
                    DirectoryInfo^ di = nullptr;
                    do
                    {
                        goAgain = false;
                        if (folder != "")
                        {
                            try
                            {
                                di = gcnew DirectoryInfo(folder);
                            }
                            catch (...)
                            {
                                return false;
                            }
                        }
                        if ((di == nullptr) || (!di->Exists))
                        {
                            String ^sn = si->ShowName();
                            String ^text = snum.ToString() + " of " + si->MaxSeason().ToString();
                            String ^theFolder = folder;

                            MissingFolderAction ^mfa = gcnew MissingFolderAction(sn, text, theFolder);
                            mfa->ShowDialog();
                            if (mfa->Result == kfaCancel)
                                return false;
                            else if (mfa->Result == kfaCreate)
                            {
                                Directory::CreateDirectory(folder);
                                goAgain = true;
                            }
                            else if (mfa->Result == kfaIgnoreAlways)
                            {
                                nextSeas = true;
                                si->IgnoreSeasons->Add(snum);
                                SetDirty();
                                break;
                            }
                            else if (mfa->Result == kfaIgnoreOnce)
                            {
                                nextSeas = true;
                                break;
                            }
                            else if (mfa->Result == kfaRetry)
                                goAgain = true;
                            else if (mfa->Result == kfaDifferentFolder)
                            {
                                folder = mfa->FolderName;
                                di = gcnew DirectoryInfo(folder);
                                goAgain = !di->Exists;
                                if (di->Exists && (si->AutoFolderNameForSeason(snum, Settings)->ToLower() != folder->ToLower()))
                                {
                                    if (!si->ManualFolderLocations->ContainsKey(snum))
                                        si->ManualFolderLocations[snum] = gcnew StringList();
                                    si->ManualFolderLocations[snum]->Add(folder);
                                }

                            }
                        }
                    } while (goAgain);

                    if (nextSeas || (di == nullptr))
                        continue;

                    array<FileInfo^>^fi = di->GetFiles();

                    // make up a list of all the epsiodes in this season that we have locally
                    Generic::List<bool> ^localEps = gcnew Generic::List<bool>;

                    ProcessedEpisodeList ^eps = si->SeasonEpisodes[snum];

                    for each (FileInfo^ fiTemp in fi)
                    {
                        int seas, ep;

                        if (!Settings->UsefulExtension(fiTemp->Extension))
                            continue; // move on

                        if (FindSeasEp(fiTemp, &seas, &ep, si->ShowName()))
                        {
                            if (seas == -1)
                                seas = snum;
                            if (seas == snum)
                            {
                                while (ep >= localEps->Count)
                                    localEps->Add(false);

                                localEps[ep] = true;
                                if (ep > maxEpNum)
                                    maxEpNum = ep;
                            }
                        }
                    }

                    // now look at EPInfos and see if we're up to date or not
                    DateTime ^today = DateTime::Now;
                    missingCount = 0;

                    TheTVDB ^db = GetTVDB(true,"UpToDateCheck");
                    for each (ProcessedEpisode ^pe in eps)
                    {
                        bool ok = true;
                        int n = pe->EpNum;
                        if ((n >= localEps->Count) || (!localEps[n])) // not here locally
                        {
                            DateTime ^dt = pe->GetAirDateDT(true);

                            bool notFuture = (dt != nullptr) && (dt->CompareTo(today) < 0); // isn't an episode yet to be aired
                            bool anyAirdates = HasAnyAirdates(si, snum);
                            bool lastSeasAirdates = (snum > 1) ? HasAnyAirdates(si, snum-1) : true; // this might be a new season, so check the last one as well
                            if (si->ForceCheckAll || // we're told to look for everything, or
                                ( (snum == 0) && (dt == nullptr) ) ||  // its a special without an airdate, or
                                (!(anyAirdates || lastSeasAirdates)) || // there are no airdates for this or last season, or
                                notFuture) // not in the future (i.e. its aired)
                            {
                                // then add it as officially missing
                                SeriesInfo ^ser = db->GetSeries(si->TVDBCode);
                                Diagnostics::Debug::Assert(ser != nullptr);
                                String ^name = folder;
                                if (!name->EndsWith("\\"))
                                    name += "\\";
                                name += FilenameFriendly(Settings->NamingStyle->NameFor(pe));
                                MissingEpisode ^mi = gcnew MissingEpisode(si, snum, ser, pe, folder, name);
                                MissingEpisodes->Add(mi);
                                missingCount++;
                                ok = false;
                            }
                        }
                        if (ok)
                            mStats->NS_NumberOfEpisodes++;
                    }
                    db->Unlock("UpToDateCheck");

                    delete[] localEps;
                } // for each folder
            } // for each seas

            return true;
        }

        //void DelThumb(FolderItem ^fi)
        //{
        // TODO: banners 'n stuff instead

        //String ^fn = fi->Folder+"\\folder.jpg";
        //try {
        //    File::Delete(fn);
        //} catch (...) { }

        //}

        //bool SaveThumbnail(FolderItem ^fi,bool forceUpdate)
        //{
        // TODO

        //if (fi == nullptr)
        //	return false;

        //// find ShowItem for that folderitem, use Downloadpage from ShowItem, etc.
        //String ^filename = fi->Folder+"\\folder.jpg";
        //if (!forceUpdate && File::Exists(filename))
        //	return true;

        //ShowItem ^si = fi->GetShowItem();
        //if (si == nullptr)
        //	return false;

        //String ^p = DownloadPage(si, true, false, false);
        //if (p == nullptr)
        //	return false;
        //Regex ^urlFinder = gcnew Regex("<img src=\"(.*/thumb/.*\\.jpg)\" alt=\".*\" />",RegexOptions::IgnoreCase);
        //Match ^mm = urlFinder->Match(p);

        //if (mm->Captures->Count == 0)
        //	return false;

        //p = urlFinder->Replace(mm->Captures[0]->ToString(), "$1");
        //array<unsigned char>^ theData;

        //System::Net::WebClient ^wc = gcnew System::Net::WebClient();
        //try 
        //{
        //	theData = wc->DownloadData(p);
        //}
        //catch (...)
        //{
        //	return false;
        //}


        //FileStream ^fs = gcnew FileStream(filename, FileMode::Create);
        //fs->Write(theData, 0, theData->Length);
        //fs->Close();
        //    return true;
        //}



        bool GenerateEpisodeDict(ShowItem ^si, bool cachedOnly, bool suppressErrors)
        {
            si->SeasonEpisodes->Clear();

            // copy data from tvdb
            // process as per rules
            // done!

            TheTVDB ^db = GetTVDB(true,"GenerateEpisodeDict");

            SeriesInfo ^ser = db->GetSeries(si->TVDBCode);

            if (ser == nullptr)
                return false; // TODO: warn user

            bool r = true;
            for each (KeyValuePair<int, Season ^> ^kvp in ser->Seasons)
            {
                ProcessedEpisodeList ^pel = GenerateEpisodes(si, ser, kvp->Key, suppressErrors, true);
                si->SeasonEpisodes[kvp->Key] = pel;
                if (pel == nullptr)
                    r = false;
            }

            Generic::List<int> ^theKeys = gcnew Generic::List<int>;
            // now, go through and number them all sequentially
            for each (int snum in ser->Seasons->Keys)
                theKeys->Add(snum);

            theKeys->Sort();

            int overallCount = 1;
            for each (int snum in theKeys)
            {
                if (snum != 0)
                {
                    for each (ProcessedEpisode ^pe in si->SeasonEpisodes[snum])
                    {
                        pe->OverallNumber = overallCount;
                        overallCount += 1 + pe->EpNum2 - pe->EpNum;
                    }
                }
            }

            db->Unlock("GenerateEpisodeDict");

            return r;
        }

        static ProcessedEpisodeList ^GenerateEpisodes(ShowItem ^si, SeriesInfo ^ser, int snum, bool supressErrors, bool applyRules)
        {
            ProcessedEpisodeList ^eis = gcnew ProcessedEpisodeList();

            if ((ser == nullptr) || !ser->Seasons->ContainsKey(snum))
                return nullptr; // todo.. something?

            Season ^seas = ser->Seasons[snum];

            if (seas == nullptr)
                return nullptr; // TODO: warn user

            for each (Episode ^e in seas->Episodes)
                eis->Add(gcnew ProcessedEpisode(e, si)); // add a copy

            if (si->DVDOrder)
            {
                eis->Sort(gcnew System::Comparison<ProcessedEpisode ^>(ProcessedEpisode::DVDOrderSorter));
                Renumber(eis);
            }
            else
                eis->Sort(gcnew System::Comparison<ProcessedEpisode ^>(ProcessedEpisode::EPNumberSorter));

            if (si->CountSpecials && ser->Seasons->ContainsKey(0))
            {
                // merge specials in
                for each (Episode ^ep in ser->Seasons[0]->Episodes)
                {
                    if (ep->Items->ContainsKey("airsbefore_season") && ep->Items->ContainsKey("airsbefore_episode"))
                    {
                        String ^seasstr = ep->Items["airsbefore_season"];
                        String ^epstr = ep->Items["airsbefore_episode"];
                        if ((seasstr == "") || (epstr == ""))
                            continue;
                        int seas = int::Parse(seasstr);
                        if (seas != snum)
                            continue;
                        int epnum = int::Parse(epstr);
                        for (int i=0;i<eis->Count;i++)
                            if ((eis[i]->SeasonNumber == seas) && (eis[i]->EpNum == epnum))
                            {
                                ProcessedEpisode ^pe = gcnew ProcessedEpisode(ep, si);
                                pe->TheSeason = eis[i]->TheSeason;
                                pe->SeasonID = eis[i]->SeasonID;
                                int c = eis[i]->EpNum;
                                eis->Insert(i, pe);
                                break;
                            }
                    }
                }
                // renumber to allow for specials
                int epnum = 1;
                for (int j=0;j<eis->Count;j++)
                {
                    eis[j]->EpNum2 = epnum + (eis[j]->EpNum2 - eis[j]->EpNum);
                    eis[j]->EpNum = epnum;
                    epnum++;
                }
            }

            if (applyRules)
            {
                RuleSet ^rules = si->RulesForSeason(snum);
                if (rules != nullptr)
                    ApplyRules(eis, rules, si);
            }

            return eis;
        }


        static void ApplyRules(ProcessedEpisodeList ^eis, RuleSet ^rules, ShowItem ^si)
        {
            for each (ShowRule ^sr in rules)
            {
                int nn1 = sr->First;
                int nn2 = sr->Second;

                int n1 = -1;
                int n2 = -1;
                // turn nn1 and nn2 from ep number into position in array
                for (int i=0;i<eis->Count;i++)
                {
                    if (eis[i]->EpNum == nn1)
                    {
                        n1 = i;
                        break;
                    }
                }
                for (int i=0;i<eis->Count;i++)
                {
                    if (eis[i]->EpNum == nn2)
                    {
                        n2 = i;
                        break;
                    }
                }

                String ^txt = sr->UserSuppliedText;
                int ec = eis->Count;

                switch (sr->DoWhatNow)
                {
                case kRename:
                    {
                        if ((n1 < ec) && (n1 >= 0))
                            eis[n1]->Name = txt;
                        break;
                    }
                case kRemove:
                    {
                        if ((n1 < ec) && (n1 >= 0) &&
                            (n2 < ec) && (n2 >= 0) )
                            eis->RemoveRange(n1,n2-n1);
                        else if ((n1 < ec) && (n1 >= 0) && (n2 == -1))
                            eis->RemoveAt(n1);
                        break;
                    }
                case kIgnoreEp:
                    {
                        if (n2 == -1)
                            n2 = n1;
                        for (int i=n1;i<=n2;i++)
                        if ((i < ec) && (i >= 0))
                            eis[i]->Ignore = true;
                        break;
                    }
                case kSplit:
                    {
                        // split one episode into a multi-parter
                        if ((n1 < ec) && (n1 >= 0))
                        {
                            ProcessedEpisode ^ei = eis[n1];
                            String ^nameBase = ei->Name;
                            eis->RemoveAt(n1); // remove old one
                            for (int i=0;i<nn2;i++) // make n2 new parts
                            {
                                ProcessedEpisode ^pe2 = gcnew ProcessedEpisode(ei, si);
                                pe2->Name = nameBase + " (Part " + (i+1).ToString() + ")";
                                pe2->EpNum = 0;
                                pe2->EpNum2 = 0;
                                eis->Insert(n1+i, pe2);
                            }
                        }
                        break;
                    }
                case kMerge:
                case kCollapse:
                    {
                        if ((n1 != -1) && (n2 != -1) && (n1 < ec) && (n2 < ec) && (n1 < n2))
                        {
                            ProcessedEpisode ^oldFirstEI = eis[n1];
                            String ^combinedName = eis[n1]->Name + " + " ;
                            String ^combinedSummary = eis[n1]->Overview + "<br/><br/>";
                            //int firstNum = eis[n1]->TVcomEpCount();
                            for (int i=n1+1;i<=n2;i++)
                            {
                                combinedName += eis[i]->Name;
                                combinedSummary += eis[i]->Overview;
                                if (i != n2)
                                {
                                    combinedName += " + ";
                                    combinedSummary += "<br/><br/>";
                                }
                            }

                            eis->RemoveRange(n1,n2-n1);

                            eis->RemoveAt(n1);

                            ProcessedEpisode ^pe2 = gcnew ProcessedEpisode(oldFirstEI, si);
                            pe2->Name = ((txt == "") ? combinedName : txt);
                            pe2->EpNum = 0;
                            if (sr->DoWhatNow == kMerge)
                                pe2->EpNum2 = 0 + n2-n1;
                            else
                                pe2->EpNum2 = 0;

                            pe2->Overview = combinedSummary;
                            eis->Insert(n1, pe2);
                        }
                        break;
                    }
                case kSwap:
                    {
                        if ((n1 != -1) && (n2 != -1) && (n1 < ec) && (n2 < ec))
                        {
                            ProcessedEpisode ^t = eis[n1];
                            eis[n1] = eis[n2];
                            eis[n2] = t;
                        }
                        break;
                    }
                case kInsert:
                    {
                        if ((n1 < ec) && (n1 >= 0))
                        {
                            ProcessedEpisode ^t = eis[n1];
                            ProcessedEpisode ^n = gcnew ProcessedEpisode(t->TheSeries, t->TheSeason, si);
                            n->Name = txt;
                            n->EpNum = 0;
                            n->EpNum2 = 0;
                            eis->Insert(n1, n);
                            break;
                        }
                    }
                } // switch DoWhatNow

                Renumber(eis);
            } // for each rule
            // now, go through and remove the ignored ones (but don't renumber!!)
            for (int i=eis->Count-1;i>=0;i--)
            {
                if (eis[i]->Ignore)
                    eis->RemoveAt(i);
            }
        }


        static void Renumber(ProcessedEpisodeList ^eis)
        {
            // renumber 
            // pay attention to specials etc.
            int n = 1;
            for (int i=0;i<eis->Count;i++)
                if (eis[i]->EpNum != -1) // is -1 if its a special or other ignored ep
                {
                    int num = eis[i]->EpNum2 - eis[i]->EpNum;
                    eis[i]->EpNum = n;
                    eis[i]->EpNum2 = n+num;
                    n += num+1;
                }
        }

        static void GuessShowName(AddItem ^ai)
        {
            // see if we can guess a season number and show name, too
            // Assume is blah\blah\blah\show\season X
            int seasNum = -1;
            String ^showName = "";

            String ^sp = ai->Folder;
            String ^seasonFinder = ".*season[ _\\.]+([0-9]+).*";
            if (Regex::Matches(sp,seasonFinder,RegexOptions::IgnoreCase)->Count)
            {
                String ^s = Regex::Replace(sp, seasonFinder, "$1",RegexOptions::IgnoreCase);
                try
                {
                    seasNum = Convert::ToInt32(s);
                    // remove season folder from end of the path
                    sp = Regex::Replace(sp, "(.*)\\\\"+seasonFinder,"$1",RegexOptions::IgnoreCase);
                }
                catch (...)
                {
                }
            }
            // assume last folder element is the show name
            showName = sp->Substring(sp->LastIndexOf("\\")+1);

            /*
            for each (ShowItem ^si in ShowItems)
            {
            if ( (si->ShowName() == showName) && 
            ((seasNum == -1) || (si->SeasonNumber == seasNum )))
            return si;
            }*/

            ai->ShowName = showName;
            //ai->Season = seasNum;
        }

        ProcessedEpisodeList ^NextNShows(int nshows, int ndays)
        {
            DateTime ^notBefore = DateTime::Now;
            ProcessedEpisodeList ^found = gcnew ProcessedEpisodeList();

            LockShowItems();
            for (int i=0;i<nshows;i++)
            {
                ProcessedEpisode ^nextAfterThat = nullptr;
                TimeSpan ^howClose = nullptr;
                for each (ShowItem ^si in GetShowItems(false))
                {
                    if (!si->ShowNextAirdate)
                        continue;
                    for each (KeyValuePair<int, ProcessedEpisodeList ^> ^v in si->SeasonEpisodes)
                    {
                        if (si->IgnoreSeasons->Contains(v->Key))
                            continue; // ignore this season

                        for each (ProcessedEpisode ^ei in v->Value)
                        {
                            if (found->Contains(ei))
                                continue;

                            DateTime ^dt = ei->GetAirDateDT(true);
                            if ((dt == nullptr) || (dt == DateTime::MaxValue))
                                continue;

                            TimeSpan ^ts = (dt->Subtract(*notBefore));
                            TimeSpan ^timeUntil = dt->Subtract(DateTime::Now);
                            if (((howClose == nullptr) || (ts->CompareTo(howClose) <= 0) && (ts->TotalHours >= 0)) &&
                                (ts->TotalHours >= 0) && (timeUntil->TotalDays <= ndays))
                            {
                                howClose = ts;
                                nextAfterThat = ei;
                            }
                        }
                    }
                }
                if (nextAfterThat == nullptr)
                    return found;
                notBefore = nextAfterThat->GetAirDateDT(true);
                found->Add(nextAfterThat);
            }
                UnlockShowItems();

            return found;
        }

        void WriteStringsToXml(StringList ^strings, XmlWriter ^writer, String ^elementName, String ^stringName)
        {
            writer->WriteStartElement(elementName);
            for each (String ^ss in strings)
            {
                writer->WriteStartElement(stringName);
                writer->WriteValue(ss);
                writer->WriteEndElement();
            }
            writer->WriteEndElement();
        }



        void WriteXMLSettings()
        {
            // backup old settings before writing new ones
            String ^filenameBase = System::Windows::Forms::Application::UserAppDataPath+"\\TVRenameSettings.xml";

            //TVDB->SaveDataXML();

            if (File::Exists(filenameBase))
            {
                for (int i=8;i>=0;i--)
                {
                    String ^fn = filenameBase + "." + i.ToString();
                    if (File::Exists(fn))
                    {
                        String ^fn2 = filenameBase + "." + (i+1).ToString();
                        if (File::Exists(fn2))
                            File::Delete(fn2);
                        File::Move(fn, fn2);
                    }
                }

                File::Copy(filenameBase, filenameBase+".0");
            }

            XmlWriterSettings ^settings = gcnew XmlWriterSettings();
            settings->Indent = true;
            settings->NewLineOnAttributes = true;
            XmlWriter ^writer = XmlWriter::Create(filenameBase, settings);

            writer->WriteStartDocument();
            writer->WriteStartElement("TVRename");
            writer->WriteStartAttribute("Version");
            writer->WriteValue("2.1");
            writer->WriteEndAttribute(); // version

            Settings->WriteXML(writer); // <Settings>

            writer->WriteStartElement("MyShows");
            for each (ShowItem ^si in ShowItems)
                si->WriteXMLSettings(writer);
            writer->WriteEndElement(); // myshows

            WriteStringsToXml(MonitorFolders, writer, "MonitorFolders", "Folder");
            WriteStringsToXml(IgnoreFolders, writer, "IgnoreFolders", "Folder");
            WriteStringsToXml(SearchFolders, writer, "FinderSearchFolders", "Folder");

            writer->WriteEndElement(); // tvrename
            writer->WriteEndDocument();
            writer->Close();

            mDirty = false;

            Stats()->Save();

        }

        StringList ^ReadStringsFromXml(XmlReader ^reader, String ^elementName, String ^stringName)
        {
            StringList ^r = gcnew StringList();

            if (reader->Name != elementName)
                return r; // uhoh

            if (!reader->IsEmptyElement)
            {

                reader->Read();
                while (!reader->EOF)
                {
                    if ((reader->Name == elementName) && !reader->IsStartElement())
                        break;
                    if (reader->Name == stringName)
                        r->Add(reader->ReadElementContentAsString());
                    else
                        reader->ReadOuterXml();
                }
            }
            reader->Read();
            return r;
        }

        bool LoadXMLSettings()
        {
            bool ok = true;
            XmlReaderSettings ^settings = gcnew XmlReaderSettings();
            settings->IgnoreComments = true;
            settings->IgnoreWhitespace = true;

            String ^f = System::Windows::Forms::Application::UserAppDataPath+"\\TVRenameSettings.xml";

            if (!FileInfo(f).Exists)
                return false;

            XmlReader ^reader = XmlReader::Create(f, settings);

            reader->Read();
            if (reader->Name != "xml")
                return false;

            reader->Read();

            if (reader->Name != "TVRename")
                return false;

            if (reader->GetAttribute("Version") != "2.1")
                return false;

            reader->Read(); // move forward one

            while (!reader->EOF)
            {
                if (reader->Name == "TVRename" && !reader->IsStartElement())
                    break; // end of it all

                if (reader->Name == "Settings")
                {
                    Settings = gcnew TVSettings(reader->ReadSubtree());
                    reader->Read();
                }
                else if (reader->Name == "MyShows")
                {
                    XmlReader ^r2 = reader->ReadSubtree();
                    r2->Read();
                    r2->Read();
                    while (!r2->EOF)
                    {
                        if ((r2->Name == "MyShows") && (!r2->IsStartElement()))
                            break;
                        if (r2->Name == "ShowItem")
                        {
                            ShowItem ^si = gcnew ShowItem(mTVDB, r2->ReadSubtree(), this->Settings);
                            
                            if (si->UseCustomShowName) // see if custom show name is actually the real show name
                            {
                                SeriesInfo ^ser = si->TheSeries();
                                if ((ser != nullptr) && (si->CustomShowName == ser->Name))
                                    {
                                        // then, turn it off
                                        si->CustomShowName = "";
                                        si->UseCustomShowName = false;
                                    }
                            }
                            ShowItems->Add(si);

                            r2->Read();
                        }
                        else 
                            r2->ReadOuterXml();
                    }
                    reader->Read();
                }
                else if (reader->Name == "MonitorFolders")
                    MonitorFolders = ReadStringsFromXml(reader, "MonitorFolders", "Folder");
                else if (reader->Name == "IgnoreFolders")
                    IgnoreFolders = ReadStringsFromXml(reader, "IgnoreFolders", "Folder");
                else if (reader->Name == "FinderSearchFolders")
                    SearchFolders = ReadStringsFromXml(reader, "FinderSearchFolders", "Folder");
                else
                    reader->ReadOuterXml();
            }

            reader->Close();

            Stats()->Load();

            return true;
        }


        static bool SysOpen(String ^what)
        {
            try
            {
                System::Diagnostics::Process::Start(what);
                return true;
            }
            catch (...)
            {
                return false;
            }
        }
        void SaveMissingListCSV(String ^filename)
        {
            TextWriter ^f = gcnew StreamWriter(filename);
            String ^line;
            line = "Show Name,Season,Episode,Episode Name,Air Date,Folder,Nice Name,thetvdb.com Code";
            f->WriteLine(line);
            for each (MissingEpisode ^me in MissingEpisodes)
            {
                String ^line = "\"" + me->TheSeries->Name + "\"" + "," + 
                    me->SeasonNumber + "," + me->EpNum + 
                    ((me->EpNum != me->EpNum2) ? "-"+me->EpNum2 : "") +
                    "," +
                    "\"" + me->Name + "\"" + "," +
                    me->GetAirDateDT(true)->ToString("G") + "," +
                    "\"" + me->WhereItBelongs + "\"" + "," +
                    "\"" + FilenameFriendly(Settings->NamingStyle->NameFor(me)) + "\"" + "," +
                    me->SeriesID;
                //(NStyle::Style)me->SI->NamingStyle

                f->WriteLine(line);
            }
            f->Close();
        }
        void SaveMissingListXML(String ^filename)
        {
            MessageBox::Show("Sorry, this hasn't been implemented yet.","Not available");
            return;

            /*
            XmlWriterSettings ^settings = gcnew XmlWriterSettings();
            settings->Indent = true;
            settings->NewLineOnAttributes = true;
            XmlWriter ^writer = XmlWriter::Create(filename, settings);

            writer->WriteStartDocument();
            writer->WriteStartElement("TVRename");
            writer->WriteStartAttribute("Version");
            writer->WriteValue("2.0");
            writer->WriteEndAttribute(); // version
            writer->WriteStartElement("MissingItems");

            for each (MissingEpisode ^me in MissingEpisodes)
            {
            writer->WriteStartElement("MissingItem");
            writer->WriteStartElement("ShowName");
            writer->WriteValue(me->GetEPInfo()->GetShowItem()->GetShowName());
            writer->WriteEndElement();
            writer->WriteStartElement("Season");
            writer->WriteValue(me->GetEPInfo()->GetShowItem()->GetSeasonNumber());
            writer->WriteEndElement();
            writer->WriteStartElement("Episode");
            String ^epl = me->GetEPInfo()->GetEpisode1().ToString();
            if (me->GetEPInfo()->GetEpisode1() != me->GetEPInfo()->GetEpisode2())
            epl += "-"+me->GetEPInfo()->GetEpisode2().ToString();
            writer->WriteValue(epl);
            writer->WriteEndElement();
            writer->WriteStartElement("EpisodeName");
            writer->WriteValue(me->GetEPInfo()->GetName());
            writer->WriteEndElement();
            writer->WriteStartElement("AirDate");
            writer->WriteValue(me->GetEPInfo()->GetAirDateDT());
            writer->WriteEndElement();
            writer->WriteStartElement("Folder");
            writer->WriteValue(me->GetFolderItem()->Folder);
            writer->WriteEndElement();
            writer->WriteStartElement("NiceName");
            writer->WriteValue(me->GetEPInfo()->NameInStyle(me->GetFolderItem()->GetNamingStyle(), true, Settings));
            writer->WriteEndElement();
            writer->WriteStartElement("tv.comcode");
            writer->WriteValue( me->GetFolderItem()->GetTVcomCode());
            writer->WriteEndElement();
            writer->WriteEndElement(); // MissingItem
            }

            writer->WriteEndElement(); // MissingItems
            writer->WriteEndElement(); // tvrename
            writer->WriteEndDocument();
            writer->Close();
            */
        }

        bool GenerateUpcomingXML(MemoryStream ^str, ProcessedEpisodeList ^elist)
        {
            if (elist == nullptr)
                return false;

            try 
            {
                XmlWriterSettings ^settings = gcnew XmlWriterSettings();
                settings->Indent = true;
                settings->NewLineOnAttributes = true;
                settings->Encoding = Encoding::ASCII;
                XmlWriter ^writer = XmlWriter::Create(str, settings);

                writer->WriteStartDocument();
                writer->WriteStartElement("rss");
                writer->WriteStartAttribute("version");
                writer->WriteValue("2.0");
                writer->WriteEndAttribute();
                writer->WriteStartElement("channel");
                writer->WriteStartElement("title");
                writer->WriteValue("Upcoming Shows");
                writer->WriteEndElement();
                writer->WriteStartElement("title");
                writer->WriteValue("http://tvrename.com");
                writer->WriteEndElement();
                writer->WriteStartElement("description");
                writer->WriteValue("Upcoming shows, exported by TVRename");
                writer->WriteEndElement();

                for each (ProcessedEpisode ^ei in elist)
                {
                    String ^niceName = Settings->NamingStyle->NameFor(ei);

                    writer->WriteStartElement("item");
                    writer->WriteStartElement("title");
                    writer->WriteValue(ei->HowLong()+" "+ei->DayOfWeek()+" "+ei->TimeOfDay()+" "+niceName);
                    writer->WriteEndElement();
                    writer->WriteStartElement("link");
                    writer->WriteValue(GetTVDB(false,"")->WebsiteURL(ei->TheSeries->TVDBCode, ei->SeasonID, false));
                    writer->WriteEndElement();
                    writer->WriteStartElement("description");
                    writer->WriteValue(niceName + "<br/>" + ei->Overview);
                    writer->WriteEndElement();
                    writer->WriteStartElement("pubDate");
                    DateTime ^DT = ei->GetAirDateDT(true);
                    if (DT != nullptr)
                        writer->WriteValue(DT->ToString("r"));
                    writer->WriteEndElement();
                    writer->WriteEndElement(); // item
                }
                writer->WriteEndElement();
                writer->WriteEndElement();
                writer->WriteEndDocument();
                writer->Close();
                return true;
            } // try
            catch (...)
            {
                return false;
            }
        }


        void WriteUpcomingRSS()
        {
            if (!Settings->ExportWTWRSS)
                return;

            try
            {
                // dirty try/catch to "fix" the problem that a share can disappear during a sleep/resume, and
                // when windows restarts, the share isn't "back" before this timer times out and fires
                // windows explorer tends to lose explorer windows on shares when slept/resumed, too, so its not
                // just me :P
                MemoryStream ^ms = gcnew MemoryStream();
                if (GenerateUpcomingXML(ms, NextNShows(Settings->ExportRSSMaxShows, Settings->ExportRSSMaxDays)))
                {
                    StreamWriter ^sw = gcnew StreamWriter(Settings->ExportWTWRSSTo);
                    sw->Write(System::Text::Encoding::UTF8->GetString(ms->ToArray()));
                    sw->Close();
                }
            }
            catch (...)
            {
            }

        }

        void MatchRSSToMissing()
        {
            RSSMissingList = gcnew RSSMissingItemList();

            for each (MissingEpisode ^me in MissingEpisodes)
            {
                for each (RSSItem ^rss in RSSList)
                {
                    if ( ((SimplifyName(rss->ShowName) == SimplifyName(me->TheSeries->Name)) ||
                          ((rss->ShowName == "") && (SimplifyName(rss->Title)->Contains(SimplifyName(me->TheSeries->Name))))) &&
                          (rss->Season == me->Season) && (rss->Episode == me->EpNum) )
                    {
                        RSSMissingList->Add(gcnew RSSMissingItem(rss, me));
                    }
                }
            }
        }

        void DownloadRSS(RSSMissingItem ^mi, String ^utorrentpath)
        {
            System::Net::WebClient ^wc = gcnew System::Net::WebClient();
            try {
                String ^URL = mi->RSS->URL;
                array<unsigned char> ^r = wc->DownloadData(URL);

                MemoryStream ^ms = gcnew MemoryStream(r);
                String ^saveTemp = Path::GetTempPath() + "\\" + FilenameFriendly(mi->RSS->Title);
                FileStream ^fs = File::Create(saveTemp);
                const int kBuffSize = 1024;
                    cli::array<unsigned char,1> ^buff = gcnew cli::array<unsigned char,1>(kBuffSize);
                for (;;)
                {
                    int n = ms->Read(buff, 0, kBuffSize);
                    if (n == 0)
                        break;
                    fs->Write(buff, 0, n);
                }
                fs->Close();
                ms->Close();
                System::Diagnostics::Process::Start(utorrentpath,"/directory \"" + mi->Episode->WhereItBelongs + "\" \"" + saveTemp + "\"");
            }
            catch (...)
            {
            }
        }
        
        static String ^SEFinderSimplifyFilename(String ^name, String ^showNameHint);
        bool MatchesSequentialNumber(String ^filename, int *seas, int *ep, ProcessedEpisode ^pe);
        bool FindSeasEp(FileInfo ^fi, int *seas, int *ep, String ^showNameHint);
        static bool FindSeasEp(FileInfo ^fi, int *seas, int *ep, String ^showNameHint, FNPRegexList ^rexps);
        static bool FindSeasEp(String ^directory, String ^filename, int *seas, int *ep, String ^showNameHint, FNPRegexList ^rexps);
    }; // class TVDoc
} // namespace

