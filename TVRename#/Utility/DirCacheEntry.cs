// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using Alphaleonis.Win32.Filesystem;
using FileSystemInfo = Alphaleonis.Win32.Filesystem.FileSystemInfo;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


namespace TVRename
{
    public class DirCacheEntry
    {
        public bool HasUsefulExtension_NotOthersToo;
        public bool HasUsefulExtension_OthersToo;
        public Int64 Length;
        public string LowerName;
        public string SimplifiedFullName;
        public FileInfo TheFile;

        public DirCacheEntry(FileInfo f)
        {
            TheFile = f;
            SimplifiedFullName = Helpers.SimplifyName(f.FullName);
            LowerName = f.Name.ToLower();
            Length = f.Length;

            if (TVSettings.Instance == null)
                return;

            HasUsefulExtension_NotOthersToo = TVSettings.Instance.UsefulExtension(f.Extension, false);
            HasUsefulExtension_OthersToo = HasUsefulExtension_NotOthersToo | TVSettings.Instance.UsefulExtension(f.Extension, true);
        }
    }
}
