//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal sealed class IncorrectFileDates : DownloadIdentifier
    {
        private List<string> doneFilesAndFolders;

        // ReSharper disable once NotNullMemberIsNotInitialized
        public IncorrectFileDates() => Reset();

        public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

        public override ItemList? ProcessShow([NotNull] ShowConfiguration si, bool forceRefresh)
        {
            DateTime? updateTime = si.LastAiredDate;
            if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue)
            {
                return null;
            }

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(updateTime.Value);

            DirectoryInfo di = new(si.AutoAddFolderBase);
            try
            {
                if (di.LastWriteTimeUtc != newUpdateTime && !doneFilesAndFolders.Contains(di.FullName))
                {
                    doneFilesAndFolders.Add(di.FullName);
                    return new ItemList { new ActionDateTouchMedia(di, si, newUpdateTime) };
                }
            }
            catch (Exception)
            {
                doneFilesAndFolders.Add(di.FullName);
                return new ItemList { new ActionDateTouchMedia(di, si, newUpdateTime) };
            }
            return null;
        }

        public override ItemList? ProcessMovie([NotNull] MovieConfiguration movie, FileInfo file, bool forceRefresh)
        {
            DateTime? updateTime = movie.CachedMovie?.FirstAired;
            if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue)
            {
                return base.ProcessMovie(movie, file, forceRefresh);
            }

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(updateTime.Value);

            DirectoryInfo di = file.Directory;
            ItemList returnItems = new();

            try
            {
                if (di.LastWriteTimeUtc != newUpdateTime && !doneFilesAndFolders.Contains(di.FullName))
                {
                    doneFilesAndFolders.Add(di.FullName);
                    returnItems.Add(new ActionDateTouchMedia(di, movie, newUpdateTime));
                }
            }
            catch (Exception)
            {
                doneFilesAndFolders.Add(di.FullName);
                returnItems.Add(new ActionDateTouchMedia(di, movie, newUpdateTime));
            }

            try
            {
                if (file.LastWriteTimeUtc != newUpdateTime && !doneFilesAndFolders.Contains(file.FullName))
                {
                    doneFilesAndFolders.Add(file.FullName);
                    returnItems.Add(new ActionDateTouchMovie(file, movie, newUpdateTime));
                }
            }
            catch (Exception)
            {
                doneFilesAndFolders.Add(file.FullName);
                returnItems.Add(new ActionDateTouchMovie(file, movie, newUpdateTime));
            }

            return returnItems;
        }

        public override ItemList? ProcessSeason([NotNull] ShowConfiguration si, string folder, int snum, bool forceRefresh)
        {
            ProcessedSeason processedSeason = si.GetSeason(snum) ?? throw new ArgumentException($"ProcessSeason called for {si.ShowName} invalid season ({snum}), show has ({si.AppropriateSeasons().Keys.Select(i => i.ToString()).ToCsv()})");
            DateTime? updateTime = processedSeason.LastAiredDate();

            if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue)
            {
                return null;
            }

            if (si.AutoAddType == ShowConfiguration.AutomaticFolderType.baseOnly && folder.Equals(si.AutoAddFolderBase))
            {
                //We do not need to look at the season folder - there is no such thing as it'll be covered by the show folder
                return null;
            }

            DateTime newUpdateTime = Helpers.GetMinWindowsTime(updateTime.Value);

            DirectoryInfo di = new(folder);
            try
            {
                if (di.LastWriteTimeUtc != newUpdateTime && !doneFilesAndFolders.Contains(di.FullName))
                {
                    doneFilesAndFolders.Add(di.FullName);
                    return new ItemList { new ActionDateTouchSeason(di, processedSeason, newUpdateTime) };
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
