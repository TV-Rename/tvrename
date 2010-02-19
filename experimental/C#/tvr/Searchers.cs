using System.Collections;
using System;
using System.Xml;
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

namespace TVRename
{

  public class Searchers
  {
	private StringList mNames;
    private StringList mURLs;

	public string CurrentSearch;

	public void SetToNumber(int n)
	{
		CurrentSearch = mNames[n];
	}
	public int CurrentSearchNum()
	{
		return NumForName(CurrentSearch);
	}
	public int NumForName(string srch)
	  {
		  for (int i =0;i<Count();i++)
			  if (mNames[i] == srch)
				  return i;
		  return 0;
	  }
	  public string CurrentSearchURL()
	  {
		  return mURLs[CurrentSearchNum()];
	  }

	public Searchers()
	{
	  mNames = new StringList();
      mURLs = new StringList();
	  CurrentSearch = "";

	  Add("Area07", "http://www.area07.net/browse.php?search={ShowName}+{Season}+{Episode}&cat=4");
	  Add("BitMeTV", "http://www.bitmetv.org/browse.php?search={ShowName}+{Season}+{Episode}");
	  Add("BushTorrents", "http://www.bushtorrent.com/torrents.php?search=&words={ShowName}+{Season}+{Episode}");
	  Add("BT Junkie", "http://btjunkie.org/search?q={ShowName}+{Season}+{Episode}");
	  Add("diwana.org", "http://diwana.org/browse.php?search={ShowName}+{Season}+{Episode}&cat=0");
	  Add("IP Torrents", "http://iptorrents.com/browse.php?incldead=0&search={ShowName}+{Season}+{Episode}&cat=0");
	  Add("ISO Hunt", "http://isohunt.com/torrents/?ihq={ShowName}+{Season}+{Episode}");
	  Add("Mininova", "http://www.mininova.org/search/?search={ShowName}+{Season}+{Episode}/8"); // "/8" for tv shows only
	  Add("Pirate Bay", "http://thepiratebay.org/search.php?q={ShowName}+{Season}+{Episode}");
	  Add("torrentz.com", "http://www.torrentz.com/search?q={ShowName}+{Season}+{Episode}");
	  Add("NewzLeech", "http://www.newzleech.com/usenet/?group=&minage=&age=&min=min&max=max&q={ShowName}+{Season}+{Episode}&mode=usenet&adv=");
	  Add("nzbs.org", "http://nzbs.org/index.php?action=search&q={ShowName}+{Season}+{Episode}");

	  CurrentSearch = "Mininova";
	}
	public Searchers(XmlReader reader)
	{
        mNames = new StringList();
		mURLs = new StringList();
		CurrentSearch = "";

		reader.Read();
		if (reader.Name != "TheSearchers")
			return; // bail out

		reader.Read();
		while (!reader.EOF)
		{
			if ((reader.Name == "TheSearchers") && !reader.IsStartElement())
				break; // all done

			if (reader.Name == "Current")
				CurrentSearch = reader.ReadElementContentAsString();
			else if (reader.Name == "Choice")
			{
				string url = reader.GetAttribute("URL");
				if (url == null)
					url = reader.GetAttribute("URL2");
				else
				{
					// old-style URL, replace "!" with "{ShowName}+{Season}+{Episode}"
					url = url.Replace("!","{ShowName}+{Season}+{Episode}");
				}
				Add(reader.GetAttribute("Name"), url);
				reader.ReadElementContentAsString();
			}
			else
				reader.ReadOuterXml();
		}
	}
	public void WriteXML(XmlWriter writer)
	{
		writer.WriteStartElement("TheSearchers");
		writer.WriteStartElement("Current");
		writer.WriteValue(CurrentSearch);
		writer.WriteEndElement();

		for (int i =0;i<Count();i++)
		{
			writer.WriteStartElement("Choice");
			writer.WriteStartAttribute("Name");
			writer.WriteValue(mNames[i]);
			writer.WriteEndAttribute();
			writer.WriteStartAttribute("URL2");
			writer.WriteValue(mURLs[i]);
			writer.WriteEndAttribute();
			writer.WriteEndElement();
		}
		writer.WriteEndElement(); // TheSearchers

	}
	public void Clear()
	{
		mNames.Clear();
		mURLs.Clear();
	}
	public void Add(string name, string url)
	{
	  mNames.Add(name);
	  mURLs.Add(url);
	}
	public int Count()
	{
	  return mNames.Count;
	}
	public string Name(int n)
	{
		if (n >= mNames.Count)
			n = mNames.Count - 1;
		return mNames[n];
	}
	public string URL(int n)
	{
		if (n >= mNames.Count)
			n = mNames.Count - 1;
		return mURLs[n];
	}


  }
}