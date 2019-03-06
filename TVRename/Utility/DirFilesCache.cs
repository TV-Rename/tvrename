// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

// Will cache the file lists of contents of single directories.  Will return the cached
// data, or read cache and return it.

namespace TVRename
{
    public class DirFilesCache
    {
        private readonly Dictionary<string, FileInfo[]> cache = new Dictionary<string, FileInfo[]>();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public FileInfo[] GetFilesIncludeSubDirs([NotNull] string folder) => Get(folder, true);

        public FileInfo[] GetFiles([NotNull] string folder) => Get(folder, false);

        private FileInfo[] Get([NotNull] string folder,bool includeSubs)
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
                FileInfo[] files = includeSubs ? di.GetFiles("*", System.IO.SearchOption.AllDirectories): di.GetFiles();
                cache[folder] = files;
                return files;
            }
            catch (System.IO.IOException) {
               Logger.Warn("IOException occurred trying to access " + folder);
               return null;
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Warn("IOException occurred trying to access " + folder);
                return null;
            }
        }
    }
}
