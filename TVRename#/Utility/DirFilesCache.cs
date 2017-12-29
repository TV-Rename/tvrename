using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

// Will cache the file lists of contents of single directories.  Will return the cached
// data, or read cache and return it.

namespace TVRename
{
    public class DirFilesCache
    {
        private readonly Dictionary<String, FileInfo[]> _cache = new Dictionary<string, FileInfo[]>();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public FileInfo[] Get(String folder)
        {
            if (_cache.ContainsKey(folder))
            {
                return _cache[folder];
            }

            DirectoryInfo di;
            try
            {
                di = new DirectoryInfo(folder);
            }
            catch
            {
                _cache[folder] = null;
                return null;
            }
            if (!di.Exists)
            {
                _cache[folder] = null;
                return null;
            }
            
            try {
                FileInfo[] files = di.GetFiles();
                _cache[folder] = files;
                return files;
            } catch (IOException) {
               _logger.Error ("IOException occurred trying to access " + folder);
                return null;
            }
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}

