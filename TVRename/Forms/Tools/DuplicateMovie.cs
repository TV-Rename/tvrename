using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;

namespace TVRename.Forms;

public class DuplicateMovie(MovieConfiguration movie, List<FileInfo> files)
{
    internal readonly MovieConfiguration Movie = movie;
    internal readonly List<FileInfo> Files = files;
    public bool IsDoublePart;
    public bool IsSample;
    public bool IsDeleted;

    public string Name => Movie.ShowName;
    public string Filenames => Files.Select(info => info.FullName).ToCsv();
    public int NumberOfFiles => Files.Count;
}
