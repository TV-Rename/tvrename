//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

public abstract class ScanMovieActivity(TVDoc doc) : ScanMediaActivity(doc)
{
    protected abstract Task CheckAsync(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings);

    public async Task CheckIfActiveAsync(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
    {
        if (Active())
        {
            await CheckAsync(si, dfc, settings);
            LogActionListSummary();
        }
    }
}
