//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public interface iTVSource
{
    void Setup(FileInfo loadFrom, FileInfo cacheFile, bool showConnectionIssues);

    bool Connect(bool showErrorMsgBox);

    void SaveCache();

    bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox);

    void UpdatesDoneOk();

    CachedSeriesInfo? GetSeries(int? id);

    bool HasSeries(int id);

    void ForgetEverything();

    void AddOrUpdateEpisode(Episode episode);

    void LatestUpdateTimeIs(string time);
    TVDoc.ProviderType SourceProvider();
}
