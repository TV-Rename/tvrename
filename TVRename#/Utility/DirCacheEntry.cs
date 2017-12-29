// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public class DirCacheEntry
    {
        public bool HasUsefulExtensionNotOthersToo;
        public bool HasUsefulExtensionOthersToo;
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

            HasUsefulExtensionNotOthersToo = TVSettings.Instance.UsefulExtension(f.Extension, false);
            HasUsefulExtensionOthersToo = HasUsefulExtensionNotOthersToo | TVSettings.Instance.UsefulExtension(f.Extension, true);
        }
    }
}
