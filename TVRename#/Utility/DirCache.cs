// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Alphaleonis.Win32.Filesystem;
using System;

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
            this.BuildDirCache(prog, 0, 0, folder, subFolders);
        }

        public static int CountFiles(string folder, bool subFolders)
        {
            int n = 0;
            if (!Directory.Exists(folder))
                return n;
//            if (folder.Length >= 248)
//                return n;
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
            return this.BuildDirCache(prog, initialCount, totalFiles, folder, subFolders);
        }
        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private int BuildDirCache(SetProgressDelegate prog, int count, int totalFiles, string folder, bool subFolders)
        {
            if (!Directory.Exists(folder))
            {
                logger.Error("The search folder \"" + folder + " does not exist.");
                return count;
            }

            try
            {
                /*
                 * if (folder.Length >= 248)
                 
                {
                    logger.Error ("Skipping folder that has a name longer than the Windows permitted 247 characters: " + folder);
                    return count;
                }

    */
                DirectoryInfo di = new DirectoryInfo(folder);
                if (!di.Exists)
                    return count;

                //                DirectorySecurity ^ds = di->GetAccessControl();

                FileInfo[] f2 = di.GetFiles();
                foreach (FileInfo ff in f2)
                {
                    count++;
//                    if ((ff.Name.Length + folder.Length) >= 260)
//                        logger.Error("Skipping file that has a path+name longer than the Windows permitted 259 characters: " + ff.Name + " in " + folder);
//                    else
                        this.Add(new DirCacheEntry(ff));
                    if ((prog != null) && (totalFiles != 0))
                        prog.Invoke(100 * (count) / totalFiles);
                }

                if (subFolders)
                {
                    DirectoryInfo[] dirs = di.GetDirectories();
                    foreach (DirectoryInfo di2 in dirs)
                        count = this.BuildDirCache(prog, count, totalFiles, di2.FullName, subFolders);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                logger.Info(e);
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
            return count;
        }
    }
}
