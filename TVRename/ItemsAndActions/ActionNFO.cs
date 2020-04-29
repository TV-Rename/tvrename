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
using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using static Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    public class ActionNfo : ActionWriteMetadata
    {
        public ActionNfo(FileInfo nfo, [NotNull] ProcessedEpisode pe) : base(nfo, pe.Show)
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

        [NotNull]
        public override ActionOutcome Go(TVRenameStats stats)
        {
            try
            {
                if (!Where.Exists)
                {
                    CreateBlankFile();
                }

                return UpdateFile();
            }
            catch (IOException ex)
            {
                return new ActionOutcome(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ActionOutcome(ex);
            }
            catch (XmlException)
            {
                //Assume that the file needs to be recreated
                CreateBlankFile();
                return Go(stats);
            }
        }

        private void CreateBlankFile()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                Encoding = Encoding.UTF8,
                NewLineChars = "\r\n",
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(Where.FullName, settings))
            {
                if (Episode != null) // specific episode
                {
                    writer.WriteStartElement("episodedetails");
                    writer.WriteEndElement(); // episodedetails
                }
                else if (SelectedShow != null) // show overview (tvshow.nfo)
                {
                    writer.WriteStartElement("tvshow");
                    writer.WriteEndElement(); // tvshow
                }
            }
        }

        [NotNull]
        private ActionOutcome UpdateFile()
        {
            //We will replace the file as too difficult to update multiparts
            //We can't use XDocument as it's not fully valid XML
            if (Episode != null && Episode.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
            {
                return ReplaceMultipartFile();
            }

            XDocument doc = XDocument.Load(Where.FullName);
            XElement root = doc.Root;
            if(root is null){
                return new ActionOutcome($"Could not load {Where.FullName}");
            }

            if (Episode != null) // specific episode
            {
                ShowItem si = Episode.Show ?? SelectedShow;
                UpdateEpisodeFields(Episode, si, root, false);
            }
            else if (SelectedShow != null) // show overview (tvshow.nfo)
            {
                UpdateShowFields(root);
            }
            
            doc.Save(Where.FullName);
            return ActionOutcome.Success();
        }

        [NotNull]
        private ActionOutcome ReplaceMultipartFile()
        {
            ShowItem si = Episode.Show ?? SelectedShow;

            //We will replace the file as too difficult to update multiparts
            //We can't use XDocument as it's not fully valid XML
            List<XElement> episodeXmLs = new List<XElement>();

            foreach (Episode ep in Episode.SourceEpisodes)
            {
                XElement epNode = new XElement("episodedetails");
                UpdateEpisodeFields(ep, si, epNode, true);
                episodeXmLs.Add(epNode);
            }

            try
            {
                using (StreamWriter writer = CreateText(Where.FullName))
                {
                    foreach (XElement ep in episodeXmLs)
                    {
                        writer.WriteLine(ep);
                    }
                }
            }
            catch (IOException e)
            {
                return new ActionOutcome(e);
            }

            return ActionOutcome.Success();
        }

        private void UpdateEpisodeFields([NotNull] Episode episode,[CanBeNull] ShowItem show, [NotNull] XElement root, bool isMultiPart)
        {
            root.UpdateElement("title", episode.Name,true);
            root.UpdateElement("id", episode.EpisodeId, true);
            root.UpdateElement("plot", episode.Overview, true);
            UpdateAmongstElements(root, "studio", episode.TheSeries?.Network);

            UpdateId(root, "tvdb", "true", episode.EpisodeId);
            UpdateId(root, "imdb", "false", episode.ImdbCode);

            string showRating = episode.EpisodeRating;
            if (showRating != null)
            {
                UpdateRatings(root, showRating, episode.SiteRatingCount ?? 0);
            }

            if (!(show is null))
            {
                root.UpdateElement("originaltitle", show.ShowName, true);
                root.UpdateElement("showtitle", show.ShowName, true);
                root.UpdateElement("season", episode.GetSeasonNumber(show.Order), true);
                root.UpdateElement("episode", episode.GetEpisodeNumber(show.Order), true);
                root.UpdateElement("mpaa", show.TheSeries()?.ContentRating, true);

                //actor(s) and guest actor(s)
                SeriesInfo s = show.TheSeries();
                if (s != null)
                {
                    ReplaceActors(root, episode.AllActors(s));
                }
            }

            if (episode.FirstAired.HasValue)
            {
                root.UpdateElement("aired", episode.FirstAired.Value.ToString("yyyy-MM-dd"), true);
            }

            //Director(s)
            string epDirector = episode.EpisodeDirector;
            if (!string.IsNullOrEmpty(epDirector))
            {
                string[] dirs = epDirector.Split('|');
                if (dirs.Any())
                {
                    root.ReplaceElements("director",dirs);
                }
            }

            //Writers(s)
            string epWriter = episode.Writer;
            if (!string.IsNullOrEmpty(epWriter))
            {
                string[] writers = epWriter.Split('|');
                if (writers.Any())
                {
                    root.ReplaceElements("credits", writers);
                }
            }

            if (isMultiPart && show!=null)
            {
                XElement resumeElement = root.GetOrCreateElement("resume");

                //we have to put 0 as we don't know where the multipart episode starts/ends
                resumeElement.UpdateElement("position", 0);
                resumeElement.UpdateElement("total", 0);

                //For now we only put art in for multipart episodes. Kodi finds the art appropriately
                //without our help for the others

                string filename = TVSettings.Instance.FilenameFriendly(show, episode);

                string thumbFilename = filename + ".jpg";
                UpdateAmongstElements(root,"thumb", thumbFilename);
                //Should be able to do this using the local filename, but only seems to work if you provide a URL
                //XMLHelper.WriteElementToXML(writer, "thumb", LocalCache.Instance.GetTVDBDownloadURL(episode.GetFilename()))
            }
        }

        private void UpdateAmongstElements(XElement e, string elementName, string value)
        {
            if (!value.HasValue())
            {
                return;
            }

            if (!e.Elements(elementName).Any())
            {
                e.Add(new XElement(elementName, value));
                return;
            }

            if (e.Elements(elementName).Any(element => element.Value == value))
            {
                //Element with this value exists
                return;
            }

            e.Add(new XElement(elementName, value));
        }

        private void UpdateShowFields([NotNull] XElement root)
        {
            SeriesInfo series = SelectedShow.TheSeries();
            root.UpdateElement("title", SelectedShow.ShowName);

            float? showRating = series?.SiteRating;
            if (showRating.HasValue)
            {
                UpdateRatings(root,showRating.Value.ToString(CultureInfo.InvariantCulture), series.SiteRatingVotes);
            }

            string lang = TVSettings.Instance.PreferredLanguageCode;
            if (SelectedShow.UseCustomLanguage && SelectedShow.PreferredLanguage != null)
            {
                lang = SelectedShow.PreferredLanguage.Abbreviation;
            }

            //https://forum.kodi.tv/showthread.php?tid=323588
            //says that we need a format like this:
            //<episodeguide><url post="yes" cache="auth.json">https://api.thetvdb.com/login?{&quot;apikey&quot;:&quot;((API-KEY))&quot;,&quot;id&quot;:((ID))}|Content-Type=application/json</url></episodeguide>

            XElement episodeGuideNode = root.GetOrCreateElement("episodeguide");
            XElement urlNode = episodeGuideNode.GetOrCreateElement("url");
            urlNode.UpdateAttribute("post", "yes");
            urlNode.UpdateAttribute("cache", "auth.json");
            urlNode.SetValue(TheTVDB.API.BuildUrl(SelectedShow.TvdbCode, lang));

            if (!(series is null))
            {
                root.UpdateElement("originaltitle", series.Name);
                root.UpdateElement("studio", series.Network);
                root.UpdateElement("id", series.TvdbCode);
                root.UpdateElement("runtime", series.Runtime, true);
                root.UpdateElement("mpaa", series.ContentRating, true);
                root.UpdateElement("premiered", series.FirstAired);
                root.UpdateElement("year", series.Year);
                root.UpdateElement("status", series.Status);
                root.UpdateElement("plot", series.Overview);

                UpdateId(root, "tvdb", "true", series.TvdbCode);
                UpdateId(root, "imdb", "false", series.Imdb);
            }

            root.ReplaceElements("genre", SelectedShow.Genres);

            ReplaceActors(root, SelectedShow.Actors);
        }

        private static void UpdateRatings([NotNull] XElement root, string rating, int votes)
        {
            XElement ratingsNode = root.GetOrCreateElement("ratings");

            XElement ratingNode = ratingsNode.GetOrCreateElement("rating","name","tvdb");
            ratingNode.UpdateAttribute("name", "tvdb");

            ratingNode.UpdateAttribute("max", "10");
            ratingNode.UpdateAttribute("default", "true");

            ratingNode.UpdateElement("value", rating);
            ratingNode.UpdateElement("votes", votes, true);
        }

        private static void UpdateId([NotNull] XElement root, string idType, [NotNull] string defaultState, string idValue)
        {
            const string NODE_NAME = "uniqueid";
            const string NODE_ATTRIBUTE_TYPE = "type";
            const string NODE_ATTRIBUTE_DEFAULT = "default";
            IEnumerable<XElement> appropriateNodes = root.Elements()
                .Where(node => node.Name == NODE_NAME && node.HasAttribute(NODE_ATTRIBUTE_TYPE, idType));

            IEnumerable<XElement> xElements = appropriateNodes.ToList();
            bool needToUpdate = xElements.Any();

            if (needToUpdate)
            {
                xElements.Single().Value = idValue;
                xElements.Single().UpdateAttribute(NODE_ATTRIBUTE_DEFAULT,defaultState);
            }
            else
            {
                root.Add(new XElement(NODE_NAME,new XAttribute(NODE_ATTRIBUTE_TYPE, idType), new XAttribute(NODE_ATTRIBUTE_DEFAULT, defaultState),idValue));
            }
        }

        private static void ReplaceActors([NotNull] XElement root, [NotNull] IEnumerable<Actor> selectedShowActors)
        {
            IEnumerable<Actor> showActors = selectedShowActors as Actor[] ?? selectedShowActors.ToArray();
            if (! showActors.ToList().Any())
            {
                return;
            }

            List<XElement> elemsToRemove = root.Elements("actor").ToList();
            foreach (XElement oldActor in elemsToRemove)
            {
                oldActor.Remove();
            }
            
            // actors...
            foreach (Actor aa in showActors.Where(aa => !string.IsNullOrEmpty(aa.ActorName)))
            {
                XElement tAdd = new XElement("actor", new XElement("name", aa.ActorName));

                if (!string.IsNullOrWhiteSpace(aa.ActorRole))
                {
                    tAdd.Add(new XElement("role", aa.ActorRole));
                }

                if (aa.ActorSortOrder>0)
                {
                    tAdd.Add(new XElement("order", aa.ActorSortOrder));
                }

                if (!string.IsNullOrWhiteSpace(aa.ActorImage))
                {
                    tAdd.Add(new XElement("thumb", TheTVDB.API.GetImageURL(aa.ActorImage)));
                }

                root.Add(tAdd);
            }
        }

        private static void UpdateId([NotNull] XElement root, [NotNull] string idType, [NotNull] string defaultState, int idValue)
        {
            UpdateId(root,idType,defaultState,idValue.ToString());
        }

        #endregion

        #region Item Members
        public override bool SameAs(Item o)
        {
            return o is ActionNfo nfo && nfo.Where == Where;
        }

        public override int CompareTo(object o)
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

            return string.Compare(Where.FullName + Episode.Name, nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }
        #endregion
    }
}
