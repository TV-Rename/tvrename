//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Generic;

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

        if (rss.Season != pe.AppropriateSeasonNumber)
        {
            return false;
        }
        if (rss.Episode != pe.AppropriateEpNum)
        {
            return false;
        }

        return true;
    }

    protected static bool RssMatch(RSSItem rss, MovieConfiguration pe)
    {
        string simpleShowName = pe.ShowName.CompareName();

        return FileHelper.SimplifyAndCheckFilename(rss.ShowName.HasValue() ? rss.ShowName : rss.Title, simpleShowName, true, false);
    }

    protected static IEnumerable<ActionTDownload> FindDuplicates(ItemList newItems)
    {
        //We now want to rationlise the newItems - just in case we've added duplicates
        List<ActionTDownload> duplicateActionRss = new();
        string[] dudTerms = TVSettings.Instance.UnwantedRSSSearchTerms();

        foreach (Item x in newItems)
        {
            if (x is not ActionTDownload testActionRssOne)
            {
                continue;
            }

            if (testActionRssOne.SourceName.ContainsOneOf(dudTerms))
            {
                duplicateActionRss.Add(testActionRssOne);
                if (TVSettings.Instance.DetailedRSSJSONLogging)
                {
                    LOGGER.Info(
                        $"Removing {testActionRssOne.Produces} as contains a term in {dudTerms.ToCsv()}");
                }

                continue;
            }

            foreach (Item y in newItems)
            {
                if (y is not ActionTDownload testActionRssTwo)
                {
                    continue;
                }

                if (x.Equals(y))
                {
                    continue;
                }

                string[] preferredTerms = TVSettings.Instance.PreferredRSSSearchTerms();

                if (testActionRssOne.SourceName.ContainsOneOf(preferredTerms) &&
                    !testActionRssTwo.SourceName.ContainsOneOf(preferredTerms))
                {
                    duplicateActionRss.Add(testActionRssTwo);
                    if (TVSettings.Instance.DetailedRSSJSONLogging)
                    {
                        LOGGER.Info(
                            $"Removing {testActionRssTwo.Produces} as it is not as good a match as {testActionRssOne.Produces}");
                    }
                }
            }
        }

        return duplicateActionRss;
    }
}
