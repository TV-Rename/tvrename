// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System;
using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;

namespace TVRename
{
    internal sealed class IncorrectFileDates : DownloadIdentifier
    {
        private List<string> doneFilesAndFolders;
        // ReSharper disable once NotNullMemberIsNotInitialized
        public IncorrectFileDates() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList? ProcessShow(ShowItem si, bool forceRefresh)
        {
            DateTime? updateTime = si.LastAiredDate;
            if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue)
            {
                return null;
            }

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(updateTime.Value);

            DirectoryInfo di = new DirectoryInfo(si.AutoAddFolderBase);
            try
            {
                if (di.LastWriteTimeUtc != newUpdateTime && !doneFilesAndFolders.Contains(di.FullName))
                {
                    doneFilesAndFolders.Add(di.FullName);
                    return new ItemList {new ActionDateTouchShow(di, si, newUpdateTime)};
                }
            }
            catch (Exception)
            {
                doneFilesAndFolders.Add(di.FullName);
                return new ItemList { new ActionDateTouchShow(di, si, newUpdateTime) };
            }
            return null;
        }

        public override ItemList? ProcessSeason(ShowItem si, string folder, int snum, bool forceRefresh)
        {
            ProcessedSeason processedSeason = si.GetSeason(snum) ?? throw new ArgumentException("ProcessSeason called for invlaid season");
            DateTime? updateTime = processedSeason.LastAiredDate();

            if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue)
            {
                return null;
            }

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(updateTime.Value);

            DirectoryInfo di = new DirectoryInfo(folder);
            try
            {
                if (di.LastWriteTimeUtc != newUpdateTime && !doneFilesAndFolders.Contains(di.FullName))
                {
                    doneFilesAndFolders.Add(di.FullName);
                    return new ItemList {new ActionDateTouchSeason(di, processedSeason, newUpdateTime)};
                }
            }
            catch (Exception)
            {
                doneFilesAndFolders.Add(di.FullName);
                return new ItemList { new ActionDateTouchSeason(di, processedSeason, newUpdateTime) };
            }

            return null;
        }

        public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
        {
            if (!TVSettings.Instance.CorrectFileDates || !episode.FirstAired.HasValue)
            {
                return null;
            }

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(episode.FirstAired.Value);

            try
            {
                if (file.LastWriteTimeUtc != newUpdateTime && !doneFilesAndFolders.Contains(file.FullName))
                {
                    doneFilesAndFolders.Add(file.FullName);
                    return new ItemList { new ActionDateTouchEpisode(file, episode, newUpdateTime) };
                }
            }
            catch (Exception)
            {
                doneFilesAndFolders.Add(file.FullName);
                return new ItemList { new ActionDateTouchEpisode(file, episode, newUpdateTime) };
            }
            return null;
        }
        public override void Reset() => doneFilesAndFolders = new List<string>();
    }
}
