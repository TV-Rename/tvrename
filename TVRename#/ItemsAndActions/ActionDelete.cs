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

                if (Episode == null)
                {
                    lvi.Text = "";
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                }
                else
                {
                    lvi.Text = Episode.TheSeries.Name;
                    lvi.SubItems.Add(Episode.SeasonNumber.ToString());
                    lvi.SubItems.Add(Episode.NumsAsString());
                    DateTime? dt = Episode.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }

                lvi.SubItems.Add(TargetFolder);
                lvi.SubItems.Add(ProgressText);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                return lvi;
            }
        }


    }
    public class ActionDeleteFile : ActionDelete
    {

        public FileInfo toRemove;
        

        public ActionDeleteFile(FileInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            _tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            toRemove = remove;
            
        }
        
        public override string ProgressText => toRemove.Name;
        public override string produces => toRemove.FullName;
        public override IgnoreItem Ignore => toRemove == null ? null : new IgnoreItem(toRemove.FullName);
        public override string TargetFolder => toRemove?.DirectoryName;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (toRemove.Exists)
                {
                    DeleteOrRecycleFile(toRemove);
                    if (_tidyup != null && _tidyup.DeleteEmpty)
                    {
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
            ActionDeleteFile cmr = o as ActionDeleteFile;

            return (cmr != null) && FileHelper.Same(toRemove , cmr.toRemove);
        }

        public override int Compare(Item o)
        {
            ActionDeleteFile cmr = o as ActionDeleteFile;

            if (cmr == null || toRemove.Directory == null || cmr.toRemove.Directory == null )
                return 0;

            return string.Compare(toRemove.FullName , cmr.toRemove.FullName , StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteFile o) => FileHelper.Same(toRemove , o.toRemove);

    }

    public class ActionDeleteDirectory : ActionDelete
    {

        public DirectoryInfo toRemove;


        public ActionDeleteDirectory(DirectoryInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            _tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            toRemove = remove;

        }


        public override string ProgressText => toRemove.Name;
        public override string produces => toRemove.FullName;
        public override IgnoreItem Ignore => toRemove == null ? null : new IgnoreItem(toRemove.FullName);
        public override string TargetFolder => toRemove?.Parent.FullName;


        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (toRemove.Exists)
                {
                    DeleteOrRecycleFolder(toRemove);
                    if (_tidyup != null && _tidyup.DeleteEmpty)
                    {
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
            ActionDeleteDirectory cmr = o as ActionDeleteDirectory;

            return (cmr != null) && FileHelper.Same(toRemove, cmr.toRemove);
        }

        public override int Compare(Item o)
        {
            ActionDeleteDirectory cmr = o as ActionDeleteDirectory;

            if (cmr == null || toRemove.Parent.FullName == null || cmr.toRemove.Parent.FullName == null)
                return 0;

            return string.Compare(toRemove.FullName, cmr.toRemove.FullName, StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteDirectory o) => FileHelper.Same(toRemove, o.toRemove);


    }
}
