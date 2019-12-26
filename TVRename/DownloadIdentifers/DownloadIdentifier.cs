using JetBrains.Annotations;
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

        [CanBeNull]
        public  ItemList ProcessShow(ShowItem si) => ProcessShow(si,false);

        [CanBeNull]
        public virtual ItemList ProcessShow(ShowItem si, bool forceRefresh) => null;

        [CanBeNull]
        public  ItemList ProcessSeason(ShowItem si, string folder, int snum) => ProcessSeason(si,folder,snum,false);

        [CanBeNull]
        public virtual ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh) => null;

        [CanBeNull]
        public  ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo) => ProcessEpisode(dbep,filo,false);

        [CanBeNull]
        public virtual ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh) => null;

        public virtual void NotifyComplete(FileInfo file)
        {
        }

        public virtual void Reset()
        {
        }
    }
}
