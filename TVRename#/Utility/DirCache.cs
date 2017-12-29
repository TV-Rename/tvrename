// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using NLog;

// Recursively reads and caches files and folders, and info about them, as this is way faster
// than repeatedly hitting the filesystem.

namespace TVRename
{
    public class DirCache : List<DirCacheEntry>
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
            if (folder.Length >= 248)
                return n;
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                n = di.GetFiles().Length;

                if (subFolders)
                {
                    DirectoryInfo[] dirs = di.GetDirectories();
                    foreach (DirectoryInfo di2 in dirs)
                        n += CountFiles(di2.FullName, subFolders);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            return n;
        }

        public int AddFolder(SetProgressDelegate prog, int initialCount, int totalFiles, string folder, bool subFolders)
        {
            return BuildDirCache(prog, initialCount, totalFiles, folder, subFolders);
        }
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        private int BuildDirCache(SetProgressDelegate prog, int count, int totalFiles, string folder, bool subFolders)
        {
            if (!Directory.Exists(folder))
            {
                Logger.Error("The search folder \"" + folder + " does not exist.\n");
                return count;
            }

            try
            {
                if (folder.Length >= 248)
                {
                    Logger.Error ("Skipping folder that has a name longer than the Windows permitted 247 characters: " + folder);
                    return count;
                }

                DirectoryInfo di = new DirectoryInfo(folder);
                if (!di.Exists)
                    return count;

                //                DirectorySecurity ^ds = di->GetAccessControl();

                FileInfo[] f2 = di.GetFiles();
                foreach (FileInfo ff in f2)
                {
                    count++;
                    if ((ff.Name.Length + folder.Length) >= 260)
                        Logger.Error("Skipping file that has a path+name longer than the Windows permitted 259 characters: " + ff.Name + " in " + folder);
                    else
                        Add(new DirCacheEntry(ff));
                    if ((prog != null) && (totalFiles != 0))
                        prog.Invoke(100 * (count) / totalFiles);
                }

                if (subFolders)
                {
                    DirectoryInfo[] dirs = di.GetDirectories();
                    foreach (DirectoryInfo di2 in dirs)
                        count = BuildDirCache(prog, count, totalFiles, di2.FullName, subFolders);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Info(e);
            }
            catch
            {
            }
            return count;
        }
    }
}
