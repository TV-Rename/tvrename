// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Globalization;

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

        public override string Name => "Write WD TV Live Hub Meta";

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            if (Where == null)
            {
                ErrorText = "No file location specified - Development Error";
                Error = true;
                Done = true;
                return false;
            }

            if (Episode != null) return WriteEpisodeMetaDataFile();
            
            if (SelectedShow != null) return WriteSeriesXml();

            ErrorText = "No details available to write - Development Error";
            Error = true;
            Done = true;
            return false;
        }

        private bool WriteSeriesXml()
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

                    XmlHelper.WriteElementToXml(writer, "title", SelectedShow.ShowName);

                    foreach (string genre in SelectedShow.TheSeries().Genres())
                    {
                        XmlHelper.WriteElementToXml(writer, "genre", genre);
                    }

                    XmlHelper.WriteElementToXml(writer, "premiered", SelectedShow.TheSeries().FirstAired);
                    XmlHelper.WriteElementToXml(writer, "year", SelectedShow.TheSeries().Year);

                    float siteRating =SelectedShow.TheSeries().SiteRating * 10;

                    int intSiteRating = (int)siteRating;
                    if (intSiteRating > 0) XmlHelper.WriteElementToXml(writer, "rating", intSiteRating);

                    XmlHelper.WriteElementToXml(writer, "status", SelectedShow.TheSeries().Status);

                    XmlHelper.WriteElementToXml(writer, "mpaa", SelectedShow.TheSeries().ContentRating);
                    XmlHelper.WriteInfo(writer, "moviedb", "imdb", "id", SelectedShow.TheSeries().Imdb);
                    XmlHelper.WriteElementToXml(writer, "tvdbid", SelectedShow.TheSeries().TvdbCode);

                    string rt = SelectedShow.TheSeries().Runtime;
                    if (!string.IsNullOrEmpty(rt))
                    {
                        XmlHelper.WriteElementToXml(writer, "runtime", rt + " min");
                    }

                    XmlHelper.WriteElementToXml(writer, "plot", SelectedShow.TheSeries().Overview);

                    writer.WriteEndElement(); // show
                    writer.WriteEndElement(); // tvshow
                }
                Done = true;
                return true;
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                Error = true;
                Done = true;
                return false;
            }
        }

        private bool WriteEpisodeMetaDataFile()
        {
            // "try" and silently fail.  eg. when file is use by other...
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true,
                };

                using (XmlWriter writer = XmlWriter.Create(Where.FullName, settings))
                {
                    writer.WriteStartElement("details");
                    XmlHelper.WriteElementToXml(writer, "title", TVSettings.Instance.NamingStyle.NameFor(Episode));
                    XmlHelper.WriteElementToXml(writer, "mpaa", Episode.TheSeries.ContentRating);

                    if (Episode.FirstAired.HasValue)
                    {
                        XmlHelper.WriteElementToXml(writer, "year", Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                        XmlHelper.WriteElementToXml(writer, "firstaired",
                            Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                    }

                    XmlHelper.WriteElementToXml(writer, "runtime", Episode.TheSeries.Runtime, true);
                    XmlHelper.WriteElementToXml(writer, "rating", Episode.EpisodeRating);
                    XmlHelper.WriteElementToXml(writer, "studio", Episode.TheSeries.Network);
                    XmlHelper.WriteElementToXml(writer, "plot", Episode.TheSeries.Overview);
                    XmlHelper.WriteElementToXml(writer, "overview", Episode.Overview);
                    foreach (string director in Episode.Directors)
                    {
                        XmlHelper.WriteElementToXml(writer, "directors", director);
                    }

                    foreach(string epwriter in Episode.Writers)
                    {
                        XmlHelper.WriteElementToXml(writer, "writers", epwriter);
                    }

                    foreach (string genre in Episode.TheSeries.Genres())
                    {
                        XmlHelper.WriteElementToXml(writer, "genre", genre);
                    }

                    // actors...
                    foreach (Actor aa in Episode.TheSeries.GetActors())
                    {
                        if (string.IsNullOrEmpty(aa.ActorName)) continue;

                        writer.WriteStartElement("actor");
                        XmlHelper.WriteElementToXml(writer, "name", aa.ActorName);
                        XmlHelper.WriteElementToXml(writer, "role", aa.ActorRole);
                        writer.WriteEndElement(); // actor
                    }

                    // guest stars...
                    foreach(string guest in Episode.GuestStars)
                    {
                        XmlHelper.WriteElementToXml(writer, "guest", guest);
                    }

                    XmlHelper.WriteElementToXml(writer, "thumbnail", TheTVDB.GetImageURL(Episode.Filename));
                    XmlHelper.WriteElementToXml(writer, "banner", TheTVDB.GetImageURL(Episode.AppropriateSeason.GetWideBannerPath()));
                    XmlHelper.WriteElementToXml(writer, "backdrop", TheTVDB.GetImageURL(Episode.TheSeries.GetSeriesFanartPath()));
                    writer.WriteEndElement(); // details
                }
                Done = true;
                return true;
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                Error = true;
                Done = true;
                return false;
            }
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o) => (o is ActionWdtvMeta meta) && (meta.Where == Where);

        public override int Compare(Item o)
        {
            ActionWdtvMeta nfo = o as ActionWdtvMeta;

            if (Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            return string.Compare((Where.FullName + Episode.Name), nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
        #endregion
    }
}
