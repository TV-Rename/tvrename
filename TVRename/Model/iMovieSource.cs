//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//




namespace TVRename;

// ReSharper disable once InconsistentNaming
public interface iMovieSource
{
    Task SetupAsync(FileInfo loadFrom, FileInfo cacheFile, bool showIssues);

    Task<bool> ConnectAsync(bool showErrorMsgBox);

    void SaveCache();

    Task<bool> EnsureUpdatedAsync(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox);

    void UpdatesDoneOk();

    Task<CachedMovieInfo?> GetMovieAsync(PossibleNewMovie show, Locale preferredLocale, bool showErrorMsgBox);

    CachedMovieInfo? GetMovie(int? id);

    bool HasMovie(int id);

    Task ForgetEverythingAsync();

    void LatestUpdateTimeIs(string time);

    TVDoc.ProviderType SourceProvider();
}
