// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    public class ActionNFO : Item, Action, ScanListItem
    {
        public ShowItem SI; // if for an entire show, rather than specific episode
        public FileInfo Where;

        public ActionNFO(FileInfo nfo, ProcessedEpisode pe)
        {
            this.SI = null;
            this.Episode = pe;
            this.Where = nfo;
        }

        public ActionNFO(FileInfo nfo, ShowItem si)
        {
            this.SI = si;
            this.Episode = null;
            this.Where = nfo;
        }

        #region Action Members

        public string Name
        {
            get { return "Write NFO"; }
        }

        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; set; }

        public string ProgressText
        {
            get { return this.Where.Name; }
        }

        public double PercentDone
        {
            get { return this.Done ? 100 : 0; }
        }

        public long SizeOfWork
        {
            get { return 10000; }
        }

        public bool Go(TVSettings tvsettings, ref bool pause)
        {
            XmlWriterSettings settings = new XmlWriterSettings {
                                                                   Indent = true,
                                                                   NewLineOnAttributes = true
                                                               };
            XmlWriter writer = XmlWriter.Create(this.Where.FullName, settings);
            if (writer == null)
                return false;

            if (this.Episode != null) // specific episode
            {
                // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
                writer.WriteStartElement("episodedetails");
                writer.WriteStartElement("title");
                writer.WriteValue(this.Episode.Name);
                writer.WriteEndElement();
                writer.WriteStartElement("season");
                writer.WriteValue(this.Episode.SeasonNumber);
                writer.WriteEndElement();
                writer.WriteStartElement("episode");
                writer.WriteValue(this.Episode.EpNum);
                writer.WriteEndElement();
                writer.WriteStartElement("plot");
                writer.WriteValue(this.Episode.Overview);
                writer.WriteEndElement();
                writer.WriteStartElement("aired");
                if (this.Episode.FirstAired != null)
                    writer.WriteValue(this.Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
                writer.WriteEndElement(); // episodedetails
            }
            else if (this.SI != null) // show overview (tvshow.nfo)
            {
                // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                writer.WriteStartElement("tvshow");

                writer.WriteStartElement("title");
                writer.WriteValue(this.SI.ShowName);
                writer.WriteEndElement();

                writer.WriteStartElement("episodeguideurl");
                writer.WriteValue(TheTVDB.BuildURL(true, true, this.SI.TVDBCode, this.SI.TVDB.PreferredLanguage(this.SI.TVDBCode)));
                writer.WriteEndElement();

                WriteInfo(writer, this.SI, "Overview", "plot");

                string genre = this.SI.TheSeries().GetItem("Genre");
                if (!string.IsNullOrEmpty(genre))
                {
                    genre = genre.Trim('|');
                    genre = genre.Replace("|", " / ");
                    writer.WriteStartElement("genre");
                    writer.WriteValue(genre);
                    writer.WriteEndElement();
                }

                WriteInfo(writer, this.SI, "FirstAired", "premiered");
                WriteInfo(writer, this.SI, "Year", "year");
                WriteInfo(writer, this.SI, "Rating", "rating");
                WriteInfo(writer, this.SI, "Status", "status");

                // actors...
                string actors = this.SI.TheSeries().GetItem("Actors");
                if (!string.IsNullOrEmpty(actors))
                {
                    foreach (string aa in actors.Split('|'))
                    {
                        if (string.IsNullOrEmpty(aa))
                            continue;

                        writer.WriteStartElement("actor");
                        writer.WriteStartElement("name");
                        writer.WriteValue(aa);
                        writer.WriteEndElement(); // name
                        writer.WriteEndElement(); // actor
                    }
                }

                WriteInfo(writer, this.SI, "ContentRating", "mpaa");
                WriteInfo(writer, this.SI, "IMDB_ID", "id");

                writer.WriteStartElement("tvdbid");
                writer.WriteValue(this.SI.TheSeries().TVDBCode);
                writer.WriteEndElement();

                string rt = this.SI.TheSeries().GetItem("Runtime");
                if (!string.IsNullOrEmpty(rt))
                {
                    writer.WriteStartElement("runtime");
                    writer.WriteValue(rt + " minutes");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // tvshow
            }

            writer.Close();
            this.Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ActionNFO) && ((o as ActionNFO).Where == this.Where);
        }

        public int Compare(Item o)
        {
            ActionNFO nfo = o as ActionNFO;

            if (this.Episode == null)
                return 1;
            if (nfo.Episode == null)
                return -1;
            return (this.Where.FullName + this.Episode.Name).CompareTo(nfo.Where.FullName + nfo.Episode.Name);
        }

        #endregion

        #region ScanListItem Members

        public IgnoreItem Ignore
        {
            get
            {
                if (this.Where == null)
                    return null;
                return new IgnoreItem(this.Where.FullName);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                if (this.Episode != null)
                {
                    lvi.Text = this.Episode.SI.ShowName;
                    lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
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

        string ScanListItem.TargetFolder
        {
            get
            {
                if (this.Where == null)
                    return null;
                return this.Where.DirectoryName;
            }
        }

        public int ScanListViewGroup
        {
            get { return 6; }
        }

        public int IconNumber
        {
            get { return 7; }
        }

        public ProcessedEpisode Episode { get; private set; }

        #endregion

        private static void WriteInfo(XmlWriter writer, ShowItem si, string whichItem, string @as)
        {
            string t = si.TheSeries().GetItem(whichItem);
            if (!string.IsNullOrEmpty(t))
            {
                writer.WriteStartElement(@as);
                writer.WriteValue(t);
                writer.WriteEndElement();
            }
        }
    }
}