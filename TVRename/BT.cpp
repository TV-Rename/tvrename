#include "stdafx.h"
#include "RenameItem.h"
#include "MissingEpisode.h"
#include "BT.h"
#include "TVDoc.h"

#pragma warning(disable:4800)

namespace TVRename
{
    enum { kPrioNormal = 8, kPrioSkip = 0x80 };

    BTItem ^BEncodeLoader::ReadString(FileStream ^sr, Int64 length)
    {
        BinaryReader ^br = gcnew BinaryReader(sr);

        array<unsigned char>^c = br->ReadBytes((int)length);

        BTString ^bts = gcnew BTString();
        bts->Data = c;
        return bts;

    }
    BTItem ^BEncodeLoader::ReadInt(FileStream ^sr)
    {
        Int64 r = 0;
        int c;
        bool neg = false;
        while ((c = sr->ReadByte())  != 'e')
        {
            if (c == '-')
                neg = true;
            else
                if ((c >= '0') && (c <= '9'))
                    r = (r*10) + c-'0';
        }
        if (neg)
            r = -r;

        BTInteger ^bti = gcnew BTInteger();
        bti->Value = r;
        return bti;
    }

    BTItem ^BEncodeLoader::ReadDictionary(FileStream ^sr)
    {
        BTDictionary ^d = gcnew BTDictionary();
        for (;;)
        {
            BTItem ^next = ReadNext(sr);
            if ((next->Type == kListOrDictionaryEnd) || (next->Type == kBTEOF))
                return d;

            if (next->Type != kString)
            {
                BTError ^e = gcnew BTError();
                e->Message = "Didn't get string as first of pair in dictionary";
                return e;
            }

            BTDictionaryItem ^di = gcnew BTDictionaryItem();
            di->Key = safe_cast<BTString ^>(next)->AsString();
            di->Data = ReadNext(sr);

            d->Items->Add(di);
        }
    }

    BTItem ^BEncodeLoader::ReadList(FileStream ^sr)
    {
        BTList ^ll = gcnew BTList();
        for (;;)
        {
            BTItem ^next = ReadNext(sr);
            if (next->Type == kListOrDictionaryEnd)
                return ll;

            ll->Items->Add(next);
        }
    }



    BTItem ^BEncodeLoader::ReadNext(FileStream ^sr)
    {
        if (sr->Length == sr->Position)
            return gcnew BTEOF();

        String ^t = NextThing(sr);

        if (t == "d")
        {
            return ReadDictionary(sr);
        } else if (t == "l")
        {
            return ReadList(sr);
        } else if (t == "e")
        {
            return gcnew BTListOrDictionaryEnd();
        }
        else if (t[0] == 's')
        {
            return ReadString(sr, Convert::ToInt32(t->Substring(1)));
        } else if (t[0] == 'i')
        {
            return ReadInt(sr);
        }
        else 
        {
            BTError ^e = gcnew BTError();
            e->Message = String::Concat("Error: unknown thing: ",t);
            return e;
        }
    }


    String ^BEncodeLoader::NextThing(FileStream ^sr)
    {
        int c = sr->ReadByte();
        if (c == 'd')
            return "d"; // dictionary
        if (c == 'l')
            return "l"; // list
        if (c == 'i')
            return "i"; // integer
        if (c == 'e')
            return "e"; // end of list/dictionary/etc.
        if ((c >= '0') && (c <= '9'))
        {
            String ^r = Convert::ToString(c-'0');
            while ((c = sr->ReadByte()) != ':')
                r += Convert::ToString(c-'0');
            return "s"+r;
        }
        return "?";
    }


    BTFile ^BEncodeLoader::Load(String ^filename)
    {
        BTFile ^f = gcnew BTFile();

        FileStream ^sr = nullptr;
        try 
        {
            sr = gcnew FileStream(filename,FileMode::Open,FileAccess::Read);
        } 
        catch (Exception ^e) 
        { 
            MessageBox::Show(e->Message, "TVRename Torrent Reader",MessageBoxButtons::OK,MessageBoxIcon::Exclamation);
            return nullptr;
        }

        while (sr->Position < sr->Length)
            f->Items->Add(ReadNext(sr));

        sr->Close();

        return f;
    }

    // --------------------------------------------------------------------------------

    FileInfo ^BTProcessor::FindLocalFileWithHashAt(array<unsigned char>^ findMe, Int32 whereInFile, int pieceSize, Int32 fileSize)
    {
        if (whereInFile < 0)
            return nullptr;

        for (int i=0;i<FileCache->Count;i++)
        {
            FileInfo ^fiTemp  = FileCache[i];
            Int64 flen = LengthCache[i];

            if ((flen != fileSize) || (flen < (whereInFile + pieceSize))) // this file is wrong size || too small
                continue;

            array<unsigned char>^ theHash = CheckCache(fiTemp->FullName, whereInFile, pieceSize, fileSize);
            if (theHash == nullptr)
            {
                // not cached, figure it out ourselves
                FileStream ^sr = nullptr;
                try {
                    sr = gcnew FileStream(fiTemp->FullName,FileMode::Open);
                } catch (...) { return nullptr; }

                array<unsigned char> ^thePiece = gcnew array<unsigned char>(pieceSize);
                sr->Seek(whereInFile, SeekOrigin::Begin);
                int n = sr->Read(thePiece, 0, pieceSize);
                sr->Close();

                System::Security::Cryptography::SHA1Managed ^sha1 = gcnew System::Security::Cryptography::SHA1Managed();

                theHash = sha1->ComputeHash(thePiece, 0, n);
                CacheThis(fiTemp->FullName, whereInFile, pieceSize, fileSize, theHash);
            }

            bool allGood = true;
            for (int j=0;j<20;j++)
                if (theHash[j] != findMe[j])
                {
                    allGood = false;
                    break;
                }
                if (allGood)
                    return fiTemp;
        } // while enum

        return nullptr;
    }

    void BTProcessor::AddToStatus(String ^s)
    {
        /*
        if (status == nullptr)
            return;

        status->Text += "\r\n"+s;
        status->SelectionStart = status->Text->Length;
        status->ScrollToCaret();
        status->Update();
        */
    }

    BTDictionary ^BTProcessor::GetTorrentDict()
    {
        // find dictionary for the current torrent file

        BTItem ^it = resumeDat->GetDict()->GetItem(torrentFile, true);
        if ((it == nullptr) || (it->Type != kDictionary))
            return nullptr;
        BTDictionary ^dict = safe_cast<BTDictionary ^>(it);
        return dict;
    }

        String ^BTProcessor::GetResumePrio(int fileNum)
    {
        BTDictionary ^dict = GetTorrentDict();
        if (dict == nullptr)
            return "";
        BTItem ^p = dict->GetItem("prio");
        if ((p == nullptr) || (p->Type != kString))
            return "";
        BTString ^prioString = safe_cast<BTString ^>(p);
        if ((fileNum < 0) || (fileNum > prioString->Data->Length))
            return "";

        int pr = prioString->Data[fileNum];
        if (pr == kPrioNormal)
            return "Normal";
        else if (pr == kPrioSkip)
          return "Skip";
        else
            return pr.ToString();
    }
    void BTProcessor::SetResumePrio(int fileNum, unsigned char newPrio)
    {
        if (!(Action & actSetPrios))
            return;

        if (fileNum == -1)
            fileNum = 0;
        BTDictionary ^dict = GetTorrentDict();
        if (dict == nullptr)
            return;
        BTItem ^p = dict->GetItem("prio");
        if ((p == nullptr) || (p->Type != kString))
            return;
        BTString ^prioString = safe_cast<BTString ^>(p);
        if ((fileNum < 0) || (fileNum > prioString->Data->Length))
            return;

        Altered = true;

        prioString->Data[fileNum] = newPrio;

        String ^ps;
        if (newPrio == kPrioSkip)
            ps = "Skip";
        else if (newPrio == kPrioNormal)
            ps = "Normal";
        else
            ps = newPrio.ToString();
        AddToStatus("  Set priority of file #"+(fileNum+1)+" to " + ps);
    }

    
    void BTProcessor::AlterResume(int fileNum, String ^toHere)
    {
        toHere = RemoveUT(toHere);

        BTDictionary ^dict = GetTorrentDict();
        if (dict == nullptr)
            return;

        Altered = true;

        if (fileNum == -1) // single file torrent
        {
            BTItem ^p = dict->GetItem("path");
            if (p == nullptr)
            {
                dict->Items->Add(gcnew BTDictionaryItem("path",gcnew BTString(toHere)));
                AddToStatus("   Added location "+ toHere);
            }
            else
            {
                if (p->Type != kString)
                    return;
                safe_cast<BTString^>(p)->SetString(toHere);
                AddToStatus("   Changed location to "+ toHere);
            }
        }
        else
        {
            // multiple file torrent, uses a list called "targets"
            BTItem ^p = dict->GetItem("targets");
            BTList ^theList = nullptr;
            if (p == nullptr)
            {
                theList = gcnew BTList();
                dict->Items->Add(gcnew BTDictionaryItem("targets",theList));
            }
            else
            {
                if (p->Type != kList)
                    return;
                theList = safe_cast<BTList ^>(p);
            }
            if (theList == nullptr)
                return;

            // the list contains two element lists, of integer/string which are filenumber/path

            BTList ^thisFileList = nullptr;
            // see if this file is already in the list
            for (int i=0;i<theList->Items->Count;i++)
            {
                if (theList->Items[i]->Type != kList)
                    return;

                BTList ^l2 = safe_cast<BTList ^>(theList->Items[i]);
                if ((l2->Items->Count != 2) || (l2->Items[0]->Type != kInteger) || (l2->Items[1]->Type != kString))
                    return;
                int n = (int)(safe_cast<BTInteger ^>(l2->Items[0])->Value);
                if (n == fileNum)
                {
                    thisFileList = l2;
                    break;
                }
            }
            if (thisFileList == nullptr) // didn't find it
            {
                thisFileList = gcnew BTList();
                thisFileList->Items->Add(gcnew BTInteger(fileNum));
                thisFileList->Items->Add(gcnew BTString(toHere));
                theList->Items->Add(thisFileList);
                AddToStatus("   Added location for file #"+(fileNum+1)+" to "+ toHere);
            }
            else
            {
                thisFileList->Items[1] = gcnew BTString(toHere);
                AddToStatus("   Changed location for file #"+(fileNum+1)+" to "+ toHere);
            }
        }
    }

    void BTProcessor::FixFileguard()
    {
        // finally, fix up ".fileguard"
        // this is the SHA1 of the entire file, without the .fileguard
        resumeDat->GetDict()->RemoveItem(".fileguard");
        MemoryStream ^ms = gcnew MemoryStream();
        resumeDat->Write(ms);
        System::Security::Cryptography::SHA1Managed ^sha1 = gcnew System::Security::Cryptography::SHA1Managed();
        array<unsigned char>^ theHash = sha1->ComputeHash(ms->GetBuffer(), 0, (int)ms->Length);
        ms->Close();
        String ^newfg = BTString::CharsToHex(theHash, 0, 20);
        resumeDat->GetDict()->Items->Add(gcnew BTDictionaryItem(".fileguard", gcnew BTString(newfg)));
    }

    void BTProcessor::DidntFindLocalFile(int torrentFileNum)
    {
        if (Action & actSetPrios)
            SetResumePrio(torrentFileNum, kPrioSkip);
    }

    String ^BTProcessor::MatchMissing(int torrentFileNum, String ^nameInTorrent)
    {
        // returns true if we found a match (if actSetPrio is on, true also means we have set a priority for this file)
        String ^simplifiedfname = SimplifyName(nameInTorrent);
        for each (MissingEpisode ^m in MissingList)
        {
            // see if the show name matches...
            if (Regex::Match(simplifiedfname,"\\b"+m->TheSeries->Name+"\\b",RegexOptions::IgnoreCase)->Success)
            {
                // see if season and episode match
                int seasF, epF;
                if (TVDoc::FindSeasEp("", simplifiedfname, &seasF, &epF, m->TheSeries->Name, Rexps) && (seasF == m->SeasonNumber) && (epF == m->EpNum))
                {
                    // match!
                    // get extension from nameInTorrent
                    int p = nameInTorrent->LastIndexOf(".");
                    String ^ext = (p == -1) ? "" : nameInTorrent->Substring(p);
                    AlterResume(torrentFileNum, m->FullNameWithPath+ext);
                    if (Action & actSetPrios)
                        SetResumePrio(torrentFileNum, kPrioNormal);
                    return m->FullNameWithPath+ext;
                }
            }
        }
        return nullptr;
    }

    bool BTProcessor::FoundLocalFile(FileInfo ^fi, int torrentFileNum, String ^nameInTorrent)
    {
        if ((fi == nullptr) || (fi->Name == nameInTorrent))  // haven't found a file with the name already in the torrent
            return false;

        if (Action & ( actCopy | actRename) )
        {
            renameList->Add(gcnew RCItem("-",
                (Action & actCopy) ? rcCopy : rcRename, 
                fi->Directory->FullName, 
                fi->Name, 
                (Action & actCopy) ? secondFolder : folder,
                nameInTorrent,
                nullptr));
            return true;
        }

        if (Action & actHashSearch)
        {
            AlterResume(torrentFileNum, fi->FullName); // point at fn

            if (Action & actSetPrios)
                SetResumePrio(torrentFileNum, kPrioNormal);
         
            return true;
        }
        return false;
    }


    void BTProcessor::WriteResumeDat()
    {
        Diagnostics::Debug::Assert(!(Action & actTestMode));

        AddToStatus("Writing resume.dat");
        FixFileguard();
        // write out new resume.dat file
        String ^to = secondFolder+"\\resume.dat.before_tvrename";
        if (File::Exists(to))
            File::Delete(to);
        File::Move(secondFolder+"\\resume.dat", to);
        Stream ^s = File::Create(secondFolder+"\\resume.dat");
        resumeDat->Write(s);
        s->Close();
        AddToStatus("  Done.");
    }



    void BTProcessor::Setup(String ^_torrent, 
        String ^_folder, 
        TreeView ^_tvTree, 
        ListView ^results,
        RCList ^_renameList,
        SetProgressDelegate ^_prog,
        int action,
        String ^_secondFolder,
        MissingEpisodeList ^missingList,
        FNPRegexList ^rexps)
    {
        Rexps = rexps;
        MissingList = missingList;
        torrentFile = _torrent;
        folder = _folder;
        tvTree = _tvTree;
        Results = results;
        renameList = _renameList;
        SetProg = _prog;
        Action = action;
        secondFolder = _secondFolder;

        DoHashChecking = action & ( actRename | actCopy | actHashSearch);
        UsingResumeDat = action & ( actHashSearch | actMatchMissing);

        Diagnostics::Debug::Assert(!(Action & actMatchMissing) || (missingList != nullptr));

        if (DoHashChecking)
            BuildFileCache(folder, action & actSearchSubfolders);
    }


    bool BTProcessor::Go()
    {
        if (SetProg != nullptr)
            SetProg->Invoke(0);

        if (folder == "") 
            return false;
        if (torrentFile == "")
            return false;

        if (tvTree != nullptr)
            tvTree->Nodes->Clear();

        if ((folder == "") || (torrentFile == ""))
            return false;
        DirectoryInfo ^di = gcnew DirectoryInfo(folder);

        if (!di->Exists)
            return false;

        // ----------------------------------------
        // read in torrent file

        BTFile ^btFile = BEncodeLoader::Load(torrentFile);
        if (btFile == nullptr)
            return false;

        // ----------------------------------------
        // read in resume.dat, if needed
        resumeDat = nullptr;

        bool testMode = (Action & actTestMode);

        if (UsingResumeDat)
        {
            resumeDat = BEncodeLoader::Load(secondFolder+"\\resume.dat");
            if (resumeDat == nullptr)
                return false;
        }

        if (tvTree != nullptr)
        {
            tvTree->BeginUpdate();
            btFile->Tree(tvTree->Nodes);
            tvTree->ExpandAll();
            //tvTree->Update();
            tvTree->EndUpdate();
        }

        BTItem ^bti = btFile->GetItem("info");
        if ((bti == nullptr) || (bti->Type != kDictionary))
            return false;

        BTDictionary ^infoDict = safe_cast<BTDictionary ^>(bti);

        bti = infoDict->GetItem("piece length");
        if ((bti == nullptr) || (bti->Type != kInteger))
            return false;

        int pieceSize = (int)safe_cast<BTInteger ^>(bti)->Value;

        if (renameList != nullptr)
            renameList->Clear();

        bti = infoDict->GetItem("pieces");
        if ((bti == nullptr) || (bti->Type != kString))
            return false;

        BTString ^torrentPieces = safe_cast<BTString ^>(bti);

        bti = infoDict->GetItem("files");
        Altered = false;

        if (bti == nullptr) // single file torrent
        {
            bti = infoDict->GetItem("name");
            if ((bti == nullptr) || (bti->Type != kString))
                return false;

            BTString ^di = safe_cast<BTString ^>(bti);
            bool prioSet = false;
            String ^newLocation = "";
            String ^type = "?";

            if (DoHashChecking)
            {
                BTItem ^fileSizeI = infoDict->GetItem("length");
                Int64 fileSize = safe_cast<BTInteger ^>(fileSizeI)->Value;

                array<unsigned char>^ torrentPieceHash = torrentPieces->StringTwentyBytePiece(0);
                if ((torrentPieceHash != nullptr) && (fileSize >= pieceSize))
                {
                    FileInfo ^fi = FindLocalFileWithHashAt(torrentPieceHash, 0, pieceSize, (int)fileSize);
                    if (fi != nullptr)
                    {
                        if (FoundLocalFile(fi, -1, di->AsString())) // file number of -1 for single torrent file
                        {
                          newLocation = fi->FullName;
                          type = "Hash";
                        }
                    }
                    else
                    {
                        DidntFindLocalFile(0);
                        type = "Not Found";
                    }
                    prioSet = true;
                }
                // don't worry about updating overallPosition as this is the only file in the torrent
            }
            if (Action & actMatchMissing)
            {
                String ^s = MatchMissing(-1, di->AsString());
                if (s != nullptr)
                {
                    type = "Missing";
                    prioSet = true;
                    newLocation = s;
                }
            }

            if (!prioSet && (Action & actSetPrios))
            {
                SetResumePrio(-1, kPrioSkip);
                prioSet = true;
                type = "Not Missing";
            }

                bool prioChanged = (Action & actSetPrios) && prioSet;
                if ( prioChanged || (newLocation != "") )
                    AddResult(type, torrentFile, "", prioChanged ? GetResumePrio(0):"", newLocation);
        }
        else
        {
            Int64 overallPosition = 0;
            Int32 lastPieceLeftover = 0;

            if (bti->Type != kList)
                return false;

            BTList ^fileList = safe_cast<BTList ^>(bti);

            // list of dictionaries
            for (int i=0;i<fileList->Items->Count;i++)
            {
                if (SetProg != nullptr)
                    SetProg->Invoke(100*i/fileList->Items->Count);
                if (fileList->Items[i]->Type != kDictionary)
                    return false;

                BTDictionary ^file = safe_cast<BTDictionary ^>(fileList->Items[i]);
                BTItem ^thePath = file->GetItem("path");
                if (thePath->Type != kList)
                    return false;
                BTList ^pathList = safe_cast<BTList ^>(thePath);
                // want the last of the items in the list, which is the filename itself
                int n = pathList->Items->Count-1;
                if (n < 0)
                    return false;
                BTString ^fileName = safe_cast<BTString ^>(pathList->Items[n]);
                bool prioSet = false;
                String ^newLocation = "";
                String ^type = "?";
                if (DoHashChecking)
                {
                    BTItem ^fileSizeI = file->GetItem("length");
                    Int64 fileSize = safe_cast<BTInteger ^>(fileSizeI)->Value;

                    int pieceNum = (int)(overallPosition / pieceSize);
                    if (overallPosition % pieceSize)
                        pieceNum++;

                    array<unsigned char>^ torrentPieceHash = torrentPieces->StringTwentyBytePiece(pieceNum);
                    if ((torrentPieceHash != nullptr) && (fileSize >= pieceSize))
                    {
                        FileInfo ^fi = FindLocalFileWithHashAt(torrentPieceHash, lastPieceLeftover, pieceSize, (int)fileSize);

                        if (fi != nullptr)
                        {
                            if (FoundLocalFile(fi, i, fileName->AsString()))
                            {
                              newLocation = fi->FullName;
                              type = "Hash";
                            }
                        }
                        else
                        {
                            DidntFindLocalFile(i);
                                type = "Not Found";
                        }
                        prioSet = true;
                    }

                    int sizeInPieces = (int)(fileSize / pieceSize);
                    if (fileSize % pieceSize)
                        sizeInPieces++; // another partial piece

                    lastPieceLeftover = (lastPieceLeftover + (Int32)((sizeInPieces * pieceSize) - fileSize)) % pieceSize;
                    overallPosition += fileSize;
                }
                if (Action & actMatchMissing)
                {
                    String ^s = MatchMissing(i, fileName->AsString());
                    if (s != nullptr)
                    {
                        prioSet = true;
                        newLocation = s;
                        type = "Missing";
                    }
                }
                
                if (!prioSet && (Action & actSetPrios))
                {
                    SetResumePrio(i, kPrioSkip);
                    prioSet = true;
                    type = "Not Missing";
                }

                bool prioChanged = (Action & actSetPrios) && prioSet;
                if ( prioChanged || (newLocation != "") )
                    AddResult(type, torrentFile, (i+1).ToString(), prioChanged ? GetResumePrio(i):"", newLocation);

            }
        }

        if (Altered && UsingResumeDat && !(Action & actTestMode))
            WriteResumeDat();

        if (SetProg != nullptr)
            SetProg->Invoke(0);

        return true;
    }

} // namespace

