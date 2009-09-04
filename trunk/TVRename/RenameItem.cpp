#include "StdAfx.h"
#include "RenameItem.h"

#include "Sorters.h"


namespace TVRename
{
  void SortSmartly(RCList ^rcl)
    {
      rcl->Sort(gcnew MoveAndCopySorter());
    }
} // namespace