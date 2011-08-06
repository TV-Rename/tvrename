// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Xml;

// Per-season sets of rules for manipulating episodes from thetvdb into multi-episode files,
// removing, adding, swapping them around, etc.

namespace TVRename.db_access.document
{
    public enum RuleAction
    {
        kRemove,
        kSwap,
        kMerge,
        kInsert,
        kIgnoreEp,
        kRename,
        kSplit,
        kCollapse
    }

    public class ShowRule
    {
        public RuleAction DoWhatNow { get; set; }
        public int First { get; set; }
        public int Second { get; set; }
        public string UserSuppliedText { get; set; }

        public ShowRule()
        {
            this.SetToDefaults();
        }

        public ShowRule(ShowRule O)
        {
            this.DoWhatNow = O.DoWhatNow;
            this.First = O.First;
            this.Second = O.Second;
            this.UserSuppliedText = O.UserSuppliedText;
        }

        public void SetToDefaults()
        {
            this.DoWhatNow = RuleAction.kIgnoreEp;
            this.First = this.Second = -1;
            this.UserSuppliedText = "";
        }
    }
}
