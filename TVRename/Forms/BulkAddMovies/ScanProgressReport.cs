//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class ScanProgressReport : ProgressReport
{
    public string LatestAction { get; set; } = string.Empty;
}

public class ProgressReport
{
    public int ProgressPercentage { get; set; }
    public string UpdateText { get; set; } = string.Empty;

}
