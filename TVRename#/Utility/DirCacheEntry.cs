// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
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
