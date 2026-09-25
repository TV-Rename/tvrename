
using NLog;
using System.Threading.Tasks;

namespace TVRename;

internal abstract class DownloadIdentifier
{
    protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    public enum DownloadType
    {
        downloadImage,
        downloadMetaData
    }

    public abstract DownloadType GetDownloadType();

    public async Task<ItemList?> ProcessShowAsync(ShowConfiguration si) => await ProcessShowAsync(si, false);

    public async virtual Task<ItemList?> ProcessShowAsync(ShowConfiguration si, bool forceRefresh) => null;

    public ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum) => ProcessSeason(si, folder, snum, false);

    public virtual ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum, bool forceRefresh) => null;

    public ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file) => ProcessEpisode(episode, file, false);

    public virtual ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh) => null;

    public async Task<ItemList?> ProcessMovieAsync(MovieConfiguration movie, FileInfo file) => await ProcessMovieAsync(movie, file, false);

    public virtual async Task<ItemList?> ProcessMovieAsync(MovieConfiguration movie, FileInfo file, bool forceRefresh) => null;

    public virtual void NotifyComplete(FileInfo file)
    {
    }

    public virtual void Reset()
    {
    }
}
