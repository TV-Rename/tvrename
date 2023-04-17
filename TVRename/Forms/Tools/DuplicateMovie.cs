using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;

namespace TVRename.Forms;

public class DuplicateMovie
{
    internal readonly MovieConfiguration Movie;
    internal readonly List<FileInfo> Files;
    public bool IsDoublePart;
    public bool IsSample;
    public bool IsDeleted;

    public DuplicateMovie(MovieConfiguration movie, List<FileInfo> files)
    {
        Movie = movie;
        Files = files;
    }

    public string Name => Movie.ShowName;
    public string Filenames => Files.Select(info => info.FullName).ToCsv();
    public int NumberOfFiles => Files.Count;
}
