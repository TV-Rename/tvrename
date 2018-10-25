// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Xml;

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

        public string CurrentSearch;
        private readonly List<Choice> choices = new List<Choice>();

        public Searchers()
        {
            CurrentSearch = "";

            Add("Google", "https://www.google.com/search?q={ShowName}+S{Season:2}E{Episode}");
            Add("Pirate Bay", "https://thepiratebay.org/search/{ShowName} S{Season:2}E{Episode}");
            Add("binsearch", "https://www.binsearch.info/?q={ShowName}+S{Season:2}E{Episode}");

            CurrentSearch = "Google";
        }
        
        public Searchers(XmlReader reader)
        {
            choices = new List<Choice>();
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
                        url = url.Replace("!", "{ShowName}+S{Season:2}E{Episode}");
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
            CurrentSearch = choices[n].Name;
        }

        public int CurrentSearchNum()
        {
            return NumForName(CurrentSearch);
        }

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
            if (choices.Count == 0)
                return "";
            return choices[CurrentSearchNum()].Url2;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("TheSearchers");
            XmlHelper.WriteElementToXml(writer,"Current",CurrentSearch);

            for (int i = 0; i < Count(); i++)
            {
                writer.WriteStartElement("Choice");
                XmlHelper.WriteAttributeToXml(writer,"Name",choices[i].Name);
                XmlHelper.WriteAttributeToXml(writer,"URL2",choices[i].Url2);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // TheSearchers
        }
        public void Clear()
        {
            choices.Clear();
        }

        public void Add(string name, string url)
        {

            choices.Add(new Choice { Name = name, Url2 = url });
        }

        public int Count()
        {
            return choices.Count;
        }

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
