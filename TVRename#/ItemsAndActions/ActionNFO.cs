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

    public class ActionNFO : ActionWriteMetadata
    {
        public ShowItem SI; // if for an entire show, rather than specific episode

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

        public override string Name => "Write KODI Metadata";

        private void WriteEpisodeDetailsFor(Episode episode, XmlWriter writer,bool multi,bool dvdOrder)
        {
            // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
            writer.WriteStartElement("episodedetails");

            XMLHelper.WriteElementToXML(writer, "title", episode.Name);
            XMLHelper.WriteElementToXML(writer,"showtitle", this.Episode.SI.ShowName );
            XMLHelper.WriteElementToXML(writer, "rating", episode.EpisodeRating);
            if (dvdOrder)
            {
                XMLHelper.WriteElementToXML(writer, "season", episode.DVDSeasonNumber);
                XMLHelper.WriteElementToXML(writer, "episode", episode.DVDEpNum);
            }
            else
            {
                XMLHelper.WriteElementToXML(writer, "season", episode.AiredSeasonNumber);
                XMLHelper.WriteElementToXML(writer, "episode", episode.AiredEpNum);

            }

            XMLHelper.WriteElementToXML(writer, "plot", episode.Overview);

            writer.WriteStartElement("aired");
            if (episode.FirstAired != null)
                writer.WriteValue(episode.FirstAired.Value.ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            XMLHelper.WriteElementToXML(writer, "mpaa", this.Episode.SI?.TheSeries()?.GetRating(),true);

            //Director(s)
            string epDirector = episode.EpisodeDirector;
            if (!string.IsNullOrEmpty(epDirector))
            {
                foreach (string daa in epDirector.Split('|'))
                {
                    XMLHelper.WriteElementToXML(writer, "director", daa,true);
                }
            }

            //Writers(s)
            string epWriter = episode.Writer;
            if (!string.IsNullOrEmpty(epWriter))
            {
                foreach (string txtWriter in epWriter.Split('|'))
                {
                    XMLHelper.WriteElementToXML(writer, "credits", txtWriter, true);
                }
            }

            // Guest Stars...
            if (!String.IsNullOrEmpty(episode.EpisodeGuestStars))
            {
                string recurringActors = "";

                if (this.Episode.SI != null)
                {
                    recurringActors = String.Join("|", this.Episode.SI.TheSeries().GetActors());
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
            if (this.Episode.SI != null)
            {
                foreach (string aa in this.Episode.SI.TheSeries().GetActors())
                {
                    if (string.IsNullOrEmpty(aa))
                        continue;

                    writer.WriteStartElement("actor");
                    XMLHelper.WriteElementToXML(writer, "name", aa);
                    writer.WriteEndElement(); // actor
                }
            }

            if (multi)
            {
                writer.WriteStartElement("resume");
                //we have to put 0 as we don't know where the multipart episode starts/ends
                XMLHelper.WriteElementToXML(writer, "position", 0);
                XMLHelper.WriteElementToXML(writer, "total", 0);
                writer.WriteEndElement(); // resume

                //For now we only put art in for multipart episodes. Kodi finds the art appropriately
                //without our help for the others

                ShowItem episodeSi = this.Episode.SI??this.SI;
                string filename =
                    TVSettings.Instance.FilenameFriendly(
                        TVSettings.Instance.NamingStyle.GetTargetEpisodeName(episode,episodeSi.ShowName, episodeSi.GetTimeZone(), episodeSi.DVDOrder));

                string thumbFilename =  filename + ".jpg";
                XMLHelper.WriteElementToXML(writer, "thumb",thumbFilename);
                //Should be able to do this using the local filename, but only seems to work if you provide a URL
                //XMLHelper.WriteElementToXML(writer, "thumb", TheTVDB.Instance.GetTVDBDownloadURL(episode.GetFilename()));


            }
            writer.WriteEndElement(); // episodedetails
        }

        public override bool Go(ref bool pause, TVRenameStats stats)
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
                writer = XmlWriter.Create(this.Where.FullName, settings);
            }
            catch (Exception)
            {
                this.Done = true;
                return true;
            }

            if (this.Episode != null) // specific episode
            {
                if (this.Episode.type == ProcessedEpisode.ProcessedEpisodeType.merged)
                {
                    foreach (Episode ep in this.Episode.sourceEpisodes) WriteEpisodeDetailsFor(ep, writer, true, this.Episode.SI.DVDOrder);
                }
                else WriteEpisodeDetailsFor(this.Episode, writer, false, this.Episode.SI.DVDOrder);
            }
            else if (this.SI != null) // show overview (tvshow.nfo)
            {
                // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                writer.WriteStartElement("tvshow");

                XMLHelper.WriteElementToXML(writer,"title",this.SI.ShowName);

                XMLHelper.WriteElementToXML(writer, "episodeguideurl", TheTVDB.BuildURL(true, true, this.SI.TVDBCode, TheTVDB.Instance.RequestLanguage));

                XMLHelper.WriteElementToXML(writer, "plot", this.SI.TheSeries().GetOverview());

                string genre = String.Join(" / ", this.SI.TheSeries().GetGenres());
                if (!string.IsNullOrEmpty(genre))
                {
                    XMLHelper.WriteElementToXML(writer,"genre",genre);
                }

                XMLHelper.WriteElementToXML(writer, "premiered", this.SI.TheSeries().GetFirstAired());
                XMLHelper.WriteElementToXML(writer, "year", this.SI.TheSeries().GetYear());
                XMLHelper.WriteElementToXML(writer, "rating", this.SI.TheSeries().GetRating());
                XMLHelper.WriteElementToXML(writer, "status", this.SI.TheSeries().getStatus());

                // actors...
                    foreach (string aa in this.SI.TheSeries().GetActors() )
                    {
                        if (string.IsNullOrEmpty(aa))
                            continue;

                        writer.WriteStartElement("actor");
                        XMLHelper.WriteElementToXML(writer,"name",aa);
                        writer.WriteEndElement(); // actor
                    }

                XMLHelper.WriteElementToXML(writer, "mpaa", this.SI.TheSeries().GetRating());
                XMLHelper.WriteInfo(writer, "id", "moviedb","imdb", this.SI.TheSeries().GetIMDB());

                XMLHelper.WriteElementToXML(writer,"tvdbid",this.SI.TheSeries().TVDBCode);

                string rt = this.SI.TheSeries().GetRuntime();
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
                this.ErrorText = e.Message;
                this.Error = true;
                this.Done = true;
                return false;     
            }

            this.Done = true;
            return true;
        }


        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ActionNFO) && ((o as ActionNFO).Where == this.Where);
        }

        public override int Compare(Item o)
        {
            ActionNFO nfo = o as ActionNFO;

            if (this.Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            return (this.Where.FullName + this.Episode.Name).CompareTo(nfo.Where.FullName + nfo.Episode.Name);
        }

        #endregion

        #region Item Members

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



        #endregion
    }
}
