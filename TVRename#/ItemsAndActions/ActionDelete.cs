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
        public override string FileInfo1 => this.TargetFolder;
        public override string FileInfo2 => this.ProgressText;
        public override string FileInfo3 => string.Empty;
        public override string FileInfo4 => string.Empty;
    }
    public class ActionDeleteFile : ActionDelete
    {
        private readonly FileInfo toRemove;

        public ActionDeleteFile(FileInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            this.Tidyup = tidyup;
            this.PercentDone = 0;
            this.Episode = ep;
            this.toRemove = remove;
        }
        
        public override string ProgressText => this.toRemove.Name;
        public override string Produces => this.toRemove.FullName;
        public override IgnoreItem Ignore => this.toRemove == null ? null : new IgnoreItem(this.toRemove.FullName);
        public override string TargetFolder => this.toRemove?.DirectoryName;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (this.toRemove.Exists)
                {
                    DeleteOrRecycleFile(this.toRemove);
                    if (this.Tidyup != null && this.Tidyup.DeleteEmpty)
                    {
                        logger.Info($"Testing {this.toRemove.Directory.FullName } to see whether it should be tidied up");
                        DoTidyup(this.toRemove.Directory);
                    }
                }

            }
            catch (Exception e)
            {
                this.Error = true;
                this.ErrorText = e.Message;
            }
            this.Done = true;
            return !this.Error;
        }
       
        public override bool SameAs(Item o)
        {
            return (o is ActionDeleteFile cmr) && FileHelper.Same(this.toRemove , cmr.toRemove);
        }

        public override int Compare(Item o)
        {
            if (!(o is ActionDeleteFile cmr) || this.toRemove.Directory == null || cmr.toRemove.Directory == null )
                return 0;

            return string.Compare(this.toRemove.FullName , cmr.toRemove.FullName , StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteFile o) => FileHelper.Same(this.toRemove , o.toRemove);
    }



    public class ActionDeleteDirectory : ActionDelete
    {

        private readonly DirectoryInfo toRemove;


        public ActionDeleteDirectory(DirectoryInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            this.Tidyup = tidyup;
            this.PercentDone = 0;
            this.Episode = ep;
            this.toRemove = remove;

        }


        public override string ProgressText => this.toRemove.Name;
        public override string Produces => this.toRemove.FullName;
        public override IgnoreItem Ignore => this.toRemove == null ? null : new IgnoreItem(this.toRemove.FullName);
        public override string TargetFolder => this.toRemove?.Parent.FullName;


        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            //if the directory is the root download folder do not delete
            if (TVSettings.Instance.MonitorFolders &&
                TVSettings.Instance.DownloadFolders.Contains(this.toRemove.FullName))
            {
                this.Error = true;
                this.ErrorText = $@"Not removing {this.toRemove.FullName} as it is a Search Folder";
                return false;
            }

            try
            {
                if ((this.toRemove.Exists) )
                {
                    DeleteOrRecycleFolder(this.toRemove);
                    if (this.Tidyup != null && this.Tidyup.DeleteEmpty)
                    {
                        logger.Info($"Testing {this.toRemove.Parent.FullName } to see whether it should be tidied up");
                        DoTidyup(this.toRemove.Parent);
                    }
                }

            }
            catch (Exception e)
            {
                this.Error = true;
                this.ErrorText = e.Message;
            }
            this.Done = true;
            return !this.Error;
        }

        public override bool SameAs(Item o)
        {
            return (o is ActionDeleteDirectory cmr) && FileHelper.Same(this.toRemove, cmr.toRemove);
        }

        public override int Compare(Item o)
        {
            if (!(o is ActionDeleteDirectory cmr) || this.toRemove.Parent.FullName == null || cmr.toRemove.Parent.FullName == null)
                return 0;

            return string.Compare(this.toRemove.FullName, cmr.toRemove.FullName, StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteDirectory o) => FileHelper.Same(this.toRemove, o.toRemove);


    }
}
