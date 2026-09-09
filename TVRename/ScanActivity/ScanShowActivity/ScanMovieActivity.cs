using System.Threading.Tasks;

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
