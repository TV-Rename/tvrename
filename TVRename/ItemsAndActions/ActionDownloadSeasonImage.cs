//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

public class ActionDownloadSeasonImage(ShowConfiguration si, int snum, FileInfo dest, string path, bool shrink) : ActionDownloadImage(si, null, dest, path, shrink)
{
    private readonly int seasonNumber = snum;
    public ActionDownloadSeasonImage(ShowConfiguration si, int snum, FileInfo dest, string path)
        : this(si, snum, dest, path, false)
    {
    }

    public override string SeasonNumber => seasonNumber != 0 ? seasonNumber.ToString() : TVSettings.SpecialsListViewName;
    public override int? SeasonNumberAsInt => seasonNumber;
    public override ShowConfiguration? Series => Si as ShowConfiguration;
}