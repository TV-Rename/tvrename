//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#include "stdafx.h"
#include "CustomName.h"
#include "ShowItem.h"

namespace TVRename
{

    StringList ^CustomName::Tags()
        {
            StringList ^res = gcnew StringList();

            res->Add("{ShowName}");
            res->Add("{Season}");
            res->Add("{Season:2}");
            res->Add("{Episode}");
            res->Add("{Episode2}");
            res->Add("{EpisodeName}");
            res->Add("{Number}");
            res->Add("{Number:2}");
            res->Add("{Number:3}");
            res->Add("{ShortDate}");
            res->Add("{LongDate}");
            res->Add("{YMDDate}");

            return res;
        }

        String ^CustomName::NameForNoExt(ProcessedEpisode ^pe, String ^styleString, bool urlEncode)
        {
            String ^name = gcnew String(styleString);

			String ^showname = pe->SI->ShowName();
			String ^epname = pe->Name;
			if (urlEncode)
			{
				showname = System::Web::HttpUtility::UrlEncode(showname);
				epname = System::Web::HttpUtility::UrlEncode(epname);
			}

            name = name->Replace("{ShowName}",showname);
            name = name->Replace("{Season}",pe->SeasonNumber.ToString());
            name = name->Replace("{Season:2}",pe->SeasonNumber.ToString("00"));
            name = name->Replace("{Episode}",pe->EpNum.ToString("00"));
            name = name->Replace("{Episode2}",pe->EpNum2.ToString("00"));
            name = name->Replace("{EpisodeName}",epname);
            name = name->Replace("{Number}",pe->OverallNumber.ToString());
            name = name->Replace("{Number:2}",pe->OverallNumber.ToString("00"));
            name = name->Replace("{Number:3}",pe->OverallNumber.ToString("000"));
            DateTime ^dt = pe->GetAirDateDT(false);
            if (dt !=  nullptr)
            {
                name = name->Replace("{ShortDate}",dt->ToString("d"));
                name = name->Replace("{LongDate}",dt->ToString("D"));
				String ^ymd = dt->ToString("yyyy/MM/dd");
				if (urlEncode)
					ymd = System::Web::HttpUtility::UrlEncode(ymd);
                name = name->Replace("{YMDDate}",ymd);
            }
            else
            {
                name = name->Replace("{ShortDate}","---");
                name = name->Replace("{LongDate}","------");
				String ^ymd = "----/--/--";
				if (urlEncode)
					ymd = System::Web::HttpUtility::UrlEncode(ymd);
                name = name->Replace("{YMDDate}",ymd);
            }

            if (pe->EpNum2 == pe->EpNum)
				name = Regex::Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]","$1"); // remove optional parts
			else
				name = Regex::Replace(name, "([^\\\\])\\[(.*?[^\\\\])\\]","$1$2"); // remove just the brackets

			name = name->Replace("\\[","[");
			name = name->Replace("\\]","]");

            return name;
        }

} // namespace