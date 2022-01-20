//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public abstract class ActionNfo : ActionWriteMetadata
    {
        #region Action Members

        protected ActionNfo([NotNull] FileInfo where, [NotNull] ShowConfiguration sI) : base(where, sI)
        {
        }

        protected ActionNfo([NotNull] FileInfo where, [NotNull] MovieConfiguration mc) : base(where, mc)
        {
        }

        public override ActionOutcome Go(TVRenameStats stats)
        {
            try
            {
                if (!Where.Exists)
                {
                    CreateBlankFile();
                }

                ActionOutcome actionOutcome = UpdateFile();
                Where.LastWriteTime = DateTimeOffset.FromUnixTimeSeconds(UpdateTime() ?? 0).UtcDateTime;
                return actionOutcome;
            }
            catch (System.IO.IOException ex)
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
                try
                {
                    Where.Delete(true);
                    return Go(stats);
                }
                catch (System.IO.IOException ex)
                {
                    return new ActionOutcome(ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return new ActionOutcome(ex);
                }
            }
        }

        protected abstract long? UpdateTime();

        private void CreateBlankFile()
        {
            XmlWriterSettings settings = new()
            {
                Indent = true,
                IndentChars = "    ",
                Encoding = Encoding.UTF8,
                NewLineChars = "\r\n",
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(Where.FullName, settings))
            {
                writer.WriteStartElement(RootName());
                writer.WriteEndElement();
            }
        }

        protected abstract string RootName();

        [NotNull]
        protected abstract ActionOutcome UpdateFile();

        protected static void UpdateAmongstElements(XElement e, string elementName, string? value)
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

        protected static void UpdateRatings([NotNull] XElement root, string rating, int votes)
        {
            XElement ratingsNode = root.GetOrCreateElement("ratings");

            XElement ratingNode = ratingsNode.GetOrCreateElement("rating", "name", "tvdb");
            ratingNode.UpdateAttribute("name", "tvdb");

            ratingNode.UpdateAttribute("max", "10");
            ratingNode.UpdateAttribute("default", "true");

            ratingNode.UpdateElement("value", rating);
            ratingNode.UpdateElement("votes", votes, true);
        }

        protected static void UpdateId([NotNull] XElement root, string idType, [NotNull] string defaultState, string? idValue)
        {
            if (idValue is null)
            {
                return;
            }
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
                xElements.Single().UpdateAttribute(NODE_ATTRIBUTE_DEFAULT, defaultState);
            }
            else
            {
                root.Add(new XElement(NODE_NAME, new XAttribute(NODE_ATTRIBUTE_TYPE, idType), new XAttribute(NODE_ATTRIBUTE_DEFAULT, defaultState), idValue));
            }
        }

        protected static void ReplaceActors([NotNull] XElement root, [NotNull] IEnumerable<Actor> selectedShowActors)
        {
            IEnumerable<Actor> showActors = selectedShowActors as Actor[] ?? selectedShowActors.ToArray();
            if (!showActors.ToList().Any())
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
                XElement tAdd = new("actor", new XElement("name", aa.ActorName));

                if (!string.IsNullOrWhiteSpace(aa.ActorRole))
                {
                    tAdd.Add(new XElement("role", aa.ActorRole));
                }

                if (aa.ActorSortOrder >= 0)
                {
                    tAdd.Add(new XElement("order", aa.ActorSortOrder));
                }

                if (!string.IsNullOrWhiteSpace(aa.ActorImage))
                {
                    tAdd.Add(new XElement("thumb", aa.ActorImage));
                }

                root.Add(tAdd);
            }
        }

        protected static void ReplaceThumbs(XElement root, string aspectAttributeName, [NotNull] IEnumerable<MediaImage> images)
        {
            {
                List<MediaImage> newImages = images.ToList();
                if (!newImages.Any())
                {
                    return;
                }

                List<XElement> elemsToRemove = root.Elements("thumb")
                    .Where(x => x.Attribute("aspect")?.ToString().Equals(aspectAttributeName) ?? false).ToList();

                foreach (XElement oldActor in elemsToRemove)
                {
                    oldActor.Remove();
                }

                foreach (MediaImage m in newImages.Where(i => i.ImageUrl.HasValue()))
                {
                    XElement tAdd = new("thumb");
                    tAdd.Add(new XAttribute("aspect", aspectAttributeName));
                    tAdd.Add(new XAttribute("preview", m.ThumbnailUrl ?? string.Empty));
                    tAdd.Value = m.ImageUrl!;
                    root.Add(tAdd);
                }
            }
        }

        protected static void ReplaceFanart(XElement root, [NotNull] IEnumerable<MediaImage> images)
        {
            {
                List<MediaImage> newImages = images.ToList();
                if (!newImages.Any())
                {
                    return;
                }

                List<XElement> elemsToRemove = root.Elements("fanart").ToList();

                foreach (XElement oldActor in elemsToRemove)
                {
                    oldActor.Remove();
                }

                XElement fanartElement = new("fanart");
                root.Add(fanartElement);

                // actors...
                foreach (MediaImage m in newImages.Where(i => i.ImageUrl.HasValue()))
                {
                    XElement tAdd = new("thumb");
                    tAdd.Add(new XAttribute("preview", m.ThumbnailUrl ?? string.Empty));
                    tAdd.Value = m.ImageUrl!;
                    fanartElement.Add(tAdd);
                }
            }
        }

        protected static void UpdateId([NotNull] XElement root, [NotNull] string idType, [NotNull] string defaultState, int idValue)
        {
            UpdateId(root, idType, defaultState, idValue.ToString());
        }

        #endregion Action Members

        #region Item Members

        public override bool SameAs(Item o)
        {
            return o is ActionNfo nfo && nfo.Where == Where;
        }

        public override int CompareTo(Item o)
        {
            if (o is not ActionNfo nfo)
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

        #endregion Item Members
    }
}
