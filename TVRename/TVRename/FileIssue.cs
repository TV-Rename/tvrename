using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public class FileIssue
{
    // ReSharper disable once NotAccessedField.Global - Used as a property in the Grid
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly string Message;

    public int? SeasonNumber { get; }
    public int? EpisodeNumber { get; }
    public ShowConfiguration Show { get; }
    public FileInfo File { get; }
    public string Showname => Show.ShowName;

    // ReSharper disable once UnusedMember.Global - Used as a property in the Grid
    public string Filename => File.Name;

    public string Directory => File.DirectoryName;

    public FileIssue(ShowConfiguration show, FileInfo file, string message)
    {
        Message = message;
        Show = show;
        File = file;
    }

    public FileIssue(ShowConfiguration show, FileInfo file, string message, int seasonNumber, int episodeNumber) : this(show, file, message)
    {
        SeasonNumber = seasonNumber;
        EpisodeNumber = episodeNumber;
    }

    public FileIssue(ShowConfiguration show, FileInfo file, string message, int seasonNumber) : this(show, file, message)
    {
        SeasonNumber = seasonNumber;
    }
}
