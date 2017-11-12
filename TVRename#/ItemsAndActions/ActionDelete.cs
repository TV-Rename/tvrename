using System;
using System.CodeDom;
using System.Diagnostics;
using Alphaleonis.Win32.Filesystem;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;
    using System.Windows.Forms;
    using System.IO;

    using File = Alphaleonis.Win32.Filesystem.File;		
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;		
    using FileSystemInfo = Alphaleonis.Win32.Filesystem.FileSystemInfo;		
    using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;


    public class ActionDeleteFile : ActionFileOperation
    {

        public FileInfo toRemove;
        

        public ActionDeleteFile(FileInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            _tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            toRemove = remove;
            
        }


        public override string Name => "Delete";

        public override string ProgressText => toRemove.Name;


        // 0.0 to 100.0
        public override long SizeOfWork => 100 ;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try {
                DeleteOrRecycleFile(toRemove );
                if (_tidyup != null && _tidyup.DeleteEmpty)
                {
                    DoTidyup(toRemove.Directory);
                }

            }
            catch (System.Exception e)
            {
                this.Error = true;
                this.ErrorText = e.Message;
            }
            this.Done = true;
            return !Error;
        }
       
        public override string produces => toRemove.FullName;


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

            return string.Compare(this.toRemove.FullName , cmr.toRemove.FullName , StringComparison.Ordinal);
        }


        
        public override int IconNumber => 9;



        public override IgnoreItem Ignore => toRemove  == null ? null : new IgnoreItem(toRemove.FullName);

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

	            lvi.SubItems.Add(toRemove.DirectoryName);
                lvi.SubItems.Add(toRemove.Name);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                return lvi;
            }
        }

        public override string ScanListViewGroup => "lvgActionDelete";

        public override string TargetFolder => toRemove?.DirectoryName;


        // --------------------------------------------------------------------------------------------------------

        public bool SameSource(ActionDeleteFile o) => FileHelper.Same(this.toRemove , o.toRemove);

        // ========================================================================================================

    }

    public class ActionDeleteDirectory : ActionFileOperation
    {

        public DirectoryInfo toRemove;


        public ActionDeleteDirectory(DirectoryInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            _tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            toRemove = remove;

        }


        public override string Name => "Delete";

        public override string ProgressText => toRemove.Name;


        // 0.0 to 100.0
        public override long SizeOfWork => 100;

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                DeleteOrRecycleFolder(toRemove);
                if (_tidyup != null && _tidyup.DeleteEmpty)
                {
                    DoTidyup(toRemove.Parent );
                }

            }
            catch (System.Exception e)
            {
                this.Error = true;
                this.ErrorText = e.Message;
            }
            this.Done = true;
            return !Error;
        }

        public override string produces => toRemove.FullName;


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

            return string.Compare(this.toRemove.FullName, cmr.toRemove.FullName, StringComparison.Ordinal);
        }



        public override int IconNumber => 9;



        public override IgnoreItem Ignore => toRemove == null ? null : new IgnoreItem(toRemove.FullName);

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

                lvi.SubItems.Add(toRemove.Parent.FullName);
                lvi.SubItems.Add(toRemove.Name);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                return lvi;
            }
        }

        public override string ScanListViewGroup => "lvgActionDelete";

        public override string TargetFolder => toRemove?.Parent.FullName ;


        // --------------------------------------------------------------------------------------------------------

        public bool SameSource(ActionDeleteDirectory o) => FileHelper.Same(this.toRemove, o.toRemove);

        // ========================================================================================================

    }
}