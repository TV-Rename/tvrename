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


    public class ActionDelete : Item, Action, ScanListItem
    {

        public FileInfo toRemove;
        private double _percent;
        private readonly TidySettings _tidyup;

        public ActionDelete(FileInfo remove, ProcessedEpisode ep, TidySettings tidyup)
        {
            _tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            toRemove = remove;
            
        }

        #region Action Members

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }

        public string Name => "Delete";

        public string ProgressText => toRemove.Name;

        public double PercentDone
        {
            get { return Done ? 100.0 : _percent; }
            set { _percent = value; }
        }

        // 0.0 to 100.0
        public long SizeOfWork => 100 ;

        public bool Go(ref bool pause, TVRenameStats stats)
        {
            try {
                DeleteOrRecycleFile();
                if (_tidyup != null && _tidyup.DeleteEmpty)
                {
                    DoTidyup();
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

        private void DeleteOrRecycleFile()
        {
            if (toRemove  == null) return;
            if (_tidyup.DeleteEmptyIsRecycle)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(toRemove.FullName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                toRemove.Delete(true);
            }
        }



        private void DeleteOrRecycleFolder()
        {
            DirectoryInfo di = toRemove.Directory;
            if (di == null) return;
            if (_tidyup.DeleteEmptyIsRecycle)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(di.FullName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                di.Delete();
            }
        }


        private void DoTidyup()
        {
#if DEBUG
            Debug.Assert(this._tidyup != null);
            Debug.Assert(this._tidyup.DeleteEmpty);
#else
            if (_tidyup == null || !_tidyup.DeleteEmpty)
                return;
#endif
            // See if we should now delete the folder we just moved that file from.
            DirectoryInfo di = toRemove.Directory;
            if (di == null)
                return;

            //if there are sub-directories then we shouldn't remove this one
            if (di.GetDirectories().Length > 0)
                return;


            FileInfo[] files = di.GetFiles();
            if (files.Length == 0)
            {
                // its empty, so just delete it
                di.Delete();
                return;
            }


            if (_tidyup.EmptyIgnoreExtensions && !_tidyup.EmptyIgnoreWords)
                return; // nope

            foreach (FileInfo fi in files)
            {
                bool okToDelete = _tidyup.EmptyIgnoreExtensions &&
                                 Array.FindIndex(_tidyup.EmptyIgnoreExtensionsArray, x => x == fi.Extension) != -1;

                if (okToDelete)
                    continue; // onto the next file

                // look in the filename
                if (_tidyup.EmptyIgnoreWordsArray.Any(word => fi.Name.Contains(word)))
                    okToDelete = true;

                if (!okToDelete)
                    return;
            }

            if (_tidyup.EmptyMaxSizeCheck)
            {
                // how many MB are we deleting?
                long totalBytes = files.Sum(fi => fi.Length);

                if (totalBytes/(1024*1024) > _tidyup.EmptyMaxSizeMB)
                    return; // too much
            }
            DeleteOrRecycleFolder();
        }

        #endregion

        public string produces => toRemove.FullName;

        #region Item Members

        public bool SameAs(Item o)
        {
            ActionDelete cmr = o as ActionDelete;

            return (cmr != null) && FileHelper.Same(toRemove , cmr.toRemove);
        }

        public int Compare(Item o)
        {
            ActionDelete cmr = o as ActionDelete;

            if (cmr == null || toRemove.Directory == null || cmr.toRemove.Directory == null )
                return 0;

            return string.Compare(this.toRemove.FullName , cmr.toRemove.FullName , StringComparison.Ordinal);
        }

        #endregion

        #region ScanListItem Members
        
        public int IconNumber => 9;

        public ProcessedEpisode Episode { get; set; }

        public IgnoreItem Ignore => toRemove  == null ? null : new IgnoreItem(toRemove.FullName);

        public ListViewItem ScanListViewItem
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

        public string ScanListViewGroup => "lvgActionDelete";

        public string TargetFolder => toRemove?.DirectoryName;

        #endregion

        private CopyMoveProgressResult CopyProgressCallback(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, int StreamNumber, CopyMoveProgressCallbackReason CallbackReason, Object UserData)
        {
             double pct = TotalBytesTransferred * 100.0 / TotalFileSize;
             this.PercentDone = pct > 100.0 ? 100.0 : pct;
             return CopyMoveProgressResult.Continue;
         }

        // --------------------------------------------------------------------------------------------------------

        public bool SameSource(ActionDelete o) => FileHelper.Same(this.toRemove , o.toRemove);

        // ========================================================================================================

    }
}