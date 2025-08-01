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

public class UpdateMediaFileSubtitle : UpdateMediaFileMetaData
{
    public override string Name => "Update Media File Metadata (subtitle)";
    public override int CompareTo(Item? o)
    {
        UpdateMediaFileSubtitle? nfo = o as UpdateMediaFileSubtitle;

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
        string desc = tfile.Tag.Subtitle;

        if (desc != Value)
        {
            tfile.Tag.Subtitle = Value;
            tfile.Save();
        }

        return ActionOutcome.Success();
    }
    public UpdateMediaFileSubtitle(FileInfo where, MovieConfiguration mc, string value) : base(where, mc, value)
    {
    }
    public UpdateMediaFileSubtitle(FileInfo where, ProcessedEpisode episode, string value) : base(where, episode, value)
    {
    }
}
