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
    }
    
    public class ItemList : System.Collections.Generic.List<Item>
    {
    }

    public interface Action  // Something we can do
    {
        bool Done { get; } // All work has been completed for this item, and can be removed from to-do list
        bool Error { get; } // Error state, after trying to do work?
        string ErrorText { get; } // Human-readable error message, for when Error is true
        string ProgressText { get; } // shortish text to display to user while task is running
        int PercentDone { get; } // 0 to 100
        long SizeOfWork { get; } // for file copy/move, number of bytes in file.  for simple tasks, 1.

        bool SameAs(Action o); // are we doing the same task as that other one?

        bool Go(TVSettings settings); // action the action.  do not return until done.  will be run in a dedicated thread
        bool Stop(); // abort any work going on in Go, and clean up.  return of false means not stopped, so use Thread.Abort
    }

    public interface EpisodeRelated // is related to some particular episode
    {
        ProcessedEpisode Episode { get; } // associated episode
        IgnoreItem Ignore { get; } // what to add to the ignore list / compare against the ignore list
    }

    public interface ScanList // something shown in the list on the Scan tab (not always an Action)
    {
        ListViewItem ScanListViewItem { get; } // to add to Scan ListView
        string TargetFolder { get; } // return a list of folders for right-click menu
        int ScanListViewGroup { get; } // which group number for the listview
        int IconNumber { get; } // which icon number to use in "ilIcons" (UI.cs). -1 for none
    }

    /*
          public enum ActionType
    {
        kMissing,
        kCopyMoveRename,
        kRSS,
        kDownload,
        kNFO,
        kuTorrenting
    }

    public abstract class ActionItem
    {
        public bool Done;
        public string ErrorText;
        public bool HasError;
        public ProcessedEpisode PE; // can be null if not applicable or known

        public ActionType Type;

        protected ActionItem(ActionType t, ProcessedEpisode pe)
        {
            this.PE = pe;
            this.Done = false;
            this.Type = t;
            this.HasError = false;
            this.ErrorText = "";
        }

        public abstract IgnoreItem GetIgnore();

        public abstract ListViewItem GetLVI(ListView lv);

        public abstract string FilenameForProgress();

        public abstract string TargetFolder(); // used for right-click on item, to open target folder

        // nullptr if none, otherwise folder "of interest" for this item
        // e.g. where file is missing from, or downloader is downloading to

        public virtual bool Action(TVDoc doc)
        {
            // default is to do nothing
            return true; // all ok
        }

        public abstract bool SameAs(ActionItem o);

        public virtual int IconNumber()
        {
            return -1;
        }
    }
    */
}