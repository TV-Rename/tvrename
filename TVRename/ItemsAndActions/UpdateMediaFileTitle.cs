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

public class UpdateMediaFileTitle : UpdateMediaFileMetaData
{
    public override string Name => "Update Media File Metadata (title)";
    public override int CompareTo(Item? o)
    {
        UpdateMediaFileTitle? nfo = o as UpdateMediaFileTitle;

        if (nfo?.Where is null)
        {
            return -1;
        }

        int compareTo = string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);

        return compareTo != 0 ? compareTo : string.CompareOrdinal(Value, nfo.Value);
    }
    public override bool SameAs(Item o) => CompareTo(o)==0;

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        TagLib.File tfile = TagLib.File.Create(Where.FullName);
        string desc = tfile.Tag.Title;

        if (desc != Value)
        {
            tfile.Tag.Title = Value;
            tfile.Save();
        }

        return ActionOutcome.Success();
    }
    public UpdateMediaFileTitle(FileInfo where, MovieConfiguration mc, string value) : base(where, mc, value)
    {
    }

    public UpdateMediaFileTitle(FileInfo where, ProcessedEpisode episode, string value) : base(where, episode, value)
    {
    }
}
