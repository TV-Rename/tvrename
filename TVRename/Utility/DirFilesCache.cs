using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

// Will cache the file lists of contents of single directories.  Will return the cached
// data, or read cache and return it.

namespace TVRename
{
    public class DirFilesCache
    {
        private Dictionary<String, FileInfo[]> Cache = new Dictionary<string, FileInfo[]>();
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public FileInfo[] Get(String folder)
        {
            if (Cache.ContainsKey(folder))
            {
                return Cache[folder];
            }

            DirectoryInfo di;
            try
            {
                di = new DirectoryInfo(folder);
            }
            catch
            {
                Cache[folder] = null;
                return null;
            }
            if (!di.Exists)
            {
                Cache[folder] = null;
                return null;
            }
            
            try {
                FileInfo[] files = di.GetFiles();
                Cache[folder] = files;
                return files;
            } catch (System.IO.IOException) {
               logger.Error ("IOException occurred trying to access " + folder);
                return null;
            }
        }

        public void Clear()
        {
            Cache.Clear();
        }
    }
}

