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
                if (di.LastWriteTimeUtc != si.TheSeries().LastAiredDate.Value) return new ItemList() { new ItemDateTouch(di, si, si.TheSeries().LastAiredDate.Value) };
            }
            return null;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            if (TVSettings.Instance.CorrectFileDates)
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                if (di.LastWriteTimeUtc != si.TheSeries().Seasons[snum].LastAiredDate.Value) return new ItemList() { new ItemDateTouch(di, si, si.TheSeries().Seasons[snum].LastAiredDate.Value) };
            }


            return null;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.CorrectFileDates)
            {
                if (filo.LastWriteTimeUtc !=  dbep.FirstAired.Value)  return  new ItemList() { new ItemDateTouch(filo,dbep, dbep.FirstAired.Value) };
            }
            return null;
        }

    }
}
