//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#include "stdafx.h"
#include "TheTVDB.h"

namespace TVRename
{
    Season::Season(SeriesInfo ^theSeries, int number, int seasonid)
    {
        TheSeries = theSeries;
        SeasonNumber = number;
        SeasonID = seasonid;
        Episodes = gcnew EpisodeList();
    }


    System::DateTime ^Episode::GetAirDateDT(bool correct)
    {
        if (FirstAired == nullptr)
            return nullptr;

        DateTime ^dt = (TheSeries->AirsTime != nullptr) ? gcnew DateTime(FirstAired->Year, FirstAired->Month, FirstAired->Day,
            TheSeries->AirsTime->Hour, TheSeries->AirsTime->Minute, 0, 0) : 
        gcnew DateTime(FirstAired->Year, FirstAired->Month, FirstAired->Day, 20, 0, 0, 0);

        if (!correct)
            return dt;

        // do timezone adjustment
        return TZMagic::AdjustTZTimeToOurs(dt, TheSeries->GetTZI());
    }

	Episode::Episode(SeriesInfo ^ser, Season ^seas, XmlReader ^r)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

			try
			{
				SetDefaults(ser, seas);

				r->Read();
				if (r->Name != "Episode")
					return;

				r->Read();
				while (!r->EOF)
				{
					if ((r->Name == "Episode") && (!r->IsStartElement()))
						break;
					if (r->Name == "id")
						EpisodeID = r->ReadElementContentAsInt();
					if (r->Name == "seriesid")
						SeriesID = r->ReadElementContentAsInt();  // thetvdb series id
					if (r->Name == "seasonid")
						SeasonID = r->ReadElementContentAsInt();
					else if (r->Name == "EpisodeNumber")
						EpNum = r->ReadElementContentAsInt();
					else if (r->Name == "SeasonNumber")
						ReadSeasonNum = r->ReadElementContentAsInt();
					else if (r->Name == "lastupdated")
						Srv_LastUpdated = r->ReadElementContentAsInt();
					else if (r->Name == "Overview")
						Overview = ReadStringFixQuotesAndSpaces(r);
					else if (r->Name == "EpisodeName")
						Name = ReadStringFixQuotesAndSpaces(r);
					else if (r->Name == "FirstAired")
					{
						try 
						{
							FirstAired = DateTime::ParseExact(r->ReadElementContentAsString(),"yyyy-MM-dd",
								gcnew System::Globalization::CultureInfo(""));
						}
						catch (...)
						{
							FirstAired = nullptr;
						}
					}					
					else
					{
						if ((r->IsEmptyElement) || !r->IsStartElement())
							r->ReadOuterXml();
						else
						{
							XmlReader ^r2 = r->ReadSubtree();
							r2->Read();
							String ^name = r2->Name;
							Items[name] = r2->ReadElementContentAsString();
							r->Read();
						}
					}
				}
			}
			catch (XmlException ^e)
			{
				String ^message = "Error processing data from TheTVDB for an episode.";
				if (SeriesID != -1)
					message += "\r\nSeries ID: "+SeriesID;
				if (EpisodeID != -1)
					message += "\r\nEpisode ID: "+EpisodeID.ToString();
				if (EpNum != -1)
					message += "\r\nEpisode Number: "+EpNum.ToString();
				if (!String::IsNullOrEmpty(Name))
					message += "\r\nName: "+Name;

				message += "\r\n"+e->Message;

				MessageBox::Show(message,"TVRename", MessageBoxButtons::OK, MessageBoxIcon::Error);
			}
		} // episode constructor
}
