//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//




// Recursively reads and caches files and folders, and info about them, as this is way faster
// than repeatedly hitting the filesystem.

namespace TVRename;

public class DirCache : List<DirCacheEntry>
{
    public DirCache()
    {
    }

    public static int CountFiles(string folder, bool subFolders)
    {
        int n = 0;
        if (!Directory.Exists(folder))
        {
            return n;
        }

        DirectoryInfo[] dirs = [];
        try
        {
            DirectoryInfo di = new(folder);
            n = di.GetFiles()?.Length ?? 0;
            if (!subFolders)
            {
                return n;
            }

            dirs = di.GetDirectories()!;
        }
        catch (NotSupportedException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (System.IO.DirectoryNotFoundException)
        {
        }
        catch (System.IO.IOException)
        {
        }

        n += dirs.Sum(di2 => CountFiles(di2.FullName!, true));

        return n;
    }

    public async Task AddFolderAsync(SetProgressDelegate? prog, int initialCount, int totalFiles, string folder,
        bool subFolders)
    {
        await BuildDirCacheAsync(prog, initialCount, totalFiles, folder, subFolders);
    }

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private async Task BuildDirCacheAsync(SetProgressDelegate? prog, int count, int totalFiles, string folder, bool subFolders)
    {
        if (!Directory.Exists(folder))
        {
            Logger.Warn($"The search folder {folder.InDoubleQuotes()} does not exist.");
            return;
        }

        try
        {
            DirectoryInfo di = new(folder);
            if (!di.Exists)
            {
                return;
            }

            foreach (FileInfo ff in await GetFilesAsync(di))
            {
                Add(new DirCacheEntry(ff));
                if (totalFiles != 0)
                {
                    prog?.Invoke(100 * count / totalFiles, string.Empty, string.Empty);
                }
            }

            if (subFolders)
            {
                foreach (DirectoryInfo di2 in GetDirectoryInfo(di))
                {
                    await BuildDirCacheAsync(prog, count, totalFiles, di2.FullName, true);
                }
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
        }
    }

    private static DirectoryInfo[] GetDirectoryInfo(DirectoryInfo di)
    {
        try
        {
            return di.GetDirectories();
        }
        catch (NotSupportedException e)
        {
            Logger.Info(e);
        }
        catch (UnauthorizedAccessException e)
        {
            Logger.Info(e);
        }
        catch (System.IO.DirectoryNotFoundException e)
        {
            Logger.Info(e);
        }
        catch (System.IO.IOException e)
        {
            Logger.Info(e);
        }

        return [];
    }

    private static async Task<IEnumerable<FileInfo>> GetFilesAsync(DirectoryInfo di)
    {
        FileInfo[] f2 = [];
        try
        {
            f2 = await di.GetFilesAsync();
        }
        catch (NotSupportedException e)
        {
            Logger.Info(e);
        }
        catch (UnauthorizedAccessException e)
        {
            Logger.Info(e);
        }
        catch (System.IO.DirectoryNotFoundException e)
        {
            Logger.Info(e);
        }
        catch (System.IO.IOException e)
        {
            Logger.Info(e);
        }

        return f2;
    }
}
