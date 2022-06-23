//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

using System;

public abstract class ActionDateTouch : ActionFileMetaData
{
    protected readonly DateTime UpdateTime;

    protected ActionDateTouch(DateTime time)
    {
        UpdateTime = time;
    }

    #region Action Members

    public override string Name => "Update Timestamp";
    public override long SizeOfWork => 100;

    #endregion Action Members

    public override string ScanListViewGroup => "lvgUpdateFileDates";
    public override int IconNumber => 7;

    public override string AirDateString =>
        UpdateTime.CompareTo(DateTime.MaxValue) != 0 ? UpdateTime.ToShortDateString() : string.Empty;

    public override DateTime? AirDate => UpdateTime;
}
