using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal abstract class DownloadIdentifier
    {
        public enum DownloadType
        {
            downloadImage,
            downloadMetaData
        }

        public abstract DownloadType GetDownloadType();

        public  ItemList? ProcessShow(ShowItem si) => ProcessShow(si,false);

        public virtual ItemList? ProcessShow(ShowItem si, bool forceRefresh) => null;

        public  ItemList? ProcessSeason(ShowItem si, string folder, int snum) => ProcessSeason(si,folder,snum,false);

        public virtual ItemList? ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh) => null;

        public  ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file) => ProcessEpisode(episode,file,false);

        public virtual ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh) => null;

        public virtual void NotifyComplete(FileInfo file)
        {
        }

        public virtual void Reset()
        {
        }
    }
}
