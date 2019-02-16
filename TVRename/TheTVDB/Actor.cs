// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Xml;
using System.Xml.Linq;

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

        public Actor(XElement r)
        {
            ActorId = r.ExtractInt("Id") ?? throw new TheTVDB.TVDBException("Error Extracting Id for Actor");
            ActorImage = r.ExtractString("Image");
            ActorName = r.ExtractString("Name");
            ActorRole = r.ExtractString("Role");
            ActorSeriesId = r.ExtractInt("SeriesId") ?? -1;
            ActorSortOrder = r.ExtractInt("SortOrder") ?? -1; 
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Actor");
            XmlHelper.WriteElementToXml(writer, "Id", ActorId);
            XmlHelper.WriteElementToXml(writer, "Image", ActorImage);
            XmlHelper.WriteElementToXml(writer, "Name", ActorName);
            XmlHelper.WriteElementToXml(writer, "Role", ActorRole);
            XmlHelper.WriteElementToXml(writer, "SeriesId", ActorSeriesId);
            XmlHelper.WriteElementToXml(writer, "SortOrder", ActorSortOrder);
            writer.WriteEndElement();
        }
    }
}
