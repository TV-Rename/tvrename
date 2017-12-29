// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualBasic.FileIO;

namespace TVRename
{
    public interface ITem
    {
        int Compare(ITem o); // for sorting items in scan list (ActionItemSorter)
        bool SameAs(ITem o); // are we the same thing as that other one?
    }

    public class ItemList : List<ITem>
    {
        public void Add(ItemList il)
        {
            if (il != null) 
            {
                foreach (ITem i in il)
                {
                    Add(i);
                }
            }
        }


    }

    public interface IAction // Something we can do
    {
        string Name { get; } // Name of this action, e.g. "Copy", "Move", "Download"
        bool Done { get; } // All work has been completed for this item, and can be removed from to-do list.  set to true on completion, even on error.
        bool Error { get; } // Error state, after trying to do work?
        string ErrorText { get; } // Human-readable error message, for when Error is true
        string ProgressText { get; } // shortish text to display to user while task is running
        double PercentDone { get; } // 0.0 to 100.0
        long SizeOfWork { get; } // for file copy/move, number of bytes in file.  for simple tasks, 1, or something proportional to how slow it is to copy files around.
        bool Go( ref bool pause, TVRenameStats stats); // action the action.  do not return until done.  will be run in a dedicated thread.  if pause is set to true, stop working until it goes back to false        
        string Produces { get; } //What does this action produce? typically a filename
    }

    public interface IScanListItem // something shown in the list on the Scan tab (not always an Action)
    {
        ListViewItem ScanListViewItem { get; } // to add to Scan ListView
        string TargetFolder { get; } // return a list of folders for right-click menu
        string ScanListViewGroup { get; } // which group name for the listview
        int IconNumber { get; } // which icon number to use in "ilIcons" (UI.cs). -1 for none
        IgnoreItem Ignore { get; } // what to add to the ignore list / compare against the ignore list
        ProcessedEpisode Episode { get; } // associated episode
    }

    public class ScanListItemList : List<IScanListItem>
    {
        public void Add(ScanListItemList slil)
        {
            if (slil != null)
            {
                foreach (IScanListItem sli in slil)
                {
                    Add(sli);
                }
            }
        }
    }

    public class ActionQueue
    {
        public List<IAction> Actions; // The contents of this queue
        public int ParallelLimit; // Number of tasks in the queue than can be run at once
        public string Name; // Name of this queue
        public int ActionPosition; // Position in the queue list of the next item to process

        public ActionQueue(string name, int parallelLimit)
        {
            Name = name;
            ParallelLimit = parallelLimit;
            Actions = new List<IAction>();
            ActionPosition = 0;
        }
    }

    public abstract class ActionFileOperation : ITem, IAction, IScanListItem
    {
        protected double Percent;
        protected TidySettings Tidyup;

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }

        public double PercentDone
        {
            get { return Done ? 100.0 : Percent; }
            set { Percent = value; }
        }

        protected void DeleteOrRecycleFile(FileInfo file )
        {
            if (file == null) return;
            if (Tidyup.DeleteEmptyIsRecycle)
            {
                FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else
            {
                file.Delete(true);
            }
        }

        protected void DeleteOrRecycleFolder(DirectoryInfo di)
        {
            if (di == null) return;
            if (Tidyup.DeleteEmptyIsRecycle)
            {
                FileSystem.DeleteDirectory(di.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else
            {
                di.Delete();
            }
        }

        protected void DoTidyup(DirectoryInfo di)
        {
#if DEBUG
            Debug.Assert(Tidyup != null);
            Debug.Assert(Tidyup.DeleteEmpty);
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


            if (Tidyup.EmptyIgnoreExtensions && !Tidyup.EmptyIgnoreWords)
                return; // nope

            foreach (FileInfo fi in files)
            {
                bool okToDelete = Tidyup.EmptyIgnoreExtensions &&
                                 Array.FindIndex(Tidyup.EmptyIgnoreExtensionsArray, x => x == fi.Extension) != -1;

                if (okToDelete)
                    continue; // onto the next file

                // look in the filename
                if (Tidyup.EmptyIgnoreWordsArray.Any(word => fi.Name.Contains(word)))
                    okToDelete = true;

                if (!okToDelete)
                    return;
            }

            if (Tidyup.EmptyMaxSizeCheck)
            {
                // how many MB are we deleting?
                long totalBytes = files.Sum(fi => fi.Length);

                if (totalBytes / (1024 * 1024) > Tidyup.EmptyMaxSizeMb)
                    return; // too much
            }
            DeleteOrRecycleFolder(di);
        }
        public ProcessedEpisode Episode { get; set; }
        public abstract string Name { get; }
        public abstract string ProgressText { get; }
        public abstract long SizeOfWork { get; }
        public abstract string Produces { get; }
        public abstract ListViewItem ScanListViewItem { get; }
        public abstract string TargetFolder { get; }
        public abstract string ScanListViewGroup { get; }
        public abstract int IconNumber { get; }
        public abstract IgnoreItem Ignore { get; }
        public abstract int Compare(ITem o);
        public abstract bool SameAs(ITem o);
        public abstract bool Go(ref bool pause, TVRenameStats stats);
    }
}
