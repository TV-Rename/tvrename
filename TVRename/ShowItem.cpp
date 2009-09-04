#include "stdafx.h"
#pragma hdrstop

#include "ShowItem.h"


namespace TVRename
{

int CompareShowItemNames(ShowItem ^one, ShowItem ^two)
    {
      String ^ones = one->ShowName();// + " " +one->SeasonNumber.ToString("D3");
      String ^twos = two->ShowName();// + " " +two->SeasonNumber.ToString("D3");
      return ones->CompareTo(twos);
    }

} // namespace


