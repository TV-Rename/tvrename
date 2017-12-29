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
    public class ActionMede8ErXML : ITem, IAction, IScanListItem, IActionWriteMetadata
    {
           public ShowItem Si; // if for an entire show, rather than specific episode
        public FileInfo Where;

        public ActionMede8ErXML(FileInfo nfo, ProcessedEpisode pe)
        {
            Si = null;
            Episode = pe;
            Where = nfo;
        }

        public ActionMede8ErXML(FileInfo nfo, ShowItem si)
        {
            Si = si;
            Episode = null;
            Where = nfo;
        }

        public string Produces
        {
            get { return Where.FullName; }
        }

        #region Action Members

        public string Name
        {
            get { return "Write Mede8er Metadata"; }
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

        public bool Go(ref bool pause, TVRenameStats stats)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
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
                // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
                writer.WriteStartElement("details");
                writer.WriteStartElement("movie");
                XMLHelper.WriteElementToXML(writer,"title",Episode.Name);
                XMLHelper.WriteElementToXML(writer,"season",Episode.SeasonNumber);
                XMLHelper.WriteElementToXML(writer,"episode",Episode.EpNum);

                writer.WriteStartElement("year");
                if (Episode.FirstAired != null)
                    writer.WriteValue(Episode.FirstAired.Value.ToString("yyyy"));
                writer.WriteEndElement();

                writer.WriteStartElement("rating");
                string rating = (Episode.EpisodeRating);
                if (!string.IsNullOrEmpty(rating))
                {
                    rating = rating.Trim('.');
                    rating = rating.Replace(".", "");
                    writer.WriteValue(rating);
                }
                writer.WriteEndElement();  // rating

                //Get the Series OverView
                string sov = Episode.Si.TheSeries().GetOverview();
                if (!string.IsNullOrEmpty(sov))
                {
                    XMLHelper.WriteElementToXML(writer,"plot",sov);
                }

                //Get the Episode overview
                XMLHelper.WriteElementToXML(writer,"episodeplot",Episode.Overview);
           
                if (Episode.Si != null)
                {
                    WriteInfo(writer, Episode.Si.TheSeries().GetRating(), "mpaa");
                }

                //Runtime...taken from overall Series, not episode specific due to thetvdb
                string rt = Episode.Si.TheSeries().GetRuntime();
                if (!string.IsNullOrEmpty(rt))
                {
                    XMLHelper.WriteElementToXML(writer,"runtime",rt + " min");
                }

                //Genres...taken from overall Series, not episode specific due to thetvdb
                writer.WriteStartElement("genres");
                string genre = String.Join(" / ", Episode.Si.TheSeries().GetGenres());
                if (!string.IsNullOrEmpty(genre))
                {
                    XMLHelper.WriteElementToXML(writer,"genre",genre);
                }
                writer.WriteEndElement();  // genres

                //Director(s)
                if (!String.IsNullOrEmpty(Episode.EpisodeDirector))
                {
                    string epDirector = Episode.EpisodeDirector;
                    if (!string.IsNullOrEmpty(epDirector))
                    {
                        foreach (string daa in epDirector.Split('|'))
                        {
                            if (string.IsNullOrEmpty(daa))
                                continue;

                            XMLHelper.WriteElementToXML(writer,"director",daa);
                        }
                    }
                }

                //Writers(s)
                if (!String.IsNullOrEmpty(Episode.Writer))
                {
                    string epWriter = Episode.Writer;
                    if (!string.IsNullOrEmpty(epWriter))
                    {
                        XMLHelper.WriteElementToXML(writer,"credits",epWriter);
                    }
                }

               
                writer.WriteStartElement("cast");

                // actors...
                if (Episode.Si != null)
                {
                        foreach (string aa in Episode.Si.TheSeries().GetActors())
                        {
                            if (string.IsNullOrEmpty(aa))
                                continue;

                            XMLHelper.WriteElementToXML(writer,"actor",aa);
                        }
                }

                writer.WriteEndElement(); // cast
                writer.WriteEndElement(); // movie
                writer.WriteEndElement(); // details
            }
            else if (Si != null) // show overview (Series.xml)
            {
                // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                writer.WriteStartElement("details");
                writer.WriteStartElement("movie");

                XMLHelper.WriteElementToXML(writer,"title",Si.ShowName);
              
                writer.WriteStartElement("genres");
                string genre = String.Join(" / ", Si.TheSeries().GetGenres());
                if (!string.IsNullOrEmpty(genre))
                {
                    XMLHelper.WriteElementToXML(writer,"genre",genre);
                }
                writer.WriteEndElement();  // genres

                WriteInfo(writer, Si.TheSeries().GetFirstAired(), "premiered");
                WriteInfo(writer, Si.TheSeries().GetYear(), "year");

                writer.WriteStartElement("rating");
                string rating = Si.TheSeries().GetRating();
                if (!string.IsNullOrEmpty(rating))
                {
                    rating = rating.Trim('.');
                    rating = rating.Replace(".", "");
                    writer.WriteValue(rating);
                }
                writer.WriteEndElement();  // rating

                WriteInfo(writer, Si.TheSeries().GetStatus(), "status");

                WriteInfo(writer, Si.TheSeries().GetRating(), "mpaa");
                WriteInfo(writer, Si.TheSeries().GetImdb(), "id", "moviedb", "imdb");

                XMLHelper.WriteElementToXML(writer,"tvdbid",Si.TheSeries().TVDBCode);

                string rt = Si.TheSeries().GetRuntime();
                if (!string.IsNullOrEmpty(rt))
                {
                    XMLHelper.WriteElementToXML(writer,"runtime",rt + " min");
                }

                WriteInfo(writer, Si.TheSeries().GetOverview(), "plot");
                
                writer.WriteStartElement("cast");

                // actors...
                
                foreach (string aa in Si.TheSeries().GetActors())
                {
                    if (string.IsNullOrEmpty(aa))
                        continue;
                    XMLHelper.WriteElementToXML(writer,"actor",aa);
                }
                
                writer.WriteEndElement(); // cast
                writer.WriteEndElement(); // movie
                writer.WriteEndElement(); // tvshow
            }

            writer.Close();
            Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public bool SameAs(ITem o)
        {
            return (o is ActionMede8ErXML) && ((o as ActionMede8ErXML).Where == Where);
        }

        public int Compare(ITem o)
        {
            ActionMede8ErXML nfo = o as ActionMede8ErXML;

            if (Episode == null)
                return 1;
            if (nfo?.Episode == null)
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

        private static void WriteInfo(XmlWriter writer, string value, string elemName)
        {
            WriteInfo(writer, value, elemName, null, null);
        }

        private static void WriteInfo(XmlWriter writer, string value, string elemName, string attribute, string attributeVal)
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
