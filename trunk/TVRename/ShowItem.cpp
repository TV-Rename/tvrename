//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
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


