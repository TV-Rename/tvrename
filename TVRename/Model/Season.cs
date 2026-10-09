//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class Season
{
    public int SeasonId { get; }
    public string? SeasonName { get; }
    public int SeasonSeriesId { get; }
    public int SeasonNumber { get; }
    public string? SeasonDescription { get; }
    public string? Url { get; }
    public string? ImageUrl { get; }

    public Season(XElement r)
    {
        SeasonId = r.ExtractInt("Id") ?? -1;
        SeasonName = r.ExtractString("Name");
        SeasonSeriesId = r.ExtractInt("SeriesId", -1);
        SeasonNumber = r.ExtractInt("seasonNumber") ?? -1;
        SeasonDescription = r.ExtractString("description");
        ImageUrl = r.ExtractString("imageUrl");
    }

    public Season(int seasonId, int seasonNumber, string? seasonName, string? description, string? url, string? imageUrl, int seriesId)
    {
        SeasonId = seasonId;
        SeasonNumber = seasonNumber;
        SeasonName = seasonName;
        SeasonDescription = description;
        SeasonSeriesId = seriesId;
        ImageUrl = imageUrl;
        Url = url;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Season");
        writer.WriteElement("Id", SeasonId);
        writer.WriteElement("Name", SeasonName);
        writer.WriteElement("SeriesId", SeasonSeriesId);
        writer.WriteElement("seasonNumber", SeasonNumber);
        writer.WriteElement("description", SeasonDescription);
        writer.WriteElement("imageUrl", ImageUrl);
        writer.WriteEndElement();
    }
}
