//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class ServerRelease(string version, Release.VersionType type, string downloadUrl, string releaseNotesText,
    string releaseNotesUrl, bool isBeta, DateTime releaseDate) : Release(version, type)
{
    public string DownloadUrl { get; } = downloadUrl;
    public string ReleaseNotesText { get; } = releaseNotesText;
    public string ReleaseNotesUrl { get; } = releaseNotesUrl;
    public bool IsBeta { get; } = isBeta;
    public DateTime ReleaseDate { get; } = releaseDate;

    public string LogMessage()
    {
        StringBuilder sb = new();
        sb.AppendLine("************************");
        sb.AppendLine("* New Update Available *");
        sb.AppendLine("************************");
        sb.AppendLine($"A new version is available: {this} since {ReleaseDate}");
        sb.AppendLine($"please download from {DownloadUrl}");
        sb.AppendLine($"full notes available from {ReleaseNotesUrl}");
        sb.AppendLine(ReleaseNotesText);
        return sb.ToString();
    }
}
