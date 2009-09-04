#pragma once

#include "TheTVDB.h"
#include "ShowItem.h"
#include "RSS.h"
#include "Helpers.h"

using namespace System::Windows::Forms;
using namespace System::IO;

namespace TVRename
{
	ref class TVDoc;

	enum class AIOType  { kMissing, kCopyMoveRename, kRSS, kDownload, kNFO,
		kuTorrenting};

	public ref class IgnoreItem
	{
	public:
		String ^FileAndPath;

		IgnoreItem(XmlReader ^r)
		{
			if (r->Name == "Ignore")
				FileAndPath = r->ReadElementContentAsString();
		}
		IgnoreItem(String ^fileAndPath)
		{
			FileAndPath = fileAndPath;
		}

		bool operator==(IgnoreItem ^o)
		{
			if (String::IsNullOrEmpty(FileAndPath) || String::IsNullOrEmpty(o->FileAndPath))
				return false;
			return FileAndPath == o->FileAndPath;
		}

		void Write(XmlWriter ^writer)
		{
			writer->WriteStartElement("Ignore");
			writer->WriteValue(FileAndPath);
			writer->WriteEndElement(); // Ignore
		}
		void Read(XmlReader ^r)
		{

		}

	};

	typedef Generic::List<IgnoreItem ^> IgnoreList;

	public ref class AIOItem abstract
	{
	public:
		ProcessedEpisode ^PE; // can be null if not applicable or known

		bool Done;
		String ^ErrorText;
		bool HasError;

		AIOType Type;

		virtual IgnoreItem^ GetIgnore() = 0;

		virtual ListViewItem ^GetLVI(ListView ^lv) = 0;

		virtual String ^FilenameForProgress() = 0;

		virtual String ^TargetFolder() = 0; // nullptr if none, otherwise folder "of interest" for this item
		                                           // e.g. where file is missing from, or downloader is downloading to

		virtual bool Action(TVDoc ^doc)
		{
			// default is to do nothing
			// also set Done
			Done = true;
			return true; // all ok
		}

		virtual bool SameAs(AIOItem ^o) = 0;
		virtual int IconNumber() 
		{
			return -1;
		}

		// Search predicate 
		static bool DoneOK(AIOItem^ i)
		{
			return i->Done && !i->HasError;
		}

	protected:
		AIOItem(AIOType t, ProcessedEpisode ^pe)
		{
			PE = pe;
			Done = false;
			Type = t;
			HasError = false;
			ErrorText = "";
		}

	};

	public ref class AIODownload : public AIOItem
	{
	public:
		ShowItem ^SI;
		FileInfo ^Destination;
		String ^BannerPath;

		AIODownload(ShowItem ^si, ProcessedEpisode ^pe, FileInfo ^dest, String ^bannerPath) : 
		AIOItem(AIOType::kDownload, pe)
		{
			Destination = dest;
			SI = si;
			BannerPath = bannerPath;
		}
		virtual IgnoreItem^ GetIgnore() override
		{
			if (Destination == nullptr)
				return nullptr;
			else
				return gcnew IgnoreItem(Destination->FullName);
		}
		virtual String ^TargetFolder() override
		{
			if (Destination == nullptr)
				return nullptr;
			else
			  return Destination->DirectoryName;
		}
		virtual String ^FilenameForProgress() override
		{
			return Destination->Name;
		}
		bool SameAs2(AIODownload ^o)
		{
			return (o->Destination == Destination);
		}
		virtual bool SameAs(AIOItem ^o) override
		{
			return (this->Type == o->Type) && SameAs2(safe_cast<AIODownload ^>(o));
		}
		virtual int IconNumber() override
		{
			return 5;
		}
		virtual ListViewItem ^GetLVI(ListView ^lv) override
		{
			ListViewItem ^lvi = gcnew ListViewItem();

			lvi->Text = SI->ShowName();
			lvi->SubItems->Add(PE != nullptr ? PE->SeasonNumber.ToString() : "");
			lvi->SubItems->Add(PE != nullptr ? PE->NumsAsString() : "");

			if (PE != nullptr)
			{
				DateTime ^dt = PE->GetAirDateDT(true);
				if ((dt != nullptr) && (dt->CompareTo(DateTime::MaxValue)))
					lvi->SubItems->Add(PE->GetAirDateDT(true)->ToShortDateString());
				else
					lvi->SubItems->Add("");
			}
			else
				lvi->SubItems->Add("");

			lvi->SubItems->Add(Destination->DirectoryName);
			lvi->SubItems->Add(BannerPath);

			if (String::IsNullOrEmpty(BannerPath))
				lvi->BackColor = WarningColor();

			lvi->SubItems->Add(Destination->Name);

			lvi->Tag = this;
			lvi->Group = lv->Groups[5]; // download image
			// lv->Items->Add(lvi);

			return lvi;
		}

		virtual bool Action(TVDoc ^doc) override
		{
			array<unsigned char>^ theData = SI->TVDB->GetPage(BannerPath, false, TVRename::tmBanner, false);
			if (theData == nullptr)
			{
				ErrorText = "Unable to download " + BannerPath;
				HasError = true;
				return false;
			}

			FileStream ^fs = gcnew FileStream(Destination->FullName, FileMode::Create);
			fs->Write(theData, 0, theData->Length);
			fs->Close();

			Done = true;
			return true;
		}
	};

	public ref class AIOCopyMoveRename : public AIOItem
	{
	public:
		FileInfo ^From;
		FileInfo ^To;

		enum class Op { Copy, Move, Rename };

		Op Operation; 

		bool IsMoveRename() { return ((Operation == Op::Move) || (Operation == Op::Rename)); } // same thing to the OS

				virtual IgnoreItem^ GetIgnore() override
		{
			if (To == nullptr)
				return nullptr;
			else
				return gcnew IgnoreItem(To->FullName);
		}

		AIOCopyMoveRename(Op operation, 
			FileInfo ^from,
			FileInfo ^to,
			ProcessedEpisode ^ep) :
		AIOItem( AIOType::kCopyMoveRename, ep )
		{
			Operation = operation;
			From = from;
			To = to;
		}
		virtual int IconNumber() override
		{
			return (IsMoveRename() ? 4 : 3);
		}
		virtual String ^TargetFolder() override
		{
			if (To == nullptr)
				return nullptr;
			else
			  return To->DirectoryName;
		}
		virtual String ^FilenameForProgress() override
		{
			return To->Name;
		}
		virtual ListViewItem ^GetLVI(ListView ^lv) override
		{
			ListViewItem ^lvi = gcnew ListViewItem();

			if (PE == nullptr)
			{
				lvi->Text = "";
				lvi->SubItems->Add("");
				lvi->SubItems->Add("");
				lvi->SubItems->Add("");
			}
			else
			{			
				lvi->Text = PE->TheSeries->Name;
				lvi->SubItems->Add(PE->SeasonNumber.ToString());
				lvi->SubItems->Add(PE->NumsAsString());
				DateTime ^dt = PE->GetAirDateDT(true);
				if ((dt != nullptr) && (dt->CompareTo(DateTime::MaxValue)))
					lvi->SubItems->Add(PE->GetAirDateDT(true)->ToShortDateString());
				else
					lvi->SubItems->Add("");
			}

			lvi->SubItems->Add(From->DirectoryName);
			lvi->SubItems->Add(From->Name);
			lvi->SubItems->Add(To->DirectoryName);
			lvi->SubItems->Add(To->Name);

			if (Operation == Op::Rename)
				lvi->Group = lv->Groups[1];
			else if (Operation == Op::Copy)
				lvi->Group = lv->Groups[2];
			else if (Operation == Op::Move)
				lvi->Group = lv->Groups[3];

			//lv->Items->Add(lvi);
			return lvi;
		}

		bool SameSource(AIOCopyMoveRename ^o)
		{
			return (Same(From,o->From));
		}
		bool SameAs2(AIOCopyMoveRename ^o)
		{
			return ((Operation == o->Operation) && Same(From,o->From) && Same(To,o->To));
		}
		virtual bool SameAs(AIOItem ^o) override
		{
			return (this->Type == o->Type) && SameAs2(safe_cast<AIOCopyMoveRename ^>(o));
		}

		long long FileSize()
		{
			try 
			{
				return From->Length;
			}
			catch (...)
			{
				return 1;
			}
		}



	};

	public ref class AIOMissing : public AIOItem
	{
	public:
		String ^TheFileNoExt;

		AIOMissing(ProcessedEpisode ^pe, String ^whereItShouldBeNoExt) :
		AIOItem( AIOType::kMissing, pe )
		{
			TheFileNoExt = whereItShouldBeNoExt;
		}
		
		virtual IgnoreItem^ GetIgnore() override
		{
			if (String::IsNullOrEmpty(TheFileNoExt))
				return nullptr;
			else
				return gcnew IgnoreItem(TheFileNoExt);
		}

		virtual String ^TargetFolder() override
		{
			if (String::IsNullOrEmpty(TheFileNoExt))
				return nullptr;
			else
				return FileInfo(TheFileNoExt).DirectoryName;
		}
		virtual String ^FilenameForProgress() override
		{
			return PE->Name;
		}
		virtual bool Action(TVDoc ^doc) override
		{
			return true; // return success, but don't set as Done
		}
		virtual int IconNumber() override
		{
			return 1;
		}
		bool SameAs2(AIOMissing ^o)
		{
			return String::Compare(o->TheFileNoExt, TheFileNoExt) == 0;
		}
		virtual bool SameAs(AIOItem ^o) override
		{
			return (this->Type == o->Type) && SameAs2(safe_cast<AIOMissing ^>(o));
		}

		virtual ListViewItem ^GetLVI(ListView ^lv) override
		{
			ListViewItem ^lvi = gcnew ListViewItem();

			lvi->Text = PE->SI->ShowName();
			lvi->SubItems->Add(PE->SeasonNumber.ToString());
			lvi->SubItems->Add(PE->NumsAsString());

			DateTime ^dt = PE->GetAirDateDT(true);
			if ((dt != nullptr) && (dt->CompareTo(DateTime::MaxValue)))
				lvi->SubItems->Add(PE->GetAirDateDT(true)->ToShortDateString());
			else
				lvi->SubItems->Add("");

			lvi->SubItems->Add(FileInfo(TheFileNoExt).DirectoryName);
			lvi->SubItems->Add(FileInfo(TheFileNoExt).Name);

			lvi->Tag = this;
			lvi->Group = lv->Groups[0];

			//lv->Items->Add(lvi);
			return lvi;
		}

	};


	public ref class AIORSS : public AIOItem
	{
	public:
		RSSItem ^RSS;
		String ^TheFileNoExt;

		AIORSS(RSSItem ^rss, String ^toWhereNoExt, ProcessedEpisode ^pe) :
		AIOItem( AIOType::kRSS, pe )
		{
			PE = pe;
			RSS = rss;
			TheFileNoExt = toWhereNoExt;
		}
				virtual IgnoreItem^ GetIgnore() override
		{
					if (String::IsNullOrEmpty(TheFileNoExt))
				return nullptr;
			else
				return gcnew IgnoreItem(TheFileNoExt);
		}
		virtual String ^TargetFolder() override
		{
			if (String::IsNullOrEmpty(TheFileNoExt))
				return nullptr;
			else
				return FileInfo(TheFileNoExt).DirectoryName;
		}
		virtual String ^FilenameForProgress() override
		{
			return RSS->Title;
		}
		bool SameAs2(AIORSS ^o)
		{
			return (o->RSS == RSS) ;
		}
		virtual bool SameAs(AIOItem ^o) override
		{
			return (this->Type == o->Type) && SameAs2(safe_cast<AIORSS ^>(o));
		}
		virtual int IconNumber() override
		{
			return 6;
		}
		virtual ListViewItem ^GetLVI(ListView ^lv) override
		{
			ListViewItem ^lvi = gcnew ListViewItem();

			lvi->Text = PE->SI->ShowName();
			lvi->SubItems->Add(PE->SeasonNumber.ToString());
			lvi->SubItems->Add(PE->NumsAsString());
			DateTime ^dt = PE->GetAirDateDT(true);
			if ((dt != nullptr) && (dt->CompareTo(DateTime::MaxValue)))
				lvi->SubItems->Add(PE->GetAirDateDT(true)->ToShortDateString());
			else
				lvi->SubItems->Add("");

			lvi->SubItems->Add(TheFileNoExt);
			lvi->SubItems->Add(RSS->Title);

			lvi->Group = lv->Groups[4];
			lvi->Tag = this;

			// lv->Items->Add(lvi);
			return lvi;
		}

		virtual bool Action(TVDoc ^doc) override;
	};


	public ref class AIONFO : public AIOItem
	{
	public:
		FileInfo ^Where;
		ShowItem ^SI; // if for an entire show, rather than specific episode

		AIONFO(FileInfo ^nfo, ProcessedEpisode ^pe) :
		AIOItem( AIOType::kNFO, pe )
		{
			SI = nullptr;
			Where = nfo;
		}
		virtual IgnoreItem^ GetIgnore() override
		{
			
			if (Where == nullptr)
				return nullptr;
			else
				return gcnew IgnoreItem(Where->FullName);
		}
		AIONFO(FileInfo ^nfo, ShowItem ^si) :
		AIOItem( AIOType::kNFO, nullptr )
		{
			SI = si;
			Where = nfo;
		}
		virtual String ^TargetFolder() override
		{
			if (Where == nullptr)
				return nullptr;
			else
				return Where->DirectoryName;
		}
		virtual String ^FilenameForProgress() override
		{
			return Where->Name;
		}
		bool SameAs2(AIONFO ^o)
		{
			return (o->Where == Where) ;
		}
		virtual bool SameAs(AIOItem ^o) override
		{
			return (this->Type == o->Type) && SameAs2(safe_cast<AIONFO ^>(o));
		}
		virtual int IconNumber() override
		{
			return 7;
		}

		virtual ListViewItem ^GetLVI(ListView ^lv) override
		{
			ListViewItem ^lvi = gcnew ListViewItem();

			if (PE != nullptr)
			{
				lvi->Text = PE->SI->ShowName();
				lvi->SubItems->Add(PE->SeasonNumber.ToString());
				lvi->SubItems->Add(PE->NumsAsString());
				DateTime ^dt = PE->GetAirDateDT(true);
				if ((dt != nullptr) && (dt->CompareTo(DateTime::MaxValue)))
					lvi->SubItems->Add(PE->GetAirDateDT(true)->ToShortDateString());
				else
					lvi->SubItems->Add("");
			}
			else
			{
				lvi->Text = SI->ShowName();
				lvi->SubItems->Add("");
				lvi->SubItems->Add("");
				lvi->SubItems->Add("");
			}

			lvi->SubItems->Add(Where->DirectoryName);
			lvi->SubItems->Add(Where->Name);

			lvi->Group = lv->Groups[6];
			lvi->Tag = this;

			//lv->Items->Add(lvi);
			return lvi;
		}

		static void WriteInfo(XmlWriter ^writer, ShowItem ^si, String ^whichItem, String ^as)
		{
			String ^t = si->TheSeries()->GetItem(whichItem);
			if (!String::IsNullOrEmpty(t))
			{
				writer->WriteStartElement(as);
				writer->WriteValue(t);
				writer->WriteEndElement();
			}
		}

		virtual bool Action(TVDoc ^doc) override
		{
			XmlWriterSettings ^settings = gcnew XmlWriterSettings();
			settings->Indent = true;
			settings->NewLineOnAttributes = true;
			XmlWriter ^writer = XmlWriter::Create(Where->FullName, settings);

			if (PE != nullptr) // specific episode
			{
				// See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
				writer->WriteStartElement("episodedetails");
				writer->WriteStartElement("title");
				writer->WriteValue(PE->Name);
				writer->WriteEndElement();
				writer->WriteStartElement("season");
				writer->WriteValue(PE->SeasonNumber);
				writer->WriteEndElement();
				writer->WriteStartElement("episode");
				writer->WriteValue(PE->EpNum);
				writer->WriteEndElement();
				writer->WriteStartElement("plot");
				writer->WriteValue(PE->Overview);
				writer->WriteEndElement();
				writer->WriteStartElement("aired");
				if (PE->FirstAired != nullptr)
					writer->WriteValue(PE->FirstAired->ToString("yyyy-MM-dd"));
				writer->WriteEndElement();
				writer->WriteEndElement(); // episodedetails
			}
			else if (SI != nullptr) // show overview (tvshow.nfo)
			{
				// http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

				writer->WriteStartElement("tvshow");

				writer->WriteStartElement("title");
				writer->WriteValue(SI->ShowName());
				writer->WriteEndElement();

				writer->WriteStartElement("episodeguideurl");
				writer->WriteValue(SI->TVDB->BuildURL(true, SI->TVDBCode, SI->TVDB->PreferredLanguage(SI->TVDBCode)));
				writer->WriteEndElement();

				WriteInfo(writer, SI, "Overview", "plot");

					String ^genre = SI->TheSeries()->GetItem("Genre");
				if (!String::IsNullOrEmpty(genre))
				{
					genre = genre->Trim('|');
					genre = genre->Replace("|"," / ");
					writer->WriteStartElement("genre");
					writer->WriteValue(genre);
					writer->WriteEndElement();
				}

				WriteInfo(writer, SI, "FirstAired", "premiered");
				WriteInfo(writer, SI, "Rating", "rating");

				// actors...
				String ^actors = SI->TheSeries()->GetItem("Actors");
				if (!String::IsNullOrEmpty(actors))
				{
					bool first = true;
					for each (String ^aa in actors->Split('|'))
					{
						if (!String::IsNullOrEmpty(aa))
						{
							writer->WriteStartElement("actor");
							writer->WriteStartElement("name");
							writer->WriteValue(aa);
							writer->WriteEndElement(); // name
							writer->WriteEndElement(); // actor
						}
					}
				}

				WriteInfo(writer, SI, "ContentRating", "mpaa");
				WriteInfo(writer, SI, "IMDB_ID", "id");

				writer->WriteStartElement("tvdbid");
				writer->WriteValue(SI->TheSeries()->TVDBCode);
				writer->WriteEndElement();

				String ^rt = SI->TheSeries()->GetItem("Runtime");
				if (!String::IsNullOrEmpty(rt))
				{
					writer->WriteStartElement("runtime");
					writer->WriteValue(rt+" minutes");
					writer->WriteEndElement();
				}

				writer->WriteEndElement(); // tvshow
			}

			writer->Close();
			Done = true;
			return true;
		}
	};

	public ref class TorrentEntry
	{
	public:
		String ^TorrentFile;
		String ^DownloadingTo;
		int PercentDone;

		TorrentEntry(String ^torrentfile, String ^to, int percent)
		{
			TorrentFile = torrentfile;
			DownloadingTo = to;
			PercentDone = percent;
		}
	};

	typedef Generic::List<TorrentEntry ^> TorrentFileList;

	public ref class AIOuTorrenting : public AIOItem
	{
	public:
		TorrentEntry ^Entry;
		String ^DesiredLocationNoExt;

		AIOuTorrenting(TorrentEntry ^te, ProcessedEpisode ^pe, String ^desiredLocationNoExt) :
		AIOItem( AIOType::kuTorrenting, pe )
		{
			DesiredLocationNoExt = desiredLocationNoExt;
			Entry = te;
		}

		virtual IgnoreItem^ GetIgnore() override
		{
					if (String::IsNullOrEmpty(DesiredLocationNoExt))
				return nullptr;
			else
				return gcnew IgnoreItem(DesiredLocationNoExt);
		}
		virtual String ^TargetFolder() override
		{
			if (String::IsNullOrEmpty(Entry->DownloadingTo))
				return nullptr;
			else
				return FileInfo(Entry->DownloadingTo).DirectoryName;
		}
		virtual String ^FilenameForProgress() override
		{
			return "";
		}
		bool SameAs2(AIOuTorrenting ^o)
		{
			return (o->Entry == Entry) ;
		}
		virtual bool SameAs(AIOItem ^o) override
		{
			return (this->Type == o->Type) && SameAs2(safe_cast<AIOuTorrenting ^>(o));
		}
		virtual int IconNumber() override
		{
			return 2;
		}

		virtual ListViewItem ^GetLVI(ListView ^lv) override;

		virtual bool Action(TVDoc ^doc) override
		{
			Done = true;
			return true;
		}
	};



	public ref class AIOSorter: public Collections::Generic::IComparer<AIOItem ^>
	{
	public:
		virtual int Compare(AIOItem ^ x, AIOItem^ y)
		{
			if (x->Type == y->Type) 
			{
				if (x->Type == AIOType::kCopyMoveRename )
				{
					AIOCopyMoveRename ^xx = safe_cast<AIOCopyMoveRename ^>(x);
					AIOCopyMoveRename ^yy = safe_cast<AIOCopyMoveRename ^>(y);

					String ^s1 = xx->From->FullName+( xx->From->Directory->Root->FullName != xx->To->Directory->Root->FullName ? "0" : "1" );
					String ^s2 = yy->From->FullName+( yy->From->Directory->Root->FullName != yy->To->Directory->Root->FullName ? "0" : "1" );

					return s1->CompareTo(s2);
				}
				if (x->Type == AIOType::kDownload)
				{
					AIODownload ^xx = safe_cast<AIODownload ^>(x);
					AIODownload ^yy = safe_cast<AIODownload ^>(y);

					return xx->Destination->FullName->CompareTo(yy->Destination->FullName);
				}
				if (x->Type == AIOType::kRSS)
				{
					AIORSS ^xx = safe_cast<AIORSS ^>(x);
					AIORSS ^yy = safe_cast<AIORSS ^>(y);

					return xx->RSS->URL->CompareTo(yy->RSS->URL);
				}
				if (x->Type == AIOType::kMissing)
				{
					AIOMissing ^xx = safe_cast<AIOMissing ^>(x);
					AIOMissing ^yy = safe_cast<AIOMissing ^>(y);

					return (xx->TheFileNoExt+xx->PE->Name)->CompareTo(yy->TheFileNoExt+yy->PE->Name);
				}
				if (x->Type == AIOType::kNFO)
				{
					AIONFO ^xx = safe_cast<AIONFO ^>(x);
					AIONFO ^yy = safe_cast<AIONFO ^>(y);

					if (xx->PE == nullptr)
						return 1;
					else if (yy->PE == nullptr)
						return -1;
					else
					    return (xx->Where->FullName+xx->PE->Name)->CompareTo(yy->Where->FullName+yy->PE->Name);
				}
				if (x->Type == AIOType::kuTorrenting)
				{
					AIOuTorrenting ^xx = safe_cast<AIOuTorrenting ^>(x);
					AIOuTorrenting ^yy = safe_cast<AIOuTorrenting ^>(y);

					if (xx->PE == nullptr)
						return 1;
					else if (yy->PE == nullptr)
						return -1;
					else
					    return (xx->DesiredLocationNoExt)->CompareTo(yy->DesiredLocationNoExt);
				}
				Diagnostics::Debug::Fail( "Unknown type in AIOItem::Compare" ); // uhoh
				return 1;
			}
			else
			{
				// different types
				return ((int)x->Type - (int)y->Type);
			}
		}

	};

	typedef Generic::List<AIOItem ^> AIOList;

	public ref class LVResults
	{
	public:
		Generic::List<AIOuTorrenting ^> ^uTorrenting;
		Generic::List<AIOMissing ^> ^Missing;
		Generic::List<AIORSS ^> ^RSS;
		Generic::List<AIOCopyMoveRename ^> ^Rename;
		Generic::List<AIOCopyMoveRename ^> ^CopyMove;
		Generic::List<AIODownload ^> ^Download;
		Generic::List<AIONFO ^> ^NFO;
		AIOList ^FlatList;
		bool AllSame;
		int Count;

		LVResults(ListView ^lv, bool checked) // if not checked, then selected items
		{
			uTorrenting = gcnew Generic::List<AIOuTorrenting ^>();
			Missing = gcnew Generic::List<AIOMissing ^>();
			RSS = gcnew Generic::List<AIORSS ^>();
			CopyMove = gcnew Generic::List<AIOCopyMoveRename ^>();
			Rename = gcnew Generic::List<AIOCopyMoveRename ^>();
			Download = gcnew Generic::List<AIODownload ^>();
			NFO = gcnew Generic::List<AIONFO ^>();
			FlatList = gcnew AIOList();

			Generic::List<ListViewItem ^> ^sel = gcnew Generic::List<ListViewItem ^>();
			if (checked)
			{
				ListView::CheckedListViewItemCollection ^ss = lv->CheckedItems;
				for each (ListViewItem ^lvi in ss)
					sel->Add(lvi);
			}
			else
			{
				ListView::SelectedListViewItemCollection ^ss = lv->SelectedItems;
				for each (ListViewItem ^lvi in ss)
					sel->Add(lvi);
			}

			Count = sel->Count;

			if (sel->Count == 0)
				return;

			AIOType t = safe_cast<AIOItem ^>(sel[0]->Tag)->Type;

			AllSame = true;
			for each (ListViewItem ^lvi in sel)
			{
				AIOItem ^aio = safe_cast<AIOItem ^>(lvi->Tag);
				FlatList->Add(aio);
				AIOType t2 = aio->Type;
				if (t2 != t)
					AllSame = false;

				switch (t2)
				{
				case AIOType::kCopyMoveRename: 
					{
						AIOCopyMoveRename ^cmr = safe_cast<AIOCopyMoveRename ^>(aio);
						if (cmr->Operation == AIOCopyMoveRename::Op::Rename)
							Rename->Add(safe_cast<AIOCopyMoveRename ^>(aio));
						else // copy/move
							CopyMove->Add(safe_cast<AIOCopyMoveRename ^>(aio));
						break;
					}
				case AIOType::kDownload: 
					Download->Add(safe_cast<AIODownload ^>(aio));
					break;
				case AIOType::kRSS: 
					RSS->Add(safe_cast<AIORSS ^>(aio));
					break;
				case AIOType::kMissing: 
					Missing->Add(safe_cast<AIOMissing ^>(aio));
					break;
				case AIOType::kNFO:
					NFO->Add(safe_cast<AIONFO ^>(aio));
					break;
				case AIOType::kuTorrenting:
					uTorrenting->Add(safe_cast<AIOuTorrenting ^>(aio));
					break;
				}
			}
		}
	};
} // namespace


