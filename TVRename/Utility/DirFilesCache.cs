// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.IO;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

// Will cache the file lists of contents of single directories.  Will return the cached
// data, or read cache and return it.

namespace TVRename
{
    public class DirFilesCache
    {
        private readonly Dictionary<string, FileInfo[]> cache = new Dictionary<string, FileInfo[]>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public FileInfo[] GetFilesIncludeSubDirs(string folder) => Get(folder, true);

        public FileInfo[] GetFiles(string folder) => Get(folder, false);

        private FileInfo[] Get(string folder,bool includeSubs)
        {
            if (cache.ContainsKey(folder))
            {
                return cache[folder];
            }

            DirectoryInfo di;
            try
            {
                di = new DirectoryInfo(folder);
            }
            catch
            {
                cache[folder] = null;
                return new FileInfo[0];
            }
            if (!di.Exists)
            {
                cache[folder] = null;
                return new FileInfo[0];
            }
            
            try {
                FileInfo[] files = includeSubs ? di.GetFiles("*",SearchOption.AllDirectories): di.GetFiles();
                cache[folder] = files;
                return files;
            } catch (IOException) {
               Logger.Warn("IOException occurred trying to access " + folder);
               return new FileInfo[0];
            }
        }
    }
}
