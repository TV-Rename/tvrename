// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using Alphaleonis.Win32.Filesystem;

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
        public override IgnoreItem Ignore => toRemove == null ? null : new IgnoreItem(toRemove.FullName);
        public override string TargetFolder => toRemove?.Parent.FullName;

        public override bool Go(ref bool pause, TVRenameStats stats)
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
                        Logger.Info($"Testing {toRemove.Parent.FullName } to see whether it should be tidied up");
                        DoTidyup(toRemove.Parent);
                    }
                }
            }
            catch (Exception e)
            {
                Error = true;
                ErrorText = e.Message;
            }
            Done = true;
            return !Error;
        }

        public override bool SameAs(Item o)
        {
            return (o is ActionDeleteDirectory cmr) && FileHelper.Same(toRemove, cmr.toRemove);
        }

        public override int Compare(Item o)
        {
            if (!(o is ActionDeleteDirectory cmr) || toRemove.Parent.FullName == null || cmr.toRemove.Parent.FullName == null)
                return 0;

            return string.Compare(toRemove.FullName, cmr.toRemove.FullName, StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteDirectory o) => FileHelper.Same(toRemove, o.toRemove);
    }
}
