using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public interface iMovieSource
    {
        void Setup(FileInfo loadFrom, FileInfo cacheFile, bool showIssues);

        bool Connect(bool showErrorMsgBox);

        void SaveCache();

        bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox);

        void UpdatesDoneOk();

        CachedMovieInfo? GetMovie(PossibleNewMovie show, Locale preferredLocale, bool showErrorMsgBox);

        CachedMovieInfo? GetMovie(int? id);

        bool HasMovie(int id);

        void ForgetEverything();

        void LatestUpdateTimeIs(string time);

        TVDoc.ProviderType SourceProvider();
    }
}
