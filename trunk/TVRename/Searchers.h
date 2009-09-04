#pragma once

namespace TVRename {
  using namespace System::Collections;
  using namespace System;
  using namespace System::Xml;

  public ref class Searchers
  {
    Generic::List<String ^> ^mNames;
    Generic::List<String ^> ^mURLs;

  public:
    String ^CurrentSearch;

    void SetToNumber(int n)
    {
        CurrentSearch = mNames[n];
    }
    int CurrentSearchNum()
    {
        return NumForName(CurrentSearch);
    }
    int NumForName(String ^srch)
      {
          for (int i=0;i<Count();i++)
              if (mNames[i] == srch)
                  return i;
          return 0;
      }
      String ^CurrentSearchURL()
      {
          return mURLs[CurrentSearchNum()];
      }

    Searchers()
    {
      mNames = gcnew Generic::List<String ^>;
      mURLs = gcnew Generic::List<String ^>;
      CurrentSearch = "";

      Add("Area07","http://www.area07.net/browse.php?search={ShowName}+{Season}+{Episode}&cat=4");
      Add("BitMeTV","http://www.bitmetv.org/browse.php?search={ShowName}+{Season}+{Episode}");
      Add("BushTorrents","http://www.bushtorrent.com/torrents.php?search=&words={ShowName}+{Season}+{Episode}");
      Add("BT Junkie","http://btjunkie.org/search?q={ShowName}+{Season}+{Episode}");
      Add("diwana.org","http://diwana.org/browse.php?search={ShowName}+{Season}+{Episode}&cat=0");
      Add("IP Torrents","http://iptorrents.com/browse.php?incldead=0&search={ShowName}+{Season}+{Episode}&cat=0");
      Add("ISO Hunt","http://isohunt.com/torrents/?ihq={ShowName}+{Season}+{Episode}");
      Add("Mininova","http://www.mininova.org/search/{ShowName}+{Season}+{Episode}/8"); // "/8" for tv shows only
      Add("Pirate Bay","http://thepiratebay.org/search.php?q={ShowName}+{Season}+{Episode}");
      Add("torrentz.com","http://www.torrentz.com/search?q={ShowName}+{Season}+{Episode}");
      Add("NewzLeech","http://www.newzleech.com/usenet/?group=&minage=&age=&min=min&max=max&q={ShowName}+{Season}+{Episode}&mode=usenet&adv=");
      Add("nzbs.org","http://nzbs.org/index.php?action=search&q={ShowName}+{Season}+{Episode}");

      CurrentSearch = "Mininova";
    }
    Searchers(XmlReader ^reader)
    {
        mNames = gcnew Generic::List<String ^>;
        mURLs = gcnew Generic::List<String ^>;
        CurrentSearch = "";

        reader->Read();
        if (reader->Name != "TheSearchers")
            return; // bail out

        reader->Read();
        while (!reader->EOF)
        {
            if ((reader->Name == "TheSearchers") && !reader->IsStartElement())
                break; // all done

            if (reader->Name == "Current")
                CurrentSearch = reader->ReadElementContentAsString();
            else if (reader->Name == "Choice")
            {
                String ^url = reader->GetAttribute("URL");
                if (url == nullptr)
                    url = reader->GetAttribute("URL2");
                else
                {
                    // old-style URL, replace "!" with "{ShowName}+{Season}+{Episode}"
                    url = url->Replace("!","{ShowName}+{Season}+{Episode}");
                }
                Add(reader->GetAttribute("Name"), url);
                reader->ReadElementContentAsString();
            }
            else
                reader->ReadOuterXml();
        }
    }
    void WriteXML(XmlWriter ^writer)
    {
        writer->WriteStartElement("TheSearchers");
        writer->WriteStartElement("Current");
        writer->WriteValue(CurrentSearch);
        writer->WriteEndElement();

        for (int i=0;i<Count();i++)
        {
            writer->WriteStartElement("Choice");
            writer->WriteStartAttribute("Name");
            writer->WriteValue(mNames[i]);
            writer->WriteEndAttribute();
            writer->WriteStartAttribute("URL2");
            writer->WriteValue(mURLs[i]);
            writer->WriteEndAttribute();
            writer->WriteEndElement();
        }
        writer->WriteEndElement(); // TheSearchers

    }
    void Clear()
    {
        mNames->Clear();
        mURLs->Clear();
    }
    void Add(String ^name, String ^url)
    {
      mNames->Add(name);
      mURLs->Add(url);
    }
    int Count()
    {
      return mNames->Count;
    }
	String ^Name(int n)
	{
		if (n >= mNames->Count)
			n = mNames->Count - 1;
		return mNames[n];
	}
	String ^URL(int n)
	{
		if (n >= mNames->Count)
			n = mNames->Count - 1;
		return mURLs[n];
	}


  };
}
