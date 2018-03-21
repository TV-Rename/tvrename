using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    using System;
    using System.Windows.Forms;
    using FileInfo = FileInfo;
    using DirectoryInfo = DirectoryInfo;

    public abstract class ActionDelete : ActionFileOperation
    {
        public override string Name => "Delete";
        // 0.0 to 100.0
        public override long SizeOfWork => 100;

        public override int IconNumber => 9;

        public override string ScanListViewGroup => "lvgActionDelete";

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                if (this.Episode == null)
                {
                    lvi.Text = "";
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                }
                else
                {
                    lvi.Text = this.Episode.TheSeries.Name;
                    lvi.SubItems.Add(this.Episode.AppropriateSeasonNumber.ToString());
                    lvi.SubItems.Add(this.Episode.NumsAsString());
                    DateTime? dt = this.Episode.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }

                lvi.SubItems.Add(this.TargetFolder);
                lvi.SubItems.Add(this.ProgressText);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                return lvi;
            }
        }


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
            ActionDeleteFile cmr = o as ActionDeleteFile;

            return (cmr != null) && FileHelper.Same(this.toRemove , cmr.toRemove);
        }

        public override int Compare(Item o)
        {
            ActionDeleteFile cmr = o as ActionDeleteFile;

            if (cmr == null || this.toRemove.Directory == null || cmr.toRemove.Directory == null )
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
                TVSettings.Instance.SearchFoldersNames.Contains(this.toRemove.FullName))
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
