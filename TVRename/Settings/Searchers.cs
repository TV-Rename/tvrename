// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

// Things like bittorrent search engines, etc.  Manages a URL template that is fed through
// CustomName.cs to generate a final URL.

namespace TVRename
{
    public class Searchers
    {
        private class Choice
        {
            public string Name;
            public string Url2;
        }

        private string currentSearch;
        private readonly List<Choice> choices = new List<Choice>();

        public Searchers()
        {
            currentSearch = "";

            Add("Google", "https://www.google.com/search?q={ShowName}+S{Season:2}E{Episode}");
            Add("Pirate Bay", "https://thepiratebay.org/search/{ShowName} S{Season:2}E{Episode}");
            Add("binsearch", "https://www.binsearch.info/?q={ShowName}+S{Season:2}E{Episode}");

            currentSearch = "Google";
        }
        
        public Searchers(XElement settings)
        {
            choices = new List<Choice>();
            currentSearch = settings.ExtractString("Current");

            foreach (XElement x in settings.Descendants("Choice"))
            {
                string url = x.Attribute("URL")?.Value;
                if (url == null)
                    url = x.Attribute("URL2")?.Value;
                else
                {
                    // old-style URL, replace "!" with "{ShowName}+{Season}+{Episode}"
                    url = url.Replace("!", "{ShowName}+S{Season:2}E{Episode}");
                }
                Add(x.Attribute("Name")?.Value,url);
            }
        }

        public void SetToNumber(int n)
        {
            currentSearch = choices[n].Name;
        }

        public int CurrentSearchNum() => NumForName(currentSearch);

        private int NumForName(string srch)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                if (choices[i].Name == srch)
                    return i;
            }
            return 0;
        }

        public string CurrentSearchUrl()
        {
            return choices.Count == 0 ? "" : choices[CurrentSearchNum()].Url2;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("TheSearchers");
            XmlHelper.WriteElementToXml(writer,"Current",currentSearch);

            for (int i = 0; i < Count(); i++)
            {
                writer.WriteStartElement("Choice");
                XmlHelper.WriteAttributeToXml(writer,"Name",choices[i].Name);
                XmlHelper.WriteAttributeToXml(writer,"URL2",choices[i].Url2);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // TheSearchers
        }
        public void Clear() => choices.Clear();

        public void Add(string name, string url)
        {
            choices.Add(new Choice { Name = name, Url2 = url });
        }

        public int Count() => choices.Count;

        public string Name(int n)
        {
            if (n >= choices.Count)
                n = choices.Count - 1;
            else if (n < 0)
                n = 0;
            return choices[n].Name;
        }

        public string Url(int n)
        {
            if (n >= choices.Count)
                n = choices.Count - 1;
            else if (n < 0)
                n = 0;
            return choices[n].Url2;
        }
    }
}
