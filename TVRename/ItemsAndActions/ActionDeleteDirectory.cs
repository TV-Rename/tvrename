// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    public class ActionDeleteDirectory : ActionDelete
    {
        private readonly DirectoryInfo toRemove;

        public ActionDeleteDirectory(DirectoryInfo remove, ProcessedEpisode ep, TVSettings.TidySettings tidyup)
        {
            Tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            toRemove = remove;
        }

        public override string ProgressText => toRemove.Name;
        public override string Produces => toRemove.FullName;
        [CanBeNull]
        public override IgnoreItem Ignore => toRemove is null ? null : new IgnoreItem(toRemove.FullName);
        [CanBeNull]
        public override string TargetFolder => toRemove?.Parent.FullName;

        public override bool Go(TVRenameStats stats)
        {
            //if the directory is the root download folder do not delete
            if (TVSettings.Instance.MonitorFolders &&
                TVSettings.Instance.DownloadFolders.Contains(toRemove.FullName))
            {
                Error = true;
                ErrorText = $@"Not removing {toRemove.FullName} as it is a Search Folder";
                return false;
            }

            try
            {
                if (toRemove.Exists)
                {
                    DeleteOrRecycleFolder(toRemove);
                    if (Tidyup != null && Tidyup.DeleteEmpty)
                    {
                        LOGGER.Info($"Testing {toRemove.Parent.FullName } to see whether it should be tidied up");
                        DoTidyup(toRemove.Parent);
                    }
                }
            }
            catch (Exception e)
            {
                Error = true;
                ErrorText = e.Message;
                LastError = e;
            }
            Done = true;
            return !Error;
        }

        public override bool SameAs(Item o)
        {
            return o is ActionDeleteDirectory cmr && FileHelper.Same(toRemove, cmr.toRemove);
        }

        public override int Compare(Item o)
        {
            if (!(o is ActionDeleteDirectory cmr) || toRemove.Parent.FullName is null || cmr.toRemove.Parent.FullName is null)
            {
                return 0;
            }

            return string.Compare(toRemove.FullName, cmr.toRemove.FullName, StringComparison.Ordinal);
        }

        public bool SameSource([NotNull] ActionDeleteDirectory o) => FileHelper.Same(toRemove, o.toRemove);
    }
}
