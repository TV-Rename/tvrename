using System;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;

namespace TVRename
{
    sealed class IncorrectFileDates : DownloadIdentifier
    {
        public IncorrectFileDates() => reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            DateTime? newUpdateTime = si.TheSeries().LastAiredDate();
            if (TVSettings.Instance.CorrectFileDates && newUpdateTime.HasValue)
            {
                DirectoryInfo di = new DirectoryInfo(si.AutoAdd_FolderBase);
                if (di.LastWriteTimeUtc != newUpdateTime.Value) return new ItemList() { new ItemDateTouch(di, si, newUpdateTime.Value) };
            }
            return null;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            DateTime? newUpdateTime = si.TheSeries().Seasons[snum].LastAiredDate();

            if (TVSettings.Instance.CorrectFileDates && newUpdateTime.HasValue)
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                if (di.LastWriteTimeUtc != newUpdateTime.Value) return new ItemList() { new ItemDateTouch(di, si, newUpdateTime.Value) };
            }
            return null;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.CorrectFileDates && dbep.FirstAired.HasValue)
            {
                DateTime newUpdateTime = dbep.FirstAired.Value;
                if (filo.LastWriteTimeUtc != newUpdateTime)  return  new ItemList() { new ItemDateTouch(filo,dbep, newUpdateTime) };
            }
            return null;
        }

    }
}
