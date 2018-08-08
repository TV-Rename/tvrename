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
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
    using System.Xml;

    public class ActionWdtvMeta : ActionWriteMetadata
    {
        public ActionWdtvMeta(FileInfo where, ProcessedEpisode pe) :base(where,null)
        {
            Episode = pe;
        }

        #region Action Members

        public override string Name => "Write WD TV Live Hub Meta";

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            if (Episode == null) return false;
            if (Where == null) return false;

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
                    XmlHelper.WriteElementToXml(writer, "mpaa", Episode.TheSeries.GetContentRating());

                    if (Episode.FirstAired.HasValue)
                    {
                        XmlHelper.WriteElementToXml(writer, "year", Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                        XmlHelper.WriteElementToXml(writer, "firstaired",
                            Episode.FirstAired.Value.ToString("yyyy-MM-dd"));
                    }

                    string rt = Episode.TheSeries.GetRuntime();
                    if (!string.IsNullOrEmpty(rt))
                    {
                        XmlHelper.WriteElementToXml(writer, "runtime", rt);
                    }

                    XmlHelper.WriteElementToXml(writer, "rating", Episode.EpisodeRating);
                    XmlHelper.WriteElementToXml(writer, "studio", Episode.TheSeries.GetNetwork());
                    XmlHelper.WriteElementToXml(writer, "plot", Episode.TheSeries.GetOverview());
                    XmlHelper.WriteElementToXml(writer, "overview", Episode.Overview);
                    XmlHelper.WriteElementToXml(writer, "directors", string.Join(",", Episode.Directors));
                    XmlHelper.WriteElementToXml(writer, "writers", string.Join(",", Episode.Writers));

                    foreach (string genre in Episode.TheSeries.GetGenres())
                    {
                        XmlHelper.WriteElementToXml(writer, "genre", genre);
                    }

                    // actors...
                    foreach (Actor aa in Episode.TheSeries.GetActors())
                    {
                        if (string.IsNullOrEmpty(aa.ActorName))continue;

                        writer.WriteStartElement("actor");
                        XmlHelper.WriteElementToXml(writer, "name", aa.ActorName);
                        XmlHelper.WriteElementToXml(writer, "role", aa.ActorRole); 
                        writer.WriteEndElement(); // actor
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
