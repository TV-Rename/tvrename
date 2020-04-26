// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public class Actor
    {
        public int ActorId { get; }
        public string ActorImage { get; }
        public string ActorName { get; }
        public string ActorRole { get; }
        public int ActorSeriesId { get; }
        public int ActorSortOrder { get; }

        public Actor(int actorId, string actorImage, string actorName, string actorRole, int actorSeriesId, int actorSortOrder)
        {
            ActorId = actorId;
            ActorImage = actorImage;
            ActorName = actorName;
            ActorRole = actorRole;
            ActorSeriesId = actorSeriesId;
            ActorSortOrder = actorSortOrder;
        }

        public Actor([NotNull] XElement r)
        {
            ActorId = r.ExtractInt("Id") ?? throw new SourceConsistencyException("Error Extracting Id for Actor",ShowItem.ProviderType.TheTVDB);
            ActorImage = r.ExtractString("Image");
            ActorName = r.ExtractString("Name");
            ActorRole = r.ExtractString("Role");
            ActorSeriesId = r.ExtractInt("SeriesId",-1);
            ActorSortOrder = r.ExtractInt("SortOrder",-1); 
        }

        public Actor(string name)
        {
            ActorName = name;
        }

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Actor");
            writer.WriteElement("Id", ActorId);
            writer.WriteElement("Image", ActorImage);
            writer.WriteElement("Name", ActorName);
            writer.WriteElement("Role", ActorRole);
            writer.WriteElement("SeriesId", ActorSeriesId);
            writer.WriteElement("SortOrder", ActorSortOrder);
            writer.WriteEndElement();
        }

        public bool AsSelf() => ActorName==ActorRole;
    }
}
