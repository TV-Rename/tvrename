//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using Alphaleonis.Win32.Filesystem;

namespace TVRename;

public abstract class ActionWriteMetadata : ActionDownload, IEquatable<ActionWriteMetadata>
{
    protected readonly FileInfo Where;
    protected readonly ShowConfiguration? SelectedShow; // if for an entire show, rather than specific episode

    protected ActionWriteMetadata(FileInfo where, ShowConfiguration sI)
    {
        Where = where;
        SelectedShow = sI;
    }

    protected ActionWriteMetadata(FileInfo where, MovieConfiguration mc)
    {
        Where = where;
        Movie = mc;
    }

    public override QueueName Queue() => QueueName.writeMetadata;
    public override string Produces => Where.FullName;
    public override string ProgressText => Where.Name;
    public override long SizeOfWork => 10000;
    public override string TargetFolder => Where.DirectoryName;
    public override IgnoreItem Ignore => new(Where.FullName);
    public override string ScanListViewGroup => "lvgActionMeta";
    public override int IconNumber => 7;
    public override string SeriesName => Series?.ShowName ?? Movie!.ShowName;
    public override string DestinationFolder => Where.DirectoryName;
    public override string DestinationFile => Where.Name;
    public override ShowConfiguration? Series => Episode?.Show ?? SelectedShow;

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ActionWriteMetadata)obj);
    }

    public bool Equals(ActionWriteMetadata? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && Where.Equals(other.Where) && Equals(SelectedShow, other.SelectedShow);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Where, SelectedShow);
}
