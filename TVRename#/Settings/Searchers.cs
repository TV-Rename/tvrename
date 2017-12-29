// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System.Collections.Generic;
using System.Xml;

// Things like bittorrent search engines, etc.  Manages a URL template that is fed through
// CustomName.cs to generate a final URL.

namespace TVRename
{
    public class Searchers
    {
        public class Choice
        {
            public string Name;
            public string URL2;
        }

        public string CurrentSearch;
        private List<Choice> Choices = new List<Choice>();

        public Searchers()
        {
            CurrentSearch = "";

            // MS Edit 1/4/13 - Removed URLs that no longer work
            //this.Add("Area07", "http://www.area07.net/browse.php?search={ShowName}+{Season}+{Episode}&cat=4");
            //this.Add("BushTorrents", "http://www.bushtorrent.com/torrents.php?search=&words={ShowName}+{Season}+{Episode}");
            //this.Add("BT Junkie", "http://btjunkie.org/search?q={ShowName}+{Season}+{Episode}");
            //this.Add("diwana.org", "http://diwana.org/browse.php?search={ShowName}+{Season}+{Episode}&cat=0");
            //this.Add("NewzLeech", "http://www.newzleech.com/usenet/?group=&minage=&age=&min=min&max=max&q={ShowName}+{Season}+{Episode}&mode=usenet&adv=");
            //this.Add("nzbs.org", "http://nzbs.org/index.php?action=search&q={ShowName}+{Season}+{Episode}");
            
            Add("BitMeTV", "http://www.bitmetv.org/browse.php?search={ShowName}+{Season}+{Episode}");
            Add("IP Torrents", "http://iptorrents.com/browse.php?incldead=0&search={ShowName}+{Season}+{Episode}&cat=0");
            Add("ISO Hunt", "http://isohunt.com/torrents/?ihq={ShowName}+{Season}+{Episode}");
            Add("Mininova", "http://www.mininova.org/search/?search={ShowName}+{Season}+{Episode}/8"); // "/8" for tv shows only
            Add("Pirate Bay", "http://thepiratebay.org/search.php?q={ShowName}+{Season}+{Episode}");
            Add("torrentz.com", "http://www.torrentz.com/search?q={ShowName}+s{Season:2}e{Episode2}");
            Add("binsearch", "http://binsearch.net/?q={ShowName}+s{Season:2}e{Episode2}&max=25&adv_age=365&server=");

            CurrentSearch = "Mininova";
        }
        
        public Searchers(XmlReader reader)
        {
            Choices = new List<Choice>();
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
                        url = url.Replace("!", "{ShowName}+{Season}+{Episode}");
                    }
                    Add(reader.GetAttribute("Name"), url);
                    reader.ReadElementContentAsString();
                }
                else
                    reader.ReadOuterXml();
            }
        }

        public void SetToNumber(int n)
        {
            CurrentSearch = Choices[n].Name;
        }

        public int CurrentSearchNum()
        {
            return NumForName(CurrentSearch);
        }

        public int NumForName(string srch)
        {
            for (int i = 0; i < Choices.Count; i++)
            {
                if (Choices[i].Name == srch)
                    return i;
            }
            return 0;
        }

        public string CurrentSearchURL()
        {
            if (Choices.Count == 0)
                return "";
            return Choices[CurrentSearchNum()].URL2;
        }
        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("TheSearchers");
            XMLHelper.WriteElementToXML(writer,"Current",CurrentSearch);

            for (int i = 0; i < Count(); i++)
            {
                writer.WriteStartElement("Choice");
                XMLHelper.WriteAttributeToXML(writer,"Name",Choices[i].Name);
                XMLHelper.WriteAttributeToXML(writer,"URL2",Choices[i].URL2);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // TheSearchers
        }
        public void Clear()
        {
            Choices.Clear();
        }

        public void Add(string name, string url)
        {

            Choices.Add(new Choice { Name = name, URL2 = url });
        }

        public int Count()
        {
            return Choices.Count;
        }

        public string Name(int n)
        {
            if (n >= Choices.Count)
                n = Choices.Count - 1;
            else if (n < 0)
                n = 0;
            return Choices[n].Name;
        }

        public string URL(int n)
        {
            if (n >= Choices.Count)
                n = Choices.Count - 1;
            else if (n < 0)
                n = 0;
            return Choices[n].URL2;
        }
    }
}
