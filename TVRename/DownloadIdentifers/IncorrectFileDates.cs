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
                //Any series before 1980 will get 1980 as the timestamp
                if (newUpdateTime.Value.CompareTo(Helpers.windowsStartDateTime) < 0)
                    newUpdateTime = Helpers.windowsStartDateTime;

                DirectoryInfo di = new DirectoryInfo(si.AutoAdd_FolderBase);
                if ((di.LastWriteTimeUtc != newUpdateTime.Value)&&(!doneFilesAndFolders.Contains(di.FullName)))
                {
                    doneFilesAndFolders.Add(di.FullName);
                    return new ItemList() { new ActionDateTouch(di, si, newUpdateTime.Value) };
                }
            }
            return null;
        }

        public override ItemList ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            DateTime? newUpdateTime = si.GetSeason(snum).LastAiredDate();

            if (TVSettings.Instance.CorrectFileDates && newUpdateTime.HasValue)
            {
                //Any series before 1980 will get 1980 as the timestamp
                if (newUpdateTime.Value.CompareTo(Helpers.windowsStartDateTime) < 0)
                    newUpdateTime = Helpers.windowsStartDateTime;


                DirectoryInfo di = new DirectoryInfo(folder);
                if ((di.LastWriteTimeUtc != newUpdateTime.Value) &&(!doneFilesAndFolders.Contains(di.FullName)))
                {
                    doneFilesAndFolders.Add(di.FullName);
                    return new ItemList() { new ActionDateTouch(di, si, newUpdateTime.Value) };
                }
                
            }
            return null;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.CorrectFileDates && dbep.FirstAired.HasValue)
            {
                DateTime newUpdateTime = dbep.FirstAired.Value;

                //Any series before 1980 will get 1980 as the timestamp
                if (newUpdateTime.CompareTo(Helpers.windowsStartDateTime) < 0)
                    newUpdateTime = Helpers.windowsStartDateTime;

                if ((filo.LastWriteTimeUtc != newUpdateTime) && (!doneFilesAndFolders.Contains(filo.FullName)))
                {
                    doneFilesAndFolders.Add(filo.FullName);
                    return  new ItemList() { new ActionDateTouch(filo,dbep, newUpdateTime) };
                }
            }
            return null;
        }
        public override void reset()
        {
            doneFilesAndFolders = new List<string>();
        }

    }
}
