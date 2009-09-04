#pragma once

#include "stdafx.h"

namespace TVRename
{
    static String ^DisplayVersionString() 
    {
		String ^v = "2.2.0a5";


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

