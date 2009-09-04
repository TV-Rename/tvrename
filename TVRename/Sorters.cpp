#include "stdafx.h"

#include "Sorters.h"
#include "RenameItem.h"

namespace TVRename
{

int MoveAndCopySorter::Compare( RCItem ^ xx, RCItem^ yy )
    {
      String ^s1 = xx->FromFolder+xx->FromName+( xx->FromFolder->Substring(0,2) != xx->ToFolder->Substring(0,2) ? "0" : "1" );
      String ^s2 = yy->FromFolder+yy->FromName+( yy->FromFolder->Substring(0,2) != yy->ToFolder->Substring(0,2) ? "0" : "1" );
      return s1->CompareTo(s2);
    }

} // namespace
