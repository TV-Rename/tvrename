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
        public class Choice
        {
            public string Name;
            public string URL2;
        }

        public string CurrentSearch;
        private List<Choice> Choices = new List<Choice>();

        public Searchers()
        {
            this.CurrentSearch = "";

            this.Add("Google", "https://www.google.com/search?q={ShowName}+S{Season:2}E{Episode}");
            this.Add("Pirate Bay", "https://thepiratebay.org/search/{ShowName} S{Season:2}E{Episode}");
            this.Add("binsearch", "https://www.binsearch.info/?q={ShowName}+S{Season:2}E{Episode}");

            this.CurrentSearch = "Google";
        }
        
        public Searchers(XmlReader reader)
        {
            this.Choices = new List<Choice>();
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
            this.CurrentSearch = this.Choices[n].Name;
        }

        public int CurrentSearchNum()
        {
            return this.NumForName(this.CurrentSearch);
        }

        public int NumForName(string srch)
        {
            for (int i = 0; i < this.Choices.Count; i++)
            {
                if (this.Choices[i].Name == srch)
                    return i;
            }
            return 0;
        }

        public string CurrentSearchURL()
        {
            if (this.Choices.Count == 0)
                return "";
            return this.Choices[this.CurrentSearchNum()].URL2;
        }
        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("TheSearchers");
            XMLHelper.WriteElementToXML(writer,"Current",this.CurrentSearch);

            for (int i = 0; i < this.Count(); i++)
            {
                writer.WriteStartElement("Choice");
                XMLHelper.WriteAttributeToXML(writer,"Name",this.Choices[i].Name);
                XMLHelper.WriteAttributeToXML(writer,"URL2",this.Choices[i].URL2);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // TheSearchers
        }
        public void Clear()
        {
            this.Choices.Clear();
        }

        public void Add(string name, string url)
        {

            this.Choices.Add(new Choice { Name = name, URL2 = url });
        }

        public int Count()
        {
            return this.Choices.Count;
        }

        public string Name(int n)
        {
            if (n >= this.Choices.Count)
                n = this.Choices.Count - 1;
            else if (n < 0)
                n = 0;
            return this.Choices[n].Name;
        }

        public string URL(int n)
        {
            if (n >= this.Choices.Count)
                n = this.Choices.Count - 1;
            else if (n < 0)
                n = 0;
            return this.Choices[n].URL2;
        }
    }
}
