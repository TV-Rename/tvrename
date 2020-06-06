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
                        LOGGER.Info(
                            $"Removing {testActionRssTwo.Produces} as it is not as good a match as {testActionRssOne.Produces}");
                    }
                }
            }

            return duplicateActionRss;
        }
    }
}
