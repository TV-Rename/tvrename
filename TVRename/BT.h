//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "Helpers.h"
#include "AIOItems.h"
#include "DirCache.h"

// http://wiki.theory.org/BitTorrentSpecification

namespace TVRename
{
	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::IO;

	using System::Windows::Forms::TreeNodeCollection;
	using System::Windows::Forms::TreeNode;

	enum { kError, kDictionary, kDictionaryItem, kList, 
		kListOrDictionaryEnd, kInteger, kString, kBTEOF };

	public ref class BTItem abstract
	{
	public:
		int Type; // from enum

	protected: BTItem(int type) :
		Type(type)
		{
		}

	public:
		virtual String ^AsText()
		{
			return String::Concat("Type=",Type.ToString());
		}

		virtual void Tree(TreeNodeCollection ^tn)
		{
			TreeNode ^n = gcnew TreeNode("BTItem:"+Type.ToString());
			tn->Add(n);
		}

		virtual void Write(Stream ^sw) = 0;
	};


	public ref class BTString :
		public BTItem
		{
		public:
			array<unsigned char> ^Data;

			BTString(String ^s) :
			BTItem(kString)
			{
				SetString(s);
			}

			BTString() :
			BTItem(kString)
			{
				Data = gcnew array<unsigned char>(0);
			}

			void SetString(String ^s)
			{
				Data = System::Text::Encoding::UTF8->GetBytes(s);
			}

			virtual String ^AsText() override
			{
				return "String="+AsString();
			}

			String ^AsString()
			{
				System::Text::Encoding ^enc = System::Text::Encoding::UTF8;
				return enc->GetString(Data);
			}

			array<unsigned char> ^StringTwentyBytePiece(int pieceNum)
			{
				array<unsigned char> ^res = gcnew array<unsigned char>(20);
				if ( ((pieceNum*20)+20) > Data->Length)
					return nullptr;
				Array::Copy(Data, pieceNum*20, res, 0, 20);
				return res;
			}


			static String ^CharsToHex(array<unsigned char > ^data, int start, int n)
			{
				String ^r = gcnew String("");
				for (int i=0;i<n;i++)
					r += (data[start+i]<16?"0":"") + data[start+i].ToString("x")->ToUpper();
				return r;
			}


			String ^PieceAsNiceString(int pieceNum)
			{
				return CharsToHex(Data, pieceNum * 20, 20);
				/*String ^r = gcnew String("");
				for (int i=0;i<20;i++,p++)
				r += (Data[p]<16?"0":"") + Data[p].ToString("x")->ToUpper();

				return r;*/
			}

			virtual void Tree(TreeNodeCollection ^tn) override
			{
				TreeNode ^n = gcnew TreeNode(String::Concat("String:",AsString()));
				tn->Add(n);
			}

			virtual void Write(Stream ^sw) override
			{
				// Byte strings are encoded as follows: <string length encoded in base ten ASCII>:<string data>

				array<Byte>^len = System::Text::Encoding::ASCII->GetBytes( Data->Length.ToString() );
				sw->Write(len, 0, len->Length);
				sw->WriteByte((unsigned char)':');
				sw->Write(Data, 0, Data->Length);
			}
		};

		public ref class BTEOF :
			public BTItem
			{
			public:
				BTEOF() :
				  BTItem(kBTEOF)
				  {
				  }
				  virtual void Write(Stream ^sw) override
				  {
				  }

			};

			public ref class BTError :
				public BTItem
				{
				public:
					String ^Message;

					BTError() :
					BTItem(kError)
					{
						Message = gcnew String("");
					}
					virtual String ^AsText() override 
					{
						return String::Concat("Error:",Message);
					}
					virtual void Tree(TreeNodeCollection ^tn) override
					{
						TreeNode ^n = gcnew TreeNode("BTError:"+Message);
						tn->Add(n);
					}
					virtual void Write(Stream ^sw) override
					{
					}
				};

				public ref class BTListOrDictionaryEnd :
					public BTItem
					{
					public:
						BTListOrDictionaryEnd() :
						  BTItem(kListOrDictionaryEnd)
						  {
						  }
						  virtual void Write(Stream ^sw) override
						  {
							  sw->WriteByte((unsigned char)'e');
						  }

					};

					public ref class BTDictionaryItem :
						public BTItem
						{
						public:
							String ^Key;
							BTItem ^Data;

							BTDictionaryItem() :
							BTItem(kDictionaryItem)
							{
							}

							BTDictionaryItem(String ^k, BTItem ^d) :
							BTItem(kDictionaryItem)
							{
								Key = k;
								Data = d;
							}

							virtual String ^AsText() override 
							{
								if ((Key == "pieces") && (Data->Type == kString))
								{
									return "<File hash data>";
								}
								else
									return String::Concat(Key,"=>",Data->AsText());
							}

							virtual void Tree(TreeNodeCollection ^tn) override
							{
								if ((Key == "pieces") && (Data->Type == kString))
								{
									// 20 byte chunks of SHA1 hash values
									TreeNode ^n = gcnew TreeNode("Key="+Key);
									tn->Add(n);
									n->Nodes->Add(gcnew TreeNode("<File hash data>" + safe_cast<BTString ^>(Data)->PieceAsNiceString(0)));
								}
								else
								{
									TreeNode ^n = gcnew TreeNode("Key="+Key);
									tn->Add(n);
									Data->Tree(n->Nodes);
								}
							}
							virtual void Write(Stream ^sw) override
							{
								BTString(Key).Write(sw);
								Data->Write(sw);
							}
						};

						public ref class BTDictionary :
							public BTItem
							{
							public:
								Generic::List<BTDictionaryItem ^> ^Items;

								BTDictionary() :
								BTItem(kDictionary)
								{
									Items = gcnew Generic::List<BTDictionaryItem ^>;
								}
								virtual String ^AsText() override
								{
									String ^r = gcnew String("Dictionary=[");
									for (int i=0;i<Items->Count;i++)
									{
										r += Items[i]->AsText();
										if (i != (Items->Count-1))
											r +=",";
									}
									r+="]";
									return r;
								}
								virtual void Tree(TreeNodeCollection ^tn) override
								{
									TreeNode ^n = gcnew TreeNode("Dictionary");
									tn->Add(n);
									for (int i=0;i<Items->Count;i++)
										Items[i]->Tree(n->Nodes);    
								}

								bool RemoveItem(String ^key)
								{
									for (int i=0;i<Items->Count;i++)
									{
										if (Items[i]->Key == key) 
										{
											Items->RemoveAt(i);
											return true;
										}
									}
									return false;
								}
								BTItem ^GetItem(String ^key)
								{
									return GetItem(key, false);
								}
								BTItem ^GetItem(String ^key, bool ignoreCase)
								{
									for (int i=0;i<Items->Count;i++)
										if ((Items[i]->Key == key) ||
											(ignoreCase && ((Items[i]->Key->ToLower() == key->ToLower()))))
											return Items[i]->Data;
									return nullptr;
								}
								virtual void Write(Stream ^sw) override
								{
									sw->WriteByte((unsigned char)'d');
									for each (BTItem ^i in Items)
										i->Write(sw);
									sw->WriteByte((unsigned char)'e');
								}

							};

							public ref class BTList :
								public BTItem
								{
								public:
									Generic::List<BTItem ^> ^Items;

									BTList() :
									BTItem(kList)
									{
										Items = gcnew Generic::List<BTItem ^>;
									}

									virtual String ^AsText() override
									{
										String ^r = gcnew String("List={");
										for (int i=0;i<Items->Count;i++)
										{
											r+=Items[i]->AsText();
											if (i != (Items->Count-1))
												r +=",";
										}
										r+="}";
										return r;
									}
									virtual void Tree(TreeNodeCollection ^tn) override
									{
										TreeNode ^n = gcnew TreeNode("List");
										tn->Add(n);
										for (int i=0;i<Items->Count;i++)
											Items[i]->Tree(n->Nodes);    
									}
									virtual void Write(Stream ^sw) override
									{
										sw->WriteByte((unsigned char)'l');
										for each (BTItem ^i in Items)
											i->Write(sw);
										sw->WriteByte((unsigned char)'e');
									}
								};

								public ref class BTInteger :
									public BTItem
									{
									public:
										Int64 Value;

										BTInteger() :
										BTItem(kInteger)
										{
											Value = 0;
										}

										BTInteger(Int64 n) :
										BTItem(kInteger)
										{
											Value = n;
										}

										virtual String ^AsText() override
										{
											return "Integer="+Value;
										}
										virtual void Tree(TreeNodeCollection ^tn) override
										{
											TreeNode ^n = gcnew TreeNode("Integer:"+Value.ToString());
											tn->Add(n);
										}
										virtual void Write(Stream ^sw) override
										{
											sw->WriteByte((unsigned char)'i');
											cli::array<unsigned char> ^b = System::Text::Encoding::ASCII->GetBytes( Value.ToString() ) ;
											sw->Write(b, 0, b->Length);
											sw->WriteByte((unsigned char)'e');
										}
									};

									public ref class BTFile
									{
									public:
										Generic::List<BTItem ^> ^Items;

										BTFile()
										{
											Items = gcnew Generic::List<BTItem ^>;
										}

										Generic::List<String ^> ^AllFilesInTorrent();


										String ^AsText()
										{ 
											String ^res = gcnew String("File= ");
											for (int i=0;i<Items->Count;i++)
												res += Items[i]->AsText() + " ";
											return res;
										}

										void Tree(TreeNodeCollection ^tn)
										{
											TreeNode ^n = gcnew TreeNode("BT File");
											tn->Add(n);
											for (int i=0;i<Items->Count;i++)
												Items[i]->Tree(n->Nodes);    
										}

										BTItem ^GetItem(String ^key)
										{
											return GetItem(key, false);
										}

										BTDictionary ^GetDict()
										{
											Diagnostics::Debug::Assert(Items->Count == 1);
											Diagnostics::Debug::Assert(Items[0]->Type == kDictionary);

											// our first (and only) Item will be a dictionary of stuff
											return safe_cast<BTDictionary ^>(Items[0]);
										}

										BTItem ^GetItem(String ^key, bool ignoreCase)
										{
											if (Items->Count == 0) 
												return nullptr;
											BTDictionary ^btd = GetDict();
											return btd->GetItem(key, ignoreCase);
										}

										void Write(Stream ^sw)
										{
											for each (BTItem ^i in Items)
												i->Write(sw);
										}

									};

									public ref class HashCacheItem
									{
									public:
										Int32 whereInFile;
										int pieceSize;
										Int32 fileSize;
										array<unsigned char>^ theHash;

										HashCacheItem(Int32 wif, int ps, Int32 fs, array<unsigned char> ^h)
										{
											whereInFile = wif;
											pieceSize = ps;
											fileSize = fs;
											theHash = h;
										}

									};
									typedef Generic::List<HashCacheItem ^> HashCacheItems;
									typedef Generic::Dictionary<String ^, HashCacheItems ^> HashCacheType;


									public ref class BEncodeLoader
									{
									private: BEncodeLoader()
											 {
											 }
									public:
										static BTItem ^ReadString(Stream ^sr, Int64 length);
										static BTItem ^ReadInt(FileStream ^sr);
										static BTItem ^ReadDictionary(FileStream ^sr);
										static BTItem ^ReadList(FileStream ^sr);
										static BTItem ^ReadNext(FileStream ^sr);
										static String ^NextThing(IO::FileStream ^sr);
										static BTFile ^Load(String ^filename);
									};

									public ref class BTCore abstract
									{
									protected:
										SetProgressDelegate ^SetProg;
										HashCacheType ^HashCache;
										int CacheChecks, CacheItems, CacheHits;
										String ^FileCacheIsFor;
										bool FileCacheWithSubFolders;
										DirCache ^FileCache;
										bool DoHashChecking;


										void Prog(int percent)
										{
											if (SetProg != nullptr)
												SetProg->Invoke(percent);
										}

										virtual bool NewTorrentEntry(String ^torrentFile, int numberInTorrent) = 0;
										virtual bool FoundFileOnDiskForFileInTorrent(String ^torrentFile, FileInfo ^onDisk,
											int numberInTorrent,
											String ^nameInTorrent) = 0;
										virtual bool DidNotFindFileOnDiskForFileInTorrent(String ^torrentFile, int numberInTorrent,
											String ^nameInTorrent) = 0;
										virtual bool FinishedTorrentEntry(String ^torrentFile, int numberInTorrent, String ^filename) = 0;


										FileInfo ^FindLocalFileWithHashAt(array<unsigned char>^ findMe, Int32 whereInFile, int pieceSize, Int32 fileSize);

										void CacheThis(String ^filename, Int32 whereInFile, int piecesize, Int32 fileSize, array<unsigned char> ^hash)
										{
											CacheItems++;
											if (!HashCache->ContainsKey(filename))
												HashCache[filename] = gcnew HashCacheItems();
											HashCache[filename]->Add(gcnew HashCacheItem(whereInFile, piecesize, fileSize, hash));
										}

										array<unsigned char> ^CheckCache(String ^filename, Int32 whereInFile, int piecesize, Int32 fileSize)
										{
											CacheChecks++;
											if (HashCache->ContainsKey(filename))
											{
												for each (HashCacheItem ^h in HashCache[filename])
													if ((h->whereInFile == whereInFile)&&(h->pieceSize==piecesize)&&(h->fileSize == fileSize))
													{
														CacheHits++;
														return h->theHash;
													}
											}
											return nullptr;
										}

										void BuildFileCache(String ^folder, bool subFolders)
										{
											if ((FileCache == nullptr) || (FileCacheIsFor == nullptr) || (FileCacheIsFor != folder) || (FileCacheWithSubFolders != subFolders))
											{
												FileCache = gcnew DirCache();
												BuildDirCache(nullptr,0,0,FileCache, folder, subFolders, nullptr);
												FileCacheIsFor = folder;
												FileCacheWithSubFolders = subFolders;

											}
										}

										bool ProcessTorrentFile(String ^torrentFile, TreeView ^tvTree);

										BTCore(SetProgressDelegate ^setprog)
										{
											SetProg = setprog;

											HashCache = gcnew HashCacheType();
											CacheChecks = CacheItems = CacheHits = 0;
											FileCache = nullptr;
											FileCacheIsFor = nullptr;
											FileCacheWithSubFolders = false;
										}
									}; // btcore

									public ref class BTFileRenamer : public BTCore
									{
									public:
										// Hash and file caches

										// settings for processing the torrent (and optionally resume) files
										//    int Action;
										//    String ^torrentFile;
										//    String ^folder;
										TreeView ^tvTree;
										//TextBox ^status;

										bool CopyNotMove;
										String ^CopyToFolder;

										AIOList ^RenameListOut;

										//    String ^secondFolder; // resume.dat location, or where to copy/move to

										virtual bool NewTorrentEntry(String ^torrentFile, int numberInTorrent) override;
										virtual bool FoundFileOnDiskForFileInTorrent(String ^torrentFile, FileInfo ^onDisk,
											int numberInTorrent,
											String ^nameInTorrent) override;
										virtual bool DidNotFindFileOnDiskForFileInTorrent(String ^torrentFile, int numberInTorrent,
											String ^nameInTorrent) override;
										virtual bool FinishedTorrentEntry(String ^torrentFile, int numberInTorrent, String ^filename) override;




									public:
										BTFileRenamer(SetProgressDelegate ^setprog) :
										  BTCore(setprog)
										  {
										  }
										  String ^CacheStats()
										  {
											  String ^r = "Hash Cache: " + CacheItems + " items for " + HashCache->Count + " files.  "+CacheHits+" hits from " + 
												  CacheChecks +" lookups";
											  if (CacheChecks != 0)
												  r += " ("+(100*CacheHits/CacheChecks)+"%)";
											  return r;
										  }

										  bool RenameFilesOnDiskToMatchTorrent(String ^torrentFile,
											  String ^folder, 
											  TreeView ^tvTree, 
											  AIOList ^renameListOut,
											  SetProgressDelegate ^_prog,
											  bool copyNotMove,
											  String ^copyDest);





										  // bool Go();
									}; // BTProcessor


									public ref class BTResume : public BTCore
									{
									public:
										bool HashSearch;
										bool TestMode;
										bool DoMatchMissing;
										bool SetPrios;
										bool SearchSubFolders;

										AIOList ^MissingList;

										int PercentBitsOn(BTString ^s);

										ListView ^Results;

										String ^NewLocation;
										String ^Type;
										bool PrioWasSet;

										BTFile ^ResumeDat; // resume file, if we're using it
										String ^ResumeDatPath;

										bool Altered;
										FNPRegexList ^Rexps; // used by MatchMissing

										bool LoadResumeDat();
										FileInfo ^MatchMissing(String ^torrentFile, int torrentFileNum, String ^nameInTorrent);

										void AlterResume(String ^torrentFile, int fileNum, String ^toHere);
										String ^GetResumePrio(String ^torrentFile, int fileNum);
										void SetResumePrio(String ^torrentFile, int fileNum, unsigned char newPrio);
										BTDictionary ^GetTorrentDict(String ^torrentFile);
										void FixFileguard();
										void WriteResumeDat();
										TorrentFileList ^AllFilesBeingDownloaded();

										virtual bool NewTorrentEntry(String ^torrentFile, int numberInTorrent) override;
										virtual bool FoundFileOnDiskForFileInTorrent(String ^torrentFile, FileInfo ^onDisk,
											int numberInTorrent,
											String ^nameInTorrent) override;
										virtual bool DidNotFindFileOnDiskForFileInTorrent(String ^torrentFile, int numberInTorrent,
											String ^nameInTorrent) override;
										virtual bool FinishedTorrentEntry(String ^torrentFile, int numberInTorrent, String ^filename) override;

										static String ^RemoveUT(String ^s)
										{
											// if it is a .!ut file, we can remove the extension
											if (s->EndsWith(".!ut"))
												return s->Remove(s->Length - 4);
											else
												return s;
										}


										void AddResult(String ^type, String ^torrent, String ^num, String ^prio, String ^location)
										{
											if (Results == nullptr)
												return;

											int p = torrent->LastIndexOf("\\");
											if (p != -1)
												torrent = torrent->Substring(p+1);
											ListViewItem ^lvi = gcnew ListViewItem(type);
											lvi->SubItems->Add(torrent);
											lvi->SubItems->Add(num);
											lvi->SubItems->Add(prio);
											lvi->SubItems->Add(RemoveUT(location));

											Results->Items->Add(lvi);
											lvi->EnsureVisible();
											Results->Update();
										}

										bool DoWork(Collections::Generic::List<String ^> ^Torrents, 
											String ^searchFolder, 
											ListView ^results,
											bool hashSearch,
											bool matchMissing,
											bool setPrios,
											bool testMode,
											bool searchSubFolders,
											AIOList ^missingList,
											FNPRegexList ^rexps);

										BTResume(SetProgressDelegate ^setprog, String ^resumeDatFile) : 
										BTCore(setprog)
										{
											ResumeDatPath = resumeDatFile;
										}

									}; // btresume


} // namespace
