using System.Xml;
using System.Xml.Linq;

namespace TVRename;

public class MovieImage : MediaImage
{
    public int MovieId;
    public TVDoc.ProviderType MovieSource;

    public MovieImage()
    {
        Subject = ImageSubject.movie;
    }

    public MovieImage(int movieId, TVDoc.ProviderType source, XElement r) : base(r)
    {
        MovieId = r.ExtractInt("MovieId") ?? movieId; // thetvdb cachedSeries id
        MovieSource = source;
        Subject = ImageSubject.movie;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("MovieImage");
        WriteCoreXml(writer);
        writer.WriteElement("MovieId", MovieId);
        writer.WriteEndElement(); //MovieImage
    }
}
