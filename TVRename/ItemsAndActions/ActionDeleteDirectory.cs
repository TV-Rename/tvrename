//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public class ActionDeleteDirectory : ActionDelete
    {
        private readonly DirectoryInfo toRemove;
        protected readonly ShowConfiguration? SelectedShow; // if for an entire show, rather than specific episode

        public ActionDeleteDirectory(DirectoryInfo remove)
        {
            Tidyup = TVSettings.Instance.Tidyup;
            PercentDone = 0;
            Episode = null;
            Movie = null;
            toRemove = remove;
        }
        public ActionDeleteDirectory(DirectoryInfo remove, ProcessedEpisode ep, TVSettings.TidySettings tidyup)
        {
            Tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            toRemove = remove;
        }

        public ActionDeleteDirectory(DirectoryInfo remove, MovieConfiguration mi, TVSettings.TidySettings tidyup)
        {
            Tidyup = tidyup;
            PercentDone = 0;
            Episode = null;
            Movie = mi;
            toRemove = remove;
        }

        public ActionDeleteDirectory(DirectoryInfo remove, ShowConfiguration si, TVSettings.TidySettings tidyup)
        {
            Tidyup = tidyup;
            PercentDone = 0;
            Episode = null;
            Movie = null;
            toRemove = remove;
            SelectedShow = si;
        }

        public override string ProgressText => toRemove.Name;
        public override string Produces => toRemove.FullName;
        public override IgnoreItem Ignore => new(toRemove.FullName);
        public override string TargetFolder => toRemove.Parent.FullName;

        public override string SeriesName => Episode?.Show.ShowName ?? SelectedShow?.ShowName ?? Movie?.ShowName ?? toRemove.Name;

        public override ActionOutcome Go(TVRenameStats stats)
        {
            //if the directory is the root download folder do not delete
            if (TVSettings.Instance.MonitorFolders &&
                TVSettings.Instance.DownloadFolders.Contains(toRemove.FullName))
            {
                return new ActionOutcome($@"Not removing {toRemove.FullName} as it is a Search Folder");
            }

            try
            {
                if (toRemove.Exists)
                {
                    DeleteOrRecycleFolder(toRemove);
                    if (Tidyup is { DeleteEmpty: true })
                    {
                        LOGGER.Info($"Testing {toRemove.Parent.FullName } to see whether it should be tidied up");
                        DoTidyUp(toRemove.Parent);
                    }
                }
                return ActionOutcome.Success();
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                LOGGER.Info($"Testing {toRemove.FullName} but it has already been removed - Job Done!");
                return ActionOutcome.Success();
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }
        }

        public override bool SameAs(Item o)
        {
            return o is ActionDeleteDirectory cmr && FileHelper.Same(toRemove, cmr.toRemove);
        }

        public override int CompareTo(Item o)
        {
            if (o is not ActionDeleteDirectory cmr || toRemove.Parent.FullName is null || cmr.toRemove.Parent.FullName is null)
            {
                return -1;
            }

            return string.Compare(toRemove.FullName, cmr.toRemove.FullName, StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteDirectory o) => FileHelper.Same(toRemove, o.toRemove);

        public bool IsFor(string folderName) => string.Equals(folderName, toRemove.FullName, StringComparison.OrdinalIgnoreCase);
    }
}
