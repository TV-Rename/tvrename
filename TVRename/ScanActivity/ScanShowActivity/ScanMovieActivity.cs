namespace TVRename;

public abstract class ScanMovieActivity(TVDoc doc) : ScanMediaActivity(doc)
{
    protected abstract void Check(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings);

    public void CheckIfActive(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
    {
        if (Active())
        {
            Check(si, dfc, settings);
            LogActionListSummary();
        }
    }
}
