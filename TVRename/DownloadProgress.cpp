//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#include "StdAfx.h"
#include "DownloadProgress.h"
#include "TVDoc.h"

namespace TVRename
{
  DownloadProgress::DownloadProgress(TVDoc ^doc)
  {
    InitializeComponent();

    mDoc = doc;
  }

  System::Void DownloadProgress::bnCancel_Click(System::Object^  sender, System::EventArgs^  e) 
  {
    tmrUpdate->Stop();
    mDoc->StopBGDownloadThread();
    this->DialogResult = ::DialogResult::Abort;
  }

  System::Void DownloadProgress::tmrUpdate_Tick(System::Object^  sender, System::EventArgs^  e) 
  {
      if (mDoc->DownloadDone)
          Close();
      else
          UpdateStuff();
  }


  void DownloadProgress::UpdateStuff()
  {
      txtCurrent->Text = mDoc->GetTVDB(false,"")->CurrentDLTask;
      pbProgressBar->Value = mDoc->DownloadPct;
  }
}
