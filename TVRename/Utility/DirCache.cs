// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

// Recursively reads and caches files and folders, and info about them, as this is way faster
// than repeatedly hitting the filesystem.

namespace TVRename
{
    public class DirCache : System.Collections.Generic.List<DirCacheEntry>
    {
        public DirCache()
        {
        }

        public DirCache(SetProgressDelegate prog, string folder, bool subFolders)
        {
            BuildDirCache(prog, 0, 0, folder, subFolders);
        }

        public static int CountFiles(string folder, bool subFolders)
        {
            int n = 0;
            if (!Directory.Exists(folder))
                return n;

            DirectoryInfo di = new DirectoryInfo(folder);
            try
            {
                n = di.GetFiles().Length;
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

            if (!subFolders) return n;

            DirectoryInfo[] dirs =new DirectoryInfo[0];
            try
            {
                dirs = di.GetDirectories();
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

            n += dirs.Sum(di2 => CountFiles(di2.FullName, true));

            return n;
        }

        public void AddFolder(SetProgressDelegate prog, int initialCount, int totalFiles, string folder,
            bool subFolders)
        {
            BuildDirCache(prog, initialCount, totalFiles, folder, subFolders);
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private int BuildDirCache(SetProgressDelegate prog, int count, int totalFiles, string folder, bool subFolders)
        {
            if (!Directory.Exists(folder))
            {
                Logger.Warn("The search folder \"" + folder + " does not exist.");
                return count;
            }

            try
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                if (!di.Exists)
                    return count;
                FileInfo[] f2 = new FileInfo[0];
                try
                {
                    f2= di.GetFiles();
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

                foreach (FileInfo ff in f2)
                {
                    count++;
                    Add(new DirCacheEntry(ff));
                    if ((prog != null) && (totalFiles != 0))
                        prog.Invoke(100 * (count) / totalFiles, string.Empty);
                }

                if (subFolders)
                {
                    DirectoryInfo[] dirs = new DirectoryInfo[0];
                    try
                    {
                        dirs = di.GetDirectories();
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

                    foreach (DirectoryInfo di2 in dirs)
                        count += BuildDirCache(prog, count, totalFiles, di2.FullName, true);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
            return count;
        }
    }
}
