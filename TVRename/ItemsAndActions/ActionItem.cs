// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

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
        public abstract string TargetFolder { get; } // return a list of folders for right-click menu
        public abstract string ScanListViewGroup { get; } // which group name for the listview
        public abstract int IconNumber { get; } // which icon number to use in "ilIcons" (UI.cs). -1 for none
        public abstract IgnoreItem Ignore { get; } // what to add to the ignore list / compare against the ignore list
        public ProcessedEpisode Episode { get; protected set; } // associated episode
        public abstract int Compare(Item o); // for sorting items in scan list (ActionItemSorter)
        public abstract bool SameAs(Item o); // are we the same thing as that other one?

        protected static IgnoreItem GenerateIgnore(string file)
        {
            return string.IsNullOrEmpty(file) ? null : new IgnoreItem(file);
        }

        public ListViewItem ScanListViewItem // to add to Scan ListView
        {
            get
            {
                ListViewItem lvi = new ListViewItem {Text = SeriesName};

                lvi.SubItems.Add(SeasonNumber);
                lvi.SubItems.Add(EpisodeNumber);
                lvi.SubItems.Add(AirDate);
                lvi.SubItems.Add(DestinationFolder);
                lvi.SubItems.Add(DestinationFile);
                lvi.SubItems.Add(SourceDetails);

                if (InError)
                    lvi.BackColor = Helpers.WarningColor();

                lvi.Tag = this;

                return lvi;
            }
        }

        protected virtual string SeriesName => Episode?.TheSeries?.Name ?? string.Empty;
        protected virtual string SeasonNumber => Episode?.AppropriateSeasonNumber.ToString() ?? string.Empty;
        protected virtual string EpisodeNumber => Episode?.NumsAsString() ?? string.Empty;
        protected virtual string AirDate => Episode?.GetAirDateDT(true).PrettyPrint() ?? string.Empty;
        protected abstract string DestinationFolder { get; }
        protected abstract string DestinationFile { get; }
        protected virtual string SourceDetails => string.Empty;
        protected virtual bool InError => false;
    }

    public abstract class Action : Item // Something we can do
    {
        public abstract string Name { get; } // Name of this action, e.g. "Copy", "Move", "Download"

        public bool
            Done
        {
            get;
            protected set;
        } // All work has been completed for this item, and can be removed from to-do list.  set to true on completion, even on error.

        public bool Error { get; protected set; } // Error state, after trying to do work?
        public string ErrorText { get; protected set; } // Human-readable error message, for when Error is true
        public abstract string ProgressText { get; } // shortish text to display to user while task is running

        private double percent;

        public double PercentDone // 0.0 to 100.0
        {
            get => Done ? 100.0 : percent;
            protected set => percent = value;
        }

        public abstract long
            SizeOfWork
        {
            get;
        } // for file copy/move, number of bytes in file.  for simple tasks, 1, or something proportional to how slow it is to copy files around.

        public abstract bool
            Go(ref bool pause,
                TVRenameStats stats); // action the action.  do not return until done.  will be run in a dedicated thread.  if pause is set to true, stop working until it goes back to false        

        public abstract string Produces { get; } //What does this action produce? typically a filename
    }

    public abstract class ActionDownload : Action
    {
    }

    public abstract class ActionFileMetaData : Action
    {
    }

    public abstract class ActionFileOperation : Action
    {
        protected TidySettings Tidyup;
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected void DeleteOrRecycleFile(FileInfo file)
        {
            if (file == null) return;
            if (Tidyup.DeleteEmptyIsRecycle)
            {
                Logger.Info($"Recycling {file.FullName}");
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file.FullName,
                    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                Logger.Info($"Deleting {file.FullName}");
                file.Delete(true);
            }
        }

        protected void DeleteOrRecycleFolder(DirectoryInfo di)
        {
            if (di == null) return;
            if (Tidyup.DeleteEmptyIsRecycle)
            {
                Logger.Info($"Recycling {di.FullName}");
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(di.FullName,
                    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                Logger.Info($"Deleting {di.FullName}");
                di.Delete(true, true);
            }
        }

        protected void DoTidyup(DirectoryInfo di)
        {
#if DEBUG
            Debug.Assert(Tidyup != null);
            Debug.Assert(Tidyup.DeleteEmpty);
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
                bool okToDelete = Tidyup.EmptyIgnoreWordsArray.Any(word =>
                    subdi.Name.Contains(word, StringComparison.OrdinalIgnoreCase));

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

            if (Tidyup.EmptyIgnoreExtensions && !Tidyup.EmptyIgnoreWords)
                return; // nope

            foreach (FileInfo fi in files)
            {
                bool okToDelete = Tidyup.EmptyIgnoreExtensions &&
                                  Array.FindIndex(Tidyup.EmptyIgnoreExtensionsArray, x => x == fi.Extension) != -1;

                if (okToDelete)
                    continue; // onto the next file

                // look in the filename
                if (Tidyup.EmptyIgnoreWordsArray.Any(word =>
                    fi.Name.Contains(word, StringComparison.OrdinalIgnoreCase)))
                    okToDelete = true;

                if (!okToDelete)
                    return;
            }

            if (Tidyup.EmptyMaxSizeCheck)
            {
                // how many MB are we deleting?
                long totalBytes = files.Sum(fi => fi.Length);

                if (totalBytes / (1024 * 1024) > Tidyup.EmptyMaxSizeMB)
                    return; // too much
            }

            DeleteOrRecycleFolder(di);
        }
    }

    public abstract class ActionWriteMetadata : ActionDownload
    {
        protected readonly FileInfo Where;
        protected readonly ShowItem SelectedShow; // if for an entire show, rather than specific episode

        protected ActionWriteMetadata(FileInfo where, ShowItem sI)
        {
            Where = where;
            SelectedShow = sI;
        }

        public override string Produces => Where.FullName;

        public override string ProgressText => Where.Name;

        public override long SizeOfWork => 10000;

        public override string TargetFolder => Where == null ? null : Where.DirectoryName;

        public override IgnoreItem Ignore => Where == null ? null : new IgnoreItem(Where.FullName);

        public override string ScanListViewGroup => "lvgActionMeta";

        public override int IconNumber => 7;

        protected override string SeriesName => Episode?.Show?.ShowName ?? SelectedShow.ShowName;
        protected override string DestinationFolder => Where.DirectoryName;
        protected override string DestinationFile => Where.Name;
    }

    public class ItemList : List<Item>
    {
        public void Add(ItemList slil)
        {
            if (slil == null) return;
            foreach (Item sli in slil)
            {
                Add(sli);
            }
        }

        public IEnumerable<Action> Actions( ) => this.OfType<Action>();
    }

    public class ActionQueue
    {
        public readonly List<Action> Actions; // The contents of this queue
        public readonly int ParallelLimit; // Number of tasks in the queue than can be run at once
        public readonly string Name; // Name of this queue
        public int ActionPosition; // Position in the queue list of the next item to process

        public ActionQueue(string name, int parallelLimit)
        {
            Name = name;
            ParallelLimit = parallelLimit;
            Actions = new List<Action>();
            ActionPosition = 0;
        }
    }
}
