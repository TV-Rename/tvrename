// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

// Will cache the file lists of contents of single directories.  Will return the cached
// data, or read cache and return it.

namespace TVRename
{
    public class DirFilesCache
    {
        private readonly Dictionary<string, FileInfo[]> cache = new Dictionary<string, FileInfo[]>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public FileInfo[] Get(string folder)
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
                return null;
            }
            if (!di.Exists)
            {
                cache[folder] = null;
                return null;
            }
            
            try {
                FileInfo[] files = di.GetFiles();
                cache[folder] = files;
                return files;
            } catch (System.IO.IOException) {
               Logger.Error ("IOException occurred trying to access " + folder);
                return null;
            }
        }
    }
}
