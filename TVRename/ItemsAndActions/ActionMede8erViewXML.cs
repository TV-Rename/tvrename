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
using System.Xml;

// ReSharper disable once InconsistentNaming
public class ActionMede8erViewXML : ActionWriteMetadata, IEquatable<ActionMede8erViewXML>
{
    private readonly int snum;

    public ActionMede8erViewXML(FileInfo nfo, ShowConfiguration si) : base(nfo, si)
    {
        snum = -1;
    }

    public ActionMede8erViewXML(FileInfo nfo, ShowConfiguration si, int snum) : base(nfo, si)
    {
        this.snum = snum;
    }

    #region Action Members
    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        XmlWriterSettings settings = new()
        {
            Indent = true,
            NewLineOnAttributes = true
        };

        try
        {
            using XmlWriter writer = XmlWriter.Create(Where.FullName, settings);
            writer.WriteStartElement("FolderTag");
            writer.WriteElement("ViewMode", "Movie");
            writer.WriteElement("ViewType", "Video");
            writer.WriteEndElement();
        }
        catch (Exception e)
        {
            return new ActionOutcome(e);
        }
        return ActionOutcome.Success();
    }

    #endregion Action Members

    #region Comparison Methods

    public override bool SameAs(Item o)
    {
        return o is ActionMede8erViewXML xml && xml.Where == Where;
    }

    public override int CompareTo(Item? o)
    {
        if (o is not ActionMede8erViewXML nfo)
        {
            return -1;
        }

        return string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);
    }

    public bool Equals(ActionMede8erViewXML? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && snum == other.snum;
    }

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

        return Equals((ActionMede8erViewXML)obj);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), snum);

    #endregion

    public override string SeriesName => SelectedShow?.ShowName ?? Movie!.ShowName;
    public override string SeasonNumber => snum > 0 ? snum.ToString() : string.Empty;
    public override int? SeasonNumberAsInt => snum;
    public override string EpisodeString => string.Empty;
    public override string AirDateString => string.Empty;
    public override string Name => "Write Mede8er View Data";
}
