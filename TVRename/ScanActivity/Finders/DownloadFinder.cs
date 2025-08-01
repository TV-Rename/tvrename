//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Generic;
using System.Linq;

namespace TVRename;

public abstract class DownloadFinder : Finder
{
    protected DownloadFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
    }

    public override FinderDisplayType DisplayType() => FinderDisplayType.search;

    protected static bool RssMatch(RSSItem rss, ProcessedEpisode pe)
    {
        return rss.Season == pe.AppropriateSeasonNumber && rss.Episode == pe.AppropriateEpNum && RssNameMatch(rss,pe);
    }

    private static bool RssNameMatch(RSSItem rss, ProcessedEpisode pe)
    {
        string simpleShowName = pe.Show.ShowName.CompareName();
        string simpleSeriesName = pe.TheCachedSeries.Name.CompareName();

        string nameFromRss = string.IsNullOrEmpty(rss.ShowName) ? rss.Title : rss.ShowName;

        if (FileHelper.SimplifyAndCheckFilename(nameFromRss, simpleShowName, true, false))
        {
            return true;
        }
        if (FileHelper.SimplifyAndCheckFilename(nameFromRss, simpleSeriesName, true, false))
        {
            return true;
        }

        return false;
    }

    protected static bool RssMatch(RSSItem rss, MovieConfiguration pe)
    {
        string simpleShowName = pe.ShowName.CompareName();
        return FileHelper.SimplifyAndCheckFilename(rss.ShowName.HasValue() ? rss.ShowName : rss.Title, simpleShowName, true, false);
    }
    protected static bool RssMatch(RSSItem rss, ShowConfiguration series, int seasonNumberAsInt)
    {
        string simpleShowName = $"{series.ShowName.CompareName()} S{seasonNumberAsInt}";
        return FileHelper.SimplifyAndCheckFilename(rss.ShowName.HasValue() ? rss.ShowName : rss.Title, simpleShowName, true, false);
    }

    protected static IEnumerable<ActionTDownload> Rationalize(ItemList newItems)
        => newItems.DownloadTorrents.Where(HasEnoughSeeders).Where(NotContainsUnwantedTerms).WithMax(NumberOfGoodTerms);

    private static bool HasEnoughSeeders(ActionTDownload arg)
        => arg.Seeders is null || TVSettings.Instance.MinRSSSeeders is null || arg.Seeders.Value >= TVSettings.Instance.MinRSSSeeders;

    private static int NumberOfGoodTerms(ActionTDownload actionTDownload)
        => actionTDownload.SourceName.NumberContains(TVSettings.Instance.PreferredRSSSearchTerms());

    private static bool NotContainsUnwantedTerms(ActionTDownload actionTDownload)
        => !actionTDownload.SourceName.ContainsOneOf(TVSettings.Instance.UnwantedRSSSearchTerms());

    protected static void Replace(ItemMissing action, ItemList toRemove, ItemList newItems, ItemList newItemsForThisMissingEpisode)
    {
        IEnumerable<ActionTDownload> bestDownloads = Rationalize(newItemsForThisMissingEpisode);

        IEnumerable<ActionTDownload> actionTDownloads = [.. bestDownloads];
        if (actionTDownloads.HasAny())
        {
            foreach (ActionTDownload x in actionTDownloads)
            {
                x.AlsoAvailable = newItemsForThisMissingEpisode;
            }
            newItems.AddNullableRange(actionTDownloads);
            toRemove.Add(action);
        }
    }
}
