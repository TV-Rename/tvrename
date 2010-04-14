// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Xml;

// Things like bittorrent search engines, etc.  Manages a URL template that is fed through
// CustomName.cs to generate a final URL.

namespace TVRename
{
    public class Searchers
    {
        public string CurrentSearch;
        private StringList mNames;
        private StringList mURLs;

        public Searchers()
        {
            this.mNames = new StringList();
            this.mURLs = new StringList();
            this.CurrentSearch = "";

            this.Add("Area07", "http://www.area07.net/browse.php?search={ShowName}+{Season}+{Episode}&cat=4");
            this.Add("BitMeTV", "http://www.bitmetv.org/browse.php?search={ShowName}+{Season}+{Episode}");
            this.Add("BushTorrents", "http://www.bushtorrent.com/torrents.php?search=&words={ShowName}+{Season}+{Episode}");
            this.Add("BT Junkie", "http://btjunkie.org/search?q={ShowName}+{Season}+{Episode}");
            this.Add("diwana.org", "http://diwana.org/browse.php?search={ShowName}+{Season}+{Episode}&cat=0");
            this.Add("IP Torrents", "http://iptorrents.com/browse.php?incldead=0&search={ShowName}+{Season}+{Episode}&cat=0");
            this.Add("ISO Hunt", "http://isohunt.com/torrents/?ihq={ShowName}+{Season}+{Episode}");
            this.Add("Mininova", "http://www.mininova.org/search/?search={ShowName}+{Season}+{Episode}/8"); // "/8" for tv shows only
            this.Add("Pirate Bay", "http://thepiratebay.org/search.php?q={ShowName}+{Season}+{Episode}");
            this.Add("torrentz.com", "http://www.torrentz.com/search?q={ShowName}+{Season}+{Episode}");
            this.Add("NewzLeech", "http://www.newzleech.com/usenet/?group=&minage=&age=&min=min&max=max&q={ShowName}+{Season}+{Episode}&mode=usenet&adv=");
            this.Add("nzbs.org", "http://nzbs.org/index.php?action=search&q={ShowName}+{Season}+{Episode}");
            this.Add("binsearch", "http://binsearch.net/?q={ShowName}+s{Season:2}e{Episode2}&max=25&adv_age=365&server=");

            this.CurrentSearch = "Mininova";
        }

        public Searchers(XmlReader reader)
        {
            this.mNames = new StringList();
            this.mURLs = new StringList();
            this.CurrentSearch = "";

            reader.Read();
            if (reader.Name != "TheSearchers")
                return; // bail out

            reader.Read();
            while (!reader.EOF)
            {
                if ((reader.Name == "TheSearchers") && !reader.IsStartElement())
                    break; // all done

                if (reader.Name == "Current")
                    this.CurrentSearch = reader.ReadElementContentAsString();
                else if (reader.Name == "Choice")
                {
                    string url = reader.GetAttribute("URL");
                    if (url == null)
                        url = reader.GetAttribute("URL2");
                    else
                    {
                        // old-style URL, replace "!" with "{ShowName}+{Season}+{Episode}"
                        url = url.Replace("!", "{ShowName}+{Season}+{Episode}");
                    }
                    this.Add(reader.GetAttribute("Name"), url);
                    reader.ReadElementContentAsString();
                }
                else
                    reader.ReadOuterXml();
            }
        }

        public void SetToNumber(int n)
        {
            this.CurrentSearch = this.mNames[n];
        }

        public int CurrentSearchNum()
        {
            return this.NumForName(this.CurrentSearch);
        }

        public int NumForName(string srch)
        {
            for (int i = 0; i < this.Count(); i++)
            {
                if (this.mNames[i] == srch)
                    return i;
            }
            return 0;
        }

        public string CurrentSearchURL()
        {
            return this.mURLs[this.CurrentSearchNum()];
        }

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("TheSearchers");
            writer.WriteStartElement("Current");
            writer.WriteValue(this.CurrentSearch);
            writer.WriteEndElement();

            for (int i = 0; i < this.Count(); i++)
            {
                writer.WriteStartElement("Choice");
                writer.WriteStartAttribute("Name");
                writer.WriteValue(this.mNames[i]);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("URL2");
                writer.WriteValue(this.mURLs[i]);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // TheSearchers
        }

        public void Clear()
        {
            this.mNames.Clear();
            this.mURLs.Clear();
        }

        public void Add(string name, string url)
        {
            this.mNames.Add(name);
            this.mURLs.Add(url);
        }

        public int Count()
        {
            return this.mNames.Count;
        }

        public string Name(int n)
        {
            if (n >= this.mNames.Count)
                n = this.mNames.Count - 1;
            return this.mNames[n];
        }

        public string URL(int n)
        {
            if (n >= this.mNames.Count)
                n = this.mNames.Count - 1;
            return this.mURLs[n];
        }
    }
}