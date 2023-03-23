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
using TMDbLib.Objects.TvShows;

namespace TVRename;

internal class JackettFinder : DownloadFinder
{
    public JackettFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
    }

    public override bool Active() => TVSettings.Instance.SearchJackett;

    protected override string CheckName() => "Asked Jackett for download links for the missing files";

    protected override void DoCheck(SetProgressDelegate progress)
    {
        if (Settings.Unattended && TVSettings.Instance.SearchJackettManualScanOnly)
        {
            LOGGER.Info("Searching Jackett is cancelled as this is an unattended scan");
            return;
        }

        if (Settings.Type == TVSettings.ScanType.Full && TVSettings.Instance.StopJackettSearchOnFullScan && Settings.AnyMediaToUpdate)
        {
            LOGGER.Info("Searching Jackett is cancelled as this is a full scan");
            return;
        }

        int c = ActionList.Missing.Count + 2;
        int n = 1;
        UpdateStatus(n, c, "Searching with Jackett...");

        ItemList newItems = new();
        ItemList toRemove = new();
        try
        {
            foreach (ItemMissing action in ActionList.Missing.ToList())
            {
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(n++, c, action.Filename);

                switch (action)
                {
                    case ShowItemMissing showItemMissing:
                        FindMissingEpisode(showItemMissing, toRemove, newItems);
                        break;
                    case MovieItemMissing movieItemMissing:
                        FindMissingEpisode(movieItemMissing, toRemove, newItems);
                        break;
                    case ShowSeasonMissing seasonMissing:
                        FindMissingSeason(seasonMissing, toRemove, newItems);
                        break;
                }
            }
        }
        catch (WebException e)
        {
            LOGGER.LogWebException($"Failed to access: {e.Response?.ResponseUri} got the following message:", e);
        }
        catch (AggregateException aex) when (aex.InnerException is WebException ex)
        {
            LOGGER.LogWebException($"Failed to access: {ex.Response?.ResponseUri} got the following message:", ex);
        }
        catch (HttpRequestException hre) when (hre.InnerException is WebException ex)
        {
            LOGGER.LogWebException($"Failed to access: {ex.Response?.ResponseUri} got the following message:", ex);
        }
        catch (AggregateException aex) when (aex.InnerException is HttpRequestException ex)
        {
            LOGGER.LogHttpRequestException("Failed to access: Jackett got the following message:", ex);
        }
        ActionList.Replace(toRemove, newItems);
    }

    private static void FindMissingEpisode(ShowItemMissing action, ItemList toRemove, ItemList newItems)
    {
        ProcessedEpisode processedEpisode = action.MissingEpisode;
        string url = TVSettings.Instance.UseJackettTextSearch ? TextJackettUrl(processedEpisode) : NormalJackettUrl(processedEpisode);

        RssItemList rssList = new();
        rssList.DownloadRSS(url, false, "Jackett");
        ItemList newItemsForThisMissingEpisode = new();

        foreach (RSSItem rss in rssList.Where(rss => RssMatch(rss, processedEpisode)))
        {
            if (TVSettings.Instance.DetailedRSSJSONLogging)
            {
                LOGGER.Info(
                    $"Adding {rss.URL} from RSS feed as it appears to be match for {processedEpisode.Show.ShowName} S{processedEpisode.AppropriateSeasonNumber}E{processedEpisode.AppropriateEpNum}");
            }

            ItemDownloading becomes = new(new FutureTorrentEntry(rss.URL, action.TheFileNoExt), action.MissingEpisode, action.TheFileNoExt, DownloadingFinder.DownloadApp.qBitTorrent, action);
            newItemsForThisMissingEpisode.Add(new ActionTDownload(rss, action, becomes));
        }

        System.Collections.Generic.IEnumerable<ActionTDownload> bestDownloads = Rationalize(newItemsForThisMissingEpisode);

        if (bestDownloads.HasAny())
        {
            newItems.AddNullableRange(bestDownloads);
            toRemove.Add(action);
        }
    }

    private static void FindMissingEpisode(MovieItemMissing action, ItemList toRemove, ItemList newItems)
    {
        string url = TVSettings.Instance.UseJackettTextSearch ? TextJackettUrl(action.MovieConfig) : NormalJackettUrl(action.MovieConfig);

        RssItemList rssList = new();
        rssList.DownloadRSS(url, false, "Jackett");
        ItemList newItemsForThisMissingEpisode = new();

        foreach (RSSItem rss in rssList.Where(rss => RssMatch(rss, action.MovieConfig)))
        {
            if (TVSettings.Instance.DetailedRSSJSONLogging)
            {
                LOGGER.Info(
                    $"Adding {rss.URL} from RSS feed as it appears to be match for {action.MovieConfig.ShowName}");
            }
            ItemDownloading becomes = new(new FutureTorrentEntry(rss.URL, action.TheFileNoExt), action.MovieConfig, action.TheFileNoExt, DownloadingFinder.DownloadApp.qBitTorrent, action);
            newItemsForThisMissingEpisode.Add(new ActionTDownload(rss, action, becomes));
        }

        System.Collections.Generic.IEnumerable<ActionTDownload> bestDownloads = Rationalize(newItemsForThisMissingEpisode);

        if (bestDownloads.HasAny())
        {
            newItems.AddNullableRange(bestDownloads);
            toRemove.Add(action);
        }
    }

    private static void FindMissingSeason(ShowSeasonMissing action, ItemList toRemove, ItemList newItems)
    {
        string url = TVSettings.Instance.UseJackettTextSearch ? TextJackettUrl(action.Series , action.SeasonNumberAsInt) : NormalJackettUrl(action.Series, action.SeasonNumberAsInt);

        RssItemList rssList = new();
        rssList.DownloadRSS(url, false, "Jackett");
        ItemList newItemsForThisMissingEpisode = new();

        foreach (RSSItem rss in rssList.Where(rss => RssMatch(rss, action.Series, action.SeasonNumberAsInt??0 )))
        {
            if (TVSettings.Instance.DetailedRSSJSONLogging)
            {
                LOGGER.Info(
                    $"Adding {rss.URL} from RSS feed as it appears to be match for {action.Show.ShowName} S{action.SeasonNumber}");
            }
            ItemDownloading becomes = new(new FutureTorrentEntry(rss.URL, action.TheFileNoExt), action.Series, action.SeasonNumberAsInt, action.TheFileNoExt, DownloadingFinder.DownloadApp.qBitTorrent, action);
            newItemsForThisMissingEpisode.Add(new ActionTDownload(rss, action, becomes));
        }

        System.Collections.Generic.IEnumerable<ActionTDownload> bestDownloads = Rationalize(newItemsForThisMissingEpisode);

        if (bestDownloads.HasAny())
        {
            newItems.AddNullableRange(bestDownloads);
            toRemove.Add(action);
        }
    }
    private static string NormalJackettUrl(ShowConfiguration series, int? seasonNumberAsInt)
    {
        string apikey = TVSettings.Instance.JackettAPIKey;
        string simpleShowName = WebUtility.UrlEncode(series.ShowName.CompareName());

        return
            $"{IndexerUrl()}api?t=tvsearch&q={simpleShowName}&tvdbid={series.TvdbCode}&season={seasonNumberAsInt}&apikey={apikey}";
    }

    private static string TextJackettUrl(ShowConfiguration series, int? seasonNumberAsInt)
    {
        string apikey = TVSettings.Instance.JackettAPIKey;
        const string FORMAT = "{ShowName} S{Season:2}E{Episode}[-E{Episode2}]";
        string text = WebUtility.UrlEncode(CustomSeasonName.NameFor(series,seasonNumberAsInt??0, FORMAT));
        return $"{IndexerUrl()}api?t=tvsearch&q={text}&apikey={apikey}";
    }

    private static string IndexerUrl()
    {
        string serverName = TVSettings.Instance.JackettServer;
        string serverPort = TVSettings.Instance.JackettPort;
        string allIndexer = TVSettings.Instance.JackettIndexer;
        return $"http://{serverName}:{serverPort}{allIndexer}/";
    }

    private static string TextJackettUrl(MovieConfiguration actionMovieConfig)
    {
        string apikey = TVSettings.Instance.JackettAPIKey;
        const string FORMAT = "{ShowName}";
        string text = WebUtility.UrlEncode(CustomMovieName.NameFor(actionMovieConfig, FORMAT));
        return $"{IndexerUrl()}api?t=movie&q={text}&apikey={apikey}";
    }

    private static string NormalJackettUrl(MovieConfiguration actionMovieConfig)
    {
        string apikey = TVSettings.Instance.JackettAPIKey;
        return $"{IndexerUrl()}api?t=movie&apikey={apikey}&tmdbid={actionMovieConfig.TmdbCode}";
    }

    private static string NormalJackettUrl(ProcessedEpisode processedEpisode)
    {
        string apikey = TVSettings.Instance.JackettAPIKey;
        string simpleShowName = WebUtility.UrlEncode(processedEpisode.Show.ShowName.CompareName());

        return
            $"{IndexerUrl()}api?t=tvsearch&q={simpleShowName}&tvdbid={processedEpisode.Show.TvdbCode}&season={processedEpisode.AppropriateSeasonNumber}&ep={processedEpisode.AppropriateEpNum}&apikey={apikey}";
    }

    private static string TextJackettUrl(ProcessedEpisode episode)
    {
        string apikey = TVSettings.Instance.JackettAPIKey;
        const string FORMAT = "{ShowName} S{Season:2}E{Episode}[-E{Episode2}]";
        string text = WebUtility.UrlEncode(CustomEpisodeName.NameForNoExt(episode, FORMAT, false));
        return $"{IndexerUrl()}api?t=tvsearch&q={text}&apikey={apikey}";
    }

    public static void SearchForEpisode(ProcessedEpisode episode)
    {
        const string FORMAT = "{ShowName} S{Season:2}E{Episode}[-E{Episode2}]";
        string searchTerm = CustomEpisodeName.NameForNoExt(episode, FORMAT, false);
        SearchFor(searchTerm);
    }

    public static void SearchForMovie(MovieConfiguration mov)
    {
        const string FORMAT = "{ShowName} ({Year})";
        string searchTerm = CustomMovieName.NameFor(mov, FORMAT);
        SearchFor(searchTerm);
    }

    public static void SearchForSeason(ShowConfiguration series, int snum)
    {
        const string FORMAT = "{ShowName} S{Season:2}";
        string searchTerm = CustomSeasonName.NameFor(series, snum, FORMAT);
        SearchFor(searchTerm);
    }

    private static void SearchFor(string searchTerm)
    {
        string serverName = TVSettings.Instance.JackettServer;
        string serverPort = TVSettings.Instance.JackettPort;
        string searchServer = $"http://{serverName}:{serverPort}/";

        string url = $"{searchServer}UI/Dashboard#search={WebUtility.UrlEncode(searchTerm)}&tracker=&category=";

        url.OpenUrlInBrowser();
    }
}
