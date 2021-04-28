// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Threading;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public interface iTVSource
    {
        void Setup(FileInfo loadFrom, FileInfo cacheFile, CommandLineArgs args);
        bool Connect(bool showErrorMsgBox);
        void SaveCache();

        bool EnsureUpdated(SeriesSpecifier s, bool bannersToo, bool showErrorMsgBox);
        void UpdatesDoneOk();

        CachedSeriesInfo GetSeries(string showName, bool showErrorMsgBox);
        CachedSeriesInfo GetSeries(int id);
        bool HasSeries(int id);

        void Tidy(IEnumerable<ShowConfiguration> libraryValues);

        void ForgetEverything();
        void ForgetShow(int id);
        void ForgetShow(int tvdb,int tvmaze,int tmdb, bool makePlaceholder,bool useCustomLanguage,string langCode);
        void UpdateSeries(CachedSeriesInfo si);
        void AddOrUpdateEpisode(Episode episode);
        void AddBanners(int seriesId, IEnumerable<Banner> select);
        void LatestUpdateTimeIs(string time);
        Language PreferredLanguage { get; }
        Language? GetLanguageFromCode(string? customLanguageCode);
    }
}
