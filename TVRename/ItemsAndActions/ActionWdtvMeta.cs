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
        public ActionWdtvMeta(FileInfo where, ProcessedEpisode pe) :base(where,pe.Show)
        {
            Episode = pe;
        }

        public ActionWdtvMeta(FileInfo where, ShowConfiguration si) : base(where, si)
        {
            Episode = null;
        }
        #region Action Members

        public override string Name => "Write WD TV Live Hub Meta";

        public override ActionOutcome Go(TVRenameStats stats)
        {
            return Episode != null ? WriteEpisodeMetaDataFile() :
                SelectedShow != null ? WriteSeriesXml() :
                ActionOutcome.Success();
                //todo WDTV Movie support WriteMovieXml();
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

                    writer.WriteElement("title", SelectedShow!.ShowName);
                    writer.WriteElement("premiered", SelectedShow.CachedShow?.FirstAired);
                    writer.WriteElement("year", SelectedShow.CachedShow?.Year);
                    writer.WriteElement("status", SelectedShow.CachedShow?.Status);
                    writer.WriteElement("mpaa", SelectedShow.CachedShow?.ContentRating);
                    writer.WriteElement("tvdbid", SelectedShow.CachedShow?.TvdbCode);
                    writer.WriteElement("plot", SelectedShow.CachedShow?.Overview);

                    foreach (string genre in SelectedShow.Genres)
                    {
                        writer.WriteElement("genre", genre);
                    }

                    float siteRating =SelectedShow.CachedShow?.SiteRating??0 * 10;

                    int intSiteRating = (int)siteRating;
                    if (intSiteRating > 0)
                    {
                        writer.WriteElement("rating", intSiteRating);
                    }

                    writer.WriteInfo("moviedb", "imdb", "id", SelectedShow.CachedShow?.Imdb);

                    string rt = SelectedShow.CachedShow?.Runtime;
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
            if (Episode != null)
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
                        writer.WriteElement("mpaa", Episode.TheCachedSeries.ContentRating);

                        if (Episode.FirstAired.HasValue)
                        {
                            writer.WriteElement("year", Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                            writer.WriteElement("firstaired",
                                Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                        }

                        writer.WriteElement("runtime", Episode.TheCachedSeries.Runtime, true);
                        writer.WriteElement("rating", Episode.EpisodeRating);
                        writer.WriteElement("studio", Episode.TheCachedSeries.Network);
                        writer.WriteElement("plot", Episode.TheCachedSeries.Overview);
                        writer.WriteElement("overview", Episode.Overview);
                        foreach (string director in Episode.Directors)
                        {
                            writer.WriteElement("directors", director);
                        }

                        foreach (string epwriter in Episode.Writers)
                        {
                            writer.WriteElement("writers", epwriter);
                        }

                        foreach (string genre in Episode.TheCachedSeries.Genres)
                        {
                            writer.WriteElement("genre", genre);
                        }

                        // actors...
                        foreach (Actor aa in Episode.TheCachedSeries.GetActors()
                            .Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
                        {
                            writer.WriteStartElement("actor");
                            writer.WriteElement("name", aa.ActorName);
                            writer.WriteElement("role", aa.ActorRole);
                            writer.WriteEndElement(); // actor
                        }

                        // guest stars...
                        foreach (string guest in Episode.GuestStars)
                        {
                            writer.WriteElement("guest", guest);
                        }

                        writer.WriteElement("thumbnail", TheTVDB.API.GetImageURL(Episode.Filename));
                        writer.WriteElement("banner",
                            TheTVDB.API.GetImageURL(Episode.AppropriateProcessedSeason.GetWideBannerPath()));

                        writer.WriteElement("backdrop",
                            TheTVDB.API.GetImageURL(Episode.TheCachedSeries.GetSeriesFanartPath()));

                        writer.WriteEndElement(); // details
                    }

                    return ActionOutcome.Success();
                }
                catch (Exception e)
                {
                    return new ActionOutcome(e);
                }
            }
            return new ActionOutcome("Write WDTV Metadata called with no Episode provided");
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o) => o is ActionWdtvMeta meta && meta.Where == Where;

        public override int CompareTo(object? o)
        {
            if (!(o is ActionWdtvMeta nfo))
            {
                return -1;
            }

            if (Episode is null && nfo.Episode is null)
            {
                return string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);
            }

            if (Episode is null)
            {
                return 1;
            }

            if (nfo.Episode is null)
            {
                return -1;
            }

            return string.Compare(Where.FullName + Episode.Name, nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
        #endregion
    }
}
