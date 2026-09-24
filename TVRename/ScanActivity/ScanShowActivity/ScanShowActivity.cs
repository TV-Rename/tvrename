//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Threading.Tasks;

namespace TVRename;

public abstract class ScanShowActivity(TVDoc doc) : ScanMediaActivity(doc)
{
    protected abstract Task CheckAsync(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings);

    public async Task CheckIfActiveAsync(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
    {
        if (Active())
        {
            await CheckAsync(si, dfc, settings);
            LogActionListSummary();
        }
    }
}
