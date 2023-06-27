//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using NLog;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace TVRename;

public static class PathManager
{
    private const string TVDB_FILE_NAME = "TheTVDB.xml";
    private const string TVMAZE_FILE_NAME = "TVmaze.xml";
    private const string TMDB_FILE_NAME = "TMDB.xml";
    private const string SETTINGS_FILE_NAME = "TVRenameSettings.xml";
    private const string UI_LAYOUT_FILE_NAME = "Layout.xml";
    private const string STATISTICS_FILE_NAME = "Statistics.xml";
    private const string STATE_FILE_NAME = "State.xml";

    private static string? UserDefinedBasePath;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static FileInfo[] GetPossibleSettingsHistory() => GetPatternFiles(SETTINGS_FILE_NAME);
    public static FileInfo[] GetPossibleTvdbHistory() => GetPatternFiles(TVDB_FILE_NAME);
    public static FileInfo[] GetPossibleTvMazeHistory() => GetPatternFiles(TVMAZE_FILE_NAME);
    public static FileInfo[] GetPossibleTmdbHistory() => GetPatternFiles(TMDB_FILE_NAME);
    public static FileInfo StateFile => GetFileInfo(STATE_FILE_NAME);
    public static FileInfo StatisticsFile => GetFileInfo(STATISTICS_FILE_NAME);
    // ReSharper disable once InconsistentNaming
    public static FileInfo UILayoutFile => GetFileInfo(UI_LAYOUT_FILE_NAME);
    // ReSharper disable once InconsistentNaming
    public static FileInfo TVDBFile => GetFileInfo(TVDB_FILE_NAME);
    // ReSharper disable once InconsistentNaming
    public static FileInfo TVmazeFile => GetFileInfo(TVMAZE_FILE_NAME);
    public static FileInfo TmdbFile => GetFileInfo(TMDB_FILE_NAME);
    // ReSharper disable once InconsistentNaming
    public static FileInfo TVDocSettingsFile => GetFileInfo(SETTINGS_FILE_NAME);
    public static string CefCachePath()
        => TvRenameFolder("cache");
    public static string CefLogFile()
        => Path.Combine(TvRenameFolder("log"), "cef-debug.log");
    public static string AuditLogFile(string postfix)
        => Path.Combine(TvRenameFolder("audit"), $"Updates{postfix}.json");

    private static FileInfo[] GetPatternFiles(string pattern)
    {
        try
        {
            return new DirectoryInfo(System.IO.Path.GetDirectoryName(TVDocSettingsFile.FullName)).GetFiles(pattern + "*");
        }
        catch (System.IO.IOException ex)
        {
            Logger.Warn(ex, $"Cannot access {pattern} files from directory {TVDocSettingsFile.FullName}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.Warn(ex, $"Cannot access {pattern} files from directory {TVDocSettingsFile.FullName}");
        }

        return Array.Empty<FileInfo>();
    }

    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException">Condition.</exception>
    public static void SetUserDefinedBasePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }
        if (System.IO.File.Exists(path))
        {
            throw new ArgumentException($"File at {path} does not exist", nameof(path));
        }
        path = System.IO.Path.GetFullPath(path); // Get absolute path, in case the given path was a relative one. This will make the Path absolute depending on the Environment.CurrentDirectory.
        // Why are we getting a absolute path here ? Simply because it is not guaranteed that the Environment.CurrentDirectory will not change a some point during runtime and then all bets are off were the Files are going to be saved, which would be fatal to the data integrity.(Saved changes might go to some file nobody even knew about )
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
        UserDefinedBasePath = path;
    }

    private static FileInfo GetFileInfo(string file)
    {
        string path = UserDefinedBasePath.HasValue()
            ? UserDefinedBasePath
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename",
                "TVRename", "2.1");
        Directory.CreateDirectory(path);
        return new FileInfo(System.IO.Path.Combine(path, file));
    }
    
    private static string TvRenameFolder(string folderName)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", folderName);

        try
        {
            Directory.CreateDirectory(path);
        }
        catch (System.IO.DirectoryNotFoundException ex)
        {
            Logger.Warn(ex, $"Could not create {path}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.Warn(ex, $"Could not create {path}");
        }
        catch (System.IO.IOException ex)
        {
            Logger.Warn(ex, $"Could not create {path}");
        }

        return path;
    }
}
