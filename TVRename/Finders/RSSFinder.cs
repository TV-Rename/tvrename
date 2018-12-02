// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class RSSFinder: DownloadFinder
    {
        public RSSFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchRSS;

        public override FinderDisplayType DisplayType() => FinderDisplayType.search;

        public override void Check(SetProgressDelegate prog, int startpct, int totPct, ICollection<ShowItem> showList,
            TVDoc.ScanSettings settings)
        {
            if (TVSettings.Instance.SearchRSSManualScanOnly && settings.Unattended)
            {
                LOGGER.Info("Searching RSS Feeds is cancelled as this is an unattended scan");
                prog.Invoke(totPct, string.Empty);
                return;
            }
            int c = ActionList.Count + 2;
            int n = 1;
            prog.Invoke(startpct,"Searching on RSS Feed...");

            // ReSharper disable once InconsistentNaming
            RssItemList RSSList = new RssItemList();
            foreach (string s in TVSettings.Instance.RSSURLs)
            {
                RSSList.DownloadRSS(s, TVSettings.Instance.FNPRegexs);
            }

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();

            foreach (ItemMissing action in ActionList.MissingItems())
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                prog.Invoke(startpct + ((totPct - startpct) * (++n) / (c)),action.Filename);

                ProcessedEpisode pe = action.Episode;
                string simpleShowName = Helpers.SimplifyName(pe.Show.ShowName);
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

                    LOGGER.Info($"Adding {rss.URL} as it appears to be match for {action.Episode.Show.ShowName} S{action.Episode.AppropriateSeasonNumber}E{action.Episode.AppropriateEpNum}");
                    newItems.Add(new ActionTDownload(rss, action.TheFileNoExt, pe,action));
                    toRemove.Add(action);
                }
            }

            foreach (ActionTDownload x in FindDuplicates(newItems))
                newItems.Remove(x);

            foreach (Item i in toRemove)
                ActionList.Remove(i);

            foreach (Item action in newItems)
                ActionList.Add(action);

            prog.Invoke(totPct,string.Empty);
        }
    }
}
