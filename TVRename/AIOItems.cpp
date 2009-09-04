#include "StdAfx.h"
#include "AIOItems.h"
#include "TVDoc.h"

namespace TVRename
{

	bool AIORSS::Action(TVDoc ^doc)
		{
			System::Net::WebClient ^wc = gcnew System::Net::WebClient();
            try {
                array<unsigned char> ^r = wc->DownloadData(RSS->URL);
				if ((r == nullptr) || (r->Length == 0))
				{
					HasError = true;
					ErrorText = "No data downloaded";
				}

				String ^saveTemp = Path::GetTempPath() + "\\" + doc->FilenameFriendly(RSS->Title);
				if (FileInfo(saveTemp).Extension->ToLower() != "torrent")
					saveTemp += ".torrent";
				File::WriteAllBytes(saveTemp, r);

                System::Diagnostics::Process::Start(doc->Settings->uTorrentPath,"/directory \"" + FileInfo(TheFileNoExt).Directory->FullName + "\" \"" + saveTemp + "\"");

				HasError = false;
            }
            catch (Exception ^e)
            {
				ErrorText = e->Message;
				HasError = true;
            }
			Done = true;
			
			return !HasError;
		}


	
	ListViewItem ^AIOuTorrenting::GetLVI(ListView ^lv)
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

			lvi->SubItems->Add(Entry->TorrentFile);
			lvi->SubItems->Add(Entry->DownloadingTo);
			int p = Entry->PercentDone;
			lvi->SubItems->Add(p == -1 ? "" : Entry->PercentDone.ToString()+"% Complete");

			lvi->Group = lv->Groups[7];
			lvi->Tag = this;

		//	lv->Items->Add(lvi);
			return lvi;
		}


}
