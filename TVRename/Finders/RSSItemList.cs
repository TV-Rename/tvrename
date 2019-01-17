// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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

                XElement x = XElement.Load(new StringReader(response));

                if (x.Name.LocalName != "rss")
                {
                    return false;
                }

                if (!ReadChannel(x.Descendants("channel").First()))
                    return false;
            }
            catch (Exception  e)
            {
                Logger.Error(e,$"Cound not parse RSS page at:{url}");
                return false;
            }
            finally
            {
                regxps = null;
            }
            return true;
        }

        private bool ReadChannel(XElement x)
        {
            return x.Descendants("item").All(ReadItem);
        }

        private bool ReadItem(XElement itemElement)
        {
            string title = itemElement.ExtractString("title");
            string link = itemElement.ExtractString("link");
            string description = itemElement.ExtractString("description");
            string enclosureLink = itemElement.Descendants("enclosure").FirstOrDefault(enclosure => enclosure.Attribute("type")?.Value == "application/x-bittorrent")?.Attribute("url")?.Value;

            if (TVSettings.Instance.DetailedRSSJSONLogging)
            {
                Logger.Info("Processing RSS Item");
                Logger.Info(itemElement.ToString);
                Logger.Info("Extracted");
                Logger.Info($"Title:       {title}");
                Logger.Info($"Link:        {link}");
                Logger.Info($"Description: {description}");
                Logger.Info($"encLink:     {enclosureLink}");
            }

            link = (string.IsNullOrWhiteSpace(enclosureLink))?link:enclosureLink;

            if ((string.IsNullOrEmpty(title)) || (string.IsNullOrEmpty(link)))
                return false;

            string showName = "";

            FinderHelper.FindSeasEp("", title, out int season, out int episode, out int _, null, regxps);

            if (TVSettings.Instance.DetailedRSSJSONLogging)
            {
                Logger.Info($"Season:      {season}");
                Logger.Info($"Episode:     {episode}");
            }

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

            if (TVSettings.Instance.DetailedRSSJSONLogging)
            {
                Logger.Info($"Show Name:   {showName}");
                Logger.Info($"Season:      {season}");
                Logger.Info($"Episode:     {episode}");
            }

            if ((season != -1) && (episode != -1))
                Add(new RSSItem(link, title, season, episode, showName));

            return true;
        }
    }
}
