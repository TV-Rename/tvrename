#pragma once

#include "Helpers.h"

// http://wiki.theory.org/BitTorrentSpecification

namespace TVRename
{
  using namespace System;
  using namespace System::ComponentModel;
  using namespace System::Collections;
  using namespace System::Windows::Forms;
  using namespace System::Data;
  using namespace System::Drawing;
  using namespace System::IO;

  using System::Windows::Forms::TreeNodeCollection;
  using System::Windows::Forms::TreeNode;

  enum { kError, kDictionary, kDictionaryItem, kList, 
	 kListOrDictionaryEnd, kInteger, kString, kBTEOF };

public ref class BTItem abstract
{
public:
    int Type; // from enum

    BTItem(int type) :
    Type(type)
    {
    }

    virtual String ^AsText()
    {
        return String::Concat("Type=",Type.ToString());
    }

    virtual void Tree(TreeNodeCollection ^tn)
    {
        TreeNode ^n = gcnew TreeNode("BTItem:"+Type.ToString());
        tn->Add(n);
    }
    
    virtual void Write(Stream ^sw) = 0;
};


public ref class BTString :
  public BTItem
  {
  public:
    array<unsigned char> ^Data;

    BTString(String ^s) :
    BTItem(kString)
    {
        SetString(s);
    }

    BTString() :
    BTItem(kString)
    {
        Data = gcnew array<unsigned char>(0);
    }

    void SetString(String ^s)
    {
        Data = System::Text::Encoding::UTF8->GetBytes(s);
    }

    virtual String ^AsText() override
    {
      return "String="+AsString();
    }

    String ^AsString()
    {
      System::Text::Encoding ^enc = System::Text::Encoding::UTF8;
      return enc->GetString(Data);
    }

    array<unsigned char> ^StringTwentyBytePiece(int pieceNum)
    {
      array<unsigned char> ^res = gcnew array<unsigned char>(20);
      if ( ((pieceNum*20)+20) > Data->Length)
        return nullptr;
      Array::Copy(Data, pieceNum*20, res, 0, 20);
      return res;
    }

    
    static String ^CharsToHex(array<unsigned char > ^data, int start, int n)
    {
      String ^r = gcnew String("");
      for (int i=0;i<n;i++)
        r += (data[start+i]<16?"0":"") + data[start+i].ToString("x")->ToUpper();
      return r;
    }


    String ^PieceAsNiceString(int pieceNum)
    {
      return CharsToHex(Data, pieceNum * 20, 20);
      /*String ^r = gcnew String("");
      for (int i=0;i<20;i++,p++)
        r += (Data[p]<16?"0":"") + Data[p].ToString("x")->ToUpper();

      return r;*/
    }

    virtual void Tree(TreeNodeCollection ^tn) override
    {
      TreeNode ^n = gcnew TreeNode(String::Concat("String:",AsString()));
      tn->Add(n);
    }

    virtual void Write(Stream ^sw) override
    {
        // Byte strings are encoded as follows: <string length encoded in base ten ASCII>:<string data>

        array<Byte>^len = System::Text::Encoding::ASCII->GetBytes( Data->Length.ToString() );
        sw->Write(len, 0, len->Length);
        sw->WriteByte((unsigned char)':');
        sw->Write(Data, 0, Data->Length);
    }
  };

  public ref class BTEOF :
      public BTItem
      {
      public:
          BTEOF() :
            BTItem(kBTEOF)
            {
            }
            virtual void Write(Stream ^sw) override
            {
            }

      };

public ref class BTError :
    public BTItem
{
public:
  String ^Message;

  BTError() :
    BTItem(kError)
  {
    Message = gcnew String("");
  }
    virtual String ^AsText() override 
    {
      return String::Concat("Error:",Message);
    }
    virtual void Tree(TreeNodeCollection ^tn) override
    {
      TreeNode ^n = gcnew TreeNode("BTError:"+Message);
      tn->Add(n);
    }
    virtual void Write(Stream ^sw) override
    {
    }
};

public ref class BTListOrDictionaryEnd :
  public BTItem
{
public:
  BTListOrDictionaryEnd() :
    BTItem(kListOrDictionaryEnd)
  {
    }
    virtual void Write(Stream ^sw) override
    {
        sw->WriteByte((unsigned char)'e');
    }

};

public ref class BTDictionaryItem :
  public BTItem
{
public:
  String ^Key;
  BTItem ^Data;

  BTDictionaryItem() :
    BTItem(kDictionaryItem)
  {
  }

    BTDictionaryItem(String ^k, BTItem ^d) :
    BTItem(kDictionaryItem)
  {
      Key = k;
      Data = d;
  }

    virtual String ^AsText() override 
    {
      if ((Key == "pieces") && (Data->Type == kString))
      {
        return "<File hash data>";
      }
      else
        return String::Concat(Key,"=>",Data->AsText());
    }

    virtual void Tree(TreeNodeCollection ^tn) override
    {
      if ((Key == "pieces") && (Data->Type == kString))
      {
        // 20 byte chunks of SHA1 hash values
        TreeNode ^n = gcnew TreeNode("Key="+Key);
        tn->Add(n);
        n->Nodes->Add(gcnew TreeNode("<File hash data>" + safe_cast<BTString ^>(Data)->PieceAsNiceString(0)));
      }
      else
      {
        TreeNode ^n = gcnew TreeNode("Key="+Key);
        tn->Add(n);
        Data->Tree(n->Nodes);
      }
    }
        virtual void Write(Stream ^sw) override
    {
        BTString(Key).Write(sw);
        Data->Write(sw);
    }
};

public ref class BTDictionary :
  public BTItem
{
public:
  Generic::List<BTDictionaryItem ^> ^Items;

  BTDictionary() :
    BTItem(kDictionary)
  {
    Items = gcnew Generic::List<BTDictionaryItem ^>;
  }
    virtual String ^AsText() override
    {
      String ^r = gcnew String("Dictionary=[");
      for (int i=0;i<Items->Count;i++)
      {
        r += Items[i]->AsText();
        if (i != (Items->Count-1))
          r +=",";
      }
      r+="]";
      return r;
    }
 virtual void Tree(TreeNodeCollection ^tn) override
 {
    TreeNode ^n = gcnew TreeNode("Dictionary");
    tn->Add(n);
    for (int i=0;i<Items->Count;i++)
      Items[i]->Tree(n->Nodes);    
  }

 bool RemoveItem(String ^key)
 {
     for (int i=0;i<Items->Count;i++)
     {
         if (Items[i]->Key == key) 
         {
             Items->RemoveAt(i);
             return true;
         }
     }
     return false;
 }
 BTItem ^GetItem(String ^key)
  {
      return GetItem(key, false);
 }
  BTItem ^GetItem(String ^key, bool ignoreCase)
  {
    for (int i=0;i<Items->Count;i++)
      if ((Items[i]->Key == key) ||
          (ignoreCase && ((Items[i]->Key->ToLower() == key->ToLower()))))
        return Items[i]->Data;
    return nullptr;
 }
     virtual void Write(Stream ^sw) override
    {
        sw->WriteByte((unsigned char)'d');
        for each (BTItem ^i in Items)
            i->Write(sw);
        sw->WriteByte((unsigned char)'e');
    }

};

public ref class BTList :
  public BTItem
{
public:
  Generic::List<BTItem ^> ^Items;

  BTList() :
    BTItem(kList)
  {
    Items = gcnew Generic::List<BTItem ^>;
  }

    virtual String ^AsText() override
    {
      String ^r = gcnew String("List={");
      for (int i=0;i<Items->Count;i++)
      {
        r+=Items[i]->AsText();
        if (i != (Items->Count-1))
          r +=",";
      }
      r+="}";
      return r;
    }
 virtual void Tree(TreeNodeCollection ^tn) override
 {
    TreeNode ^n = gcnew TreeNode("List");
    tn->Add(n);
    for (int i=0;i<Items->Count;i++)
      Items[i]->Tree(n->Nodes);    
  }
     virtual void Write(Stream ^sw) override
    {
        sw->WriteByte((unsigned char)'l');
        for each (BTItem ^i in Items)
            i->Write(sw);
        sw->WriteByte((unsigned char)'e');
    }
};

public ref class BTInteger :
  public BTItem
{
public:
  Int64 Value;

  BTInteger() :
    BTItem(kInteger)
  {
    Value = 0;
  }

    BTInteger(Int64 n) :
  BTItem(kInteger)
  {
      Value = n;
  }

    virtual String ^AsText() override
    {
      return "Integer="+Value;
    }
        virtual void Tree(TreeNodeCollection ^tn) override
    {
      TreeNode ^n = gcnew TreeNode("Integer:"+Value.ToString());
      tn->Add(n);
    }
        virtual void Write(Stream ^sw) override
    {
        sw->WriteByte((unsigned char)'i');
        cli::array<unsigned char> ^b = System::Text::Encoding::ASCII->GetBytes( Value.ToString() ) ;
        sw->Write(b, 0, b->Length);
        sw->WriteByte((unsigned char)'e');
    }
  };

  public ref class BTFile
{
public:
  Generic::List<BTItem ^> ^Items;

  BTFile()
  {
    Items = gcnew Generic::List<BTItem ^>;
  }

  String ^AsText()
      { 
    String ^res = gcnew String("File= ");
    for (int i=0;i<Items->Count;i++)
      res += Items[i]->AsText() + " ";
    return res;
      }

  void Tree(TreeNodeCollection ^tn)
      {
          TreeNode ^n = gcnew TreeNode("BT File");
          tn->Add(n);
          for (int i=0;i<Items->Count;i++)
              Items[i]->Tree(n->Nodes);    
      }

   BTItem ^GetItem(String ^key)
  {
      return GetItem(key, false);
 }

   BTDictionary ^GetDict()
   {
       Diagnostics::Debug::Assert(Items->Count == 1);
       Diagnostics::Debug::Assert(Items[0]->Type == kDictionary);

       // our first (and only) Item will be a dictionary of stuff
       return safe_cast<BTDictionary ^>(Items[0]);
   }

   BTItem ^GetItem(String ^key, bool ignoreCase)
  {
    if (Items->Count == 0) 
      return nullptr;
    BTDictionary ^btd = GetDict();
    return btd->GetItem(key, ignoreCase);
  }

  void Write(Stream ^sw)
  {
      for each (BTItem ^i in Items)
          i->Write(sw);
  }

};

  ref class HashCacheItem
  {
  public:
    Int32 whereInFile;
    int pieceSize;
    Int32 fileSize;
    array<unsigned char>^ theHash;

    HashCacheItem(Int32 wif, int ps, Int32 fs, array<unsigned char> ^h)
    {
      whereInFile = wif;
      pieceSize = ps;
      fileSize = fs;
      theHash = h;
    }
            
  };
  typedef Generic::List<HashCacheItem ^> HashCacheItems;
  typedef Generic::Dictionary<String ^, HashCacheItems ^> HashCacheType;


public ref class BEncodeLoader
{
public:
   static BTItem ^ReadString(FileStream ^sr, Int64 length);
   static BTItem ^ReadInt(FileStream ^sr);
   static BTItem ^ReadDictionary(FileStream ^sr);
   static BTItem ^ReadList(FileStream ^sr);
   static BTItem ^ReadNext(FileStream ^sr);
   static String ^NextThing(IO::FileStream ^sr);
   static BTFile ^Load(String ^filename);
};

  enum ProcessBTActionBits { actNone = 0,
                             actRename = 1, actCopy = 2, actHashSearch = 4, actTestMode = 8, actMatchMissing = 16,
                             actSetPrios = 32, actSearchSubfolders = 64};

 public ref class BTProcessor
 {
 private:
   // Hash and file caches
   HashCacheType ^HashCache;
   int CacheChecks, CacheItems, CacheHits;
   String ^FileCacheIsFor;
   bool FileCacheWithSubFolders;
   FileList ^FileCache;
   FileLengths ^LengthCache;

   // settings for processing the torrent (and optionally resume) files
   int Action;
   String ^torrentFile;
   String ^folder;
   TreeView ^tvTree;
   //TextBox ^status;
   ListView ^Results;
   RCList ^renameList;
   ProgressBar ^pbProgress;
   String ^secondFolder; // resume.dat location, or where to copy/move to
   bool Altered;
   MissingEpisodeList ^MissingList;
   FNPRegexList ^Rexps;

   bool DoHashChecking;
   bool UsingResumeDat;

   BTFile ^resumeDat; // resume file, if we're using it


   void AddToStatus(String ^s);
   void AlterResume(int fileNum, String ^toHere);
   String ^BTProcessor::GetResumePrio(int fileNum);
   void SetResumePrio(int fileNum, unsigned char newPrio);
   BTDictionary ^GetTorrentDict();
   void FixFileguard();
   void WriteResumeDat();
   FileInfo ^FindLocalFileWithHashAt(array<unsigned char>^ findMe, Int32 whereInFile, int pieceSize, Int32 fileSize);
   void DidntFindLocalFile(int torrentFileNum);
   bool FoundLocalFile(FileInfo ^f, int torrentFileNum, String ^nameInTorrent);
   String ^MatchMissing(int torrentFileNum, String ^nameInTorrent);

   void AddResult(String ^type, String ^torrent, String ^num, String ^prio, String ^location)
   {
       if (Results == nullptr)
           return;

       int p = torrent->LastIndexOf("\\");
       if (p != -1)
           torrent = torrent->Substring(p+1);
       ListViewItem ^lvi = gcnew ListViewItem(type);
       lvi->SubItems->Add(torrent);
       lvi->SubItems->Add(num);
       lvi->SubItems->Add(prio);
       lvi->SubItems->Add(RemoveUT(location));

       Results->Items->Add(lvi);
       lvi->EnsureVisible();
       Results->Update();
   }
   void CacheThis(String ^filename, Int32 whereInFile, int piecesize, Int32 fileSize, array<unsigned char> ^hash)
   {
     CacheItems++;
     if (!HashCache->ContainsKey(filename))
       HashCache[filename] = gcnew HashCacheItems();
     HashCache[filename]->Add(gcnew HashCacheItem(whereInFile, piecesize, fileSize, hash));
   }

   array<unsigned char> ^CheckCache(String ^filename, Int32 whereInFile, int piecesize, Int32 fileSize)
   {
     CacheChecks++;
     if (HashCache->ContainsKey(filename))
       {
	 for each (HashCacheItem ^h in HashCache[filename])
		    if ((h->whereInFile == whereInFile)&&(h->pieceSize==piecesize)&&(h->fileSize == fileSize))
		      {
			CacheHits++;
			return h->theHash;
		      }
       }
     return nullptr;
   }

   void BuildFileCache(String ^folder, bool subFolders)
   {
     if ((FileCache == nullptr) || (FileCacheIsFor == nullptr) || (FileCacheIsFor != folder) || (FileCacheWithSubFolders != subFolders))
       {
         AddToStatus("Filling file cache from " + folder + (subFolders?" and subfolders":"") + "...");

	 FileCache = gcnew FileList;
	 BuildDirCache(FileCache, folder, subFolders);
	 LengthCache = gcnew FileLengths();
	 GetFileLengths(FileCache, LengthCache);
	 FileCacheIsFor = folder;
	 FileCacheWithSubFolders = subFolders;

         AddToStatus("  " + FileCache->Count + " files found.");
       }
   }

   String ^RemoveUT(String ^s)
   {
       // if it is a .!ut file, we can remove the extension
       if (s->EndsWith(".!ut"))
           return s->Remove(s->Length - 4);
       else
           return s;
   }

 public:
   BTProcessor()
   {
     HashCache = gcnew HashCacheType();
     CacheChecks = CacheItems = CacheHits = 0;
     FileCache = nullptr;
     FileCacheIsFor = nullptr;
     FileCacheWithSubFolders = false;
   }

   String ^CacheStats()
   {
     String ^r = "Hash Cache: " + CacheItems + " items for " + HashCache->Count + " files.  "+CacheHits+" hits from " + 
       CacheChecks +" lookups";
     if (CacheChecks != 0)
       r += " ("+(100*CacheHits/CacheChecks)+"%)";
     return r;
   }

   void Setup(String ^torrent, 
	      String ^folder, 
	      TreeView ^tvTree, 
	      ListView ^results,
	      RCList ^renameList,
	      ProgressBar ^pbProgress,
	      int action,
	      String ^secondFolder,
              MissingEpisodeList ^missingList,
              FNPRegexList ^rexps);

   bool Go();
 };
} // namespace
