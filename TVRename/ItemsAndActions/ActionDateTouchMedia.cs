//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class ActionDateTouchMedia : ActionDateTouchDirectory
{
    private readonly MediaConfiguration show; // if for an entire show, rather than specific episode

    public ActionDateTouchMedia(DirectoryInfo dir, MediaConfiguration si, DateTime date) : base(dir, date)
    {
        show = si;
        if (si is MovieConfiguration m)
        {
            Movie = m;
        }
    }

    public override string SeriesName => show.ShowName;
    public override string SeasonNumber => string.Empty;
    public override int? SeasonNumberAsInt => null;

    public override ShowConfiguration? Series => show as ShowConfiguration;
}
