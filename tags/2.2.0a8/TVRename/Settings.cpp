//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
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
            return CustomName::NameForNoExt(epi, url, true);
        }



} // namespace