//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename.SAB;

public class QueueSlotsSlot : IDownloadInformation
{
    public string? Status { get; set; }
    public string? Mb { get; set; }
    public string? Filename { get; set; }
    public string? SizeLeft { get; set; }
    public string? TimeLeft { get; set; }

    string? IDownloadInformation.FileIdentifier => Filename;
    string? IDownloadInformation.Destination => Filename;

    string IDownloadInformation.RemainingText
    {
        get
        {
            string txt = $"{Status}, {SizeLeft}% Complete";
            if (Status == "Downloading")
            {
                txt += $", {TimeLeft} left";
            }
            return txt;
        }
    }
}
