// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
    using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;



    public abstract class Item // something shown in the list on the Scan tab (not always an Action)
    {
        public abstract ListViewItem ScanListViewItem { get; } // to add to Scan ListView
        public abstract string TargetFolder { get; } // return a list of folders for right-click menu
        public abstract string ScanListViewGroup { get; } // which group name for the listview
        public abstract int IconNumber { get; } // which icon number to use in "ilIcons" (UI.cs). -1 for none
        public abstract IgnoreItem Ignore { get; } // what to add to the ignore list / compare against the ignore list
        public ProcessedEpisode Episode { get; protected set; } // associated episode
        public abstract int Compare(Item o); // for sorting items in scan list (ActionItemSorter)
        public abstract bool SameAs(Item o); // are we the same thing as that other one?
    }

    public abstract class ItemInProgress : Item
    {
        public string DesiredLocationNoExt;

        public override string ScanListViewGroup => "lvgDownloading";


        public override IgnoreItem Ignore
        {
            get
            {
                if (string.IsNullOrEmpty(this.DesiredLocationNoExt))
                    return null;
                return new IgnoreItem(this.DesiredLocationNoExt);
            }
        }

    }

    public abstract class Action :Item // Something we can do
    {
        public abstract string Name { get; } // Name of this action, e.g. "Copy", "Move", "Download"
        public bool Done { get; protected set; } // All work has been completed for this item, and can be removed from to-do list.  set to true on completion, even on error.
        public bool Error { get; protected set; } // Error state, after trying to do work?
        public string ErrorText { get; protected set; } // Human-readable error message, for when Error is true
        public abstract string ProgressText { get; } // shortish text to display to user while task is running

        protected double Percent;
        public double PercentDone  // 0.0 to 100.0
        {
            get => this.Done ? 100.0 : this.Percent;
            set => this.Percent = value;
        }
        
        public abstract long SizeOfWork { get; } // for file copy/move, number of bytes in file.  for simple tasks, 1, or something proportional to how slow it is to copy files around.
        public abstract bool Go( ref bool pause, TVRenameStats stats); // action the action.  do not return until done.  will be run in a dedicated thread.  if pause is set to true, stop working until it goes back to false        
        public abstract string Produces { get; } //What does this action produce? typically a filename
    }



    public abstract class ActionDownload : Action {
    }

    public abstract class ActionFileMetaData : Action
    {
    }
    public abstract class ActionFileOperation : Action
    {
        
        protected TidySettings Tidyup;
        protected readonly static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected void DeleteOrRecycleFile(FileInfo file)
        {
            if (file == null) return;
            if (this.Tidyup.DeleteEmptyIsRecycle)
            {
                logger.Info($"Recycling {file.FullName}");
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file.FullName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                logger.Info($"Deleting {file.FullName}");
                file.Delete(true);
            }
        }
        protected void DeleteOrRecycleFolder(DirectoryInfo di)
        {
            if (di == null) return;
            if (this.Tidyup.DeleteEmptyIsRecycle)
            {
                logger.Info($"Recycling {di.FullName}");
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(di.FullName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                logger.Info($"Deleting {di.FullName}");
                di.Delete(true, true);
            }
        }
        protected void DoTidyup(DirectoryInfo di)
        {
#if DEBUG
            Debug.Assert(this.Tidyup != null);
            Debug.Assert(this.Tidyup.DeleteEmpty);
#else
            if (this.Tidyup == null || !this.Tidyup.DeleteEmpty)
                return;
#endif
            // See if we should now delete the folder we just moved that file from.
            if (di == null)
                return;

            //if there are sub-directories then we shouldn't remove this one
            DirectoryInfo[] directories = di.GetDirectories();
            foreach (DirectoryInfo subdi in directories)
            {
                bool okToDelete = this.Tidyup.EmptyIgnoreWordsArray.Any(word => subdi.Name.Contains(word,StringComparison.OrdinalIgnoreCase));

                if (!okToDelete)
                    return;
            }
            //we know that each subfolder is OK to delete


            //if the directory is the root download folder do not delete
            if (TVSettings.Instance.DownloadFolders.Contains(di.FullName))
                return;

            // Do not delete any monitor folders either
            if (TVSettings.Instance.LibraryFolders.Contains(di.FullName))
                return;


            FileInfo[] files = di.GetFiles();
            if (files.Length == 0)
            {
                // its empty, so just delete it
                DeleteOrRecycleFolder(di);
                return;
            }


            if (this.Tidyup.EmptyIgnoreExtensions && !this.Tidyup.EmptyIgnoreWords)
                return; // nope

            foreach (FileInfo fi in files)
            {
                bool okToDelete = this.Tidyup.EmptyIgnoreExtensions &&
                                 Array.FindIndex(this.Tidyup.EmptyIgnoreExtensionsArray, x => x == fi.Extension) != -1;

                if (okToDelete)
                    continue; // onto the next file

                // look in the filename
                if (this.Tidyup.EmptyIgnoreWordsArray.Any(word => fi.Name.Contains(word,StringComparison.OrdinalIgnoreCase)))
                    okToDelete = true;

                if (!okToDelete)
                    return;
            }

            if (this.Tidyup.EmptyMaxSizeCheck)
            {
                // how many MB are we deleting?
                long totalBytes = files.Sum(fi => fi.Length);

                if (totalBytes / (1024 * 1024) > this.Tidyup.EmptyMaxSizeMB)
                    return; // too much
            }
            DeleteOrRecycleFolder(di);
        }
    }

    public abstract class ActionWriteMetadata : ActionDownload
    {
        public FileInfo Where;

        public override string Produces => this.Where.FullName;

        public override string ProgressText => this.Where.Name;

        public override long SizeOfWork => 10000;


        public override string TargetFolder
        {
            get
            {
                if (this.Where == null)
                    return null;
                return this.Where.DirectoryName;
            }
        }

        public override IgnoreItem Ignore
        {
            get
            {
                if (this.Where == null)
                    return null;
                return new IgnoreItem(this.Where.FullName);
            }
        }

        public override string ScanListViewGroup => "lvgActionMeta";

        public override int IconNumber => 7;
    }

    public class ItemList : System.Collections.Generic.List<Item>
    {
        public void Add(ItemList slil)
        {
            if (slil == null) return;
            foreach (Item sli in slil)
            {
                Add(sli);
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
            this.Name = name;
            this.ParallelLimit = parallelLimit;
            this.Actions = new System.Collections.Generic.List<Action>();
            this.ActionPosition = 0;
        }
    }

}
