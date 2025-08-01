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

// Will cache the file lists of contents of single directories.  Will return the cached
// data, or read cache and return it.

namespace TVRename;

public class DirFilesCache
{
    private readonly Dictionary<string, FileInfo[]> cache = [];
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public IEnumerable<FileInfo> GetFilesIncludeSubDirs(string folder) => Get(folder, true);

    public FileInfo[] GetFiles(string folder) => Get(folder, false);

    private FileInfo[] Get(string folder, bool includeSubs)
    {
        if (cache.TryGetValue(folder, out FileInfo[]? value))
        {
            return value;
        }

        DirectoryInfo di;
        try
        {
            di = new DirectoryInfo(folder);
        }
        catch
        {
            cache[folder] = Array.Empty<FileInfo>();
            return Array.Empty<FileInfo>();
        }
        if (!di.Exists)
        {
            cache[folder] = Array.Empty<FileInfo>();
            return Array.Empty<FileInfo>();
        }

        try
        {
            FileInfo[] files = includeSubs ? di.GetFiles("*", System.IO.SearchOption.AllDirectories) : di.GetFiles();
            cache[folder] = files;
            return files;
        }
        catch (System.IO.IOException)
        {
            Logger.Warn("IOException occurred trying to access " + folder);
            return Array.Empty<FileInfo>();
        }
        catch (UnauthorizedAccessException)
        {
            Logger.Warn("UnauthorizedAccessException occurred trying to access " + folder);
            return Array.Empty<FileInfo>();
        }
    }
}
