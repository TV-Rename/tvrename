//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

// Things like bittorrent search engines, etc.  Manages a URL template that is fed through
// CustomName.cs to generate a final URL.

namespace TVRename
{
    public class Searchers : List<SearchEngine>
    {
        public SearchEngine CurrentSearch { get; private set; }

        public Searchers(MediaConfiguration.MediaType t)
        {
            switch (t)
            {
                case MediaConfiguration.MediaType.tv:
                    SearchEngine google2 = new()
                    {
                        Name = "Google",
                        Url = "https://www.google.com/search?q={ShowName}+S{Season:2}E{Episode}"
                    };
                    Add(google2);
                    Add(new SearchEngine { Name = "YouTube", Url = "https://www.youtube.com/results?search_query={ShowName}+{EpisodeName}" });
                    Add(new SearchEngine { Name = "Vimeo", Url = "https://vimeo.com/search?q={ShowName}+{EpisodeName}" });

                    CurrentSearch = google2;
                    break;

                case MediaConfiguration.MediaType.movie:
                    SearchEngine google = new()
                    {
                        Name = "Google",
                        Url = "https://www.google.com/search?q={ShowName}"
                    };
                    Add(google);
                    Add(new SearchEngine { Name = "YouTube", Url = "https://www.youtube.com/results?search_query={ShowName}" });
                    Add(new SearchEngine { Name = "Vimeo", Url = "https://vimeo.com/search?q={ShowName}" });

                    CurrentSearch = google;
                    break;

                case MediaConfiguration.MediaType.both:
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }

        public Searchers(XElement? settings, MediaConfiguration.MediaType t) : this(t)
        {
            if (settings is null)
            {
                return;
            }
            Clear();
            string currentSearchString = settings.ExtractString("Current");

            foreach (SearchEngine engine in settings.Descendants("Choice").Select(GenerateSearchEngine).Where(x => x.HasValue).Select(x => x.Value))
            {
                Add(engine);
                if (engine.Name == currentSearchString)
                {
                    CurrentSearch = engine;
                }
            }
        }

        private SearchEngine? GenerateSearchEngine(XElement x)
        {
            string? url = x.Attribute("URL")?.Value.Replace("!", "{ShowName}+S{Season:2}E{Episode}");
            string? url2 = x.Attribute("URL2")?.Value;
            string? name = x.Attribute("Name")?.Value;
            string? link = url ?? url2;

            if (name != null && link != null)
            {
                return new SearchEngine { Name = name, Url = link };
            }
            return null;
        }

        public void SetSearchEngine(SearchEngine s)
        {
            CurrentSearch = s;
        }

        public void WriteXml([NotNull] XmlWriter writer, string startElementName)
        {
            writer.WriteStartElement(startElementName);
            writer.WriteElement("Current", CurrentSearch.Name);

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
