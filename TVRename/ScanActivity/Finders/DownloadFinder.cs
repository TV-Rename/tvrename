// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using JetBrains.Annotations;

namespace TVRename
{
    public abstract class DownloadFinder:Finder
    {
        protected DownloadFinder(TVDoc doc) : base(doc)
        {
        }
        public override FinderDisplayType DisplayType() => FinderDisplayType.search;

        protected static bool RssMatch([NotNull] RSSItem rss, [NotNull] ProcessedEpisode pe)
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

        protected static bool RssMatch([NotNull] RSSItem rss, [NotNull] MovieConfiguration pe)
        {
            string simpleShowName = pe.ShowName.CompareName();

            return FileHelper.SimplifyAndCheckFilename(rss.ShowName.HasValue()? rss.ShowName: rss.Title, simpleShowName, true, false);
        }

        [NotNull]
        protected static IEnumerable<ActionTDownload> FindDuplicates([NotNull] ItemList newItems)
        {
            //We now want to rationlise the newItems - just in case we've added duplicates
            List<ActionTDownload> duplicateActionRss = new List<ActionTDownload>();

            foreach (Item x in newItems)
            {
                if (!(x is ActionTDownload testActionRssOne))
                {
                    continue;
                }

                foreach (Item y in newItems)
                {
                    if (!(y is ActionTDownload testActionRssTwo))
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
}
