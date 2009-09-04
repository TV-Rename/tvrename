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
}
