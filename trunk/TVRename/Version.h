#pragma once

#include "stdafx.h"

namespace TVRename
{
    static bool IncludeExperimentalStuff()
    {
#ifdef DEBUG
        return true;
#else
        return false; // *********************
#endif
    }

    static String ^DisplayVersionString() 
    {
        return "2.1.0";
    }
}

