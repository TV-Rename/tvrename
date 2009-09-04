#pragma once

#include "ShowItem.h"

namespace TVRename {
  using namespace System;
  using namespace System::ComponentModel;
  using namespace System::Collections;
  using namespace System::Windows::Forms;
  using namespace System::Data;
  using namespace System::Drawing;
  using namespace System::IO;

  ref class TextSorter: public IComparer
  {
  private:
    int col;

  public:
    TextSorter()
    {
      col = 0;
    }

    TextSorter( int column )
    {
      col = column;
    }

    virtual int Compare( Object^ xx, Object^ yy )
    {
		ListViewItem ^x = dynamic_cast<ListViewItem^>(xx);
		ListViewItem ^y = dynamic_cast<ListViewItem^>(yy);
      return String::Compare( x->SubItems[ col ]->Text, y->SubItems[ col ]->Text );
    }
  };
  /*
  ref class TimeToNextSorter: public IComparer
  {
  private:
    int col;

  public:
    TimeToNextSorter()
    {
      col = 0;
    }

    TimeToNextSorter( int column )
    {
      col = column;
    }

    virtual int Compare( Object^ x, Object^ y )
    {
      try 
      {
        return (Convert::ToDouble((dynamic_cast<ListViewItem^>(x))->SubItems[ col ]->Text) >
          Convert::ToDouble((dynamic_cast<ListViewItem^>(y))->SubItems[ col ]->Text ) );
      }
      catch (...)
      {
        return String::Compare( (dynamic_cast<ListViewItem^>(x))->SubItems[ col ]->Text,
          (dynamic_cast<ListViewItem^>(y))->SubItems[ col ]->Text );
      }
    }
  };
	*/


  ref class DateSorterWTW: public IComparer
  {
  public:
    DateSorterWTW()
    {
    }

    virtual int Compare( Object^ x, Object^ y )
    {
      DateTime ^d1, ^d2;

      try
      {
        d1 = safe_cast<Episode ^>(dynamic_cast<ListViewItem^>(x)->Tag)->GetAirDateDT(true);
      }
      catch (...)
      {
        d1 = DateTime::Now;
      }

      try {
        d2 = safe_cast<Episode ^>(dynamic_cast<ListViewItem^>(y)->Tag)->GetAirDateDT(true);
      }
      catch (...)
      {
        d2 = DateTime::Now;
      }

      return d1->CompareTo(d2);
    }
  };
/*
was for missinglist
    ref class DateSorterML: public IComparer
  {
  private:
    int col;

  public:
    DateSorterML()
    {
      col = 0;
    }

    DateSorterML( int column )
    {
      col = column;
    }

    virtual int Compare( Object^ x, Object^ y )
    {
      DateTime ^d1= nullptr, ^d2 = nullptr;

      MissingEpisode ^me1 = safe_cast<MissingEpisode ^>(dynamic_cast<ListViewItem^>(x)->Tag);
      MissingEpisode ^me2 = safe_cast<MissingEpisode ^>(dynamic_cast<ListViewItem^>(y)->Tag);
      try
      {
        d1 = me1->GetAirDateDT(true);
      }
      catch (...)
      {
        d1 = nullptr;
      }

      try {
        d2 = me2->GetAirDateDT(true);
      }
      catch (...)
      {
        d2 = nullptr;
      }

      if ((d1 != nullptr) && (d2 != nullptr))
       return d1->CompareTo(d2);
      if (d1 == nullptr)
          return -1;
      else
          return 1;
    }
  };
*/
  ref class DaySorter: public IComparer
  {
  private:
    int col;

  public:
    DaySorter()
    {
      col = 0;
    }

    DaySorter( int column )
    {
      col = column;
    }

    virtual int Compare( Object^ x, Object^ y )
    {
      int d1 = 8, d2 = 8;


      try 
      {
        String ^t1 = (dynamic_cast<ListViewItem^>(x))->SubItems[ col ]->Text;
        String ^t2 = (dynamic_cast<ListViewItem^>(y))->SubItems[ col ]->Text;

        DateTime ^now  = DateTime::Now;

        for (int i=0;i<7;i++)
        {
          if ((*now+TimeSpan(i,0,0,0)).ToString("ddd") == t1)
            d1 = i;
           if ((*now+TimeSpan(i,0,0,0)).ToString("ddd") == t2)
            d2 = i;
        }
      }
      catch (...)
      {
      }

      return d1-d2;
    }
  };

ref class NumberAsTextSorter: public IComparer
  {
  private:
    int col;

  public:
    NumberAsTextSorter()
    {
      col = 0;
    }

    NumberAsTextSorter( int column )
    {
      col = column;
    }

    virtual int Compare( Object^ x, Object^ y )
    {
      int one,two;
      String ^s1 = dynamic_cast<ListViewItem^>(x)->SubItems[ col ]->Text;
      String ^s2 = dynamic_cast<ListViewItem^>(y)->SubItems[ col ]->Text;
      if (String::IsNullOrEmpty(s1))
        s1 = "-1";
      if (String::IsNullOrEmpty(s2))
        s2 = "-1";
      
      try
      {
        one = System::Convert::ToInt32(s1);
      }
      catch (...)
      {
        one = 0;
      }
      try
      {
        two = System::Convert::ToInt32(s2);
      }
      catch (...)
      {
        two = 0;
      }

      return one-two;
    }
  };


  //ref class RCItem;

  //public ref class MoveAndCopySorter: public Collections::Generic::IComparer<RCItem ^>
  //{
  //public:
  //  MoveAndCopySorter()
  //  {
  //  }

  //  virtual int Compare( RCItem ^ xx, RCItem^ yy );
  //};


}
