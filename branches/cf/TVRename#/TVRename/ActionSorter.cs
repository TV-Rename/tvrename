// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    public class ActionSorter : System.Collections.Generic.IComparer<ActionItem>
    {
        #region IComparer<ActionItem> Members

        public virtual int Compare(ActionItem x, ActionItem y)
        {
            if (x.Type == y.Type)
            {
                if (x.Type == ActionType.kCopyMoveRename)
                {
                    ActionCopyMoveRename xx = (ActionCopyMoveRename) (x);
                    ActionCopyMoveRename yy = (ActionCopyMoveRename) (y);

                    string s1 = xx.From.FullName + (xx.From.Directory.Root.FullName != xx.To.Directory.Root.FullName ? "0" : "1");
                    string s2 = yy.From.FullName + (yy.From.Directory.Root.FullName != yy.To.Directory.Root.FullName ? "0" : "1");

                    return s1.CompareTo(s2);
                }
                if (x.Type == ActionType.kDownload)
                {
                    ActionDownload xx = (ActionDownload) (x);
                    ActionDownload yy = (ActionDownload) (y);

                    return xx.Destination.FullName.CompareTo(yy.Destination.FullName);
                }
                if (x.Type == ActionType.kRSS)
                {
                    ActionRSS xx = (ActionRSS) (x);
                    ActionRSS yy = (ActionRSS) (y);

                    return xx.RSS.URL.CompareTo(yy.RSS.URL);
                }
                if (x.Type == ActionType.kMissing)
                {
                    ActionMissing xx = (ActionMissing) (x);
                    ActionMissing yy = (ActionMissing) (y);

                    return (xx.TheFileNoExt + xx.PE.Name).CompareTo(yy.TheFileNoExt + yy.PE.Name);
                }
                if (x.Type == ActionType.kNFO)
                {
                    ActionNFO xx = (ActionNFO) (x);
                    ActionNFO yy = (ActionNFO) (y);

                    if (xx.PE == null)
                        return 1;
                    if (yy.PE == null)
                        return -1;
                    return (xx.Where.FullName + xx.PE.Name).CompareTo(yy.Where.FullName + yy.PE.Name);
                }
                if (x.Type == ActionType.kuTorrenting)
                {
                    ActionuTorrenting xx = (ActionuTorrenting) (x);
                    ActionuTorrenting yy = (ActionuTorrenting) (y);

                    if (xx.PE == null)
                        return 1;
                    if (yy.PE == null)
                        return -1;
                    return (xx.DesiredLocationNoExt).CompareTo(yy.DesiredLocationNoExt);
                }
                System.Diagnostics.Debug.Fail("Unknown type in ActionItem::Compare"); // uhoh
                return 1;
            }
            // different types
            return ((int) x.Type - (int) y.Type);
        }

        #endregion
    }
}