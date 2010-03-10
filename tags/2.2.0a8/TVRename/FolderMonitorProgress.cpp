//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#include "StdAfx.h"
#include "FolderMonitorProgress.h"
#include "UI.h"

namespace TVRename
{
    FolderMonitorProgress::FolderMonitorProgress(FolderMonitor ^thefm)
  {
      mFM = thefm;

    InitializeComponent();
  }

  System::Void FolderMonitorProgress::bnCancel_Click(System::Object^  sender, System::EventArgs^  e) 
  {
    this->DialogResult = ::DialogResult::Abort;
    mFM->FMPStopNow = true;
  }
  System::Void FolderMonitorProgress::timer1_Tick(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (mFM->FMPStopNow)
                     this->Close();
             }

}
