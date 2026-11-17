//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//






// Will cache the file lists of contents of single directories.  Will return the cached
// data, or read cache and return it.

namespace TVRename;

public class DirFilesCache
{
    private readonly ConcurrentDictionary<string, FileInfo[]> cache = [];
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public async Task<IEnumerable<FileInfo>> GetFilesIncludeSubDirs(string folder) => await GetAsync(folder, true);

    public async Task<FileInfo[]> GetFilesAsync(string folder) => await GetAsync(folder, false);

    private async Task<FileInfo[]> GetAsync(string folder, bool includeSubs)
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
            cache[folder] = [];
            return [];
        }
        if (!di.Exists)
        {
            cache[folder] = [];
            return [];
        }

        try
        {
            FileInfo[] files = includeSubs ? await di.GetFilesAsync("*", System.IO.SearchOption.AllDirectories) : await di.GetFilesAsync();
            cache[folder] = files;
            return files;
        }
        catch (System.IO.IOException)
        {
            Logger.Warn("IOException occurred trying to access " + folder);
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            Logger.Warn("UnauthorizedAccessException occurred trying to access " + folder);
            return [];
        }
    }
}
