//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    public abstract class ScanShowActivity : ScanMediaActivity
    {
        protected abstract void Check(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings);

        public void CheckIfActive(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (Active())
            {
                Check(si, dfc, settings);
                LogActionListSummary();
            }
        }

        protected ScanShowActivity(TVDoc doc) : base(doc)
        {
        }
    }
}
