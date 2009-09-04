#pragma once

#include "TheTVDB.h"
#include "ShowItem.h"

namespace TVRename
{

    //ref class FolderItem;

    public ref class MissingEpisode :
        public ProcessedEpisode
        {
        public:
            ShowItem ^SI;
            int Season;
            SeriesInfo ^TheSeries;
            String ^WhereItBelongs;
            String ^FullNameWithPath;

            MissingEpisode(ShowItem ^show, int season, SeriesInfo ^ser, ProcessedEpisode ^e, String ^whereBelongs, String ^nameWithPath) :
            ProcessedEpisode(e)
            {
                WhereItBelongs = whereBelongs;
                SI = show;
                Season = season;
                TheSeries = ser;
                FullNameWithPath = nameWithPath;
            }
        };

        typedef Generic::List<MissingEpisode ^> MissingEpisodeList;
}


