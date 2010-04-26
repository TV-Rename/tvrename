// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    using System.Windows.Forms;

    public interface Item
    {
        int Compare(Item o); // for sorting items in scan list (ActionItemSorter)
        bool SameAs(Item o); // are we the same thing as that other one?
    }

    public class ItemList : System.Collections.Generic.List<Item>
    {
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
        bool Go(TVSettings settings, ref bool pause); // action the action.  do not return until done.  will be run in a dedicated thread.  if pause is set to true, stop working until it goes back to false        
    }

    public interface ScanListItem // something shown in the list on the Scan tab (not always an Action)
    {
        ListViewItem ScanListViewItem { get; } // to add to Scan ListView
        string TargetFolder { get; } // return a list of folders for right-click menu
        int ScanListViewGroup { get; } // which group number for the listview
        int IconNumber { get; } // which icon number to use in "ilIcons" (UI.cs). -1 for none
        IgnoreItem Ignore { get; } // what to add to the ignore list / compare against the ignore list
        ProcessedEpisode Episode { get; } // associated episode
    }

    public class ScanListItemList : System.Collections.Generic.List<ScanListItem>
    {
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