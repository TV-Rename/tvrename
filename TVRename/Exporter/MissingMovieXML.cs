using System.Text;
using System.Xml;

namespace TVRename;

internal class MissingMovieXml : ActionListExporter
{
    public MissingMovieXml(ItemList theActionList) : base(theActionList)
    {
    }

    public override bool Active() => TVSettings.Instance.ExportMissingMoviesXML;
    protected override string Name() => "Missing Movie XML Exporter";

    protected override string Location() => TVSettings.Instance.ExportMissingMoviesXMLTo;

    public override bool ApplicableFor(TVSettings.ScanType st) => st == TVSettings.ScanType.Full;

    protected override void Do()
    {
        XmlWriterSettings settings = new()
        {
            Indent = true,
            NewLineOnAttributes = true,
            Encoding = Encoding.ASCII
        };

        using (XmlWriter writer = XmlWriter.Create(Location(), settings))
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("TVRename");
            writer.WriteAttributeToXml("Version", "2.1");
            writer.WriteStartElement("MissingMovieItems");

            foreach (MovieItemMissing missing in TheActionList.MissingMovies)
            {
                writer.WriteStartElement("MissingMovieItem");

                writer.WriteElement("id", missing.MovieConfig.Code);
                writer.WriteElement("title", missing.MovieConfig.ShowName);
                writer.WriteElement("description", missing.MovieConfig.CachedData?.Overview);
                writer.WriteElement("pubDate", missing.MovieConfig.CachedMovie?.Year);

                writer.WriteEndElement(); // MissingMovieItem
            }

            writer.WriteEndElement(); // MissingMovieItems
            writer.WriteEndElement(); // tvrename
            writer.WriteEndDocument();
        }
    }
}
