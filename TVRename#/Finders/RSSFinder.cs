using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace TVRename
{
    class RSSFinder:Finder
    {
        public RSSFinder(TVDoc i) : base(i) { }

        public override bool Active()
        {
            return TVSettings.Instance.SearchRSS;
        }

        public override FinderDisplayType DisplayType()
        {
            return FinderDisplayType.RSS;
        }

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            int c = TheActionList.Count + 2;
            int n = 1;
            prog.Invoke(100 * n / c);
            RSSItemList RSSList = new RSSItemList();
            foreach (string s in TVSettings.Instance.RSSURLs)
                RSSList.DownloadRSS(s, TVSettings.Instance.FNPRegexs);

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();

            foreach (Item Action1 in TheActionList)
            {
                if (ActionCancel)
                    return;

                n++;
                prog.Invoke(100 * n / c);

                if (!(Action1 is ItemMissing))
                    continue;

                ItemMissing Action = (ItemMissing)(Action1);

                ProcessedEpisode pe = Action.Episode;
                string simpleShowName = Helpers.SimplifyName(pe.SI.ShowName);
                string simpleSeriesName = Helpers.SimplifyName(pe.TheSeries.Name);

                foreach (RSSItem rss in RSSList)
                {
                    if ((FileHelper.SimplifyAndCheckFilename(rss.ShowName, simpleShowName, true, false) || (string.IsNullOrEmpty(rss.ShowName) && FileHelper.SimplifyAndCheckFilename(rss.Title, simpleSeriesName, true, false))) && (rss.Season == pe.SeasonNumber) && (rss.Episode == pe.EpNum))
                    {
                        newItems.Add(new ActionRSS(rss, Action.TheFileNoExt, pe));
                        toRemove.Add(Action1);
                    }
                }
            }
            foreach (Item i in toRemove)
                TheActionList.Remove(i);

            foreach (Item Action in newItems)
                TheActionList.Add(Action);

            prog.Invoke(100);

        }
    
    }
    public class RSSItem
    {
        public int Episode;
        public int Season;
        public string ShowName;
        public string Title;
        public string URL;

        public RSSItem(string url, string title, int season, int episode, string showName)
        {
            URL = url;
            Season = season;
            Episode = episode;
            Title = title;
            ShowName = showName;
        }
    }

    public class RSSItemList : List<RSSItem>
    {
        private List<FilenameProcessorRE> Rexps; // only trustable while in DownloadRSS or its called functions

        public bool DownloadRSS(string URL, List<FilenameProcessorRE> rexps)
        {
            Rexps = rexps;

            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] r = wc.DownloadData(URL);

                MemoryStream ms = new MemoryStream(r);

                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                };
                XmlReader reader = XmlReader.Create(ms, settings);

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

                ms.Close();
            }
            catch
            {
                return false;
            }
            finally
            {
                Rexps = null;
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

        public bool ReadItem(XmlReader r)
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

            int season = -1;
            int episode = -1;
            string showName = "";

            TVDoc.FindSeasEp("", title, out season, out episode, null, Rexps);

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
            }

            if ((season != -1) && (episode != -1))
                Add(new RSSItem(link, title, season, episode, showName));

            return true;
        }
    }

}
