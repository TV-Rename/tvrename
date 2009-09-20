//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "stdafx.h"

namespace TVRename
{
    static String ^DisplayVersionString() 
    {
		String ^v = "2.2.0a7";


#ifdef MONOSTUFF
        return v + " (Mono)";
#elif _DEBUG > 0
		return v + " (Debug)";
#else
		return v;
#endif
    }
	
	static bool ForceExperimentalOn()
	{
		return true; // ************************
	}
}

