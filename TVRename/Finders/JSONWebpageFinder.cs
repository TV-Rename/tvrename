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
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class JSONFinder: DownloadFinder
    {
        public JSONFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchJSON;
        [NotNull]
        protected override string Checkname() => "Check JSON links for the missing files";

        public override FinderDisplayType DisplayType() => FinderDisplayType.search;

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            if (TVSettings.Instance.SearchJSONManualScanOnly && settings.Unattended)
            {
                LOGGER.Info("Searching JSON Wepages is cancelled as this is an unattended scan");
                return;
            }
            int c = ActionList.MissingItems().Count() + 1;
            int n = 0;
            UpdateStatus(n,c, "Searching on JSON Page...");

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();
            UrlCache cache = new UrlCache();
            try
            {
                foreach (ItemMissing action in ActionList.MissingItems().ToList())
                {
                    if (settings.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    UpdateStatus(n++, c, action.Filename);

                    FindMissingEpisode(action, toRemove, newItems, cache);
                }
            }
            catch (WebException e)
            {
                LOGGER.LogWebException($"Failed to access: {TVSettings.Instance.SearchJSONURL} got the following message:", e);
            }
            catch (AggregateException aex) when (aex.InnerException is WebException ex)
            {
                LOGGER.LogWebException($"Failed to access: {TVSettings.Instance.SearchJSONURL} got the following message:", ex);
            }
            catch (JsonReaderException ex)
            {
                LOGGER.Warn(ex, $"Failed to Parse {TVSettings.Instance.SearchJSONURL} into JSON - Please check that the URL is valid JSON format.");
            }
            ActionList.Replace(toRemove,newItems);
        }

        private static void FindMissingEpisode([NotNull] ItemMissing action, ItemList toRemove, ItemList newItems, UrlCache cache)
        {
            ProcessedEpisode pe = action.Episode;

            string imdbId = pe.TheSeries.GetImdbNumber();
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return;
            }

            string simpleShowName = Helpers.SimplifyName(pe.Show.ShowName);
            string simpleSeriesName = Helpers.SimplifyName(pe.TheSeries.Name);
            ItemList newItemsForThisMissingEpisode = new ItemList();

            string response = cache.GetUrl($"{TVSettings.Instance.SearchJSONURL}{imdbId}",TVSettings.Instance.SearchJSONUseCloudflare);

            if (string.IsNullOrWhiteSpace(response))
            {
                LOGGER.Warn(
                    $"Got no response from {TVSettings.Instance.SearchJSONURL}{imdbId} for {action.Episode.TheSeries.Name}");

                return;
            }

            JObject jsonResponse = JObject.Parse(response);
            if (jsonResponse.ContainsKey(TVSettings.Instance.SearchJSONRootNode))
            {
                foreach (JToken item in jsonResponse[TVSettings.Instance.SearchJSONRootNode])
                {
                    if (item is null || !(item is JObject episodeResponse))
                    {
                        continue;
                    }

                    if (episodeResponse.ContainsKey(TVSettings.Instance.SearchJSONFilenameToken) &&
                        episodeResponse.ContainsKey(TVSettings.Instance.SearchJSONURLToken))
                    {
                        string itemName = (string) item[TVSettings.Instance.SearchJSONFilenameToken];
                        string itemUrl = (string) item[TVSettings.Instance.SearchJSONURLToken];
                        long itemSizeBytes = CalculateItemSizeBytes(item);

                        if (TVSettings.Instance.DetailedRSSJSONLogging)
                        {
                            LOGGER.Info("Processing JSON Item");
                            LOGGER.Info(episodeResponse.ToString);
                            LOGGER.Info("Extracted");
                            LOGGER.Info($"Name:        {itemName}");
                            LOGGER.Info($"URL:         {itemUrl}");
                            LOGGER.Info($"Size:        {itemSizeBytes}");
                        }

                        if (!FileHelper.SimplifyAndCheckFilename(itemName, simpleShowName, true, false) &&
                            !FileHelper.SimplifyAndCheckFilename(itemName, simpleSeriesName, true, false))
                        {
                            continue;
                        }

                        if (!FinderHelper.FindSeasEp(itemName, out int seas, out int ep, out int _, pe.Show))
                        {
                            continue;
                        }

                        if (TVSettings.Instance.DetailedRSSJSONLogging)
                        {
                            LOGGER.Info($"Season:      {seas}");
                            LOGGER.Info($"Episode:     {ep}");
                        }

                        if (seas != pe.AppropriateSeasonNumber)
                        {
                            continue;
                        }

                        if (ep != pe.AppropriateEpNum)
                        {
                            continue;
                        }

                        LOGGER.Info(
                            $"Adding {itemUrl} from JSON page as it appears to be match for {pe.Show.ShowName} S{pe.AppropriateSeasonNumber}E{pe.AppropriateEpNum}");

                        newItemsForThisMissingEpisode.Add(new ActionTDownload(itemName, itemSizeBytes, itemUrl,
                            action.TheFileNoExt, pe, action));

                        toRemove.Add(action);
                    }
                    else
                    {
                        LOGGER.Info(
                            $"{TVSettings.Instance.SearchJSONFilenameToken} or {TVSettings.Instance.SearchJSONURLToken} not found in {TVSettings.Instance.SearchJSONURL}{imdbId} for {action.Episode.TheSeries.Name}");
                    }
                }
            }
            else
            {
                LOGGER.Info(
                    $"{TVSettings.Instance.SearchJSONRootNode} not found in {TVSettings.Instance.SearchJSONURL}{imdbId} for {action.Episode.TheSeries.Name}");
            }

            RemoveDuplicates(newItemsForThisMissingEpisode);

            newItems.AddNullableRange(newItemsForThisMissingEpisode);
        }

        private static void RemoveDuplicates([NotNull] ItemList newItemsForThisMissingEpisode)
        {
            foreach (ActionTDownload x in FindDuplicates(newItemsForThisMissingEpisode))
            {
                newItemsForThisMissingEpisode.Remove(x);
            }
        }

        private static long CalculateItemSizeBytes(JToken item)
        {
            try
            {
                return (long) item[TVSettings.Instance.SearchJSONFileSizeToken];
            }
            catch
            {
                //-1 as size is not available (empty string or other)
                return -1;
            }
        }
    }
}
