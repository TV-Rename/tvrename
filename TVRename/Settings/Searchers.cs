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
    public struct SearchEngine
    {
        public string Name;
        public string Url;
    }

    public class Searchers:List<SearchEngine>
    {
        public SearchEngine CurrentSearch { get; private set; }

        public Searchers()
        {
            SearchEngine google = new SearchEngine
            {
                Name = "Google", Url = "https://www.google.com/search?q={ShowName}+S{Season:2}E{Episode}"
            };
            Add(google);
            Add(new SearchEngine {Name="Pirate Bay" ,Url= "https://thepiratebay.org/search/{ShowName} S{Season:2}E{Episode}"});
            Add(new SearchEngine { Name = "binsearch", Url = "https://www.binsearch.info/?q={ShowName}+S{Season:2}E{Episode}"});

            CurrentSearch = google;
        }
        
        public Searchers([CanBeNull] XElement settings)
        {
            Clear();
            if (settings is null)
            {
                return;
            }

            string currentSearchString = settings.ExtractString("Current");

            foreach (XElement x in settings.Descendants("Choice"))
            {
                string url = x.Attribute("URL")?.Value;
                url = url is null
                    ? x.Attribute("URL2")?.Value
                    : url.Replace("!", "{ShowName}+S{Season:2}E{Episode}");

                SearchEngine engine = new SearchEngine {Name= x.Attribute("Name")?.Value,Url=url };

                Add(engine);
                if (engine.Name == currentSearchString)
                {
                    CurrentSearch = engine;
                }
            }
        }

        public void SetSearchEngine(SearchEngine s)
        {
            CurrentSearch = s;
        }

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("TheSearchers");
            writer.WriteElement("Current",CurrentSearch.Name);

            foreach (SearchEngine e in this)
            {
                writer.WriteStartElement("Choice");
                writer.WriteAttributeToXml("Name", e.Name);
                writer.WriteAttributeToXml("URL2", e.Url);
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // TheSearchers
        }
    }
}
