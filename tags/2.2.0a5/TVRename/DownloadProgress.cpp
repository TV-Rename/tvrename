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
