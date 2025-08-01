//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Threading;

namespace TVRename;
using Alphaleonis.Win32.Filesystem;
using System;

public abstract class ActionFileMetaData : Action
{
}

public class UpdateMediaFileDescription : UpdateMediaFileMetaData
{
    public override string Name => "Update Media File Metadata (description)";

    public override int CompareTo(Item? o)
    {
        UpdateMediaFileDescription? nfo = o as UpdateMediaFileDescription;

        if (nfo?.Where is null)
        {
            return -1;
        }

        int compareTo = string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);

        return compareTo != 0 ? compareTo : string.CompareOrdinal(Value, nfo.Value);
    }
    public override bool SameAs(Item o) => CompareTo(o) == 0;

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        TagLib.File tfile = TagLib.File.Create(Where.FullName);
        string desc = tfile.Tag.Description;

            if (desc != Value)
        {
            tfile.Tag.Description = Value;
            tfile.Save();
        }

        return ActionOutcome.Success();
    }
    public UpdateMediaFileDescription(FileInfo where, MovieConfiguration mc, string value) : base(where, mc,value)
    {
    }

    public UpdateMediaFileDescription(FileInfo where, ProcessedEpisode episode, string value) : base(where, episode,  value)
    {
    }
}

public class UpdateMediaFileComment : UpdateMediaFileMetaData
{
    public override string Name => "Update Media File Metadata (comment)";
    public override int CompareTo(Item? o)
    {
        UpdateMediaFileComment? nfo = o as UpdateMediaFileComment;

        if (nfo?.Where is null)
        {
            return -1;
        }

        int compareTo = string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);

        return compareTo != 0 ? compareTo : string.CompareOrdinal(Value, nfo.Value);
    }

    public override bool SameAs(Item o) => CompareTo(o) == 0;

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        TagLib.File tfile = TagLib.File.Create(Where.FullName);
        string desc = tfile.Tag.Comment;

        if (desc != Value)
        {
            tfile.Tag.Comment = Value;
            tfile.Save();
        }

        return ActionOutcome.Success();
    }
    public UpdateMediaFileComment(FileInfo where, MovieConfiguration mc, string value) : base(where, mc, value)
    {
    }
    public UpdateMediaFileComment(FileInfo where, ProcessedEpisode episode, string value) : base(where, episode, value)
    {
    }
}

public class UpdateMediaFileYear : UpdateMediaFileMetaData
{
    private readonly uint intValue;
    public override string Name => "Update Media File Metadata (year)";
    public override int CompareTo(Item? o)
    {
        UpdateMediaFileYear? nfo = o as UpdateMediaFileYear;

        if (nfo?.Where is null)
        {
            return -1;
        }

        int compareTo = string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);

        return compareTo != 0 ? compareTo : string.CompareOrdinal(Value, nfo.Value);
    }

    public override bool SameAs(Item o) => CompareTo(o) == 0;

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        TagLib.File tfile = TagLib.File.Create(Where.FullName);
        uint desc = tfile.Tag.Year;

        if (desc != intValue)
        {
            tfile.Tag.Year = intValue;
            tfile.Save();
        }

        return ActionOutcome.Success();
    }
    public UpdateMediaFileYear(FileInfo where, MovieConfiguration mc, uint value) : base(where, mc, value.ToString())
    {
        intValue = value;
    }
    public UpdateMediaFileYear(FileInfo where, ProcessedEpisode episode, uint value) : base(where, episode, value.ToString())
    {
        intValue = value;
    }
}
