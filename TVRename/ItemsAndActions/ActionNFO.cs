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
    using System.Xml;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

    public class ActionNfo : ActionWriteMetadata
    {
        public ActionNfo(FileInfo nfo, ProcessedEpisode pe) : base(nfo, null)
        {
            Episode = pe;
        }

        public ActionNfo(FileInfo nfo, ShowItem si) : base(nfo, si)
        {
            Episode = null;
        }

        #region Action Members

        public override string Name => "Write KODI Metadata";

        private void WriteEpisodeDetailsFor(Episode episode, XmlWriter writer,bool multi,bool dvdOrder)
        {
            // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
            writer.WriteStartElement("episodedetails");

            XmlHelper.WriteElementToXml(writer, "title", episode.Name);
            XmlHelper.WriteElementToXml(writer,"showtitle", Episode.Show.ShowName );
            XmlHelper.WriteElementToXml(writer, "rating", episode.EpisodeRating);
            if (dvdOrder)
            {
                XmlHelper.WriteElementToXml(writer, "season", episode.DvdSeasonNumber);
                XmlHelper.WriteElementToXml(writer, "episode", episode.DvdEpNum);
            }
            else
            {
                XmlHelper.WriteElementToXml(writer, "season", episode.AiredSeasonNumber);
                XmlHelper.WriteElementToXml(writer, "episode", episode.AiredEpNum);
            }

            XmlHelper.WriteElementToXml(writer, "plot", episode.Overview);

            writer.WriteStartElement("aired");
            if (episode.FirstAired != null)
                writer.WriteValue(episode.FirstAired.Value.ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            XmlHelper.WriteElementToXml(writer, "mpaa", Episode.Show?.TheSeries()?.GetContentRating(),true);

            //Director(s)
            string epDirector = episode.EpisodeDirector;
            if (!string.IsNullOrEmpty(epDirector))
            {
                foreach (string daa in epDirector.Split('|'))
                {
                    XmlHelper.WriteElementToXml(writer, "director", daa,true);
                }
            }

            //Writers(s)
            string epWriter = episode.Writer;
            if (!string.IsNullOrEmpty(epWriter))
            {
                foreach (string txtWriter in epWriter.Split('|'))
                {
                    XmlHelper.WriteElementToXml(writer, "credits", txtWriter, true);
                }
            }

            // Guest Stars...
            if (!string.IsNullOrEmpty(episode.EpisodeGuestStars))
            {
                string recurringActors = "";

                if (Episode.Show != null)
                {
                    recurringActors = string.Join("|", Episode.Show.TheSeries().GetActorNames());
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
                        XmlHelper.WriteElementToXml(writer, "name", gaa);
                        writer.WriteEndElement(); // actor
                    }
                }
            }

            // actors...
            if (Episode.Show != null)
            {
                foreach (Actor aa in Episode.Show.TheSeries().GetActors())
                {
                    if (string.IsNullOrEmpty(aa.ActorName))
                        continue;

                    writer.WriteStartElement("actor");
                    XmlHelper.WriteElementToXml(writer, "name", aa.ActorName);
                    XmlHelper.WriteElementToXml(writer, "role", aa.ActorRole);
                    XmlHelper.WriteElementToXml(writer, "order", aa.ActorSortOrder);
                    XmlHelper.WriteElementToXml(writer, "thumb", aa.ActorImage);
                    writer.WriteEndElement(); // actor
                }
            }

            if (multi)
            {
                writer.WriteStartElement("resume");
                //we have to put 0 as we don't know where the multipart episode starts/ends
                XmlHelper.WriteElementToXml(writer, "position", 0);
                XmlHelper.WriteElementToXml(writer, "total", 0);
                writer.WriteEndElement(); // resume

                //For now we only put art in for multipart episodes. Kodi finds the art appropriately
                //without our help for the others

                ShowItem episodeSi = Episode.Show??SelectedShow;
                string filename =
                    TVSettings.Instance.FilenameFriendly(
                        TVSettings.Instance.NamingStyle.GetTargetEpisodeName(episode,episodeSi.ShowName, episodeSi.GetTimeZone(), episodeSi.DvdOrder));

                string thumbFilename =  filename + ".jpg";
                XmlHelper.WriteElementToXml(writer, "thumb",thumbFilename);
                //Should be able to do this using the local filename, but only seems to work if you provide a URL
                //XMLHelper.WriteElementToXML(writer, "thumb", TheTVDB.Instance.GetTVDBDownloadURL(episode.GetFilename()))
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
            try
            {
                // "try" and silently fail.  eg. when file is use by other...
                using (XmlWriter writer = XmlWriter.Create(Where.FullName, settings))
                {
                    if (Episode != null) // specific episode
                    {
                        if (Episode.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
                        {
                            foreach (Episode ep in Episode.SourceEpisodes)
                                WriteEpisodeDetailsFor(ep, writer, true, Episode.Show.DvdOrder);
                        }
                        else WriteEpisodeDetailsFor(Episode, writer, false, Episode.Show.DvdOrder);
                    }
                    else if (SelectedShow != null) // show overview (tvshow.nfo)
                    {
                        // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows
                        writer.WriteStartElement("tvshow");

                        XmlHelper.WriteElementToXml(writer, "title", SelectedShow.ShowName);

                        XmlHelper.WriteElementToXml(writer, "episodeguideurl",
                            TheTVDB.BuildUrl(SelectedShow.TvdbCode, TVSettings.Instance.PreferredLanguage));

                        XmlHelper.WriteElementToXml(writer, "plot", SelectedShow.TheSeries().GetOverview());

                        string genre = string.Join(" / ", SelectedShow.TheSeries().GetGenres());
                        if (!string.IsNullOrEmpty(genre))
                        {
                            XmlHelper.WriteElementToXml(writer, "genre", genre);
                        }

                        XmlHelper.WriteElementToXml(writer, "premiered", SelectedShow.TheSeries().GetFirstAired());
                        XmlHelper.WriteElementToXml(writer, "year", SelectedShow.TheSeries().GetYear());
                        XmlHelper.WriteElementToXml(writer, "rating", SelectedShow.TheSeries().GetContentRating());
                        XmlHelper.WriteElementToXml(writer, "status", SelectedShow.TheSeries().GetStatus());

                        // actors...
                        foreach (Actor aa in SelectedShow.TheSeries().GetActors())
                        {
                            if (string.IsNullOrEmpty(aa.ActorName))
                                continue;

                            writer.WriteStartElement("actor");
                            XmlHelper.WriteElementToXml(writer, "name", aa.ActorName);
                            XmlHelper.WriteElementToXml(writer, "role", aa.ActorRole);
                            XmlHelper.WriteElementToXml(writer, "order", aa.ActorSortOrder);
                            XmlHelper.WriteElementToXml(writer, "thumb", aa.ActorImage);
                            writer.WriteEndElement(); // actor
                        }

                        XmlHelper.WriteElementToXml(writer, "mpaa", SelectedShow.TheSeries().GetContentRating());
                        XmlHelper.WriteInfo(writer, "id", "moviedb", "imdb", SelectedShow.TheSeries().GetImdb());

                        XmlHelper.WriteElementToXml(writer, "tvdbid", SelectedShow.TheSeries().TvdbCode);

                        string rt = SelectedShow.TheSeries().GetRuntime();
                        if (!string.IsNullOrEmpty(rt))
                        {
                            XmlHelper.WriteElementToXml(writer, "runtime", rt + " minutes");
                        }
                        writer.WriteEndElement(); // tvshow
                    }
                }
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
        public override bool SameAs(Item o)
        {
            return (o is ActionNfo nfo) && (nfo.Where == Where);
        }

        public override int Compare(Item o)
        {
            ActionNfo nfo = o as ActionNfo;

            if (Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            return string.Compare((Where.FullName + Episode.Name), nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
        #endregion
    }
}
