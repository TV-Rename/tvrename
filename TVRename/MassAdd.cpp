#include "StdAfx.h"
#include "MassAdd.h"
#include "TVDoc.h"

namespace TVRename 
{

  System::Void MassAdd::bnOK_Click(System::Object^  sender, System::EventArgs^  e) 
  {
    // refuse to do OK click until nothing busted in list
    for (int i=0;i<mCandidates->Count();i++)
    {
      if (!mCandidatesIgnore[i] && mCandidates[i]->Busted())
      {
        System::Windows::Forms::DialogResult dr = MessageBox::Show("There are starred items in the list.  Continue, and automatically skip them?","Warning",MessageBoxButtons::YesNo, MessageBoxIcon::Warning);
        if (dr == System::Windows::Forms::DialogResult::No)
          return;
        break;
      }
    }

    FolderList ^fl = mDoc->GetFolderList();
    MonitorList ^ml = mDoc->GetMonitorList();

    for (int i=0;i<mCandidates->Count();i++)
    {
      FolderItem ^fi = mCandidates[i];

     if (mCandidatesIgnore[i])
        ml->Add(gcnew MonitorItem(fi->GetFolder(), true));
      else if (!fi->Busted())
        fl->Add(fi);
    }
    ml->Save();
    fl->Save();
    mDoc->MegaRefill(true,false,false, false, false);
    this->Close();
  }


  System::Void MassAdd::bnEdit_Click(System::Object^  sender, System::EventArgs^  e) 
  {
    if (lvTheList->SelectedItems->Count)
    {
      int idx = safe_cast<int>(lvTheList->SelectedItems[0]->Tag);
      FolderItem ^fi = gcnew FolderItem(mCandidates->GetNum(idx));
      AddEditFolder ^aef = gcnew AddEditFolder(fi, fi->GetFolder(), fi->Busted());
      System::Windows::Forms::DialogResult dr = aef->ShowDialog();
      if (dr == System::Windows::Forms::DialogResult::OK)
      {
        mCandidates->ReplaceNum(idx,fi);
        FillList();
      }
    }
  }


} // namespace