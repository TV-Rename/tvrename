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
    using System.Windows.Forms;
    using System.Xml;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

    public class ActionNFO : Item, Action, ScanListItem, ActionWriteMetadata
    {
        public ShowItem SI; // if for an entire show, rather than specific episode
        public FileInfo Where;

        public ActionNFO(FileInfo nfo, ProcessedEpisode pe)
        {
            SI = null;
            Episode = pe;
            Where = nfo;
        }

        public ActionNFO(FileInfo nfo, ShowItem si)
        {
            SI = si;
            Episode = null;
            Where = nfo;
        }

        #region Action Members

        public string Name
        {
            get { return "Write KODI Metadata"; }
        }

        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; set; }

        public string ProgressText
        {
            get { return Where.Name; }
        }

        public double PercentDone
        {
            get { return Done ? 100 : 0; }
        }

        public long SizeOfWork
        {
            get { return 10000; }
        }

        public string produces
        {
            get { return Where.FullName; }
        }

        private void writeEpisodeDetailsFor(Episode episode, XmlWriter writer)
        {
            // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
            writer.WriteStartElement("episodedetails");

            XMLHelper.WriteElementToXML(writer, "title", episode.Name);
            XMLHelper.WriteElementToXML(writer, "rating", episode.EpisodeRating);
            XMLHelper.WriteElementToXML(writer, "season", episode.SeasonNumber);
            XMLHelper.WriteElementToXML(writer, "episode", episode.EpNum);
            XMLHelper.WriteElementToXML(writer, "plot", episode.Overview);

            writer.WriteStartElement("aired");
            if (episode.FirstAired != null)
                writer.WriteValue(episode.FirstAired.Value.ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            if (Episode.SI != null)
            {
                XMLHelper.WriteElementToXML(writer, "mpaa", Episode.SI.TheSeries().GetRating());
            }

            //Director(s)
            if (!String.IsNullOrEmpty(episode.EpisodeDirector))
            {
                string EpDirector = episode.EpisodeDirector;
                if (!string.IsNullOrEmpty(EpDirector))
                {
                    foreach (string Daa in EpDirector.Split('|'))
                    {
                        if (string.IsNullOrEmpty(Daa))
                            continue;

                        XMLHelper.WriteElementToXML(writer, "director", Daa);
                    }
                }
            }

            //Writers(s)
            if (!String.IsNullOrEmpty(episode.Writer))
            {
                string EpWriter = episode.Writer;
                if (!string.IsNullOrEmpty(EpWriter))
                {
                    XMLHelper.WriteElementToXML(writer, "credits", EpWriter);
                }
            }

            // Guest Stars...
            if (!String.IsNullOrEmpty(episode.EpisodeGuestStars))
            {
                string RecurringActors = "";

                if (Episode.SI != null)
                {
                    RecurringActors = String.Join("|", Episode.SI.TheSeries().GetActors());
                }

                string GuestActors = episode.EpisodeGuestStars;
                if (!string.IsNullOrEmpty(GuestActors))
                {
                    foreach (string Gaa in GuestActors.Split('|'))
                    {
                        if (string.IsNullOrEmpty(Gaa))
                            continue;

                        // Skip if the guest actor is also in the overal recurring list
                        if (!string.IsNullOrEmpty(RecurringActors) && RecurringActors.Contains(Gaa))
                        {
                            continue;
                        }

                        writer.WriteStartElement("actor");
                        XMLHelper.WriteElementToXML(writer, "name", Gaa);
                        writer.WriteEndElement(); // actor
                    }
                }
            }

            // actors...
            if (Episode.SI != null)
            {
                foreach (string aa in Episode.SI.TheSeries().GetActors())
                {
                    if (string.IsNullOrEmpty(aa))
                        continue;

                    writer.WriteStartElement("actor");
                    XMLHelper.WriteElementToXML(writer, "name", aa);
                    writer.WriteEndElement(); // actor
                }
            }

            writer.WriteEndElement(); // episodedetails
        }


        public bool Go(ref bool pause, TVRenameStats stats)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
                
                //Multipart NFO files are not actually valid XML as they have multiple episodeDetails elements
                ConformanceLevel = ConformanceLevel.Fragment
        };
            // "try" and silently fail.  eg. when file is use by other...
            XmlWriter writer;
            try
            {
                //                XmlWriter writer = XmlWriter.Create(this.Where.FullName, settings);
                writer = XmlWriter.Create(Where.FullName, settings);
                if (writer == null)
                    return false;
            }
            catch (Exception)
            {
                Done = true;
                return true;
            }

            if (Episode != null) // specific episode
            {
                if (Episode.type == ProcessedEpisode.ProcessedEpisodeType.merged)
                {
                    foreach (Episode ep in Episode.sourceEpisodes) writeEpisodeDetailsFor(ep, writer);
                }
                else writeEpisodeDetailsFor(Episode, writer);
            }
            else if (SI != null) // show overview (tvshow.nfo)
            {
                // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                writer.WriteStartElement("tvshow");

                XMLHelper.WriteElementToXML(writer,"title",SI.ShowName);

                XMLHelper.WriteElementToXML(writer, "episodeguideurl", TheTVDB.BuildURL(true, true, SI.TVDBCode, TheTVDB.Instance.RequestLanguage));

                XMLHelper.WriteElementToXML(writer, "plot", SI.TheSeries().GetOverview());

                string genre = String.Join(" / ", SI.TheSeries().GetGenres());
                if (!string.IsNullOrEmpty(genre))
                {
                    XMLHelper.WriteElementToXML(writer,"genre",genre);
                }

                XMLHelper.WriteElementToXML(writer, "premiered", SI.TheSeries().GetFirstAired());
                XMLHelper.WriteElementToXML(writer, "year", SI.TheSeries().GetYear());
                XMLHelper.WriteElementToXML(writer, "rating", SI.TheSeries().GetRating());
                XMLHelper.WriteElementToXML(writer, "status", SI.TheSeries().getStatus());

                // actors...
                    foreach (string aa in SI.TheSeries().GetActors() )
                    {
                        if (string.IsNullOrEmpty(aa))
                            continue;

                        writer.WriteStartElement("actor");
                        XMLHelper.WriteElementToXML(writer,"name",aa);
                        writer.WriteEndElement(); // actor
                    }

                XMLHelper.WriteElementToXML(writer, "mpaa", SI.TheSeries().GetRating());
                XMLHelper.WriteInfo(writer, "id", "moviedb","imdb", SI.TheSeries().GetIMDB());

                XMLHelper.WriteElementToXML(writer,"tvdbid",SI.TheSeries().TVDBCode);

                string rt = SI.TheSeries().GetRuntime();
                if (!string.IsNullOrEmpty(rt))
                {
                    XMLHelper.WriteElementToXML(writer,"runtime",rt + " minutes");
                }

                writer.WriteEndElement(); // tvshow
            }

            try
            {
                writer.Close();
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                Error = true;
                Done = true;
                return false;     
            }

            Done = true;
            return true;
        }


        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ActionNFO) && ((o as ActionNFO).Where == Where);
        }

        public int Compare(Item o)
        {
            ActionNFO nfo = o as ActionNFO;

            if (Episode == null)
                return 1;
            if (nfo == null || nfo.Episode == null)
                return -1;
            return (Where.FullName + Episode.Name).CompareTo(nfo.Where.FullName + nfo.Episode.Name);
        }

        #endregion

        #region ScanListItem Members

        public IgnoreItem Ignore
        {
            get
            {
                if (Where == null)
                    return null;
                return new IgnoreItem(Where.FullName);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                if (Episode != null)
                {
                    lvi.Text = Episode.SI.ShowName;
                    lvi.SubItems.Add(Episode.SeasonNumber.ToString());
                    lvi.SubItems.Add(Episode.NumsAsString());
                    DateTime? dt = Episode.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }
                else
                {
                    lvi.Text = SI.ShowName;
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                }

                lvi.SubItems.Add(Where.DirectoryName);
                lvi.SubItems.Add(Where.Name);

                lvi.Tag = this;

                //lv->Items->Add(lvi);
                return lvi;
            }
        }

        string ScanListItem.TargetFolder
        {
            get
            {
                if (Where == null)
                    return null;
                return Where.DirectoryName;
            }
        }

        public string ScanListViewGroup
        {
            get { return "lvgActionMeta"; }
        }

        public int IconNumber
        {
            get { return 7; }
        }

        public ProcessedEpisode Episode { get; private set; }

        #endregion
    }
}
