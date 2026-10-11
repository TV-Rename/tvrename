//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

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

    public Crew(XElement r)
    {
        Id = r.ExtractInt("Id") ?? throw new ArgumentException("Error Extracting Id for Crew");
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

    internal XElement GenerateXElement()
    {
        return new XElement("CrewMember",
            new XElement("Id", Id),
            XmlHelper.GenerateOptionalXElement("Image", ImageUrl),
            XmlHelper.GenerateOptionalXElement("Name", Name),
            XmlHelper.GenerateOptionalXElement("Job", Job),
            XmlHelper.GenerateOptionalXElement("Department", Department),
            XmlHelper.GenerateOptionalXElement("CreditId", CreditId)
        );
    }
}
