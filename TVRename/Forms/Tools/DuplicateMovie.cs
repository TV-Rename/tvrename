using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename.Forms
{
    public class DuplicateMovie
    {
        internal MovieConfiguration Movie;
        internal List<FileInfo> Files;
        public bool IsDoublePart;
        public bool IsSample;
        public bool IsDeleted;
        public string Name => Movie.ShowName;
        [NotNull]
        public string Filenames => Files.Select(info => info.FullName).ToCsv();
        public int NumberOfFiles => Files.Count;
    }
}
