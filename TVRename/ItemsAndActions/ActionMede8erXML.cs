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
                    XmlHelper.WriteElementToXml(writer, "title", Episode.Name);
                    XmlHelper.WriteElementToXml(writer, "season", Episode.AppropriateSeasonNumber);
                    XmlHelper.WriteElementToXml(writer, "episode", Episode.AppropriateEpNum);

                    writer.WriteStartElement("year");
                    if (Episode.FirstAired != null)
                        writer.WriteValue(Episode.FirstAired.Value.ToString("yyyy"));
                    writer.WriteEndElement();

                    //Mede8er Ratings are on a 100 point scale; TVDB are on a 10 point scale
                    float siteRating = float.Parse(Episode.EpisodeRating, new CultureInfo("en-US")) * 10;
                    int intSiteRating = (int)siteRating;
                    if (intSiteRating > 0) XmlHelper.WriteElementToXml(writer, "rating", intSiteRating);

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
                        XmlHelper.WriteElementToXml(writer, "plot", sov);
                    }

                    //Get the Episode overview
                    XmlHelper.WriteElementToXml(writer, "episodeplot", Episode.Overview);

                    if (Episode.SI != null)
                    {
                        WriteInfo(writer, Episode.SI.TheSeries().GetContentRating(), "mpaa");
                    }

                    //Runtime...taken from overall Series, not episode specific due to thetvdb
                    string rt = Episode.SI.TheSeries().GetRuntime();
                    if (!string.IsNullOrEmpty(rt))
                    {
                        XmlHelper.WriteElementToXml(writer, "runtime", rt + " min");
                    }

                    //Genres...taken from overall Series, not episode specific due to thetvdb
                    writer.WriteStartElement("genres");
                    string genre = string.Join(" / ", Episode.SI.TheSeries().GetGenres());
                    if (!string.IsNullOrEmpty(genre))
                    {
                        XmlHelper.WriteElementToXml(writer, "genre", genre);
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

                                XmlHelper.WriteElementToXml(writer, "director", daa);
                            }
                        }
                    }

                    //Writers(s)
                    if (!string.IsNullOrEmpty(Episode.Writer))
                    {
                        string epWriter = Episode.Writer;
                        if (!string.IsNullOrEmpty(epWriter))
                        {
                            XmlHelper.WriteElementToXml(writer, "credits", epWriter);
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

                            XmlHelper.WriteElementToXml(writer, "actor", aa);
                        }
                    }

                    writer.WriteEndElement(); // cast
                    writer.WriteEndElement(); // movie
                    writer.WriteEndElement(); // details
                }
                else if (SelectedShow != null) // show overview (Series.xml)
                {
                    // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                    writer.WriteStartElement("details");
                    writer.WriteStartElement("movie");

                    XmlHelper.WriteElementToXml(writer, "title", SelectedShow.ShowName);

                    writer.WriteStartElement("genres");
                    string genre = string.Join(" / ", SelectedShow.TheSeries().GetGenres());
                    if (!string.IsNullOrEmpty(genre))
                    {
                        XmlHelper.WriteElementToXml(writer, "genre", genre);
                    }

                    writer.WriteEndElement(); // genres

                    WriteInfo(writer, SelectedShow.TheSeries().GetFirstAired(), "premiered");
                    WriteInfo(writer, SelectedShow.TheSeries().GetYear(), "year");


                    //Mede8er Ratings are on a 100 point scale; TVDB are on a 10 point scale
                    float siteRating = float.Parse(SelectedShow.TheSeries().GetSiteRating(), new CultureInfo("en-US")) * 10;
                    int intSiteRating = (int)siteRating;
                    if (intSiteRating > 0) XmlHelper.WriteElementToXml(writer, "rating", intSiteRating);

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

                    WriteInfo(writer, SelectedShow.TheSeries().GetStatus(), "status");

                    WriteInfo(writer, SelectedShow.TheSeries().GetContentRating(), "mpaa");
                    WriteInfo(writer, SelectedShow.TheSeries().GetImdb(), "id", "moviedb", "imdb");

                    XmlHelper.WriteElementToXml(writer, "tvdbid", SelectedShow.TheSeries().TVDBCode);

                    string rt = SelectedShow.TheSeries().GetRuntime();
                    if (!string.IsNullOrEmpty(rt))
                    {
                        XmlHelper.WriteElementToXml(writer, "runtime", rt + " min");
                    }

                    WriteInfo(writer, SelectedShow.TheSeries().GetOverview(), "plot");

                    writer.WriteStartElement("cast");

                    // actors...

                    foreach (string aa in SelectedShow.TheSeries().GetActors())
                    {
                        if (string.IsNullOrEmpty(aa))
                            continue;
                        XmlHelper.WriteElementToXml(writer, "actor", aa);
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
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeVal))
                {
                    writer.WriteAttributeString(attribute, attributeVal);
                }
                writer.WriteValue(value);
                writer.WriteEndElement();
            }
        }
    }
}
