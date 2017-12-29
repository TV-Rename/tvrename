using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    abstract class DownloadIdentifier
    {
        public enum DownloadType
        {
            DownloadImage,
            DownloadMetaData
        }

        public abstract DownloadType GetDownloadType();

        public  ItemList ProcessShow(ShowItem si)
        {
            return ProcessShow(si,false);
        }

        public virtual ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            return null;
        }

        public  ItemList ProcessSeason(ShowItem si, string folder, int snum)
        {
            return ProcessSeason(si,folder,snum,false);
        }
        public virtual ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            return null;
        }


        public  ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo)
        {
            return ProcessEpisode(dbep,filo,false);
        }
        public virtual ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            return null;
        }

        public virtual void NotifyComplete(FileInfo file)
        {
        }

        public virtual void Reset() { }
    }
}
