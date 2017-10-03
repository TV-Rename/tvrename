// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.IO;
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
            this.TheFile = f;
            this.SimplifiedFullName = Helpers.SimplifyName(f.FullName);
            this.LowerName = f.Name.ToLower();
            this.Length = f.Length;

            if (TVSettings.Instance == null)
                return;

            this.HasUsefulExtension_NotOthersToo = TVSettings.Instance.UsefulExtension(f.Extension, false);
            this.HasUsefulExtension_OthersToo = this.HasUsefulExtension_NotOthersToo | TVSettings.Instance.UsefulExtension(f.Extension, true);
        }
    }
}