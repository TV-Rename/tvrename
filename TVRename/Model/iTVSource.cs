// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public interface iTVSource
    {
        void Setup(FileInfo loadFrom, FileInfo cacheFile, CommandLineArgs args);
        bool Connect(bool showErrorMsgBox);
        void SaveCache();

        bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox);
        void UpdatesDoneOk();

        CachedSeriesInfo GetSeries(string showName, bool showErrorMsgBox, Locale preferredLocale);
        CachedSeriesInfo GetSeries(int id);
        bool HasSeries(int id);

        void Tidy(IEnumerable<ShowConfiguration> libraryValues);

        void ForgetEverything();
        void ForgetShow(int id);
        void ForgetShow(ISeriesSpecifier ss);
        void UpdateSeries(CachedSeriesInfo si);
        void AddOrUpdateEpisode(Episode episode);
        void AddBanners(int seriesId, IEnumerable<Banner> select);
        void LatestUpdateTimeIs(string time);
    }
}
