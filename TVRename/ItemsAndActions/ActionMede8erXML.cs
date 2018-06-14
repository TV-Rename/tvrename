// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Globalization;

namespace TVRename
{
    using System;
    using System.Xml;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


    public class ActionMede8erXML : ActionWriteMetadata
    {

        public ActionMede8erXML(FileInfo nfo, ProcessedEpisode pe) : base(nfo, null)
        {
            Episode = pe;
        }

        public ActionMede8erXML(FileInfo nfo, ShowItem si) : base(nfo, si)
        {
            Episode = null;
        }


        #region Action Members

        public override string Name => "Write Mede8er Metadata";

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true
                };

                using (XmlWriter writer = XmlWriter.Create(Where.FullName, settings))
                {

                
                if (Episode != null) // specific episode
                {
                    // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
                    writer.WriteStartElement("details");
                    writer.WriteStartElement("movie");
                    XMLHelper.WriteElementToXML(writer, "title", Episode.Name);
                    XMLHelper.WriteElementToXML(writer, "season", Episode.AppropriateSeasonNumber);
                    XMLHelper.WriteElementToXML(writer, "episode", Episode.AppropriateEpNum);

                    writer.WriteStartElement("year");
                    if (Episode.FirstAired != null)
                        writer.WriteValue(Episode.FirstAired.Value.ToString("yyyy"));
                    writer.WriteEndElement();

                    //Mede8er Ratings are on a 100 point scale; TVDB are on a 10 point scale
                    float siteRating = float.Parse(Episode.EpisodeRating, new CultureInfo("en-US")) * 10;
                    int intSiteRating = (int)siteRating;
                    if (intSiteRating > 0) XMLHelper.WriteElementToXML(writer, "rating", intSiteRating);

//                        writer.WriteStartElement("rating");
//                    string rating = (this.Episode.EpisodeRating);
//                    if (!string.IsNullOrEmpty(rating))
//                    {
//                        rating = rating.Trim('.');
//                        rating = rating.Replace(".", "");
//                        writer.WriteValue(rating);
//                    }
//
//                    writer.WriteEndElement(); // rating

                    //Get the Series OverView
                    string sov = Episode.SI.TheSeries().GetOverview();
                    if (!string.IsNullOrEmpty(sov))
                    {
                        XMLHelper.WriteElementToXML(writer, "plot", sov);
                    }

                    //Get the Episode overview
                    XMLHelper.WriteElementToXML(writer, "episodeplot", Episode.Overview);

                    if (Episode.SI != null)
                    {
                        WriteInfo(writer, Episode.SI.TheSeries().GetContentRating(), "mpaa");
                    }

                    //Runtime...taken from overall Series, not episode specific due to thetvdb
                    string rt = Episode.SI.TheSeries().GetRuntime();
                    if (!string.IsNullOrEmpty(rt))
                    {
                        XMLHelper.WriteElementToXML(writer, "runtime", rt + " min");
                    }

                    //Genres...taken from overall Series, not episode specific due to thetvdb
                    writer.WriteStartElement("genres");
                    string genre = String.Join(" / ", Episode.SI.TheSeries().GetGenres());
                    if (!string.IsNullOrEmpty(genre))
                    {
                        XMLHelper.WriteElementToXML(writer, "genre", genre);
                    }

                    writer.WriteEndElement(); // genres

                    //Director(s)
                    if (!string.IsNullOrEmpty(Episode.EpisodeDirector))
                    {
                        string epDirector = Episode.EpisodeDirector;
                        if (!string.IsNullOrEmpty(epDirector))
                        {
                            foreach (string daa in epDirector.Split('|'))
                            {
                                if (string.IsNullOrEmpty(daa))
                                    continue;

                                XMLHelper.WriteElementToXML(writer, "director", daa);
                            }
                        }
                    }

                    //Writers(s)
                    if (!String.IsNullOrEmpty(Episode.Writer))
                    {
                        string epWriter = Episode.Writer;
                        if (!string.IsNullOrEmpty(epWriter))
                        {
                            XMLHelper.WriteElementToXML(writer, "credits", epWriter);
                        }
                    }


                    writer.WriteStartElement("cast");

                    // actors...
                    if (Episode.SI != null)
                    {
                        foreach (string aa in Episode.SI.TheSeries().GetActors())
                        {
                            if (string.IsNullOrEmpty(aa))
                                continue;

                            XMLHelper.WriteElementToXML(writer, "actor", aa);
                        }
                    }

                    writer.WriteEndElement(); // cast
                    writer.WriteEndElement(); // movie
                    writer.WriteEndElement(); // details
                }
                else if (SI != null) // show overview (Series.xml)
                {
                    // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                    writer.WriteStartElement("details");
                    writer.WriteStartElement("movie");

                    XMLHelper.WriteElementToXML(writer, "title", SI.ShowName);

                    writer.WriteStartElement("genres");
                    string genre = String.Join(" / ", SI.TheSeries().GetGenres());
                    if (!string.IsNullOrEmpty(genre))
                    {
                        XMLHelper.WriteElementToXML(writer, "genre", genre);
                    }

                    writer.WriteEndElement(); // genres

                    WriteInfo(writer, SI.TheSeries().GetFirstAired(), "premiered");
                    WriteInfo(writer, SI.TheSeries().GetYear(), "year");


                    //Mede8er Ratings are on a 100 point scale; TVDB are on a 10 point scale
                    float siteRating = float.Parse(SI.TheSeries().GetSiteRating(), new CultureInfo("en-US")) * 10;
                    int intSiteRating = (int)siteRating;
                    if (intSiteRating > 0) XMLHelper.WriteElementToXML(writer, "rating", intSiteRating);

                    /*
                    writer.WriteStartElement("rating");
                    string rating = this.SI.TheSeries().GetSiteRating();
                    if (!string.IsNullOrEmpty(rating))
                    {
                        rating = rating.Trim('.');
                        rating = rating.Replace(".", "");
                        writer.WriteValue(rating);
                    }

                    writer.WriteEndElement(); // rating
                    */

                    WriteInfo(writer, SI.TheSeries().getStatus(), "status");

                    WriteInfo(writer, SI.TheSeries().GetContentRating(), "mpaa");
                    WriteInfo(writer, SI.TheSeries().GetIMDB(), "id", "moviedb", "imdb");

                    XMLHelper.WriteElementToXML(writer, "tvdbid", SI.TheSeries().TVDBCode);

                    string rt = SI.TheSeries().GetRuntime();
                    if (!string.IsNullOrEmpty(rt))
                    {
                        XMLHelper.WriteElementToXML(writer, "runtime", rt + " min");
                    }

                    WriteInfo(writer, SI.TheSeries().GetOverview(), "plot");

                    writer.WriteStartElement("cast");

                    // actors...

                    foreach (string aa in SI.TheSeries().GetActors())
                    {
                        if (string.IsNullOrEmpty(aa))
                            continue;
                        XMLHelper.WriteElementToXML(writer, "actor", aa);
                    }

                    writer.WriteEndElement(); // cast
                    writer.WriteEndElement(); // movie
                    writer.WriteEndElement(); // tvshow
                }

                Done = true;
                return true;
                }
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                Error = true;
                Done = true;
                return false;
            }
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ActionMede8erXML xml) && (xml.Where == Where);
        }

        public override int Compare(Item o)
        {
            ActionMede8erXML nfo = o as ActionMede8erXML;

            if (Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            return (Where.FullName + Episode.Name).CompareTo(nfo.Where.FullName + nfo.Episode.Name);
        }

        #endregion

        private static void WriteInfo(XmlWriter writer, string value, string elemName, string attribute = null, string attributeVal = null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteStartElement(elemName);
                if (!String.IsNullOrEmpty(attribute) && !String.IsNullOrEmpty(attributeVal))
                {
                    writer.WriteAttributeString(attribute, attributeVal);
                }
                writer.WriteValue(value);
                writer.WriteEndElement();
            }
        }
    }
}
