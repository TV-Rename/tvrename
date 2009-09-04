#include "stdafx.h"
#include "BT.h"
#include "TVDoc.h"

#pragma warning(disable:4800)

namespace TVRename
{
	enum { kPrioNormal = 8, kPrioSkip = 0x80 };

	BTItem ^BEncodeLoader::ReadString(Stream ^sr, Int64 length)
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

	FileInfo ^BTCore::FindLocalFileWithHashAt(array<unsigned char>^ findMe, Int32 whereInFile, int pieceSize, Int32 fileSize)
	{
		if (whereInFile < 0)
			return nullptr;

		for (int i=0;i<FileCache->Count;i++)
		{
			FileInfo ^fiTemp  = FileCache[i]->TheFile;
			Int64 flen = FileCache[i]->Length;

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

	BTDictionary ^BTResume::GetTorrentDict(String ^torrentFile)
	{
		// find dictionary for the specified torrent file

		BTItem ^it = ResumeDat->GetDict()->GetItem(torrentFile, true);
		if ((it == nullptr) || (it->Type != kDictionary))
			return nullptr;
		BTDictionary ^dict = safe_cast<BTDictionary ^>(it);
		return dict;
	}

	int BTResume::PercentBitsOn(BTString ^s)
	{
		int totalBits = 0;
		int bitsOn = 0;

		for (int i=0;i<s->Data->Length;i++)
		{
			totalBits += 8;
			unsigned char c = s->Data[i];
			for (int j=0;j<8;j++)
			{
				if (c & 0x01)
					bitsOn++;
				c >>= 1;
			}
		}

		return (100 * bitsOn + totalBits/2) / totalBits;
	}

	TorrentFileList ^BTResume::AllFilesBeingDownloaded()
	{
		TorrentFileList ^r = gcnew TorrentFileList();

		for each (BTItem ^it in ResumeDat->GetDict()->Items)
		{
			if ((it->Type != kDictionaryItem))
				continue;

			BTDictionaryItem ^dictitem = safe_cast<BTDictionaryItem ^>(it);

			if ((dictitem->Key == ".fileguard") || (dictitem->Data->Type != kDictionary))
				continue;

			String ^torrentFile = dictitem->Key;
			BTDictionary ^d2 = safe_cast<BTDictionary ^>(dictitem->Data);

			BTItem ^p = d2->GetItem("prio");
			if ((p == nullptr) || (p->Type != kString))
				continue;

			BTString ^prioString = safe_cast<BTString ^>(p);
			
			BTFile ^tor = BEncodeLoader::Load(torrentFile);
			if (tor == nullptr)
				continue;

			Collections::Generic::List<String ^> ^a = tor->AllFilesInTorrent();
			if (a != nullptr)
			{
				int c = 0;

				p = d2->GetItem("path");
				if ((p == nullptr) || (p->Type != kString))
					continue;
				String ^defaultFolder = safe_cast<BTString ^>(p)->AsString();
				
				BTItem ^targets = d2->GetItem("targets");
				bool hasTargets = ((targets != nullptr) && (targets->Type == kList));
				BTList ^targetList = safe_cast<BTList ^>(targets);
				
				for each (String ^s in a)
				{
					if ((c < prioString->Data->Length) && ((int)prioString->Data[c] != kPrioSkip))
					{
						String ^saveTo = FileInFolder(defaultFolder, s)->Name;
						if (hasTargets)
						{
							// see if there is a target for this (the c'th) file
							for (int i=0;i<targetList->Items->Count;i++)
							{
								BTList ^l = safe_cast<BTList ^>(targetList->Items[i]);
								BTInteger ^n = safe_cast<BTInteger ^>(l->Items[0]);
								BTString ^dest = safe_cast<BTString ^>(l->Items[1]);
								if (n->Value == c)
								{
									saveTo = dest->AsString();
									break;
								}
							}
						}
						int percent = (a->Count == 1) ? PercentBitsOn(safe_cast<BTString ^>(d2->GetItem("have"))) : -1;
						TorrentEntry ^te = gcnew TorrentEntry(torrentFile, saveTo, percent);
						r->Add(te);
					}
					c++;

				}
			}
		}

		return r;
	}

	String ^BTResume::GetResumePrio(String ^torrentFile, int fileNum)
	{
		BTDictionary ^dict = GetTorrentDict(torrentFile);
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

	void BTResume::SetResumePrio(String ^torrentFile, int fileNum, unsigned char newPrio)
	{
		if (!SetPrios)
			return;

		if (fileNum == -1)
			fileNum = 0;
		BTDictionary ^dict = GetTorrentDict(torrentFile);
		if (dict == nullptr)
			return;
		BTItem ^p = dict->GetItem("prio");
		if ((p == nullptr) || (p->Type != kString))
			return;
		BTString ^prioString = safe_cast<BTString ^>(p);
		if ((fileNum < 0) || (fileNum > prioString->Data->Length))
			return;

		Altered = true;
		PrioWasSet = true;

		prioString->Data[fileNum] = newPrio;

		String ^ps;
		if (newPrio == kPrioSkip)
			ps = "Skip";
		else if (newPrio == kPrioNormal)
			ps = "Normal";
		else
			ps = newPrio.ToString();
	}


	void BTResume::AlterResume(String ^torrentFile, int fileNum, String ^toHere)
	{
		toHere = RemoveUT(toHere);

		BTDictionary ^dict = GetTorrentDict(torrentFile);
		if (dict == nullptr)
			return;

		Altered = true;

		if (fileNum == -1) // single file torrent
		{
			BTItem ^p = dict->GetItem("path");
			if (p == nullptr)
			{
				dict->Items->Add(gcnew BTDictionaryItem("path",gcnew BTString(toHere)));
			}
			else
			{
				if (p->Type != kString)
					return;
				safe_cast<BTString^>(p)->SetString(toHere);
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
			}
			else
			{
				thisFileList->Items[1] = gcnew BTString(toHere);
			}
		}
	}

	void BTResume::FixFileguard()
	{
		// finally, fix up ".fileguard"
		// this is the SHA1 of the entire file, without the .fileguard
		ResumeDat->GetDict()->RemoveItem(".fileguard");
		MemoryStream ^ms = gcnew MemoryStream();
		ResumeDat->Write(ms);
		System::Security::Cryptography::SHA1Managed ^sha1 = gcnew System::Security::Cryptography::SHA1Managed();
		array<unsigned char>^ theHash = sha1->ComputeHash(ms->GetBuffer(), 0, (int)ms->Length);
		ms->Close();
		String ^newfg = BTString::CharsToHex(theHash, 0, 20);
		ResumeDat->GetDict()->Items->Add(gcnew BTDictionaryItem(".fileguard", gcnew BTString(newfg)));
	}

	FileInfo ^BTResume::MatchMissing(String ^torrentFile, int torrentFileNum, String ^nameInTorrent)
	{
		// returns true if we found a match (if actSetPrio is on, true also means we have set a priority for this file)
		String ^simplifiedfname = SimplifyName(nameInTorrent);

		for each (AIOItem ^aio1 in MissingList)
		{
			if ((aio1->Type != AIOType::kMissing) && (aio1->Type != AIOType::kuTorrenting))
				continue;

			ProcessedEpisode ^m = nullptr;
			String ^name = nullptr;

			if (aio1->Type == AIOType::kMissing)
			{
				AIOMissing ^aio = safe_cast<AIOMissing ^>(aio1);
				m = aio->PE;
				name = aio->TheFileNoExt;
			}
			else if (aio1->Type == AIOType::kuTorrenting)
			{
				AIOuTorrenting ^aio = safe_cast<AIOuTorrenting ^>(aio1);
				m = aio->PE;
				name = aio->DesiredLocationNoExt;
			}
			
			if ((m == nullptr) || String::IsNullOrEmpty(name))
				continue;

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
					AlterResume(torrentFile, torrentFileNum, name+ext);
					if (SetPrios)
						SetResumePrio(torrentFile, torrentFileNum, kPrioNormal);
					return gcnew FileInfo(name+ext);
				}
			}
		}
		return nullptr;
	}

	void BTResume::WriteResumeDat()
	{
		FixFileguard();
		// write out new resume.dat file
		String ^to = ResumeDatPath+".before_tvrename";
		if (File::Exists(to))
			File::Delete(to);
		File::Move(ResumeDatPath, to);
		Stream ^s = File::Create(ResumeDatPath);
		ResumeDat->Write(s);
		s->Close();
	}

	bool BTCore::ProcessTorrentFile(String ^torrentFile, TreeView ^tvTree)
	{
		// ----------------------------------------
		// read in torrent file

		if (tvTree != nullptr)
			tvTree->Nodes->Clear();

		BTFile ^btFile = BEncodeLoader::Load(torrentFile);

		if (btFile == nullptr)
			return false;

		BTItem ^bti = btFile->GetItem("info");
		if ((bti == nullptr) || (bti->Type != kDictionary))
			return false;

		BTDictionary ^infoDict = safe_cast<BTDictionary ^>(bti);

		bti = infoDict->GetItem("piece length");
		if ((bti == nullptr) || (bti->Type != kInteger))
			return false;

		int pieceSize = (int)safe_cast<BTInteger ^>(bti)->Value;

		bti = infoDict->GetItem("pieces");
		if ((bti == nullptr) || (bti->Type != kString))
			return false;

		BTString ^torrentPieces = safe_cast<BTString ^>(bti);

		bti = infoDict->GetItem("files");

		if (bti == nullptr) // single file torrent
		{
			bti = infoDict->GetItem("name");
			if ((bti == nullptr) || (bti->Type != kString))
				return false;

			BTString ^di = safe_cast<BTString ^>(bti);
			String ^nameInTorrent = di->AsString();

			BTItem ^fileSizeI = infoDict->GetItem("length");
			Int64 fileSize = safe_cast<BTInteger ^>(fileSizeI)->Value;

			NewTorrentEntry(torrentFile, -1);
			if (DoHashChecking)
			{
				array<unsigned char>^ torrentPieceHash = torrentPieces->StringTwentyBytePiece(0);

				FileInfo ^fi = FindLocalFileWithHashAt(torrentPieceHash, 0, pieceSize, (int)fileSize);
				if (fi != nullptr)
					FoundFileOnDiskForFileInTorrent(torrentFile, fi, -1, nameInTorrent);
				else
					DidNotFindFileOnDiskForFileInTorrent(torrentFile, -1, nameInTorrent);
			}
			FinishedTorrentEntry(torrentFile, -1, nameInTorrent);

			// don't worry about updating overallPosition as this is the only file in the torrent
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
				Prog(100*i/fileList->Items->Count);
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

				BTItem ^fileSizeI = file->GetItem("length");
				Int64 fileSize = safe_cast<BTInteger ^>(fileSizeI)->Value;

				int pieceNum = (int)(overallPosition / pieceSize);
				if (overallPosition % pieceSize)
					pieceNum++;

				NewTorrentEntry(torrentFile, i);
				
				if (DoHashChecking)
				{
					array<unsigned char>^ torrentPieceHash = torrentPieces->StringTwentyBytePiece(pieceNum);

					FileInfo ^fi = FindLocalFileWithHashAt(torrentPieceHash, lastPieceLeftover, pieceSize, (int)fileSize);
					if (fi != nullptr)
						FoundFileOnDiskForFileInTorrent(torrentFile, fi, i, fileName->AsString());
					else
						DidNotFindFileOnDiskForFileInTorrent(torrentFile, i, fileName->AsString());
				}

				FinishedTorrentEntry(torrentFile, i, fileName->AsString());

				int sizeInPieces = (int)(fileSize / pieceSize);
				if (fileSize % pieceSize)
					sizeInPieces++; // another partial piece

				lastPieceLeftover = (lastPieceLeftover + (Int32)((sizeInPieces * pieceSize) - fileSize)) % pieceSize;
				overallPosition += fileSize;

			} // for each file in the torrent
		}

		if (tvTree != nullptr)
		{
			tvTree->BeginUpdate();
			btFile->Tree(tvTree->Nodes);
			tvTree->ExpandAll();
			tvTree->EndUpdate();
			tvTree->Update();
		}

		Prog(0);

		return true;
	}

	bool BTFileRenamer::NewTorrentEntry(String ^torrentFile, int numberInTorrent)
	{
		return true;
	}

	bool BTFileRenamer::FoundFileOnDiskForFileInTorrent(String ^torrentFile, 
		FileInfo ^onDisk,
		int numberInTorrent,
		String ^nameInTorrent)
	{
		RenameListOut->Add(gcnew AIOCopyMoveRename(CopyNotMove ? AIOCopyMoveRename::Op::Copy : AIOCopyMoveRename::Op::Rename, 
			onDisk, 
			FileInFolder(CopyNotMove ? CopyToFolder : onDisk->Directory->Name, nameInTorrent),
			nullptr));

		return true;
	}

	bool BTFileRenamer::DidNotFindFileOnDiskForFileInTorrent(String ^torrentFile, 
		int numberInTorrent,
		String ^nameInTorrent)
	{
		return true;
	}

	bool BTFileRenamer::FinishedTorrentEntry(String ^torrentFile, 
		int numberInTorrent, String ^filename)
	{
		return true;
	}


	bool BTResume::NewTorrentEntry(String ^torrentFile, int numberInTorrent)
	{
		NewLocation = "";
		PrioWasSet = false;
		Type = "?";
		return true;
	}

	bool BTResume::FoundFileOnDiskForFileInTorrent(String ^torrentFile, FileInfo ^onDisk,
		int numberInTorrent,
		String ^nameInTorrent)
	{
		NewLocation = onDisk->FullName;
		Type = "Hash";

		AlterResume(torrentFile, numberInTorrent, onDisk->FullName); // make resume.dat point to the file we found

		if (SetPrios)
		{
			SetResumePrio(torrentFile, numberInTorrent, kPrioNormal);
		}

		return true;
	}


	bool BTResume::DidNotFindFileOnDiskForFileInTorrent(String ^torrentFile, int numberInTorrent,
		String ^nameInTorrent)
	{
		Type = "Not Found";

		if (SetPrios)
		{
			SetResumePrio(torrentFile, numberInTorrent, kPrioSkip);
		}
		return true;
	}

	bool BTResume::FinishedTorrentEntry(String ^torrentFile, int numberInTorrent, String ^filename)
	{
 		if (DoMatchMissing)
		{
			FileInfo ^s = MatchMissing(torrentFile, numberInTorrent, filename);
			if (s != nullptr)
			{
				PrioWasSet = true;
				NewLocation = s->FullName;
				Type = "Missing";
			}
		}

		if (SetPrios && !PrioWasSet)
		{
			SetResumePrio(torrentFile, numberInTorrent, kPrioSkip);
			Type = "Not Missing";
		}

		bool prioChanged = SetPrios && PrioWasSet;
		if ( prioChanged || (!String::IsNullOrEmpty(NewLocation)) )
			AddResult(Type, torrentFile, (numberInTorrent+1).ToString(), 
			prioChanged ? GetResumePrio(torrentFile, numberInTorrent):"", NewLocation);
		return true;
	}



	bool BTFileRenamer::RenameFilesOnDiskToMatchTorrent(String ^torrentFile,
		String ^folder, 
		TreeView ^tvTree, 
		AIOList ^renameListOut,
		SetProgressDelegate ^_prog,
		bool copyNotMove,
		String ^copyDest)
	{
		if ((String::IsNullOrEmpty(folder) ||
			!Directory::Exists(folder)))
			return false;

		if (String::IsNullOrEmpty(torrentFile))
			return false;

		if (renameListOut == nullptr)
			return false;

		if (copyNotMove && 
			(String::IsNullOrEmpty(copyDest) ||
			!Directory::Exists(copyDest)))
			return false;

		CopyNotMove = copyNotMove;
		CopyToFolder = copyDest;
		DoHashChecking = true;
		RenameListOut = renameListOut;

		Prog(0);

		BuildFileCache(folder, false); // don't do subfolders

		RenameListOut->Clear();

		bool r = ProcessTorrentFile(torrentFile, tvTree);

		return r;
	}



	bool BTResume::LoadResumeDat()
	{
		ResumeDat = BEncodeLoader::Load(ResumeDatPath);
		return (ResumeDat != nullptr);
	}

	bool BTResume::DoWork(Collections::Generic::List<String ^> ^Torrents, 
		String ^searchFolder, 
		ListView ^results,
		bool hashSearch,
		bool matchMissing,
		bool setPrios,
		bool testMode,
						  bool searchSubFolders,
		AIOList ^missingList,
		FNPRegexList ^rexps)
	{
		Rexps = rexps;

		if (!matchMissing && !hashSearch)
			return true; // nothing to do

		if (hashSearch && String::IsNullOrEmpty(searchFolder))
			return false;

		if (matchMissing && ((missingList == nullptr) || (rexps == nullptr)))
			return false;

		MissingList = missingList;
		DoMatchMissing = matchMissing;
		DoHashChecking = hashSearch;
		SetPrios = setPrios;
		Results = results;

		Prog(0);

		if (!LoadResumeDat())
			return false;

		bool r = true;

		Prog(0);

		if (hashSearch)
		  BuildFileCache(searchFolder, searchSubFolders);


		for each (String ^tf in Torrents)
		{
			r = r && ProcessTorrentFile(tf, nullptr);
			if (!r)
				break;
		}

		if (Altered && !testMode)
			WriteResumeDat();

		Prog(0);

		return r;

	}

Generic::List<String ^> ^BTFile::AllFilesInTorrent()
{
  Generic::List<String ^> ^r = gcnew Generic::List<String ^>();
  
  BTItem ^bti = GetItem("info");
  if ((bti == nullptr) || (bti->Type != kDictionary))
	return nullptr;

  BTDictionary ^infoDict = safe_cast<BTDictionary ^>(bti);

  bti = infoDict->GetItem("files");

  if (bti == nullptr) // single file torrent
	{
	  bti = infoDict->GetItem("name");
	  if ((bti == nullptr) || (bti->Type != kString))
		  return nullptr;
	  r->Add(safe_cast<BTString ^>(bti)->AsString());
  }
  else
  { // multiple file torrent
	  BTList ^fileList = safe_cast<BTList ^>(bti);
	  
	  for each (BTItem ^it in fileList->Items)
	  {
		  BTDictionary ^file = safe_cast<BTDictionary ^>(it);
		  BTItem ^thePath = file->GetItem("path");
		  if (thePath->Type != kList)
			  return nullptr;
		  BTList ^pathList = safe_cast<BTList ^>(thePath);
		  // want the last of the items in the list, which is the filename itself
		  int n = pathList->Items->Count-1;
		  if (n < 0)
			  return nullptr;
		  BTString ^fileName = safe_cast<BTString ^>(pathList->Items[n]);
		  r->Add(fileName->AsString());
	  }
  }

  return r;
}
} // namespace

