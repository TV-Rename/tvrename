// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

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

        [NotNull]
        public override string Name => "Write KODI Metadata";

        private void WriteEpisodeDetailsFor([NotNull] Episode episode, [NotNull] XmlWriter writer,bool multi,bool dvdOrder)
        {
            // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
            writer.WriteStartElement("episodedetails");

            writer.WriteElement("title", episode.Name);
            writer.WriteElement("showtitle", Episode.Show.ShowName );
            writer.WriteElement("rating", episode.EpisodeRating);
            if (dvdOrder)
            {
                writer.WriteElement("season", episode.DvdSeasonNumber);
                writer.WriteElement("episode", episode.DvdEpNum);
            }
            else
            {
                writer.WriteElement("season", episode.AiredSeasonNumber);
                writer.WriteElement("episode", episode.AiredEpNum);
            }

            writer.WriteElement("plot", episode.Overview);

            writer.WriteStartElement("aired");
            if (episode.FirstAired != null)
            {
                writer.WriteValue(episode.FirstAired.Value.ToString("yyyy-MM-dd"));
            }

            writer.WriteEndElement();

            writer.WriteElement("mpaa", Episode.Show?.TheSeries()?.ContentRating,true);

            //Director(s)
            string epDirector = episode.EpisodeDirector;
            if (!string.IsNullOrEmpty(epDirector))
            {
                foreach (string daa in epDirector.Split('|'))
                {
                    writer.WriteElement("director", daa,true);
                }
            }

            //Writers(s)
            string epWriter = episode.Writer;
            if (!string.IsNullOrEmpty(epWriter))
            {
                foreach (string txtWriter in epWriter.Split('|'))
                {
                    writer.WriteElement("credits", txtWriter, true);
                }
            }

            // Guest Stars...
            if (!string.IsNullOrEmpty(episode.EpisodeGuestStars))
            {
                string recurringActors = "";

                if (Episode.Show != null)
                {
                    recurringActors = string.Join("|", Episode.Show.TheSeries()?.GetActorNames()??new List<string>());
                }

                string guestActors = episode.EpisodeGuestStars;
                if (!string.IsNullOrEmpty(guestActors))
                {
                    foreach (string gaa in guestActors.Split('|')
                        .Where(gaa => !string.IsNullOrEmpty(gaa))
                        .Where(gaa => string.IsNullOrEmpty(recurringActors) || !recurringActors.Contains(gaa)))
                    {
                        writer.WriteStartElement("actor");
                        writer.WriteElement("name", gaa);
                        writer.WriteEndElement(); // actor
                    }
                }
            }

            // actors...
            if (Episode.Show != null)
            {
                foreach (Actor aa in (Episode.Show.TheSeries()?.GetActors()??new List<Actor>())
                    .Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
                {
                    writer.WriteStartElement("actor");
                    writer.WriteElement("name", aa.ActorName);
                    writer.WriteElement("role", aa.ActorRole);
                    writer.WriteElement("order", aa.ActorSortOrder);
                    writer.WriteElement("thumb", aa.ActorImage);
                    writer.WriteEndElement(); // actor
                }
            }

            if (multi)
            {
                writer.WriteStartElement("resume");
                //we have to put 0 as we don't know where the multipart episode starts/ends
                writer.WriteElement("position", 0);
                writer.WriteElement("total", 0);
                writer.WriteEndElement(); // resume

                //For now we only put art in for multipart episodes. Kodi finds the art appropriately
                //without our help for the others

                ShowItem episodeSi = Episode.Show??SelectedShow;
                string filename =
                    TVSettings.Instance.FilenameFriendly(
                        TVSettings.Instance.NamingStyle.GetTargetEpisodeName(episodeSi,episode,episodeSi.GetTimeZone(), episodeSi.DvdOrder));

                string thumbFilename =  filename + ".jpg";
                writer.WriteElement("thumb",thumbFilename);
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
                IndentChars = "    ",
                Encoding = Encoding.UTF8,
                NewLineChars = "\r\n",
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
                            {
                                WriteEpisodeDetailsFor(ep, writer, true, Episode.Show.DvdOrder);
                            }
                        }
                        else
                        {
                            WriteEpisodeDetailsFor(Episode, writer, false, Episode.Show.DvdOrder);
                        }
                    }
                    else if (SelectedShow != null) // show overview (tvshow.nfo)
                    {
                        SeriesInfo series = SelectedShow.TheSeries();

                        // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows
                        writer.WriteStartElement("tvshow");

                        writer.WriteElement("title", SelectedShow.ShowName);
                        writer.WriteElement("originaltitle", series?.Name);

                        float? showRating = series?.SiteRating;
                        if (showRating.HasValue)
                        {
                            writer.WriteStartElement("ratings");

                            writer.WriteStartElement("rating");
                            writer.WriteAttributeString("name", "tvdb");
                            writer.WriteAttributeString("max", "10");
                            writer.WriteAttributeString("default", "true");

                            writer.WriteElement("value", showRating.Value);
                            writer.WriteElement("votes", series.SiteRatingVotes, true);

                            writer.WriteEndElement();//rating

                            writer.WriteEndElement();//ratings
                        }

                        string lang = TVSettings.Instance.PreferredLanguageCode;
                        if (SelectedShow.UseCustomLanguage && SelectedShow.PreferredLanguage != null)
                        {
                            lang = SelectedShow.PreferredLanguage.Abbreviation;
                        }

                        writer.WriteElement("episodeguideurl",
                            TheTVDB.BuildUrl(SelectedShow.TvdbCode, lang));

                        if (!(series is null))
                        {
                            writer.WriteElement("id", series.SeriesId);
                            writer.WriteElement("runtime", series.Runtime, true);
                            writer.WriteElement("mpaa", series.ContentRating,true);

                            writer.WriteStartElement("uniqueid");
                            writer.WriteAttributeString("type", "tvdb");
                            writer.WriteAttributeString("default", "true");
                            writer.WriteValue(series.TvdbCode);
                            writer.WriteEndElement();

                            writer.WriteStartElement("uniqueid");
                            writer.WriteAttributeString("type", "imdb");
                            writer.WriteAttributeString("default", "false");
                            writer.WriteValue(series.Imdb);
                            writer.WriteEndElement();

                            writer.WriteElement("plot", series.Overview);

                            writer.WriteElement("premiered", series.FirstAired);
                            writer.WriteElement("year", series.Year);
                            writer.WriteElement("status", series.Status);
                        }

                        writer.WriteStringsToXml("genre", SelectedShow.Genres);

                        // actors...
                        foreach (Actor aa in SelectedShow.Actors.Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
                        {
                            writer.WriteStartElement("actor");
                            writer.WriteElement("name", aa.ActorName);
                            writer.WriteElement("role", aa.ActorRole);
                            writer.WriteElement("order", aa.ActorSortOrder);
                            writer.WriteElement("thumb", aa.ActorImage);
                            writer.WriteEndElement(); // actor
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

            if (Episode is null)
            {
                return 1;
            }

            if (nfo?.Episode is null)
            {
                return -1;
            }

            return string.Compare((Where.FullName + Episode.Name), nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
        #endregion
    }
}
