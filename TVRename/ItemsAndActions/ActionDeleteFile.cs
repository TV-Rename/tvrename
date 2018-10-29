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
    public class ActionDeleteFile : ActionDelete
    {
        private readonly FileInfo toRemove;

        public ActionDeleteFile(FileInfo remove, ProcessedEpisode ep, TVSettings.TidySettings tidyup)
        {
            Tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            toRemove = remove;
        }

        public override string ProgressText => toRemove.Name;
        public override string Produces => toRemove.FullName;
        public override IgnoreItem Ignore => toRemove == null ? null : new IgnoreItem(toRemove.FullName);
        public override string TargetFolder => toRemove?.DirectoryName;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (toRemove.Exists)
                {
                    DeleteOrRecycleFile(toRemove);
                    if (Tidyup != null && Tidyup.DeleteEmpty)
                    {
                        Logger.Info($"Testing {toRemove.Directory.FullName } to see whether it should be tidied up");
                        DoTidyup(toRemove.Directory);
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
            return (o is ActionDeleteFile cmr) && FileHelper.Same(toRemove , cmr.toRemove);
        }

        public override int Compare(Item o)
        {
            if (!(o is ActionDeleteFile cmr) || toRemove.Directory == null || cmr.toRemove.Directory == null )
                return 0;

            return string.Compare(toRemove.FullName , cmr.toRemove.FullName , StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteFile o) => FileHelper.Same(toRemove , o.toRemove);
    }
}
