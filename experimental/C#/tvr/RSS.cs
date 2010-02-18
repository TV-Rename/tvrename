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
	public class RSSItem
	{
		public string URL;
		public int Season;
		public int Episode;
		public string Title;
		public string ShowName;

		public RSSItem(string url, string title, int season, int episode, string showName)
		{
			URL = url;
			Season = season;
			Episode = episode;
			Title = title;
			ShowName = showName;
		}
	}

	public class RSSItemList : System.Collections.Generic.List<RSSItem>
	{
		private System.Collections.Generic.List<FilenameProcessorRE > Rexps; // only trustable while in DownloadRSS or its called functions

        public bool DownloadRSS(string URL, System.Collections.Generic.List<FilenameProcessorRE> rexps)
		{
			Rexps = rexps;

			System.Net.WebClient wc = new System.Net.WebClient();
			try
			{
				byte[] r = wc.DownloadData(URL);

				MemoryStream ms = new MemoryStream(r);

				XmlReaderSettings settings = new XmlReaderSettings();
				settings.IgnoreComments = true;
				settings.IgnoreWhitespace = true;
				XmlReader reader = XmlReader.Create(ms, settings);

				reader.Read();
				if (reader.Name != "xml")
					return false;

				reader.Read();

				if (reader.Name != "rss")
					return false;

				reader.Read();

				while (!reader.EOF)
				{
					if ((reader.Name == "rss") && (!reader.IsStartElement()))
						break;

					if (reader.Name == "channel")
					{
						if (!ReadChannel(reader.ReadSubtree()))
							return false;
						reader.Read();
					}
					else
						reader.ReadOuterXml();

				}

				ms.Close();

			}
			catch
			{
				return false;
			}
			finally
			{
				Rexps = null;
			}

			return true;
		}

	private bool ReadChannel(XmlReader r)
		{
			r.Read();
			r.Read();
			while (!r.EOF)
			{
				if ((r.Name == "channel") && (!r.IsStartElement()))
					break;
				if (r.Name == "item")
				{
					if (!ReadItem(r.ReadSubtree()))
						return false;
					r.Read();
				}
				else
					r.ReadOuterXml();
			}
			return true;
		}

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//	bool ReadItem(XmlReader r);


	}



	// ref class MissingEpisode;
//
//    public ref class RSSMissingItem      ------------> becomes AIORSS
//    {
//    public:
//        RSSItem ^RSS;
//        MissingEpisode ^Episode;
//
//        RSSMissingItem(RSSItem ^rss, MissingEpisode ^ep)
//        {
//            RSS = rss;
//            Episode = ep;
//        }
//    };
//
//    typedef System::Collections::Generic::List<RSSMissingItem ^> RSSMissingItemList;
//



} // namespace