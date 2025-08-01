//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;
using Alphaleonis.Win32.Filesystem;
using System;

public abstract class UpdateMediaFileMetaData : ActionFileMetaData
{
    protected readonly FileInfo Where;
    protected readonly string Value;

    protected UpdateMediaFileMetaData(FileInfo where, MovieConfiguration mc, string value)
    {
        Where = where;
        this.Value = value;
        Movie = mc;
    }

    protected UpdateMediaFileMetaData(FileInfo where,ProcessedEpisode episode, string value)
    {
        Where = where;
        Episode = episode;
        this.Value = value;
    }

    public override QueueName Queue() => QueueName.slowFileOperation;
    public override string Produces => Where.FullName;
    public override string ProgressText => Where.Name;
    public override long SizeOfWork => 100;
    public override string TargetFolder => Where.DirectoryName;
    public override IgnoreItem Ignore => new(Where.FullName);
    public override string ScanListViewGroup => "lvgUpdateFileDates";
    public override int IconNumber => 7;
    public override string SourceDetails => Value;
    public override string SeriesName => Series?.ShowName ?? Movie!.ShowName;
    public override string DestinationFolder => Where.DirectoryName;
    public override string DestinationFile => Where.Name;
    public override ShowConfiguration? Series => Episode?.Show;
    public override int Order => 9;
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

        return Equals((UpdateMediaFileMetaData)obj);
    }

    public bool Equals(UpdateMediaFileMetaData? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && Where.Equals(other.Where) && Value.Equals(other.Value);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Where, Value);
}
