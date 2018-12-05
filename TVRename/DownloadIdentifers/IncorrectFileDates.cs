using System;
using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;

namespace TVRename
{
    internal sealed class IncorrectFileDates : DownloadIdentifier
    {
        private List<string> doneFilesAndFolders;
        public IncorrectFileDates() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            DateTime? updateTime = si.TheSeries().LastAiredDate;
            if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue) return null;

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(updateTime.Value);

            DirectoryInfo di = new DirectoryInfo(si.AutoAddFolderBase);
            if ((di.LastWriteTimeUtc != newUpdateTime)&&(!doneFilesAndFolders.Contains(di.FullName)))
            {
                doneFilesAndFolders.Add(di.FullName);
                return new ItemList() { new ActionDateTouch(di, si, newUpdateTime) };
            }
            return null;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            DateTime? updateTime = si.GetSeason(snum)?.LastAiredDate();

            if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue) return null;

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(updateTime.Value);

            DirectoryInfo di = new DirectoryInfo(folder);
            if ((di.LastWriteTimeUtc != newUpdateTime) &&(!doneFilesAndFolders.Contains(di.FullName)))
            {
                doneFilesAndFolders.Add(di.FullName);
                return new ItemList() { new ActionDateTouch(di, si, newUpdateTime) };
            }
            return null;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (!TVSettings.Instance.CorrectFileDates || !dbep.FirstAired.HasValue) return null;

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(dbep.FirstAired.Value);

            if ((filo.LastWriteTimeUtc != newUpdateTime) && (!doneFilesAndFolders.Contains(filo.FullName)))
            {
                doneFilesAndFolders.Add(filo.FullName);
                return  new ItemList() { new ActionDateTouch(filo,dbep, newUpdateTime) };
            }
            return null;
        }
        public override void Reset() => doneFilesAndFolders = new List<string>();
    }
}
