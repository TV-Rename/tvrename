using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using NLog;

namespace TVRename
{
    internal class RssItemList : List<RSSItem>
    {
        private List<TVSettings.FilenameProcessorRE> regxps; // only trustable while in DownloadRSS or its called functions
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // ReSharper disable once InconsistentNaming
        public bool DownloadRSS(string url, List<TVSettings.FilenameProcessorRE> rexps)
        {
            regxps = rexps;

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string response = client.DownloadString(url);

                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                };
                using (XmlReader reader = XmlReader.Create(new StringReader(response), settings))
                {
                    reader.Read();
                    if (reader.Name != "xml")
                        return false;

                    reader.Read();

                    if (reader.Name != "rss")
                        return false;

                    reader.Read();

                    while (!reader.EOF)
                    {
                        if ((reader.Name == "rss") && (!reader.IsStartElement()))
                            break;

                        if (reader.Name == "channel")
                        {
                            if (!ReadChannel(reader.ReadSubtree()))
                                return false;
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                }
            }
            catch (Exception  e)
            {
                Logger.Error($"Cound not access {url}",e);
                return false;
            }
            finally
            {
                regxps = null;
            }
            return true;
        }

        private bool ReadChannel(XmlReader r)
        {
            r.Read();
            r.Read();
            while (!r.EOF)
            {
                if ((r.Name == "channel") && (!r.IsStartElement()))
                    break;
                if (r.Name == "item")
                {
                    if (!ReadItem(r.ReadSubtree()))
                        return false;
                    r.Read();
                }
                else
                    r.ReadOuterXml();
            }
            return true;
        }

        private bool ReadItem(XmlReader r)
        {
            string title = "";
            string link = "";
            string description = "";

            r.Read();
            r.Read();
            while (!r.EOF)
            {
                if ((r.Name == "item") && (!r.IsStartElement()))
                    break;
                if (r.Name == "title")
                    title = r.ReadElementContentAsString();
                else if (r.Name == "description")
                    description = r.ReadElementContentAsString();
                else if ((r.Name == "link") && (string.IsNullOrEmpty(link)))
                    link = r.ReadElementContentAsString();
                else if ((r.Name == "enclosure") && (r.GetAttribute("type") == "application/x-bittorrent"))
                {
                    link = r.GetAttribute("url");
                    r.ReadOuterXml();
                }
                else
                    r.ReadOuterXml();
            }
            if ((string.IsNullOrEmpty(title)) || (string.IsNullOrEmpty(link)))
                return false;

            string showName = "";

            TVDoc.FindSeasEp("", title, out int season, out int episode, out int _, null, regxps);

            try
            {
                Match m = Regex.Match(description, "Show Name: (.*?)[;|$]", RegexOptions.IgnoreCase);
                if (m.Success)
                    showName = m.Groups[1].ToString();
                m = Regex.Match(description, "Season: ([0-9]+)", RegexOptions.IgnoreCase);
                if (m.Success)
                    season = int.Parse(m.Groups[1].ToString());
                m = Regex.Match(description, "Episode: ([0-9]+)", RegexOptions.IgnoreCase);
                if (m.Success)
                    episode = int.Parse(m.Groups[1].ToString());
            }
            catch
            {
                // ignored
            }

            if ((season != -1) && (episode != -1))
                Add(new RSSItem(link, title, season, episode, showName));

            return true;
        }
    }
}
