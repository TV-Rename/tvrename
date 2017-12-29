using System;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
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
                    DateTime? dt = Episode.GetAirDateDt(true);
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

        public FileInfo ToRemove;
        

        public ActionDeleteFile(FileInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            Tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            ToRemove = remove;
            
        }
        
        public override string ProgressText => ToRemove.Name;
        public override string Produces => ToRemove.FullName;
        public override IgnoreItem Ignore => ToRemove == null ? null : new IgnoreItem(ToRemove.FullName);
        public override string TargetFolder => ToRemove?.DirectoryName;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (ToRemove.Exists)
                {
                    DeleteOrRecycleFile(ToRemove);
                    if (Tidyup != null && Tidyup.DeleteEmpty)
                    {
                        DoTidyup(ToRemove.Directory);
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

            return (cmr != null) && FileHelper.Same(ToRemove , cmr.ToRemove);
        }

        public override int Compare(Item o)
        {
            ActionDeleteFile cmr = o as ActionDeleteFile;

            if (cmr == null || ToRemove.Directory == null || cmr.ToRemove.Directory == null )
                return 0;

            return string.Compare(ToRemove.FullName , cmr.ToRemove.FullName , StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteFile o) => FileHelper.Same(ToRemove , o.ToRemove);

    }

    public class ActionDeleteDirectory : ActionDelete
    {

        public DirectoryInfo ToRemove;


        public ActionDeleteDirectory(DirectoryInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            Tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            ToRemove = remove;

        }


        public override string ProgressText => ToRemove.Name;
        public override string Produces => ToRemove.FullName;
        public override IgnoreItem Ignore => ToRemove == null ? null : new IgnoreItem(ToRemove.FullName);
        public override string TargetFolder => ToRemove?.Parent.FullName;


        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (ToRemove.Exists)
                {
                    DeleteOrRecycleFolder(ToRemove);
                    if (Tidyup != null && Tidyup.DeleteEmpty)
                    {
                        DoTidyup(ToRemove.Parent);
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

            return (cmr != null) && FileHelper.Same(ToRemove, cmr.ToRemove);
        }

        public override int Compare(Item o)
        {
            ActionDeleteDirectory cmr = o as ActionDeleteDirectory;

            if (cmr == null || ToRemove.Parent.FullName == null || cmr.ToRemove.Parent.FullName == null)
                return 0;

            return string.Compare(ToRemove.FullName, cmr.ToRemove.FullName, StringComparison.Ordinal);
        }

        public bool SameSource(ActionDeleteDirectory o) => FileHelper.Same(ToRemove, o.ToRemove);


    }
}
