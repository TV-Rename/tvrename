//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class RSSFinder : DownloadFinder
    {
        public RSSFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
        {
        }

        public override bool Active() => TVSettings.Instance.SearchRSS;

        [NotNull]
        protected override string CheckName() => "Looked in the listed RSS URLs for download links for the missing files";

        protected override void DoCheck(SetProgressDelegate prog)
        {
            if (TVSettings.Instance.SearchRSSManualScanOnly && Settings.Unattended)
            {
                LOGGER.Info("Searching RSS Feeds is cancelled as this is an unattended scan");
                return;
            }
            int c = ActionList.Missing.Count + 2;
            int n = 1;
            UpdateStatus(n, c, "Searching on RSS Feed...");

            // ReSharper disable once InconsistentNaming
            RssItemList RSSList = new();
            foreach (string s in TVSettings.Instance.RSSURLs)
            {
                RSSList.DownloadRSS(s, TVSettings.Instance.RSSUseCloudflare, "RSS");
            }

            ItemList newItems = new();
            ItemList toRemove = new();

            foreach (ShowItemMissing action in ActionList.MissingEpisodes)
            {
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(n++, c, action.Filename);

                ProcessedEpisode pe = action.MissingEpisode;
                ItemList newItemsForThisMissingEpisode = new();

                foreach (RSSItem rss in RSSList.Where(rss => RssMatch(rss, pe)))
                {
                    LOGGER.Info(
                        $"Adding {rss.URL} from RSS feed as it appears to be match for {pe.Show.ShowName} {pe}");

                    ItemDownloading eventualItem = new(new FutureTorrentEntry(rss.URL, action.TheFileNoExt), action.MissingEpisode, action.TheFileNoExt, DownloadingFinder.DownloadApp.qBitTorrent, action);
                    newItemsForThisMissingEpisode.Add(new ActionTDownload(rss, action, eventualItem));
                    toRemove.Add(action);
                }

                foreach (ActionTDownload x in FindDuplicates(newItemsForThisMissingEpisode))
                {
                    newItemsForThisMissingEpisode.Remove(x);
                }

                newItems.AddNullableRange(newItemsForThisMissingEpisode);
            }
            ActionList.Replace(toRemove, newItems);
        }
    }
}
