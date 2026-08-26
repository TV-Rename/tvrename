using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public class ActionDownloadTvShowImage(ShowConfiguration si, FileInfo dest, string path, bool shrink) : ActionDownloadImage(si, null, dest, path, shrink)
{
    public ActionDownloadTvShowImage(ShowConfiguration si, FileInfo dest, string path)
        : this(si, dest, path, false)
    {
    }

    public override ShowConfiguration? Series => Si as ShowConfiguration;
}