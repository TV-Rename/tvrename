using Alphaleonis.Win32.Filesystem;

namespace TVRename;

internal abstract class DownloadIdentifier
{
    public enum DownloadType
    {
        downloadImage,
        downloadMetaData
    }

    public abstract DownloadType GetDownloadType();

    public ItemList? ProcessShow(ShowConfiguration si) => ProcessShow(si, false);

    public virtual ItemList? ProcessShow(ShowConfiguration si, bool forceRefresh) => null;

    public ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum) => ProcessSeason(si, folder, snum, false);

    public virtual ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum, bool forceRefresh) => null;

    public ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file) => ProcessEpisode(episode, file, false);

    public virtual ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh) => null;

    public ItemList? ProcessMovie(MovieConfiguration movie, FileInfo file) => ProcessMovie(movie, file, false);

    public virtual ItemList? ProcessMovie(MovieConfiguration movie, FileInfo file, bool forceRefresh) => null;

    public virtual void NotifyComplete(FileInfo file)
    {
    }

    public virtual void Reset()
    {
    }
}
