using System;
using System.Collections.Generic;
using System.IO;

// Will cache the file lists of contents of single directories.  Will return the cached
// data, or read cache and return it.

namespace TVRename
{
    public class DirFilesCache
    {
        private Dictionary<String, FileInfo[]> Cache = new Dictionary<string, FileInfo[]>();

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

            FileInfo[] files = di.GetFiles();
            Cache[folder] = files;
            return files;
        }

        public void Clear()
        {
            Cache.Clear();
        }
    }
}

