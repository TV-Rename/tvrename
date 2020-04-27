// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    using System;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
    using System.Xml;

    public class ActionWdtvMeta : ActionWriteMetadata
    {
        public ActionWdtvMeta(FileInfo where, ProcessedEpisode pe) :base(where,null)
        {
            Episode = pe;
        }

        public ActionWdtvMeta(FileInfo where, ShowItem si) : base(where, si)
        {
            Episode = null;
        }
        #region Action Members

        [NotNull]
        public override string Name => "Write WD TV Live Hub Meta";

        [NotNull]
        public override ActionOutcome Go(TVRenameStats stats)
        {
            if (Where is null)
            {
                return new ActionOutcome("No file location specified - Development Error");
            }

            if (Episode != null)
            {
                return WriteEpisodeMetaDataFile();
            }

            if (SelectedShow != null)
            {
                return WriteSeriesXml();
            }

            return new ActionOutcome("No details available to write - Development Error");
        }

        [NotNull]
        private ActionOutcome WriteSeriesXml()
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
                    writer.WriteStartElement("details");
                    writer.WriteStartElement("show");

                    writer.WriteElement("title", SelectedShow.ShowName);
                    writer.WriteElement("premiered", SelectedShow.TheSeries()?.FirstAired);
                    writer.WriteElement("year", SelectedShow.TheSeries()?.Year);
                    writer.WriteElement("status", SelectedShow.TheSeries()?.Status);
                    writer.WriteElement("mpaa", SelectedShow.TheSeries()?.ContentRating);
                    writer.WriteElement("tvdbid", SelectedShow.TheSeries()?.TvdbCode);
                    writer.WriteElement("plot", SelectedShow.TheSeries()?.Overview);

                    foreach (string genre in SelectedShow.Genres)
                    {
                        writer.WriteElement("genre", genre);
                    }

                    float siteRating =SelectedShow.TheSeries()?.SiteRating??0 * 10;

                    int intSiteRating = (int)siteRating;
                    if (intSiteRating > 0)
                    {
                        writer.WriteElement("rating", intSiteRating);
                    }

                    writer.WriteInfo("moviedb", "imdb", "id", SelectedShow.TheSeries()?.Imdb);

                    string rt = SelectedShow.TheSeries()?.Runtime;
                    if (!string.IsNullOrEmpty(rt))
                    {
                        writer.WriteElement("runtime", rt + " min");
                    }

                    writer.WriteEndElement(); // show
                    writer.WriteEndElement(); // tvshow
                }
                return ActionOutcome.Success();
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }
        }

        [NotNull]
        private ActionOutcome WriteEpisodeMetaDataFile()
        {
            // "try" and silently fail.  eg. when file is use by other...
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true
                };

                using (XmlWriter writer = XmlWriter.Create(Where.FullName, settings))
                {
                    writer.WriteStartElement("details");
                    writer.WriteElement("title", TVSettings.Instance.NamingStyle.NameFor(Episode));
                    writer.WriteElement("mpaa", Episode.TheSeries.ContentRating);

                    if (Episode.FirstAired.HasValue)
                    {
                        writer.WriteElement("year", Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                        writer.WriteElement("firstaired",
                            Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                    }

                    writer.WriteElement("runtime", Episode.TheSeries.Runtime, true);
                    writer.WriteElement("rating", Episode.EpisodeRating);
                    writer.WriteElement("studio", Episode.TheSeries.Network);
                    writer.WriteElement("plot", Episode.TheSeries.Overview);
                    writer.WriteElement("overview", Episode.Overview);
                    foreach (string director in Episode.Directors)
                    {
                        writer.WriteElement("directors", director);
                    }

                    foreach(string epwriter in Episode.Writers)
                    {
                        writer.WriteElement("writers", epwriter);
                    }

                    foreach (string genre in Episode.TheSeries.Genres)
                    {
                        writer.WriteElement("genre", genre);
                    }

                    // actors...
                    foreach (Actor aa in Episode.TheSeries.GetActors().Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
                    {
                        writer.WriteStartElement("actor");
                        writer.WriteElement("name", aa.ActorName);
                        writer.WriteElement("role", aa.ActorRole);
                        writer.WriteEndElement(); // actor
                    }

                    // guest stars...
                    foreach(string guest in Episode.GuestStars)
                    {
                        writer.WriteElement("guest", guest);
                    }

                    writer.WriteElement("thumbnail", TheTVDB.API.GetImageURL(Episode.Filename));
                    writer.WriteElement("banner", TheTVDB.API.GetImageURL(Episode.AppropriateProcessedSeason.GetWideBannerPath()));
                    writer.WriteElement("backdrop", TheTVDB.API.GetImageURL(Episode.TheSeries.GetSeriesFanartPath()));
                    writer.WriteEndElement(); // details
                }
                return ActionOutcome.Success();
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o) => o is ActionWdtvMeta meta && meta.Where == Where;

        public override int CompareTo(object o)
        {
            ActionWdtvMeta nfo = o as ActionWdtvMeta;

            if (Episode is null)
            {
                return 1;
            }

            if (nfo?.Episode is null)
            {
                return -1;
            }

            return string.Compare(Where.FullName + Episode.Name, nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
        #endregion
    }
}
