// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.IO;
using System.Xml;

// Keeps count of some statistics.

namespace TVRename
{
    public class TVRenameStats
    {
        public int AutoAddedShows;
        public int FilesCopied;
        public int FilesMoved;
        public int FilesRenamed;
        public int FindAndOrganisesDone;
        public int MissingChecksDone;
        public int NS_NumberOfEpisodes; // NS = "Not Saved", i.e. counted when doing a Scan
        public int NS_NumberOfEpisodesExpected;
        public int NS_NumberOfSeasons;
        public int NS_NumberOfShows;
        public int RenameChecksDone;
        public int TorrentsMatched;

        public TVRenameStats()
        {
            this.FilesMoved = 0;
            this.FilesRenamed = 0;
            this.FilesCopied = 0;
            this.RenameChecksDone = 0;
            this.MissingChecksDone = 0;
            this.FindAndOrganisesDone = 0;
            this.AutoAddedShows = 0;
            this.TorrentsMatched = 0;
            this.NS_NumberOfShows = 0;
            this.NS_NumberOfSeasons = 0;
            this.NS_NumberOfEpisodes = -1; // -1 signals 'unknown'
            this.NS_NumberOfEpisodesExpected = 0;
        }

        public bool Load()
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            string fn = PathManager.StatisticsFile.FullName;
            if (!File.Exists(fn))
                return true;

            XmlReader reader = XmlReader.Create(fn, settings);

            reader.Read();

            if (reader.Name != "xml")
                return false;

            reader.Read();

            if (reader.Name != "Statistics")
                return false;

            reader.Read();
            while (!reader.EOF)
            {
                if ((reader.Name == "Statistics") && !reader.IsStartElement())
                    break;

                if (reader.Name == "FilesMoved")
                    this.FilesMoved = reader.ReadElementContentAsInt();
                else if (reader.Name == "FilesRenamed")
                    this.FilesRenamed = reader.ReadElementContentAsInt();
                else if (reader.Name == "FilesCopied")
                    this.FilesCopied = reader.ReadElementContentAsInt();
                else if (reader.Name == "RenameChecksDone")
                    this.RenameChecksDone = reader.ReadElementContentAsInt();
                else if (reader.Name == "MissingChecksDone")
                    this.MissingChecksDone = reader.ReadElementContentAsInt();
                else if (reader.Name == "FindAndOrganisesDone")
                    this.FindAndOrganisesDone = reader.ReadElementContentAsInt();
                else if (reader.Name == "AutoAddedShows")
                    this.AutoAddedShows = reader.ReadElementContentAsInt();
                else if (reader.Name == "TorrentsMatched")
                    this.TorrentsMatched = reader.ReadElementContentAsInt();
            }
            reader.Close();
            return true;
        }

        public void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };

            XmlWriter writer = XmlWriter.Create(PathManager.StatisticsFile.FullName, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Statistics");

            writer.WriteStartElement("FilesMoved");
            writer.WriteValue(this.FilesMoved);
            writer.WriteEndElement();

            writer.WriteStartElement("FilesRenamed");
            writer.WriteValue(this.FilesRenamed);
            writer.WriteEndElement();

            writer.WriteStartElement("FilesCopied");
            writer.WriteValue(this.FilesCopied);
            writer.WriteEndElement();

            writer.WriteStartElement("RenameChecksDone");
            writer.WriteValue(this.RenameChecksDone);
            writer.WriteEndElement();

            writer.WriteStartElement("MissingChecksDone");
            writer.WriteValue(this.MissingChecksDone);
            writer.WriteEndElement();

            writer.WriteStartElement("FindAndOrganisesDone");
            writer.WriteValue(this.FindAndOrganisesDone);
            writer.WriteEndElement();

            writer.WriteStartElement("AutoAddedShows");
            writer.WriteValue(this.AutoAddedShows);
            writer.WriteEndElement();

            writer.WriteStartElement("TorrentsMatched");
            writer.WriteValue(this.TorrentsMatched);
            writer.WriteEndElement();

            writer.WriteEndElement(); // statistics

            writer.WriteEndDocument();
            writer.Close();
        }
    }
}