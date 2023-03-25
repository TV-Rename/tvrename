//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;

namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class JSONWebpageFinder : DownloadFinder
{
    public JSONWebpageFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
    }

    public override bool Active() => TVSettings.Instance.SearchJSON;

    protected override string CheckName() => "Check JSON links for the missing files";

    protected override void DoCheck(SetProgressDelegate progress)
    {
        if (TVSettings.Instance.SearchJSONManualScanOnly && Settings.Unattended)
        {
            LOGGER.Info("Searching JSON Wepages is cancelled as this is an unattended scan");
            return;
        }
        int c = ActionList.Missing.Count + 1;
        int n = 0;
        UpdateStatus(n, c, "Searching on JSON Page...");

        ItemList newItems = new();
        ItemList toRemove = new();
        UrlCache cache = new();
        try
        {
            foreach (ShowItemMissing action in ActionList.MissingEpisodes)
            {
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(n++, c, action.Filename);

                FindMissingEpisode(action, toRemove, newItems, cache);
            }
        }
        catch (WebException e)
        {
            LOGGER.LogWebException(
                $"Failed to access: {TVSettings.Instance.SearchJSONURL} got the following message:", e);
        }
        catch (AggregateException aex) when (aex.InnerException is WebException ex)
        {
            LOGGER.LogWebException(
                $"Failed to access: {TVSettings.Instance.SearchJSONURL} got the following message:", ex);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException hex)
        {
            LOGGER.LogHttpRequestException($"Could not download RSS page at: {TVSettings.Instance.SearchJSONURL} got the following message: ", hex);
        }
        catch (HttpRequestException htec) when (htec.InnerException is WebException ex)
        {
            LOGGER.LogWebException(
                $"Failed to access: {TVSettings.Instance.SearchJSONURL} got the following message:", ex);
        }
        catch (HttpRequestException htec)
        {
            LOGGER.Warn($"Failed to access: {TVSettings.Instance.SearchJSONURL} got the following message: {htec.Message}");
        }
        catch (JsonReaderException ex)
        {
            LOGGER.Warn(ex, $"Failed to Parse {TVSettings.Instance.SearchJSONURL} into JSON - Please check that the URL is valid JSON format.");
        }
        ActionList.Replace(toRemove, newItems);
    }

    private static void FindMissingEpisode(ShowItemMissing action, ItemList toRemove, ItemList newItems, UrlCache cache)
    {
        ProcessedEpisode pe = action.MissingEpisode;

        string imdbId = pe.TheCachedSeries.GetImdbNumber();
        if (string.IsNullOrWhiteSpace(imdbId))
        {
            return;
        }

        string simpleShowName = pe.Show.ShowName.CompareName();
        string simpleSeriesName = pe.TheCachedSeries.Name.CompareName();
        ItemList newItemsForThisMissingEpisode = new();

        string response = cache.GetUrl($"{TVSettings.Instance.SearchJSONURL}{imdbId}", TVSettings.Instance.SearchJSONUseCloudflare);

        if (string.IsNullOrWhiteSpace(response))
        {
            LOGGER.Warn(
                $"Got no response from {TVSettings.Instance.SearchJSONURL}{imdbId} for {action.MissingEpisode.TheCachedSeries.Name}");

            return;
        }

        JObject jsonResponse = JObject.Parse(response);
        if (jsonResponse.ContainsKey(TVSettings.Instance.SearchJSONRootNode))
        {
            JToken? x = jsonResponse[TVSettings.Instance.SearchJSONRootNode];
            if (x is null)
            {
                LOGGER.Warn($"Could not find {TVSettings.Instance.SearchJSONRootNode} in JSON Repsonse {jsonResponse}");
                return;
            }
            foreach (JToken item in x)
            {
                if (item is not JObject episodeResponse)
                {
                    continue;
                }

                if (episodeResponse.ContainsKey(TVSettings.Instance.SearchJSONFilenameToken) &&
                    episodeResponse.ContainsKey(TVSettings.Instance.SearchJSONURLToken))
                {
                    string? itemName = (string?)item[TVSettings.Instance.SearchJSONFilenameToken];
                    string? itemUrl = (string?)item[TVSettings.Instance.SearchJSONURLToken];
                    int? seeders = (int?)item[TVSettings.Instance.SearchJSONSeedersToken];
                    long itemSizeBytes = CalculateItemSizeBytes(item);

                    if (TVSettings.Instance.DetailedRSSJSONLogging)
                    {
                        LOGGER.Info("Processing JSON Item");
                        LOGGER.Info(episodeResponse.ToString);
                        LOGGER.Info("Extracted");
                        LOGGER.Info($"Name:        {itemName}");
                        LOGGER.Info($"URL:         {itemUrl}");
                        LOGGER.Info($"Size:        {itemSizeBytes}");
                        LOGGER.Info($"Seeds:       {seeders}");
                    }

                    if (itemName is null || itemUrl is null)
                    {
                        continue;
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

                    ItemDownloading becomes = new(new FutureTorrentEntry(itemUrl, action.TheFileNoExt), action.MissingEpisode, action.TheFileNoExt, DownloadingFinder.DownloadApp.qBitTorrent, action);

                    newItemsForThisMissingEpisode.Add(new ActionTDownload(itemName, itemSizeBytes, seeders, itemUrl,
                        action.TheFileNoExt, pe, action, $"JSON WebPage: {TVSettings.Instance.SearchJSONURL}{imdbId}", becomes));
                }
                else
                {
                    LOGGER.Info(
                        $"{TVSettings.Instance.SearchJSONFilenameToken} or {TVSettings.Instance.SearchJSONURLToken} not found in {TVSettings.Instance.SearchJSONURL}{imdbId} for {action.MissingEpisode.TheCachedSeries.Name}");
                }
            }
        }
        else
        {
            LOGGER.Info(
                $"{TVSettings.Instance.SearchJSONRootNode} not found in {TVSettings.Instance.SearchJSONURL}{imdbId} for {action.MissingEpisode.TheCachedSeries.Name}");
        }

        Replace(action, toRemove, newItems, newItemsForThisMissingEpisode);
    }

    private static long CalculateItemSizeBytes(JToken item)
    {
        try
        {
            return (long?)item[TVSettings.Instance.SearchJSONFileSizeToken] ?? -1;
        }
        catch
        {
            //-1 as size is not available (empty string or other)
            return -1;
        }
    }
}
