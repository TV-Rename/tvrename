// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    using System;
    using System.Windows.Forms;
    using System.Xml;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


    public class ActionMede8erXML : ActionWriteMetadata
    {
           public ShowItem SI; // if for an entire show, rather than specific episode

        public ActionMede8erXML(FileInfo nfo, ProcessedEpisode pe)
        {
            this.SI = null;
            this.Episode = pe;
            this.Where = nfo;
        }

        public ActionMede8erXML(FileInfo nfo, ShowItem si)
        {
            this.SI = si;
            this.Episode = null;
            this.Where = nfo;
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

                using (XmlWriter writer = XmlWriter.Create(this.Where.FullName, settings))
                {

                
                if (this.Episode != null) // specific episode
                {
                    // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
                    writer.WriteStartElement("details");
                    writer.WriteStartElement("movie");
                    XMLHelper.WriteElementToXML(writer, "title", this.Episode.Name);
                    XMLHelper.WriteElementToXML(writer, "season", this.Episode.AppropriateSeasonNumber);
                    XMLHelper.WriteElementToXML(writer, "episode", this.Episode.AppropriateEpNum);

                    writer.WriteStartElement("year");
                    if (this.Episode.FirstAired != null)
                        writer.WriteValue(this.Episode.FirstAired.Value.ToString("yyyy"));
                    writer.WriteEndElement();

                    //Mede8er Ratings are on a 100 point scale; TVDB are on a 10 point scale
                    float siteRating = float.Parse(this.Episode.EpisodeRating) * 10;
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
                    string sov = this.Episode.SI.TheSeries().GetOverview();
                    if (!string.IsNullOrEmpty(sov))
                    {
                        XMLHelper.WriteElementToXML(writer, "plot", sov);
                    }

                    //Get the Episode overview
                    XMLHelper.WriteElementToXML(writer, "episodeplot", this.Episode.Overview);

                    if (this.Episode.SI != null)
                    {
                        WriteInfo(writer, this.Episode.SI.TheSeries().GetContentRating(), "mpaa");
                    }

                    //Runtime...taken from overall Series, not episode specific due to thetvdb
                    string rt = this.Episode.SI.TheSeries().GetRuntime();
                    if (!string.IsNullOrEmpty(rt))
                    {
                        XMLHelper.WriteElementToXML(writer, "runtime", rt + " min");
                    }

                    //Genres...taken from overall Series, not episode specific due to thetvdb
                    writer.WriteStartElement("genres");
                    string genre = String.Join(" / ", this.Episode.SI.TheSeries().GetGenres());
                    if (!string.IsNullOrEmpty(genre))
                    {
                        XMLHelper.WriteElementToXML(writer, "genre", genre);
                    }

                    writer.WriteEndElement(); // genres

                    //Director(s)
                    if (!string.IsNullOrEmpty(this.Episode.EpisodeDirector))
                    {
                        string epDirector = this.Episode.EpisodeDirector;
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
                    if (!String.IsNullOrEmpty(this.Episode.Writer))
                    {
                        string epWriter = this.Episode.Writer;
                        if (!string.IsNullOrEmpty(epWriter))
                        {
                            XMLHelper.WriteElementToXML(writer, "credits", epWriter);
                        }
                    }


                    writer.WriteStartElement("cast");

                    // actors...
                    if (this.Episode.SI != null)
                    {
                        foreach (string aa in this.Episode.SI.TheSeries().GetActors())
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
                else if (this.SI != null) // show overview (Series.xml)
                {
                    // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                    writer.WriteStartElement("details");
                    writer.WriteStartElement("movie");

                    XMLHelper.WriteElementToXML(writer, "title", this.SI.ShowName);

                    writer.WriteStartElement("genres");
                    string genre = String.Join(" / ", this.SI.TheSeries().GetGenres());
                    if (!string.IsNullOrEmpty(genre))
                    {
                        XMLHelper.WriteElementToXML(writer, "genre", genre);
                    }

                    writer.WriteEndElement(); // genres

                    WriteInfo(writer, this.SI.TheSeries().GetFirstAired(), "premiered");
                    WriteInfo(writer, this.SI.TheSeries().GetYear(), "year");


                    //Mede8er Ratings are on a 100 point scale; TVDB are on a 10 point scale
                    float siteRating = float.Parse(this.SI.TheSeries().GetSiteRating()) * 10;
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

                    WriteInfo(writer, this.SI.TheSeries().getStatus(), "status");

                    WriteInfo(writer, this.SI.TheSeries().GetContentRating(), "mpaa");
                    WriteInfo(writer, this.SI.TheSeries().GetIMDB(), "id", "moviedb", "imdb");

                    XMLHelper.WriteElementToXML(writer, "tvdbid", this.SI.TheSeries().TVDBCode);

                    string rt = this.SI.TheSeries().GetRuntime();
                    if (!string.IsNullOrEmpty(rt))
                    {
                        XMLHelper.WriteElementToXML(writer, "runtime", rt + " min");
                    }

                    WriteInfo(writer, this.SI.TheSeries().GetOverview(), "plot");

                    writer.WriteStartElement("cast");

                    // actors...

                    foreach (string aa in this.SI.TheSeries().GetActors())
                    {
                        if (string.IsNullOrEmpty(aa))
                            continue;
                        XMLHelper.WriteElementToXML(writer, "actor", aa);
                    }

                    writer.WriteEndElement(); // cast
                    writer.WriteEndElement(); // movie
                    writer.WriteEndElement(); // tvshow
                }

                this.Done = true;
                return true;
                }
            }
            catch (Exception e)
            {
                this.ErrorText = e.Message;
                this.Error = true;
                this.Done = true;
                return false;
            }
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ActionMede8erXML xml) && (xml.Where == this.Where);
        }

        public override int Compare(Item o)
        {
            ActionMede8erXML nfo = o as ActionMede8erXML;

            if (this.Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            return (this.Where.FullName + this.Episode.Name).CompareTo(nfo.Where.FullName + nfo.Episode.Name);
        }

        #endregion

        #region Item Members
        public override IgnoreItem Ignore
        {
            get
            {
                if (this.Where == null)
                    return null;
                return new IgnoreItem(this.Where.FullName);
            }
        }

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                if (this.Episode != null)
                {
                    lvi.Text = this.Episode.SI.ShowName;
                    lvi.SubItems.Add(this.Episode.AppropriateSeasonNumber.ToString());
                    lvi.SubItems.Add(this.Episode.NumsAsString());
                    DateTime? dt = this.Episode.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }
                else
                {
                    lvi.Text = this.SI.ShowName;
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                }

                lvi.SubItems.Add(this.Where.DirectoryName);
                lvi.SubItems.Add(this.Where.Name);

                lvi.Tag = this;

                //lv->Items->Add(lvi);
                return lvi;
            }
        }

        public override string TargetFolder
        {
            get
            {
                if (this.Where == null)
                    return null;
                return this.Where.DirectoryName;
            }
        }

        public override string ScanListViewGroup => "lvgActionMeta";

        public override int IconNumber => 7;

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
