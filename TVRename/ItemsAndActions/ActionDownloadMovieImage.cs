using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public class ActionDownloadMovieImage(MovieConfiguration si, FileInfo dest, string path, bool shrink) : ActionDownloadImage(si, null, dest, path, shrink)
{
    public ActionDownloadMovieImage(MovieConfiguration si, FileInfo dest, string path)
        : this(si, dest, path, false)
    {
    }
}