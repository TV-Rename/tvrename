using System;
using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;

namespace TVRename
{
    sealed class IncorrectFileDates : DownloadIdentifier
    {
        private List<string> doneFilesAndFolders;
        public IncorrectFileDates() => reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            DateTime? newUpdateTime = si.TheSeries().LastAiredDate();
            if (TVSettings.Instance.CorrectFileDates && newUpdateTime.HasValue)
            {
                //Any series before 1970 will get 1970 as the timestamp
                if (newUpdateTime.Value.CompareTo(Helpers.FromUnixTime(0)) < 0)
                    newUpdateTime = Helpers.FromUnixTime(0);

                DirectoryInfo di = new DirectoryInfo(si.AutoAdd_FolderBase);
                if ((di.LastWriteTimeUtc != newUpdateTime.Value)&&(!this.doneFilesAndFolders.Contains(di.FullName)))
                {
                    this.doneFilesAndFolders.Add(di.FullName);
                    return new ItemList() { new ItemDateTouch(di, si, newUpdateTime.Value) };
                }
            }
            return null;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            DateTime? newUpdateTime = si.TheSeries().Seasons[snum].LastAiredDate();

            if (TVSettings.Instance.CorrectFileDates && newUpdateTime.HasValue)
            {
                //Any series before 1970 will get 1970 as the timestamp
                if (newUpdateTime.Value.CompareTo(Helpers.FromUnixTime(0)) < 0)
                    newUpdateTime = Helpers.FromUnixTime(0);

                DirectoryInfo di = new DirectoryInfo(folder);
                if ((di.LastWriteTimeUtc != newUpdateTime.Value) &&(!this.doneFilesAndFolders.Contains(di.FullName)))
                {
                    this.doneFilesAndFolders.Add(di.FullName);
                    return new ItemList() { new ItemDateTouch(di, si, newUpdateTime.Value) };
                }
                
            }
            return null;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.CorrectFileDates && dbep.FirstAired.HasValue)
            {
                DateTime newUpdateTime = dbep.FirstAired.Value;

                //Any series before 1970 will get 1970 as the timestamp
                if (newUpdateTime.CompareTo(Helpers.FromUnixTime(0)) < 0)
                    newUpdateTime = Helpers.FromUnixTime(0);

                if ((filo.LastWriteTimeUtc != newUpdateTime) && (!this.doneFilesAndFolders.Contains(filo.FullName)))
                {
                    this.doneFilesAndFolders.Add(filo.FullName);
                    return  new ItemList() { new ItemDateTouch(filo,dbep, newUpdateTime) };
                }
            }
            return null;
        }
        public override void reset()
        {
            this.doneFilesAndFolders = new List<string>();
            base.reset();
        }

    }
}
