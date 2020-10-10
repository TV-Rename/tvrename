// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
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

        protected override string CheckName() => "Asked Jackett for download links for the missing files";

        protected override void DoCheck(SetProgressDelegate prog, TVDoc.ScanSettings settings)
        {
            if ( settings.Unattended && TVSettings.Instance.SearchJackettManualScanOnly)
            {
                LOGGER.Info("Searching Jackett is cancelled as this is an unattended scan");
                return;
            }

            if (settings.Type ==TVSettings.ScanType.Full && TVSettings.Instance.StopJackettSearchOnFullScan)
            {
                LOGGER.Info("Searching Jackett is cancelled as this is a full scan");
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

                    if (action is ShowItemMissing)
                    {
                        FindMissingEpisode((ShowItemMissing)action, toRemove, newItems);
                    }
                    else
                    {
                        FindMissingEpisode((MovieItemMissing)action, toRemove, newItems);
                    }
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

        private static void FindMissingEpisode([NotNull] ShowItemMissing action, ItemList toRemove, ItemList newItems)
        {
            ProcessedEpisode processedEpisode = action.MissingEpisode;
            string url = TVSettings.Instance.UseJackettTextSearch ? TextJackettUrl(processedEpisode) : NormalJackettUrl( processedEpisode);

            RssItemList rssList = new RssItemList();
            rssList.DownloadRSS(url, false,"Jackett");
            ItemList newItemsForThisMissingEpisode = new ItemList();

            foreach (RSSItem rss in rssList.Where(rss => RssMatch(rss, processedEpisode)))
            {
                if (TVSettings.Instance.DetailedRSSJSONLogging)
                {
                    LOGGER.Info(
                        $"Adding {rss.URL} from RSS feed as it appears to be match for {processedEpisode.Show.ShowName} S{processedEpisode.AppropriateSeasonNumber}E{processedEpisode.AppropriateEpNum}");
                }
                newItemsForThisMissingEpisode.Add(new ActionTDownload(rss, action.TheFileNoExt, processedEpisode, action));
                toRemove.Add(action);
            }

            foreach (ActionTDownload x in FindDuplicates(newItemsForThisMissingEpisode))
            {
                newItemsForThisMissingEpisode.Remove(x);
            }

            newItems.AddNullableRange(newItemsForThisMissingEpisode);
        }

        private static void FindMissingEpisode([NotNull] MovieItemMissing action, ItemList toRemove, ItemList newItems)
        {
            string url = TVSettings.Instance.UseJackettTextSearch ? TextJackettUrl(action.MovieConfig) : NormalJackettUrl(action.MovieConfig);

            RssItemList rssList = new RssItemList();
            rssList.DownloadRSS(url, false, "Jackett");
            ItemList newItemsForThisMissingEpisode = new ItemList();

            foreach (RSSItem rss in rssList.Where(rss => RssMatch(rss, action.MovieConfig)))
            {
                if (TVSettings.Instance.DetailedRSSJSONLogging)
                {
                    LOGGER.Info(
                        $"Adding {rss.URL} from RSS feed as it appears to be match for {action.MovieConfig.ShowName}");
                }
                newItemsForThisMissingEpisode.Add(new ActionTDownload(rss, action.TheFileNoExt, null, action));
                toRemove.Add(action);
            }

            foreach (ActionTDownload x in FindDuplicates(newItemsForThisMissingEpisode))
            {
                newItemsForThisMissingEpisode.Remove(x);
            }

            newItems.AddNullableRange(newItemsForThisMissingEpisode);
        }

        private static string TextJackettUrl(MovieConfiguration actionMovieConfig)
        {
            string serverName = TVSettings.Instance.JackettServer;
            string serverPort = TVSettings.Instance.JackettPort;
            string allIndexer = TVSettings.Instance.JackettIndexer;
            string apikey = TVSettings.Instance.JackettAPIKey;
            const string FORMAT = "{ShowName}";
            string text = CustomMovieName.NameFor(actionMovieConfig,FORMAT);
            return
                $"http://{serverName}:{serverPort}{allIndexer}/api?t=movie&q={text}&apikey={apikey}";
        }

        private static string NormalJackettUrl(MovieConfiguration actionMovieConfig)
        {
            string serverName = TVSettings.Instance.JackettServer;
            string serverPort = TVSettings.Instance.JackettPort;
            string allIndexer = TVSettings.Instance.JackettIndexer;
            string apikey = TVSettings.Instance.JackettAPIKey;
            return
                $"http://{serverName}:{serverPort}{allIndexer}/api?t=movie&apikey={apikey}&tmdbid={actionMovieConfig.TmdbCode}";
        }

        private static string NormalJackettUrl(ProcessedEpisode processedEpisode)
        {
            string serverName = TVSettings.Instance.JackettServer;
            string serverPort = TVSettings.Instance.JackettPort;
            string allIndexer = TVSettings.Instance.JackettIndexer;
            string apikey = TVSettings.Instance.JackettAPIKey;
            string simpleShowName = Helpers.SimplifyName(processedEpisode.Show.ShowName);

            return
                $"http://{serverName}:{serverPort}{allIndexer}/api?t=tvsearch&q={simpleShowName}&tvdbid={processedEpisode.Show.TvdbCode}&season={processedEpisode.AppropriateSeasonNumber}&ep={processedEpisode.AppropriateEpNum}&apikey={apikey}";
        }

        private static string TextJackettUrl(ProcessedEpisode episode)
        {
            string serverName = TVSettings.Instance.JackettServer;
            string serverPort = TVSettings.Instance.JackettPort;
            string allIndexer = TVSettings.Instance.JackettIndexer;
            string apikey = TVSettings.Instance.JackettAPIKey;
            const string FORMAT = "{ShowName} S{Season:2}E{Episode}[-E{Episode2}]";
            string text = CustomEpisodeName.NameForNoExt(episode, FORMAT, false);
            return
                $"http://{serverName}:{serverPort}{allIndexer}/api?t=tvsearch&q={text}&apikey={apikey}";
        }

        public static void SearchForEpisode(ProcessedEpisode episode)
        {
            string serverName = TVSettings.Instance.JackettServer;
            string serverPort = TVSettings.Instance.JackettPort;
            const string FORMAT = "{ShowName} S{Season:2}E{Episode}[-E{Episode2}]";

            string url = $"http://{serverName}:{serverPort}/UI/Dashboard#search={CustomEpisodeName.NameForNoExt(episode,FORMAT,false)}";

            Helpers.OpenUrl(url);
        }

        public static void SearchForMovie(MovieConfiguration mov)
        {
            string serverName = TVSettings.Instance.JackettServer;
            string serverPort = TVSettings.Instance.JackettPort;
            const string FORMAT = "{ShowName} ({Year})";

            string url = $"http://{serverName}:{serverPort}/UI/Dashboard#search={Uri.EscapeDataString(CustomMovieName.NameFor(mov, FORMAT))}";

            Helpers.OpenUrl(url);
        }
    }
}
