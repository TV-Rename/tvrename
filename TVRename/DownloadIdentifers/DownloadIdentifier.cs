using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    abstract class DownloadIdentifier
    {
        public enum DownloadType
        {
            downloadImage,
            downloadMetaData
        }

        public abstract DownloadType GetDownloadType();

        [CanBeNull]
        public  ItemList ProcessShow(ShowItem si)
        {
            return ProcessShow(si,false);
        }

        [CanBeNull]
        public virtual ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            return null;
        }

        [CanBeNull]
        public  ItemList ProcessSeason(ShowItem si, string folder, int snum)
        {
            return ProcessSeason(si,folder,snum,false);
        }

        [CanBeNull]
        public virtual ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            return null;
        }

        [CanBeNull]
        public  ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo)
        {
            return ProcessEpisode(dbep,filo,false);
        }

        [CanBeNull]
        public virtual ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            return null;
        }

        public virtual void NotifyComplete(FileInfo file)
        {
        }

        public virtual void Reset()
        {
        }
    }
}
