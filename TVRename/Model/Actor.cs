//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Xml;
using System.Xml.Linq;
using System;

namespace TVRename;

public class Actor
{
    public int ActorId { get; }
    public string? ActorImage { get; }
    public string ActorName { get; }
    public string? ActorRole { get; }
    public int ActorSortOrder { get; }

    public Actor(int actorId, string? actorImage, string actorName, string? actorRole, int? actorSortOrder)
    {
        ActorId = actorId;
        ActorImage = actorImage;
        ActorName = actorName;
        ActorRole = actorRole;

        if (actorSortOrder.HasValue)
        {
            ActorSortOrder = actorSortOrder.Value;
        }
        else
        {
            ActorSortOrder = -1;
        }
    }

    /// <exception cref="ArgumentException">Error Extracting Id for Actor</exception>
    public Actor(XElement r)
    {
        ActorId = r.ExtractInt("Id") ?? throw new ArgumentException("Error Extracting Id for Actor");
        ActorImage = r.ExtractString("Image");
        ActorName = r.ExtractString("Name");
        ActorRole = r.ExtractString("Role");
        r.ExtractInt("SeriesId", -1);
        ActorSortOrder = r.ExtractInt("SortOrder", -1);
    }

    public Actor(string name)
    {
        ActorName = name;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Actor");
        writer.WriteElement("Id", ActorId);
        writer.WriteElement("Image", ActorImage, true);
        writer.WriteElement("Name", ActorName, true);
        writer.WriteElement("Role", ActorRole, true);
        //writer.WriteElement("SeriesId", ActorSeriesId, true);
        writer.WriteElement("SortOrder", ActorSortOrder);
        writer.WriteEndElement();
    }

    public bool AsSelf() => ActorName == ActorRole;
}
