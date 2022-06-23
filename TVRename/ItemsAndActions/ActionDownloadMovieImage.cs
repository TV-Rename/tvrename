using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public class ActionDownloadMovieImage : ActionDownloadImage
{
    public ActionDownloadMovieImage(MovieConfiguration si, FileInfo dest, string path)
        : this(si, dest, path, false)
    {
    }

    public ActionDownloadMovieImage(MovieConfiguration si, FileInfo dest, string path, bool shrink)
        : base(si, null, dest, path, shrink)
    {
    }
}