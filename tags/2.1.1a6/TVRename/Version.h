#pragma once

#include "stdafx.h"

namespace TVRename
{
    static String ^DisplayVersionString() 
    {
		String ^v = "2.1.1a6";


#ifdef MONOSTUFF
        return v + " (Mono)";
#else
		return v;
#endif
    }
	
	static bool ForceExperimentalOn()
	{
		return true; // ************************
	}
}

