//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace TVRename;

public abstract class ActionNfo : ActionWriteMetadata
{
    #region Action Members

    protected ActionNfo(FileInfo where, ShowConfiguration sI) : base(where, sI)
    {
    }

    protected ActionNfo(FileInfo where, MovieConfiguration mc) : base(where, mc)
    {
    }
    #endregion Action Members

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
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
                return Go(stats, cancellationToken);
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

        using XmlWriter writer = XmlWriter.Create(Where.FullName, settings);
        writer.WriteStartElement(RootName());
        writer.WriteEndElement();
    }

    protected abstract string RootName();

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

    protected static void UpdateRatings(XElement root, string rating, int votes)
    {
        if (!rating.HasValue() ||  rating == "0" || votes == 0)
        {
            return;
        }
        XElement ratingsNode = root.GetOrCreateElement("ratings");

        XElement ratingNode = ratingsNode.GetOrCreateElement("rating", "name", "tvdb");
        ratingNode.UpdateAttribute("name", "tvdb");

        ratingNode.UpdateAttribute("max", "10");
        ratingNode.UpdateAttribute("default", "true");

        ratingNode.UpdateElement("value", rating);
        ratingNode.UpdateElement("votes", votes, true);
    }

    protected static void UpdateId(XElement root, string idType, bool defaultState, string? idValue)
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

        IEnumerable<XElement> xElements = [.. appropriateNodes];
        bool needToUpdate = xElements.Any();

        if (needToUpdate)
        {
            xElements.Single().Value = idValue;
            xElements.Single().UpdateAttribute(NODE_ATTRIBUTE_DEFAULT, defaultState.ToString());
        }
        else
        {
            root.Add(new XElement(NODE_NAME, new XAttribute(NODE_ATTRIBUTE_TYPE, idType), new XAttribute(NODE_ATTRIBUTE_DEFAULT, defaultState), idValue));
        }
    }

    protected static void ReplaceActors(XElement root, IEnumerable<Actor> selectedShowActors)
    {
        IEnumerable<Actor> showActors = selectedShowActors as Actor[] ?? [.. selectedShowActors];
        if (!showActors.ToList().Any())
        {
            return;
        }

        List<XElement> elemsToRemove = [.. root.Elements("actor")];
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

    protected static void ReplaceThumbs(XElement root, string aspectAttributeName, IEnumerable<MediaImage> images)
    {
        {
            List<MediaImage> newImages = [.. images];
            if (!newImages.Any())
            {
                return;
            }

            List<XElement> elemsToRemove = [.. root.Elements("thumb").Where(x => x.Attribute("aspect")?.Value.Equals(aspectAttributeName) ?? false)];

            foreach (XElement oldActor in elemsToRemove)
            {
                oldActor.Remove();
            }

            foreach (MediaImage m in newImages.Where(i => i.ImageUrl.HasValue()))
            {
                XElement tAdd = new("thumb");
                tAdd.Add(new XAttribute("aspect", aspectAttributeName));
                if (m.ThumbnailUrl.HasValue())
                {
                    tAdd.Add(new XAttribute("preview", m.ThumbnailUrl ?? string.Empty));
                }
                tAdd.Value = m.ImageUrl!;
                root.Add(tAdd);
            }
        }
    }

    protected static void ReplaceFanart(XElement root, IEnumerable<MediaImage> images)
    {
        {
            List<MediaImage> newImages = [.. images];
            if (!newImages.Any())
            {
                return;
            }

            List<XElement> elemsToRemove = [.. root.Elements("fanart")];

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

    protected static void UpdateId(XElement root, string idType, bool defaultState, int idValue)
    {
        if (idValue < 0)
        {
            return;
        }
        UpdateId(root, idType, defaultState, idValue.ToString());
    }

    #region Item Members

    public override bool SameAs(Item o)
    {
        return o is ActionNfo nfo && nfo.Where == Where;
    }

    public override int CompareTo(Item? o)
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
