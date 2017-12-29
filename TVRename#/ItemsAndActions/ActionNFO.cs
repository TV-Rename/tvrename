// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Windows.Forms;
using System.Xml;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public class ActionNfo : Item, IAction, IScanListItem, IActionWriteMetadata
    {
        public ShowItem Si; // if for an entire show, rather than specific episode
        public FileInfo Where;

        public ActionNfo(FileInfo nfo, ProcessedEpisode pe)
        {
            Si = null;
            Episode = pe;
            Where = nfo;
        }

        public ActionNfo(FileInfo nfo, ShowItem si)
        {
            Si = si;
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

        public string Produces
        {
            get { return Where.FullName; }
        }

        private void WriteEpisodeDetailsFor(Episode episode, XmlWriter writer)
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

            if (Episode.Si != null)
            {
                XMLHelper.WriteElementToXML(writer, "mpaa", Episode.Si.TheSeries().GetRating());
            }

            //Director(s)
            if (!String.IsNullOrEmpty(episode.EpisodeDirector))
            {
                string epDirector = episode.EpisodeDirector;
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
            if (!String.IsNullOrEmpty(episode.Writer))
            {
                string epWriter = episode.Writer;
                if (!string.IsNullOrEmpty(epWriter))
                {
                    XMLHelper.WriteElementToXML(writer, "credits", epWriter);
                }
            }

            // Guest Stars...
            if (!String.IsNullOrEmpty(episode.EpisodeGuestStars))
            {
                string recurringActors = "";

                if (Episode.Si != null)
                {
                    recurringActors = String.Join("|", Episode.Si.TheSeries().GetActors());
                }

                string guestActors = episode.EpisodeGuestStars;
                if (!string.IsNullOrEmpty(guestActors))
                {
                    foreach (string gaa in guestActors.Split('|'))
                    {
                        if (string.IsNullOrEmpty(gaa))
                            continue;

                        // Skip if the guest actor is also in the overal recurring list
                        if (!string.IsNullOrEmpty(recurringActors) && recurringActors.Contains(gaa))
                        {
                            continue;
                        }

                        writer.WriteStartElement("actor");
                        XMLHelper.WriteElementToXML(writer, "name", gaa);
                        writer.WriteEndElement(); // actor
                    }
                }
            }

            // actors...
            if (Episode.Si != null)
            {
                foreach (string aa in Episode.Si.TheSeries().GetActors())
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
            }
            catch (Exception)
            {
                Done = true;
                return true;
            }

            if (Episode != null) // specific episode
            {
                if (Episode.Type == ProcessedEpisode.ProcessedEpisodeType.Merged)
                {
                    foreach (Episode ep in Episode.SourceEpisodes) WriteEpisodeDetailsFor(ep, writer);
                }
                else WriteEpisodeDetailsFor(Episode, writer);
            }
            else if (Si != null) // show overview (tvshow.nfo)
            {
                // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                writer.WriteStartElement("tvshow");

                XMLHelper.WriteElementToXML(writer,"title",Si.ShowName);

                XMLHelper.WriteElementToXML(writer, "episodeguideurl", TheTVDB.BuildUrl(true, true, Si.TVDBCode, TheTVDB.Instance.RequestLanguage));

                XMLHelper.WriteElementToXML(writer, "plot", Si.TheSeries().GetOverview());

                string genre = String.Join(" / ", Si.TheSeries().GetGenres());
                if (!string.IsNullOrEmpty(genre))
                {
                    XMLHelper.WriteElementToXML(writer,"genre",genre);
                }

                XMLHelper.WriteElementToXML(writer, "premiered", Si.TheSeries().GetFirstAired());
                XMLHelper.WriteElementToXML(writer, "year", Si.TheSeries().GetYear());
                XMLHelper.WriteElementToXML(writer, "rating", Si.TheSeries().GetRating());
                XMLHelper.WriteElementToXML(writer, "status", Si.TheSeries().GetStatus());

                // actors...
                    foreach (string aa in Si.TheSeries().GetActors() )
                    {
                        if (string.IsNullOrEmpty(aa))
                            continue;

                        writer.WriteStartElement("actor");
                        XMLHelper.WriteElementToXML(writer,"name",aa);
                        writer.WriteEndElement(); // actor
                    }

                XMLHelper.WriteElementToXML(writer, "mpaa", Si.TheSeries().GetRating());
                XMLHelper.WriteInfo(writer, "id", "moviedb","imdb", Si.TheSeries().GetImdb());

                XMLHelper.WriteElementToXML(writer,"tvdbid",Si.TheSeries().TVDBCode);

                string rt = Si.TheSeries().GetRuntime();
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
            return (o is ActionNfo) && ((o as ActionNfo).Where == Where);
        }

        public int Compare(Item o)
        {
            ActionNfo nfo = o as ActionNfo;

            if (Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            return String.Compare((Where.FullName + Episode.Name), nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
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
                    lvi.Text = Episode.Si.ShowName;
                    lvi.SubItems.Add(Episode.SeasonNumber.ToString());
                    lvi.SubItems.Add(Episode.NumsAsString());
                    DateTime? dt = Episode.GetAirDateDt(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }
                else
                {
                    lvi.Text = Si.ShowName;
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

        string IScanListItem.TargetFolder
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
