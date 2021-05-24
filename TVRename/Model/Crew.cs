using JetBrains.Annotations;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public class Crew
    {
        public int Id { get; }
        public string? ImageUrl { get; }
        public string Name { get; }
        public string? Job { get; }
        public string? Department { get; }
        public string? CreditId { get; }

        public Crew(int id, string? image, string actorName, string? job, string? department, string? creditId)
        {
            Id = id;
            ImageUrl = image;
            Name = actorName;
            Job = job;
            Department = department;
            CreditId = creditId;
        }

        public Crew([NotNull] XElement r)
        {
            Id = r.ExtractInt("Id") ?? throw new SourceConsistencyException("Error Extracting Id for Crew", TVDoc.ProviderType.TheTVDB);
            ImageUrl = r.ExtractString("Image");
            Name = r.ExtractString("Name");
            Job = r.ExtractString("Job");
            Department = r.ExtractString("Department");
            CreditId = r.ExtractString("CreditId");
        }

        public Crew(string name)
        {
            Name = name;
        }

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("CrewMember");
            writer.WriteElement("Id", Id);
            writer.WriteElement("Image", ImageUrl);
            writer.WriteElement("Name", Name);
            writer.WriteElement("Job", Job);
            writer.WriteElement("Department", Department);
            writer.WriteElement("CreditId", CreditId);
            writer.WriteEndElement();
        }
    }
}
