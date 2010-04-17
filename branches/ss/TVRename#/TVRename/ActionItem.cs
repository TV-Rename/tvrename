// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System.Windows.Forms;

namespace TVRename
{
    public interface Item
    {
        int Compare(Item o); // for sorting items in scan list
        bool SameAs(Item o); // are we the same thing as that other one?
    }
    
    public class ItemList : System.Collections.Generic.List<Item>
    {
    }

    public interface Action // Something we can do
    {
        bool Done { get; } // All work has been completed for this item, and can be removed from to-do list
        bool Error { get; } // Error state, after trying to do work?
        string ErrorText { get; } // Human-readable error message, for when Error is true
        string ProgressText { get; } // shortish text to display to user while task is running
        int PercentDone { get; } // 0 to 100
        long SizeOfWork { get; } // for file copy/move, number of bytes in file.  for simple tasks, 1.
        bool Go(TVSettings settings); // action the action.  do not return until done.  will be run in a dedicated thread
        bool Stop(); // abort any work going on in Go, and clean up.  return of false means not stopped, so use Thread.Abort
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
}