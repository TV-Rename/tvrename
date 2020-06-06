// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class RSSFinder: DownloadFinder
    {
        public RSSFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchRSS;

        [NotNull]
        protected override string CheckName() => "Looked in the listed RSS URLs for download links for the missing files";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            if (TVSettings.Instance.SearchRSSManualScanOnly && settings.Unattended)
            {
                LOGGER.Info("Searching RSS Feeds is cancelled as this is an unattended scan");
                return;
            }
            int c = ActionList.Missing.Count + 2;
            int n = 1;
            UpdateStatus(n, c, "Searching on RSS Feed...");

            // ReSharper disable once InconsistentNaming
            RssItemList RSSList = new RssItemList();
            foreach (string s in TVSettings.Instance.RSSURLs)
            {
                RSSList.DownloadRSS(s, TVSettings.Instance.RSSUseCloudflare);
            }

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();

            foreach (ItemMissing action in ActionList.Missing.ToList())
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(n++, c, action.Filename);

                ProcessedEpisode pe = action.Episode;
                string simpleShowName = Helpers.SimplifyName(pe.Show.ShowName);
                string simpleSeriesName = Helpers.SimplifyName(pe.TheSeries.Name);
                ItemList newItemsForThisMissingEpisode = new ItemList();

                foreach (RSSItem rss in RSSList)
                {
                    if (
                        !FileHelper.SimplifyAndCheckFilename(rss.ShowName, simpleShowName, true, false) &&
                        !(
                            string.IsNullOrEmpty(rss.ShowName) &&
                            FileHelper.SimplifyAndCheckFilename(rss.Title, simpleSeriesName, true, false)
                         )
                       )
                    {
                        continue;
                    }

                    if (rss.Season != pe.AppropriateSeasonNumber)
                    {
                        continue;
                    }
                    if (rss.Episode != pe.AppropriateEpNum)
                    {
                        continue;
                    }

                    LOGGER.Info($"Adding {rss.URL} from RSS feed as it appears to be match for {action.Episode.Show.ShowName} S{action.Episode.AppropriateSeasonNumber}E{action.Episode.AppropriateEpNum}");
                    newItemsForThisMissingEpisode.Add(new ActionTDownload(rss, action.TheFileNoExt, pe,action));
                    toRemove.Add(action);
                }

                foreach (ActionTDownload x in FindDuplicates(newItemsForThisMissingEpisode))
                {
                    newItemsForThisMissingEpisode.Remove(x);
                }

                newItems.AddNullableRange(newItemsForThisMissingEpisode);
            }
            ActionList.Replace(toRemove,newItems);
        }
    }
}
