using System;
using System.Text;
using JetBrains.Annotations;

public class ServerRelease : Release
{
    public string DownloadUrl { get; }
    public string ReleaseNotesText { get; }
    public string ReleaseNotesUrl { get; }
    public bool IsBeta { get; }
    public DateTime ReleaseDate { get; }

    public ServerRelease([NotNull] string version, VersionType type, string downloadUrl, string releaseNotesText, string releaseNotesUrl, bool isBeta, DateTime releaseDate) : base(version, type)
    {
        DownloadUrl = downloadUrl;
        ReleaseNotesText = releaseNotesText;
        ReleaseNotesUrl = releaseNotesUrl;
        IsBeta = isBeta;
        ReleaseDate = releaseDate;
    }

    public string LogMessage()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("************************");
        sb.AppendLine("* New Update Available *");
        sb.AppendLine("************************");
        sb.AppendLine($"A new verion is available: {ToString()} since {ReleaseDate}");
        sb.AppendLine($"please download from {DownloadUrl}");
        sb.AppendLine($"full notes available from {ReleaseNotesUrl}");
        sb.AppendLine(ReleaseNotesText);
        return sb.ToString();
    }
}
