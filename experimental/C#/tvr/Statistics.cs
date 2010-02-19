//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System.Xml;
using System;
using System.IO;

namespace TVRename
{
	public class TVRenameStats
	{
		public int FilesMoved;
		public int FilesRenamed;
		public int FilesCopied;
		public int RenameChecksDone;
		public int MissingChecksDone;
		public int FindAndOrganisesDone;
		public int AutoAddedShows;
		public int TorrentsMatched;

		// these starts are generated on the fly, not saved:
		public int NS_NumberOfShows;
		public int NS_NumberOfSeasons;
		public int NS_NumberOfEpisodes;
		public int NS_NumberOfEpisodesExpected;

		public TVRenameStats()
		{
			FilesMoved = 0;
			FilesRenamed = 0;
			FilesCopied = 0;
			RenameChecksDone = 0;
			MissingChecksDone = 0;
			FindAndOrganisesDone = 0;
			AutoAddedShows = 0;
			TorrentsMatched = 0;
			NS_NumberOfShows = 0;
			NS_NumberOfSeasons = 0;
			NS_NumberOfEpisodes = -1;
			NS_NumberOfEpisodesExpected = 0;
		}
		public bool Load()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreComments = true;
			settings.IgnoreWhitespace = true;

			string fn = System.Windows.Forms.Application.UserAppDataPath+"\\Statistics.xml";
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
					FilesMoved = reader.ReadElementContentAsInt();
				else if (reader.Name == "FilesRenamed")
					FilesRenamed = reader.ReadElementContentAsInt();
				else if (reader.Name == "FilesCopied")
					FilesCopied = reader.ReadElementContentAsInt();
				else if (reader.Name == "RenameChecksDone")
					RenameChecksDone = reader.ReadElementContentAsInt();
				else if (reader.Name == "MissingChecksDone")
					MissingChecksDone = reader.ReadElementContentAsInt();
				else if (reader.Name == "FindAndOrganisesDone")
					FindAndOrganisesDone = reader.ReadElementContentAsInt();
				else if (reader.Name == "AutoAddedShows")
					AutoAddedShows = reader.ReadElementContentAsInt();
				else if (reader.Name == "TorrentsMatched")
					TorrentsMatched = reader.ReadElementContentAsInt();
			}
			reader.Close();
			return true;
		}
		public void Save()
		{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.NewLineOnAttributes = true;
				XmlWriter writer = XmlWriter.Create(System.Windows.Forms.Application.UserAppDataPath+"\\Statistics.xml", settings);

				writer.WriteStartDocument();
			writer.WriteStartElement("Statistics");

			writer.WriteStartElement("FilesMoved");
			writer.WriteValue(FilesMoved);
			writer.WriteEndElement();

			writer.WriteStartElement("FilesRenamed");
			writer.WriteValue(FilesRenamed);
			writer.WriteEndElement();

			writer.WriteStartElement("FilesCopied");
			writer.WriteValue(FilesCopied);
			writer.WriteEndElement();

			writer.WriteStartElement("RenameChecksDone");
			writer.WriteValue(RenameChecksDone);
			writer.WriteEndElement();

			writer.WriteStartElement("MissingChecksDone");
			writer.WriteValue(MissingChecksDone);
			writer.WriteEndElement();

			writer.WriteStartElement("FindAndOrganisesDone");
			writer.WriteValue(FindAndOrganisesDone);
			writer.WriteEndElement();

			writer.WriteStartElement("AutoAddedShows");
			writer.WriteValue(AutoAddedShows);
			writer.WriteEndElement();

			writer.WriteStartElement("TorrentsMatched");
			writer.WriteValue(TorrentsMatched);
			writer.WriteEndElement();

			writer.WriteEndElement(); // statistics

			writer.WriteEndDocument();
			writer.Close();
		}
	} // TVRenameStatistics
} // namespace