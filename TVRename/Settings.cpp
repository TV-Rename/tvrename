#include "stdafx.h"

#include "Settings.h"
#include "ShowItem.h"

using namespace System;


namespace TVRename
{

String ^TVSettings::BTSearchURL(ProcessedEpisode ^epi)
        {
            if (epi == nullptr)
                return "";

            SeriesInfo ^s = epi->TheSeries;
            if (s == nullptr)
                return "";

            String ^url = TheSearchers->CurrentSearchURL();
            String ^seas = epi->SeasonNumber.ToString();
            int e = epi->EpNum;
            String ^ep = (e > 0) ? (" " + e.ToString()) : "";
            String ^s2 = s->Name + " " + seas + ep;
            s2 = s2->Replace(" ","+");
            //url = url->Replace("!", s2);
            return CustomName::NameFor(epi, url);
        }



} // namespace