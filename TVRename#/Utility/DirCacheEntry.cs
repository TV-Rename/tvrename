using System;
using System.IO;

namespace TVRename
{
    public class DirCacheEntry
    {
        public FileInfo TheFile;
        public string SimplifiedFullName;
        public string LowerName;
        public bool HasUsefulExtension_NotOthersToo;
        public bool HasUsefulExtension_OthersToo;
        public Int64 Length;

        public DirCacheEntry(FileInfo f, TVSettings theSettings)
        {
            TheFile = f;
            SimplifiedFullName = Helpers.SimplifyName(f.FullName);
            LowerName = f.Name.ToLower();
            Length = f.Length;
            
            if (theSettings == null) 
                return;

            HasUsefulExtension_NotOthersToo = theSettings.UsefulExtension(f.Extension, false);
            HasUsefulExtension_OthersToo = HasUsefulExtension_NotOthersToo | theSettings.UsefulExtension(f.Extension, true);
        }
    }
}