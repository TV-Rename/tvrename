using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace TVRename
{
    class RSSFinder:Finder
    {
        public RSSFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchRSS;

        public override Finder.FinderDisplayType DisplayType() => FinderDisplayType.RSS;

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            int c = this.ActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct);
            RSSItemList RSSList = new RSSItemList();
            foreach (string s in TVSettings.Instance.RSSURLs)
                RSSList.DownloadRSS(s, TVSettings.Instance.FNPRegexs);

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();

            foreach (Item Action1 in this.ActionList)
            {
                if (this.ActionCancel)
                    return;

                prog.Invoke(startpct + (totPct - startpct) * (++n) / (c));

                if (!(Action1 is ItemMissing))
                    continue;

                ItemMissing Action = (ItemMissing)(Action1);

                ProcessedEpisode pe = Action.Episode;
                string simpleShowName = Helpers.SimplifyName(pe.SI.ShowName);
                string simpleSeriesName = Helpers.SimplifyName(pe.TheSeries.Name);

                foreach (RSSItem rss in RSSList)
                {
                    if ((FileHelper.SimplifyAndCheckFilename(rss.ShowName, simpleShowName, true, false) || (string.IsNullOrEmpty(rss.ShowName) && FileHelper.SimplifyAndCheckFilename(rss.Title, simpleSeriesName, true, false))) && (rss.Season == pe.AppropriateSeasonNumber) && (rss.Episode == pe.AppropriateEpNum ))
                    {
                        newItems.Add(new ActionRSS(rss, Action.TheFileNoExt, pe));
                        toRemove.Add(Action1);
                    }
                }
            }
            foreach (Item i in toRemove)
                this.ActionList.Remove(i);

            foreach (Item Action in newItems)
                this.ActionList.Add(Action);

            prog.Invoke(totPct);

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
            this.URL = url;
            this.Season = season;
            this.Episode = episode;
            this.Title = title;
            this.ShowName = showName;
        }
    }

    public class RSSItemList : System.Collections.Generic.List<RSSItem>
    {
        private List<FilenameProcessorRE> Rexps; // only trustable while in DownloadRSS or its called functions

        public bool DownloadRSS(string URL, List<FilenameProcessorRE> rexps)
        {
            this.Rexps = rexps;

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
                using (XmlReader reader = XmlReader.Create(ms, settings))
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
                            if (!this.ReadChannel(reader.ReadSubtree()))
                                return false;
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                this.Rexps = null;
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
                    if (!this.ReadItem(r.ReadSubtree()))
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

            TVDoc.FindSeasEp("", title, out int season, out int episode, out int maxEp, null, this.Rexps);

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
                this.Add(new RSSItem(link, title, season, episode, showName));

            return true;
        }
    }

}
