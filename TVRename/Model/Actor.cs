//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class Actor
{
    public int ActorId { get; }
    public string? ActorImage { get; }
    public string ActorName { get; }
    public string? ActorRole { get; }
    public int? ActorSortOrder { get; }

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
            ActorSortOrder = null;
        }
    }

    /// <exception cref="ArgumentException">Error Extracting Id for Actor</exception>
    public Actor(XElement r)
    {
        ActorId = r.ExtractInt("Id") ?? throw new ArgumentException("Error Extracting Id for Actor");
        ActorImage = r.ExtractString("Image");
        ActorName = r.ExtractString("Name");
        ActorRole = r.ExtractString("Role");
        ActorSortOrder = r.ExtractInt("SortOrder");
    }

    public Actor(string name)
    {
        ActorName = name;
    }

    internal XElement GenerateXElement()
    {
        return new XElement("Actor",
            new XElement("Id", ActorId),
            XmlHelper.GenerateOptionalXElement("Image", ActorImage),
            XmlHelper.GenerateOptionalXElement("Name", ActorName),
            XmlHelper.GenerateOptionalXElement("Role", ActorRole),
            new XElement("SortOrder", ActorSortOrder)
            );
    }
    public bool AsSelf() => ActorName == ActorRole;
}
