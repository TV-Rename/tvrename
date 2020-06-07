// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using JetBrains.Annotations;

namespace TVRename
{
    internal class JackettFinder : DownloadFinder
    {
        public JackettFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchJackett;

        [NotNull]
        protected override string CheckName() => "Asked Jackett for download links for the missing files";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            if ( settings.Unattended && TVSettings.Instance.SearchJackettManualScanOnly)
            {
                LOGGER.Info("Searching Jackett is cancelled as this is an unattended scan");
                return;
            }
            int c = ActionList.Missing.Count + 2;
            int n = 1;
            UpdateStatus(n, c, "Searching with Jackett...");

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();
            try
            {
                foreach (ItemMissing action in ActionList.Missing.ToList())
                {
                    if (settings.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    UpdateStatus(n++, c, action.Filename);

                    FindMissingEpisode(action, toRemove, newItems);
                }
            }
            catch (WebException e)
            {
                LOGGER.LogWebException($"Failed to access: {e.Response.ResponseUri} got the following message:", e);
            }
            catch (AggregateException aex) when (aex.InnerException is WebException ex)
            {
                LOGGER.LogWebException($"Failed to access: {ex.Response.ResponseUri} got the following message:", ex);
            }
            catch (HttpRequestException htec) when (htec.InnerException is WebException ex)
            {
                LOGGER.LogWebException($"Failed to access: {ex.Response.ResponseUri} got the following message:", ex);
            }

            ActionList.Replace(toRemove, newItems);
        }

        private void FindMissingEpisode([NotNull] ItemMissing action, ItemList toRemove, ItemList newItems)
        {
            string serverName = TVSettings.Instance.JackettServer;
            string serverPort = TVSettings.Instance.JackettPort;
            string allIndexer = TVSettings.Instance.JackettIndexer;
            string apikey = TVSettings.Instance.JackettAPIKey;
            string simpleShowName = Helpers.SimplifyName(action.Episode.Show.ShowName);
            string simpleSeriesName = Helpers.SimplifyName(action.Episode.TheSeries.Name);

            string url = $"http://{serverName}:{serverPort}{allIndexer}/api?t=tvsearch&q={simpleShowName}&tvdbid={action.Episode.Show.TvdbCode}&season={action.Episode.AppropriateSeasonNumber}&ep={action.Episode.AppropriateEpNum}&apikey={apikey}";

            RssItemList rssList = new RssItemList();
            rssList.DownloadRSS(url, false,"Jackett");
            ItemList newItemsForThisMissingEpisode = new ItemList();

            foreach (RSSItem rss in rssList)
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

                if (rss.Season != action.Episode.AppropriateSeasonNumber)
                {
                    continue;
                }
                if (rss.Episode != action.Episode.AppropriateEpNum)
                {
                    continue;
                }
                LOGGER.Info($"Adding {rss.URL} from RSS feed as it appears to be match for {action.Episode.Show.ShowName} S{action.Episode.AppropriateSeasonNumber}E{action.Episode.AppropriateEpNum}");
                newItemsForThisMissingEpisode.Add(new ActionTDownload(rss, action.TheFileNoExt, action.Episode, action));
                toRemove.Add(action);
            }

            foreach (ActionTDownload x in FindDuplicates(newItemsForThisMissingEpisode))
            {
                newItemsForThisMissingEpisode.Remove(x);
            }

            newItems.AddNullableRange(newItemsForThisMissingEpisode);
        }
    }
}
