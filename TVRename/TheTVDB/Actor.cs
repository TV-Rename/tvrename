// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Runtime.Serialization;
using NLog;

namespace TVRename
{
    public class Actor
    {
        private readonly int actorId;
        private readonly string actorImage;
        private readonly string actorName;
        private readonly string actorRole;
        private readonly int actorSeriesId;
        private readonly int actorSortOrder;

        public int ActorId => actorId;
        public string ActorImage => actorImage;
        public string ActorName => actorName;
        public string ActorRole => actorRole;
        public int ActorSeriesId => actorSeriesId;
        public int ActorSortOrder => actorSortOrder;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Actor(string name)
        {
            this.actorName = name;
        }
        public Actor(int actorId, string actorImage, string actorName, string actorRole, int actorSeriesId, int actorSortOrder)
        {
            this.actorId = actorId;
            this.actorImage = actorImage;
            this.actorName = actorName;
            this.actorRole = actorRole;
            this.actorSeriesId = actorSeriesId;
            this.actorSortOrder = actorSortOrder;
        }
        public Actor(XmlReader r)
        {
            try
            {
                r.Read();
                if (r.Name != "Actor")
                    return;

                r.Read();
                while (!r.EOF)
                {
                    if ((r.Name == "Actor") && (!r.IsStartElement()))
                        break;

                    if (r.Name == "Id") actorId = r.ReadElementContentAsInt();
                    else if (r.Name == "Image") actorImage = r.ReadElementContentAsString();
                    else if (r.Name == "Name") actorName = r.ReadElementContentAsString();
                    else if (r.Name == "Role") actorRole = r.ReadElementContentAsString();
                    else if (r.Name == "SeriesId") actorSeriesId = r.ReadElementContentAsInt();
                    else if (r.Name == "SortOrder") actorSortOrder = r.ReadElementContentAsInt();
                    //   r->ReadOuterXml(); // skip
                } // while
            } // try
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for a show.";
                Logger.Error(e, message);
                throw new TheTVDB.TVDBException(e.Message);
            }
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
