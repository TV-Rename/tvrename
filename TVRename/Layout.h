#pragma once

namespace TVRename
{

  //enum { kUnknown = 0, kMDI = 1, kWatchFolders = 2, kUpcoming = 3, kRename = 4, kMonitorFolders = 5, kTorrentMatch = 6, kEPFinder = 7 };

  public ref class LayoutInfo
  {
  public:
    Generic::List<int> ^mColWidths;
    int mWindowID;
    bool mMaximised;
    Size mSize;
    Point mLocation;
    bool mLocked;

    LayoutInfo()
    {
      mColWidths = gcnew Generic::List<int>;
      mWindowID = kUnknown;
      mMaximised = false;
      mSize = Size(-1,-1);
      mLocation = Point(-1,-1);
      mLocked = true;
     }

    bool Locked()
    {
      return mLocked;
    }

    void Unlock()
    {
      mLocked = false;
    }
    void Lock()
    {
      mLocked = true;
    }

    void Save(StreamWriter ^sw)
    {
      sw->WriteLine("WindowID="+mWindowID);
      sw->WriteLine("Maximised=" + (mMaximised?"1":"0"));
      if (!mMaximised)
      {
        sw->WriteLine("Size=" + mSize.Width+" "+mSize.Height);
        sw->WriteLine("Location=" + mLocation.X + " " + mLocation.Y);
      }
      sw->Write("ColWidths=");
      for (int i=0;i<mColWidths->Count;i++)
        sw->Write(mColWidths[i] + " ");
      sw->WriteLine();
      sw->WriteLine("--");
    }

    bool Load(StreamReader ^sr)
    {
      String ^l1;
      while ((l1 = sr->ReadLine()) != nullptr)
      {
        String ^what;
        if (l1 == "--")
          return true;
        int p = l1->IndexOf('=');
        if (p != -1)
        {
          what = l1->Substring(0,p);
          l1 = l1->Substring(p+1);
          if (what == "ColWidths")
          {
            int c = 0;
            while ((p = l1->IndexOf(' ')) != -1)
            {
              int n = Convert::ToInt32(l1->Substring(0,p));
              l1 = l1->Substring(p+1);
              mColWidths->Add(n);
            }
          } else if (what == "Maximised")
          {
            mMaximised = l1 == "1";
          } else if (what == "Size")
          {
            p = l1->IndexOf(' ');
            int x = Convert::ToInt32(l1->Substring(0,p));
            int y = Convert::ToInt32(l1->Substring(p+1));
            mSize = System::Drawing::Size(x,y);
          } else if (what == "Location")
          {
            p = l1->IndexOf(' ');
            int x = Convert::ToInt32(l1->Substring(0,p));
            int y = Convert::ToInt32(l1->Substring(p+1));
            mLocation = Point(x,y);
          } else if (what == "WindowID")
          {
            int n = Convert::ToInt32(l1);
            mWindowID = n;
          }
        }
      }
    return false;
    }

    void SetFrom(Form ^f, ListView ^lv)
    {
      if (mLocked)
        return;

      mLocked = true;

      if (f != nullptr)
      {
           mSize = f->Size;
           mLocation = f->Location;
           mMaximised = f->WindowState == FormWindowState::Maximized;
      }

      mColWidths->Clear();
      if (lv != nullptr)
      {       
           for (int i=0;i<lv->Columns->Count;i++)
             mColWidths->Add(lv->Columns[i]->Width);
      }

      mLocked = false;
    }

    void Fixup(Form ^f, ListView ^lv)
    {
      if (f != nullptr)
      {
        if (mSize != Size(-1,-1))
          f->Size = mSize;
        if (mLocation != Point(-1,-1))
          f->Location = mLocation;
        f->WindowState = mMaximised ? FormWindowState::Maximized : FormWindowState::Normal;
       
      }
      if (lv != nullptr)
      {
        for (int i=0;i<Math::Min(mColWidths->Count,lv->Columns->Count);i++)
          lv->Columns[i]->Width = mColWidths[i];
      }
    }
  };


  public ref class Layout
  {
    Generic::List<LayoutInfo^> ^mLayouts;

  public:
    Layout()
    {
      mLayouts = gcnew Generic::List<LayoutInfo^>;
      Load();
    }

    LayoutInfo ^Get(int id)
    {
      for (int i=0;i<mLayouts->Count;i++)
        if (mLayouts[i]->mWindowID == id)
          return mLayouts[i];
      LayoutInfo ^li = gcnew LayoutInfo();
      li->mWindowID = id;
      mLayouts->Add(li);
      return li;
    }

    void Save()
    {
      StreamWriter ^sw = gcnew StreamWriter(System::Windows::Forms::Application::UserAppDataPath+"\\TVRenameLayout.dat");
      sw->WriteLine("Version=2");

      for (int i=0;i<mLayouts->Count;i++)
        mLayouts[i]->Save(sw);
      
      sw->Close();

    }

    void Load()
    {
      System::IO::StreamReader ^sr;
      try {
        sr = gcnew StreamReader(System::Windows::Forms::Application::UserAppDataPath+"\\TVRenameLayout.dat");
      } 
      catch (...) { return; }
      String ^l1;
      l1 = sr->ReadLine();
      if (l1 == nullptr)
        return;

      String ^what;
      int p = l1->IndexOf('=');
      if (p == -1)
        return;

      what = l1->Substring(0,p);
      l1 = l1->Substring(p+1);
      if ((what != "Version") && (l1 != "2"))
        return;

      for (;;)
      {
        LayoutInfo ^li = gcnew LayoutInfo();
        if (li->Load(sr))
        {
          for (int i=0;i<mLayouts->Count;i++)
            if (mLayouts[i]->mWindowID == li->mWindowID)
            {
              mLayouts->RemoveAt(i);
              break;
            }

          mLayouts->Add(li);
        }
        else
          break;
      }
      sr->Close();
    }
  };

}
