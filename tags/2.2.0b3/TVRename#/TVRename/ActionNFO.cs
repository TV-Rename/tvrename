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

    public class ActionNFO : ActionItem
    {
        public ShowItem SI; // if for an entire show, rather than specific episode
        public FileInfo Where;

        public ActionNFO(FileInfo nfo, ProcessedEpisode pe)
            : base(ActionType.kNFO, pe)
        {
            this.SI = null;
            this.Where = nfo;
        }

        public ActionNFO(FileInfo nfo, ShowItem si)
            : base(ActionType.kNFO, null)
        {
            this.SI = si;
            this.Where = nfo;
        }

        public override IgnoreItem GetIgnore()
        {
            if (this.Where == null)
                return null;
            return new IgnoreItem(this.Where.FullName);
        }

        public override string TargetFolder()
        {
            if (this.Where == null)
                return null;
            return this.Where.DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return this.Where.Name;
        }

        public bool SameAs2(ActionNFO o)
        {
            return (o.Where == this.Where);
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionNFO) (o));
        }

        public override int IconNumber()
        {
            return 7;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            if (this.PE != null)
            {
                lvi.Text = this.PE.SI.ShowName();
                lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
                lvi.SubItems.Add(this.PE.NumsAsString());
                DateTime? dt = this.PE.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }
            else
            {
                lvi.Text = this.SI.ShowName();
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
            }

            lvi.SubItems.Add(this.Where.DirectoryName);
            lvi.SubItems.Add(this.Where.Name);

            lvi.Group = lv.Groups[6];
            lvi.Tag = this;

            //lv->Items->Add(lvi);
            return lvi;
        }

        public static void WriteInfo(XmlWriter writer, ShowItem si, string whichItem, string @as)
        {
            string t = si.TheSeries().GetItem(whichItem);
            if (!string.IsNullOrEmpty(t))
            {
                writer.WriteStartElement(@as);
                writer.WriteValue(t);
                writer.WriteEndElement();
            }
        }

        public override bool Action(TVDoc doc)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlWriter writer = XmlWriter.Create(this.Where.FullName, settings);

            if (this.PE != null) // specific episode
            {
                // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
                writer.WriteStartElement("episodedetails");
                writer.WriteStartElement("title");
                writer.WriteValue(this.PE.Name);
                writer.WriteEndElement();
                writer.WriteStartElement("season");
                writer.WriteValue(this.PE.SeasonNumber);
                writer.WriteEndElement();
                writer.WriteStartElement("episode");
                writer.WriteValue(this.PE.EpNum);
                writer.WriteEndElement();
                writer.WriteStartElement("plot");
                writer.WriteValue(this.PE.Overview);
                writer.WriteEndElement();
                writer.WriteStartElement("aired");
                if (this.PE.FirstAired != null)
                    writer.WriteValue(this.PE.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
                writer.WriteEndElement(); // episodedetails
            }
            else if (this.SI != null) // show overview (tvshow.nfo)
            {
                // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                writer.WriteStartElement("tvshow");

                writer.WriteStartElement("title");
                writer.WriteValue(this.SI.ShowName());
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
                WriteInfo(writer, this.SI, "Rating", "rating");

                // actors...
                string actors = this.SI.TheSeries().GetItem("Actors");
                if (!string.IsNullOrEmpty(actors))
                {
                    foreach (string aa in actors.Split('|'))
                    {
                        if (!string.IsNullOrEmpty(aa))
                        {
                            writer.WriteStartElement("actor");
                            writer.WriteStartElement("name");
                            writer.WriteValue(aa);
                            writer.WriteEndElement(); // name
                            writer.WriteEndElement(); // actor
                        }
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
    }
}