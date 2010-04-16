// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Windows.Forms;

// Derivatives of "ActionItem" are the actions to do, as a result of doing a scan on the "Scan" tab.

namespace TVRename
{
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

        public abstract string TargetFolder();

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

        // Search predicate 
        public static bool DoneOK(ActionItem i)
        {
            return i.Done && !i.HasError;
        }
    }
}