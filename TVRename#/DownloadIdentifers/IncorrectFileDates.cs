using System;
using System.Collections.Generic;
using System.Text;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;

namespace TVRename
{
    class IncorrectFileDates : DownloadIdentifier
    {
        public IncorrectFileDates() => reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            if (TVSettings.Instance.CorrectFileDates)
            {
                DirectoryInfo di = new DirectoryInfo(si.AutoAdd_FolderBase);
                DateTime newUpdateTime = si.TheSeries().LastAiredDate.Value;
                if (di.LastWriteTimeUtc != newUpdateTime) return new ItemList() { new ItemDateTouch(di, si, newUpdateTime) };
            }
            return null;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.CorrectFileDates)
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                DateTime newUpdateTime = si.TheSeries().Seasons[snum].LastAiredDate.Value;
                if (di.LastWriteTimeUtc != newUpdateTime) return new ItemList() { new ItemDateTouch(di, si, newUpdateTime) };
            }
            return null;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.CorrectFileDates)
            {
                DateTime newUpdateTime = dbep.FirstAired.Value;
                if (filo.LastWriteTimeUtc != newUpdateTime)  return  new ItemList() { new ItemDateTouch(filo,dbep, newUpdateTime) };
            }
            return null;
        }

    }
}
