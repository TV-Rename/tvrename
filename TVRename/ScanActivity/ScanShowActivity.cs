// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    public abstract class ScanShowActivity
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        protected readonly TVDoc mDoc;

        protected ScanShowActivity(TVDoc doc) => mDoc = doc;

        protected abstract void Check(ShowItem si, DirFilesCache dfc, TVDoc.ScanSettings settings);

        protected abstract bool Active();

        public void CheckIfActive(ShowItem si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (Active())
            {
                Check(si,dfc,settings);
            }
        }
    }
}
