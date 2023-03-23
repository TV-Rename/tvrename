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

namespace TVRename;

public abstract class DownloadFinder : Finder
{
    protected DownloadFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
    }

    public override FinderDisplayType DisplayType() => FinderDisplayType.search;

    protected static bool RssMatch(RSSItem rss, ProcessedEpisode pe)
    {
        string simpleShowName = pe.Show.ShowName.CompareName();
        string simpleSeriesName = pe.TheCachedSeries.Name.CompareName();

        if (!FileHelper.SimplifyAndCheckFilename(rss.ShowName, simpleShowName, true, false) &&
            !(
                string.IsNullOrEmpty(rss.ShowName) &&
                FileHelper.SimplifyAndCheckFilename(rss.Title, simpleSeriesName, true, false)
            )
           )
        {
            return false;
        }

        return rss.Season == pe.AppropriateSeasonNumber && rss.Episode == pe.AppropriateEpNum;
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
}
