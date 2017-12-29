using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace TVRename
{
    class RssFinder:Finder
    {
        public RssFinder(TVDoc i) : base(i) { }

        public override bool Active()
        {
            return TVSettings.Instance.SearchRss;
        }

        public override FinderDisplayType DisplayType()
        {
            return FinderDisplayType.Rss;
        }

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            int c = TheActionList.Count + 2;
            int n = 1;
            prog.Invoke(100 * n / c);
            RssItemList rssList = new RssItemList();
            foreach (string s in TVSettings.Instance.RssurLs)
                rssList.DownloadRss(s, TVSettings.Instance.FnpRegexs);

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();

            foreach (ITem action1 in TheActionList)
            {
                if (ActionCancel)
                    return;

                n++;
                prog.Invoke(100 * n / c);

                if (!(action1 is ItemMissing))
                    continue;

                ItemMissing action = (ItemMissing)(action1);

                ProcessedEpisode pe = action.Episode;
                string simpleShowName = Helpers.SimplifyName(pe.Si.ShowName);
                string simpleSeriesName = Helpers.SimplifyName(pe.TheSeries.Name);

                foreach (RssItem rss in rssList)
                {
                    if ((FileHelper.SimplifyAndCheckFilename(rss.ShowName, simpleShowName, true, false) || (string.IsNullOrEmpty(rss.ShowName) && FileHelper.SimplifyAndCheckFilename(rss.Title, simpleSeriesName, true, false))) && (rss.Season == pe.SeasonNumber) && (rss.Episode == pe.EpNum))
                    {
                        newItems.Add(new ActionRss(rss, action.TheFileNoExt, pe));
                        toRemove.Add(action1);
                    }
                }
            }
            foreach (ITem i in toRemove)
                TheActionList.Remove(i);

            foreach (ITem action in newItems)
                TheActionList.Add(action);

            prog.Invoke(100);

        }
    
    }
    public class RssItem
    {
        public int Episode;
        public int Season;
        public string ShowName;
        public string Title;
        public string Url;

        public RssItem(string url, string title, int season, int episode, string showName)
        {
            Url = url;
            Season = season;
            Episode = episode;
            Title = title;
            ShowName = showName;
        }
    }

    public class RssItemList : List<RssItem>
    {
        private List<FilenameProcessorRe> _rexps; // only trustable while in DownloadRSS or its called functions

        public bool DownloadRss(string url, List<FilenameProcessorRe> rexps)
        {
            _rexps = rexps;

            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] r = wc.DownloadData(url);

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
                _rexps = null;
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

            string showName = "";

            TVDoc.FindSeasEp("", title, out int season, out int episode, null, _rexps);

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
                Add(new RssItem(link, title, season, episode, showName));

            return true;
        }
    }

}
