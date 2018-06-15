using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class RSSFinder:Finder
    {
        public RSSFinder(TVDoc i) : base(i) { }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override bool Active() => TVSettings.Instance.SearchRSS;

        public override FinderDisplayType DisplayType() => FinderDisplayType.rss;

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            int c = ActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct);
            // ReSharper disable once InconsistentNaming
            RSSItemList RSSList = new RSSItemList();
            foreach (string s in TVSettings.Instance.RSSURLs)
                RSSList.DownloadRSS(s, TVSettings.Instance.FNPRegexs);

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();

            foreach (Item testItem in ActionList)
            {
                if (ActionCancel)
                    return;

                prog.Invoke(startpct + ((totPct - startpct) * (++n) / (c)));

                if (!(testItem is ItemMissing action))
                    continue;

                ProcessedEpisode pe = action.Episode;
                string simpleShowName = Helpers.SimplifyName(pe.SI.ShowName);
                string simpleSeriesName = Helpers.SimplifyName(pe.TheSeries.Name);

                foreach (RSSItem rss in RSSList)
                {
                    if (
                        !FileHelper.SimplifyAndCheckFilename(rss.ShowName, simpleShowName, true, false) &&
                        !(
                            string.IsNullOrEmpty(rss.ShowName) &&
                            FileHelper.SimplifyAndCheckFilename(rss.Title, simpleSeriesName, true, false)
                         )
                       ) continue;

                    if (rss.Season != pe.AppropriateSeasonNumber) continue;
                    if (rss.Episode != pe.AppropriateEpNum) continue;

                    Logger.Info($"Adding {rss.URL} as it appears to be match for {testItem.Episode.SI.ShowName} S{testItem.Episode.AppropriateSeasonNumber}E{testItem.Episode.AppropriateEpNum}");
                    newItems.Add(new ActionRSS(rss, action.TheFileNoExt, pe));
                    toRemove.Add(testItem);
                }
            }

            //We now want to rationlise the newItems - just in case we've added duplicates
            List<ActionRSS>  duplicateActionRSS = new List<ActionRSS>();

            foreach (Item x in newItems)
            {
                if (!(x is ActionRSS testActionRSSOne))
                    continue;
                foreach (Item y in newItems)
                {
                    if (!(y is ActionRSS testActionRSSTwo))
                        continue;
                    if (x.Equals(y)) continue;

                    string[] preferredTerms = TVSettings.Instance.PreferredRSSSearchTerms();

                    if (testActionRSSOne.RSS.ShowName.ContainsOneOf(preferredTerms) &&
                        !testActionRSSTwo.RSS.ShowName.ContainsOneOf(preferredTerms))
                    {
                        duplicateActionRSS.Add(testActionRSSTwo);
                        Logger.Info($"Removing {testActionRSSTwo.RSS.URL} as it is not as good a match as {testActionRSSOne.RSS.URL }");
                    }

                    if (testActionRSSOne.RSS.Title.ContainsOneOf(preferredTerms) &&
                        !testActionRSSTwo.RSS.Title.ContainsOneOf(preferredTerms))
                    {

                        duplicateActionRSS.Add(testActionRSSTwo);
                        Logger.Info(
                            $"Removing {testActionRSSTwo.RSS.URL} as it is not as good a match as {testActionRSSOne.RSS.URL}");
                    }
                }
            }

            foreach (ActionRSS x in duplicateActionRSS)
                newItems.Remove(x);

            foreach (Item i in toRemove)
                ActionList.Remove(i);

            foreach (Item action in newItems)
                ActionList.Add(action);

            prog.Invoke(totPct);
        }
    }
    // ReSharper disable once InconsistentNaming
    public class RSSItem
    {
        public readonly int Episode;
        public readonly int Season;
        public readonly string ShowName;
        public readonly string Title;
        // ReSharper disable once InconsistentNaming
        public readonly string URL;

        public RSSItem(string url, string title, int season, int episode, string showName)
        {
            URL = url;
            Season = season;
            Episode = episode;
            Title = title;
            ShowName = showName;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class RSSItemList : List<RSSItem>
    {
        private List<FilenameProcessorRE> regxps; // only trustable while in DownloadRSS or its called functions

        // ReSharper disable once InconsistentNaming
        public bool DownloadRSS(string URL, List<FilenameProcessorRE> rexps)
        {
            regxps = rexps;

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
                            if (!ReadChannel(reader.ReadSubtree()))
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

            TVDoc.FindSeasEp("", title, out int season, out int episode, out int maxEp, null, regxps);

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
