// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
    using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;


    public interface Item
    {
        int Compare(Item o); // for sorting items in scan list (ActionItemSorter)
        bool SameAs(Item o); // are we the same thing as that other one?
    }

    public class ItemList : System.Collections.Generic.List<Item>
    {
        public void Add(ItemList il)
        {
            if (il != null) 
            {
                foreach (Item i in il)
                {
                    Add(i);
                }
            }
        }


    }

    public interface Action // Something we can do
    {
        string Name { get; } // Name of this action, e.g. "Copy", "Move", "Download"
        bool Done { get; } // All work has been completed for this item, and can be removed from to-do list.  set to true on completion, even on error.
        bool Error { get; } // Error state, after trying to do work?
        string ErrorText { get; } // Human-readable error message, for when Error is true
        string ProgressText { get; } // shortish text to display to user while task is running
        double PercentDone { get; } // 0.0 to 100.0
        long SizeOfWork { get; } // for file copy/move, number of bytes in file.  for simple tasks, 1, or something proportional to how slow it is to copy files around.
        bool Go( ref bool pause, TVRenameStats stats); // action the action.  do not return until done.  will be run in a dedicated thread.  if pause is set to true, stop working until it goes back to false        
        string produces { get; } //What does this action produce? typically a filename
    }

    public interface ScanListItem // something shown in the list on the Scan tab (not always an Action)
    {
        ListViewItem ScanListViewItem { get; } // to add to Scan ListView
        string TargetFolder { get; } // return a list of folders for right-click menu
        string ScanListViewGroup { get; } // which group name for the listview
        int IconNumber { get; } // which icon number to use in "ilIcons" (UI.cs). -1 for none
        IgnoreItem Ignore { get; } // what to add to the ignore list / compare against the ignore list
        ProcessedEpisode Episode { get; } // associated episode
    }

    public class ScanListItemList : System.Collections.Generic.List<ScanListItem>
    {
        public void Add(ScanListItemList slil)
        {
            if (slil != null)
            {
                foreach (ScanListItem sli in slil)
                {
                    Add(sli);
                }
            }
        }
    }

    public class ActionQueue
    {
        public System.Collections.Generic.List<Action> Actions; // The contents of this queue
        public int ParallelLimit; // Number of tasks in the queue than can be run at once
        public string Name; // Name of this queue
        public int ActionPosition; // Position in the queue list of the next item to process

        public ActionQueue(string name, int parallelLimit)
        {
            Name = name;
            ParallelLimit = parallelLimit;
            Actions = new System.Collections.Generic.List<Action>();
            ActionPosition = 0;
        }
    }

    public abstract class ActionFileOperation : Item, Action, ScanListItem
    {
        protected double _percent;
        protected TidySettings _tidyup;

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }

        public double PercentDone
        {
            get { return Done ? 100.0 : _percent; }
            set { _percent = value; }
        }

        protected void DeleteOrRecycleFile(FileInfo file )
        {
            if (file == null) return;
            if (_tidyup.DeleteEmptyIsRecycle)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file.FullName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                file.Delete(true);
            }
        }

        protected void DeleteOrRecycleFolder(DirectoryInfo di)
        {
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

        protected void DoTidyup(DirectoryInfo di)
        {
#if DEBUG
            Debug.Assert(_tidyup != null);
            Debug.Assert(_tidyup.DeleteEmpty);
#else
            if (_tidyup == null || !_tidyup.DeleteEmpty)
                return;
#endif
            // See if we should now delete the folder we just moved that file from.
            if (di == null)
                return;

            //if there are sub-directories then we shouldn't remove this one
            if (di.GetDirectories().Length > 0)
                return;

            //if the directory is the root download folder do not delete
            if (TVSettings.Instance.MonitorFolders && TVSettings.Instance.SearchFoldersNames.Contains(di.FullName))
                return;

            // Do not delete any monitor folders either
            if (TVSettings.Instance.MonitorFoldersNames.Contains(di.FullName))
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

                if (totalBytes / (1024 * 1024) > _tidyup.EmptyMaxSizeMB)
                    return; // too much
            }
            DeleteOrRecycleFolder(di);
        }
        public ProcessedEpisode Episode { get; set; }
        public abstract string Name { get; }
        public abstract string ProgressText { get; }
        public abstract long SizeOfWork { get; }
        public abstract string produces { get; }
        public abstract ListViewItem ScanListViewItem { get; }
        public abstract string TargetFolder { get; }
        public abstract string ScanListViewGroup { get; }
        public abstract int IconNumber { get; }
        public abstract IgnoreItem Ignore { get; }
        public abstract int Compare(Item o);
        public abstract bool SameAs(Item o);
        public abstract bool Go(ref bool pause, TVRenameStats stats);
    }
}
