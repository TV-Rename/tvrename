using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    using System;
    using FileInfo = FileInfo;
    using DirectoryInfo = DirectoryInfo;

    public abstract class ActionDelete : ActionFileOperation
    {
        public override string Name => "Delete";
        public override long SizeOfWork => 100;
        public override int IconNumber => 9;
        public override string ScanListViewGroup => "lvgActionDelete";
        protected override string FileInfo1 => TargetFolder;
        protected override string FileInfo2 => ProgressText;
        protected override string FileInfo3 => string.Empty;
        protected override string FileInfo4 => string.Empty;
    }
    public class ActionDeleteFile : ActionDelete
    {
        private readonly FileInfo toRemove;

        public ActionDeleteFile(FileInfo remove, ProcessedEpisode ep, TidySettings tidyup)
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

    public class ActionDeleteDirectory : ActionDelete
    {
        private readonly DirectoryInfo toRemove;

        public ActionDeleteDirectory(DirectoryInfo remove, ProcessedEpisode ep, TidySettings tidyup)
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
                if ((toRemove.Exists) )
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
