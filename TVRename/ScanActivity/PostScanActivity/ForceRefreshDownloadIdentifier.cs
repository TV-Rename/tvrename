using Alphaleonis.Win32.Filesystem;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename;

internal class ForceRefreshDownloadIdentifier(DownloadIdentifier action, TVDoc doc, string name) : PostScanActivity(doc)
{
    private readonly DownloadIdentifiersController cx = new(action);
    private readonly string name = name;

    public override string ActivityName() => name;

    protected override bool Active() => true;

    protected override async Task DoCheckAsync(CancellationToken token)
    {
        int totalRecords = MDoc.TvLibrary.Count + MDoc.FilmLibrary.Count;
        ThreadSafeCounter currentRecord = new();
        foreach (ShowConfiguration si in MDoc.TvLibrary.GetSortedShows())
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (!si.AutoAddFolderBase.HasValue() || !(await si.AllExistngFolderLocationsAsync()).IsAny())
            {
                continue;
            }

            MDoc.TheActionList.AddNullableRange(await cx.ForceUpdateShowAsync(DownloadIdentifier.DownloadType.downloadMetaData, si));

            UpdateStatus(currentRecord.Increment(), totalRecords, "Updating TV Shows", si.Name ?? string.Empty);
        }

        foreach (MovieConfiguration si in MDoc.FilmLibrary.GetSortedMovies())
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (!(await si.AllExistngFolderLocationsAsync()).IsAny())
            {
                continue;
            }
            foreach (FileInfo file in (await si.MovieFilesAsync()))
            {
                MDoc.TheActionList.AddNullableRange(await cx.ForceUpdateMovieAsync(DownloadIdentifier.DownloadType.downloadMetaData, si, file));
            }

            UpdateStatus(currentRecord.Increment(), totalRecords, "Updating Movies", si.Name ?? string.Empty);
        }
    }
}
