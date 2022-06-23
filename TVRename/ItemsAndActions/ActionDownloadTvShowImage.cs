using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public class ActionDownloadTvShowImage : ActionDownloadImage
{
    public ActionDownloadTvShowImage(ShowConfiguration si,FileInfo dest, string path)
        : this(si,  dest, path, false)
    {
    }

    public ActionDownloadTvShowImage(ShowConfiguration si, FileInfo dest, string path, bool shrink)
        : base(si, null, dest, path, shrink)
    {
    }
    public override ShowConfiguration? Series => Si as ShowConfiguration;
}