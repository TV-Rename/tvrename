// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

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
        
        public Searchers([CanBeNull] XElement settings)
        {
            choices = new List<Choice>();
            if (settings is null)
            {
                return;
            }

            currentSearch = settings.ExtractString("Current");

            foreach (XElement x in settings.Descendants("Choice"))
            {
                string url = x.Attribute("URL")?.Value;
                url = url is null
                    ? x.Attribute("URL2")?.Value
                    : url.Replace("!", "{ShowName}+S{Season:2}E{Episode}");

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
                {
                    return i;
                }
            }
            return 0;
        }

        public string CurrentSearchUrl()
        {
            return choices.Count == 0 ? "" : choices[CurrentSearchNum()].Url2;
        }

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("TheSearchers");
            writer.WriteElement("Current",currentSearch);

            for (int i = 0; i < Count(); i++)
            {
                writer.WriteStartElement("Choice");
                writer.WriteAttributeToXml("Name",choices[i].Name);
                writer.WriteAttributeToXml("URL2",choices[i].Url2);
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

        public string Name(int n) => choices.Count == 0 ? string.Empty : choices[Between(n, 0, choices.Count - 1)].Name;

        public string Url(int n) => choices.Count == 0 ? string.Empty : choices[Between(n,0,choices.Count - 1)].Url2;

        private static int Between(int n, int min, int max)
        {
            return n < min ? min
                : n > max ? max
                : n;
        }
    }
}
