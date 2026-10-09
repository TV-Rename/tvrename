//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

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
