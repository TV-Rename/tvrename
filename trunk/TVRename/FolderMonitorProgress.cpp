#include "StdAfx.h"
#include "FolderMonitorProgress.h"
#include "UI.h"

namespace TVRename
{
    FolderMonitorProgress::FolderMonitorProgress(UI ^theui)
  {
      mUI = theui;

    InitializeComponent();
  }

  System::Void FolderMonitorProgress::bnCancel_Click(System::Object^  sender, System::EventArgs^  e) 
  {
    this->DialogResult = ::DialogResult::Abort;
    mUI->FMPStopNow = true;
  }
  System::Void FolderMonitorProgress::timer1_Tick(System::Object^  sender, System::EventArgs^  e) 
             {
                 if (mUI->FMPStopNow)
                     this->Close();
             }

}
