// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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

        bool EnsureUpdated(int code, bool bannersToo, bool useCustomLangCode, string langCode);
        bool GetUpdates(bool showErrorMsgBox);
        void UpdatesDoneOk();

        SeriesInfo GetSeries(string showName, bool showErrorMsgBox);
        SeriesInfo GetSeries(int id);
        bool HasSeries(int id);

        void Tidy(ICollection<ShowItem> libraryValues);

        void ForgetEverything();
        void ForgetShow(int id);
        void ForgetShow(int id, bool makePlaceholder,bool useCustomLanguage,string langCode);
    }
}
