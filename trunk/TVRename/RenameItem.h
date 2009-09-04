#pragma once

#include "TheTVDB.h"

using System::String;
using namespace System::Collections;
using namespace System::IO;

namespace TVRename
{
  enum RCOperation { rcCopy, rcMove, rcRename };

  ref class ProcessedEpisode;

  public ref class RCItem  
  {
  public:
    RCOperation Operation;
    String ^FromFolder;
    String ^FromName;
    String ^ToFolder;
    String ^ToName;
    String ^ShowName;
    ProcessedEpisode ^TheEpisode;
    String ^LastError;
    // bool Enabled;

  public:
    RCItem(String ^showName, RCOperation operation, String ^fromFolder, String ^fromName, String ^toFolder, String ^toName, 
        ProcessedEpisode ^ep)
    {
        LastError = "";
      ShowName = showName;
      Operation = operation;
      FromFolder = fromFolder;
      FromName = fromName;
      ToFolder = toFolder;
      ToName = toName;
      //mEnabled = true;
	  TheEpisode = ep;
    }

    bool SameSource(RCItem ^o)
    {
        return ( (FromFolder == o->FromFolder) &&
            (FromName == o->FromName) );
    }
	bool SameAs(RCItem ^o)
	{
		return ( (ShowName = o->ShowName) &&
			(Operation == o->Operation) &&
			(FromFolder == o->FromFolder) &&
			(FromName == o->FromName) &&
			(ToFolder == o->ToFolder) &&
			(ToName == o->ToName) );

	}

    String ^FullFromName()
    {
      return FromFolder+"\\"+FromName;
    }
    String ^FullToName()
    {
      return ToFolder+"\\"+ToName;
    }

    String ^GetOperationName()
    {
      switch (Operation)
      {
      case rcCopy: return "Copy";
      case rcMove: return "Move";
      case rcRename: return "Rename";
      default: return "Unknown";
      }
    }


    long long FileSize()
    {
      try 
      {
      return FileInfo(FromFolder+"\\"+FromName).Length;
      }
      catch (...)
      {
        return 1;
      }
    }
  };


  typedef Generic::List<RCItem ^> RCList;

  void SortSmartly(RCList ^rcl);
}
