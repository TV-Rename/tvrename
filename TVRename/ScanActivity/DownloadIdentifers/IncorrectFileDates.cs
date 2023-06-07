//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal sealed class IncorrectFileDates : DownloadIdentifier
{
    private List<string> doneFilesAndFolders = null!;

    // ReSharper disable once NotNullMemberIsNotInitialized
    public IncorrectFileDates() => Reset();

    public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

    public override ItemList? ProcessShow(ShowConfiguration si, bool forceRefresh)
    {
        DateTime? updateTime = si.LastAiredDate;
        if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue)
        {
            return null;
        }

        DateTime newUpdateTime = FileHelper.GetMinWindowsTime(updateTime.Value);

        DirectoryInfo di = new(si.AutoAddFolderBase);

        if (!ShouldUpdate(di, newUpdateTime))
        {
            return null;
        }

        doneFilesAndFolders.Add(di.FullName);
        return new ItemList { new ActionDateTouchMedia(di, si, newUpdateTime) };
    }

    public override ItemList? ProcessMovie(MovieConfiguration movie, FileInfo file, bool forceRefresh)
    {
        DateTime? updateTime = movie.CachedMovie?.FirstAired;
        if (!TVSettings.Instance.CorrectFileDates || !updateTime.HasValue)
        {
            return base.ProcessMovie(movie, file, forceRefresh);
        }

        DateTime newUpdateTime = FileHelper.GetMinWindowsTime(updateTime.Value);
        DirectoryInfo di = file.Directory;
        ItemList returnItems = new();

        if (ShouldUpdate(di, newUpdateTime))
        {
            doneFilesAndFolders.Add(di.FullName);
            returnItems.Add(new ActionDateTouchMedia(di, movie, newUpdateTime));
        }

        if (ShouldUpdate(file, newUpdateTime))
        {
            doneFilesAndFolders.Add(file.FullName);
            returnItems.Add(new ActionDateTouchMovie(file, movie, newUpdateTime));
        }

        return returnItems;
    }

    private bool ShouldUpdate(FileSystemInfo di, DateTime newUpdateTime)
    {
        try
        {
            return di.LastWriteTimeUtc != newUpdateTime && !doneFilesAndFolders.Contains(di.FullName);
        }
        catch (Exception)
        {
            return true;
        }
    }

    public override ItemList? ProcessSeason(ShowConfiguration si, string folder, int snum, bool forceRefresh)
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

        DateTime newUpdateTime = FileHelper.GetMinWindowsTime(updateTime.Value);
        DirectoryInfo di = new(folder);

        if (!ShouldUpdate(di, newUpdateTime))
        {
            return null;
        }

        doneFilesAndFolders.Add(di.FullName);
        return new ItemList { new ActionDateTouchSeason(di, processedSeason, newUpdateTime) };
    }

    public override ItemList? ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
    {
        if (!TVSettings.Instance.CorrectFileDates || !episode.FirstAired.HasValue)
        {
            return null;
        }

        DateTime newUpdateTime = FileHelper.GetMinWindowsTime(episode.FirstAired.Value);

        if (!ShouldUpdate(file, newUpdateTime))
        {
            return null;
        }

        doneFilesAndFolders.Add(file.FullName);
        return new ItemList { new ActionDateTouchEpisode(file, episode, newUpdateTime) };
    }

    public override void Reset() => doneFilesAndFolders = new List<string>();
}
